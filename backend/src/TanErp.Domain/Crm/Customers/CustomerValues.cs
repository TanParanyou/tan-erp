using System.Text.RegularExpressions;

namespace TanErp.Domain.Crm.Customers;

public static class CustomerType
{
    public const string Person = "person";
    public const string Organization = "organization";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Person,
        Organization
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class CustomerStatus
{
    public const string Draft = "draft";
    public const string Active = "active";
    public const string Inactive = "inactive";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Active,
        Inactive
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class PreferredLocale
{
    public const string Thai = "th";
    public const string English = "en";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Thai,
        English
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class ContactChannel
{
    public const string Phone = "phone";
    public const string Email = "email";
    public const string Line = "line";
    public const string Other = "other";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Phone,
        Email,
        Line,
        Other
    };

    public static bool IsValid(string value) => All.Contains(value.Trim());
}

public static class CustomerNormalizer
{
    private static readonly Regex MultipleWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DigitsOnlyRegex = new(@"\D", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string NormalizeName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return MultipleWhitespaceRegex.Replace(value.Trim(), " ").ToLowerInvariant();
    }

    public static string CollapseWhitespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return MultipleWhitespaceRegex.Replace(value.Trim(), " ");
    }

    public static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        var digits = DigitsOnlyRegex.Replace(phone, "");
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }

    public static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var normalized = email.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    public static bool IsValidEmail(string email) => EmailRegex.IsMatch(email);
}

public sealed record PrimaryContactInput(
    string Name,
    string? RoleTitle,
    string? Phone,
    string? Email,
    string PreferredChannel);
