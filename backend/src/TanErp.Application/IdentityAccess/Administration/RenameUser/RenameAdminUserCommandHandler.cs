using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.RenameUser;

public class RenameAdminUserHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public RenameAdminUserHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(RenameAdminUserCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.UsersManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var name = command.DisplayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            return AdminHandlerSupport.Validation<AdminUser>("Display name is required (max 255 characters).");

        var a = access.Value!;
        return await _store.RenameUserAsync(a.OrganizationId, command.UserId, command.ExpectedRowVersion, name, new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
