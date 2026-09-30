using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Api.Contracts.Items;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/cost-review-queue")]
public sealed class CostReviewQueueController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICostReviewQueueReader _reader;

    public CostReviewQueueController(IRequestAccessResolver accessResolver, ICostReviewQueueReader reader)
    {
        _accessResolver = accessResolver;
        _reader = reader;
    }

    [HttpGet]
    [ProducesResponseType<CostReviewQueueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (pageNumber < 1 || pageSize is < 1 or > 100)
            return ProblemDetailsMapper.CreateProblemResult("REQUEST_VALIDATION_FAILED", HttpContext);
        if (!string.IsNullOrWhiteSpace(status) && status.Trim().ToLowerInvariant() is not ("submitted" or "approved"))
            return ProblemDetailsMapper.CreateProblemResult("REQUEST_VALIDATION_FAILED", HttpContext);
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.read", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var page = await _reader.ListAsync(accessResult.Value!.OrganizationId, status, search, pageNumber, pageSize, ct);
        return Ok(CostReviewQueueResponseMapper.ToResponse(page));
    }
}
