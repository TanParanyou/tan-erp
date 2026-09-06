using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class PermissionScopeTests
{
    [Theory]
    [InlineData("organization")]
    [InlineData("branch")]
    [InlineData("project")]
    [InlineData("own")]
    public void PermissionScope_ValidValues_CreateSuccessfully(string validScope)
    {
        var scope = PermissionScope.Create(validScope);
        Assert.Equal(validScope, scope.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("tenant")]
    public void PermissionScope_InvalidValues_ThrowsArgumentException(string invalidScope)
    {
        Assert.Throws<ArgumentException>(() => PermissionScope.Create(invalidScope));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Permission_BlankKey_ThrowsArgumentException(string? blankKey)
    {
        Assert.Throws<ArgumentException>(() => Permission.Create(blankKey!));
    }

    [Fact]
    public void Permission_ValidKey_CreatesSuccessfully()
    {
        var permission = Permission.Create("organizations.read");
        Assert.Equal("organizations.read", permission.Key);
    }

    [Fact]
    public void Membership_IsActiveAt_ReturnsTrue_WhenAllConditionsMet()
    {
        var org = new Organization(Guid.NewGuid(), "Org", isActive: true);
        var branch = new Branch(Guid.NewGuid(), org.Id, "B01", "Main Branch", isActive: true);
        var user = new User(Guid.NewGuid(), "uid-123", "User", "user@test.com", isActive: true);

        var now = DateTimeOffset.UtcNow;
        var membership = new Membership(
            Guid.NewGuid(),
            org.Id,
            branch.Id,
            user.Id,
            isActive: true,
            startsAtUtc: now.AddDays(-1),
            expiresAtUtc: now.AddDays(1))
        {
            Organization = org,
            Branch = branch,
            User = user
        };

        Assert.True(membership.IsActiveAt(now));
    }

    [Fact]
    public void Membership_IsActiveAt_ReturnsFalse_WhenUserDisabled()
    {
        var org = new Organization(Guid.NewGuid(), "Org", isActive: true);
        var branch = new Branch(Guid.NewGuid(), org.Id, "B01", "Main Branch", isActive: true);
        var user = new User(Guid.NewGuid(), "uid-123", "User", "user@test.com", isActive: false);

        var now = DateTimeOffset.UtcNow;
        var membership = new Membership(
            Guid.NewGuid(),
            org.Id,
            branch.Id,
            user.Id,
            isActive: true,
            startsAtUtc: now.AddDays(-1),
            expiresAtUtc: now.AddDays(1))
        {
            Organization = org,
            Branch = branch,
            User = user
        };

        Assert.False(membership.IsActiveAt(now));
    }

    [Fact]
    public void Membership_IsActiveAt_ReturnsFalse_WhenOrganizationInactive()
    {
        var org = new Organization(Guid.NewGuid(), "Org", isActive: false);
        var branch = new Branch(Guid.NewGuid(), org.Id, "B01", "Main Branch", isActive: true);
        var user = new User(Guid.NewGuid(), "uid-123", "User", "user@test.com", isActive: true);

        var now = DateTimeOffset.UtcNow;
        var membership = new Membership(
            Guid.NewGuid(),
            org.Id,
            branch.Id,
            user.Id,
            isActive: true,
            startsAtUtc: now.AddDays(-1),
            expiresAtUtc: now.AddDays(1))
        {
            Organization = org,
            Branch = branch,
            User = user
        };

        Assert.False(membership.IsActiveAt(now));
    }

    [Fact]
    public void Membership_IsActiveAt_ReturnsFalse_WhenBranchInactive()
    {
        var org = new Organization(Guid.NewGuid(), "Org", isActive: true);
        var branch = new Branch(Guid.NewGuid(), org.Id, "B01", "Main Branch", isActive: false);
        var user = new User(Guid.NewGuid(), "uid-123", "User", "user@test.com", isActive: true);

        var now = DateTimeOffset.UtcNow;
        var membership = new Membership(
            Guid.NewGuid(),
            org.Id,
            branch.Id,
            user.Id,
            isActive: true,
            startsAtUtc: now.AddDays(-1),
            expiresAtUtc: now.AddDays(1))
        {
            Organization = org,
            Branch = branch,
            User = user
        };

        Assert.False(membership.IsActiveAt(now));
    }

    [Fact]
    public void Membership_IsActiveAt_ReturnsFalse_WhenExpired()
    {
        var org = new Organization(Guid.NewGuid(), "Org", isActive: true);
        var branch = new Branch(Guid.NewGuid(), org.Id, "B01", "Main Branch", isActive: true);
        var user = new User(Guid.NewGuid(), "uid-123", "User", "user@test.com", isActive: true);

        var now = DateTimeOffset.UtcNow;
        var membership = new Membership(
            Guid.NewGuid(),
            org.Id,
            branch.Id,
            user.Id,
            isActive: true,
            startsAtUtc: now.AddDays(-10),
            expiresAtUtc: now.AddDays(-1))
        {
            Organization = org,
            Branch = branch,
            User = user
        };

        Assert.False(membership.IsActiveAt(now));
    }
}
