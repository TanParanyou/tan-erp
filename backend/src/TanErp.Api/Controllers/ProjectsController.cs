using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Projects;
using TanErp.Application.Projects.CreateProjectFromHandover;
using TanErp.Application.Projects.GetHandoverSource;
using TanErp.Application.Projects.GetProject;
using TanErp.Application.Projects.ListProjects;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly CreateProjectFromHandoverHandler _createHandler;
    private readonly GetProjectHandler _getHandler;
    private readonly ListProjectsHandler _listHandler;
    private readonly GetHandoverSourceHandler _handoverSourceHandler;

    public ProjectsController(
        CreateProjectFromHandoverHandler createHandler,
        GetProjectHandler getHandler,
        ListProjectsHandler listHandler,
        GetHandoverSourceHandler handoverSourceHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _listHandler = listHandler;
        _handoverSourceHandler = handoverSourceHandler;
    }

    [HttpPost]
    [ProducesResponseType<ProjectResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateFromHandover(
        [FromBody] CreateProjectFromHandoverRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _createHandler.Handle(
            new CreateProjectFromHandoverCommand(
                auth.FirebaseUid,
                auth.MembershipId,
                request.QuotationId,
                request.ExpectedQuotationVersion,
                request.OwnerUserId,
                request.PlannedStartDate,
                request.Name,
                auth.IdempotencyKey,
                HttpContext.TraceIdentifier),
            cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var project = result.Value!;
        Response.Headers.ETag = $"\"{project.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = project.Id }, ToResponse(project));
    }

    [HttpGet("handover-source")]
    [ProducesResponseType<ProjectHandoverSourceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHandoverSource([FromQuery] Guid opportunityId, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _handoverSourceHandler.Handle(
            new GetHandoverSourceQuery(auth.FirebaseUid, auth.MembershipId, opportunityId), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var s = result.Value!;
        return Ok(new ProjectHandoverSourceResponse(
            s.QuotationId, s.QuotationNumber, s.QuotationStatus, s.QuotationRowVersion, s.ContractAmount,
            s.OpportunityStage, s.ExistingProjectId, s.ExistingProjectCode));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProjectResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _getHandler.Handle(new GetProjectQuery(auth.FirebaseUid, auth.MembershipId, id), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToResponse(result.Value!));
    }

    [HttpGet]
    [ProducesResponseType<ProjectListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = ListProjectsHandler.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _listHandler.Handle(
            new ListProjectsQuery(auth.FirebaseUid, auth.MembershipId, search, status, page, pageSize), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var paged = result.Value!;
        var totalPages = paged.PageSize == 0 ? 0 : (int)Math.Ceiling(paged.TotalCount / (double)paged.PageSize);
        return Ok(new ProjectListResponse(
            paged.Items.Select(i => new ProjectListItemResponse(
                i.Id, i.Code, i.Name, i.Status, i.PlannedStartDate,
                new ProjectPersonResponse(i.Owner.Id, i.Owner.DisplayName, i.Owner.Email),
                new ProjectCustomerResponse(i.Customer.Id, i.Customer.Code, i.Customer.DisplayNameTh, i.Customer.DisplayNameEn),
                i.ContractAmount, i.CreatedAtUtc)).ToList(),
            new ProjectPaginationResponse(paged.Page, paged.PageSize, paged.TotalCount, totalPages)));
    }

    private static ProjectResponse ToResponse(ProjectDetailProjection p) => new(
        p.Id, p.BranchId, p.Code, p.Name, p.Status, p.PlannedStartDate,
        new ProjectPersonResponse(p.Owner.Id, p.Owner.DisplayName, p.Owner.Email),
        new ProjectCustomerResponse(p.Customer.Id, p.Customer.Code, p.Customer.DisplayNameTh, p.Customer.DisplayNameEn),
        p.Site is null ? null : new ProjectSiteResponse(p.Site.Id, p.Site.Label),
        new ProjectOpportunityResponse(p.Opportunity.Id, p.Opportunity.Code, p.Opportunity.Title),
        new ProjectBaselineResponse(
            p.Baseline.QuotationId, p.Baseline.QuotationNumber, p.Baseline.ContractAmount, p.Baseline.QuotationSnapshotHash,
            p.Baseline.EstimateId, p.Baseline.EstimateRevisionId, p.Baseline.SiteSurveyRevisionId,
            p.Baseline.SiteSurveySnapshotHash, p.Baseline.BaselineHash),
        p.RowVersion, p.CreatedAtUtc);
}
