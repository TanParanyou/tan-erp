using TanErp.Domain.Crm.Customers;
using Xunit;

namespace TanErp.UnitTests.Crm.Customers;

public sealed class TaxIdentifierValidatorTests
{
    [Fact]
    public void IsValid_AcceptsThirteenDigitValueWithCorrectCheckDigit()
    {
        Assert.True(TaxIdentifierValidator.IsValid("0105563001236"));
    }

    [Theory]
    [InlineData("0105563001235")]
    [InlineData("010556300123")]
    [InlineData("01055630012A6")]
    public void IsValid_RejectsInvalidLengthOrCheckDigit(string value)
    {
        Assert.False(TaxIdentifierValidator.IsValid(value));
    }
}
