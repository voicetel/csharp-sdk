using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;
using Xunit;

namespace VoiceTel.Sdk.Tests;

public class ResourceTests
{
    // ----------------------------------------------------------------- Account ---

    [Fact]
    public async Task Account_Get()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account",
            "{\"username\":\"u\",\"name\":\"n\",\"email\":\"e\",\"enabled\":true,\"cash\":1.0,\"rates\":{\"sms\":0.01},\"services\":{\"sms\":true}}");
        var d = await c.Account.GetAsync();
        Assert.Equal("u", d.Username);
        Assert.True(d.Services!.Sms);
        Assert.Equal(0.01, d.Rates!.Sms);
    }

    [Fact]
    public async Task Account_Update_Add_Signup()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/account", "{\"updated\":[\"timezone\"]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/account", "{\"username\":\"new\"}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/accounts", "{\"username\":\"public\"}");

        var u = await c.Account.UpdateAsync(new AccountPutRequest { Timezone = "UTC" });
        Assert.Contains("timezone", u.Updated);

        var a = await c.Account.AddAsync(new AccountAddRequest { Username = "1000000002", Name = "n", Email = "e" });
        Assert.Equal("new", a.Username);

        var s = await c.Account.SignupAsync(new AccountSignupRequest { Name = "n", Email = "e" });
        Assert.Equal("public", s.Username);
        Assert.Null(h.Captured[2].Request.Headers.Authorization); // signup is public
    }

    [Fact]
    public async Task Account_CdrCreditsMrcPaymentsRegistration()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/cdr", "{\"cdr\":[{\"id\":\"1\",\"key\":[],\"value\":{\"dur\":\"1\"}}],\"start\":0,\"end\":0}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/credits", "{\"credits\":[{\"date\":\"d\",\"paid\":true,\"amount\":1}]}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/recurring-charges", "{\"charges\":[{\"amount\":1,\"description\":\"x\"}],\"total\":1}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/payments", "{\"payments\":[{\"date\":\"d\",\"status\":\"Completed\",\"amount\":1}]}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/account/registration", "{\"agent\":\"a\"}");

        Assert.Single((await c.Account.CdrAsync(0, 0)).Cdr);
        Assert.Single((await c.Account.CreditsAsync("paid")).Credits);
        Assert.Equal(1.0, (await c.Account.RecurringChargesAsync()).Total);
        Assert.Equal("Completed", (await c.Account.PaymentsAsync()).Payments[0].Status);
        Assert.Equal("a", (await c.Account.RegistrationAsync()).Agent);
    }

    [Fact]
    public async Task Account_Recover_NoAuth()
    {
        var handler = new MockHttpHandler();
        var http = new HttpClient(handler);
        using var c = new VoiceTelClient(apiKey: null, baseUrl: "https://x", httpClient: http);
        handler.EnqueueEnvelope(HttpMethod.Post, "/v2.2/account/recovery", "{\"message\":\"ok\"}");
        var r = await c.Account.RecoverAsync(new AccountRecoverRequest { Email = "e" });
        Assert.Equal("ok", r.Message);
        Assert.Null(handler.Captured[0].Request.Headers.Authorization);
    }

    // ------------------------------------------------------------------- ACL ---

    [Fact]
    public async Task Acl_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/acl", "{\"acl\":[{\"cidr\":\"1.1.1.1/32\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/acl", "{\"added\":[{\"cidr\":\"1.1.1.1/32\"}]}");
        h.EnqueueEnvelope(HttpMethod.Delete, "/v2.2/acl", "{\"removed\":[{\"cidr\":\"1.1.1.1/32\"}]}");
        var body = new AclModifyRequest { Acl = new List<CidrEntry> { new() { Cidr = "1.1.1.1/32" } } };
        Assert.Single((await c.Acl.ListAsync()).Acl);
        Assert.Single((await c.Acl.AddAsync(body)).Added);
        Assert.Single((await c.Acl.RemoveAsync(new[] { "1.1.1.1/32" })).Removed);
        var removeUri = h.Captured[2].Request.RequestUri!.ToString();
        Assert.Contains("cidrs=1.1.1.1%2F32", removeUri);
        Assert.Equal(string.Empty, h.Captured[2].Body); // remove takes no body
    }

    [Fact]
    public async Task Acl_ConflictBodyIsPreservedInError()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueJson(HttpMethod.Post, "/v2.2/acl",
            "{\"failed\":[{\"cidr\":\"x\",\"reason\":\"bad\"}]}",
            HttpStatusCode.Conflict);
        var ex = await Assert.ThrowsAsync<ApiError>(() =>
            c.Acl.AddAsync(new AclModifyRequest()));
        Assert.Equal(ErrorKind.Conflict, ex.Kind);
        Assert.NotNull(ex.Body);
    }

    // -------------------------------------------------------- Authentication ---

    [Fact]
    public async Task Authentication_GetUpdate()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/auth", "{\"authType\":1,\"authTypeDescription\":\"Lax (Digest | IP)\",\"acl\":[]}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/auth", "{\"updated\":[{\"field\":\"authType\",\"value\":1}]}");
        var g = await c.Authentication.GetAsync();
        Assert.Equal(1, g.AuthType);
        var u = await c.Authentication.UpdateAsync(new AuthPutRequest { AuthType = AuthTypes.Lax });
        Assert.Single(u.Updated);
    }

    // ------------------------------------------------------------------ E911 ---

    [Fact]
    public async Task E911_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/e911", "{\"records\":[{\"dn\":\"1X\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/e911", "{\"record\":{\"dn\":\"1X\"}}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/e911/validations", "{\"address\":{\"addressid\":7,\"address1\":\"a\",\"city\":\"c\",\"state\":\"NJ\",\"zip\":\"01\"}}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/e911/1234567890", "{\"record\":{\"dn\":\"11234567890\"}}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/e911/1234567890", "{\"record\":{\"dn\":\"11234567890\"}}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/e911/1234567890");

        Assert.Single((await c.E911.ListAsync()).Records);
        await c.E911.CreateAsync(new E911CreateRequest { Dn = "1234567890", Callername = "Test", Address1 = "a", City = "c", State = "NJ", Zip = "01" });
        var v = await c.E911.ValidateAsync(new E911AddressRequest { Address1 = "a", City = "c", State = "NJ", Zip = "01" });
        Assert.Equal(7, v.Address.AddressId);
        await c.E911.GetAsync("1234567890");
        await c.E911.ProvisionAsync("1234567890", new E911ProvisionByIdRequest { Callername = "Test", AddressId = 7 });
        await c.E911.RemoveAsync("1234567890");
    }

    // -------------------------------------------------------------- Gateways ---

    [Fact]
    public async Task Gateways_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/gateways", "{\"gateways\":[{\"id\":1}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/gateways", "{\"id\":2,\"gateway\":\"1.2.3.4\"}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/gateways/2", "{\"id\":2}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/gateways/2", "{\"id\":2,\"prefix\":\"1\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/gateways/2");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/gateways/2/numbers", "{\"numbers\":[]}");

        Assert.Single((await c.Gateways.ListAsync()).Gateways);
        var added = await c.Gateways.AddAsync(new GatewayAddRequest { Gateway = "1.2.3.4" });
        Assert.Equal(2, added.Id);
        await c.Gateways.GetAsync(2);
        await c.Gateways.UpdateAsync(2, new GatewayUpdateRequest { Prefix = "1" });
        await c.Gateways.RemoveAsync(2);
        await c.Gateways.NumbersAsync(2);
    }

    // ------------------------------------------------------------ INumbering ---

    [Fact]
    public async Task INumbering_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/inventory",
            "{\"numbers\":[{\"number\":\"2015550123\",\"rateCenter\":\"r\",\"city\":\"c\",\"province\":\"NJ\",\"lata\":\"l\"}]}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/inventory/coverage",
            "{\"coverage\":[{\"count\":10,\"npa\":\"201\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/orders",
            "{\"orderId\":\"o\",\"amountCharged\":1.0,\"numbersOrdered\":[\"2015550123\"]}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/ports", "{\"ports\":[{\"status\":\"new\"}]}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/ports/y0d36", "{\"port\":{\"status\":\"done\"}}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/ports",
            "{\"pid\":\"ABCDE\",\"ticket\":1,\"message\":\"ok\",\"loaUrl\":\"u\",\"portUrl\":\"v\"}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/ports/availability/2015550123",
            "{\"number\":\"2015550123\",\"portable\":true,\"losingCarrier\":\"X\",\"localRoutingNumber\":\"2015550000\",\"rateCenterTier\":\"A\",\"reason\":null}");

        var inv = await c.INumbering.SearchInventoryAsync(new InventoryQuery { TnMask = "201555*", Quantity = 5, Province = "NJ", RateCenter = "NWRK", Lata = "224" });
        Assert.Single(inv.Numbers);
        var invUri = h.Captured[0].Request.RequestUri!.ToString();
        Assert.Contains("tnMask=201555%2A", invUri);
        Assert.Contains("quantity=5", invUri);
        Assert.Contains("province=NJ", invUri);
        Assert.Contains("rateCenter=NWRK", invUri);
        Assert.Contains("lata=224", invUri);
        var cov = await c.INumbering.CoverageAsync(new CoverageQuery { CountBy = CoverageCountBy.State, LocState = "NJ" });
        Assert.Single(cov.Coverage);
        var covUri = h.Captured[1].Request.RequestUri!.ToString();
        Assert.Contains("countBy=state", covUri);
        Assert.Contains("locState=NJ", covUri);
        var ord = await c.INumbering.OrderAsync(new OrderCreateRequest
        {
            Numbers = new List<OrderNumber> {
                new("2015550123"),
                new(new OrderNumberSpec { Number = "2015551235", Route = 4 }),
            },
        });
        Assert.Equal("o", ord.OrderId);
        await c.INumbering.PortsAsync();
        await c.INumbering.PortAsync("y0d36");
        var p = await c.INumbering.SubmitPortAsync(new PortSubmitRequest
        {
            Did = new List<string> { "2015550123" },
            Name = "n",
            NameType = "business",
            LcBtn = "1",
            LcAccountNumber = "1",
            StreetNumber = "1",
            Street = "Main",
            StreetType = "ST",
            City = "NWK",
            State = "NJ",
            Zip = "07102",
            Country = "US",
            AuthPerson = "X",
        });
        Assert.Equal("ABCDE", p.Pid);

        var pa = await c.INumbering.PortAvailabilityAsync("2015550123");
        Assert.True(pa.Portable);
        Assert.Equal("2015550000", pa.LocalRoutingNumber);
        Assert.Equal("A", pa.RateCenterTier);
    }

    [Fact]
    public async Task INumbering_OrderNumber_StringForm_SerializesAsString()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/orders", "{\"orderId\":\"o\",\"amountCharged\":0,\"numbersOrdered\":[]}");
        await c.INumbering.OrderAsync(new OrderCreateRequest
        {
            Numbers = new List<OrderNumber> { new("2015550123") },
        });
        var body = h.Captured[0].Body;
        Assert.Contains("\"numbers\":[\"2015550123\"]", body);
    }

    [Fact]
    public async Task INumbering_OrderNumber_ObjectForm_SerializesAsObject()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/orders", "{\"orderId\":\"o\",\"amountCharged\":0,\"numbersOrdered\":[]}");
        await c.INumbering.OrderAsync(new OrderCreateRequest
        {
            Numbers = new List<OrderNumber> { new(new OrderNumberSpec { Number = "2015550123", Route = 4 }) },
        });
        var body = h.Captured[0].Body;
        Assert.Contains("\"number\":\"2015550123\"", body);
        Assert.Contains("\"route\":4", body);
    }

    // ----------------------------------------------------------------- Lookups ---

    [Fact]
    public async Task Lookups_BothMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/cnam/2015550123",
            "{\"cnam\":\"ACME\",\"number\":\"2015550123\"}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/lrn/2015550123/2015550000",
            "{\"ani\":\"2015550000\",\"destination\":\"2015550123\",\"lrn\":{\"lrn\":\"L\",\"state\":\"NJ\"}}");
        var cn = await c.Lookups.CnamAsync("2015550123");
        Assert.Equal("ACME", cn.Cnam);
        var ln = await c.Lookups.LrnAsync("2015550123", "2015550000");
        Assert.Equal("L", ln.Lrn.Lrn);
    }

    // --------------------------------------------------------------- Messaging ---

    [Fact]
    public async Task Messaging_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/messages",
            "{\"number\":\"2015550123\",\"type\":\"sms\",\"fromTs\":1,\"toTs\":2,\"messages\":[]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/messages",
            "{\"id\":\"i\",\"type\":\"sms\",\"fromNumber\":\"a\",\"toNumber\":\"b\",\"parts\":1}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/messaging/brands",
            "{\"result\":{\"statusCode\":\"200\",\"status\":\"Success\"}}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/messaging/campaigns",
            "{\"campaigns\":[{\"id\":\"c\",\"status\":\"CAMPAIGN_PENDING_DCA\",\"numbers\":[\"2015550123\"]," +
            "\"dca\":{\"electionComplete\":false,\"electedCarriers\":[\"AT&T\"],\"carriers\":[]}}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/messaging/campaigns",
            "{\"result\":{\"statusCode\":\"200\",\"status\":\"Success\"}}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/messaging", "{\"numbers\":[]}");

        await c.Messaging.HistoryAsync(new HistoryOptions { Number = "2015550123", From = 1, To = 2, Type = "sms" });
        var histUri = h.Captured[0].Request.RequestUri!.ToString();
        Assert.Contains("number=2015550123", histUri);
        Assert.Contains("from=1", histUri);
        Assert.Contains("to=2", histUri);
        Assert.Contains("type=sms", histUri);
        var s = await c.Messaging.SendAsync(new MessageSendRequest { FromNumber = "a", ToNumber = "b", Text = "hi" });
        Assert.Equal("i", s.Id);

        // Verify the wire field names from the send body
        var sendBody = h.Captured[1].Body;
        Assert.Contains("\"fromNumber\":\"a\"", sendBody);
        Assert.Contains("\"toNumber\":\"b\"", sendBody);

        await c.Messaging.CreateBrandAsync(new MessagingBrandCreateRequest { MessagingBrandId = "Bx", MessagingBrandName = "n" });
        var cs = await c.Messaging.CampaignStatusAsync(new[] { "CAMP002" });
        Assert.Equal("CAMPAIGN_PENDING_DCA", cs.Campaigns[0].Status);
        Assert.False(cs.Campaigns[0].Dca!.ElectionComplete);
        Assert.Contains("ids=CAMP002", h.Captured[3].Request.RequestUri!.ToString());
        await c.Messaging.CreateCampaignAsync(new MessagingCampaignCreateRequest { MessagingBrandId = "Bx", ExternalCampaignId = "e", CampaignDescription = "d" });
        await c.Messaging.NumbersStateAsync(new[] { "2015550123", "2015551235" });
    }

    [Fact]
    public async Task Messaging_NumbersState_SendsRequiredNumbersQuery()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/messaging", "{\"numbers\":[]}");
        await c.Messaging.NumbersStateAsync(new[] { "2015550123" });
        Assert.Contains("numbers=2015550123", h.Captured[0].Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task Messaging_Health_AllMethods()
    {
        // v2.2.15: messaging-health endpoints have no response schema in the spec, so the
        // SDK returns the decoded JSON object as a raw Dictionary<string, object?> (nested
        // values are JsonElement); request bodies/query are still shaped.
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/messaging/health",
            "{\"summary\":{\"numbers\":3,\"healthy\":2}}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/messaging/health/alerts", "{\"enabled\":true}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/messaging/health/alerts", "{\"enabled\":false}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/messaging/health/check",
            "{\"number\":\"2125550123\",\"inService\":true}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/messaging/health/samples",
            "{\"samples\":[{\"reason\":\"spam\"}]}");

        var health = await c.Messaging.HealthAsync();
        var summary = (JsonElement)health["summary"]!;
        Assert.Equal(3, summary.GetProperty("numbers").GetInt32());
        Assert.Equal("/v2.2/messaging/health", h.Captured[0].Request.RequestUri!.AbsolutePath);

        var alerts = await c.Messaging.HealthAlertsAsync();
        Assert.True(((JsonElement)alerts["enabled"]!).GetBoolean());

        var off = await c.Messaging.SetHealthAlertsAsync(false);
        Assert.False(((JsonElement)off["enabled"]!).GetBoolean());
        Assert.Equal(HttpMethod.Put, h.Captured[2].Request.Method);
        Assert.Contains("\"enabled\":false", h.Captured[2].Body);

        var check = await c.Messaging.HealthCheckAsync("2125550123");
        Assert.True(((JsonElement)check["inService"]!).GetBoolean());
        Assert.Contains("\"number\":\"2125550123\"", h.Captured[3].Body);

        var samples = await c.Messaging.HealthSamplesAsync("2125550123");
        var samplesArr = (JsonElement)samples["samples"]!;
        Assert.Equal("spam", samplesArr[0].GetProperty("reason").GetString());
        Assert.Contains("number=2125550123", h.Captured[4].Request.RequestUri!.ToString());
    }

    // ----------------------------------------------------------------- Numbers ---

    [Fact]
    public async Task Numbers_CoreMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers", "{\"numbers\":[{\"number\":\"2015550123\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/numbers", "{\"number\":\"2015550123\",\"route\":4}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/2015550123", "{\"number\":\"2015550123\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/numbers/2015550123");
        h.EnqueueEnvelope(new HttpMethod("PATCH"), "/v2.2/numbers/2015550123", "{\"number\":\"2015550123\",\"accountId\":1,\"route\":4}");
        h.EnqueueNoContent(HttpMethod.Post, "/v2.2/numbers/2015550123/release");

        Assert.Single((await c.Numbers.ListAsync()).Numbers);
        await c.Numbers.AddAsync(new NumberAddRequest { Number = "2015550123" });
        await c.Numbers.GetAsync("2015550123");
        await c.Numbers.RemoveAsync("2015550123");
        await c.Numbers.MoveAsync("2015550123", new NumberMoveRequest { AccountId = 1, Route = 4 });
        await c.Numbers.ReleaseAsync("2015550123");
    }

    [Fact]
    public async Task Numbers_FeatureMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/route", "{\"number\":\"n\",\"route\":4}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/translation", "{\"number\":\"n\",\"translation\":\"1\"}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/cnam", "{\"number\":\"n\",\"cnam\":true}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/lidb", "{\"number\":\"n\",\"cnam\":\"X\",\"customerOrderReference\":\"r\",\"carrierStatus\":\"Success\"}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/n/fax", "{\"number\":\"n\",\"email\":\"e\"}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/fax", "{\"number\":\"n\",\"email\":\"e\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/numbers/n/fax");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/forward", "{\"number\":\"n\",\"forwardTo\":\"x\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/numbers/n/forward");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/n/sms", "{\"number\":\"n\",\"type\":\"email\",\"resource\":\"x\"}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/sms", "{\"number\":\"n\",\"type\":\"email\",\"resource\":\"x\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/numbers/n/sms");

        await c.Numbers.SetRouteAsync("n", new NumberRouteRequest { Route = 4 });
        await c.Numbers.SetTranslationAsync("n", new NumberTranslationRequest { Translation = "1" });
        await c.Numbers.SetCnamAsync("n", new NumberCnamRequest { Enabled = true });
        var lidb = await c.Numbers.SetLidbAsync("n", new NumberLidbRequest { Cnam = "X" });
        Assert.Equal("Success", lidb.CarrierStatus);

        await c.Numbers.GetFaxAsync("n");
        await c.Numbers.SetFaxAsync("n", new NumberFaxRequest { Email = "e" });
        await c.Numbers.RemoveFaxAsync("n");
        await c.Numbers.SetForwardAsync("n", new NumberForwardRequest { Destination = "2015550123" });
        await c.Numbers.RemoveForwardAsync("n");
        await c.Numbers.GetSmsAsync("n");
        await c.Numbers.SetSmsAsync("n", new NumberSmsRequest { Type = "email", Resource = "e" });
        await c.Numbers.RemoveSmsAsync("n");
    }

    [Fact]
    public async Task Numbers_MessagingAndCampaign()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/numbers/n/messaging",
            "{\"number\":\"n\",\"enabled\":true,\"carrier\":1,\"routeIn\":1,\"resource\":\"r\",\"network\":\"C\"," +
            "\"campaign\":{\"id\":\"c\",\"network\":\"A\",\"status\":\"ACTIVE\",\"upstreamCnpId\":\"u\"}," +
            "\"pending\":{\"campaignId\":\"c\",\"state\":\"routing\",\"message\":\"assigning routes\",\"createdAt\":\"t1\",\"updatedAt\":\"t2\"}}");
        h.EnqueueEnvelope(new HttpMethod("PATCH"), "/v2.2/numbers/n/messaging", "{\"number\":\"n\",\"updated\":[\"routeIn\"]}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/numbers/n/messaging-campaign",
            "{\"number\":\"n\",\"campaignId\":\"c\",\"status\":\"enabled\",\"carrier\":19,\"network\":\"B\",\"upstreamCnpId\":\"CAMP003\"," +
            "\"alreadyAssigned\":false,\"previousNetwork\":\"A\",\"previousNetworkCleared\":true,\"orderId\":\"987654\"," +
            "\"warning\":null,\"message\":null,\"job\":null}");
        h.EnqueueEnvelope(HttpMethod.Delete, "/v2.2/numbers/n/messaging-campaign",
            "{\"number\":\"n\",\"campaignId\":\"c\",\"network\":\"A\",\"upstreamCnpId\":\"u\",\"unassigned\":true}");
        h.EnqueueEnvelope(HttpMethod.Delete, "/v2.2/numbers/messaging-campaign",
            "{\"campaignId\":\"c\",\"network\":\"A\",\"upstreamCnpId\":\"u\",\"unassignedNumbers\":[\"n\"]}");
        h.EnqueueEnvelope(new HttpMethod("PATCH"), "/v2.2/numbers/n/port-out-pin",
            "{\"number\":\"n\",\"portOutPin\":\"1234\",\"changed\":true}");

        var ms = await c.Numbers.GetMessagingAsync("n");
        Assert.Equal("ACTIVE", ms.Campaign!.Status);
        Assert.Equal("C", ms.Network);
        Assert.Equal("routing", ms.Pending!.State);
        var pm = await c.Numbers.PatchMessagingAsync("n", new NumberMessagingPatchRequest { RouteIn = 1 });
        Assert.Contains("routeIn", pm.Updated);
        var ac = await c.Numbers.AssignCampaignAsync("n", new NumberCampaignAssignRequest { CampaignId = "c" });
        Assert.Equal("enabled", ac.Status);
        Assert.Equal("B", ac.Network);
        Assert.False(ac.AlreadyAssigned);
        Assert.Equal("987654", ((System.Text.Json.JsonElement)ac.OrderId!).GetString());
        var uc = await c.Numbers.UnassignCampaignAsync("n", "c");
        Assert.True(uc.Unassigned);
        Assert.Contains("campaignId=c", h.Captured[3].Request.RequestUri!.ToString());
        var bulk = await c.Numbers.BulkUnassignCampaignAsync(new[] { "n" }, "c");
        Assert.Single(bulk.UnassignedNumbers);
        var bulkUri = h.Captured[4].Request.RequestUri!.ToString();
        Assert.Contains("numbers=n", bulkUri);
        Assert.Contains("campaignId=c", bulkUri);
        Assert.Equal(string.Empty, h.Captured[4].Body); // bulk unassign takes no body
        var pin = await c.Numbers.SetPortOutPinAsync("n", new PortOutPinUpdateRequest { Pin = "1234" });
        Assert.Equal("1234", pin.PortOutPin);
        Assert.True(pin.Changed);
    }

    // ----------------------------------------------------------------- Support ---

    [Fact]
    public async Task Support_AllMethods()
    {
        var (c, h) = TestFactory.NewClient();
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/support/tickets",
            "{\"totalCount\":1,\"tickets\":[{\"id\":1,\"number\":1015,\"status\":\"active\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/support/tickets",
            "{\"ticket\":{\"id\":2,\"number\":1016,\"status\":\"active\",\"subject\":\"s\"}}");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/support/tickets/2",
            "{\"ticket\":{\"id\":2,\"number\":1016,\"status\":\"active\"}}");
        h.EnqueueEnvelope(HttpMethod.Put, "/v2.2/support/tickets/2",
            "{\"id\":2,\"status\":\"success\"}");
        h.EnqueueNoContent(HttpMethod.Delete, "/v2.2/support/tickets/2");
        h.EnqueueEnvelope(HttpMethod.Get, "/v2.2/support/tickets/2/messages",
            "{\"messages\":[{\"id\":1,\"status\":\"active\",\"body\":\"hi\"}]}");
        h.EnqueueEnvelope(HttpMethod.Post, "/v2.2/support/tickets/2/replies",
            "{\"message\":\"Reply added\"}");

        var list = await c.Support.ListAsync();
        Assert.Equal((int?)1, list.TotalCount);
        Assert.Equal(1015, list.Tickets[0].TicketNumber); // sequence number, NOT a phone number
        var created = await c.Support.CreateAsync(new TicketCreateRequest { Subject = "s", Message = "m" });
        Assert.Equal(1016, created.Ticket.TicketNumber);
        await c.Support.GetAsync(2);
        await c.Support.UpdateAsync(2, new TicketUpdateRequest { Status = "closed" });
        await c.Support.DeleteAsync(2);
        await c.Support.MessagesAsync(2);
        var reply = await c.Support.ReplyAsync(2, new TicketReplyRequest { Message = "x" });
        Assert.Equal("Reply added", reply.Message);
    }

    // --------------------------------------------------------------- Disposal ---

    [Fact]
    public void Client_IsDisposable()
    {
        var c = new VoiceTelClient(apiKey: "x", baseUrl: "https://x");
        c.Dispose();
    }

    [Fact]
    public void Client_DefaultBaseUrl()
    {
        using var c = new VoiceTelClient(apiKey: "x");
        Assert.Equal(SdkInfo.DefaultBaseUrl, c.BaseUrl);
    }
}
