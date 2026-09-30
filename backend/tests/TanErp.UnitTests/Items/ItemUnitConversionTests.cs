using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public sealed class ItemUnitConversionTests
{
    private static ItemUnitConversion Create(decimal factor = 12m, Guid? from = null, Guid? to = null,
        DateOnly? start = null, DateOnly? end = null) => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), from ?? Guid.NewGuid(), to ?? Guid.NewGuid(),
        factor, start ?? new DateOnly(2026, 1, 1), end, "Package definition", Guid.NewGuid(), DateTimeOffset.UtcNow);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000000000000)]
    [InlineData(1.0000001)]
    public void Factor_MustFitPositiveNumericSixScale(decimal factor)
    {
        Assert.Equal("ITEM_UNIT_CONVERSION_FACTOR_INVALID", Assert.Throws<ItemValidationException>(() => Create(factor)).Code);
    }

    [Fact]
    public void SourceAndTarget_MustBeDifferent()
    {
        var id = Guid.NewGuid();
        Assert.Equal("ITEM_UNIT_CONVERSION_SELF_LOOP", Assert.Throws<ItemValidationException>(() => Create(from: id, to: id)).Code);
    }

    [Fact]
    public void EffectivePeriod_IsInclusiveAndMustBeOrdered()
    {
        var conversion = Create(start: new DateOnly(2026, 9, 1), end: new DateOnly(2026, 9, 30));
        Assert.True(conversion.IsEffectiveOn(new DateOnly(2026, 9, 1)));
        Assert.True(conversion.IsEffectiveOn(new DateOnly(2026, 9, 30)));
        Assert.False(conversion.IsEffectiveOn(new DateOnly(2026, 10, 1)));
        Assert.Equal("ITEM_UNIT_CONVERSION_PERIOD_INVALID", Assert.Throws<ItemValidationException>(() =>
            Create(start: new DateOnly(2026, 9, 30), end: new DateOnly(2026, 9, 1))).Code);
    }
}
