using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class AdministrationPolicyTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid OtherOrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();

    private static PermissionGrant Org(string key, Guid? orgId = null) => new(key, PermissionScope.Organization, orgId ?? OrgId);
    private static PermissionGrant Branch(string key, Guid? branchId = null) => new(key, PermissionScope.Branch, branchId ?? BranchId);

    [Fact]
    public void Covers_WhenCallerHoldsEveryKeyAtOrganizationScope_ReturnsTrue()
    {
        var held = new[] { Org("estimates.read"), Org("estimates.update") };
        var wanted = new[] { Org("estimates.read"), Branch("estimates.update") };

        Assert.True(AdministrationPolicy.Covers(held, OrgId, wanted));
    }

    [Fact]
    public void Covers_WhenAKeyIsMissing_ReturnsFalse()
    {
        var held = new[] { Org("estimates.read") };
        var wanted = new[] { Org("estimates.read"), Org("estimates.approve") };

        Assert.False(AdministrationPolicy.Covers(held, OrgId, wanted));
    }

    [Fact]
    public void Covers_WhenCallerOrganizationScopeBelongsToAnotherOrganization_ReturnsFalse()
    {
        var held = new[] { Org("estimates.read", OtherOrgId) };

        Assert.False(AdministrationPolicy.Covers(held, OrgId, new[] { Org("estimates.read") }));
    }

    [Fact]
    public void Covers_WhenCallerOnlyHoldsABranchScope_CoversThatBranchButNotOrganizationScope()
    {
        var held = new[] { Branch("estimates.read") };

        Assert.True(AdministrationPolicy.Covers(held, OrgId, new[] { Branch("estimates.read") }));
        Assert.False(AdministrationPolicy.Covers(held, OrgId, new[] { Org("estimates.read") }));
        Assert.False(AdministrationPolicy.Covers(held, OrgId, new[] { Branch("estimates.read", Guid.NewGuid()) }));
    }

    [Fact]
    public void Covers_ForARoleWithNoPermissions_ReturnsTrue()
    {
        Assert.True(AdministrationPolicy.Covers(Array.Empty<PermissionGrant>(), OrgId, Array.Empty<PermissionGrant>()));
    }

    [Theory]
    [InlineData("estimates.approve", true)]
    [InlineData("cost-records.approve", true)]
    [InlineData("cost-records.publish", true)]
    [InlineData("estimates.read", false)]
    public void RequiresApproval_FlagsOnlyApprovalPermissions(string key, bool expected)
    {
        Assert.Equal(expected, AdministrationPolicy.RequiresApproval(new[] { "customers.read", key }));
    }

    [Fact]
    public void WouldRemoveLastAdministrator_WhenRemovingTheOnlyAdministrator_ReturnsTrue()
    {
        var admin = new AdministratorRow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.True(AdministrationPolicy.WouldRemoveLastAdministrator(new[] { admin }, row => row.UserId == admin.UserId));
    }

    [Fact]
    public void WouldRemoveLastAdministrator_WhenAnotherAdministratorRemains_ReturnsFalse()
    {
        var first = new AdministratorRow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var second = new AdministratorRow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.False(AdministrationPolicy.WouldRemoveLastAdministrator(new[] { first, second }, row => row.UserId == first.UserId));
    }

    [Fact]
    public void WouldRemoveLastAdministrator_WhenChangeTouchesNoAdministrator_ReturnsFalseEvenWithOneAdmin()
    {
        var admin = new AdministratorRow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.False(AdministrationPolicy.WouldRemoveLastAdministrator(new[] { admin }, row => row.UserId == Guid.NewGuid()));
    }

    [Fact]
    public void WouldRemoveLastAdministrator_WhenOneUserHoldsTwoAdminRoles_RemovingOneRoleKeepsTheUser()
    {
        var userId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var roleA = Guid.NewGuid();
        var roleB = Guid.NewGuid();
        var rows = new[] { new AdministratorRow(userId, membershipId, roleA), new AdministratorRow(userId, membershipId, roleB) };

        Assert.False(AdministrationPolicy.WouldRemoveLastAdministrator(rows, row => row.RoleId == roleA));
        Assert.True(AdministrationPolicy.WouldRemoveLastAdministrator(rows, row => row.MembershipId == membershipId));
    }

    [Fact]
    public void WouldRemoveLastAdministrator_WhenThereAreNoAdministrators_ReturnsFalse()
    {
        Assert.False(AdministrationPolicy.WouldRemoveLastAdministrator(Array.Empty<AdministratorRow>(), _ => true));
    }
}
