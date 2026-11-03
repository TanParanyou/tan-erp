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

/// <summary>
/// MinValue/MaxValue bound a plausible single measurement in <paramref name="UnitCode"/> (per dimension for box).
/// They only drive non-blocking warnings in clients; they are not validation limits.
/// </summary>
public sealed record SurveyMeasurementTypeOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string UnitCode,
    int SortOrder,
    decimal? MinValue = null,
    decimal? MaxValue = null);

public sealed record SurveyDesignStyleOptionResponse(
    string Code,
    SurveyLocalizedLabel Name,
    string? Description,
    int SortOrder,
    Guid? ImageFileId = null,
    IReadOnlyList<string>? ImageFileIds = null);

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
    IReadOnlyList<SurveyMoodToneOptionResponse> MoodTones,
    bool NarrativeAssistEnabled = false);
