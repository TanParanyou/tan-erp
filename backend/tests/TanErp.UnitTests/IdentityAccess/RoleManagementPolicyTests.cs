using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleManagementPolicyTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();

    private static PermissionGrant Org(string key) => new(key, PermissionScope.Organization, OrgId);

    private static RoleInput Input(string name = "Sales Lead", params RoleGrantInput[] grants) => new(name, null, grants);

    private static RoleGrantInput G(string key, string scope = "organization", Guid? branchId = null) => new(key, scope, branchId);

    [Fact]
    public void ValidateShape_AcceptsAWellFormedRole()
    {
        Assert.Null(RoleManagementPolicy.ValidateShape(Input("Sales Lead", G("customers.read"), G("estimates.read", "branch", BranchId), G("quotations.read", "own"))));
    }

    [Fact]
    public void ValidateShape_RejectsBadNamesScopeMismatchesBlankKeysAndTooManyGrants()
    {
        Assert.Equal("ROLE_NAME_RESERVED", RoleManagementPolicy.ValidateShape(Input("system.root"))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("ab"))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "branch")))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "organization", BranchId)))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "project")))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G(" ")))!.Code);
        var tooMany = Enumerable.Range(0, RoleManagementPolicy.MaxGrants + 1).Select(i => G($"k{i}.read")).ToArray();
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", tooMany))!.Code);
    }

    [Fact]
    public void Canonicalize_TrimsKeysDropsDuplicatesAndSortsForAStablePayloadHash()
    {
        var canonical = RoleManagementPolicy.Canonicalize(Input("Valid Name", G(" b.read "), G("a.read"), G("b.read")));

        Assert.Equal(new[] { "a.read", "b.read" }, canonical.Permissions.Select(p => p.PermissionKey).ToArray());
    }

    [Fact]
    public void ToGrant_UsesOrganizationIdBranchIdOrNullByScope()
    {
        Assert.Equal(new PermissionGrant("a.read", "organization", OrgId), RoleManagementPolicy.ToGrant(OrgId, "a.read", "organization", null));
        Assert.Equal(new PermissionGrant("a.read", "branch", BranchId), RoleManagementPolicy.ToGrant(OrgId, "a.read", "branch", BranchId));
        Assert.Equal(new PermissionGrant("a.read", "own", null), RoleManagementPolicy.ToGrant(OrgId, "a.read", "own", null));
    }

    [Fact]
    public void Diff_ReturnsAddedAndRemovedGrants()
    {
        var (added, removed) = RoleManagementPolicy.Diff(new[] { Org("a.read"), Org("b.read") }, new[] { Org("b.read"), Org("c.read") });

        Assert.Equal(new[] { Org("c.read") }, added);
        Assert.Equal(new[] { Org("a.read") }, removed);
    }

    [Theory]
    [InlineData(true, "estimates.approve", true)]   // holders + approval role + something added
    [InlineData(false, "estimates.approve", false)] // nobody holds it: the assignment gate (CP-02) applies later
    [InlineData(true, "customers.read", false)]     // role carries no approval permission
    public void NeedsChecker_OnlyWhenHeldRoleThatRequiresApprovalGainsGrants(bool hasHolders, string key, bool expected)
    {
        var added = new[] { Org(key) };

        Assert.Equal(expected, RoleManagementPolicy.NeedsChecker(hasHolders, new[] { key, "customers.read" }, added));
    }

    [Fact]
    public void NeedsChecker_WhenNothingIsAdded_IsFalseEvenForAHeldApprovalRole()
    {
        Assert.False(RoleManagementPolicy.NeedsChecker(true, new[] { "estimates.approve" }, Array.Empty<PermissionGrant>()));
    }
}
