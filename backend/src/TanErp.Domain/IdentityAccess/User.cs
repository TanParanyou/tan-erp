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

    public string? PhoneNumber { get; private set; }
    public string? EmployeeCode { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Department? Department { get; set; }
    public Guid? JobTitleId { get; private set; }
    public JobTitle? JobTitle { get; set; }
    public Guid? AvatarFileId { get; private set; }
    public string? Notes { get; private set; }

    public bool IsPending => FirebaseUid is null;

    private readonly List<Membership> _memberships = new();
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();

    protected User() { }

    public User(
        Guid id,
        string firebaseUid,
        string displayName,
        string email,
        bool isActive = true,
        DateTimeOffset? createdAtUtc = null,
        string? phoneNumber = null,
        string? employeeCode = null,
        Guid? departmentId = null,
        Guid? jobTitleId = null,
        Guid? avatarFileId = null,
        string? notes = null)
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
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        EmployeeCode = string.IsNullOrWhiteSpace(employeeCode) ? null : employeeCode.Trim().ToUpperInvariant();
        DepartmentId = departmentId;
        JobTitleId = jobTitleId;
        AvatarFileId = avatarFileId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    /// <summary>Creates an administrator-invited user that has no Firebase identity yet.</summary>
    public static User CreatePending(
        Guid id,
        string displayName,
        string email,
        DateTimeOffset? createdAtUtc = null,
        string? phoneNumber = null,
        string? employeeCode = null,
        Guid? departmentId = null,
        Guid? jobTitleId = null,
        Guid? avatarFileId = null,
        string? notes = null)
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
            RowVersion = Guid.NewGuid(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            EmployeeCode = string.IsNullOrWhiteSpace(employeeCode) ? null : employeeCode.Trim().ToUpperInvariant(),
            DepartmentId = departmentId,
            JobTitleId = jobTitleId,
            AvatarFileId = avatarFileId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
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

    public void UpdateProfile(
        string displayName,
        string? phoneNumber,
        string? employeeCode,
        Guid? departmentId,
        Guid? jobTitleId,
        Guid? avatarFileId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));

        DisplayName = displayName.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        EmployeeCode = string.IsNullOrWhiteSpace(employeeCode) ? null : employeeCode.Trim().ToUpperInvariant();
        DepartmentId = departmentId;
        JobTitleId = jobTitleId;
        AvatarFileId = avatarFileId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void SetAvatar(Guid? avatarFileId)
    {
        AvatarFileId = avatarFileId;
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
    public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }
}
