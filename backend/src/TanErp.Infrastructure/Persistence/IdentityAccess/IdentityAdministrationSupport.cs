using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

/// <summary>Queries and audit helpers shared by the identity and role administration stores (G-03b).</summary>
internal static class IdentityAdministrationSupport
{
    /// <summary>Permissions a membership holds through its active roles (the anti-escalation baseline).</summary>
    public static async Task<List<PermissionGrant>> LoadHeldPermissionsAsync(AppDbContext db, Guid membershipId, CancellationToken cancellationToken) =>
        await db.MembershipRoles.AsNoTracking()
            .Where(mr => mr.MembershipId == membershipId && mr.Role!.IsActive)
            .SelectMany(mr => mr.Role!.RolePermissions)
            .Where(rp => rp.Permission!.IsActive)
            .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId))
            .ToListAsync(cancellationToken);

    /// <summary>Rows of active administrators (users.manage at organization scope) used for the last-administrator guard.</summary>
    public static async Task<List<AdministratorRow>> LoadAdministratorRowsAsync(
        AppDbContext db, Guid organizationId, DateTimeOffset now, CancellationToken cancellationToken) =>
        await db.Memberships.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.IsActive)
            .Where(m => m.User!.IsActive && m.User.FirebaseUid != null && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .Where(mr => mr.Role!.RolePermissions.Any(rp =>
                    rp.Permission!.IsActive &&
                    rp.Permission.Key == AdministrationPermissions.UsersManage &&
                    rp.Scope == PermissionScope.Organization &&
                    rp.ScopeId == organizationId))
                .Select(mr => new AdministratorRow(m.UserId, m.Id, mr.RoleId)))
            .ToListAsync(cancellationToken);

    public static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    public static void AddAudit(
        AppDbContext db, Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId, string traceId,
        DateTimeOffset now, object changes, Guid? rowVersionBefore = null, Guid? rowVersionAfter = null)
    {
        db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            actor.UserId,
            action,
            resourceType,
            resourceId,
            now,
            traceId,
            JsonSerializer.Serialize(changes),
            actorMembershipId: actor.MembershipId,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: rowVersionAfter));
    }
}
