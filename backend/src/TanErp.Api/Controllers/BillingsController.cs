using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Finance;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Finance;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class BillingsController : FinanceControllerBase
{
    private readonly FinanceHandler _handler;

    public BillingsController(FinanceHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("api/v1/billings")]
    [ProducesResponseType<BillingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] BillingRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateBillingAsync(caller, key, new BillingInput(request.ProjectId, request.Kind ?? string.Empty, request.Description ?? string.Empty, request.Amount, request.DueDate), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, FinanceMapper.To(result.Value));
    }

    [HttpGet("api/v1/billings/{id:guid}")]
    [ProducesResponseType<BillingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetBillingAsync(caller, id, ct));
    }

    [HttpGet("api/v1/billings")]
    [ProducesResponseType<BillingListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] Guid? projectId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = FinanceHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListBillingsAsync(caller, search, status, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new BillingListResponse(paged.Items.Select(FinanceMapper.To).ToList(), FinanceMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpGet("api/v1/projects/{projectId:guid}/billing-summary")]
    [ProducesResponseType<ProjectBillingSummaryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Summary([FromRoute] Guid projectId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetProjectSummaryAsync(caller, projectId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(FinanceMapper.To(result.Value!));
    }

    [HttpPost("api/v1/billings/{id:guid}/void")]
    [ProducesResponseType<BillingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Void([FromRoute] Guid id, [FromBody] FinanceReasonRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.VoidBillingAsync(caller, id, version, request.Reason, ct));
    }

    [HttpPost("api/v1/billings/{id:guid}/payments")]
    [ProducesResponseType<BillingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordPayment([FromRoute] Guid id, [FromBody] PaymentRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.RecordPaymentAsync(caller, id, key, new PaymentInput(request.Amount, request.Method ?? string.Empty, request.Reference ?? string.Empty, request.ReceivedDate), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(StatusCodes.Status201Created, FinanceMapper.To(result.Value));
    }

    [HttpPost("api/v1/billings/{id:guid}/payments/{paymentId:guid}/reverse")]
    [ProducesResponseType<BillingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReversePayment([FromRoute] Guid id, [FromRoute] Guid paymentId, [FromBody] FinanceReasonRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.ReversePaymentAsync(caller, id, paymentId, version, request.Reason, ct));
    }

    private IActionResult Respond(Result<BillingProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(FinanceMapper.To(result.Value));
    }
}
