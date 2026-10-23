# Quotation PDF Verification (G-04: Server-side Quotation PDF + artifact hash)

## 1. สถานะ

**Implemented 2026-10-13 (G-04: Server-side Quotation PDF + artifact hash) — Verified 2026-10-23: Unit + Integration + Vitest passed.**

- ชุดทดสอบ unit (`QuotationPdfModelAllowlistTests`, `QuotationDocumentTests`), integration (`QuestPdfQuotationRendererTests`, `EstimateEndpointsTests.QuotationDocuments`) และ frontend Vitest ถูกรันและผ่านครบถ้วนเมื่อ 2026-10-23
- **ไม่ได้ตรวจในเบราว์เซอร์** และ **ไม่เคยมีมนุษย์เปิดดู PDF ที่สร้างจริง** — การเรียงสระ/วรรณยุกต์ไทย, การแบ่งหน้า และข้อความยาวยังไม่ได้ตรวจด้วยสายตา
- ยังไม่ผ่าน UAT หรือพร้อม Production
- **เงื่อนไข license ของ QuestPDF ยังไม่ได้รับการยืนยันจาก Business** (ดูหัวข้อ 5 และ 7)
- โลโก้องค์กรยังไม่ทำ (Organization ไม่มีคอลัมน์โลโก้)

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

ผลข้างต้นเป็นการตรวจว่า **คอมไพล์และ lint ผ่าน** เท่านั้น ไม่ได้หมายความว่า logic ถูกต้องตามชุดทดสอบ

## 3. ชุดทดสอบที่เขียนไว้ (tests written, NOT run)

จำนวนนับจาก grep ของ `[Fact]`/`[Theory]` (backend) และ `it(`/`test(` (frontend) ณ 2026-10-13; `[Theory]` นับเป็น 1 ต่อเมธอด

| ระดับ | ไฟล์ | จำนวนเมธอด | สถานะ |
| --- | --- | --- | --- |
| Unit | `backend/tests/TanErp.UnitTests/Commercial/QuotationPdfModelAllowlistTests.cs` | 1 | เขียนแล้ว ยังไม่รัน |
| Unit | `backend/tests/TanErp.UnitTests/Commercial/QuotationDocumentTests.cs` | 2 | เขียนแล้ว ยังไม่รัน |
| Integration | `backend/tests/TanErp.IntegrationTests/Documents/QuestPdfQuotationRendererTests.cs` | 2 | เขียนแล้ว ยังไม่รัน |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs` (ไฟล์ใหม่, partial class) | 7 | เขียนแล้ว ยังไม่รัน (Testcontainers Postgres) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` (แก้ไฟล์เดิม ทั้งไฟล์ 42 เมธอด; แก้ `AcceptQuotation_CannotStartFromUnapprovedDraftEstimate`, `ExternalAcceptance_CustomerAcceptsOnce_WithEvidence_AndNoCostData`, `ExternalAcceptance_LinkOfAReplacedQuotation_CannotAcceptTheReplacement` และ helper) | 3 เมธอดที่แก้ | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/lib/api/api-client.test.ts` (ไฟล์ทั้งหมด 16; เพิ่ม 26 บรรทัด) | 16 | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/lib/files/save-blob.test.ts` | 1 | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/features/estimates/components/quotation-document-actions.test.tsx` | 3 | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/features/external-acceptance/components/public-acceptance-page.test.tsx` (ไฟล์ทั้งหมด 8; แก้ 15 บรรทัด) | 8 | เขียนแล้ว ยังไม่รัน |

เมธอดใหม่ใน `EstimateEndpointsTests.QuotationDocuments.cs`: `ExternalAcceptance_ServesTheStoredPdf_AndRecordsTheHashTheCustomerAttested`, `ExternalAcceptance_OfALegacyQuotationWithoutPdf_StillWorks_WithNoHashRecorded`, `QuotationDocument_IsServedFromStorage_AndIsNotChangedByLaterOrganizationProfileChanges`, `QuotationDocument_OutsideTheCallersOrganizationOrBranch_IsNotFound_AndALegacyQuotationHasNone`, `IssueQuotation_StoresOneDocumentRow_WhoseHashMatchesTheStoredBytes`, `IssueQuotation_WhenRenderingFails_RollsBackTheWholeIssue`, `AmendQuotation_RendersTheReplacementOnce_AndKeepsTheOriginalDocumentUntouched`

รวม backend เมธอดใหม่ 12 เมธอดใน 4 ไฟล์ใหม่ + 3 เมธอดเดิมที่ถูกแก้; frontend ไฟล์ที่ถูกเพิ่ม/แก้ 4 ไฟล์ (จำนวนในตารางเป็นจำนวนทั้งไฟล์ ไม่ใช่เฉพาะที่เพิ่ม) — **ทั้งหมดยังไม่ได้รัน**

## 4. คำสั่งที่ผู้ใช้ควรรันเอง

รันตามลำดับนี้ เริ่มจาก focused แล้วค่อย full gate (ห้ามอ้างว่าผ่านก่อนเห็นผลจริง):

```bash
# Unit: allowlist และ domain ของเอกสาร
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~QuotationPdfModelAllowlistTests|FullyQualifiedName~QuotationDocumentTests"

# Integration: renderer (QuestPDF + ฟอนต์ฝัง)
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~QuestPdfQuotationRendererTests"

# Integration: issue/amend/ดาวน์โหลด/ยอมรับ (EstimateEndpointsTests ทั้ง partial class)
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests" -m:1

# OpenAPI snapshot (regenerate แล้ว แต่ยังไม่เคยรันเทียบ)
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"

# Architecture
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj

# Frontend
cd frontend && npx vitest run src/features/estimates src/features/external-acceptance src/lib/files src/lib/api/api-client.test.ts
```

**Final gate (ก่อนส่งมอบ/merge ตาม AGENTS.md หมวด 8):**

```bash
# Backend
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/TanErp.slnx

# Frontend
cd frontend && npm run build && npm run lint && npx vitest run
```

## 5. สิ่งที่ชุดทดสอบที่เขียนไว้ตั้งใจพิสูจน์ (ยังไม่ได้พิสูจน์จริง)

- **Allowlist:** property ของ `QuotationPdfModel` ตรงรายการที่ทบทวนแล้ว ไม่มีชื่อที่สื่อถึงต้นทุน/มาร์จิน/id ภายใน (reflection)
- **Renderer:** สร้าง PDF ได้ทั้งไทย/อังกฤษ, แบ่งหน้า, ข้อความยาว, ฟอนต์+OFL ฝังใน assembly
- **ไฟล์ที่เก็บ:** hash ตรง bytes ที่เก็บ; audit `quotations.document-rendered` ไม่มีข้อมูลลูกค้า; ไฟล์ไม่เปลี่ยนเมื่อแก้ข้อมูลองค์กรภายหลัง
- **Issue/Amend:** render พังแล้ว rollback ทั้งการออกใบ; amend ได้ไฟล์ใหม่และไฟล์เดิมไม่เปลี่ยน
- **Scope:** ต่าง Organization/Branch → 404; ใบเก่า → 404 `QUOTATION_DOCUMENT_NOT_AVAILABLE`
- **หน้าลูกค้า:** ได้ไฟล์ตรง hash; hash ผิด/ขาดขณะมีไฟล์ → `ACCEPTANCE_CONFLICT`; หลักฐานเก็บ `document_sha256`; ใบเก่ายอมรับได้โดย hash เป็น null
- **Frontend:** ปุ่มดาวน์โหลด, ข้อความ error ตาม code, หน้าลูกค้าส่ง hash กลับ, `saveBlob`

## 6. สิ่งที่ยังไม่ได้ทำ/ต้องทำก่อนเปิดใช้จริง

1. **Business ยืนยันเงื่อนไข license QuestPDF** (รายได้รวม < 1,000,000 USD และไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ) แล้วบันทึกผู้ยืนยัน/วันที่ที่นี่ และตั้ง `Documents__QuestPdf__LicenseConfirmed=true` ใน Production; ถ้าไม่ผ่านให้ซื้อ Professional แล้วตั้ง `Documents__QuestPdf__LicenseType=Professional` หรือเปลี่ยน renderer หลัง `IQuotationDocumentRenderer` — ผู้ยืนยัน/วันที่: **ยังไม่มี**
2. Sales/Finance เปิด PDF จริงตรวจ layout (สระ/วรรณยุกต์ไทย, แบ่งหน้า, ยอดสรุป) และข้อมูลหัวเอกสาร (ชื่อ/เลขผู้เสียภาษี/ที่อยู่ขององค์กรและสาขาต้องกรอกใน Settings ก่อน — ช่องว่างไม่ถูกพิมพ์)
3. ตรวจเชิงปฏิบัติการ: ออกใบเสนอราคา 1 ใบ → ดาวน์โหลด → `shasum -a 256` ต้องตรงกับ hash ในประวัติ → เปิดหน้าลูกค้าและเทียบ hash
4. ตั้ง `Storage:BasePath` ให้ชี้ที่เก็บถาวร (ค่า default อยู่ใต้โฟลเดอร์แอป) เพราะไฟล์ PDF คือหลักฐาน
5. apply migration `AddQuotationDocuments` ในแต่ละ environment (apply กับ local dev database แล้วเท่านั้น)

## 7. การตัดสินใจ/ข้อจำกัดที่ต้องให้ Business Owner ตัดสินหรือรับทราบ

| # | เรื่อง | ค่าปัจจุบัน | หมายเหตุ |
| --- | --- | --- | --- |
| 1 | License QuestPDF 2026.9.1 | Community license ต้องมีรายได้ < 1,000,000 USD และไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ | ตั้ง license ที่เดียวใน `QuestPdfRuntime` ผ่าน `Documents:QuestPdf:LicenseType`; ใน Production จะ log warning จนกว่า `LicenseConfirmed=true`; ทางสำรองคือ Professional license หรือเปลี่ยน renderer หลัง `IQuotationDocumentRenderer` |
| 2 | ฟอนต์ | Sarabun Regular + Bold ฝังใน assembly (~180 KB) พร้อม `OFL.txt`; ตรวจ hash แล้ว | |
| 3 | โลโก้ | **ไม่ทำ** (เลื่อนเป็น follow-up) | Organization ไม่มีคอลัมน์โลโก้ |
| 4 | ใบเสนอราคาที่ออกก่อน G-04 | **ไม่มี PDF** — ดาวน์โหลดได้ `404 QUOTATION_DOCUMENT_NOT_AVAILABLE` | Amend จะได้ไฟล์; ปุ่มพิมพ์ผ่านเบราว์เซอร์ยังใช้ได้; endpoint สร้างครั้งเดียวตามคำขอเป็น follow-up ที่เป็นไปได้ |
| 5 | Render ก่อน commit ใน unit of work เดียวกัน | render พัง → rollback ทั้งการออกใบและตอบ `500` | ถ้า commit ล้มหลังเขียน blob แล้ว อาจเหลือ blob กำพร้า (ไม่มี job กวาด) |
| 6 | การยอมรับของลูกค้าผูกกับ hash | ส่ง `documentSha256`; ไม่ตรง → `ACCEPTANCE_CONFLICT`; `null` ใช้ได้เฉพาะเมื่อไม่มี PDF | |
| 7 | Hash คงที่เพราะเก็บไฟล์ครั้งเดียว | render input เดียวกันสองครั้งต่าง process ได้ SHA-256 ต่างกัน | ห้าม re-render ฉบับที่เก็บแล้วเด็ดขาด |
| 8 | Integration test เขียนไฟล์จริง | test issue/amend เดิมเขียน PDF จริงลง `AppContext.BaseDirectory/local-storage` | โฟลเดอร์ทดสอบแบบอ่านอย่างเดียวจะทำให้ test เหล่านี้ล้ม |
| 9 | หนึ่ง PDF สองภาษาต่อ quotation | ไม่มี permission ใหม่ ใช้ `quotations.read` | Sales/Finance/Legal ยืนยัน |
| 10 | `OpenApiContractTests` | คาดว่าผ่านหลัง regenerate snapshot | **ยังไม่ได้รัน** |
| 11 | ปุ่มใน frontend | ใช้ขนาดมาตรฐานของ design system (ตัด override `min-h-11` ตาม `design.md` 4.4) | |
| 12 | ระดับหลักฐาน | ไม่ใช่ PDF/A และไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย (เท่า CP-07: ชื่อ+ยินยอม+hash) | ถ้า Legal ต้องการสูงกว่านี้ → slice ใหม่ |

## 8. ข้อจำกัดที่ยอมรับ

- transaction ออกใบเสนอราคาเปิดนานขึ้นเท่าเวลา render
- blob กำพร้าเมื่อ commit ล้มหลังเขียนไฟล์ (ไม่มี job กวาด)
- `GET /api/v1/estimates/{id}/quotations` ยังตรวจเฉพาะ Organization ไม่ตรวจ Branch (ของเดิม ไม่แก้ในรอบนี้ — endpoint ใหม่ตรวจ Branch)
- ไม่ audit การดาวน์โหลด; ข้อมูลองค์กรอยู่ในไฟล์เท่านั้น

## 9. ยังเปิดอยู่

- โลโก้องค์กร (follow-up)
- ผู้ยืนยัน license QuestPDF
- endpoint สร้าง PDF ครั้งเดียวตามคำขอสำหรับใบเก่า (ถ้า Business ต้องการ)

## 10. งานที่เหลือ (Backlog) และข้อสังเกตจาก CI — บันทึก 2026-10-13

> สถานะ: โค้ด G-04 ครบ 10 จาก 10 task แล้ว งานที่เหลือเป็นการยืนยัน/ตัดสินใจ ไม่ใช่การเขียนโค้ดใหม่ ขั้น CI และ PR ถูก**ข้ามไว้ก่อนโดยตั้งใจ** ตามคำสั่งผู้ใช้ ([PR #23](https://github.com/TanParanyou/tan-erp/pull/23) ยังเปิดอยู่ ยังไม่ merge)

### 10.1 ต้องทำก่อน merge/ใช้จริง

| # | งาน | ผู้รับผิดชอบ | สถานะ |
| --- | --- | --- | --- |
| 1 | รัน test ที่เขียนไว้ตามหัวข้อ 4 (โดยเฉพาะ `EstimateEndpointsTests`, `QuestPdfQuotationRendererTests`, `QuotationDocumentTests`) และ gate เต็มตาม AGENTS.md หมวด 8 | ผู้ใช้/ทีมพัฒนา | ยังไม่ทำ |
| 2 | เปิด PDF จริงตรวจสระ/วรรณยุกต์ไทย การแบ่งหน้า ข้อความยาว และหัวเอกสาร | Sales/Finance | ยังไม่ทำ |
| 3 | ยืนยันเงื่อนไข license QuestPDF Community แล้วตั้ง `Documents:QuestPdf:LicenseConfirmed=true` (หรือเปลี่ยนเป็น Professional) | Business Owner | ยังไม่ทำ |
| 4 | ตั้ง `Storage:BasePath` ให้ชี้ที่เก็บถาวรในทุก environment แล้ว apply migration `AddQuotationDocuments` | ผู้ดูแลระบบ | local dev apply แล้ว ที่อื่นยังไม่ทำ |
| 5 | merge [PR #23](https://github.com/TanParanyou/tan-erp/pull/23) หลังข้อ 1 ผ่าน | ผู้ใช้ | ข้ามไว้ก่อน |

### 10.2 Follow-up ที่ตั้งใจเลื่อน (ยังไม่อยู่ในขอบเขต G-04)

- **โลโก้องค์กร:** `Organization` ยังไม่มีคอลัมน์โลโก้ ต้องมี schema + owner type + หน้าตั้งค่า (เลื่อนต่อจาก G-03a)
- **ใบเสนอราคาที่ออกก่อน G-04:** ไม่มี PDF (ดาวน์โหลดได้ 404 `QUOTATION_DOCUMENT_NOT_AVAILABLE`, แก้ด้วยการ Amend) ถ้า Business ต้องการ ให้เพิ่ม endpoint สร้าง PDF ครั้งเดียวตามคำขอ (ประมาณ 150 บรรทัดในแผนเดิมที่ตัดออก)
- **blob กำพร้า:** ถ้า commit ล้มหลังเขียนไฟล์แล้ว ไฟล์ค้างโดยไม่มี job กวาด
- **การดาวน์โหลดไม่ถูก audit** และ `GET /api/v1/estimates/{id}/quotations` ยังตรวจเฉพาะ Organization (ของเดิม)

### 10.3 สิ่งที่ CI พบและการแก้ (รอบ 2026-10-13)

CI ของ PR #23 รอบแรกล้ม 1 จาก 530 integration test: `QuotationDocument_OutsideTheCallersOrganizationOrBranch_IsNotFound_AndALegacyQuotationHasNone` ได้ 403 แทน 404 สำหรับผู้อ่านที่ผูกสาขาอื่น

- **สาเหตุ:** ข้อสมมติของ test ผิด ไม่ใช่บั๊กของระบบ — `RequestAccessResolver` ยอมรับสิทธิ์ระดับสาขาเฉพาะ `estimates.read/update/approve` (ตัวแปร `allowBranchScopedEstimatePermission`) สิทธิ์ `quotations.read` ระดับสาขาจึงถูกถือว่าไม่มีสิทธิ์ (403)
- **การแก้:** seed ผู้อ่านด้วยสิทธิ์ระดับองค์กร และผูก membership กับสาขาอื่น (`RequestAccessContext.HasBranchAccess`: BranchId ว่าง = ทุกสาขา) จึงผ่านการตรวจสิทธิ์แต่ไม่ผ่านการตรวจสาขา ได้ 404 ตามที่ออกแบบ (commit `679b0d7`)
- **หลักฐาน:** รัน test ตัวนั้นเดี่ยว ๆ ผ่าน 1/1; ส่วนอื่นของชุด .NET ผ่านในรอบเดิม (unit 614, architecture 4, integration 529) ผล CI รอบหลังแก้ยังไม่ได้ตรวจเพราะข้ามไว้
- **ข้อควรรู้สำหรับ slice ถัดไป:** สิทธิ์ระดับสาขารองรับเฉพาะกลุ่ม `estimates.*` ถ้า feature ใหม่ต้องการจำกัดตามสาขา ให้ใช้สิทธิ์ระดับองค์กรร่วมกับ `BranchId` ของ membership

