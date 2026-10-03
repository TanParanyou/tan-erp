using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Production;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Production;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/boms")]
[Authorize]
public class BomsController : ProductionControllerBase
{
    private readonly ProductionHandler _handler;

    public BomsController(ProductionHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<BomResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] BomRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new BomInput(request.ItemId, request.OutputQuantity, request.Note, ToLines(request.Lines));
        var result = await _handler.CreateBomAsync(caller, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, ToResponse(result.Value));
    }

    [HttpPost("{id:guid}/revisions")]
    [ProducesResponseType<BomResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateRevision([FromRoute] Guid id, [FromBody] BomDraftRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateRevisionAsync(caller, id, new BomDraftInput(request.OutputQuantity, request.Note, ToLines(request.Lines)), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value!));
    }

    [HttpPut("{id:guid}/revisions/{revisionId:guid}")]
    [ProducesResponseType<BomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateDraft([FromRoute] Guid id, [FromRoute] Guid revisionId, [FromBody] BomDraftRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateDraftAsync(caller, id, revisionId, version, new BomDraftInput(request.OutputQuantity, request.Note, ToLines(request.Lines)), ct));
    }

    [HttpPost("{id:guid}/revisions/{revisionId:guid}/approve")]
    [ProducesResponseType<BomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Approve([FromRoute] Guid id, [FromRoute] Guid revisionId, CancellationToken ct) => ActionAsync(id, revisionId, BomAction.Approve, ct);

    [HttpPost("{id:guid}/revisions/{revisionId:guid}/obsolete")]
    [ProducesResponseType<BomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Obsolete([FromRoute] Guid id, [FromRoute] Guid revisionId, CancellationToken ct) => ActionAsync(id, revisionId, BomAction.Obsolete, ct);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<BomResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetBomAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<BomListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = ProductionHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListBomsAsync(caller, search, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new BomListResponse(
            paged.Items.Select(b => new BomListItemResponse(b.Id, b.Code, ToItem(b.Item), b.ApprovedRevisionNo, b.LatestRevisionNo, b.LatestStatus, b.CreatedAtUtc)).ToList(),
            new ProductionPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize))));
    }

    private async Task<IActionResult> ActionAsync(Guid id, Guid revisionId, BomAction action, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.BomActionAsync(caller, id, revisionId, version, action, ct));
    }

    private IActionResult Respond(Result<BomProjection> result) =>
        result.IsFailure ? Problem(result.Error.Code) : Ok(ToResponse(result.Value!));

    private static List<BomLineInput> ToLines(List<BomLineRequest>? lines) =>
        lines?.Select(l => new BomLineInput(l.ComponentItemId, l.Quantity, l.ScrapPercent)).ToList() ?? new List<BomLineInput>();

    internal static ProductionItemResponse ToItem(ProductionItemRef i) => new(i.Id, i.Code, i.NameTh, i.UnitCode);

    private static ProductionPersonResponse? ToPerson(ProductionPerson? p) => p is null ? null : new ProductionPersonResponse(p.Id, p.DisplayName, p.Email);

    private static BomResponse ToResponse(BomProjection b) => new(
        b.Id, b.Code, ToItem(b.Item), b.CreatedAtUtc,
        b.Revisions.Select(r => new BomRevisionResponse(
            r.Id, r.RevisionNo, r.Status, r.OutputQuantity, r.Note, ToPerson(r.CreatedBy)!, r.CreatedAtUtc, ToPerson(r.ApprovedBy), r.ApprovedAtUtc, r.RowVersion,
            r.Lines.Select(l => new BomLineResponse(l.Id, ToItem(l.Component), l.Quantity, l.ScrapPercent, l.GrossQuantity)).ToList())).ToList());
}
