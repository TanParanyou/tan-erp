using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.RoleManagement;

/// <summary>
/// Persistence for role definitions. Mutating members enforce anti-escalation, maker–checker, last-administrator and
/// concurrency in one transaction and write the audit event. Everything is scoped to <c>organizationId</c>.
/// </summary>
public interface IRoleManagementStore
{
    Task<IReadOnlyList<PermissionCatalogEntry>> ListPermissionsAsync(CancellationToken ct);

    Task<RoleDefinitionPage> ListRolesAsync(Guid organizationId, RoleListQuery query, CancellationToken ct);

    Task<Result<RoleDefinition>> GetRoleAsync(Guid organizationId, Guid roleId, CancellationToken ct);
}
