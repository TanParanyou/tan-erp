using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess;
using TanErp.Api.ErrorHandling;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

namespace TanErp.Api.Controllers;

[ApiController]
public class CurrentUserController : ControllerBase
{
    private readonly GetCurrentUserHandler _handler;

    public CurrentUserController(GetCurrentUserHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("/api/v1/me")]
    [Authorize]
    [ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CurrentUserResponse>> Get(CancellationToken cancellationToken)
    {
        var firebaseUid = User.FindFirst("firebase_uid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return ProblemDetailsMapper.CreateProblemResult("AUTHENTICATION_REQUIRED", HttpContext);
        }

        var result = await _handler.Handle(new GetCurrentUserQuery(firebaseUid), cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var data = result.Value!;
        var response = new CurrentUserResponse(
            new UserDto(data.User.Id, data.User.DisplayName, data.User.Email),
            data.Memberships.Select(m => new MembershipDto(
                m.Id,
                new OrganizationDto(m.Organization.Id, m.Organization.Name),
                m.Branch != null ? new BranchDto(m.Branch.Id, m.Branch.Name) : null,
                m.Permissions.Select(p => new PermissionDto(p.Key, p.Scope, p.ScopeId)).ToList()
            )).ToList()
        );

        return Ok(response);
    }
}
