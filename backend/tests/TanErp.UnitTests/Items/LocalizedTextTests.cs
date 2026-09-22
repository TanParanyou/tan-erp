using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public class LocalizedTextTests
{
    [Fact]
    public void Create_WithValidThaiAndEnglish_TrimsAndStoresCorrectly()
    {
        var text = LocalizedText.Create("  ไม้อัด 18 มม.  ", "  18mm Plywood  ");

        Assert.Equal("ไม้อัด 18 มม.", text.Thai);
        Assert.Equal("18mm Plywood", text.English);
    }

    [Fact]
    public void Create_WithValidThaiAndNullEnglish_AllowsNullEnglish()
    {
        var text = LocalizedText.Create("ไม้สัก", null);

        Assert.Equal("ไม้สัก", text.Thai);
        Assert.Null(text.English);
    }

    [Fact]
    public void Create_WithWhitespaceOnlyEnglish_NormalizesToNull()
    {
        var text = LocalizedText.Create("ไม้สัก", "   ");

        Assert.Equal("ไม้สัก", text.Thai);
        Assert.Null(text.English);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrBlankThai_ThrowsItemValidationException(string? thai)
    {
        var ex = Assert.Throws<ItemValidationException>(() => LocalizedText.Create(thai!, "Wood"));
        Assert.Equal("ITEM_FIELD_REQUIRED", ex.Code);
    }

    [Fact]
    public void Create_WhenThaiExceedsMaxLength_ThrowsItemValidationException()
    {
        var longThai = new string('ก', 251);
        var ex = Assert.Throws<ItemValidationException>(() => LocalizedText.Create(longThai, "Wood"));
        Assert.Equal("ITEM_TEXT_TOO_LONG", ex.Code);
    }

    [Fact]
    public void Create_WhenEnglishExceedsMaxLength_ThrowsItemValidationException()
    {
        var longEnglish = new string('A', 251);
        var ex = Assert.Throws<ItemValidationException>(() => LocalizedText.Create("ไม้", longEnglish));
        Assert.Equal("ITEM_TEXT_TOO_LONG", ex.Code);
    }
}
