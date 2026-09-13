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
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private readonly List<SiteSurveyArea> _areas = new();
    public IReadOnlyCollection<SiteSurveyArea> Areas => _areas.AsReadOnly();

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

        Readiness = SurveyReadiness.Ready;
        Status = SurveyRevisionStatus.Ready;
        ReadyAtUtc = now.ToUniversalTime();
        ReadyByUserId = actorUserId;
        SnapshotHash = snapshotHash;
        RowVersion = Guid.NewGuid();
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

