using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyEvidence : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyRevisionId { get; private set; }
    public Guid FileId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public string? Caption { get; private set; }
    public int SortOrder { get; private set; }

    protected SiteSurveyEvidence() { }

    public SiteSurveyEvidence(
        Guid id,
        Guid organizationId,
        Guid siteSurveyRevisionId,
        Guid fileId,
        string kind,
        string? caption,
        int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyRevisionId == Guid.Empty) throw new ArgumentException("Site survey revision ID cannot be empty.", nameof(siteSurveyRevisionId));
        if (fileId == Guid.Empty) throw new ArgumentException("File ID cannot be empty.", nameof(fileId));
        if (!EvidenceKind.IsValid(kind)) throw new ArgumentException($"Invalid evidence kind: '{kind}'.", nameof(kind));

        OrganizationId = organizationId;
        SiteSurveyRevisionId = siteSurveyRevisionId;
        FileId = fileId;
        Kind = kind.Trim();
        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
        SortOrder = sortOrder;
    }
}
