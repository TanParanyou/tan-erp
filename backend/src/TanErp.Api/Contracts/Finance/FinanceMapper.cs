using TanErp.Application.Finance;

namespace TanErp.Api.Contracts.Finance;

internal static class FinanceMapper
{
    public static FinancePersonResponse To(FinancePerson x) => new(x.Id, x.DisplayName);
    public static FinanceProjectRefResponse To(FinanceProjectRef x) => new(x.Id, x.Code, x.Name);
    public static PaymentResponse To(PaymentProjection x) => new(
        x.Id, x.Number, x.Amount, x.Method, x.Reference, x.ReceivedDate, x.Status, x.ReversalReason, x.ReversedAtUtc,
        To(x.RecordedBy), x.RecordedAtUtc, x.ReceiptNumber, x.WhtRate, x.WhtAmount, x.NetCashReceived, x.WhtType, x.WhtCertificateNumber);

    public static BillingLineResponse To(BillingLineProjection x) => new(
        x.Id, x.LineNumber, x.Description, x.TaxBaseAmount, x.VatRate, x.VatAmount, x.TotalAmount);

    public static BillingAdjustmentNoteResponse To(BillingAdjustmentNoteProjection x) => new(
        x.Id, x.Number, x.NoteType, x.Reason, x.TaxBaseDelta, x.VatRate, x.VatDelta, x.TotalDelta, x.Status,
        To(x.CreatedBy), x.DecidedBy is null ? null : To(x.DecidedBy), x.CreatedAtUtc, x.DecidedAtUtc);

    public static BillingResponse To(BillingProjection x) => new(
        x.Id, x.BranchId, x.Number, To(x.Project), x.Kind, x.Description, x.Amount, x.PaidAmount, x.Outstanding,
        x.Currency, x.DueDate, x.Status, x.ReferenceHash, x.VoidReason, x.VoidedAtUtc, To(x.CreatedBy), x.IssuedAtUtc,
        x.RowVersion, x.Payments.Select(To).ToList(),
        x.TaxInvoiceNumber, x.VatRate, x.VatAmount, x.GrossTotal, x.EffectiveReceivable, x.RetentionRate, x.RetentionAmount,
        x.RetentionStatus, x.ReleasedRetentionAtUtc, x.ReleasedRetentionBy is null ? null : To(x.ReleasedRetentionBy),
        x.CreditNoteBaseAmount, x.CreditNoteVatAmount, x.DebitNoteBaseAmount, x.DebitNoteVatAmount,
        (x.Lines ?? Array.Empty<BillingLineProjection>()).Select(To).ToList(),
        (x.Notes ?? Array.Empty<BillingAdjustmentNoteProjection>()).Select(To).ToList());

    public static BillingListItemResponse To(BillingListItemProjection x) => new(x.Id, x.Number, x.ProjectCode, x.Kind, x.Amount, x.PaidAmount, x.Status, x.DueDate, x.IssuedAtUtc);
    public static OutboxMessageResponse To(OutboxMessageProjection x) => new(x.Id, x.Kind, x.ResourceNumber, x.Amount, x.Status, x.Attempts, x.NextAttemptAtUtc, x.LastError, x.ExternalRef, x.ExternalAmount, x.ConfirmedAtUtc, x.CreatedAtUtc, x.SentAtUtc, x.RowVersion);
    public static DispatchResultResponse To(DispatchResult x) => new(x.Processed, x.Sent, x.Failed, x.Dead);
    public static FinanceReconciliationRowResponse To(ReconciliationRow x) => new(x.Issue, x.Kind, x.ResourceNumber, x.MessageId, x.ErpAmount, x.ExternalAmount);
    public static FinanceReconciliationResponse To(ReconciliationProjection x) => new(x.ErpBilled, x.ErpPaid, x.ConfirmedBilled, x.ConfirmedPaid, x.PendingCount, x.FailedCount, x.DeadCount, x.Rows.Select(To).ToList());
    public static ProjectBillingSummaryResponse To(ProjectBillingSummary x) => new(To(x.Project), x.ContractAmount, x.Billed, x.Paid, x.Outstanding, x.Unbilled);
    public static FinancePaginationResponse Pagination(int page, int pageSize, int total) => new(page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));

    public static CreditOverrideResponse To(CreditOverrideProjection x) => new(
        x.Id, x.OrganizationId, x.BranchId, x.CustomerId, x.CustomerName, x.RequestedCeilingAmount, x.Reason, x.ExpiresOn,
        x.Status, To(x.CreatedBy), x.CreatedAtUtc, x.DecidedBy is null ? null : To(x.DecidedBy), x.DecidedAtUtc, x.DecisionNote, x.RowVersion);

    public static CustomerCreditExposureResponse To(CustomerCreditExposureProjection x) => new(
        x.CustomerId, x.CustomerName, x.CreditLimit, x.CreditTermDays, x.OutstandingReceivables, x.UnbilledAcceptedAmount,
        x.TotalExposure, x.EffectiveCreditCeiling, x.AvailableCredit, x.IsOverLimit, x.ActiveOverrides.Select(To).ToList());

    public static ArAgingCustomerBucketResponse To(ArAgingCustomerBucketProjection x) => new(
        x.CustomerId, x.CustomerCode, x.CustomerName, x.Current, x.Bucket1To30, x.Bucket31To60, x.Bucket61To90, x.BucketOver90, x.TotalOutstanding);

    public static ArAgingReportResponse To(ArAgingReportProjection x) => new(
        x.AsOfDate, x.TotalCurrent, x.TotalBucket1To30, x.TotalBucket31To60, x.TotalBucket61To90, x.TotalBucketOver90, x.GrandTotalOutstanding,
        x.Customers.Select(To).ToList());

    // Wave D G-17 Journal Export & COA Mapping
    public static AccountMappingResponse To(AccountMappingProjection x) => new(
        x.Id, x.Kind, x.DebitAccountCode, x.DebitAccountName, x.CreditAccountCode, x.CreditAccountName,
        To(x.UpdatedBy), x.UpdatedAtUtc, x.RowVersion);

    public static JournalExportLineResponse To(JournalExportLineProjection x) => new(
        x.BatchRef, x.MessageId, x.DedupeKey, x.Kind, x.ResourceNumber, x.DebitAccountCode, x.DebitAccountName,
        x.CreditAccountCode, x.CreditAccountName, x.Amount, x.Currency, x.CreatedAtUtc);

    public static JournalExportResultResponse To(JournalExportResult x) => new(
        x.BatchRef, x.ExportedCount, x.DeadCount, x.Lines.Select(To).ToList(), x.CsvContent);
}
