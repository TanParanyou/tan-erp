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
[Route("api/v1/items/{itemId:guid}/barcodes")]
public sealed class ItemBarcodesController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemStore _store;

    public ItemBarcodesController(IRequestAccessResolver accessResolver, IItemStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ItemBarcodeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(Guid itemId, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "items.read", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        if (await _store.GetItemAsync(access.Value!.OrganizationId, itemId, ct) is null)
            return ProblemDetailsMapper.CreateProblemResult("ITEM_NOT_FOUND", HttpContext);
        var rows = await _store.ListBarcodesAsync(access.Value.OrganizationId, itemId, ct);
        return Ok(rows.Select(ItemBarcodeResponseMapper.ToResponse).ToArray());
    }

    [HttpGet("/api/v1/items/by-barcode")]
    [ProducesResponseType<ItemBarcodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByBarcode([FromQuery] string value, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "items.read", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        if (string.IsNullOrWhiteSpace(value)) return ProblemDetailsMapper.CreateProblemResult("ITEM_BARCODE_INVALID", HttpContext);
        var result = await _store.FindActiveBarcodeAsync(access.Value!.OrganizationId, access.Value.BranchId, value, ct);
        return result is null ? ProblemDetailsMapper.CreateProblemResult("ITEM_BARCODE_NOT_FOUND", HttpContext) : Ok(ItemBarcodeResponseMapper.ToResponse(result));
    }

    [HttpPost]
    [ProducesResponseType<ItemBarcodeResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(Guid itemId, CreateItemBarcodeRequest request, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        if (!string.Equals(auth.Value!.IdempotencyKey, idempotencyKey, StringComparison.Ordinal))
            return ProblemDetailsMapper.CreateProblemResult("IDEMPOTENCY_KEY_INVALID", HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value.FirebaseUid, auth.Value.MembershipId, "items.manage-barcodes", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        if (!decimal.TryParse(request.QuantityInBaseUnit, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var quantity))
            return ProblemDetailsMapper.CreateProblemResult("ITEM_BARCODE_INVALID", HttpContext);
        var result = await _store.CreateBarcodeAsync(new CreateItemBarcodeData(itemId, request.IdentifierType, request.Value,
            request.UnitId, quantity, request.PackagingLevel, request.IsPrimary), access.Value!, auth.Value.IdempotencyKey, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var barcode = result.Value!;
        Response.Headers.ETag = $"\"{barcode.RowVersion}\"";
        return CreatedAtAction(nameof(List), new { itemId }, ItemBarcodeResponseMapper.ToResponse(barcode));
    }

    [HttpPost("{barcodeId:guid}/primary")]
    [ProducesResponseType<ItemBarcodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> SetPrimary(Guid itemId, Guid barcodeId, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "items.manage-barcodes", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        var result = await _store.SetBarcodePrimaryAsync(access.Value!.OrganizationId, itemId, barcodeId, auth.Value.IfMatchRowVersion, access.Value, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ItemBarcodeResponseMapper.ToResponse(result.Value));
    }

    [HttpPost("{barcodeId:guid}/deactivate")]
    [ProducesResponseType<ItemBarcodeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Deactivate(Guid itemId, Guid barcodeId, CancellationToken ct)
    {
        var auth = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);
        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "items.manage-barcodes", ct);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);
        var result = await _store.DeactivateBarcodeAsync(access.Value!.OrganizationId, itemId, barcodeId, auth.Value.IfMatchRowVersion, access.Value, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ItemBarcodeResponseMapper.ToResponse(result.Value));
    }
}
