namespace TanErp.Application.Crm.Opportunities.WorkImages.AttachWorkImages;

public sealed record WorkImageItemInput(
    Guid FileId,
    string? Caption);

public sealed record AttachWorkImagesCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid ExpectedVersion,
    IReadOnlyList<WorkImageItemInput> Images,
    string IdempotencyKey,
    string TraceId);
