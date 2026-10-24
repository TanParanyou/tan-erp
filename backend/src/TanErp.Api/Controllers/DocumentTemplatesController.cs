using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.DocumentTemplates;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.DocumentTemplates.CreateDocumentTemplate;
using TanErp.Application.DocumentTemplates.DeleteDocumentTemplate;
using TanErp.Application.DocumentTemplates.GetDocumentTemplate;
using TanErp.Application.DocumentTemplates.ListDocumentTemplates;
using TanErp.Application.DocumentTemplates.UpdateDocumentTemplate;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class DocumentTemplatesController : ControllerBase
{
    private readonly ListDocumentTemplatesHandler _listHandler;
    private readonly GetDocumentTemplateByIdHandler _getHandler;
    private readonly CreateDocumentTemplateHandler _createHandler;
    private readonly UpdateDocumentTemplateHandler _updateHandler;
    private readonly DeleteDocumentTemplateHandler _deleteHandler;

    public DocumentTemplatesController(
        ListDocumentTemplatesHandler listHandler,
        GetDocumentTemplateByIdHandler getHandler,
        CreateDocumentTemplateHandler createHandler,
        UpdateDocumentTemplateHandler updateHandler,
        DeleteDocumentTemplateHandler deleteHandler)
    {
        _listHandler = listHandler;
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet("api/v1/settings/document-templates")]
    [ProducesResponseType<IReadOnlyList<DocumentTemplateResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] string? documentType,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var query = new ListDocumentTemplatesQuery(auth.FirebaseUid, auth.MembershipId, documentType, branchId);
        var result = await _listHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = result.Value!.Select(p => new DocumentTemplateResponse
        {
            Id = p.Id,
            OrganizationId = p.OrganizationId,
            BranchId = p.BranchId,
            DocumentType = p.DocumentType,
            Name = p.Name,
            IsDefault = p.IsDefault,
            LayoutConfigJson = p.LayoutConfigJson,
            LogoFileId = p.LogoFileId,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            RowVersion = p.RowVersion
        }).ToList();

        return Ok(response);
    }

    [HttpGet("api/v1/settings/document-templates/{id:guid}")]
    [ProducesResponseType<DocumentTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var query = new GetDocumentTemplateByIdQuery(auth.FirebaseUid, auth.MembershipId, id);
        var result = await _getHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var p = result.Value!;
        Response.Headers.ETag = $"\"{p.RowVersion}\"";
        return Ok(new DocumentTemplateResponse
        {
            Id = p.Id,
            OrganizationId = p.OrganizationId,
            BranchId = p.BranchId,
            DocumentType = p.DocumentType,
            Name = p.Name,
            IsDefault = p.IsDefault,
            LayoutConfigJson = p.LayoutConfigJson,
            LogoFileId = p.LogoFileId,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            RowVersion = p.RowVersion
        });
    }

    [HttpPost("api/v1/settings/document-templates")]
    [ProducesResponseType<DocumentTemplateResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDocumentTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var command = new CreateDocumentTemplateCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            request.DocumentType,
            request.Name,
            request.IsDefault,
            request.LayoutConfigJson,
            request.LogoFileId,
            request.BranchId);

        var result = await _createHandler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var p = result.Value!;
        Response.Headers.ETag = $"\"{p.RowVersion}\"";
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, new DocumentTemplateResponse
        {
            Id = p.Id,
            OrganizationId = p.OrganizationId,
            BranchId = p.BranchId,
            DocumentType = p.DocumentType,
            Name = p.Name,
            IsDefault = p.IsDefault,
            LayoutConfigJson = p.LayoutConfigJson,
            LogoFileId = p.LogoFileId,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            RowVersion = p.RowVersion
        });
    }

    [HttpPut("api/v1/settings/document-templates/{id:guid}")]
    [ProducesResponseType<DocumentTemplateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDocumentTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var command = new UpdateDocumentTemplateCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IfMatchRowVersion,
            id,
            request.Name,
            request.IsDefault,
            request.LayoutConfigJson,
            request.LogoFileId);

        var result = await _updateHandler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var p = result.Value!;
        Response.Headers.ETag = $"\"{p.RowVersion}\"";
        return Ok(new DocumentTemplateResponse
        {
            Id = p.Id,
            OrganizationId = p.OrganizationId,
            BranchId = p.BranchId,
            DocumentType = p.DocumentType,
            Name = p.Name,
            IsDefault = p.IsDefault,
            LayoutConfigJson = p.LayoutConfigJson,
            LogoFileId = p.LogoFileId,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            RowVersion = p.RowVersion
        });
    }

    [HttpDelete("api/v1/settings/document-templates/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var command = new DeleteDocumentTemplateCommand(auth.FirebaseUid, auth.MembershipId, id);
        var result = await _deleteHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return NoContent();
    }
}
