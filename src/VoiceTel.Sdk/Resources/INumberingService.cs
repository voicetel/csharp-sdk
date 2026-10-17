using System;
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

    /// <summary>Searches available TNs by mask, quantity, province, rate center, and LATA.</summary>
    public Task<InventorySearchData> SearchInventoryAsync(InventoryQuery query, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("tnMask", query.TnMask);
        q.AddInt("quantity", query.Quantity);
        q.Add("province", query.Province);
        q.Add("rateCenter", query.RateCenter);
        q.Add("lata", query.Lata);
        return _t.RequestAsync<InventorySearchData>(HttpMethod.Get, "/v2.2/inventory", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Returns aggregated availability buckets.</summary>
    public Task<InventoryCoverageData> CoverageAsync(CoverageQuery query, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("countBy", query.CountBy);
        q.Add("locState", query.LocState);
        q.Add("city", query.City);
        q.Add("rcAbbre", query.RcAbbre);
        q.Add("npa", query.Npa);
        q.Add("lata", query.Lata);
        return _t.RequestAsync<InventoryCoverageData>(HttpMethod.Get, "/v2.2/inventory/coverage", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Purchases new TNs.</summary>
    public Task<OrderCreateData> OrderAsync(OrderCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<OrderCreateData>(HttpMethod.Post, "/v2.2/orders", null, body, requireAuth: true, cancellationToken);

    /// <summary>Lists every port-in record on the account.</summary>
    public Task<PortListData> PortsAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortListData>(HttpMethod.Get, "/v2.2/ports", null, null, requireAuth: true, cancellationToken);

    /// <summary>Fetches detail for one port-in by id (the 5-character <c>pid</c> from the list response).</summary>
    public Task<PortDetailData> PortAsync(string id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortDetailData>(HttpMethod.Get, "/v2.2/ports/" + Uri.EscapeDataString(id), null, null, requireAuth: true, cancellationToken);

    /// <summary>Submits a port-in order.</summary>
    public Task<PortSubmitData> SubmitPortAsync(PortSubmitRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortSubmitData>(HttpMethod.Post, "/v2.2/ports", null, body, requireAuth: true, cancellationToken);

    /// <summary>Checks whether a given TN can be ported in.</summary>
    public Task<PortAvailabilityData> PortAvailabilityAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortAvailabilityData>(HttpMethod.Get, "/v2.2/ports/availability/" + Uri.EscapeDataString(number), null, null, requireAuth: true, cancellationToken);
}
