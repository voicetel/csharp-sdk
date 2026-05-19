using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Covers inventory searches, orders, and port-ins.</summary>
public sealed class INumberingService
{
    private readonly Transport _t;
    internal INumberingService(Transport t) => _t = t;

    /// <summary>Searches available TNs by NPA/NXX/state/rate-center/etc.</summary>
    public Task<InventorySearchData> SearchInventoryAsync(InventoryQuery query, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.AddInt("npa", query.Npa);
        q.AddInt("nxx", query.Nxx);
        q.Add("state", query.State);
        q.Add("ratecenter", query.RateCenter);
        q.Add("contains", query.Contains);
        q.Add("endswith", query.EndsWith);
        q.AddInt("limit", query.Limit);
        return _t.RequestAsync<InventorySearchData>(HttpMethod.Get, "/v2.2/inventory", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Returns aggregated availability buckets.</summary>
    public Task<InventoryCoverageData> CoverageAsync(CoverageQuery query, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("state", query.State);
        q.Add("ratecenter", query.RateCenter);
        return _t.RequestAsync<InventoryCoverageData>(HttpMethod.Get, "/v2.2/inventory/coverage", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Purchases new TNs.</summary>
    public Task<OrderCreateData> OrderAsync(OrderCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<OrderCreateData>(HttpMethod.Post, "/v2.2/orders", null, body, requireAuth: true, cancellationToken);

    /// <summary>Lists every port-in record on the account.</summary>
    public Task<PortListData> PortsAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortListData>(HttpMethod.Get, "/v2.2/ports", null, null, requireAuth: true, cancellationToken);

    /// <summary>Fetches detail for one port-in by id.</summary>
    public Task<PortDetailData> PortAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortDetailData>(HttpMethod.Get, "/v2.2/ports/" + id.ToString(CultureInfo.InvariantCulture), null, null, requireAuth: true, cancellationToken);

    /// <summary>Submits a port-in order.</summary>
    public Task<PortSubmitData> SubmitPortAsync(PortSubmitRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortSubmitData>(HttpMethod.Post, "/v2.2/ports", null, body, requireAuth: true, cancellationToken);

    /// <summary>Checks whether a given TN can be ported in.</summary>
    public Task<PortAvailabilityData> PortAvailabilityAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortAvailabilityData>(HttpMethod.Get, "/v2.2/ports/availability/" + number, null, null, requireAuth: true, cancellationToken);
}
