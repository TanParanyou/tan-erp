namespace TanErp.Api.Contracts.Finance;

public sealed record FinancePersonResponse(Guid Id, string DisplayName);

public sealed record FinanceProjectRefResponse(Guid Id, string Code, string Name);

public sealed record PaymentResponse(Guid Id, string Number, decimal Amount, string Method, string Reference, DateOnly ReceivedDate, string Status, string? ReversalReason, DateTimeOffset? ReversedAtUtc, FinancePersonResponse RecordedBy, DateTimeOffset RecordedAtUtc);

public sealed record BillingResponse(Guid Id, Guid BranchId, string Number, FinanceProjectRefResponse Project, string Kind, string Description, decimal Amount, decimal PaidAmount, decimal Outstanding, string Currency, DateOnly? DueDate, string Status, string ReferenceHash, string? VoidReason, DateTimeOffset? VoidedAtUtc, FinancePersonResponse CreatedBy, DateTimeOffset IssuedAtUtc, Guid RowVersion, IReadOnlyList<PaymentResponse> Payments);

public sealed record BillingListItemResponse(Guid Id, string Number, string ProjectCode, string Kind, decimal Amount, decimal PaidAmount, string Status, DateOnly? DueDate, DateTimeOffset IssuedAtUtc);

public sealed record OutboxMessageResponse(Guid Id, string Kind, string ResourceNumber, decimal Amount, string Status, int Attempts, DateTimeOffset NextAttemptAtUtc, string? LastError, string? ExternalRef, decimal? ExternalAmount, DateTimeOffset? ConfirmedAtUtc, DateTimeOffset CreatedAtUtc, DateTimeOffset? SentAtUtc, Guid RowVersion);

public sealed record DispatchResultResponse(int Processed, int Sent, int Failed, int Dead);

public sealed record FinanceReconciliationRowResponse(string Issue, string Kind, string ResourceNumber, Guid? MessageId, decimal ErpAmount, decimal? ExternalAmount);

public sealed record FinanceReconciliationResponse(decimal ErpBilled, decimal ErpPaid, decimal ConfirmedBilled, decimal ConfirmedPaid, int PendingCount, int FailedCount, int DeadCount, IReadOnlyList<FinanceReconciliationRowResponse> Rows);

public sealed record OutboxListResponse(IReadOnlyList<OutboxMessageResponse> Items, FinancePaginationResponse Pagination);

public sealed record BillingListResponse(IReadOnlyList<BillingListItemResponse> Items, FinancePaginationResponse Pagination);

public sealed record ProjectBillingSummaryResponse(FinanceProjectRefResponse Project, decimal ContractAmount, decimal Billed, decimal Paid, decimal Outstanding, decimal Unbilled);

public sealed record FinancePaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record BillingRequest(Guid ProjectId, string? Kind, string? Description, decimal Amount, DateOnly? DueDate);

public sealed record PaymentRequest(decimal Amount, string? Method, string? Reference, DateOnly ReceivedDate);

public sealed record FinanceReasonRequest(string? Reason);

public sealed record ConfirmRequest(string? ExternalRef, decimal ExternalAmount);
