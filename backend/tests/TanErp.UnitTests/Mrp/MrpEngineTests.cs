using TanErp.Domain.Mrp;
using Xunit;

namespace TanErp.UnitTests.Mrp;

public class MrpEngineTests
{
    private static readonly Guid Cabinet = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid Panel = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid Plywood = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private static readonly Guid Screw = Guid.Parse("00000000-0000-0000-0000-000000000004");
    private static readonly Guid Mystery = Guid.Parse("00000000-0000-0000-0000-000000000005");
    private static readonly DateOnly AsOf = new(2026, 10, 1);
    private static readonly DateOnly Due = new(2026, 10, 20);

    private static MrpParameters Parameters(int purchase = 7, int production = 3) => new(AsOf, purchase, production);

    private static readonly MrpItemInput[] Items =
    {
        new(Cabinet, "CAB", false), new(Panel, "PANEL", false), new(Plywood, "PLY", true), new(Screw, "SCR", true), new(Mystery, "MYS", false)
    };

    /// <summary>Cabinet = 2 panels + 8 screws; Panel = 1 plywood (+10% scrap) per 2 panels.</summary>
    private static readonly MrpBomInput[] Boms =
    {
        new(Cabinet, "BOM-1", 1, 1m, new[] { new MrpBomComponentInput(Panel, 2m, 0m), new MrpBomComponentInput(Screw, 8m, 0m) }),
        new(Panel, "BOM-2", 1, 2m, new[] { new MrpBomComponentInput(Plywood, 1m, 10m) }),
    };

    private static MrpSnapshot Snapshot(IEnumerable<MrpDemandInput>? demands = null, IEnumerable<MrpStockInput>? stock = null, IEnumerable<MrpSupplyInput>? supplies = null, MrpParameters? parameters = null) =>
        new(parameters ?? Parameters(), Items, Boms, (demands ?? new[] { new MrpDemandInput(Cabinet, 10m, Due, MrpDemandSource.Manual, "SO-1") }).ToList(), (supplies ?? Array.Empty<MrpSupplyInput>()).ToList(), (stock ?? Array.Empty<MrpStockInput>()).ToList());

    [Fact]
    public void ExplodesMultipleLevels_WithScrapAndLeadTimes()
    {
        var plan = MrpEngine.Plan(Snapshot());

        var make = plan.Orders.Single(o => o.ItemId == Cabinet);
        Assert.Equal((MrpAction.Make, 10m, Due), (make.Action, make.Quantity, make.NeedBy));
        Assert.Equal(Due.AddDays(-3), make.OrderBy);

        // 10 cabinets need 20 panels, released 3 days before the cabinets are due.
        var panel = plan.Orders.Single(o => o.ItemId == Panel);
        Assert.Equal(20m, panel.Quantity);
        Assert.Equal(Due.AddDays(-3), panel.NeedBy);
        Assert.Equal(1, panel.Level);

        // 20 panels = 10 recipes of 1 plywood + 10% scrap = 11 plywood, ordered 7 days before it is needed.
        var plywood = plan.Orders.Single(o => o.ItemId == Plywood);
        Assert.Equal((MrpAction.Buy, 11m), (plywood.Action, plywood.Quantity));
        Assert.Equal(Due.AddDays(-6), plywood.NeedBy);
        Assert.Equal(Due.AddDays(-13), plywood.OrderBy);
        Assert.Equal(2, plywood.Level);

        var screws = plan.Orders.Single(o => o.ItemId == Screw);
        Assert.Equal(80m, screws.Quantity);
        Assert.Equal("BOM-1", Assert.Single(screws.Reasons).SourceRef);
    }

    [Fact]
    public void NetsStockThenScheduledReceipts_AndOnlyOrdersTheShortage()
    {
        var plan = MrpEngine.Plan(Snapshot(
            stock: new[] { new MrpStockInput(Panel, 8m), new MrpStockInput(Plywood, 3m), new MrpStockInput(Screw, 500m) },
            supplies: new[] { new MrpSupplyInput(Plywood, 4m, Due.AddDays(-20), "purchase_order", "PO-1") }));

        // 20 panels needed − 8 in stock → make 12 → 6 recipes → 6.6 plywood − 3 stock − 4 on order = 0 → nothing to buy.
        Assert.Equal(12m, plan.Orders.Single(o => o.ItemId == Panel).Quantity);
        Assert.DoesNotContain(plan.Orders, o => o.ItemId == Plywood);
        Assert.DoesNotContain(plan.Orders, o => o.ItemId == Screw);
        var panel = plan.Orders.Single(o => o.ItemId == Panel);
        Assert.Equal((20m, 8m), (panel.GrossRequirement, panel.StockUsed));
    }

    [Fact]
    public void IgnoresReceiptsThatArriveAfterTheNeedDate()
    {
        var late = MrpEngine.Plan(Snapshot(supplies: new[] { new MrpSupplyInput(Screw, 100m, Due.AddDays(30), "purchase_order", "PO-LATE") }));
        Assert.Equal(80m, late.Orders.Single(o => o.ItemId == Screw).Quantity);

        var early = MrpEngine.Plan(Snapshot(supplies: new[] { new MrpSupplyInput(Screw, 100m, Due.AddDays(-10), "purchase_order", "PO-EARLY") }));
        Assert.DoesNotContain(early.Orders, o => o.ItemId == Screw);
    }

    [Fact]
    public void FlagsItemsWithNoSourcingRoute()
    {
        var plan = MrpEngine.Plan(Snapshot(demands: new[] { new MrpDemandInput(Mystery, 5m, Due, MrpDemandSource.Manual, "X") }));
        Assert.Equal(MrpAction.Shortage, Assert.Single(plan.Orders).Action);
    }

    [Fact]
    public void IsDeterministic_AndHashIgnoresInputOrder_ButTracksChanges()
    {
        var demands = new[]
        {
            new MrpDemandInput(Cabinet, 4m, Due, MrpDemandSource.Manual, "SO-1"),
            new MrpDemandInput(Cabinet, 6m, Due.AddDays(5), MrpDemandSource.Manual, "SO-2"),
            new MrpDemandInput(Screw, 9m, Due, MrpDemandSource.Manual, "SO-3"),
        };
        var a = MrpEngine.Plan(Snapshot(demands));
        var b = MrpEngine.Plan(Snapshot(demands.Reverse()));
        Assert.Equal(a.InputHash, b.InputHash);
        Assert.Equal(a.Orders.Select(o => (o.ItemId, o.Quantity, o.NeedBy, o.OrderBy)), b.Orders.Select(o => (o.ItemId, o.Quantity, o.NeedBy, o.OrderBy)));

        var changed = MrpEngine.Plan(Snapshot(demands, stock: new[] { new MrpStockInput(Screw, 1m) }));
        Assert.NotEqual(a.InputHash, changed.InputHash);
    }

    [Fact]
    public void SeparateNeedDates_AreNettedInDateOrder_AgainstSharedStock()
    {
        var plan = MrpEngine.Plan(Snapshot(
            demands: new[] { new MrpDemandInput(Screw, 30m, Due, MrpDemandSource.Manual, "A"), new MrpDemandInput(Screw, 30m, Due.AddDays(10), MrpDemandSource.Manual, "B") },
            stock: new[] { new MrpStockInput(Screw, 40m) }));

        var orders = plan.Orders.Where(o => o.ItemId == Screw).ToList();
        Assert.Equal(new[] { 20m }, orders.Select(o => o.Quantity));
        Assert.Equal(Due.AddDays(10), orders[0].NeedBy);
    }

    [Fact]
    public void RejectsBomCycles()
    {
        var cyclic = new MrpSnapshot(Parameters(), Items,
            new[] { new MrpBomInput(Cabinet, "A", 1, 1m, new[] { new MrpBomComponentInput(Panel, 1m, 0m) }), new MrpBomInput(Panel, "B", 1, 1m, new[] { new MrpBomComponentInput(Cabinet, 1m, 0m) }) },
            new[] { new MrpDemandInput(Cabinet, 1m, Due, MrpDemandSource.Manual, "X") }, Array.Empty<MrpSupplyInput>(), Array.Empty<MrpStockInput>());
        var ex = Assert.Throws<MrpDomainException>(() => MrpEngine.Plan(cyclic));
        Assert.Equal("MRP_BOM_CYCLE", ex.Code);
    }

    [Fact]
    public void RecommendationDecision_RequiresDifferentUser_AndConvertibleAction()
    {
        var run = Guid.NewGuid();
        var maker = Guid.NewGuid();
        var order = new MrpPlannedOrder(Plywood, MrpAction.Buy, 1m, Due, Due, 0, 1m, 0m, 0m, Array.Empty<MrpReason>());
        var rec = new MrpRecommendation(Guid.NewGuid(), Guid.NewGuid(), run, 1, order, "[]");
        Assert.Equal("MRP_SELF_APPROVAL", Assert.Throws<MrpDomainException>(() => rec.Decide(true, maker, maker, DateTimeOffset.UtcNow)).Code);
        rec.Decide(true, Guid.NewGuid(), maker, DateTimeOffset.UtcNow);
        Assert.Equal("approved", rec.Status);
        Assert.Equal("MRP_INVALID_STATE", Assert.Throws<MrpDomainException>(() => rec.Decide(false, Guid.NewGuid(), maker, DateTimeOffset.UtcNow)).Code);

        var shortage = new MrpRecommendation(Guid.NewGuid(), Guid.NewGuid(), run, 2, order with { Action = MrpAction.Shortage }, "[]");
        Assert.Equal("MRP_NOT_CONVERTIBLE", Assert.Throws<MrpDomainException>(() => shortage.Decide(true, Guid.NewGuid(), maker, DateTimeOffset.UtcNow)).Code);
        shortage.Decide(false, Guid.NewGuid(), maker, DateTimeOffset.UtcNow);
        Assert.Equal("rejected", shortage.Status);
    }
}
