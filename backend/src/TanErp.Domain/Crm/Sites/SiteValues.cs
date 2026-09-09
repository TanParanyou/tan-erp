using System.Text.RegularExpressions;

namespace TanErp.Domain.Crm.Sites;

public static class SiteStatus
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

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public sealed record SiteAddressInput(
    string AddressLine1,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode);

public static class SiteNormalizer
{
    private static readonly Regex MultipleWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static string CollapseWhitespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return MultipleWhitespaceRegex.Replace(value.Trim(), " ");
    }

    public static string NormalizeLabel(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return CollapseWhitespace(value).ToLowerInvariant();
    }
}
