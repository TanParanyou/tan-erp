namespace TanErp.Api.Contracts.Surveys;

public sealed record PolishSurveyNarrativeRequest(string Text, string Language);

public sealed record PolishSurveyNarrativeResponse(string Text, string Provider);
