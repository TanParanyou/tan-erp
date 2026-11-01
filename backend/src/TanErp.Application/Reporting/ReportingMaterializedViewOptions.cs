namespace TanErp.Application.Reporting;

/// <summary>
/// Opt-in read path for the heaviest dashboard reports (VQ-RPT-01). Off by default: reports stay live. When on, pipeline, project budget and
/// stock value read PostgreSQL materialized views in schema "reporting" that a background worker refreshes, so they can be up to one interval old.
/// </summary>
public sealed class ReportingMaterializedViewOptions
{
    public const string SectionName = "Reporting:MaterializedViews";

    public static readonly IReadOnlyList<string> ViewNames =
        ["reporting.mv_pipeline_stages", "reporting.mv_project_budget", "reporting.mv_stock_value"];

    public bool Enabled { get; set; }
    public int RefreshIntervalMinutes { get; set; } = 15;
    public int StartupDelaySeconds { get; set; } = 30;

    public TimeSpan EffectiveInterval => TimeSpan.FromMinutes(Math.Clamp(RefreshIntervalMinutes, 1, 1440));
    public TimeSpan EffectiveStartupDelay => TimeSpan.FromSeconds(Math.Clamp(StartupDelaySeconds, 0, 3600));
}
