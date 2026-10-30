using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Inventory;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Inventory;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
public class InventoryController : InventoryControllerBase
{
    private readonly InventoryHandler _handler;

    public InventoryController(InventoryHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("receipts")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Receive([FromBody] ReceiveGoodsReceiptRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new ReceiveGoodsReceiptLineInput(l.ItemId, l.Quantity, l.LocationId, l.LotNumber, l.SerialNumbers)).ToList();
        return Created(await _handler.ReceiveGoodsReceiptAsync(caller, key, request.GoodsReceiptId, request.WarehouseId, request.LocationId, lines, ct));
    }

    [HttpPost("issues")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Issue([FromBody] IssueStockRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new StockLineInput(l.ItemId, l.Quantity, l.LocationId, l.ToLocationId, l.LotNumber, l.SerialNumbers)).ToList();
        return Created(await _handler.IssueAsync(caller, key, request.WarehouseId, request.ProjectId, request.Reason, lines, request.LocationId, request.OccurredAtUtc, ct));
    }

    [HttpPost("transfers")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Transfer([FromBody] TransferStockRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new StockLineInput(l.ItemId, l.Quantity, l.LocationId, l.ToLocationId, l.LotNumber, l.SerialNumbers)).ToList();
        return Created(await _handler.TransferAsync(caller, key, request.FromWarehouseId, request.ToWarehouseId, request.Reason, lines, request.FromLocationId, request.ToLocationId, request.OccurredAtUtc, ct));
    }

    [HttpPost("adjustments")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Adjust([FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new AdjustmentLineInput(l.ItemId, l.CountedQuantity, l.UnitCost, l.LocationId, l.LotNumber, l.SerialNumbers)).ToList();
        return Created(await _handler.AdjustAsync(caller, key, request.WarehouseId, request.Reason, lines, request.LocationId, request.OccurredAtUtc, ct));
    }

    [HttpPost("returns")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Return([FromBody] ReturnStockRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new ReturnLineInput(l.ItemId, l.Quantity, l.UnitCost, l.LocationId, l.LotNumber, l.SerialNumbers)).ToList();
        return Created(await _handler.ReturnAsync(caller, key, request.WarehouseId, request.LocationId, request.ProjectId, request.SourceIssueDocumentId, request.OccurredAtUtc, request.Reason, lines, ct));
    }

    [HttpPost("reservations")]
    [ProducesResponseType<ReservationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Reserve([FromBody] ReserveStockRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ReserveAsync(caller, key, request.WarehouseId, request.ItemId, request.ProjectId, request.Quantity, request.Note, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value));
    }

    [HttpPost("reservations/{id:guid}/release")]
    [ProducesResponseType<ReservationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Release([FromRoute] Guid id, [FromBody] ReleaseReservationRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ReleaseReservationAsync(caller, id, request.ExpectedVersion, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    [HttpPost("period-closes")]
    [ProducesResponseType<InventoryPeriodCloseResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ClosePeriod([FromBody] CloseInventoryPeriodRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ClosePeriodAsync(caller, key, request.PeriodYear, request.PeriodMonth, request.Note, ct);
        return result.IsFailure ? Problem(result.Error.Code) : StatusCode(StatusCodes.Status201Created, ToResponse(result.Value!));
    }

    [HttpGet("period-closes")]
    [ProducesResponseType<InventoryPeriodCloseListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPeriodCloses(CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListPeriodClosesAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new InventoryPeriodCloseListResponse(result.Value!.Select(ToResponse).ToList()));
    }

    [HttpGet("period-closes/{id:guid}")]
    [ProducesResponseType<InventoryPeriodCloseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriodClose([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetPeriodCloseAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(ToResponse(result.Value!));
    }

    [HttpGet("documents/{id:guid}")]
    [ProducesResponseType<StockDocumentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocument([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetDocumentAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(ToResponse(result.Value!));
    }

    [HttpGet("balances")]
    [ProducesResponseType<StockBalanceListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Balances(
        [FromQuery] Guid? warehouseId, [FromQuery] Guid? itemId, [FromQuery] string? search, [FromQuery] bool inStockOnly = false,
        [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListBalancesAsync(caller, warehouseId, itemId, search, inStockOnly, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new StockBalanceListResponse(
            paged.Items.Select(b => new StockBalanceResponse(b.Id, ToResponse(b.Warehouse), ToResponse(b.Item), b.OnHand, b.Reserved, b.Available, b.AverageCost, b.TotalValue, b.UpdatedAtUtc)).ToList(),
            Pagination(paged.Page, paged.PageSize, paged.TotalCount), paged.TotalValue));
    }

    [HttpGet("location-balances")]
    [ProducesResponseType<StockLocationBalanceListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LocationBalances(
        [FromQuery] Guid? warehouseId, [FromQuery] Guid? locationId, [FromQuery] Guid? itemId, [FromQuery] string? lotNumber, [FromQuery] bool inStockOnly = false,
        [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListLocationBalancesAsync(caller, warehouseId, locationId, itemId, lotNumber, inStockOnly, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new StockLocationBalanceListResponse(
            paged.Items.Select(b => new StockLocationBalanceResponse(b.Id, ToResponse(b.Warehouse), ToResponse(b.Location), ToResponse(b.Item), b.LotNumber, b.OnHand, b.UpdatedAtUtc)).ToList(),
            Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("serials")]
    [ProducesResponseType<StockSerialListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Serials(
        [FromQuery] Guid? warehouseId, [FromQuery] Guid? locationId, [FromQuery] Guid? itemId, [FromQuery] string? status, [FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListSerialsAsync(caller, warehouseId, locationId, itemId, status, search, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new StockSerialListResponse(
            paged.Items.Select(s => new StockSerialResponse(
                s.Id,
                ToResponse(s.Item),
                s.SerialNumber,
                s.Warehouse is null ? null : ToResponse(s.Warehouse),
                s.Location is null ? null : ToResponse(s.Location),
                s.LotNumber,
                s.Status,
                s.LastStockDocumentId,
                s.UpdatedAtUtc)).ToList(),
            Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("movements")]
    [ProducesResponseType<StockMovementListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Movements(
        [FromQuery] Guid? warehouseId, [FromQuery] Guid? itemId, [FromQuery] string? kind, [FromQuery] Guid? documentId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListMovementsAsync(caller, warehouseId, itemId, kind, documentId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new StockMovementListResponse(paged.Items.Select(ToResponse).ToList(), Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("reservations")]
    [ProducesResponseType<ReservationListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reservations(
        [FromQuery] Guid? projectId, [FromQuery] Guid? warehouseId, [FromQuery] string? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = InventoryHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListReservationsAsync(caller, projectId, warehouseId, status, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new ReservationListResponse(paged.Items.Select(ToResponse).ToList(), Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("reconciliation")]
    [ProducesResponseType<ReconciliationResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reconciliation([FromQuery] Guid? warehouseId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ReconcileAsync(caller, warehouseId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var r = result.Value!;
        return Ok(new ReconciliationResponse(
            r.RowCount, r.InconsistentCount,
            r.Rows.Select(x => new ReconciliationRowResponse(ToResponse(x.Warehouse), ToResponse(x.Item), x.BalanceOnHand, x.LedgerQuantity, x.BalanceValue, x.LedgerValue, x.IsConsistent)).ToList()));
    }

    private IActionResult Created(Result<StockDocumentProjection> result) =>
        result.IsFailure ? Problem(result.Error.Code) : StatusCode(StatusCodes.Status201Created, ToResponse(result.Value!));
}
