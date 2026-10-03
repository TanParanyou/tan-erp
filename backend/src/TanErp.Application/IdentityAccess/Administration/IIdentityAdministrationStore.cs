using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration;

/// <summary>
/// Persistence for identity administration. Every mutating method enforces the invariants
/// (last administrator, anti-escalation, maker-checker, concurrency) in one transaction and writes the audit event.
/// All methods are scoped to <c>organizationId</c>; anything outside it is reported as RESOURCE_NOT_FOUND.
/// </summary>
public interface IIdentityAdministrationStore
{
    Task<AdminUserPage> ListUsersAsync(Guid organizationId, string? search, string? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<AdminUser?> GetUserAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> CreateUserAsync(Guid organizationId, CreateAdminUserInput input, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> RenameUserAsync(Guid organizationId, Guid userId, Guid expectedRowVersion, string displayName, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> SetUserActiveAsync(Guid organizationId, Guid userId, Guid expectedRowVersion, bool active, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> UpdateMembershipAsync(Guid organizationId, Guid membershipId, Guid expectedRowVersion, Guid? branchId, DateTimeOffset? startsAtUtc, DateTimeOffset? expiresAtUtc, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> SetMembershipActiveAsync(Guid organizationId, Guid membershipId, Guid expectedRowVersion, bool active, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AdminRole>> ListRolesAsync(Guid organizationId, Guid actorMembershipId, CancellationToken cancellationToken);

    Task<Result<AssignRoleOutcome>> AssignRoleAsync(Guid organizationId, Guid membershipId, Guid roleId, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<Result<AdminUser>> RevokeRoleAsync(Guid organizationId, Guid membershipId, Guid roleId, AdminActor actor, string traceId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AdminRoleRequest>> ListRoleRequestsAsync(Guid organizationId, string status, CancellationToken cancellationToken);

    Task<Result<AdminRoleRequest>> DecideRoleRequestAsync(Guid organizationId, Guid requestId, Guid expectedRowVersion, RoleRequestDecision decision, AdminActor actor, string traceId, CancellationToken cancellationToken);
}

public enum RoleRequestDecision
{
    Approve,
    Reject,
    Cancel
}
