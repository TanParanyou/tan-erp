using System.Text.Json;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;

namespace TanErp.Application.QuickEstimates;

/// <summary>
/// Builds the scope lines of an official estimate draft from one frozen quick-estimate calculation: one work item per measurement line
/// (billable quantity and the template unit). No price or cost is copied; the estimator adds costs and calculates the estimate anew.
/// </summary>
public static class QuickEstimateConversionMapper
{
    public const string SectionCode = "QE";
    public const string ReasonCode = "QUICK_ESTIMATE_CONVERSION";

    /// <summary>The EstimateWorkItem default margin; the estimator can change it before calculating.</summary>
    public const decimal DefaultMargin = 0.30m;

    private static string Cut(string value, int max) => value.Length <= max ? value : value[..max];

    public static IReadOnlyList<EstimateSectionDraftDto> BuildSections(string quickEstimateNumber, string roomOrArea, int calculationVersion, string templateCode, string snapshotJson)
    {
        using var document = JsonDocument.Parse(snapshotJson);
        var unitCode = document.RootElement.GetProperty("template").GetProperty("unitCode").GetString() ?? string.Empty;
        var reason = Cut($"{quickEstimateNumber} · v{calculationVersion}", 500);
        var items = new List<EstimateWorkItemDraftDto>();
        foreach (var line in document.RootElement.GetProperty("lines").EnumerateArray())
        {
            var order = items.Count + 1;
            var subtype = line.TryGetProperty("workSubtype", out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
            var label = string.IsNullOrWhiteSpace(subtype) ? roomOrArea : subtype.Trim();
            items.Add(new EstimateWorkItemDraftDto(
                Id: null, Code: $"QE-{order:00}", DescriptionTh: Cut($"{templateCode} · {label}", 500), DescriptionEn: null,
                Quantity: line.GetProperty("billableQuantity").GetDecimal(), UnitCode: unitCode, SellingRuleType: SellingRuleType.Margin, SellingRuleValue: DefaultMargin,
                SortOrder: order, CostComponents: Array.Empty<EstimateCostComponentDraftDto>(), OverrideReasonCode: ReasonCode, OverrideReason: reason));
        }

        return items.Count == 0 ? Array.Empty<EstimateSectionDraftDto>() : new[] { new EstimateSectionDraftDto(null, SectionCode, Cut(roomOrArea, 200), null, 1, items) };
    }
}
