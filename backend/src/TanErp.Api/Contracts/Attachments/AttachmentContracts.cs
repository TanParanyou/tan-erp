namespace TanErp.Api.Contracts.Attachments;

public sealed record AttachmentPersonResponse(Guid Id, string DisplayName);

/// <summary>Body of POST /api/v1/attachment-owners/{ownerType}/{ownerId}/attachments.</summary>
public sealed record AttachFilesRequest(string? Purpose, IReadOnlyList<Guid>? FileIds);

public sealed record AttachmentLinkResponse(
    Guid Id,
    string OwnerType,
    Guid OwnerId,
    Guid FileId,
    string Purpose,
    string Filename,
    string MediaType,
    long FileSizeBytes,
    string ServingUrl,
    AttachmentPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc);

public sealed record AttachmentListResponse(IReadOnlyList<AttachmentLinkResponse> Items);

/// <summary>Body of POST /api/v1/attachment-owners/{ownerType}/{ownerId}/signatures. The image is a PNG already uploaded through the Files module.</summary>
public sealed record CaptureSignatureRequest(
    string? Purpose,
    string? SignerName,
    string? SignerRole,
    Guid ImageFileId,
    bool ConsentAccepted,
    string? ConsentTextVersion);

public sealed record SignatureCaptureResponse(
    Guid Id,
    string OwnerType,
    Guid OwnerId,
    string Purpose,
    string SignerName,
    string? SignerRole,
    DateTimeOffset SignedAtUtc,
    Guid ImageFileId,
    string ServingUrl,
    string ConsentTextVersion,
    string ContentHash,
    AttachmentPersonResponse CapturedBy);

public sealed record SignatureCaptureListResponse(IReadOnlyList<SignatureCaptureResponse> Items);
