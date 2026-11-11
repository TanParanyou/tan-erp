# Finance Integration Verification (CP-15 + G-15 + G-16 + G-17 + G-26)

**สถานะ:** Implemented 2026-10-04 (`CP-15`) และขยายรองรับภาษีไทยครบวงจร + วงเงินเครดิตและรายงานอายุหนี้ลูกหนี้ + ผังบัญชีและส่งออกสมุดรายวัน 2026-10-30 (`G-15` + `G-16` + `G-17`); เพิ่มการรองรับ Organization Logo & Brand Primary Color บน PDF, การเชื่อม `DocumentTemplateLayoutConfig`, และการลงนามดิจิทัล e-Tax XML (`G-26` / `ETaxSigningOptions`). กฎ: [Finance API Contract](../03-contracts/finance-api-contract.md), [แผน G-15](../superpowers/plans/2026-10-30-g15-thai-tax.md), [แผน G-16](../superpowers/plans/2026-10-30-g16-credit-exposure-ar-aging.md), และ [แผน G-17](../superpowers/plans/2026-10-30-g17-accounting-connector-journal-export.md).

## ผลที่รันจริง

- Unit `FinanceDomainTests` (รวมใน `TanErp.UnitTests` 702/702):
  - `CP-15`: reference hash คงที่และจับการเปลี่ยนยอด, ยอดไม่ถูกต้อง, สถานะตามการรับชำระบางส่วน/เต็ม/กลับรายการและห้ามรับเกิน, void ไม่ได้เมื่อมีเงินรับ, ใบที่ void ไม่มียอดค้างและไม่รับชำระ, validation การรับชำระ, outbox backoff 2/4/8/16 นาทีแล้ว dead ครั้งที่ 5 และ requeue, การยืนยันซ้ำ idempotent/ต่างกัน → conflict, ตัด error ยาว.
  - `G-15`: คำนวณ VAT 7% พร้อมปัดเศษ `AwayFromZero` ทศนิยม 2 ตำแหน่งทั้งรายบรรทัดและระดับเอกสาร, รักษา `ReferenceHash` เดิมเมื่อไม่มีภาษี/เงินประกันและเปลี่ยนค่า Hash เมื่อมีภาษี/เงินประกัน, คำนวณภาษีหัก ณ ที่จ่าย (`WhtRate`, `WhtAmount`, `NetCashReceived`) พร้อมบังคับเลข 50 ทวิ (`PAYMENT_WHT_CERTIFICATE_REQUIRED`), หักเงินประกันผลงาน (`RetentionStatus = "held"`) และปลดเงินประกันเมื่อครบกำหนด (`ReleaseRetention`) โดยไม่เพิ่มยอดวางบิลเกินสัญญา, และออกใบลดหนี้/ใบเพิ่มหนี้ (`BillingAdjustmentNote`) พร้อมตรวจเพดานยอดลดหนี้ (`BILLING_NOTE_EXCEEDS_ORIGINAL`) และ Maker ≠ Checker (`BILLING_NOTE_SELF_APPROVAL`).
  - `G-16`: คำนวณช่วงอายุหนี้ `ArAgingCalculator` (`current`, `1_30`, `31_60`, `61_90`, `over_90`), คำนวณวันเกินกำหนด (`DueDate` หรือ fallback `IssuedDate + CreditTermDays`), เพดานเครดิตประสิทธิผล `ComputeEffectiveCeiling`, และกฎของ `CreditLimitOverrideRequest` (`CREDIT_OVERRIDE_FIELD_INVALID`, `CREDIT_OVERRIDE_SELF_APPROVAL`, `CREDIT_OVERRIDE_INVALID_STATE`).
  - `G-17`: ตรวจสอบ `AccountMapping.Create/Update` (`ACCOUNT_MAPPING_FIELD_INVALID`) และการเปลี่ยนสถานะ Outbox `MarkExported` (`status = "exported"`, เก็บ `ExportedAtUtc` และ `ExportBatchId` แยกจาก `posted`) และ `MarkDeadMissingMapping`.
- Backend Integration `FinanceEndpointsTests` 8/8 (+ `DocumentSequencesEndpointsTests` 14/14, `ApprovalLimitEndpointsTests` 6/6) บน PostgreSQL:
  - `CP-15` (5 tests): วงเงินสัญญา, ยิงพร้อมกัน 4 คำขอไม่เกินสัญญา, รับชำระบางส่วน/เต็ม/กลับรายการ/ยิงพร้อมกันไม่รับเกิน, และคิวส่งบัญชี `accounting_outbox` + กระทบยอด.
  - `G-15` (`ThaiTaxG15_VatWhtRetentionNotesAndPdf_FullLifecycle`): ออกใบแจ้งหนี้พร้อม `ApplyVat: true` (7%, รันเลข `TIV-`), `RetentionRate: 0.10m` (10% `held`) และรายการบรรทัด; ตรวจว่า `release-retention` คืน `409 RETENTION_WARRANTY_NOT_EXPIRED` ก่อนหมดประกัน และปลดสำเร็จเมื่อใบรับประกัน (`Warranty`) หมดอายุแล้วโดยไม่เพิ่มยอด `Billed` เกินสัญญา; บันทึกรับชำระพร้อม WHT 3% (`50BIS-2026-0001`, รันเลข `RCT-`) และตรวจว่าหากไม่ส่งเลข 50 ทวิ คืน `422 PAYMENT_WHT_CERTIFICATE_REQUIRED`; ออกใบลดหนี้ (`CN-`) ตรวจ `422 BILLING_NOTE_EXCEEDS_ORIGINAL` และ `409 BILLING_NOTE_SELF_APPROVAL`, อนุมัติด้วยผู้อนุมัติคนละคนพร้อมส่ง `billing-note.approved` เข้า Outbox; และดาวน์โหลด PDF ใบกำกับภาษี/ใบเสร็จ (`GET /api/v1/billings/{id}/tax-document/pdf` → `200 OK`, `%PDF-`).
  - `G-16` (`CreditExposureAndArAgingG16_EnforcesLimit_RequiresIndependentOverride_AndReducesExposureOnPayment`): ตั้งวงเงินเครดิตลูกค้า `CreditLimit`, ตรวจยอด `TotalExposure = OutstandingReceivables + UnbilledAcceptedAmount`, บล็อกการออกใบแจ้งหนี้เมื่อเกินวงเงิน (`409 CREDIT_LIMIT_EXCEEDED`), สร้างคำขอขยายเพดานเครดิต (`POST /api/v1/finance/credit-overrides`), ปฏิเสธการอนุมัติตัวเอง (`409 CREDIT_OVERRIDE_SELF_APPROVAL`), อนุมัติโดยผู้อื่นสำเร็จและออกใบแจ้งหนี้ผ่าน, ตรวจรายงานอายุหนี้ `GET /api/v1/finance/ar-aging`, และตรวจว่าเมื่อรับชำระเงินแล้ว `OutstandingReceivables` และ `TotalExposure` ลดลงทันที.
  - `G-17` (`JournalExportAndCoaMappingG17_ParksUnmappedAsDead_ExportsMappedOnce_AndNeverDuplicates`): ทดสอบการส่งออกสมุดรายวัน (`POST /api/v1/finance/outbox/export-journal`) เมื่อยังไม่ได้ตั้งผังบัญชี → รายการถูกพักเป็น `dead` พร้อม `LastError = "ACCOUNT_MAPPING_MISSING:..."`; ตั้งค่าผังบัญชี (`PUT /api/v1/finance/account-mappings`) และกด `requeue` แล้วส่งออกใหม่ → ได้ไฟล์ CSV สมุดรายวันที่มี `DedupeKey` และเปลี่ยนสถานะเป็น `exported`; ส่งออกซ้ำอีกครั้ง → `ExportedCount == 0` ไม่ส่งออกซ้ำรายการที่ `exported` หรือ `posted` แล้ว.
- Frontend Verification (`features/finance`):
  - `npx tsc --noEmit` (0 errors), `npm run lint` (0 errors), `npm run build` (89/89 routes).
  - Vitest `src/features/finance/components/finance-components.test.tsx` 17/17 passed (ครอบคลุม `BillingEditor`, `BillingDetail`, `<ArAgingPanel />` 5 ช่วงอายุหนี้ + การขอและอนุมัติขยายเพดานเครดิตแบบ Maker–Checker, และ `FinanceSyncPage` พร้อมตารางตั้งค่าผังบัญชี COA Mapping และปุ่มส่งออกสมุดรายวัน CSV).

---

## Tax Document PDF with Organization Branding & Template Layout

การสร้างเอกสาร PDF ใบเสร็จรับเงิน/ใบกำกับภาษี (`QuestPdfTaxDocumentRenderer.cs`) ได้รับการปรับปรุงเพื่อสนับสนุนภาพลักษณ์องค์กรและรูปแบบเอกสารที่ยืดหยุ่น:

1. **Organization Branding (`BrandPrimaryColor` & `BrandLogoFileId`):**
   - ดึงสีแบรนด์ประจำองค์กรผ่าน `ResolvePrimaryColor(model.Organization.BrandPrimaryColor)` โดยมี fallback เป็น Navy `#0B3056` (Atelier Architectural Navy Sharp)
   - ใช้สีแบรนด์สำหรับแถบ Header, เส้นคั่น Grid, และข้อความเน้นย้ำ
   - โหลดภาพโลโก้องค์กรจาก `BrandLogoFileId` ผ่าน `IFileStorageProvider` และจัดวางที่มุมซ้ายบนของเอกสารอย่างคมชัด
2. **Document Template Layout Config:**
   - เชื่อมต่อ `DocumentTemplateLayoutConfig` (`billing-documents`) เพื่อควบคุมการแสดงผลหมายเหตุ ข้อมูลผู้ติดต่อ เงื่อนไขการชำระเงิน และการจัดหน้า

---

## Thai Accounting Connectors & Standard Thai COA (G-17)

1. **Configurable Accounting Connector (`ConfigurableAccountingConnector.cs`):**
   - รองรับโหมด `local` (จำลองการบันทึกบัญชีสำเร็จพร้อมรหัสอ้างอิง `LOCAL-{ResourceNumber}-{DedupeKey}`), `peak` (PEAK Account API v1 JSON payload), `flowaccount` (FlowAccount OpenAPI payload), และ `webhook` (Generic JSON พร้อม HMAC Signature)
2. **Multi-format CSV Journal Export:**
   - ส่งออก CSV ได้ 3 รูปแบบตามที่ผู้ใช้เลือก: Standard Format, Express Software Format (`VOUCHER_NO,VOUCHER_DATE,ACCT_CODE,ACCT_NAME,DR_AMT,CR_AMT,REMARK`), และ Cloud Accounting Format
3. **Standard Thai COA Presets (`StandardThaiCoaPresets`):**
   - กำหนดค่าเริ่มต้นผังบัญชีมาตรฐานของไทยใน `FinanceStore.cs` สำหรับทุก Event Type ในระบบ

---

## ETDA e-Tax Invoice XML with Digital Signature (G-26)

การสร้างเอกสาร e-Tax Invoice XML (`ETaxInvoiceXmlBuilder.cs`) ได้รับการยกระดับตามมาตรฐาน ETDA Recommendation 3-2560 (UN/CEFACT CII):

1. **Structured Buyer & Seller `PostalTradeAddress`:**
   - แปลงข้อมูลที่อยู่ของทั้งผู้ขายและผู้ซื้อเป็น XML element ที่มีโครงสร้างสมบูรณ์: `BuildingName`, `LineOne`, `CitySubDivisionName` (ตำบล/แขวง), `CityName` (อำเภอ/เขต), `CountrySubDivisionID` (รหัสจังหวัด 2 หลัก), `PostcodeCode`, และ `CountryID` (`TH`)
2. **Reference to Adjustment Notes:**
   - สำหรับใบลดหนี้ (Credit Note - Code 81) และใบเพิ่มหนี้ (Debit Note - Code 80) จะสร้าง element `<ram:InvoiceReferencedDocument>` อ้างอิงเลขที่ใบกำกับภาษีเดิมและวันที่ออกเอกสารต้นฉบับ
3. **Enveloped RSA-SHA256 XML Digital Signature (`ETaxSigningOptions`):**
   - รองรับการลงนามดิจิทัลผ่านคลาส `ETaxSigningOptions` (`SigningCertificatePath`, `SigningCertificatePassword`)
   - นำเข้าใบรับรอง PKCS#12 (.p12/.pfx) ดึงกุญแจส่วนตัว (Private Key) และสร้าง `<ds:Signature>` Enveloped Signature ครบถ้วน:
     - Canonicalization: `http://www.w3.org/TR/2001/REC-xml-c14n-20010315`
     - Signature Method: `http://www.w3.org/2001/04/xmldsig-more#rsa-sha256`
     - Digest Method: `http://www.w3.org/2001/04/xmlenc#sha256`
     - KeyInfo: `<ds:X509Certificate>` บรรจุไบต์ใบรับรองสาธารณะ Base64
   - กรณีไม่มีการระบุใบรับรอง จะส่งออกเอกสารฉบับร่างพร้อม XML Comment กำกับชัดเจน
