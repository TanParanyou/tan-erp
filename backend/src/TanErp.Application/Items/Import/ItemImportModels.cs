namespace TanErp.Application.Items.Import;

public static class ItemImportLimits
{
    /// <summary>Synchronous preview/commit limits (Phase 1).</summary>
    public const int MaxRows = 500;
    public const int MaxContentLength = 1_000_000;

    /// <summary>Background job limits (Phase 2). Larger files must use the job endpoint.</summary>
    public const int MaxJobRows = 20_000;
    public const int MaxJobContentLength = 10_000_000;

    /// <summary>Rows committed per transaction when no configured value applies; see ItemImportJobOptions.</summary>
    public const int DefaultBatchSize = 100;
    public const int MinBatchSize = 10;
    public const int MaxBatchSize = 1000;

    /// <summary>Row errors stored on a job; the rest are counted but not kept.</summary>
    public const int MaxReportedRowErrors = 500;
}

/// <summary>How an Item file is applied. Create (Phase 1) only adds new Items; Upsert also updates existing ones by Item Code.</summary>
public static class ItemImportMode
{
    public const string Create = "create";
    public const string Upsert = "upsert";

    public static bool IsValid(string? value) => value is Create or Upsert;
}

/// <summary>What a file contains: Items (master data) or Cost Records (always imported as Draft).</summary>
public static class ItemImportKind
{
    public const string Items = "items";
    public const string Costs = "costs";

    public static bool IsValid(string? value) => value is Items or Costs;
}

/// <summary>What happened (or would happen) to one valid row.</summary>
public static class ItemImportOutcomes
{
    public const string Created = "created";
    public const string Updated = "updated";
    public const string Unchanged = "unchanged";
}

/// <summary>
/// The only Item fields an upsert may overwrite on an existing Item. Item Code is the key. itemType and baseUnitCode are
/// identity-like and must equal the stored value (otherwise the row fails). Lifecycle state (status, activation,
/// availability, attributes, images, barcodes) is never touched by import.
/// </summary>
public static class ItemImportUpsertPolicy
{
    public static readonly IReadOnlyList<string> OverwritableFields = new[]
    {
        "nameTh", "nameEn", "descriptionTh", "descriptionEn", "categoryCode", "brandCode", "taxCategoryCode",
        "canSell", "canCost", "canPurchase", "canStock", "canProduce"
    };

    public static readonly IReadOnlyList<string> MustMatchFields = new[] { "itemType", "baseUnitCode" };
}

/// <summary>Raw (untrusted) CSV cells for one data row. Nothing is interpreted until validation.</summary>
public sealed record ItemImportRowDraft(
    int RowNumber,
    string? Code,
    string? ItemType,
    string? CategoryCode,
    string? BrandCode,
    string? BaseUnitCode,
    string? NameTh,
    string? NameEn,
    string? DescriptionTh,
    string? DescriptionEn,
    string? TaxCategoryCode,
    string? CanSell,
    string? CanCost,
    string? CanPurchase,
    string? CanStock,
    string? CanProduce);

/// <summary>A row whose syntax passed validation; lookups (category/brand/unit/tax) are still unresolved codes.</summary>
public sealed record ItemImportRow(
    int RowNumber,
    string? Code,
    string ItemType,
    string CategoryCode,
    string? BrandCode,
    string BaseUnitCode,
    string NameTh,
    string? NameEn,
    string? DescriptionTh,
    string? DescriptionEn,
    string? TaxCategoryCode,
    bool CanSell,
    bool CanCost,
    bool CanPurchase,
    bool CanStock,
    bool CanProduce);

public static class ItemImportErrorCodes
{
    public const string Required = "REQUIRED";
    public const string Invalid = "INVALID";
    public const string TooLong = "TOO_LONG";
    public const string NotFound = "NOT_FOUND";
    public const string Inactive = "INACTIVE";
    public const string DuplicateInFile = "DUPLICATE_IN_FILE";
    public const string AlreadyExists = "ALREADY_EXISTS";
    public const string Mismatch = "MISMATCH";
    public const string Locked = "LOCKED";
    public const string BranchAccessDenied = "BRANCH_ACCESS_DENIED";
    public const string DuplicateCost = "DUPLICATE_COST";
}

public sealed record ItemImportRowError(string Field, string Code);

public sealed record ItemImportRowResult(
    int RowNumber,
    string? Code,
    string? NameTh,
    IReadOnlyList<ItemImportRowError> Errors,
    string? Outcome = null)
{
    public bool IsValid => Errors.Count == 0;
}

public sealed record ItemImportPreview(
    string ContentSha256,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ItemImportRowResult> Rows,
    int CreateCount = 0,
    int UpdateCount = 0,
    int UnchangedCount = 0);

public sealed record ItemImportCommitResult(
    Guid BatchId,
    int CreatedCount,
    string ContentSha256,
    bool Replayed,
    int UpdatedCount = 0,
    int UnchangedCount = 0);

/// <summary>Result of one atomic batch: either every row was applied, or nothing was and the row errors are listed.</summary>
public sealed record ItemImportBatchOutcome(
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    IReadOnlyList<ItemImportRowResult> InvalidRows)
{
    public bool IsApplied => InvalidRows.Count == 0;
}
