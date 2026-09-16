using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Sites;
using TanErp.Api.Contracts.Surveys;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Surveys;
using TanErp.Application.Surveys.CreateSiteSurvey;
using TanErp.Application.Surveys.GetSiteSurvey;
using TanErp.Application.Surveys.MarkSurveyReady;
using TanErp.Application.Surveys.UpdateSurveyDraft;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/opportunities/{opportunityId:guid}/surveys")]
[Authorize]
public class SiteSurveysController : ControllerBase
{
    private readonly CreateSiteSurveyHandler _createHandler;
    private readonly GetSiteSurveyHandler _getHandler;
    private readonly UpdateSurveyDraftHandler _updateDraftHandler;
    private readonly MarkSurveyReadyHandler _markReadyHandler;

    public SiteSurveysController(
        CreateSiteSurveyHandler createHandler,
        GetSiteSurveyHandler getHandler,
        UpdateSurveyDraftHandler updateDraftHandler,
        MarkSurveyReadyHandler markReadyHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _updateDraftHandler = updateDraftHandler;
        _markReadyHandler = markReadyHandler;
    }

    [HttpPost]
    [ProducesResponseType<SiteSurveyResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid opportunityId,
        [FromBody] CreateSiteSurveyRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new CreateSiteSurveyCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            opportunityId,
            request.SiteId,
            request.AssignedSurveyorId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            request.ExpectedOpportunityVersion,
            auth.IdempotencyKey,
            traceId);

        var result = await _createHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var survey = result.Value!;
        Response.Headers.ETag = $"\"{survey.RowVersion}\"";

        return CreatedAtAction(nameof(Get), new { opportunityId = survey.OpportunityId }, ToResponse(survey));
    }

    [HttpGet]
    [ProducesResponseType<SiteSurveyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid opportunityId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new GetSiteSurveyQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            opportunityId);

        var result = await _getHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        if (result.Value == null)
        {
            return NoContent();
        }

        var survey = result.Value;
        Response.Headers.ETag = $"\"{survey.RowVersion}\"";

        return Ok(ToResponse(survey));
    }

    [HttpPut("{surveyId:guid}/revisions/{revisionId:guid}/draft")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromRoute] Guid revisionId,
        [FromBody] UpdateSurveyDraftRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var expectedVersion = auth.IfMatchRowVersion;
        var traceId = HttpContext.TraceIdentifier;

        var areas = request.Areas?.Select(a => new AreaInput(
            a.Id,
            a.Code,
            a.Name,
            a.Description,
            a.SortOrder,
            a.Measurements.Select(m => new MeasurementInput(
                m.Id,
                m.MeasurementType,
                m.Value,
                m.UnitCode,
                m.CaptureMethod,
                m.Notes,
                m.SortOrder)).ToList())).ToList() ?? new List<AreaInput>();

        var command = new UpdateSurveyDraftCommand(
            surveyId,
            revisionId,
            expectedVersion,
            request.VisitedAtUtc,
            request.ScopeSummary,
            request.Assumptions ?? new List<string>(),
            request.Constraints ?? new List<string>(),
            request.MissingDetails ?? new List<string>(),
            areas,
            auth.FirebaseUid,
            auth.MembershipId,
            traceId);

        var result = await _updateDraftHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return Ok(ToRevisionResponse(revision));
    }

    [HttpPost("{surveyId:guid}/revisions/{revisionId:guid}/mark-ready")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> MarkReady(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromRoute] Guid revisionId,
        [FromBody] MarkSurveyReadyRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new MarkSurveyReadyCommand(
            surveyId,
            revisionId,
            request.ExpectedRevisionVersion,
            request.ExpectedOpportunityVersion,
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IdempotencyKey,
            traceId);

        var result = await _markReadyHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return Ok(ToRevisionResponse(revision));
    }

    private static SiteSurveyRevisionResponse ToRevisionResponse(SiteSurveyRevisionProjection r)
    {
        var areas = r.Areas?.Select(a => new SiteSurveyAreaResponse(
            a.Id,
            a.SiteSurveyRevisionId,
            a.Code,
            a.Name,
            a.Description,
            a.SortOrder,
            a.Measurements.Select(m => new SiteSurveyMeasurementResponse(
                m.Id,
                m.SiteSurveyAreaId,
                m.MeasurementType,
                m.Value,
                m.UnitCode,
                m.CaptureMethod,
                m.Notes,
                m.SortOrder)).ToList())).ToList();

        return new SiteSurveyRevisionResponse(
            r.Id,
            r.SiteSurveyId,
            r.RevisionNumber,
            r.SurveyTemplateVersion,
            r.VisitedAtUtc,
            r.ScopeSummary,
            r.Assumptions,
            r.Constraints,
            r.MissingDetails,
            r.Readiness,
            r.Status,
            r.ReadyAtUtc,
            r.ReadyByUserId,
            r.SnapshotHash,
            r.RowVersion,
            r.CreatedAtUtc,
            r.CreatedByUserId,
            areas);
    }

    private static SiteSurveyResponse ToResponse(SiteSurveyProjection s)
    {
        SiteSurveyRevisionResponse? currentRev = null;
        if (s.CurrentRevision != null)
        {
            currentRev = ToRevisionResponse(s.CurrentRevision);
        }

        SurveyorSummaryResponse? assignedSurveyor = s.AssignedSurveyor != null
            ? new SurveyorSummaryResponse(s.AssignedSurveyor.Id, s.AssignedSurveyor.DisplayName, s.AssignedSurveyor.Email)
            : null;

        SiteSummaryResponse? site = s.Site != null
            ? new SiteSummaryResponse(s.Site.Id, s.Site.Label, s.Site.AddressLine1)
            : null;

        return new SiteSurveyResponse(
            s.Id,
            s.OrganizationId,
            s.BranchId,
            s.OpportunityId,
            s.SiteId,
            s.SurveyNumber,
            s.AssignedSurveyorId,
            s.ScheduledStartUtc,
            s.ScheduledEndUtc,
            s.Status,
            s.RowVersion,
            s.CreatedAtUtc,
            s.CreatedByUserId,
            currentRev,
            assignedSurveyor,
            site);
    }
}
