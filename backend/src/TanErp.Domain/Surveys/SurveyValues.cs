namespace TanErp.Domain.Surveys;

public static class SiteSurveyStatus
{
    public const string Scheduled = "scheduled";
    public const string InProgress = "in_progress";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SurveyRevisionStatus
{
    public const string Draft = "draft";
    public const string Ready = "ready";
    public const string Superseded = "superseded";
    public const string Void = "void";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Ready,
        Superseded,
        Void
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SurveyReadiness
{
    public const string Incomplete = "incomplete";
    public const string RequiresAttention = "requiresAttention";
    public const string Ready = "ready";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Incomplete,
        RequiresAttention,
        Ready
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SurveyDefaults
{
    public const string BaselineTemplateVersion = "SURVEY-BASELINE-v1";
}
