using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

public class WarehouseLocation : Entity
{
    public const string DefaultCode = "DEFAULT";
    public const string DefaultName = "Default Location";

    public Guid OrganizationId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public string Status { get; private set; } = WarehouseLocationStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected WarehouseLocation() { }

    public WarehouseLocation(
        Guid id,
        Guid organizationId,
        Guid warehouseId,
        string code,
        string name,
        bool isDefault,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || warehouseId == Guid.Empty)
        {
            throw new ArgumentException("Organization and warehouse are required.");
        }

        var trimmedCode = (code ?? string.Empty).Trim();
        if (trimmedCode.Length is 0 or > 32)
        {
            throw new InventoryDomainException("LOCATION_FIELD_INVALID", "Location code is required and cannot exceed 32 characters.");
        }

        var trimmedName = (name ?? string.Empty).Trim();
        if (trimmedName.Length is 0 or > 120)
        {
            throw new InventoryDomainException("LOCATION_FIELD_INVALID", "Location name is required and cannot exceed 120 characters.");
        }

        OrganizationId = organizationId;
        WarehouseId = warehouseId;
        Code = trimmedCode;
        NormalizedCode = trimmedCode.ToUpperInvariant();
        Name = trimmedName;
        IsDefault = isDefault;
        Status = WarehouseLocationStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
    }

    public static WarehouseLocation CreateDefault(Guid organizationId, Guid warehouseId, DateTimeOffset now) =>
        new(Guid.NewGuid(), organizationId, warehouseId, DefaultCode, DefaultName, true, now);

    public void Update(string name, bool? active, DateTimeOffset now)
    {
        var trimmedName = (name ?? string.Empty).Trim();
        if (trimmedName.Length is 0 or > 120)
        {
            throw new InventoryDomainException("LOCATION_FIELD_INVALID", "Location name is required and cannot exceed 120 characters.");
        }

        if (active.HasValue)
        {
            if (!active.Value && IsDefault)
            {
                throw new InventoryDomainException("LOCATION_IS_DEFAULT", "The default warehouse location cannot be deactivated.");
            }

            Status = active.Value ? WarehouseLocationStatus.Active : WarehouseLocationStatus.Inactive;
        }

        Name = trimmedName;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
