namespace TanErp.Application.IdentityAccess.Administration.RevokeRole;

public sealed record RevokeAdminRoleCommand(AdminCaller Caller, Guid MembershipId, Guid RoleId, string TraceId);
