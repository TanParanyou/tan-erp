using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Opportunities;

namespace TanErp.Application.Crm.Opportunities.QualifyOpportunity;

public class QualifyOpportunityHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public QualifyOpportunityHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        QualifyOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required opportunities.transition permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.transition",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<OpportunityProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Require active branch
        if (!access.BranchId.HasValue)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to qualify an opportunity."));
        }

        // 3. ExpectedVersion required
        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        // 4. Normalize target and validate
        var normalizedTarget = command.TargetStage?.Trim().ToLowerInvariant();
        if (normalizedTarget != OpportunityStage.Qualified)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_INVALID_TRANSITION", $"Cannot transition opportunity to target stage '{command.TargetStage}'."));
        }

        // 5. Compute deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant($"{command.OpportunityId:D}|qualified|{command.ExpectedVersion:D}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        var normalizedCommand = command with { TargetStage = normalizedTarget };

        // 6. Delegate to store
        return await _store.QualifyAsync(access, normalizedCommand, keyHash, payloadHash, cancellationToken);
    }
}
