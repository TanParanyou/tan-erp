namespace TanErp.Domain.DocumentNumbering;

public enum ResetPeriod
{
    Never = 0,
    Yearly = 1,
    Monthly = 2,
    Daily = 3
}

public static class DocumentTypes
{
    public const string Estimates = "estimates";
    public const string Surveys = "surveys";
    public const string Opportunities = "opportunities";
    public const string Quotations = "quotations";
    public const string Projects = "projects";
    public const string ProjectChangeOrders = "project-change-orders";
    public const string Suppliers = "suppliers";
    public const string PurchaseOrders = "purchase-orders";
    public const string GoodsReceipts = "goods-receipts";
    public const string Warehouses = "warehouses";
    public const string StockReceipts = "stock-receipts";
    public const string StockIssues = "stock-issues";
    public const string StockTransfers = "stock-transfers";
    public const string StockAdjustments = "stock-adjustments";
    public const string StockReturns = "stock-returns";
    public const string Boms = "boms";
    public const string WorkOrders = "work-orders";
    public const string Customers = "customers";
    public const string Items = "items";
    public const string ItemCategories = "item-categories";
    public const string ItemBrands = "item-brands";
    public const string UnitsOfMeasure = "units-of-measure";
    public const string ItemTaxCategories = "item-tax-categories";
    public const string CostSources = "cost-sources";

    public static readonly IReadOnlyCollection<string> MasterData = new[]
    {
        Customers,
        Items,
        ItemCategories,
        ItemBrands,
        UnitsOfMeasure,
        ItemTaxCategories,
        CostSources,
        Suppliers,
        Warehouses,
        Boms
    };

    public static readonly IReadOnlyCollection<string> GeneratedMasterData = new[]
    {
        Items,
        ItemCategories,
        ItemBrands,
        UnitsOfMeasure,
        ItemTaxCategories,
        CostSources,
        Suppliers,
        Warehouses,
        Boms
    };

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Estimates,
        Surveys,
        Opportunities,
        Quotations,
        Projects,
        ProjectChangeOrders,
        Suppliers,
        PurchaseOrders,
        GoodsReceipts,
        Warehouses,
        StockReceipts,
        StockIssues,
        StockTransfers,
        StockAdjustments,
        StockReturns,
        Boms,
        WorkOrders,
        Customers,
        Items,
        ItemCategories,
        ItemBrands,
        UnitsOfMeasure,
        ItemTaxCategories,
        CostSources
    };

    public static bool IsMasterData(string documentType) =>
        MasterData.Contains(documentType, StringComparer.OrdinalIgnoreCase);

    public static bool IsValid(string documentType) =>
        All.Contains(documentType, StringComparer.OrdinalIgnoreCase);
}

public sealed record DocumentSequenceDefaults(
    string Prefix,
    string FormatPattern,
    ResetPeriod ResetPeriod,
    int Padding,
    bool IsBranchSpecific)
{
    public static DocumentSequenceDefaults For(string documentType)
    {
        var type = documentType.Trim().ToLowerInvariant();
        if (type == DocumentTypes.Customers)
        {
            return new("CUS-", "{PREFIX}{SEQ:5}", ResetPeriod.Never, 5, false);
        }

        var masterPrefix = type switch
        {
            DocumentTypes.Items => "ITM-",
            DocumentTypes.ItemCategories => "CAT-",
            DocumentTypes.ItemBrands => "BRD-",
            DocumentTypes.UnitsOfMeasure => "UOM-",
            DocumentTypes.ItemTaxCategories => "TAX-",
            DocumentTypes.CostSources => "SRC-",
            DocumentTypes.Suppliers => "SUP-",
            DocumentTypes.Warehouses => "WH-",
            DocumentTypes.Boms => "BOM-",
            _ => null
        };

        if (masterPrefix is not null)
        {
            return new(masterPrefix, "{PREFIX}{SEQ:5}", ResetPeriod.Never, 5, false);
        }

        var transactionalPrefix = type switch
        {
            DocumentTypes.Estimates => "EST",
            DocumentTypes.Surveys => "SRV",
            DocumentTypes.Opportunities => "OPP",
            DocumentTypes.Quotations => "QT",
            DocumentTypes.Projects => "PRJ",
            DocumentTypes.ProjectChangeOrders => "PCO",
            DocumentTypes.PurchaseOrders => "PO",
            DocumentTypes.GoodsReceipts => "GR",
            DocumentTypes.StockReceipts => "SR",
            DocumentTypes.StockIssues => "SI",
            DocumentTypes.StockTransfers => "ST",
            DocumentTypes.StockAdjustments => "SA",
            DocumentTypes.StockReturns => "SRT",
            DocumentTypes.WorkOrders => "WO",
            _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "Unknown document type.")
        };

        return new(transactionalPrefix, "{PREFIX}-{YYYY}-{SEQ:4}", ResetPeriod.Yearly, 4, false);
    }
}

public static class DocumentNumberTokens
{
    public const string Prefix = "{PREFIX}";
    public const string Branch = "{BRANCH}";
    public const string YearAD = "{YYYY}";
    public const string ShortYearAD = "{YY}";
    public const string YearBE = "{BBBB}";
    public const string ShortYearBE = "{BB}";
    public const string Month = "{MM}";
    public const string Day = "{DD}";
    public const string Sequence = "{SEQ:";
}
