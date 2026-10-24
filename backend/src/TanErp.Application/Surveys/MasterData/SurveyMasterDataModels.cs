namespace TanErp.Application.Surveys.MasterData;

public sealed record LocalizedNameInput(string Thai, string? English);

public sealed record CreateSurveyDesignStyleInput(
    string Code,
    LocalizedNameInput Name,
    LocalizedNameInput? Description,
    int SortOrder = 0);

public sealed record UpdateSurveyDesignStyleInput(
    LocalizedNameInput Name,
    LocalizedNameInput? Description,
    int SortOrder,
    bool IsActive);

public sealed record CreateSurveyMoodToneInput(
    string Code,
    LocalizedNameInput Name,
    LocalizedNameInput? Description,
    IReadOnlyList<string>? ColorPalette,
    int SortOrder = 0);

public sealed record UpdateSurveyMoodToneInput(
    LocalizedNameInput Name,
    LocalizedNameInput? Description,
    IReadOnlyList<string>? ColorPalette,
    int SortOrder,
    bool IsActive);

public sealed record LocalizedNameDto(string Thai, string? English);

public sealed record SurveyDesignStyleDto(
    Guid Id,
    string Code,
    LocalizedNameDto Name,
    LocalizedNameDto? Description,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);

public sealed record SurveyMoodToneDto(
    Guid Id,
    string Code,
    LocalizedNameDto Name,
    LocalizedNameDto? Description,
    IReadOnlyList<string> ColorPalette,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);
