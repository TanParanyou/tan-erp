namespace TanErp.Application.Finance;

public sealed record FinancePerson(Guid Id, string DisplayName);

public sealed record FinanceProjectRef(Guid Id, string Code, string Name);

public sealed record BillingLineInput(string Description, decimal TaxBaseAmount, decimal? VatRate = null);

public sealed record BillingInput(
    Guid ProjectId,
    string Kind,
    string Description,
    decimal Amount,
    DateOnly? DueDate,
    bool ApplyVat = false,
    decimal? VatRate = null,
    decimal RetentionRate = 0m,
    IReadOnlyList<BillingLineInput>? Lines = null);

public sealed record PaymentInput(
    decimal Amount,
    string Method,
    string Reference,
    DateOnly ReceivedDate,
    decimal WhtRate = 0m,
    string? WhtType = null,
    string? WhtCertificateNumber = null);

public sealed record BillingNoteInput(string NoteType, string Reason, decimal TaxBaseDelta);

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
    DateTimeOffset RecordedAtUtc,
    string? ReceiptNumber = null,
    decimal WhtRate = 0m,
    decimal WhtAmount = 0m,
    decimal NetCashReceived = 0m,
    string? WhtType = null,
    string? WhtCertificateNumber = null);

public sealed record BillingLineProjection(
    Guid Id,
    int LineNumber,
    string Description,
    decimal TaxBaseAmount,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalAmount);

public sealed record BillingAdjustmentNoteProjection(
    Guid Id,
    string Number,
    string NoteType,
    string Reason,
    decimal TaxBaseDelta,
    decimal VatRate,
    decimal VatDelta,
    decimal TotalDelta,
    string Status,
    FinancePerson CreatedBy,
    FinancePerson? DecidedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DecidedAtUtc);

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
    IReadOnlyList<PaymentProjection> Payments,
    string? TaxInvoiceNumber = null,
    decimal VatRate = 0m,
    decimal VatAmount = 0m,
    decimal GrossTotal = 0m,
    decimal EffectiveReceivable = 0m,
    decimal RetentionRate = 0m,
    decimal RetentionAmount = 0m,
    string RetentionStatus = "none",
    DateTimeOffset? ReleasedRetentionAtUtc = null,
    FinancePerson? ReleasedRetentionBy = null,
    decimal CreditNoteBaseAmount = 0m,
    decimal CreditNoteVatAmount = 0m,
    decimal DebitNoteBaseAmount = 0m,
    decimal DebitNoteVatAmount = 0m,
    IReadOnlyList<BillingLineProjection>? Lines = null,
    IReadOnlyList<BillingAdjustmentNoteProjection>? Notes = null);

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
