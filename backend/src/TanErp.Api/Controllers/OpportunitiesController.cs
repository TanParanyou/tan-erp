using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Opportunities;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.GetOpportunity;
using TanErp.Application.Crm.Opportunities.ListOpportunities;
using TanErp.Application.Crm.Opportunities.QualifyOpportunity;
using TanErp.Application.Crm.Opportunities.UpdateDraftQGate;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/opportunities")]
[Authorize]
public class OpportunitiesController : ControllerBase
{
    private readonly CreateOpportunityHandler _createHandler;
    private readonly ListOpportunitiesHandler _listHandler;
    private readonly GetOpportunityHandler _getHandler;
    private readonly QualifyOpportunityHandler _qualifyHandler;
    private readonly UpdateDraftQGateHandler _updateDraftQGateHandler;
    private readonly TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity.UpdateOpenOpportunityHandler _updateOpenHandler;
    private readonly TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner.ReassignOpportunityOwnerHandler _reassignOwnerHandler;
    private readonly TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory.GetOpportunityStageHistoryHandler _getStageHistoryHandler;

    public OpportunitiesController(
        CreateOpportunityHandler createHandler,
        ListOpportunitiesHandler listHandler,
        GetOpportunityHandler getHandler,
        QualifyOpportunityHandler qualifyHandler,
        UpdateDraftQGateHandler updateDraftQGateHandler,
        TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity.UpdateOpenOpportunityHandler updateOpenHandler,
        TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner.ReassignOpportunityOwnerHandler reassignOwnerHandler,
        TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory.GetOpportunityStageHistoryHandler getStageHistoryHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _qualifyHandler = qualifyHandler;
        _updateDraftQGateHandler = updateDraftQGateHandler;
        _updateOpenHandler = updateOpenHandler;
        _reassignOwnerHandler = reassignOwnerHandler;
        _getStageHistoryHandler = getStageHistoryHandler;
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
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] int? page = null,
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
            sortBy,
            sortOrder,
            page,
            limit,
            cursor,
            traceId);

        var result = await _listHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Items.Select(ToResponse).ToList();
        var totalCount = result.Value.TotalCount;
        var pageSize = result.Value.PageSize;
        var currentPage = result.Value.Page;
        var totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;

        var pagination = new TanErp.Api.Contracts.Common.PaginationMetadataResponse(
            currentPage,
            pageSize,
            totalCount,
            totalPages,
            result.Value.NextCursor);

        return Ok(new OpportunityListResponse(items, pagination));
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

    [HttpPost("{id:guid}/stage-transitions")]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> TransitionStage(
        [FromRoute] Guid id,
        [FromBody] TransitionOpportunityStageRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new QualifyOpportunityCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            request.TargetStage,
            request.ExpectedVersion,
            auth.IdempotencyKey,
            traceId,
            request.ReasonCode,
            request.Note);

        var result = await _qualifyHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var opp = result.Value!;
        Response.Headers.ETag = $"\"{opp.RowVersion}\"";

        return Ok(ToResponse(opp));
    }

    [HttpGet("{id:guid}/stage-history")]
    [ProducesResponseType<OpportunityStageHistoryListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStageHistory(
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

        var query = new TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory.GetOpportunityStageHistoryQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            traceId);

        var result = await _getStageHistoryHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Select(h => new OpportunityStageHistoryItemResponse(
            h.Id,
            h.OpportunityId,
            h.FromStage,
            h.ToStage,
            h.ReasonCode,
            h.Note,
            h.ActorUserId,
            h.OccurredAtUtc,
            h.PolicyVersion)).ToList();

        return Ok(new OpportunityStageHistoryListResponse(items));
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Patch(
        [FromRoute] Guid id,
        [FromBody] UpdateDraftQGateRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new UpdateDraftQGateCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            auth.IfMatchRowVersion,
            request.ScopeSummary,
            request.WorkTypes,
            request.NextActionAtUtc,
            request.NextActionNote,
            auth.IdempotencyKey,
            traceId);

        var result = await _updateDraftQGateHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var opp = result.Value!;
        Response.Headers.ETag = $"\"{opp.RowVersion}\"";

        return Ok(ToResponse(opp));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateOpen(
        [FromRoute] Guid id,
        [FromBody] UpdateOpenOpportunityRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity.UpdateOpenOpportunityCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            auth.IfMatchRowVersion,
            request.Title,
            request.PrimarySiteId,
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

        var result = await _updateOpenHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var opp = result.Value!;
        Response.Headers.ETag = $"\"{opp.RowVersion}\"";

        return Ok(ToResponse(opp));
    }

    [HttpPost("{id:guid}/owner-changes")]
    [ProducesResponseType<OpportunityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReassignOwner(
        [FromRoute] Guid id,
        [FromBody] ReassignOpportunityOwnerRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner.ReassignOpportunityOwnerCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            request.ExpectedVersion,
            request.TargetOwnerUserId,
            auth.IdempotencyKey,
            traceId);

        var result = await _reassignOwnerHandler.Handle(command, cancellationToken);
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
