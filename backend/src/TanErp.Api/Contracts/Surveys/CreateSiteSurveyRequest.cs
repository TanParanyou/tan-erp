namespace TanErp.Api.Contracts.Surveys;

public sealed record CreateSiteSurveyRequest(
    Guid SiteId,
    Guid AssignedSurveyorId,
    DateTimeOffset? ScheduledStartUtc,
    DateTimeOffset? ScheduledEndUtc,
    Guid ExpectedOpportunityVersion);
