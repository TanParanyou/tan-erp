namespace TanErp.Application.Estimates.CancelEstimate;

public sealed record CancelEstimateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedEstimateVersion,
    string Reason);
