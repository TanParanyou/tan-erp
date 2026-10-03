namespace TanErp.Application.IdentityAccess.Administration.UpdateMembership;

public sealed record UpdateAdminMembershipCommand(
    AdminCaller Caller, Guid MembershipId, Guid ExpectedRowVersion, Guid? BranchId, DateTimeOffset? StartsAtUtc, DateTimeOffset? ExpiresAtUtc, string TraceId);
