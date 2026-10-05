using System.Text.RegularExpressions;
using TanErp.Domain.Common;

namespace TanErp.Domain.Attachments;

/// <summary>
/// Evidence that a named person signed (image of a handwritten signature) for a registered owner record.
/// This is a PNG image plus SHA-256 and consent wording version, not a legal electronic signature.
/// </summary>
public class SignatureCapture : Entity
{
    private static readonly Regex LowerSha256 = new("^[0-9a-f]{64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public Guid OrganizationId { get; private set; }
    public string OwnerType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public string SignerName { get; private set; } = string.Empty;
    public string? SignerRole { get; private set; }
    public DateTimeOffset SignedAtUtc { get; private set; }
    public Guid ImageFileId { get; private set; }
    public string ConsentTextVersion { get; private set; } = string.Empty;
    public string ContentHash { get; private set; } = string.Empty;
    public Guid CapturedByUserId { get; private set; }

    protected SignatureCapture() { }

    public SignatureCapture(
        Guid id, Guid organizationId, string ownerType, Guid ownerId, string purpose, string signerName, string? signerRole,
        DateTimeOffset signedAtUtc, Guid imageFileOrganizationId, Guid imageFileId, string consentTextVersion, string contentHash,
        Guid capturedByUserId) : base(id)
    {
        if (organizationId == Guid.Empty || ownerId == Guid.Empty || imageFileId == Guid.Empty || capturedByUserId == Guid.Empty)
        {
            throw new AttachmentDomainException("ATTACHMENT_FIELD_INVALID", "Organization, owner, image file and actor are required.");
        }

        var normalizedOwner = AttachmentOwnerTypes.Normalize(ownerType)
            ?? throw new AttachmentDomainException("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");
        var normalizedPurpose = AttachmentPurposes.Normalize(purpose);
        if (normalizedPurpose != AttachmentPurposes.Handover)
        {
            throw new AttachmentDomainException("ATTACHMENT_PURPOSE_INVALID", "Signatures are supported only for the handover purpose.");
        }

        var name = signerName?.Trim() ?? string.Empty;
        if (name.Length < SignatureEvidenceLimits.MinSignerNameLength || name.Length > SignatureEvidenceLimits.MaxSignerNameLength)
        {
            throw new AttachmentDomainException("SIGNATURE_SUBMISSION_INVALID", "A signer name of 2-200 characters is required.");
        }

        var role = string.IsNullOrWhiteSpace(signerRole) ? null : signerRole.Trim();
        if (role is { Length: > SignatureEvidenceLimits.MaxSignerRoleLength })
        {
            throw new AttachmentDomainException("SIGNATURE_SUBMISSION_INVALID", "The signer role cannot exceed 100 characters.");
        }

        var consent = consentTextVersion?.Trim() ?? string.Empty;
        if (consent.Length == 0 || consent.Length > SignatureEvidenceLimits.MaxConsentVersionLength)
        {
            throw new AttachmentDomainException("SIGNATURE_CONSENT_REQUIRED", "The consent wording version is required.");
        }

        if (!LowerSha256.IsMatch(contentHash ?? string.Empty))
        {
            throw new AttachmentDomainException("SIGNATURE_IMAGE_INVALID", "A lower-case SHA-256 content hash of the image is required.");
        }

        if (imageFileOrganizationId != organizationId)
        {
            throw new AttachmentDomainException("ATTACHMENT_FILE_SCOPE_MISMATCH", "The image does not belong to the owner's organization.");
        }

        OrganizationId = organizationId;
        OwnerType = normalizedOwner;
        OwnerId = ownerId;
        Purpose = normalizedPurpose;
        SignerName = name;
        SignerRole = role;
        SignedAtUtc = signedAtUtc.ToUniversalTime();
        ImageFileId = imageFileId;
        ConsentTextVersion = consent;
        ContentHash = contentHash!;
        CapturedByUserId = capturedByUserId;
    }
}
