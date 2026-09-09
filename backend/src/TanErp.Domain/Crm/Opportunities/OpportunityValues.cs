using System.Text.RegularExpressions;

namespace TanErp.Domain.Crm.Opportunities;

public static class OpportunityStage
{
    public const string Draft = "draft";
    public const string Qualified = "qualified";
    public const string Estimation = "estimation";
    public const string Proposal = "proposal";
    public const string Won = "won";
    public const string Lost = "lost";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        Draft,
        Qualified,
        Estimation,
        Proposal,
        Won,
        Lost
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class OpportunityWorkType
{
    public const string BuiltIn = "built-in";
    public const string Interior = "interior";
    public const string Curtain = "curtain";
    public const string Wallpaper = "wallpaper";
    public const string Exterior = "exterior";
    public const string Other = "other";

    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        BuiltIn,
        Interior,
        Curtain,
        Wallpaper,
        Exterior,
        Other
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class OpportunityNormalizer
{
    private static readonly Regex MultipleWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static string CollapseWhitespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return MultipleWhitespaceRegex.Replace(value.Trim(), " ");
    }

    public static string NormalizeTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return CollapseWhitespace(value).ToLowerInvariant();
    }
}
