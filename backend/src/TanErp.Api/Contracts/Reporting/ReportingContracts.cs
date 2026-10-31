namespace TanErp.Api.Contracts.Reporting;

public sealed record ReportSectionResponse<T>(bool Permitted, T? Summary) where T : class;

// 1. Pipeline & Win Rate
public sealed record PipelineSummaryMetricsResponse(
    int TotalCount,
    decimal TotalExpectedBudget,
    int OpenCount,
    decimal OpenExpectedBudget,
    int WonCount,
    decimal WonBudget,
    int LostCount,
    decimal WinRatePercent);

public sealed record PipelineStageMetricsResponse(
    string Stage,
    int OpportunityCount,
    decimal ExpectedBudgetTotal);

public sealed record PipelineReportResponse(
    PipelineSummaryMetricsResponse Summary,
    IReadOnlyList<PipelineStageMetricsResponse> Stages);

// 2. Quotation Value
public sealed record QuotationSummaryMetricsResponse(
    int TotalCount,
    int IssuedCount,
    decimal IssuedAmount,
    int AcceptedCount,
    decimal AcceptedAmount,
    decimal AcceptanceRatePercent);

public sealed record QuotationReportRowResponse(
    Guid QuotationId,
    string Number,
    string CustomerCode,
    string CustomerName,
    string BranchCode,
    string Status,
    decimal TotalAmount,
    DateTimeOffset? IssuedAtUtc,
    DateTimeOffset? AcceptedAtUtc);

public sealed record QuotationReportResponse(
    QuotationSummaryMetricsResponse Summary,
    IReadOnlyList<QuotationReportRowResponse> Rows);

// 3. Project Budget vs Actual
public sealed record ProjectBudgetSummaryMetricsResponse(
    int ProjectCount,
    int OverBudgetCount,
    decimal TotalBaselineBudget,
    decimal TotalApprovedChangeOrderBudget,
    decimal TotalCurrentBudget,
    decimal TotalCommittedAmount,
    decimal TotalActualAmount,
    decimal TotalVarianceAmount);

public sealed record ProjectBudgetReportRowResponse(
    Guid ProjectId,
    string ProjectCode,
    string ProjectName,
    string BranchCode,
    string Status,
    decimal BaselineContractAmount,
    decimal BaselineBudget,
    decimal ApprovedChangeOrderBudget,
    decimal CurrentBudget,
    decimal CommittedAmount,
    decimal ActualAmount,
    decimal VarianceAmount,
    bool IsOverBudget);

public sealed record ProjectBudgetReportResponse(
    ProjectBudgetSummaryMetricsResponse Summary,
    IReadOnlyList<ProjectBudgetReportRowResponse> Rows);

// 4. Stock Value
public sealed record StockValueSummaryMetricsResponse(
    int WarehouseCount,
    int SkuCount,
    decimal TotalOnHand,
    decimal TotalReserved,
    decimal TotalValue);

public sealed record StockValueReportRowResponse(
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    string BranchCode,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    string ItemNameEn,
    decimal OnHand,
    decimal Reserved,
    decimal Available,
    decimal TotalValue);

public sealed record StockValueReportResponse(
    StockValueSummaryMetricsResponse Summary,
    IReadOnlyList<StockValueReportRowResponse> Rows);

// 5. AR Aging
public sealed record ArAgingSummaryMetricsResponse(
    DateOnly AsOfDate,
    int InvoiceCount,
    decimal Current,
    decimal Bucket1To30,
    decimal Bucket31To60,
    decimal Bucket61To90,
    decimal BucketOver90,
    decimal TotalOutstanding);

public sealed record ArAgingReportRowResponse(
    Guid BillingDocumentId,
    string Number,
    string ProjectCode,
    string CustomerCode,
    string CustomerName,
    string BranchCode,
    DateOnly? DueDate,
    int DaysOverdue,
    string AgingBucket,
    decimal GrossTotal,
    decimal PaidAmount,
    decimal OutstandingAmount);

public sealed record ReportingArAgingResponse(
    ArAgingSummaryMetricsResponse Summary,
    IReadOnlyList<ArAgingReportRowResponse> Rows);

// 6. Service Backlog & SLA
public sealed record ServiceSlaSummaryMetricsResponse(
    int TotalCount,
    int OpenBacklogCount,
    int WaitingCustomerCount,
    int SlaWarningCount,
    int SlaBreachedCount,
    decimal OutOfWarrantyChargedTotal);

public sealed record ServiceSlaReportRowResponse(
    Guid ServiceRequestId,
    string Number,
    string Title,
    string ProjectCode,
    string BranchCode,
    string Priority,
    string Status,
    bool InWarranty,
    string SlaStatus,
    decimal ElapsedHours,
    int ResolveTargetHours,
    decimal ServiceChargeAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record ServiceSlaReportResponse(
    ServiceSlaSummaryMetricsResponse Summary,
    IReadOnlyList<ServiceSlaReportRowResponse> Rows);

// 7. Executive Dashboard Summary
public sealed record DashboardSummaryResponse(
    DateTimeOffset GeneratedAtUtc,
    Guid? EffectiveBranchId,
    ReportSectionResponse<PipelineSummaryMetricsResponse> Pipeline,
    ReportSectionResponse<QuotationSummaryMetricsResponse> Quotations,
    ReportSectionResponse<ProjectBudgetSummaryMetricsResponse> ProjectsBudget,
    ReportSectionResponse<StockValueSummaryMetricsResponse> StockValue,
    ReportSectionResponse<ArAgingSummaryMetricsResponse> ArAging,
    ReportSectionResponse<ServiceSlaSummaryMetricsResponse> ServiceSla);
