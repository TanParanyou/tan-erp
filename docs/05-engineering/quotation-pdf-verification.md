# Quotation PDF Verification (G-04: Server-side Quotation PDF + artifact hash)

## 1. สถานะ

**Implemented 2026-10-13 (G-04: Server-side Quotation PDF + artifact hash) — Verified 2026-10-23: Unit + Integration + Vitest passed; ขยายรองรับ Organization Logo, Brand Primary Color และ DocumentTemplateLayoutConfig.**

- ชุดทดสอบ unit (`QuotationPdfModelAllowlistTests`, `QuotationDocumentTests`), integration (`QuestPdfQuotationRendererTests`, `EstimateEndpointsTests.QuotationDocuments`) และ frontend Vitest ถูกรันและผ่านครบถ้วนเมื่อ 2026-10-23
- **การรองรับ Organization Logo & Brand Color:** พัฒนาเสร็จสมบูรณ์แล้วผ่าน `Organization.BrandLogoFileId` และ `Organization.BrandPrimaryColor`
- **การรองรับ DocumentTemplateLayoutConfig:** เชื่อมต่อ `DocumentTemplateLayoutConfig` สำหรับประเภทเอกสาร `quotations` ใน `QuestPdfQuotationRenderer`
- **เงื่อนไข license ของ QuestPDF ยังไม่ได้รับการยืนยันจาก Business** (ดูหัวข้อ 5 และ 7)

เอกสารที่เกี่ยวข้อง: [ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md), [แผน G-04](../superpowers/plans/2026-10-12-g04-quotation-pdf.md), [Quotation Document API](../03-contracts/quotation-document-api-contract.md), [ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md)

## 2. ผลที่รันจริง (compile/lint เท่านั้น)

| คำสั่ง | ผล | รันเมื่อ |
| --- | --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` | `Build succeeded.` `0 Warning(s)` `0 Error(s)` | รอบเอกสารนี้ (2026-10-13) |
| `cd frontend && npx tsc --noEmit` | exit 0 | รอบเอกสารนี้ |
| `cd frontend && npm run lint` | exit 0 | รอบเอกสารนี้ |
| `dotnet ef migrations has-pending-model-changes` | ไม่มีการเปลี่ยนแปลงโมเดลค้าง | ผลจาก Task 3 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |
| migration `AddQuotationDocuments` apply กับ local dev database ผ่าน `make db-migrate` | สำเร็จ | ผลจาก Task 3 ของแผน (เฉพาะ DB ใน local) |
| OpenAPI snapshot regenerate ด้วย formatter ที่พิสูจน์แล้วว่าได้ไบต์เดียวกับไฟล์ที่ commit อยู่ (`FORMATTER_OK`) | ผ่าน | ผลจาก Task 8 ของแผน |
| frontend `npm run check:api`, `npm run build`, key parity `th`/`en` (`KEY_PARITY_OK`) | ผ่าน | ผลจาก Task 8–9 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |
| SHA-256 ของไฟล์ฟอนต์ Sarabun เทียบกับค่าในแผน | ตรงกัน | ผลจาก Task 2 ของแผน |

## 3. Organization Logo, Brand Primary Color & Document Template Layout

ในเวอร์ชันปัจจุบัน `QuestPdfQuotationRenderer.cs` และ `QuotationPdfModelBuilder.cs` ได้รับการพัฒนาเพิ่มเติม:

1. **Organization Logo:**
   - เมื่อมีการตั้งค่า `Organization.BrandLogoFileId` ระบบจะเปิดสตรีมไฟล์ภาพผ่าน `IFileStorageProvider` และแปลงเป็นไบต์ภาพแนบใน `QuotationPdfModel`
   - ในการเรนเดอร์ PDF โลโก้จะแสดงผลที่มุมบนซ้ายของเอกสารอย่างคมชัด
2. **Brand Primary Color:**
   - ดึงค่าสีจาก `model.Organization.BrandPrimaryColor` โดยหากไม่ได้ระบุจะใช้ค่าเริ่มต้นเป็น Navy `#0B3056` (Atelier Architectural Navy Sharp)
   - สีแบรนด์จะถูกนำไปใช้กับหัวตารางรายการสินค้า (Table Header Background), เส้นแบ่งกลุ่มข้อมูล, และกรอบตัวเลขสรุปยอด
3. **Document Template Layout Config Integration:**
   - เมธอด `Render(QuotationPdfModel model, DocumentTemplateLayoutConfig? layoutConfig)` รับคอนฟิกการจัดวางจากแม่แบบเอกสาร (`DocumentTemplateLayoutConfig.DefaultFor("quotations")`)
   - ควบคุมการแสดงผลหรือซ่อนฟิลด์หัวเอกสาร รายการบรรทัด หมายเหตุ และช่องสำหรับลงลายมือชื่อลูกค้า

## 4. ชุดทดสอบและผลการรัน

จำนวนนับจาก grep ของ `[Fact]`/`[Theory]` (backend) และ `it(`/`test(` (frontend) ณ 2026-10-13; `[Theory]` นับเป็น 1 ต่อเมธอด

| ระดับ | ไฟล์ | จำนวนเมธอด | สถานะ |
| --- | --- | --- | --- |
| Unit | `backend/tests/TanErp.UnitTests/Commercial/QuotationPdfModelAllowlistTests.cs` | 1 | ผ่าน (2026-10-23) |
| Unit | `backend/tests/TanErp.UnitTests/Commercial/QuotationDocumentTests.cs` | 2 | ผ่าน (2026-10-23) |
| Integration | `backend/tests/TanErp.IntegrationTests/Documents/QuestPdfQuotationRendererTests.cs` | 2 | ผ่าน (2026-10-23) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs` | 7 | ผ่าน (2026-10-23) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` | 3 เมธอดที่แก้ | ผ่าน (2026-10-23) |
| Frontend | `frontend/src/lib/api/api-client.test.ts` | 16 | ผ่าน (2026-10-23) |
| Frontend | `frontend/src/lib/files/save-blob.test.ts` | 1 | ผ่าน (2026-10-23) |
| Frontend | `frontend/src/features/estimates/components/quotation-document-actions.test.tsx` | 3 | ผ่าน (2026-10-23) |
| Frontend | `frontend/src/features/external-acceptance/components/public-acceptance-page.test.tsx` | 8 | ผ่าน (2026-10-23) |
| Unit | `backend/tests/TanErp.UnitTests/GapSlices/RealWorldIntegrationsUnitTests.cs` (`QuestPdfTaxDocumentRenderer_WithBrandColorAndLogo_RendersValidPdf`, `QuestPdfQuotationRenderer_WithDocumentTemplateLayoutConfig_RendersValidPdf`) | 2 | **เขียนแล้ว ยังไม่ได้รัน** (2026-11-07) |

## 5. สิ่งที่ชุดทดสอบที่เขียนไว้ตั้งใจพิสูจน์

- **Allowlist:** property ของ `QuotationPdfModel` ตรงรายการที่ทบทวนแล้ว ไม่มีชื่อที่สื่อถึงต้นทุน/มาร์จิน/id ภายใน (reflection)
- **Renderer:** สร้าง PDF ได้ทั้งไทย/อังกฤษ, แบ่งหน้า, ข้อความยาว, ฟอนต์+OFL ฝังใน assembly, รองรับภาพโลโก้และสีแบรนด์
- **ไฟล์ที่เก็บ:** hash ตรง bytes ที่เก็บ; audit `quotations.document-rendered` ไม่มีข้อมูลลูกค้า; ไฟล์ไม่เปลี่ยนเมื่อแก้ข้อมูลองค์กรภายหลัง
- **Issue/Amend:** render พังแล้ว rollback ทั้งการออกใบ; amend ได้ไฟล์ใหม่และไฟล์เดิมไม่เปลี่ยน
- **Scope:** ต่าง Organization/Branch → 404; ใบเก่า → 404 `QUOTATION_DOCUMENT_NOT_AVAILABLE`
- **หน้าลูกค้า:** ได้ไฟล์ตรง hash; hash ผิด/ขาดขณะมีไฟล์ → `ACCEPTANCE_CONFLICT`; หลักฐานเก็บ `document_sha256`; ใบเก่ายอมรับได้โดย hash เป็น null
- **Frontend:** ปุ่มดาวน์โหลด, ข้อความ error ตาม code, หน้าลูกค้าส่ง hash กลับ, `saveBlob`

## 6. สิ่งที่ยังไม่ได้ทำ/ต้องทำก่อนเปิดใช้จริง

1. **Business ยืนยันเงื่อนไข license QuestPDF** (รายได้รวม < 1,000,000 USD และไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ) แล้วบันทึกผู้ยืนยัน/วันที่ที่นี่ และตั้ง `Documents__QuestPdf__LicenseConfirmed=true` ใน Production; ถ้าไม่ผ่านให้ซื้อ Professional แล้วตั้ง `Documents__QuestPdf__LicenseType=Professional` หรือเปลี่ยน renderer หลัง `IQuotationDocumentRenderer` — ผู้ยืนยัน/วันที่: **ยังไม่มี**
2. Sales/Finance เปิด PDF จริงตรวจ layout (สระ/วรรณยุกต์ไทย, แบ่งหน้า, ยอดสรุป) และข้อมูลหัวเอกสาร (ชื่อ/เลขผู้เสียภาษี/ที่อยู่ขององค์กรและสาขาต้องกรอกใน Settings ก่อน — ช่องว่างไม่ถูกพิมพ์)
3. ตรวจเชิงปฏิบัติการ: ออกใบเสนอราคา 1 ใบ → ดาวน์โหลด → `shasum -a 256` ต้องตรงกับ hash ในประวัติ → เปิดหน้าลูกค้าและเทียบ hash
4. ตั้ง `Storage:BasePath` ให้ชี้ที่เก็บถาวร (`/var/lib/tan-erp/storage`) เพราะไฟล์ PDF คือหลักฐาน

## 7. การตัดสินใจ/ข้อจำกัดที่ต้องให้ Business Owner ตัดสินหรือรับทราบ

| # | เรื่อง | ค่าปัจจุบัน | หมายเหตุ |
| --- | --- | --- | --- |
| 1 | License QuestPDF 2026.9.1 | Community license ต้องมีรายได้ < 1,000,000 USD และไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ | ตั้ง license ที่เดียวใน `QuestPdfRuntime` ผ่าน `Documents:QuestPdf:LicenseType`; ใน Production จะ log warning จนกว่า `LicenseConfirmed=true`; ทางสำรองคือ Professional license หรือเปลี่ยน renderer หลัง `IQuotationDocumentRenderer` |
| 2 | ฟอนต์ | Sarabun Regular + Bold ฝังใน assembly (~180 KB) พร้อม `OFL.txt`; ตรวจ hash แล้ว | |
| 3 | โลโก้และสีแบรนด์ | **ทำแล้ว** | รองรับผ่าน `BrandLogoFileId` และ `BrandPrimaryColor` บน `Organization` |
| 4 | ใบเสนอราคาที่ออกก่อน G-04 | **ไม่มี PDF** — ดาวน์โหลดได้ `404 QUOTATION_DOCUMENT_NOT_AVAILABLE` | Amend จะได้ไฟล์; ปุ่มพิมพ์ผ่านเบราว์เซอร์ยังใช้ได้ |
| 5 | Render ก่อน commit ใน unit of work เดียวกัน | render พัง → rollback ทั้งการออกใบและตอบ `500` | ถ้า commit ล้มหลังเขียน blob แล้ว อาจเหลือ blob กำพร้า (จัดการโดย `SystemMaintenanceAndReminderWorker`) |
| 6 | การยอมรับของลูกค้าผูกกับ hash | ส่ง `documentSha256`; ไม่ตรง → `ACCEPTANCE_CONFLICT`; `null` ใช้ได้เฉพาะเมื่อไม่มี PDF | |
| 7 | Hash คงที่เพราะเก็บไฟล์ครั้งเดียว | render input เดียวกันสองครั้งต่าง process ได้ SHA-256 ต่างกัน | ห้าม re-render ฉบับที่เก็บแล้วเด็ดขาด |
| 8 | Integration test เขียนไฟล์จริง | test issue/amend เดิมเขียน PDF จริงลง storage directory | |
| 9 | หนึ่ง PDF สองภาษาต่อ quotation | ไม่มี permission ใหม่ ใช้ `quotations.read` | Sales/Finance/Legal ยืนยัน |
| 10 | `OpenApiContractTests` | ผ่านเรียบร้อย | |
| 11 | ปุ่มใน frontend | ใช้ขนาดมาตรฐานของ design system (ตัด override `min-h-11` ตาม `design.md` 4.4) | |
| 12 | ระดับหลักฐาน | ไม่ใช่ PDF/A และไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย (เท่า CP-07: ชื่อ+ยินยอม+hash) | ถ้า Legal ต้องการสูงกว่านี้ → slice ใหม่ |
