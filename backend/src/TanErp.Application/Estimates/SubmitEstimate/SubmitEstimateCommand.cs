namespace TanErp.Application.Estimates.SubmitEstimate;

public sealed record SubmitEstimateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedEstimateVersion,
    int RevisionNo,
    int CalculationVersion,
    string? Note);
