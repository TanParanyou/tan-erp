namespace TanErp.Application.IdentityAccess.Administration.AssignRole;

public sealed record AssignAdminRoleCommand(AdminCaller Caller, string IdempotencyKey, Guid MembershipId, Guid RoleId, string TraceId);
