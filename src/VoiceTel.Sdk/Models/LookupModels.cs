using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Response data for <c>GET /v2.2/cnam/{number}</c>.</summary>
public sealed class CnamData
{
    [JsonPropertyName("cnam")] public string? Cnam { get; set; }
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
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
