using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk;

namespace VoiceTel.Sdk.Tests;

/// <summary>
/// One captured HTTP request plus the response to return.
/// </summary>
internal sealed class MockExchange
{
    public required HttpMethod Method { get; init; }
    public required string Path { get; init; }
    public HttpStatusCode Status { get; init; } = HttpStatusCode.OK;
    public string? ResponseBody { get; init; }
    public Dictionary<string, string>? ResponseHeaders { get; init; }
}

/// <summary>
/// <see cref="HttpMessageHandler"/> that matches the next request against a queued
/// expectation. Captures the request URI and body for inspection.
/// </summary>
internal sealed class MockHttpHandler : HttpMessageHandler
{
    private readonly Queue<MockExchange> _exchanges = new();
    public List<(HttpRequestMessage Request, string Body)> Captured { get; } = new();

    public MockHttpHandler Enqueue(MockExchange exchange)
    {
        _exchanges.Enqueue(exchange);
        return this;
    }

    public MockHttpHandler EnqueueJson(HttpMethod method, string path, string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        _exchanges.Enqueue(new MockExchange
        {
            Method = method,
            Path = path,
            Status = status,
            ResponseBody = json,
        });
        return this;
    }

    public MockHttpHandler EnqueueEnvelope(HttpMethod method, string path, string innerJson, HttpStatusCode status = HttpStatusCode.OK)
        => EnqueueJson(method, path, "{\"status\":\"success\",\"data\":" + innerJson + "}", status);

    public MockHttpHandler EnqueueNoContent(HttpMethod method, string path)
        => EnqueueJson(method, path, string.Empty, HttpStatusCode.NoContent);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_exchanges.Count == 0)
        {
            throw new InvalidOperationException("Unexpected HTTP request: " + request.Method + " " + request.RequestUri);
        }
        var exchange = _exchanges.Dequeue();
        var body = string.Empty;
        if (request.Content is not null)
        {
            body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        Captured.Add((request, body));

        var response = new HttpResponseMessage(exchange.Status);
        if (exchange.ResponseBody is not null)
        {
            response.Content = new StringContent(exchange.ResponseBody, System.Text.Encoding.UTF8, "application/json");
        }
        if (exchange.ResponseHeaders is not null)
        {
            foreach (var kv in exchange.ResponseHeaders)
            {
                response.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }
        }
        return response;
    }
}

internal static class TestFactory
{
    public const string ApiKey = "deadbeefdeadbeefdeadbeefdeadbeef";

    public static (VoiceTelClient Client, MockHttpHandler Handler) NewClient(int maxRetries = 0)
    {
        var handler = new MockHttpHandler();
        var http = new HttpClient(handler) { BaseAddress = null };
        var client = new VoiceTelClient(
            apiKey: ApiKey,
            baseUrl: "https://api.example.com",
            httpClient: http,
            maxRetries: maxRetries);
        return (client, handler);
    }
}
