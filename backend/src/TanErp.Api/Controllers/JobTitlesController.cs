using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Organization;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.MasterData;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/job-titles")]
public class JobTitlesController : ControllerBase
{
    private readonly MasterDataOrganizationHandler _handler;

    public JobTitlesController(MasterDataOrganizationHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<JobTitleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] Guid? departmentId = null, [FromQuery] bool onlyActive = true, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.ListJobTitlesAsync(caller, departmentId, onlyActive, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(result.Value!.Select(j => new JobTitleResponse(j.Id, j.DepartmentId, j.Code, j.Name, j.NameEn, j.IsActive)).ToList());
    }

    [HttpPost]
    [ProducesResponseType<JobTitleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateJobTitleRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.CreateJobTitleAsync(caller, new CreateJobTitleInput(request.Code, request.Name, request.DepartmentId, request.NameEn), cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var item = result.Value!;
        return Created($"/api/v1/job-titles/{item.Id}", new JobTitleResponse(item.Id, item.DepartmentId, item.Code, item.Name, item.NameEn, item.IsActive));
    }

    private ObjectResult Problem(Error error) =>
        ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);
}
