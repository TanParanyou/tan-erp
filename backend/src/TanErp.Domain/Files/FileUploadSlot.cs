using TanErp.Domain.Common;

namespace TanErp.Domain.Files;

public class FileUploadSlot : Entity
{
    public Guid SessionId { get; private set; }
    public int SlotIndex { get; private set; }
    public string Filename { get; private set; } = string.Empty;
    public string MediaType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public Guid? UploadedFileId { get; private set; }

    protected FileUploadSlot() { }

    public FileUploadSlot(
        Guid id,
        Guid sessionId,
        int slotIndex,
        string filename,
        string mediaType,
        long fileSizeBytes) : base(id)
    {
        if (slotIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Slot index cannot be negative.");

        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentException("Filename cannot be blank.", nameof(filename));

        if (string.IsNullOrWhiteSpace(mediaType))
            throw new ArgumentException("Media type cannot be blank.", nameof(mediaType));

        if (fileSizeBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(fileSizeBytes), "File size must be positive.");

        SessionId = sessionId;
        SlotIndex = slotIndex;
        Filename = filename.Trim();
        MediaType = mediaType.Trim().ToLowerInvariant();
        FileSizeBytes = fileSizeBytes;
    }

    public void LinkUploadedFile(Guid uploadedFileId)
    {
        UploadedFileId = uploadedFileId;
    }
}
