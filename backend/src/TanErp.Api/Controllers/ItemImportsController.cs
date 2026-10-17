using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Items.Import;
using TanErp.Application.Items.Import.CommitItemImport;
using TanErp.Application.Items.Import.CreateItemImportJob;
using TanErp.Application.Items.Import.GetItemImportJob;
using TanErp.Application.Items.Import.PreviewItemImport;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/items/imports")]
[Authorize]
public class ItemImportsController : ControllerBase
{
    private const long MaxRequestBytes = 4_000_000;
    private const long MaxJobRequestBytes = 21_000_000;

    private readonly PreviewItemImportHandler _previewHandler;
    private readonly CommitItemImportHandler _commitHandler;
    private readonly CreateItemImportJobHandler _createJobHandler;
    private readonly GetItemImportJobHandler _getJobHandler;

    public ItemImportsController(
        PreviewItemImportHandler previewHandler,
        CommitItemImportHandler commitHandler,
        CreateItemImportJobHandler createJobHandler,
        GetItemImportJobHandler getJobHandler)
    {
        _previewHandler = previewHandler;
        _commitHandler = commitHandler;
        _createJobHandler = createJobHandler;
        _getJobHandler = getJobHandler;
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
            new PreviewItemImportCommand(
                auth.FirebaseUid, auth.MembershipId, request.Content,
                request.Kind ?? ItemImportKind.Items, request.Mode ?? ItemImportMode.Create),
            cancellationToken);
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
            preview.Rows.Select(ToRowResponse).ToList(),
            preview.CreateCount,
            preview.UpdateCount,
            preview.UnchangedCount));
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
                HttpContext.TraceIdentifier,
                request.Kind ?? ItemImportKind.Items,
                request.Mode ?? ItemImportMode.Create),
            cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var commit = result.Value!;
        return StatusCode(
            StatusCodes.Status201Created,
            new ItemImportCommitResponse(
                commit.BatchId, commit.CreatedCount, commit.ContentSha256, commit.Replayed, commit.UpdatedCount, commit.UnchangedCount));
    }

    /// <summary>Queues a large file as a background job (Idempotency-Key required). Progress is read from GET jobs/{id}.</summary>
    [HttpPost("jobs")]
    [RequestSizeLimit(MaxJobRequestBytes)]
    [ProducesResponseType<ItemImportJobResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateJob([FromBody] CreateItemImportJobRequest request, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _createJobHandler.Handle(
            new CreateItemImportJobCommand(
                auth.FirebaseUid,
                auth.MembershipId,
                request.Kind,
                request.Mode,
                request.Content,
                request.ExpectedContentSha256,
                auth.IdempotencyKey,
                HttpContext.TraceIdentifier),
            cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var job = result.Value!;
        Response.Headers.Location = $"/api/v1/items/imports/jobs/{job.Id}";
        return StatusCode(StatusCodes.Status202Accepted, ToJobResponse(job));
    }

    [HttpGet("jobs/{jobId:guid}")]
    [ProducesResponseType<ItemImportJobResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJob([FromRoute] Guid jobId, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _getJobHandler.Handle(new GetItemImportJobQuery(auth.FirebaseUid, auth.MembershipId, jobId), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(ToJobResponse(result.Value!));
    }

    private static ItemImportJobResponse ToJobResponse(ItemImportJobSnapshot job) => new(
        job.Id, job.Kind, job.Mode, job.Status, job.ContentSha256, job.TotalRows, job.ProcessedRows, job.BatchSize,
        job.TotalBatches, job.CompletedBatches, job.RejectedBatches, job.CreatedCount, job.UpdatedCount, job.UnchangedCount,
        job.ErrorRowCount, job.ErrorsTruncated,
        job.Errors.Select(e => new ItemImportJobRowResponse(
            e.RowNumber, e.Code, e.NameTh, e.Errors.Select(x => new ItemImportRowErrorResponse(x.Field, x.Code)).ToList())).ToList(),
        job.FailureCode, job.CreatedAtUtc, job.StartedAtUtc, job.CompletedAtUtc, job.Replayed);

    private static ItemImportRowResponse ToRowResponse(ItemImportRowResult row) => new(
        row.RowNumber,
        row.Code,
        row.NameTh,
        row.IsValid,
        row.Errors.Select(e => new ItemImportRowErrorResponse(e.Field, e.Code)).ToList(),
        row.Outcome);
}
