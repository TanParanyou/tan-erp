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
    /// <summary>Self-selected UI language ("th" or "en"); null means follow the browser/route default.</summary>
    public string? PreferredLocale { get; private set; }

    public static readonly IReadOnlyList<string> SupportedLocales = new[] { "th", "en" };

    public const string ThemeModeLight = "light";
    public const string ThemeModeDark = "dark";
    public const string ThemeModeSystem = "system";
    public const string DensityComfortable = "comfortable";
    public const string DensityCompact = "compact";
    public const int MinTextScale = 90;
    public const int MaxTextScale = 130;
    public static readonly IReadOnlyList<string> SupportedThemeModes = new[] { ThemeModeLight, ThemeModeDark, ThemeModeSystem };
    public static readonly IReadOnlyList<string> SupportedDensities = new[] { DensityComfortable, DensityCompact };

    /// <summary>Self-selected colour mode: "light", "dark" or "system" (follow the device).</summary>
    public string ThemeMode { get; private set; } = ThemeModeSystem;
    /// <summary>Self-selected layout density: "comfortable" or "compact".</summary>
    public string Density { get; private set; } = DensityComfortable;
    /// <summary>Self-selected text size as a percentage, 90-130.</summary>
    public int TextScale { get; private set; } = 100;

    public const int MaxFailedLoginAttempts = 5;
    public static readonly TimeSpan DefaultLockoutDuration = TimeSpan.FromMinutes(15);

    public string? PasswordHash { get; private set; }
    public DateTimeOffset? PasswordChangedAtUtc { get; private set; }
    public string? GoogleSubjectId { get; private set; }
    public string? TotpSecretEncrypted { get; private set; }
    public string? PendingTotpSecretEncrypted { get; private set; }
    public bool IsTotpEnabled { get; private set; }
    public DateTimeOffset? TotpEnabledAtUtc { get; private set; }
    public bool RequireTwoFactor { get; private set; }
    public int FailedLoginCount { get; private set; }
    public DateTimeOffset? LockoutEndUtc { get; private set; }

    public bool IsPending => FirebaseUid is null && PasswordHash is null && GoogleSubjectId is null;

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

    /// <summary>Self-service profile edit: only the fields a user may change about themselves.</summary>
    public void UpdateOwnProfile(string displayName, string? phoneNumber, string? preferredLocale, Guid? avatarFileId)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));

        var locale = string.IsNullOrWhiteSpace(preferredLocale) ? null : preferredLocale.Trim().ToLowerInvariant();
        if (locale is not null && !SupportedLocales.Contains(locale))
            throw new ArgumentException("Preferred locale is not supported.", nameof(preferredLocale));

        DisplayName = displayName.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        PreferredLocale = locale;
        AvatarFileId = avatarFileId;
        RowVersion = Guid.NewGuid();
    }

    public void UpdateThemePreferences(string themeMode, string density, int textScale, string? preferredLocale = null)
    {
        if (!SupportedThemeModes.Contains(themeMode))
            throw new ArgumentException("Theme mode is not supported.", nameof(themeMode));
        if (!SupportedDensities.Contains(density))
            throw new ArgumentException("Density is not supported.", nameof(density));
        if (textScale is < MinTextScale or > MaxTextScale)
            throw new ArgumentException("Text scale must be between 90 and 130.", nameof(textScale));

        ThemeMode = themeMode;
        Density = density;
        TextScale = textScale;
        if (preferredLocale is not null)
        {
            if (!SupportedLocales.Contains(preferredLocale))
                throw new ArgumentException("Preferred locale is not supported.", nameof(preferredLocale));
            PreferredLocale = preferredLocale;
        }
        RowVersion = Guid.NewGuid();
    }

    public void SetAvatar(Guid? avatarFileId)
    {
        AvatarFileId = avatarFileId;
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
    public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }

    public string EnsureSubjectUid()
    {
        if (string.IsNullOrWhiteSpace(FirebaseUid))
        {
            FirebaseUid = $"usr_{Id:N}";
            RowVersion = Guid.NewGuid();
        }

        return FirebaseUid;
    }

    public void SetPassword(string passwordHash, DateTimeOffset changedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        PasswordHash = passwordHash;
        PasswordChangedAtUtc = changedAtUtc;
        FailedLoginCount = 0;
        LockoutEndUtc = null;
        EnsureSubjectUid();
        RowVersion = Guid.NewGuid();
    }

    public void LinkGoogleAccount(string googleSubjectId)
    {
        if (string.IsNullOrWhiteSpace(googleSubjectId))
            throw new ArgumentException("Google subject ID cannot be empty.", nameof(googleSubjectId));

        var trimmed = googleSubjectId.Trim();
        if (!string.IsNullOrWhiteSpace(GoogleSubjectId) && !string.Equals(GoogleSubjectId, trimmed, StringComparison.Ordinal))
            throw new InvalidOperationException("User is already linked to a different Google account.");

        GoogleSubjectId = trimmed;
        EnsureSubjectUid();
        RowVersion = Guid.NewGuid();
    }

    public void SetRequireTwoFactor(bool requireTwoFactor)
    {
        RequireTwoFactor = requireTwoFactor;
        RowVersion = Guid.NewGuid();
    }

    public void BeginTotpSetup(string pendingTotpSecretEncrypted)
    {
        if (string.IsNullOrWhiteSpace(pendingTotpSecretEncrypted))
            throw new ArgumentException("Encrypted pending TOTP secret cannot be empty.", nameof(pendingTotpSecretEncrypted));

        PendingTotpSecretEncrypted = pendingTotpSecretEncrypted;
        RowVersion = Guid.NewGuid();
    }

    public void EnableTotp(DateTimeOffset enabledAtUtc)
    {
        if (string.IsNullOrWhiteSpace(PendingTotpSecretEncrypted))
            throw new InvalidOperationException("Pending TOTP setup secret is required before enabling 2FA.");

        TotpSecretEncrypted = PendingTotpSecretEncrypted;
        PendingTotpSecretEncrypted = null;
        IsTotpEnabled = true;
        TotpEnabledAtUtc = enabledAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void DisableTotp()
    {
        TotpSecretEncrypted = null;
        PendingTotpSecretEncrypted = null;
        IsTotpEnabled = false;
        TotpEnabledAtUtc = null;
        RowVersion = Guid.NewGuid();
    }

    public bool IsLockedOut(DateTimeOffset now) =>
        LockoutEndUtc.HasValue && LockoutEndUtc.Value > now;

    public void RecordFailedLogin(DateTimeOffset now)
    {
        if (LockoutEndUtc.HasValue && LockoutEndUtc.Value <= now)
        {
            FailedLoginCount = 0;
            LockoutEndUtc = null;
        }

        FailedLoginCount++;
        if (FailedLoginCount >= MaxFailedLoginAttempts)
        {
            LockoutEndUtc = now.Add(DefaultLockoutDuration);
        }
    }

    public void ResetLoginLockout()
    {
        FailedLoginCount = 0;
        LockoutEndUtc = null;
    }
}
