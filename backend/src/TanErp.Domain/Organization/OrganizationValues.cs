using System.Text.RegularExpressions;

namespace TanErp.Domain.Organization;

public sealed class OrganizationDomainException : Exception
{
    public OrganizationDomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

public static class OrganizationLimits
{
    public const int Name = 255;
    public const int Address = 500;
    public const int Phone = 30;
    public const int BranchCode = 50;
}

public static class ThaiTaxIdentifier
{
    /// <summary>13 digits where the last digit is the mod-11 check digit of the first 12 (Revenue Department rule).</summary>
    public static bool IsValid(string? value)
    {
        if (value is null || value.Length != 13 || !value.All(char.IsAsciiDigit)) return false;
        var sum = 0;
        for (var i = 0; i < 12; i++) sum += (value[i] - '0') * (13 - i);
        return (11 - sum % 11) % 10 == value[12] - '0';
    }
}

public static class TaxBranchCodeRule
{
    public static bool IsValid(string? value) => value is { Length: 5 } && value.All(char.IsAsciiDigit);
}

public static partial class BranchCode
{
    [GeneratedRegex("^[A-Za-z0-9_-]{1,50}$")]
    private static partial Regex Pattern();

    public static bool IsValid(string? value) => value is not null && Pattern().IsMatch(value);
}

internal static class ProfileText
{
    public static string? Optional(string? value, int max, string field)
    {
        var trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (trimmed is not null && trimmed.Length > max)
            throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", $"{field} cannot exceed {max} characters.");
        return trimmed;
    }
}
