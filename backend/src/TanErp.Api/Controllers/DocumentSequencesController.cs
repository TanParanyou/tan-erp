using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.DocumentNumbering;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.DocumentNumbering.ListDocumentSequences;
using TanErp.Application.DocumentNumbering.PreviewDocumentSequence;
using TanErp.Application.DocumentNumbering.UpdateDocumentSequence;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class DocumentSequencesController : ControllerBase
{
    private readonly ListDocumentSequencesHandler _listHandler;
    private readonly UpdateDocumentSequenceHandler _updateHandler;
    private readonly PreviewDocumentSequenceHandler _previewHandler;

    public DocumentSequencesController(
        ListDocumentSequencesHandler listHandler,
        UpdateDocumentSequenceHandler updateHandler,
        PreviewDocumentSequenceHandler previewHandler)
    {
        _listHandler = listHandler;
        _updateHandler = updateHandler;
        _previewHandler = previewHandler;
    }

    [HttpGet("api/v1/settings/document-sequences")]
    [ProducesResponseType<IReadOnlyList<DocumentSequenceResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var query = new ListDocumentSequencesQuery(auth.FirebaseUid, auth.MembershipId);
        var result = await _listHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = result.Value!.Select(p => new DocumentSequenceResponse
        {
            Id = p.Id,
            DocumentType = p.DocumentType,
            Prefix = p.Prefix,
            FormatPattern = p.FormatPattern,
            ResetPeriod = p.ResetPeriod,
            Padding = p.Padding,
            IsBranchSpecific = p.IsBranchSpecific,
            IsActive = p.IsActive,
            SamplePreview = p.SamplePreview,
            RowVersion = p.RowVersion
        }).ToList();

        return Ok(response);
    }

    [HttpPut("api/v1/settings/document-sequences/{documentType}")]
    [ProducesResponseType<DocumentSequenceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(
        [FromRoute] string documentType,
        [FromBody] UpdateDocumentSequenceRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var command = new UpdateDocumentSequenceCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IfMatchRowVersion,
            documentType,
            request.Prefix,
            request.FormatPattern,
            request.ResetPeriod,
            request.Padding,
            request.IsBranchSpecific);

        var result = await _updateHandler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var p = result.Value!;
        Response.Headers.ETag = $"\"{p.RowVersion}\"";
        return Ok(new DocumentSequenceResponse
        {
            Id = p.Id,
            DocumentType = p.DocumentType,
            Prefix = p.Prefix,
            FormatPattern = p.FormatPattern,
            ResetPeriod = p.ResetPeriod,
            Padding = p.Padding,
            IsBranchSpecific = p.IsBranchSpecific,
            IsActive = p.IsActive,
            SamplePreview = p.SamplePreview,
            RowVersion = p.RowVersion
        });
    }

    [HttpPost("api/v1/settings/document-sequences/preview")]
    [ProducesResponseType<PreviewDocumentSequenceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Preview(
        [FromBody] PreviewDocumentSequenceRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var query = new PreviewDocumentSequenceQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            request.Prefix,
            request.FormatPattern,
            request.BranchCode,
            request.Padding,
            request.SampleSequence);

        var result = await _previewHandler.HandleAsync(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(new PreviewDocumentSequenceResponse { Preview = result.Value! });
    }
}
