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

    private static AdminCaller Caller(AuthenticatedRequest a) => new(a.FirebaseUid, a.MembershipId);

    [HttpGet("branches")]
    [ProducesResponseType<IReadOnlyList<BranchResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListBranches([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        BranchStatusFilter filter;
        switch (status)
        {
            case null or "": filter = BranchStatusFilter.All; break;
            case "active": filter = BranchStatusFilter.Active; break;
            case "inactive": filter = BranchStatusFilter.Inactive; break;
            default: return Problem(new Error("REQUEST_VALIDATION_FAILED", "Status must be 'active' or 'inactive'."));
        }

        var result = await _handler.ListBranchesAsync(Caller(auth.Value!), filter, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(result.Value!.Select(BranchResponse.From).ToArray());
    }

    [HttpGet("branches/{branchId:guid}")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch([FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        return BranchResult(await _handler.GetBranchAsync(Caller(auth.Value!), branchId, cancellationToken));
    }

    [HttpPost("branches")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (idempotent.IsFailure) return Problem(idempotent.Error);

        var result = await _handler.CreateBranchAsync(
            new AdminCaller(idempotent.Value!.FirebaseUid, idempotent.Value.MembershipId), idempotent.Value.IdempotencyKey,
            new CreateBranchInput(request.Code, new BranchInput(request.Name, request.NameEn, request.TaxBranchCode, request.AddressTh, request.AddressEn, request.Phone)),
            HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Created($"/api/v1/admin/branches/{result.Value.Id}", BranchResponse.From(result.Value));
    }

    private IActionResult BranchResult(Result<BranchDetail> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(BranchResponse.From(result.Value));
    }
}
