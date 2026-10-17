using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Surveys;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Surveys;
using TanErp.Application.Surveys.ListSurveyTemplateVersions;
using TanErp.Domain.Surveys;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/survey-template-versions")]
[Authorize]
public class SurveyTemplateVersionsController : ControllerBase
{
    private readonly ListSurveyTemplateVersionsHandler _handler;
    private readonly ManageSurveyTemplateVersionsHandler _manageHandler;

    public SurveyTemplateVersionsController(ListSurveyTemplateVersionsHandler handler, ManageSurveyTemplateVersionsHandler manageHandler)
    {
        _handler = handler;
        _manageHandler = manageHandler;
    }

    [HttpGet]
    [ProducesResponseType<SurveyTemplateVersionListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _handler.Handle(new ListSurveyTemplateVersionsQuery(auth.FirebaseUid, auth.MembershipId), cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(new SurveyTemplateVersionListResponse(
            result.Value!
                .Select(t => new SurveyTemplateVersionResponse(
                    t.Code, t.RequiredChecklistItems, t.MinimumEvidenceCount, t.MinimumAreaEvidenceCount, t.SnapshotHashVersion,
                    t.EffectiveFromUtc, t.EffectiveToUtc, t.IsCurrent))
                .ToList()));
    }

    /// <summary>Publishes a new immutable template version (survey-templates.manage).</summary>
    [HttpPost]
    [ProducesResponseType<SurveyTemplateVersionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Publish([FromBody] PublishSurveyTemplateVersionRequest request, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _manageHandler.Publish(
            auth.FirebaseUid, auth.MembershipId,
            new PublishSurveyTemplateCommand(
                request.Code, request.RequiredChecklistItems ?? Array.Empty<string>(), request.MinimumEvidenceCount,
                request.MinimumAreaEvidenceCount, request.SnapshotHashVersion, request.EffectiveFromUtc),
            cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return StatusCode(StatusCodes.Status201Created, ToResponse(result.Value!));
    }

    /// <summary>Ends the effective period of a template now; surveys already created keep it.</summary>
    [HttpPost("{code}/retire")]
    [ProducesResponseType<SurveyTemplateVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retire(string code, CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var result = await _manageHandler.Retire(auth.FirebaseUid, auth.MembershipId, code, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(ToResponse(result.Value!));
    }

    private static SurveyTemplateVersionResponse ToResponse(SurveyTemplateVersion t) =>
        new(t.Code, t.RequiredChecklistItems, t.MinimumEvidenceCount, t.MinimumAreaEvidenceCount, t.SnapshotHashVersion,
            t.EffectiveFromUtc, t.EffectiveToUtc, IsCurrent: false);
}
