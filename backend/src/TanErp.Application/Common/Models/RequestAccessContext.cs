namespace TanErp.Application.Common.Models;

public sealed record RequestAccessContext(
    Guid ActorUserId,
    Guid MembershipId,
    Guid OrganizationId,
    Guid? BranchId,
    string PermissionKey,
    string PermissionScope);
