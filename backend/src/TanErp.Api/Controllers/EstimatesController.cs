using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Application.Estimates.CalculateEstimate;
using TanErp.Application.Estimates.CreateEstimateDraft;
using TanErp.Application.Estimates.GetEstimate;
using TanErp.Application.Estimates.UpdateEstimateDraft;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
public class EstimatesController : ControllerBase
{
    private readonly CreateEstimateDraftHandler _createHandler;
    private readonly GetEstimateHandler _getHandler;
    private readonly UpdateEstimateDraftHandler _updateDraftHandler;
    private readonly CalculateEstimateHandler _calculateHandler;

    public EstimatesController(
        CreateEstimateDraftHandler createHandler,
        GetEstimateHandler getHandler,
        UpdateEstimateDraftHandler updateDraftHandler,
        CalculateEstimateHandler calculateHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _updateDraftHandler = updateDraftHandler;
        _calculateHandler = calculateHandler;
    }

    [HttpPost("api/v1/estimates")]
    [ProducesResponseType<EstimateDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEstimateDraftRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var command = new CreateEstimateDraftCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            request.CustomerId,
            request.OpportunityId,
            request.BranchId,
            request.SiteSurveyRevisionId,
            request.SiteSurveySnapshotHash,
            request.Currency);

        var result = await _createHandler.HandleAsync(command, auth.IdempotencyKey, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = EstimateDetailResponse.FromProjection(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("api/v1/estimates/{id:guid}")]
    [ProducesResponseType<EstimateDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _getHandler.HandleByIdAsync(new GetEstimateQuery(auth.FirebaseUid, auth.MembershipId, id), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = EstimateDetailResponse.FromProjection(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpGet("api/v1/opportunities/{opportunityId:guid}/estimates")]
    [ProducesResponseType<EstimateDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByOpportunityId(
        [FromRoute] Guid opportunityId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _getHandler.HandleByOpportunityIdAsync(new GetOpportunityEstimateQuery(auth.FirebaseUid, auth.MembershipId, opportunityId), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        if (result.Value is null)
        {
            return NoContent();
        }

        var response = EstimateDetailResponse.FromProjection(result.Value);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPut("api/v1/estimates/{id:guid}/revisions/{revisionId:guid}/draft")]
    [ProducesResponseType<EstimateRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid id,
        [FromRoute] Guid revisionId,
        [FromBody] UpdateEstimateDraftRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var sectionsDto = request.Sections.Select(s => new EstimateSectionDraftDto(
            s.Id,
            s.Code,
            s.NameTh,
            s.NameEn,
            s.SortOrder,
            (s.WorkItems ?? Array.Empty<UpdateEstimateWorkItemDto>()).Select(w => new EstimateWorkItemDraftDto(
                w.Id,
                w.Code,
                w.DescriptionTh,
                w.DescriptionEn,
                w.Quantity,
                w.UnitCode,
                w.SellingRuleType,
                w.SellingRuleValue,
                w.SortOrder,
                (w.CostComponents ?? Array.Empty<UpdateEstimateCostComponentDto>()).Select(c => new EstimateCostComponentDraftDto(
                    c.Id,
                    c.Type,
                    c.Description,
                    c.Quantity,
                    c.UnitCode,
                    c.UnitCost,
                    c.Currency,
                    c.SortOrder)).ToList())).ToList())).ToList();

        var command = new UpdateEstimateDraftCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            revisionId,
            request.ExpectedRevisionVersion,
            sectionsDto);

        var result = await _updateDraftHandler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = EstimateDetailResponse.EstimateRevisionResponseFromProjection(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("api/v1/estimates/{id:guid}/revisions/{revisionId:guid}/calculate")]
    [ProducesResponseType<EstimateRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Calculate(
        [FromRoute] Guid id,
        [FromRoute] Guid revisionId,
        [FromBody] CalculateEstimateRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var command = new CalculateEstimateCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            revisionId,
            request.ExpectedRevisionVersion,
            request.DiscountAmount);

        var result = await _calculateHandler.HandleAsync(command, auth.IdempotencyKey, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = EstimateDetailResponse.EstimateRevisionResponseFromProjection(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }
}
