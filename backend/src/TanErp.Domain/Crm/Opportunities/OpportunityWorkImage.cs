using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Opportunities;

/// <summary>
/// Represents a work photo attached to an Opportunity.
/// Belongs to the crm schema, referencing a verified UploadedFile in the files schema.
/// </summary>
public class OpportunityWorkImage : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public Guid FileId { get; private set; }
    public string StageAtAttach { get; private set; } = string.Empty;
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected OpportunityWorkImage() { }

    public OpportunityWorkImage(
        Guid id,
        Guid organizationId,
        Guid opportunityId,
        Guid fileId,
        string stageAtAttach,
        string? caption,
        int displayOrder,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));

        if (opportunityId == Guid.Empty)
            throw new ArgumentException("Opportunity ID cannot be empty.", nameof(opportunityId));

        if (fileId == Guid.Empty)
            throw new ArgumentException("File ID cannot be empty.", nameof(fileId));

        if (string.IsNullOrWhiteSpace(stageAtAttach))
            throw new ArgumentException("Stage at attach cannot be blank.", nameof(stageAtAttach));

        if (displayOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order must be non-negative.");

        if (caption != null && caption.Length > 500)
            throw new ArgumentException("Caption cannot exceed 500 characters.", nameof(caption));

        OrganizationId = organizationId;
        OpportunityId = opportunityId;
        FileId = fileId;
        StageAtAttach = stageAtAttach.Trim().ToLowerInvariant();
        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
        DisplayOrder = displayOrder;
        IsDeleted = false;
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
