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
}
