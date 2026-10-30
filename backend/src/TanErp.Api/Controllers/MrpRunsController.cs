using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Mrp;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Mrp;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/mrp/runs")]
[Authorize]
public class MrpRunsController : MrpControllerBase
{
    private readonly MrpHandler _handler;

    public MrpRunsController(MrpHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<MrpRunResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] MrpRunRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new MrpRunInput(
            request.AsOfDate, request.PurchaseLeadTimeDays, request.ProductionLeadTimeDays, request.IncludeOpenWorkOrders,
            request.Demands?.Select(d => new MrpDemandRequestInput(d.ItemId, d.Quantity, d.NeedBy, d.Reference, d.UnitId)).ToList() ?? new List<MrpDemandRequestInput>());
        var result = await _handler.CreateRunAsync(caller, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, ToResponse(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<MrpRunResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetRunAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<MrpRunListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = MrpHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListRunsAsync(caller, search, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new MrpRunListResponse(
            paged.Items.Select(r => new MrpRunListItemResponse(r.Id, r.Number, r.AsOfDate, r.RecommendationCount, r.ShortageCount, r.OpenCount, r.CreatedAtUtc)).ToList(),
            new MrpPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    [HttpPost("{id:guid}/recommendations/{recommendationId:guid}/approve")]
    [ProducesResponseType<MrpRunResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Approve([FromRoute] Guid id, [FromRoute] Guid recommendationId, CancellationToken ct) => DecideAsync(id, recommendationId, true, ct);

    [HttpPost("{id:guid}/recommendations/{recommendationId:guid}/reject")]
    [ProducesResponseType<MrpRunResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Reject([FromRoute] Guid id, [FromRoute] Guid recommendationId, CancellationToken ct) => DecideAsync(id, recommendationId, false, ct);

    [HttpPost("{id:guid}/recommendations/{recommendationId:guid}/convert")]
    [ProducesResponseType<MrpRunResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Convert([FromRoute] Guid id, [FromRoute] Guid recommendationId, [FromBody] MrpConvertRequest? request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var input = request is null ? null : new MrpConvertInput(request.SupplierId, request.UnitPrice, request.WarehouseId);
        return Respond(await _handler.ConvertAsync(caller, id, recommendationId, version, input, ct));
    }

    [HttpGet("/api/v1/mrp/planning-parameters")]
    [ProducesResponseType<MrpPlanningParameterListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPlanningParameters(
        [FromQuery] Guid? itemId,
        [FromQuery] Guid? warehouseId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = MrpHandler.DefaultPageSize,
        CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListPlanningParametersAsync(caller, itemId, warehouseId, search, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new MrpPlanningParameterListResponse(
            paged.Items.Select(ToPlanningParameterResponse).ToList(),
            new MrpPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    [HttpPut("/api/v1/mrp/planning-parameters")]
    [ProducesResponseType<MrpPlanningParameterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpsertPlanningParameter([FromBody] MrpPlanningParameterRequest? request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var input = request is null
            ? null
            : new MrpPlanningParameterInput(
                request.ItemId,
                request.WarehouseId,
                request.SafetyStock,
                request.MinOrderQuantity,
                request.LotMultiple,
                request.LeadTimeDays);
        var result = await _handler.UpsertPlanningParameterAsync(caller, input, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(ToPlanningParameterResponse(result.Value!));
    }

    [HttpDelete("/api/v1/mrp/planning-parameters/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlanningParameter([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.DeletePlanningParameterAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : NoContent();
    }

    [HttpGet("/api/v1/mrp/calendar-holidays")]
    [ProducesResponseType<WorkingCalendarHolidayListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ListHolidays(
        [FromQuery] Guid? branchId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListHolidaysAsync(caller, branchId, fromDate, toDate, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return Ok(new WorkingCalendarHolidayListResponse(result.Value!.Select(ToHolidayResponse).ToList()));
    }

    [HttpPost("/api/v1/mrp/calendar-holidays")]
    [ProducesResponseType<WorkingCalendarHolidayResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateHoliday([FromBody] WorkingCalendarHolidayRequest? request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = request is null ? null : new WorkingCalendarHolidayInput(request.HolidayDate, request.Name, request.BranchId);
        var result = await _handler.CreateHolidayAsync(caller, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return StatusCode(StatusCodes.Status201Created, ToHolidayResponse(result.Value!));
    }

    [HttpDelete("/api/v1/mrp/calendar-holidays/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHoliday([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.DeleteHolidayAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : NoContent();
    }

    private async Task<IActionResult> DecideAsync(Guid id, Guid recommendationId, bool approve, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DecideAsync(caller, id, recommendationId, version, approve, ct));
    }

    private IActionResult Respond(Result<MrpRunProjection> result) =>
        result.IsFailure ? Problem(result.Error.Code) : Ok(ToResponse(result.Value!));

    private static MrpPersonResponse? ToPerson(MrpPerson? p) => p is null ? null : new MrpPersonResponse(p.Id, p.DisplayName, p.Email);

    private static MrpRunResponse ToResponse(MrpRunProjection r) => new(
        r.Id, r.Number, r.AsOfDate, r.PurchaseLeadTimeDays, r.ProductionLeadTimeDays, r.InputHash,
        new MrpSnapshotSummaryResponse(r.Snapshot.DemandCount, r.Snapshot.SupplyCount, r.Snapshot.BomCount, r.Snapshot.StockItemCount),
        ToPerson(r.CreatedBy)!, r.CreatedAtUtc,
        r.Recommendations.Select(x => new MrpRecommendationResponse(
            x.Id, x.LineNo, new MrpItemResponse(x.Item.Id, x.Item.Code, x.Item.NameTh, x.Item.UnitCode), x.Action, x.Quantity, x.NeedBy, x.OrderBy, x.Level,
            x.GrossRequirement, x.StockUsed, x.ScheduledReceiptsUsed,
            x.Reasons.Select(re => new MrpReasonResponse(re.SourceType, re.SourceRef, re.Quantity, re.NeedBy)).ToList(), x.Status,
            ToPerson(x.DecidedBy), x.DecidedAtUtc, x.Converted is null ? null : new MrpConvertedResponse(x.Converted.Type, x.Converted.Id, x.Converted.Number), x.RowVersion)).ToList());

    private static MrpPlanningParameterResponse ToPlanningParameterResponse(MrpPlanningParameterProjection p) => new(
        p.Id,
        new MrpItemResponse(p.Item.Id, p.Item.Code, p.Item.NameTh, p.Item.UnitCode),
        p.Warehouse is null ? null : new MrpWarehouseResponse(p.Warehouse.Id, p.Warehouse.Code, p.Warehouse.Name),
        p.SafetyStock,
        p.MinOrderQuantity,
        p.LotMultiple,
        p.LeadTimeDays,
        p.UpdatedAtUtc,
        p.RowVersion);

    private static WorkingCalendarHolidayResponse ToHolidayResponse(WorkingCalendarHolidayProjection h) => new(
        h.Id,
        h.BranchId,
        h.HolidayDate,
        h.Name,
        h.CreatedAtUtc);
}
