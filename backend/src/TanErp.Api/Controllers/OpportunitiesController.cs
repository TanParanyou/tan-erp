using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Opportunities;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.GetOpportunity;
using TanErp.Application.Crm.Opportunities.ListOpportunities;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/opportunities")]
[Authorize]
public class OpportunitiesController : ControllerBase
{
    private readonly CreateOpportunityHandler _createHandler;
    private readonly ListOpportunitiesHandler _listHandler;
    private readonly GetOpportunityHandler _getHandler;

    public OpportunitiesController(
        CreateOpportunityHandler createHandler,
        ListOpportunitiesHandler listHandler,
        GetOpportunityHandler getHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _getHandler = getHandler;
    }

    [HttpPost]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOpportunityRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new CreateOpportunityCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            request.CustomerId,
            request.PrimarySiteId,
            request.Title,
            request.ScopeSummary,
            request.WorkTypes,
            request.SourceCode,
            request.ExpectedBudget,
            request.CurrencyCode,
            request.TargetDecisionDate,
            request.NextActionAtUtc,
            request.NextActionNote,
            auth.IdempotencyKey,
            traceId);

        var result = await _createHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var opp = result.Value!;
        Response.Headers.ETag = $"\"{opp.RowVersion}\"";

        var response = ToResponse(opp);
        return CreatedAtAction(
            nameof(Get),
            new { id = opp.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType<OpportunityListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] Guid? customerId,
        [FromQuery] string? stage,
        [FromQuery] int limit = 25,
        [FromQuery] string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var query = new ListOpportunitiesQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            search,
            customerId,
            stage,
            limit,
            cursor,
            traceId);

        var result = await _listHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Items.Select(ToResponse).ToList();
        return Ok(new OpportunityListResponse(items, result.Value.NextCursor));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var query = new GetOpportunityQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            traceId);

        var result = await _getHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var opp = result.Value!;
        Response.Headers.ETag = $"\"{opp.RowVersion}\"";

        return Ok(ToResponse(opp));
    }

    private static OpportunityResponse ToResponse(OpportunityProjection o) => new(
        o.Id,
        o.Code,
        o.CustomerId,
        o.PrimarySiteId,
        o.BranchId,
        o.OwnerUserId,
        o.Title,
        o.ScopeSummary,
        o.WorkTypes,
        o.SourceCode,
        o.ExpectedBudget,
        o.CurrencyCode,
        o.TargetDecisionDate,
        o.NextActionAtUtc,
        o.NextActionNote,
        o.Stage,
        o.RowVersion,
        o.CreatedAtUtc);
}
