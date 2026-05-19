using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceTel.Sdk.Models;

namespace VoiceTel.Sdk.Resources;

/// <summary>Manages the IP allowlist (CIDR entries) bound to the account.</summary>
public sealed class AclService
{
    private readonly Transport _t;
    internal AclService(Transport t) => _t = t;

    /// <summary>Returns the current allowlist.</summary>
    public Task<AclListData> ListAsync(CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AclListData>(HttpMethod.Get, "/v2.2/acl", null, null, requireAuth: true, cancellationToken);

    /// <summary>Appends one or more CIDR entries to the allowlist.</summary>
    public Task<AclAddData> AddAsync(AclModifyRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AclAddData>(HttpMethod.Post, "/v2.2/acl", null, body, requireAuth: true, cancellationToken);

    /// <summary>Removes one or more CIDR entries from the allowlist (returns 200 with body).</summary>
    public Task<AclRemoveData> RemoveAsync(AclModifyRequest body, CancellationToken cancellationToken = default) =>
        _t.RequestAsync<AclRemoveData>(HttpMethod.Delete, "/v2.2/acl", null, body, requireAuth: true, cancellationToken);
}
