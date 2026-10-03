using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.IdentityAccess.Administration.AssignRole;

public class AssignAdminRoleHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public AssignAdminRoleHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AssignRoleOutcome>> Handle(AssignAdminRoleCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<AssignRoleOutcome>.Failure(access.Error);

        var a = access.Value!;
        return await _store.AssignRoleAsync(
            a.OrganizationId, command.MembershipId, command.RoleId, new AdminActor(a.ActorUserId, a.MembershipId),
            Sha256Hex.Compute(command.IdempotencyKey), Sha256Hex.Compute($"{command.MembershipId}|{command.RoleId}"),
            command.TraceId, cancellationToken);
    }
}
