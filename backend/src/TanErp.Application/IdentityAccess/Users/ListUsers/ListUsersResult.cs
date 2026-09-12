namespace TanErp.Application.IdentityAccess.Users.ListUsers;

public sealed record ListUsersResult(
    IReadOnlyList<UserListItem> Items,
    int TotalCount
);
