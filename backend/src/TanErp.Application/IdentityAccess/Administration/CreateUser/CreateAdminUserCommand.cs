namespace TanErp.Application.IdentityAccess.Administration.CreateUser;

public sealed record CreateAdminUserCommand(
    AdminCaller Caller, string IdempotencyKey, string? DisplayName, string? Email, Guid? BranchId, IReadOnlyList<Guid>? RoleIds, string TraceId);
