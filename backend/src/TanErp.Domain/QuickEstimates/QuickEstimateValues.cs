namespace TanErp.Domain.QuickEstimates;

public class QuickEstimateException : Exception
{
    public string Code { get; }

    public QuickEstimateException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class WorkType
{
    public const string BuiltIn = "built-in";
    public const string Curtain = "curtain";
    public const string Wallpaper = "wallpaper";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { BuiltIn, Curtain, Wallpaper };
}

/// <summary>How a measurement line becomes a billable quantity. The unit of the reference rate must match.</summary>
public static class MeasurementRule
{
    public const string Area = "area";
    public const string Length = "length";
    public const string Volume = "volume";
    public const string Count = "count";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Area, Length, Volume, Count };
}

public static class TemplateStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Calibration = "calibration";
    public const string Active = "active";
    public const string Superseded = "superseded";
    public const string Disabled = "disabled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Draft, Submitted, Approved, Calibration, Active, Superseded, Disabled };

    /// <summary>Only Calibration and Active versions may price a quick estimate.</summary>
    public static bool IsUsable(string status) => status is Calibration or Active;
}

public static class QuickEstimateStatus
{
    public const string Draft = "draft";
    public const string Calculated = "calculated";
    public const string PendingReview = "pending_review";
    public const string Approved = "approved";
    public const string Returned = "returned";
    public const string Converted = "converted";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Draft, Calculated, PendingReview, Approved, Returned, Converted };
}

public static class MeasurementConfidence
{
    public const string Low = "low";
    public const string Medium = "medium";
    public const string High = "high";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Low, Medium, High };
}

public static class ShareDecision
{
    public const string Blocked = "blocked";
    public const string PendingReview = "pending_review";
    public const string Shareable = "shareable";
}

public static class ReviewStatus
{
    public const string Requested = "requested";
    public const string Approved = "approved";
    public const string Returned = "returned";
}

public static class TaxDisplay
{
    public const string Exclusive = "exclusive";
    public const string Inclusive = "inclusive";
}
