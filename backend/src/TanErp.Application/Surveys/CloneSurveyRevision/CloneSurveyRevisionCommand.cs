namespace TanErp.Application.Surveys.CloneSurveyRevision;

public sealed record CloneSurveyRevisionCommand(
    Guid OpportunityId,
    Guid SiteSurveyId,
    Guid SourceRevisionId,
    string Reason,
    string FirebaseUid,
    Guid MembershipId,
    string IdempotencyKey,
    string TraceId);
