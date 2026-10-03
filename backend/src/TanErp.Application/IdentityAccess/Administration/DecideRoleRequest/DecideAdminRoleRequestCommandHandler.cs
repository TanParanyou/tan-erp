using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.DecideRoleRequest;

public class DecideAdminRoleRequestHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public DecideAdminRoleRequestHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminRoleRequest>> Handle(DecideAdminRoleRequestCommand command, CancellationToken cancellationToken = default)
    {
        // Cancelling is the requester's own action (roles.assign); approving or rejecting is the checker's (roles.assign-approval).
        var permission = command.Decision == RoleRequestDecision.Cancel
            ? AdministrationPermissions.RolesAssign
            : AdministrationPermissions.RolesAssignApproval;
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, permission, cancellationToken);
        if (access.IsFailure) return Result<AdminRoleRequest>.Failure(access.Error);

        var a = access.Value!;
        return await _store.DecideRoleRequestAsync(
            a.OrganizationId, command.RequestId, command.ExpectedRowVersion, command.Decision,
            new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
