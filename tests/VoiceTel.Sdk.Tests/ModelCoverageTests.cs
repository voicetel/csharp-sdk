using System.Collections.Generic;
using System.Text.Json;
using VoiceTel.Sdk.Models;
using Xunit;

namespace VoiceTel.Sdk.Tests;

/// <summary>
/// Round-trip every public model through System.Text.Json to make sure all
/// property setters/getters are exercised. This both guards against subtle
/// attribute regressions and lifts coverage of the model files into the 90%+
/// band our CI quality bar expects.
/// </summary>
public class ModelCoverageTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private static void Roundtrip<T>(T value) where T : class
    {
        var json = JsonSerializer.Serialize(value, Options);
        var back = JsonSerializer.Deserialize<T>(json, Options);
        Assert.NotNull(back);
        var json2 = JsonSerializer.Serialize(back, Options);
        Assert.Equal(json, json2);
    }

    [Fact]
    public void AccountModels_Roundtrip()
    {
        Roundtrip(new AccountRates { Cnam = 1, IntlMax = 2, Nibble = 3, Lrn = 4, Fax = 5, TfAdj = 6, Did = 7, Mms = 8, Sms = 9 });
        Roundtrip(new AccountServices { E911 = true, Cnam = true, BypassMedia = true, Intl = true, Rcid = true, Mms = true, Dialer = true, Sms = true });
        Roundtrip(new AccountData
        {
            Username = "u", Name = "n", Email = "e", Enabled = true, Created = "c", Cash = 1, CallerId = "ci",
            Timezone = "tz", AuthType = 1, Ccs = 2, Notify = true, NotifyThreshold = 3,
            Rates = new AccountRates(), Services = new AccountServices(),
        });
        Roundtrip(new CreditEntry { Date = "d", Paid = true, Amount = 1 });
        Roundtrip(new PaymentEntry { TransactionId = "t", Date = "d", PayerEmail = "p", Status = "s", Amount = 1 });
        Roundtrip(new CdrEntryValue { Dur = "1", Dst = "d", Ba = "b", Nr = "n", Cn = "c", Ip = "i", Cid = "c" });
        Roundtrip(new CdrEntry { Id = "i", Key = new List<string> { "a", "b" }, Value = new CdrEntryValue() });
        Roundtrip(new AccountCdrData { Cdr = new List<CdrEntry>(), Start = 1, End = 2 });
        Roundtrip(new AccountCreditsData());
        Roundtrip(new AccountPaymentsData());
        Roundtrip(new MrcCharge { Amount = 1, Description = "d" });
        Roundtrip(new AccountMrcData { Total = 1 });
        Roundtrip(new AccountRegistrationData { Agent = "a", Uri = "u", Expires = 1 });
        Roundtrip(new AccountAddRequest { Username = 1, Name = "n", Email = "e", MasterAccount = 2 });
        Roundtrip(new AccountAddData { Username = "u", Name = "n", Email = "e", MasterAccount = "m", Password = "p" });
        Roundtrip(new AccountPutRequest { Notify = true, NotifyThreshold = 1, Timezone = "tz", CallerId = "ci", E911 = true, Intl = true, Sms = true, Mms = true, Ccs = 1 });
        Roundtrip(new AccountPutData { Updated = new List<string> { "x" } });
        Roundtrip(new AccountSignupRequest { Name = "n", Email = "e", Promo = "p" });
        Roundtrip(new AccountSignupData { Username = "u", Name = "n", Email = "e", Password = "p" });
        Roundtrip(new AccountRecoverRequest { Email = "e" });
        Roundtrip(new AccountRecoverData { Message = "m" });
        Roundtrip(new AccountApiKeyData { ApiKey = "k" });
    }

    [Fact]
    public void AclAndAuth_Roundtrip()
    {
        Roundtrip(new AclModifyRequest { Acl = new List<CidrEntry> { new() { Cidr = "x" } } });
        Roundtrip(new AclListData());
        Roundtrip(new AclAddData());
        Roundtrip(new AclRemoveData());
        Roundtrip(new AclFailedEntry { Cidr = "c", Reason = "r" });
        Roundtrip(new AclConflictData { Added = new(), Removed = new(), Failed = new() });

        Roundtrip(new AuthPutRequest { AuthType = 1, Password = "p" });
        Roundtrip(new AuthGetData { AuthType = 1, AuthTypeDescription = "d", Acl = new() });
        Roundtrip(new AuthUpdatedEntry { Field = "f", Value = 1 });
        Roundtrip(new AuthPutData { Updated = new() });
        Roundtrip(new AuthPutConflictData { Updated = new() });
        Assert.Equal(0, AuthTypes.Digest);
        Assert.Equal(1, AuthTypes.IpAuth);
        Assert.Equal(2, AuthTypes.DigestOrIp);
        Assert.Equal(3, AuthTypes.DigestAndIp);
    }

    [Fact]
    public void E911AndGateway_Roundtrip()
    {
        Roundtrip(new E911AddressRequest { Address1 = "a", Address2 = "b", City = "c", State = "s", Zip = "z" });
        Roundtrip(new E911CreateRequest { Dn = "d", Callername = "c", Address1 = "a", Address2 = "b", City = "c", State = "s", Zip = "z" });
        Roundtrip(new E911ProvisionByIdRequest { Callername = "c", AddressId = 1 });
        Roundtrip(new E911Entry { Dn = "d", Callername = "c", Address1 = "a", Address2 = "b", City = "c", State = "s", Zip = "z" });
        Roundtrip(new E911ValidatedAddress { AddressId = 1, Address1 = "a", Address2 = "b", City = "c", State = "s", Zip = "z" });
        Roundtrip(new E911AllData());
        Roundtrip(new E911RecordData());
        Roundtrip(new E911ValidateData());

        Roundtrip(new GatewayAddRequest { Gateway = "g", Prefix = "p", Limit = 1 });
        Roundtrip(new GatewayUpdateRequest { Gateway = "g", Prefix = "p", Limit = 1 });
        Roundtrip(new GatewayEntry { Id = 1, Gateway = "g", Prefix = "p", Limit = 1, System = true });
        Roundtrip(new GatewayNumberSummary { Number = "n", Translated = "t", Forward = true, ForwardTo = "f", Cnam = true, Carrier = 1, SmsEnabled = true, FaxEnabled = true });
        Roundtrip(new GatewaysListData());
        Roundtrip(new GatewayNumbersData());
    }

    [Fact]
    public void INumbering_Roundtrip()
    {
        Roundtrip(new OrderCreateRequest { Numbers = new() { new("2015551234"), new(new OrderNumberSpec { Number = "2015551235", Route = 4 }) } });
        Roundtrip(new OrderNumberSpec { Number = "n", Route = 4 });
        Roundtrip(new PortFeatureLidb { Name = "n" });
        Roundtrip(new PortFeatureRouting { GatewayId = 1 });
        Roundtrip(new PortFeatureSms { CampaignId = "c" });
        Roundtrip(new PortFeature { Number = "n", Routing = new(), Lidb = new(), Sms = new() });
        Roundtrip(new PortSubmitRequest
        {
            Did = new() { "n" }, Name = "n", NameType = "business", LcBtn = "l", LcAccountNumber = "a",
            StreetNumber = "1", Street = "s", StreetType = "ST", City = "c", State = "s", Zip = "z", Country = "US", AuthPerson = "p",
            StreetPrefix = "N", StreetSuffix = "S", Floor = "1", Room = "2", Building = "B", UnitValue = "U",
            DesiredDueDate = "2026-01-01", Pin = "1234", Features = new(),
        });
        Roundtrip(new InventoryItem { Number = "n", RateCenter = "r", City = "c", Province = "p", Lata = "l" });
        Roundtrip(new InventoryCoverageItem { Count = 1, Npa = "201", Nxx = "555", Block = "1", City = "c", RcAbbre = "r", Lata = "l", LocState = "s" });
        Roundtrip(new PortSummary { Status = "s", Id = "i", Pid = "p", Foc = "f", CreatedAt = "c", Message = "m", SupportUrl = "u" });
        Roundtrip(new PortDetail { Status = "s", Id = "i", Pid = "p", Name = "n", Email = "e", Foc = "f", CreatedAt = "c", Numbers = new() { "n" }, Message = "m" });
        Roundtrip(new InventorySearchData());
        Roundtrip(new InventoryCoverageData());
        Roundtrip(new OrderFailedEntry { Number = "n", Reason = "r" });
        Roundtrip(new OrderCreateData { OrderId = "o", AmountCharged = 1, NumbersOrdered = new() { "n" }, Failed = new() });
        Roundtrip(new PortListData());
        Roundtrip(new PortDetailData());
        Roundtrip(new PortSubmitData { Pid = "p", Ticket = 1, Message = "m", LoaUrl = "l", PortUrl = "p" });
        Roundtrip(new PortAvailabilityData { Number = "n", Portable = true, LosingCarrier = "c", LocalRoutingNumber = "l", RateCenterTier = "A", Reason = "r" });

        // Touch the query POCOs
        var iq = new InventoryQuery { Npa = 1, Nxx = 2, State = "s", RateCenter = "r", Contains = "c", EndsWith = "e", Limit = 1 };
        Assert.Equal(1, iq.Npa);
        Assert.Equal(2, iq.Nxx);
        Assert.Equal("s", iq.State);
        Assert.Equal("r", iq.RateCenter);
        Assert.Equal("c", iq.Contains);
        Assert.Equal("e", iq.EndsWith);
        Assert.Equal(1, iq.Limit);
        var cq = new CoverageQuery { State = "s", RateCenter = "r" };
        Assert.Equal("s", cq.State);
        Assert.Equal("r", cq.RateCenter);

        var hopts = new HistoryOptions { Number = "n", Start = 1, End = 2, Type = "sms" };
        Assert.Equal("n", hopts.Number);
        Assert.Equal(1, hopts.Start);
        Assert.Equal(2, hopts.End);
        Assert.Equal("sms", hopts.Type);
    }

    [Fact]
    public void Lookups_Roundtrip()
    {
        Roundtrip(new CnamData { Cnam = "c", Number = "n" });
        Roundtrip(new LrnData { Lrn = "l", State = "s", City = "c", Rc = "r", Lata = "l", Ocn = "o", Lec = "l", LecType = "t", Jurisdiction = "j", Local = "Y" });
        Roundtrip(new LrnLookupData { Ani = "a", Destination = "d", Lrn = new LrnData() });
    }

    [Fact]
    public void Messaging_Roundtrip()
    {
        Roundtrip(new MessageSendRequest { FromNumber = "f", ToNumber = "t", Text = "x", Subject = "s", MediaUrls = new() { "u" } });
        Roundtrip(new MessagingBrandCreateRequest { MessagingBrandId = "B", MessagingBrandName = "n", MessagingBrandDescription = "d" });
        Roundtrip(new MessagingCampaignCreateRequest { MessagingBrandId = "B", ExternalCampaignId = "e", CampaignDescription = "d", CampaignClassName = "c", CampaignStartDate = "s" });
        Roundtrip(new MessageRecordValue { SourceNumber = "s", DestinationNumber = "d", Direction = "in", Rate = "0.01", Number = 1, Message = "m" });
        Roundtrip(new MessageRecord { Id = "i", Key = new(), Value = new() });
        Roundtrip(new MessageHistoryData { Number = "n", Type = "sms", FromTs = 1, ToTs = 2, Messages = new() });
        Roundtrip(new MessageSendData { Id = "i", Type = "sms", FromNumber = "f", ToNumber = "t", Parts = 1, Subject = "s", MediaUrls = new() { "u" } });
        Roundtrip(new BrandRegistrationResult { StatusCode = "200", Status = "Success" });
        Roundtrip(new MessagingBrandCreateData { Result = new() });
        Roundtrip(new CampaignRegistrationResult { StatusCode = "200", Status = "Success" });
        Roundtrip(new MessagingCampaignCreateData { Result = new() });
        Roundtrip(new CampaignStatusItem { Id = "i", Status = "ACTIVE", Numbers = new() { "n" } });
        Roundtrip(new MessagingCampaignStatusData());
    }

    [Fact]
    public void Numbers_Roundtrip()
    {
        Roundtrip(new NumberAddRequest { Number = "n", Route = 4 });
        Roundtrip(new NumberRouteRequest { Route = 4 });
        Roundtrip(new NumberCnamRequest { Enabled = true });
        Roundtrip(new NumberLidbRequest { Cnam = "x", CustomerOrderReference = "r" });
        Roundtrip(new NumberFaxRequest { Email = "e" });
        Roundtrip(new NumberForwardRequest { Destination = 1 });
        Roundtrip(new NumberTranslationRequest { Translation = "1" });
        Roundtrip(new NumberSmsRequest { Type = "email", Resource = "r" });
        Roundtrip(new NumberMessagingPatchRequest { RouteIn = 1, RouteOut = 2 });
        Roundtrip(new NumberCampaignAssignRequest { CampaignId = "c" });
        Roundtrip(new NumberMoveRequest { AccountId = 1, Route = 4 });
        Roundtrip(new PortOutPinUpdateRequest { Pin = "1234" });
        Roundtrip(new BulkUnassignRequest { Numbers = new() { "n" } });
        Roundtrip(new NumberDetail { Number = "n", Translated = "t", Route = 4, Gateway = "g", Cnam = true, Forward = true, ForwardTo = "f", Carrier = 1, SmsEnabled = true, FaxEnabled = true });
        Roundtrip(new CampaignBinding { Id = "i", Network = "A", Status = "ACTIVE", UpstreamCnpId = "u" });
        Roundtrip(new NumberMessagingState { Number = "n", OnAccount = true, Enabled = true, Carrier = 1, RouteIn = 1, Resource = "r", Network = "A", Campaign = new() });
        Roundtrip(new NumberAddData { Number = "n", Route = 4 });
        Roundtrip(new NumberCnamData { Number = "n", Cnam = true });
        Roundtrip(new NumberFaxData { Number = "n", Email = "e" });
        Roundtrip(new NumberForwardData { Number = "n", ForwardTo = "f" });
        Roundtrip(new NumberLidbData { Number = "n", Cnam = "x", CustomerOrderReference = "r", CarrierStatus = "Success" });
        Roundtrip(new NumberMessagingPatchData { Number = "n", Updated = new() { "routeIn" } });
        Roundtrip(new NumberMoveData { Number = "n", AccountId = 1, Route = 4 });
        Roundtrip(new NumberRouteData { Number = "n", Route = 4 });
        Roundtrip(new NumberSmsData { Number = "n", Type = "email", Resource = "r" });
        Roundtrip(new NumberTranslationData { Number = "n", Translation = "1" });
        Roundtrip(new NumberMessagingCampaignAssignData { Number = "n", CampaignId = "c", Carrier = 17, Network = "A", UpstreamCnpId = "u", PreviousNetwork = "B", PreviousNetworkCleared = true });
        Roundtrip(new NumberMessagingCampaignUnassignData { Number = "n", CampaignId = "c", Network = "A", UpstreamCnpId = "u", Unassigned = true });
        Roundtrip(new CampaignUnassignFailure { Number = "n", Reason = "r" });
        Roundtrip(new NumbersMessagingCampaignUnassignData { CampaignId = "c", Network = "A", UpstreamCnpId = "u", UnassignedNumbers = new() { "n" }, Failed = new() });
        Roundtrip(new NumbersListData());
        Roundtrip(new NumbersMessagingListData());
        Roundtrip(new PortOutPinUpdateData { Number = "n", PortOutPin = "1234" });
    }

    [Fact]
    public void Support_Roundtrip()
    {
        Roundtrip(new TicketCreateRequest { Subject = "s", Message = "m", Email = "e" });
        Roundtrip(new TicketUpdateRequest { Status = "closed" });
        Roundtrip(new TicketReplyRequest { Message = "m" });
        Roundtrip(new TicketSource { Via = "v", Type = "t" });
        Roundtrip(new TicketAction { Text = "x", Type = "t" });
        Roundtrip(new TicketActor { Id = 1, Type = "user", Email = "e", FirstName = "f", LastName = "l", PhotoUrl = "p" });
        Roundtrip(new CustomFieldValue { Id = 1, Value = "v", Text = "t" });
        Roundtrip(new CustomerContactEntry { Id = 1, Value = "v", Type = "t" });
        Roundtrip(new CustomerWebsiteEntry { Id = 1, Value = "v" });
        Roundtrip(new CustomerAddress { Street = "s", City = "c", State = "S", Country = "US", Zip = "z" });
        Roundtrip(new CustomerEmbedded { Address = new(), Emails = new(), Phones = new(), SocialProfiles = new(), Websites = new() });
        Roundtrip(new SupportAttachment { Id = 1, MimeType = "m", FileName = "f", FileUrl = "u", Size = 1 });
        Roundtrip(new ThreadEmbedded { Attachments = new() });
        Roundtrip(new ConversationEmbedded { Threads = new() });
        Roundtrip(new SupportCustomer { Id = 1, FirstName = "f", LastName = "l", Email = "e", Company = "c", JobTitle = "j", PhotoType = "p", PhotoUrl = "u", Notes = "n", Type = "customer", CreatedAt = "c", UpdatedAt = "u", Embedded = new() });
        Roundtrip(new SupportThread
        {
            Id = 1, Status = "active", State = "s", Type = "message", Body = "b", Rating = 5, RatingComment = "r",
            OpenedAt = "o", CreatedAt = "c", Source = new(), Action = new(), CreatedBy = new(), AssignedTo = new(),
            Customer = new(), To = new() { "t" }, Cc = new() { "c" }, Bcc = new() { "b" }, Embedded = new(),
        });
        Roundtrip(new SupportConversation
        {
            Id = 1, TicketNumber = 1015, Status = "active", State = "s", Subject = "sub", Preview = "p", Type = "t",
            MailboxId = 1, FolderId = 1, ThreadsCount = 1, ClosedBy = 1, ClosedAt = "c", CreatedAt = "c", UpdatedAt = "u", UserUpdatedAt = "u",
            CustomerWaitingSince = new(), Source = new(), CreatedBy = new(), Assignee = new(), ClosedByUser = new(),
            Customer = new(), Cc = new(), Bcc = new(), CustomFields = new(), Embedded = new(),
        });
        Roundtrip(new TicketData { Ticket = new() });
        Roundtrip(new TicketsListData());
        Roundtrip(new TicketThreadsData());
        Roundtrip(new TicketReplyData { Message = "Reply added" });
        Roundtrip(new TicketUpdateData { Id = 1, Status = "success" });
    }

    [Fact]
    public void Common_AndSdkInfo()
    {
        Roundtrip(new CidrEntry { Cidr = "1.1.1.1/32" });
        Assert.Equal("2.2.10", SdkInfo.SdkVersion);
        Assert.Equal("v2.2.10", SdkInfo.ApiVersion);
        Assert.Equal("https://api.voicetel.com", SdkInfo.DefaultBaseUrl);
        Assert.Contains("voicetel-csharp/", SdkInfo.DefaultUserAgent);
    }

    [Fact]
    public void SupportConversation_TicketNumberFieldMapsToWireField()
    {
        // The spec uses `number` for the ticket sequence; verify mapping.
        var json = "{\"number\":1015,\"status\":\"active\"}";
        var c = JsonSerializer.Deserialize<SupportConversation>(json);
        Assert.NotNull(c);
        Assert.Equal(1015, c!.TicketNumber);
        Assert.Equal("active", c.Status);

        var serialized = JsonSerializer.Serialize(c);
        Assert.Contains("\"number\":1015", serialized);
    }

    [Fact]
    public void PortAvailability_NewFields_DeserializeCorrectly()
    {
        var json = "{\"number\":\"2015551234\",\"portable\":true,\"losingCarrier\":\"X\",\"localRoutingNumber\":\"2015550000\",\"rateCenterTier\":\"A\",\"reason\":null}";
        var pa = JsonSerializer.Deserialize<PortAvailabilityData>(json);
        Assert.NotNull(pa);
        Assert.Equal("2015550000", pa!.LocalRoutingNumber);
        Assert.Equal("A", pa.RateCenterTier);
        Assert.Null(pa.Reason);
    }
}
