namespace TanErp.Application.Crm.Opportunities.QualifyOpportunity;

public sealed record QualifyOpportunityCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    string TargetStage,
    Guid ExpectedVersion,
    string IdempotencyKey,
    string TraceId);
