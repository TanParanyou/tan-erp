using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Application.IdentityAccess.Administration.UserSecurity;

public sealed record SetAdminUserPasswordCommand(
    AdminCaller Caller,
    Guid UserId,
    Guid ExpectedRowVersion,
    string? NewPassword,
    string TraceId);

public sealed record UpdateAdminUserTwoFactorPolicyCommand(
    AdminCaller Caller,
    Guid UserId,
    Guid ExpectedRowVersion,
    bool RequireTwoFactor,
    string TraceId);

public sealed record ResetAdminUserTwoFactorCommand(
    AdminCaller Caller,
    Guid UserId,
    Guid ExpectedRowVersion,
    string TraceId);

public class AdminUserSecurityHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;
    private readonly IPasswordHasher _passwordHasher;

    public AdminUserSecurityHandler(
        IRequestAccessResolver access,
        IIdentityAdministrationStore store,
        IPasswordHasher passwordHasher)
    {
        _access = access;
        _store = store;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AdminUser>> SetPasswordAsync(
        SetAdminUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(
            command.Caller.FirebaseUid,
            command.Caller.MembershipId,
            AdministrationPermissions.UsersManage,
            cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var passwordValidation = PasswordPolicy.Validate(command.NewPassword);
        if (passwordValidation.IsFailure) return Result<AdminUser>.Failure(passwordValidation.Error);

        var passwordHash = _passwordHasher.HashPassword(command.NewPassword!);
        var a = access.Value!;
        return await _store.SetUserPasswordAsync(
            a.OrganizationId,
            command.UserId,
            command.ExpectedRowVersion,
            passwordHash,
            new AdminActor(a.ActorUserId, a.MembershipId),
            command.TraceId,
            cancellationToken);
    }

    public async Task<Result<AdminUser>> UpdateTwoFactorPolicyAsync(
        UpdateAdminUserTwoFactorPolicyCommand command,
        CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(
            command.Caller.FirebaseUid,
            command.Caller.MembershipId,
            AdministrationPermissions.UsersManage,
            cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.UpdateUserTwoFactorPolicyAsync(
            a.OrganizationId,
            command.UserId,
            command.ExpectedRowVersion,
            command.RequireTwoFactor,
            new AdminActor(a.ActorUserId, a.MembershipId),
            command.TraceId,
            cancellationToken);
    }

    public async Task<Result<AdminUser>> ResetTwoFactorAsync(
        ResetAdminUserTwoFactorCommand command,
        CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(
            command.Caller.FirebaseUid,
            command.Caller.MembershipId,
            AdministrationPermissions.UsersManage,
            cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.ResetUserTwoFactorAsync(
            a.OrganizationId,
            command.UserId,
            command.ExpectedRowVersion,
            new AdminActor(a.ActorUserId, a.MembershipId),
            command.TraceId,
            cancellationToken);
    }
}
