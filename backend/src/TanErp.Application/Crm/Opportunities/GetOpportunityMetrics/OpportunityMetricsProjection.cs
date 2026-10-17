namespace TanErp.Application.Crm.Opportunities.GetOpportunityMetrics;

public sealed record OpportunityMetricsProjection(
    decimal TotalPipelineValue,
    int ActiveDealsCount,
    int ClosingThisMonthCount,
    decimal WonValue,
    int WonDealsCount,
    decimal WinRatePercentage);
