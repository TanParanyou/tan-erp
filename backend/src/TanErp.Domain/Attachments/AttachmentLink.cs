using TanErp.Domain.Common;

namespace TanErp.Domain.Attachments;

/// <summary>Links one verified file to one registered owner record for a purpose. Removing a link never deletes the file.</summary>
public class AttachmentLink : Entity
{
    public const int MaxActiveLinksPerOwner = 50;

    public Guid OrganizationId { get; private set; }
    public string OwnerType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public Guid FileId { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? RemovedAtUtc { get; private set; }
    public Guid? RemovedByUserId { get; private set; }

    public bool IsActive => RemovedAtUtc is null;

    protected AttachmentLink() { }

    public AttachmentLink(
        Guid id, Guid organizationId, string ownerType, Guid ownerId, Guid fileOrganizationId, Guid fileId,
        string purpose, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || ownerId == Guid.Empty || fileId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new AttachmentDomainException("ATTACHMENT_FIELD_INVALID", "Organization, owner, file and actor are required.");
        }

        var normalizedOwner = AttachmentOwnerTypes.Normalize(ownerType)
            ?? throw new AttachmentDomainException("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");
        var normalizedPurpose = AttachmentPurposes.Normalize(purpose)
            ?? throw new AttachmentDomainException("ATTACHMENT_PURPOSE_INVALID", $"Purpose '{purpose}' is not supported.");

        if (fileOrganizationId != organizationId)
        {
            throw new AttachmentDomainException("ATTACHMENT_FILE_SCOPE_MISMATCH", "The file does not belong to the owner's organization.");
        }

        OrganizationId = organizationId;
        OwnerType = normalizedOwner;
        OwnerId = ownerId;
        FileId = fileId;
        Purpose = normalizedPurpose;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    /// <summary>Rejects a duplicate (same owner, file and purpose) and enforces the per-owner limit. Removed links are ignored.</summary>
    public static void AssertCanAdd(IReadOnlyCollection<AttachmentLink> activeLinksOfOwner, AttachmentLink candidate)
    {
        ArgumentNullException.ThrowIfNull(activeLinksOfOwner);
        ArgumentNullException.ThrowIfNull(candidate);

        var active = activeLinksOfOwner.Where(l => l.IsActive).ToList();
        if (active.Any(l => l.OrganizationId == candidate.OrganizationId && l.OwnerType == candidate.OwnerType
                            && l.OwnerId == candidate.OwnerId && l.FileId == candidate.FileId && l.Purpose == candidate.Purpose))
        {
            throw new AttachmentDomainException("ATTACHMENT_DUPLICATE", "This file is already attached to this record for the same purpose.");
        }

        if (active.Count >= MaxActiveLinksPerOwner)
        {
            throw new AttachmentDomainException("ATTACHMENT_LIMIT_EXCEEDED", $"A record can have at most {MaxActiveLinksPerOwner} attachments.");
        }
    }

    public void Remove(Guid actorUserId, DateTimeOffset now)
    {
        if (!IsActive) throw new AttachmentDomainException("RESOURCE_NOT_FOUND", "Attachment not found.");
        RemovedByUserId = actorUserId;
        RemovedAtUtc = now.ToUniversalTime();
    }
}
