using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RolePermissionTests
{
    private readonly Guid _roleId = Guid.NewGuid();
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _permId = Guid.NewGuid();
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public void RolePermission_OrganizationScope_ValidCreation_SetsScopeIdToOrganizationId()
    {
        var rp = new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Organization);

        Assert.Equal(_orgId, rp.OrganizationId);
        Assert.Equal(_orgId, rp.ScopeId);
        Assert.Null(rp.BranchId);
        Assert.Equal(PermissionScope.Organization, rp.Scope);
    }

    [Fact]
    public void RolePermission_OrganizationScope_ExplicitMatchingScopeId_Succeeds()
    {
        var rp = new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Organization, scopeId: _orgId);

        Assert.Equal(_orgId, rp.ScopeId);
        Assert.Null(rp.BranchId);
    }

    [Fact]
    public void RolePermission_OrganizationScope_MismatchedScopeId_ThrowsArgumentException()
    {
        var foreignOrgId = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Organization, scopeId: foreignOrgId));
    }

    [Fact]
    public void RolePermission_OrganizationScope_WithBranchId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Organization, branchId: _branchId));
    }

    [Fact]
    public void RolePermission_BranchScope_ValidCreation_SetsBranchIdAndScopeId()
    {
        var rp = new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Branch, branchId: _branchId);

        Assert.Equal(_orgId, rp.OrganizationId);
        Assert.Equal(_branchId, rp.BranchId);
        Assert.Equal(_branchId, rp.ScopeId);
        Assert.Equal(PermissionScope.Branch, rp.Scope);
    }

    [Fact]
    public void RolePermission_BranchScope_MissingBranchId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Branch));
    }

    [Fact]
    public void RolePermission_BranchScope_MismatchedScopeIdAndBranchId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Branch, scopeId: Guid.NewGuid(), branchId: _branchId));
    }

    [Fact]
    public void RolePermission_OwnScope_ValidCreation_LeavesScopeIdAndBranchIdNull()
    {
        var rp = new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Own);

        Assert.Equal(_orgId, rp.OrganizationId);
        Assert.Null(rp.ScopeId);
        Assert.Null(rp.BranchId);
        Assert.Equal(PermissionScope.Own, rp.Scope);
    }

    [Fact]
    public void RolePermission_OwnScope_WithScopeId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, _permId, PermissionScope.Own, scopeId: _orgId));
    }

    [Fact]
    public void RolePermission_EmptyRoleId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), Guid.Empty, _orgId, _permId, PermissionScope.Organization));
    }

    [Fact]
    public void RolePermission_EmptyOrganizationId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, Guid.Empty, _permId, PermissionScope.Organization));
    }

    [Fact]
    public void RolePermission_EmptyPermissionId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RolePermission(
            Guid.NewGuid(), _roleId, _orgId, Guid.Empty, PermissionScope.Organization));
    }
}
