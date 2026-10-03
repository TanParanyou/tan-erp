using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Procurement;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/purchase-orders")]
[Authorize]
public class PurchaseOrdersController : ProcurementControllerBase
{
    private readonly ProcurementHandler _handler;

    public PurchaseOrdersController(ProcurementHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] PurchaseOrderRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreatePurchaseOrderAsync(caller, key, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PurchaseOrderRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdatePurchaseOrderAsync(caller, id, version, ToInput(request), ct));
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, PurchaseOrderAction.Submit, null, ct);

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] PurchaseOrderActionRequest? request, CancellationToken ct) => ActionAsync(id, PurchaseOrderAction.Approve, request?.Note, ct);

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] PurchaseOrderActionRequest? request, CancellationToken ct) => ActionAsync(id, PurchaseOrderAction.Reject, request?.Note, ct);

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] PurchaseOrderActionRequest? request, CancellationToken ct) => ActionAsync(id, PurchaseOrderAction.Cancel, request?.Note, ct);

    [HttpPost("{id:guid}/receipts")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostReceipt([FromRoute] Guid id, [FromBody] GoodsReceiptRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new GoodsReceiptInput(request.ReceivedAtUtc, request.Note, request.Lines?.Select(l => new GoodsReceiptLineInput(l.PurchaseOrderLineId, l.Quantity)).ToList() ?? new List<GoodsReceiptLineInput>());
        var result = await _handler.PostGoodsReceiptAsync(caller, id, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetPurchaseOrderAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<PurchaseOrderListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] Guid? supplierId, [FromQuery] Guid? projectId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ProcurementHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListPurchaseOrdersAsync(caller, search, status, supplierId, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new PurchaseOrderListResponse(
            paged.Items.Select(i => new PurchaseOrderListItemResponse(i.Id, i.Number, i.Status, i.TotalAmount, i.SupplierCode, i.SupplierNameTh, i.ProjectCode, i.ExpectedDeliveryDate, i.CreatedAtUtc)).ToList(),
            new ProcurementPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private async Task<IActionResult> ActionAsync(Guid id, PurchaseOrderAction action, string? note, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.ActionAsync(caller, id, version, action, note, ct));
    }

    private IActionResult Respond(Result<PurchaseOrderProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static PurchaseOrderInput ToInput(PurchaseOrderRequest r) => new(
        r.SupplierId, r.ProjectId, r.ExpectedDeliveryDate, r.Note,
        r.Lines?.Select(l => new PurchaseOrderLineInput(l.ItemId, l.Quantity, l.UnitPrice)).ToList() ?? new List<PurchaseOrderLineInput>());

    private static ProcurementPersonResponse? ToPerson(ProcurementPerson? p) => p is null ? null : new ProcurementPersonResponse(p.Id, p.DisplayName, p.Email);

    private static PurchaseOrderResponse ToResponse(PurchaseOrderProjection o) => new(
        o.Id, o.BranchId, o.Number, o.Status, o.Currency, o.TotalAmount, o.ExpectedDeliveryDate, o.Note,
        new PurchaseOrderSupplierResponse(o.Supplier.Id, o.Supplier.Code, o.Supplier.NameTh, o.Supplier.NameEn),
        o.Project is null ? null : new PurchaseOrderProjectResponse(o.Project.Id, o.Project.Code, o.Project.Name),
        ToPerson(o.CreatedBy)!, o.CreatedAtUtc, o.SubmittedAtUtc, ToPerson(o.DecidedBy), o.DecidedAtUtc, o.DecisionNote, o.CancelReason, o.RowVersion,
        o.Lines.Select(l => new PurchaseOrderLineResponse(l.Id, l.LineNo, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode, l.Quantity, l.UnitPrice, l.LineTotal, l.ReceivedQuantity, l.RemainingQuantity)).ToList(),
        o.Receipts.Select(r => new GoodsReceiptSummaryResponse(r.Id, r.Number, r.ReceivedAtUtc, r.Note, ToPerson(r.ReceivedBy)!, r.LineCount, r.TotalQuantity)).ToList());
}
