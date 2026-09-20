namespace TanErp.Application.Estimates.CreateEstimateDraft;

public sealed record CreateEstimateDraftCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid SiteSurveyRevisionId,
    string? Currency);
