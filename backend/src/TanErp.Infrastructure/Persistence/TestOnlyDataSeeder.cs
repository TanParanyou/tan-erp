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

    public static readonly Guid TestUserIdB = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b20");
    public static readonly Guid TestMembershipBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b21");
    public static readonly Guid TestOrgBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b22");
    public static readonly Guid TestBranchBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b23");
    public const string TestFirebaseUidB = "foundation-user-b-test-only";
    public const string TestUserEmailB = "foundation-user-b@example.test";

    public static async Task SeedAsync(AppDbContext db, string environmentName, bool seedTestData)
    {
        // Environment-lock: strictly only run in Test environment when SeedTestData is true
        if (!string.Equals(environmentName, "Test", StringComparison.OrdinalIgnoreCase) || !seedTestData)
        {
            return;
        }

        // Ensure database schema is migrated
        await db.Database.MigrateAsync();

        // NOTE: no early return here; every block below is find-or-create so
        // restarts backfill permission keys/links added after the first seed.

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
        var user = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == TestFirebaseUid);
        if (user == null)
        {
            user = new User(TestUserId, TestFirebaseUid, TestUserDisplayName, TestUserEmail, isActive: true);
            db.Users.Add(user);
        }

        // Seed Membership
        var membership = await db.Memberships.FindAsync(TestMembershipId);
        if (membership == null)
        {
            membership = new Membership(TestMembershipId, TestOrgId, TestBranchId, TestUserId, isActive: true);
            db.Memberships.Add(membership);
        }

        // Seed Permissions (Identity + CRM)
        var permKeys = new[]
        {
            ("organizations.read", "Read Organization"),
            ("customers.read", "Read Customers"),
            ("customers.create", "Create Customers"),
            ("customer-contacts.manage", "Manage Customer Contacts"),
            ("customers.activate", "Activate Customers"),
            ("sites.read", "Read Sites"),
            ("sites.manage", "Manage Sites"),
            ("opportunities.read", "Read Opportunities"),
            ("opportunities.create", "Create Opportunities"),
            ("opportunities.update", "Update Opportunities"),
            ("opportunities.transition", "Transition Opportunities"),
            ("surveys.read", "Read Site Surveys"),
            ("surveys.create", "Create Site Surveys"),
            ("surveys.update", "Update Site Surveys"),
            ("surveys.mark-ready", "Mark Site Surveys Ready"),
            ("estimates.read", "Read Estimates"),
            ("estimates.create", "Create Estimates"),
            ("estimates.update", "Update Estimates")
        };


        var seededPerms = new List<Permission>();
        foreach (var (key, desc) in permKeys)
        {
            var perm = await db.Permissions.FirstOrDefaultAsync(p => p.Key == key);
            if (perm == null)
            {
                perm = Permission.Create(key, desc);
                db.Permissions.Add(perm);
            }
            seededPerms.Add(perm);
        }

        // Seed Role for Org A (backfill new permission links for roles seeded before CRM keys existed)
        var role = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Admin");
        if (role == null)
        {
            role = new Role(Guid.NewGuid(), TestOrgId, "Test Admin", "Test Admin Role", isActive: true);
            db.Roles.Add(role);

            var membershipRole = new MembershipRole(TestMembershipId, role.Id, TestOrgId);
            db.MembershipRoles.Add(membershipRole);
        }

        foreach (var perm in seededPerms)
        {
            var exists = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == role.Id && rp.PermissionId == perm.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgId);
            if (!exists)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, TestOrgId, perm.Id, PermissionScope.Organization, TestOrgId));
            }
        }

        // Seed Deterministic Org B Fixture
        var orgB = await db.Organizations.FindAsync(TestOrgBId);
        if (orgB == null)
        {
            orgB = new Organization(TestOrgBId, "TEST_ONLY Organization B");
            db.Organizations.Add(orgB);
        }

        // Seed Branch B
        var branchB = await db.Branches.FindAsync(TestBranchBId);
        if (branchB == null)
        {
            branchB = new Branch(TestBranchBId, TestOrgBId, "B01-B", "สาขาทดสอบ B");
            db.Branches.Add(branchB);
        }

        var userB = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == TestFirebaseUidB);
        if (userB == null)
        {
            userB = new User(TestUserIdB, TestFirebaseUidB, "ผู้ใช้ TEST_ONLY B", TestUserEmailB, isActive: true);
            db.Users.Add(userB);
        }

        var membershipB = await db.Memberships.FindAsync(TestMembershipBId);
        if (membershipB == null)
        {
            membershipB = new Membership(TestMembershipBId, TestOrgBId, TestBranchBId, userB.Id, isActive: true);
            db.Memberships.Add(membershipB);
        }

        var roleB = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgBId && r.Name == "Test Admin B");
        if (roleB == null)
        {
            roleB = new Role(Guid.NewGuid(), TestOrgBId, "Test Admin B", "Test Admin Role B", isActive: true);
            db.Roles.Add(roleB);

            var membershipRoleB = new MembershipRole(TestMembershipBId, roleB.Id, TestOrgBId);
            db.MembershipRoles.Add(membershipRoleB);
        }

        foreach (var perm in seededPerms)
        {
            var existsB = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == roleB.Id && rp.PermissionId == perm.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgBId);
            if (!existsB)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleB.Id, TestOrgBId, perm.Id, PermissionScope.Organization, TestOrgBId));
            }
        }

        await db.SaveChangesAsync();
    }
}
