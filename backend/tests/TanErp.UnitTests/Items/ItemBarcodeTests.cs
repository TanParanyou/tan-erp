using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public class ItemBarcodeTests
{
    private static ItemBarcode Create(string identifierType = "gtin", string value = "6291041500213", decimal quantity = 1m) => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), identifierType, value,
        Guid.NewGuid(), quantity, "each", true, Guid.NewGuid(), DateTimeOffset.UtcNow);

    [Fact]
    public void Gtin_PreservesLeadingZerosAndValidatesCheckDigit()
    {
        var barcode = Create(value: "06291041500213");
        Assert.Equal("06291041500213", barcode.Value);
        Assert.Equal("06291041500213", barcode.NormalizedValue);
        Assert.Equal(barcode.NormalizedValue, Create(value: "6291041500213").NormalizedValue);
        Assert.Equal("ITEM_BARCODE_INVALID", Assert.Throws<ItemValidationException>(() => Create(value: "6291041500214")).Code);
    }

    [Fact]
    public void Gtin_AcceptsValidEightTwelveAndThirteenDigitValues()
    {
        Assert.Equal("96385074", Create(value: "96385074").Value);
        Assert.Equal("036000291452", Create(value: "036000291452").Value);
        Assert.Equal("6291041500213", Create(value: "6291041500213").Value);
    }

    [Fact]
    public void InternalBarcode_NormalizesCaseWithoutChangingDisplayedValue()
    {
        var barcode = Create("internal", "  Bin-A_01  ");
        Assert.Equal("Bin-A_01", barcode.Value);
        Assert.Equal("BIN-A_01", barcode.NormalizedValue);
        Assert.Equal("ITEM_BARCODE_INVALID", Assert.Throws<ItemValidationException>(() => Create("internal", "bad value")).Code);
    }

    [Fact]
    public void Quantity_MustBePositiveAndHaveAtMostFourDecimals()
    {
        Assert.Equal("ITEM_BARCODE_INVALID", Assert.Throws<ItemValidationException>(() => Create(quantity: 0m)).Code);
        Assert.Equal("ITEM_BARCODE_INVALID", Assert.Throws<ItemValidationException>(() => Create(quantity: 1.00001m)).Code);
    }

    [Fact]
    public void PackagingLevel_MustBeKnown()
    {
        Assert.Equal("ITEM_BARCODE_INVALID", Assert.Throws<ItemValidationException>(() => new ItemBarcode(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "internal", "UNIT-1", Guid.NewGuid(), 1m,
            "unknown", false, Guid.NewGuid(), DateTimeOffset.UtcNow)).Code);
    }

    [Fact]
    public void Deactivate_PreservesIdentityAndRejectsPrimaryTransition()
    {
        var barcode = Create();
        var value = barcode.Value;
        barcode.Deactivate(Guid.NewGuid(), DateTimeOffset.UtcNow);
        Assert.Equal(ItemStatus.Inactive, barcode.Status);
        Assert.False(barcode.IsPrimary);
        Assert.Equal(value, barcode.Value);
        Assert.Equal("ITEM_BARCODE_INACTIVE", Assert.Throws<ItemValidationException>(() => barcode.SetPrimary(Guid.NewGuid(), DateTimeOffset.UtcNow)).Code);
    }
}
