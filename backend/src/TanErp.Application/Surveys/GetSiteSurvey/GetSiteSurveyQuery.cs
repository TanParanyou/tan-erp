namespace TanErp.Application.Surveys.GetSiteSurvey;

public sealed record GetSiteSurveyQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId);
