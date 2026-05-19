using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Body for <c>POST /v2.2/orders</c>.</summary>
public sealed class OrderCreateRequest
{
    [JsonPropertyName("numbers")] public List<OrderNumber> Numbers { get; set; } = new();
}

/// <summary>
/// Single entry in <see cref="OrderCreateRequest.Numbers"/>.
/// May be a plain TN (use <see cref="OrderNumber(string)"/>) or a
/// <c>{number, route}</c> object (use <see cref="OrderNumber(OrderNumberSpec)"/>).
/// </summary>
[JsonConverter(typeof(OrderNumberJsonConverter))]
public sealed class OrderNumber
{
    /// <summary>Plain 10-digit TN, when <see cref="Spec"/> is <c>null</c>.</summary>
    public string? Value { get; }

    /// <summary>The object form, when present overrides <see cref="Value"/>.</summary>
    public OrderNumberSpec? Spec { get; }

    /// <summary>Constructs a plain-TN entry.</summary>
    public OrderNumber(string value)
    {
        Value = value;
        Spec = null;
    }

    /// <summary>Constructs the object-form entry.</summary>
    public OrderNumber(OrderNumberSpec spec)
    {
        Value = null;
        Spec = spec;
    }
}

/// <summary>The <c>{number, route}</c> object variant of <see cref="OrderNumber"/>.</summary>
public sealed class OrderNumberSpec
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("route")] public int? Route { get; set; }
}

internal sealed class OrderNumberJsonConverter : JsonConverter<OrderNumber>
{
    public override OrderNumber Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new OrderNumber(reader.GetString() ?? string.Empty);
        }
        var spec = JsonSerializer.Deserialize<OrderNumberSpec>(ref reader, options) ?? new OrderNumberSpec();
        return new OrderNumber(spec);
    }

    public override void Write(Utf8JsonWriter writer, OrderNumber value, JsonSerializerOptions options)
    {
        if (value.Spec is not null)
        {
            JsonSerializer.Serialize(writer, value.Spec, options);
        }
        else
        {
            writer.WriteStringValue(value.Value ?? string.Empty);
        }
    }
}

/// <summary>LIDB feature for a port-in TN.</summary>
public sealed class PortFeatureLidb
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

/// <summary>Routing feature for a port-in TN.</summary>
public sealed class PortFeatureRouting
{
    [JsonPropertyName("gatewayId")] public int GatewayId { get; set; }
}

/// <summary>SMS feature for a port-in TN.</summary>
public sealed class PortFeatureSms
{
    [JsonPropertyName("campaignId")] public string? CampaignId { get; set; }
}

/// <summary>Per-TN feature configuration applied after the port completes.</summary>
public sealed class PortFeature
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("routing")] public PortFeatureRouting? Routing { get; set; }
    [JsonPropertyName("lidb")] public PortFeatureLidb? Lidb { get; set; }
    [JsonPropertyName("sms")] public PortFeatureSms? Sms { get; set; }
}

/// <summary>Body for <c>POST /v2.2/ports</c>.</summary>
public sealed class PortSubmitRequest
{
    [JsonPropertyName("did")] public List<string> Did { get; set; } = new();
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("nameType")] public string NameType { get; set; } = string.Empty;
    [JsonPropertyName("lcBtn")] public string LcBtn { get; set; } = string.Empty;
    [JsonPropertyName("lcAccountNumber")] public string LcAccountNumber { get; set; } = string.Empty;
    [JsonPropertyName("streetNumber")] public string StreetNumber { get; set; } = string.Empty;
    [JsonPropertyName("street")] public string Street { get; set; } = string.Empty;
    [JsonPropertyName("streetType")] public string StreetType { get; set; } = string.Empty;
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
    [JsonPropertyName("country")] public string Country { get; set; } = string.Empty;
    [JsonPropertyName("authPerson")] public string AuthPerson { get; set; } = string.Empty;
    [JsonPropertyName("streetPrefix")] public string? StreetPrefix { get; set; }
    [JsonPropertyName("streetSuffix")] public string? StreetSuffix { get; set; }
    [JsonPropertyName("floor")] public string? Floor { get; set; }
    [JsonPropertyName("room")] public string? Room { get; set; }
    [JsonPropertyName("building")] public string? Building { get; set; }
    [JsonPropertyName("unitValue")] public string? UnitValue { get; set; }
    [JsonPropertyName("desiredDueDate")] public string? DesiredDueDate { get; set; }
    [JsonPropertyName("pin")] public string? Pin { get; set; }
    [JsonPropertyName("features")] public List<PortFeature>? Features { get; set; }
}

/// <summary>One TN available for assignment.</summary>
public sealed class InventoryItem
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("rateCenter")] public string RateCenter { get; set; } = string.Empty;
    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;
    [JsonPropertyName("province")] public string Province { get; set; } = string.Empty;
    [JsonPropertyName("lata")] public string Lata { get; set; } = string.Empty;
}

/// <summary>One aggregated availability bucket.</summary>
public sealed class InventoryCoverageItem
{
    [JsonPropertyName("count")] public int Count { get; set; }
    [JsonPropertyName("npa")] public string? Npa { get; set; }
    [JsonPropertyName("nxx")] public string? Nxx { get; set; }
    [JsonPropertyName("block")] public string? Block { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("rcAbbre")] public string? RcAbbre { get; set; }
    [JsonPropertyName("lata")] public string? Lata { get; set; }
    [JsonPropertyName("locState")] public string? LocState { get; set; }
}

/// <summary>One row in the port-status list.</summary>
public sealed class PortSummary
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("pid")] public string? Pid { get; set; }
    [JsonPropertyName("foc")] public string? Foc { get; set; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("supportUrl")] public string? SupportUrl { get; set; }
}

/// <summary>Full record for a single port-in.</summary>
public sealed class PortDetail
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("pid")] public string? Pid { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("foc")] public string? Foc { get; set; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; set; }
    [JsonPropertyName("numbers")] public List<string>? Numbers { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/inventory</c>.</summary>
public sealed class InventorySearchData
{
    [JsonPropertyName("numbers")] public List<InventoryItem> Numbers { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/inventory/coverage</c>.</summary>
public sealed class InventoryCoverageData
{
    [JsonPropertyName("coverage")] public List<InventoryCoverageItem> Coverage { get; set; } = new();
}

/// <summary>One failed-row entry inside <see cref="OrderCreateData.Failed"/>.</summary>
public sealed class OrderFailedEntry
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
}

/// <summary>Response data for <c>POST /v2.2/orders</c>.</summary>
public sealed class OrderCreateData
{
    [JsonPropertyName("orderId")] public string OrderId { get; set; } = string.Empty;
    [JsonPropertyName("amountCharged")] public double AmountCharged { get; set; }
    [JsonPropertyName("numbersOrdered")] public List<string> NumbersOrdered { get; set; } = new();
    [JsonPropertyName("failed")] public List<OrderFailedEntry>? Failed { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/ports</c>.</summary>
public sealed class PortListData
{
    [JsonPropertyName("ports")] public List<PortSummary> Ports { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/ports/{id}</c>.</summary>
public sealed class PortDetailData
{
    [JsonPropertyName("port")] public PortDetail Port { get; set; } = new();
}

/// <summary>Response data for <c>POST /v2.2/ports</c>.</summary>
public sealed class PortSubmitData
{
    [JsonPropertyName("pid")] public string Pid { get; set; } = string.Empty;
    [JsonPropertyName("ticket")] public int Ticket { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("loaUrl")] public string LoaUrl { get; set; } = string.Empty;
    [JsonPropertyName("portUrl")] public string PortUrl { get; set; } = string.Empty;
}

/// <summary>Response data for <c>GET /v2.2/ports/availability/{number}</c>.</summary>
public sealed class PortAvailabilityData
{
    [JsonPropertyName("number")] public string Number { get; set; } = string.Empty;
    [JsonPropertyName("portable")] public bool Portable { get; set; }
    [JsonPropertyName("losingCarrier")] public string? LosingCarrier { get; set; }
    /// <summary>LRN of destination switch (v2.2.10+); nullable.</summary>
    [JsonPropertyName("localRoutingNumber")] public string? LocalRoutingNumber { get; set; }
    /// <summary>Rate-center tier classification (v2.2.10+); nullable.</summary>
    [JsonPropertyName("rateCenterTier")] public string? RateCenterTier { get; set; }
    [JsonPropertyName("reason")] public string? Reason { get; set; }
}

/// <summary>Query filters for <c>SearchInventoryAsync</c>.</summary>
public sealed class InventoryQuery
{
    public int Npa { get; set; }
    public int Nxx { get; set; }
    public string? State { get; set; }
    public string? RateCenter { get; set; }
    public string? Contains { get; set; }
    public string? EndsWith { get; set; }
    public int Limit { get; set; }
}

/// <summary>Query filters for <c>CoverageAsync</c>.</summary>
public sealed class CoverageQuery
{
    public string? State { get; set; }
    public string? RateCenter { get; set; }
}
