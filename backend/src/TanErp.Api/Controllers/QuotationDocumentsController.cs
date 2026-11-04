using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Commercial;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Commercial;
using TanErp.Application.Commercial.Documents;

namespace TanErp.Api.Controllers;

/// <summary>The stored, immutable PDF of a quotation. It is served from storage; it is never rendered again.</summary>
[ApiController]
[Authorize]
public class QuotationDocumentsController(QuotationDocumentHandler handler) : ControllerBase
{
    [HttpGet("api/v1/quotations/{id:guid}/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download([FromRoute] Guid id, CancellationToken ct)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var caller = new QuotationLifecycleCaller(context.Value!.FirebaseUid, context.Value.MembershipId, HttpContext.TraceIdentifier);
        var result = await handler.DownloadAsync(caller, id, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);

        var document = result.Value!;
        Response.Headers.CacheControl = "private, no-store";
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers.ETag = $"\"{document.Summary.Sha256}\"";
        return File(document.Content, "application/pdf", document.FileName);
    }

    [HttpPost("api/v1/quotations/{id:guid}/document")]
    [ProducesResponseType<QuotationDocumentSummaryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<QuotationDocumentSummaryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RenderLegacy([FromRoute] Guid id, CancellationToken ct)
    {
        var context = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var caller = new QuotationLifecycleCaller(context.Value!.FirebaseUid, context.Value.MembershipId, HttpContext.TraceIdentifier);
        var result = await handler.EnsureRenderedAsync(caller, id, context.Value.IdempotencyKey, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);

        var backfill = result.Value!;
        var summary = backfill.Summary;
        Response.Headers.ETag = $"\"{summary.Sha256}\"";
        var response = new QuotationDocumentSummaryResponse(
            summary.Sha256,
            summary.ByteCount,
            summary.RendererVersion,
            summary.RenderedAtUtc,
            summary.QuotationId ?? id,
            summary.FileId);
        return StatusCode(backfill.WasCreated ? StatusCodes.Status201Created : StatusCodes.Status200OK, response);
    }
}
