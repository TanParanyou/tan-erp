using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

public class Warehouse : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string Status { get; private set; } = WarehouseStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected Warehouse() { }

    public Warehouse(Guid id, Guid organizationId, Guid branchId, string code, string name, string? address, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Warehouse code cannot be blank.", nameof(code));

        OrganizationId = organizationId;
        BranchId = branchId;
        Code = code.Trim();
        NormalizedCode = Code.ToUpperInvariant();
        Apply(name, address);
        Status = WarehouseStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void Update(string name, string? address, DateTimeOffset now)
    {
        Apply(name, address);
        Touch(now);
    }

    public void SetActive(bool active, DateTimeOffset now)
    {
        var target = active ? WarehouseStatus.Active : WarehouseStatus.Inactive;
        if (Status == target)
        {
            throw new InventoryDomainException("WAREHOUSE_INVALID_STATE", $"The warehouse is already {Status}.");
        }

        Status = target;
        Touch(now);
    }

    private void Apply(string name, string? address)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length == 0 || trimmed.Length > 200)
        {
            throw new InventoryDomainException("WAREHOUSE_FIELD_INVALID", "Warehouse name is required and cannot exceed 200 characters.");
        }

        var trimmedAddress = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        if (trimmedAddress is { Length: > 500 })
        {
            throw new InventoryDomainException("WAREHOUSE_FIELD_INVALID", "Address cannot exceed 500 characters.");
        }

        Name = trimmed;
        NormalizedName = trimmed.ToLowerInvariant();
        Address = trimmedAddress;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
