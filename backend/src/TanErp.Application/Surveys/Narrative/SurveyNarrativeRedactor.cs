using System.Text.RegularExpressions;

namespace TanErp.Application.Surveys.Narrative;

/// <summary>
/// Removes contact details and long identifiers before text leaves the system.
/// Best-effort: it cannot detect names, so free-text from users is still treated as sensitive.
/// </summary>
public static class SurveyNarrativeRedactor
{
    private static readonly Regex Email = new(@"[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}", RegexOptions.Compiled);
    private static readonly Regex Url = new(@"https?://\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex Phone = new(@"(?<!\d)0\d{1,2}[-\s]?\d{3}[-\s]?\d{3,4}(?!\d)", RegexOptions.Compiled);
    private static readonly Regex LongNumber = new(@"(?<!\d)\d{9,}(?!\d)", RegexOptions.Compiled);

    public static string Redact(string text)
    {
        var redacted = Email.Replace(text, "[email]");
        redacted = Url.Replace(redacted, "[link]");
        redacted = Phone.Replace(redacted, "[phone]");
        return LongNumber.Replace(redacted, "[number]");
    }
}
