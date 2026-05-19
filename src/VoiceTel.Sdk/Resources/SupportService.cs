using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Manages support tickets (create, read, update, delete, reply).</summary>
public sealed class SupportService
{
    private const string Prefix = "/v2.2/support/tickets";
    private readonly Transport _t;
    internal SupportService(Transport t) => _t = t;

    /// <summary>Returns every ticket on the account.</summary>
    public Task<TicketsListData> ListAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketsListData>(HttpMethod.Get, Prefix, null, null, requireAuth: true, cancellationToken);

    /// <summary>Opens a new support ticket.</summary>
    public Task<TicketData> CreateAsync(TicketCreateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketData>(HttpMethod.Post, Prefix, null, body, requireAuth: true, cancellationToken);

    /// <summary>Fetches one ticket by id.</summary>
    public Task<TicketData> GetAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketData>(HttpMethod.Get, Prefix + "/" + id.ToString(CultureInfo.InvariantCulture), null, null, requireAuth: true, cancellationToken);

    /// <summary>Changes a ticket's status.</summary>
    public Task<TicketUpdateData> UpdateAsync(int id, TicketUpdateRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketUpdateData>(HttpMethod.Put, Prefix + "/" + id.ToString(CultureInfo.InvariantCulture), null, body, requireAuth: true, cancellationToken);

    /// <summary>Removes a ticket. Admin only. Returns on 204 No Content.</summary>
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync(HttpMethod.Delete, Prefix + "/" + id.ToString(CultureInfo.InvariantCulture), null, null, requireAuth: true, cancellationToken);

    /// <summary>Returns every thread (message) on a ticket.</summary>
    public Task<TicketThreadsData> MessagesAsync(int id, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketThreadsData>(HttpMethod.Get, Prefix + "/" + id.ToString(CultureInfo.InvariantCulture) + "/messages", null, null, requireAuth: true, cancellationToken);

    /// <summary>Adds a reply to a ticket.</summary>
    public Task<TicketReplyData> ReplyAsync(int id, TicketReplyRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<TicketReplyData>(HttpMethod.Post, Prefix + "/" + id.ToString(CultureInfo.InvariantCulture) + "/replies", null, body, requireAuth: true, cancellationToken);
}
