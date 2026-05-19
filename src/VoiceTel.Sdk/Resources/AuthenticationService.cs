using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Manages SIP/HTTP authentication settings (mode + password).</summary>
public sealed class AuthenticationService
{
    private readonly Transport _t;
    internal AuthenticationService(Transport t) => _t = t;

    /// <summary>Returns the current auth mode and allowlist.</summary>
    public Task<AuthGetData> GetAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AuthGetData>(HttpMethod.Get, "/v2.2/auth", null, null, requireAuth: true, cancellationToken);

    /// <summary>Sets the auth mode and/or password.</summary>
    public Task<AuthPutData> UpdateAsync(AuthPutRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AuthPutData>(HttpMethod.Put, "/v2.2/auth", null, body, requireAuth: true, cancellationToken);
}
