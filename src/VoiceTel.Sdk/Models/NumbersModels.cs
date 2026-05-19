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

/// <summary>Body for <c>DELETE /v2.2/numbers/messaging-campaign</c>.</summary>
public sealed class BulkUnassignRequest
{
    [JsonPropertyName("numbers")] public List<string> Numbers { get; set; } = new();
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

/// <summary>Messaging-routing state for one number.</summary>
public sealed class NumberMessagingState
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("onAccount")] public bool? OnAccount { get; set; }
    [JsonPropertyName("enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    [JsonPropertyName("routeIn")] public int RouteIn { get; set; }
    [JsonPropertyName("resource")] public string Resource { get; set; } = string.Empty;
    [JsonPropertyName("network")] public string? Network { get; set; }
    [JsonPropertyName("campaign")] public CampaignBinding? Campaign { get; set; }
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

/// <summary>Response data for <c>PUT /v2.2/numbers/{number}/messaging-campaign</c>.</summary>
public sealed class NumberMessagingCampaignAssignData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("campaignId")] public string CampaignId { get; set; } = string.Empty;
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    [JsonPropertyName("network")] public string? Network { get; set; }
    [JsonPropertyName("upstreamCnpId")] public string? UpstreamCnpId { get; set; }
    [JsonPropertyName("previousNetwork")] public string? PreviousNetwork { get; set; }
    [JsonPropertyName("previousNetworkCleared")] public bool PreviousNetworkCleared { get; set; }
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

/// <summary>Response data for <c>PATCH /v2.2/numbers/{number}/port-out-pin</c>.</summary>
public sealed class PortOutPinUpdateData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("portOutPin")] public string PortOutPin { get; set; } = string.Empty;
}
