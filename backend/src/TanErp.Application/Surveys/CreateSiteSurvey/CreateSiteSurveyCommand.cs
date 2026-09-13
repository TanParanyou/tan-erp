namespace TanErp.Application.Surveys.CreateSiteSurvey;

public sealed record CreateSiteSurveyCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid SiteId,
    Guid AssignedSurveyorId,
    DateTimeOffset? ScheduledStartUtc,
    DateTimeOffset? ScheduledEndUtc,
    Guid ExpectedOpportunityVersion,
    string IdempotencyKey,
    string TraceId);
