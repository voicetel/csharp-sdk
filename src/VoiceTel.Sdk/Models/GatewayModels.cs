using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/gateways</c>.</summary>
public sealed class GatewayAddRequest
{
    [JsonPropertyName("gateway")] public string Gateway { get; set; } = string.Empty;
    [JsonPropertyName("prefix")] public string? Prefix { get; set; }
    [JsonPropertyName("limit")] public int? Limit { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/gateways/{id}</c>. Pass <c>null</c> to leave a field unchanged.</summary>
public sealed class GatewayUpdateRequest
{
    [JsonPropertyName("gateway")] public string? Gateway { get; set; }
    [JsonPropertyName("prefix")] public string? Prefix { get; set; }
    [JsonPropertyName("limit")] public int? Limit { get; set; }
}

/// <summary>A single gateway row.</summary>
public sealed class GatewayEntry
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("gateway")] public string? Gateway { get; set; }
    [JsonPropertyName("prefix")] public string? Prefix { get; set; }
    [JsonPropertyName("limit")] public int? Limit { get; set; }
    [JsonPropertyName("system")] public bool System { get; set; }
}

/// <summary>One number bound to a gateway, as returned by <c>GET /v2.2/gateways/{id}/numbers</c>.</summary>
public sealed class GatewayNumberSummary
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("translated")] public string Translated { get; set; } = string.Empty;
    [JsonPropertyName("forward")] public bool Forward { get; set; }
    [JsonPropertyName("forwardTo")] public string? ForwardTo { get; set; }
    [JsonPropertyName("cnam")] public bool Cnam { get; set; }
    [JsonPropertyName("carrier")] public int Carrier { get; set; }
    [JsonPropertyName("smsEnabled")] public bool SmsEnabled { get; set; }
    [JsonPropertyName("faxEnabled")] public bool FaxEnabled { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/gateways</c>.</summary>
public sealed class GatewaysListData
{
    [JsonPropertyName("gateways")] public List<GatewayEntry> Gateways { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/gateways/{id}/numbers</c>.</summary>
public sealed class GatewayNumbersData
{
    [JsonPropertyName("numbers")] public List<GatewayNumberSummary> Numbers { get; set; } = new();
}
