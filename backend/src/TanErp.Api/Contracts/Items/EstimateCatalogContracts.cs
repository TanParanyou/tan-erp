using System.Text.Json;

namespace TanErp.Api.Contracts.Items;

public sealed record EstimateCatalogResponse
{
    public IReadOnlyList<EstimateCatalogItemResponse> Items { get; init; } = Array.Empty<EstimateCatalogItemResponse>();
    public CatalogFacetsResponse Facets { get; init; } = new();
    public CatalogPageInfoResponse PageInfo { get; init; } = new();
}

public sealed record EstimateCatalogItemResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public LocalizedTextResponse Name { get; init; } = new();
    public LocalizedTextResponse? Description { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public CategorySummaryResponse Category { get; init; } = new();
    public BrandSummaryResponse? Brand { get; init; }
    public UnitSummaryResponse BaseUnit { get; init; } = new();
    public JsonDocument Attributes { get; init; } = JsonDocument.Parse("{}");
    public CatalogPrimaryImageResponse? PrimaryImage { get; init; }
    public CatalogResolvedCostResponse? ResolvedCost { get; init; }
}

public sealed record CatalogPrimaryImageResponse
{
    public Guid FileId { get; init; }
    public LocalizedTextResponse AltText { get; init; } = new();
}

public sealed record CatalogResolvedCostResponse
{
    public Guid CostRecordId { get; init; }
    public int Version { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "THB";
    public string UnitCode { get; init; } = string.Empty;
    public string Scope { get; init; } = "organization";
    public DateTimeOffset EffectiveFromUtc { get; init; }
    public string PolicyVersion { get; init; } = "COST-RESOLVE-v1";
}

public sealed record CatalogFacetsResponse
{
    public IReadOnlyList<CatalogFacetValueResponse> ItemTypes { get; init; } = Array.Empty<CatalogFacetValueResponse>();
    public IReadOnlyList<CatalogCategoryFacetResponse> Categories { get; init; } = Array.Empty<CatalogCategoryFacetResponse>();
    public IReadOnlyList<CatalogBrandFacetResponse> Brands { get; init; } = Array.Empty<CatalogBrandFacetResponse>();
}

public sealed record CatalogFacetValueResponse
{
    public string Value { get; init; } = string.Empty;
    public int Count { get; init; }
}

public sealed record CatalogCategoryFacetResponse
{
    public Guid Id { get; init; }
    public LocalizedTextResponse Name { get; init; } = new();
    public int Count { get; init; }
}

public sealed record CatalogBrandFacetResponse
{
    public Guid Id { get; init; }
    public LocalizedTextResponse Name { get; init; } = new();
    public int Count { get; init; }
}

public sealed record CatalogPageInfoResponse
{
    public string? NextCursor { get; init; }
    public bool HasNextPage { get; init; }
}
