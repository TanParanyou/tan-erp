using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Inventory;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Inventory;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
[Authorize]
public class WarehousesController : InventoryControllerBase
{
    private readonly InventoryHandler _handler;

    public WarehousesController(InventoryHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<WarehouseResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] WarehouseRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateWarehouseAsync(caller, key, new WarehouseInput(request.Name, request.Address), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<WarehouseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] WarehouseRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateWarehouseAsync(caller, id, version, new WarehouseInput(request.Name, request.Address), ct));
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType<WarehouseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetWarehouseActiveAsync(caller, id, version, true, ct));
    }

    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType<WarehouseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetWarehouseActiveAsync(caller, id, version, false, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<WarehouseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetWarehouseAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<WarehouseListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListWarehousesAsync(caller, search, status, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new WarehouseListResponse(paged.Items.Select(ToResponse).ToList(), Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("{id:guid}/locations")]
    [ProducesResponseType<WarehouseLocationListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListLocations([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListWarehouseLocationsAsync(caller, id, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return Ok(new WarehouseLocationListResponse(result.Value!.Select(ToResponse).ToList()));
    }

    [HttpPost("{id:guid}/locations")]
    [ProducesResponseType<WarehouseLocationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateLocation([FromRoute] Guid id, [FromBody] CreateWarehouseLocationRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateWarehouseLocationAsync(caller, key, id, new WarehouseLocationCreateInput(request.Code, request.Name), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}/locations/{locationId:guid}")]
    [ProducesResponseType<WarehouseLocationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateLocation([FromRoute] Guid id, [FromRoute] Guid locationId, [FromBody] UpdateWarehouseLocationRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.UpdateWarehouseLocationAsync(caller, id, locationId, version, new WarehouseLocationUpdateInput(request.Name, request.Active), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private IActionResult Respond(Result<WarehouseProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static WarehouseResponse ToResponse(WarehouseProjection w) => new(w.Id, w.BranchId, w.Code, w.Name, w.Address, w.Status, w.RowVersion, w.CreatedAtUtc);

    private static WarehouseLocationResponse ToResponse(WarehouseLocationProjection l) =>
        new(l.Id, l.WarehouseId, l.Code, l.Name, l.IsDefault, l.Status, l.RowVersion, l.CreatedAtUtc, l.UpdatedAtUtc);
}
