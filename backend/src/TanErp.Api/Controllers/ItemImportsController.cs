using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Items.Import;
using TanErp.Application.Items.Import.CommitItemImport;
using TanErp.Application.Items.Import.PreviewItemImport;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/items/imports")]
[Authorize]
public class ItemImportsController : ControllerBase
{
    private const long MaxRequestBytes = 4_000_000;

    private readonly PreviewItemImportHandler _previewHandler;
    private readonly CommitItemImportHandler _commitHandler;

    public ItemImportsController(PreviewItemImportHandler previewHandler, CommitItemImportHandler commitHandler)
    {
        _previewHandler = previewHandler;
        _commitHandler = commitHandler;
    }

    [HttpPost("preview")]
    [RequestSizeLimit(MaxRequestBytes)]
    [ProducesResponseType<ItemImportPreviewResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Preview([FromBody] PreviewItemImportRequest request, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _previewHandler.Handle(
            new PreviewItemImportCommand(auth.FirebaseUid, auth.MembershipId, request.Content), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var preview = result.Value!;
        return Ok(new ItemImportPreviewResponse(
            preview.ContentSha256,
            preview.TotalRows,
            preview.ValidRows,
            preview.InvalidRows,
            preview.Rows.Select(ToRowResponse).ToList()));
    }

    [HttpPost("commit")]
    [RequestSizeLimit(MaxRequestBytes)]
    [ProducesResponseType<ItemImportCommitResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Commit([FromBody] CommitItemImportRequest request, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _commitHandler.Handle(
            new CommitItemImportCommand(
                auth.FirebaseUid,
                auth.MembershipId,
                request.Content,
                request.ExpectedContentSha256,
                auth.IdempotencyKey,
                HttpContext.TraceIdentifier),
            cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var commit = result.Value!;
        return StatusCode(
            StatusCodes.Status201Created,
            new ItemImportCommitResponse(commit.BatchId, commit.CreatedCount, commit.ContentSha256, commit.Replayed));
    }

    private static ItemImportRowResponse ToRowResponse(ItemImportRowResult row) => new(
        row.RowNumber,
        row.Code,
        row.NameTh,
        row.IsValid,
        row.Errors.Select(e => new ItemImportRowErrorResponse(e.Field, e.Code)).ToList());
}
