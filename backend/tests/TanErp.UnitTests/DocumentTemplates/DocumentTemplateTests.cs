using TanErp.Domain.DocumentTemplates;
using Xunit;

namespace TanErp.UnitTests.DocumentTemplates;

public class DocumentTemplateTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _templateId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Create_WithValidParameters_SetsPropertiesCorrectly()
    {
        var config = DocumentTemplateLayoutConfig.DefaultFor("quotations");
        var configJson = System.Text.Json.JsonSerializer.Serialize(config);

        var template = new DocumentTemplate(
            _templateId,
            _orgId,
            branchId: null,
            documentType: "quotations",
            name: "Standard Quotation",
            isDefault: true,
            layoutConfigJson: configJson,
            logoFileId: null,
            createdAtUtc: _now);

        Assert.Equal(_templateId, template.Id);
        Assert.Equal(_orgId, template.OrganizationId);
        Assert.Null(template.BranchId);
        Assert.Equal("quotations", template.DocumentType);
        Assert.Equal("Standard Quotation", template.Name);
        Assert.True(template.IsDefault);
        Assert.Equal(configJson, template.LayoutConfigJson);
        Assert.Null(template.LogoFileId);
        Assert.True(template.IsActive);
        Assert.Equal(_now, template.CreatedAtUtc);
        Assert.Equal(_now, template.UpdatedAtUtc);
        Assert.NotEqual(Guid.Empty, template.RowVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        var configJson = "{}";

        Assert.Throws<ArgumentException>(() => new DocumentTemplate(
            _templateId,
            _orgId,
            null,
            "quotations",
            invalidName!,
            true,
            configJson,
            null,
            _now));
    }

    [Fact]
    public void Update_ModifiesPropertiesAndUpdatesTimestamp()
    {
        var template = new DocumentTemplate(
            _templateId,
            _orgId,
            null,
            "quotations",
            "Initial Name",
            false,
            "{}",
            null,
            _now);

        var initialRowVersion = template.RowVersion;
        var later = _now.AddMinutes(5);
        var newLogoId = Guid.NewGuid();

        template.Update("Updated Name", true, "{\"updated\": true}", newLogoId, later);

        Assert.Equal("Updated Name", template.Name);
        Assert.True(template.IsDefault);
        Assert.Equal("{\"updated\": true}", template.LayoutConfigJson);
        Assert.Equal(newLogoId, template.LogoFileId);
        Assert.Equal(later, template.UpdatedAtUtc);
        Assert.NotEqual(initialRowVersion, template.RowVersion);
    }

    [Fact]
    public void LayoutConfig_DefaultFor_GeneratesValidDefaults()
    {
        var quotationConfig = DocumentTemplateLayoutConfig.DefaultFor("quotations");
        Assert.Equal("ใบเสนอราคา / QUOTATION", quotationConfig.Header.DocumentTitle);
        Assert.True(quotationConfig.Header.ShowLogo);
        Assert.True(quotationConfig.Columns.ShowSku);
        Assert.True(quotationConfig.Summary.ShowVat);
        Assert.Equal(2, quotationConfig.Signatures.SignatureCount);

        var poConfig = DocumentTemplateLayoutConfig.DefaultFor("purchase-orders");
        Assert.Equal("ใบสั่งซื้อสินค้า / PURCHASE ORDER", poConfig.Header.DocumentTitle);
    }
}
