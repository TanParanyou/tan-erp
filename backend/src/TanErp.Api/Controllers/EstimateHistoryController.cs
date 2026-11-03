using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Estimates;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/estimates/{id:guid}/history")]
public sealed class EstimateHistoryController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateHistoryReader _reader;

    public EstimateHistoryController(IRequestAccessResolver accessResolver, IEstimateHistoryReader reader)
    {
        _accessResolver = accessResolver;
        _reader = reader;
    }

    /// <summary>Revision summaries and recent events for an estimate; needs estimates.read, not the organization-wide audit.read.</summary>
    [HttpGet]
    [ProducesResponseType<EstimateHistoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "estimates.read", ct);
        if (accessResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var history = await _reader.GetAsync(access.OrganizationId, id, ct);
        if (history is null || !access.HasBranchAccess(history.BranchId))
            return ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext);
        return Ok(EstimateHistoryResponse.FromProjection(history));
    }

    /// <summary>Work item lines of one revision, for comparing two revisions.</summary>
    [HttpGet("revisions/{revisionId:guid}/lines")]
    [ProducesResponseType<EstimateRevisionLinesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRevisionLines([FromRoute] Guid id, [FromRoute] Guid revisionId, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "estimates.read", ct);
        if (accessResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var lines = await _reader.GetRevisionLinesAsync(access.OrganizationId, id, revisionId, ct);
        if (lines is null || !access.HasBranchAccess(lines.BranchId))
            return ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext);
        return Ok(EstimateRevisionLinesResponse.FromProjection(lines));
    }
}
