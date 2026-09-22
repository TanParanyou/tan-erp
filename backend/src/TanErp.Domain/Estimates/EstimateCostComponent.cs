using TanErp.Domain.Items;

namespace TanErp.Domain.Estimates;

public class EstimateCostComponent
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateWorkItemId { get; private set; }
    public string Type { get; private set; } = CostComponentType.Material;
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public decimal UnitCost { get; private set; }
    public string Currency { get; private set; } = EstimateDefaults.DefaultCurrency;
    public decimal TotalCost { get; private set; }
    public int SortOrder { get; private set; }

    // Catalog item snapshot
    public Guid? ItemId { get; private set; }
    public Guid? CostRecordId { get; private set; }
    public int? CostRecordVersion { get; private set; }
    public string? ItemCodeSnapshot { get; private set; }
    public LocalizedText? ItemNameSnapshot { get; private set; }
    public string? UnitSnapshot { get; private set; }
    public decimal? UnitCostSnapshot { get; private set; }
    public string? CurrencySnapshot { get; private set; }
    public string? CostScopeSnapshot { get; private set; }
    public DateTimeOffset? CostEffectiveFromUtc { get; private set; }
    public string? CostPolicyVersion { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }

    private EstimateCostComponent() { }

    public EstimateCostComponent(
        Guid id,
        Guid organizationId,
        Guid estimateWorkItemId,
        string type,
        string description,
        decimal quantity,
        string unitCode,
        decimal unitCost,
        string currency = EstimateDefaults.DefaultCurrency,
        int sortOrder = 1)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Cost component ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (estimateWorkItemId == Guid.Empty)
            throw new ArgumentException("Work item ID cannot be empty.", nameof(estimateWorkItemId));
        if (!CostComponentType.IsValid(type))
            throw new ArgumentException($"Invalid cost component type: '{type}'.", nameof(type));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("Unit code cannot be empty.", nameof(unitCode));
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));

        Id = id;
        OrganizationId = organizationId;
        EstimateWorkItemId = estimateWorkItemId;
        Type = type.Trim();
        Description = description.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        UnitCost = decimal.Round(unitCost, 4, MidpointRounding.AwayFromZero);
        Currency = string.IsNullOrWhiteSpace(currency) ? EstimateDefaults.DefaultCurrency : currency.Trim();
        TotalCost = decimal.Round(Quantity * UnitCost, 2, MidpointRounding.AwayFromZero);
        SortOrder = sortOrder;
    }

    public void Update(
        string type,
        string description,
        decimal quantity,
        string unitCode,
        decimal unitCost,
        int sortOrder)
    {
        if (!CostComponentType.IsValid(type))
            throw new ArgumentException($"Invalid cost component type: '{type}'.", nameof(type));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("Unit code cannot be empty.", nameof(unitCode));
        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));

        Type = type.Trim();
        Description = description.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        UnitCost = decimal.Round(unitCost, 4, MidpointRounding.AwayFromZero);
        TotalCost = decimal.Round(Quantity * UnitCost, 2, MidpointRounding.AwayFromZero);
        SortOrder = sortOrder;
    }

    public void SetCatalogCostSnapshot(
        Guid itemId,
        Guid costRecordId,
        int costRecordVersion,
        string itemCodeSnapshot,
        LocalizedText itemNameSnapshot,
        string unitSnapshot,
        decimal unitCostSnapshot,
        string currencySnapshot,
        string costScopeSnapshot,
        DateTimeOffset? costEffectiveFromUtc,
        string? costPolicyVersion,
        DateTimeOffset resolvedAtUtc)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));
        if (costRecordId == Guid.Empty)
            throw new ArgumentException("Cost record ID cannot be empty.", nameof(costRecordId));
        if (costRecordVersion <= 0)
            throw new ArgumentException("Cost record version must be positive.", nameof(costRecordVersion));
        if (string.IsNullOrWhiteSpace(itemCodeSnapshot))
            throw new ArgumentException("Item code snapshot cannot be empty.", nameof(itemCodeSnapshot));
        if (itemNameSnapshot is null)
            throw new ArgumentNullException(nameof(itemNameSnapshot), "Item name snapshot cannot be null.");
        if (string.IsNullOrWhiteSpace(unitSnapshot))
            throw new ArgumentException("Unit snapshot cannot be empty.", nameof(unitSnapshot));
        if (unitCostSnapshot < 0)
            throw new ArgumentException("Unit cost snapshot cannot be negative.", nameof(unitCostSnapshot));

        ItemId = itemId;
        CostRecordId = costRecordId;
        CostRecordVersion = costRecordVersion;
        ItemCodeSnapshot = itemCodeSnapshot.Trim();
        ItemNameSnapshot = itemNameSnapshot;
        UnitSnapshot = unitSnapshot.Trim();
        UnitCostSnapshot = decimal.Round(unitCostSnapshot, 4, MidpointRounding.AwayFromZero);
        CurrencySnapshot = string.IsNullOrWhiteSpace(currencySnapshot) ? EstimateDefaults.DefaultCurrency : currencySnapshot.Trim();
        CostScopeSnapshot = string.IsNullOrWhiteSpace(costScopeSnapshot) ? null : costScopeSnapshot.Trim();
        CostEffectiveFromUtc = costEffectiveFromUtc;
        CostPolicyVersion = string.IsNullOrWhiteSpace(costPolicyVersion) ? null : costPolicyVersion.Trim();
        ResolvedAtUtc = resolvedAtUtc;

        // Authoritatively update UnitCost & TotalCost from snapshot
        UnitCost = UnitCostSnapshot.Value;
        TotalCost = decimal.Round(Quantity * UnitCost, 2, MidpointRounding.AwayFromZero);
    }

    public void ClearCatalogCostSnapshot()
    {
        ItemId = null;
        CostRecordId = null;
        CostRecordVersion = null;
        ItemCodeSnapshot = null;
        ItemNameSnapshot = null;
        UnitSnapshot = null;
        UnitCostSnapshot = null;
        CurrencySnapshot = null;
        CostScopeSnapshot = null;
        CostEffectiveFromUtc = null;
        CostPolicyVersion = null;
        ResolvedAtUtc = null;
    }
}
