namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OpportunityMetricsResponse(
    decimal TotalPipelineValue,
    int ActiveDealsCount,
    int ClosingThisMonthCount,
    decimal WonValue,
    int WonDealsCount,
    decimal WinRatePercentage);
