using TanErp.Domain.Estimates;
using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public class EstimateCostComponentSnapshotTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _workItemId = Guid.NewGuid();

    [Fact]
    public void CreateAdHoc_WithoutCatalogItem_SnapshotFieldsAreNull()
    {
        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            _workItemId,
            CostComponentType.Material,
            "Custom Non-Catalog Bracket",
            quantity: 5m,
            unitCode: "PCS",
            unitCost: 150m);

        Assert.Null(comp.ItemId);
        Assert.Null(comp.CostRecordId);
        Assert.Null(comp.CostRecordVersion);
        Assert.Null(comp.ItemCodeSnapshot);
        Assert.Null(comp.ItemNameSnapshot);
        Assert.Null(comp.UnitSnapshot);
        Assert.Null(comp.UnitCostSnapshot);
        Assert.Null(comp.CurrencySnapshot);
        Assert.Null(comp.CostScopeSnapshot);
        Assert.Null(comp.CostEffectiveFromUtc);
        Assert.Null(comp.CostPolicyVersion);
        Assert.Null(comp.ResolvedAtUtc);
        Assert.Equal(150m, comp.UnitCost);
        Assert.Equal(750m, comp.TotalCost);
    }

    [Fact]
    public void SetCatalogSnapshot_WithValidSnapshot_PopulatesAllSnapshotProperties()
    {
        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            _workItemId,
            CostComponentType.Material,
            "Solar Panel 550W",
            quantity: 10m,
            unitCode: "PCS",
            unitCost: 3000m);

        var itemId = Guid.NewGuid();
        var costRecordId = Guid.NewGuid();
        var effectiveFrom = DateTimeOffset.UtcNow.AddDays(-5);
        var resolvedAt = DateTimeOffset.UtcNow;
        var itemName = LocalizedText.Create("แผงโซลาร์ 550W", "Solar Panel 550W");

        comp.SetCatalogCostSnapshot(
            itemId: itemId,
            costRecordId: costRecordId,
            costRecordVersion: 2,
            itemCodeSnapshot: "SOLAR-550W",
            itemNameSnapshot: itemName,
            unitSnapshot: "PCS",
            unitCostSnapshot: 3200m,
            currencySnapshot: "THB",
            costScopeSnapshot: "branch",
            costEffectiveFromUtc: effectiveFrom,
            costPolicyVersion: "v1",
            resolvedAtUtc: resolvedAt);

        Assert.Equal(itemId, comp.ItemId);
        Assert.Equal(costRecordId, comp.CostRecordId);
        Assert.Equal(2, comp.CostRecordVersion);
        Assert.Equal("SOLAR-550W", comp.ItemCodeSnapshot);
        Assert.NotNull(comp.ItemNameSnapshot);
        Assert.Equal("แผงโซลาร์ 550W", comp.ItemNameSnapshot.Thai);
        Assert.Equal("Solar Panel 550W", comp.ItemNameSnapshot.English);
        Assert.Equal("PCS", comp.UnitSnapshot);
        Assert.Equal(3200m, comp.UnitCostSnapshot);
        Assert.Equal("THB", comp.CurrencySnapshot);
        Assert.Equal("branch", comp.CostScopeSnapshot);
        Assert.Equal(effectiveFrom, comp.CostEffectiveFromUtc);
        Assert.Equal("v1", comp.CostPolicyVersion);
        Assert.Equal(resolvedAt, comp.ResolvedAtUtc);

        // UnitCost and TotalCost should be updated authoritatively from the snapshot
        Assert.Equal(3200m, comp.UnitCost);
        Assert.Equal(32000m, comp.TotalCost);
    }

    [Fact]
    public void SetCatalogSnapshot_WithEmptyItemId_ThrowsArgumentException()
    {
        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            _workItemId,
            CostComponentType.Material,
            "Solar Panel 550W",
            quantity: 1m,
            unitCode: "PCS",
            unitCost: 100m);

        Assert.Throws<ArgumentException>(() =>
            comp.SetCatalogCostSnapshot(
                itemId: Guid.Empty,
                costRecordId: Guid.NewGuid(),
                costRecordVersion: 1,
                itemCodeSnapshot: "ITEM-01",
                itemNameSnapshot: LocalizedText.Create("ทดสอบ", "Test"),
                unitSnapshot: "PCS",
                unitCostSnapshot: 100m,
                currencySnapshot: "THB",
                costScopeSnapshot: "organization",
                costEffectiveFromUtc: DateTimeOffset.UtcNow,
                costPolicyVersion: "v1",
                resolvedAtUtc: DateTimeOffset.UtcNow));
    }
}
