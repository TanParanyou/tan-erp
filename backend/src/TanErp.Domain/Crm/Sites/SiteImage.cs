using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Sites;

/// <summary>
/// Represents a photo attached to a Site (job site / location).
/// Belongs to the crm schema, referencing a verified UploadedFile in the files schema.
/// </summary>
public class SiteImage : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteId { get; private set; }
    public Guid FileId { get; private set; }
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected SiteImage() { }

    public SiteImage(
        Guid id,
        Guid organizationId,
        Guid siteId,
        Guid fileId,
        string? caption,
        int displayOrder,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));

        if (siteId == Guid.Empty)
            throw new ArgumentException("Site ID cannot be empty.", nameof(siteId));

        if (fileId == Guid.Empty)
            throw new ArgumentException("File ID cannot be empty.", nameof(fileId));

        if (displayOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order must be non-negative.");

        if (caption != null && caption.Length > 500)
            throw new ArgumentException("Caption cannot exceed 500 characters.", nameof(caption));

        OrganizationId = organizationId;
        SiteId = siteId;
        FileId = fileId;
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
