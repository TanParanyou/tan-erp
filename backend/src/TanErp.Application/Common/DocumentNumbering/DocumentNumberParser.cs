using System.Text.RegularExpressions;
using TanErp.Domain.DocumentNumbering;

namespace TanErp.Application.Common.DocumentNumbering;

public static class DocumentNumberParser
{
    private static readonly Regex SequenceRegex = new(@"\{SEQ(?::(\d+))?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string Format(
        string formatPattern,
        string prefix,
        string? branchCode,
        DateTimeOffset timestamp,
        long sequenceValue,
        int defaultPadding = 4)
    {
        if (string.IsNullOrWhiteSpace(formatPattern))
        {
            return $"{prefix}{sequenceValue.ToString($"D{defaultPadding}")}";
        }

        var yearAd = timestamp.Year;
        var yearBe = yearAd + 543;
        var shortYearAd = (yearAd % 100).ToString("D2");
        var shortYearBe = (yearBe % 100).ToString("D2");
        var month = timestamp.Month.ToString("D2");
        var day = timestamp.Day.ToString("D2");

        var result = formatPattern
            .Replace("{PREFIX}", prefix ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("{BRANCH}", branchCode ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("{YYYY}", yearAd.ToString("D4"), StringComparison.OrdinalIgnoreCase)
            .Replace("{YY}", shortYearAd, StringComparison.OrdinalIgnoreCase)
            .Replace("{BBBB}", yearBe.ToString("D4"), StringComparison.OrdinalIgnoreCase)
            .Replace("{BB}", shortYearBe, StringComparison.OrdinalIgnoreCase)
            .Replace("{MM}", month, StringComparison.OrdinalIgnoreCase)
            .Replace("{DD}", day, StringComparison.OrdinalIgnoreCase);

        result = SequenceRegex.Replace(result, match =>
        {
            int padding = defaultPadding;
            if (match.Groups[1].Success && int.TryParse(match.Groups[1].Value, out var parsedPadding))
            {
                padding = parsedPadding;
            }
            return sequenceValue.ToString($"D{padding}");
        });

        return result;
    }

    public static string ComputePeriodKey(ResetPeriod resetPeriod, DateTimeOffset timestamp)
    {
        return resetPeriod switch
        {
            ResetPeriod.Yearly => timestamp.Year.ToString("D4"),
            ResetPeriod.Monthly => $"{timestamp.Year:D4}{timestamp.Month:D2}",
            ResetPeriod.Daily => $"{timestamp.Year:D4}{timestamp.Month:D2}{timestamp.Day:D2}",
            _ => "ALL"
        };
    }
}
