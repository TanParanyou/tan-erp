using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Procurement;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/purchase-requests")]
[Authorize]
public class PurchaseRequestsController : ProcurementControllerBase
{
    private readonly ProcurementHandler _handler;

    public PurchaseRequestsController(ProcurementHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] PurchaseRequestRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreatePurchaseRequestAsync(caller, key, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PurchaseRequestRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdatePurchaseRequestAsync(caller, id, version, ToInput(request), ct));
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct) =>
        ActionAsync(id, PurchaseRequestAction.Submit, null, ct);

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] PurchaseRequestActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, PurchaseRequestAction.Approve, request?.Note, ct);

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] PurchaseRequestActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, PurchaseRequestAction.Reject, request?.Note, ct);

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] PurchaseRequestActionRequest? request, CancellationToken ct) =>
        ActionAsync(id, PurchaseRequestAction.Cancel, request?.Note, ct);

    [HttpPost("convert")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Convert([FromBody] ConvertPurchaseRequestsRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new ConvertPurchaseRequestsInput(
            request.PurchaseRequestIds ?? new List<Guid>(),
            request.SupplierId,
            request.ExpectedDeliveryDate,
            request.Note,
            request.UnitPriceOverrides?.Select(o => new PurchaseRequestUnitPriceOverrideInput(o.ItemId, o.UnitPrice)).ToList());
        var result = await _handler.ConvertPurchaseRequestsAsync(caller, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(
            nameof(PurchaseOrdersController.Get),
            "PurchaseOrders",
            new { id = result.Value.Id },
            PurchaseOrdersController.ToResponse(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PurchaseRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetPurchaseRequestAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<PurchaseRequestListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = ProcurementHandler.DefaultPageSize,
        CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListPurchaseRequestsAsync(caller, search, status, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new PurchaseRequestListResponse(
            paged.Items.Select(i => new PurchaseRequestListItemResponse(
                i.Id, i.BranchId, i.Number, i.Status, i.TotalEstimatedAmount, i.ProjectCode, i.RequiredByDate,
                i.ConvertedPurchaseOrderId, i.ConvertedPurchaseOrderNumber, i.CreatedAtUtc)).ToList(),
            new ProcurementPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private async Task<IActionResult> ActionAsync(Guid id, PurchaseRequestAction action, string? note, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.PurchaseRequestActionAsync(caller, id, version, action, note, ct));
    }

    private IActionResult Respond(Result<PurchaseRequestProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static PurchaseRequestInput ToInput(PurchaseRequestRequest r) => new(
        r.ProjectId,
        r.RequiredByDate,
        r.Note,
        r.Lines?.Select(l => new PurchaseRequestLineInput(l.ItemId, l.Quantity, l.EstimatedUnitPrice, l.Note)).ToList() ?? new List<PurchaseRequestLineInput>());

    private static PurchaseRequestResponse ToResponse(PurchaseRequestProjection r) => new(
        r.Id,
        r.BranchId,
        r.Number,
        r.Status,
        r.Currency,
        r.TotalEstimatedAmount,
        r.RequiredByDate,
        r.Note,
        r.Project is null ? null : new PurchaseOrderProjectResponse(r.Project.Id, r.Project.Code, r.Project.Name),
        PurchaseOrdersController.ToPerson(r.CreatedBy)!,
        r.CreatedAtUtc,
        r.SubmittedAtUtc,
        PurchaseOrdersController.ToPerson(r.DecidedBy),
        r.DecidedAtUtc,
        r.DecisionNote,
        r.CancelReason,
        r.ConvertedPurchaseOrder is null ? null : new PurchaseRequestConvertedOrderResponse(r.ConvertedPurchaseOrder.Id, r.ConvertedPurchaseOrder.Number, r.ConvertedPurchaseOrder.Status),
        r.ConvertedAtUtc,
        r.RowVersion,
        r.Lines.Select(l => new PurchaseRequestLineResponse(
            l.Id, l.LineNo, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode,
            l.Quantity, l.EstimatedUnitPrice, l.EstimatedLineTotal, l.Note)).ToList());
}
