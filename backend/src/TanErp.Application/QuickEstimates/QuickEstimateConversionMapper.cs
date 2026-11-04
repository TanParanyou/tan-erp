using System.Text.Json;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Application.QuickEstimates;

/// <summary>
/// Builds the scope lines of an official estimate draft from one frozen quick-estimate calculation: one work item per measurement line
/// (billable quantity and the template unit), plus cost components derived from the template's cost recipe when present.
/// </summary>
public static class QuickEstimateConversionMapper
{
    public const string SectionCode = "QE";
    public const string ReasonCode = "QUICK_ESTIMATE_CONVERSION";

    /// <summary>The EstimateWorkItem default margin; the estimator can change it before calculating.</summary>
    public const decimal DefaultMargin = 0.30m;

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static string Cut(string value, int max) => value.Length <= max ? value : value[..max];

    public static IReadOnlyList<EstimateSectionDraftDto> BuildSections(string quickEstimateNumber, string roomOrArea, int calculationVersion, string templateCode, string snapshotJson)
    {
        using var document = JsonDocument.Parse(snapshotJson);
        var templateElement = document.RootElement.GetProperty("template");
        var unitCode = templateElement.GetProperty("unitCode").GetString() ?? string.Empty;
        var recipeLines = ReadCostRecipe(document.RootElement, templateElement);
        var reason = Cut($"{quickEstimateNumber} · v{calculationVersion}", 500);
        var items = new List<EstimateWorkItemDraftDto>();
        foreach (var line in document.RootElement.GetProperty("lines").EnumerateArray())
        {
            var order = items.Count + 1;
            var subtype = line.TryGetProperty("workSubtype", out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
            var label = string.IsNullOrWhiteSpace(subtype) ? roomOrArea : subtype.Trim();
            var billableQuantity = line.GetProperty("billableQuantity").GetDecimal();

            var costComponents = recipeLines.Count == 0
                ? Array.Empty<EstimateCostComponentDraftDto>()
                : recipeLines.Select((recipe, index) => new EstimateCostComponentDraftDto(
                    Id: null,
                    Type: string.IsNullOrWhiteSpace(recipe.ComponentType) ? CostComponentType.Material : recipe.ComponentType.Trim(),
                    Description: Cut(recipe.DescriptionTh.Trim(), 500),
                    Quantity: Math.Round(billableQuantity * recipe.QuantityPerUnit, 4, MidpointRounding.AwayFromZero),
                    UnitCode: recipe.UnitCode.Trim(),
                    UnitCost: recipe.UnitCost,
                    Currency: EstimateDefaults.DefaultCurrency,
                    SortOrder: index + 1,
                    ItemId: recipe.ItemId,
                    ProvisionalReasonCode: recipe.ItemId.HasValue ? null : ReasonCode,
                    ProvisionalNote: recipe.ItemId.HasValue ? null : reason)).ToArray();

            items.Add(new EstimateWorkItemDraftDto(
                Id: null, Code: $"QE-{order:00}", DescriptionTh: Cut($"{templateCode} · {label}", 500), DescriptionEn: null,
                Quantity: billableQuantity, UnitCode: unitCode, SellingRuleType: SellingRuleType.Margin, SellingRuleValue: DefaultMargin,
                SortOrder: order, CostComponents: costComponents, OverrideReasonCode: ReasonCode, OverrideReason: reason));
        }

        return items.Count == 0 ? Array.Empty<EstimateSectionDraftDto>() : new[] { new EstimateSectionDraftDto(null, SectionCode, Cut(roomOrArea, 200), null, 1, items) };
    }

    private static IReadOnlyList<TemplateCostRecipeLine> ReadCostRecipe(JsonElement root, JsonElement templateElement)
    {
        if (templateElement.TryGetProperty("config", out var configEl)
            && configEl.ValueKind == JsonValueKind.Object
            && configEl.TryGetProperty("costRecipe", out var recipeEl)
            && recipeEl.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<TemplateCostRecipeLine>>(recipeEl.GetRawText(), Json) ?? [];
        }

        if (templateElement.TryGetProperty("costRecipe", out var directEl)
            && directEl.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<TemplateCostRecipeLine>>(directEl.GetRawText(), Json) ?? [];
        }

        if (root.TryGetProperty("costRecipe", out var rootEl)
            && rootEl.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<TemplateCostRecipeLine>>(rootEl.GetRawText(), Json) ?? [];
        }

        return [];
    }
}
