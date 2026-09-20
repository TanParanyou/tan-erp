using TanErp.Domain.Estimates;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public class EstimateTests
{
    private readonly Guid _orgId = Guid.NewGuid();

    [Fact]
    public void CreateDraft_InitializesEstimateWithRevisionOne()
    {
        var estimateId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var oppId = Guid.NewGuid();
        var surveyRevId = Guid.NewGuid();
        var snapshotHash = "hash123456789";

        var estimate = Estimate.CreateDraft(
            estimateId,
            _orgId,
            branchId,
            customerId,
            oppId,
            "EST-2026-0001",
            surveyRevId,
            snapshotHash);

        Assert.Equal(estimateId, estimate.Id);
        Assert.Equal(_orgId, estimate.OrganizationId);
        Assert.Equal("EST-2026-0001", estimate.Number);
        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.Equal(1, estimate.CurrentRevisionNo);
        Assert.NotNull(estimate.CurrentRevision);
        Assert.Equal(1, estimate.CurrentRevision.RevisionNo);
        Assert.Equal(_orgId, estimate.CurrentRevision.OrganizationId);
        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision.Status);
        Assert.Equal(surveyRevId, estimate.SiteSurveyRevisionId);
        Assert.Equal(snapshotHash, estimate.SiteSurveySnapshotHash);
    }

    [Fact]
    public void Calculate_WithMarginSellingRule_CalculatesCorrectTotalsAndMarginRate()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งาน Built-in", "Built-in Works");
        
        // Work item 1: 2 units, margin 30% (0.30)
        var workItem = new EstimateWorkItem(
            Guid.NewGuid(),
            _orgId,
            section.Id,
            "WI-01",
            "ตู้เสื้อผ้า Built-in",
            "Built-in Wardrobe",
            quantity: 2m,
            unitCode: "ชุด",
            sellingRuleType: SellingRuleType.Margin,
            sellingRuleValue: 0.30m);

        // Component 1: Material cost 10,000 THB x 1 = 10,000 THB
        var material = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            workItem.Id,
            CostComponentType.Material,
            "ไม้ MDF",
            quantity: 10m,
            unitCode: "แผ่น",
            unitCost: 1000m);

        // Component 2: Labor cost 4,000 THB
        var labor = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            workItem.Id,
            CostComponentType.Labor,
            "ค่าแรงติดตั้ง",
            quantity: 2m,
            unitCode: "คน/วัน",
            unitCost: 2000m);

        workItem.AddCostComponent(material);
        workItem.AddCostComponent(labor);
        section.AddWorkItem(workItem);
        revision.AddSection(section);

        // Total Cost = 10,000 + 4,000 = 14,000 THB
        // Selling Price with Margin 30% = Cost / (1 - 0.30) = 14,000 / 0.70 = 20,000 THB
        // Tax 7% = 20,000 * 0.07 = 1,400 THB
        // Grand Total = 20,000 + 1,400 = 21,400 THB
        // Margin Amount = 20,000 - 14,000 = 6,000 THB
        // Margin Rate = 6,000 / 20,000 = 0.30 (30%)

        revision.Calculate();

        Assert.Equal(14000m, revision.NetCost);
        Assert.Equal(20000m, revision.SellingBeforeDiscount);
        Assert.Equal(0m, revision.DiscountAmount);
        Assert.Equal(20000m, revision.NetBeforeTax);
        Assert.Equal(1400m, revision.TaxAmount);
        Assert.Equal(21400m, revision.GrandTotal);
        Assert.Equal(6000m, revision.MarginAmount);
        Assert.Equal(0.30m, revision.MarginRate);
        Assert.Equal(1, revision.CalculationVersion);
        Assert.NotNull(revision.CalculationSnapshotJson);
    }

    [Fact]
    public void Calculate_WithDiscount_AdjustsTaxAndGrandTotalProperly()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งานไฟฟ้า", null);
        
        var workItem = new EstimateWorkItem(
            Guid.NewGuid(),
            _orgId,
            section.Id,
            "WI-01",
            "เดินสายไฟ",
            null,
            quantity: 1m,
            unitCode: "จุด",
            sellingRuleType: SellingRuleType.FixedPrice,
            sellingRuleValue: 10000m);

        var cost = new EstimateCostComponent(
            Guid.NewGuid(),
            _orgId,
            workItem.Id,
            CostComponentType.Material,
            "สายไฟและท่อร้อย",
            quantity: 1m,
            unitCode: "เหมา",
            unitCost: 5000m);

        workItem.AddCostComponent(cost);
        section.AddWorkItem(workItem);
        revision.AddSection(section);

        // Selling = 10,000 THB, Discount = 1,000 THB
        // Net Before Tax = 9,000 THB
        // Tax 7% = 9,000 * 0.07 = 630 THB
        // Grand Total = 9,630 THB
        // Margin Amount = 9,000 - 5,000 = 4,000 THB
        revision.Calculate(discountAmount: 1000m);

        Assert.Equal(5000m, revision.NetCost);
        Assert.Equal(10000m, revision.SellingBeforeDiscount);
        Assert.Equal(1000m, revision.DiscountAmount);
        Assert.Equal(9000m, revision.NetBeforeTax);
        Assert.Equal(630m, revision.TaxAmount);
        Assert.Equal(9630m, revision.GrandTotal);
        Assert.Equal(4000m, revision.MarginAmount);
    }

    [Fact]
    public void MarkQuoted_TransitionsEstimateAndRevisionToQuoted()
    {
        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-2026-0002");

        var oldEstimateVersion = estimate.RowVersion;
        var oldRevVersion = estimate.CurrentRevision!.RowVersion;

        estimate.MarkQuoted();

        Assert.Equal(EstimateStatus.Quoted, estimate.Status);
        Assert.Equal(EstimateRevisionStatus.Quoted, estimate.CurrentRevision.Status);
        Assert.NotEqual(oldEstimateVersion, estimate.RowVersion);
        Assert.NotEqual(oldRevVersion, estimate.CurrentRevision.RowVersion);
    }
}
