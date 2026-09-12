namespace TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner;

public sealed record ReassignOpportunityOwnerCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid ExpectedVersion,
    Guid TargetOwnerUserId,
    string IdempotencyKey,
    string TraceId);
