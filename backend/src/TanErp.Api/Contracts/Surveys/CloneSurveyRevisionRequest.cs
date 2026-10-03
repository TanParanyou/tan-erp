namespace TanErp.Api.Contracts.Surveys;

public sealed record CloneSurveyRevisionRequest(
    Guid SourceRevisionId,
    string Reason);
