using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Commercial;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Commercial;
using TanErp.Application.Common.Results;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class QuotationLifecycleController : ControllerBase
{
    private readonly QuotationLifecycleHandler _handler;

    public QuotationLifecycleController(QuotationLifecycleHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("api/v1/quotations/{id:guid}/void")]
    [ProducesResponseType<QuotationHistoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Void([FromRoute] Guid id, [FromBody] QuotationReasonRequest request, CancellationToken ct)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var caller = new QuotationLifecycleCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
        return Respond(await _handler.VoidAsync(caller, id, auth.IfMatchRowVersion, request.Reason, ct));
    }

    [HttpPost("api/v1/quotations/{id:guid}/amend")]
    [ProducesResponseType<QuotationHistoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Amend([FromRoute] Guid id, [FromBody] QuotationReasonRequest request, CancellationToken ct)
    {
        var context = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var caller = new QuotationLifecycleCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
        var result = await _handler.AmendAsync(caller, id, auth.IfMatchRowVersion, auth.IdempotencyKey, request.Reason, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value!));
    }

    [HttpGet("api/v1/estimates/{id:guid}/quotations")]
    [ProducesResponseType<QuotationHistoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> History([FromRoute] Guid id, CancellationToken ct)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        return Respond(await _handler.GetHistoryAsync(new QuotationLifecycleCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier), id, ct));
    }

    private IActionResult Respond(Result<QuotationHistoryProjection> result) =>
        result.IsFailure ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext) : Ok(ToResponse(result.Value!));

    private static QuotationHistoryResponse ToResponse(QuotationHistoryProjection h) => new(h.EstimateId, h.Items.Select(q => new QuotationHistoryItemResponse(
        q.Id, q.Number, q.Status, q.TotalAmount, q.EstimateRevisionId, q.EstimateRevisionNo, q.IssuedAtUtc, q.AcceptedAtUtc,
        q.SupersedesQuotationId, q.SupersedesNumber, q.SupersededByQuotationId, q.SupersededByNumber, q.AmendmentReason,
        q.VoidedAtUtc, q.VoidedBy is null ? null : new QuotationPersonResponse(q.VoidedBy.Id, q.VoidedBy.DisplayName), q.VoidReason, q.RowVersion)).ToList());
}
