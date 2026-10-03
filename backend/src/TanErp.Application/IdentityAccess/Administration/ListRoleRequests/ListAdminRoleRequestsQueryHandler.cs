using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.ListRoleRequests;

public class ListAdminRoleRequestsHandler
{
    private static readonly HashSet<string> RequestStatuses = new(StringComparer.Ordinal) { "pending", "approved", "rejected", "cancelled" };

    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public ListAdminRoleRequestsHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<AdminRoleRequest>>> Handle(ListAdminRoleRequestsQuery query, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(query.Caller.FirebaseUid, query.Caller.MembershipId, AdministrationPermissions.RolesAssignApproval, cancellationToken);
        if (access.IsFailure) return Result<IReadOnlyList<AdminRoleRequest>>.Failure(access.Error);

        var status = string.IsNullOrWhiteSpace(query.Status) ? "pending" : query.Status.Trim();
        if (!RequestStatuses.Contains(status))
            return AdminHandlerSupport.Validation<IReadOnlyList<AdminRoleRequest>>("Unknown request status filter.");

        return Result<IReadOnlyList<AdminRoleRequest>>.Success(
            await _store.ListRoleRequestsAsync(access.Value!.OrganizationId, status, cancellationToken));
    }
}
