using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Files;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Files.CompleteUploadSession;
using TanErp.Application.Files.CreateUploadSession;
using TanErp.Application.Files.GetFileContent;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly CreateUploadSessionHandler _createSessionHandler;
    private readonly CompleteUploadSessionHandler _completeSessionHandler;
    private readonly GetFileContentHandler _getFileContentHandler;

    public FilesController(
        CreateUploadSessionHandler createSessionHandler,
        CompleteUploadSessionHandler completeSessionHandler,
        GetFileContentHandler getFileContentHandler)
    {
        _createSessionHandler = createSessionHandler;
        _completeSessionHandler = completeSessionHandler;
        _getFileContentHandler = getFileContentHandler;
    }

    /// <summary>
    /// Creates a parent-bound upload session and returns declared slots.
    /// </summary>
    [HttpPost("upload-sessions")]
    [HttpPost("sessions")]
    [ProducesResponseType<CreateUploadSessionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateUploadSessionRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var slots = request.Files
            .Select(f => new FileSlotInput(f.Filename, f.MediaType, f.FileSizeBytes))
            .ToList();

        var command = new CreateUploadSessionCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            request.ParentType,
            request.ParentId,
            request.CreationIntentId,
            slots,
            auth.IdempotencyKey,
            traceId);

        var result = await _createSessionHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var session = result.Value!;
        var response = new CreateUploadSessionResponse(
            session.SessionId,
            session.ExpiresAtUtc,
            session.Slots.Select(s => new UploadSlotResponse(s.SlotId, s.Filename, s.MediaType, s.FileSizeBytes)).ToList());

        return CreatedAtAction(null, response);
    }

    /// <summary>
    /// Completes an upload session by verifying and streaming multipart file binaries.
    /// </summary>
    [HttpPost("upload-sessions/{sessionId:guid}/complete")]
    [HttpPost("sessions/{sessionId:guid}/complete")]
    [ProducesResponseType<CompleteUploadSessionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [RequestFormLimits(MultipartBodyLengthLimit = 200 * 1024 * 1024)] // 200 MB aggregate
    [RequestSizeLimit(200 * 1024 * 1024)]
    public async Task<IActionResult> CompleteSession(
        [FromRoute] Guid sessionId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;

        if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
        {
            return ProblemDetailsMapper.CreateProblemResult("FILE_UPLOAD_SESSION_INVALID", HttpContext);
        }

        var fileInputs = new List<FileCompletionInput>();

        for (var i = 0; i < Request.Form.Files.Count; i++)
        {
            var formFile = Request.Form.Files[i];

            // Resolve Slot ID: from form field slotId, part name, or header
            Guid slotId = Guid.Empty;
            if (Guid.TryParse(formFile.Name, out var nameGuid))
            {
                slotId = nameGuid;
            }
            else if (Request.Form.TryGetValue($"slotId_{i}", out var slotIdVal) && Guid.TryParse(slotIdVal, out var parsedVal))
            {
                slotId = parsedVal;
            }
            else if (Request.Form.TryGetValue("slotId", out var singleSlotId) && Guid.TryParse(singleSlotId, out var parsedSingle))
            {
                slotId = parsedSingle;
            }
            else if (Request.Headers.TryGetValue("X-Slot-Id", out var headerVal) && Guid.TryParse(headerVal, out var parsedHeader))
            {
                slotId = parsedHeader;
            }

            fileInputs.Add(new FileCompletionInput(
                SlotId: slotId,
                OriginalFilename: formFile.FileName,
                MediaType: formFile.ContentType,
                FileSizeBytes: formFile.Length,
                Content: formFile.OpenReadStream()));
        }

        var command = new CompleteUploadSessionCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            sessionId,
            fileInputs,
            HttpContext.TraceIdentifier);

        var result = await _completeSessionHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var session = result.Value!;
        var response = new CompleteUploadSessionResponse(
            session.SessionId,
            session.Files.Select(f => new CompletedFileResponse(
                f.FileId,
                f.Filename,
                f.MediaType,
                f.FileSizeBytes,
                f.ServingUrl)).ToList());

        return Ok(response);
    }

    /// <summary>
    /// Streams protected file content to authorized users. Anonymous or unauthenticated access is strictly forbidden.
    /// </summary>
    [HttpGet("{fileId:guid}/content")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileContent(
        [FromRoute] Guid fileId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;

        var query = new GetFileContentQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            fileId);

        var result = await _getFileContentHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        Response.Headers["Cache-Control"] = "private, no-store";

        var content = result.Value!;
        return File(content.ContentStream, content.MediaType, content.Filename, enableRangeProcessing: true);
    }
}
