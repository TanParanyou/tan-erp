using System.Security.Cryptography;
using System.Text;

namespace TanErp.Domain.Estimates;

public static class TestOnlyEstimateApprovalPolicy
{
    public const string PolicyCode = "TEST_ONLY-TH-EST-V1";
    public const int Version = 1;

    public static IReadOnlyList<string> ReviewerRoleNames { get; } =
    [
        "TEST ONLY CHECKER",
        "TEST ONLY MANAGER",
        "TEST ONLY FINANCIAL APPROVER",
        "TEST ONLY DIRECTOR",
        "TEST ONLY SPECIALIST CHECKER"
    ];

    public static string ContentHash { get; } = CreateContentHash();

    public static TestOnlyApprovalRoute Resolve(
        decimal grandTotal,
        decimal marginRate,
        decimal sellingBeforeDiscount,
        decimal discountAmount,
        IEnumerable<string> exceptionCodes)
    {
        if (grandTotal < 0m)
            throw new ArgumentOutOfRangeException(nameof(grandTotal));
        if (sellingBeforeDiscount < 0m || discountAmount < 0m)
            throw new ArgumentOutOfRangeException(nameof(sellingBeforeDiscount));
        ArgumentNullException.ThrowIfNull(exceptionCodes);

        var triggers = new List<TestOnlyApprovalTrigger>();
        var routeLevel = 1;

        if (grandTotal > 100_000m)
        {
            routeLevel = Math.Max(routeLevel, grandTotal > 1_500_000m ? 4 : grandTotal > 500_000m ? 3 : 2);
            var threshold = grandTotal > 1_500_000m ? 1_500_000m : grandTotal > 500_000m ? 500_000m : 100_000m;
            triggers.Add(new TestOnlyApprovalTrigger("AMOUNT_AUTHORITY_EXCEEDED", grandTotal, threshold, "THB"));
        }

        if (marginRate < 0.30m)
        {
            routeLevel = Math.Max(routeLevel, marginRate < 0.20m ? 4 : 2);
            triggers.Add(new TestOnlyApprovalTrigger("MARGIN_BELOW_MINIMUM", marginRate, 0.30m, "rate"));
        }

        var discountRate = sellingBeforeDiscount == 0m ? 0m : discountAmount / sellingBeforeDiscount;
        if (discountRate > 0.10m)
        {
            routeLevel = Math.Max(routeLevel, 3);
            triggers.Add(new TestOnlyApprovalTrigger("DISCOUNT_ABOVE_LIMIT", discountRate, 0.10m, "rate"));
        }

        var requiresSpecialist = false;
        foreach (var exceptionCode in exceptionCodes.Distinct(StringComparer.Ordinal))
        {
            var trigger = exceptionCode switch
            {
                "ESTIMATE_FIXED_PRICE_OVERRIDE" => "MANUAL_OVERRIDE",
                "ESTIMATE_PROVISIONAL_COST" => "PROVISIONAL_COST",
                "CUSTOM_WORK_ITEM" => "CUSTOM_WORK_ITEM",
                _ => null
            };
            if (trigger is null)
                continue;
            requiresSpecialist = true;
            triggers.Add(new TestOnlyApprovalTrigger(trigger, null, null, null));
        }

        var roles = routeLevel switch
        {
            1 => new List<string> { "TEST ONLY CHECKER" },
            2 => new List<string> { "TEST ONLY MANAGER" },
            3 => new List<string> { "TEST ONLY MANAGER", "TEST ONLY FINANCIAL APPROVER" },
            _ => new List<string> { "TEST ONLY MANAGER", "TEST ONLY FINANCIAL APPROVER", "TEST ONLY DIRECTOR" }
        };
        if (requiresSpecialist)
            roles.Add("TEST ONLY SPECIALIST CHECKER");

        return new TestOnlyApprovalRoute(
            PolicyCode,
            Version,
            ContentHash,
            new TestOnlyApprovalThresholds(100_000m, 500_000m, 1_500_000m, 0.30m, 0.20m, 0.10m),
            triggers,
            roles);
    }

    private static string CreateContentHash()
    {
        const string canonical = "TEST_ONLY-TH-EST-V1|1|100000|500000|1500000|0.30|0.20|0.10|checker>manager>financial>director|specialist-on-exception";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}

public sealed record TestOnlyApprovalTrigger(string Code, decimal? ActualValue, decimal? ThresholdValue, string? Unit);

public sealed record TestOnlyApprovalThresholds(
    decimal ManagerAmountLimit,
    decimal FinancialAmountLimit,
    decimal DirectorAmountLimit,
    decimal CheckerMinimumMarginRate,
    decimal DirectorMinimumMarginRate,
    decimal DiscountLimitRate);

public sealed record TestOnlyApprovalRoute(
    string PolicyCode,
    int PolicyVersion,
    string PolicyHash,
    TestOnlyApprovalThresholds Thresholds,
    IReadOnlyList<TestOnlyApprovalTrigger> Triggers,
    IReadOnlyList<string> ReviewerRoleNames);
