using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>
/// CNAM, carrier &amp; line type, and LRN dips.
/// <para>Each call costs money; rate them per call rather than fanning out blindly.</para>
/// </summary>
public sealed class LookupsService
{
    private readonly Transport _t;
    internal LookupsService(Transport t) => _t = t;

    /// <summary>Performs a CNAM dip on <paramref name="number"/> (10-digit TN).</summary>
    public Task<CnamData> CnamAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<CnamData>(HttpMethod.Get, "/v2.2/cnam/" + Uri.EscapeDataString(number), null, null, requireAuth: true, cancellationToken);

    /// <summary>
    /// Looks up the carrier and line type of <paramref name="number"/> (10-digit TN) —
    /// voip, landline, mobile, ... Billed per lookup at the account's CNAM rate, only
    /// when a result is returned.
    /// </summary>
    public Task<CarrierData> CarrierAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<CarrierData>(HttpMethod.Get, "/v2.2/carrier/" + Uri.EscapeDataString(number), null, null, requireAuth: true, cancellationToken);

    /// <summary>
    /// Performs an LRN dip. <paramref name="ani"/> is the presented ANI used only
    /// for billing/auth — it is not echoed back.
    /// </summary>
    public Task<LrnLookupData> LrnAsync(string number, string ani, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<LrnLookupData>(HttpMethod.Get, "/v2.2/lrn/" + Uri.EscapeDataString(number) + "/" + Uri.EscapeDataString(ani), null, null, requireAuth: true, cancellationToken);
}
