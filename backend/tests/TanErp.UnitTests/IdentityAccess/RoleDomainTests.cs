using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleDomainTests
{
    private static readonly Guid OrgId = Guid.NewGuid();

    [Fact]
    public void Normalize_TrimsNameAndTurnsBlankDescriptionIntoNull()
    {
        var (name, description) = RoleRules.Normalize("  ผู้จัดการ  ", "   ");

        Assert.Equal("ผู้จัดการ", name);
        Assert.Null(description);
    }

    [Theory]
    [InlineData("ab", "REQUEST_VALIDATION_FAILED")]
    [InlineData(null, "REQUEST_VALIDATION_FAILED")]
    [InlineData("SYSTEM.Auditor", "ROLE_NAME_RESERVED")]
    public void Normalize_RejectsInvalidNames(string? name, string expectedCode) =>
        Assert.Equal(expectedCode, Assert.Throws<RoleDomainException>(() => RoleRules.Normalize(name, null)).Code);

    [Fact]
    public void Normalize_EnforcesLengthLimits()
    {
        Assert.Equal(100, RoleRules.Normalize(new string('a', 100), null).Name.Length);
        Assert.Throws<RoleDomainException>(() => RoleRules.Normalize(new string('a', 101), null));
        Assert.Throws<RoleDomainException>(() => RoleRules.Normalize("abc", new string('d', 501)));
    }

    [Fact]
    public void CreateCustom_UpdateDetails_Deactivate_Activate_BumpTheVersion()
    {
        var role = Role.CreateCustom(Guid.NewGuid(), OrgId, " Old Name ", null, DateTimeOffset.UtcNow);
        Assert.True(role.IsActive);
        Assert.False(role.IsSystem);
        var versions = new List<Guid> { role.RowVersion };

        role.UpdateDetails("New Name", "d");
        versions.Add(role.RowVersion);
        role.Deactivate();
        versions.Add(role.RowVersion);
        role.Activate();
        versions.Add(role.RowVersion);

        Assert.Equal("NEW NAME", role.NormalizedName);
        Assert.Equal("d", role.Description);
        Assert.True(role.IsActive);
        Assert.Equal(4, versions.Distinct().Count());
    }

    [Fact]
    public void SystemRole_RejectsEveryMutation()
    {
        var role = new Role(Guid.NewGuid(), OrgId, "Built In", "d", isActive: true);
        Assert.False(role.IsSystem); // the existing constructor stays non-system
        role.MarkAsSystemRole();

        foreach (var action in new Action[] { () => role.UpdateDetails("Other Name", null), role.Deactivate, role.Activate, role.EnsureMutable })
            Assert.Equal("ROLE_SYSTEM_IMMUTABLE", Assert.Throws<RoleDomainException>(action).Code);

        Assert.Equal("Built In", role.Name);
        Assert.True(role.IsActive);
    }
}
