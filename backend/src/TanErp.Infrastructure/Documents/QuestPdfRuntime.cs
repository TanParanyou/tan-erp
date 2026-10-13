using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace TanErp.Infrastructure.Documents;

/// <summary>
/// Process-wide QuestPDF setup, run once: the license type, font ownership (embedded Sarabun only, never system fonts) and
/// fail-fast when a font family is missing. QuestPDF settings are static, so they cannot be per-request.
/// </summary>
public static class QuestPdfRuntime
{
    public const string FontFamily = "Sarabun";
    public const string FontResourcePrefix = "TanErp.Infrastructure.Documents.Fonts.";
    public static readonly IReadOnlyList<string> FontFiles = ["Sarabun-Regular.ttf", "Sarabun-Bold.ttf"];

    private static readonly object Gate = new();
    private static bool _initialized;

    /// <summary>Only the three production license types are accepted; "Evaluation" or a typo fails fast.</summary>
    public static LicenseType ParseLicense(string? licenseType) => licenseType switch
    {
        "Community" => LicenseType.Community,
        "Professional" => LicenseType.Professional,
        "Enterprise" => LicenseType.Enterprise,
        _ => throw new InvalidOperationException($"Unsupported Documents:QuestPdf:LicenseType '{licenseType}'. Use Community, Professional or Enterprise.")
    };

    public static void EnsureInitialized(string licenseType)
    {
        lock (Gate)
        {
            if (_initialized) return;

            // The ONLY place the license is chosen. Community is free only below the vendor's revenue threshold (ADR 0023).
            QuestPDF.Settings.License = ParseLicense(licenseType);
            QuestPDF.Settings.UseSystemFonts = false;
            QuestPDF.Settings.ThrowOnMissingFontFamilies = true;

            var assembly = typeof(QuestPdfRuntime).Assembly;
            foreach (var file in FontFiles)
            {
                using var stream = assembly.GetManifestResourceStream(FontResourcePrefix + file)
                    ?? throw new InvalidOperationException($"Embedded font '{file}' is missing from the Infrastructure assembly.");
                FontManager.RegisterFontFromStream(stream);
            }

            _initialized = true;
        }
    }
}
