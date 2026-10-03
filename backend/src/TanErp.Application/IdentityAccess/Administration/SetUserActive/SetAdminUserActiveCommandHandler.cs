using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.SetUserActive;

public class SetAdminUserActiveHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public SetAdminUserActiveHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(SetAdminUserActiveCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.UsersManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.SetUserActiveAsync(a.OrganizationId, command.UserId, command.ExpectedRowVersion, command.Active, new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
