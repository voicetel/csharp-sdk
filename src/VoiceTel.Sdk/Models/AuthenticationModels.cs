using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>
/// Authentication-mode constants for SIP/HTTP credentials:
/// 0 = Digest, 1 = Lax (Digest | IP), 2 = IP, 3 = Strict (Digest + IP).
/// </summary>
public static class AuthTypes
{
    /// <summary>SIP Digest auth.</summary>
    public const int Digest = 0;
    /// <summary>Lax — Digest or IP allowlist.</summary>
    public const int Lax = 1;
    /// <summary>IP allowlist only.</summary>
    public const int Ip = 2;
    /// <summary>Strict — Digest and IP allowlist (both required).</summary>
    public const int Strict = 3;
}

/// <summary>Body for <c>PUT /v2.2/auth</c>. <c>null</c> means "leave unchanged".</summary>
public sealed class AuthPutRequest
{
    [JsonPropertyName("authType")] public int? AuthType { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/auth</c>.</summary>
public sealed class AuthGetData
{
    [JsonPropertyName("authType")] public int AuthType { get; set; }
    [JsonPropertyName("authTypeDescription")] public string AuthTypeDescription { get; set; } = string.Empty;
    [JsonPropertyName("acl")] public List<CidrEntry> Acl { get; set; } = new();
}

/// <summary>One updated field returned by <c>PUT /v2.2/auth</c>.</summary>
public sealed class AuthUpdatedEntry
{
    [JsonPropertyName("field")] public string Field { get; set; } = string.Empty;
    [JsonPropertyName("value")] public int Value { get; set; }
}

/// <summary>Response data for <c>PUT /v2.2/auth</c>.</summary>
public sealed class AuthPutData
{
    [JsonPropertyName("updated")] public List<AuthUpdatedEntry> Updated { get; set; } = new();
}

/// <summary>Data payload returned in a 409 from <c>PUT /v2.2/auth</c>.</summary>
public sealed class AuthPutConflictData
{
    [JsonPropertyName("updated")] public List<AuthUpdatedEntry>? Updated { get; set; }
}
