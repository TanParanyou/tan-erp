namespace TanErp.Application.IdentityAccess.Administration.ListUsers;

public sealed record ListAdminUsersQuery(
    AdminCaller Caller, string? Search, string? Status, string? SortBy, string? SortOrder, int Page, int Limit);
