using TanErp.Application.Common.Security;
using Xunit;

namespace TanErp.UnitTests.Common.Security;

public class Sha256HexTests
{
    [Theory]
    [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
    [InlineData("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    public void Compute_ReturnsLowercaseSha256(string value, string expected)
    {
        Assert.Equal(expected, Sha256Hex.Compute(value));
    }
}
