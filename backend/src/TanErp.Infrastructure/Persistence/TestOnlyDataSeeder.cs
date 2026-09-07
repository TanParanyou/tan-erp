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

        // Seed Permissions (Identity + CRM)
        var permKeys = new[]
        {
            ("organizations.read", "Read Organization"),
            ("customers.read", "Read Customers"),
            ("customers.create", "Create Customers"),
            ("customer-contacts.manage", "Manage Customer Contacts")
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

        // Seed Role for Org A
        var role = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Admin");
        if (role == null)
        {
            role = new Role(Guid.NewGuid(), TestOrgId, "Test Admin", "Test Admin Role", isActive: true);
            db.Roles.Add(role);

            foreach (var perm in seededPerms)
            {
                var rolePermission = new RolePermission(Guid.NewGuid(), role.Id, TestOrgId, perm.Id, PermissionScope.Organization, TestOrgId);
                db.RolePermissions.Add(rolePermission);
            }

            var membershipRole = new MembershipRole(TestMembershipId, role.Id, TestOrgId);
            db.MembershipRoles.Add(membershipRole);
        }

        // Seed Deterministic Org B Fixture
        var orgB = await db.Organizations.FindAsync(TestOrgBId);
        if (orgB == null)
        {
            orgB = new Organization(TestOrgBId, "TEST_ONLY Organization B");
            db.Organizations.Add(orgB);
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
            membershipB = new Membership(TestMembershipBId, TestOrgBId, null, userB.Id, isActive: true);
            db.Memberships.Add(membershipB);
        }

        var roleB = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgBId && r.Name == "Test Admin B");
        if (roleB == null)
        {
            roleB = new Role(Guid.NewGuid(), TestOrgBId, "Test Admin B", "Test Admin Role B", isActive: true);
            db.Roles.Add(roleB);

            foreach (var perm in seededPerms)
            {
                var rolePermB = new RolePermission(Guid.NewGuid(), roleB.Id, TestOrgBId, perm.Id, PermissionScope.Organization, TestOrgBId);
                db.RolePermissions.Add(rolePermB);
            }

            var membershipRoleB = new MembershipRole(TestMembershipBId, roleB.Id, TestOrgBId);
            db.MembershipRoles.Add(membershipRoleB);
        }

        await db.SaveChangesAsync();
    }
}
