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
}
