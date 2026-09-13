using System.Text.RegularExpressions;

namespace TanErp.Domain.Crm.Opportunities;

public static class OpportunityStage
{
    public const string Draft = "draft";
    public const string Qualified = "qualified";
    public const string Surveying = "surveying";
    public const string Estimating = "estimating";
    public const string Proposed = "proposed";
    public const string Won = "won";
    public const string Lost = "lost";
    public const string Cancelled = "cancelled";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Qualified,
        Surveying,
        Estimating,
        Proposed,
        Won,
        Lost,
        Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class OpportunityReasonCodes
{
    // Lost reasons
    public const string LostPriceTooHigh = "lost_price_too_high";
    public const string LostCompetitorSelected = "lost_competitor_selected";
    public const string LostScopeMismatch = "lost_scope_mismatch";
    public const string LostTimelineUnfeasible = "lost_timeline_unfeasible";
    public const string LostNoResponse = "lost_no_response";
    public const string LostOther = "lost_other";

    public static readonly HashSet<string> LostReasons = new(StringComparer.Ordinal)
    {
        LostPriceTooHigh,
        LostCompetitorSelected,
        LostScopeMismatch,
        LostTimelineUnfeasible,
        LostNoResponse,
        LostOther
    };

    // Cancelled reasons
    public const string CancelledCustomerAbandoned = "cancelled_customer_abandoned";
    public const string CancelledDuplicate = "cancelled_duplicate";
    public const string CancelledInvalidLead = "cancelled_invalid_lead";
    public const string CancelledOther = "cancelled_other";

    public static readonly HashSet<string> CancelledReasons = new(StringComparer.Ordinal)
    {
        CancelledCustomerAbandoned,
        CancelledDuplicate,
        CancelledInvalidLead,
        CancelledOther
    };

    // Reopen reasons
    public const string ReopenCustomerReengaged = "reopen_customer_reengaged";
    public const string ReopenBudgetAdjusted = "reopen_budget_adjusted";
    public const string ReopenScopeRedefined = "reopen_scope_redefined";
    public const string ReopenErroneousClosure = "reopen_erroneous_closure";
    public const string ReopenOther = "reopen_other";

    public static readonly HashSet<string> ReopenReasons = new(StringComparer.Ordinal)
    {
        ReopenCustomerReengaged,
        ReopenBudgetAdjusted,
        ReopenScopeRedefined,
        ReopenErroneousClosure,
        ReopenOther
    };

    public static bool IsValidLostReason(string? reason) =>
        !string.IsNullOrWhiteSpace(reason) && LostReasons.Contains(reason.Trim());

    public static bool IsValidCancelledReason(string? reason) =>
        !string.IsNullOrWhiteSpace(reason) && CancelledReasons.Contains(reason.Trim());

    public static bool IsValidReopenReason(string? reason) =>
        !string.IsNullOrWhiteSpace(reason) && ReopenReasons.Contains(reason.Trim());
}

public static class OpportunityStagePolicy
{
    public const string Version = "opportunity-stage-v1";
}

public static class OpportunityWorkType
{
    public const string BuiltIn = "built-in";
    public const string Interior = "interior";
    public const string Curtain = "curtain";
    public const string Wallpaper = "wallpaper";
    public const string Exterior = "exterior";
    public const string Other = "other";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        BuiltIn,
        Interior,
        Curtain,
        Wallpaper,
        Exterior,
        Other
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class OpportunityNormalizer
{
    private static readonly Regex MultipleWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static string CollapseWhitespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return MultipleWhitespaceRegex.Replace(value.Trim(), " ");
    }

    public static string NormalizeTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return CollapseWhitespace(value).ToLowerInvariant();
    }
}
