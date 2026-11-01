using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Procurement;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/suppliers/{supplierId:guid}/item-prices")]
[Authorize]
public class SupplierItemPricesController : ProcurementControllerBase
{
    private readonly SupplierItemPriceHandler _handler;

    public SupplierItemPricesController(SupplierItemPriceHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<SupplierItemPriceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromRoute] Guid supplierId, [FromBody] SupplierItemPriceRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateAsync(caller, supplierId, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value));
    }

    [HttpPut("{priceId:guid}")]
    [ProducesResponseType<SupplierItemPriceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid supplierId, [FromRoute] Guid priceId, [FromBody] SupplierItemPriceRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.UpdateAsync(caller, supplierId, priceId, version, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    [HttpDelete("{priceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete([FromRoute] Guid supplierId, [FromRoute] Guid priceId, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.DeleteAsync(caller, supplierId, priceId, version, ct);
        return result.IsFailure ? Problem(result.Error.Code) : NoContent();
    }

    [HttpGet]
    [ProducesResponseType<SupplierItemPriceListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromRoute] Guid supplierId, [FromQuery] Guid? itemId, [FromQuery] int page = 1, [FromQuery] int pageSize = ProcurementHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, supplierId, itemId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new SupplierItemPriceListResponse(
            paged.Items.Select(ToResponse).ToList(),
            new ProcurementPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    [HttpGet("effective")]
    [ProducesResponseType<SupplierItemPriceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Effective([FromRoute] Guid supplierId, [FromQuery] Guid itemId, [FromQuery] DateOnly? onDate, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ResolveEffectiveAsync(caller, supplierId, itemId, onDate ?? DateOnly.FromDateTime(DateTime.UtcNow), ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(ToResponse(result.Value!));
    }

    private static SupplierItemPriceInput ToInput(SupplierItemPriceRequest r) =>
        new(r.ItemId, r.UnitPrice, r.MinOrderQuantity, r.LeadTimeDays, r.EffectiveFrom, r.EffectiveTo, r.IsPreferred);

    private static SupplierItemPriceResponse ToResponse(SupplierItemPriceProjection p) => new(
        p.Id, p.SupplierId, p.ItemId, p.ItemCode, p.ItemNameTh, p.UnitPrice, p.MinOrderQuantity, p.LeadTimeDays,
        p.EffectiveFrom, p.EffectiveTo, p.IsPreferred, p.RowVersion);
}
