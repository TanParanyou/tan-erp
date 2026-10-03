namespace TanErp.Application.IdentityAccess.Administration.SetMembershipActive;

public sealed record SetAdminMembershipActiveCommand(AdminCaller Caller, Guid MembershipId, Guid ExpectedRowVersion, bool Active, string TraceId);
