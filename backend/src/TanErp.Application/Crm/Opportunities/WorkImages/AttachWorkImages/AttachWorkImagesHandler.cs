using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Opportunities.WorkImages.AttachWorkImages;

public class AttachWorkImagesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public AttachWorkImagesHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<AttachWorkImagesResultProjection>> Handle(
        AttachWorkImagesCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<AttachWorkImagesResultProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<AttachWorkImagesResultProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to attach work images."));
        }

        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<AttachWorkImagesResultProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        if (command.Images == null || command.Images.Count == 0 || command.Images.Count > 20)
        {
            return Result<AttachWorkImagesResultProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Between 1 and 20 images must be attached."));
        }

        foreach (var img in command.Images)
        {
            if (img.FileId == Guid.Empty)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", "File ID is required for each image."));
            }

            if (img.Caption != null && img.Caption.Length > 500)
            {
                return Result<AttachWorkImagesResultProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", "Caption cannot exceed 500 characters."));
            }
        }

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant(
            $"{command.OpportunityId:D}|{command.ExpectedVersion:D}|{string.Join(",", command.Images.Select(i => $"{i.FileId:D}:{i.Caption ?? string.Empty}"))}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        return await _store.AttachWorkImagesAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
