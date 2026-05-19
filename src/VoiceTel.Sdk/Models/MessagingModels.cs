using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>
/// Body for <c>POST /v2.2/messages</c>.
/// <para>
/// Wire field names are <c>fromNumber</c> / <c>toNumber</c>. Supplying
/// <see cref="MediaUrls"/> switches the message to MMS and unlocks <see cref="Subject"/>.
/// </para>
/// </summary>
public sealed class MessageSendRequest
{
    [JsonPropertyName("fromNumber")] public string FromNumber { get; set; } = string.Empty;
    [JsonPropertyName("toNumber")] public string ToNumber { get; set; } = string.Empty;
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("mediaUrls")] public List<string>? MediaUrls { get; set; }
}

/// <summary>Body for <c>POST /v2.2/messaging/brands</c>.</summary>
public sealed class MessagingBrandCreateRequest
{
    [JsonPropertyName("messagingBrandId")] public string MessagingBrandId { get; set; } = string.Empty;
    [JsonPropertyName("messagingBrandName")] public string MessagingBrandName { get; set; } = string.Empty;
    [JsonPropertyName("messagingBrandDescription")] public string? MessagingBrandDescription { get; set; }
}

/// <summary>Body for <c>POST /v2.2/messaging/campaigns</c>.</summary>
public sealed class MessagingCampaignCreateRequest
{
    [JsonPropertyName("messagingBrandId")] public string MessagingBrandId { get; set; } = string.Empty;
    [JsonPropertyName("externalCampaignId")] public string ExternalCampaignId { get; set; } = string.Empty;
    [JsonPropertyName("campaignDescription")] public string CampaignDescription { get; set; } = string.Empty;
    [JsonPropertyName("campaignClassName")] public string? CampaignClassName { get; set; }
    [JsonPropertyName("campaignStartDate")] public string? CampaignStartDate { get; set; }
}

/// <summary>Per-record value inside a <see cref="MessageRecord"/>.</summary>
public sealed class MessageRecordValue
{
    [JsonPropertyName("sourceNumber")] public string? SourceNumber { get; set; }
    [JsonPropertyName("destinationNumber")] public string? DestinationNumber { get; set; }
    [JsonPropertyName("direction")] public string? Direction { get; set; }
    [JsonPropertyName("rate")] public string? Rate { get; set; }
    [JsonPropertyName("number")] public int Number { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
}

/// <summary>One row in <see cref="MessageHistoryData.Messages"/>.</summary>
public sealed class MessageRecord
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("key")] public List<object?> Key { get; set; } = new();
    [JsonPropertyName("value")] public MessageRecordValue Value { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/messages</c>.</summary>
public sealed class MessageHistoryData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("fromTs")] public int FromTs { get; set; }
    [JsonPropertyName("toTs")] public int ToTs { get; set; }
    [JsonPropertyName("messages")] public List<MessageRecord> Messages { get; set; } = new();
}

/// <summary>Response data for <c>POST /v2.2/messages</c>.</summary>
public sealed class MessageSendData
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("fromNumber")] public string FromNumber { get; set; } = string.Empty;
    [JsonPropertyName("toNumber")] public string ToNumber { get; set; } = string.Empty;
    [JsonPropertyName("parts")] public int Parts { get; set; }
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("mediaUrls")] public List<string>? MediaUrls { get; set; }
}

/// <summary>Status payload for brand registration.</summary>
public sealed class BrandRegistrationResult
{
    [JsonPropertyName("statusCode")] public string StatusCode { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}

/// <summary>Response data for <c>POST /v2.2/messaging/brands</c>.</summary>
public sealed class MessagingBrandCreateData
{
    [JsonPropertyName("result")] public BrandRegistrationResult Result { get; set; } = new();
}

/// <summary>Status payload for campaign registration.</summary>
public sealed class CampaignRegistrationResult
{
    [JsonPropertyName("statusCode")] public string StatusCode { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}

/// <summary>Response data for <c>POST /v2.2/messaging/campaigns</c>.</summary>
public sealed class MessagingCampaignCreateData
{
    [JsonPropertyName("result")] public CampaignRegistrationResult Result { get; set; } = new();
}

/// <summary>A single campaign and its currently-bound numbers.</summary>
public sealed class CampaignStatusItem
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("numbers")] public List<string> Numbers { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/messaging/campaigns</c>.</summary>
public sealed class MessagingCampaignStatusData
{
    [JsonPropertyName("campaigns")] public List<CampaignStatusItem> Campaigns { get; set; } = new();
}

/// <summary>Optional query filters for <c>History</c>.</summary>
public sealed class HistoryOptions
{
    public string? Number { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public string? Type { get; set; }
}
