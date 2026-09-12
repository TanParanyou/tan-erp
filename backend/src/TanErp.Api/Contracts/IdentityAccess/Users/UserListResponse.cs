namespace TanErp.Api.Contracts.IdentityAccess.Users;

public record UserListItemResponse(
    Guid Id,
    string DisplayName,
    string Email,
    Guid? BranchId
);

public record UserListResponse(
    IReadOnlyList<UserListItemResponse> Items,
    int TotalCount
);
