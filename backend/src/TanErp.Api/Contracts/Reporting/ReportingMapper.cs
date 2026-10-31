using TanErp.Application.Reporting;

namespace TanErp.Api.Contracts.Reporting;

public static class ReportingMapper
{
    public static PipelineSummaryMetricsResponse To(PipelineSummaryMetrics s) =>
        new(s.TotalCount, s.TotalExpectedBudget, s.OpenCount, s.OpenExpectedBudget, s.WonCount, s.WonBudget, s.LostCount, s.WinRatePercent);

    public static PipelineStageMetricsResponse To(PipelineStageMetrics st) =>
        new(st.Stage, st.OpportunityCount, st.ExpectedBudgetTotal);

    public static PipelineReportResponse To(PipelineReportProjection p) =>
        new(To(p.Summary), p.Stages.Select(To).ToList());

    public static QuotationSummaryMetricsResponse To(QuotationSummaryMetrics s) =>
        new(s.TotalCount, s.IssuedCount, s.IssuedAmount, s.AcceptedCount, s.AcceptedAmount, s.AcceptanceRatePercent);

    public static QuotationReportRowResponse To(QuotationReportRow r) =>
        new(r.QuotationId, r.Number, r.CustomerCode, r.CustomerName, r.BranchCode, r.Status, r.TotalAmount, r.IssuedAtUtc, r.AcceptedAtUtc);

    public static QuotationReportResponse To(QuotationReportProjection p) =>
        new(To(p.Summary), p.Rows.Select(To).ToList());

    public static ProjectBudgetSummaryMetricsResponse To(ProjectBudgetSummaryMetrics s) =>
        new(s.ProjectCount, s.OverBudgetCount, s.TotalBaselineBudget, s.TotalApprovedChangeOrderBudget, s.TotalCurrentBudget, s.TotalCommittedAmount, s.TotalActualAmount, s.TotalVarianceAmount);

    public static ProjectBudgetReportRowResponse To(ProjectBudgetReportRow r) =>
        new(r.ProjectId, r.ProjectCode, r.ProjectName, r.BranchCode, r.Status, r.BaselineContractAmount, r.BaselineBudget, r.ApprovedChangeOrderBudget, r.CurrentBudget, r.CommittedAmount, r.ActualAmount, r.VarianceAmount, r.IsOverBudget);

    public static ProjectBudgetReportResponse To(ProjectBudgetReportProjection p) =>
        new(To(p.Summary), p.Rows.Select(To).ToList());

    public static StockValueSummaryMetricsResponse To(StockValueSummaryMetrics s) =>
        new(s.WarehouseCount, s.SkuCount, s.TotalOnHand, s.TotalReserved, s.TotalValue);

    public static StockValueReportRowResponse To(StockValueReportRow r) =>
        new(r.WarehouseId, r.WarehouseCode, r.WarehouseName, r.BranchCode, r.ItemId, r.ItemCode, r.ItemNameTh, r.ItemNameEn, r.OnHand, r.Reserved, r.Available, r.TotalValue);

    public static StockValueReportResponse To(StockValueReportProjection p) =>
        new(To(p.Summary), p.Rows.Select(To).ToList());

    public static ArAgingSummaryMetricsResponse To(ArAgingSummaryMetrics s) =>
        new(s.AsOfDate, s.InvoiceCount, s.Current, s.Bucket1To30, s.Bucket31To60, s.Bucket61To90, s.BucketOver90, s.TotalOutstanding);

    public static ArAgingReportRowResponse To(ArAgingReportRow r) =>
        new(r.BillingDocumentId, r.Number, r.ProjectCode, r.CustomerCode, r.CustomerName, r.BranchCode, r.DueDate, r.DaysOverdue, r.AgingBucket, r.GrossTotal, r.PaidAmount, r.OutstandingAmount);

    public static ReportingArAgingResponse To(ArAgingReportProjection p) =>
        new(To(p.Summary), p.Rows.Select(To).ToList());

    public static ServiceSlaSummaryMetricsResponse To(ServiceSlaSummaryMetrics s) =>
        new(s.TotalCount, s.OpenBacklogCount, s.WaitingCustomerCount, s.SlaWarningCount, s.SlaBreachedCount, s.OutOfWarrantyChargedTotal);

    public static ServiceSlaReportRowResponse To(ServiceSlaReportRow r) =>
        new(r.ServiceRequestId, r.Number, r.Title, r.ProjectCode, r.BranchCode, r.Priority, r.Status, r.InWarranty, r.SlaStatus, r.ElapsedHours, r.ResolveTargetHours, r.ServiceChargeAmount, r.CreatedAtUtc);

    public static ServiceSlaReportResponse To(ServiceSlaReportProjection p) =>
        new(To(p.Summary), p.Rows.Select(To).ToList());

    public static DashboardSummaryResponse To(DashboardSummaryProjection p) =>
        new(
            p.GeneratedAtUtc,
            p.EffectiveBranchId,
            new ReportSectionResponse<PipelineSummaryMetricsResponse>(p.Pipeline.Permitted, p.Pipeline.Summary is null ? null : To(p.Pipeline.Summary)),
            new ReportSectionResponse<QuotationSummaryMetricsResponse>(p.Quotations.Permitted, p.Quotations.Summary is null ? null : To(p.Quotations.Summary)),
            new ReportSectionResponse<ProjectBudgetSummaryMetricsResponse>(p.ProjectsBudget.Permitted, p.ProjectsBudget.Summary is null ? null : To(p.ProjectsBudget.Summary)),
            new ReportSectionResponse<StockValueSummaryMetricsResponse>(p.StockValue.Permitted, p.StockValue.Summary is null ? null : To(p.StockValue.Summary)),
            new ReportSectionResponse<ArAgingSummaryMetricsResponse>(p.ArAging.Permitted, p.ArAging.Summary is null ? null : To(p.ArAging.Summary)),
            new ReportSectionResponse<ServiceSlaSummaryMetricsResponse>(p.ServiceSla.Permitted, p.ServiceSla.Summary is null ? null : To(p.ServiceSla.Summary)));
}
