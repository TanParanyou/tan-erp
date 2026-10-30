using TanErp.Domain.Production;
using Xunit;

namespace TanErp.UnitTests.Production;

public class ProductionCompletionDomainTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid ItemId = Guid.NewGuid();
    private static readonly Guid ComponentItemId = Guid.NewGuid();
    private static readonly Guid WarehouseId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 30, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void WorkCenter_ValidatesCapacityAndHourlyRate_AndSupportsStatusTransitions()
    {
        var wc = new WorkCenter(Guid.NewGuid(), OrgId, BranchId, " wc-01 ", " เครื่องตัดเลเซอร์ CNC ", 8.5m, 450m, UserId, Now);
        Assert.Equal("wc-01", wc.Code);
        Assert.Equal("WC-01", wc.NormalizedCode);
        Assert.Equal("เครื่องตัดเลเซอร์ CNC", wc.Name);
        Assert.Equal(8.5m, wc.DailyCapacityHours);
        Assert.Equal(450m, wc.HourlyRate);
        Assert.Equal(WorkCenterStatus.Active, wc.Status);

        wc.Update("เครื่องตัดเลเซอร์ไฟเบอร์", 16m, 500m, Now.AddMinutes(5));
        Assert.Equal("เครื่องตัดเลเซอร์ไฟเบอร์", wc.Name);
        Assert.Equal(16m, wc.DailyCapacityHours);
        Assert.Equal(500m, wc.HourlyRate);

        wc.Deactivate(Now.AddMinutes(10));
        Assert.Equal(WorkCenterStatus.Inactive, wc.Status);

        wc.Activate(Now.AddMinutes(15));
        Assert.Equal(WorkCenterStatus.Active, wc.Status);

        var invalidLowCap = Assert.Throws<ProductionDomainException>(() =>
            new WorkCenter(Guid.NewGuid(), OrgId, BranchId, "WC-02", "Welding", 0.1m, 300m, UserId, Now));
        Assert.Equal("PRODUCTION_WORK_CENTER_INVALID", invalidLowCap.Code);

        var invalidHighCap = Assert.Throws<ProductionDomainException>(() =>
            new WorkCenter(Guid.NewGuid(), OrgId, BranchId, "WC-02", "Welding", 24.5m, 300m, UserId, Now));
        Assert.Equal("PRODUCTION_WORK_CENTER_INVALID", invalidHighCap.Code);

        var invalidRate = Assert.Throws<ProductionDomainException>(() =>
            new WorkCenter(Guid.NewGuid(), OrgId, BranchId, "WC-02", "Welding", 8m, -1m, UserId, Now));
        Assert.Equal("PRODUCTION_WORK_CENTER_INVALID", invalidRate.Code);
    }

    [Fact]
    public void BomRevision_RoutingSteps_EnforceSequenceAndSubcontractInvariants()
    {
        var bom = new Bom(Guid.NewGuid(), OrgId, ItemId, "BOM-00001", UserId, Now);
        var rev = new BomRevision(Guid.NewGuid(), OrgId, bom.Id, 1, 2m, "Rev 1", UserId, Now);
        bom.AddRevision(rev);
        rev.AddLine(new BomLine(Guid.NewGuid(), OrgId, rev.Id, ComponentItemId, 4m, 5m, 1));

        var wcId = Guid.NewGuid();
        var step1 = new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 10, "OP-10", "ตัดโครงสร้าง", wcId, 1m, 2m, false, 0m, "Internal cut");
        var step2 = new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 20, "OP-20", "ชุบกัลวาไนซ์ภายนอก", null, 0m, 0m, true, 1500m, "Outsource coating");
        rev.AddRoutingStep(step1);
        rev.AddRoutingStep(step2);

        Assert.Equal(2, rev.RoutingSteps.Count);
        Assert.Equal(wcId, step1.WorkCenterId);
        Assert.True(step2.IsSubcontracted);
        Assert.Null(step2.WorkCenterId);
        Assert.Equal(1500m, step2.StdSubcontractCost);

        // Duplicate sequence must fail
        var dupSeq = Assert.Throws<ProductionDomainException>(() =>
            rev.AddRoutingStep(new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 10, "OP-15", "เชื่อมประกอบ", wcId, 0.5m, 1m, false, 0m, null)));
        Assert.Equal("BOM_ROUTING_INVALID", dupSeq.Code);

        // Internal operation without WorkCenterId must fail
        var missingWc = Assert.Throws<ProductionDomainException>(() =>
            new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 30, "OP-30", "ประกอบ", null, 0.5m, 1m, false, 0m, null));
        Assert.Equal("BOM_ROUTING_INVALID", missingWc.Code);

        // Subcontract operation with WorkCenterId must fail
        var subWithWc = Assert.Throws<ProductionDomainException>(() =>
            new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 40, "OP-40", "อบสีฝุ่น", wcId, 0m, 0m, true, 800m, null));
        Assert.Equal("BOM_ROUTING_INVALID", subWithWc.Code);

        // Internal operation with zero setup + zero run hours must fail
        var zeroHours = Assert.Throws<ProductionDomainException>(() =>
            new BomRoutingStep(Guid.NewGuid(), OrgId, rev.Id, 50, "OP-50", "ตรวจสอบ", wcId, 0m, 0m, false, 0m, null));
        Assert.Equal("BOM_ROUTING_INVALID", zeroHours.Code);
    }

    [Fact]
    public void WorkOrder_TotalManufacturingCost_AllocatesMaterialLaborAndSubcontractAcrossPartialAndFullCompletions()
    {
        var wo = new WorkOrder(
            Guid.NewGuid(), OrgId, BranchId, "WO-2026-00001",
            ItemId, Guid.NewGuid(), WarehouseId, projectId: Guid.NewGuid(),
            plannedQuantity: 10m, note: "Order 10 sets", createdByUserId: UserId, now: Now,
            plannedStartDate: new DateOnly(2026, 10, 30),
            plannedEndDate: new DateOnly(2026, 11, 2));

        wo.AddMaterial(new WorkOrderMaterial(Guid.NewGuid(), OrgId, wo.Id, ComponentItemId, 10m, 1));
        var wcId = Guid.NewGuid();
        var opInternal = new WorkOrderOperation(Guid.NewGuid(), OrgId, wo.Id, 10, "OP-10", "ตัดพับ", wcId, 1m, 5m, false, null);
        var opSub = new WorkOrderOperation(Guid.NewGuid(), OrgId, wo.Id, 20, "OP-20", "ชุบสี", null, 0m, 0m, true, null);
        wo.AddOperation(opInternal);
        wo.AddOperation(opSub);

        wo.Release(Now.AddMinutes(5));

        // Issue material: 10 units valued at 5,000 THB
        wo.RecordIssue(ComponentItemId, 10m, 5000m, Now.AddMinutes(10));
        Assert.Equal(5000m, wo.NetIssuedValue);

        // Record approved timesheet labor: 8 hours, 2,000 THB
        wo.RecordLabor(8m, 2000m, Now.AddMinutes(15));
        Assert.Equal(8m, wo.LaborHours);
        Assert.Equal(2000m, wo.LaborCost);

        // Complete internal operation
        wo.CompleteOperation(opInternal.Id, 6.5m, "เสร็จตามแผน", Now.AddMinutes(20));
        Assert.Equal(WorkOrderOperationStatus.Completed, opInternal.Status);
        Assert.Equal(6.5m, opInternal.ActualHours);

        // Record subcontract PO: 3,000 THB
        var poId = Guid.NewGuid();
        wo.RecordSubcontract(opSub.Id, poId, 3000m, "PO งานชุบ", Now.AddMinutes(25));
        Assert.Equal(WorkOrderOperationStatus.Completed, opSub.Status);
        Assert.Equal(poId, opSub.SubcontractPurchaseOrderId);
        Assert.Equal(3000m, wo.SubcontractCost);

        // TotalManufacturingCost = 5,000 + 2,000 + 3,000 = 10,000 THB
        Assert.Equal(10000m, wo.TotalManufacturingCost);

        // Return material and verify Cancel still blocked by actual labor/subcontract/completed operations
        wo.RecordReturn(ComponentItemId, 10m, 5000m, Now.AddMinutes(26));
        var cancelEx = Assert.Throws<ProductionDomainException>(() => wo.Cancel("ต้องการยกเลิก", Now.AddMinutes(27)));
        Assert.Equal("PRODUCTION_HAS_ACTUAL_COSTS", cancelEx.Code);

        // Re-issue material 10 units @ 5,000 THB
        wo.RecordIssue(ComponentItemId, 10m, 5000m, Now.AddMinutes(28));
        Assert.Equal(10000m, wo.TotalManufacturingCost);

        // Partial completion: 4 of 10 units -> 40% of 10,000 = 4,000 THB
        var val1 = wo.Complete(4m, Now.AddMinutes(30));
        Assert.Equal(4000m, val1);
        Assert.Equal(WorkOrderStatus.InProgress, wo.Status);

        // Final completion: remaining 6 of 10 units -> remaining 6,000 THB
        var val2 = wo.Complete(6m, Now.AddMinutes(40));
        Assert.Equal(6000m, val2);
        Assert.Equal(WorkOrderStatus.Completed, wo.Status);
        Assert.Equal(10000m, wo.CostAllocated);
    }
}
