using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner;

public class ReassignOpportunityOwnerHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public ReassignOpportunityOwnerHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        ReassignOpportunityOwnerCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required opportunities.update permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.update",
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
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to reassign opportunity owner."));
        }

        // 3. ExpectedVersion required
        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        // 4. TargetOwnerUserId required
        if (command.TargetOwnerUserId == Guid.Empty)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Target owner user ID is required."));
        }

        // 5. Deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant(
            $"{command.OpportunityId:D}|{command.ExpectedVersion:D}|{command.TargetOwnerUserId:D}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        // 6. Delegate to store
        return await _store.ReassignOwnerAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
