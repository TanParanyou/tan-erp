using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Domain.Organization;

public class Membership : Entity
{
    public Guid OrganizationId { get; private set; }
    public Organization? Organization { get; set; }

    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; set; }

    public Guid UserId { get; private set; }
    public User? User { get; set; }

    public bool IsActive { get; private set; }
    public DateTimeOffset? StartsAtUtc { get; private set; }
    public DateTimeOffset? ExpiresAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private readonly List<MembershipRole> _membershipRoles = new();
    public IReadOnlyCollection<MembershipRole> MembershipRoles => _membershipRoles.AsReadOnly();

    protected Membership() { }

    public Membership(
        Guid id,
        Guid organizationId,
        Guid? branchId,
        Guid userId,
        bool isActive = true,
        DateTimeOffset? startsAtUtc = null,
        DateTimeOffset? expiresAtUtc = null,
        DateTimeOffset? createdAtUtc = null) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        OrganizationId = organizationId;
        BranchId = branchId;
        UserId = userId;
        IsActive = isActive;
        StartsAtUtc = startsAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public bool IsActiveAt(DateTimeOffset instant)
    {
        if (!IsActive) return false;
        if (StartsAtUtc.HasValue && instant < StartsAtUtc.Value) return false;
        if (ExpiresAtUtc.HasValue && instant >= ExpiresAtUtc.Value) return false;
        if (User != null && !User.IsActive) return false;
        if (Organization != null && !Organization.IsActive) return false;
        if (Branch != null && !Branch.IsActive) return false;

        return true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
