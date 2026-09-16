using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Opportunities.WorkImages.DetachWorkImage;

public class DetachWorkImageHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public DetachWorkImageHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<Guid>> Handle(
        DetachWorkImageCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<Guid>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to detach work images."));
        }

        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<Guid>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant(
            $"{command.OpportunityId:D}|{command.WorkImageId:D}|{command.ExpectedVersion:D}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        return await _store.DetachWorkImageAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
