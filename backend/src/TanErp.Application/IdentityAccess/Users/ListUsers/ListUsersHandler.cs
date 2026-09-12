using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Users.ListUsers;

public class ListUsersHandler
{
    private static readonly string[] AllowedPermissions =
    {
        "organizations.read",
        "opportunities.read",
        "opportunities.update"
    };

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IUserReadStore _store;

    public ListUsersHandler(
        IRequestAccessResolver accessResolver,
        IUserReadStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ListUsersResult>> Handle(
        ListUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAnyAsync(
            query.FirebaseUid,
            query.MembershipId,
            AllowedPermissions,
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<ListUsersResult>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var limit = query.Limit;
        if (limit < 1) limit = 25;
        if (limit > 100) limit = 100;

        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        var users = await _store.ListActiveUsersAsync(
            access.OrganizationId,
            query.BranchId,
            search,
            limit,
            cancellationToken);

        return Result<ListUsersResult>.Success(new ListUsersResult(users, users.Count));
    }
}
