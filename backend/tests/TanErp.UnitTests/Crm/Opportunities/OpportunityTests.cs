using TanErp.Domain.Crm.Opportunities;
using Xunit;

namespace TanErp.UnitTests.Crm.Opportunities;

public class OpportunityTests
{
    [Fact]
    public void CreateDraft_WhenValid_SetsPropertiesAndGeneratesCode()
    {
        var id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d10");
        var orgId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var ownerUserId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var nextActionUtc = now.AddDays(2);
        var targetDate = new DateOnly(2026, 10, 15);

        var opp = Opportunity.CreateDraft(
            id,
            orgId,
            branchId,
            customerId,
            siteId,
            ownerUserId,
            actorId,
            "  Built-in ห้องนอนใหญ่ TEST_ONLY  ",
            "  สำรวจและประเมินตู้เสื้อผ้า  ",
            new[] { "built-in", "built-in" },
            null,
            250000.00m,
            "thb",
            targetDate,
            nextActionUtc,
            "  นัดหมายล่วงหน้า  ",
            now);

        Assert.Equal(id, opp.Id);
        Assert.Equal(orgId, opp.OrganizationId);
        Assert.Equal(branchId, opp.BranchId);
        Assert.Equal(customerId, opp.CustomerId);
        Assert.Equal(siteId, opp.PrimarySiteId);
        Assert.Equal(ownerUserId, opp.OwnerUserId);
        Assert.Equal("OPP-019A3CF896F0", opp.Code);
        Assert.Equal("Built-in ห้องนอนใหญ่ TEST_ONLY", opp.Title);
        Assert.Equal("built-in ห้องนอนใหญ่ test_only", opp.NormalizedTitle);
        Assert.Equal("สำรวจและประเมินตู้เสื้อผ้า", opp.ScopeSummary);
        Assert.Equal("draft", opp.Stage);
        Assert.Single(opp.WorkTypes);
        Assert.Equal("built-in", opp.WorkTypes.First());
        Assert.Equal(250000.00m, opp.ExpectedBudget);
        Assert.Equal("THB", opp.CurrencyCode);
        Assert.Equal(targetDate, opp.TargetDecisionDate);
        Assert.Equal(nextActionUtc, opp.NextActionAtUtc);
        Assert.Equal("นัดหมายล่วงหน้า", opp.NextActionNote);
        Assert.NotEqual(Guid.Empty, opp.RowVersion);
        Assert.Equal(now, opp.CreatedAtUtc);
    }

    [Fact]
    public void CreateDraft_WhenBlankTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Opportunity.CreateDraft(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, Guid.NewGuid(), Guid.NewGuid(),
            "   ", null, new[] { "built-in" }, null, null, null, null, null, null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateDraft_WhenInvalidWorkType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Opportunity.CreateDraft(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, Guid.NewGuid(), Guid.NewGuid(),
            "งานบิวต์อิน", null, new[] { "invalid-type" }, null, null, null, null, null, null, DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CreateDraft_WhenNonPositiveBudget_ThrowsArgumentOutOfRangeException(decimal budget)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Opportunity.CreateDraft(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, Guid.NewGuid(), Guid.NewGuid(),
            "งานบิวต์อิน", null, new[] { "built-in" }, null, budget, "THB", null, null, null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Qualify_FromDraftWithCompleteGate_SetsQualifiedAndRotatesVersion()
    {
        var id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d10");
        var now = DateTimeOffset.UtcNow;
        var opp = Opportunity.CreateDraft(
            id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(),
            "งานห้องนอน TEST_ONLY",
            "ออกแบบตกแต่งภายใน",
            new[] { "built-in" },
            null, 150000m, "THB",
            new DateOnly(2026, 12, 1),
            now.AddDays(1),
            "ติดตามสรุปแบบ",
            now);

        var initialVersion = opp.RowVersion;

        opp.Qualify(initialVersion);

        Assert.Equal(OpportunityStage.Qualified, opp.Stage);
        Assert.NotEqual(initialVersion, opp.RowVersion);
        Assert.Equal("งานห้องนอน TEST_ONLY", opp.Title);
        Assert.Equal("ออกแบบตกแต่งภายใน", opp.ScopeSummary);
        Assert.Equal("ติดตามสรุปแบบ", opp.NextActionNote);

        var expectedCanonicalStages = new[]
        {
            "draft", "qualified", "surveying", "estimating", "proposed", "won", "lost", "cancelled"
        };
        Assert.Equal(expectedCanonicalStages.OrderBy(s => s), OpportunityStage.All.OrderBy(s => s));
    }

    private static Opportunity CreateValidDraft()
    {
        var now = DateTimeOffset.UtcNow;
        return Opportunity.CreateDraft(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(),
            "งานห้องนอน TEST_ONLY",
            "ออกแบบตกแต่งภายใน",
            new[] { "built-in" },
            null, 150000m, "THB",
            new DateOnly(2026, 12, 1),
            now.AddDays(1),
            "ติดตามสรุปแบบ",
            now);
    }

    [Fact]
    public void CloseLost_ValidReason_ChangesStageAndRotatesVersion()
    {
        var opp = CreateValidDraft();
        var initialVersion = opp.RowVersion;

        opp.Close(initialVersion, OpportunityStage.Lost, OpportunityReasonCodes.LostPriceTooHigh, "ราคาสูงเกินงบ");

        Assert.Equal(OpportunityStage.Lost, opp.Stage);
        Assert.NotEqual(initialVersion, opp.RowVersion);
    }

    [Fact]
    public void CloseCancelled_ValidReason_ChangesStageAndRotatesVersion()
    {
        var opp = CreateValidDraft();
        var initialVersion = opp.RowVersion;

        opp.Close(initialVersion, OpportunityStage.Cancelled, OpportunityReasonCodes.CancelledCustomerAbandoned);

        Assert.Equal(OpportunityStage.Cancelled, opp.Stage);
        Assert.NotEqual(initialVersion, opp.RowVersion);
    }

    [Fact]
    public void Reopen_ApprovedTarget_ChangesStageAndRotatesVersion()
    {
        var opp = CreateValidDraft();
        opp.Close(opp.RowVersion, OpportunityStage.Lost, OpportunityReasonCodes.LostPriceTooHigh);

        var closedVersion = opp.RowVersion;
        opp.Reopen(closedVersion, OpportunityStage.Draft, OpportunityReasonCodes.ReopenBudgetAdjusted);

        Assert.Equal(OpportunityStage.Draft, opp.Stage);
        Assert.NotEqual(closedVersion, opp.RowVersion);
    }

    [Fact]
    public void EnterProposed_FromEstimating_SetsProposedAndRotatesVersion()
    {
        var opp = CreateValidDraft();
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, Guid.NewGuid());
        opp.EnterEstimating(opp.RowVersion);

        var estimatingVersion = opp.RowVersion;
        opp.EnterProposed(estimatingVersion);

        Assert.Equal(OpportunityStage.Proposed, opp.Stage);
        Assert.NotEqual(estimatingVersion, opp.RowVersion);
    }

    [Fact]
    public void MarkWon_FromProposed_SetsWonAndRotatesVersion()
    {
        var opp = CreateValidDraft();
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, Guid.NewGuid());
        opp.EnterEstimating(opp.RowVersion);
        opp.EnterProposed(opp.RowVersion);

        var proposedVersion = opp.RowVersion;
        opp.MarkWon(proposedVersion);

        Assert.Equal(OpportunityStage.Won, opp.Stage);
        Assert.NotEqual(proposedVersion, opp.RowVersion);
    }
}
