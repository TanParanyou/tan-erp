using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Attachments;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Attachments;

namespace TanErp.Api.Controllers;

/// <summary>Shared attachments and signatures for any registered owner type. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/attachment-owners/{ownerType}/{ownerId:guid}")]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly AttachmentHandler _handler;

    public AttachmentsController(AttachmentHandler handler)
    {
        _handler = handler;
    }

    private AttachmentCaller? ReadCaller(out string idempotencyKey, bool requireIdempotency, out IActionResult? failure)
    {
        idempotencyKey = string.Empty;
        if (requireIdempotency)
        {
            var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
            if (idempotent.IsFailure)
            {
                failure = ProblemDetailsMapper.CreateProblemResult(idempotent.Error.Code, HttpContext);
                return null;
            }

            failure = null;
            idempotencyKey = idempotent.Value!.IdempotencyKey;
            return new AttachmentCaller(idempotent.Value.FirebaseUid, idempotent.Value.MembershipId, HttpContext.TraceIdentifier);
        }

        var authenticated = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authenticated.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(authenticated.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        return new AttachmentCaller(authenticated.Value!.FirebaseUid, authenticated.Value.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    private static AttachmentPersonResponse To(AttachmentPerson person) => new(person.Id, person.DisplayName);

    private static AttachmentLinkResponse To(AttachmentLinkProjection p) => new(
        p.Id, p.OwnerType, p.OwnerId, p.FileId, p.Purpose, p.Filename, p.MediaType, p.FileSizeBytes, p.ServingUrl, To(p.CreatedBy), p.CreatedAtUtc);

    private static SignatureCaptureResponse To(SignatureCaptureProjection p) => new(
        p.Id, p.OwnerType, p.OwnerId, p.Purpose, p.SignerName, p.SignerRole, p.SignedAtUtc, p.ImageFileId, p.ServingUrl,
        p.ConsentTextVersion, p.ContentHash, To(p.CapturedBy));

    [HttpGet("attachments")]
    [ProducesResponseType<AttachmentListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ListAttachments([FromRoute] string ownerType, [FromRoute] Guid ownerId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, ownerType, ownerId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new AttachmentListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpPost("attachments")]
    [ProducesResponseType<AttachmentListResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Attach([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromBody] AttachFilesRequest request, CancellationToken ct)
    {
        var caller = ReadCaller(out var key, true, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.AttachAsync(caller, ownerType, ownerId, key, new AttachFilesInput(request.Purpose, request.FileIds), ct);
        return result.IsFailure
            ? Problem(result.Error.Code)
            : StatusCode(StatusCodes.Status201Created, new AttachmentListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpDelete("attachments/{linkId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Unlink([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromRoute] Guid linkId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.UnlinkAsync(caller, ownerType, ownerId, linkId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : NoContent();
    }

    [HttpGet("signatures")]
    [ProducesResponseType<SignatureCaptureListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ListSignatures([FromRoute] string ownerType, [FromRoute] Guid ownerId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListSignaturesAsync(caller, ownerType, ownerId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new SignatureCaptureListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpPost("signatures")]
    [ProducesResponseType<SignatureCaptureResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CaptureSignature([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromBody] CaptureSignatureRequest request, CancellationToken ct)
    {
        var caller = ReadCaller(out var key, true, out var failure);
        if (caller is null) return failure!;
        var input = new SignatureCaptureInput(request.Purpose, request.SignerName, request.SignerRole, request.ImageFileId, request.ConsentAccepted, request.ConsentTextVersion);
        var result = await _handler.CaptureSignatureAsync(caller, ownerType, ownerId, key, input, ct);
        return result.IsFailure ? Problem(result.Error.Code) : StatusCode(StatusCodes.Status201Created, To(result.Value!));
    }
}
