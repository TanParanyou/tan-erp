using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Audit;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Audit;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Api.Controllers;

/// <summary>Read-only audit trail (ADR 0027 D-B/B1). Requires audit.read; always organization-scoped.</summary>
[ApiController]
[Authorize]
[Route("api/v1/audit-events")]
public sealed class AuditEventsController : ControllerBase
{
    private readonly AuditEventHandler _handler;

    public AuditEventsController(AuditEventHandler handler) => _handler = handler;

    [HttpGet]
    [ProducesResponseType<AuditEventListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] string? resourceType = null, [FromQuery] string? resourceId = null, [FromQuery] string? action = null,
        [FromQuery] Guid? actorUserId = null, [FromQuery] DateTimeOffset? from = null, [FromQuery] DateTimeOffset? to = null,
        [FromQuery] int page = 1, [FromQuery] int limit = 25, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);

        var result = await _handler.ListAsync(
            new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), resourceType, resourceId, action, actorUserId,
            from, to, page, limit, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(AuditEventListResponse.From(result.Value!));
    }

    [HttpPost("exports")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RecordExport(
        [FromBody] RecordExportAuditRequest request, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);

        if (string.IsNullOrWhiteSpace(request.ResourceType))
            return ProblemDetailsMapper.CreateProblemResult("REQUEST_VALIDATION_FAILED", HttpContext);

        var command = new RecordExportAuditCommand(
            request.ResourceType,
            Math.Max(request.RowCount, 0),
            string.IsNullOrWhiteSpace(request.Format) ? "csv" : request.Format.Trim().ToLowerInvariant(),
            request.Filename,
            request.FilterSummary,
            HttpContext.TraceIdentifier);

        var result = await _handler.RecordExportAsync(
            new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), command, cancellationToken);

        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : NoContent();
    }
}
