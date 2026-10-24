using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.UpdateUserProfile;

public class UpdateAdminUserProfileHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public UpdateAdminUserProfileHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(UpdateAdminUserProfileCommand command, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, AdministrationPermissions.UsersManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var name = command.DisplayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            return AdminHandlerSupport.Validation<AdminUser>("Display name is required (max 255 characters).");

        var phone = command.PhoneNumber?.Trim();
        if (phone is { Length: > 50 })
            return AdminHandlerSupport.Validation<AdminUser>("Phone number must not exceed 50 characters.");

        var code = command.EmployeeCode?.Trim();
        if (code is { Length: > 50 })
            return AdminHandlerSupport.Validation<AdminUser>("Employee code must not exceed 50 characters.");

        var notes = command.Notes?.Trim();
        if (notes is { Length: > 1000 })
            return AdminHandlerSupport.Validation<AdminUser>("Notes must not exceed 1000 characters.");

        var input = new UpdateAdminUserProfileInput(
            name,
            phone,
            code,
            command.DepartmentId,
            command.JobTitleId,
            command.AvatarFileId,
            notes);

        return await _store.UpdateUserProfileAsync(
            access.Value!.OrganizationId,
            command.UserId,
            command.ExpectedRowVersion,
            input,
            new AdminActor(access.Value.ActorUserId, access.Value.MembershipId),
            command.TraceId,
            cancellationToken);
    }
}
