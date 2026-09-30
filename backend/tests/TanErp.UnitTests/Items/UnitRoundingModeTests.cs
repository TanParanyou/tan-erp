using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public sealed class UnitRoundingModeTests
{
    [Theory]
    [InlineData(2.345, 2, "half_up", 2.35)]
    [InlineData(-2.345, 2, "half_up", -2.35)]
    [InlineData(2.345, 2, "half_even", 2.34)]
    [InlineData(2.355, 2, "half_even", 2.36)]
    [InlineData(1.201, 2, "up", 1.21)]
    [InlineData(-1.201, 2, "up", -1.21)]
    [InlineData(1.209, 2, "down", 1.20)]
    [InlineData(-1.209, 2, "down", -1.20)]
    [InlineData(1.201, 2, "ceiling", 1.21)]
    [InlineData(-1.201, 2, "ceiling", -1.20)]
    [InlineData(1.209, 2, "floor", 1.20)]
    [InlineData(-1.209, 2, "floor", -1.21)]
    public void Round_UsesSelectedPolicyAndDecimalScale(double input, int scale, string mode, double expected)
    {
        var result = UnitRoundingMode.Round((decimal)input, scale, mode);

        Assert.Equal((decimal)expected, result);
    }

    [Theory]
    [InlineData("HalfUp", "half_up")]
    [InlineData("half-even", "half_even")]
    [InlineData("FLOOR", "floor")]
    public void TryNormalize_AcceptsKnownLegacyCasing(string input, string expected)
    {
        Assert.True(UnitRoundingMode.TryNormalize(input, out var normalized));
        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void TryNormalize_RejectsUnknownPolicy()
    {
        Assert.False(UnitRoundingMode.TryNormalize("roundish", out _));
    }
}
