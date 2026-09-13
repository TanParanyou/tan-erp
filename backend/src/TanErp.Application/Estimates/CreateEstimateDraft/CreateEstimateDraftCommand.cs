namespace TanErp.Application.Estimates.CreateEstimateDraft;

public sealed record CreateEstimateDraftCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid CustomerId,
    Guid OpportunityId,
    Guid BranchId,
    Guid? SiteSurveyRevisionId,
    string? SiteSurveySnapshotHash,
    string? Currency);
