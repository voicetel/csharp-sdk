using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>
/// One document to send with a fax: file name, raw bytes and media type.
/// The server validates content (magic bytes), not the extension — PDF, DOC
/// and DOCX are accepted, at most 8 MiB each and 8 MiB per fax.
/// </summary>
public sealed class FaxDocument
{
    /// <param name="name">File name, e.g. <c>contract.pdf</c>.</param>
    /// <param name="content">Raw document bytes.</param>
    /// <param name="contentType">Media type; defaults to <c>application/pdf</c>.</param>
    public FaxDocument(string name, byte[] content, string contentType = "application/pdf")
    {
        Name = name;
        Content = content;
        ContentType = contentType;
    }

    /// <summary>File name sent in the part's Content-Disposition.</summary>
    public string Name { get; }

    /// <summary>Raw document bytes.</summary>
    public byte[] Content { get; }

    /// <summary>Media type sent as the part's Content-Type.</summary>
    public string ContentType { get; }

    /// <summary>A PDF document with the standard media type.</summary>
    public static FaxDocument Pdf(string name, byte[] content) => new(name, content, "application/pdf");
}

/// <summary>Sending faxes from the account's fax numbers and reading their history and documents.</summary>
public sealed class FaxService
{
    private readonly Transport _t;
    internal FaxService(Transport t) => _t = t;

    /// <summary>
    /// Every fax-capable number on the account, with the email address each
    /// delivers to (DestinationId 0 = delivers to the account email).
    /// </summary>
    public Task<FaxNumbersData> ListNumbersAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<FaxNumbersData>(HttpMethod.Get, "/v2.2/numbers/fax", null, null, requireAuth: true, cancellationToken);

    /// <summary>
    /// Submit a fax for delivery (multipart form, HTTP 201, status queued).
    /// <paramref name="idempotencyKey"/> is a client-generated UUID:
    /// resubmitting the same documents with the same key returns the original
    /// job instead of sending twice. It is generated when omitted — pass your
    /// own to make retries safe across process restarts. Rate-limited to 10
    /// sends per minute per account (429 with <c>Retry-After: 60</c>).
    /// </summary>
    public Task<FaxSendData> SendAsync(
        string fromNumber,
        string toNumber,
        IReadOnlyList<FaxDocument> documents,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        var key = string.IsNullOrEmpty(idempotencyKey) ? Guid.NewGuid().ToString() : idempotencyKey;
        var files = new List<MultipartFormData.FilePart>(documents.Count);
        foreach (var document in documents)
        {
            files.Add(new MultipartFormData.FilePart("documents", document.Name, document.ContentType, document.Content));
        }

        var (contentType, body) = MultipartFormData.Build(
            new[] { ("fromNumber", fromNumber), ("toNumber", toNumber) },
            files,
            MultipartFormData.RandomBoundary());
        return _t.RequestWithRawBodyAsync<FaxSendData>(
            HttpMethod.Post, "/v2.2/faxes", null, contentType, body, key, requireAuth: true, cancellationToken);
    }

    /// <summary>
    /// One page of fax history for a number, newest first.
    /// <see cref="FaxHistoryOptions.Number"/> is required; From/To are unix
    /// seconds (From is the newer edge, defaulting to the last 24 hours).
    /// </summary>
    public Task<FaxHistoryData> HistoryAsync(FaxHistoryOptions options, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("number", options.Number);
        q.AddInt("from", options.From);
        q.AddInt("to", options.To);
        q.AddInt("limit", options.Limit);
        q.Add("cursor", options.Cursor);
        q.Add("status", options.Status);
        return _t.RequestAsync<FaxHistoryData>(HttpMethod.Get, "/v2.2/faxes", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>One fax job — poll until status is <c>sent</c> or <c>failed</c>.</summary>
    public Task<FaxJob> GetAsync(string id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<FaxJob>(HttpMethod.Get, "/v2.2/faxes/" + Uri.EscapeDataString(id), null, null, requireAuth: true, cancellationToken);

    /// <summary>
    /// The PDF rendering of a fax as raw bytes (an attachment). Available
    /// when the job reports <see cref="FaxJob.DocumentAvailable"/> true.
    /// </summary>
    public Task<byte[]> DownloadDocumentAsync(string id, CancellationToken cancellationToken = default) =>
        _t.RequestBytesAsync(HttpMethod.Get, "/v2.2/faxes/" + Uri.EscapeDataString(id) + "/document", null, requireAuth: true, cancellationToken);
}
