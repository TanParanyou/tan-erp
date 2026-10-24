namespace TanErp.Api.Contracts.Surveys;

public sealed record SurveyLocalizedLabel(string Thai, string English);

public sealed record SurveyCategoryOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string? Description,
    int SortOrder);

public sealed record SurveyPhotoTagOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string? Description,
    int SortOrder);

public sealed record SurveyMeasurementTypeOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string UnitCode,
    int SortOrder);

public sealed record SurveyDesignStyleOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string? Description,
    int SortOrder);

public sealed record SurveyMoodToneOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string? Description,
    IReadOnlyList<string> ColorPalette,
    int SortOrder);

public sealed record SurveyReferenceDataResponse(
    IReadOnlyList<SurveyCategoryOptionResponse> Categories,
    IReadOnlyList<SurveyPhotoTagOptionResponse> PhotoTags,
    IReadOnlyList<SurveyMeasurementTypeOptionResponse> MeasurementTypes,
    IReadOnlyList<SurveyDesignStyleOptionResponse> DesignStyles,
    IReadOnlyList<SurveyMoodToneOptionResponse> MoodTones);
