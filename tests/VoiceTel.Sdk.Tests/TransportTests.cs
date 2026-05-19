using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace VoiceTel.Sdk.Tests;

public class TransportTests
{
    [Fact]
    public async Task SuccessfulRequest_StripsEnvelopeAndDeserializes()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account",
            "{\"username\":\"1234\",\"name\":\"Test User\",\"cash\":42.5}");

        var data = await client.Account.GetAsync();

        Assert.Equal("Test User", data.Name);
        Assert.Equal(42.5, data.Cash);
        Assert.Single(handler.Captured);
        Assert.Equal("Bearer deadbeefdeadbeefdeadbeefdeadbeef",
            handler.Captured[0].Request.Headers.Authorization!.ToString());
    }

    [Fact]
    public async Task UnwrappedResponse_StillDeserializes()
    {
        // The transport should fall through gracefully when no envelope is present.
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account", "{\"name\":\"Raw\"}");
        var data = await client.Account.GetAsync();
        Assert.Equal("Raw", data.Name);
    }

    [Theory]
    [InlineData(400, ErrorKind.BadRequest)]
    [InlineData(401, ErrorKind.Authentication)]
    [InlineData(403, ErrorKind.PermissionDenied)]
    [InlineData(404, ErrorKind.NotFound)]
    [InlineData(409, ErrorKind.Conflict)]
    [InlineData(429, ErrorKind.RateLimit)]
    [InlineData(500, ErrorKind.Server)]
    [InlineData(502, ErrorKind.Server)]
    [InlineData(418, ErrorKind.Unknown)]
    public async Task ErrorStatuses_MapToKind(int status, ErrorKind expected)
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account",
            "{\"code\":\"X\",\"message\":\"boom\"}",
            (HttpStatusCode)status);

        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal(expected, ex.Kind);
        Assert.Equal(status, ex.StatusCode);
        Assert.Equal("X", ex.Code);
        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public async Task ErrorWithoutStructuredBody_KeepsStringBody()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account",
            "not really json",
            HttpStatusCode.BadGateway);

        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal(ErrorKind.Server, ex.Kind);
        Assert.Equal("not really json", ex.Body as string);
    }

    [Fact]
    public async Task ErrorBodyWithErrorField_PopulatesCodeAndMessage()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account",
            "{\"error\":\"NopeError\"}",
            HttpStatusCode.BadRequest);
        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal("NopeError", ex.Code);
        Assert.Equal("NopeError", ex.Message);
    }

    [Fact]
    public async Task RetriesOn429_ThenSucceeds()
    {
        var (client, handler) = TestFactory.NewClient(maxRetries: 3);
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account",
            "{}",
            HttpStatusCode.TooManyRequests);
        handler.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account", "{\"name\":\"OK\"}");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var data = await client.Account.GetAsync();
        sw.Stop();

        Assert.Equal("OK", data.Name);
        Assert.Equal(2, handler.Captured.Count);
        Assert.True(sw.ElapsedMilliseconds >= 400, "should have slept the backoff window");
    }

    [Fact]
    public async Task RetriesOn5xx_ThenGivesUp()
    {
        var (client, handler) = TestFactory.NewClient(maxRetries: 1);
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account", "{\"error\":\"x\"}", HttpStatusCode.InternalServerError);
        handler.EnqueueJson(HttpMethod.Get, "/v2.2/account", "{\"error\":\"x\"}", HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal(ErrorKind.Server, ex.Kind);
        Assert.Equal(2, handler.Captured.Count);
    }

    [Fact]
    public async Task Retry_HonorsRetryAfter()
    {
        var (client, handler) = TestFactory.NewClient(maxRetries: 1);
        handler.Enqueue(new MockExchange
        {
            Method = HttpMethod.Get,
            Path = "/v2.2/account",
            Status = HttpStatusCode.ServiceUnavailable,
            ResponseBody = "{}",
            ResponseHeaders = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Retry-After"] = "1",
            },
        });
        handler.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account", "{\"name\":\"OK\"}");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var data = await client.Account.GetAsync();
        sw.Stop();

        Assert.Equal("OK", data.Name);
        Assert.True(sw.ElapsedMilliseconds >= 900, $"expected ~1s sleep, got {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task NoApiKey_ThrowsAuthentication()
    {
        var handler = new MockHttpHandler();
        var http = new HttpClient(handler);
        using var client = new VoiceTelClient(apiKey: null, baseUrl: "https://x", httpClient: http);
        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal(ErrorKind.Authentication, ex.Kind);
    }

    [Fact]
    public async Task RequestBody_IsJsonSerialized()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueEnvelope(HttpMethod.Put, "/v2.2/account", "{\"updated\":[\"timezone\"]}");
        await client.Account.UpdateAsync(new Models.AccountPutRequest
        {
            Timezone = "America/New_York",
            Notify = true,
        });

        var body = handler.Captured[0].Body;
        Assert.Contains("\"timezone\":\"America/New_York\"", body);
        Assert.Contains("\"notify\":true", body);
        Assert.DoesNotContain("\"sms\"", body); // omitted null
    }

    [Fact]
    public async Task LoginAsync_StoresApiKeyAndDoesNotRequireAuth()
    {
        var handler = new MockHttpHandler();
        var http = new HttpClient(handler);
        using var client = new VoiceTelClient(apiKey: null, baseUrl: "https://x", httpClient: http);

        handler.EnqueueEnvelope(HttpMethod.Post, "/v2.2/account/api-key",
            "{\"apikey\":\"new-key\"}");

        var key = await client.LoginAsync(1000000001, "hunter2");

        Assert.Equal("new-key", key);
        Assert.Equal("new-key", client.ApiKey);
        Assert.Null(handler.Captured[0].Request.Headers.Authorization);
    }

    [Fact]
    public async Task LoginAsync_MissingApiKey_Throws()
    {
        var handler = new MockHttpHandler();
        var http = new HttpClient(handler);
        using var client = new VoiceTelClient(apiKey: null, baseUrl: "https://x", httpClient: http);
        handler.EnqueueEnvelope(HttpMethod.Post, "/v2.2/account/api-key", "{}");
        var ex = await Assert.ThrowsAsync<ApiError>(() => client.LoginAsync(1, "p"));
        Assert.Equal(ErrorKind.Authentication, ex.Kind);
    }

    [Fact]
    public async Task CancellationToken_IsHonored()
    {
        var (client, handler) = TestFactory.NewClient();
        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.Account.GetAsync(cts.Token));
    }

    [Fact]
    public async Task QueryStringIsBuiltCorrectly()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/cdr", "{\"cdr\":[],\"start\":1,\"end\":2}");
        await client.Account.CdrAsync(1700000000, 1700003600);
        var uri = handler.Captured[0].Request.RequestUri!.ToString();
        Assert.Contains("start=1700000000", uri);
        Assert.Contains("end=1700003600", uri);
    }

    [Fact]
    public void ApiError_HelpersReturnExpectedFlags()
    {
        var e = new ApiError(ErrorKind.RateLimit, 429, "x");
        Assert.True(e.IsRateLimit);
        Assert.False(e.IsNotFound);

        var n = new ApiError(ErrorKind.NotFound, 404, "x");
        Assert.True(n.IsNotFound);

        var a = new ApiError(ErrorKind.Authentication, 401, "x");
        Assert.True(a.IsAuthentication);

        var c = new ApiError(ErrorKind.Conflict, 409, "x");
        Assert.True(c.IsConflict);
    }

    [Fact]
    public async Task DecodeJsonFailure_WrapsAsApiError()
    {
        var (client, handler) = TestFactory.NewClient();
        handler.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account", "not-json-at-all");
        await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
    }

    [Fact]
    public async Task TransportError_IsWrappedAfterRetries()
    {
        // Pass an HttpClient with a handler that always throws.
        var throwingHandler = new ThrowingHandler();
        var http = new HttpClient(throwingHandler);
        using var client = new VoiceTelClient(apiKey: "k", baseUrl: "https://x", httpClient: http, maxRetries: 1);
        var ex = await Assert.ThrowsAsync<ApiError>(() => client.Account.GetAsync());
        Assert.Equal(ErrorKind.Unknown, ex.Kind);
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            => throw new HttpRequestException("simulated");
    }
}
