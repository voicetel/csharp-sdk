using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/support/tickets</c>.</summary>
public sealed class TicketCreateRequest
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string? Email { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/support/tickets/{id}</c>.</summary>
public sealed class TicketUpdateRequest
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}

/// <summary>Body for <c>POST /v2.2/support/tickets/{id}/replies</c>.</summary>
public sealed class TicketReplyRequest
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}

/// <summary>How a ticket or thread originated.</summary>
public sealed class TicketSource
{
    [JsonPropertyName("via")] public string? Via { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
}

/// <summary>Action descriptor on a thread.</summary>
public sealed class TicketAction
{
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
}

/// <summary>The <c>createdBy</c>/<c>assignee</c>/<c>assignedTo</c>/<c>closedByUser</c> shape.</summary>
public sealed class TicketActor
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("firstName")] public string? FirstName { get; set; }
    [JsonPropertyName("lastName")] public string? LastName { get; set; }
    [JsonPropertyName("photoUrl")] public string? PhotoUrl { get; set; }
}

/// <summary>One custom-field row on a conversation.</summary>
public sealed class CustomFieldValue
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("value")] public string? Value { get; set; }
    [JsonPropertyName("text")] public string? Text { get; set; }
}

/// <summary>An <c>{id, value, type}</c> entry under <c>embedded.emails</c>/<c>phones</c>/<c>socialProfiles</c>.</summary>
public sealed class CustomerContactEntry
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("value")] public string? Value { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
}

/// <summary>An <c>{id, value}</c> entry under <c>embedded.websites</c>.</summary>
public sealed class CustomerWebsiteEntry
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("value")] public string? Value { get; set; }
}

/// <summary>The <c>embedded.address</c> shape on a <see cref="SupportCustomer"/>.</summary>
public sealed class CustomerAddress
{
    [JsonPropertyName("street")] public string? Street { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
    [JsonPropertyName("zip")] public string? Zip { get; set; }
}

/// <summary>The <c>embedded</c> shape on a <see cref="SupportCustomer"/>.</summary>
public sealed class CustomerEmbedded
{
    [JsonPropertyName("address")] public CustomerAddress? Address { get; set; }
    [JsonPropertyName("emails")] public List<CustomerContactEntry>? Emails { get; set; }
    [JsonPropertyName("phones")] public List<CustomerContactEntry>? Phones { get; set; }
    [JsonPropertyName("socialProfiles")] public List<CustomerContactEntry>? SocialProfiles { get; set; }
    [JsonPropertyName("websites")] public List<CustomerWebsiteEntry>? Websites { get; set; }
}

/// <summary>One file attached to a support thread.</summary>
public sealed class SupportAttachment
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("mimeType")] public string? MimeType { get; set; }
    [JsonPropertyName("fileName")] public string? FileName { get; set; }
    [JsonPropertyName("fileUrl")] public string? FileUrl { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
}

/// <summary>The <c>embedded</c> shape on a <see cref="SupportThread"/>.</summary>
public sealed class ThreadEmbedded
{
    [JsonPropertyName("attachments")] public List<SupportAttachment>? Attachments { get; set; }
}

/// <summary>The <c>embedded</c> shape on a <see cref="SupportConversation"/>.</summary>
public sealed class ConversationEmbedded
{
    [JsonPropertyName("threads")] public List<SupportThread>? Threads { get; set; }
}

/// <summary>The end-user profile attached to a support ticket.</summary>
public sealed class SupportCustomer
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("firstName")] public string? FirstName { get; set; }
    [JsonPropertyName("lastName")] public string? LastName { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("company")] public string? Company { get; set; }
    [JsonPropertyName("jobTitle")] public string? JobTitle { get; set; }
    [JsonPropertyName("photoType")] public string? PhotoType { get; set; }
    [JsonPropertyName("photoUrl")] public string? PhotoUrl { get; set; }
    [JsonPropertyName("notes")] public string? Notes { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")] public string? UpdatedAt { get; set; }
    [JsonPropertyName("embedded")] public CustomerEmbedded? Embedded { get; set; }
}

/// <summary>One message in a ticket conversation.</summary>
public sealed class SupportThread
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("body")] public string? Body { get; set; }
    [JsonPropertyName("rating")] public int Rating { get; set; }
    [JsonPropertyName("ratingComment")] public string? RatingComment { get; set; }
    [JsonPropertyName("openedAt")] public string? OpenedAt { get; set; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    [JsonPropertyName("source")] public TicketSource? Source { get; set; }
    [JsonPropertyName("action")] public TicketAction? Action { get; set; }
    [JsonPropertyName("createdBy")] public TicketActor? CreatedBy { get; set; }
    [JsonPropertyName("assignedTo")] public TicketActor? AssignedTo { get; set; }
    [JsonPropertyName("customer")] public SupportCustomer? Customer { get; set; }
    [JsonPropertyName("to")] public List<string>? To { get; set; }
    [JsonPropertyName("cc")] public List<string>? Cc { get; set; }
    [JsonPropertyName("bcc")] public List<string>? Bcc { get; set; }
    [JsonPropertyName("embedded")] public ThreadEmbedded? Embedded { get; set; }
}

/// <summary>
/// A support ticket. Note: the wire field <c>number</c> is a ticket sequence
/// number (1015, 2114, ...), NOT a phone number. It is surfaced here as
/// <see cref="TicketNumber"/> to avoid confusion with 10-digit TNs.
/// </summary>
public sealed class SupportConversation
{
    [JsonPropertyName("id")] public int Id { get; set; }

    /// <summary>Human-readable ticket sequence number (e.g. 1015). Not a phone number.</summary>
    [JsonPropertyName("number")] public int TicketNumber { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("preview")] public string? Preview { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("mailboxId")] public int MailboxId { get; set; }
    [JsonPropertyName("folderId")] public int FolderId { get; set; }
    [JsonPropertyName("threadsCount")] public int ThreadsCount { get; set; }
    [JsonPropertyName("closedBy")] public int ClosedBy { get; set; }
    [JsonPropertyName("closedAt")] public string? ClosedAt { get; set; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")] public string? UpdatedAt { get; set; }
    [JsonPropertyName("userUpdatedAt")] public string? UserUpdatedAt { get; set; }
    [JsonPropertyName("customerWaitingSince")] public Dictionary<string, object?>? CustomerWaitingSince { get; set; }
    [JsonPropertyName("source")] public TicketSource? Source { get; set; }
    [JsonPropertyName("createdBy")] public TicketActor? CreatedBy { get; set; }
    [JsonPropertyName("assignee")] public TicketActor? Assignee { get; set; }
    [JsonPropertyName("closedByUser")] public TicketActor? ClosedByUser { get; set; }
    [JsonPropertyName("customer")] public SupportCustomer? Customer { get; set; }
    [JsonPropertyName("cc")] public List<string>? Cc { get; set; }
    [JsonPropertyName("bcc")] public List<string>? Bcc { get; set; }
    [JsonPropertyName("customFields")] public List<CustomFieldValue>? CustomFields { get; set; }
    [JsonPropertyName("embedded")] public ConversationEmbedded? Embedded { get; set; }
}

/// <summary>Response data for <c>GET/POST /v2.2/support/tickets/{...}</c>.</summary>
public sealed class TicketData
{
    [JsonPropertyName("ticket")] public SupportConversation Ticket { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/support/tickets</c>.</summary>
public sealed class TicketsListData
{
    [JsonPropertyName("tickets")] public List<SupportConversation> Tickets { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/support/tickets/{id}/messages</c>.</summary>
public sealed class TicketThreadsData
{
    [JsonPropertyName("messages")] public List<SupportThread> Messages { get; set; } = new();
}

/// <summary>Response data for <c>POST /v2.2/support/tickets/{id}/replies</c>.</summary>
public sealed class TicketReplyData
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}

/// <summary>Response data for <c>PUT /v2.2/support/tickets/{id}</c>.</summary>
public sealed class TicketUpdateData
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}
