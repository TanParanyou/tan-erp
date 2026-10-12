using TanErp.Application.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class MembershipExpiryReminderPlannerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 12, 8, 0, 0, TimeSpan.Zero);
    private static readonly IReadOnlyList<int> Default = MembershipExpiryReminderPlanner.NormalizeThresholds(null);

    [Fact]
    public void Defaults_AreThirtySevenAndOneDays_Ascending()
    {
        Assert.Equal([1, 7, 30], Default);
    }

    [Theory]
    [InlineData(new[] { 30, 7, 1, 7 }, new[] { 1, 7, 30 })]
    [InlineData(new[] { 0, -3, 366, 14 }, new[] { 14 })]
    [InlineData(new int[0], new[] { 1, 7, 30 })]
    [InlineData(new[] { 0, 999 }, new[] { 1, 7, 30 })]
    public void NormalizeThresholds_KeepsValidDistinctValuesAscending_OrFallsBackToTheDefaults(int[] configured, int[] expected)
    {
        Assert.Equal(expected, MembershipExpiryReminderPlanner.NormalizeThresholds(configured));
    }

    [Theory]
    [InlineData(45 * 24, null)]       // beyond the largest threshold
    [InlineData(30 * 24, 30)]         // exactly on the boundary belongs to that threshold
    [InlineData(20 * 24, 30)]
    [InlineData(7 * 24, 7)]
    [InlineData(6 * 24, 7)]
    [InlineData(24, 1)]
    [InlineData(12, 1)]
    [InlineData(0, null)]             // already expired: never reminded
    [InlineData(-5, null)]
    public void SelectThreshold_PicksTheSmallestThresholdTheRemainingTimeFitsIn(int hoursRemaining, int? expected)
    {
        Assert.Equal(expected, MembershipExpiryReminderPlanner.SelectThreshold(Now, Now.AddHours(hoursRemaining), Default));
    }

    [Fact]
    public void TransitionId_IsDeterministic_AndChangesWithEachInput()
    {
        var membership = Guid.NewGuid();
        var expiry = Now.AddDays(10);
        var id = MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry));
        Assert.Equal(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry.ToOffset(TimeSpan.FromHours(7))));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(membership, 7, expiry));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry.AddDays(1)));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(Guid.NewGuid(), 30, expiry));
    }

    [Fact]
    public void Options_ClampOutOfRangeValues()
    {
        var options = new MembershipExpiryReminderOptions { IntervalMinutes = 0, StartupDelaySeconds = -5, MaxMembershipsPerRun = 99999 };
        Assert.Equal(TimeSpan.FromMinutes(5), options.EffectiveInterval);
        Assert.Equal(TimeSpan.Zero, options.EffectiveStartupDelay);
        Assert.Equal(1000, options.EffectiveMaxMemberships);
        Assert.True(new MembershipExpiryReminderOptions().Enabled);
    }
}
