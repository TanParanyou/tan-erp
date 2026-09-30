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
    public string? SellingRuleReasonCode { get; private set; }
    public Guid? ItemId { get; private set; }
    public string? ItemCodeSnapshot { get; private set; }
    public string? ItemNameThSnapshot { get; private set; }
    public string? ItemNameEnSnapshot { get; private set; }
    public string? OverrideReasonCode { get; private set; }
    public string? OverrideReason { get; private set; }
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
        int sortOrder = 1,
        string? sellingRuleReasonCode = null)
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
        var normalizedSellingRuleValue = decimal.Round(sellingRuleValue, 4, MidpointRounding.AwayFromZero);
        ValidateSellingRuleValue(sellingRuleType, normalizedSellingRuleValue);

        Id = id;
        OrganizationId = organizationId;
        EstimateSectionId = estimateSectionId;
        Code = code.Trim();
        DescriptionTh = descriptionTh.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        SellingRuleType = sellingRuleType.Trim();
        SellingRuleValue = normalizedSellingRuleValue;
        SellingRuleReasonCode = NormalizeSellingRuleReason(sellingRuleType, sellingRuleReasonCode);
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

    public void SetItemMasterLink(Guid? itemId, string? itemCode, string? itemNameTh, string? itemNameEn)
    {
        if (!itemId.HasValue)
        {
            ItemId = null;
            ItemCodeSnapshot = null;
            ItemNameThSnapshot = null;
            ItemNameEnSnapshot = null;
            return;
        }

        if (itemId == Guid.Empty || string.IsNullOrWhiteSpace(itemCode) || string.IsNullOrWhiteSpace(itemNameTh))
            throw new ArgumentException("A linked Item Master reference requires its ID, code, and Thai name.");

        ItemId = itemId;
        ItemCodeSnapshot = itemCode.Trim();
        ItemNameThSnapshot = itemNameTh.Trim();
        ItemNameEnSnapshot = string.IsNullOrWhiteSpace(itemNameEn) ? null : itemNameEn.Trim();
    }

    public void SetCustomWorkItemReason(string? reasonCode, string? reason)
    {
        OverrideReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim();
        OverrideReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (OverrideReasonCode?.Length > 64)
            throw new ArgumentException("Custom work item reason code cannot exceed 64 characters.", nameof(reasonCode));
        if (OverrideReason?.Length > 500)
            throw new ArgumentException("Custom work item reason cannot exceed 500 characters.", nameof(reason));
    }

    public void Update(
        string code,
        string descriptionTh,
        string? descriptionEn,
        decimal quantity,
        string unitCode,
        string sellingRuleType,
        decimal sellingRuleValue,
        int sortOrder,
        string? sellingRuleReasonCode = null)
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
        var normalizedReason = NormalizeSellingRuleReason(sellingRuleType, sellingRuleReasonCode);
        if (sellingRuleType == Estimates.SellingRuleType.FixedPrice && normalizedReason is null)
            throw new EstimateFixedPriceReasonRequiredException();
        var normalizedSellingRuleValue = decimal.Round(sellingRuleValue, 4, MidpointRounding.AwayFromZero);
        ValidateSellingRuleValue(sellingRuleType, normalizedSellingRuleValue);

        Code = code.Trim();
        DescriptionTh = descriptionTh.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitCode = unitCode.Trim();
        SellingRuleType = sellingRuleType.Trim();
        SellingRuleValue = normalizedSellingRuleValue;
        SellingRuleReasonCode = normalizedReason;
        SortOrder = sortOrder;
        Recalculate();
    }

    public void Recalculate()
    {
        RecalculateWithCostBase(_costComponents.Sum(c => c.TotalCost));
    }

    public void RecalculateWithCostBase(decimal totalCost)
    {
        if (totalCost < 0m)
            throw new ArgumentOutOfRangeException(nameof(totalCost));

        TotalCost = decimal.Round(totalCost, 2, MidpointRounding.AwayFromZero);
        UnitCost = Quantity > 0
            ? decimal.Round(TotalCost / Quantity, 4, MidpointRounding.AwayFromZero)
            : 0;

        if (SellingRuleType == Estimates.SellingRuleType.Margin)
        {
            TotalSellingPrice = decimal.Round(TotalCost / (1m - SellingRuleValue), 2, MidpointRounding.AwayFromZero);
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

    private static void ValidateSellingRuleValue(string ruleType, decimal value)
    {
        if (ruleType == Estimates.SellingRuleType.Margin && (value < 0m || value >= 1m))
            throw new ArgumentOutOfRangeException(nameof(value), "Margin rate must be greater than or equal to zero and less than one.");

        if (ruleType == Estimates.SellingRuleType.Markup && value < 0m)
            throw new ArgumentOutOfRangeException(nameof(value), "Markup rate cannot be negative.");

        if (ruleType == Estimates.SellingRuleType.FixedPrice && value < 0m)
            throw new ArgumentOutOfRangeException(nameof(value), "Fixed price cannot be negative.");
    }

    private static string? NormalizeSellingRuleReason(string ruleType, string? reasonCode)
    {
        var normalizedReason = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim();
        if (normalizedReason is { Length: > 64 })
            throw new ArgumentOutOfRangeException(nameof(reasonCode), "Selling rule reason code cannot exceed 64 characters.");

        return ruleType == Estimates.SellingRuleType.FixedPrice ? normalizedReason : null;
    }
}
