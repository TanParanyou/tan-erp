using TanErp.Domain.Organization;

namespace TanErp.Domain.IdentityAccess;

public class MembershipRole
{
    public Guid MembershipId { get; private set; }
    public Membership? Membership { get; set; }

    public Guid RoleId { get; private set; }
    public Role? Role { get; set; }

    public Guid OrganizationId { get; private set; }

    public DateTimeOffset AssignedAtUtc { get; private set; }

    protected MembershipRole() { }

    public MembershipRole(
        Guid membershipId,
        Guid roleId,
        Guid organizationId,
        DateTimeOffset? assignedAtUtc = null)
    {
        if (membershipId == Guid.Empty)
            throw new ArgumentException("Membership ID cannot be empty.", nameof(membershipId));
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));

        MembershipId = membershipId;
        RoleId = roleId;
        OrganizationId = organizationId;
        AssignedAtUtc = assignedAtUtc ?? DateTimeOffset.UtcNow;
    }
}
