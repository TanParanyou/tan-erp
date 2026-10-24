namespace TanErp.Application.IdentityAccess.Administration.CreateUser;

public sealed record CreateAdminUserCommand(
    AdminCaller Caller,
    string IdempotencyKey,
    string? DisplayName,
    string? Email,
    Guid? BranchId,
    IReadOnlyList<Guid>? RoleIds,
    string TraceId,
    string? PhoneNumber = null,
    string? EmployeeCode = null,
    Guid? DepartmentId = null,
    Guid? JobTitleId = null,
    Guid? AvatarFileId = null,
    string? Notes = null,
    DateTimeOffset? StartsAtUtc = null,
    DateTimeOffset? ExpiresAtUtc = null);
