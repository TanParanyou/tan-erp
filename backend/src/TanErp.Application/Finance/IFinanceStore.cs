using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Finance;

public interface IFinanceStore
{
    Task<Result<BillingProjection>> CreateBillingAsync(RequestAccessContext access, BillingInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> VoidBillingAsync(RequestAccessContext access, Guid billingId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> ReleaseRetentionAsync(RequestAccessContext access, Guid billingId, Guid expectedVersion, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> CreateBillingNoteAsync(RequestAccessContext access, Guid billingId, BillingNoteInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> ApproveBillingNoteAsync(RequestAccessContext access, Guid billingId, Guid noteId, Guid expectedVersion, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> RecordPaymentAsync(RequestAccessContext access, Guid billingId, PaymentInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BillingProjection>> ReversePaymentAsync(RequestAccessContext access, Guid billingId, Guid paymentId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default);
    Task<BillingProjection?> GetBillingAsync(Guid organizationId, Guid billingId, CancellationToken ct = default);
    Task<PagedBillings> ListBillingsAsync(Guid organizationId, BillingListQuery query, CancellationToken ct = default);
    Task<ProjectBillingSummary?> GetProjectSummaryAsync(Guid organizationId, Guid projectId, CancellationToken ct = default);
    Task<byte[]?> RenderTaxDocumentPdfAsync(Guid organizationId, Guid billingId, CancellationToken ct = default);

    Task<PagedOutbox> ListOutboxAsync(Guid organizationId, OutboxListQuery query, CancellationToken ct = default);
    Task<Result<DispatchResult>> DispatchAsync(RequestAccessContext access, int batchSize, string traceId, CancellationToken ct = default);
    Task<Result<OutboxMessageProjection>> ConfirmAsync(RequestAccessContext access, Guid messageId, string externalRef, decimal externalAmount, string traceId, CancellationToken ct = default);
    Task<Result<OutboxMessageProjection>> RequeueAsync(RequestAccessContext access, Guid messageId, string traceId, CancellationToken ct = default);
    Task<ReconciliationProjection> ReconcileAsync(Guid organizationId, CancellationToken ct = default);

    // Wave D G-16 Credit Exposure & AR Aging
    Task<ArAgingReportProjection> GetArAgingAsync(Guid organizationId, DateOnly? asOfDate, Guid? customerId, CancellationToken ct = default);
    Task<CustomerCreditExposureProjection?> GetCustomerCreditExposureAsync(Guid organizationId, Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<CreditOverrideProjection>> ListCreditOverridesAsync(Guid organizationId, CreditOverrideListQuery query, CancellationToken ct = default);
    Task<Result<CreditOverrideProjection>> CreateCreditOverrideAsync(RequestAccessContext access, CreditOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<CreditOverrideProjection>> ApproveCreditOverrideAsync(RequestAccessContext access, Guid overrideId, Guid expectedVersion, string traceId, CancellationToken ct = default);

    // Wave D G-17 Journal Export & COA Mapping
    Task<IReadOnlyList<AccountMappingProjection>> ListAccountMappingsAsync(Guid organizationId, CancellationToken ct = default);
    Task<Result<AccountMappingProjection>> UpsertAccountMappingAsync(RequestAccessContext access, AccountMappingInput input, string traceId, CancellationToken ct = default);
    Task<Result<JournalExportResult>> ExportJournalAsync(RequestAccessContext access, int batchSize, string traceId, CancellationToken ct = default);
}
