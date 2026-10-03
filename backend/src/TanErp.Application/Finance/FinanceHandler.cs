using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Finance;

namespace TanErp.Application.Finance;

public sealed record FinanceCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>Billing, payment and accounting-sync use cases. Permission is resolved from PostgreSQL per call.</summary>
public class FinanceHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxDispatchBatch = 50;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IFinanceStore _store;

    public FinanceHandler(IRequestAccessResolver accessResolver, IFinanceStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(FinanceCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string Num(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    // ----- billing ---------------------------------------------------------------------------------

    public async Task<Result<BillingProjection>> CreateBillingAsync(FinanceCaller caller, string idempotencyKey, BillingInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "billings.manage", ct);
        if (access.IsFailure) return Result<BillingProjection>.Failure(access.Error);
        if (input is null || input.ProjectId == Guid.Empty) return Fail<BillingProjection>("BILLING_FIELD_REQUIRED", "A project is required.");
        if (!BillingKind.All.Contains(input.Kind ?? string.Empty) || string.IsNullOrWhiteSpace(input.Description) || input.Amount <= 0)
        {
            return Fail<BillingProjection>("BILLING_FIELD_INVALID", "A valid kind, a description and an amount above zero are required.");
        }

        var payloadHash = Sha256Hex.Compute($"{input.ProjectId}|{input.Kind}|{input.Description.Trim()}|{Num(input.Amount)}|{input.DueDate:O}");
        return await _store.CreateBillingAsync(access.Value!, input, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<BillingProjection>> VoidBillingAsync(FinanceCaller caller, Guid billingId, Guid expectedVersion, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "billings.manage", ct);
        if (access.IsFailure) return Result<BillingProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BillingProjection>("BILLING_FIELD_REQUIRED", "Expected version is required.");
        if (string.IsNullOrWhiteSpace(reason)) return Fail<BillingProjection>("BILLING_REASON_REQUIRED", "A reason is required.");
        return await _store.VoidBillingAsync(access.Value!, billingId, expectedVersion, reason.Trim(), caller.TraceId, ct);
    }

    public async Task<Result<BillingProjection>> RecordPaymentAsync(FinanceCaller caller, Guid billingId, string idempotencyKey, PaymentInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "payments.manage", ct);
        if (access.IsFailure) return Result<BillingProjection>.Failure(access.Error);
        if (input is null || input.Amount <= 0 || !PaymentMethod.All.Contains(input.Method ?? string.Empty) || string.IsNullOrWhiteSpace(input.Reference))
        {
            return Fail<BillingProjection>("PAYMENT_FIELD_INVALID", "An amount above zero, a valid method and a reference are required.");
        }

        var payloadHash = Sha256Hex.Compute($"{billingId}|{Num(input.Amount)}|{input.Method}|{input.Reference.Trim()}|{input.ReceivedDate:O}");
        return await _store.RecordPaymentAsync(access.Value!, billingId, input, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<BillingProjection>> ReversePaymentAsync(FinanceCaller caller, Guid billingId, Guid paymentId, Guid expectedVersion, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "payments.manage", ct);
        if (access.IsFailure) return Result<BillingProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BillingProjection>("BILLING_FIELD_REQUIRED", "Expected version is required.");
        if (string.IsNullOrWhiteSpace(reason)) return Fail<BillingProjection>("BILLING_REASON_REQUIRED", "A reason is required.");
        return await _store.ReversePaymentAsync(access.Value!, billingId, paymentId, expectedVersion, reason.Trim(), caller.TraceId, ct);
    }

    public async Task<Result<BillingProjection>> GetBillingAsync(FinanceCaller caller, Guid billingId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "billings.read", ct);
        if (access.IsFailure) return Result<BillingProjection>.Failure(access.Error);
        var billing = await _store.GetBillingAsync(access.Value!.OrganizationId, billingId, ct);
        return billing is null ? Fail<BillingProjection>("RESOURCE_NOT_FOUND", "Billing not found.") : Result<BillingProjection>.Success(billing);
    }

    public async Task<Result<PagedBillings>> ListBillingsAsync(FinanceCaller caller, string? search, string? status, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "billings.read", ct);
        if (access.IsFailure) return Result<PagedBillings>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !BillingStatus.All.Contains(status.Trim())) return Fail<PagedBillings>("BILLING_FIELD_INVALID", "The status filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedBillings>.Success(await _store.ListBillingsAsync(access.Value!.OrganizationId, new BillingListQuery(Clean(search), Clean(status), projectId, p, size), ct));
    }

    public async Task<Result<ProjectBillingSummary>> GetProjectSummaryAsync(FinanceCaller caller, Guid projectId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "billings.read", ct);
        if (access.IsFailure) return Result<ProjectBillingSummary>.Failure(access.Error);
        var summary = await _store.GetProjectSummaryAsync(access.Value!.OrganizationId, projectId, ct);
        return summary is null ? Fail<ProjectBillingSummary>("RESOURCE_NOT_FOUND", "Project not found.") : Result<ProjectBillingSummary>.Success(summary);
    }

    // ----- accounting sync -------------------------------------------------------------------------

    public async Task<Result<PagedOutbox>> ListOutboxAsync(FinanceCaller caller, string? status, string? kind, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "finance-sync.read", ct);
        if (access.IsFailure) return Result<PagedOutbox>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !OutboxStatus.All.Contains(status.Trim())) return Fail<PagedOutbox>("OUTBOX_FIELD_INVALID", "The status filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedOutbox>.Success(await _store.ListOutboxAsync(access.Value!.OrganizationId, new OutboxListQuery(Clean(status), Clean(kind), p, size), ct));
    }

    public async Task<Result<DispatchResult>> DispatchAsync(FinanceCaller caller, int? batchSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "finance-sync.run", ct);
        if (access.IsFailure) return Result<DispatchResult>.Failure(access.Error);
        var size = batchSize is null or <= 0 ? 20 : Math.Min(batchSize.Value, MaxDispatchBatch);
        return await _store.DispatchAsync(access.Value!, size, caller.TraceId, ct);
    }

    public async Task<Result<OutboxMessageProjection>> ConfirmAsync(FinanceCaller caller, Guid messageId, string? externalRef, decimal externalAmount, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "finance-sync.run", ct);
        if (access.IsFailure) return Result<OutboxMessageProjection>.Failure(access.Error);
        if (string.IsNullOrWhiteSpace(externalRef) || externalAmount < 0) return Fail<OutboxMessageProjection>("OUTBOX_FIELD_INVALID", "An external reference and a non-negative amount are required.");
        return await _store.ConfirmAsync(access.Value!, messageId, externalRef.Trim(), externalAmount, caller.TraceId, ct);
    }

    public async Task<Result<OutboxMessageProjection>> RequeueAsync(FinanceCaller caller, Guid messageId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "finance-sync.run", ct);
        if (access.IsFailure) return Result<OutboxMessageProjection>.Failure(access.Error);
        return await _store.RequeueAsync(access.Value!, messageId, caller.TraceId, ct);
    }

    public async Task<Result<ReconciliationProjection>> ReconcileAsync(FinanceCaller caller, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "finance-sync.read", ct);
        if (access.IsFailure) return Result<ReconciliationProjection>.Failure(access.Error);
        return Result<ReconciliationProjection>.Success(await _store.ReconcileAsync(access.Value!.OrganizationId, ct));
    }
}
