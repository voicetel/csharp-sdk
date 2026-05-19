using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>
/// Manages e911 records and address validation.
/// <para>
/// Note the asymmetric <c>dn</c> formats: requests take a 10-digit TN; responses
/// return the 11-digit E.164 US form (country code 1 prepended).
/// </para>
/// </summary>
public sealed class E911Service
{
    private readonly Transport _t;
    internal E911Service(Transport t) => _t = t;

    /// <summary>Returns every e911 record on the account.</summary>
    public Task<E911AllData> ListAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<E911AllData>(HttpMethod.Get, "/v2.2/e911", null, null, requireAuth: true, cancellationToken);

    /// <summary>Validates and provisions an e911 record in one call.</summary>
    public Task<E911RecordData> CreateAsync(E911CreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<E911RecordData>(HttpMethod.Post, "/v2.2/e911", null, body, requireAuth: true, cancellationToken);

    /// <summary>Validates an address, returning an AddressID for use with <see cref="ProvisionAsync"/>.</summary>
    public Task<E911ValidateData> ValidateAsync(E911AddressRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<E911ValidateData>(HttpMethod.Post, "/v2.2/e911/validations", null, body, requireAuth: true, cancellationToken);

    /// <summary>Fetches the e911 record for <paramref name="dn"/>.</summary>
    public Task<E911RecordData> GetAsync(string dn, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<E911RecordData>(HttpMethod.Get, "/v2.2/e911/" + dn, null, null, requireAuth: true, cancellationToken);

    /// <summary>Uses a previously-validated AddressID to provision e911 for <paramref name="dn"/>.</summary>
    public Task<E911RecordData> ProvisionAsync(string dn, E911ProvisionByIdRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<E911RecordData>(HttpMethod.Put, "/v2.2/e911/" + dn, null, body, requireAuth: true, cancellationToken);

    /// <summary>Deletes the e911 record for <paramref name="dn"/>. Returns on 204 No Content.</summary>
    public Task RemoveAsync(string dn, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, "/v2.2/e911/" + dn, null, null, requireAuth: true, cancellationToken);
}
