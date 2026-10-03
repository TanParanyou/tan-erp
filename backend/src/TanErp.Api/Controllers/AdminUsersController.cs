using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.Administration.AssignRole;
using TanErp.Application.IdentityAccess.Administration.CreateUser;
using TanErp.Application.IdentityAccess.Administration.DecideRoleRequest;
using TanErp.Application.IdentityAccess.Administration.GetUser;
using TanErp.Application.IdentityAccess.Administration.ListRoleRequests;
using TanErp.Application.IdentityAccess.Administration.ListRoles;
using TanErp.Application.IdentityAccess.Administration.ListUsers;
using TanErp.Application.IdentityAccess.Administration.RenameUser;
using TanErp.Application.IdentityAccess.Administration.RevokeRole;
using TanErp.Application.IdentityAccess.Administration.SetMembershipActive;
using TanErp.Application.IdentityAccess.Administration.SetUserActive;
using TanErp.Application.IdentityAccess.Administration.UpdateMembership;

namespace TanErp.Api.Controllers;

/// <summary>Users, memberships and role assignment administration (CP-02). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public class AdminUsersController : ControllerBase
{
    private readonly ListAdminUsersHandler _listUsers;
    private readonly GetAdminUserHandler _getUser;
    private readonly CreateAdminUserHandler _createUser;
    private readonly RenameAdminUserHandler _renameUser;
    private readonly SetAdminUserActiveHandler _setUserActive;
    private readonly UpdateAdminMembershipHandler _updateMembership;
    private readonly SetAdminMembershipActiveHandler _setMembershipActive;
    private readonly ListAdminRolesHandler _listRoles;
    private readonly AssignAdminRoleHandler _assignRole;
    private readonly RevokeAdminRoleHandler _revokeRole;
    private readonly ListAdminRoleRequestsHandler _listRoleRequests;
    private readonly DecideAdminRoleRequestHandler _decideRoleRequest;

    public AdminUsersController(
        ListAdminUsersHandler listUsers,
        GetAdminUserHandler getUser,
        CreateAdminUserHandler createUser,
        RenameAdminUserHandler renameUser,
        SetAdminUserActiveHandler setUserActive,
        UpdateAdminMembershipHandler updateMembership,
        SetAdminMembershipActiveHandler setMembershipActive,
        ListAdminRolesHandler listRoles,
        AssignAdminRoleHandler assignRole,
        RevokeAdminRoleHandler revokeRole,
        ListAdminRoleRequestsHandler listRoleRequests,
        DecideAdminRoleRequestHandler decideRoleRequest)
    {
        _listUsers = listUsers;
        _getUser = getUser;
        _createUser = createUser;
        _renameUser = renameUser;
        _setUserActive = setUserActive;
        _updateMembership = updateMembership;
        _setMembershipActive = setMembershipActive;
        _listRoles = listRoles;
        _assignRole = assignRole;
        _revokeRole = revokeRole;
        _listRoleRequests = listRoleRequests;
        _decideRoleRequest = decideRoleRequest;
    }

    [HttpGet("users")]
    [ProducesResponseType<AdminUserListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUsers(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25,
        CancellationToken cancellationToken = default)
    {
        var auth = ReadAuth(out var failure);
        if (failure is not null) return failure;

        var result = await _listUsers.Handle(new ListAdminUsersQuery(auth!, search, status, sortBy, sortOrder, page, limit), cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var data = result.Value!;
        var totalPages = (int)Math.Ceiling(data.TotalCount / (double)data.PageSize);
        return Ok(new AdminUserListResponse(
            data.Items.Select(AdminUserResponse.From).ToList(),
            new AdminPaginationResponse(data.Page, data.PageSize, data.TotalCount, totalPages)));
    }

    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var auth = ReadAuth(out var failure);
        if (failure is not null) return failure;

        var result = await _getUser.Handle(new GetAdminUserQuery(auth!, userId), cancellationToken);
        return UserResult(result);
    }

    [HttpPost("users")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateAdminUserRequest request, CancellationToken cancellationToken)
    {
        var idempotent = ReadIdempotent(out var failure);
        if (failure is not null) return failure;

        var result = await _createUser.Handle(
            new CreateAdminUserCommand(
                idempotent!.Caller, idempotent.Key, request.DisplayName, request.Email, request.BranchId, request.RoleIds, HttpContext.TraceIdentifier),
            cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var user = result.Value!;
        Response.Headers.ETag = $"\"{user.RowVersion}\"";
        return Created($"/api/v1/admin/users/{user.Id}", AdminUserResponse.From(user));
    }

    [HttpPatch("users/{userId:guid}")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RenameUser([FromRoute] Guid userId, [FromBody] RenameAdminUserRequest request, CancellationToken cancellationToken)
    {
        var conditional = ReadConditional(out var failure);
        if (failure is not null) return failure;

        var result = await _renameUser.Handle(
            new RenameAdminUserCommand(conditional!.Caller, userId, conditional.RowVersion, request.DisplayName, HttpContext.TraceIdentifier),
            cancellationToken);
        return UserResult(result);
    }

    [HttpPost("users/{userId:guid}/deactivate")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> DeactivateUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        SetUserActive(userId, active: false, cancellationToken);

    [HttpPost("users/{userId:guid}/activate")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> ActivateUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        SetUserActive(userId, active: true, cancellationToken);

    [HttpPatch("memberships/{membershipId:guid}")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMembership(
        [FromRoute] Guid membershipId, [FromBody] UpdateAdminMembershipRequest request, CancellationToken cancellationToken)
    {
        var conditional = ReadConditional(out var failure);
        if (failure is not null) return failure;

        var result = await _updateMembership.Handle(
            new UpdateAdminMembershipCommand(
                conditional!.Caller, membershipId, conditional.RowVersion, request.BranchId, request.StartsAtUtc, request.ExpiresAtUtc,
                HttpContext.TraceIdentifier),
            cancellationToken);
        return UserResult(result);
    }

    [HttpPost("memberships/{membershipId:guid}/deactivate")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> DeactivateMembership([FromRoute] Guid membershipId, CancellationToken cancellationToken) =>
        SetMembershipActive(membershipId, active: false, cancellationToken);

    [HttpPost("memberships/{membershipId:guid}/activate")]
    [ProducesResponseType<AdminUserResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> ActivateMembership([FromRoute] Guid membershipId, CancellationToken cancellationToken) =>
        SetMembershipActive(membershipId, active: true, cancellationToken);

    [HttpGet("roles")]
    [ProducesResponseType<AdminRoleListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListRoles(CancellationToken cancellationToken)
    {
        var auth = ReadAuth(out var failure);
        if (failure is not null) return failure;

        var result = await _listRoles.Handle(new ListAdminRolesQuery(auth!), cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(new AdminRoleListResponse(result.Value!
            .Select(r => new AdminRoleResponse(r.Id, r.Name, r.Assignable, r.RequiresApproval, r.PermissionKeys)).ToList()));
    }

    [HttpPost("memberships/{membershipId:guid}/roles")]
    [ProducesResponseType<AdminAssignRoleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<AdminAssignRoleResponse>(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> AssignRole(
        [FromRoute] Guid membershipId, [FromBody] AssignAdminRoleRequest request, CancellationToken cancellationToken)
    {
        var idempotent = ReadIdempotent(out var failure);
        if (failure is not null) return failure;

        var result = await _assignRole.Handle(
            new AssignAdminRoleCommand(idempotent!.Caller, idempotent.Key, membershipId, request.RoleId, HttpContext.TraceIdentifier),
            cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var outcome = result.Value!;
        var body = new AdminAssignRoleResponse(
            AdminUserResponse.From(outcome.User),
            outcome.Request is null ? null : AdminRoleRequestResponse.From(outcome.Request));
        return outcome.Pending
            ? StatusCode(StatusCodes.Status202Accepted, body)
            : StatusCode(StatusCodes.Status201Created, body);
    }

    [HttpDelete("memberships/{membershipId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeRole([FromRoute] Guid membershipId, [FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var auth = ReadAuth(out var failure);
        if (failure is not null) return failure;

        var result = await _revokeRole.Handle(
            new RevokeAdminRoleCommand(auth!, membershipId, roleId, HttpContext.TraceIdentifier), cancellationToken);
        return result.IsFailure ? Problem(result.Error) : NoContent();
    }

    [HttpGet("role-assignment-requests")]
    [ProducesResponseType<AdminRoleRequestListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListRoleRequests([FromQuery] string? status = null, CancellationToken cancellationToken = default)
    {
        var auth = ReadAuth(out var failure);
        if (failure is not null) return failure;

        var result = await _listRoleRequests.Handle(new ListAdminRoleRequestsQuery(auth!, status), cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(new AdminRoleRequestListResponse(result.Value!.Select(AdminRoleRequestResponse.From).ToList()));
    }

    [HttpPost("role-assignment-requests/{requestId:guid}/approve")]
    [ProducesResponseType<AdminRoleRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> ApproveRoleRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideRoleRequest(requestId, RoleRequestDecision.Approve, cancellationToken);

    [HttpPost("role-assignment-requests/{requestId:guid}/reject")]
    [ProducesResponseType<AdminRoleRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> RejectRoleRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideRoleRequest(requestId, RoleRequestDecision.Reject, cancellationToken);

    [HttpPost("role-assignment-requests/{requestId:guid}/cancel")]
    [ProducesResponseType<AdminRoleRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> CancelRoleRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideRoleRequest(requestId, RoleRequestDecision.Cancel, cancellationToken);

    private async Task<IActionResult> SetUserActive(Guid userId, bool active, CancellationToken cancellationToken)
    {
        var conditional = ReadConditional(out var failure);
        if (failure is not null) return failure;

        var result = await _setUserActive.Handle(
            new SetAdminUserActiveCommand(conditional!.Caller, userId, conditional.RowVersion, active, HttpContext.TraceIdentifier),
            cancellationToken);
        return UserResult(result);
    }

    private async Task<IActionResult> SetMembershipActive(Guid membershipId, bool active, CancellationToken cancellationToken)
    {
        var conditional = ReadConditional(out var failure);
        if (failure is not null) return failure;

        var result = await _setMembershipActive.Handle(
            new SetAdminMembershipActiveCommand(conditional!.Caller, membershipId, conditional.RowVersion, active, HttpContext.TraceIdentifier),
            cancellationToken);
        return UserResult(result);
    }

    private async Task<IActionResult> DecideRoleRequest(Guid requestId, RoleRequestDecision decision, CancellationToken cancellationToken)
    {
        var conditional = ReadConditional(out var failure);
        if (failure is not null) return failure;

        var result = await _decideRoleRequest.Handle(
            new DecideAdminRoleRequestCommand(conditional!.Caller, requestId, conditional.RowVersion, decision, HttpContext.TraceIdentifier),
            cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(AdminRoleRequestResponse.From(result.Value!));
    }

    private IActionResult UserResult(Result<AdminUser> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        var user = result.Value!;
        Response.Headers.ETag = $"\"{user.RowVersion}\"";
        return Ok(AdminUserResponse.From(user));
    }

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    private AdminCaller? ReadAuth(out IActionResult? failure)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure)
        {
            failure = Problem(context.Error);
            return null;
        }

        failure = null;
        return new AdminCaller(context.Value!.FirebaseUid, context.Value.MembershipId);
    }

    private ConditionalCaller? ReadConditional(out IActionResult? failure)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure)
        {
            failure = Problem(context.Error);
            return null;
        }

        failure = null;
        return new ConditionalCaller(new AdminCaller(context.Value!.FirebaseUid, context.Value.MembershipId), context.Value.IfMatchRowVersion);
    }

    private IdempotentCaller? ReadIdempotent(out IActionResult? failure)
    {
        var context = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (context.IsFailure)
        {
            failure = Problem(context.Error);
            return null;
        }

        failure = null;
        return new IdempotentCaller(new AdminCaller(context.Value!.FirebaseUid, context.Value.MembershipId), context.Value.IdempotencyKey);
    }

    private sealed record ConditionalCaller(AdminCaller Caller, Guid RowVersion);

    private sealed record IdempotentCaller(AdminCaller Caller, string Key);
}
