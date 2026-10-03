namespace TanErp.Application.Estimates.ReviewEstimate;

public sealed record ReviewEstimateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedEstimateVersion,
    int RevisionNo,
    string Decision,
    string? ReasonCode,
    string? Note);
