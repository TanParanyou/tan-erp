using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TanErp.Api.Contracts.Commercial;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Commercial;

namespace TanErp.Api.Controllers;

/// <summary>
/// Customer side of external acceptance. Anonymous by design: the unguessable token is the credential. Rate limited per client,
/// never cached, and every unusable-token case returns the same 404 so tokens cannot be probed.
/// </summary>
[ApiController]
[AllowAnonymous]
[EnableRateLimiting(PublicRateLimiting.PolicyName)]
[Route("api/public/v1/quotation-acceptance")]
public class PublicQuotationAcceptanceController : ControllerBase
{
    private readonly QuotationAcceptanceHandler _handler;

    public PublicQuotationAcceptanceController(QuotationAcceptanceHandler handler)
    {
        _handler = handler;
    }

    private void NoStore()
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers["Referrer-Policy"] = "no-referrer";
    }

    [HttpGet("{token}")]
    [ProducesResponseType<PublicAcceptanceViewResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Get([FromRoute] string token, [FromQuery] string? locale, CancellationToken ct)
    {
        NoStore();
        var result = await _handler.GetPublicViewAsync(token, locale, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var v = result.Value!;
        return Ok(new PublicAcceptanceViewResponse(v.Status, v.ExpiresAtUtc, v.SignerHint, v.ConsentVersion, v.AcceptedAtUtc, QuotationDocumentResponse.FromProjection(v.Document)));
    }

    [HttpPost("{token}/accept")]
    [ProducesResponseType<PublicAcceptanceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Accept([FromRoute] string token, [FromBody] PublicAcceptRequest request, CancellationToken ct)
    {
        NoStore();
        var client = new ClientInfo(HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        var result = await _handler.AcceptAsync(
            token, new AcceptanceSubmission(request.SignerName, request.SignerRole, request.ConsentAccepted, request.ConsentVersion, request.SignatureImage),
            client, HttpContext.TraceIdentifier, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var r = result.Value!;
        return Ok(new PublicAcceptanceResponse(r.Status, r.AcceptedAtUtc, r.QuotationNumber, QuotationAcceptanceLinksController.ToEvidence(r.Evidence)!));
    }
}
