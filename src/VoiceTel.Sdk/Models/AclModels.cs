using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/acl</c> (add) and <c>DELETE /v2.2/acl</c> (remove).</summary>
public sealed class AclModifyRequest
{
    [JsonPropertyName("acl")] public List<CidrEntry> Acl { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/acl</c>.</summary>
public sealed class AclListData
{
    [JsonPropertyName("acl")] public List<CidrEntry> Acl { get; set; } = new();
}

/// <summary>Response data for <c>POST /v2.2/acl</c>.</summary>
public sealed class AclAddData
{
    [JsonPropertyName("added")] public List<CidrEntry> Added { get; set; } = new();
}

/// <summary>Response data for <c>DELETE /v2.2/acl</c>.</summary>
public sealed class AclRemoveData
{
    [JsonPropertyName("removed")] public List<CidrEntry> Removed { get; set; } = new();
}

/// <summary>One CIDR row that the server rejected, with reason.</summary>
public sealed class AclFailedEntry
{
    [JsonPropertyName("cidr")] public string Cidr { get; set; } = string.Empty;
    [JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
}

/// <summary>Data payload inside a 409 from <c>POST/DELETE /v2.2/acl</c>.</summary>
public sealed class AclConflictData
{
    [JsonPropertyName("added")] public List<CidrEntry>? Added { get; set; }
    [JsonPropertyName("removed")] public List<CidrEntry>? Removed { get; set; }
    [JsonPropertyName("failed")] public List<AclFailedEntry>? Failed { get; set; }
}
