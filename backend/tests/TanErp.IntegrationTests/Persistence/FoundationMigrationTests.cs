using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class FoundationMigrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Migration_FromZero_CreatesAllExpectedTablesAndConstraints()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        // 1. Verify all 9 tables exist in their respective schemas
        var expectedTables = new[]
        {
            ("organization", "organizations"),
            ("organization", "branches"),
            ("identity_access", "users"),
            ("organization", "memberships"),
            ("identity_access", "roles"),
            ("identity_access", "permissions"),
            ("identity_access", "membership_roles"),
            ("identity_access", "role_permissions"),
            ("audit", "audit_events")
        };

        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();

        foreach (var (schema, table) in expectedTables)
        {
            await using var cmd = new NpgsqlCommand(
                "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = @schema AND table_name = @table);",
                conn);
            cmd.Parameters.AddWithValue("schema", schema);
            cmd.Parameters.AddWithValue("table", table);

            var exists = (bool)(await cmd.ExecuteScalarAsync())!;
            Assert.True(exists, $"Expected table {schema}.{table} to exist.");
        }

        // 2. Assert unique constraint on users.firebase_uid
        var user1 = new User(Guid.NewGuid(), "uid-unique-test", "User 1", "u1@test.com");
        var user2 = new User(Guid.NewGuid(), "uid-unique-test", "User 2", "u2@test.com");
        db.Users.Add(user1);
        await db.SaveChangesAsync();

        db.Users.Add(user2);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());

        // Reset db context
        db.ChangeTracker.Clear();

        // 3. Assert unique constraint on (organization_id, branch_code)
        var org = new Organization(Guid.NewGuid(), "TEST_ONLY Unique Org");
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var branch1 = new Branch(Guid.NewGuid(), org.Id, "B01", "Branch 1");
        var branch2 = new Branch(Guid.NewGuid(), org.Id, "B01", "Branch 1 duplicate");
        db.Branches.Add(branch1);
        await db.SaveChangesAsync();

        db.Branches.Add(branch2);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        // 4. Assert unique constraint on (organization_id, normalized_role_name)
        var role1 = new Role(Guid.NewGuid(), org.Id, "Admin");
        var role2 = new Role(Guid.NewGuid(), org.Id, "admin"); // Normalizes to "ADMIN"
        db.Roles.Add(role1);
        await db.SaveChangesAsync();

        db.Roles.Add(role2);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        // 5. Assert unique constraint on permissions.key
        var perm1 = Permission.Create("test.perm");
        var perm2 = Permission.Create("test.perm");
        db.Permissions.Add(perm1);
        await db.SaveChangesAsync();

        db.Permissions.Add(perm2);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CrossOrganization_And_Rollback_Tests()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        // Seed two TEST_ONLY organizations
        var orgA = new Organization(Guid.NewGuid(), "TEST_ONLY Organization A");
        var orgB = new Organization(Guid.NewGuid(), "TEST_ONLY Organization B");
        db.Organizations.AddRange(orgA, orgB);

        var branchA = new Branch(Guid.NewGuid(), orgA.Id, "BR-A", "Branch A");
        var branchB = new Branch(Guid.NewGuid(), orgB.Id, "BR-B", "Branch B");
        db.Branches.AddRange(branchA, branchB);

        var userA = new User(Guid.NewGuid(), "uid-a", "User A", "a@test.com");
        var userB = new User(Guid.NewGuid(), "uid-b", "User B", "b@test.com");
        db.Users.AddRange(userA, userB);

        var memberA = new Membership(Guid.NewGuid(), orgA.Id, branchA.Id, userA.Id);
        var memberB = new Membership(Guid.NewGuid(), orgB.Id, branchB.Id, userB.Id);
        db.Memberships.AddRange(memberA, memberB);

        var roleA = new Role(Guid.NewGuid(), orgA.Id, "Role A");
        var roleB = new Role(Guid.NewGuid(), orgB.Id, "Role B");
        db.Roles.AddRange(roleA, roleB);

        var perm = Permission.Create("organizations.read");
        db.Permissions.Add(perm);

        var rolePermA = new RolePermission(Guid.NewGuid(), roleA.Id, orgA.Id, perm.Id, PermissionScope.Organization, orgA.Id);
        var rolePermB = new RolePermission(Guid.NewGuid(), roleB.Id, orgB.Id, perm.Id, PermissionScope.Organization, orgB.Id);
        db.RolePermissions.AddRange(rolePermA, rolePermB);

        var memberRoleA = new MembershipRole(memberA.Id, roleA.Id, orgA.Id);
        var memberRoleB = new MembershipRole(memberB.Id, roleB.Id, orgB.Id);
        db.MembershipRoles.AddRange(memberRoleA, memberRoleB);

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var crossBranchMembership = new Membership(
            Guid.NewGuid(), orgA.Id, branchB.Id, userA.Id);
        db.Memberships.Add(crossBranchMembership);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var crossRoleAssignment = new MembershipRole(
            memberA.Id, roleB.Id, orgA.Id);
        db.MembershipRoles.Add(crossRoleAssignment);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        // Negative test: RolePermission with branch from a different organization
        var crossBranchRolePerm = new RolePermission(
            Guid.NewGuid(), roleA.Id, orgA.Id, perm.Id, PermissionScope.Branch, branchB.Id, branchB.Id);
        db.RolePermissions.Add(crossBranchRolePerm);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        // Negative test: RolePermission with organizationId that does not match role's organizationId
        var crossOrgRolePerm = new RolePermission(
            Guid.NewGuid(), roleB.Id, orgA.Id, perm.Id, PermissionScope.Organization, orgA.Id);
        db.RolePermissions.Add(crossOrgRolePerm);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        // Assert joins for Org A never return Membership, Role or Permission assignment from Org B
        var orgAMemberships = await db.Memberships
            .Where(m => m.OrganizationId == orgA.Id)
            .Include(m => m.MembershipRoles)
                .ThenInclude(mr => mr.Role)
                    .ThenInclude(r => r!.RolePermissions)
            .ToListAsync();

        Assert.Single(orgAMemberships);
        Assert.Equal(memberA.Id, orgAMemberships[0].Id);
        Assert.All(orgAMemberships[0].MembershipRoles, mr =>
        {
            Assert.Equal(orgA.Id, mr.Role!.OrganizationId);
            Assert.All(mr.Role.RolePermissions, rp => Assert.Equal(orgA.Id, rp.ScopeId));
        });

        // Rollback test: migrate to 0 then back forward
        var migrator = db.Database.GetService<IMigrator>();
        await migrator.MigrateAsync("0");

        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();
        await using (var cmd = new NpgsqlCommand(
            "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'organization' AND table_name = 'organizations');",
            conn))
        {
            var existsAfterRollback = (bool)(await cmd.ExecuteScalarAsync())!;
            Assert.False(existsAfterRollback, "Table organization.organizations should not exist after rollback to 0.");
        }

        // Migrate forward again
        await migrator.MigrateAsync();

        await using (var cmd = new NpgsqlCommand(
            "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'organization' AND table_name = 'organizations');",
            conn))
        {
            var existsAfterReapply = (bool)(await cmd.ExecuteScalarAsync())!;
            Assert.True(existsAfterReapply, "Table organization.organizations should exist after reapplying migration.");
        }
    }
}
