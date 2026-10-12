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
    public bool IsSystem { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    private readonly List<MembershipRole> _membershipRoles = new();
    public IReadOnlyCollection<MembershipRole> MembershipRoles => _membershipRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    protected Role() { }

    public Role(Guid id, Guid organizationId, string name, string? description = null, bool isActive = true, DateTimeOffset? createdAtUtc = null, bool isSystem = false)
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
        IsSystem = isSystem;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public static Role CreateCustom(Guid id, Guid organizationId, string? name, string? description, DateTimeOffset createdAtUtc)
    {
        var (normalizedName, normalizedDescription) = RoleRules.Normalize(name, description);
        return new Role(id, organizationId, normalizedName, normalizedDescription, isActive: true, createdAtUtc);
    }

    /// <summary>Seed/bootstrap only: no API marks a role as a system role.</summary>
    public void MarkAsSystemRole()
    {
        IsSystem = true;
        RowVersion = Guid.NewGuid();
    }

    public void EnsureMutable()
    {
        if (IsSystem) throw new RoleDomainException("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
    }

    public void UpdateDetails(string? name, string? description)
    {
        EnsureMutable();
        var (normalizedName, normalizedDescription) = RoleRules.Normalize(name, description);
        Name = normalizedName;
        NormalizedName = normalizedName.ToUpperInvariant();
        Description = normalizedDescription;
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate()
    {
        EnsureMutable();
        IsActive = false;
        RowVersion = Guid.NewGuid();
    }

    public void Activate()
    {
        EnsureMutable();
        IsActive = true;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Call after the role's permission grants change so concurrent editors are detected.</summary>
    public void AdvanceVersion() => RowVersion = Guid.NewGuid();
}
