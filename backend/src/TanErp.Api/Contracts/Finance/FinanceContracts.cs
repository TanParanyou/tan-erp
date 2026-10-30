namespace TanErp.Api.Contracts.Finance;

public sealed record FinancePersonResponse(Guid Id, string DisplayName);

public sealed record FinanceProjectRefResponse(Guid Id, string Code, string Name);

public sealed record BillingLineResponse(
    Guid Id,
    int LineNumber,
    string Description,
    decimal TaxBaseAmount,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalAmount);

public sealed record BillingAdjustmentNoteResponse(
    Guid Id,
    string Number,
    string NoteType,
    string Reason,
    decimal TaxBaseDelta,
    decimal VatRate,
    decimal VatDelta,
    decimal TotalDelta,
    string Status,
    FinancePersonResponse CreatedBy,
    FinancePersonResponse? DecidedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DecidedAtUtc);

public sealed record PaymentResponse(
    Guid Id,
    string Number,
    decimal Amount,
    string Method,
    string Reference,
    DateOnly ReceivedDate,
    string Status,
    string? ReversalReason,
    DateTimeOffset? ReversedAtUtc,
    FinancePersonResponse RecordedBy,
    DateTimeOffset RecordedAtUtc,
    string? ReceiptNumber,
    decimal WhtRate,
    decimal WhtAmount,
    decimal NetCashReceived,
    string? WhtType,
    string? WhtCertificateNumber);

public sealed record BillingResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    FinanceProjectRefResponse Project,
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
    FinancePersonResponse CreatedBy,
    DateTimeOffset IssuedAtUtc,
    Guid RowVersion,
    IReadOnlyList<PaymentResponse> Payments,
    string? TaxInvoiceNumber,
    decimal VatRate,
    decimal VatAmount,
    decimal GrossTotal,
    decimal EffectiveReceivable,
    decimal RetentionRate,
    decimal RetentionAmount,
    string RetentionStatus,
    DateTimeOffset? ReleasedRetentionAtUtc,
    FinancePersonResponse? ReleasedRetentionBy,
    decimal CreditNoteBaseAmount,
    decimal CreditNoteVatAmount,
    decimal DebitNoteBaseAmount,
    decimal DebitNoteVatAmount,
    IReadOnlyList<BillingLineResponse> Lines,
    IReadOnlyList<BillingAdjustmentNoteResponse> Notes);

public sealed record BillingListItemResponse(
    Guid Id,
    string Number,
    string ProjectCode,
    string Kind,
    decimal Amount,
    decimal PaidAmount,
    string Status,
    DateOnly? DueDate,
    DateTimeOffset IssuedAtUtc);

public sealed record OutboxMessageResponse(Guid Id, string Kind, string ResourceNumber, decimal Amount, string Status, int Attempts, DateTimeOffset NextAttemptAtUtc, string? LastError, string? ExternalRef, decimal? ExternalAmount, DateTimeOffset? ConfirmedAtUtc, DateTimeOffset CreatedAtUtc, DateTimeOffset? SentAtUtc, Guid RowVersion);

public sealed record DispatchResultResponse(int Processed, int Sent, int Failed, int Dead);

public sealed record FinanceReconciliationRowResponse(string Issue, string Kind, string ResourceNumber, Guid? MessageId, decimal ErpAmount, decimal? ExternalAmount);

public sealed record FinanceReconciliationResponse(decimal ErpBilled, decimal ErpPaid, decimal ConfirmedBilled, decimal ConfirmedPaid, int PendingCount, int FailedCount, int DeadCount, IReadOnlyList<FinanceReconciliationRowResponse> Rows);

public sealed record OutboxListResponse(IReadOnlyList<OutboxMessageResponse> Items, FinancePaginationResponse Pagination);

public sealed record BillingListResponse(IReadOnlyList<BillingListItemResponse> Items, FinancePaginationResponse Pagination);

public sealed record ProjectBillingSummaryResponse(FinanceProjectRefResponse Project, decimal ContractAmount, decimal Billed, decimal Paid, decimal Outstanding, decimal Unbilled);

public sealed record FinancePaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record BillingLineRequest(string Description, decimal TaxBaseAmount, decimal? VatRate = null);

public sealed record BillingRequest(
    Guid ProjectId,
    string? Kind,
    string? Description,
    decimal Amount,
    DateOnly? DueDate,
    bool ApplyVat = false,
    decimal? VatRate = null,
    decimal RetentionRate = 0m,
    IReadOnlyList<BillingLineRequest>? Lines = null);

public sealed record PaymentRequest(
    decimal Amount,
    string? Method,
    string? Reference,
    DateOnly ReceivedDate,
    decimal WhtRate = 0m,
    string? WhtType = null,
    string? WhtCertificateNumber = null);

public sealed record BillingNoteRequest(string? NoteType, string? Reason, decimal TaxBaseDelta);

public sealed record FinanceReasonRequest(string? Reason);

public sealed record ConfirmRequest(string? ExternalRef, decimal ExternalAmount);
