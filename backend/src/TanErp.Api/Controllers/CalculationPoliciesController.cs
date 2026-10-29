using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Estimates.CalculationPolicies;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/calculation-policies")]
[Authorize]
public sealed class CalculationPoliciesController : ControllerBase
{
    private readonly ManageCalculationPoliciesHandler _handler;

    public CalculationPoliciesController(ManageCalculationPoliciesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [ProducesResponseType<CalculationPolicyListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] Guid? branchId, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _handler.ListAsync(auth.FirebaseUid, auth.MembershipId, branchId, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Select(CalculationPolicyVersionResponse.From).ToList();
        return Ok(new CalculationPolicyListResponse(items));
    }

    [HttpPost]
    [ProducesResponseType<CalculationPolicyVersionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateDraft(
        [FromBody] CreateCalculationPolicyRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var command = new CreateCalculationPolicyCommand(
            request.BranchId,
            request.PolicyCode,
            request.Version,
            request.OverheadMethod,
            request.OverheadValue,
            request.RoundingMode,
            request.UnitConversionMode,
            request.EffectiveFromUtc,
            request.EffectiveToUtc);

        var result = await _handler.CreateDraftAsync(auth.FirebaseUid, auth.MembershipId, command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = CalculationPolicyVersionResponse.From(result.Value!);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType<CalculationPolicyVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _handler.PublishAsync(auth.FirebaseUid, auth.MembershipId, id, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(CalculationPolicyVersionResponse.From(result.Value!));
    }
}
