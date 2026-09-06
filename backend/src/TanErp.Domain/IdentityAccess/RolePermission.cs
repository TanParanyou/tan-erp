using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Role? Role { get; set; }

    public Guid OrganizationId { get; private set; }

    public Guid PermissionId { get; private set; }
    public Permission? Permission { get; set; }

    public string Scope { get; private set; } = string.Empty;
    public Guid? ScopeId { get; private set; }
    public Guid? BranchId { get; private set; }

    public DateTimeOffset AssignedAtUtc { get; private set; }

    protected RolePermission() { }

    public RolePermission(
        Guid id,
        Guid roleId,
        Guid organizationId,
        Guid permissionId,
        string scope,
        Guid? scopeId = null,
        Guid? branchId = null,
        DateTimeOffset? assignedAtUtc = null)
        : base(id)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty.", nameof(permissionId));

        var validatedScope = PermissionScope.Create(scope);

        if (validatedScope.Value == PermissionScope.Organization)
        {
            if (scopeId.HasValue && scopeId.Value != organizationId)
                throw new ArgumentException("Organization scope ID must match the role organization ID.", nameof(scopeId));
            if (branchId.HasValue)
                throw new ArgumentException("Branch ID must be null for organization scope.", nameof(branchId));

            scopeId = organizationId;
            branchId = null;
        }
        else if (validatedScope.Value == PermissionScope.Branch)
        {
            if (!branchId.HasValue && !scopeId.HasValue)
                throw new ArgumentException("Branch ID is required for branch scope.", nameof(branchId));

            branchId ??= scopeId;
            scopeId ??= branchId;

            if (branchId != scopeId)
                throw new ArgumentException("Scope ID must match Branch ID for branch scope.", nameof(scopeId));
        }
        else if (validatedScope.Value == PermissionScope.Own)
        {
            if (scopeId.HasValue || branchId.HasValue)
                throw new ArgumentException("Scope ID and Branch ID must be null for own scope.", nameof(scopeId));

            scopeId = null;
            branchId = null;
        }

        RoleId = roleId;
        OrganizationId = organizationId;
        PermissionId = permissionId;
        Scope = validatedScope.Value;
        ScopeId = scopeId;
        BranchId = branchId;
        AssignedAtUtc = assignedAtUtc ?? DateTimeOffset.UtcNow;
    }
}
