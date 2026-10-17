using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.QuickEstimates;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Controllers;

/// <summary>Branch-specific rate overrides of a pricing template. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/pricing-templates/{templateId:guid}/branch-rate-overrides")]
[Authorize]
public class BranchRateOverridesController : QuickEstimateControllerBase
{
    private readonly BranchRateOverrideHandler _handler;

    public BranchRateOverridesController(BranchRateOverrideHandler handler)
    {
        _handler = handler;
    }

    private IActionResult Respond(Result<BranchOverrideProjection> result, int status = StatusCodes.Status200OK)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(status, BranchRateOverrideMapper.To(result.Value));
    }

    [HttpPost]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status201Created), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RequestOverride([FromRoute] Guid templateId, [FromBody] BranchRateOverrideRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new BranchOverrideInput(request.ReferenceRate, request.MinimumCharge, request.EffectiveFrom, request.EffectiveTo, request.ReasonCode, request.ReasonDetail, request.SourceReference);
        return Respond(await _handler.RequestAsync(caller, templateId, key, input, ct), StatusCodes.Status201Created);
    }

    [HttpGet]
    [ProducesResponseType<BranchRateOverrideListResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromRoute] Guid templateId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, templateId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new BranchRateOverrideListResponse(result.Value!.Select(BranchRateOverrideMapper.To).ToList()));
    }

    [HttpPost("{overrideId:guid}/decision")]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Decide([FromRoute] Guid templateId, [FromRoute] Guid overrideId, [FromBody] BranchRateOverrideDecisionRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DecideAsync(caller, templateId, overrideId, version, request.Decision, request.Note, ct));
    }

    [HttpPost("{overrideId:guid}/revoke")]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Revoke([FromRoute] Guid templateId, [FromRoute] Guid overrideId, [FromBody] BranchRateOverrideRevokeRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.RevokeAsync(caller, templateId, overrideId, version, request.Reason, ct));
    }
}
