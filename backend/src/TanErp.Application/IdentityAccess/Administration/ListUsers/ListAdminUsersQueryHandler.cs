using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.ListUsers;

public class ListAdminUsersHandler
{
    private static readonly HashSet<string> UserStatuses = new(StringComparer.Ordinal) { "pending", "active", "inactive" };

    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public ListAdminUsersHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUserPage>> Handle(ListAdminUsersQuery query, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(query.Caller.FirebaseUid, query.Caller.MembershipId, AdministrationPermissions.UsersRead, cancellationToken);
        if (access.IsFailure) return Result<AdminUserPage>.Failure(access.Error);

        if (query.Status is not null && !UserStatuses.Contains(query.Status))
            return AdminHandlerSupport.Validation<AdminUserPage>("Unknown user status filter.");

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? AdminUserSortKey.CreatedAt : query.SortBy.Trim();
        if (!AdminUserSortKey.IsValid(sortBy))
            return Result<AdminUserPage>.Failure(new Error("USER_SORT_INVALID", "Invalid user sort key."));

        var sortOrder = string.IsNullOrWhiteSpace(query.SortOrder) ? "desc" : query.SortOrder.Trim().ToLowerInvariant();
        if (sortOrder is not ("asc" or "desc"))
            return Result<AdminUserPage>.Failure(new Error("USER_SORT_ORDER_INVALID", "Invalid sort order."));

        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        var page = Math.Max(query.Page, 1);
        var limit = query.Limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(query.Limit, AdministrationPolicy.MaxPageSize);
        return Result<AdminUserPage>.Success(await _store.ListUsersAsync(
            access.Value!.OrganizationId, search, query.Status, sortBy, sortOrder == "desc", page, limit, cancellationToken));
    }
}
