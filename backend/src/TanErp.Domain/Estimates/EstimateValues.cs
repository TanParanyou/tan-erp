namespace TanErp.Domain.Estimates;

public static class EstimateStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Returned = "returned";
    public const string Approved = "approved";
    public const string Quoted = "quoted";
    public const string Cancelled = "cancelled";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Submitted,
        Returned,
        Approved,
        Quoted,
        Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class CostComponentType
{
    public const string Material = "material";
    public const string Labor = "labor";
    public const string Subcontract = "subcontract";
    public const string Service = "service";
    public const string OtherDirect = "other_direct";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Material,
        Labor,
        Subcontract,
        Service,
        OtherDirect
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SellingRuleType
{
    public const string Margin = "margin";
    public const string Markup = "markup";
    public const string FixedPrice = "fixed_price";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Margin,
        Markup,
        FixedPrice
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class EstimateDefaults
{
    public const decimal DefaultTaxRate = 0.07m;
    public const string DefaultCurrency = "THB";
    public const string DefaultCalculationPolicyVersion = "EST-CALC-TH-v1";
    public const string DefaultTaxPolicyVersion = "TAX-TH-v1";
}

public static class EstimateRevisionStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Returned = "returned";
    public const string Approved = "approved";
    public const string Quoted = "quoted";
    public const string Cancelled = "cancelled";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Submitted,
        Returned,
        Approved,
        Quoted,
        Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}
