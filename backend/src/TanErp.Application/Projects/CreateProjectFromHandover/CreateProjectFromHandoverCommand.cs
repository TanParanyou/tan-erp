namespace TanErp.Application.Projects.CreateProjectFromHandover;

public sealed record CreateProjectFromHandoverCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid QuotationId,
    Guid ExpectedQuotationVersion,
    Guid OwnerUserId,
    DateOnly? PlannedStartDate,
    string? Name,
    string IdempotencyKey,
    string TraceId);
