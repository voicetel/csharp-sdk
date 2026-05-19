using System.Text.Json.Serialization;

namespace VoiceTel.Sdk;

/// <summary>
/// A single row in the IP allowlist.
/// Mask must be /8, /16, /24, or /32 and must describe a routable public address.
/// </summary>
public sealed class CidrEntry
{
    /// <summary>The CIDR block, e.g. <c>"203.0.113.0/24"</c>.</summary>
    [JsonPropertyName("cidr")]
    public string Cidr { get; set; } = string.Empty;
}
