using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Domain.IdentityAccess;

public class User : Entity
{
    /// <summary>Null while the user is pending: created by an administrator and not yet linked to a Firebase identity.</summary>
    public string? FirebaseUid { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    public bool IsPending => FirebaseUid is null;

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
        NormalizedEmail = NormalizeEmail(Email);
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    /// <summary>Creates an administrator-invited user that has no Firebase identity yet.</summary>
    public static User CreatePending(Guid id, string displayName, string email, DateTimeOffset? createdAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        var normalized = NormalizeEmail(email);
        if (normalized.Length == 0 || !normalized.Contains('@'))
            throw new ArgumentException("A valid email is required.", nameof(email));

        return new User
        {
            Id = id,
            FirebaseUid = null,
            DisplayName = displayName.Trim(),
            Email = email.Trim(),
            NormalizedEmail = normalized,
            IsActive = true,
            CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow,
            RowVersion = Guid.NewGuid()
        };
    }

    public static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    /// <summary>Links the first verified Firebase identity. An already linked user can never be re-linked.</summary>
    public void LinkFirebaseIdentity(string firebaseUid)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new ArgumentException("Firebase UID cannot be empty.", nameof(firebaseUid));
        if (FirebaseUid is not null)
            throw new InvalidOperationException("User is already linked to a Firebase identity.");

        FirebaseUid = firebaseUid.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void Rename(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        DisplayName = displayName.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
    public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }
}
