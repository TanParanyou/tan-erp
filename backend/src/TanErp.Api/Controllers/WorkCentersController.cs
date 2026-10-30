using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Production;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Production;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/work-centers")]
[Authorize]
public class WorkCentersController : ProductionControllerBase
{
    private readonly ProductionHandler _handler;

    public WorkCentersController(ProductionHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<WorkCenterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateWorkCenterRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateWorkCenterAsync(
            caller, key,
            new WorkCenterInput(request.Code, request.Name, request.DailyCapacityHours, request.HourlyRate, request.BranchId),
            ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ToResponse(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<WorkCenterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWorkCenterRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateWorkCenterAsync(
            caller, id, version,
            new WorkCenterUpdateInput(request.Name, request.DailyCapacityHours, request.HourlyRate),
            ct));
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType<WorkCenterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetWorkCenterActiveAsync(caller, id, version, true, ct));
    }

    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType<WorkCenterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetWorkCenterActiveAsync(caller, id, version, false, ct));
    }

    [HttpGet("capacity")]
    [ProducesResponseType<WorkCenterCapacityViewResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetCapacity(
        [FromQuery] Guid? branchId, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetCapacityViewAsync(caller, branchId, fromDate, toDate, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var view = result.Value!;
        return Ok(new WorkCenterCapacityViewResponse(
            view.FromDate,
            view.ToDate,
            view.WorkCenters.Select(w => new WorkCenterCapacityRowResponse(
                w.WorkCenterId, w.BranchId, w.Code, w.Name, w.DailyCapacityHours, w.HourlyRate,
                w.TotalCapacityHours, w.TotalPlannedHours, w.UtilizationPercent, w.IsOverloaded,
                w.Days.Select(d => new WorkCenterDailyCapacityPointResponse(
                    d.Date, d.CapacityHours, d.PlannedHours, d.RemainingHours, d.UtilizationPercent, d.IsOverloaded)).ToList())).ToList()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<WorkCenterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetWorkCenterAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<WorkCenterListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] Guid? branchId, [FromQuery] string? status, [FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ProductionHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListWorkCentersAsync(caller, branchId, status, search, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new WorkCenterListResponse(
            paged.Items.Select(ToResponse).ToList(),
            new ProductionPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, paged.PageSize == 0 ? 0 : (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private IActionResult Respond(Result<WorkCenterProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static WorkCenterResponse ToResponse(WorkCenterProjection w) => new(
        w.Id, w.BranchId, w.Code, w.Name, w.DailyCapacityHours, w.HourlyRate, w.Status, w.RowVersion, w.CreatedAtUtc, w.UpdatedAtUtc);
}
