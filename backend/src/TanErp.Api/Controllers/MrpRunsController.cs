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
            request.Demands?.Select(d => new MrpDemandRequestInput(d.ItemId, d.Quantity, d.NeedBy, d.Reference)).ToList() ?? new List<MrpDemandRequestInput>());
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
}
