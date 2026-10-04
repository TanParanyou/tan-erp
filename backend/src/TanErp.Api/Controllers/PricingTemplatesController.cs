using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.QuickEstimates;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/pricing-templates")]
[Authorize]
public class PricingTemplatesController : QuickEstimateControllerBase
{
    private readonly QuickEstimateHandler _handler;

    public PricingTemplatesController(QuickEstimateHandler handler)
    {
        _handler = handler;
    }

    internal static TemplateInput ToInput(PricingTemplateRequest r) => new(
        r.Code ?? string.Empty, r.WorkType ?? string.Empty, r.Name ?? string.Empty, r.MeasurementRule ?? string.Empty, r.UnitCode ?? string.Empty,
        r.ReferenceRate, r.MinimumCharge, r.BaseRangeRate, r.MaxRangeRate, r.RoundingStep, r.ValidityDays, r.TaxRate, r.TaxDisplay ?? string.Empty, r.DirectShareLimit,
        r.EffectiveFrom, r.EffectiveTo,
        r.Grades?.Select(g => new GradeInput(g.Code ?? string.Empty, g.Name ?? string.Empty, g.Factor)).ToList() ?? new List<GradeInput>(),
        r.Complexities?.Select(c => new ComplexityInput(c.Code ?? string.Empty, c.Name ?? string.Empty, c.Factor, c.RiskModifier)).ToList() ?? new List<ComplexityInput>(),
        r.AddOns?.Select(a => new AddOnInput(a.Code ?? string.Empty, a.Name ?? string.Empty, a.Amount, a.PerLine)).ToList() ?? new List<AddOnInput>(),
        r.LowConfidenceModifier, r.MediumConfidenceModifier, r.CustomMaterialModifier,
        r.Assumptions ?? new List<string>(), r.Exclusions ?? new List<string>());

    [HttpPost]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] PricingTemplateRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateTemplateAsync(caller, key, ToInput(request), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, QuickEstimateMapper.To(result.Value));
    }

    [HttpPost("{id:guid}/versions")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> NewVersion([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.NewTemplateVersionAsync(caller, id, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, QuickEstimateMapper.To(result.Value));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PricingTemplateRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateTemplateAsync(caller, id, version, ToInput(request), ct));
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, TemplateAction.Submit, ct);

    [HttpPost("{id:guid}/decision")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Decide([FromRoute] Guid id, [FromBody] TemplateDecisionRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DecideTemplateAsync(caller, id, version, request.Decision, request.Note, ct));
    }

    [HttpPost("{id:guid}/calibration")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Calibrate([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, TemplateAction.Calibrate, ct);

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, TemplateAction.Activate, ct);

    [HttpPost("{id:guid}/disable")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Disable([FromRoute] Guid id, CancellationToken ct) => ActionAsync(id, TemplateAction.Disable, ct);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PricingTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetTemplateAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<PricingTemplateListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? workType, [FromQuery] int page = 1, [FromQuery] int pageSize = QuickEstimateHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListTemplatesAsync(caller, search, status, workType, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new PricingTemplateListResponse(paged.Items.Select(QuickEstimateMapper.To).ToList(), QuickEstimateMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    /// <summary>Templates a capture form can price with today. Rates and factors are not included.</summary>
    [HttpGet("effective")]
    [ProducesResponseType<List<EffectiveTemplateResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Effective([FromQuery] string? workType, [FromQuery] DateOnly? at, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListEffectiveAsync(caller, workType, at, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(result.Value!.Select(QuickEstimateMapper.To).ToList());
    }

    private async Task<IActionResult> ActionAsync(Guid id, TemplateAction action, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.TemplateActionAsync(caller, id, version, action, ct));
    }

    private IActionResult Respond(Result<PricingTemplateProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(QuickEstimateMapper.To(result.Value));
    }
}
