using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.CalculationPolicies;

public sealed class ManageCalculationPoliciesHandler
{
    private const string ReadPermission = "estimates.read";
    private const string CreatePermission = "estimates.update";
    private const string PublishPermission = "estimates.approve";

    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICalculationPolicyStore _store;
    private readonly IClock _clock;

    public ManageCalculationPoliciesHandler(
        IRequestAccessResolver accessResolver,
        ICalculationPolicyStore store,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _store = store;
        _clock = clock;
    }

    public async Task<Result<IReadOnlyList<CalculationPolicyVersion>>> ListAsync(
        string firebaseUid, Guid membershipId, Guid? branchId, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(firebaseUid, membershipId, ReadPermission, ct);
        if (access.IsFailure) return Result<IReadOnlyList<CalculationPolicyVersion>>.Failure(access.Error);

        var orgId = access.Value!.OrganizationId;
        if (branchId.HasValue && !access.Value.HasBranchAccess(branchId.Value))
        {
            return Result<IReadOnlyList<CalculationPolicyVersion>>.Failure(
                new Error("FORBIDDEN", "Caller does not have access to the requested branch."));
        }

        var list = await _store.ListAsync(orgId, branchId, ct);
        return Result<IReadOnlyList<CalculationPolicyVersion>>.Success(list);
    }

    public async Task<Result<CalculationPolicyVersion>> CreateDraftAsync(
        string firebaseUid, Guid membershipId, CreateCalculationPolicyCommand command, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(firebaseUid, membershipId, CreatePermission, ct);
        if (access.IsFailure) return Result<CalculationPolicyVersion>.Failure(access.Error);

        var caller = access.Value!;
        if (command.BranchId.HasValue && !caller.HasBranchAccess(command.BranchId.Value))
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("FORBIDDEN", "Caller does not have access to the requested branch."));
        }

        return await _store.CreateDraftAsync(
            caller.OrganizationId,
            command.BranchId,
            command.PolicyCode,
            command.Version,
            command.OverheadMethod,
            command.OverheadValue,
            command.RoundingMode,
            command.UnitConversionMode,
            command.EffectiveFromUtc,
            command.EffectiveToUtc,
            caller.ActorUserId,
            ct);
    }

    public async Task<Result<CalculationPolicyVersion>> PublishAsync(
        string firebaseUid, Guid membershipId, Guid policyId, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(firebaseUid, membershipId, PublishPermission, ct);
        if (access.IsFailure) return Result<CalculationPolicyVersion>.Failure(access.Error);

        var caller = access.Value!;
        var existing = await _store.GetByIdAsync(caller.OrganizationId, policyId, ct);
        if (existing is null)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Calculation policy was not found."));
        }

        if (existing.BranchId.HasValue && !caller.HasBranchAccess(existing.BranchId.Value))
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("FORBIDDEN", "Caller does not have access to the policy branch."));
        }

        // Maker-Checker enforcement: creator cannot publish their own draft
        if (existing.CreatedByUserId == caller.ActorUserId)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("MAKER_CHECKER_VIOLATION", "Calculation policy publication requires an independent checker. The author cannot publish their own draft."));
        }

        return await _store.PublishAsync(caller.OrganizationId, policyId, caller.ActorUserId, _clock.UtcNow, ct);
    }
}
