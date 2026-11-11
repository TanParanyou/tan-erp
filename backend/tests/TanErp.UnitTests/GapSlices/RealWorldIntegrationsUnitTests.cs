using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Finance;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.DocumentTemplates;
using TanErp.Domain.Finance;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Documents;
using TanErp.Infrastructure.Email;
using TanErp.Infrastructure.Persistence.Finance;
using TanErp.Infrastructure.Persistence.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.GapSlices;

public class RealWorldIntegrationsUnitTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 5, 8, 30, 0, TimeSpan.Zero);
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    // ===== 1. ConfigurableAccountingConnector & Multi-Format CSV Export (G-17) =======================

    [Fact]
    public async Task ConfigurableAccountingConnector_WhenDisabled_ReturnsConnectorNotConfigured()
    {
        var connector = new ConfigurableAccountingConnector(new AccountingConnectorOptions { Enabled = false });
        var envelope = new AccountingEnvelope(Guid.NewGuid(), "billing.issued:1", OutboxKind.BillingIssued, "BIL-2026-0001", 50_000m, "{}", 1);

        var result = await connector.SendAsync(envelope);

        Assert.False(result.Success);
        Assert.Null(result.ExternalRef);
        Assert.Equal("CONNECTOR_NOT_CONFIGURED", result.Error);
    }

    [Fact]
    public async Task ConfigurableAccountingConnector_LocalMode_GeneratesDeterministicExternalReference()
    {
        var connector = new ConfigurableAccountingConnector(new AccountingConnectorOptions
        {
            Enabled = true,
            Provider = "local"
        });
        var envelope = new AccountingEnvelope(
            Guid.NewGuid(),
            "billing.issued:abcdef123456",
            OutboxKind.BillingIssued,
            "BIL-2026-0009",
            125_000m,
            "{}",
            1);

        var result = await connector.SendAsync(envelope);

        Assert.True(result.Success);
        Assert.Equal("LOCAL-BIL-2026-0009-billing.", result.ExternalRef);
        Assert.Null(result.Error);
    }

    [Theory]
    [InlineData("peak", "Client-Token", "PEAK-DOC-99")]
    [InlineData("flowaccount", "Authorization", "FA-DOC-88")]
    [InlineData("webhook", "X-Api-Key", "WH-DOC-77")]
    public async Task ConfigurableAccountingConnector_HttpProviders_SendPayloadHeadersAndHmacSignature(
        string provider,
        string expectedAuthHeader,
        string remoteDocumentId)
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;

        var handler = new StubHttpMessageHandler(async (req, ct) =>
        {
            capturedRequest = req;
            capturedBody = req.Content is null ? null : await req.Content.ReadAsStringAsync(ct);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { externalRef = remoteDocumentId }),
                    Encoding.UTF8,
                    "application/json")
            };
        });

        using var httpClient = new HttpClient(handler);
        var options = new AccountingConnectorOptions
        {
            Enabled = true,
            Provider = provider,
            EndpointUrl = $"https://accounting.example.com/api/{provider}",
            ApiKey = "secret-api-key-123",
            ClientId = "tan-erp-client",
            ClientSecret = "hmac-signing-secret"
        };

        var connector = new ConfigurableAccountingConnector(
            options,
            httpClient: httpClient,
            logger: NullLogger<ConfigurableAccountingConnector>.Instance);

        var envelope = new AccountingEnvelope(
            Guid.NewGuid(),
            "billing.issued:001",
            OutboxKind.BillingIssued,
            "BIL-2026-0042",
            250_000.50m,
            "{\"number\":\"BIL-2026-0042\"}",
            1,
            OrgId,
            Guid.NewGuid(),
            new DateOnly(2026, 11, 5));

        var result = await connector.SendAsync(envelope);

        Assert.True(result.Success);
        Assert.Equal(remoteDocumentId, result.ExternalRef);
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedBody);
        Assert.True(capturedRequest!.Headers.Contains(expectedAuthHeader));
        Assert.True(capturedRequest.Headers.TryGetValues("Idempotency-Key", out var idemValues));
        Assert.Equal("billing.issued:001", Assert.Single(idemValues!));

        Assert.True(capturedRequest.Headers.TryGetValues("X-TanErp-Signature", out var sigValues));
        var expectedSig = $"sha256={ConfigurableAccountingConnector.ComputeHmacSha256Hex("hmac-signing-secret", capturedBody!)}";
        Assert.Equal(expectedSig, Assert.Single(sigValues!));
    }

    [Fact]
    public void FinanceStore_BuildJournalCsv_SupportsStandardPeakFlowAccountAndExpressFormats()
    {
        var lines = new List<JournalExportLineProjection>
        {
            new(
                "JRN-20261105-ABCD123",
                Guid.NewGuid(),
                "billing.issued:1",
                OutboxKind.BillingIssued,
                "BIL-2026-0001",
                "113100",
                "ลูกหนี้การค้า - ในประเทศ",
                "411100",
                "รายได้ตามสัญญาโครงการ",
                107_000m,
                "THB",
                Now)
        };

        var standardCsv = FinanceStore.BuildJournalCsv(lines, "standard");
        Assert.Contains("BatchRef,DateUtc,DedupeKey,Kind,DocumentNumber,DebitAccountCode,DebitAccountName,CreditAccountCode,CreditAccountName,Amount,Currency", standardCsv);
        Assert.Contains("BIL-2026-0001,113100,ลูกหนี้การค้า - ในประเทศ,411100,รายได้ตามสัญญาโครงการ,107000.00,THB", standardCsv);

        var peakCsv = FinanceStore.BuildJournalCsv(lines, "peak");
        Assert.Contains("Date,DocNo,AccountCode,AccountName,Debit,Credit,Description,Reference", peakCsv);
        Assert.Contains("2026-11-05,JRN-20261105-ABCD123,113100,ลูกหนี้การค้า - ในประเทศ,107000.00,0.00,billing.issued BIL-2026-0001,BIL-2026-0001", peakCsv);
        Assert.Contains("2026-11-05,JRN-20261105-ABCD123,411100,รายได้ตามสัญญาโครงการ,0.00,107000.00,billing.issued BIL-2026-0001,BIL-2026-0001", peakCsv);

        var flowAccountCsv = FinanceStore.BuildJournalCsv(lines, "flowaccount");
        Assert.Contains("DocumentDate,DocumentNumber,AccountCode,AccountName,DebitAmount,CreditAmount,Remarks", flowAccountCsv);
        Assert.Contains("2026-11-05,BIL-2026-0001,113100,ลูกหนี้การค้า - ในประเทศ,107000.00,0.00", flowAccountCsv);
        Assert.Contains("2026-11-05,BIL-2026-0001,411100,รายได้ตามสัญญาโครงการ,0.00,107000.00", flowAccountCsv);

        var expressCsv = FinanceStore.BuildJournalCsv(lines, "express");
        Assert.Contains("VOUCHER_NO,VOUCHER_DATE,ACCT_CODE,ACCT_NAME,DR_AMT,CR_AMT,REMARK", expressCsv);
        Assert.Contains("JRN-20261105-ABCD123,2026-11-05,113100,ลูกหนี้การค้า - ในประเทศ,107000.00,0.00,billing.issued:BIL-2026-0001", expressCsv);
        Assert.Contains("JRN-20261105-ABCD123,2026-11-05,411100,รายได้ตามสัญญาโครงการ,0.00,107000.00,billing.issued:BIL-2026-0001", expressCsv);

        // Verify all StandardThaiCoaPresets have valid OutboxKind and account codes
        Assert.All(FinanceStore.StandardThaiCoaPresets, preset =>
        {
            Assert.Contains(preset.Kind, OutboxKind.All);
            var mapping = new AccountMapping(Guid.NewGuid(), OrgId, preset.Kind, preset.DebitCode, preset.DebitName, preset.CreditCode, preset.CreditName, UserId, Now);
            Assert.Equal(preset.DebitCode, mapping.DebitAccountCode);
            Assert.Equal(preset.CreditCode, mapping.CreditAccountCode);
        });
    }

    // ===== 2. ETaxInvoiceXmlBuilder (G-15b / G-26) ===================================================

    [Fact]
    public void ETaxInvoiceXmlBuilder_PopulatesBuyerPostalTradeAddress_ReferencedNotes_AndSignsXmlWithRsaKey()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "บริษัท แทน อีอาร์พี จำกัด", true, Now);
        org.UpdateProfile("บริษัท แทน อีอาร์พี จำกัด", "Tan ERP Co., Ltd.", "0105546148739", "99 ถนนสาทร แขวงยานนาวา เขตสาทร กรุงเทพฯ 10120", "99 Sathorn Rd", "021234567");
        var branch = Branch.Create(BranchId, OrgId, "HQ", "สำนักงานใหญ่", "Head Office", "00000", "99 ถนนสาทร แขวงยานนาวา เขตสาทร กรุงเทพฯ 10120", null, "021234567", Now);
        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            OrgId,
            UserId,
            "organization",
            "บริษัท สยาม คอนสตรัคชั่น จำกัด",
            "Siam Construction Co., Ltd.",
            "th",
            new PrimaryContactInput("คุณสมชาย", "ผู้จัดการ", "0812345678", "somchai@siam.co.th", "phone"),
            Now);
        customer.UpdateCommercialProfile("บริษัท สยาม คอนสตรัคชั่น จำกัด (มหาชน)", "1100400000005", "00000", 30, 1_000_000m, "THB", null, null, null);

        var buyerAddress = new CustomerAddress(
            Guid.NewGuid(),
            customer.Id,
            OrgId,
            UserId,
            "billing",
            "สำนักงานใหญ่",
            "888 ถนนสุขุมวิท",
            "คลองเตยเหนือ",
            "วัฒนา",
            "กรุงเทพมหานคร",
            "10110",
            "TH",
            true,
            Now);
        customer.AddAddress(buyerAddress);

        var project = Project.CreateFromHandover(
            Guid.NewGuid(),
            OrgId,
            BranchId,
            customer.Id,
            null,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "PRJ-2026-0001",
            "โครงการตกแต่งสำนักงานใหญ่",
            UserId,
            new DateOnly(2026, 11, 1),
            "QT-2026-0001",
            500_000m,
            "hash-q",
            null,
            "hash-base",
            UserId,
            Now);

        var billingId = Guid.NewGuid();
        var line = new BillingLine(Guid.NewGuid(), OrgId, billingId, 1, "งานติดตั้งระบบไฟฟ้าและตกแต่งภายใน", 100_000m, 0.07m);
        var billing = new BillingDocument(
            billingId,
            OrgId,
            BranchId,
            project.Id,
            "BIL-2026-0001",
            BillingKind.Milestone,
            "งวดที่ 1",
            100_000m,
            new DateOnly(2026, 12, 5),
            UserId,
            Now,
            0.07m,
            0m,
            "TIV-2026-0001",
            [line]);

        var approverId = Guid.NewGuid();
        var creditNote = new BillingAdjustmentNote(
            Guid.NewGuid(),
            OrgId,
            BranchId,
            billing.Id,
            "CN-2026-0001",
            BillingNoteType.Credit,
            "ส่วนลดพิเศษตามข้อตกลง",
            10_000m,
            0.07m,
            UserId,
            Now);
        creditNote.Approve(approverId, Now.AddMinutes(10));
        billing.ApplyApprovedNote(creditNote, Now.AddMinutes(10));

        var debitNote = new BillingAdjustmentNote(
            Guid.NewGuid(),
            OrgId,
            BranchId,
            billing.Id,
            "DN-2026-0001",
            BillingNoteType.Debit,
            "งานเพิ่มจุดปลั๊กไฟ",
            5_000m,
            0.07m,
            UserId,
            Now);
        debitNote.Approve(approverId, Now.AddMinutes(20));
        billing.ApplyApprovedNote(debitNote, Now.AddMinutes(20));

        var model = new TaxDocumentPdfModel(
            org,
            branch,
            project,
            customer,
            billing,
            [line],
            [creditNote, debitNote],
            []);

        var xml = ETaxInvoiceXmlBuilder.Build(model);
        var doc = XDocument.Parse(xml);
        XNamespace ram = "urn:etda:uncefact:data:standard:TaxInvoice_ReusableAggregateBusinessInformationEntity:2";

        // 1. Verify Buyer PostalTradeAddress is populated
        var buyerParty = doc.Descendants(ram + "BuyerTradeParty").Single();
        var postalAddr = buyerParty.Element(ram + "PostalTradeAddress");
        Assert.NotNull(postalAddr);
        Assert.Equal("888 ถนนสุขุมวิท", postalAddr!.Element(ram + "LineOne")?.Value);
        Assert.Equal("คลองเตยเหนือ", postalAddr.Element(ram + "CitySubDivisionName")?.Value);
        Assert.Equal("วัฒนา", postalAddr.Element(ram + "CityName")?.Value);
        Assert.Equal("กรุงเทพมหานคร", postalAddr.Element(ram + "CountrySubDivisionName")?.Value);
        Assert.Equal("10110", postalAddr.Element(ram + "PostcodeCode")?.Value);

        // 2. Verify Approved Credit/Debit Notes are referenced
        var refDocs = doc.Descendants(ram + "AdditionalReferencedDocument").ToList();
        Assert.Equal(2, refDocs.Count);
        Assert.Contains(refDocs, r =>
            r.Element(ram + "IssuerAssignedID")?.Value == "CN-2026-0001" &&
            r.Element(ram + "TypeCode")?.Value == "81" &&
            r.Element(ram + "Name")?.Value == "ส่วนลดพิเศษตามข้อตกลง");
        Assert.Contains(refDocs, r =>
            r.Element(ram + "IssuerAssignedID")?.Value == "DN-2026-0001" &&
            r.Element(ram + "TypeCode")?.Value == "80" &&
            r.Element(ram + "Name")?.Value == "งานเพิ่มจุดปลั๊กไฟ");

        // 3. Verify XML Digital Signature with RSA key
        using var rsa = RSA.Create(2048);
        var signedXml = ETaxInvoiceXmlBuilder.SignXml(xml, rsa);
        XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
        var signedDoc = XDocument.Parse(signedXml, LoadOptions.PreserveWhitespace);
        Assert.NotNull(signedDoc.Root?.Element(ds + "Signature"));
        Assert.True(ETaxInvoiceXmlBuilder.VerifyXmlSignature(signedXml, rsa));
    }

    // ===== 3. HttpSmsSender & SmsOptions (G-05) ======================================================

    [Fact]
    public async Task HttpSmsSender_SupportsThaiBulkSms_Twilio_AndWebhook()
    {
        var disabledSender = new LoggingSmsSender(NullLogger<LoggingSmsSender>.Instance);
        Assert.False(disabledSender.IsEnabled);

        HttpRequestMessage? lastRequest = null;
        string? lastBody = null;
        var handler = new StubHttpMessageHandler(async (req, ct) =>
        {
            lastRequest = req;
            lastBody = req.Content is null ? null : await req.Content.ReadAsStringAsync(ct);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"status\":\"queued\"}", Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler);

        // 1. ThaiBulkSMS
        var thaiBulkOptions = new SmsOptions
        {
            Enabled = true,
            Provider = "thaibulksms",
            ApiKey = "tbs-key",
            ApiSecret = "tbs-secret",
            SenderName = "TAN-ERP"
        };
        thaiBulkOptions.Validate();
        var thaiBulkSender = new HttpSmsSender(thaiBulkOptions, httpClient: httpClient);
        Assert.True(thaiBulkSender.IsEnabled);
        await thaiBulkSender.SendAsync(new SmsMessage("0812345678", "OTP ของคุณคือ 123456"));

        Assert.NotNull(lastRequest);
        Assert.Equal("https://api-v2.thaibulksms.com/sms", lastRequest!.RequestUri!.ToString());
        Assert.Equal("Basic", lastRequest.Headers.Authorization?.Scheme);
        Assert.Contains("msisdn=0812345678", lastBody);
        Assert.Contains("sender=TAN-ERP", lastBody);

        // 2. Webhook
        var webhookOptions = new SmsOptions
        {
            Enabled = true,
            Provider = "webhook",
            EndpointUrl = "https://sms.example.com/webhook",
            ApiKey = "wh-secret-token",
            SenderName = "TAN-ERP"
        };
        webhookOptions.Validate();
        var webhookSender = new HttpSmsSender(webhookOptions, httpClient: httpClient);
        await webhookSender.SendAsync(new SmsMessage("0899998888", "รหัส OTP: 654321"));

        Assert.Equal("https://sms.example.com/webhook", lastRequest.RequestUri!.ToString());
        Assert.True(lastRequest.Headers.TryGetValues("X-Api-Key", out var apiKeyHeader));
        Assert.Equal("wh-secret-token", Assert.Single(apiKeyHeader!));
        Assert.Contains("\"to\":\"0899998888\"", lastBody);
        Assert.Contains("\"sender\":\"TAN-ERP\"", lastBody);
    }

    // ===== 4. Default Thai ERP Approval Limits Preset (G-03c) ========================================

    [Theory]
    [InlineData("System Administrator", true, null)]
    [InlineData("กรรมการผู้จัดการ (Managing Director)", true, null)]
    [InlineData("ผู้บริหารระดับสูง (Executive)", true, null)]
    [InlineData("ผู้จัดการฝ่ายประเมินราคา (Estimation Manager)", false, 1_000_000)]
    [InlineData("หัวหน้าวิศวกรโครงการ (Project Lead)", false, 1_000_000)]
    [InlineData("เจ้าหน้าที่จัดซื้อ (Procurement Officer)", false, 100_000)]
    [InlineData("วิศวกรสนาม (Site Engineer)", false, 100_000)]
    public void ApprovalLimitStore_ResolveDefaultTier_ClassifiesThaiAndEnglishRolesCorrectly(
        string roleName,
        bool expectedUnlimited,
        int? expectedMaxAmount)
    {
        var (isUnlimited, maxAmount) = ApprovalLimitStore.ResolveDefaultTier(roleName);
        Assert.Equal(expectedUnlimited, isUnlimited);
        Assert.Equal(expectedMaxAmount.HasValue ? (decimal?)expectedMaxAmount.Value : null, maxAmount);
    }

    [Fact]
    public async Task ApprovalLimitHandler_SeedDefaultsAsync_RequiresPermission_AndReturnsSeededPage()
    {
        var resolver = new StubAccessResolver();
        var store = new StubApprovalLimitStore();
        var handler = new ApprovalLimitHandler(resolver, store);

        var result = await handler.SeedDefaultsAsync(new AdminCaller("uid-admin", Guid.NewGuid()), "trace-seed-1");

        Assert.True(result.IsSuccess);
        Assert.Equal(1, store.SeedCallCount);
        Assert.Equal(ApprovalDocumentType.Enforced.Count, result.Value!.Items.Count);
    }

    // ===== 5. PDF Branding & Template Layout Integration =============================================

    private static readonly byte[] ValidOneByOnePng =
    [
        137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82,
        0, 0, 0, 1, 0, 0, 0, 1, 8, 6, 0, 0, 0, 31, 21, 196, 137,
        0, 0, 0, 10, 73, 68, 65, 84, 120, 156, 99, 0, 1, 0, 0, 5,
        0, 1, 13, 10, 45, 180, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66,
        96, 130
    ];

    [Fact]
    public void QuestPdfTaxDocumentRenderer_WithBrandColorAndLogo_RendersValidPdf()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "บริษัท แทน อีอาร์พี จำกัด", true, Now);
        org.UpdateProfile("บริษัท แทน อีอาร์พี จำกัด", "Tan ERP Co., Ltd.", "0105546148739", "99 ถนนสาทร กรุงเทพฯ", "99 Sathorn Rd", "021234567");
        org.UpdateBranding("#1E3A8A", Guid.NewGuid());

        var branch = Branch.Create(BranchId, OrgId, "HQ", "สำนักงานใหญ่", "Head Office", "00000", "99 ถนนสาทร กรุงเทพฯ", null, "021234567", Now);
        var project = Project.CreateFromHandover(
            Guid.NewGuid(), OrgId, BranchId, Guid.NewGuid(), null,
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null,
            "PRJ-2026-0001", "โครงการตกแต่งสำนักงานใหญ่", UserId,
            new DateOnly(2026, 11, 1), "QT-2026-0001", 500_000m,
            "hash-q", null, "hash-base", UserId, Now);

        var billingId = Guid.NewGuid();
        var line = new BillingLine(Guid.NewGuid(), OrgId, billingId, 1, "งานติดตั้งระบบไฟฟ้า", 100_000m, 0.07m);
        var billing = new BillingDocument(
            billingId, OrgId, BranchId, project.Id, "BIL-2026-0001",
            BillingKind.Milestone, "งวดที่ 1", 100_000m, new DateOnly(2026, 12, 5),
            UserId, Now, 0.07m, 0.05m, "TIV-2026-0001", [line]);

        var model = new TaxDocumentPdfModel(org, branch, project, null, billing, [line], [], [], ValidOneByOnePng);

        var pdf = QuestPdfTaxDocumentRenderer.Render(model);

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 1000);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(pdf, 0, 5));
    }

    [Fact]
    public void QuestPdfQuotationRenderer_WithDocumentTemplateLayoutConfig_RendersValidPdf()
    {
        var model = new QuotationPdfModel(
            "QT-2026-TPL",
            Now,
            "THB",
            null,
            new QuotationPdfIssuer(
                "บริษัท แทน อีอาร์พี จำกัด", "Tan ERP Co., Ltd.", "สำนักงานใหญ่", "Head Office",
                "0105546148739", "00000", "99 ถนนสาทร กรุงเทพฯ", "99 Sathorn Rd, Bangkok", "02-123-4567",
                ValidOneByOnePng),
            new QuotationPdfCustomer(
                "บริษัท ลูกค้า จำกัด", "Customer Co., Ltd.", "บริษัท ลูกค้า จำกัด (มหาชน)",
                "0105536000002", "00000",
                new QuotationPdfAddress("888 ถนนสุขุมวิท", "คลองเตยเหนือ", "วัฒนา", "กรุงเทพมหานคร", "10110")),
            [
                new QuotationPdfSection("S1", "หมวดงานโครงสร้าง", "Structural Work", 100_000m,
                [
                    new QuotationPdfWorkItem("W001", "งานติดตั้งโครงเหล็ก", "Steel structure installation", 1m, "งาน", 100_000m, 100_000m)
                ])
            ],
            new QuotationPdfTotals(100_000m, 0m, 100_000m, 7_000m, 107_000m));

        var baseLayout = DocumentTemplateLayoutConfig.DefaultFor("quotations");
        var layoutConfig = baseLayout with
        {
            Header = baseLayout.Header with
            {
                DocumentTitle = "ใบเสนอราคาโครงการพิเศษ / SPECIAL QUOTATION",
                ShowLogo = true,
                LogoPosition = "right"
            },
            Terms = baseLayout.Terms with
            {
                ValidityPeriodText = "ยืนราคา 45 วัน",
                CommercialTermsText = "แบ่งชำระ 3 งวดตามความก้าวหน้าของงาน",
                ShowBankDetails = true,
                BankName = "ธนาคารกสิกรไทย",
                AccountNo = "123-4-56789-0",
                AccountName = "บจก. แทน อีอาร์พี"
            },
            Signatures = baseLayout.Signatures with
            {
                SignatureCount = 3,
                Box1Label = "ผู้เสนอราคา",
                Box2Label = "ผู้ตรวจสอบ",
                Box3Label = "ผู้อนุมัติสั่งจ้าง"
            }
        };

        var renderer = new QuestPdfQuotationRenderer(new StubConfiguration(), NullLogger<QuestPdfQuotationRenderer>.Instance);
        var pdf = renderer.Render(model, layoutConfig);

        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 1000);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(pdf, 0, 5));
    }

    private sealed class StubConfiguration : IConfiguration
    {
        public string? this[string key] { get => null; set { } }
        public IEnumerable<IConfigurationSection> GetChildren() => [];
        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken() => throw new NotSupportedException();
        public IConfigurationSection GetSection(string key) => throw new NotSupportedException();
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            handler(request, cancellationToken);
    }

    private sealed class StubAccessResolver : IRequestAccessResolver
    {
        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string requiredPermission, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result<RequestAccessContext>.Success(new RequestAccessContext(UserId, membershipId, OrgId, BranchId, requiredPermission, "organization")));
    }

    private sealed class StubApprovalLimitStore : IApprovalLimitStore
    {
        public int SeedCallCount { get; private set; }

        public Task<ApprovalLimitPage> ListAsync(Guid organizationId, ApprovalLimitListQuery query, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<ApprovalLimitOptions> GetOptionsAsync(Guid organizationId, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<Result<ApprovalLimitView>> GetAsync(Guid organizationId, Guid limitId, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<Result<ApprovalLimitView>> CreateAsync(
            Guid organizationId, Guid roleId, string documentType, Guid? branchId, string currency, bool isUnlimited, decimal? maxAmount,
            AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<Result<ApprovalLimitView>> UpdateAsync(
            Guid organizationId, Guid limitId, Guid ifMatch, bool isUnlimited, decimal? maxAmount, bool isActive,
            AdminActor actor, string traceId, CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<ApprovalLimitPage> SeedDefaultsAsync(Guid organizationId, AdminActor actor, string traceId, CancellationToken ct = default)
        {
            SeedCallCount++;
            var roleRef = new AdminRef(Guid.NewGuid(), "ผู้จัดการโครงการ (Project Manager)");
            var items = ApprovalDocumentType.Enforced
                .Select(docType => new ApprovalLimitView(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    roleRef,
                    docType,
                    null,
                    "THB",
                    false,
                    1_000_000m,
                    true,
                    Now,
                    Now))
                .ToList();
            return Task.FromResult(new ApprovalLimitPage(items, items.Count, 1, 100));
        }
    }
}
