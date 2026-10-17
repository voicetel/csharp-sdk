using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Entry point for every operation on a telephone number owned by the account.</summary>
public sealed class NumbersService
{
    private const string Prefix = "/v2.2/numbers/";
    private readonly Transport _t;
    internal NumbersService(Transport t) => _t = t;

    private static string NumberPath(string number) => Prefix + Uri.EscapeDataString(number);

    /// <summary>Returns every TN on the account.</summary>
    public Task<NumbersListData> ListAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumbersListData>(HttpMethod.Get, "/v2.2/numbers", null, null, requireAuth: true, cancellationToken);

    /// <summary>Attaches a TN to the account.</summary>
    public Task<NumberAddData> AddAsync(NumberAddRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberAddData>(HttpMethod.Post, "/v2.2/numbers", null, body, requireAuth: true, cancellationToken);

    /// <summary>Fetches one TN.</summary>
    public Task<NumberDetail> GetAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberDetail>(HttpMethod.Get, NumberPath(number), null, null, requireAuth: true, cancellationToken);

    /// <summary>Detaches a TN. Returns on 204 No Content.</summary>
    public Task RemoveAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, NumberPath(number), null, null, requireAuth: true, cancellationToken);

    /// <summary>Transfers a TN to another account on the same authenticated org.</summary>
    public Task<NumberMoveData> MoveAsync(string number, NumberMoveRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberMoveData>(new HttpMethod("PATCH"), NumberPath(number), null, body, requireAuth: true, cancellationToken);

    /// <summary>Returns a TN to the network. Returns on 204 No Content.</summary>
    public Task ReleaseAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Post, NumberPath(number) + "/release", null, null, requireAuth: true, cancellationToken);

    /// <summary>Updates a TN's outbound route.</summary>
    public Task<NumberRouteData> SetRouteAsync(string number, NumberRouteRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberRouteData>(HttpMethod.Put, NumberPath(number) + "/route", null, body, requireAuth: true, cancellationToken);

    /// <summary>Updates a TN's DNIS translation.</summary>
    public Task<NumberTranslationData> SetTranslationAsync(string number, NumberTranslationRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberTranslationData>(HttpMethod.Put, NumberPath(number) + "/translation", null, body, requireAuth: true, cancellationToken);

    /// <summary>Toggles inbound CNAM lookup for a TN.</summary>
    public Task<NumberCnamData> SetCnamAsync(string number, NumberCnamRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberCnamData>(HttpMethod.Put, NumberPath(number) + "/cnam", null, body, requireAuth: true, cancellationToken);

    /// <summary>Updates a TN's outbound caller name (LIDB).</summary>
    public Task<NumberLidbData> SetLidbAsync(string number, NumberLidbRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberLidbData>(HttpMethod.Put, NumberPath(number) + "/lidb", null, body, requireAuth: true, cancellationToken);

    /// <summary>Reads fax-to-email routing.</summary>
    public Task<NumberFaxData> GetFaxAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberFaxData>(HttpMethod.Get, NumberPath(number) + "/fax", null, null, requireAuth: true, cancellationToken);

    /// <summary>Enables fax-to-email routing.</summary>
    public Task<NumberFaxData> SetFaxAsync(string number, NumberFaxRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberFaxData>(HttpMethod.Put, NumberPath(number) + "/fax", null, body, requireAuth: true, cancellationToken);

    /// <summary>Disables fax-to-email. Returns on 204 No Content.</summary>
    public Task RemoveFaxAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, NumberPath(number) + "/fax", null, null, requireAuth: true, cancellationToken);

    /// <summary>Enables call forwarding.</summary>
    public Task<NumberForwardData> SetForwardAsync(string number, NumberForwardRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberForwardData>(HttpMethod.Put, NumberPath(number) + "/forward", null, body, requireAuth: true, cancellationToken);

    /// <summary>Disables call forwarding. Returns on 204 No Content.</summary>
    public Task RemoveForwardAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, NumberPath(number) + "/forward", null, null, requireAuth: true, cancellationToken);

    /// <summary>Reads SMS routing.</summary>
    public Task<NumberSmsData> GetSmsAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberSmsData>(HttpMethod.Get, NumberPath(number) + "/sms", null, null, requireAuth: true, cancellationToken);

    /// <summary>Configures SMS routing.</summary>
    public Task<NumberSmsData> SetSmsAsync(string number, NumberSmsRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberSmsData>(HttpMethod.Put, NumberPath(number) + "/sms", null, body, requireAuth: true, cancellationToken);

    /// <summary>Clears SMS routing. Returns on 204 No Content.</summary>
    public Task RemoveSmsAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, NumberPath(number) + "/sms", null, null, requireAuth: true, cancellationToken);

    /// <summary>Returns the messaging state for one TN.</summary>
    public Task<NumberMessagingState> GetMessagingAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberMessagingState>(HttpMethod.Get, NumberPath(number) + "/messaging", null, null, requireAuth: true, cancellationToken);

    /// <summary>Updates inbound/outbound routing for one TN.</summary>
    public Task<NumberMessagingPatchData> PatchMessagingAsync(string number, NumberMessagingPatchRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberMessagingPatchData>(new HttpMethod("PATCH"), NumberPath(number) + "/messaging", null, body, requireAuth: true, cancellationToken);

    /// <summary>Binds a 10DLC campaign to a TN.</summary>
    public Task<NumberMessagingCampaignAssignData> AssignCampaignAsync(string number, NumberCampaignAssignRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<NumberMessagingCampaignAssignData>(HttpMethod.Put, NumberPath(number) + "/messaging-campaign", null, body, requireAuth: true, cancellationToken);

    /// <summary>Removes the campaign binding from a TN. Returns 200 with a body.</summary>
    public Task<NumberMessagingCampaignUnassignData> UnassignCampaignAsync(string number, string campaignId, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("campaignId", campaignId);
        return _t.RequestAsync<NumberMessagingCampaignUnassignData>(HttpMethod.Delete, NumberPath(number) + "/messaging-campaign", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Removes the campaign binding from many TNs at once (max 1000). Returns 200 with a body.</summary>
    public Task<NumbersMessagingCampaignUnassignData> BulkUnassignCampaignAsync(IEnumerable<string> numbers, string campaignId, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("numbers", string.Join(",", numbers));
        q.Add("campaignId", campaignId);
        return _t.RequestAsync<NumbersMessagingCampaignUnassignData>(
            HttpMethod.Delete,
            "/v2.2/numbers/messaging-campaign",
            q.HasAny ? q.ToString() : null,
            null,
            requireAuth: true,
            cancellationToken);
    }

    /// <summary>Sets the port-out PIN for a TN.</summary>
    public Task<PortOutPinUpdateData> SetPortOutPinAsync(string number, PortOutPinUpdateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<PortOutPinUpdateData>(new HttpMethod("PATCH"), NumberPath(number) + "/port-out-pin", null, body, requireAuth: true, cancellationToken);
}
