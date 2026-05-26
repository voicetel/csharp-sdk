using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTel.Sdk;

/// <summary>
/// Low-level HTTP transport used by every resource service. Hand-written, no
/// codegen. Handles authentication, JSON envelope unwrapping, error mapping,
/// and 429/5xx retry with exponential backoff and <c>Retry-After</c> support.
/// </summary>
internal sealed class Transport
{
    private static readonly HashSet<int> RetryableStatuses = new()
    {
        429, 500, 502, 503, 504,
    };

    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly string _baseUrl;
    private readonly string _userAgent;
    private readonly int _maxRetries;
    private string? _apiKey;

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public Transport(
        HttpClient? httpClient,
        bool ownsHttpClient,
        string baseUrl,
        string? apiKey,
        string userAgent,
        int maxRetries)
    {
        _httpClient = httpClient ?? new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        })
        { Timeout = TimeSpan.FromSeconds(30) };
        _ownsHttpClient = ownsHttpClient || httpClient is null;
        _baseUrl = (baseUrl ?? SdkInfo.DefaultBaseUrl).TrimEnd('/');
        _userAgent = userAgent;
        _maxRetries = maxRetries < 0 ? 0 : maxRetries;
        _apiKey = apiKey;
    }

    public string BaseUrl => _baseUrl;

    public string? ApiKey => _apiKey;

    public void SetBearer(string apiKey) => _apiKey = apiKey;

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    /// <summary>
    /// Issue an HTTP request and decode the JSON response.
    /// </summary>
    /// <typeparam name="T">Response data type after envelope unwrapping.</typeparam>
    /// <param name="method">HTTP method.</param>
    /// <param name="path">Request path beginning with <c>/v2.2/</c>.</param>
    /// <param name="query">URL-encoded query string or null.</param>
    /// <param name="body">Request body, will be JSON serialized.</param>
    /// <param name="requireAuth">When true, sends the bearer token (or throws if not set).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T> RequestAsync<T>(
        HttpMethod method,
        string path,
        string? query,
        object? body,
        bool requireAuth,
        CancellationToken cancellationToken)
    {
        var raw = await SendAsync(method, path, query, body, requireAuth, cancellationToken).ConfigureAwait(false);
        if (raw.Length == 0)
        {
            return default!;
        }

        var inner = Unwrap(raw);
        try
        {
            var value = JsonSerializer.Deserialize<T>(inner, JsonOptions);
            return value!;
        }
        catch (JsonException ex)
        {
            throw new ApiError(
                ErrorKind.Unknown,
                200,
                "decode response body",
                code: null,
                body: Encoding.UTF8.GetString(inner),
                innerException: ex);
        }
    }

    /// <summary>
    /// Issue an HTTP request and discard the response body. Used for DELETE
    /// endpoints that return 204 No Content.
    /// </summary>
    public async Task RequestAsync(
        HttpMethod method,
        string path,
        string? query,
        object? body,
        bool requireAuth,
        CancellationToken cancellationToken)
    {
        await SendAsync(method, path, query, body, requireAuth, cancellationToken).ConfigureAwait(false);
    }

    private async Task<byte[]> SendAsync(
        HttpMethod method,
        string path,
        string? query,
        object? body,
        bool requireAuth,
        CancellationToken cancellationToken)
    {
        if (requireAuth && string.IsNullOrEmpty(_apiKey))
        {
            throw new ApiError(
                ErrorKind.Authentication,
                0,
                "no api key set; call client.LoginAsync or pass apiKey to VoiceTelClient");
        }

        var target = _baseUrl + path;
        if (!string.IsNullOrEmpty(query))
        {
            target += "?" + query;
        }

        byte[]? bodyBytes = null;
        if (body is not null)
        {
            try
            {
                bodyBytes = JsonSerializer.SerializeToUtf8Bytes(body, body.GetType(), JsonOptions);
            }
            catch (Exception ex)
            {
                throw new ApiError(ErrorKind.Unknown, 0, "marshal request body", innerException: ex);
            }
        }

        string? idempotencyKey = (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
            ? Guid.NewGuid().ToString()
            : null;

        Exception? lastError = null;
        for (var attempt = 0; attempt <= _maxRetries; attempt++)
        {
            using var request = new HttpRequestMessage(method, target);
            request.Headers.UserAgent.Clear();
            request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (requireAuth && !string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }
            if (idempotencyKey is not null)
            {
                request.Headers.TryAddWithoutValidation("Idempotency-Key", idempotencyKey);
            }
            if (bodyBytes is not null)
            {
                request.Content = new ByteArrayContent(bodyBytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastError = ex;
                if (attempt >= _maxRetries)
                {
                    throw new ApiError(
                        ErrorKind.Unknown,
                        0,
                        $"transport error after {attempt + 1} attempt(s): {ex.Message}",
                        innerException: ex);
                }
                await DelayAsync(BackoffDelay(attempt, null), cancellationToken).ConfigureAwait(false);
                continue;
            }

            try
            {
                var statusCode = (int)response.StatusCode;
                if (RetryableStatuses.Contains(statusCode) && attempt < _maxRetries)
                {
                    var delay = BackoffDelay(attempt, response);
                    response.Dispose();
                    await DelayAsync(delay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                return await DecodeAsync(response, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                response.Dispose();
            }
        }

        throw new ApiError(
            ErrorKind.Unknown,
            0,
            "retry loop exhausted",
            innerException: lastError);
    }

    private static async Task<byte[]> DecodeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        byte[] raw;
        if (response.Content is null)
        {
            raw = Array.Empty<byte>();
        }
        else
        {
#if NET8_0_OR_GREATER
            raw = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
#else
            cancellationToken.ThrowIfCancellationRequested();
            raw = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
#endif
        }

        var statusCode = (int)response.StatusCode;
        if (statusCode >= 200 && statusCode < 300)
        {
            return raw;
        }

        // Error path — try to extract a structured code/message.
        object? body = raw.Length == 0 ? null : Encoding.UTF8.GetString(raw);
        string? code = null;
        string message = $"HTTP {statusCode}";

        if (raw.Length > 0)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    body = JsonElementToObject(doc.RootElement);
                    if (doc.RootElement.TryGetProperty("code", out var codeEl) && codeEl.ValueKind == JsonValueKind.String)
                    {
                        code = codeEl.GetString();
                    }
                    else if (doc.RootElement.TryGetProperty("error", out var errEl) && errEl.ValueKind == JsonValueKind.String)
                    {
                        code = errEl.GetString();
                    }
                    if (doc.RootElement.TryGetProperty("message", out var msgEl) && msgEl.ValueKind == JsonValueKind.String)
                    {
                        message = msgEl.GetString() ?? message;
                    }
                    else if (doc.RootElement.TryGetProperty("error", out var errMsg) && errMsg.ValueKind == JsonValueKind.String)
                    {
                        message = errMsg.GetString() ?? message;
                    }
                }
            }
            catch (JsonException)
            {
                // Non-JSON body — keep it as a string.
            }
        }

        throw ApiError.FromStatus(statusCode, code, message, body);
    }

    private static object? JsonElementToObject(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    obj[prop.Name] = JsonElementToObject(prop.Value);
                }
                return obj;
            case JsonValueKind.Array:
                var arr = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    arr.Add(JsonElementToObject(item));
                }
                return arr;
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var l))
                {
                    return l;
                }
                return element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                return null;
        }
    }

    /// <summary>
    /// Strips the <c>{"status":"success","data":...}</c> envelope when present.
    /// Returns the inner <c>data</c> payload as raw UTF-8 JSON bytes; if no
    /// envelope, returns the input unchanged.
    /// </summary>
    private static byte[] Unwrap(byte[] raw)
    {
        if (raw.Length == 0)
        {
            return raw;
        }
        // Cheap path: only attempt unwrapping if it looks like a JSON object.
        var i = 0;
        while (i < raw.Length && (raw[i] == (byte)' ' || raw[i] == (byte)'\t' || raw[i] == (byte)'\n' || raw[i] == (byte)'\r'))
        {
            i++;
        }
        if (i >= raw.Length || raw[i] != (byte)'{')
        {
            return raw;
        }
        try
        {
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                return raw;
            }
            var hasStatus = doc.RootElement.TryGetProperty("status", out _);
            if (hasStatus && doc.RootElement.TryGetProperty("data", out var data))
            {
                return Encoding.UTF8.GetBytes(data.GetRawText());
            }
            return raw;
        }
        catch (JsonException)
        {
            return raw;
        }
    }

    private static TimeSpan BackoffDelay(int attempt, HttpResponseMessage? response)
    {
        if (response is not null)
        {
            var retryAfter = response.Headers.RetryAfter;
            if (retryAfter is not null)
            {
                if (retryAfter.Delta is { } delta && delta >= TimeSpan.Zero)
                {
                    return delta;
                }
                if (retryAfter.Date is { } date)
                {
                    var d = date - DateTimeOffset.UtcNow;
                    if (d > TimeSpan.Zero)
                    {
                        return d;
                    }
                }
            }
        }
        // Exponential, capped at 8s.
        var ms = 500 * (1 << attempt);
        if (ms > 8000)
        {
            ms = 8000;
        }
        return TimeSpan.FromMilliseconds(ms);
    }

    private static async Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        if (delay <= TimeSpan.Zero)
        {
            return;
        }
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
    }
}
