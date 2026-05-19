using System.Text.Json.Serialization;

namespace VoiceTel.Sdk.Models;

/// <summary>Per-service rates exposed on an account. Read-only for non-administrators.</summary>
public sealed class AccountRates
{
    [JsonPropertyName("cnam")] public double Cnam { get; set; }
    [JsonPropertyName("intlMax")] public double IntlMax { get; set; }
    [JsonPropertyName("nibble")] public double Nibble { get; set; }
    [JsonPropertyName("lrn")] public double Lrn { get; set; }
    [JsonPropertyName("fax")] public double Fax { get; set; }
    [JsonPropertyName("tfAdj")] public double TfAdj { get; set; }
    [JsonPropertyName("did")] public double Did { get; set; }
    [JsonPropertyName("mms")] public double Mms { get; set; }
    [JsonPropertyName("sms")] public double Sms { get; set; }
}

/// <summary>Per-service feature flags. <c>true</c> means enabled on this account.</summary>
public sealed class AccountServices
{
    [JsonPropertyName("e911")] public bool E911 { get; set; }
    [JsonPropertyName("cnam")] public bool Cnam { get; set; }
    [JsonPropertyName("bypassMedia")] public bool BypassMedia { get; set; }
    [JsonPropertyName("intl")] public bool Intl { get; set; }
    [JsonPropertyName("rcid")] public bool Rcid { get; set; }
    [JsonPropertyName("mms")] public bool Mms { get; set; }
    [JsonPropertyName("dialer")] public bool Dialer { get; set; }
    [JsonPropertyName("sms")] public bool Sms { get; set; }
}

/// <summary>The profile returned by <c>GET /v2.2/account</c>.</summary>
public sealed class AccountData
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("created")] public string? Created { get; set; }
    [JsonPropertyName("cash")] public double Cash { get; set; }
    [JsonPropertyName("callerId")] public string? CallerId { get; set; }
    [JsonPropertyName("timezone")] public string? Timezone { get; set; }
    [JsonPropertyName("authType")] public int AuthType { get; set; }
    [JsonPropertyName("ccs")] public int Ccs { get; set; }
    [JsonPropertyName("notify")] public bool Notify { get; set; }
    [JsonPropertyName("notifyThreshold")] public int NotifyThreshold { get; set; }
    [JsonPropertyName("rates")] public AccountRates? Rates { get; set; }
    [JsonPropertyName("services")] public AccountServices? Services { get; set; }
}

/// <summary>One credit row in <see cref="AccountCreditsData"/>.</summary>
public sealed class CreditEntry
{
    [JsonPropertyName("date")] public string Date { get; set; } = string.Empty;
    [JsonPropertyName("paid")] public bool Paid { get; set; }
    [JsonPropertyName("amount")] public double Amount { get; set; }
}

/// <summary>One payment row in <see cref="AccountPaymentsData"/>.</summary>
public sealed class PaymentEntry
{
    [JsonPropertyName("transactionId")] public string? TransactionId { get; set; }
    [JsonPropertyName("date")] public string Date { get; set; } = string.Empty;
    [JsonPropertyName("payerEmail")] public string? PayerEmail { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("amount")] public double Amount { get; set; }
}

/// <summary>Per-call billing summary inside a CDR row.</summary>
public sealed class CdrEntryValue
{
    [JsonPropertyName("dur")] public string? Dur { get; set; }
    [JsonPropertyName("dst")] public string? Dst { get; set; }
    [JsonPropertyName("ba")] public string? Ba { get; set; }
    [JsonPropertyName("nr")] public string? Nr { get; set; }
    [JsonPropertyName("cn")] public string? Cn { get; set; }
    [JsonPropertyName("ip")] public string? Ip { get; set; }
    [JsonPropertyName("cid")] public string? Cid { get; set; }
}

/// <summary>One CDR row in <see cref="AccountCdrData"/>.</summary>
public sealed class CdrEntry
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("key")] public System.Collections.Generic.List<string> Key { get; set; } = new();
    [JsonPropertyName("value")] public CdrEntryValue Value { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/account/cdr</c>.</summary>
public sealed class AccountCdrData
{
    [JsonPropertyName("cdr")] public System.Collections.Generic.List<CdrEntry> Cdr { get; set; } = new();
    [JsonPropertyName("start")] public int Start { get; set; }
    [JsonPropertyName("end")] public int End { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/account/credits</c>.</summary>
public sealed class AccountCreditsData
{
    [JsonPropertyName("credits")] public System.Collections.Generic.List<CreditEntry> Credits { get; set; } = new();
}

/// <summary>Response data for <c>GET /v2.2/account/payments</c>.</summary>
public sealed class AccountPaymentsData
{
    [JsonPropertyName("payments")] public System.Collections.Generic.List<PaymentEntry> Payments { get; set; } = new();
}

/// <summary>One monthly-recurring charge row.</summary>
public sealed class MrcCharge
{
    [JsonPropertyName("amount")] public double Amount { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/account/recurring-charges</c>.</summary>
public sealed class AccountMrcData
{
    [JsonPropertyName("charges")] public System.Collections.Generic.List<MrcCharge> Charges { get; set; } = new();
    [JsonPropertyName("total")] public double Total { get; set; }
}

/// <summary>Response data for <c>GET /v2.2/account/registration</c>.</summary>
public sealed class AccountRegistrationData
{
    [JsonPropertyName("agent")] public string? Agent { get; set; }
    [JsonPropertyName("uri")] public string? Uri { get; set; }
    [JsonPropertyName("expires")] public int Expires { get; set; }
}

/// <summary>Body for <c>POST /v2.2/account</c> (admin-only sub-account creation).</summary>
public sealed class AccountAddRequest
{
    [JsonPropertyName("username")] public int Username { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    [JsonPropertyName("masterAccount")] public int? MasterAccount { get; set; }
}

/// <summary>Response data for <c>POST /v2.2/account</c>.</summary>
public sealed class AccountAddData
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("masterAccount")] public string? MasterAccount { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
}

/// <summary>Body for <c>PUT /v2.2/account</c>. <c>null</c> means "leave unchanged".</summary>
public sealed class AccountPutRequest
{
    [JsonPropertyName("notify")] public bool? Notify { get; set; }
    [JsonPropertyName("notifyThreshold")] public int? NotifyThreshold { get; set; }
    [JsonPropertyName("timezone")] public string? Timezone { get; set; }
    [JsonPropertyName("callerId")] public string? CallerId { get; set; }
    [JsonPropertyName("e911")] public bool? E911 { get; set; }
    [JsonPropertyName("intl")] public bool? Intl { get; set; }
    [JsonPropertyName("sms")] public bool? Sms { get; set; }
    [JsonPropertyName("mms")] public bool? Mms { get; set; }
    [JsonPropertyName("ccs")] public int? Ccs { get; set; }
}

/// <summary>Response data for <c>PUT /v2.2/account</c>.</summary>
public sealed class AccountPutData
{
    [JsonPropertyName("updated")] public System.Collections.Generic.List<string> Updated { get; set; } = new();
}

/// <summary>Body for <c>POST /v2.2/accounts</c> (public sign-up).</summary>
public sealed class AccountSignupRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    [JsonPropertyName("promo")] public string? Promo { get; set; }
}

/// <summary>Response data for <c>POST /v2.2/accounts</c>.</summary>
public sealed class AccountSignupData
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
}

/// <summary>Body for <c>POST /v2.2/account/recovery</c> (no auth required).</summary>
public sealed class AccountRecoverRequest
{
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
}

/// <summary>Response data for <c>POST /v2.2/account/recovery</c>.</summary>
public sealed class AccountRecoverData
{
    [JsonPropertyName("message")] public string? Message { get; set; }
}

/// <summary>Response data for <c>POST /v2.2/account/api-key</c>.</summary>
public sealed class AccountApiKeyData
{
    [JsonPropertyName("apikey")] public string? ApiKey { get; set; }
}
