using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>One fax-capable number on the account (GET /v2.2/numbers/fax).</summary>
public sealed class FaxNumberEntry
{
    /// <summary>The fax number, ten digits.</summary>
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    /// <summary>Per-number fax destination row id, or 0 when it delivers to the account email.</summary>
    [JsonPropertyName("destinationId")] public int DestinationId { get; set; }
    /// <summary>Email address faxes to this number deliver to.</summary>
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
}

/// <summary>Response data for <c>GET /v2.2/numbers/fax</c>.</summary>
public sealed class FaxNumbersData
{
    [JsonPropertyName("numbers")] public List<FaxNumberEntry> Numbers { get; set; } = new();
}

/// <summary>Metadata of one document attached to a fax.</summary>
public sealed class FaxDocumentMeta
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("size")] public int Size { get; set; }
    [JsonPropertyName("contentType")] public string ContentType { get; set; } = string.Empty;
}

/// <summary>Response data for <c>POST /v2.2/faxes</c> — the accepted, queued job.</summary>
public sealed class FaxSendData
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("fromNumber")] public string FromNumber { get; set; } = string.Empty;
    [JsonPropertyName("toNumber")] public string ToNumber { get; set; } = string.Empty;
    /// <summary>Job state at acceptance: <c>queued</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("documents")] public List<FaxDocumentMeta> Documents { get; set; } = new();
}

/// <summary>
/// One fax job: routing, lifecycle timestamps and documents. Poll
/// <c>GET /v2.2/faxes/{id}</c> until <see cref="Status"/> is <c>sent</c> or
/// <c>failed</c>; <see cref="DocumentAvailable"/> tells whether
/// <c>DownloadDocumentAsync</c> can serve the PDF.
/// </summary>
public sealed class FaxJob
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("fromNumber")] public string FromNumber { get; set; } = string.Empty;
    [JsonPropertyName("toNumber")] public string ToNumber { get; set; } = string.Empty;
    /// <summary>The Idempotency-Key the fax was submitted with, or null.</summary>
    [JsonPropertyName("clientToken")] public string? ClientToken { get; set; }
    /// <summary><c>queued</c>, <c>sending</c>, <c>retry_pending</c>, <c>sent</c> or <c>failed</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    /// <summary>Acceptance time, ISO-8601 UTC.</summary>
    [JsonPropertyName("submittedAt")] public string? SubmittedAt { get; set; }
    /// <summary>When delivery was first attempted, ISO-8601 UTC, or null.</summary>
    [JsonPropertyName("startedAt")] public string? StartedAt { get; set; }
    /// <summary>When the job reached sent or failed, ISO-8601 UTC, or null while active.</summary>
    [JsonPropertyName("completedAt")] public string? CompletedAt { get; set; }
    /// <summary>Delivery attempts so far, or null while queued.</summary>
    [JsonPropertyName("attempts")] public int? Attempts { get; set; }
    /// <summary>Why the fax failed, on failed jobs; null otherwise.</summary>
    [JsonPropertyName("failureReason")] public string? FailureReason { get; set; }
    [JsonPropertyName("documents")] public List<FaxDocumentMeta> Documents { get; set; } = new();
    /// <summary>Whether <c>GET /v2.2/faxes/{id}/document</c> can serve the PDF.</summary>
    [JsonPropertyName("documentAvailable")] public bool DocumentAvailable { get; set; }
    /// <summary>File name the document downloads as.</summary>
    [JsonPropertyName("documentName")] public string DocumentName { get; set; } = string.Empty;
    /// <summary>Total document size in bytes, or null when there are no documents.</summary>
    [JsonPropertyName("documentSize")] public int? DocumentSize { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/faxes</c> — one page of history, newest first.</summary>
public sealed class FaxHistoryData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    /// <summary>Newer edge of the returned range, unix seconds.</summary>
    [JsonPropertyName("fromTs")] public long? FromTs { get; set; }
    /// <summary>Older edge of the returned range, unix seconds.</summary>
    [JsonPropertyName("toTs")] public long? ToTs { get; set; }
    [JsonPropertyName("faxes")] public List<FaxJob> Faxes { get; set; } = new();
    /// <summary>Cursor for the next page, or null when the page is the last.</summary>
    [JsonPropertyName("nextCursor")] public string? NextCursor { get; set; }
}

/// <summary>Query for <see cref="Resources.FaxService.HistoryAsync"/> — mirrors <see cref="HistoryOptions"/>.</summary>
public sealed class FaxHistoryOptions
{
    /// <summary>Required. A fax number on the account whose history to read.</summary>
    public string Number { get; set; } = string.Empty;
    /// <summary>Newer edge of the range, unix seconds. Defaults to now.</summary>
    public long? From { get; set; }
    /// <summary>Older edge of the range, unix seconds. Defaults to 24 hours ago.</summary>
    public long? To { get; set; }
    /// <summary>Page size, 1 to 100 (default 50).</summary>
    public int? Limit { get; set; }
    /// <summary>Paging cursor from a previous response NextCursor.</summary>
    public string? Cursor { get; set; }
    /// <summary>Filter by job state: all, active, queued, sending, retry_pending, sent or failed.</summary>
    public string? Status { get; set; }
}
