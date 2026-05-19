using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>
/// CNAM and LRN dips.
/// <para>Each call costs money; rate them per call rather than fanning out blindly.</para>
/// </summary>
public sealed class LookupsService
{
    private readonly Transport _t;
    internal LookupsService(Transport t) => _t = t;

    /// <summary>Performs a CNAM dip on <paramref name="number"/> (10-digit TN).</summary>
    public Task<CnamData> CnamAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<CnamData>(HttpMethod.Get, "/v2.2/cnam/" + number, null, null, requireAuth: true, cancellationToken);

    /// <summary>
    /// Performs an LRN dip. <paramref name="ani"/> is the presented ANI used only
    /// for billing/auth — it is not echoed back.
    /// </summary>
    public Task<LrnLookupData> LrnAsync(string number, string ani, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<LrnLookupData>(HttpMethod.Get, "/v2.2/lrn/" + number + "/" + ani, null, null, requireAuth: true, cancellationToken);
}
