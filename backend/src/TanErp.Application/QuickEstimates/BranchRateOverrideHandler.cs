using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.QuickEstimates;

/// <summary>Branch rate override use cases. Permission is resolved from PostgreSQL per call; the store enforces branch scope and maker–checker.</summary>
public class BranchRateOverrideHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IBranchRateOverrideStore _store;

    public BranchRateOverrideHandler(IRequestAccessResolver accessResolver, IBranchRateOverrideStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(QeCaller caller, string permission, CancellationToken ct) => _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    public async Task<Result<BranchOverrideProjection>> RequestAsync(QeCaller caller, Guid templateId, string idempotencyKey, BranchOverrideInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.override-rate", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (input is null) return Fail<BranchOverrideProjection>("BRANCH_OVERRIDE_INVALID", "A request body is required.");
        var payload = FormattableString.Invariant($"{templateId:N}|{input.ReferenceRate}|{input.MinimumCharge}|{input.EffectiveFrom:yyyy-MM-dd}|{input.EffectiveTo:yyyy-MM-dd}|{input.ReasonCode?.Trim()}|{input.ReasonDetail?.Trim()}|{input.SourceReference?.Trim()}");
        return await _store.RequestAsync(access.Value!, templateId, input, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute(payload), caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<BranchOverrideProjection>>> ListAsync(QeCaller caller, Guid templateId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<BranchOverrideProjection>>.Failure(access.Error);
        return Result<IReadOnlyList<BranchOverrideProjection>>.Success(await _store.ListAsync(access.Value!, templateId, ct));
    }

    public async Task<Result<BranchOverrideProjection>> DecideAsync(QeCaller caller, Guid templateId, Guid overrideId, Guid expectedVersion, string? decision, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.approve", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        if (decision is not ("approved" or "returned")) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The decision must be approved or returned.");
        return await _store.DecideAsync(access.Value!, templateId, overrideId, expectedVersion, decision == "approved", note, caller.TraceId, ct);
    }

    public async Task<Result<BranchOverrideProjection>> RevokeAsync(QeCaller caller, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.approve", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        return await _store.RevokeAsync(access.Value!, templateId, overrideId, expectedVersion, reason, caller.TraceId, ct);
    }
}
