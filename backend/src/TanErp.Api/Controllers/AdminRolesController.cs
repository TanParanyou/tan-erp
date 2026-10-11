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

    [HttpPost("role-definitions")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRoleDefinition([FromBody] SaveRoleDefinitionRequest request, CancellationToken cancellationToken)
    {
        var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (idempotent.IsFailure) return Problem(idempotent.Error);

        var result = await _handler.CreateRoleAsync(
            new AdminCaller(idempotent.Value!.FirebaseUid, idempotent.Value.MembershipId), idempotent.Value.IdempotencyKey,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Created($"/api/v1/admin/role-definitions/{result.Value.Id}", RoleDefinitionResponse.From(result.Value));
    }

    [HttpPut("role-definitions/{roleId:guid}")]
    [ProducesResponseType<UpdateRoleDefinitionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UpdateRoleDefinitionResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRoleDefinition([FromRoute] Guid roleId, [FromBody] SaveRoleDefinitionRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        var result = await _handler.UpdateRoleAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), roleId, conditional.Value.IfMatchRowVersion,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var outcome = result.Value!;
        var body = new UpdateRoleDefinitionResponse(
            RoleDefinitionResponse.From(outcome.Role), outcome.Request is null ? null : RoleChangeRequestResponse.From(outcome.Request));
        if (outcome.Pending) return StatusCode(StatusCodes.Status202Accepted, body);

        Response.Headers.ETag = $"\"{outcome.Role.RowVersion}\"";
        return Ok(body);
    }

    private IActionResult RoleResult(Result<RoleDefinition> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(RoleDefinitionResponse.From(result.Value!));
    }
}
