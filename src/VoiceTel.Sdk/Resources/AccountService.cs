using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>
/// Every operation under the Account tag.
/// <para>
/// CDR, recurring-charges, payments, registration, and the api-key exchange
/// share a 6 req/hour/IP rate limit. Bursting will trigger 429s.
/// </para>
/// </summary>
public sealed class AccountService
{
    private readonly Transport _t;
    internal AccountService(Transport t) => _t = t;

    /// <summary>Returns the authenticated account's profile.</summary>
    public Task<AccountData> GetAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountData>(HttpMethod.Get, "/v2.2/account", null, null, requireAuth: true, cancellationToken);

    /// <summary>Partial-updates account settings; only fields you set on <paramref name="body"/> are sent.</summary>
    public Task<AccountPutData> UpdateAsync(AccountPutRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountPutData>(HttpMethod.Put, "/v2.2/account", null, body, requireAuth: true, cancellationToken);

    /// <summary>Creates a sub-account. Admin-only.</summary>
    public Task<AccountAddData> AddAsync(AccountAddRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountAddData>(HttpMethod.Post, "/v2.2/account", null, body, requireAuth: true, cancellationToken);

    /// <summary>Public sign-up: <c>POST /v2.2/accounts</c>.</summary>
    public Task<AccountSignupData> SignupAsync(AccountSignupRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountSignupData>(HttpMethod.Post, "/v2.2/accounts", null, body, requireAuth: true, cancellationToken);

    /// <summary>Fetches CDRs in the [start, end] Unix-seconds range. Rate-limited.</summary>
    public Task<AccountCdrData> CdrAsync(int start, int end, CancellationToken cancellationToken = default)
    {
        var q = new QueryBuilder();
        q.AddInt("start", start);
        q.AddInt("end", end);
        return _t.RequestAsync<AccountCdrData>(HttpMethod.Get, "/v2.2/account/cdr", q.HasAny ? q.ToString() : null, null, requireAuth: true, cancellationToken);
    }

    /// <summary>Full credit history, newest first.</summary>
    public Task<AccountCreditsData> CreditsAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountCreditsData>(HttpMethod.Get, "/v2.2/account/credits", null, null, requireAuth: true, cancellationToken);

    /// <summary>Active monthly-recurring charges. Rate-limited.</summary>
    public Task<AccountMrcData> RecurringChargesAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountMrcData>(HttpMethod.Get, "/v2.2/account/recurring-charges", null, null, requireAuth: true, cancellationToken);

    /// <summary>Full payment history, newest first. Rate-limited.</summary>
    public Task<AccountPaymentsData> PaymentsAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountPaymentsData>(HttpMethod.Get, "/v2.2/account/payments", null, null, requireAuth: true, cancellationToken);

    /// <summary>Current SIP registration. Rate-limited.</summary>
    public Task<AccountRegistrationData> RegistrationAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountRegistrationData>(HttpMethod.Get, "/v2.2/account/registration", null, null, requireAuth: true, cancellationToken);

    /// <summary>Starts the password recovery flow (no auth required).</summary>
    public Task<AccountRecoverData> RecoverAsync(AccountRecoverRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AccountRecoverData>(HttpMethod.Post, "/v2.2/account/recovery", null, body, requireAuth: false, cancellationToken);
}
