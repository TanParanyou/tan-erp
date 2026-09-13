namespace TanErp.Api.Contracts.Surveys;

public sealed record MarkSurveyReadyRequest(
    Guid ExpectedRevisionVersion,
    Guid ExpectedOpportunityVersion);
