namespace TanErp.Domain.IdentityAccess;

/// <summary>
/// The one place that decides whether a user may hold a membership in an organization (ADR 0027 D-A, option A1:
/// one user belongs to exactly one organization, many branches allowed). Every code path that creates or links a
/// Membership for an existing user must ask here. Moving to A2 (many organizations) means changing only this method.
/// </summary>
public static class UserOrganizationPolicy
{
    public static bool MayHoldMembershipIn(IReadOnlyCollection<Guid> existingOrganizationIds, Guid targetOrganizationId) =>
        existingOrganizationIds.All(id => id == targetOrganizationId);
}
