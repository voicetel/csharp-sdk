using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>SMS/MMS sending and 10DLC brand/campaign registration.</summary>
public sealed class MessagingService
{
    private readonly Transport _t;
    internal MessagingService(Transport t) => _t = t;

    /// <summary>Fetches message history. <see cref="HistoryOptions.Number"/> is required.</summary>
    public Task<MessageHistoryData> HistoryAsync(HistoryOptions options, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("number", options.Number);
        q.AddInt("from", options.From);
        q.AddInt("to", options.To);
        q.Add("type", options.Type);
        return _t.RequestAsync<MessageHistoryData>(HttpMethod.Get, "/v2.2/messages", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Sends an SMS or MMS.</summary>
    public Task<MessageSendData> SendAsync(MessageSendRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessageSendData>(HttpMethod.Post, "/v2.2/messages", null, body, requireAuth: true, cancellationToken);

    /// <summary>Registers a 10DLC brand with the campaign registry.</summary>
    public Task<MessagingBrandCreateData> CreateBrandAsync(MessagingBrandCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessagingBrandCreateData>(HttpMethod.Post, "/v2.2/messaging/brands", null, body, requireAuth: true, cancellationToken);

    /// <summary>Returns the current 10DLC campaign statuses for the given campaign ids (max 100).</summary>
    public Task<MessagingCampaignStatusData> CampaignStatusAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("ids", string.Join(",", ids));
        return _t.RequestAsync<MessagingCampaignStatusData>(HttpMethod.Get, "/v2.2/messaging/campaigns", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Registers a 10DLC campaign with the carrier.</summary>
    public Task<MessagingCampaignCreateData> CreateCampaignAsync(MessagingCampaignCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessagingCampaignCreateData>(HttpMethod.Post, "/v2.2/messaging/campaigns", null, body, requireAuth: true, cancellationToken);

    /// <summary>
    /// Returns the messaging state for the given numbers (comma-separated on the
    /// wire, max 1000). The <c>numbers</c> query parameter is required by the API.
    /// </summary>
    public Task<NumbersMessagingListData> NumbersStateAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("numbers", string.Join(",", numbers));
        return _t.RequestAsync<NumbersMessagingListData>(HttpMethod.Get, "/v2.2/numbers/messaging", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>
    /// Returns the account-wide messaging health overview — the same view as the portal
    /// Messaging tab: summary counts (numbers, messaging on, healthy, needing attention,
    /// in progress), campaigns with carrier-review progress, numbers needing attention
    /// (each with a plain-language title, detail and the actions that fix it), 14 days of
    /// delivery counts, and the alert-email setting. Updated hourly.
    /// </summary>
    /// <remarks>
    /// The API spec defines no response schema for this endpoint, so the decoded JSON
    /// object is returned as a raw <c>Dictionary&lt;string, object?&gt;</c> (nested values
    /// are <see cref="System.Text.Json.JsonElement"/>) rather than a typed model.
    /// </remarks>
    public Task<Dictionary<string, object?>> HealthAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<Dictionary<string, object?>>(HttpMethod.Get, "/v2.2/messaging/health", null, null, requireAuth: true, cancellationToken);

    /// <summary>Returns the alert-email setting: whether new messaging problems are emailed to the account.</summary>
    /// <remarks>The spec defines no response schema, so the decoded JSON object is returned raw.</remarks>
    public Task<Dictionary<string, object?>> HealthAlertsAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<Dictionary<string, object?>>(HttpMethod.Get, "/v2.2/messaging/health/alerts", null, null, requireAuth: true, cancellationToken);

    /// <summary>Turns alert emails on or off. When enabled, the account is emailed as new
    /// messaging problems are found (at most one digest about every 20 hours).</summary>
    /// <remarks>The spec defines no response schema, so the decoded JSON object is returned raw.</remarks>
    public Task<Dictionary<string, object?>> SetHealthAlertsAsync(bool enabled, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<Dictionary<string, object?>>(HttpMethod.Put, "/v2.2/messaging/health/alerts", null, new { enabled }, requireAuth: true, cancellationToken);

    /// <summary>
    /// Performs a live health check of one number: in service, messaging enabled, campaign
    /// approved, assigned to the campaign by the messaging provider, delivery failures in
    /// the last 24 hours, and enablement in progress. Limited to 30 checks per hour per account.
    /// </summary>
    /// <remarks>The spec defines no response schema, so the decoded JSON object is returned raw.</remarks>
    public Task<Dictionary<string, object?>> HealthCheckAsync(string number, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<Dictionary<string, object?>>(HttpMethod.Post, "/v2.2/messaging/health/check", null, new { number }, requireAuth: true, cancellationToken);

    /// <summary>
    /// Returns up to 3 recent outbound messages from the number that carriers blocked as spam
    /// (time, masked recipient, text, reason), to help review content.
    /// </summary>
    /// <remarks>The spec defines no response schema, so the decoded JSON object is returned raw.</remarks>
    public Task<Dictionary<string, object?>> HealthSamplesAsync(string number, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("number", number);
        return _t.RequestAsync<Dictionary<string, object?>>(HttpMethod.Get, "/v2.2/messaging/health/samples", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }
}
