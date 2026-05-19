using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Manages outbound termination gateways on the account.</summary>
public sealed class GatewaysService
{
    private readonly Transport _t;
    internal GatewaysService(Transport t) => _t = t;

    /// <summary>Returns every gateway on the account.</summary>
    public Task<GatewaysListData> ListAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<GatewaysListData>(HttpMethod.Get, "/v2.2/gateways", null, null, requireAuth: true, cancellationToken);

    /// <summary>Creates a new gateway.</summary>
    public Task<GatewayEntry> AddAsync(GatewayAddRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<GatewayEntry>(HttpMethod.Post, "/v2.2/gateways", null, body, requireAuth: true, cancellationToken);

    /// <summary>Fetches a single gateway by id.</summary>
    public Task<GatewayEntry> GetAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<GatewayEntry>(HttpMethod.Get, "/v2.2/gateways/" + id.ToString(CultureInfo.InvariantCulture), null, null, requireAuth: true, cancellationToken);

    /// <summary>Partial-updates a gateway.</summary>
    public Task<GatewayEntry> UpdateAsync(int id, GatewayUpdateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<GatewayEntry>(HttpMethod.Put, "/v2.2/gateways/" + id.ToString(CultureInfo.InvariantCulture), null, body, requireAuth: true, cancellationToken);

    /// <summary>Deletes a gateway. Returns on 204 No Content.</summary>
    public Task RemoveAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, "/v2.2/gateways/" + id.ToString(CultureInfo.InvariantCulture), null, null, requireAuth: true, cancellationToken);

    /// <summary>Returns every number routed through <paramref name="id"/>.</summary>
    public Task<GatewayNumbersData> NumbersAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<GatewayNumbersData>(HttpMethod.Get, "/v2.2/gateways/" + id.ToString(CultureInfo.InvariantCulture) + "/numbers", null, null, requireAuth: true, cancellationToken);
}
