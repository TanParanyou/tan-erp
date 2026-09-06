using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public class Permission : Entity
{
    public string Key { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    protected Permission() { }

    public Permission(Guid id, string key, string? description = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Permission key cannot be blank.", nameof(key));
        }

        Key = key.Trim();
        Description = description;
    }

    public static Permission Create(string key, string? description = null)
    {
        return new Permission(Guid.NewGuid(), key, description);
    }
}
