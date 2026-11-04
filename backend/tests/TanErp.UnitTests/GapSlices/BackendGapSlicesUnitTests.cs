using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Application.QuickEstimates;
using TanErp.Domain.Files;
using TanErp.Domain.Items;
using TanErp.Domain.QuickEstimates;
using Xunit;

namespace TanErp.UnitTests.GapSlices;

public class BackendGapSlicesUnitTests
{
    [Fact]
    public void AllowedMediaTypes_IncludesPdf()
    {
        Assert.True(AllowedMediaTypes.IsAllowed("application/pdf"));
        Assert.Equal("application/pdf", AllowedMediaTypes.Pdf);
    }
    [Fact]
    public void BranchRateOverride_WithinThirtyPercentOfTemplateReferenceRate_Succeeds()
    {
        var now = DateTimeOffset.UtcNow;
        var today = new DateOnly(2026, 1, 1);
        var overrideRow = new BranchRateOverride(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1300m,
            5000m,
            today,
            today.AddDays(90),
            "LABOR_COST",
            "Regional labor adjustment",
            null,
            Guid.NewGuid(),
            today,
            now,
            templateReferenceRate: 1000m);

        Assert.Equal(1300m, overrideRow.ReferenceRate);
    }

    [Fact]
    public void BranchRateOverride_ExceedingThirtyPercentOfTemplateReferenceRate_ThrowsCapExceeded()
    {
        var now = DateTimeOffset.UtcNow;
        var today = new DateOnly(2026, 1, 1);
        var ex = Assert.Throws<QuickEstimateException>(() => new BranchRateOverride(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1301m,
            5000m,
            today,
            today.AddDays(90),
            "LABOR_COST",
            "Excessive override",
            null,
            Guid.NewGuid(),
            today,
            now,
            templateReferenceRate: 1000m));

        Assert.Equal("BRANCH_OVERRIDE_CAP_EXCEEDED", ex.Code);
    }

    [Fact]
    public void TemplateConfig_WithValidCostRecipe_MapsToEstimateCostComponentsOnConversion()
    {
        var itemId = Guid.NewGuid();
        var config = new TemplateConfig(
            [new TemplateGrade("STD", "มาตรฐาน", 1.0m)],
            [],
            [],
            0.05m,
            0.05m,
            0.05m,
            [],
            [],
            [new TemplateCostRecipeLine("material", "แผ่นไม้ MDF", "MDF Board", "SQM", 1.2m, 450m, itemId),
             new TemplateCostRecipeLine("labor", "ค่าแรงช่างไม้", "Carpentry Labor", "SQM", 1.0m, 300m)]);

        config.Validate();

        var snapshotJson = JsonSerializer.Serialize(new
        {
            template = new
            {
                code = "TPL-BUILT-IN",
                version = 1,
                workType = "built_in",
                unitCode = "SQM",
                config = new
                {
                    costRecipe = config.CostRecipe
                }
            },
            lines = new[]
            {
                new { lineId = "L1", workSubtype = "ตู้เสื้อผ้า", rawQuantity = 10m, wasteFactor = 0.1m, billableQuantity = 11m, appliedRate = 1500m, lineTotal = 16500m }
            }
        }, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var sections = QuickEstimateConversionMapper.BuildSections("QE-2026-0001", "Master Bedroom", 1, "TPL-BUILT-IN", snapshotJson);
        Assert.Single(sections);
        var workItem = Assert.Single(sections[0].WorkItems);
        Assert.Equal(2, workItem.CostComponents.Count);
        Assert.Equal("material", workItem.CostComponents[0].Type);
        Assert.Equal(13.2m, workItem.CostComponents[0].Quantity);
        Assert.Equal(450m, workItem.CostComponents[0].UnitCost);
        Assert.Equal(itemId, workItem.CostComponents[0].ItemId);
        Assert.Equal("labor", workItem.CostComponents[1].Type);
        Assert.Equal(11m, workItem.CostComponents[1].Quantity);
        Assert.Equal(300m, workItem.CostComponents[1].UnitCost);
    }

    [Fact]
    public void ItemImportJob_Cancel_AllowsQueuedOrRunning_AndRejectsTerminalStates()
    {
        var now = DateTimeOffset.UtcNow;
        var job = ItemImportJob.Enqueue(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "items",
            "create",
            "sha256",
            "code,name\nA,B",
            1,
            100,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "uid-1",
            "key-hash",
            "trace-1",
            now);

        job.Cancel(now.AddSeconds(5));
        Assert.Equal(ItemImportJobStatus.Cancelled, job.Status);
        Assert.Null(job.Content);
        Assert.NotNull(job.CompletedAtUtc);

        var ex = Assert.Throws<ItemDomainException>(() => job.Cancel(now.AddSeconds(10)));
        Assert.Equal("ITEM_IMPORT_JOB_NOT_CANCELLABLE", ex.Code);
    }

    [Fact]
    public void UnitConversion_And_ItemUnitConversion_UpdateAndDeactivate_BumpRowVersion()
    {
        var now = DateTimeOffset.UtcNow;
        var uc = new UnitConversion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            12m,
            new DateOnly(2026, 1, 1),
            null,
            "Initial",
            Guid.NewGuid(),
            now);
        var v1 = uc.RowVersion;

        uc.Update(24m, new DateOnly(2026, 2, 1), new DateOnly(2026, 12, 31), "Updated pack size", now);
        Assert.Equal(24m, uc.Factor);
        Assert.Equal(new DateOnly(2026, 2, 1), uc.EffectiveFrom);
        Assert.NotEqual(v1, uc.RowVersion);

        var v2 = uc.RowVersion;
        uc.Deactivate(new DateOnly(2026, 6, 30), "Discontinued pack", now);
        Assert.Equal(ItemStatus.Inactive, uc.Status);
        Assert.Equal(new DateOnly(2026, 6, 30), uc.EffectiveTo);
        Assert.NotEqual(v2, uc.RowVersion);

        var iuc = new ItemUnitConversion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            10m,
            new DateOnly(2026, 1, 1),
            null,
            "Initial item conversion",
            Guid.NewGuid(),
            now);
        iuc.Update(20m, null, new DateOnly(2026, 12, 31), "Updated item factor", now);
        Assert.Equal(20m, iuc.Factor);
        iuc.Deactivate(new DateOnly(2026, 6, 30), "Deactivated item factor", now);
        Assert.Equal(ItemStatus.Inactive, iuc.Status);
    }

    [Fact]
    public void SystemMaintenanceAndReminderPlanner_SelectsSmallestApplicableWarrantyThreshold()
    {
        var thresholds = SystemMaintenanceAndReminderPlanner.NormalizeWarrantyThresholds(null);
        var today = new DateOnly(2026, 6, 1);

        Assert.Equal(30, SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, today.AddDays(20), thresholds));
        Assert.Equal(7, SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, today.AddDays(5), thresholds));
        Assert.Equal(1, SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, today.AddDays(1), thresholds));
        Assert.Null(SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, today.AddDays(45), thresholds));
        Assert.Null(SystemMaintenanceAndReminderPlanner.SelectWarrantyThreshold(today, today.AddDays(-1), thresholds));
    }
}
