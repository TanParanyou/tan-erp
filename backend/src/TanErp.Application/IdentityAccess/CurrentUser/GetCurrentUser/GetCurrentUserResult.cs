namespace TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

public sealed record GetCurrentUserResult(
    UserContext User,
    IReadOnlyList<MembershipContext> Memberships);

public sealed record UserContext(
    Guid Id,
    string DisplayName,
    string Email);

public sealed record MembershipContext(
    Guid Id,
    OrganizationContext Organization,
    BranchContext? Branch,
    IReadOnlyList<PermissionContext> Permissions);

public sealed record OrganizationContext(
    Guid Id,
    string Name);

public sealed record BranchContext(
    Guid Id,
    string Name);

public sealed record PermissionContext(
    string Key,
    string Scope,
    Guid? ScopeId);
