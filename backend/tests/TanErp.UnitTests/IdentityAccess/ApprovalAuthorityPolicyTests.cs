using TanErp.Application.IdentityAccess.ApprovalLimits;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class ApprovalAuthorityPolicyTests
{
    private static readonly Guid Branch = Guid.NewGuid(), OtherBranch = Guid.NewGuid(), RoleA = Guid.NewGuid(), RoleB = Guid.NewGuid();

    private static ApprovalLimitCandidate Limit(Guid role, decimal max, Guid? branch = null) => new(role, branch, false, max);

    private static ApprovalAuthorityOutcome Outcome(decimal amount, params ApprovalLimitCandidate[] rows) =>
        ApprovalAuthorityPolicy.Evaluate(rows, Branch, amount).Outcome;

    [Fact]
    public void NoApplicableRow_IsNotConfigured()
    {
        Assert.Equal(ApprovalAuthorityOutcome.NotConfigured, Outcome(1m));
        Assert.Equal(ApprovalAuthorityOutcome.NotConfigured, Outcome(1m, Limit(RoleA, 1_000m, OtherBranch)));
    }

    [Theory]
    [InlineData(1000.0, ApprovalAuthorityOutcome.Allowed)]          // equal to the ceiling passes
    [InlineData(1000.01, ApprovalAuthorityOutcome.LimitExceeded)]
    [InlineData(-1000.01, ApprovalAuthorityOutcome.LimitExceeded)]  // the sign is ignored
    public void SingleRow_ComparesTheAbsoluteAmountToTheCeiling(double amount, ApprovalAuthorityOutcome expected) =>
        Assert.Equal(expected, Outcome((decimal)amount, Limit(RoleA, 1_000m)));

    [Fact]
    public void AcrossRoles_TheHighestCeilingWins_AndUnlimitedBeatsANumber()
    {
        Assert.Equal(ApprovalAuthorityOutcome.Allowed, Outcome(40_000m, Limit(RoleA, 1_000m), Limit(RoleB, 50_000m)));
        Assert.Equal(ApprovalAuthorityOutcome.LimitExceeded, Outcome(50_000.01m, Limit(RoleA, 1_000m), Limit(RoleB, 50_000m)));
        Assert.Equal(ApprovalAuthorityOutcome.Allowed, Outcome(9_999_999m, Limit(RoleA, 1m), new ApprovalLimitCandidate(RoleB, null, true, null)));
    }

    [Fact]
    public void PerRole_ABranchRowOverridesTheOrganizationRowEvenWhenLower_AndOtherRolesAreUnaffected()
    {
        var restricted = new[] { Limit(RoleA, 100_000m), Limit(RoleA, 5_000m, Branch) };

        Assert.Equal(ApprovalAuthorityOutcome.LimitExceeded, Outcome(6_000m, restricted));
        Assert.Equal(ApprovalAuthorityOutcome.Allowed, Outcome(20_000m, restricted.Append(Limit(RoleB, 20_000m)).ToArray()));
        Assert.Equal(ApprovalAuthorityOutcome.Allowed, Outcome(99_000m, Limit(RoleA, 100_000m), Limit(RoleA, 5_000m, OtherBranch)));
    }
}
