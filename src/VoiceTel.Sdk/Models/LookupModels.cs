using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Response data for <c>GET /v2.2/cnam/{number}</c>.</summary>
public sealed class CnamData
{
    [JsonPropertyName("cnam")] public string? Cnam { get; set; }
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
}

/// <summary>
/// Carrier and line-type details for <c>GET /v2.2/carrier/{number}</c>. The wire
/// field names are the underlying carrier lookup's (Twilio-style snake_case:
/// <c>mobile_country_code</c>, <c>mobile_network_code</c>, <c>error_code</c>).
/// </summary>
public sealed class CarrierInfo
{
    /// <summary>Carrier or network name.</summary>
    [JsonPropertyName("name")] public string? Name { get; set; }

    /// <summary>Line type, such as voip, landline or mobile.</summary>
    [JsonPropertyName("type")] public string? Type { get; set; }

    /// <summary>Mobile country code, on mobile numbers.</summary>
    [JsonPropertyName("mobile_country_code")] public string? MobileCountryCode { get; set; }

    /// <summary>Mobile network code, on mobile numbers.</summary>
    [JsonPropertyName("mobile_network_code")] public string? MobileNetworkCode { get; set; }

    /// <summary>Error code when carrier details could not be determined.</summary>
    [JsonPropertyName("error_code")] public int? ErrorCode { get; set; }
}

/// <summary>
/// Response data for <c>GET /v2.2/carrier/{number}</c>. Billed per lookup at the
/// account's CNAM rate, only when a result is returned.
/// </summary>
public sealed class CarrierData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("carrier")] public CarrierInfo? Carrier { get; set; }
}

/// <summary>LRN dip result, nested inside <see cref="LrnLookupData"/>.</summary>
public sealed class LrnData
{
    [JsonPropertyName("lrn")] public string? Lrn { get; set; }
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("rc")] public string? Rc { get; set; }
    [JsonPropertyName("lata")] public string? Lata { get; set; }
    [JsonPropertyName("ocn")] public string? Ocn { get; set; }
    [JsonPropertyName("lec")] public string? Lec { get; set; }
    [JsonPropertyName("lecType")] public string? LecType { get; set; }
    [JsonPropertyName("jurisdiction")] public string? Jurisdiction { get; set; }
    [JsonPropertyName("local")] public string? Local { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/lrn/{number}/{ani}</c>.</summary>
public sealed class LrnLookupData
{
    [JsonPropertyName("ani")] public string Ani { get; set; } = string.Empty;
    [JsonPropertyName("destination")] public string Destination { get; set; } = string.Empty;
    [JsonPropertyName("lrn")] public LrnData Lrn { get; set; } = new();
}
