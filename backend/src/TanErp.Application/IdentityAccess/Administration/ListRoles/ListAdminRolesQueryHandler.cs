using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.ListRoles;

public class ListAdminRolesHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public ListAdminRolesHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<AdminRole>>> Handle(ListAdminRolesQuery query, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(query.Caller.FirebaseUid, query.Caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<IReadOnlyList<AdminRole>>.Failure(access.Error);

        var roles = await _store.ListRolesAsync(access.Value!.OrganizationId, access.Value.MembershipId, cancellationToken);
        return Result<IReadOnlyList<AdminRole>>.Success(roles);
    }
}
