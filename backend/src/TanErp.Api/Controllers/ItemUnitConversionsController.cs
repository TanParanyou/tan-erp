using System.Globalization;
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
[Route("api/v1/items/{itemId:guid}/unit-conversions")]
public sealed class ItemUnitConversionsController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemStore _store;

    public ItemUnitConversionsController(IRequestAccessResolver accessResolver, IItemStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ItemUnitConversionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(Guid itemId, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "items.read", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        if (await _store.GetItemAsync(access.Value!.OrganizationId, itemId, ct) is null)
            return ProblemDetailsMapper.CreateProblemResult("ITEM_NOT_FOUND", HttpContext);
        var rows = await _store.ListItemUnitConversionsAsync(access.Value.OrganizationId, itemId, ct);
        return Ok(rows.Select(ItemUnitConversionResponseMapper.ToResponse).ToArray());
    }

    [HttpPost]
    [ProducesResponseType<ItemUnitConversionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(Guid itemId, CreateItemUnitConversionRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        if (!string.Equals(auth.Value!.IdempotencyKey, idempotencyKey, StringComparison.Ordinal))
            return ProblemDetailsMapper.CreateProblemResult("IDEMPOTENCY_KEY_INVALID", HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value.FirebaseUid, auth.Value.MembershipId, "items.manage-barcodes", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        if (!decimal.TryParse(request.Factor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var factor))
            return ProblemDetailsMapper.CreateProblemResult("ITEM_UNIT_CONVERSION_FACTOR_INVALID", HttpContext);
        var result = await _store.CreateItemUnitConversionAsync(new CreateItemUnitConversionData(itemId,
            request.FromUnitId, request.ToUnitId, factor, request.EffectiveFrom, request.EffectiveTo, request.Reason),
            access.Value!, auth.Value.IdempotencyKey, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var response = ItemUnitConversionResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return CreatedAtAction(nameof(List), new { itemId }, response);
    }
}
