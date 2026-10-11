using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.RoleManagement;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;

namespace TanErp.Api.Controllers;

/// <summary>Role definition administration (G-03b). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public sealed class AdminRolesController : ControllerBase
{
    private readonly RoleManagementHandler _handler;

    public AdminRolesController(RoleManagementHandler handler) => _handler = handler;

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    private static AdminCaller Caller(AuthenticatedRequest a) => new(a.FirebaseUid, a.MembershipId);

    [HttpGet("permissions")]
    [ProducesResponseType<PermissionCatalogResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPermissions(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListPermissionsAsync(Caller(auth.Value!), cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(PermissionCatalogResponse.From(result.Value!));
    }

    [HttpGet("role-definitions")]
    [ProducesResponseType<RoleDefinitionListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListRoleDefinitions(
        [FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null, [FromQuery] int page = 1, [FromQuery] int limit = 25,
        CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListRolesAsync(Caller(auth.Value!), search, status, sortBy, sortOrder, page, limit, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(RoleDefinitionListResponse.From(result.Value!));
    }

    [HttpGet("role-definitions/{roleId:guid}")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleDefinition([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        return RoleResult(await _handler.GetRoleAsync(Caller(auth.Value!), roleId, cancellationToken));
    }

    private IActionResult RoleResult(Result<RoleDefinition> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(RoleDefinitionResponse.From(result.Value!));
    }
}
