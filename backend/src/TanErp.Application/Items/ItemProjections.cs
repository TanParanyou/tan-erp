namespace TanErp.Application.Items;

public sealed record LocalizedTextDto(string Thai, string? English);

public sealed record CategorySummaryDto(Guid Id, string Code, LocalizedTextDto Name, Guid? ParentCategoryId);

public sealed record BrandSummaryDto(Guid Id, string Code, LocalizedTextDto Name);

public sealed record UnitSummaryDto(Guid Id, string Code, string Symbol, LocalizedTextDto Name);

public sealed record ItemCapabilitiesDto(bool CanSell, bool CanCost, bool CanPurchase, bool CanStock, bool CanProduce);

public sealed record ItemAliasDto(Guid Id, LocalizedTextDto Alias, string Status);

public sealed record ItemBranchAvailabilityDto(
    Guid Id,
    Guid BranchId,
    string BranchCode,
    string BranchName,
    string Status,
    DateTimeOffset? EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc);

public sealed record ItemImageSummaryDto(
    Guid Id,
    Guid FileId,
    string Role,
    bool IsPrimary,
    int DisplayOrder,
    LocalizedTextDto AltText,
    LocalizedTextDto? Caption);

public sealed record ItemDetailProjection(
    Guid Id,
    Guid OrganizationId,
    string Code,
    string ItemType,
    CategorySummaryDto Category,
    BrandSummaryDto? Brand,
    UnitSummaryDto BaseUnit,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    string? TaxCategoryCode,
    string AvailabilityMode,
    ItemCapabilitiesDto Capabilities,
    Dictionary<string, string>? Attributes,
    int? AttributesSchemaVersion,
    string Status,
    bool ActivatedOnce,
    DateTimeOffset? ActivatedAtUtc,
    Guid? ActivatedByUserId,
    DateTimeOffset? InactiveAtUtc,
    Guid? InactiveByUserId,
    string? InactiveReasonCode,
    string? InactiveReason,
    IReadOnlyList<ItemAliasDto> Aliases,
    IReadOnlyList<ItemBranchAvailabilityDto> BranchAvailabilities,
    ItemImageSummaryDto? PrimaryImage,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    Guid CreatedByUserId,
    DateTimeOffset UpdatedAtUtc,
    Guid UpdatedByUserId);

public sealed record ItemCategoryDetailProjection(
    Guid Id,
    Guid OrganizationId,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    Guid? ParentCategoryId,
    CategorySummaryDto? ParentCategory,
    IReadOnlyList<string> AllowedItemTypes,
    int SortOrder,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record ItemBrandDetailProjection(
    Guid Id,
    Guid OrganizationId,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    int SortOrder,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record UnitOfMeasureDetailProjection(
    Guid Id,
    Guid OrganizationId,
    string Code,
    LocalizedTextDto Name,
    string Symbol,
    string Dimension,
    int DecimalScale,
    string RoundingMode,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
