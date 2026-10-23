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

    [Fact]
    public void SetConversionSnapshot_FreezesFactorAndUpdatesUnitAndTotals_AndPolicyContentHashIncludesSnapshotMode()
    {
        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            _workItemId,
            CostComponentType.Material,
            "Galvanized Steel Pipe",
            quantity: 4m,
            unitCode: "BOX",
            unitCost: 100m);

        var snapshot = EstimateConversionSnapshot.Create(Guid.NewGuid(), UnitConversionScopes.Item, "BOX", "PCS", 12.5m);
        Assert.Throws<ArgumentException>(() => comp.SetConversionSnapshot(snapshot));

        comp.SetCatalogCostSnapshot(
            itemId: Guid.NewGuid(),
            costRecordId: Guid.NewGuid(),
            costRecordVersion: 1,
            itemCodeSnapshot: "PIPE-01",
            itemNameSnapshot: LocalizedText.Create("ท่อเหล็ก", "Steel Pipe"),
            unitSnapshot: "PCS",
            unitCostSnapshot: 100m,
            currencySnapshot: "THB",
            costScopeSnapshot: "organization",
            costEffectiveFromUtc: DateTimeOffset.UtcNow.AddDays(-1),
            costPolicyVersion: "v1",
            resolvedAtUtc: DateTimeOffset.UtcNow);

        comp.SetConversionSnapshot(snapshot);
        Assert.NotNull(comp.ConversionSnapshot);
        Assert.Equal(snapshot.ConversionId, comp.ConversionId);
        Assert.Equal(UnitConversionScopes.Item, comp.ConversionScope);
        Assert.Equal("BOX", comp.ConversionFromUnit);
        Assert.Equal("PCS", comp.ConversionToUnit);
        Assert.Equal(12.5m, comp.ConversionFactor);
        Assert.Equal("BOX", comp.UnitCode);
        // UnitCost = 100 * 12.5 = 1250; TotalCost = 4 * 12.5 * 100 = 5000
        Assert.Equal(1250m, comp.UnitCost);
        Assert.Equal(5000m, comp.TotalCost);
        Assert.Throws<EstimateInvalidStateException>(() =>
            comp.Update(CostComponentType.Material, "Edited", 1m, "BOX", 1250m, 1));

        // CalculationPolicyVersion hash differs when UnitConversionMode is Snapshot vs None
        var effectiveFrom = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);
        var publishedAt = new DateTimeOffset(2026, 10, 2, 0, 0, 0, TimeSpan.Zero);
        var creator = Guid.NewGuid();
        var publisher = Guid.NewGuid();
        var nonePolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), _orgId, null, "CALC-STD", 1, "none", 0m, "away-from-zero", effectiveFrom, null, creator, UnitConversionModes.None);
        var snapshotPolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), _orgId, null, "CALC-STD", 1, "none", 0m, "away-from-zero", effectiveFrom, null, creator, UnitConversionModes.Snapshot);
        nonePolicy.Publish(publisher, publishedAt);
        snapshotPolicy.Publish(publisher, publishedAt);
        Assert.NotEqual(nonePolicy.ContentHash, snapshotPolicy.ContentHash);
    }
}
