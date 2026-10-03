using TanErp.Domain.Estimates;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public class EstimateTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _makerId = Guid.NewGuid();
    private readonly Guid _checkerId = Guid.NewGuid();

    [Fact]
    public void CostComponent_CalculatesQuantityTimesUnitCostWithPolicyRounding()
    {
        var workItemId = Guid.NewGuid();
        var component = new EstimateCostComponent(
            Guid.NewGuid(), _orgId, workItemId, CostComponentType.Material,
            "TEST_ONLY rounding", quantity: 3m, unitCode: "unit", unitCost: 33.335m);

        Assert.Equal(100.01m, component.TotalCost);
    }

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
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");

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

        Calculate(revision);

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
        Assert.Equal(EstimateReadinessStatus.RequiresAttention, revision.EvaluateReadiness().Status);
        Assert.Contains("\"readiness\":\"requiresAttention\"", revision.CalculationSnapshotJson, StringComparison.Ordinal);
    }

    [Fact]
    public void EvaluateReadiness_BlocksMissingCostAndReturnsWorkItemPointer()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(Guid.NewGuid(), _orgId, section.Id,
            "WI-01", "งานที่ยังไม่มีต้นทุน", null, 1m, "unit", SellingRuleType.Margin, 0.25m);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        section.AddWorkItem(workItem);
        revision.AddSection(section);
        Calculate(revision);

        var readiness = revision.EvaluateReadiness();

        Assert.Equal(EstimateReadinessStatus.Blocked, readiness.Status);
        var reason = Assert.Single(readiness.Reasons);
        Assert.Equal("ESTIMATE_COST_INCOMPLETE", reason.Code);
        Assert.Equal("workItem", reason.TargetType);
        Assert.Equal(workItem.Id, reason.TargetId);
        Assert.Equal("costComponents", reason.TargetField);
    }

    [Fact]
    public void EvaluateReadiness_BlocksUnexplainedProvisionalCostAndRequiresAttentionAfterReasonIsProvided()
    {
        var revision = CreateRevisionWithSingleCostWorkItem(100m, SellingRuleType.Margin, 0.25m);
        var component = revision.Sections.Single().WorkItems.Single().CostComponents.Single();
        Calculate(revision);
        component.SetProvisionalReason(null, null);

        var blocked = revision.EvaluateReadiness();
        Assert.Equal(EstimateReadinessStatus.Blocked, blocked.Status);
        Assert.Contains(blocked.Reasons, reason => reason.Code == "ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED" &&
            reason.TargetId == component.Id && reason.TargetField == "provisionalReasonCode");

        component.SetProvisionalReason("market-benchmark", "Synthetic fixture");
        var requiresAttention = revision.EvaluateReadiness();
        Assert.Equal(EstimateReadinessStatus.RequiresAttention, requiresAttention.Status);
        Assert.Contains(requiresAttention.Reasons, reason => reason.Code == "ESTIMATE_PROVISIONAL_COST");
    }

    [Fact]
    public void EvaluateReadiness_RequiresAttentionWhenMarginDenominatorIsZero()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(Guid.NewGuid(), _orgId, section.Id,
            "WI-01", "งานราคาศูนย์", null, 1m, "unit", SellingRuleType.FixedPrice, 0m, 1, "TEST_ONLY_ZERO_PRICE");
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), _orgId, workItem.Id,
            CostComponentType.Material, "TEST_ONLY zero cost", 1m, "unit", 0m));
        section.AddWorkItem(workItem);
        revision.AddSection(section);
        Calculate(revision);

        var readiness = revision.EvaluateReadiness();

        Assert.Equal(EstimateReadinessStatus.RequiresAttention, readiness.Status);
        Assert.Contains(readiness.Reasons, reason => reason.Code == "ESTIMATE_ZERO_DENOMINATOR");
    }

    [Fact]
    public void FixedPriceWithReason_RequiresAttentionAndPersistsReasonInSnapshot()
    {
        var revision = CreateRevisionWithSingleCostWorkItem(100m, SellingRuleType.FixedPrice, 125m);
        var workItem = revision.Sections.Single().WorkItems.Single();
        workItem.Update(workItem.Code, workItem.DescriptionTh, workItem.DescriptionEn, workItem.Quantity,
            workItem.UnitCode, SellingRuleType.FixedPrice, 125m, workItem.SortOrder, "TEST_ONLY_PRICE_OVERRIDE");
        Calculate(revision, taxMode: "exempt");

        Assert.Equal("TEST_ONLY_PRICE_OVERRIDE", workItem.SellingRuleReasonCode);
        Assert.Equal(EstimateReadinessStatus.RequiresAttention, revision.EvaluateReadiness().Status);
        Assert.Contains(revision.EvaluateReadiness().Reasons, reason =>
            reason.Code == "ESTIMATE_FIXED_PRICE_OVERRIDE" && reason.TargetId == workItem.Id);
        Assert.Contains("TEST_ONLY_PRICE_OVERRIDE", revision.CalculationSnapshotJson, StringComparison.Ordinal);
    }

    [Fact]
    public void FixedPriceWithoutReason_BlocksReadiness()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(Guid.NewGuid(), _orgId, section.Id,
            "WI-01", "รายการราคาคงที่เดิม", null, 1m, "unit", SellingRuleType.FixedPrice, 125m);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), _orgId, workItem.Id,
            CostComponentType.Material, "TEST_ONLY cost", 1m, "unit", 100m));
        section.AddWorkItem(workItem);
        revision.AddSection(section);
        Calculate(revision, taxMode: "exempt");

        Assert.Equal(EstimateReadinessStatus.Blocked, revision.EvaluateReadiness().Status);
        Assert.Contains(revision.EvaluateReadiness().Reasons, reason =>
            reason.Code == "ESTIMATE_FIXED_PRICE_REASON_REQUIRED" && reason.TargetId == workItem.Id);
    }

    [Fact]
    public void UpdatingFixedPriceWithoutReason_IsRejected()
    {
        var revision = CreateRevisionWithSingleCostWorkItem(100m, SellingRuleType.Margin, 0.25m);
        var workItem = revision.Sections.Single().WorkItems.Single();

        Assert.Throws<EstimateFixedPriceReasonRequiredException>(() => workItem.Update(
            workItem.Code, workItem.DescriptionTh, workItem.DescriptionEn, workItem.Quantity,
            workItem.UnitCode, SellingRuleType.FixedPrice, 125m, workItem.SortOrder));
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
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");

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
        Calculate(revision, discountAmount: 1000m);

        Assert.Equal(5000m, revision.NetCost);
        Assert.Equal(10000m, revision.SellingBeforeDiscount);
        Assert.Equal(1000m, revision.DiscountAmount);
        Assert.Equal(9000m, revision.NetBeforeTax);
        Assert.Equal(630m, revision.TaxAmount);
        Assert.Equal(9630m, revision.GrandTotal);
        Assert.Equal(4000m, revision.MarginAmount);
    }

    [Fact]
    public void Calculate_WithPercentDiscount_UsesDocumentSellingTotalAndPersistsReasonInSnapshot()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-DISC", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(Guid.NewGuid(), _orgId, section.Id,
            "WI-01", "งานราคา", null, 1m, "unit", SellingRuleType.FixedPrice, 1000m);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), _orgId, workItem.Id,
            CostComponentType.Material, "TEST_ONLY cost", 1m, "unit", 500m));
        section.AddWorkItem(workItem);
        revision.AddSection(section);

        Calculate(revision, new EstimateDiscount(EstimateDiscount.Percent, 0.10m, "TEST_ONLY_DISCOUNT"));

        Assert.Equal(100m, revision.DiscountAmount);
        Assert.Equal(900m, revision.NetBeforeTax);
        Assert.Contains("\"discountType\":\"percent\"", revision.CalculationSnapshotJson, StringComparison.Ordinal);
        Assert.Contains("\"discountReasonCode\":\"TEST_ONLY_DISCOUNT\"", revision.CalculationSnapshotJson, StringComparison.Ordinal);
    }

    [Fact]
    public void EstimateDiscount_RejectsRateOverOneAndMissingReason()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EstimateDiscount(EstimateDiscount.Percent, 1.01m, "TEST_ONLY").ResolveAmount(100m, MidpointRounding.AwayFromZero));
        Assert.Throws<EstimateDiscountReasonRequiredException>(() => new EstimateDiscount(EstimateDiscount.FixedAmount, 1m, null).ResolveAmount(100m, MidpointRounding.AwayFromZero));
    }

    [Fact]
    public void Calculate_RejectsDiscountAboveSellingPriceWithoutChangingSnapshotTotals()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-01", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(
            Guid.NewGuid(), _orgId, section.Id, "WI-01", "งานทดสอบ", null,
            1m, "unit", SellingRuleType.FixedPrice, 100m);
        section.AddWorkItem(workItem);
        revision.AddSection(section);
        Calculate(revision);
        var priorGrandTotal = revision.GrandTotal;
        var priorSnapshot = revision.CalculationSnapshotJson;

        Assert.Throws<ArgumentOutOfRangeException>(() => Calculate(revision, 100.01m));

        Assert.Equal(priorGrandTotal, revision.GrandTotal);
        Assert.Equal(priorSnapshot, revision.CalculationSnapshotJson);
    }

    [Fact]
    public void MarkCalculationOutdated_PreservesLastSnapshotUntilRecalculated()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        Calculate(revision);
        var latestSnapshot = revision.CalculationSnapshotJson;

        revision.MarkCalculationOutdated();

        Assert.True(revision.CalculationOutdated);
        Assert.Equal(latestSnapshot, revision.CalculationSnapshotJson);
        Calculate(revision);
        Assert.False(revision.CalculationOutdated);
    }

    [Fact]
    public void Submit_RequiresCurrentCalculationSnapshot()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        revision.MarkFinancialInputChanged(_makerId);

        Assert.Throws<EstimateInvalidStateException>(() => revision.Submit(_makerId, DateTimeOffset.UtcNow));

        Calculate(revision);
        revision.Submit(_makerId, DateTimeOffset.UtcNow);
        Assert.Equal(EstimateRevisionStatus.Submitted, revision.Status);
    }

    [Fact]
    public void Approve_RejectsMakerAndAcceptsIndependentChecker()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        revision.MarkFinancialInputChanged(_makerId);
        Calculate(revision);
        revision.Submit(_makerId, DateTimeOffset.UtcNow);

        Assert.Throws<EstimateInvalidStateException>(() => revision.Approve(_makerId, DateTimeOffset.UtcNow, "{}"));

        revision.Approve(_checkerId, DateTimeOffset.UtcNow, "{}");
        Assert.Equal(EstimateRevisionStatus.Approved, revision.Status);
    }

    [Fact]
    public void Calculate_WithFixedOverhead_DistributesRoundingRemainderAndPricesOnLoadedCost()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-OH", "งานทดสอบ", null);
        for (var index = 0; index < 3; index++)
        {
            var workItem = new EstimateWorkItem(
                Guid.NewGuid(), _orgId, section.Id, $"WI-{index + 1}", "งานทดสอบ", null,
                1m, "unit", SellingRuleType.Margin, 0.50m, index + 1);
            workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
            workItem.AddCostComponent(new EstimateCostComponent(
                Guid.NewGuid(), _orgId, workItem.Id, CostComponentType.Material,
                "ต้นทุนทดสอบ", 1m, "unit", 1m));
            section.AddWorkItem(workItem);
        }
        revision.AddSection(section);

        Calculate(revision, overheadMethod: "fixed-amount", overheadValue: 0.05m);

        Assert.Equal(3.05m, revision.NetCost);
        Assert.Equal(6.10m, revision.SellingBeforeDiscount);
        Assert.Contains("\"overheadAmount\":0.05", revision.CalculationSnapshotJson, StringComparison.Ordinal);
        Assert.Equal(new[] { 1.02m, 1.02m, 1.01m }, section.WorkItems.OrderBy(item => item.SortOrder).Select(item => item.TotalCost));
    }

    [Theory]
    [InlineData("margin", 0.25, 133.33)]
    [InlineData("markup", 0.25, 125.00)]
    public void Calculate_UsesDistinctMarginAndMarkupFormulas(string pricingType, double rate, double expectedSelling)
    {
        var revision = CreateRevisionWithSingleCostWorkItem(100m, pricingType, (decimal)rate);

        Calculate(revision, taxMode: "exempt");

        Assert.Equal((decimal)expectedSelling, revision.SellingBeforeDiscount);
        Assert.Equal(0m, revision.TaxAmount);
        Assert.Equal((decimal)expectedSelling, revision.GrandTotal);
    }

    [Theory]
    [InlineData("exclusive", 107d, 107d, 7.49d, 114.49d)]
    [InlineData("inclusive", 107d, 100d, 7d, 107d)]
    [InlineData("exempt", 107d, 107d, 0d, 107d)]
    public void Calculate_AppliesTaxModeToDiscountedSellingTotal(
        string taxMode,
        double selling,
        double expectedNetBeforeTax,
        double expectedTax,
        double expectedGrandTotal)
    {
        var revision = CreateRevisionWithSingleCostWorkItem(50m, SellingRuleType.FixedPrice, (decimal)selling);

        Calculate(revision, taxMode: taxMode);

        Assert.Equal((decimal)expectedNetBeforeTax, revision.NetBeforeTax);
        Assert.Equal((decimal)expectedTax, revision.TaxAmount);
        Assert.Equal((decimal)expectedGrandTotal, revision.GrandTotal);
    }

    [Fact]
    public void Calculate_WithPercentOverheadAllocatesAgainstDirectCost()
    {
        var revision = new EstimateRevision(Guid.NewGuid(), _orgId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), _orgId, revision.Id, "SEC-PCT-OH", "งานทดสอบ", null);
        var first = CreateWorkItem(_orgId, section.Id, "WI-01", 10m);
        var second = CreateWorkItem(_orgId, section.Id, "WI-02", 20m);
        section.AddWorkItem(first);
        section.AddWorkItem(second);
        revision.AddSection(section);

        Calculate(revision, overheadMethod: "percent-direct-cost", overheadValue: 0.10m, taxMode: "exempt");

        Assert.Equal(33m, revision.NetCost);
        Assert.Equal(11m, first.TotalCost);
        Assert.Equal(22m, second.TotalCost);
        Assert.Contains("\"overheadAmount\":3", revision.CalculationSnapshotJson, StringComparison.Ordinal);
    }

    [Fact]
    public void FinancialEdit_BlocksReadinessAndPreservesSnapshotUntilRecalculated()
    {
        var revision = CreateRevisionWithSingleCostWorkItem(100m, SellingRuleType.Margin, 0.25m);
        Calculate(revision, taxMode: "exempt");
        var oldSnapshot = revision.CalculationSnapshotJson;
        var oldVersion = revision.CalculationVersion;

        revision.MarkFinancialInputChanged(_makerId);

        Assert.True(revision.CalculationOutdated);
        Assert.Equal(oldSnapshot, revision.CalculationSnapshotJson);
        Assert.Equal(EstimateReadinessStatus.Blocked, revision.EvaluateReadiness().Status);
        Assert.Contains(revision.EvaluateReadiness().Reasons, reason => reason.Code == "ESTIMATE_CALCULATION_OUTDATED");

        Calculate(revision, taxMode: "exempt");

        Assert.False(revision.CalculationOutdated);
        Assert.Equal(oldVersion + 1, revision.CalculationVersion);
        Assert.Equal(EstimateReadinessStatus.RequiresAttention, revision.EvaluateReadiness().Status);
    }

    [Fact]
    public void HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies()
    {
        var calculatedAt = DateTimeOffset.UtcNow;
        var policyCutover = calculatedAt.AddHours(1);
        var original = CreateRevisionWithSingleCostWorkItem(100m, SellingRuleType.Margin, 0.20m);
        foreach (var component in original.Sections.SelectMany(section => section.WorkItems)
                     .SelectMany(workItem => workItem.CostComponents))
            component.SetProvisionalReason("TEST_ONLY", "Synthetic historical reproduction fixture");

        var originalCalculationPolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), original.OrganizationId, null, "TEST_ONLY_CALC", 1,
            "none", 0m, "away-from-zero", calculatedAt.AddMinutes(-5), policyCutover, _makerId);
        originalCalculationPolicy.Publish(_checkerId, calculatedAt);
        var originalTaxPolicy = new TaxPolicyVersion(
            Guid.NewGuid(), original.OrganizationId, null, "TEST_ONLY_TAX", 1,
            "exclusive", 0.07m, "TEST_ONLY_TAX", calculatedAt.AddMinutes(-5), policyCutover, _makerId);
        originalTaxPolicy.Publish(_checkerId, calculatedAt);
        var noDiscount = new EstimateDiscount(EstimateDiscount.None, 0m, null);

        original.Calculate(noDiscount, originalCalculationPolicy, originalTaxPolicy, calculatedAt);
        var frozenSnapshot = original.CalculationSnapshotJson;
        var historicalGrandTotal = original.GrandTotal;
        var historicalInputCost = Assert.Single(Assert.Single(original.Sections).WorkItems).TotalCost;
        original.Submit(_makerId, calculatedAt);
        original.Approve(_checkerId, calculatedAt, "{\"approved\":true}");

        var successorCalculationPolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), original.OrganizationId, null, "TEST_ONLY_CALC", 2,
            "fixed-amount", 50m, "away-from-zero", policyCutover, null, _makerId);
        successorCalculationPolicy.Publish(_checkerId, calculatedAt);
        var successorTaxPolicy = new TaxPolicyVersion(
            Guid.NewGuid(), original.OrganizationId, null, "TEST_ONLY_TAX", 2,
            "exclusive", 0.15m, "TEST_ONLY_TAX", policyCutover, null, _makerId);
        successorTaxPolicy.Publish(_checkerId, calculatedAt);

        var nextRevision = EstimateRevision.CloneAsDraft(Guid.NewGuid(), 2, original);
        nextRevision.Calculate(noDiscount, successorCalculationPolicy, successorTaxPolicy, policyCutover.AddMinutes(1));
        Assert.Equal("TEST_ONLY_CALC-v2", nextRevision.CalculationPolicyVersion);
        Assert.Equal("TEST_ONLY_TAX-v2", nextRevision.TaxPolicyVersion);
        Assert.NotEqual(historicalGrandTotal, nextRevision.GrandTotal);

        var replay = EstimateRevision.CloneAsDraft(Guid.NewGuid(), 3, original);
        replay.Calculate(noDiscount, originalCalculationPolicy, originalTaxPolicy, calculatedAt);

        Assert.Equal(historicalInputCost, Assert.Single(Assert.Single(replay.Sections).WorkItems).TotalCost);
        Assert.Equal(original.NetCost, replay.NetCost);
        Assert.Equal(original.SellingBeforeDiscount, replay.SellingBeforeDiscount);
        Assert.Equal(original.TaxAmount, replay.TaxAmount);
        Assert.Equal(historicalGrandTotal, replay.GrandTotal);
        Assert.Equal("TEST_ONLY_CALC-v1", replay.CalculationPolicyVersion);
        Assert.Equal("TEST_ONLY_TAX-v1", replay.TaxPolicyVersion);
        Assert.Equal(frozenSnapshot, original.CalculationSnapshotJson);
        Assert.Equal(EstimateRevisionStatus.Approved, original.Status);
    }

    [Fact]
    public void WorkItem_RejectsMarginRateAtOrAboveOne()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EstimateWorkItem(
            Guid.NewGuid(), _orgId, Guid.NewGuid(), "WI-01", "งานทดสอบ", null,
            1m, "unit", SellingRuleType.Margin, 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EstimateWorkItem(
            Guid.NewGuid(), _orgId, Guid.NewGuid(), "WI-02", "งานทดสอบ", null,
            1m, "unit", SellingRuleType.Markup, -0.01m));
    }

    [Fact]
    public void MarkQuoted_TransitionsEstimateAndRevisionToQuoted()
    {
        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-2026-0002");
        var approvedRevision = new EstimateRevision(Guid.NewGuid(), _orgId, estimate.Id, 2);
        approvedRevision.MarkFinancialInputChanged(_makerId);
        Calculate(approvedRevision);
        approvedRevision.Submit(_makerId, DateTimeOffset.UtcNow);
        approvedRevision.Approve(_checkerId, DateTimeOffset.UtcNow, "{\"approval\":true}");
        estimate.AddRevision(approvedRevision);

        var oldEstimateVersion = estimate.RowVersion;
        var oldRevVersion = estimate.CurrentRevision!.RowVersion;

        estimate.MarkQuoted();

        Assert.Equal(EstimateStatus.Quoted, estimate.Status);
        Assert.Equal(EstimateRevisionStatus.Quoted, estimate.CurrentRevision.Status);
        Assert.NotEqual(oldEstimateVersion, estimate.RowVersion);
        Assert.NotEqual(oldRevVersion, estimate.CurrentRevision.RowVersion);
    }

    [Fact]
    public void MarkQuoted_RejectsDraftRevisionWithoutChangingEstimate()
    {
        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-2026-0003");
        var oldEstimateVersion = estimate.RowVersion;
        var oldRevisionVersion = estimate.CurrentRevision!.RowVersion;

        Assert.Throws<EstimateInvalidStateException>(() => estimate.MarkQuoted());

        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision.Status);
        Assert.Equal(oldEstimateVersion, estimate.RowVersion);
        Assert.Equal(oldRevisionVersion, estimate.CurrentRevision.RowVersion);
    }

    [Fact]
    public void CreateNextDraftRevision_ClonesBoqWithNewIdsAndInvalidatesCalculation()
    {
        var estimate = Estimate.CreateDraft(Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-REV");
        var original = estimate.CurrentRevision!;
        var section = new EstimateSection(Guid.NewGuid(), _orgId, original.Id, "SEC-01", "งานทดสอบ", "Test work");
        var workItem = new EstimateWorkItem(Guid.NewGuid(), _orgId, section.Id, "WI-01", "วัสดุ", "Material", 2m, "unit", SellingRuleType.Margin, 0.2m);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), _orgId, workItem.Id, CostComponentType.Material, "TEST_ONLY", 3m, "unit", 100m));
        section.AddWorkItem(workItem);
        original.AddSection(section);
        original.MarkFinancialInputChanged(_makerId);
        Calculate(original);
        original.Submit(_makerId, DateTimeOffset.UtcNow);
        original.Approve(_checkerId, DateTimeOffset.UtcNow, "{\"approval\":true}");

        var next = estimate.CreateNextDraftRevision("TEST_ONLY change request");

        Assert.Equal(2, next.RevisionNo);
        Assert.Equal(EstimateRevisionStatus.Draft, next.Status);
        Assert.True(next.CalculationOutdated);
        Assert.Null(next.CalculationSnapshotJson);
        var clonedSection = Assert.Single(next.Sections);
        var clonedWorkItem = Assert.Single(clonedSection.WorkItems);
        var clonedCost = Assert.Single(clonedWorkItem.CostComponents);
        Assert.NotEqual(section.Id, clonedSection.Id);
        Assert.NotEqual(workItem.Id, clonedWorkItem.Id);
        Assert.NotEqual(workItem.CostComponents.Single().Id, clonedCost.Id);
        Assert.Equal(300m, clonedCost.TotalCost);
        Assert.Equal(EstimateRevisionStatus.Approved, original.Status);
        Assert.NotNull(original.ApprovalSnapshotJson);
    }

    [Fact]
    public void CancelCurrentRevision_CancelsDraftAndRejectsApprovedRevision()
    {
        var estimate = Estimate.CreateDraft(Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-CANCEL");
        estimate.CancelCurrentRevision(DateTimeOffset.UtcNow, "TEST_ONLY cancellation reason");

        Assert.Equal(EstimateStatus.Cancelled, estimate.Status);
        Assert.Equal(EstimateRevisionStatus.Cancelled, estimate.CurrentRevision!.Status);

        var approvedEstimate = Estimate.CreateDraft(Guid.NewGuid(), _orgId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "EST-APPROVED-CANCEL");
        var approved = approvedEstimate.CurrentRevision!;
        approved.MarkFinancialInputChanged(_makerId);
        Calculate(approved);
        approved.Submit(_makerId, DateTimeOffset.UtcNow);
        approved.Approve(_checkerId, DateTimeOffset.UtcNow, "{\"approval\":true}");
        Assert.Throws<EstimateInvalidStateException>(() => approvedEstimate.CancelCurrentRevision(DateTimeOffset.UtcNow, "TEST_ONLY prohibited"));
        Assert.Equal(EstimateRevisionStatus.Approved, approvedEstimate.CurrentRevision!.Status);
    }

    private static void Calculate(
        EstimateRevision revision,
        decimal discountAmount = 0m,
        string overheadMethod = "none",
        decimal overheadValue = 0m,
        string taxMode = "exclusive")
        => Calculate(revision, new EstimateDiscount(EstimateDiscount.FixedAmount, discountAmount,
            discountAmount > 0m ? "TEST_ONLY_DISCOUNT" : null), overheadMethod, overheadValue, taxMode);

    private static void Calculate(
        EstimateRevision revision,
        EstimateDiscount discount,
        string overheadMethod = "none",
        decimal overheadValue = 0m,
        string taxMode = "exclusive")
    {
        foreach (var component in revision.Sections.SelectMany(section => section.WorkItems).SelectMany(item => item.CostComponents))
            component.SetProvisionalReason("TEST_ONLY", "Synthetic estimate fixture");

        var now = DateTimeOffset.UtcNow;
        var calculationPolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), revision.OrganizationId, null, "TEST_ONLY_CALC", 1,
            overheadMethod, overheadValue, "away-from-zero", now.AddDays(-1), null, Guid.NewGuid());
        calculationPolicy.Publish(Guid.NewGuid(), now);
        var taxPolicy = new TaxPolicyVersion(
            Guid.NewGuid(), revision.OrganizationId, null, "TEST_ONLY_TAX", 1,
            taxMode, taxMode == "exempt" ? 0m : 0.07m, "TEST_ONLY_TAX", now.AddDays(-1), null, Guid.NewGuid());
        taxPolicy.Publish(Guid.NewGuid(), now);
        revision.Calculate(discount, calculationPolicy, taxPolicy, now);
    }

    private static EstimateRevision CreateRevisionWithSingleCostWorkItem(decimal cost, string pricingType, decimal pricingValue)
    {
        var organizationId = Guid.NewGuid();
        var revision = new EstimateRevision(Guid.NewGuid(), organizationId, Guid.NewGuid(), 1);
        var section = new EstimateSection(Guid.NewGuid(), revision.OrganizationId, revision.Id, "SEC-TEST", "งานทดสอบ", null);
        var workItem = new EstimateWorkItem(Guid.NewGuid(), revision.OrganizationId, section.Id,
            "WI-TEST", "รายการทดสอบ", null, 1m, "unit", pricingType, pricingValue, 1,
            pricingType == SellingRuleType.FixedPrice ? "TEST_ONLY_FIXED_PRICE" : null);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), revision.OrganizationId, workItem.Id,
            CostComponentType.Material, "TEST_ONLY cost", 1m, "unit", cost));
        section.AddWorkItem(workItem);
        revision.AddSection(section);
        return revision;
    }

    private static EstimateWorkItem CreateWorkItem(Guid organizationId, Guid sectionId, string code, decimal cost)
    {
        var workItem = new EstimateWorkItem(Guid.NewGuid(), organizationId, sectionId,
            code, "รายการทดสอบ", null, 1m, "unit", SellingRuleType.Markup, 0m);
        workItem.SetCustomWorkItemReason("TEST_ONLY", "Unit test work item");
        workItem.AddCostComponent(new EstimateCostComponent(Guid.NewGuid(), organizationId, workItem.Id,
            CostComponentType.Material, "TEST_ONLY cost", 1m, "unit", cost));
        return workItem;
    }
}
