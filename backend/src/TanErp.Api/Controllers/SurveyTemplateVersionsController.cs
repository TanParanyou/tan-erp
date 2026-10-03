using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Surveys;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Surveys.ListSurveyTemplateVersions;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/survey-template-versions")]
[Authorize]
public class SurveyTemplateVersionsController : ControllerBase
{
    private readonly ListSurveyTemplateVersionsHandler _handler;

    public SurveyTemplateVersionsController(ListSurveyTemplateVersionsHandler handler)
    {
        _handler = handler;
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
                    t.Code, t.RequiredChecklistItems, t.MinimumEvidenceCount, t.SnapshotHashVersion, t.IsCurrent))
                .ToList()));
    }
}
