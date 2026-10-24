namespace TanErp.Domain.DocumentTemplates;

public sealed record DocumentTemplateLayoutConfig(
    HeaderConfig Header,
    ColumnsConfig Columns,
    SummaryConfig Summary,
    TermsConfig Terms,
    SignaturesConfig Signatures)
{
    public static DocumentTemplateLayoutConfig DefaultFor(string documentType)
    {
        var normalized = (documentType ?? string.Empty).Trim().ToLowerInvariant();
        return new DocumentTemplateLayoutConfig(
            new HeaderConfig(
                ShowLogo: true,
                LogoPosition: "left",
                LogoHeight: "42px",
                CompanyNameTh: null,
                CompanyNameEn: null,
                TaxId: null,
                Address: null,
                Phone: null,
                Email: null,
                DocumentTitle: GetDefaultTitle(normalized)),
            new ColumnsConfig(
                ShowSku: true,
                ShowSpecs: true,
                ShowThumbnail: false,
                ShowLineDiscount: false),
            new SummaryConfig(
                ShowVat: true,
                ShowWithholdingTax: false,
                ShowBahtText: true),
            new TermsConfig(
                ValidityPeriodText: "ราคาในเอกสารนี้ยืนราคาภายใน 30 วัน นับจากวันที่ออกเอกสาร",
                CommercialTermsText: "1. ชำระมัดจำงวดแรก 50% ณ วันลงนามสั่งจ้าง\n2. ชำระงวดที่สอง 40% เมื่อส่งมอบสินค้า/วัสดุเข้าหน้างาน\n3. ชำระงวดสุดท้าย 10% เมื่อตรวจสอบและส่งมอบงานเรียบร้อย",
                ShowBankDetails: true,
                BankName: null,
                AccountNo: null,
                AccountName: null,
                FooterNote: "เอกสารนี้สร้างและรับรองความถูกต้องด้วยระบบอิเล็กทรอนิกส์ (tan-erp)"),
            new SignaturesConfig(
                SignatureCount: 2,
                Box1Label: "ผู้จัดทำเอกสาร (Prepared By)",
                Box2Label: "ผู้มีอำนาจลงนาม (Authorized Signatory)",
                Box3Label: "การยอมรับของลูกค้า (Customer Acceptance)"));
    }

    private static string GetDefaultTitle(string docType) => docType switch
    {
        "quotations" => "ใบเสนอราคา / QUOTATION",
        "estimates" => "ใบประมาณการราคาและถอดแบบ BOQ / ESTIMATE",
        "purchase-orders" => "ใบสั่งซื้อสินค้า / PURCHASE ORDER",
        "billing-documents" => "ใบแจ้งหนี้ / ใบวางบิล / BILLING",
        "goods-receipts" => "ใบรับสินค้าและตรวจรับพัสดุ / GOODS RECEIPT",
        _ => "เอกสาร / DOCUMENT"
    };
}

public sealed record HeaderConfig(
    bool ShowLogo,
    string LogoPosition,
    string LogoHeight,
    string? CompanyNameTh,
    string? CompanyNameEn,
    string? TaxId,
    string? Address,
    string? Phone,
    string? Email,
    string DocumentTitle);

public sealed record ColumnsConfig(
    bool ShowSku,
    bool ShowSpecs,
    bool ShowThumbnail,
    bool ShowLineDiscount);

public sealed record SummaryConfig(
    bool ShowVat,
    bool ShowWithholdingTax,
    bool ShowBahtText);

public sealed record TermsConfig(
    string? ValidityPeriodText,
    string? CommercialTermsText,
    bool ShowBankDetails,
    string? BankName,
    string? AccountNo,
    string? AccountName,
    string? FooterNote);

public sealed record SignaturesConfig(
    int SignatureCount,
    string Box1Label,
    string? Box2Label,
    string? Box3Label);
