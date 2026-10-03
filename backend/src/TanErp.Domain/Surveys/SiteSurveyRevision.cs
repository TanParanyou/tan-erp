using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyRevision : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string SurveyTemplateVersion { get; private set; } = string.Empty;
    public DateTimeOffset? VisitedAtUtc { get; private set; }
    public string? ScopeSummary { get; private set; }
    public IReadOnlyCollection<string> Assumptions { get; private set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Constraints { get; private set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> MissingDetails { get; private set; } = Array.Empty<string>();
    public string Readiness { get; private set; } = SurveyReadiness.Incomplete;
    public string Status { get; private set; } = SurveyRevisionStatus.Draft;
    public DateTimeOffset? ReadyAtUtc { get; private set; }
    public Guid? ReadyByUserId { get; private set; }
    public string? SnapshotHash { get; private set; }
    public Guid? SourceRevisionId { get; private set; }
    public string? CloneReason { get; private set; }
    public string? VoidReason { get; private set; }
    public DateTimeOffset? VoidedAtUtc { get; private set; }
    public Guid? VoidedByUserId { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private readonly List<SiteSurveyArea> _areas = new();
    public IReadOnlyCollection<SiteSurveyArea> Areas => _areas.AsReadOnly();

    private readonly List<SiteSurveyChecklistResult> _checklistResults = new();
    public IReadOnlyCollection<SiteSurveyChecklistResult> ChecklistResults => _checklistResults.AsReadOnly();

    private readonly List<SiteSurveyEvidence> _evidence = new();
    public IReadOnlyCollection<SiteSurveyEvidence> Evidence => _evidence.AsReadOnly();

    protected SiteSurveyRevision() { }

    private SiteSurveyRevision(
        Guid id,
        Guid organizationId,
        Guid siteSurveyId,
        int revisionNumber,
        string surveyTemplateVersion,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyId == Guid.Empty) throw new ArgumentException("Site survey ID cannot be empty.", nameof(siteSurveyId));
        if (revisionNumber <= 0) throw new ArgumentException("Revision number must be positive.", nameof(revisionNumber));
        if (string.IsNullOrWhiteSpace(surveyTemplateVersion)) throw new ArgumentException("Survey template version cannot be blank.", nameof(surveyTemplateVersion));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));

        OrganizationId = organizationId;
        SiteSurveyId = siteSurveyId;
        RevisionNumber = revisionNumber;
        SurveyTemplateVersion = surveyTemplateVersion.Trim();
        Readiness = SurveyReadiness.Incomplete;
        Status = SurveyRevisionStatus.Draft;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
    }

    public void AddArea(SiteSurveyArea area)
    {
        ArgumentNullException.ThrowIfNull(area);
        _areas.Add(area);
    }

    public void UpdateDraft(
        DateTimeOffset? visitedAtUtc,
        string? scopeSummary,
        IEnumerable<string>? assumptions,
        IEnumerable<string>? constraints,
        IEnumerable<string>? missingDetails)
    {
        if (Status != SurveyRevisionStatus.Draft)
        {
            throw new SurveyInvalidStateException(Status, $"Cannot edit survey revision in status '{Status}'. Only draft revisions can be edited.");
        }

        VisitedAtUtc = visitedAtUtc?.ToUniversalTime();
        ScopeSummary = string.IsNullOrWhiteSpace(scopeSummary) ? null : scopeSummary.Trim();
        Assumptions = assumptions?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
        Constraints = constraints?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
        MissingDetails = missingDetails?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
        RowVersion = Guid.NewGuid();
    }

    public void MarkReady(Guid actorUserId, DateTimeOffset now, string snapshotHash)
    {
        if (Status != SurveyRevisionStatus.Draft)
        {
            throw new SurveyInvalidStateException(Status, $"Cannot mark survey revision ready when in status '{Status}'.");
        }

        if (!VisitedAtUtc.HasValue)
        {
            throw new SurveyReadinessException(nameof(VisitedAtUtc), "Visit date/time is required to mark revision ready.");
        }

        if (string.IsNullOrWhiteSpace(ScopeSummary))
        {
            throw new SurveyReadinessException(nameof(ScopeSummary), "Scope summary is required to mark revision ready.");
        }

        if (_areas.Count == 0)
        {
            throw new SurveyReadinessException(nameof(Areas), "At least one survey area is required to mark revision ready.");
        }

        foreach (var area in _areas)
        {
            if (area.Measurements.Count == 0)
            {
                throw new SurveyReadinessException(nameof(SiteSurveyMeasurement), $"Area '{area.Name}' must have at least one measurement.");
            }

            foreach (var m in area.Measurements)
            {
                if (m.Value <= 0)
                {
                    throw new SurveyReadinessException(nameof(m.Value), $"Measurement '{m.MeasurementType}' in area '{area.Name}' must be strictly positive.");
                }
            }
        }

        EnsureTemplateRequirementsMet();

        Readiness = SurveyReadiness.Ready;
        Status = SurveyRevisionStatus.Ready;
        ReadyAtUtc = now.ToUniversalTime();
        ReadyByUserId = actorUserId;
        SnapshotHash = snapshotHash;
        RowVersion = Guid.NewGuid();
    }

    private void EnsureTemplateRequirementsMet()
    {
        var template = SurveyTemplates.Find(SurveyTemplateVersion)
            ?? throw new SurveyReadinessException(nameof(SurveyTemplateVersion), $"Survey template '{SurveyTemplateVersion}' is not available.");

        foreach (var itemCode in template.RequiredChecklistItems)
        {
            var result = _checklistResults.FirstOrDefault(r => string.Equals(r.ItemCode, itemCode, StringComparison.Ordinal));
            if (result == null)
            {
                throw new SurveyReadinessException(nameof(ChecklistResults), $"Checklist item '{itemCode}' must be answered to mark revision ready.");
            }

            if (result.Result != ChecklistResultValue.Pass && string.IsNullOrWhiteSpace(result.Note))
            {
                throw new SurveyReadinessException(nameof(ChecklistResults), $"Checklist item '{itemCode}' needs a note unless it passed.");
            }
        }

        if (_evidence.Count < template.MinimumEvidenceCount)
        {
            throw new SurveyReadinessException(nameof(Evidence), $"At least {template.MinimumEvidenceCount} evidence file(s) are required to mark revision ready.");
        }
    }

    public void AddChecklistResult(SiteSurveyChecklistResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _checklistResults.Add(result);
    }

    public void AddEvidence(SiteSurveyEvidence evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        _evidence.Add(evidence);
    }

    public void Supersede()
    {
        if (Status != SurveyRevisionStatus.Ready)
        {
            throw new SurveyInvalidStateException(Status, $"Only a ready revision can be superseded; current status is '{Status}'.");
        }

        Status = SurveyRevisionStatus.Superseded;
        RowVersion = Guid.NewGuid();
    }

    public void Void(Guid actorUserId, DateTimeOffset now, string reason)
    {
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor user ID cannot be empty.", nameof(actorUserId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A void reason is required.", nameof(reason));

        if (Status == SurveyRevisionStatus.Void)
        {
            throw new SurveyInvalidStateException(Status, "Survey revision is already void.");
        }

        Status = SurveyRevisionStatus.Void;
        VoidReason = reason.Trim();
        VoidedAtUtc = now.ToUniversalTime();
        VoidedByUserId = actorUserId;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>
    /// Builds a new draft revision that copies the business content of <paramref name="source"/>.
    /// The source is left untouched; area and measurement rows are duplicated with new identities.
    /// </summary>
    public static SiteSurveyRevision CloneFrom(
        SiteSurveyRevision source,
        int revisionNumber,
        string reason,
        Guid createdByUserId,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A clone reason is required.", nameof(reason));

        if (source.Status == SurveyRevisionStatus.Draft)
        {
            throw new SurveyInvalidStateException(source.Status, "A draft revision is edited in place and cannot be cloned.");
        }

        var clone = new SiteSurveyRevision(
            Guid.NewGuid(),
            source.OrganizationId,
            source.SiteSurveyId,
            revisionNumber,
            source.SurveyTemplateVersion,
            createdByUserId,
            now)
        {
            VisitedAtUtc = source.VisitedAtUtc,
            ScopeSummary = source.ScopeSummary,
            Assumptions = source.Assumptions.ToArray(),
            Constraints = source.Constraints.ToArray(),
            MissingDetails = source.MissingDetails.ToArray(),
            SourceRevisionId = source.Id,
            CloneReason = reason.Trim()
        };

        foreach (var area in source.Areas.OrderBy(a => a.SortOrder))
        {
            var areaCopy = new SiteSurveyArea(
                Guid.NewGuid(),
                source.OrganizationId,
                clone.Id,
                area.Code,
                area.Name,
                area.Description,
                area.SortOrder);

            foreach (var m in area.Measurements.OrderBy(x => x.SortOrder))
            {
                areaCopy.AddMeasurement(new SiteSurveyMeasurement(
                    Guid.NewGuid(),
                    source.OrganizationId,
                    areaCopy.Id,
                    m.MeasurementType,
                    m.Value,
                    m.UnitCode,
                    m.CaptureMethod,
                    m.Notes,
                    m.SortOrder));
            }

            clone.AddArea(areaCopy);
        }

        foreach (var r in source.ChecklistResults)
        {
            clone.AddChecklistResult(new SiteSurveyChecklistResult(
                Guid.NewGuid(), source.OrganizationId, clone.Id, r.ItemCode, r.Result, r.Note));
        }

        foreach (var e in source.Evidence.OrderBy(x => x.SortOrder))
        {
            clone.AddEvidence(new SiteSurveyEvidence(
                Guid.NewGuid(), source.OrganizationId, clone.Id, e.FileId, e.Kind, e.Caption, e.SortOrder));
        }

        return clone;
    }

    public static SiteSurveyRevision CreateBaseline(
        Guid organizationId,
        Guid siteSurveyId,
        Guid createdByUserId,
        DateTimeOffset now,
        string surveyTemplateVersion = SurveyDefaults.BaselineTemplateVersion)
    {
        return new SiteSurveyRevision(
            Guid.NewGuid(),
            organizationId,
            siteSurveyId,
            revisionNumber: 1,
            surveyTemplateVersion: surveyTemplateVersion,
            createdByUserId: createdByUserId,
            now: now);
    }
}

