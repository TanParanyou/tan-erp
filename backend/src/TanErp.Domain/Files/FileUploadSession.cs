using TanErp.Domain.Common;

namespace TanErp.Domain.Files;

public static class FileParentTypes
{
    public const string Opportunity = "opportunity";
    public const string Customer = "customer";
    public const string Site = "site";

    public static bool IsValid(string? parentType) =>
        parentType is Opportunity or Customer or Site;
}

public static class FileUploadSessionStatus
{
    public const string Pending = "pending";
    public const string Consumed = "consumed";
    public const string Expired = "expired";
}

public class FileUploadSession : Entity
{
    public Guid OrganizationId { get; private set; }
    public string ParentType { get; private set; } = string.Empty;
    public Guid? ParentId { get; private set; }
    public Guid? CreationIntentId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public string Status { get; private set; } = FileUploadSessionStatus.Pending;
    public string IdempotencyKeyHash { get; private set; } = string.Empty;
    public string RequestPayloadHash { get; private set; } = string.Empty;

    private readonly List<FileUploadSlot> _slots = new();
    public IReadOnlyCollection<FileUploadSlot> Slots => _slots.AsReadOnly();

    protected FileUploadSession() { }

    public FileUploadSession(
        Guid id,
        Guid organizationId,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc,
        string idempotencyKeyHash,
        string requestPayloadHash) : base(id)
    {
        if (!FileParentTypes.IsValid(parentType))
            throw new ArgumentException($"Invalid parent type: {parentType}", nameof(parentType));

        if ((parentId.HasValue && creationIntentId.HasValue) || (!parentId.HasValue && !creationIntentId.HasValue))
            throw new ArgumentException("Exactly one of parentId or creationIntentId must be provided.");

        OrganizationId = organizationId;
        ParentType = parentType.ToLowerInvariant();
        ParentId = parentId;
        CreationIntentId = creationIntentId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        IdempotencyKeyHash = idempotencyKeyHash;
        RequestPayloadHash = requestPayloadHash;
        Status = FileUploadSessionStatus.Pending;
    }

    public void AddSlot(FileUploadSlot slot)
    {
        _slots.Add(slot);
    }

    public bool IsExpired(DateTimeOffset nowUtc) =>
        nowUtc >= ExpiresAtUtc || Status == FileUploadSessionStatus.Expired;

    public void MarkConsumed()
    {
        Status = FileUploadSessionStatus.Consumed;
    }

    public void MarkExpired()
    {
        Status = FileUploadSessionStatus.Expired;
    }

    public void BindActualParentId(Guid actualParentId)
    {
        ParentId = actualParentId;
    }
}
