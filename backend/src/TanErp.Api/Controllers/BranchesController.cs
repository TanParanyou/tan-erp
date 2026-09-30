using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Common;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/branches")]
public sealed class BranchesController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOrganizationBranchReader _branchReader;

    public BranchesController(IRequestAccessResolver accessResolver, IOrganizationBranchReader branchReader)
    {
        _accessResolver = accessResolver;
        _branchReader = branchReader;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrganizationBranchResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return ProblemDetailsMapper.CreateProblemResult(auth.Error.Code, HttpContext);

        var access = await _accessResolver.ResolveAsync(auth.Value!.FirebaseUid, auth.Value.MembershipId, "organizations.read", cancellationToken);
        if (access.IsFailure) return ProblemDetailsMapper.CreateProblemResult(access.Error.Code, HttpContext);

        var branches = await _branchReader.ListActiveAsync(access.Value!.OrganizationId, cancellationToken);
        return Ok(branches.Select(branch => new OrganizationBranchResponse(branch.Id, branch.Code, branch.Name)).ToArray());
    }
}
