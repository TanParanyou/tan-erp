namespace TanErp.Domain.Surveys;

/// <summary>
/// System-owned, immutable survey template version (a row of survey_template_versions). A published version
/// never changes except for ending its effective period, so Ready revisions that reference it stay valid.
/// </summary>
public sealed record SurveyTemplateVersion(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    string SnapshotHashVersion,
    int MinimumAreaEvidenceCount = 0,
    DateTimeOffset EffectiveFromUtc = default,
    DateTimeOffset? EffectiveToUtc = null)
{
    public const string AreaEvidenceHashVersion = "v4";

    /// <summary>Area evidence (attachments owned by a survey area) is part of the snapshot hash only from hash version v4.</summary>
    public bool SupportsAreaEvidence => SnapshotHashVersion == AreaEvidenceHashVersion;

    public bool IsEffectiveAt(DateTimeOffset now) => EffectiveFromUtc <= now && (EffectiveToUtc is null || now < EffectiveToUtc);
}

public static class SurveyChecklistItems
{
    public const string SiteAccessConfirmed = "site_access_confirmed";
    public const string UtilitiesChecked = "utilities_checked";
    public const string ExistingConditionsInspected = "existing_conditions_inspected";
    public const string CustomerRequirementsConfirmed = "customer_requirements_confirmed";
}

/// <summary>
/// The two built-in baseline definitions. They are the source of the seed rows in the AddSurveyTemplateVersions migration and
/// stay here for unit tests and the TEST_ONLY seeder; the runtime source of truth is the survey_template_versions table.
/// </summary>
public static class SurveyTemplates
{
    public static readonly SurveyTemplateVersion BaselineV1 = new(
        SurveyDefaults.BaselineTemplateVersion,
        Array.Empty<string>(),
        MinimumEvidenceCount: 0,
        SnapshotHashVersion: "v2");

    public static readonly SurveyTemplateVersion BaselineV2 = new(
        SurveyDefaults.ChecklistTemplateVersion,
        new[]
        {
            SurveyChecklistItems.SiteAccessConfirmed,
            SurveyChecklistItems.UtilitiesChecked,
            SurveyChecklistItems.ExistingConditionsInspected,
            SurveyChecklistItems.CustomerRequirementsConfirmed
        },
        MinimumEvidenceCount: 1,
        SnapshotHashVersion: "v3");

    public static IReadOnlyList<SurveyTemplateVersion> All { get; } = new[] { BaselineV1, BaselineV2 };

    public static SurveyTemplateVersion? Find(string? code) =>
        All.FirstOrDefault(t => string.Equals(t.Code, code?.Trim(), StringComparison.Ordinal));
}
