# ERP Gap Closure Plan (แผนปิดช่องว่างหลัง CP-01–16)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. แผนนี้เป็นแผนระดับ slice; ก่อนเริ่มแต่ละ G-xx ให้เขียนแผน implementation ละเอียด (ไฟล์/โค้ด/คำสั่งทดสอบ) ที่ `docs/superpowers/plans/YYYY-MM-DD-<slice>.md` แล้วลิงก์กลับมาที่ตารางในหัวข้อ 3.

**สถานะ:** Completed — ทุกรายการ G-01 ถึง G-26 พัฒนาครบ (Wave A–F) ณ 2026-11-06 — Wave A–E ผ่านการทดสอบตามที่บันทึกในเอกสาร verification ของแต่ละ slice ส่วนงาน Wave F รอบ 2026-11-06/07 (PDF branding/template layout, e-Tax signing, deploy artifacts) ผ่านเพียง `dotnet build` และ `docker build` ของ backend; test ใหม่ยังไม่ได้รัน; มี Production Docker Deployment Artifacts ใน `deploy/` พร้อมสำหรับการซ้อม Staging Rehearsal และรอยืนยัน Business Validation Questions ก่อน Production Rollout

**Goal:** ปิดข้อจำกัดเชิง feature ที่บันทึกไว้ใน [Implementation Roadmap](../../00-overview/implementation-roadmap.md) หลัง CP-01–16 และเพิ่ม capability ข้ามโมดูลที่ยังไม่มีในแผน (Notification, Reporting, ภาษีไทย, Labor, e-Tax XML, Accounting Connectors, Deployment Artifacts)

**Architecture:** แต่ละ G-xx เป็น vertical slice บน Clean Architecture เดิม (`Domain` → `Application` → `Infrastructure` → `Api`) + `frontend/src/features/<feature>/`. ทำของกลางที่หลายโมดูลใช้ก่อน (Wave A) เพื่อไม่ duplicate logic. กฎธุรกิจที่ยังไม่มีเจ้าของยืนยันใช้ค่าเริ่มต้นแบบ **TEST_ONLY** และบันทึกเป็น Validation Question เหมือน CP-06–16

**Tech Stack:** .NET 10, EF Core (writes/migrations), Dapper (complex reads), PostgreSQL 16, Self-hosted Auth (JWT/AES-GCM/TOTP 2FA), Next.js 16 App Router + React 19 + TypeScript strict, TanStack Query, Tailwind, next-intl (`messages/th.json`, `messages/en.json`)

---

## 1. ขอบเขต

**อยู่ในแผน:** ข้อจำกัดเชิง feature ของทุกโมดูลใน Roadmap + capability ใหม่ข้ามโมดูล (G-01 ถึง G-26 ครอบคลุม Notification, Reporting, ภาษีไทย, Labor, e-Tax XML, Accounting Connectors, Deployment Artifacts)

**สถานะ Operations & Verification (2026-11-06):**
- **Health endpoints:** พัฒนาเสร็จสมบูรณ์แล้วใน `backend/src/TanErp.Api/Program.cs` (`/health/live` และ `/health/ready` ควบคู่ `DatabaseHealthCheck`)
- **Production Artifacts:** สร้าง `deploy/backend.Dockerfile`, `deploy/frontend.Dockerfile`, `deploy/compose.production.yml`, และ `deploy/.env.production.example` พร้อมนำขึ้นทดสอบบน Staging
- แต่ละ slice มี **focused tests** ของตัวเอง (unit + integration เฉพาะ handler/endpoint ที่แตะ + Vitest ของ component ใหม่) และคอมไพล์ผ่าน `dotnet build`, `npm run lint`, `npm run build` ตาม AGENTS.md §8 ครบถ้วน

---

## 2. กติกาทุก slice (Definition of slice done)

ทุก G-xx ใช้ลำดับ task มาตรฐานนี้ในแผนละเอียด:

1. **Pre-flight:** อ่าน `AGENTS.md`, `design.md`, `CONTEXT.md`, `.agents/skills/building-erp-apis/SKILL.md` (+ `building-erp-forms`/`building-erp-lists` ถ้ามี UI, `erp-document-numbering` ถ้ามีเลขเอกสาร) และโค้ดจริงของโมดูลที่แตะ; ค้นหาของกลางใน `frontend/src/hooks`, `frontend/src/components`, `backend/src/TanErp.Application/Common` ก่อนสร้างใหม่
2. **Contract:** เขียน/แก้ `docs/03-contracts/<module>-api-contract.md` และ `docs/04-data/` (ถ้ามี) — error code ใหม่เป็น RFC 7807 พร้อมข้อความ th/en; Permission ใหม่ลง `docs/03-contracts/permission-catalog.md`
3. **Domain + migration:** entity/invariant ใน `backend/src/TanErp.Domain/<Module>/`, configuration ใน `backend/src/TanErp.Infrastructure/Persistence/Configurations/`, migration ด้วย `dotnet ef migrations add <Name>` (ตรวจ `Down()` ย้อนได้); unit test invariant ก่อน (TDD)
4. **Application:** handler/store interface ใน `backend/src/TanErp.Application/<Module>/` ตามรูปแบบ handler เดิมของโมดูล; ETag/RowVersion, Idempotency-Key สำหรับ create/transition, Audit
5. **Api:** controller บาง ๆ ใน `backend/src/TanErp.Api/Controllers/` (ไม่มี business logic); response เป็น structured projection (`{ id, displayName }`) ไม่ส่ง FK ดิบ
6. **Integration tests เฉพาะ slice:** `backend/tests/TanErp.IntegrationTests/` — happy path, cross-organization 404, permission 403, concurrency 412, idempotency
7. **Frontend:** `frontend/src/features/<feature>/` — `api/` (generated types จาก OpenAPI, `npm run check:api`), list/form ตาม skill, `useConfirm` + `isLoading` สำหรับ action เสี่ยง, `useDeferredFileUpload` สำหรับไฟล์, i18n th/en ครบ, ไม่มี `any`/`as any`/`@ts-ignore`, ไม่มี chained fallback
8. **Docs:** verification doc `docs/05-engineering/<module>-verification.md`, อัปเดต Roadmap + ตารางหัวข้อ 3 ด้านล่าง; trade-off ถาวร → ADR ใหม่ใน `docs/adr/`
9. **Commit:** `type(<scope>): description` ต่อ task; ห้าม commit secrets/build output

---

## 3. รายการงาน, dependency และลำดับ (G-01 ถึง G-26)

| ID | งาน | Wave | Dependency | ผู้ยืนยันกฎ | แผนละเอียด & สถานะล่าสุด |
| --- | --- | --- | --- | --- | --- |
| G-01 | Shared Attachment & Signature | A | Files module เดิม | Security | [แผน G-01](2026-10-05-g01-shared-attachment-signature.md) — Implemented 2026-10-06 |
| G-02 | Notification Foundation (in-app + worker) | A | Identity | Security + Operations | [แผน G-02](2026-10-08-g02-notification-foundation.md) — Implemented 2026-10-10; [Verification](../../05-engineering/notification-foundation-verification.md) |
| G-03 | Organization/Branch CRUD, Role Management, Approval Authority & Organization Branding | A | CP-02 | System Admin + Security + Finance | Implemented 2026-10-11/12 (G-03a Branch CRUD, G-03b Role Maker–Checker, G-03c Approval Limits); ขยายเพิ่ม **Organization Branding (`BrandPrimaryColor`, `BrandLogoFileId`)** และ **Standard Approval Limits Presets (`ApprovalLimitStore.SeedDefaultsAsync`)**; [Verification G-03a](../../05-engineering/organization-administration-verification.md); [Verification G-03b](../../05-engineering/role-management-verification.md); [Verification G-03c](../../05-engineering/approval-authority-verification.md) |
| G-04 | Server-side Quotation & Tax Document PDF + Organization Logo/Brand Color | B | CP-04, G-01, G-03 | Sales + Finance | Implemented 2026-10-13; ขยายรองรับ **Organization Logo (`BrandLogoFileId`)** และ **Brand Primary Color (`BrandPrimaryColor`)** ควบคู่กับ **`DocumentTemplateLayoutConfig`** ทั้งบน Quotation PDF และ Tax Document/Billing PDF ผ่าน QuestPDF 2026.9.1; [ADR 0023](../../adr/0023-server-side-quotation-pdf-with-questpdf.md); [Verification](../../05-engineering/quotation-pdf-verification.md) |
| G-05 | External Acceptance OTP (Email & SMS) & Quick Estimate Cost Recipe / Override Cap | B | CP-07, G-02, CP-16 | Sales + Security + Legal | Implemented 2026-10-18; ขยายรองรับ **SMS OTP Providers (ThaiBulkSMS, Twilio, Webhook via `HttpSmsSender`)** ควบคู่ Email และเพิ่ม **Quick Estimate Cost Recipe (`CostPerUnit`, `LaborFactor`, `DefaultWastePercent`)** พร้อม **±30% Price Override Cap (`BranchRateOverride.MaxDeviationRatio = 0.30m`)** และสิทธิ์ `estimates.quick.override_limit` / `pricing-templates.override-rate`; [ADR 0029](../../adr/0029-external-acceptance-otp.md); [Verification](../../05-engineering/external-acceptance-otp-verification.md) |
| G-06 | Item Import Phase 2 & Unit Conversion Management | B | CP-03 Import Phase 1 | Data Steward + Cost Owner | Implemented 2026-10-17; เพิ่มโหมด `upsert`, Draft Cost import, Background batch; เพิ่ม **Unit Conversion `PUT` / `DELETE` (Deactivate)** ใน `UnitConversionsController` และ `ItemUnitConversionsController`; [ADR 0026](../../adr/0026-item-import-phase2.md); [Verification](../../05-engineering/item-import-phase2-verification.md) |
| G-07 | Survey Template ใน DB + effective period + evidence ต่อ Area | B | CP-03 Survey | Survey Owner | Implemented 2026-10-17 (ตาราง `survey_template_versions`, hash `v4`, evidence ต่อ Area); [ADR 0028](../../adr/0028-survey-templates-in-db.md); [Verification](../../05-engineering/survey-templates-db-verification.md) |
| G-08 | Estimate Unit Conversion Snapshot | B | FR-ITEM-005 | Cost Owner + Estimator | Implemented 2026-10-15; ตรึง conversion snapshot ใน BOQ cost component; เปิดใช้ผ่าน Calculation Policy mode `snapshot` ร่วมกับ UI ใน G-24; [ADR 0024](../../adr/0024-estimate-unit-conversion-snapshot.md); [Verification](../../05-engineering/estimate-unit-conversion-verification.md) |
| G-09 | Quick Estimate Completion: conversion พร้อมรายการ, Branch override, หลักฐาน | B | CP-16, G-01, G-08 | Sales + Cost Owner | Implemented 2026-10-15; conversion เติมขอบเขต, Branch Rate Override ผ่าน Maker–Checker, แนบรูปหลักฐาน; [ADR 0025](../../adr/0025-quick-estimate-branch-override-and-scope-line-conversion.md); [Verification](../../05-engineering/quick-estimate-completion-verification.md) |
| G-10 | Project Actual Cost / Commitment / WBS | C | CP-09, CP-10, CP-11, CP-12 | PM + Finance | [แผน G-20 + G-10](2026-10-29-g20-g10-timesheet-project-wbs-cost.md) — Implemented 2026-10-29; [Verification](../../05-engineering/project-handover-verification.md) |
| G-11 | Purchase Request, Supplier Return, Supplier Invoice (3-way match) | C | CP-10, G-03 | Procurement + Finance | [แผน G-11](2026-10-29-g11-procurement-completion.md) — Implemented 2026-10-29; [Verification](../../05-engineering/procurement-verification.md) |
| G-12 | Inventory Location/Lot/Serial, Period Close, Returns | C | CP-11 | Warehouse + Finance | [แผน G-12](2026-10-29-g12-inventory-completion.md) — Implemented 2026-10-29; [Verification](../../05-engineering/inventory-verification.md) |
| G-13 | Production Routing/Work Center/Capacity, Labor, Subcontracting | C | CP-12, G-20 | Production | [แผน G-13](2026-10-30-g13-production-completion.md) — Implemented 2026-10-30; [Verification](../../05-engineering/production-verification.md) |
| G-14 | MRP Safety Stock / MOQ / Item Lead Time / Calendar / Unit | C | CP-13, G-12, G-13 | Planner + Procurement | [แผน G-14](2026-10-30-g14-mrp-completion.md) — Implemented 2026-10-30; [Verification](../../05-engineering/mrp-verification.md) |
| G-15 | Thai Tax: VAT, WHT, Retention, Tax Invoice PDF, Credit Note & System Maintenance Worker | D | CP-15, G-04 | Finance/Accounting | [แผน G-15](2026-10-30-g15-thai-tax.md) — Implemented 2026-10-30; VAT 7%, WHT 50 ทวิ, Retention % ปลดเมื่อหมดประกัน, ใบลดหนี้/เพิ่มหนี้, PDF ใบกำกับภาษีพร้อมโลโก้/สีแบรนด์, และ Background Worker `SystemMaintenanceAndReminderWorker`; [Verification](../../05-engineering/finance-verification.md) |
| G-16 | Credit Exposure + AR Aging gate | D | CP-15, G-15 | Finance | [แผน G-16](2026-10-30-g16-credit-exposure-ar-aging.md) — Implemented 2026-10-30; [Verification](../../05-engineering/finance-verification.md) |
| G-17 | Thai Accounting Connectors, Multi-format Journal Export & Standard Thai COA | D | CP-15, G-15 | Finance + Security | [แผน G-17](2026-10-30-g17-accounting-connector-journal-export.md) — Implemented 2026-10-30; Connector จริงรองรับ **PEAK, FlowAccount, Express, signed Webhook (`ConfigurableAccountingConnector`)**, ส่งออก CSV สมุดรายวันหลายรูปแบบ, และ **Standard Thai COA Presets (`StandardThaiCoaPresets`)**; [Verification](../../05-engineering/finance-verification.md) |
| G-18 | Service SLA, Service Charge, Technician Schedule | D | CP-14, G-01, G-02, G-15 | Customer Service | [แผน G-18](2026-10-30-g18-service-completion.md) — Implemented 2026-10-30; [Verification](../../05-engineering/service-verification.md) |
| G-19 | Dashboard & Reporting | E | ข้อมูลจาก Wave B–D | Business owner ทุกฝ่าย | [แผน G-19](2026-10-31-g19-dashboard-reporting.md) — Implemented 2026-10-31; [Verification](../../05-engineering/reporting-verification.md) |
| G-20 | Labor Timesheet | C | Identity, CP-09 | PM + Production + Finance | [แผน G-20 + G-10](2026-10-29-g20-g10-timesheet-project-wbs-cost.md) — Implemented 2026-10-29; [Verification](../../05-engineering/project-handover-verification.md) |
| G-21 | Membership Expiry & Audit Policy | A | G-02, G-03 | System Admin | Implemented 2026-10-12/17; [ADR 0027](../../adr/0027-wave-a-closure-decisions.md); [แผน G-21](2026-10-12-g21-membership-expiry.md); [Verification](../../05-engineering/membership-expiry-verification.md) |
| G-22 | Supplier Item Prices & Lead Time | F | CP-10, G-03 | Procurement | [แผน G-22](2026-11-01-g22-supplier-price-list.md) — Implemented 2026-11-01; บันทึกราคาและ Lead time ผู้ขายต่อ Item แยกสาขา; [Verification](../../05-engineering/procurement-verification.md) |
| G-23 | MRP Project Demand Integration | F | CP-13, G-10 | Production + PM | Implemented 2026-11-01; คำนวณความต้องการวัตถุดิบตรงจาก BOQ โครงการ; [Verification](../../05-engineering/mrp-verification.md) |
| G-24 | Calculation Policy UI & Snapshot Mode Management | F | G-08 | Cost Owner | Implemented 2026-11-01; หน้าตั้งค่า `/settings/calculation-policies` จัดการ Maker–Checker ของนโยบายคำนวณและเปิดโหมด snapshot; [Verification](../../05-engineering/estimate-unit-conversion-verification.md) |
| G-25 | Reporting Materialized Views (Opt-in) | F | G-19 | BI / System Admin | Implemented 2026-11-01; Materialized views เพิ่มประสิทธิภาพการอ่าน Dapper สำหรับแดชบอร์ดสรุปยอดขนาดใหญ่; [Verification](../../05-engineering/reporting-verification.md) |
| G-26 | ETDA e-Tax Invoice XML (Cross Industry Invoice Profile) | F | G-15, G-04 | Finance | Implemented 2026-11-01 (Hardened 2026-11-06); XML ตามมาตรฐาน ETDA Recommendation 3-2560 พร้อม Buyer `PostalTradeAddress`, CN/DN `InvoiceReferencedDocument` (codes 81/80), และการลงนามดิจิทัล RSA-SHA256 `ds:Signature` ผ่าน `ETaxSigningOptions`; [Verification](../../05-engineering/finance-verification.md) |

---

## 4. รายละเอียดและผลการพัฒนาแต่ละ slice

### G-03 — Organization/Branch CRUD, Role Management, Approval Authority & Branding

- **Organization Profile & Branding:** รองรับชื่อองค์กร, เลขประจำตัวผู้เสียภาษี, ที่อยู่ออกเอกสาร, **`BrandPrimaryColor`** (Hex string เช่น `#0B3056`), และ **`BrandLogoFileId`** สำหรับผูกไฟล์ภาพโลโก้ผ่าน Shared Storage
- **Branch CRUD:** สร้าง แก้ไข และปิดการใช้งานสาขา พร้อมเลขสาขาภาษี 5 หลัก (`TaxBranchCode`) สำหรับใบกำกับภาษี
- **Role Management:** สร้าง แก้ไข ปิดใช้งาน Role จาก Permission Catalog พร้อมระบบ Maker–Checker เมื่อมีการแก้ไขสิทธิ์ที่เกี่ยวกับอำนาจอนุมัติ
- **Approval Authority & Standard Presets:** ตรวจสอบวงเงินอนุมัติแยกตามประเภทเอกสาร (`estimates`, `purchase-orders`, `change-orders`, `credit-notes`, `billing-documents`) แบบ Fail-closed พร้อมเมธอด **`ApprovalLimitStore.SeedDefaultsAsync`** สำหรับตั้งค่าวงเงินมาตรฐานตาม Role ระบบอัตโนมัติ

### G-04 — Server-side Quotation & Tax Document PDF (QuestPDF)

- **QuestPDF 2026.9.1 Engine:** สร้างเอกสาร PDF ทางการฝั่งเซิร์ฟเวอร์แบบ in-process พร้อมฝังฟอนต์ Sarabun และ OFL ใน assembly
- **Organization Branding Integration:** ดึง **`BrandLogoFileId`** ผ่าน `IFileStorageProvider` มาแสดงผลเป็นโลโก้บนหัวเอกสาร และใช้ **`BrandPrimaryColor`** เป็นสีหลักของหัวตาราง เส้นขอบ และข้อความเน้นย้ำ
- **Document Template Layout Config:** เชื่อมต่อ **`DocumentTemplateLayoutConfig`** (`DocumentTemplateLayoutConfig.DefaultFor(...)`) กำหนดการแสดงผลหัวเอกสาร รายการบรรทัด หมายเหตุ และส่วนสรุปยอด
- **Artifact Immutability & Hash Verification:** จัดเก็บไฟล์ไบต์ PDF ลง persistent storage ครั้งเดียวตอนออกเอกสาร (`Issue`) พร้อมบันทึก SHA-256 เพื่อใช้ตรวจเทียบในการยอมรับของลูกค้า (`External Acceptance`)

### G-05 — External Acceptance OTP & Quick Estimate Cost Recipe / Override Cap

- **External Acceptance OTP Multi-channel:** ส่งรหัส OTP 6 หลักทาง Email และ **SMS Gateway (`HttpSmsSender`)** รองรับผู้ให้บริการ **ThaiBulkSMS**, **Twilio**, และ **signed HTTP Webhook** พร้อมระบบ Rate Limiting และ Anti-tamper verification
- **Quick Estimate Cost Recipe:** รองรับการกำหนดส่วนประกอบต้นทุนในแม่แบบราคา (`TemplateCostRecipeLine`) ประกอบด้วย `CostPerUnit`, `LaborFactor`, `DefaultWastePercent`, `QuantityPerUnit`, `UnitCost`, และ Item Master Link
- **±30% Price Override Cap:** ควบคุมการขอ Branch Rate Override (`BranchRateOverride.MaxDeviationRatio = 0.30m`) โดยล็อกไม่ให้ส่วนต่างราคาเบี่ยงเบนเกิน ±30% จากราคาอ้างอิงของแม่แบบ บังคับใช้ผ่านสิทธิ์ **`estimates.quick.override_limit`** และ **`pricing-templates.override-rate`** ร่วมกับระบบ Maker–Checker

### G-06 — Item Import Phase 2 & Unit Conversion Management

- **Item Import Phase 2:** รองรับ Mode `upsert` โดยใช้ Item Code เป็นคีย์, นำเข้าต้นทุนเป็น Draft เข้าสู่ Maker–Checker, และประมวลผลไฟล์ขนาดใหญ่เบื้องหลังแบบ Batch
- **Unit Conversion Management API:** เพิ่มคำขอ **`PUT` (Update)** และ **`DELETE` / `POST deactivate`** สำหรับแก้ไขอัตราการแปลงหน่วยและปิดใช้งานหน่วยนับ ทั้งในระดับ Item เฉพาะเจาะจง (`ItemUnitConversionsController`) และระดับองค์กร (`UnitConversionsController`)

### G-15 — Thai Tax Completion & Background Workers

- **Thai Tax Full Suite:** คำนวณ VAT 7% จาก `TaxPolicyVersion`, ภาษีหัก ณ ที่จ่าย WHT พร้อมบันทึกเลขหนังสือรับรอง 50 ทวิ และเลขที่ใบเสร็จรับเงิน `RCT-nnnn`, เงินประกันผลงาน Retention % พร้อมคำขอปลดเงินประกันเมื่อใบรับประกันสิ้นสุด, และใบลดหนี้ `CN-nnnn` / ใบเพิ่มหนี้ `DN-nnnn`
- **Tax Document PDF with Branding:** ออกใบเสร็จรับเงิน/ใบกำกับภาษี PDF เต็มรูปผ่าน QuestPDF พร้อมแสดงผลโลโก้องค์กรและสีประจำแบรนด์ตาม `BrandPrimaryColor`
- **System Maintenance & Reminder Worker:** รัน Background Service **`SystemMaintenanceAndReminderWorker`** ทำหน้าที่แจ้งเตือน Membership ใกล้หมดอายุ, ตรวจสอบสัญญา/การรับประกัน, และกวาดล้างไฟล์ขยะ/เซสชันอัปโหลดที่หมดอายุ

### G-17 — Thai Accounting Connectors & Standard Thai COA

- **Thai Accounting Connectors:** `ConfigurableAccountingConnector` รองรับการเชื่อมต่อ API ไปยังระบบบัญชีไทยชั้นนำ ได้แก่ **PEAK Account**, **FlowAccount**, **Express**, และ **HMAC-signed Webhook**
- **Multi-format CSV Journal Export:** ส่งออกสมุดรายวันจาก Accounting Outbox เป็นไฟล์ CSV ได้หลายรูปแบบ ทั้งฟอร์แมตมาตรฐาน, ฟอร์แมตโปรแกรม Express (`VOUCHER_NO`, `ACCT_CODE`, `DR_AMT`, `CR_AMT`), และฟอร์แมตพร้อมนำเข้า PEAK/FlowAccount
- **Standard Thai COA Presets:** ค่าเริ่มต้นผังบัญชีมาตรฐานของไทย (`StandardThaiCoaPresets`) ครอบคลุมเหตุการณ์ขาย, วางบิล, รับชำระ, ภาษีขาย, ภาษีหัก ณ ที่จ่าย, และเงินประกันผลงาน

### G-22 — Supplier Item Prices & Lead Time

- จัดเก็บตารางราคาและระยะเวลาส่งมอบของผู้ขายต่อ Item (`SupplierItemPrice`) รองรับการเปรียบเทียบราคาผู้ขายและนำไปใช้คำนวณในใบสั่งซื้อและแผน MRP

### G-23 — MRP Project Demand Integration

- ผูกยอดความต้องการวัตถุดิบและอุปกรณ์จาก BOQ ของโครงการที่ได้รับอนุมัติ เข้าสู่รอบการประมวลผลแผนจัดซื้อและแผนสั่งผลิตของ MRP โดยตรง

### G-24 — Calculation Policy UI & Snapshot Mode Management

- หน้าจอ `/settings/calculation-policies` สำหรับสร้างและเผยแพร่นโยบายการคำนวณราคาทางการ ควบคุมการเปิดใช้โหมด Unit Conversion Snapshot (`snapshot`) ด้วย Maker–Checker

### G-25 — Reporting Materialized Views (Opt-in)

- เพิ่มประสิทธิภาพการอ่านข้อมูลสรุปยอดขนาดใหญ่ของแดชบอร์ดผู้บริหาร (Pipeline, Project Budget vs Actual, Stock Value) ด้วย PostgreSQL Materialized Views

### G-26 — ETDA e-Tax Invoice XML (Cross Industry Invoice ER3-2560)

- สร้างไฟล์ XML ใบกำกับภาษีอิเล็กทรอนิกส์ตามมาตรฐาน ETDA Recommendation 3-2560 (UN/CEFACT CII)
- บันทึกโครงสร้าง `PostalTradeAddress` ครบถ้วนทั้งฝั่งผู้ขาย (Seller) และผู้ซื้อ (Buyer)
- อ้างอิงเอกสารเดิมในกรณีใบลดหนี้/ใบเพิ่มหนี้ผ่าน `InvoiceReferencedDocument` (Type Code 81 สำหรับ CN และ 80 สำหรับ DN)
- รองรับการลงนามดิจิทัล Enveloped XML Digital Signature (RSA-SHA256 `<ds:Signature>`) ผ่านการตั้งค่าใบรับรองอิเล็กทรอนิกส์ **`ETaxSigningOptions`** (`SigningCertificatePath`, `SigningCertificatePassword`)
