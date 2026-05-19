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

    /// <summary>Fetches message history. Pass <c>new HistoryOptions()</c> for defaults.</summary>
    public Task<MessageHistoryData> HistoryAsync(HistoryOptions options, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.Add("number", options.Number);
        q.AddInt("start", options.Start);
        q.AddInt("end", options.End);
        q.Add("type", options.Type);
        return _t.RequestAsync<MessageHistoryData>(HttpMethod.Get, "/v2.2/messages", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Sends an SMS or MMS.</summary>
    public Task<MessageSendData> SendAsync(MessageSendRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessageSendData>(HttpMethod.Post, "/v2.2/messages", null, body, requireAuth: true, cancellationToken);

    /// <summary>Registers a 10DLC brand with the campaign registry.</summary>
    public Task<MessagingBrandCreateData> CreateBrandAsync(MessagingBrandCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessagingBrandCreateData>(HttpMethod.Post, "/v2.2/messaging/brands", null, body, requireAuth: true, cancellationToken);

    /// <summary>Returns the current 10DLC campaign statuses.</summary>
    public Task<MessagingCampaignStatusData> CampaignStatusAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessagingCampaignStatusData>(HttpMethod.Get, "/v2.2/messaging/campaigns", null, null, requireAuth: true, cancellationToken);

    /// <summary>Registers a 10DLC campaign with the carrier.</summary>
    public Task<MessagingCampaignCreateData> CreateCampaignAsync(MessagingCampaignCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<MessagingCampaignCreateData>(HttpMethod.Post, "/v2.2/messaging/campaigns", null, body, requireAuth: true, cancellationToken);

    /// <summary>
    /// Returns the messaging state for many numbers at once. Pass <c>null</c> or
    /// an empty list for "all numbers on the account".
    /// </summary>
    public Task<NumbersMessagingListData> NumbersStateAsync(IEnumerable<string>? numbers = null, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        if (numbers is not null)
        {
            var joined = string.Join(",", numbers);
            if (!string.IsNullOrEmpty(joined))
            {
                q.Add("numbers", joined);
            }
        }
        return _t.RequestAsync<NumbersMessagingListData>(HttpMethod.Get, "/v2.2/numbers/messaging", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }
}
