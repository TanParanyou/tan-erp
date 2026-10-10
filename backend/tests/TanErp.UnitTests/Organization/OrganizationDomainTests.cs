using TanErp.Domain.Organization;
using Xunit;

namespace TanErp.UnitTests.OrganizationDomain;

public class OrganizationDomainTests
{
    private static readonly Guid OrgId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12");

    [Theory]
    [InlineData("0105536000003", true)]
    [InlineData("0105554000001", true)]
    [InlineData("0105536000004", false)] // wrong checksum
    [InlineData("010553600000", false)]  // 12 digits
    [InlineData("01055360000a3", false)]
    [InlineData("", false)]
    public void ThaiTaxIdentifier_ValidatesLengthDigitsAndChecksum(string value, bool expected) =>
        Assert.Equal(expected, ThaiTaxIdentifier.IsValid(value));

    [Theory]
    [InlineData("00000", true)]
    [InlineData("00012", true)]
    [InlineData("0001", false)]
    [InlineData("000123", false)]
    [InlineData("0001a", false)]
    public void TaxBranchCodeRule_RequiresExactlyFiveDigits(string value, bool expected) =>
        Assert.Equal(expected, TaxBranchCodeRule.IsValid(value));

    [Theory]
    [InlineData("B01", true)]
    [InlineData("hq_1-a", true)]
    [InlineData("", false)]
    [InlineData("สาขา", false)]
    [InlineData("a b", false)]
    public void BranchCode_AllowsAsciiLettersDigitsUnderscoreHyphen(string value, bool expected) =>
        Assert.Equal(expected, BranchCode.IsValid(value));

    [Fact]
    public void UpdateProfile_TrimsNormalizesBlankToNullAndBumpsRowVersion()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "เก่า");
        var before = org.RowVersion;

        org.UpdateProfile("  ใหม่  ", " ", "0105536000003", "  ที่อยู่ ", null, "  ");

        Assert.Equal("ใหม่", org.Name);
        Assert.Null(org.NameEn);
        Assert.Equal("0105536000003", org.TaxIdentifier);
        Assert.Equal("ที่อยู่", org.AddressTh);
        Assert.Null(org.Phone);
        Assert.NotEqual(before, org.RowVersion);
    }

    [Fact]
    public void UpdateProfile_InvalidTaxIdentifier_ThrowsWithCode()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "x");

        var ex = Assert.Throws<OrganizationDomainException>(() => org.UpdateProfile("x", null, "0105536000004", null, null, null));

        Assert.Equal("ORGANIZATION_TAX_ID_INVALID", ex.Code);
    }

    [Fact]
    public void BranchCreate_SetsDetailsAndRejectsInvalidTaxBranchCode()
    {
        var branch = Branch.Create(Guid.NewGuid(), OrgId, " B02 ", "สาขา 2", "Branch 2", "00001", "addr", null, "02-123", DateTimeOffset.UtcNow);

        Assert.Equal("B02", branch.Code);
        Assert.Equal("00001", branch.TaxBranchCode);
        Assert.True(branch.IsActive);

        var ex = Assert.Throws<OrganizationDomainException>(() =>
            Branch.Create(Guid.NewGuid(), OrgId, "B03", "x", null, "1", null, null, null, DateTimeOffset.UtcNow));
        Assert.Equal("BRANCH_TAX_CODE_INVALID", ex.Code);

        var codeEx = Assert.Throws<OrganizationDomainException>(() =>
            Branch.Create(Guid.NewGuid(), OrgId, "bad code", "x", null, null, null, null, null, DateTimeOffset.UtcNow));
        Assert.Equal("BRANCH_CODE_INVALID", codeEx.Code);
    }

    [Fact]
    public void BranchUpdateDeactivateActivate_EachBumpsRowVersionAndKeepsCode()
    {
        var branch = Branch.Create(Guid.NewGuid(), OrgId, "B04", "x", null, null, null, null, null, DateTimeOffset.UtcNow);
        var v0 = branch.RowVersion;

        branch.UpdateDetails("y", null, "00004", null, null, null);
        var v1 = branch.RowVersion;
        branch.Deactivate();
        var v2 = branch.RowVersion;
        branch.Activate();

        Assert.Equal("B04", branch.Code);
        Assert.Equal("y", branch.Name);
        Assert.True(branch.IsActive);
        Assert.Equal(4, new[] { v0, v1, v2, branch.RowVersion }.Distinct().Count());
    }
}
