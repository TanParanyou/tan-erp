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
[Route("api/v1/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly MasterDataOrganizationHandler _handler;

    public DepartmentsController(MasterDataOrganizationHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<DepartmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] bool onlyActive = true, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.ListDepartmentsAsync(caller, onlyActive, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(result.Value!.Select(d => new DepartmentResponse(d.Id, d.Code, d.Name, d.NameEn, d.IsActive)).ToList());
    }

    [HttpPost]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.CreateDepartmentAsync(caller, new CreateDepartmentInput(request.Code, request.Name, request.NameEn), cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var item = result.Value!;
        return Created($"/api/v1/departments/{item.Id}", new DepartmentResponse(item.Id, item.Code, item.Name, item.NameEn, item.IsActive));
    }

    private ObjectResult Problem(Error error) =>
        ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);
}
