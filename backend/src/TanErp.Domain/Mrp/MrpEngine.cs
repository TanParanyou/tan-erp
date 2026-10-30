using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TanErp.Domain.Mrp;

/// <summary>An independent requirement: manual demand or the unissued material of an open work order.</summary>
public sealed record MrpDemandInput(Guid ItemId, decimal Quantity, DateOnly NeedBy, string SourceType, string SourceRef);

public sealed record MrpBomComponentInput(Guid ItemId, decimal Quantity, decimal ScrapPercent);

/// <summary>Approved BOM revision frozen into the snapshot.</summary>
public sealed record MrpBomInput(Guid ItemId, string BomCode, int RevisionNo, decimal OutputQuantity, IReadOnlyList<MrpBomComponentInput> Components);

/// <summary>Scheduled receipt: open purchase order balance or planned work order output.</summary>
public sealed record MrpSupplyInput(Guid ItemId, decimal Quantity, DateOnly ArrivesOn, string SourceType, string SourceRef);

public sealed record MrpStockInput(Guid ItemId, decimal Available);

public sealed record MrpItemInput(
    Guid ItemId,
    string Code,
    bool CanPurchase,
    decimal SafetyStock = 0m,
    decimal MinOrderQuantity = 0m,
    decimal LotMultiple = 0m,
    int? LeadTimeDays = null);

public sealed record MrpParameters(DateOnly AsOfDate, int PurchaseLeadTimeDays, int ProductionLeadTimeDays);

/// <summary>Everything the engine reads. A run stores this verbatim, so the plan never depends on live data.</summary>
public sealed record MrpSnapshot(
    MrpParameters Parameters,
    IReadOnlyList<MrpItemInput> Items,
    IReadOnlyList<MrpBomInput> Boms,
    IReadOnlyList<MrpDemandInput> Demands,
    IReadOnlyList<MrpSupplyInput> Supplies,
    IReadOnlyList<MrpStockInput> Stock,
    IReadOnlyList<DateOnly>? Holidays = null);

public sealed record MrpReason(string SourceType, string SourceRef, decimal Quantity, DateOnly NeedBy);

public sealed record MrpPlannedOrder(
    Guid ItemId,
    string Action,
    decimal Quantity,
    DateOnly NeedBy,
    DateOnly OrderBy,
    int Level,
    decimal GrossRequirement,
    decimal StockUsed,
    decimal ScheduledReceiptsUsed,
    IReadOnlyList<MrpReason> Reasons);

public sealed record MrpPlan(IReadOnlyList<MrpPlannedOrder> Orders, string InputHash);

/// <summary>
/// Deterministic multi-level, lot-for-lot MRP. Items are planned level by level from finished goods down to raw
/// materials. For each item, requirements are netted in date order against available stock and then scheduled
/// receipts; whatever is still short becomes a planned order that creates dependent demand on its components.
/// </summary>
public static class MrpEngine
{
    public const int MaxLevels = 20;

    public static string HashOf(MrpSnapshot snapshot)
    {
        var hasExtendedItemParams = snapshot.Items.Any(x => x.SafetyStock > 0m || x.MinOrderQuantity > 0m || x.LotMultiple > 0m || x.LeadTimeDays.HasValue);
        var holidays = snapshot.Holidays?.Distinct().OrderBy(h => h).ToList() ?? new List<DateOnly>();

        // Preserve identical SHA-256 hash when all items use default parameters and no holidays are configured.
        if (!hasExtendedItemParams && holidays.Count == 0)
        {
            var legacyCanonical = new
            {
                p = snapshot.Parameters,
                i = snapshot.Items.OrderBy(x => x.ItemId).Select(x => new { x.ItemId, x.Code, x.CanPurchase }),
                b = snapshot.Boms.OrderBy(x => x.ItemId).Select(b => new { b.ItemId, b.BomCode, b.RevisionNo, b.OutputQuantity, c = b.Components.OrderBy(c => c.ItemId) }),
                d = snapshot.Demands.OrderBy(x => x.ItemId).ThenBy(x => x.NeedBy).ThenBy(x => x.SourceRef, StringComparer.Ordinal).ThenBy(x => x.Quantity),
                s = snapshot.Supplies.OrderBy(x => x.ItemId).ThenBy(x => x.ArrivesOn).ThenBy(x => x.SourceRef, StringComparer.Ordinal).ThenBy(x => x.Quantity),
                k = snapshot.Stock.OrderBy(x => x.ItemId),
            };
            var legacyJson = JsonSerializer.Serialize(legacyCanonical);
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(legacyJson))).ToLowerInvariant();
        }

        // Sorted canonical form including item planning parameters and working calendar holidays.
        var canonical = new
        {
            p = snapshot.Parameters,
            i = snapshot.Items.OrderBy(x => x.ItemId),
            b = snapshot.Boms.OrderBy(x => x.ItemId).Select(b => new { b.ItemId, b.BomCode, b.RevisionNo, b.OutputQuantity, c = b.Components.OrderBy(c => c.ItemId) }),
            d = snapshot.Demands.OrderBy(x => x.ItemId).ThenBy(x => x.NeedBy).ThenBy(x => x.SourceRef, StringComparer.Ordinal).ThenBy(x => x.Quantity),
            s = snapshot.Supplies.OrderBy(x => x.ItemId).ThenBy(x => x.ArrivesOn).ThenBy(x => x.SourceRef, StringComparer.Ordinal).ThenBy(x => x.Quantity),
            k = snapshot.Stock.OrderBy(x => x.ItemId),
            h = holidays,
        };
        var json = JsonSerializer.Serialize(canonical);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    }

    public static MrpPlan Plan(MrpSnapshot snapshot)
    {
        var boms = snapshot.Boms.ToDictionary(b => b.ItemId);
        var items = snapshot.Items.ToDictionary(i => i.ItemId);
        var levels = ComputeLevels(boms);
        var holidays = snapshot.Holidays is { Count: > 0 }
            ? new HashSet<DateOnly>(snapshot.Holidays)
            : new HashSet<DateOnly>();

        // Requirements per item, filled as higher levels plan their orders.
        var requirements = new Dictionary<Guid, List<MrpDemandInput>>();
        foreach (var demand in snapshot.Demands.Where(d => d.Quantity > 0))
        {
            if (!requirements.TryGetValue(demand.ItemId, out var list)) requirements[demand.ItemId] = list = new List<MrpDemandInput>();
            list.Add(demand);
        }

        var itemIds = new HashSet<Guid>(requirements.Keys);
        foreach (var bom in snapshot.Boms) foreach (var c in bom.Components) itemIds.Add(c.ItemId);
        foreach (var item in snapshot.Items.Where(i => i.SafetyStock > 0m)) itemIds.Add(item.ItemId);

        var orders = new List<MrpPlannedOrder>();
        var stock = snapshot.Stock.ToDictionary(s => s.ItemId, s => Math.Max(0m, s.Available));
        var supplies = snapshot.Supplies.Where(s => s.Quantity > 0)
            .GroupBy(s => s.ItemId).ToDictionary(g => g.Key, g => g.OrderBy(s => s.ArrivesOn).ThenBy(s => s.SourceRef, StringComparer.Ordinal).ThenBy(s => s.Quantity).ToList());

        // Process by level (0 = never a component), then by item id so the output order is stable.
        foreach (var itemId in itemIds.OrderBy(id => levels.GetValueOrDefault(id, 0)).ThenBy(id => id))
        {
            items.TryGetValue(itemId, out var info);
            var rawAvailable = stock.GetValueOrDefault(itemId, 0m);
            var safetyStock = Math.Max(0m, info?.SafetyStock ?? 0m);
            var onHand = Math.Max(0m, rawAvailable - safetyStock);
            var safetyDeficit = Math.Max(0m, safetyStock - rawAvailable);

            requirements.TryGetValue(itemId, out var needs);
            if (safetyDeficit > 0m)
            {
                needs ??= requirements[itemId] = new List<MrpDemandInput>();
                var firstNeedDate = needs.Count > 0 ? needs.Min(n => n.NeedBy) : snapshot.Parameters.AsOfDate;
                needs.Add(new MrpDemandInput(itemId, Round(safetyDeficit), firstNeedDate, MrpDemandSource.SafetyStock, "SAFETY-STOCK"));
            }

            if (needs is null || needs.Count == 0) continue;

            var level = levels.GetValueOrDefault(itemId, 0);
            var receipts = supplies.GetValueOrDefault(itemId)?.Select(s => (s.ArrivesOn, Remaining: s.Quantity)).ToList() ?? new List<(DateOnly ArrivesOn, decimal Remaining)>();
            var receiptRemaining = receipts.Select(r => r.Remaining).ToArray();

            var action = boms.ContainsKey(itemId) ? MrpAction.Make : info is { CanPurchase: true } ? MrpAction.Buy : MrpAction.Shortage;
            var defaultLeadDays = action == MrpAction.Make ? snapshot.Parameters.ProductionLeadTimeDays : snapshot.Parameters.PurchaseLeadTimeDays;
            var leadDays = Math.Max(0, info?.LeadTimeDays ?? defaultLeadDays);
            var moq = Math.Max(0m, info?.MinOrderQuantity ?? 0m);
            var lotMultiple = Math.Max(0m, info?.LotMultiple ?? 0m);

            // Net each requirement date, earliest first.
            var byDate = needs.GroupBy(n => n.NeedBy).OrderBy(g => g.Key).ToList();
            foreach (var group in byDate)
            {
                var gross = group.Sum(n => n.Quantity);
                var remaining = gross;

                var fromStock = Math.Min(onHand, remaining);
                onHand -= fromStock;
                remaining -= fromStock;

                decimal fromReceipts = 0m;
                for (var i = 0; i < receipts.Count && remaining > 0; i++)
                {
                    if (receipts[i].ArrivesOn > group.Key || receiptRemaining[i] <= 0) continue;
                    var take = Math.Min(receiptRemaining[i], remaining);
                    receiptRemaining[i] -= take;
                    remaining -= take;
                    fromReceipts += take;
                }

                if (remaining <= 0) continue;

                var orderQty = remaining;
                if (moq > 0m && orderQty < moq)
                {
                    orderQty = moq;
                }
                if (lotMultiple > 0m)
                {
                    orderQty = decimal.Ceiling(orderQty / lotMultiple) * lotMultiple;
                }
                orderQty = Round(orderQty);
                var excessOrdered = Math.Max(0m, orderQty - remaining);
                onHand += excessOrdered;

                var orderBy = SubtractWorkingDays(group.Key, leadDays, holidays);
                var reasons = group.Select(n => new MrpReason(n.SourceType, n.SourceRef, n.Quantity, n.NeedBy)).ToList();
                orders.Add(new MrpPlannedOrder(itemId, action, orderQty, group.Key, orderBy, level, Round(gross), Round(fromStock), Round(fromReceipts), reasons));

                if (action != MrpAction.Make) continue;

                // The make order creates dependent demand on components at its release date.
                var bom = boms[itemId];
                foreach (var component in bom.Components)
                {
                    var qty = Round(component.Quantity * (1 + component.ScrapPercent / 100m) * orderQty / bom.OutputQuantity);
                    if (qty <= 0) continue;
                    if (!requirements.TryGetValue(component.ItemId, out var list)) requirements[component.ItemId] = list = new List<MrpDemandInput>();
                    list.Add(new MrpDemandInput(component.ItemId, qty, orderBy, MrpDemandSource.Dependent, bom.BomCode));
                }
            }
        }

        return new MrpPlan(orders.OrderBy(o => o.Level).ThenBy(o => o.ItemId).ThenBy(o => o.NeedBy).ToList(), HashOf(snapshot));
    }

    private static DateOnly SubtractWorkingDays(DateOnly needBy, int leadDays, IReadOnlySet<DateOnly> holidays)
    {
        if (holidays.Count == 0)
        {
            return needBy.AddDays(-leadDays);
        }

        var date = needBy;
        if (leadDays <= 0)
        {
            while (holidays.Contains(date))
            {
                date = date.AddDays(-1);
            }
            return date;
        }

        var remaining = leadDays;
        while (remaining > 0)
        {
            date = date.AddDays(-1);
            if (!holidays.Contains(date))
            {
                remaining--;
            }
        }

        return date;
    }

    private static decimal Round(decimal value) => decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    /// <summary>Low-level code: the deepest position an item takes as a component of any BOM (0 for top level).</summary>
    private static Dictionary<Guid, int> ComputeLevels(IReadOnlyDictionary<Guid, MrpBomInput> boms)
    {
        var levels = new Dictionary<Guid, int>();
        var changed = true;
        var iterations = 0;
        foreach (var bom in boms.Values) levels.TryAdd(bom.ItemId, 0);
        while (changed)
        {
            if (++iterations > MaxLevels + 1) throw new MrpDomainException("MRP_BOM_CYCLE", "The BOM structure contains a cycle or is deeper than the supported number of levels.");
            changed = false;
            foreach (var bom in boms.Values)
            {
                var parentLevel = levels.GetValueOrDefault(bom.ItemId, 0);
                foreach (var component in bom.Components)
                {
                    var wanted = parentLevel + 1;
                    if (levels.GetValueOrDefault(component.ItemId, -1) < wanted)
                    {
                        levels[component.ItemId] = wanted;
                        changed = true;
                    }
                }
            }
        }

        return levels;
    }
}
