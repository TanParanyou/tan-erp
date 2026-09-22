using System.Text.Json.Serialization;

namespace TanErp.Domain.Items;

public sealed record LocalizedText
{
    [JsonPropertyName("th")]
    public string Thai { get; init; }

    [JsonPropertyName("en")]
    public string? English { get; init; }

    public LocalizedText(string thai, string? english)
    {
        Thai = thai;
        English = english;
    }

    public static LocalizedText Create(string thai, string? english, int maxLength = 250)
    {
        if (string.IsNullOrWhiteSpace(thai))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Thai name is required.");
        }

        var normalizedThai = thai.Trim();
        if (normalizedThai.Length > maxLength)
        {
            throw new ItemValidationException("ITEM_TEXT_TOO_LONG", $"Thai text exceeds maximum length of {maxLength}.");
        }

        string? normalizedEnglish = null;
        if (!string.IsNullOrWhiteSpace(english))
        {
            normalizedEnglish = english.Trim();
            if (normalizedEnglish.Length > maxLength)
            {
                throw new ItemValidationException("ITEM_TEXT_TOO_LONG", $"English text exceeds maximum length of {maxLength}.");
            }
        }

        return new LocalizedText(normalizedThai, normalizedEnglish);
    }

    public static LocalizedText? CreateOptional(string? thai, string? english, int maxLength = 2000)
    {
        var normalizedThai = string.IsNullOrWhiteSpace(thai) ? null : thai.Trim();
        var normalizedEnglish = string.IsNullOrWhiteSpace(english) ? null : english.Trim();

        if (normalizedThai is null && normalizedEnglish is null)
        {
            return null;
        }

        if (normalizedThai is not null && normalizedThai.Length > maxLength)
        {
            throw new ItemValidationException("ITEM_TEXT_TOO_LONG", $"Thai text exceeds maximum length of {maxLength}.");
        }

        if (normalizedEnglish is not null && normalizedEnglish.Length > maxLength)
        {
            throw new ItemValidationException("ITEM_TEXT_TOO_LONG", $"English text exceeds maximum length of {maxLength}.");
        }

        return new LocalizedText(normalizedThai ?? string.Empty, normalizedEnglish);
    }
}
