namespace TanErp.Application.Estimates.CalculateEstimate;

public sealed record CalculateEstimateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid RevisionId,
    Guid ExpectedRevisionVersion,
    decimal DiscountAmount);
