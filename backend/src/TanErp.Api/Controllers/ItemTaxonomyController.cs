using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class ItemTaxonomyController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemStore _store;

    public ItemTaxonomyController(IRequestAccessResolver accessResolver, IItemStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    // ================= Categories =================

    [HttpGet("api/v1/item-categories")]
    [ProducesResponseType<IReadOnlyList<ItemCategoryDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListCategories(CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var categories = await _store.ListCategoriesAsync(access.OrganizationId, cancellationToken);
        var response = categories.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("api/v1/item-categories/{id:guid}")]
    [ProducesResponseType<ItemCategoryDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var category = await _store.GetCategoryAsync(access.OrganizationId, id, cancellationToken);
        if (category == null)
        {
            return ProblemDetailsMapper.CreateProblemResult("ITEM_CATEGORY_NOT_FOUND", HttpContext);
        }

        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(category));
    }

    [HttpPost("api/v1/item-categories")]
    [ProducesResponseType<ItemCategoryDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateItemCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-taxonomy",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new CreateCategoryData(
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.ParentCategoryId,
            request.AllowedItemTypes,
            request.SortOrder);

        var result = await _store.CreateCategoryAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var category = result.Value!;
        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, ItemResponseMapper.ToResponse(category));
    }

    [HttpPut("api/v1/item-categories/{id:guid}")]
    [ProducesResponseType<ItemCategoryDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateItemCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-taxonomy",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new UpdateCategoryData(
            id,
            auth.IfMatchRowVersion,
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.ParentCategoryId,
            request.AllowedItemTypes,
            request.SortOrder);

        var result = await _store.UpdateCategoryAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var category = result.Value!;
        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(category));
    }

    // ================= Brands =================

    [HttpGet("api/v1/item-brands")]
    [ProducesResponseType<IReadOnlyList<ItemBrandDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListBrands(CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var brands = await _store.ListBrandsAsync(access.OrganizationId, cancellationToken);
        var response = brands.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("api/v1/item-brands/{id:guid}")]
    [ProducesResponseType<ItemBrandDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBrand(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var brand = await _store.GetBrandAsync(access.OrganizationId, id, cancellationToken);
        if (brand == null)
        {
            return ProblemDetailsMapper.CreateProblemResult("ITEM_BRAND_NOT_FOUND", HttpContext);
        }

        Response.Headers.ETag = $"\"{brand.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(brand));
    }

    [HttpPost("api/v1/item-brands")]
    [ProducesResponseType<ItemBrandDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateBrand(
        [FromBody] CreateItemBrandRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-taxonomy",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new CreateBrandData(
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.SortOrder);

        var result = await _store.CreateBrandAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var brand = result.Value!;
        Response.Headers.ETag = $"\"{brand.RowVersion}\"";
        return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, ItemResponseMapper.ToResponse(brand));
    }

    [HttpPut("api/v1/item-brands/{id:guid}")]
    [ProducesResponseType<ItemBrandDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateBrand(
        [FromRoute] Guid id,
        [FromBody] UpdateItemBrandRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-taxonomy",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new UpdateBrandData(
            id,
            auth.IfMatchRowVersion,
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.SortOrder);

        var result = await _store.UpdateBrandAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var brand = result.Value!;
        Response.Headers.ETag = $"\"{brand.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(brand));
    }

    // ================= Units of Measure =================

    [HttpGet("api/v1/units-of-measure")]
    [ProducesResponseType<IReadOnlyList<UnitOfMeasureDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUnits(CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "units.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var units = await _store.ListUnitsAsync(access.OrganizationId, cancellationToken);
        var response = units.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("api/v1/units-of-measure/{id:guid}")]
    [ProducesResponseType<UnitOfMeasureDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnit(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "units.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var unit = await _store.GetUnitAsync(access.OrganizationId, id, cancellationToken);
        if (unit == null)
        {
            return ProblemDetailsMapper.CreateProblemResult("ITEM_UNIT_NOT_FOUND", HttpContext);
        }

        Response.Headers.ETag = $"\"{unit.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(unit));
    }

    [HttpPost("api/v1/units-of-measure")]
    [ProducesResponseType<UnitOfMeasureDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUnit(
        [FromBody] CreateUnitOfMeasureRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "units.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new CreateUnitData(
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Symbol,
            request.Dimension,
            request.DecimalScale,
            request.RoundingMode);

        var result = await _store.CreateUnitAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var unit = result.Value!;
        Response.Headers.ETag = $"\"{unit.RowVersion}\"";
        return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, ItemResponseMapper.ToResponse(unit));
    }

    [HttpPut("api/v1/units-of-measure/{id:guid}")]
    [ProducesResponseType<UnitOfMeasureDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateUnit(
        [FromRoute] Guid id,
        [FromBody] UpdateUnitOfMeasureRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "units.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new UpdateUnitData(
            id,
            auth.IfMatchRowVersion,
            request.Code,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Symbol,
            request.Dimension,
            request.DecimalScale,
            request.RoundingMode);

        var result = await _store.UpdateUnitAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var unit = result.Value!;
        Response.Headers.ETag = $"\"{unit.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(unit));
    }
}
