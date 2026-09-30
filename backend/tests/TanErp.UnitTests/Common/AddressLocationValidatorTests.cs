using TanErp.Domain.Common;
using Xunit;

namespace TanErp.UnitTests.Common;

public sealed class AddressLocationValidatorTests
{
    [Fact]
    public void IsValid_AcceptsCompleteThaiAddressLocation()
    {
        Assert.True(AddressLocationValidator.IsValid("คลองตันเหนือ", "วัฒนา", "กรุงเทพมหานคร", "10110", "TH"));
    }

    [Fact]
    public void IsValid_AcceptsValuesWithSurroundingWhitespace()
    {
        Assert.True(AddressLocationValidator.IsValid(" คลองตันเหนือ ", " วัฒนา ", " กรุงเทพมหานคร ", " 10110 ", " th "));
    }

    [Theory]
    [InlineData("", "วัฒนา", "กรุงเทพมหานคร", "10110", "TH")]
    [InlineData("คลองตันเหนือ", "", "กรุงเทพมหานคร", "10110", "TH")]
    [InlineData("คลองตันเหนือ", "วัฒนา", "", "10110", "TH")]
    [InlineData("คลองตันเหนือ", "วัฒนา", "กรุงเทพมหานคร", "1011", "TH")]
    [InlineData("คลองตันเหนือ", "วัฒนา", "กรุงเทพมหานคร", "10A10", "TH")]
    [InlineData("คลองตันเหนือ", "วัฒนา", "กรุงเทพมหานคร", "10110", "T")]
    public void IsValid_RejectsIncompleteOrMalformedLocation(
        string subdistrict,
        string district,
        string province,
        string postalCode,
        string countryCode)
    {
        Assert.False(AddressLocationValidator.IsValid(subdistrict, district, province, postalCode, countryCode));
    }
}
