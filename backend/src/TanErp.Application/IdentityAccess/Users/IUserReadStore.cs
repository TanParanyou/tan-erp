namespace TanErp.Application.IdentityAccess.Users;

public sealed record UserListItem(
    Guid Id,
    string DisplayName,
    string Email,
    Guid? BranchId
);

public interface IUserReadStore
{
    Task<IReadOnlyList<UserListItem>> ListActiveUsersAsync(
        Guid organizationId,
        Guid? branchId,
        string? search,
        int limit,
        CancellationToken cancellationToken = default);
}
