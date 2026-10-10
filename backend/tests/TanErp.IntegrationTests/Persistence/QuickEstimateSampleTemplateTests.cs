using Microsoft.EntityFrameworkCore;
using TanErp.Domain.QuickEstimates;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class QuickEstimateSampleTemplateTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public async Task DisposeAsync() => await _postgres.DisposeAsync();

    private AppDbContext CreateDbContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);

    [Fact]
    public async Task DemoSeed_CreatesFiveCalibrationTemplatesOnceAndTheyPriceDeterministically()
    {
        await using (var db = CreateDbContext())
        {
            await db.Database.MigrateAsync();
            await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedEstimateDemoData: true);
        }

        await using (var db = CreateDbContext())
        {
            // A second seed must not duplicate or reset anything.
            await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedEstimateDemoData: true);
        }

        await using var read = CreateDbContext();
        var templates = await read.PricingTemplates.AsNoTracking().Where(t => t.Code.StartsWith("SAMPLE-")).OrderBy(t => t.Code).ToListAsync();
        Assert.Equal(5, templates.Count);
        Assert.All(templates, template =>
        {
            Assert.Equal(TemplateStatus.Calibration, template.Status);
            Assert.StartsWith("[ตัวอย่าง]", template.Name);
            Assert.True(template.Config.Grades.Count >= 2);
        });
        Assert.Contains(templates, t => t.WorkType == WorkType.Wallpaper);
        Assert.Contains(templates, t => t.WorkType == WorkType.Curtain);
        Assert.Contains(templates, t => t.WorkType == WorkType.BuiltIn);

        // 3.0 m of wardrobe, premium grade: 3.0 x 8,500 x 1.25 = 31,875, above the 25,000 minimum.
        var wardrobe = templates.Single(t => t.Code == "SAMPLE-BI-WARDROBE-LM");
        var result = QuickEstimateEngine.Calculate(
            wardrobe,
            new QuickEstimateInput("คอนโด", "ห้องนอน", "PREMIUM", Array.Empty<string>(), Array.Empty<string>(), MeasurementConfidence.High, false,
                new[] { new MeasurementLine(Guid.NewGuid(), "wardrobe", 3.0m, null, null, 1m) }),
            DateOnly.FromDateTime(DateTime.UtcNow));
        Assert.Equal(31875m, result.NetAmount);
        // A calibration template can never be shared without a review.
        Assert.Equal(ShareDecision.PendingReview, result.ShareDecision);
        Assert.Contains("TEMPLATE_IN_CALIBRATION", result.ReasonCodes);
    }
}
