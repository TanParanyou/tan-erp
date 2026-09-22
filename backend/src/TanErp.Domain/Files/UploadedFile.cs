using TanErp.Domain.Common;

namespace TanErp.Domain.Files;

/// <summary>
/// Represents a single file that has been uploaded and verified in the file store.
/// Stores metadata only; the binary is managed by IFileStorageProvider.
/// </summary>
public class UploadedFile : Entity
{
    public Guid OrganizationId { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string OriginalFilename { get; private set; } = string.Empty;
    public string MediaType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public string UploadSessionId { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public Guid UploadedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public string? ContentSha256 { get; private set; }
    public string ScanStatus { get; private set; } = "content_verified";
    public DateTimeOffset? VerifiedAtUtc { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }

    protected UploadedFile() { }

    public UploadedFile(
        Guid id,
        Guid organizationId,
        string storagePath,
        string originalFilename,
        string mediaType,
        long fileSizeBytes,
        string uploadSessionId,
        Guid uploadedByUserId,
        DateTimeOffset createdAtUtc,
        string? contentSha256 = null,
        string scanStatus = "content_verified",
        DateTimeOffset? verifiedAtUtc = null,
        int? width = null,
        int? height = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException("Storage path cannot be blank.", nameof(storagePath));

        if (string.IsNullOrWhiteSpace(originalFilename))
            throw new ArgumentException("Original filename cannot be blank.", nameof(originalFilename));

        if (string.IsNullOrWhiteSpace(mediaType))
            throw new ArgumentException("Media type cannot be blank.", nameof(mediaType));

        if (fileSizeBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(fileSizeBytes), "File size must be positive.");

        if (string.IsNullOrWhiteSpace(uploadSessionId))
            throw new ArgumentException("Upload session ID cannot be blank.", nameof(uploadSessionId));

        OrganizationId = organizationId;
        StoragePath = storagePath.Trim();
        OriginalFilename = originalFilename.Trim();
        MediaType = mediaType.Trim().ToLowerInvariant();
        FileSizeBytes = fileSizeBytes;
        UploadSessionId = uploadSessionId.Trim();
        Status = UploadedFileStatus.Verified;
        UploadedByUserId = uploadedByUserId;
        CreatedAtUtc = createdAtUtc;
        ContentSha256 = string.IsNullOrWhiteSpace(contentSha256) ? null : contentSha256.Trim();
        ScanStatus = string.IsNullOrWhiteSpace(scanStatus) ? "content_verified" : scanStatus.Trim();
        VerifiedAtUtc = verifiedAtUtc ?? createdAtUtc;
        Width = width;
        Height = height;
    }
}
