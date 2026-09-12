using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.Users;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.IdentityAccess.Users.ListUsers;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ListUsersHandler _listHandler;

    public UsersController(ListUsersHandler listHandler)
    {
        _listHandler = listHandler;
    }

    [HttpGet]
    [ProducesResponseType<UserListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] Guid? branchId = null,
        [FromQuery] string? search = null,
        [FromQuery] int limit = 25,
        CancellationToken cancellationToken = default)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new ListUsersQuery(auth.FirebaseUid, auth.MembershipId, branchId, search, limit);

        var result = await _listHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Items.Select(u => new UserListItemResponse(
            u.Id,
            u.DisplayName,
            u.Email,
            u.BranchId)).ToList();

        return Ok(new UserListResponse(items, result.Value!.TotalCount));
    }
}
