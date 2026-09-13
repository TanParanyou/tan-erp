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

public static class MeasurementType
{
    public const string Width = "width";
    public const string Depth = "depth";
    public const string Height = "height";
    public const string Length = "length";
    public const string Area = "area";
    public const string Opening = "opening";
    public const string Count = "count";
    public const string Custom = "custom";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Width, Depth, Height, Length, Area, Opening, Count, Custom
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class MeasurementUnit
{
    public const string Millimeter = "mm";
    public const string Centimeter = "cm";
    public const string Meter = "m";
    public const string SquareMeter = "sqm";
    public const string Unit = "unit";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Millimeter, Centimeter, Meter, SquareMeter, Unit
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class CaptureMethod
{
    public const string Measured = "measured";
    public const string CustomerProvided = "customer_provided";
    public const string Derived = "derived";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Measured, CustomerProvided, Derived
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}
