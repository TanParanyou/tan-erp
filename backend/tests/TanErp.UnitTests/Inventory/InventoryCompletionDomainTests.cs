using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Inventory;

public class InventoryCompletionDomainTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid WarehouseId = Guid.NewGuid();
    private static readonly Guid ItemId = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 29, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void WarehouseLocation_DefaultCannotBeDeactivated_AndValidatesFields()
    {
        var defaultLoc = WarehouseLocation.CreateDefault(OrgId, WarehouseId, Now);
        Assert.Equal("DEFAULT", defaultLoc.Code);
        Assert.True(defaultLoc.IsDefault);
        Assert.Equal(WarehouseLocationStatus.Active, defaultLoc.Status);

        var ex = Assert.Throws<InventoryDomainException>(() => defaultLoc.Update("Default Bin", active: false, Now.AddMinutes(1)));
        Assert.Equal("LOCATION_IS_DEFAULT", ex.Code);

        var bin = new WarehouseLocation(Guid.NewGuid(), OrgId, WarehouseId, " bin-a1 ", " ชั้นวาง A1 ", isDefault: false, Now);
        Assert.Equal("bin-a1", bin.Code);
        Assert.Equal("BIN-A1", bin.NormalizedCode);
        Assert.Equal("ชั้นวาง A1", bin.Name);
        Assert.False(bin.IsDefault);

        bin.Update("ชั้นวาง A1-แก้ไข", active: false, Now.AddMinutes(2));
        Assert.Equal("ชั้นวาง A1-แก้ไข", bin.Name);
        Assert.Equal(WarehouseLocationStatus.Inactive, bin.Status);

        var invalidCode = Assert.Throws<InventoryDomainException>(() =>
            new WarehouseLocation(Guid.NewGuid(), OrgId, WarehouseId, " ", "Valid Name", false, Now));
        Assert.Equal("LOCATION_FIELD_INVALID", invalidCode.Code);
    }

    [Fact]
    public void StockLocationBalance_NormalizesLot_AndRefusesNegativeStock()
    {
        var locId = Guid.NewGuid();
        var balance = new StockLocationBalance(Guid.NewGuid(), OrgId, WarehouseId, locId, ItemId, UnitId, " lot-2026-a ", Now);
        Assert.Equal("LOT-2026-A", balance.LotNumber);
        Assert.Equal(0m, balance.OnHand);

        balance.Receive(10.5m, Now.AddMinutes(1));
        Assert.Equal(10.5m, balance.OnHand);

        balance.Issue(4.5m, Now.AddMinutes(2));
        Assert.Equal(6m, balance.OnHand);

        var ex = Assert.Throws<InventoryDomainException>(() => balance.Issue(6.0001m, Now.AddMinutes(3)));
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", ex.Code);
        Assert.Equal(6m, balance.OnHand);
    }

    [Fact]
    public void StockSerial_TransitionsBetweenInStockIssuedTransferredAndReturned()
    {
        var locA = Guid.NewGuid();
        var locB = Guid.NewGuid();
        var whB = Guid.NewGuid();
        var doc1 = Guid.NewGuid();
        var doc2 = Guid.NewGuid();
        var doc3 = Guid.NewGuid();
        var doc4 = Guid.NewGuid();

        var serial = new StockSerial(Guid.NewGuid(), OrgId, ItemId, " sn-001 ", WarehouseId, locA, "lot-1", doc1, Now);
        Assert.Equal("sn-001", serial.SerialNumber);
        Assert.Equal("SN-001", serial.NormalizedSerialNumber);
        Assert.Equal("LOT-1", serial.LotNumber);
        Assert.Equal(StockSerialStatus.InStock, serial.Status);

        serial.TransferTo(whB, locB, doc2, Now.AddMinutes(1));
        Assert.Equal(whB, serial.WarehouseId);
        Assert.Equal(locB, serial.LocationId);
        Assert.Equal(StockSerialStatus.InStock, serial.Status);

        serial.IssueOut(doc3, Now.AddMinutes(2));
        Assert.Equal(StockSerialStatus.Issued, serial.Status);
        Assert.Null(serial.WarehouseId);
        Assert.Null(serial.LocationId);

        // Cannot issue again while already issued
        var issueAgain = Assert.Throws<InventoryDomainException>(() => serial.IssueOut(Guid.NewGuid(), Now.AddMinutes(3)));
        Assert.Equal("INVENTORY_SERIAL_NOT_AVAILABLE", issueAgain.Code);

        // Can return from project back into stock
        serial.ReturnFromProject(WarehouseId, locA, "lot-1", doc4, Now.AddMinutes(4));
        Assert.Equal(StockSerialStatus.InStock, serial.Status);
        Assert.Equal(WarehouseId, serial.WarehouseId);
        Assert.Equal(locA, serial.LocationId);
    }

    [Fact]
    public void InventoryPeriodClose_ComputesPeriodEndDate_AndAggregatesLines()
    {
        var close = new InventoryPeriodClose(Guid.NewGuid(), OrgId, 2024, 2, " ปิดงวด ก.พ. ปีอธิกสุรทิน ", UserId, Now);
        Assert.Equal(new DateOnly(2024, 2, 29), close.PeriodEndDate);
        Assert.Equal("ปิดงวด ก.พ. ปีอธิกสุรทิน", close.Note);

        close.AddLine(new InventoryPeriodCloseLine(Guid.NewGuid(), close.Id, WarehouseId, ItemId, UnitId, 10m, 125.5m, 1255m));
        close.AddLine(new InventoryPeriodCloseLine(Guid.NewGuid(), close.Id, WarehouseId, Guid.NewGuid(), UnitId, 5m, 200m, 1000m));

        Assert.Equal(2, close.LineCount);
        Assert.Equal(15m, close.TotalOnHandQuantity);
        Assert.Equal(2255m, close.TotalValuationAmount);

        var invalidMonth = Assert.Throws<InventoryDomainException>(() =>
            new InventoryPeriodClose(Guid.NewGuid(), OrgId, 2026, 13, null, UserId, Now));
        Assert.Equal("INVENTORY_PERIOD_INVALID", invalidMonth.Code);
    }

    [Fact]
    public void ItemCapabilities_SupportsTrackLotAndTrackSerial_WithBackwardCompatibleDefaults()
    {
        var defaultCaps = new ItemCapabilities(true, true, true, true, false);
        Assert.False(defaultCaps.TrackLot);
        Assert.False(defaultCaps.TrackSerial);

        var trackedCaps = new ItemCapabilities(true, true, true, true, false, TrackLot: true, TrackSerial: true);
        Assert.True(trackedCaps.TrackLot);
        Assert.True(trackedCaps.TrackSerial);
    }
}
