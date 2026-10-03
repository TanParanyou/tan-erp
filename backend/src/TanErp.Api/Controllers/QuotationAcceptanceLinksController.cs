using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Commercial;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Commercial;
using TanErp.Application.Common.Results;

namespace TanErp.Api.Controllers;

/// <summary>Staff side of external acceptance: create (token shown once), list and revoke links.</summary>
[ApiController]
[Authorize]
public class QuotationAcceptanceLinksController : ControllerBase
{
    private readonly QuotationAcceptanceHandler _handler;

    public QuotationAcceptanceLinksController(QuotationAcceptanceHandler handler)
    {
        _handler = handler;
    }

    private QuotationLifecycleCaller? Caller(out IActionResult? failure)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        return new QuotationLifecycleCaller(context.Value!.FirebaseUid, context.Value.MembershipId, HttpContext.TraceIdentifier);
    }

    [HttpPost("api/v1/quotations/{id:guid}/acceptance-links")]
    [ProducesResponseType<CreatedAcceptanceLinkResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromRoute] Guid id, [FromBody] CreateAcceptanceLinkRequest request, CancellationToken ct)
    {
        var caller = Caller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateLinkAsync(caller, id, request.LifetimeDays, request.SignerHint, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.CacheControl = "no-store";
        return StatusCode(StatusCodes.Status201Created, new CreatedAcceptanceLinkResponse(ToResponse(result.Value!.Link), result.Value.Token, $"/accept/{result.Value.Token}"));
    }

    [HttpGet("api/v1/quotations/{id:guid}/acceptance-links")]
    [ProducesResponseType<AcceptanceLinkListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = Caller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListLinksAsync(caller, id, ct);
        return result.IsFailure ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext) : Ok(new AcceptanceLinkListResponse(result.Value!.Select(ToResponse).ToList()));
    }

    [HttpPost("api/v1/acceptance-links/{id:guid}/revoke")]
    [ProducesResponseType<AcceptanceLinkResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Revoke([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = Caller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.RevokeLinkAsync(caller, id, ct);
        return result.IsFailure ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext) : Ok(ToResponse(result.Value!));
    }

    internal static AcceptanceEvidenceResponse? ToEvidence(AcceptanceEvidenceSummary? e) =>
        e is null ? null : new AcceptanceEvidenceResponse(e.SignerName, e.SignerRole, e.ConsentVersion, e.HasSignatureImage, e.SignatureHash, e.AcceptedAtUtc);

    private static AcceptanceLinkResponse ToResponse(AcceptanceLinkProjection l) => new(
        l.Id, l.QuotationId, l.QuotationNumber, l.Status, l.QuotationStatus, l.IsUsable, l.SignerHint, l.ExpiresAtUtc, l.CreatedAtUtc,
        new QuotationPersonResponse(l.CreatedBy.Id, l.CreatedBy.DisplayName), l.RevokedAtUtc, l.AcceptedAtUtc, ToEvidence(l.Evidence));
}
