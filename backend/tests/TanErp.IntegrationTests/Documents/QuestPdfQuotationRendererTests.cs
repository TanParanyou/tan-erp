using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using QuestPDF.Infrastructure;
using TanErp.Application.Commercial.Documents;
using TanErp.Infrastructure.Documents;
using Xunit;

namespace TanErp.IntegrationTests.Documents;

/// <summary>Pure rendering tests: no database, no Docker.</summary>
public class QuestPdfQuotationRendererTests
{
    private static QuotationPdfModel Model(int items, int sections, string descriptionTh) => new(
        "QT-2026-0001", new DateTimeOffset(2026, 10, 12, 3, 0, 0, TimeSpan.Zero), "THB", null,
        new QuotationPdfIssuer("บริษัท ตัวอย่าง จำกัด", "Example Co., Ltd.", "สำนักงานใหญ่", "Head office", "0105536000001", "00000", "99 ถนนสุขุมวิท กรุงเทพมหานคร", "99 Sukhumvit Rd, Bangkok", "02-000-0000"),
        new QuotationPdfCustomer("ลูกค้าทดสอบ", "Test Customer", "บริษัท ลูกค้า จำกัด", "0105536000002", "00000", new QuotationPdfAddress("1 ถนนหนึ่ง", "แขวงหนึ่ง", "เขตหนึ่ง", "กรุงเทพมหานคร", "10110")),
        Enumerable.Range(1, sections).Select(s => new QuotationPdfSection($"S{s}", $"งานระบบ {s}", s % 2 == 0 ? null : $"Systems {s}", 1000m * items,
            Enumerable.Range(1, items).Select(i => new QuotationPdfWorkItem($"W{s}{i:000}", descriptionTh, i % 2 == 0 ? null : "Install air conditioning", 1.5m, "ชุด", 666.67m, 1000m)).ToList())).ToList(),
        new QuotationPdfTotals(1000m * items * sections, 0m, 1000m * items * sections, 70m * items * sections, 1070m * items * sections));

    private static byte[] Render(QuotationPdfModel model) =>
        new QuestPdfQuotationRenderer(new ConfigurationBuilder().Build(), NullLogger<QuestPdfQuotationRenderer>.Instance).Render(model);

    private static int Pages(byte[] pdf) => Regex.Matches(Encoding.Latin1.GetString(pdf), @"/Type\s*/Page[^s]").Count;

    [Fact]
    public void Render_ThaiAndEnglish_ProducesOnePage_AndManyMaximumLengthRowsPaginate()
    {
        var small = Render(Model(3, 1, "ติดตั้งระบบปรับอากาศ"));
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(small, 0, 5));
        Assert.Equal(1, Pages(small));

        // EstimateWorkItem.DescriptionTh is at most 500 characters; rows may break across pages.
        Assert.True(Pages(Render(Model(40, 3, new string('ก', 500)))) >= 5);
    }

    [Fact]
    public void Version_AndLicenseAndEmbeddedFonts_AreWhatTheAdrSays()
    {
        Assert.Matches(@"^questpdf-\d+\.\d+\.\d+/quotation-v1$", new QuestPdfQuotationRenderer(new ConfigurationBuilder().Build(), NullLogger<QuestPdfQuotationRenderer>.Instance).Version);
        Assert.Equal(LicenseType.Community, QuestPdfRuntime.ParseLicense("Community"));
        Assert.Equal(LicenseType.Professional, QuestPdfRuntime.ParseLicense("Professional"));
        Assert.Throws<InvalidOperationException>(() => QuestPdfRuntime.ParseLicense("Evaluation"));
        Assert.Throws<InvalidOperationException>(() => QuestPdfRuntime.ParseLicense(null));

        var resources = typeof(QuestPdfRuntime).Assembly.GetManifestResourceNames();
        foreach (var file in QuestPdfRuntime.FontFiles.Append("OFL.txt")) Assert.Contains(QuestPdfRuntime.FontResourcePrefix + file, resources);
    }
}
