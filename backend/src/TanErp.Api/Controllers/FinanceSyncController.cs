using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Finance;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Finance;

namespace TanErp.Api.Controllers;

/// <summary>Accounting hand-off: the outbox, delivery to the (not yet configured) connector, confirmations and reconciliation.</summary>
[ApiController]
[Route("api/v1/finance")]
[Authorize]
public class FinanceSyncController : FinanceControllerBase
{
    private readonly FinanceHandler _handler;

    public FinanceSyncController(FinanceHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("outbox")]
    [ProducesResponseType<OutboxListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] string? kind, [FromQuery] int page = 1, [FromQuery] int pageSize = FinanceHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListOutboxAsync(caller, status, kind, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new OutboxListResponse(paged.Items.Select(FinanceMapper.To).ToList(), FinanceMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpPost("outbox/dispatch")]
    [ProducesResponseType<DispatchResultResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Dispatch([FromQuery] int? batchSize, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.DispatchAsync(caller, batchSize, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(FinanceMapper.To(result.Value!));
    }

    [HttpPost("outbox/{id:guid}/confirm")]
    [ProducesResponseType<OutboxMessageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Confirm([FromRoute] Guid id, [FromBody] ConfirmRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ConfirmAsync(caller, id, request.ExternalRef, request.ExternalAmount, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(FinanceMapper.To(result.Value!));
    }

    [HttpPost("outbox/{id:guid}/requeue")]
    [ProducesResponseType<OutboxMessageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Requeue([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.RequeueAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(FinanceMapper.To(result.Value!));
    }

    [HttpGet("reconciliation")]
    [ProducesResponseType<FinanceReconciliationResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reconciliation(CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ReconcileAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(FinanceMapper.To(result.Value!));
    }
}
