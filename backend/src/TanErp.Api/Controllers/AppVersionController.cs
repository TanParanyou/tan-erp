using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.AppVersion;
using TanErp.Api.ErrorHandling;
using TanErp.Application.AppVersion;

namespace TanErp.Api.Controllers;

/// <summary>Anonymous by design: the mobile app checks the policy before sign-in. Exposes no user data.</summary>
[ApiController]
[AllowAnonymous]
public class AppVersionController : ControllerBase
{
    private readonly GetAppVersionPolicyHandler _handler;

    public AppVersionController(GetAppVersionPolicyHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("/api/v1/app/version-policy")]
    [ProducesResponseType<AppVersionPolicyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult Get([FromQuery] string? platform)
    {
        var result = _handler.Handle(new GetAppVersionPolicyQuery(platform));
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return Ok(new AppVersionPolicyResponse(result.Value!.MinimumVersion, result.Value.LatestVersion));
    }
}
