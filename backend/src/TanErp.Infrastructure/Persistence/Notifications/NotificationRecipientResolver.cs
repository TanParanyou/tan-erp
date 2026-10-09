using Microsoft.EntityFrameworkCore;
using TanErp.Application.Notifications;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// Same grant rule as RequestAccessResolver.ResolveAsync (organization-scope grant on an active role/permission, active membership in the
/// time window), asked for every membership at once. A branch-limited membership only matches a document of its own branch.
/// Estimate's branch-scoped grants are not consulted: Estimate notifies its route reviewers explicitly.
/// </summary>
public class NotificationRecipientResolver : INotificationRecipientResolver
{
    private readonly AppDbContext _db;

    public NotificationRecipientResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Guid>> ResolveAsync(
        Guid organizationId, Guid? branchId, string permissionKey, IReadOnlyCollection<Guid> excludedUserIds, DateTimeOffset atUtc, CancellationToken ct = default)
    {
        var excluded = excludedUserIds.ToArray();
        return await _db.Memberships
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => branchId == null || m.BranchId == null || m.BranchId == branchId)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= atUtc)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > atUtc)
            .Where(m => !excluded.Contains(m.UserId))
            .Where(m => m.MembershipRoles.Any(mr => mr.Role!.IsActive
                && mr.Role.RolePermissions.Any(rp => rp.Permission!.IsActive
                    && rp.Permission.Key == permissionKey
                    && rp.Scope == PermissionScope.Organization
                    && rp.ScopeId == m.OrganizationId)))
            .Select(m => m.UserId)
            .Distinct()
            .OrderBy(id => id)
            // The planner caps at MaxRecipientsPerEvent after also dropping the actor, hence one spare row.
            .Take(NotificationLimits.MaxRecipientsPerEvent + 1)
            .ToListAsync(ct);
    }
}
