using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Items;

public sealed class LocalizedTextInput
{
    [Required]
    [MaxLength(200)]
    public string Thai { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? English { get; set; }
}

public sealed class ItemCapabilitiesInput
{
    public bool CanSell { get; set; } = true;
    public bool CanCost { get; set; } = true;
    public bool CanPurchase { get; set; } = true;
    public bool CanStock { get; set; } = true;
    public bool CanProduce { get; set; } = false;
}

public sealed class CreateItemRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    public Guid? BrandId { get; set; }

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    [Required]
    public Guid BaseUnitId { get; set; }

    [Required]
    [MaxLength(32)]
    public string AvailabilityMode { get; set; } = "all_branches";

    public ItemCapabilitiesInput Capabilities { get; set; } = new();

    public List<Guid>? SelectedBranchIds { get; set; }

    public List<LocalizedTextInput>? Aliases { get; set; }

    public Dictionary<string, string>? Attributes { get; set; }
    public int? AttributesSchemaVersion { get; set; }
}

public sealed class UpdateItemRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string ItemType { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    public Guid? BrandId { get; set; }

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    [Required]
    public Guid BaseUnitId { get; set; }

    [Required]
    [MaxLength(32)]
    public string AvailabilityMode { get; set; } = "all_branches";

    public ItemCapabilitiesInput Capabilities { get; set; } = new();

    public Dictionary<string, string>? Attributes { get; set; }
    public int? AttributesSchemaVersion { get; set; }
}

public sealed class DeactivateItemRequest
{
    [Required]
    [MaxLength(64)]
    public string ReasonCode { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Reason { get; set; }
}

public sealed class SetBranchAvailabilityRequest
{
    [Required]
    [MaxLength(32)]
    public string Mode { get; set; } = "all_branches";

    public List<Guid> BranchIds { get; set; } = new();
}

public sealed class AddAliasRequest
{
    [Required]
    public LocalizedTextInput Alias { get; set; } = new();
}

public sealed class LocalizedTextResponse
{
    public string Thai { get; set; } = string.Empty;
    public string? English { get; set; }
}

public sealed class CategorySummaryResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
    public Guid? ParentCategoryId { get; set; }
}

public sealed class BrandSummaryResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
}

public sealed class UnitSummaryResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
}

public sealed class ItemCapabilitiesResponse
{
    public bool CanSell { get; set; }
    public bool CanCost { get; set; }
    public bool CanPurchase { get; set; }
    public bool CanStock { get; set; }
    public bool CanProduce { get; set; }
}

public sealed class ItemAliasResponse
{
    public Guid Id { get; set; }
    public LocalizedTextResponse Alias { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}

public sealed class ItemBranchAvailabilityResponse
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? EffectiveFromUtc { get; set; }
    public DateTimeOffset? EffectiveToUtc { get; set; }
}

public sealed class ItemImageSummaryResponse
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public LocalizedTextResponse AltText { get; set; } = new();
    public LocalizedTextResponse? Caption { get; set; }
}

public sealed class ItemResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public CategorySummaryResponse Category { get; set; } = new();
    public BrandSummaryResponse? Brand { get; set; }
    public UnitSummaryResponse BaseUnit { get; set; } = new();
    public LocalizedTextResponse Name { get; set; } = new();
    public LocalizedTextResponse? Description { get; set; }
    public string? TaxCategoryCode { get; set; }
    public string AvailabilityMode { get; set; } = string.Empty;
    public ItemCapabilitiesResponse Capabilities { get; set; } = new();
    public Dictionary<string, string>? Attributes { get; set; }
    public int? AttributesSchemaVersion { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool ActivatedOnce { get; set; }
    public DateTimeOffset? ActivatedAtUtc { get; set; }
    public Guid? ActivatedByUserId { get; set; }
    public DateTimeOffset? InactiveAtUtc { get; set; }
    public Guid? InactiveByUserId { get; set; }
    public string? InactiveReasonCode { get; set; }
    public string? InactiveReason { get; set; }
    public IReadOnlyList<ItemAliasResponse> Aliases { get; set; } = Array.Empty<ItemAliasResponse>();
    public IReadOnlyList<ItemBranchAvailabilityResponse> BranchAvailabilities { get; set; } = Array.Empty<ItemBranchAvailabilityResponse>();
    public ItemImageSummaryResponse? PrimaryImage { get; set; }
    public Guid RowVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public Guid UpdatedByUserId { get; set; }
}

public sealed class PagedItemsResponse
{
    public IReadOnlyList<ItemResponse> Items { get; set; } = Array.Empty<ItemResponse>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
