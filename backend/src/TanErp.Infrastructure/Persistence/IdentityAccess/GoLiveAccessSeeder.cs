using Microsoft.EntityFrameworkCore;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

/// <summary>One approval limit requested in config (ApprovalLimits:Seed). RoleCode matches the role name, case-insensitively.</summary>
public sealed class ApprovalLimitSeedEntry
{
    public string? DocumentType { get; set; }
    public string? RoleCode { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Currency { get; set; }
}

/// <summary>
/// Opt-in, idempotent go-live seeding (ADR 0027 D-C). Production has no other seeder: the Test seeder never runs there.
/// Both steps only add missing rows and never change an existing one.
/// </summary>
public static class GoLiveAccessSeeder
{
    /// <summary>Governance (G-03) and audit keys every system role must hold so a fresh deployment is manageable.</summary>
    public static readonly IReadOnlyList<(string Key, string Description)> SystemAdministratorKeys = new[]
    {
        ("organizations.read", "Read Organization"),
        ("organizations.manage", "Manage Organization Profile"),
        ("branches.manage", "Manage Branches"),
        ("users.read", "Read Users and Memberships"),
        ("users.manage", "Manage Users"),
        ("memberships.manage", "Manage Memberships"),
        ("roles.assign", "Assign Roles"),
        ("roles.assign-approval", "Approve Role Assignments"),
        ("roles.manage", "Manage Roles"),
        ("approval-limits.manage", "Manage Approval Limits"),
        ("audit.read", "Read Audit Trail")
    };

    /// <summary>Ensures the permission rows exist and grants each to every active system (is_system) role at its own organization scope.</summary>
    public static async Task<int> EnsureSystemAdministratorPermissionsAsync(AppDbContext db, CancellationToken ct = default)
    {
        var permissions = new List<Permission>();
        foreach (var (key, description) in SystemAdministratorKeys)
        {
            var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Key == key, ct);
            if (permission is null)
            {
                permission = Permission.Create(key, description);
                db.Permissions.Add(permission);
            }
            permissions.Add(permission);
        }

        var added = 0;
        var roles = await db.Roles.Where(r => r.IsSystem && r.IsActive).ToListAsync(ct);
        foreach (var role in roles)
        {
            foreach (var permission in permissions)
            {
                var exists = await db.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == role.Id && rp.PermissionId == permission.Id
                    && rp.Scope == PermissionScope.Organization && rp.ScopeId == role.OrganizationId, ct);
                if (exists) continue;
                db.RolePermissions.Add(new RolePermission(
                    Guid.NewGuid(), role.Id, role.OrganizationId, permission.Id, PermissionScope.Organization, role.OrganizationId));
                added++;
            }
        }

        await db.SaveChangesAsync(ct);
        return added;
    }

    /// <summary>
    /// Creates organization-wide limits that do not exist yet (active or not) and audits each. Invalid entries and entries with
    /// no matching role are skipped and reported, never guessed. Returns one human-readable line per skipped or created entry.
    /// </summary>
    public static async Task<IReadOnlyList<string>> SeedApprovalLimitsAsync(
        AppDbContext db, IReadOnlyList<ApprovalLimitSeedEntry> entries, DateTimeOffset now, CancellationToken ct = default)
    {
        var report = new List<string>();
        foreach (var entry in entries)
        {
            var label = $"{entry.DocumentType}/{entry.RoleCode}";
            var documentType = entry.DocumentType?.Trim().ToLowerInvariant();
            var currency = entry.Currency?.Trim().ToUpperInvariant();
            var roleCode = entry.RoleCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(roleCode) || entry.MaxAmount is null)
            {
                report.Add($"skipped {label}: roleCode and maxAmount are required");
                continue;
            }

            try
            {
                ApprovalLimitRules.ValidateCreate(documentType, currency, isUnlimited: false, entry.MaxAmount);
            }
            catch (ApprovalLimitDomainException ex)
            {
                report.Add($"skipped {label}: {ex.Message}");
                continue;
            }

            var roles = await db.Roles.Where(r => r.NormalizedName == roleCode && r.IsActive).ToListAsync(ct);
            if (roles.Count == 0) report.Add($"skipped {label}: no active role with this name");

            foreach (var role in roles)
            {
                var exists = await db.ApprovalLimits.AnyAsync(l =>
                    l.OrganizationId == role.OrganizationId && l.RoleId == role.Id && l.DocumentType == documentType
                    && l.BranchId == null && l.Currency == currency, ct);
                if (exists)
                {
                    report.Add($"kept {label}: a limit already exists");
                    continue;
                }

                // Audit needs a real user; attribute to the organization's oldest membership.
                var actor = await db.Memberships.AsNoTracking().Where(m => m.OrganizationId == role.OrganizationId)
                    .OrderBy(m => m.CreatedAtUtc).Select(m => new { m.Id, m.UserId }).FirstOrDefaultAsync(ct);
                if (actor is null)
                {
                    report.Add($"skipped {label}: organization has no membership to attribute the audit to");
                    continue;
                }

                var limit = ApprovalLimit.Create(
                    Guid.NewGuid(), role.OrganizationId, role.Id, documentType!, null, currency!, isUnlimited: false, entry.MaxAmount, now);
                db.ApprovalLimits.Add(limit);
                db.AuditEvents.Add(new AuditEvent(
                    Guid.NewGuid(), role.OrganizationId, actor.UserId, "approval-limits.seeded", "ApprovalLimit", limit.Id.ToString(),
                    now, "config-seed",
                    System.Text.Json.JsonSerializer.Serialize(new { roleId = role.Id, documentType, currency, isUnlimited = false, maxAmount = entry.MaxAmount }),
                    actorMembershipId: actor.Id));
                report.Add($"created {label}");
            }
        }

        await db.SaveChangesAsync(ct);
        return report;
    }
}
