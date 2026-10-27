using System.Text.RegularExpressions;

namespace TanErp.Domain.Organization;

/// <summary>WCAG 2.x relative luminance and contrast ratio. Mirrored in frontend/src/lib/color/wcag.ts and mobile/lib/core/theme/brand_color.dart.</summary>
public static class WcagContrast
{
    public static double RelativeLuminance(byte r, byte g, byte b)
    {
        static double Linear(byte c)
        {
            var s = c / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }
        return 0.2126 * Linear(r) + 0.7152 * Linear(g) + 0.0722 * Linear(b);
    }

    public static double Ratio(double luminanceA, double luminanceB)
    {
        var (hi, lo) = luminanceA >= luminanceB ? (luminanceA, luminanceB) : (luminanceB, luminanceA);
        return (hi + 0.05) / (lo + 0.05);
    }
}

/// <summary>Organization brand colour rule (design.md 7.4): white text on it >= 4.5:1 and it on the light canvas >= 3:1.</summary>
public static partial class BrandColor
{
    public const double MinTextContrast = 4.5;
    public const double MinCanvasContrast = 3.0;

    // Light --erp-bg-canvas value from design-tokens (tokens.css --erp-canvas).
    private static readonly double LightCanvasLuminance = WcagContrast.RelativeLuminance(0xF4, 0xF6, 0xF9);
    private static readonly double WhiteLuminance = WcagContrast.RelativeLuminance(0xFF, 0xFF, 0xFF);

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexPattern();

    /// <summary>Returns the normalized upper-case #RRGGBB, or null when the value is not a valid hex colour that passes WCAG AA.</summary>
    public static string? NormalizeIfAccessible(string value)
    {
        if (!HexPattern().IsMatch(value)) return null;

        var r = Convert.ToByte(value.Substring(1, 2), 16);
        var g = Convert.ToByte(value.Substring(3, 2), 16);
        var b = Convert.ToByte(value.Substring(5, 2), 16);
        var luminance = WcagContrast.RelativeLuminance(r, g, b);

        return WcagContrast.Ratio(WhiteLuminance, luminance) >= MinTextContrast
            && WcagContrast.Ratio(luminance, LightCanvasLuminance) >= MinCanvasContrast
            ? value.ToUpperInvariant()
            : null;
    }
}
