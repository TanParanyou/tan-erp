namespace TanErp.Application.Finance;

public sealed record FinancePerson(Guid Id, string DisplayName);

public sealed record FinanceProjectRef(Guid Id, string Code, string Name);

public sealed record BillingInput(Guid ProjectId, string Kind, string Description, decimal Amount, DateOnly? DueDate);

public sealed record PaymentInput(decimal Amount, string Method, string Reference, DateOnly ReceivedDate);

public sealed record PaymentProjection(
    Guid Id,
    string Number,
    decimal Amount,
    string Method,
    string Reference,
    DateOnly ReceivedDate,
    string Status,
    string? ReversalReason,
    DateTimeOffset? ReversedAtUtc,
    FinancePerson RecordedBy,
    DateTimeOffset RecordedAtUtc);

public sealed record BillingProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    FinanceProjectRef Project,
    string Kind,
    string Description,
    decimal Amount,
    decimal PaidAmount,
    decimal Outstanding,
    string Currency,
    DateOnly? DueDate,
    string Status,
    string ReferenceHash,
    string? VoidReason,
    DateTimeOffset? VoidedAtUtc,
    FinancePerson CreatedBy,
    DateTimeOffset IssuedAtUtc,
    Guid RowVersion,
    IReadOnlyList<PaymentProjection> Payments);

public sealed record BillingListItemProjection(
    Guid Id,
    string Number,
    string ProjectCode,
    string Kind,
    decimal Amount,
    decimal PaidAmount,
    string Status,
    DateOnly? DueDate,
    DateTimeOffset IssuedAtUtc);

public sealed record BillingListQuery(string? Search, string? Status, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedBillings(IReadOnlyList<BillingListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record ProjectBillingSummary(
    FinanceProjectRef Project,
    decimal ContractAmount,
    decimal Billed,
    decimal Paid,
    decimal Outstanding,
    decimal Unbilled);

public sealed record OutboxMessageProjection(
    Guid Id,
    string Kind,
    string ResourceNumber,
    decimal Amount,
    string Status,
    int Attempts,
    DateTimeOffset NextAttemptAtUtc,
    string? LastError,
    string? ExternalRef,
    decimal? ExternalAmount,
    DateTimeOffset? ConfirmedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SentAtUtc,
    Guid RowVersion);

public sealed record OutboxListQuery(string? Status, string? Kind, int Page, int PageSize);

public sealed record PagedOutbox(IReadOnlyList<OutboxMessageProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record DispatchResult(int Processed, int Sent, int Failed, int Dead);

public sealed record ReconciliationRow(string Issue, string Kind, string ResourceNumber, Guid? MessageId, decimal ErpAmount, decimal? ExternalAmount);

public sealed record ReconciliationProjection(
    decimal ErpBilled,
    decimal ErpPaid,
    decimal ConfirmedBilled,
    decimal ConfirmedPaid,
    int PendingCount,
    int FailedCount,
    int DeadCount,
    IReadOnlyList<ReconciliationRow> Rows);

/// <summary>What the accounting connector is given. The payload is the frozen snapshot, never recomputed from current data.</summary>
public sealed record AccountingEnvelope(Guid MessageId, string DedupeKey, string Kind, string ResourceNumber, decimal Amount, string PayloadJson, int Attempt);

public sealed record ConnectorResult(bool Success, string? ExternalRef, string? Error);
