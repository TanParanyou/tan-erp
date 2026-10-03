using TanErp.Domain.DocumentNumbering;
using Xunit;

namespace TanErp.UnitTests.DocumentNumbering;

public sealed class DocumentSequenceDefaultsTests
{
    [Theory]
    [InlineData(DocumentTypes.Items, "ITM-")]
    [InlineData(DocumentTypes.ItemCategories, "CAT-")]
    [InlineData(DocumentTypes.ItemBrands, "BRD-")]
    [InlineData(DocumentTypes.UnitsOfMeasure, "UOM-")]
    [InlineData(DocumentTypes.ItemTaxCategories, "TAX-")]
    [InlineData(DocumentTypes.CostSources, "SRC-")]
    public void Master_data_defaults_are_organization_wide_and_never_reset(string type, string prefix)
    {
        var defaults = DocumentSequenceDefaults.For(type);

        Assert.Equal(prefix, defaults.Prefix);
        Assert.Equal("{PREFIX}{SEQ:5}", defaults.FormatPattern);
        Assert.Equal(ResetPeriod.Never, defaults.ResetPeriod);
        Assert.Equal(5, defaults.Padding);
        Assert.False(defaults.IsBranchSpecific);
        Assert.Contains(type, DocumentTypes.GeneratedMasterData);
    }

    [Fact]
    public void Customer_defaults_remain_unchanged_and_outside_new_master_data_restrictions()
    {
        var defaults = DocumentSequenceDefaults.For(DocumentTypes.Customers);

        Assert.Equal("CUS-", defaults.Prefix);
        Assert.Equal(ResetPeriod.Never, defaults.ResetPeriod);
        Assert.DoesNotContain(DocumentTypes.Customers, DocumentTypes.GeneratedMasterData);
    }
}
