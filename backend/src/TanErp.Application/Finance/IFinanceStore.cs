using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Finance;

public interface IFinanceStore
{
    Task<Result<BillingProjection>> CreateBillingAsync(RequestAccessContext access, BillingInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> VoidBillingAsync(RequestAccessContext access, Guid billingId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> RecordPaymentAsync(RequestAccessContext access, Guid billingId, PaymentInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> ReversePaymentAsync(RequestAccessContext access, Guid billingId, Guid paymentId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default);
    Task<BillingProjection?> GetBillingAsync(Guid organizationId, Guid billingId, CancellationToken ct = default);
    Task<PagedBillings> ListBillingsAsync(Guid organizationId, BillingListQuery query, CancellationToken ct = default);
    Task<ProjectBillingSummary?> GetProjectSummaryAsync(Guid organizationId, Guid projectId, CancellationToken ct = default);

    Task<PagedOutbox> ListOutboxAsync(Guid organizationId, OutboxListQuery query, CancellationToken ct = default);
    Task<Result<DispatchResult>> DispatchAsync(RequestAccessContext access, int batchSize, string traceId, CancellationToken ct = default);
    Task<Result<OutboxMessageProjection>> ConfirmAsync(RequestAccessContext access, Guid messageId, string externalRef, decimal externalAmount, string traceId, CancellationToken ct = default);
    Task<Result<OutboxMessageProjection>> RequeueAsync(RequestAccessContext access, Guid messageId, string traceId, CancellationToken ct = default);
    Task<ReconciliationProjection> ReconcileAsync(Guid organizationId, CancellationToken ct = default);
}
