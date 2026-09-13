namespace TanErp.Application.Surveys.MarkSurveyReady;

public sealed record MarkSurveyReadyCommand(
    Guid SiteSurveyId,
    Guid RevisionId,
    Guid ExpectedRevisionVersion,
    Guid ExpectedOpportunityVersion,
    string FirebaseUid,
    Guid MembershipId,
    string IdempotencyKey,
    string TraceId);
