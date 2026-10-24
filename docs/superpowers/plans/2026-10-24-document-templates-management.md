# Document Templates Management (Document Form Customizer) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** พัฒนาระบบศูนย์กลางการจัดการเทมเพลตเอกสารสำหรับพิมพ์และส่งออก PDF (Document & Print Form Customizer) ที่รองรับเอกสารหลายประเภท (Quotation, Estimate BOQ, Purchase Order, Billing, Goods Receipt) และหลายเทมเพลตต่อประเภท พร้อม Live Split-Pane A4 Real-time Preview ตามมาตรฐานดีไซน์ *Atelier Architectural Navy Sharp* และเชื่อมโยงกับเอนจิน QuestPDF

**Architecture:** Clean Architecture 4 ชั้น (.NET 9 C#) ร่วมกับ Next.js 15 App Router (TypeScript):
1. **Domain:** `DocumentTemplate` Entity ใน schema `common.document_templates` และ `DocumentTemplateConfig` JSON model
2. **Application:** CQRS Handlers (List, Get, Create, Update, Delete) พร้อม ETag Concurrency (`If-Match`) และ Permission Authorization
3. **Infrastructure:** EF Core Persistence และการฉีด Layout Config เข้าสู่ `QuestPdfQuotationRenderer` (และเอนจินเอกสารอื่น)
4. **Frontend:** หน้าจอ Live Split-Pane Studio (`/settings/document-templates`) พร้อม `FormTabs`, `useDeferredFileUpload`, และ React Hook Form

**Tech Stack:** .NET 9, C#, EF Core, PostgreSQL (JSONB / JsonDocument), QuestPDF, Next.js 15 App Router, TypeScript, Tailwind CSS, TanStack Query, React Hook Form, Zod, next-intl.

**Spec:** [AGENTS.md](../../AGENTS.md), [design.md](../../design.md), [ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md)

---

## Global Constraints

- **Language & Communication:** ตอบกลับผู้ใช้เป็นภาษาไทยเสมอ; แปลภาษา i18n ครบถ้วนทั้ง `th.json` และ `en.json` (100% key parity); ห้าม Hardcode ข้อความบน UI
- **Strict TypeScript:** ห้ามใช้ `any`, `as any` หรือ `@ts-ignore` เด็ดขาด; ใช้ strict types หรือ `unknown` แล้ว narrow
- **Design System:** *Atelier Architectural Navy Sharp* (`border-radius: 0px !important`, Solid Navy `#0B3056`, Pure SVG Stroke Icons, ห้าม CDN ภายนอก)
- **Deferred Upload:** การอัปโหลดโลโก้หรือรูปลายเซ็นต้องใช้ Custom Hook `useDeferredFileUpload` อัปโหลดจริงใน `onSubmit` เท่านั้น
- **Concurrency & RFC 7807:** การบันทึกแก้ไขเทมเพลตต้องใช้ `If-Match` RowVersion/ETag ป้องกันแก้ไขทับ (409 Conflict) และคืน Error แบบ RFC 7807 Problem Details
- **Template Immutability on Issued Docs:** การแก้เทมเพลตต้องไม่มีผลย้อนหลังกับเอกสารในอดีตที่ Issued/Accepted ไปแล้ว (คงสภาพ SHA-256 Hash เดิม)
- **Verification Gates:** ต้องผ่าน `dotnet build`, `dotnet test`, `npm run build`, `npm run lint` ก่อนส่งมอบ

---

## Review Focus

1. **JSON Schema Evolution:** การ deserialize `configuration_json` หากมีฟิลด์ใหม่เพิ่มเข้ามาในอนาคต ต้องมี default fallback ที่ปลอดภัย ไม่พังเอกสารเดิม
2. **Multi-Branch Isolation:** เทมเพลตที่ผูกกับ `branch_id` ต้องไม่ถูกดึงไปแสดงข้ามสาขา และองค์กรต้องมี `is_default` template หลัก 1 ตัวเสมอต่อ `document_type`
3. **QuestPDF Typography & Alignment:** การจัดวางโลโก้และฟอนต์สารบรรณ (Sarabun) ใน PDF ต้องตรงกับหน้าเว็บ ไม่เกิดสระลอยหรือข้อความล้นตกขอบหน้า A4
4. **Permission & Security:** มีเฉพาะผู้ถือสิทธิ์ `document-templates.manage` (หรือผู้ดูแลระบบ) เท่านั้นที่สามารถสร้าง แก้ไข หรือลบเทมเพลตเอกสารได้
5. **No File Orphan:** การเปลี่ยนโลโก้เทมเพลตต้องจัดการอ้างอิงกับโมดูล Files อย่างถูกต้อง ไม่ทิ้งไฟล์ขยะค้างในระบบ

---

## Task Breakdown

### Task 1: Domain Entity & Database Migration (Backend)
- [ ] สร้าง Entity `DocumentTemplate` ใน `backend/src/TanErp.Domain/DocumentTemplates/`
  - Properties: `Id`, `OrganizationId`, `BranchId` (nullable), `DocumentType`, `Name`, `IsDefault`, `ConfigurationJson`, `LogoFileId` (nullable), `CreatedAtUtc`, `UpdatedAtUtc`, `RowVersion`
  - Value Record `DocumentTemplateLayoutConfig`:
    - `HeaderConfig` (ShowLogo, LogoPosition, LogoHeight, CompanyNameTh, CompanyNameEn, TaxId, Address, Phone, Email, DocumentTitle)
    - `ColumnsConfig` (ShowSku, ShowSpecs, ShowThumbnail, ShowLineDiscount)
    - `SummaryConfig` (ShowVat, ShowWithholdingTax, ShowBahtText)
    - `TermsConfig` (ValidityPeriodText, CommercialTermsText, ShowBankDetails, BankName, AccountNo, AccountName, FooterNote)
    - `SignaturesConfig` (SignatureCount: 2 | 3, Box1Label, Box2Label, Box3Label)
- [ ] สร้าง EF Core Entity Configuration ใน `backend/src/TanErp.Infrastructure/Persistence/DocumentTemplates/DocumentTemplateConfiguration.cs`
  - Mapping schema `common.document_templates`
  - Unique Index: `(organization_id, document_type, name)` และ Partial Unique Index: `(organization_id, document_type) WHERE is_default = true AND branch_id IS NULL`
- [ ] เพิ่ม `DbSet<DocumentTemplate> DocumentTemplates` ใน `AppDbContext.cs`
- [ ] สร้าง Migration Script และ Apply เข้าสู่ PostgreSQL Database
- [ ] เขียน Unit Tests ตรวจสอบ Domain Entity, Validation Rules, และ RowVersion Hashing ใน `TanErp.UnitTests/DocumentTemplates/DocumentTemplateTests.cs`

### Task 2: Application CQRS Handlers & Validation (Backend)
- [ ] สร้าง DTOs และ Contracts:
  - `DocumentTemplateResponse`, `DocumentTemplateLayoutConfigDto`, `CreateDocumentTemplateRequest`, `UpdateDocumentTemplateRequest`
- [ ] สร้าง `ListDocumentTemplatesQuery` & Handler:
  - ดึงรายการเทมเพลตทั้งหมดขององค์กร กรองตาม `documentType` และ `branchId`
  - รวมการประเมิน default fallback หากองค์กรยังไม่มีการบันทึกเทมเพลตเฉพาะ
- [ ] สร้าง `GetDocumentTemplateByIdQuery` & Handler:
  - ดึงข้อมูลเทมเพลตระบุ ID พร้อมตรวจสอบ Organization boundary
- [ ] สร้าง `CreateDocumentTemplateCommand` & Handler:
  - ตรวจสอบสิทธิ์ `document-templates.manage`
  - ตรวจสอบชื่อเทมเพลตไม่ซ้ำ
  - จัดการสลับ `is_default` เดิมออกหากมีการตั้งเทมเพลตใหม่เป็น Default
- [ ] สร้าง `UpdateDocumentTemplateCommand` & Handler:
  - ตรวจสอบ `If-Match` RowVersion ป้องกัน Version Conflict (409)
  - อัปเดต Layout Config และบันทึก `UpdatedAtUtc`
- [ ] สร้าง `DeleteDocumentTemplateCommand` & Handler:
  - ห้ามลบเทมเพลตที่เป็น `is_default` หรือเทมเพลตระบบ
- [ ] เขียน Unit Tests ครอบคลุม Handlers ทั้ง 5 ตัวใน `TanErp.UnitTests/DocumentTemplates/`

### Task 3: API Controller & Integration Tests (Backend)
- [ ] สร้าง `DocumentTemplatesController.cs` ใน `backend/src/TanErp.Api/Controllers/`:
  - `GET /api/v1/settings/document-templates` (List)
  - `GET /api/v1/settings/document-templates/{id}` (Get by Id)
  - `POST /api/v1/settings/document-templates` (Create)
  - `PUT /api/v1/settings/document-templates/{id}` (Update with ETag)
  - `DELETE /api/v1/settings/document-templates/{id}` (Delete)
  - `POST /api/v1/settings/document-templates/preview` (Generate live preview sample)
- [ ] สร้าง Permission Definition `document-templates.manage` ใน `PermissionsCatalog.cs`
- [ ] เขียน Integration Tests ใน `TanErp.IntegrationTests/Controllers/DocumentTemplatesControllerTests.cs`:
  - ทดสอบ 401 Unauthorized / 403 Forbidden เมื่อไม่มีสิทธิ์
  - ทดสอบ Create, Read, Update Concurrency (409 Conflict), และ Delete
  - ทดสอบการรักษาความปลอดภัยข้ามองค์กร (Cross-tenant Isolation)

### Task 4: QuestPDF Dynamic Layout Integration (Backend)
- [ ] อัปเดต `QuestPdfQuotationRenderer.cs` (และ Renderer Interfaces):
  - รองรับการรับ `DocumentTemplateLayoutConfig` เข้าไปกำหนดการ Render หน้า PDF
  - ปรับใช้โลโก้องค์กรจาก `LogoFileId` (ถ้ามี)
  - ปรับการแสดงผลคอลัมน์ สเปก ส่วนลด เงื่อนไขการชำระเงิน และบล็อกลายเซ็นตาม Config
- [ ] เขียน Unit/Integration Tests ตรวจสอบว่า PDF ที่ออกมาสอดคล้องกับพารามิเตอร์ของเทมเพลต

### Task 5: Frontend API Client & State Hooks (Frontend)
- [ ] อัปเดต TypeScript Types และ API Client ใน `frontend/src/lib/api/api-client.ts`:
  - เพิ่มเมธอด `listDocumentTemplates`, `getDocumentTemplate`, `createDocumentTemplate`, `updateDocumentTemplate`, `deleteDocumentTemplate`
- [ ] สร้าง React Query Hooks ใน `frontend/src/features/settings/document-templates/api/document-template-queries.ts`:
  - `useDocumentTemplates(documentType, branchId)`
  - `useDocumentTemplate(id)`
  - `useCreateDocumentTemplate()`
  - `useUpdateDocumentTemplate()`
  - `useDeleteDocumentTemplate()`

### Task 6: Frontend Template Studio UI (Atelier Architectural Navy Sharp)
- [ ] สร้าง Reusable Live Paper Visualizer `<A4DocumentPreview />`:
  - แสดงผลสัดส่วนกระดาษ A4 เสมือนจริง (`210mm x 297mm`)
  - อัปเดตตาม State ของฟอร์มแบบ Reactive
  - รองรับ Print Stylesheet (`@media print`) สำหรับการสั่งพิมพ์โดยตรง
- [ ] สร้าง Split-Pane Form Studio Page ที่ `frontend/src/app/[locale]/(erp)/settings/document-templates/`:
  - แผงตั้งค่าฝั่งซ้ายพร้อม `<FormTabs />` 4 แท็บ:
    1. *Branding & Header* (อัปโหลดโลโก้ด้วย `useDeferredFileUpload`, ข้อมูลภาษี, ตำแหน่งโลโก้)
    2. *Columns & Items Table* (สวิตช์เปิด/ปิด SKU, สเปก, ส่วนลด, VAT 7%, หัก ณ ที่จ่าย)
    3. *Terms & Payment* (ระยะเวลายืนราคา, เงื่อนไขการชำระเงิน, ข้อมูลบัญชีธนาคาร)
    4. *Signatures* (จำนวนกล่อง 2/3, ชื่อตำแหน่ง, ระบุวันที่)
  - แผงขวา: Live A4 Visualizer พร้อมปุ่ม Zoom (85%, 100%, 115%) และปุ่มพิมพ์
  - ตัวเลือกสลับประเภทเอกสาร (Quotation, Estimate, PO, Billing, Goods Receipt)
  - ปุ่ม `[+ สร้างเทมเพลตใหม่]` และปุ่มบันทึกพร้อมสถานะ Loading / Double Submit Protection
- [ ] เพิ่มคำแปลภาษา i18n ใน `messages/th.json` และ `messages/en.json` ครบทุกคำ (Zero missing keys)
- [ ] เพิ่มลิงก์เมนูใน Settings Navigation (`/settings`)
- [ ] เขียน Vitest Component Tests ใน `frontend/src/features/settings/document-templates/`:
  - ทดสอบการสลับแท็บ, การอัปเดตฟิลด์แล้วหน้ากระดาษเปลี่ยนตาม, การตรวจสอบ Validation Schema

### Task 7: Verification Gates & Release Readiness
- [ ] รันการตรวจสอบ Backend:
  ```bash
  dotnet build backend/src/TanErp.Api/TanErp.Api.csproj
  dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj
  dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj
  ```
- [ ] รันการตรวจสอบ Frontend:
  ```bash
  cd frontend
  npm run lint
  npm test
  npm run build
  ```
- [ ] ยืนยันผลลัพธ์ว่าผ่าน 100% ไม่มีข้อผิดพลาด (0 Errors, 0 Warnings) ก่อนส่งมอบงาน

---

## Definition of Done

1. มีตารางฐานข้อมูลและ API รองรับการสร้าง/แก้ไข/ลบเทมเพลตเอกสารแยกตามประเภทเอกสารและสาขา
2. หน้าจอ UI รองรับการปรับแต่งแบบ Live Split-Pane A4 Preview ตามมาตรฐาน *Atelier Architectural Navy Sharp*
3. มีระบบ Concurrency Control ป้องกันการบันทึกทับด้วย ETag/If-Match
4. โลโก้ถูกอัปโหลดด้วย Deferred Upload ไม่เกิดไฟล์ขยะ
5. แปลภาษาครบทั้งไทย (`th`) และอังกฤษ (`en`)
6. ผ่านการทดสอบ Unit Tests, Integration Tests, และ Build ทั้ง Backend & Frontend 100%
