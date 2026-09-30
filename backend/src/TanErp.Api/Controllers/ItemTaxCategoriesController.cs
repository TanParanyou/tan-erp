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
[Route("api/v1/item-tax-categories")]
public sealed class ItemTaxCategoriesController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemStore _store;

    public ItemTaxCategoriesController(IRequestAccessResolver accessResolver, IItemStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ItemTaxCategoryDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "items.read", cancellationToken);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var rows = await _store.ListTaxCategoriesAsync(accessResult.Value!.OrganizationId, cancellationToken);
        return Ok(rows.Select(ItemResponseMapper.ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ItemTaxCategoryDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "items.read", cancellationToken);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var category = await _store.GetTaxCategoryAsync(accessResult.Value!.OrganizationId, id, cancellationToken);
        if (category is null) return ProblemDetailsMapper.CreateProblemResult("ITEM_TAX_CATEGORY_NOT_FOUND", HttpContext);
        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(category));
    }

    [HttpPost]
    [ProducesResponseType<ItemTaxCategoryDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemTaxCategoryRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        if (!string.Equals(auth.IdempotencyKey, idempotencyKey, StringComparison.Ordinal))
            return ProblemDetailsMapper.CreateProblemResult("IDEMPOTENCY_KEY_INVALID", HttpContext);
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "items.manage-taxonomy", cancellationToken);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var access = accessResult.Value!;
        var data = new CreateTaxCategoryData(request.Code, new LocalizedTextDto(request.Name.Thai, request.Name.English), request.SortOrder);
        var result = await _store.CreateTaxCategoryAsync(data, access, auth.IdempotencyKey, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var category = result.Value!;
        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = category.Id }, ItemResponseMapper.ToResponse(category));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ItemTaxCategoryDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateItemTaxCategoryRequest request, CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "items.manage-taxonomy", cancellationToken);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var access = accessResult.Value!;
        var data = new UpdateTaxCategoryData(id, auth.IfMatchRowVersion, request.Code, new LocalizedTextDto(request.Name.Thai, request.Name.English), request.SortOrder);
        var result = await _store.UpdateTaxCategoryAsync(data, access, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var category = result.Value!;
        Response.Headers.ETag = $"\"{category.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(category));
    }
}
