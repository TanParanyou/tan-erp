namespace TanErp.Application.IdentityAccess.Administration.DecideRoleRequest;

public sealed record DecideAdminRoleRequestCommand(AdminCaller Caller, Guid RequestId, Guid ExpectedRowVersion, RoleRequestDecision Decision, string TraceId);
