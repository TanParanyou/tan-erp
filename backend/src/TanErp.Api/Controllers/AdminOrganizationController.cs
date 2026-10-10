using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Common;
using TanErp.Api.Contracts.Organization;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;

namespace TanErp.Api.Controllers;

/// <summary>Organization profile and branch administration (G-03a). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public sealed class AdminOrganizationController : ControllerBase
{
    private readonly OrganizationAdministrationHandler _handler;

    public AdminOrganizationController(OrganizationAdministrationHandler handler) => _handler = handler;

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    [HttpGet("organization")]
    [ProducesResponseType<OrganizationProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.GetProfileAsync(new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), cancellationToken);
        return ProfileResult(result);
    }

    [HttpPut("organization")]
    [ProducesResponseType<OrganizationProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateOrganizationProfileRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        var result = await _handler.UpdateProfileAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), conditional.Value.IfMatchRowVersion,
            new OrganizationProfileInput(request.Name, request.NameEn, request.TaxIdentifier, request.AddressTh, request.AddressEn, request.Phone),
            HttpContext.TraceIdentifier, cancellationToken);
        return ProfileResult(result);
    }

    private IActionResult ProfileResult(Result<OrganizationProfile> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(OrganizationProfileResponse.From(result.Value));
    }
}
