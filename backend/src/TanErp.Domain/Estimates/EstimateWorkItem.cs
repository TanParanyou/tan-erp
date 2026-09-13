namespace TanErp.Domain.Estimates;

public class EstimateWorkItem
{
    private readonly List<EstimateCostComponent> _costComponents = new();

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateSectionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string DescriptionTh { get; private set; } = string.Empty;
    public string? DescriptionEn { get; private set; }
    public decimal Quantity { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public string SellingRuleType { get; private set; } = Estimates.SellingRuleType.Margin;
    public decimal SellingRuleValue { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal UnitSellingPrice { get; private set; }
    public decimal TotalSellingPrice { get; private set; }
    public int SortOrder { get; private set; }

    public IReadOnlyCollection<EstimateCostComponent> CostComponents => _costComponents.AsReadOnly();

    private EstimateWorkItem() { }

    public EstimateWorkItem(
        Guid id,
        Guid organizationId,
        Guid estimateSectionId,
        string code,
        string descriptionTh,
        string? descriptionEn,
        decimal quantity,
        string unitCode,
        string sellingRuleType = Estimates.SellingRuleType.Margin,
        decimal sellingRuleValue = 0.30m,
        int sortOrder = 1)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Work item ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (estimateSectionId == Guid.Empty)
            throw new ArgumentException("Section ID cannot be empty.", nameof(estimateSectionId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Work item code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(descriptionTh))
            throw new ArgumentException("Thai description cannot be empty.", nameof(descriptionTh));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("Unit code cannot be empty.", nameof(unitCode));
        if (!Estimates.SellingRuleType.IsValid(sellingRuleType))
            throw new ArgumentException($"Invalid selling rule type: '{sellingRuleType}'.", nameof(sellingRuleType));

        Id = id;
        OrganizationId = organizationId;
        EstimateSectionId = estimateSectionId;
        Code = code.Trim();
        DescriptionTh = descriptionTh.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        SellingRuleType = sellingRuleType.Trim();
        SellingRuleValue = decimal.Round(sellingRuleValue, 4, MidpointRounding.AwayFromZero);
        SortOrder = sortOrder;
    }

    public void AddCostComponent(EstimateCostComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);
        _costComponents.Add(component);
        Recalculate();
    }

    public void ClearCostComponents()
    {
        _costComponents.Clear();
        Recalculate();
    }

    public void Update(
        string code,
        string descriptionTh,
        string? descriptionEn,
        decimal quantity,
        string unitCode,
        string sellingRuleType,
        decimal sellingRuleValue,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Work item code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(descriptionTh))
            throw new ArgumentException("Thai description cannot be empty.", nameof(descriptionTh));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("Unit code cannot be empty.", nameof(unitCode));
        if (!Estimates.SellingRuleType.IsValid(sellingRuleType))
            throw new ArgumentException($"Invalid selling rule type: '{sellingRuleType}'.", nameof(sellingRuleType));

        Code = code.Trim();
        DescriptionTh = descriptionTh.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        SellingRuleType = sellingRuleType.Trim();
        SellingRuleValue = decimal.Round(sellingRuleValue, 4, MidpointRounding.AwayFromZero);
        SortOrder = sortOrder;
        Recalculate();
    }

    public void Recalculate()
    {
        TotalCost = decimal.Round(_costComponents.Sum(c => c.TotalCost), 2, MidpointRounding.AwayFromZero);
        UnitCost = Quantity > 0
            ? decimal.Round(TotalCost / Quantity, 4, MidpointRounding.AwayFromZero)
            : 0;

        if (SellingRuleType == Estimates.SellingRuleType.Margin)
        {
            decimal divisor = 1m - SellingRuleValue;
            if (divisor <= 0)
            {
                TotalSellingPrice = TotalCost;
            }
            else
            {
                TotalSellingPrice = decimal.Round(TotalCost / divisor, 2, MidpointRounding.AwayFromZero);
            }
        }
        else if (SellingRuleType == Estimates.SellingRuleType.Markup)
        {
            TotalSellingPrice = decimal.Round(TotalCost * (1m + SellingRuleValue), 2, MidpointRounding.AwayFromZero);
        }
        else
        {
            TotalSellingPrice = decimal.Round(Quantity * SellingRuleValue, 2, MidpointRounding.AwayFromZero);
        }

        UnitSellingPrice = Quantity > 0
            ? decimal.Round(TotalSellingPrice / Quantity, 4, MidpointRounding.AwayFromZero)
            : 0;
    }
}
