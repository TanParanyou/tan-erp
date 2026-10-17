namespace TanErp.Api.Contracts.Surveys;

public sealed record SurveyTemplateVersionResponse(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    int MinimumAreaEvidenceCount,
    string SnapshotHashVersion,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    bool IsCurrent);

public sealed record SurveyTemplateVersionListResponse(
    IReadOnlyList<SurveyTemplateVersionResponse> Items);

public sealed record PublishSurveyTemplateVersionRequest(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    int MinimumAreaEvidenceCount,
    string SnapshotHashVersion,
    DateTimeOffset? EffectiveFromUtc);
