using TanErp.Domain.Crm.Sites;
using Xunit;

namespace TanErp.UnitTests.Crm.Sites;

public class SiteTests
{
    [Fact]
    public void CreateActive_WhenValid_SetsPropertiesAndGeneratesCode()
    {
        var id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c10");
        var orgId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var address = new SiteAddressInput(
            "  99/9 สุขุมวิท TEST_ONLY  ",
            "  คลองตันเหนือ  ",
            "  วัฒนา  ",
            "  กรุงเทพมหานคร  ",
            "  10110  ",
            "th");

        var site = Site.CreateActive(
            id,
            orgId,
            customerId,
            actorId,
            "  คอนโดสุขุมวิท TEST_ONLY  ",
            address,
            13.736717m,
            100.561081m,
            "  ติดต่อเจ้าหน้าที่ก่อน  ",
            now);

        Assert.Equal(id, site.Id);
        Assert.Equal(orgId, site.OrganizationId);
        Assert.Equal(customerId, site.CustomerId);
        Assert.Equal("SITE-019A3CF896F0", site.Code);
        Assert.Equal("คอนโดสุขุมวิท TEST_ONLY", site.Label);
        Assert.Equal("คอนโดสุขุมวิท test_only", site.NormalizedLabel);
        Assert.Equal("active", site.Status);
        Assert.Equal("99/9 สุขุมวิท TEST_ONLY", site.AddressLine1);
        Assert.Equal("คลองตันเหนือ", site.Subdistrict);
        Assert.Equal("วัฒนา", site.District);
        Assert.Equal("กรุงเทพมหานคร", site.Province);
        Assert.Equal("10110", site.PostalCode);
        Assert.Equal("TH", site.CountryCode);
        Assert.Equal(13.736717m, site.Latitude);
        Assert.Equal(100.561081m, site.Longitude);
        Assert.Equal("ติดต่อเจ้าหน้าที่ก่อน", site.AccessNote);
        Assert.NotEqual(Guid.Empty, site.RowVersion);
        Assert.Equal(now, site.CreatedAtUtc);
    }

    [Fact]
    public void CreateActive_WhenBlankLabel_ThrowsArgumentException()
    {
        var address = new SiteAddressInput("99/9 สุขุมวิท", "คลองตันเหนือ", "วัฒนา", "กรุงเทพฯ", "10110", "TH");
        Assert.Throws<ArgumentException>(() => Site.CreateActive(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "   ", address, null, null, null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateActive_WhenAddressFieldBlank_ThrowsArgumentException()
    {
        var address = new SiteAddressInput("   ", "คลองตันเหนือ", "วัฒนา", "กรุงเทพฯ", "10110", "TH");
        Assert.Throws<ArgumentException>(() => Site.CreateActive(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "คอนโด", address, null, null, null, DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("13.7", null)]
    [InlineData(null, "100.5")]
    public void CreateActive_WhenUnpairedCoordinates_ThrowsArgumentException(string? latStr, string? lonStr)
    {
        decimal? lat = latStr != null ? decimal.Parse(latStr) : null;
        decimal? lon = lonStr != null ? decimal.Parse(lonStr) : null;
        var address = new SiteAddressInput("99/9 สุขุมวิท", "คลองตันเหนือ", "วัฒนา", "กรุงเทพฯ", "10110", "TH");
        Assert.Throws<ArgumentException>(() => Site.CreateActive(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "คอนโด", address, lat, lon, null, DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("91.0", "100.0")]
    [InlineData("-91.0", "100.0")]
    [InlineData("13.0", "181.0")]
    [InlineData("13.0", "-181.0")]
    public void CreateActive_WhenCoordinateOutOfRange_ThrowsArgumentOutOfRangeException(string latStr, string lonStr)
    {
        var lat = decimal.Parse(latStr);
        var lon = decimal.Parse(lonStr);
        var address = new SiteAddressInput("99/9 สุขุมวิท", "คลองตันเหนือ", "วัฒนา", "กรุงเทพฯ", "10110", "TH");
        Assert.Throws<ArgumentOutOfRangeException>(() => Site.CreateActive(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "คอนโด", address, lat, lon, null, DateTimeOffset.UtcNow));
    }
}
