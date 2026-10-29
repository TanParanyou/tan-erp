using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Procurement;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/supplier-invoices")]
[Authorize]
public class SupplierInvoicesController : ProcurementControllerBase
{
    private readonly ProcurementHandler _handler;

    public SupplierInvoicesController(ProcurementHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] SupplierInvoiceRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateSupplierInvoiceAsync(caller, key, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SupplierInvoiceRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateSupplierInvoiceAsync(caller, id, version, ToInput(request), ct));
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct) =>
        ActionAsync(id, SupplierInvoiceAction.Submit, null, ct);

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] SupplierInvoiceActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, SupplierInvoiceAction.Approve, request?.Note, ct);

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] SupplierInvoiceActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, SupplierInvoiceAction.Reject, request?.Note, ct);

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] SupplierInvoiceActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, SupplierInvoiceAction.Cancel, request?.Note, ct);

    [HttpPost("{id:guid}/void")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Void([FromRoute] Guid id, [FromBody] SupplierInvoiceActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, SupplierInvoiceAction.Void, request?.Note, ct);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SupplierInvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetSupplierInvoiceAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<SupplierInvoiceListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? supplierId,
        [FromQuery] Guid? purchaseOrderId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = ProcurementHandler.DefaultPageSize,
        CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListSupplierInvoicesAsync(caller, search, status, supplierId, purchaseOrderId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new SupplierInvoiceListResponse(
            paged.Items.Select(i => new SupplierInvoiceListItemResponse(
                i.Id, i.Number, i.SupplierInvoiceNumber, i.Status, i.MatchStatus, i.PurchaseOrderId, i.PurchaseOrderNumber,
                i.SupplierCode, i.SupplierNameTh, i.ProjectCode, i.InvoiceDate, i.DueDate, i.SubtotalAmount, i.TaxAmount,
                i.TotalAmount, i.TotalVarianceAmount, i.CreatedAtUtc)).ToList(),
            new ProcurementPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private async Task<IActionResult> ActionAsync(Guid id, SupplierInvoiceAction action, string? note, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SupplierInvoiceActionAsync(caller, id, version, action, note, ct));
    }

    private IActionResult Respond(Result<SupplierInvoiceProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static SupplierInvoiceInput ToInput(SupplierInvoiceRequest r) => new(
        r.PurchaseOrderId,
        r.SupplierInvoiceNumber,
        r.InvoiceDate,
        r.DueDate,
        r.TaxAmount,
        r.Note,
        r.Lines?.Select(l => new SupplierInvoiceLineInput(l.PurchaseOrderLineId, l.Quantity, l.UnitPrice)).ToList() ?? new List<SupplierInvoiceLineInput>());

    private static SupplierInvoiceResponse ToResponse(SupplierInvoiceProjection i) => new(
        i.Id,
        i.BranchId,
        i.Number,
        i.SupplierInvoiceNumber,
        i.Status,
        i.MatchStatus,
        i.Currency,
        i.InvoiceDate,
        i.DueDate,
        i.SubtotalAmount,
        i.TaxAmount,
        i.TotalAmount,
        i.TotalPoAmount,
        i.TotalVarianceAmount,
        i.Note,
        new SupplierInvoiceOrderRefResponse(i.PurchaseOrder.Id, i.PurchaseOrder.Number, i.PurchaseOrder.Status),
        new PurchaseOrderSupplierResponse(i.Supplier.Id, i.Supplier.Code, i.Supplier.NameTh, i.Supplier.NameEn),
        i.Project is null ? null : new PurchaseOrderProjectResponse(i.Project.Id, i.Project.Code, i.Project.Name),
        PurchaseOrdersController.ToPerson(i.CreatedBy)!,
        i.CreatedAtUtc,
        i.SubmittedAtUtc,
        i.PostedAtUtc,
        PurchaseOrdersController.ToPerson(i.DecidedBy),
        i.DecidedAtUtc,
        i.DecisionNote,
        i.CancelReason,
        i.VoidReason,
        PurchaseOrdersController.ToPerson(i.VoidedBy),
        i.VoidedAtUtc,
        i.RowVersion,
        i.Lines.Select(l => new SupplierInvoiceLineResponse(
            l.Id, l.LineNo, l.PurchaseOrderLineId, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode,
            l.Quantity, l.PoUnitPrice, l.UnitPrice, l.LineTotal, l.PriceVarianceAmount, l.PriceVariancePercent, l.ExceedsTolerance)).ToList());
}
