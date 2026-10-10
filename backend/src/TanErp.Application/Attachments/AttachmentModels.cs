using TanErp.Domain.Attachments;

namespace TanErp.Application.Attachments;

public sealed record AttachmentCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public sealed record AttachmentPerson(Guid Id, string DisplayName);

/// <summary>Where an owner record lives and what state it is in; null from the reader means "not visible to this organization".</summary>
public sealed record AttachmentOwnerScope(Guid OrganizationId, Guid? BranchId, string State);

public sealed record AttachFilesInput(string? Purpose, IReadOnlyList<Guid>? FileIds);

public sealed record SignatureCaptureInput(string? Purpose, string? SignerName, string? SignerRole, Guid ImageFileId, bool ConsentAccepted, string? ConsentTextVersion);

/// <summary>Normalized signature input that already passed handler validation.</summary>
public sealed record SignatureCaptureCommand(string Purpose, string SignerName, string? SignerRole, Guid ImageFileId, string ConsentTextVersion);

public sealed record AttachmentLinkProjection(
    Guid Id, string OwnerType, Guid OwnerId, Guid FileId, string Purpose, string Filename, string MediaType, long FileSizeBytes,
    string ServingUrl, AttachmentPerson CreatedBy, DateTimeOffset CreatedAtUtc);

public sealed record SignatureCaptureProjection(
    Guid Id, string OwnerType, Guid OwnerId, string Purpose, string SignerName, string? SignerRole, DateTimeOffset SignedAtUtc,
    Guid ImageFileId, string ServingUrl, string ConsentTextVersion, string ContentHash, AttachmentPerson CapturedBy);

/// <summary>Current consent wording version per signature purpose. The wording itself is localized in the UI.</summary>
public static class SignatureConsentVersions
{
    public const string Handover = "handover-2026-10-v1";

    public static string? Current(string purpose) => purpose == AttachmentPurposes.Handover ? Handover : null;
}
