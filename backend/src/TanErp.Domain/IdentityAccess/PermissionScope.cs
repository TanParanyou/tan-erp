namespace TanErp.Domain.IdentityAccess;

public sealed record PermissionScope
{
    public static readonly string Organization = "organization";
    public static readonly string Branch = "branch";
    public static readonly string Project = "project";
    public static readonly string Own = "own";

    private static readonly HashSet<string> ValidScopes = new(StringComparer.Ordinal)
    {
        Organization,
        Branch,
        Project,
        Own
    };

    public string Value { get; }

    private PermissionScope(string value)
    {
        Value = value;
    }

    public static PermissionScope Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !ValidScopes.Contains(value.Trim()))
        {
            throw new ArgumentException($"Invalid permission scope: '{value}'. Allowed scopes are: {string.Join(", ", ValidScopes)}.", nameof(value));
        }

        return new PermissionScope(value.Trim());
    }

    public override string ToString() => Value;
}
