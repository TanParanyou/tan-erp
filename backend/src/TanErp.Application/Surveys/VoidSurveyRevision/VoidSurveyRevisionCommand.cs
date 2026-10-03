namespace TanErp.Application.Surveys.VoidSurveyRevision;

public sealed record VoidSurveyRevisionCommand(
    Guid OpportunityId,
    Guid SiteSurveyId,
    Guid RevisionId,
    Guid ExpectedRevisionVersion,
    string Reason,
    string FirebaseUid,
    Guid MembershipId,
    string IdempotencyKey,
    string TraceId);
