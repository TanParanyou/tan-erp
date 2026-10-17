using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Audit;

/// <summary>Resolves audit.read before the store is reached (permission before existence).</summary>
public sealed class AuditEventHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IAuditEventStore _store;

    public AuditEventHandler(IRequestAccessResolver access, IAuditEventStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AuditEventPage>> ListAsync(
        AdminCaller caller, string? resourceType, string? resourceId, string? action, Guid? actorUserId,
        DateTimeOffset? from, DateTimeOffset? to, int page, int limit, CancellationToken ct)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AuditPermissions.Read, ct);
        if (access.IsFailure) return Result<AuditEventPage>.Failure(access.Error);

        if (from is not null && to is not null && from > to)
            return AdminHandlerSupport.Validation<AuditEventPage>("'from' must not be later than 'to'.");

        var query = new AuditEventListQuery(
            Clean(resourceType), Clean(resourceId), Clean(action), actorUserId, from, to, Math.Max(page, 1),
            limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(limit, AdministrationPolicy.MaxPageSize));
        return Result<AuditEventPage>.Success(await _store.ListAsync(access.Value!.OrganizationId, query, ct));
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
