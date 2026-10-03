using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public class CostPublicationPolicyTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _itemId = Guid.NewGuid();
    private readonly Guid _unitId = Guid.NewGuid();
    private readonly Guid _makerId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void PartiallyOverlappingDateAndQuantityRangesConflict()
    {
        var existing = Record(0m, 10m, _now.AddDays(-5), _now.AddDays(5));
        var candidate = Record(5m, 20m, _now, _now.AddDays(10));

        Assert.True(CostPublicationPolicy.Overlaps(candidate, existing));
        Assert.False(CostPublicationPolicy.CanSupersede(candidate, existing));
    }

    [Fact]
    public void AdjacentQuantityBandsCanCoexist()
    {
        var existing = Record(1m, 9m, _now, null);
        var candidate = Record(10m, null, _now, null);

        Assert.False(CostPublicationPolicy.Overlaps(candidate, existing));
    }

    [Fact]
    public void ShorterReplacementCannotLeaveGapAfterItsEnd()
    {
        var existing = Record(1m, null, _now.AddDays(-5), null);
        var candidate = Record(1m, null, _now.AddDays(1), _now.AddDays(3));

        Assert.True(CostPublicationPolicy.Overlaps(candidate, existing));
        Assert.False(CostPublicationPolicy.CanSupersede(candidate, existing));
    }

    private CostRecord Record(decimal minimum, decimal? maximum, DateTimeOffset effectiveFrom, DateTimeOffset? effectiveTo) =>
        CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            100m, minimum, maximum, effectiveFrom, effectiveTo, 1, null, null, null, null, _makerId, _now);
}
