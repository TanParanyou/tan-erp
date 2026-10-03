using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Production;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Production;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/work-orders")]
[Authorize]
public class WorkOrdersController : ProductionControllerBase
{
    private readonly ProductionHandler _handler;

    public WorkOrdersController(ProductionHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] WorkOrderRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateWorkOrderAsync(caller, key, new WorkOrderInput(request.ItemId, request.WarehouseId, request.ProjectId, request.PlannedQuantity, request.Note), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPost("{id:guid}/release")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Release([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, WorkOrderAction.Release, null, ct);

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] WorkOrderActionRequest? request, CancellationToken ct) => ActionAsync(id, WorkOrderAction.Cancel, request?.Reason, ct);

    [HttpPost("{id:guid}/issues")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Issue([FromRoute] Guid id, [FromBody] WorkOrderMaterialsRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        return Created(await _handler.IssueAsync(caller, id, key, ToLines(request.Lines), ct));
    }

    [HttpPost("{id:guid}/returns")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Return([FromRoute] Guid id, [FromBody] WorkOrderMaterialsRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        return Created(await _handler.ReturnAsync(caller, id, key, ToLines(request.Lines), ct));
    }

    [HttpPost("{id:guid}/completions")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Complete([FromRoute] Guid id, [FromBody] WorkOrderCompleteRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        return Created(await _handler.CompleteAsync(caller, id, key, request.Quantity, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetWorkOrderAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<WorkOrderListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] Guid? projectId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ProductionHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListWorkOrdersAsync(caller, search, status, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new WorkOrderListResponse(
            paged.Items.Select(o => new WorkOrderListItemResponse(o.Id, o.Number, o.Status, o.ItemCode, o.ItemNameTh, o.PlannedQuantity, o.CompletedQuantity, o.ProjectCode, o.CreatedAtUtc)).ToList(),
            new ProductionPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private async Task<IActionResult> ActionAsync(Guid id, WorkOrderAction action, string? reason, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.WorkOrderActionAsync(caller, id, version, action, reason, ct));
    }

    private IActionResult Respond(Result<WorkOrderProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private IActionResult Created(Result<WorkOrderProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value));
    }

    private static List<WorkOrderMaterialQuantityInput> ToLines(List<WorkOrderMaterialLineRequest>? lines) =>
        lines?.Select(l => new WorkOrderMaterialQuantityInput(l.ItemId, l.Quantity)).ToList() ?? new List<WorkOrderMaterialQuantityInput>();

    private static WorkOrderResponse ToResponse(WorkOrderProjection o) => new(
        o.Id, o.BranchId, o.Number, o.Status, BomsController.ToItem(o.Item), o.BomRevisionId, o.BomCode, o.BomRevisionNo,
        new WorkOrderRefResponse(o.Warehouse.Id, o.Warehouse.Code, o.Warehouse.Name),
        o.Project is null ? null : new WorkOrderRefResponse(o.Project.Id, o.Project.Code, o.Project.Name),
        o.PlannedQuantity, o.CompletedQuantity, o.CostAllocated, o.Note, o.CancelReason,
        new ProductionPersonResponse(o.CreatedBy.Id, o.CreatedBy.DisplayName, o.CreatedBy.Email), o.CreatedAtUtc, o.RowVersion,
        o.Materials.Select(m => new WorkOrderMaterialResponse(m.Id, BomsController.ToItem(m.Item), m.RequiredQuantity, m.IssuedQuantity, m.ReturnedQuantity, m.NetIssuedQuantity, m.RemainingQuantity, m.IssuedValue, m.ReturnedValue)).ToList(),
        o.Transactions.Select(t => new WorkOrderTransactionResponse(t.Id, t.Kind, t.StockDocumentId, t.StockDocumentNumber, t.Quantity, t.Value, new ProductionPersonResponse(t.Actor.Id, t.Actor.DisplayName, t.Actor.Email), t.CreatedAtUtc)).ToList());
}
