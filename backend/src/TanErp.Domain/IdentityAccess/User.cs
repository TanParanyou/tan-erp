using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Domain.IdentityAccess;

public class User : Entity
{
    public string FirebaseUid { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    private readonly List<Membership> _memberships = new();
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();

    protected User() { }

    public User(Guid id, string firebaseUid, string displayName, string email, bool isActive = true, DateTimeOffset? createdAtUtc = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new ArgumentException("Firebase UID cannot be empty.", nameof(firebaseUid));

        FirebaseUid = firebaseUid.Trim();
        DisplayName = displayName?.Trim() ?? string.Empty;
        Email = email?.Trim() ?? string.Empty;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
