using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Domain.IdentityAccess;

public class Role : Entity
{
    public Guid OrganizationId { get; private set; }
    public Organization.Organization? Organization { get; set; }

    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private readonly List<MembershipRole> _membershipRoles = new();
    public IReadOnlyCollection<MembershipRole> MembershipRoles => _membershipRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    protected Role() { }

    public Role(Guid id, Guid organizationId, string name, string? description = null, bool isActive = true, DateTimeOffset? createdAtUtc = null)
        : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        OrganizationId = organizationId;
        Name = name.Trim();
        NormalizedName = name.Trim().ToUpperInvariant();
        Description = description;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }
}
