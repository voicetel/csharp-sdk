# 📞 VoiceTel C# / .NET SDK

The official C# client for the [VoiceTel REST API](https://voicetel.com/docs/api/v2.2/) — provision numbers, place orders, validate e911, send messages, and manage your account, all with strongly-typed, `async`/`await`-friendly C#.

![Version](https://img.shields.io/badge/version-2.2.10-blue)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%20netstandard2.1-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)
![Coverage](https://img.shields.io/badge/coverage-95%25-brightgreen)

## 📚 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Quickstart](#-quickstart)
- [Authentication](#-authentication)
- [Resource Reference](#-resource-reference)
- [Error Handling](#-error-handling)
- [Cancellation and Timeouts](#-cancellation-and-timeouts)
- [Rate Limits](#-rate-limits)
- [Development](#-development)
- [API Documentation](#-api-documentation)
- [Contributors](#-contributors)
- [Sponsors](#-sponsors)
- [License](#-license)

## ✨ Features

### 🛡️ Strongly Typed End-to-End
- **Native C# classes** for every one of the 73 API operations — serialized with `System.Text.Json`, no reflection magic.
- **Nullable reference types** throughout — distinguish "not set" from "empty" cleanly when PATCH-ing.
- **Nullable response fields** — `ForwardTo` is `string?` so you can tell apart "no forward configured" from an empty destination.
- **`CancellationToken` everywhere.** Every method takes a `CancellationToken` as the last argument; cancellation and timeouts propagate cleanly down to the HTTP layer.

### 🔁 Production-Grade Transport
- Built on `System.Net.Http.HttpClient` — no third-party dependencies on `net8.0`.
- **Automatic retry** with exponential backoff on 429 / 5xx — honors `Retry-After` headers, capped at 8s.
- **Configurable retries and `HttpClient`** — bring your own for connection pooling, instrumentation, or HTTP/2.
- **Bearer auth** managed for you; the password→key exchange is one method call (`client.LoginAsync`).
- **Structured `ApiError`** exception with typed `Kind` enum so you can `switch (ex.Kind) { case ErrorKind.RateLimit: ... }` without parsing HTTP status codes.

### 📞 Complete API Coverage
- **Numbers** — list, get, add, remove, route, translate, CNAM, LIDB, fax, forward, SMS, messaging campaigns, port-out PIN, account moves.
- **Account** — profile, sub-accounts, CDRs, credits, payments, MRC, registration, password recovery.
- **e911** — record provisioning, address validation, lookup, removal.
- **Gateways** — list, create, update, delete, view bound numbers.
- **Messaging** — SMS & MMS sending, message history, 10DLC brand and campaign registration, per-number messaging state.
- **Lookups** — CNAM and LRN dips.
- **iNumbering** — inventory search, coverage queries, number orders, port-in submissions, port-out availability (with v2.2.10 `localRoutingNumber` and `rateCenterTier` fields).
- **Support** — ticket create / read / update / delete, threaded messages, replies.
- **ACL** — IP allowlist management with structured 409 conflict bodies.
- **Authentication** — switch between Digest, IP-only, or hybrid modes; rotate passwords.

### 🧪 Battle-Tested
- **xUnit unit tests** that exercise every public method and every error path.
- **HttpMessageHandler mocks** — no network calls in unit tests.
- **`dotnet build` clean with `TreatWarningsAsErrors`** on both `net8.0` and `netstandard2.1`.

### 📦 Clean Distribution
- Zero codegen footprint — every byte hand-written.
- Single NuGet package: `VoiceTel.Sdk`.
- `netstandard2.1` for broad reach (Unity, Xamarin, .NET Framework 4.8 via shim) and `net8.0` for first-class modern .NET.

## 🚀 Installation

```bash
dotnet add package VoiceTel.Sdk
```

Or `<PackageReference>` it directly:

```xml
<PackageReference Include="VoiceTel.Sdk" Version="2.2.10" />
```

Targets **.NET 8.0** (preferred) and **.NET Standard 2.1**.

## 🏁 Quickstart

```csharp
using VoiceTel.Sdk;

using var client = new VoiceTelClient();

// Exchange username + password for an API key (one-time per session)
await client.LoginAsync(username: 1000000001, password: "hunter2");

// Typed responses — your IDE knows what `me` is.
var me = await client.Account.GetAsync();
Console.WriteLine($"Balance: ${me.Cash:F2}  |  Caller ID: {me.CallerId}");

// List your numbers
var numbers = await client.Numbers.ListAsync();
foreach (var n in numbers.Numbers)
{
    Console.WriteLine($"{n.Number}  route={n.Route}  cnam={n.Cnam}  sms={n.SmsEnabled}");
}
```

Or, if you already have an API key:

```csharp
using var client = new VoiceTelClient(apiKey: "32hex...");
var coverage = await client.INumbering.CoverageAsync(new() { State = "NJ" });
foreach (var bucket in coverage.Coverage)
{
    Console.WriteLine($"{bucket.Npa}-{bucket.Nxx}: {bucket.Count} TNs available");
}
```

## 🔑 Authentication

Every endpoint requires `Authorization: Bearer <apikey>` **except** `POST /v2.2/account/api-key`, which exchanges username + password for a fresh key. `LoginAsync()` handles the exchange and installs the returned key on the client.

Re-fetch the API key after any password change — the old one is invalidated.

> Don't have credentials yet? Get them at **[voicetel.com/docs/api/v2.2/credentials](https://voicetel.com/docs/api/v2.2/credentials/)**.

```csharp
using var client = new VoiceTelClient();
var key = await client.LoginAsync(1000000001, "hunter2");
// `key` is the new 32-hex bearer; the client already has it installed.
```

## 🗺️ Resource Reference

| Resource | Property on Client | Example |
|---|---|---|
| Account | `client.Account` | `await client.Account.CdrAsync(t1, t2)` |
| ACL | `client.Acl` | `await client.Acl.AddAsync(new AclModifyRequest { ... })` |
| Authentication | `client.Authentication` | `await client.Authentication.UpdateAsync(new AuthPutRequest { AuthType = AuthTypes.IpAuth })` |
| e911 | `client.E911` | `await client.E911.ValidateAsync(new E911AddressRequest { ... })` |
| Gateways | `client.Gateways` | `await client.Gateways.ListAsync()` |
| iNumbering | `client.INumbering` | `await client.INumbering.SearchInventoryAsync(new() { Npa = 201 })` |
| Lookups | `client.Lookups` | `await client.Lookups.LrnAsync("2015551234", "2012548000")` |
| Messaging | `client.Messaging` | `await client.Messaging.SendAsync(new MessageSendRequest { ... })` |
| Numbers | `client.Numbers` | `await client.Numbers.AssignCampaignAsync("2015551234", ...)` |
| Support | `client.Support` | `await client.Support.CreateAsync(new TicketCreateRequest { ... })` |

Optional request fields use nullable types. Initialize only what you want to change:

```csharp
await client.Account.UpdateAsync(new AccountPutRequest
{
    Timezone        = "America/Chicago",
    NotifyThreshold = 5,
    Notify          = true,
});
```

## 🚨 Error Handling

All HTTP errors throw `ApiError`. Switch on `Kind`:

| Kind | HTTP status |
|---|---|
| `ErrorKind.BadRequest` | 400 |
| `ErrorKind.Authentication` | 401 |
| `ErrorKind.PermissionDenied` | 403 |
| `ErrorKind.NotFound` | 404 |
| `ErrorKind.Conflict` | 409 |
| `ErrorKind.RateLimit` | 429 |
| `ErrorKind.Server` | 5xx |
| `ErrorKind.Unknown` | other / transport |

```csharp
try
{
    var n = await client.Numbers.GetAsync("9999999999");
    Console.WriteLine($"Got: {n.Number}");
}
catch (ApiError ex) when (ex.IsNotFound)
{
    Console.WriteLine("That number isn't on your account.");
}
catch (ApiError ex) when (ex.IsRateLimit)
{
    Console.WriteLine("Slow down — backoff and retry.");
}
catch (ApiError ex)
{
    Console.WriteLine($"status={ex.StatusCode} code={ex.Code} body={ex.Body}");
}
```

`ex.Body` preserves the parsed response body — useful for 409 conflicts where the server returns structured detail (see `AclConflictData`, `AuthPutConflictData`).

## ⏱️ Cancellation and Timeouts

Every method takes a `CancellationToken`. Use it for cancellation and per-call deadlines:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var me = await client.Account.GetAsync(cts.Token);
```

For a global per-request timeout, configure the `HttpClient`:

```csharp
var http = new HttpClient { Timeout = TimeSpan.FromSeconds(45) };
using var client = new VoiceTelClient(apiKey: "...", httpClient: http);
```

## ⏱️ Rate Limits

These endpoints are limited to **6 requests per hour per IP**:

- `account/info` (`client.Account.GetAsync`)
- `account/cdr` (`client.Account.CdrAsync`)
- `account/recurring-charges` (`client.Account.RecurringChargesAsync`)
- `account/payments` (`client.Account.PaymentsAsync`)
- `account/registration` (`client.Account.RegistrationAsync`)
- `account/api-key` (`client.LoginAsync`)

The SDK automatically retries 429 responses with `Retry-After` honored, up to `maxRetries` (default 2). To bump it:

```csharp
using var client = new VoiceTelClient(
    apiKey: key,
    maxRetries: 4);
```

## 🛠️ Development

```bash
git clone https://github.com/voicetel/csharp-sdk
cd csharp-sdk

# Restore and build
dotnet build VoiceTel.Sdk.sln

# Unit tests (no network)
dotnet test --filter "Category!=Integration"

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Integration tests (read-only) — requires real credentials
export VOICETEL_USERNAME=1000000001
export VOICETEL_PASSWORD=hunter2
dotnet test --filter "Category=Integration"
```

## 📖 API Documentation

- **Reference docs:** [voicetel.com/docs/api/v2.2/](https://voicetel.com/docs/api/v2.2/)
- **Interactive playground:** [voicetel.com/docs/api/v2.2/playground/](https://voicetel.com/docs/api/v2.2/playground/) — try the API in your browser without writing any code
- **API credentials:** [voicetel.com/docs/api/v2.2/credentials/](https://voicetel.com/docs/api/v2.2/credentials/)

## 🙌 Contributors

- [Michael Mavroudis](https://github.com/mavroudis) — Lead Developer

Contributions welcome. Open an issue describing the change, or send a pull request against `main`.

## 💖 Sponsors

| Sponsor | Contribution |
|---------|--------------|
| [VoiceTel Communications](https://voicetel.com) | Primary development and production hosting |

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
