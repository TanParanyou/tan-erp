namespace TanErp.Application.Reporting;

public interface IReportingStore
{
    Task<PipelineReportProjection> GetPipelineReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);
    Task<PipelineSummaryMetrics> GetPipelineSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);

    Task<QuotationReportProjection> GetQuotationReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);
    Task<QuotationSummaryMetrics> GetQuotationSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);

    Task<ProjectBudgetReportProjection> GetProjectBudgetReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);
    Task<ProjectBudgetSummaryMetrics> GetProjectBudgetSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);

    Task<StockValueReportProjection> GetStockValueReportAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);
    Task<StockValueSummaryMetrics> GetStockValueSummaryAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);

    Task<ArAgingReportProjection> GetArAgingReportAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default);
    Task<ArAgingSummaryMetrics> GetArAgingSummaryAsync(Guid organizationId, Guid? branchId, DateOnly asOfDate, CancellationToken ct = default);

    Task<ServiceSlaReportProjection> GetServiceSlaReportAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default);
    Task<ServiceSlaSummaryMetrics> GetServiceSlaSummaryAsync(Guid organizationId, Guid? branchId, DateTimeOffset nowUtc, CancellationToken ct = default);
}
