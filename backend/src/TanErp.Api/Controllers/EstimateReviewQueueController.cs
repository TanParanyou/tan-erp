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
[Route("api/v1/estimates/review-queue")]
public sealed class EstimateReviewQueueController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateReviewQueueReader _reader;

    public EstimateReviewQueueController(IRequestAccessResolver accessResolver, IEstimateReviewQueueReader reader)
    {
        _accessResolver = accessResolver;
        _reader = reader;
    }

    [HttpGet]
    [ProducesResponseType<EstimateReviewQueueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (pageNumber < 1 || pageSize is < 1 or > 100)
            return ProblemDetailsMapper.CreateProblemResult("REQUEST_VALIDATION_FAILED", HttpContext);

        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid, auth.MembershipId, "estimates.approve", ct);
        if (accessResult.IsFailure)
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var page = await _reader.ListAsync(accessResult.Value!.OrganizationId, auth.MembershipId,
            search, pageNumber, pageSize, ct);
        return Ok(EstimateReviewQueueResponseMapper.ToResponse(page));
    }
}
