using System.Text.RegularExpressions;
using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SurveyTemplateInvalidException : Exception
{
    public SurveyTemplateInvalidException(string message) : base(message) { }
}

/// <summary>Persisted survey template version. Content is fixed at publish; only the effective period can end (once).</summary>
public class SurveyTemplateVersionRow : Entity
{
    private static readonly Regex CodePattern = new("^[A-Za-z0-9][A-Za-z0-9._-]{2,63}$", RegexOptions.Compiled);
    private static readonly Regex ItemPattern = new("^[a-z][a-z0-9_]{1,63}$", RegexOptions.Compiled);

    public string Code { get; private set; } = string.Empty;
    public string[] RequiredChecklistItems { get; private set; } = Array.Empty<string>();
    public int MinimumEvidenceCount { get; private set; }
    public int MinimumAreaEvidenceCount { get; private set; }
    public string SnapshotHashVersion { get; private set; } = string.Empty;
    public DateTimeOffset EffectiveFromUtc { get; private set; }
    public DateTimeOffset? EffectiveToUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    protected SurveyTemplateVersionRow() { }

    public SurveyTemplateVersionRow(
        Guid id,
        string code,
        IEnumerable<string> requiredChecklistItems,
        int minimumEvidenceCount,
        int minimumAreaEvidenceCount,
        string snapshotHashVersion,
        DateTimeOffset effectiveFromUtc,
        DateTimeOffset now,
        Guid createdByUserId) : base(id)
    {
        var trimmedCode = code?.Trim() ?? string.Empty;
        if (!CodePattern.IsMatch(trimmedCode)) throw new SurveyTemplateInvalidException("Template code must be 3-64 letters, digits, '.', '_' or '-'.");

        var items = (requiredChecklistItems ?? Array.Empty<string>()).Select(i => i?.Trim() ?? string.Empty).ToArray();
        if (items.Any(i => !ItemPattern.IsMatch(i)) || items.Distinct(StringComparer.Ordinal).Count() != items.Length)
            throw new SurveyTemplateInvalidException("Checklist item codes must be unique snake_case codes.");

        // v2 is reserved for the legacy baseline; new templates always hash with at least v3.
        if (snapshotHashVersion != "v3" && snapshotHashVersion != SurveyTemplateVersion.AreaEvidenceHashVersion)
            throw new SurveyTemplateInvalidException("Snapshot hash version must be 'v3' or 'v4'.");

        if (minimumEvidenceCount < 0 || minimumAreaEvidenceCount < 0 || minimumEvidenceCount > 50 || minimumAreaEvidenceCount > 50)
            throw new SurveyTemplateInvalidException("Minimum evidence counts must be between 0 and 50.");

        if (minimumAreaEvidenceCount > 0 && snapshotHashVersion != SurveyTemplateVersion.AreaEvidenceHashVersion)
            throw new SurveyTemplateInvalidException("Area evidence requires snapshot hash version 'v4'.");

        Code = trimmedCode;
        RequiredChecklistItems = items;
        MinimumEvidenceCount = minimumEvidenceCount;
        MinimumAreaEvidenceCount = minimumAreaEvidenceCount;
        SnapshotHashVersion = snapshotHashVersion;
        EffectiveFromUtc = effectiveFromUtc.ToUniversalTime();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
    }

    public void Retire(DateTimeOffset now)
    {
        if (EffectiveToUtc.HasValue) throw new SurveyTemplateInvalidException("Template is already retired.");
        EffectiveToUtc = now.ToUniversalTime();
    }

    public SurveyTemplateVersion ToTemplate() =>
        new(Code, RequiredChecklistItems, MinimumEvidenceCount, SnapshotHashVersion, MinimumAreaEvidenceCount, EffectiveFromUtc, EffectiveToUtc);
}
