using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Common;
using TanErp.Api.Contracts.Notifications;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Notifications;

namespace TanErp.Api.Controllers;

/// <summary>The caller's own in-app notifications. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationHandler _handler;

    public NotificationsController(NotificationHandler handler)
    {
        _handler = handler;
    }

    private NotificationCaller? ReadCaller(out IActionResult? failure)
    {
        var authenticated = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authenticated.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(authenticated.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        return new NotificationCaller(authenticated.Value!.FirebaseUid, authenticated.Value.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    private static NotificationResponse To(NotificationProjection p) => new(p.Id, p.Type, p.Payload, p.DeepLink, p.CreatedAtUtc, p.ReadAtUtc);

    [HttpGet]
    [ProducesResponseType<NotificationListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = NotificationLimits.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, unreadOnly, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        var paged = result.Value!;
        return Ok(new NotificationListResponse(
            paged.Items.Select(To).ToList(),
            new PaginationMetadataResponse(paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages)));
    }

    [HttpGet("unread-count")]
    [ProducesResponseType<UnreadCountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CountUnreadAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new UnreadCountResponse(result.Value));
    }

    [HttpPost("{id:guid}/read")]
    [ProducesResponseType<NotificationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.MarkReadAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(To(result.Value!));
    }

    [HttpPost("read-all")]
    [ProducesResponseType<MarkAllReadResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.MarkAllReadAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new MarkAllReadResponse(result.Value));
    }
}
