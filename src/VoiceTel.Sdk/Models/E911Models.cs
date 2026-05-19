using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/e911/validations</c>.</summary>
public sealed class E911AddressRequest
{
    [JsonPropertyName("address1")] public string Address1 { get; set; } = string.Empty;
    [JsonPropertyName("address2")] public string? Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
}

/// <summary>Body for <c>POST /v2.2/e911</c> (validate + provision in one call).</summary>
public sealed class E911CreateRequest
{
    [JsonPropertyName("dn")] public string Dn { get; set; } = string.Empty;
    [JsonPropertyName("callername")] public string Callername { get; set; } = string.Empty;
    [JsonPropertyName("address1")] public string Address1 { get; set; } = string.Empty;
    [JsonPropertyName("address2")] public string? Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
}

/// <summary>Body for <c>PUT /v2.2/e911/{dn}</c>.</summary>
public sealed class E911ProvisionByIdRequest
{
    [JsonPropertyName("callername")] public string Callername { get; set; } = string.Empty;
    [JsonPropertyName("addressid")] public int AddressId { get; set; }
}

/// <summary>An e911 record bound to a TN.</summary>
public sealed class E911Entry
{
    [JsonPropertyName("dn")] public string Dn { get; set; } = string.Empty;
    [JsonPropertyName("callername")] public string Callername { get; set; } = string.Empty;
    [JsonPropertyName("address1")] public string Address1 { get; set; } = string.Empty;
    [JsonPropertyName("address2")] public string? Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
}

/// <summary>Result from <c>POST /v2.2/e911/validations</c>.</summary>
public sealed class E911ValidatedAddress
{
    [JsonPropertyName("addressid")] public int AddressId { get; set; }
    [JsonPropertyName("address1")] public string Address1 { get; set; } = string.Empty;
    [JsonPropertyName("address2")] public string? Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
}

/// <summary>Response data for <c>GET /v2.2/e911</c>.</summary>
public sealed class E911AllData
{
    [JsonPropertyName("records")] public List<E911Entry> Records { get; set; } = new();
}

/// <summary>Response data for <c>GET/POST/PUT /v2.2/e911[/{dn}]</c>.</summary>
public sealed class E911RecordData
{
    [JsonPropertyName("record")] public E911Entry Record { get; set; } = new();
}

/// <summary>Response data for <c>POST /v2.2/e911/validations</c>.</summary>
public sealed class E911ValidateData
{
    [JsonPropertyName("address")] public E911ValidatedAddress Address { get; set; } = new();
}
