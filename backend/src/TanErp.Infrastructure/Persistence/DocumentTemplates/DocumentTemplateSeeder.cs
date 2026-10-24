using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Domain.DocumentTemplates;

namespace TanErp.Infrastructure.Persistence.DocumentTemplates;

public static class DocumentTemplateSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static readonly Guid DemoQuotationTemplateHqId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5001");
    public static readonly Guid DemoQuotationTemplateProjectId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5002");
    public static readonly Guid DemoEstimateTemplateId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5003");
    public static readonly Guid DemoPurchaseOrderTemplateId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5004");
    public static readonly Guid DemoBillingTemplateId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5005");
    public static readonly Guid DemoGoodsReceiptTemplateId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5006");

    public static async Task SeedAsync(AppDbContext db, Guid organizationId, Guid? branchId, DateTimeOffset now, CancellationToken ct = default)
    {
        // 1. Quotation - Standard HQ (Default)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoQuotationTemplateHqId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("quotations");
            var hqLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    CompanyNameTh = "บริษัท แทน คอนสตรัคชั่น แอนด์ ดีไซน์ จำกัด (สำนักงานใหญ่)",
                    CompanyNameEn = "Tan Construction & Design Co., Ltd. (Headquarters)",
                    Address = "123/45 ถนนสุขุมวิท แขวงคลองเตย เขตคลองเตย กรุงเทพมหานคร 10110",
                    TaxId = "0105560000000",
                    Phone = "02-123-4567, 081-999-8888",
                    Email = "contact@tan-erp.com",
                    DocumentTitle = "ใบเสนอราคา / QUOTATION",
                },
                Columns = baseConfig.Columns with
                {
                    ShowThumbnail = true,
                    ShowLineDiscount = true,
                },
                Terms = baseConfig.Terms with
                {
                    ValidityPeriodText = "กำหนดยืนราคา 30 วัน นับจากวันที่ออกเอกสาร",
                    CommercialTermsText = "1. งวดที่ 1: ชำระมัดจำ 40% เมื่อยืนยันการสั่งจ้าง\n2. งวดที่ 2: ชำระ 40% เมื่อส่งมอบโครงสร้างหน้างาน\n3. งวดที่ 3: ชำระ 20% เมื่อส่งมอบงานงวดสุดท้ายและตรวจรับเรียบร้อย",
                    ShowBankDetails = true,
                    BankName = "ธนาคารกสิกรไทย (KBANK)",
                    AccountNo = "123-2-34567-8",
                    AccountName = "บจก. แทน คอนสตรัคชั่น แอนด์ ดีไซน์",
                    FooterNote = "ราคานี้รวมค่าขนส่งและบริการติดตั้งเรียบร้อยแล้ว รับประกันงานโครงสร้างและอุปกรณ์ฟิตติ้ง 1 ปีเต็ม",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 3,
                    Box1Label = "ผู้สั่งซื้อ / ลูกค้า",
                    Box2Label = "พนักงานขาย / ผู้เสนอราคา",
                    Box3Label = "ผู้อนุมัติ / ผู้มีอำนาจลงนาม",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoQuotationTemplateHqId,
                organizationId,
                branchId,
                "quotation",
                "ใบเสนอราคามาตรฐาน (สำนักงานใหญ่)",
                isDefault: true,
                JsonSerializer.Serialize(hqLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        // 2. Quotation - Commercial Fast-Track Project (Alternative)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoQuotationTemplateProjectId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("quotations");
            var projectLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    DocumentTitle = "ใบเสนอราคาโครงการ / PROJECT QUOTATION",
                    LogoPosition = "right",
                    LogoHeight = "60px",
                },
                Columns = baseConfig.Columns with
                {
                    ShowThumbnail = false,
                    ShowLineDiscount = true,
                },
                Terms = baseConfig.Terms with
                {
                    ValidityPeriodText = "กำหนดยืนราคา 15 วัน นับจากวันที่ออกเอกสาร",
                    CommercialTermsText = "เครดิตเทอม 30 วันหลังส่งมอบงานและวางบิลตามเงื่อนไขสัญญาโครงการ",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 2,
                    Box1Label = "ผู้ว่าจ้าง / ผู้สั่งซื้อ",
                    Box2Label = "ผู้เสนอราคา / ฝ่ายขายโครงการ",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoQuotationTemplateProjectId,
                organizationId,
                branchId,
                "quotation",
                "ใบเสนอราคางานโครงการด่วน (Commercial Project)",
                isDefault: false,
                JsonSerializer.Serialize(projectLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        // 3. Estimate BOQ - Standard (Default)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoEstimateTemplateId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("estimates");
            var estimateLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    DocumentTitle = "ใบประมาณราคาและบัญชีแสดงปริมาณวัสดุ (BOQ)",
                },
                Terms = baseConfig.Terms with
                {
                    ValidityPeriodText = "ราคาประเมินมีผลบังคับใช้ 45 วัน",
                    CommercialTermsText = "ปริมาณงานและราคาอ้างอิงจากการสำรวจพื้นที่หน้างานจริงและแบบสถาปัตยกรรมฉบับล่าสุด",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 3,
                    Box1Label = "ผู้สำรวจหน้างาน",
                    Box2Label = "ผู้จัดทำ BOQ",
                    Box3Label = "วิศวกร / หัวหน้าฝ่ายประมาณราคา",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoEstimateTemplateId,
                organizationId,
                branchId,
                "estimate",
                "ใบประมาณราคา BOQ มาตรฐาน",
                isDefault: true,
                JsonSerializer.Serialize(estimateLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        // 4. Purchase Order - Standard (Default)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoPurchaseOrderTemplateId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("purchase-orders");
            var poLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    DocumentTitle = "ใบสั่งซื้อวัสดุและอุปกรณ์ / PURCHASE ORDER",
                },
                Terms = baseConfig.Terms with
                {
                    ValidityPeriodText = "กรุณายืนยันการสั่งซื้อภายใน 3 วันทำการ",
                    CommercialTermsText = "1. ส่งมอบสินค้าตามรายการ ณ คลังสินค้าสาขาหลัก ภายในเวลาทำการ\n2. เครดิตการชำระเงิน 30 วัน นับจากวันส่งมอบสินค้าและได้รับใบกำกับภาษีถูกต้อง",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 2,
                    Box1Label = "เจ้าหน้าที่ฝ่ายจัดซื้อ",
                    Box2Label = "ผู้จัดการฝ่ายจัดซื้อ / ผู้มีอำนาจสั่งซื้อ",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoPurchaseOrderTemplateId,
                organizationId,
                branchId,
                "purchase_order",
                "ใบสั่งซื้อสินค้าและวัสดุมาตรฐาน",
                isDefault: true,
                JsonSerializer.Serialize(poLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        // 5. Billing / Invoice - Standard (Default)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoBillingTemplateId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("billing-documents");
            var billingLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    DocumentTitle = "ใบแจ้งหนี้และใบวางบิล / INVOICE & BILLING",
                },
                Terms = baseConfig.Terms with
                {
                    ValidityPeriodText = "กรุณาชำระเงินภายในวันที่ครบกำหนด",
                    CommercialTermsText = "ชำระเงินด้วยการโอนเงินเข้าบัญชี หรือสั่งจ่ายเช็คขีดคร่อมในนาม บจก. แทน คอนสตรัคชั่น แอนด์ ดีไซน์",
                    ShowBankDetails = true,
                    BankName = "ธนาคารกสิกรไทย (KBANK) สาขาสุขุมวิท",
                    AccountNo = "123-2-34567-8",
                    AccountName = "บจก. แทน คอนสตรัคชั่น แอนด์ ดีไซน์",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 2,
                    Box1Label = "ผู้รับวางบิล / ลูกค้า",
                    Box2Label = "เจ้าหน้าที่การเงินและบัญชี",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoBillingTemplateId,
                organizationId,
                branchId,
                "billing",
                "ใบแจ้งหนี้และใบวางบิลมาตรฐาน",
                isDefault: true,
                JsonSerializer.Serialize(billingLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        // 6. Goods Receipt - Standard (Default)
        if (!await db.DocumentTemplates.AnyAsync(t => t.Id == DemoGoodsReceiptTemplateId, ct))
        {
            var baseConfig = DocumentTemplateLayoutConfig.DefaultFor("goods-receipts");
            var grLayout = baseConfig with
            {
                Header = baseConfig.Header with
                {
                    DocumentTitle = "ใบรับสินค้าและตรวจรับพัสดุ / GOODS RECEIPT NOTE",
                },
                Terms = baseConfig.Terms with
                {
                    CommercialTermsText = "สินค้าและพัสดุได้รับการตรวจนับและตรวจสอบสภาพความสมบูรณ์เรียบร้อยถูกต้องตามใบสั่งซื้อ",
                },
                Signatures = baseConfig.Signatures with
                {
                    SignatureCount = 3,
                    Box1Label = "ผู้ส่งมอบสินค้า / ผู้ขาย",
                    Box2Label = "ผู้รับพัสดุ / เจ้าหน้าที่คลังสินค้า",
                    Box3Label = "ผู้จัดการฝ่ายคลังสินค้า",
                }
            };

            var tmpl = new DocumentTemplate(
                DemoGoodsReceiptTemplateId,
                organizationId,
                branchId,
                "goods_receipt",
                "ใบรับสินค้าและตรวจรับพัสดุมาตรฐาน",
                isDefault: true,
                JsonSerializer.Serialize(grLayout, JsonOptions),
                logoFileId: null,
                now);

            db.DocumentTemplates.Add(tmpl);
        }

        await db.SaveChangesAsync(ct);
    }
}
