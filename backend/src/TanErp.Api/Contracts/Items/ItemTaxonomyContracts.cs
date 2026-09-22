using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Items;

public sealed class CreateItemCategoryRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public List<string>? AllowedItemTypes { get; set; }

    public int SortOrder { get; set; } = 0;
}

public sealed class UpdateItemCategoryRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public List<string>? AllowedItemTypes { get; set; }

    public int SortOrder { get; set; } = 0;
}

public sealed class ItemCategoryDetailResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
    public LocalizedTextResponse? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public CategorySummaryResponse? ParentCategory { get; set; }
    public IReadOnlyList<string> AllowedItemTypes { get; set; } = Array.Empty<string>();
    public int SortOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid RowVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public sealed class CreateItemBrandRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    public int SortOrder { get; set; } = 0;
}

public sealed class UpdateItemBrandRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    public LocalizedTextInput? Description { get; set; }

    public int SortOrder { get; set; } = 0;
}

public sealed class ItemBrandDetailResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
    public LocalizedTextResponse? Description { get; set; }
    public int SortOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid RowVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public sealed class CreateUnitOfMeasureRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    [Required]
    [MaxLength(16)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Dimension { get; set; } = "Count";

    public int DecimalScale { get; set; } = 0;

    [MaxLength(32)]
    public string RoundingMode { get; set; } = "HalfUp";
}

public sealed class UpdateUnitOfMeasureRequest
{
    [Required]
    [MaxLength(64)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedTextInput Name { get; set; } = new();

    [Required]
    [MaxLength(16)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Dimension { get; set; } = "Count";

    public int DecimalScale { get; set; } = 0;

    [MaxLength(32)]
    public string RoundingMode { get; set; } = "HalfUp";
}

public sealed class UnitOfMeasureDetailResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public LocalizedTextResponse Name { get; set; } = new();
    public string Symbol { get; set; } = string.Empty;
    public string Dimension { get; set; } = string.Empty;
    public int DecimalScale { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid RowVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
