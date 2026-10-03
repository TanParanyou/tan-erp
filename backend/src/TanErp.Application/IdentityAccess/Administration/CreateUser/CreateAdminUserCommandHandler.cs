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

        var roles = (command.RoleIds ?? Array.Empty<Guid>()).Distinct().ToList();
        if (roles.Count == 0 || roles.Count > AdministrationPolicy.MaxRolesPerRequest || roles.Contains(Guid.Empty))
            return AdminHandlerSupport.Validation<AdminUser>("Between 1 and 20 roles are required.");

        var payloadHash = Sha256Hex.Compute(
            $"{name}|{User.NormalizeEmail(email)}|{command.BranchId}|{string.Join(",", roles.OrderBy(id => id))}");
        return await _store.CreateUserAsync(
            access!.OrganizationId,
            new CreateAdminUserInput(name, email!, command.BranchId, roles),
            new AdminActor(access.ActorUserId, access.MembershipId),
            Sha256Hex.Compute(command.IdempotencyKey),
            payloadHash,
            command.TraceId,
            cancellationToken);
    }
}
