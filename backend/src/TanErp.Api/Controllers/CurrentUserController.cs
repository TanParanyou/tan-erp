using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;

namespace TanErp.Api.Controllers;

[ApiController]
public class CurrentUserController : ControllerBase
{
    private readonly GetCurrentUserHandler _handler;
    private readonly UpdateMyProfileHandler _updateMyProfile;

    public CurrentUserController(GetCurrentUserHandler handler, UpdateMyProfileHandler updateMyProfile)
    {
        _handler = handler;
        _updateMyProfile = updateMyProfile;
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

        var verifiedEmail = User.FindFirst("verified_email")?.Value;
        var result = await _handler.Handle(new GetCurrentUserQuery(firebaseUid, verifiedEmail), cancellationToken);

        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var data = result.Value!;
        var response = new CurrentUserResponse(
            ToDto(data.User),
            data.Memberships.Select(m => new MembershipDto(
                m.Id,
                new OrganizationDto(m.Organization.Id, m.Organization.Name),
                m.Branch != null ? new BranchDto(m.Branch.Id, m.Branch.Name) : null,
                m.Permissions.Select(p => new PermissionDto(p.Key, p.Scope, p.ScopeId)).ToList()
            )).ToList()
        );

        return Ok(response);
    }

    [HttpPut("/api/v1/me/profile")]
    [Authorize]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var authenticated = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authenticated.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authenticated.Error.Code, HttpContext);
        }

        var result = await _updateMyProfile.Handle(
            new UpdateMyProfileCommand(
                authenticated.Value!.FirebaseUid,
                authenticated.Value.MembershipId,
                request.DisplayName ?? string.Empty,
                request.PhoneNumber,
                request.PreferredLocale,
                request.AvatarFileId,
                HttpContext.TraceIdentifier),
            cancellationToken);

        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(ToDto(result.Value!));
    }

    private static UserDto ToDto(UserContext user) =>
        new(user.Id, user.DisplayName, user.Email, user.PhoneNumber, user.AvatarFileId, user.PreferredLocale);
}
