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
        CostSources
    };

    public static readonly IReadOnlyCollection<string> GeneratedMasterData = new[]
    {
        Items,
        ItemCategories,
        ItemBrands,
        UnitsOfMeasure,
        ItemTaxCategories,
        CostSources
    };

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Estimates,
        Surveys,
        Opportunities,
        Quotations,
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
