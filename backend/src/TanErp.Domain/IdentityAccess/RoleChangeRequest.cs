using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

/// <summary>
/// A proposed definition of an existing Role that adds permissions to a Role people already hold and carries an
/// approval permission. An independent checker must apply it (maker-checker). Mirrors <see cref="RoleAssignmentRequest"/>.
/// </summary>
public class RoleChangeRequest : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; }
    public string Status { get; private set; } = RoleAssignmentRequestStatus.Pending;
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }

    /// <summary>The Role's row version when the request was made; approval is refused if the Role moved since.</summary>
    public Guid BaseRoleRowVersion { get; private set; }

    public string ProposedName { get; private set; } = string.Empty;
    public string? ProposedDescription { get; private set; }

    /// <summary>JSON array of { permissionKey, scope, branchId } — the complete proposed grant set.</summary>
    public string ProposedGrantsJson { get; private set; } = "[]";

    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected RoleChangeRequest() { }

    public RoleChangeRequest(
        Guid id, Guid organizationId, Guid roleId, Guid requestedByUserId, DateTimeOffset requestedAtUtc,
        Guid baseRoleRowVersion, string proposedName, string? proposedDescription, string proposedGrantsJson) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (roleId == Guid.Empty) throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (requestedByUserId == Guid.Empty) throw new ArgumentException("Requester cannot be empty.", nameof(requestedByUserId));
        if (string.IsNullOrWhiteSpace(proposedName)) throw new ArgumentException("Proposed name cannot be empty.", nameof(proposedName));

        OrganizationId = organizationId;
        RoleId = roleId;
        RequestedByUserId = requestedByUserId;
        RequestedAtUtc = requestedAtUtc;
        BaseRoleRowVersion = baseRoleRowVersion;
        ProposedName = proposedName;
        ProposedDescription = proposedDescription;
        ProposedGrantsJson = proposedGrantsJson;
    }

    public bool IsPending => Status == RoleAssignmentRequestStatus.Pending;

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
