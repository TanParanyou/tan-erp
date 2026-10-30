using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Finance;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Documents;

public sealed record TaxDocumentPdfModel(
    Organization Organization,
    Branch Branch,
    Project Project,
    Customer? Customer,
    BillingDocument Billing,
    IReadOnlyList<BillingLine> Lines,
    IReadOnlyList<BillingAdjustmentNote> Notes,
    IReadOnlyList<Payment> Payments);

public static class QuestPdfTaxDocumentRenderer
{
    private const string Navy = "#0B3056";
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private static string Money(decimal value) => value.ToString("#,##0.00", Invariant);

    public static byte[] Render(TaxDocumentPdfModel model)
    {
        QuestPdfRuntime.EnsureInitialized("Community");

        var docTitle = !string.IsNullOrEmpty(model.Billing.TaxInvoiceNumber)
            ? "ใบแจ้งหนี้ / ใบกำกับภาษี (Invoice / Tax Invoice)"
            : "ใบแจ้งหนี้ (Invoice)";

        var document = Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(32);
            page.DefaultTextStyle(style => style.FontFamily(QuestPdfRuntime.FontFamily).FontSize(9.5f));

            page.Header().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(model.Organization.Name).Bold().FontSize(12).FontColor(Navy);
                    if (!string.IsNullOrWhiteSpace(model.Organization.NameEn))
                    {
                        left.Item().Text(model.Organization.NameEn).FontSize(9).FontColor(Colors.Grey.Darken2);
                    }
                    var branchText = string.Join(" / ", new[] { model.Branch.Name, model.Branch.NameEn }.Where(s => !string.IsNullOrWhiteSpace(s)));
                    left.Item().Text($"สาขา: {branchText} (รหัสสาขา: {model.Branch.TaxBranchCode ?? "00000"})").FontSize(8.5f);
                    if (!string.IsNullOrWhiteSpace(model.Organization.TaxIdentifier))
                    {
                        left.Item().Text($"เลขประจำตัวผู้เสียภาษี: {model.Organization.TaxIdentifier}").FontSize(8.5f);
                    }
                    var addr = model.Branch.AddressTh ?? model.Organization.AddressTh;
                    if (!string.IsNullOrWhiteSpace(addr))
                    {
                        left.Item().Text(addr).FontSize(8.5f);
                    }
                    var phone = model.Branch.Phone ?? model.Organization.Phone;
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        left.Item().Text($"โทร: {phone}").FontSize(8.5f);
                    }
                });

                row.ConstantItem(220).Column(right =>
                {
                    right.Item().AlignRight().Text(docTitle).Bold().FontSize(11).FontColor(Navy);
                    right.Item().PaddingTop(4).AlignRight().Text($"เลขที่ใบแจ้งหนี้: {model.Billing.Number}").Bold().FontSize(9.5f);
                    if (!string.IsNullOrWhiteSpace(model.Billing.TaxInvoiceNumber))
                    {
                        right.Item().AlignRight().Text($"เลขที่ใบกำกับภาษี: {model.Billing.TaxInvoiceNumber}").Bold().FontSize(9.5f).FontColor(Navy);
                    }
                    var issuedDate = model.Billing.IssuedAtUtc.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy", Invariant);
                    right.Item().AlignRight().Text($"วันที่ออก: {issuedDate}").FontSize(8.5f);
                    if (model.Billing.DueDate.HasValue)
                    {
                        right.Item().AlignRight().Text($"วันครบกำหนด: {model.Billing.DueDate.Value.ToString("dd/MM/yyyy", Invariant)}").FontSize(8.5f);
                    }
                    right.Item().AlignRight().Text($"สถานะ: {model.Billing.Status}").FontSize(8.5f);
                });
            });

            page.Content().PaddingTop(16).Column(col =>
            {
                col.Spacing(12);

                // Customer & Project Info
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Row(infoRow =>
                {
                    infoRow.RelativeItem().Column(cCol =>
                    {
                        cCol.Item().Text("ข้อมูลลูกค้า / Customer Details").Bold().FontSize(9).FontColor(Navy);
                        var custName = model.Customer?.DisplayNameTh ?? "ลูกค้าทั่วไป";
                        cCol.Item().Text(custName).Bold().FontSize(9.5f);
                        if (!string.IsNullOrWhiteSpace(model.Customer?.TaxIdentifier))
                        {
                            cCol.Item().Text($"เลขประจำตัวผู้เสียภาษี: {model.Customer.TaxIdentifier}").FontSize(8.5f);
                        }
                    });

                    infoRow.RelativeItem().Column(pCol =>
                    {
                        pCol.Item().Text("ข้อมูลโครงการ / Project").Bold().FontSize(9).FontColor(Navy);
                        pCol.Item().Text($"{model.Project.Code} - {model.Project.Name}").Bold().FontSize(9.5f);
                        pCol.Item().Text($"ประเภทการวางบิล: {model.Billing.Kind}").FontSize(8.5f);
                        pCol.Item().Text($"รายละเอียด: {model.Billing.Description}").FontSize(8.5f);
                    });
                });

                // Line items or billing summary table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(5);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Navy).Padding(4).AlignCenter().Text("#").Bold().FontColor(Colors.White).FontSize(8.5f);
                        header.Cell().Background(Navy).Padding(4).Text("รายการ / Description").Bold().FontColor(Colors.White).FontSize(8.5f);
                        header.Cell().Background(Navy).Padding(4).AlignRight().Text("ฐานภาษี / Tax Base").Bold().FontColor(Colors.White).FontSize(8.5f);
                        header.Cell().Background(Navy).Padding(4).AlignRight().Text("VAT %").Bold().FontColor(Colors.White).FontSize(8.5f);
                        header.Cell().Background(Navy).Padding(4).AlignRight().Text("ภาษีมูลค่าเพิ่ม").Bold().FontColor(Colors.White).FontSize(8.5f);
                        header.Cell().Background(Navy).Padding(4).AlignRight().Text("รวมเงิน / Total").Bold().FontColor(Colors.White).FontSize(8.5f);
                    });

                    if (model.Lines.Count > 0)
                    {
                        foreach (var line in model.Lines.OrderBy(l => l.LineNumber))
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(line.LineNumber.ToString()).FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(line.Description).FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(line.TaxBaseAmount)).FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{(line.VatRate * 100):0.##}%").FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(line.VatAmount)).FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(line.TotalAmount)).FontSize(8.5f);
                        }
                    }
                    else
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("1").FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(model.Billing.Description).FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(model.Billing.Amount)).FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{(model.Billing.VatRate * 100):0.##}%").FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(model.Billing.VatAmount)).FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Money(model.Billing.Amount + model.Billing.VatAmount)).FontSize(8.5f);
                    }
                });

                // Summary / Totals block
                col.Item().Row(summaryRow =>
                {
                    summaryRow.RelativeItem(3).Column(leftNotes =>
                    {
                        if (model.Billing.RetentionAmount > 0)
                        {
                            var statusText = model.Billing.RetentionStatus == RetentionHoldingStatus.Held ? "หักเงินประกันไว้ (Held)" : "ปลดเงินประกันแล้ว (Released)";
                            leftNotes.Item().Text($"เงินประกันผลงาน (Retention {(model.Billing.RetentionRate * 100):0.##}%): {Money(model.Billing.RetentionAmount)} บาท — สถานะ: {statusText}").FontSize(8.5f);
                        }

                        if (model.Notes.Any(n => n.Status == BillingNoteStatus.Approved))
                        {
                            leftNotes.Item().PaddingTop(4).Text("ใบลดหนี้ / ใบเพิ่มหนี้ที่อนุมัติแล้ว:").Bold().FontSize(8.5f);
                            foreach (var note in model.Notes.Where(n => n.Status == BillingNoteStatus.Approved))
                            {
                                var typeTh = note.NoteType == BillingNoteType.Credit ? "ใบลดหนี้" : "ใบเพิ่มหนี้";
                                leftNotes.Item().Text($"• {typeTh} {note.Number}: {Money(note.TotalDelta)} บาท ({note.Reason})").FontSize(8f);
                            }
                        }
                    });

                    summaryRow.RelativeItem(2).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Column(totals =>
                    {
                        totals.Spacing(2);
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text("รวมฐานภาษี (Subtotal):").FontSize(8.5f);
                            r.ConstantItem(80).AlignRight().Text(Money(model.Billing.Amount)).FontSize(8.5f);
                        });
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"ภาษีมูลค่าเพิ่ม VAT ({(model.Billing.VatRate * 100):0.##}%):").FontSize(8.5f);
                            r.ConstantItem(80).AlignRight().Text(Money(model.Billing.VatAmount)).FontSize(8.5f);
                        });
                        if (model.Billing.CreditNoteBaseAmount > 0 || model.Billing.CreditNoteVatAmount > 0)
                        {
                            totals.Item().Row(r =>
                            {
                                r.RelativeItem().Text("หัก ใบลดหนี้รวม (CN):").FontSize(8.5f).FontColor(Colors.Red.Medium);
                                r.ConstantItem(80).AlignRight().Text($"-{Money(model.Billing.CreditNoteBaseAmount + model.Billing.CreditNoteVatAmount)}").FontSize(8.5f).FontColor(Colors.Red.Medium);
                            });
                        }
                        if (model.Billing.DebitNoteBaseAmount > 0 || model.Billing.DebitNoteVatAmount > 0)
                        {
                            totals.Item().Row(r =>
                            {
                                r.RelativeItem().Text("บวก ใบเพิ่มหนี้รวม (DN):").FontSize(8.5f);
                                r.ConstantItem(80).AlignRight().Text($"+{Money(model.Billing.DebitNoteBaseAmount + model.Billing.DebitNoteVatAmount)}").FontSize(8.5f);
                            });
                        }
                        totals.Item().BorderTop(1).BorderColor(Colors.Grey.Darken1).PaddingTop(2).Row(r =>
                        {
                            r.RelativeItem().Text("ยอดรวมทั้งสิ้น (Gross Total):").Bold().FontSize(9.5f).FontColor(Navy);
                            r.ConstantItem(80).AlignRight().Text(Money(model.Billing.GrossTotal)).Bold().FontSize(9.5f).FontColor(Navy);
                        });
                        if (model.Billing.RetentionStatus == RetentionHoldingStatus.Held)
                        {
                            totals.Item().Row(r =>
                            {
                                r.RelativeItem().Text("หัก เงินประกันผลงาน (Held):").FontSize(8.5f);
                                r.ConstantItem(80).AlignRight().Text($"-{Money(model.Billing.RetentionAmount)}").FontSize(8.5f);
                            });
                            totals.Item().Row(r =>
                            {
                                r.RelativeItem().Text("ยอดเรียกเก็บจริง (Receivable):").Bold().FontSize(9f);
                                r.ConstantItem(80).AlignRight().Text(Money(model.Billing.EffectiveReceivable)).Bold().FontSize(9f);
                            });
                        }
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text("ชำระแล้ว (Paid):").FontSize(8.5f);
                            r.ConstantItem(80).AlignRight().Text(Money(model.Billing.PaidAmount)).FontSize(8.5f);
                        });
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text("ยอดคงเหลือ (Outstanding):").Bold().FontSize(9f).FontColor(Navy);
                            r.ConstantItem(80).AlignRight().Text(Money(model.Billing.Outstanding)).Bold().FontSize(9f).FontColor(Navy);
                        });
                    });
                });

                // Recorded Payments / Receipts / WHT Section
                if (model.Payments.Any(p => p.Status == PaymentStatus.Recorded))
                {
                    col.Item().PaddingTop(8).Column(payCol =>
                    {
                        payCol.Item().Text("ประวัติการรับชำระ / ใบเสร็จรับเงิน (Payment Receipts & WHT 50-Bis)").Bold().FontSize(9).FontColor(Navy);
                        payCol.Item().Table(pTable =>
                        {
                            pTable.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(100);
                                cols.ConstantColumn(70);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1.5f);
                                cols.RelativeColumn(1.5f);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1.5f);
                            });

                            pTable.Header(pHeader =>
                            {
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("เลขที่ใบเสร็จ").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("วันที่รับ").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("วิธี / อ้างอิง").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("ยอดชำระ").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("หัก ณ ที่จ่าย").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("เลข 50 ทวิ").Bold().FontSize(8f);
                                pHeader.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("รับสุทธิ").Bold().FontSize(8f);
                            });

                            foreach (var pay in model.Payments.Where(p => p.Status == PaymentStatus.Recorded))
                            {
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(pay.ReceiptNumber ?? pay.Number).FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(pay.ReceivedDate.ToString("dd/MM/yyyy", Invariant)).FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text($"{pay.Method} ({pay.Reference})").FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(Money(pay.Amount)).FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(Money(pay.WhtAmount)).FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(pay.WhtCertificateNumber ?? "-").FontSize(8f);
                                pTable.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(Money(pay.NetCashReceived)).FontSize(8f);
                            }
                        });
                    });
                }
            });

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
            Title = model.Billing.Number,
            Author = model.Organization.Name,
            Creator = "tan-erp",
            Language = "th-TH",
            CreationDate = model.Billing.IssuedAtUtc,
            ModifiedDate = model.Billing.IssuedAtUtc
        });

        return document.GeneratePdf();
    }
}
