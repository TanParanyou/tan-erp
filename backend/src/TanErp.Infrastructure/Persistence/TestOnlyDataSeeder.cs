using Microsoft.EntityFrameworkCore;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence;

public static class TestOnlyDataSeeder
{
    public static readonly Guid TestUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a10");
    public static readonly Guid TestMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a11");
    public static readonly Guid TestOrgId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12");
    public static readonly Guid TestBranchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a13");
    public const string TestFirebaseUid = "foundation-user-test-only";
    public const string TestUserEmail = "foundation-user@example.test";
    public const string TestUserDisplayName = "ผู้ใช้ TEST_ONLY";

    public static async Task SeedAsync(AppDbContext db, string environmentName, bool seedTestData)
    {
        // Environment-lock: strictly only run in Test environment when SeedTestData is true
        if (!string.Equals(environmentName, "Test", StringComparison.OrdinalIgnoreCase) || !seedTestData)
        {
            return;
        }

        // Ensure database schema is migrated
        await db.Database.MigrateAsync();

        // Check if already seeded (idempotency)
        if (await db.Users.AnyAsync(u => u.FirebaseUid == TestFirebaseUid))
        {
            return;
        }

        // Seed Organization
        var org = await db.Organizations.FindAsync(TestOrgId);
        if (org == null)
        {
            org = new Organization(TestOrgId, "TEST_ONLY Project ERP");
            db.Organizations.Add(org);
        }

        // Seed Branch
        var branch = await db.Branches.FindAsync(TestBranchId);
        if (branch == null)
        {
            branch = new Branch(TestBranchId, TestOrgId, "B01", "สาขาทดสอบ");
            db.Branches.Add(branch);
        }

        // Seed User
        var user = new User(TestUserId, TestFirebaseUid, TestUserDisplayName, TestUserEmail, isActive: true);
        db.Users.Add(user);

        // Seed Membership
        var membership = await db.Memberships.FindAsync(TestMembershipId);
        if (membership == null)
        {
            membership = new Membership(TestMembershipId, TestOrgId, TestBranchId, TestUserId, isActive: true);
            db.Memberships.Add(membership);
        }

        // Seed Permission
        var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Key == "organizations.read");
        if (permission == null)
        {
            permission = Permission.Create("organizations.read", "Read Organization");
            db.Permissions.Add(permission);
        }

        // Seed Role
        var role = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Admin");
        if (role == null)
        {
            role = new Role(Guid.NewGuid(), TestOrgId, "Test Admin", "Test Admin Role", isActive: true);
            db.Roles.Add(role);

            var rolePermission = new RolePermission(Guid.NewGuid(), role.Id, permission.Id, PermissionScope.Organization, TestOrgId);
            db.RolePermissions.Add(rolePermission);

            var membershipRole = new MembershipRole(TestMembershipId, role.Id, TestOrgId);
            db.MembershipRoles.Add(membershipRole);
        }

        await db.SaveChangesAsync();
    }
}
