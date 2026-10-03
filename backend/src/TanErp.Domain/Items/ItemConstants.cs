namespace TanErp.Domain.Items;

public static class ItemType
{
    public const string Material = "material";
    public const string Labor = "labor";
    public const string Service = "service";
    public const string Subcontract = "subcontract";
    public const string Other = "other";
    public const string Product = "product";

    public static readonly string[] All = [Material, Labor, Service, Subcontract, Other, Product];

    public static bool IsValid(string type) => All.Contains(type, StringComparer.OrdinalIgnoreCase);
}

public static class ItemStatus
{
    public const string Draft = "draft";
    public const string Active = "active";
    public const string Inactive = "inactive";

    public static readonly string[] All = [Draft, Active, Inactive];
}

public static class ItemSortKey
{
    public const string Code = "code";
    public const string ItemType = "itemType";
    public const string Status = "status";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Code,
        ItemType,
        Status
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class ItemSortOrder
{
    public const string Asc = "asc";
    public const string Desc = "desc";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Asc,
        Desc
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class ItemAvailabilityMode
{
    public const string AllBranches = "all_branches";
    public const string SelectedBranches = "selected_branches";

    public static readonly string[] All = [AllBranches, SelectedBranches];

    public static bool IsValid(string mode) => All.Contains(mode, StringComparer.OrdinalIgnoreCase);
}

public static class ItemImageRole
{
    public const string Primary = "primary";
    public const string Gallery = "gallery";
    public const string Technical = "technical";

    public static readonly string[] All = [Primary, Gallery, Technical];
}

public static class CostRecordStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Returned = "returned";
    public const string Approved = "approved";
    public const string Published = "published";
    public const string Superseded = "superseded";
    public const string Disabled = "disabled";

    public static readonly string[] All = [Draft, Submitted, Returned, Approved, Published, Superseded, Disabled];
}

public static class CostScopeType
{
    public const string Organization = "organization";
    public const string Branch = "branch";

    public static readonly string[] All = [Organization, Branch];
}
