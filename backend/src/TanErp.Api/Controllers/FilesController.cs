using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TanErp.Api.Contracts.Files;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Files;
using TanErp.Application.Files.CompleteUploadSession;
using TanErp.Application.Files.CreateUploadSession;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly CreateUploadSessionHandler _createSessionHandler;
    private readonly CompleteUploadSessionHandler _completeSessionHandler;
    private readonly IFileStorageProvider _storageProvider;
    private readonly AppDbContext _dbContext;
    private readonly string _storageBasePath;

    public FilesController(
        CreateUploadSessionHandler createSessionHandler,
        CompleteUploadSessionHandler completeSessionHandler,
        IFileStorageProvider storageProvider,
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _createSessionHandler = createSessionHandler;
        _completeSessionHandler = completeSessionHandler;
        _storageProvider = storageProvider;
        _dbContext = dbContext;

        var basePath = configuration["Storage:BasePath"];
        if (string.IsNullOrWhiteSpace(basePath))
        {
            basePath = Path.Combine(AppContext.BaseDirectory, "local-storage");
        }
        _storageBasePath = basePath;
    }

    /// <summary>
    /// Creates a new upload session and returns pre-authorized slot URLs for each file.
    /// The client uploads each file to its slot URL, then calls complete-session.
    /// </summary>
    [HttpPost("sessions")]
    [ProducesResponseType<CreateUploadSessionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
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
            session.Slots.Select(s => new UploadSlotResponse(s.SlotId, s.UploadUrl)).ToList());

        return CreatedAtAction(null, response);
    }

    /// <summary>
    /// Uploads a single file to a specific slot in a session.
    /// Accepts multipart/form-data with a single "file" field.
    /// </summary>
    [HttpPost("sessions/{sessionId}/slots/{slotIndex:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [RequestFormLimits(MultipartBodyLengthLimit = 15 * 1024 * 1024)] // 15 MB raw limit
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<IActionResult> UploadSlot(
        [FromRoute] string sessionId,
        [FromRoute] int slotIndex,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
        {
            return ProblemDetailsMapper.CreateProblemResult("FILE_UPLOAD_SESSION_INVALID", HttpContext);
        }

        var formFile = Request.Form.Files.GetFile("file");
        if (formFile is null)
        {
            return ProblemDetailsMapper.CreateProblemResult("FILE_UPLOAD_SESSION_INVALID", HttpContext);
        }

        // Temporarily persist to session slot storage path; CompleteSession will read it back
        var auth = contextResult.Value!;

        // We use CompleteUploadSessionHandler directly for single-file sessions in this slot
        var fileInput = new FileCompletionInput(
            $"{sessionId}_{slotIndex}",
            formFile.FileName,
            formFile.ContentType,
            formFile.Length,
            formFile.OpenReadStream());

        var command = new CompleteUploadSessionCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            sessionId,
            [fileInput],
            HttpContext.TraceIdentifier);

        var result = await _completeSessionHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return NoContent();
    }

    /// <summary>
    /// Completes an upload session after all slot files have been uploaded.
    /// Verifies magic numbers, persists metadata, and returns fileIds.
    /// The client submits file references; actual binaries come from slot uploads.
    /// </summary>
    [HttpPost("sessions/{sessionId}/complete")]
    [ProducesResponseType<CompleteUploadSessionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [RequestFormLimits(MultipartBodyLengthLimit = 200 * 1024 * 1024)] // 200 MB aggregate
    [RequestSizeLimit(200 * 1024 * 1024)]
    public async Task<IActionResult> CompleteSession(
        [FromRoute] string sessionId,
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

        var fileInputs = Request.Form.Files
            .Select((f, i) => new FileCompletionInput(
                SlotId: $"{sessionId}_{i}",
                OriginalFilename: f.FileName,
                MediaType: f.ContentType,
                FileSizeBytes: f.Length,
                Content: f.OpenReadStream()))
            .ToList();

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
    /// Serves a stored file by its file ID (GUID) or storage path segments.
    /// </summary>
    [HttpGet("{**storagePath}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ServeFile(
        [FromRoute] string storagePath,
        CancellationToken cancellationToken)
    {
        string? relativePath = null;
        string mediaType = "application/octet-stream";

        if (Guid.TryParse(storagePath, out var fileId))
        {
            var file = await _dbContext.UploadedFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);

            if (file is null)
            {
                return ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext);
            }

            relativePath = file.StoragePath;
            mediaType = string.IsNullOrWhiteSpace(file.MediaType) ? "image/webp" : file.MediaType;
        }
        else
        {
            relativePath = storagePath;
        }

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext);
        }

        var fullPath = Path.GetFullPath(Path.Combine(_storageBasePath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        var baseFullPath = Path.GetFullPath(_storageBasePath);

        // Security: directory traversal guard
        if (!fullPath.StartsWith(baseFullPath, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
        {
            return ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext);
        }

        return PhysicalFile(fullPath, mediaType, enableRangeProcessing: true);
    }
}
