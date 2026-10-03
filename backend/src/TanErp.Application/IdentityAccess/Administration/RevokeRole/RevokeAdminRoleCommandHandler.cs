using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.RevokeRole;

public class RevokeAdminRoleHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public RevokeAdminRoleHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(RevokeAdminRoleCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.RevokeRoleAsync(a.OrganizationId, command.MembershipId, command.RoleId, new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
