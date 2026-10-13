using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TanErp.Application.Commercial.Documents;

namespace TanErp.Infrastructure.Documents;

public sealed class QuestPdfQuotationRenderer : IQuotationDocumentRenderer
{
    public const string LayoutVersion = "quotation-v1";
    private const string Navy = "#0B3056";
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public QuestPdfQuotationRenderer(IConfiguration configuration, ILogger<QuestPdfQuotationRenderer> logger)
    {
        var licenseType = configuration["Documents:QuestPdf:LicenseType"] ?? "Community";
        QuestPdfRuntime.EnsureInitialized(licenseType);
        // The business must confirm the license before production (ADR 0023). A warning keeps issuing available.
        if (string.Equals(configuration["ASPNETCORE_ENVIRONMENT"], "Production", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(configuration["Documents:QuestPdf:LicenseConfirmed"], "true", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("QuestPDF license type {LicenseType} is not confirmed by the business (Documents:QuestPdf:LicenseConfirmed). See ADR 0023.", licenseType);
        }
    }

    public string Version => $"questpdf-{typeof(QuestPDF.Settings).Assembly.GetName().Version?.ToString(3)}/{LayoutVersion}";

    public byte[] Render(QuotationPdfModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var document = Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(32);
            page.DefaultTextStyle(style => style.FontFamily(QuestPdfRuntime.FontFamily).FontSize(9.5f));
            page.Header().AlignRight().Text(model.Number).FontSize(8).FontColor(Colors.Grey.Darken1);
            page.Content().Element(content => Compose(content, model));
            page.Footer().AlignCenter().Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Darken1));
                text.Span("หน้า / Page ");
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }));
        document.WithMetadata(new DocumentMetadata
        {
            Title = model.Number, Author = model.Issuer.NameTh, Creator = "tan-erp", Language = "th-TH",
            CreationDate = model.IssuedAtUtc, ModifiedDate = model.IssuedAtUtc
        });
        return document.GeneratePdf();
    }

    private static string Money(decimal value) => value.ToString("#,##0.00", Invariant);

    private static string? Join(string separator, params string?[] parts)
    {
        var present = parts.Where(part => !string.IsNullOrWhiteSpace(part)).ToList();
        return present.Count == 0 ? null : string.Join(separator, present);
    }

    private static void Lines(ColumnDescriptor column, params string?[] lines)
    {
        foreach (var line in lines.Where(line => !string.IsNullOrWhiteSpace(line))) column.Item().Text(line);
    }

    private static void Compose(IContainer container, QuotationPdfModel model)
    {
        var issued = model.IssuedAtUtc.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy", Invariant); // Thailand has no DST
        var issuer = model.Issuer;
        container.Column(column =>
        {
            column.Spacing(8);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(issuer.NameTh).Bold().FontSize(12);
                    Lines(left, issuer.NameEn, Join(" / ", issuer.BranchNameTh, issuer.BranchNameEn),
                        Join("  ", issuer.TaxIdentifier is null ? null : $"เลขประจำตัวผู้เสียภาษี / Tax ID {issuer.TaxIdentifier}",
                            issuer.TaxBranchCode is null ? null : $"สาขา / Branch {issuer.TaxBranchCode}"),
                        issuer.AddressTh, issuer.AddressEn, issuer.Phone is null ? null : $"โทร / Tel {issuer.Phone}");
                });
                row.ConstantItem(190).Column(right =>
                {
                    right.Item().AlignRight().Text("ใบเสนอราคา / Quotation").Bold().FontSize(15).FontColor(Navy);
                    right.Item().AlignRight().Text($"เลขที่ / No. {model.Number}");
                    right.Item().AlignRight().Text($"วันที่ / Date {issued}");
                    right.Item().AlignRight().Text($"สกุลเงิน / Currency {model.Currency}");
                    if (model.SupersedesNumber is not null) right.Item().AlignRight().Text($"แทนที่ฉบับเลขที่ / Replaces {model.SupersedesNumber}").FontSize(8.5f);
                });
            });
            column.Item().LineHorizontal(1.5f).LineColor(Navy);
            column.Item().Border(0.75f).BorderColor(Colors.Grey.Medium).Padding(6).Column(box =>
            {
                var customer = model.Customer;
                box.Item().Text("ลูกค้า / Customer").Bold().FontColor(Navy);
                box.Item().Text(customer.DisplayNameTh).Bold();
                var address = customer.Address is null ? null : Join(" ", customer.Address.AddressLine1, customer.Address.Subdistrict, customer.Address.District, customer.Address.Province, customer.Address.PostalCode);
                Lines(box, customer.DisplayNameEn, customer.LegalName is null ? null : $"ชื่อตามกฎหมาย / Legal name: {customer.LegalName}",
                    Join("  ", customer.TaxIdentifier is null ? null : $"เลขประจำตัวผู้เสียภาษี / Tax ID {customer.TaxIdentifier}", customer.BranchCode is null ? null : $"สาขา / Branch {customer.BranchCode}"),
                    address is null ? null : $"ที่อยู่ / Address: {address}");
            });
            foreach (var section in model.Sections) column.Item().Element(content => ComposeSection(content, section));
            column.Item().ShowEntire().AlignRight().Width(240).Column(totals =>
            {
                void Line(string label, decimal value, bool strong = false) => totals.Item().Row(row =>
                {
                    var name = row.RelativeItem().Text(label);
                    var amount = row.ConstantItem(90).AlignRight().Text(Money(value));
                    if (strong) { name.Bold(); amount.Bold(); }
                });
                Line("รวมเป็นเงิน / Subtotal", model.Totals.Subtotal);
                if (model.Totals.DiscountAmount > 0) Line("ส่วนลด / Discount", model.Totals.DiscountAmount);
                Line("ยอดหลังหักส่วนลด / Net before tax", model.Totals.NetBeforeTax);
                Line("ภาษีมูลค่าเพิ่ม / VAT", model.Totals.TaxAmount);
                totals.Item().PaddingTop(2).LineHorizontal(1.5f).LineColor(Navy);
                Line($"ยอดรวมสุทธิ / Grand total ({model.Currency})", model.Totals.GrandTotal, strong: true);
            });
        });
    }

    private static IContainer Cell(IContainer container) =>
        container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).PaddingVertical(3).PaddingHorizontal(2);

    private static void ComposeSection(IContainer container, QuotationPdfSection section)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(56); columns.RelativeColumn(); columns.ConstantColumn(44);
                columns.ConstantColumn(40); columns.ConstantColumn(64); columns.ConstantColumn(68);
            });
            table.Header(header =>
            {
                header.Cell().ColumnSpan(6).Background(Colors.Grey.Lighten3).Padding(3).Text(text =>
                {
                    text.Span($"{section.Code}  {section.NameTh}").Bold();
                    if (!string.IsNullOrWhiteSpace(section.NameEn)) text.Span($" / {section.NameEn}");
                });
                foreach (var (label, right) in new[] { ("รหัส / Code", false), ("รายละเอียด / Description", false), ("จำนวน / Qty", true), ("หน่วย / Unit", false), ("ราคา/หน่วย / Unit price", true), ("รวม / Amount", true) })
                {
                    var cell = header.Cell().Element(Cell);
                    (right ? cell.AlignRight() : cell).Text(label).Bold();
                }
            });
            foreach (var item in section.WorkItems)
            {
                table.Cell().Element(Cell).Text(item.Code);
                table.Cell().Element(Cell).Column(description =>
                {
                    description.Item().Text(item.DescriptionTh);
                    if (!string.IsNullOrWhiteSpace(item.DescriptionEn)) description.Item().Text(item.DescriptionEn).FontColor(Colors.Grey.Darken2);
                });
                table.Cell().Element(Cell).AlignRight().Text(item.Quantity.ToString("#,##0.####", Invariant));
                table.Cell().Element(Cell).Text(item.UnitCode);
                table.Cell().Element(Cell).AlignRight().Text(Money(item.UnitPrice));
                table.Cell().Element(Cell).AlignRight().Text(Money(item.LineTotal));
            }
            table.Cell().ColumnSpan(5).Element(Cell).AlignRight().Text("รวมหมวด / Section subtotal").Bold();
            table.Cell().Element(Cell).AlignRight().Text(Money(section.Subtotal)).Bold();
        });
    }
}
