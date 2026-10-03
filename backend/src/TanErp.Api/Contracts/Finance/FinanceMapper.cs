using TanErp.Application.Finance;

namespace TanErp.Api.Contracts.Finance;

internal static class FinanceMapper
{
    public static FinancePersonResponse To(FinancePerson x) => new(x.Id, x.DisplayName);
    public static FinanceProjectRefResponse To(FinanceProjectRef x) => new(x.Id, x.Code, x.Name);
    public static PaymentResponse To(PaymentProjection x) => new(x.Id, x.Number, x.Amount, x.Method, x.Reference, x.ReceivedDate, x.Status, x.ReversalReason, x.ReversedAtUtc, To(x.RecordedBy), x.RecordedAtUtc);
    public static BillingResponse To(BillingProjection x) => new(x.Id, x.BranchId, x.Number, To(x.Project), x.Kind, x.Description, x.Amount, x.PaidAmount, x.Outstanding, x.Currency, x.DueDate, x.Status, x.ReferenceHash, x.VoidReason, x.VoidedAtUtc, To(x.CreatedBy), x.IssuedAtUtc, x.RowVersion, x.Payments.Select(To).ToList());
    public static BillingListItemResponse To(BillingListItemProjection x) => new(x.Id, x.Number, x.ProjectCode, x.Kind, x.Amount, x.PaidAmount, x.Status, x.DueDate, x.IssuedAtUtc);
    public static OutboxMessageResponse To(OutboxMessageProjection x) => new(x.Id, x.Kind, x.ResourceNumber, x.Amount, x.Status, x.Attempts, x.NextAttemptAtUtc, x.LastError, x.ExternalRef, x.ExternalAmount, x.ConfirmedAtUtc, x.CreatedAtUtc, x.SentAtUtc, x.RowVersion);
    public static DispatchResultResponse To(DispatchResult x) => new(x.Processed, x.Sent, x.Failed, x.Dead);
    public static FinanceReconciliationRowResponse To(ReconciliationRow x) => new(x.Issue, x.Kind, x.ResourceNumber, x.MessageId, x.ErpAmount, x.ExternalAmount);
    public static FinanceReconciliationResponse To(ReconciliationProjection x) => new(x.ErpBilled, x.ErpPaid, x.ConfirmedBilled, x.ConfirmedPaid, x.PendingCount, x.FailedCount, x.DeadCount, x.Rows.Select(To).ToList());
    public static ProjectBillingSummaryResponse To(ProjectBillingSummary x) => new(To(x.Project), x.ContractAmount, x.Billed, x.Paid, x.Outstanding, x.Unbilled);
    public static FinancePaginationResponse Pagination(int page, int pageSize, int total) => new(page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
}
