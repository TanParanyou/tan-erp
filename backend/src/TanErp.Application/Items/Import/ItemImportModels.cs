namespace TanErp.Application.Items.Import;

public static class ItemImportLimits
{
    public const int MaxRows = 500;
    public const int MaxContentLength = 1_000_000;
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
}

public sealed record ItemImportRowError(string Field, string Code);

public sealed record ItemImportRowResult(
    int RowNumber,
    string? Code,
    string? NameTh,
    IReadOnlyList<ItemImportRowError> Errors)
{
    public bool IsValid => Errors.Count == 0;
}

public sealed record ItemImportPreview(
    string ContentSha256,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ItemImportRowResult> Rows);

public sealed record ItemImportCommitResult(
    Guid BatchId,
    int CreatedCount,
    string ContentSha256,
    bool Replayed);
