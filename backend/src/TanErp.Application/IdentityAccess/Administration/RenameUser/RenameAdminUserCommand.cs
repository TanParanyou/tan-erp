namespace TanErp.Application.IdentityAccess.Administration.RenameUser;

public sealed record RenameAdminUserCommand(AdminCaller Caller, Guid UserId, Guid ExpectedRowVersion, string? DisplayName, string TraceId);
