using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Resources;

namespace VoiceTel.Sdk;

/// <summary>
/// Entry point for the VoiceTel API. Construct one and reach the API through
/// its resource properties — for example <c>client.Numbers.ListAsync(ct)</c>.
/// <para>
/// <see cref="VoiceTelClient"/> is safe to share across threads. If you supply
/// your own <see cref="HttpClient"/>, follow the standard guidance and reuse a
/// single instance for the lifetime of your app.
/// </para>
/// </summary>
public sealed class VoiceTelClient : IDisposable
{
    private readonly Transport _transport;

    /// <summary>Construct a client.</summary>
    /// <param name="apiKey">Bearer token. If <c>null</c>, call <see cref="LoginAsync"/> before making other requests.</param>
    /// <param name="baseUrl">Override the API endpoint. Defaults to <see cref="SdkInfo.DefaultBaseUrl"/>.</param>
    /// <param name="httpClient">Bring-your-own <see cref="HttpClient"/>. The SDK will not dispose it.</param>
    /// <param name="userAgent">Override the <c>User-Agent</c> header.</param>
    /// <param name="maxRetries">Number of retries on 429/5xx. Total attempts is N+1. Defaults to 2.</param>
    public VoiceTelClient(
        string? apiKey = null,
        string? baseUrl = null,
        HttpClient? httpClient = null,
        string? userAgent = null,
        int maxRetries = 2)
    {
        _transport = new Transport(
            httpClient: httpClient,
            ownsHttpClient: httpClient is null,
            baseUrl: baseUrl ?? SdkInfo.DefaultBaseUrl,
            apiKey: apiKey,
            userAgent: userAgent ?? SdkInfo.DefaultUserAgent,
            maxRetries: maxRetries);

        Account = new AccountService(_transport);
        Acl = new AclService(_transport);
        Authentication = new AuthenticationService(_transport);
        E911 = new E911Service(_transport);
        Gateways = new GatewaysService(_transport);
        INumbering = new INumberingService(_transport);
        Lookups = new LookupsService(_transport);
        Messaging = new MessagingService(_transport);
        Numbers = new NumbersService(_transport);
        Support = new SupportService(_transport);
    }

    /// <summary>Account profile, sub-accounts, CDRs, credits, payments, MRC, registration, password recovery.</summary>
    public AccountService Account { get; }

    /// <summary>IP allowlist management.</summary>
    public AclService Acl { get; }

    /// <summary>SIP/HTTP authentication mode and password rotation.</summary>
    public AuthenticationService Authentication { get; }

    /// <summary>e911 record provisioning, address validation, lookup, removal.</summary>
    public E911Service E911 { get; }

    /// <summary>Termination gateway management.</summary>
    public GatewaysService Gateways { get; }

    /// <summary>Inventory search, coverage queries, number orders, and port-ins.</summary>
    public INumberingService INumbering { get; }

    /// <summary>CNAM and LRN dips.</summary>
    public LookupsService Lookups { get; }

    /// <summary>SMS/MMS sending and 10DLC brand/campaign registration.</summary>
    public MessagingService Messaging { get; }

    /// <summary>Telephone-number management.</summary>
    public NumbersService Numbers { get; }

    /// <summary>Support tickets — CRUD + threaded messages + replies.</summary>
    public SupportService Support { get; }

    /// <summary>The configured API endpoint.</summary>
    public string BaseUrl => _transport.BaseUrl;

    /// <summary>The currently installed bearer token, or <c>null</c> before <see cref="LoginAsync"/>.</summary>
    public string? ApiKey => _transport.ApiKey;

    /// <summary>
    /// Exchange username + password for a 32-hex API key and install it on
    /// this client. The exchange counts against the 6 req/hour/IP rate limit
    /// shared by every account/* endpoint.
    /// </summary>
    public async Task<string> LoginAsync(int username, string password, CancellationToken cancellationToken = default)
    {
        var body = new System.Collections.Generic.Dictionary<string, object?>
        {
            ["username"] = username,
            ["password"] = password,
        };
        var data = await _transport.RequestAsync<Models.AccountApiKeyData>(
            HttpMethod.Post, "/v2.2/account/api-key", null, body, requireAuth: false, cancellationToken)
            .ConfigureAwait(false);
        if (data is null || string.IsNullOrEmpty(data.ApiKey))
        {
            throw new ApiError(
                ErrorKind.Authentication,
                0,
                "api-key response did not contain data.apikey",
                body: data);
        }
        _transport.SetBearer(data.ApiKey!);
        return data.ApiKey!;
    }

    /// <inheritdoc />
    public void Dispose() => _transport.Dispose();
}
