namespace TanErp.Api.Contracts.Surveys;

public sealed record VoidSurveyRevisionRequest(
    Guid ExpectedRevisionVersion,
    string Reason);
