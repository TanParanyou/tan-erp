using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyArea : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyRevisionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<SiteSurveyMeasurement> _measurements = new();
    public IReadOnlyCollection<SiteSurveyMeasurement> Measurements => _measurements.AsReadOnly();

    protected SiteSurveyArea() { }

    public SiteSurveyArea(
        Guid id,
        Guid organizationId,
        Guid siteSurveyRevisionId,
        string code,
        string name,
        string? description,
        int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyRevisionId == Guid.Empty) throw new ArgumentException("Site survey revision ID cannot be empty.", nameof(siteSurveyRevisionId));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Area code cannot be blank.", nameof(code));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Area name cannot be blank.", nameof(name));

        OrganizationId = organizationId;
        SiteSurveyRevisionId = siteSurveyRevisionId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        SortOrder = sortOrder;
    }

    public void AddMeasurement(SiteSurveyMeasurement measurement)
    {
        ArgumentNullException.ThrowIfNull(measurement);
        _measurements.Add(measurement);
    }
}
