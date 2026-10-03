using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.SetMembershipActive;

public class SetAdminMembershipActiveHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public SetAdminMembershipActiveHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(SetAdminMembershipActiveCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.MembershipsManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.SetMembershipActiveAsync(a.OrganizationId, command.MembershipId, command.ExpectedRowVersion, command.Active, new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
