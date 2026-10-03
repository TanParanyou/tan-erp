namespace TanErp.Application.Estimates.CreateEstimateRevision;

public sealed record CreateEstimateRevisionCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedEstimateVersion,
    string Reason);
