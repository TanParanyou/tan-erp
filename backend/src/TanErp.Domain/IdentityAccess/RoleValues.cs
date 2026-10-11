namespace TanErp.Domain.IdentityAccess;

public sealed class RoleDomainException : Exception
{
    public RoleDomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

public static class RoleRules
{
    public const int NameMin = 3;
    public const int NameMax = 100;
    public const int DescriptionMax = 500;
    public const string ReservedPrefix = "system.";

    /// <summary>
    /// Trims and validates a custom role name and description. The "system." prefix is reserved for built-in roles;
    /// this is a naming rule only — authorization never inspects role names.
    /// </summary>
    public static (string Name, string? Description) Normalize(string? name, string? description)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMin or > NameMax)
            throw new RoleDomainException("REQUEST_VALIDATION_FAILED", $"Role name must be {NameMin}-{NameMax} characters.");
        if (trimmedName.StartsWith(ReservedPrefix, StringComparison.OrdinalIgnoreCase))
            throw new RoleDomainException("ROLE_NAME_RESERVED", "The 'system.' prefix is reserved for built-in roles.");

        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription is { Length: > DescriptionMax })
            throw new RoleDomainException("REQUEST_VALIDATION_FAILED", $"Description cannot exceed {DescriptionMax} characters.");

        return (trimmedName, trimmedDescription);
    }
}
