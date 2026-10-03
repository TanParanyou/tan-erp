using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.UpdateMembership;

public class UpdateAdminMembershipHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public UpdateAdminMembershipHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(UpdateAdminMembershipCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.MembershipsManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        if (command.StartsAtUtc.HasValue && command.ExpiresAtUtc.HasValue && command.ExpiresAtUtc <= command.StartsAtUtc)
            return AdminHandlerSupport.Validation<AdminUser>("Expiry must be after the start time.");

        var a = access.Value!;
        return await _store.UpdateMembershipAsync(a.OrganizationId, command.MembershipId, command.ExpectedRowVersion, command.BranchId, command.StartsAtUtc, command.ExpiresAtUtc, new AdminActor(a.ActorUserId, a.MembershipId), command.TraceId, cancellationToken);
    }
}
