using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/numbers</c>.</summary>
public sealed class NumberAddRequest
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("route")] public int? Route { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/route</c>.</summary>
public sealed class NumberRouteRequest
{
    [JsonPropertyName("route")] public int Route { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/cnam</c>.</summary>
public sealed class NumberCnamRequest
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/lidb</c>.</summary>
public sealed class NumberLidbRequest
{
    [JsonPropertyName("cnam")] public string Cnam { get; set; } = string.Empty;
    [JsonPropertyName("customerOrderReference")] public string? CustomerOrderReference { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/fax</c>.</summary>
public sealed class NumberFaxRequest
{
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/forward</c>.</summary>
public sealed class NumberForwardRequest
{
    [JsonPropertyName("destination")] public string Destination { get; set; } = string.Empty;
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/translation</c>.</summary>
public sealed class NumberTranslationRequest
{
    [JsonPropertyName("translation")] public string Translation { get; set; } = string.Empty;
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/sms</c>.</summary>
public sealed class NumberSmsRequest
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("resource")] public string Resource { get; set; } = string.Empty;
}

/// <summary>Body for <c>PATCH /v2.2/numbers/{number}/messaging</c>. At least one field must be set.</summary>
public sealed class NumberMessagingPatchRequest
{
    [JsonPropertyName("routeIn")] public int? RouteIn { get; set; }
    [JsonPropertyName("routeOut")] public int? RouteOut { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/numbers/{number}/messaging-campaign</c>.</summary>
public sealed class NumberCampaignAssignRequest
{
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
}

/// <summary>Body for <c>PATCH /v2.2/numbers/{number}</c>.</summary>
public sealed class NumberMoveRequest
{
    [JsonPropertyName("accountId")] public int AccountId { get; set; }
    [JsonPropertyName("route")] public int Route { get; set; }
}

/// <summary>Body for <c>PATCH /v2.2/numbers/{number}/port-out-pin</c>.</summary>
public sealed class PortOutPinUpdateRequest
{
    [JsonPropertyName("pin")] public string Pin { get; set; } = string.Empty;
}

/// <summary>Per-number routing/feature state returned by <c>GET /v2.2/numbers</c> and <c>GET /v2.2/numbers/{number}</c>.</summary>
public sealed class NumberDetail
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("translated")] public string Translated { get; set; } = string.Empty;
    [JsonPropertyName("route")] public int Route { get; set; }
    [JsonPropertyName("gateway")] public string? Gateway { get; set; }
    [JsonPropertyName("cnam")] public bool Cnam { get; set; }
    [JsonPropertyName("forward")] public bool Forward { get; set; }
    [JsonPropertyName("forwardTo")] public string? ForwardTo { get; set; }
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    [JsonPropertyName("smsEnabled")] public bool SmsEnabled { get; set; }
    [JsonPropertyName("faxEnabled")] public bool FaxEnabled { get; set; }
}

/// <summary>Campaign currently bound to a number, with CSP status.</summary>
public sealed class CampaignBinding
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("network")] public string Network { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("upstreamCnpId")] public string UpstreamCnpId { get; set; } = string.Empty;
}

/// <summary>
/// State of an asynchronous (path C) enablement. <see cref="State"/> progresses
/// queued -> hosting -> assigning -> routing -> done, or failed (see <see cref="Message"/>);
/// the number keeps its previous routing until done. Attached to a campaign
/// assignment (<c>Status</c> "pending") or surfaced by
/// <c>GET /v2.2/numbers/{number}/messaging</c> while provisioning is in flight.
/// </summary>
public sealed class MessagingEnablementJob
{
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
    /// <summary>queued, hosting, assigning, routing, done, or failed.</summary>
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    /// <summary>Latest progress note, or the reason for failure.</summary>
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("createdAt")] public string CreatedAt { get; set; } = string.Empty;
    [JsonPropertyName("updatedAt")] public string UpdatedAt { get; set; } = string.Empty;
}

/// <summary>Messaging-routing state for one number.</summary>
public sealed class NumberMessagingState
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("onAccount")] public bool? OnAccount { get; set; }
    [JsonPropertyName("enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    [JsonPropertyName("routeIn")] public int RouteIn { get; set; }
    [JsonPropertyName("resource")] public string Resource { get; set; } = string.Empty;
    /// <summary>Messaging path the number is currently on ("A", "B", or "C"); <c>null</c> when carrier is 0 or any non-messaging carrier.</summary>
    [JsonPropertyName("network")] public string? Network { get; set; }
    [JsonPropertyName("campaign")] public CampaignBinding? Campaign { get; set; }
    /// <summary>Async path-C enablement in progress for this number; <c>null</c> when none.</summary>
    [JsonPropertyName("pending")] public MessagingEnablementJob? Pending { get; set; }
}

/// <summary>Response data for <c>POST /v2.2/numbers</c>.</summary>
public sealed class NumberAddData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("route")] public int Route { get; set; }
}

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/cnam</c>.</summary>
public sealed class NumberCnamData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("cnam")] public bool Cnam { get; set; }
}

/// <summary>Response data for <c>GET/PUT /v2.2/numbers/{number}/fax</c>.</summary>
public sealed class NumberFaxData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
}

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/forward</c>.</summary>
public sealed class NumberForwardData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("forwardTo")] public string? ForwardTo { get; set; }
}

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/lidb</c>.</summary>
public sealed class NumberLidbData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("cnam")] public string Cnam { get; set; } = string.Empty;
    [JsonPropertyName("customerOrderReference")] public string CustomerOrderReference { get; set; } = string.Empty;
    [JsonPropertyName("carrierStatus")] public string CarrierStatus { get; set; } = string.Empty;
}

/// <summary>Response data for <c>PATCH /v2.2/numbers/{number}/messaging</c>.</summary>
public sealed class NumberMessagingPatchData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("updated")] public List<string> Updated { get; set; } = new();
}

/// <summary>Response data for <c>PATCH /v2.2/numbers/{number}</c>.</summary>
public sealed class NumberMoveData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("accountId")] public int AccountId { get; set; }
    [JsonPropertyName("route")] public int Route { get; set; }
}

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/route</c>.</summary>
public sealed class NumberRouteData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("route")] public int Route { get; set; }
}

/// <summary>Response data for <c>GET/PUT /v2.2/numbers/{number}/sms</c>.</summary>
public sealed class NumberSmsData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("resource")] public string Resource { get; set; } = string.Empty;
}

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/translation</c>.</summary>
public sealed class NumberTranslationData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("translation")] public string Translation { get; set; } = string.Empty;
}

/// <summary>
/// Response data for <c>PUT /v2.2/numbers/{number}/messaging-campaign</c>.
/// <para>
/// Result of binding a single number to a 10DLC campaign. The campaign must be
/// available to the account and ACTIVE; the number is enabled on the messaging
/// path the campaign lives on. Idempotent: a number already bound to the campaign
/// reports <see cref="Status"/> "already".
/// </para>
/// <para>
/// <see cref="Status"/> is "enabled" (bound now, path B), "already" (the number was
/// already bound to this campaign — records reconciled, no new order), or "pending"
/// (accepted and being provisioned on path C — poll
/// <c>GET /v2.2/numbers/{number}/messaging</c> until the job's state is done or
/// failed). <see cref="OrderId"/> (a string or number on the wire) is set for
/// path-B assignments only; path C reports a <see cref="Job"/> instead.
/// </para>
/// </summary>
public sealed class NumberMessagingCampaignAssignData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
    /// <summary>"enabled" (bound now, path B), "already" (already bound, no new order), or "pending" (provisioning asynchronously on path C).</summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    /// <summary>Outbound carrier id the number routes through once enabled (19 = path B, 1 = path C).</summary>
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    /// <summary>Messaging path the campaign resolves to ("B" or "C"); path B is assigned synchronously, path C asynchronously.</summary>
    [JsonPropertyName("network")] public string? Network { get; set; }
    /// <summary>CNP ID from the CSP sharing record: CAMP003 = path B, BANDW = path C.</summary>
    [JsonPropertyName("upstreamCnpId")] public string? UpstreamCnpId { get; set; }
    /// <summary>True when the number was already bound to this campaign on the target messaging path; records are reconciled and no new network order is placed.</summary>
    [JsonPropertyName("alreadyAssigned")] public bool AlreadyAssigned { get; set; }
    /// <summary>Messaging path the number was bound to before this call ("A" or "B"); <c>null</c> if there was no prior binding.</summary>
    [JsonPropertyName("previousNetwork")] public string? PreviousNetwork { get; set; }
    [JsonPropertyName("previousNetworkCleared")] public bool PreviousNetworkCleared { get; set; }
    /// <summary>Network order id created for a path-B assignment. <c>null</c> for path C (see <see cref="Job"/>), idempotent no-ops, and assignments confirmed without a new order.</summary>
    [JsonPropertyName("orderId")] public object? OrderId { get; set; }
    /// <summary>Non-fatal condition detected during the assignment (for example a stale registration on the other messaging path whose removal has been requested).</summary>
    [JsonPropertyName("warning")] public string? Warning { get; set; }
    /// <summary>Human-readable note for a pending enablement.</summary>
    [JsonPropertyName("message")] public string? Message { get; set; }
    /// <summary>Async path-C enablement job when <see cref="Status"/> is "pending"; <c>null</c> otherwise.</summary>
    [JsonPropertyName("job")] public MessagingEnablementJob? Job { get; set; }
}

/// <summary>Response data for <c>DELETE /v2.2/numbers/{number}/messaging-campaign</c>.</summary>
public sealed class NumberMessagingCampaignUnassignData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
    [JsonPropertyName("network")] public string? Network { get; set; }
    [JsonPropertyName("upstreamCnpId")] public string? UpstreamCnpId { get; set; }
    [JsonPropertyName("unassigned")] public bool Unassigned { get; set; }
}

/// <summary>One row in <see cref="NumbersMessagingCampaignUnassignData.Failed"/>.</summary>
public sealed class CampaignUnassignFailure
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
}

/// <summary>Response data for <c>DELETE /v2.2/numbers/messaging-campaign</c> (bulk unassign).</summary>
public sealed class NumbersMessagingCampaignUnassignData
{
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
    [JsonPropertyName("network")] public string? Network { get; set; }
    [JsonPropertyName("upstreamCnpId")] public string? UpstreamCnpId { get; set; }
    [JsonPropertyName("unassignedNumbers")] public List<string> UnassignedNumbers { get; set; } = new();
    [JsonPropertyName("failed")] public List<CampaignUnassignFailure>? Failed { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/numbers</c>.</summary>
public sealed class NumbersListData
{
    [JsonPropertyName("numbers")] public List<NumberDetail> Numbers { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/numbers/messaging</c>.</summary>
public sealed class NumbersMessagingListData
{
    [JsonPropertyName("numbers")] public List<NumberMessagingState> Numbers { get; set; } = new();
}

/// <summary>
/// Response data for <c>PATCH /v2.2/numbers/{number}/port-out-pin</c>.
/// Idempotent: a PIN is locked for 30 days once set, so a number that already has
/// one returns it unchanged with <see cref="Changed"/> <c>false</c>.
/// </summary>
public sealed class PortOutPinUpdateData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    /// <summary>The port-out PIN now on file for this number (the value just set, or the existing value if the number was already locked).</summary>
    [JsonPropertyName("portOutPin")] public string PortOutPin { get; set; } = string.Empty;
    /// <summary>True if this request set the PIN; false if the number already had one, which is locked for 30 days once set and is returned unchanged.</summary>
    [JsonPropertyName("changed")] public bool Changed { get; set; }
}
