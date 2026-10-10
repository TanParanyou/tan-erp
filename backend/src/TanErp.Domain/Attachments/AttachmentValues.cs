namespace TanErp.Domain.Attachments;

public class AttachmentDomainException : Exception
{
    public string Code { get; }

    public AttachmentDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Code-defined whitelist of record types that can own attachments and signatures.
/// A new owner type also needs an AttachmentOwnerRegistry descriptor and an AttachmentOwnerScopeReader case.
/// </summary>
public static class AttachmentOwnerTypes
{
    public const string InstallationJob = "installation-job";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal) { InstallationJob };

    public static IReadOnlyCollection<string> All => Registered;

    public static bool IsRegistered(string? ownerType) => ownerType is not null && Registered.Contains(ownerType);

    /// <summary>Returns the canonical (trimmed, lower-case) owner type, or null when it is not registered.</summary>
    public static string? Normalize(string? ownerType)
    {
        var candidate = ownerType?.Trim().ToLowerInvariant();
        return IsRegistered(candidate) ? candidate : null;
    }
}

public static class AttachmentPurposes
{
    public const string General = "general";
    public const string Evidence = "evidence";
    public const string Handover = "handover";
    public const string Defect = "defect";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { General, Evidence, Handover, Defect };

    /// <summary>Returns the canonical purpose, or null when it is unknown.</summary>
    public static string? Normalize(string? purpose)
    {
        var candidate = purpose?.Trim().ToLowerInvariant();
        return candidate is not null && All.Contains(candidate) ? candidate : null;
    }
}
