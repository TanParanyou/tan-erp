using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.QuickEstimates;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/quick-estimates")]
[Authorize]
public class QuickEstimatesController : QuickEstimateControllerBase
{
    private readonly QuickEstimateHandler _handler;

    public QuickEstimatesController(QuickEstimateHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateQuickEstimateRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateAsync(caller, key, request.CustomerId, request.OpportunityId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, QuickEstimateMapper.To(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<QuickEstimateListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = QuickEstimateHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, search, status, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new QuickEstimateListResponse(paged.Items.Select(QuickEstimateMapper.To).ToList(), QuickEstimateMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    /// <summary>Autosave of the draft: only the fields present are changed.</summary>
    [HttpPatch("{id:guid}/draft")]
    [Consumes("application/json", "application/merge-patch+json")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] QuickEstimateDraftRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var patch = new QuickEstimateDraftPatch(
            request.TemplateId, request.PropertyType, request.RoomOrArea, request.GradeCode, request.ComplexityCodes, request.AddOnCodes, request.MeasurementConfidence, request.CustomMaterial,
            request.Measurements?.Select(m => new MeasurementInput(m.LineId, m.WorkSubtype, m.WidthM, m.HeightM, m.DepthM, m.Quantity)).ToList());
        return Respond(await _handler.PatchAsync(caller, id, version, patch, ct));
    }

    [HttpPost("{id:guid}/calculate")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Calculate([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.CalculateAsync(caller, id, version, ct));
    }

    [HttpGet("{id:guid}/calculations/{version:int}/snapshot")]
    [ProducesResponseType<CalculationSnapshotResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Snapshot([FromRoute] Guid id, [FromRoute] int version, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetSnapshotAsync(caller, id, version, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new CalculationSnapshotResponse(version, result.Value!));
    }

    [HttpPost("{id:guid}/submit-review")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitReview([FromRoute] Guid id, [FromBody] SubmitReviewRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SubmitReviewAsync(caller, id, request.SourceVersion, request.Note, ct));
    }

    [HttpPost("{id:guid}/review-decisions")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DecideReview([FromRoute] Guid id, [FromBody] ReviewDecisionRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DecideReviewAsync(caller, id, request.SourceVersion, request.Decision, request.ReasonCode, request.Note, ct));
    }

    [HttpPost("{id:guid}/shares")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Share([FromRoute] Guid id, [FromBody] ShareRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ShareAsync(caller, id, key, new ShareInput(request.SourceVersion, request.Channel ?? string.Empty, request.Recipient, request.Locale ?? string.Empty), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, QuickEstimateMapper.To(result.Value));
    }

    [HttpPost("{id:guid}/conversion")]
    [ProducesResponseType<QuickEstimateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Convert([FromRoute] Guid id, [FromBody] ConversionRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ConvertAsync(caller, id, key, new ConversionInput(request.SourceVersion, request.SiteSurveyRevisionId), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, QuickEstimateMapper.To(result.Value));
    }

    private IActionResult Respond(Result<QuickEstimateProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(QuickEstimateMapper.To(result.Value));
    }
}
