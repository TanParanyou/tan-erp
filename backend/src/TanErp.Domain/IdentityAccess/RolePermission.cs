using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Role? Role { get; set; }

    public Guid PermissionId { get; private set; }
    public Permission? Permission { get; set; }

    public string Scope { get; private set; } = string.Empty;
    public Guid? ScopeId { get; private set; }

    public DateTimeOffset AssignedAtUtc { get; private set; }

    protected RolePermission() { }

    public RolePermission(Guid id, Guid roleId, Guid permissionId, string scope, Guid? scopeId = null, DateTimeOffset? assignedAtUtc = null)
        : base(id)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (permissionId == Guid.Empty)
            throw new ArgumentException("Permission ID cannot be empty.", nameof(permissionId));

        var validatedScope = PermissionScope.Create(scope);

        RoleId = roleId;
        PermissionId = permissionId;
        Scope = validatedScope.Value;
        ScopeId = scopeId;
        AssignedAtUtc = assignedAtUtc ?? DateTimeOffset.UtcNow;
    }
}
