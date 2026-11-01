namespace TanErp.Api.Contracts.Surveys;

public sealed record SurveyLocalizedNameContract(string Thai, string? English);

public sealed record CreateSurveyDesignStyleRequest(
    string Code,
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    int SortOrder = 0,
    Guid? ImageFileId = null,
    IReadOnlyList<string>? ImageFileIds = null);

public sealed record UpdateSurveyDesignStyleRequest(
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    int SortOrder,
    bool IsActive,
    Guid? ImageFileId = null,
    IReadOnlyList<string>? ImageFileIds = null);

public sealed record SurveyDesignStyleResponse(
    Guid Id,
    string Code,
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    Guid? ImageFileId = null,
    IReadOnlyList<string>? ImageFileIds = null);

public sealed record CreateSurveyMoodToneRequest(
    string Code,
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    IReadOnlyList<string>? ColorPalette,
    int SortOrder = 0);

public sealed record UpdateSurveyMoodToneRequest(
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    IReadOnlyList<string>? ColorPalette,
    int SortOrder,
    bool IsActive);

public sealed record SurveyMoodToneResponse(
    Guid Id,
    string Code,
    SurveyLocalizedNameContract Name,
    SurveyLocalizedNameContract? Description,
    IReadOnlyList<string> ColorPalette,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);
