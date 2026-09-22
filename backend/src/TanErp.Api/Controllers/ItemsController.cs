using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/items")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemStore _store;

    public ItemsController(IRequestAccessResolver accessResolver, IItemStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    [HttpPost]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemRequest request,
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
            "items.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;

        var data = new CreateItemData(
            request.Code,
            request.ItemType,
            request.CategoryId,
            request.BrandId,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.BaseUnitId,
            request.AvailabilityMode,
            new ItemCapabilitiesDto(
                request.Capabilities.CanSell,
                request.Capabilities.CanCost,
                request.Capabilities.CanPurchase,
                request.Capabilities.CanStock,
                request.Capabilities.CanProduce),
            request.SelectedBranchIds,
            request.Aliases?.Select(a => new LocalizedTextDto(a.Thai, a.English)).ToList(),
            request.Attributes,
            request.AttributesSchemaVersion);

        var result = await _store.CreateItemAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ItemResponseMapper.ToResponse(item));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
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
        var item = await _store.GetItemAsync(access.OrganizationId, id, cancellationToken);
        if (item == null)
        {
            return ProblemDetailsMapper.CreateProblemResult("ITEM_NOT_FOUND", HttpContext);
        }

        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpGet]
    [ProducesResponseType<PagedItemsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? itemType,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? brandId,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
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
        var query = new ItemQuery(
            access.OrganizationId,
            search,
            itemType,
            categoryId,
            brandId,
            status,
            pageNumber,
            pageSize);

        var paged = await _store.ListItemsAsync(query, cancellationToken);
        var response = new PagedItemsResponse
        {
            Items = paged.Items.Select(ItemResponseMapper.ToResponse).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateItemRequest request,
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
            "items.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new UpdateItemData(
            id,
            auth.IfMatchRowVersion,
            request.Code,
            request.ItemType,
            request.CategoryId,
            request.BrandId,
            new LocalizedTextDto(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedTextDto(request.Description.Thai, request.Description.English) : null,
            request.BaseUnitId,
            request.AvailabilityMode,
            new ItemCapabilitiesDto(
                request.Capabilities.CanSell,
                request.Capabilities.CanCost,
                request.Capabilities.CanPurchase,
                request.Capabilities.CanStock,
                request.Capabilities.CanProduce),
            request.Attributes,
            request.AttributesSchemaVersion);

        var result = await _store.UpdateItemAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Activate(
        [FromRoute] Guid id,
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
            "items.activate",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _store.ActivateItemAsync(access.OrganizationId, id, auth.IfMatchRowVersion, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Deactivate(
        [FromRoute] Guid id,
        [FromBody] DeactivateItemRequest request,
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
            "items.deactivate",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _store.DeactivateItemAsync(
            access.OrganizationId,
            id,
            auth.IfMatchRowVersion,
            request.ReasonCode,
            request.Reason,
            access,
            cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpPut("{id:guid}/branch-availability")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> SetBranchAvailability(
        [FromRoute] Guid id,
        [FromBody] SetBranchAvailabilityRequest request,
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
            "items.manage-branches",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _store.SetBranchAvailabilityAsync(
            access.OrganizationId,
            id,
            auth.IfMatchRowVersion,
            request.Mode,
            request.BranchIds,
            access,
            cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpPost("{id:guid}/aliases")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddAlias(
        [FromRoute] Guid id,
        [FromBody] AddAliasRequest request,
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
            "items.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _store.AddAliasAsync(
            access.OrganizationId,
            id,
            new LocalizedTextDto(request.Alias.Thai, request.Alias.English),
            access,
            cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }

    [HttpDelete("{id:guid}/aliases/{aliasId:guid}")]
    [ProducesResponseType<ItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAlias(
        [FromRoute] Guid id,
        [FromRoute] Guid aliasId,
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
            "items.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _store.RemoveAliasAsync(access.OrganizationId, id, aliasId, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var item = result.Value!;
        Response.Headers.ETag = $"\"{item.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(item));
    }
}
