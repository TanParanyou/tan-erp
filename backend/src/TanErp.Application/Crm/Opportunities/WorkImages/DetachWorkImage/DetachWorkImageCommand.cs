namespace TanErp.Application.Crm.Opportunities.WorkImages.DetachWorkImage;

public sealed record DetachWorkImageCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid WorkImageId,
    Guid ExpectedVersion,
    string IdempotencyKey,
    string TraceId);
