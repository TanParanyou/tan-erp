using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Sites;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Crm.Sites;
using TanErp.Application.Crm.Sites.CreateSite;
using TanErp.Application.Crm.Sites.ListSites;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/customers/{customerId}/sites")]
[Authorize]
public class SitesController : ControllerBase
{
    private readonly CreateSiteHandler _createHandler;
    private readonly ListSitesHandler _listHandler;
    private readonly TanErp.Application.Crm.Sites.ManageCustomerSiteHandler _manageHandler;

    public SitesController(
        CreateSiteHandler createHandler,
        ListSitesHandler listHandler,
        TanErp.Application.Crm.Sites.ManageCustomerSiteHandler manageHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _manageHandler = manageHandler;
    }

    [HttpPost]
    [ProducesResponseType<SiteResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid customerId,
        [FromBody] CreateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new CreateSiteCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            customerId,
            auth.IdempotencyKey,
            request.Label,
            request.AddressLine1,
            request.Subdistrict,
            request.District,
            request.Province,
            request.PostalCode,
            request.CountryCode,
            request.Latitude,
            request.Longitude,
            request.AccessNote,
            traceId,
            request.Images?.Select(i => new CreateSiteImageInput(i.FileId, i.Caption)).ToList(),
            request.FileUploadIntentId);

        var result = await _createHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var site = result.Value!;
        Response.Headers.ETag = $"\"{site.RowVersion}\"";

        var response = ToResponse(site);
        return CreatedAtAction(
            nameof(List),
            new { customerId = site.CustomerId },
            response);
    }

    [HttpGet]
    [ProducesResponseType<SiteListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(
        [FromRoute] Guid customerId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var query = new ListSitesQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            customerId,
            traceId);

        var result = await _listHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Select(ToResponse).ToList();
        return Ok(new SiteListResponse(items));
    }

    [HttpPatch("{siteId:guid}")]
    [ProducesResponseType<SiteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update([FromRoute] Guid customerId, [FromRoute] Guid siteId, [FromBody] UpdateSiteRequest request, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var data = new TanErp.Application.Crm.Sites.UpdateSiteData(request.Label, request.AddressLine1, request.Subdistrict, request.District, request.Province, request.PostalCode, request.CountryCode, request.Latitude, request.Longitude, request.AccessNote);
        var result = await _manageHandler.UpdateAsync(auth.FirebaseUid, auth.MembershipId, customerId, siteId, auth.IfMatchRowVersion, data, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    [HttpPost("{siteId:guid}/deactivate")]
    [ProducesResponseType<SiteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Deactivate([FromRoute] Guid customerId, [FromRoute] Guid siteId, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _manageHandler.DeactivateAsync(auth.FirebaseUid, auth.MembershipId, customerId, siteId, auth.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value));
    }

    private static SiteResponse ToResponse(SiteProjection s) => new(
        s.Id,
        s.CustomerId,
        s.Code,
        s.Label,
        s.AddressLine1,
        s.Subdistrict,
        s.District,
        s.Province,
        s.PostalCode,
        s.CountryCode,
        s.Latitude,
        s.Longitude,
        s.AccessNote,
        s.Status,
        s.RowVersion,
        s.CreatedAtUtc,
        s.Images?.Select(img => new SiteImageResponse(
            img.Id,
            img.FileId,
            img.Caption,
            img.DisplayOrder,
            img.CreatedAtUtc)).ToList());
}
