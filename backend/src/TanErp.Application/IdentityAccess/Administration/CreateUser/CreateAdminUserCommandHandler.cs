using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Security;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Administration.CreateUser;

public class CreateAdminUserHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public CreateAdminUserHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(CreateAdminUserCommand command, CancellationToken cancellationToken = default)
    {
        // Creating a user also creates its membership and assigns roles, so the caller needs all three permissions.
        RequestAccessContext? access = null;
        foreach (var permission in new[]
                 {
                     AdministrationPermissions.UsersManage,
                     AdministrationPermissions.MembershipsManage,
                     AdministrationPermissions.RolesAssign
                 })
        {
            var resolved = await _access.ResolveAsync(command.Caller.FirebaseUid, command.Caller.MembershipId, permission, cancellationToken);
            if (resolved.IsFailure) return Result<AdminUser>.Failure(resolved.Error);
            access ??= resolved.Value;
        }

        var name = command.DisplayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            return AdminHandlerSupport.Validation<AdminUser>("Display name is required (max 255 characters).");

        var email = command.Email?.Trim();
        if (!AdminHandlerSupport.IsPlausibleEmail(email))
            return AdminHandlerSupport.Validation<AdminUser>("A valid email is required.");

        var phone = command.PhoneNumber?.Trim();
        if (phone is { Length: > 50 })
            return AdminHandlerSupport.Validation<AdminUser>("Phone number must not exceed 50 characters.");

        var code = command.EmployeeCode?.Trim();
        if (code is { Length: > 50 })
            return AdminHandlerSupport.Validation<AdminUser>("Employee code must not exceed 50 characters.");

        var notes = command.Notes?.Trim();
        if (notes is { Length: > 1000 })
            return AdminHandlerSupport.Validation<AdminUser>("Notes must not exceed 1000 characters.");

        if (command.StartsAtUtc.HasValue && command.ExpiresAtUtc.HasValue && command.StartsAtUtc > command.ExpiresAtUtc)
            return AdminHandlerSupport.Validation<AdminUser>("Starts at must be before or equal to expires at.");

        var roles = (command.RoleIds ?? Array.Empty<Guid>()).Distinct().ToList();
        if (roles.Count == 0 || roles.Count > AdministrationPolicy.MaxRolesPerRequest || roles.Contains(Guid.Empty))
            return AdminHandlerSupport.Validation<AdminUser>("Between 1 and 20 roles are required.");

        var payloadHash = Sha256Hex.Compute(
            $"{name}|{User.NormalizeEmail(email)}|{command.BranchId}|{command.DepartmentId}|{command.JobTitleId}|{string.Join(",", roles.OrderBy(id => id))}");

        var input = new CreateAdminUserInput(
            name,
            email!,
            command.BranchId,
            roles,
            phone,
            code,
            command.DepartmentId,
            command.JobTitleId,
            command.AvatarFileId,
            notes,
            command.StartsAtUtc,
            command.ExpiresAtUtc);

        return await _store.CreateUserAsync(
            access!.OrganizationId,
            input,
            new AdminActor(access.ActorUserId, access.MembershipId),
            Sha256Hex.Compute(command.IdempotencyKey),
            payloadHash,
            command.TraceId,
            cancellationToken);
    }
}
