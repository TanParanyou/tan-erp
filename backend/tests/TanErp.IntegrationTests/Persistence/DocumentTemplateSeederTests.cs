using Microsoft.EntityFrameworkCore;
using TanErp.Domain.DocumentTemplates;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.DocumentTemplates;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class DocumentTemplateSeederTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task SeedAsync_CreatesExpectedTemplates_Idempotently()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var now = DateTimeOffset.UtcNow;

        // Ensure Org & Branch exist first
        var org = await _db.Organizations.FindAsync(orgId);
        if (org == null)
        {
            _db.Organizations.Add(new TanErp.Domain.Organization.Organization(orgId, "TEST_ORG"));
            _db.Branches.Add(new TanErp.Domain.Organization.Branch(branchId, orgId, "HQ", "สำนักงานใหญ่"));
            await _db.SaveChangesAsync();
        }

        // First run
        await DocumentTemplateSeeder.SeedAsync(_db, orgId, branchId, now);

        var templates = await _db.DocumentTemplates.ToListAsync();
        Assert.Equal(6, templates.Count);

        var hqQuotation = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoQuotationTemplateHqId);
        Assert.Equal("quotation", hqQuotation.DocumentType);
        Assert.True(hqQuotation.IsDefault);
        Assert.Contains("บริษัท แทน คอนสตรัคชั่น แอนด์ ดีไซน์ จำกัด (สำนักงานใหญ่)", hqQuotation.LayoutConfigJson);

        var projectQuotation = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoQuotationTemplateProjectId);
        Assert.Equal("quotation", projectQuotation.DocumentType);
        Assert.False(projectQuotation.IsDefault);

        var estimate = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoEstimateTemplateId);
        Assert.Equal("estimate", estimate.DocumentType);
        Assert.True(estimate.IsDefault);

        var po = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoPurchaseOrderTemplateId);
        Assert.Equal("purchase_order", po.DocumentType);
        Assert.True(po.IsDefault);

        var billing = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoBillingTemplateId);
        Assert.Equal("billing", billing.DocumentType);
        Assert.True(billing.IsDefault);

        var gr = templates.Single(t => t.Id == DocumentTemplateSeeder.DemoGoodsReceiptTemplateId);
        Assert.Equal("goods_receipt", gr.DocumentType);
        Assert.True(gr.IsDefault);

        // Second run - Idempotency test (no duplicates added)
        await DocumentTemplateSeeder.SeedAsync(_db, orgId, branchId, now);
        var secondRunCount = await _db.DocumentTemplates.CountAsync();
        Assert.Equal(6, secondRunCount);
    }
}
