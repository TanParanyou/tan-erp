using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public static class RoleAssignmentRequestStatus
{
    public const string Pending = "pending";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";
}

/// <summary>
/// A proposed Role assignment that needs an independent checker (maker-checker) because the Role
/// carries an approval permission. The checker must be a different user than the requester.
/// </summary>
public class RoleAssignmentRequest : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid MembershipId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; }
    public string Status { get; private set; } = RoleAssignmentRequestStatus.Pending;
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected RoleAssignmentRequest() { }

    public RoleAssignmentRequest(
        Guid id,
        Guid organizationId,
        Guid membershipId,
        Guid roleId,
        Guid requestedByUserId,
        DateTimeOffset requestedAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (membershipId == Guid.Empty) throw new ArgumentException("Membership ID cannot be empty.", nameof(membershipId));
        if (roleId == Guid.Empty) throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (requestedByUserId == Guid.Empty) throw new ArgumentException("Requester cannot be empty.", nameof(requestedByUserId));

        OrganizationId = organizationId;
        MembershipId = membershipId;
        RoleId = roleId;
        RequestedByUserId = requestedByUserId;
        RequestedAtUtc = requestedAtUtc;
    }

    public bool IsPending => Status == RoleAssignmentRequestStatus.Pending;

    /// <summary>Returns false when the decider is the requester or the request is no longer pending.</summary>
    public bool TryApprove(Guid deciderUserId, DateTimeOffset now) => TryDecide(RoleAssignmentRequestStatus.Approved, deciderUserId, now, requireIndependent: true);

    public bool TryReject(Guid deciderUserId, DateTimeOffset now) => TryDecide(RoleAssignmentRequestStatus.Rejected, deciderUserId, now, requireIndependent: true);

    /// <summary>Only the requester can withdraw a pending request.</summary>
    public bool TryCancel(Guid actorUserId, DateTimeOffset now)
    {
        if (!IsPending || actorUserId != RequestedByUserId) return false;
        return TryDecide(RoleAssignmentRequestStatus.Cancelled, actorUserId, now, requireIndependent: false);
    }

    private bool TryDecide(string status, Guid deciderUserId, DateTimeOffset now, bool requireIndependent)
    {
        if (!IsPending || deciderUserId == Guid.Empty) return false;
        if (requireIndependent && deciderUserId == RequestedByUserId) return false;

        Status = status;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now;
        RowVersion = Guid.NewGuid();
        return true;
    }
}
