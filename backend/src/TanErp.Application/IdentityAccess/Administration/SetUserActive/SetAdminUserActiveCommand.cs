namespace TanErp.Application.IdentityAccess.Administration.SetUserActive;

public sealed record SetAdminUserActiveCommand(AdminCaller Caller, Guid UserId, Guid ExpectedRowVersion, bool Active, string TraceId);
