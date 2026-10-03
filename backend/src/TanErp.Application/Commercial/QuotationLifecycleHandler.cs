using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Commercial;

/// <summary>Void and amend use cases for issued quotations; both need a reason and the quotation's current version.</summary>
public class QuotationLifecycleHandler
{
    public const int MaxReasonLength = 500;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IQuotationLifecycleStore _store;

    public QuotationLifecycleHandler(IRequestAccessResolver accessResolver, IQuotationLifecycleStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<string> CleanReason(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return Fail<string>("QUOTATION_REASON_REQUIRED", "A reason is required.");
        var trimmed = reason.Trim();
        return trimmed.Length > MaxReasonLength ? Fail<string>("QUOTATION_FIELD_INVALID", $"The reason cannot exceed {MaxReasonLength} characters.") : Result<string>.Success(trimmed);
    }

    public async Task<Result<QuotationHistoryProjection>> VoidAsync(QuotationLifecycleCaller caller, Guid quotationId, Guid expectedVersion, string? reason, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.void", ct);
        if (access.IsFailure) return Result<QuotationHistoryProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<QuotationHistoryProjection>("QUOTATION_FIELD_INVALID", "Expected version is required.");
        var clean = CleanReason(reason);
        if (clean.IsFailure) return Result<QuotationHistoryProjection>.Failure(clean.Error);
        return await _store.VoidAsync(access.Value!, quotationId, expectedVersion, clean.Value!, caller.TraceId, ct);
    }

    public async Task<Result<QuotationHistoryProjection>> AmendAsync(QuotationLifecycleCaller caller, Guid quotationId, Guid expectedVersion, string idempotencyKey, string? reason, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.amend", ct);
        if (access.IsFailure) return Result<QuotationHistoryProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<QuotationHistoryProjection>("QUOTATION_FIELD_INVALID", "Expected version is required.");
        var clean = CleanReason(reason);
        if (clean.IsFailure) return Result<QuotationHistoryProjection>.Failure(clean.Error);
        return await _store.AmendAsync(access.Value!, quotationId, expectedVersion, clean.Value!, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute($"{quotationId}|{expectedVersion}|{clean.Value}"), caller.TraceId, ct);
    }

    public async Task<Result<QuotationHistoryProjection>> GetHistoryAsync(QuotationLifecycleCaller caller, Guid estimateId, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.read", ct);
        if (access.IsFailure) return Result<QuotationHistoryProjection>.Failure(access.Error);
        var history = await _store.GetHistoryAsync(access.Value!.OrganizationId, estimateId, ct);
        return history is null ? Fail<QuotationHistoryProjection>("RESOURCE_NOT_FOUND", "No quotation found for the estimate.") : Result<QuotationHistoryProjection>.Success(history);
    }
}
