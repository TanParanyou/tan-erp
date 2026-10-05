using TanErp.Application.Common.Security;
using Xunit;

namespace TanErp.UnitTests.Common;

public class SignatureEvidenceRulesTests
{
    // 8-byte PNG signature + one filler byte: the shortest payload CP-07 accepts (length must be > 8).
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];

    [Theory]
    [InlineData("ab", true)]
    [InlineData("  ab  ", true)]
    [InlineData("a", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void TryNormalizeSigner_EnforcesTheNameLowerBound(string? name, bool expected)
    {
        Assert.Equal(expected, SignatureEvidenceRules.TryNormalizeSigner(name, null, out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_EnforcesTheNameUpperBound_AndTrims()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner(new string('x', 200), null, out var name, out _));
        Assert.Equal(200, name.Length);
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner(new string('x', 201), null, out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_MeasuresTheRoleBeforeTrimming_AsCp07Did()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 100), out _, out var role));
        Assert.Equal(100, role!.Length);
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 101), out _, out _));
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 100) + " ", out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_TurnsABlankRoleIntoNull()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner("ab", "   ", out _, out var role));
        Assert.Null(role);
    }

    [Fact]
    public void IsPngBase64_AcceptsRawAndDataUrlPayloads()
    {
        var base64 = Convert.ToBase64String(PngBytes);
        Assert.True(SignatureEvidenceRules.IsPngBase64(base64));
        Assert.True(SignatureEvidenceRules.IsPngBase64("data:image/png;base64," + base64));
    }

    [Fact]
    public void IsPngBase64_RejectsNonPngInvalidBase64AndTooShortPayloads()
    {
        Assert.False(SignatureEvidenceRules.IsPngBase64(Convert.ToBase64String(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0 })));
        Assert.False(SignatureEvidenceRules.IsPngBase64("not base64 !!"));
        Assert.False(SignatureEvidenceRules.IsPngBase64(Convert.ToBase64String(PngBytes[..8])));
    }

    [Fact]
    public void IsPngSignature_ChecksTheLeadingMagicBytes()
    {
        Assert.True(SignatureEvidenceRules.IsPngSignature(PngBytes));
        Assert.False(SignatureEvidenceRules.IsPngSignature(PngBytes[..8]));
        Assert.False(SignatureEvidenceRules.IsPngSignature(new byte[] { 0x00, 0x50, 0x4E, 0x47, 0, 0, 0, 0, 0 }));
    }

    [Fact]
    public void HashSubmittedImage_IsTheSha256OfTheSubmittedString()
    {
        var hash = SignatureEvidenceRules.HashSubmittedImage("abc");
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }

    [Theory]
    [InlineData("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", true)]
    [InlineData("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", false)]
    [InlineData("abc", false)]
    [InlineData(null, false)]
    public void IsSha256Hex_RequiresSixtyFourLowerCaseHexCharacters(string? value, bool expected)
    {
        Assert.Equal(expected, SignatureEvidenceRules.IsSha256Hex(value));
    }
}
