using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.ApprovalLimits;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.ApprovalLimits;

namespace TanErp.Api.Controllers;

/// <summary>Approval limit administration (G-03c). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin/approval-limits")]
public sealed class AdminApprovalLimitsController : ControllerBase
{
    private readonly ApprovalLimitHandler _handler;

    public AdminApprovalLimitsController(ApprovalLimitHandler handler) => _handler = handler;

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    private IActionResult LimitResult(Result<ApprovalLimitView> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ApprovalLimitResponse.From(result.Value!));
    }

    [HttpGet]
    [ProducesResponseType<ApprovalLimitListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] Guid? roleId = null, [FromQuery] string? documentType = null, [FromQuery] string? status = null,
        [FromQuery] int page = 1, [FromQuery] int limit = 25, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListAsync(
            new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), roleId, documentType, status, page, limit, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(ApprovalLimitListResponse.From(result.Value!));
    }

    [HttpGet("options")]
    [ProducesResponseType<ApprovalLimitOptionsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.GetOptionsAsync(new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(ApprovalLimitOptionsResponse.From(result.Value!));
    }

    [HttpGet("{limitId:guid}")]
    [ProducesResponseType<ApprovalLimitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid limitId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        return LimitResult(await _handler.GetAsync(new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), limitId, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<ApprovalLimitResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateApprovalLimitRequest request, CancellationToken cancellationToken)
    {
        var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (idempotent.IsFailure) return Problem(idempotent.Error);

        var result = await _handler.CreateAsync(
            new AdminCaller(idempotent.Value!.FirebaseUid, idempotent.Value.MembershipId), idempotent.Value.IdempotencyKey,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Created($"/api/v1/admin/approval-limits/{result.Value.Id}", ApprovalLimitResponse.From(result.Value));
    }

    [HttpPut("{limitId:guid}")]
    [ProducesResponseType<ApprovalLimitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid limitId, [FromBody] UpdateApprovalLimitRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        return LimitResult(await _handler.UpdateAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), limitId, conditional.Value.IfMatchRowVersion,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken));
    }
}
