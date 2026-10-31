namespace TanErp.Application.Reporting;

public sealed record ReportingCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public sealed record ReportSectionProjection<T>(bool Permitted, T? Summary) where T : class;

// 1. Pipeline & Win Rate
public sealed record PipelineSummaryMetrics(
    int TotalCount,
    decimal TotalExpectedBudget,
    int OpenCount,
    decimal OpenExpectedBudget,
    int WonCount,
    decimal WonBudget,
    int LostCount,
    decimal WinRatePercent);

public sealed record PipelineStageMetrics(
    string Stage,
    int OpportunityCount,
    decimal ExpectedBudgetTotal);

public sealed record PipelineReportProjection(
    PipelineSummaryMetrics Summary,
    IReadOnlyList<PipelineStageMetrics> Stages);

// 2. Quotation Value
public sealed record QuotationSummaryMetrics(
    int TotalCount,
    int IssuedCount,
    decimal IssuedAmount,
    int AcceptedCount,
    decimal AcceptedAmount,
    decimal AcceptanceRatePercent);

public sealed record QuotationReportRow(
    Guid QuotationId,
    string Number,
    string CustomerCode,
    string CustomerName,
    string BranchCode,
    string Status,
    decimal TotalAmount,
    DateTimeOffset? IssuedAtUtc,
    DateTimeOffset? AcceptedAtUtc);

public sealed record QuotationReportProjection(
    QuotationSummaryMetrics Summary,
    IReadOnlyList<QuotationReportRow> Rows);

// 3. Project Budget vs Actual
public sealed record ProjectBudgetSummaryMetrics(
    int ProjectCount,
    int OverBudgetCount,
    decimal TotalBaselineBudget,
    decimal TotalApprovedChangeOrderBudget,
    decimal TotalCurrentBudget,
    decimal TotalCommittedAmount,
    decimal TotalActualAmount,
    decimal TotalVarianceAmount);

public sealed record ProjectBudgetReportRow(
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

public sealed record ProjectBudgetReportProjection(
    ProjectBudgetSummaryMetrics Summary,
    IReadOnlyList<ProjectBudgetReportRow> Rows);

// 4. Stock Value
public sealed record StockValueSummaryMetrics(
    int WarehouseCount,
    int SkuCount,
    decimal TotalOnHand,
    decimal TotalReserved,
    decimal TotalValue);

public sealed record StockValueReportRow(
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

public sealed record StockValueReportProjection(
    StockValueSummaryMetrics Summary,
    IReadOnlyList<StockValueReportRow> Rows);

// 5. AR Aging
public sealed record ArAgingSummaryMetrics(
    DateOnly AsOfDate,
    int InvoiceCount,
    decimal Current,
    decimal Bucket1To30,
    decimal Bucket31To60,
    decimal Bucket61To90,
    decimal BucketOver90,
    decimal TotalOutstanding);

public sealed record ArAgingReportRow(
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

public sealed record ArAgingReportProjection(
    ArAgingSummaryMetrics Summary,
    IReadOnlyList<ArAgingReportRow> Rows);

// 6. Service Backlog & SLA
public sealed record ServiceSlaSummaryMetrics(
    int TotalCount,
    int OpenBacklogCount,
    int WaitingCustomerCount,
    int SlaWarningCount,
    int SlaBreachedCount,
    decimal OutOfWarrantyChargedTotal);

public sealed record ServiceSlaReportRow(
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

public sealed record ServiceSlaReportProjection(
    ServiceSlaSummaryMetrics Summary,
    IReadOnlyList<ServiceSlaReportRow> Rows);

// 7. Executive Dashboard Summary
public sealed record DashboardSummaryProjection(
    DateTimeOffset GeneratedAtUtc,
    Guid? EffectiveBranchId,
    ReportSectionProjection<PipelineSummaryMetrics> Pipeline,
    ReportSectionProjection<QuotationSummaryMetrics> Quotations,
    ReportSectionProjection<ProjectBudgetSummaryMetrics> ProjectsBudget,
    ReportSectionProjection<StockValueSummaryMetrics> StockValue,
    ReportSectionProjection<ArAgingSummaryMetrics> ArAging,
    ReportSectionProjection<ServiceSlaSummaryMetrics> ServiceSla);
