using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class RequestAccessResolverTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;
    private IRequestAccessResolver _resolver = null!;

    private class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);

        _resolver = new RequestAccessResolver(_db, new SystemClock());
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task ResolveAsync_WhenActiveOrganizationPermission_ReturnsSuccessWithContext()
    {
        var result = await _resolver.ResolveAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(TestOnlyDataSeeder.TestUserId, result.Value.ActorUserId);
        Assert.Equal(TestOnlyDataSeeder.TestMembershipId, result.Value.MembershipId);
        Assert.Equal(TestOnlyDataSeeder.TestOrgId, result.Value.OrganizationId);
        Assert.Equal(TestOnlyDataSeeder.TestBranchId, result.Value.BranchId);
        Assert.Equal("organizations.read", result.Value.PermissionKey);
        Assert.Equal(PermissionScope.Organization, result.Value.PermissionScope);
    }

    [Fact]
    public async Task ResolveAsync_WhenMissingPermission_ReturnsPermissionDenied()
    {
        var result = await _resolver.ResolveAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "nonexistent.permission");

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task ResolveAsync_WhenMembershipBelongsToAnotherUid_ReturnsActiveMembershipRequired()
    {
        var result = await _resolver.ResolveAsync(
            "another-firebase-uid",
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read");

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task ResolveAsync_WhenBranchScopedPermission_ReturnsPermissionDeniedInSlice1()
    {
        // Seed a branch-scoped permission
        var branchPerm = Permission.Create("branch.action", "Branch Action");
        _db.Permissions.Add(branchPerm);

        var branchRole = new Role(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, "Branch Role", "Branch Role", isActive: true);
        _db.Roles.Add(branchRole);

        var rolePerm = new RolePermission(Guid.NewGuid(), branchRole.Id, TestOnlyDataSeeder.TestOrgId, branchPerm.Id, PermissionScope.Branch, TestOnlyDataSeeder.TestBranchId, TestOnlyDataSeeder.TestBranchId);
        _db.RolePermissions.Add(rolePerm);

        var mr = new MembershipRole(TestOnlyDataSeeder.TestMembershipId, branchRole.Id, TestOnlyDataSeeder.TestOrgId);
        _db.MembershipRoles.Add(mr);
        await _db.SaveChangesAsync();

        var result = await _resolver.ResolveAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "branch.action");

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task ResolveAsync_WhenPermissionInactive_ReturnsPermissionDenied()
    {
        var permission = await _db.Permissions.FirstAsync(p => p.Key == "organizations.read");
        permission.Deactivate();
        await _db.SaveChangesAsync();

        var result = await _resolver.ResolveAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read");

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);

        var membership = await _db.Memberships.FirstAsync(m => m.Id == TestOnlyDataSeeder.TestMembershipId);
        Assert.True(membership.IsActive);
    }

    [Fact]
    public async Task ResolveAsync_WhenBranchInactive_ReturnsPermissionDenied()
    {
        var branch = await _db.Branches.FirstAsync(b => b.Id == TestOnlyDataSeeder.TestBranchId);
        branch.Deactivate();
        await _db.SaveChangesAsync();

        var result = await _resolver.ResolveAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read");

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);

        var membership = await _db.Memberships.FirstAsync(m => m.Id == TestOnlyDataSeeder.TestMembershipId);
        Assert.True(membership.IsActive);
    }

    [Fact]
    public async Task ResolveBranchAccessAsync_WhenTargetBranchNotFound_ReturnsBranchNotFound()
    {
        var result = await _resolver.ResolveBranchAccessAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read",
            Guid.NewGuid()); // Nonexistent branch

        Assert.True(result.IsFailure);
        Assert.Equal("BRANCH_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task ResolveBranchAccessAsync_WhenTargetBranchInactive_ReturnsBranchInactive()
    {
        var targetBranch = new Branch(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, "B-DEACT", "Deactivated Branch", false, DateTimeOffset.UtcNow);
        _db.Branches.Add(targetBranch);
        await _db.SaveChangesAsync();

        var result = await _resolver.ResolveBranchAccessAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read",
            targetBranch.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("BRANCH_INACTIVE", result.Error.Code);
    }

    [Fact]
    public async Task ResolveBranchAccessAsync_WhenSameBranch_ReturnsSuccess()
    {
        var result = await _resolver.ResolveBranchAccessAsync(
            TestOnlyDataSeeder.TestFirebaseUid,
            TestOnlyDataSeeder.TestMembershipId,
            "organizations.read",
            TestOnlyDataSeeder.TestBranchId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(TestOnlyDataSeeder.TestBranchId, result.Value.BranchId);
    }

    [Fact]
    public async Task ResolveBranchAccessAsync_WhenCrossBranchWithoutOrgWidePermission_ReturnsPermissionDenied()
    {
        // Target branch is Branch B (different branch in Org A if exists, or another branch)
        var anotherBranchInOrgA = new Branch(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, "B02", "สาขา 2", true, DateTimeOffset.UtcNow);
        _db.Branches.Add(anotherBranchInOrgA);
        await _db.SaveChangesAsync();

        // Seed a role with ONLY items.read (scoped to branch, not org-wide management)
        var roleBranchOnly = new Role(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, "Branch Only User", "Branch Only Role", isActive: true);
        _db.Roles.Add(roleBranchOnly);

        var readPerm = await _db.Permissions.FirstAsync(p => p.Key == "items.read");
        var rolePerm = new RolePermission(Guid.NewGuid(), roleBranchOnly.Id, TestOnlyDataSeeder.TestOrgId, readPerm.Id, PermissionScope.Organization, TestOnlyDataSeeder.TestOrgId);
        _db.RolePermissions.Add(rolePerm);

        // Membership belongs to TestBranchId
        var branchOnlyUser = new User(Guid.NewGuid(), "branch-user-uid", "branch-user@example.test", "Branch User", true, DateTimeOffset.UtcNow);
        _db.Users.Add(branchOnlyUser);

        var membership = new Membership(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestBranchId, branchOnlyUser.Id, true, DateTimeOffset.UtcNow, null);
        _db.Memberships.Add(membership);

        var mr = new MembershipRole(membership.Id, roleBranchOnly.Id, TestOnlyDataSeeder.TestOrgId);
        _db.MembershipRoles.Add(mr);
        await _db.SaveChangesAsync();

        // User belongs to TestBranchId, attempts to access anotherBranchInOrgA with items.read
        var result = await _resolver.ResolveBranchAccessAsync(
            "branch-user-uid",
            membership.Id,
            "items.read",
            anotherBranchInOrgA.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }
}
