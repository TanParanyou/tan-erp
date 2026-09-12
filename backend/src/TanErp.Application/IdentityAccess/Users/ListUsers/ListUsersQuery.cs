namespace TanErp.Application.IdentityAccess.Users.ListUsers;

public sealed record ListUsersQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid? BranchId = null,
    string? Search = null,
    int Limit = 25
);
