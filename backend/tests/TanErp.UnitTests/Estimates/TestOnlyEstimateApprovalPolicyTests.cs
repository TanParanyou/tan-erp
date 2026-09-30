using TanErp.Domain.Estimates;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public sealed class TestOnlyEstimateApprovalPolicyTests
{
    [Fact]
    public void Resolve_AmountAndLowMarginTriggers_UsesStrongestRouteWithoutDuplicateSteps()
    {
        var route = TestOnlyEstimateApprovalPolicy.Resolve(
            grandTotal: 1_750_000m,
            marginRate: 0.15m,
            sellingBeforeDiscount: 1_750_000m,
            discountAmount: 0m,
            exceptionCodes: []);

        Assert.Equal(
            ["TEST ONLY MANAGER", "TEST ONLY FINANCIAL APPROVER", "TEST ONLY DIRECTOR"],
            route.ReviewerRoleNames);
        Assert.Equal(["AMOUNT_AUTHORITY_EXCEEDED", "MARGIN_BELOW_MINIMUM"],
            route.Triggers.Select(trigger => trigger.Code));
        Assert.Equal(1_500_000m, route.Triggers[0].ThresholdValue);
        Assert.Equal(0.30m, route.Triggers[1].ThresholdValue);
        Assert.Equal(TestOnlyEstimateApprovalPolicy.PolicyCode, route.PolicyCode);
        Assert.Equal(64, route.PolicyHash.Length);
    }

    [Fact]
    public void Resolve_CombinesDiscountAndExceptionTriggersWithoutRepeatingRequiredRoles()
    {
        var route = TestOnlyEstimateApprovalPolicy.Resolve(
            grandTotal: 250_000m,
            marginRate: 0.28m,
            sellingBeforeDiscount: 200_000m,
            discountAmount: 25_000m,
            exceptionCodes: ["ESTIMATE_PROVISIONAL_COST", "ESTIMATE_FIXED_PRICE_OVERRIDE", "ESTIMATE_PROVISIONAL_COST"]);

        Assert.Equal(
            ["TEST ONLY MANAGER", "TEST ONLY FINANCIAL APPROVER", "TEST ONLY SPECIALIST CHECKER"],
            route.ReviewerRoleNames);
        Assert.Equal(3, route.ReviewerRoleNames.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(
            ["AMOUNT_AUTHORITY_EXCEEDED", "MARGIN_BELOW_MINIMUM", "DISCOUNT_ABOVE_LIMIT", "PROVISIONAL_COST", "MANUAL_OVERRIDE"],
            route.Triggers.Select(trigger => trigger.Code));
    }

    [Fact]
    public void Resolve_WithinBaselineThresholds_RequiresOneChecker()
    {
        var route = TestOnlyEstimateApprovalPolicy.Resolve(
            grandTotal: 100_000m,
            marginRate: 0.30m,
            sellingBeforeDiscount: 100_000m,
            discountAmount: 10_000m,
            exceptionCodes: []);

        Assert.Equal(["TEST ONLY CHECKER"], route.ReviewerRoleNames);
        Assert.Empty(route.Triggers);
    }
}
