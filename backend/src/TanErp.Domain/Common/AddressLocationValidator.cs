namespace TanErp.Domain.Common;

public static class AddressLocationValidator
{
    public static bool IsValid(
        string? subdistrict,
        string? district,
        string? province,
        string? postalCode,
        string? countryCode)
    {
        return HasValueWithinLimit(subdistrict, 100)
            && HasValueWithinLimit(district, 100)
            && HasValueWithinLimit(province, 100)
            && HasFiveDigitPostalCode(postalCode)
            && HasTwoLetterCountryCode(countryCode);
    }

    private static bool HasValueWithinLimit(string? value, int maximumLength)
    {
        var normalizedValue = value?.Trim();
        return !string.IsNullOrEmpty(normalizedValue) && normalizedValue.Length <= maximumLength;
    }

    private static bool HasFiveDigitPostalCode(string? postalCode)
    {
        var normalizedPostalCode = postalCode?.Trim();
        return normalizedPostalCode is { Length: 5 } && normalizedPostalCode.All(char.IsAsciiDigit);
    }

    private static bool HasTwoLetterCountryCode(string? countryCode)
    {
        var normalizedCountryCode = countryCode?.Trim();
        return normalizedCountryCode is { Length: 2 } && normalizedCountryCode.All(char.IsAsciiLetter);
    }
}
