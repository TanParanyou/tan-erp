using TanErp.Domain.Projects;
using Xunit;

namespace TanErp.UnitTests.Projects;

public class ProjectWbsCostAndTimesheetTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid ProjectId = Guid.NewGuid();
    private static readonly Guid CreatorId = Guid.NewGuid();
    private static readonly Guid WorkerId = Guid.NewGuid();
    private static readonly Guid ApproverId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 29, 10, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ProjectWbsNode_ValidDepths_AreAcceptedAndUpdated(int level)
    {
        var parentId = level == 1 ? (Guid?)null : Guid.NewGuid();
        var node = new ProjectWbsNode(Guid.NewGuid(), OrgId, ProjectId, parentId, "1.1", "งานโครงสร้าง", level, 0);

        Assert.Equal("1.1", node.Code);
        Assert.Equal("งานโครงสร้าง", node.Name);
        Assert.Equal(level, node.Level);

        var originalVersion = node.RowVersion;
        node.Update("1.1-A", "งานโครงสร้างหลัก");
        Assert.Equal("1.1-A", node.Code);
        Assert.Equal("งานโครงสร้างหลัก", node.Name);
        Assert.NotEqual(originalVersion, node.RowVersion);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    public void ProjectWbsNode_DepthGreaterThan3_ThrowsDepthExceeded(int level)
    {
        var ex = Assert.Throws<ProjectDomainException>(() =>
            new ProjectWbsNode(Guid.NewGuid(), OrgId, ProjectId, Guid.NewGuid(), "1.1.1.1", "เกินระดับ", level, 0));

        Assert.Equal("PROJECT_WBS_DEPTH_EXCEEDED", ex.Code);
    }

    [Theory]
    [InlineData("", "Valid Name")]
    [InlineData("1.1", "")]
    public void ProjectWbsNode_BlankCodeOrName_ThrowsInvalid(string code, string name)
    {
        var ex = Assert.Throws<ProjectDomainException>(() =>
            new ProjectWbsNode(Guid.NewGuid(), OrgId, ProjectId, null, code, name, 1, 0));

        Assert.Equal("PROJECT_WBS_INVALID", ex.Code);
    }

    [Fact]
    public void LaborTimesheetEntry_ValidatesHoursDailyCapOvertimeAndImmutability()
    {
        // Under 0.25h -> invalid
        var tooShort = Assert.Throws<ProjectDomainException>(() =>
            new LaborTimesheetEntry(Guid.NewGuid(), OrgId, BranchId, ProjectId, Guid.NewGuid(), null,
                "TS-0001", WorkerId, new DateOnly(2026, 10, 29), 0.10m, 200m, 0m, "เตรียมไม้", CreatorId, Now));
        Assert.Equal("TIMESHEET_HOURS_INVALID", tooShort.Code);

        // Exceeds 24h cumulative day cap -> daily hours exceeded
        var dailyExceeded = Assert.Throws<ProjectDomainException>(() =>
            new LaborTimesheetEntry(Guid.NewGuid(), OrgId, BranchId, ProjectId, Guid.NewGuid(), null,
                "TS-0002", WorkerId, new DateOnly(2026, 10, 29), 6.5m, 200m, 18.0m, "ประกอบตู้", CreatorId, Now));
        Assert.Equal("TIMESHEET_DAILY_HOURS_EXCEEDED", dailyExceeded.Code);

        // Normal 8h entry -> no overtime warning
        var regular = new LaborTimesheetEntry(Guid.NewGuid(), OrgId, BranchId, ProjectId, Guid.NewGuid(), null,
            "TS-0003", WorkerId, new DateOnly(2026, 10, 29), 8.0m, 200m, 0m, "ประกอบตู้", CreatorId, Now);
        Assert.False(regular.IsOvertimeWarning);
        Assert.Equal(1600.00m, regular.CostAmount);

        // Overtime (> 8h cumulative) -> overtime warning set
        var overtime = new LaborTimesheetEntry(Guid.NewGuid(), OrgId, BranchId, ProjectId, Guid.NewGuid(), null,
            "TS-0004", WorkerId, new DateOnly(2026, 10, 29), 2.5m, 200m, 8.0m, "เก็บงานล่วงเวลา", CreatorId, Now);
        Assert.True(overtime.IsOvertimeWarning);
        Assert.Equal(500.00m, overtime.CostAmount);

        overtime.Submit(Now);
        Assert.Equal(TimesheetStatus.Submitted, overtime.Status);

        // Maker-checker: neither creator nor worker can approve
        var selfCreator = Assert.Throws<ProjectDomainException>(() => overtime.Decide(true, CreatorId, null, Now));
        Assert.Equal("TIMESHEET_SELF_APPROVAL", selfCreator.Code);

        var selfWorker = Assert.Throws<ProjectDomainException>(() => overtime.Decide(true, WorkerId, null, Now));
        Assert.Equal("TIMESHEET_SELF_APPROVAL", selfWorker.Code);

        // Independent approver succeeds
        overtime.Decide(true, ApproverId, "อนุมัติ OT", Now);
        Assert.Equal(TimesheetStatus.Approved, overtime.Status);

        // Once approved, timesheet is strictly immutable
        Assert.Equal("TIMESHEET_IMMUTABLE", Assert.Throws<ProjectDomainException>(() => overtime.Cancel()).Code);
        Assert.Equal("TIMESHEET_IMMUTABLE", Assert.Throws<ProjectDomainException>(() => overtime.Submit(Now)).Code);
        Assert.Equal("TIMESHEET_IMMUTABLE", Assert.Throws<ProjectDomainException>(() => overtime.Decide(false, ApproverId, "เปลี่ยนใจ", Now)).Code);
    }

    [Fact]
    public void ProjectCostLedgerEntry_ValidatesAndCreatesReversal()
    {
        var wbsId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var entry = new ProjectCostLedgerEntry(
            Guid.NewGuid(),
            OrgId,
            ProjectId,
            wbsId,
            ProjectBudgetCategory.Material,
            ProjectCostEntryType.Commitment,
            ProjectCostSourceType.PurchaseOrder,
            sourceId,
            "PO-202610-0001",
            "PO commitment",
            12500.50m,
            false,
            CreatorId,
            Now);

        Assert.Equal(12500.50m, entry.Amount);
        Assert.False(entry.IsReversal);

        var reversal = entry.CreateReversal(Guid.NewGuid(), ApproverId, Now.AddHours(1), "PO cancelled");
        Assert.Equal(-12500.50m, reversal.Amount);
        Assert.True(reversal.IsReversal);
        Assert.Equal(entry.SourceId, reversal.SourceId);
        Assert.Equal(entry.Category, reversal.Category);
        Assert.Equal(entry.EntryType, reversal.EntryType);

        var zeroEx = Assert.Throws<ProjectDomainException>(() =>
            new ProjectCostLedgerEntry(
                Guid.NewGuid(), OrgId, ProjectId, null, ProjectBudgetCategory.Material,
                ProjectCostEntryType.Actual, ProjectCostSourceType.StockIssue, sourceId, "SI-0001", "Zero", 0m, false, CreatorId, Now));
        Assert.Equal("PROJECT_COST_ENTRY_INVALID", zeroEx.Code);
    }
}
