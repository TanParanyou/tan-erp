namespace TanErp.Domain.Common;

public class IdempotencyRecord : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Operation { get; private set; } = string.Empty;
    public string KeyHash { get; private set; } = string.Empty;
    public string PayloadHash { get; private set; } = string.Empty;
    public string ResourceId { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected IdempotencyRecord() { }

    public IdempotencyRecord(
        Guid id,
        Guid organizationId,
        string operation,
        string keyHash,
        string payloadHash,
        string resourceId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Operation cannot be blank.", nameof(operation));
        if (string.IsNullOrWhiteSpace(keyHash))
            throw new ArgumentException("KeyHash cannot be blank.", nameof(keyHash));
        if (string.IsNullOrWhiteSpace(payloadHash))
            throw new ArgumentException("PayloadHash cannot be blank.", nameof(payloadHash));
        if (string.IsNullOrWhiteSpace(resourceId))
            throw new ArgumentException("ResourceId cannot be blank.", nameof(resourceId));

        OrganizationId = organizationId;
        Operation = operation.Trim();
        KeyHash = keyHash.Trim();
        PayloadHash = payloadHash.Trim();
        ResourceId = resourceId.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public override string ToString() => $"IdempotencyRecord [Id={Id}, OrgId={OrganizationId}, Op={Operation}]";
}
