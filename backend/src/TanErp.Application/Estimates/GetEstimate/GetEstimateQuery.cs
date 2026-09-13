namespace TanErp.Application.Estimates.GetEstimate;

public sealed record GetEstimateQuery(string FirebaseUid, Guid MembershipId, Guid EstimateId);
public sealed record GetOpportunityEstimateQuery(string FirebaseUid, Guid MembershipId, Guid OpportunityId);
