# ERP Gap Closure Plan (แผนปิดช่องว่างหลัง CP-01–16)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. แผนนี้เป็นแผนระดับ slice; ก่อนเริ่มแต่ละ G-xx ให้เขียนแผน implementation ละเอียด (ไฟล์/โค้ด/คำสั่งทดสอบ) ที่ `docs/superpowers/plans/YYYY-MM-DD-<slice>.md` แล้วลิงก์กลับมาที่ตารางในหัวข้อ 3.

**สถานะ:** Draft — รอผู้ใช้อนุมัติขอบเขตรายการ G-xx ก่อน implementation (ตาม AGENTS.md: Draft/Future ต้องมี task ที่อนุมัติก่อน)

**Goal:** ปิดข้อจำกัดเชิง feature ที่บันทึกไว้ใน [Implementation Roadmap](../../00-overview/implementation-roadmap.md) หลัง CP-01–16 และเพิ่ม capability ข้ามโมดูลที่ยังไม่มีในแผน (Notification, Reporting, ภาษีไทย, Labor)

**Architecture:** แต่ละ G-xx เป็น vertical slice บน Clean Architecture เดิม (`Domain` → `Application` → `Infrastructure` → `Api`) + `frontend/src/features/<feature>/`. ทำของกลางที่หลายโมดูลใช้ก่อน (Wave A) เพื่อไม่ duplicate logic. กฎธุรกิจที่ยังไม่มีเจ้าของยืนยันใช้ค่าเริ่มต้นแบบ **TEST_ONLY** และบันทึกเป็น Validation Question เหมือน CP-06–16

**Tech Stack:** .NET 9, EF Core (writes/migrations), Dapper (complex reads), PostgreSQL, Firebase Auth (identity only), Next.js App Router + TypeScript strict, TanStack Query, Tailwind, next-intl (`messages/th.json`, `messages/en.json`)

---

## 1. ขอบเขต

**อยู่ในแผน:** ข้อจำกัดเชิง feature ของทุกโมดูลใน Roadmap + capability ใหม่ 4 เรื่อง (G-02, G-19, G-15 ส่วนภาษี, G-20)

**ไม่อยู่ในแผนนี้ (ตามคำสั่งผู้ใช้ 2026-10-05):**
- CI (`verify.yml`, PR #12) และ immutable artifact build/publish
- Test ใหญ่: full backend/frontend gate ทั้งชุด, Playwright journey ข้ามโมดูล, authorized-role UAT, staging rehearsal, migration rehearsal บน sanitized legacy copy, restore drill
- Health endpoint — มีแล้ว (`ff2d253`, `backend/src/TanErp.Api/Health/`)

แต่ละ slice ยังต้องมี **focused tests** ของตัวเอง (unit + integration เฉพาะ handler/endpoint ที่แตะ + Vitest ของ component ใหม่) และรัน `dotnet build`, `npm run lint`, `npm run build` ตาม AGENTS.md §8 ก่อนบอกว่าเสร็จ — ไม่นับเป็น "Test ใหญ่"

**Mobile/offline Quick Estimate** ยังเป็น Future (YAGNI จนมีผู้ใช้หน้างานยืนยันความต้องการ)

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
9. **Commit:** `feat(<scope>): ...` ต่อ task; ห้าม commit secrets/build output

## 3. รายการงาน, dependency และลำดับ

| ID | งาน | Wave | Dependency | ผู้ยืนยันกฎ | แผนละเอียด |
| --- | --- | --- | --- | --- | --- |
| G-01 | Shared Attachment & Signature | A | Files module เดิม | Security | [แผน G-01](2026-10-05-g01-shared-attachment-signature.md) — Implemented 2026-10-06 (focused tests; browser check + full gate pending) |
| G-02 | Notification Foundation (in-app เท่านั้น; อีเมลเลื่อน) | A | Identity | Security + Operations | [แผน G-02](2026-10-08-g02-notification-foundation.md) — Implemented 2026-10-10 (in-app only; focused tests; browser check + full gate pending); [Verification](../../05-engineering/notification-foundation-verification.md) |
| G-03 | Organization/Branch CRUD, Role management, Approval Authority matrix | A | CP-02 | System Admin + Security + Finance | G-03a Implemented 2026-10-11 (focused tests; browser check not done in this round); G-03b Implemented 2026-10-11 (verified 2026-10-23: Integration 36/36 passed; browser not checked); G-03c Implemented 2026-10-12 (verified 2026-10-23: Integration 6/6 passed; browser not checked; ยังไม่ผ่าน UAT; fail-closed จนกว่าจะตั้งวงเงินจริง); เตรียม go-live 2026-10-17 (seeder สิทธิ์ระบบ + seed วงเงิน opt-in + [runbook](../../05-engineering/wave-a-go-live-runbook.md); verified 2026-10-23: Unit + Integration passed; browser not checked); [แผน G-03](2026-10-10-g03-org-role-authority.md); [Verification G-03a](../../05-engineering/organization-administration-verification.md); [Verification G-03b](../../05-engineering/role-management-verification.md); [Verification G-03c](../../05-engineering/approval-authority-verification.md) |
| G-04 | Server-side Quotation PDF + artifact hash | B | CP-04, G-01 | Sales + Finance | Implemented 2026-10-13 (verified 2026-10-23: Unit + Integration passed; browser/PDF not visually checked; ยังไม่ผ่าน UAT; **เงื่อนไข license QuestPDF รอ Business ยืนยันก่อน production**; โลโก้ยังไม่ทำ) — [แผน G-04](2026-10-12-g04-quotation-pdf.md); [ADR 0023](../../adr/0023-server-side-quotation-pdf-with-questpdf.md); [Verification](../../05-engineering/quotation-pdf-verification.md) |
| G-05 | External Acceptance OTP | B | CP-07, G-02 | Sales + Security + Legal | Implemented 2026-10-18 (email + SMS-ready structure; verified 2026-10-23: Unit 8/8, Integration 1/1, Vitest 12/12 passed; browser not checked) — [ADR 0029](../../adr/0029-external-acceptance-otp.md); [Verification](../../05-engineering/external-acceptance-otp-verification.md) |
| G-06 | Item Import Phase 2 (upsert / Cost / Batch) | B | CP-03 Import Phase 1 | Data Steward + Cost Owner | Implemented 2026-10-17 (verified 2026-10-23: Unit + Integration + Vitest passed; browser not checked) — [ADR 0026](../../adr/0026-item-import-phase2.md); [Verification](../../05-engineering/item-import-phase2-verification.md) |
| G-07 | Survey Template ใน DB + effective period + evidence ต่อ Area | B | CP-03 Survey | Survey Owner | Implemented 2026-10-17 (verified 2026-10-23: Unit + Integration `v4` area evidence passed; browser not checked) — [ADR 0028](../../adr/0028-survey-templates-in-db.md); [Verification](../../05-engineering/survey-templates-db-verification.md) |
| G-08 | Estimate Unit Conversion Snapshot | B | FR-ITEM-005 | Cost Owner + Estimator | G-08 Implemented 2026-10-15 (verified 2026-10-23: Unit + Integration + Vitest passed; browser not checked; feature OFF until a snapshot-mode calculation policy is published, which has no API yet) — [แผน G-08](2026-10-15-g08-estimate-unit-conversion.md); [ADR 0024](../../adr/0024-estimate-unit-conversion-snapshot.md); [Verification](../../05-engineering/estimate-unit-conversion-verification.md) |
| G-09 | Quick Estimate: conversion พร้อมรายการ, Branch override, หลักฐาน | B | CP-16, G-01, G-08 | Sales + Cost Owner | G-09 Implemented 2026-10-15 (verified 2026-10-23: Unit + Integration + Vitest passed; browser not checked; migration apply เฉพาะ local dev DB; ยังไม่ผ่าน UAT; ร่างที่แปลงมีรายการขอบเขตแต่ยังไม่มีต้นทุน) — [แผน G-09](2026-10-15-g09-quick-estimate-completion.md); [ADR 0025](../../adr/0025-quick-estimate-branch-override-and-scope-line-conversion.md); [Verification](../../05-engineering/quick-estimate-completion-verification.md) |
| G-10 | Project Actual Cost / Commitment / WBS | C | CP-09, CP-10, CP-11, CP-12 | PM + Finance | [แผน G-20 + G-10](2026-10-29-g20-g10-timesheet-project-wbs-cost.md) — Implemented 2026-10-29 (Unit + Integration + Vitest passed; [Verification](../../05-engineering/project-handover-verification.md)) |
| G-11 | Purchase Request, Supplier Return, Supplier Invoice (3-way match) | C | CP-10, G-03 | Procurement + Finance | [แผน G-11](2026-10-29-g11-procurement-completion.md) — Implemented 2026-10-29 (Unit + Integration + Vitest passed; [Verification](../../05-engineering/procurement-verification.md)) |
| G-12 | Inventory Location/Lot/Serial, Period Close, Returns | C | CP-11 | Warehouse + Finance | [แผน G-12](2026-10-29-g12-inventory-completion.md) — Implemented 2026-10-29 (Unit + Integration + Vitest passed; [Verification](../../05-engineering/inventory-verification.md)) |
| G-13 | Production Routing/Work Center/Capacity, Labor, Subcontracting | C | CP-12, G-20 | Production | [แผน G-13](2026-10-30-g13-production-completion.md) — Implemented 2026-10-30 (Unit 679/679 + Integration 6/6 + Vitest 17/17 passed; [Verification](../../05-engineering/production-verification.md)) |
| G-14 | MRP Safety Stock / MOQ / Item Lead Time / Calendar / Unit | C | CP-13, G-12, G-13 | Planner + Procurement | — |
| G-15 | Thai Tax: VAT, WHT, Retention, Tax Invoice, Credit Note | D | CP-15, G-04 | Finance/Accounting | — |
| G-16 | Credit Exposure + AR Aging gate | D | CP-15, G-15 | Finance | — |
| G-17 | Accounting Connector จริง (+ Journal Export ชั่วคราว) | D | CP-15, G-15 | Finance + Security | — |
| G-18 | Service SLA, Service Charge, Technician Schedule | D | CP-14, G-01, G-02, G-15 | Customer Service | — |
| G-19 | Dashboard & Reporting | E | ข้อมูลจาก Wave B–D | Business owner ทุกฝ่าย | — |
| G-20 | Labor Timesheet | C | Identity, CP-09 | PM + Production + Finance | [แผน G-20 + G-10](2026-10-29-g20-g10-timesheet-project-wbs-cost.md) — Implemented 2026-10-29 (Unit + Integration + Vitest passed; [Verification](../../05-engineering/project-handover-verification.md)) |
| G-21 | Membership expiry และ policy ค้างของ CP-02 | A | G-02, G-03 | System Admin | G-21 ข้อ 1 Implemented 2026-10-12 (verified 2026-10-23: Unit + Integration 11/11 passed; browser not checked); ข้อ 2–3 ตัดสิน 2026-10-17 โดย owner delegation (A1, B1+B3, B2 เลื่อน; Legal ต้องยืนยัน retention) และ Implemented 2026-10-17 (verified 2026-10-23: Unit 6/6, Integration 1/1, Vitest 5/5 passed; browser not checked) — [ADR 0027](../../adr/0027-wave-a-closure-decisions.md); [แผน G-21](2026-10-12-g21-membership-expiry.md); [Open Decisions](2026-10-12-g21-open-decisions.md); [Verification](../../05-engineering/membership-expiry-verification.md) |

**ลำดับแนะนำ:** Wave A (ของกลาง) → B (Commercial/Estimate) ↔ C (Project/Supply) ทำคู่ขนานได้ → D (Finance/Service) → E (Reporting). ภายใน Wave C: G-20 → G-10 → G-11 → G-12 → G-13 → G-14.

**Track ไม่ใช่โค้ด (คู่ขนาน):** รวบรวม Validation Question ทุกข้อด้านล่างเป็นเอกสารเดียวให้เจ้าของงานตอบ (`docs/01-business/open-validation-questions.md`) — งานโค้ดใช้ค่า TEST_ONLY ไปก่อนได้ แต่ห้ามเปิดใช้ Production จนกว่าจะตอบ

---

## 4. รายละเอียดแต่ละ slice

### G-01 — Shared Attachment & Signature (ของกลาง)

**เหตุผล Global Reuse:** Service (punch list/handover), Quick Estimate (หลักฐาน), Procurement (ใบเสนอราคาผู้ขาย/ใบแจ้งหนี้), Survey (evidence ต่อ Area) ต้องแนบไฟล์และ/หรือลายเซ็นเหมือนกัน — ทำครั้งเดียวแทนทำในแต่ละโมดูล. **ต้องเสนอผู้ใช้ก่อนสร้าง (AGENTS.md §2)**

**แตะ:** `backend/src/TanErp.Domain/Files/` (มี `UploadedFile`, `FileUploadSession`), `backend/src/TanErp.Application/Files/`, `FilesController.cs`; frontend `frontend/src/hooks/useDeferredFileUpload.ts`, `frontend/src/components/forms/`

- [ ] Contract: `AttachmentLink { ownerType, ownerId, fileId, purpose, createdBy }` แบบ polymorphic owner ที่ whitelist owner type ใน code (ไม่ใช่ string อิสระ) + permission ตาม owner
- [ ] Domain: `AttachmentLink` + invariant (owner type ต้องอยู่ใน registry, ไฟล์ต้อง scope เดียวกับ owner)
- [ ] Signature: `SignatureCapture { signerName, signedAtUtc, imageFileId, consentTextVersion, contentHash }` reuse รูปแบบหลักฐานจาก CP-07 External Acceptance (ย้าย logic ร่วมขึ้น Application/Common แทน copy)
- [ ] Frontend: `<AttachmentList ownerType ownerId />` และ `<SignaturePad />` กลางใน `frontend/src/components/forms/` ใช้ `useDeferredFileUpload` (อัปโหลดตอน submit เท่านั้น)
- [ ] Tests: cross-owner/cross-org access 404, owner type ที่ไม่อยู่ใน registry ถูกปฏิเสธ, ไฟล์ค้างไม่ถูกผูกเมื่อ submit ล้มเหลว

**TEST_ONLY default:** ขนาดไฟล์และชนิดตาม `FileConstants.cs` เดิม; ลายเซ็นเป็นภาพ PNG + hash ไม่ใช่ digital signature ตามกฎหมาย
**Validation Question:** ลายเซ็นต้องเป็น e-Signature ตาม พ.ร.บ. ธุรกรรมอิเล็กทรอนิกส์ระดับใด?
**เกณฑ์จบ:** อย่างน้อย 2 โมดูล (G-18 Service, G-09 Quick Estimate) ใช้ component เดียวกันได้โดยไม่ต้องแก้ของกลาง

### G-02 — Notification Foundation

**แตะ:** ใหม่ `backend/src/TanErp.Domain/Notifications/`, `backend/src/TanErp.Application/Notifications/`, outbox pattern เดียวกับ Finance accounting outbox (`backend/src/TanErp.Infrastructure/Persistence/Finance/`) — ย้าย retry/backoff/dead/requeue ขึ้นเป็นของกลางก่อน; frontend bell ใน `frontend/src/components/layout/`

- [x] ADR 0018: ไม่ใช้ outbox ร่วม และไม่แตะ Finance outbox ([ADR 0018](../../adr/0018-in-app-notification-foundation.md))
- [x] Domain: `Notification` แถวต่อผู้รับ + `NotificationTypes` whitelist ในโค้ด (ดู [สัญญา](../../03-contracts/notification-api-contract.md))
- [x] Event sources รอบแรก: รออนุมัติ 6 แหล่ง (Estimate, Cost, PO, Change Order, MRP, Role maker–checker)
- [ ] (Future) Event ที่ต้องมี scheduler: ใบประกันใกล้หมดอายุ, งานบริการเกิน SLA (หลัง G-18), Membership ใกล้หมดอายุ (G-21: ทำแล้ว 2026-10-12 ด้วย MembershipExpiryReminderWorker — scheduler ตัวแรกของ repo, ADR 0022); ที่เหลือ (ใบประกัน, SLA G-18) ยังไม่มี job
- [ ] (Future) Email channel — เลื่อนตามคำสั่งผู้ใช้; ไม่อยู่ใน slice G-02 รอบแรก (ดู [ADR 0018](../../adr/0018-in-app-notification-foundation.md))
- [x] Frontend: bell + หน้า `/notifications` + mark read ผ่าน TanStack Query (polling 30 วินาที; ไม่ทำ websocket)
- [x] Tests: ผู้รับเห็นเฉพาะของตน, payload ผ่าน allowlist ไม่มีต้นทุน, registry parity (ผล: [Verification](../../05-engineering/notification-foundation-verification.md))

**Validation Question:** email provider, ภาษาอีเมลตามผู้ใช้หรือองค์กร, ช่องทาง LINE ต้องมีไหม

### G-03 — Organization/Branch CRUD, Role management, Approval Authority matrix

**แตะ:** `backend/src/TanErp.Domain/Organization/` (`Organization.cs`, `Branch.cs`), `backend/src/TanErp.Domain/IdentityAccess/` (`Role.cs`, `RolePermission.cs`), `BranchesController.cs`, `AdminUsersController.cs`; frontend `frontend/src/features/settings/`

- [x] Branch CRUD: สร้าง/แก้/ปิดใช้ (ห้ามลบเมื่อมีเอกสารอ้างอิง) + เลขสาขาภาษี (ใช้กับ G-15)
- [x] Organization profile: ชื่อ th/en, เลขผู้เสียภาษี, ที่อยู่ออกเอกสาร, โลโก้ (G-01) — ใช้กับ G-04 (โลโก้เลื่อนไป G-04)
- [ ] Role management: สร้าง/แก้ Role จาก Permission catalog ที่มีอยู่; Role ระบบแก้ไม่ได้; anti-escalation เดิมของ CP-02 ใช้ซ้ำ; เปลี่ยน Role ที่มี approval permission ต้อง maker–checker — G-03b Implemented 2026-10-11 (tests written, not run; browser not checked; ยังไม่ผ่าน UAT) — [Verification](../../05-engineering/role-management-verification.md)
- [ ] Approval Authority matrix: `ApprovalLimit { documentType, roleId, branchId?, maxAmount, currency }` + service `IApprovalAuthority.CanApprove(user, documentType, amount, branch)` แล้วต่อเข้า Estimate approve, PO approve, Change Order approve, Credit Note (G-15) — G-03c Implemented 2026-10-12 (Estimate/PO/Change Order; Credit Note ยังเป็น hook ของ G-15; tests written, not run; browser not checked) — [Verification](../../05-engineering/approval-authority-verification.md)
- [ ] Tests: เกินวงเงิน → 403 code ใหม่, maker = checker ถูกปฏิเสธ, ปิด Branch ที่มีเอกสารเปิดอยู่ถูกปฏิเสธ

**Validation Question:** วงเงินอนุมัติต่อ Role/เอกสาร (ดู `docs/01-business/approval-matrix.md`), หลาย Organization ต่อ deployment หรือไม่

### G-04 — Server-side Quotation PDF + artifact hash

**แตะ:** `backend/src/TanErp.Application/Commercial/`, projection ของ CP-04, ADR ใหม่ที่ supersede/เสริม [ADR 0016](../../adr/0016-browser-print-for-quotation-pdf.md)

> **สถานะ G-04 (2026-10-13):** Implemented — tests written, not run; browser/PDF not visually checked; ยังไม่ผ่าน UAT. ทุกข้อด้านล่างคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: ADR 0023 ([QuestPDF 2026.9.1 + Sarabun ฝังใน assembly](../../adr/0023-server-side-quotation-pdf-with-questpdf.md)), `IQuotationDocumentRenderer`, render ครั้งเดียวตอน issue/amend ใน unit of work เดียวกัน → `UploadedFile` + `quotation_documents` (sha256 ผูก quotation), pagination/ข้อความยาว/ไทย-อังกฤษ/ข้อมูลองค์กร-สาขา, allowlist + test, cross-scope 404, หน้าลูกค้า CP-07 ดาวน์โหลดไฟล์พร้อม SHA-256 และส่ง hash ตอนยอมรับ (ไม่ตรง → `ACCEPTANCE_CONFLICT`, หลักฐานเก็บ `document_sha256`). **ยังไม่ทำ/ยังเปิด:** โลโก้ (follow-up), PDF ย้อนหลังของใบเก่า (ตอบ `404 QUOTATION_DOCUMENT_NOT_AVAILABLE`; ใช้ Amend), **เงื่อนไข license QuestPDF รอ Business ยืนยันก่อน production**. หมายเหตุ: hash คงที่เพราะเก็บไฟล์ครั้งเดียว ไม่ใช่เพราะ renderer deterministic. [Verification](../../05-engineering/quotation-pdf-verification.md)
>
> **งานค้างของ G-04 (บันทึก 2026-10-13):** ดู [Verification หัวข้อ 10](../../05-engineering/quotation-pdf-verification.md) — รัน test เอง, ตรวจ PDF จริง, ยืนยัน license QuestPDF, ตั้ง `Storage:BasePath`, merge PR #23 (ข้าม CI/PR ไว้ก่อนตามคำสั่งผู้ใช้); follow-up: โลโก้, endpoint สร้าง PDF สำหรับใบเก่า, กวาด blob กำพร้า

- [ ] ADR: เลือก renderer (เสนอ: QuestPDF — .NET in-process, ฟอนต์ไทยฝังได้, ไม่มี headless browser) และนโยบาย font ownership (Sarabun/IBM Plex Sans Thai OFL เก็บใน repo)
- [ ] `IQuotationDocumentRenderer` ใน Application; implementation ใน Infrastructure
- [ ] ออก Quotation → render PDF ครั้งเดียว → เก็บเป็น `UploadedFile` + `sha256` ผูก quotation revision (immutable); re-download ใช้ไฟล์เดิม ไม่ re-render
- [ ] Pagination, ข้อความยาว, ไทย/อังกฤษ, โลโก้/ข้อมูลองค์กรจาก G-03
- [ ] Tests: hash คงที่ต่อฉบับ, master data เปลี่ยนแล้วไฟล์ฉบับเก่าไม่เปลี่ยน, payload ไม่มีต้นทุน/margin (allowlist), cross-scope 404
- [ ] หน้าลูกค้า CP-07 แสดง PDF ฉบับที่ hash ตรงกับที่ยอมรับ

### G-05 — External Acceptance OTP

**แตะ:** `PublicQuotationAcceptanceController.cs`, `QuotationAcceptanceHandler.cs`, rate limiting เดิมใน `backend/src/TanErp.Api/RateLimiting/`

- [ ] OTP 6 หลักส่งไปอีเมลผู้ติดต่อที่ผูกกับลิงก์ (ผ่าน G-02 email channel), เก็บ hash, อายุ 10 นาที, ผิด 5 ครั้งล็อกลิงก์
- [ ] การยอมรับต้องมี OTP verified ใน session เดียวกัน; หลักฐานบันทึกว่า verify ผ่านช่องทางใด
- [ ] Tests: OTP ผิด/หมดอายุ/ใช้ซ้ำ, response ไม่เผยว่ามีอีเมลนี้หรือไม่ (404 เหมือนเดิม), rate limit

**Validation Question:** OTP ทาง SMS ด้วยไหม, Legal ยอมรับระดับหลักฐานนี้หรือไม่

> **สถานะ G-05 (2026-10-18):** Implemented (email only) — no tests written, none run; browser not checked; no real email sent; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: `IEmailSender` (SMTP ผ่าน `System.Net.Mail` + logging no-op เมื่อ `Email:Enabled=false`) — **G-02 มีเฉพาะ in-app notification ไม่มี email channel จึงสร้างพอร์ตเล็กนี้ขึ้นใหม่**; ตาราง `commercial.acceptance_otp_challenges`; endpoint `POST /api/public/v1/quotation-acceptance/{token}/otp` และ `…/otp/verify`; `accept` รับ `otpVerificationToken`; OTP 6 หลัก (CSPRNG), เก็บเป็น HMAC-SHA256 ผูกกับ link id, หมดอายุ 10 นาที, ผิด 5 ครั้งล็อกลิงก์, ขอใหม่ได้ทุก 60 วินาทีและ 3 ครั้ง/ชั่วโมง, ใช้ครั้งเดียว; หลักฐานเก็บ channel/เวลา/challenge id (ไม่เก็บรหัส); ปิดไว้ด้วย `AcceptanceOtp:Required=false` (พฤติกรรมเดิมไม่เปลี่ยน). อีเมลผู้รับกรอกตอนสร้างลิงก์ (`recipientEmail`) เพราะลิงก์เดิมไม่ผูกผู้ติดต่อ. **ยังไม่ทำ/เปิด:** test ทั้งหมด, SMS, Legal ยืนยันระดับหลักฐาน, ให้ staff ออกลิงก์ใหม่แทนลิงก์ที่ล็อก (ใช้การสร้างลิงก์ใหม่ตามเดิม), migration ยังไม่ apply. [ADR 0029](../../adr/0029-external-acceptance-otp.md), [Verification](../../05-engineering/external-acceptance-otp-verification.md)

### G-06 — Item Import Phase 2

**แตะ:** `ItemImportsController.cs`, `backend/src/TanErp.Application/Items/` (import handler Phase 1)

- [ ] Mode `upsert` ด้วย key = Item Code; field ที่อนุญาตให้ทับระบุใน contract (ไม่ทับ lifecycle state)
- [ ] Import Cost: สร้าง Cost Record เป็น **Draft ที่ต้องผ่าน maker–checker เดิม** ไม่ publish อัตโนมัติ
- [ ] Batch: ไฟล์ใหญ่ทำงานเป็น background job + สถานะ progress; commit ยัง atomic ต่อ batch
- [ ] Tests: upsert idempotent (ไฟล์เดิมสองครั้งผลเท่าเดิม), cost import ไม่กระทบราคาที่ publish แล้ว, rollback เมื่อแถวใดผิด — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-06 (2026-10-17):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: mode `upsert` (key = Item Code; ทับได้เฉพาะชื่อ/รายละเอียด/หมวด/แบรนด์/ประเภทภาษี/ความสามารถ, `itemType` และหน่วยฐานต้องตรงของเดิม, ไม่แตะสถานะ/การเปิดใช้งาน/สาขา/attribute, Item ที่ปิดใช้งานถูกปฏิเสธ, แถวเหมือนเดิมรายงาน `unchanged` และไม่เขียนอะไร); import ต้นทุนเป็น **Draft เท่านั้น** ด้วยสิทธิ์ `cost-records.create` เดิม (ผู้สร้าง = ผู้ import จึงยังติดกฎ maker ≠ checker เดิม; ไม่แตะ record ที่ published และข้ามแถวที่เหมือน record ที่มีอยู่); งานเบื้องหลัง `item_import_jobs` ประมวลผลทีละ batch (ค่าเริ่มต้น 100 แถว ตั้งที่ `ItemImportJobs:BatchSize`) แต่ละ batch commit พร้อม progress ใน transaction เดียว (Serializable) แถวผิดแถวเดียว = ถอย batch นั้นและเก็บ row error (สูงสุด 500 แถว), batch อื่นทำต่อ, worker ตายแล้วทำต่อได้ด้วย lease; endpoint `POST/GET /api/v1/items/imports/jobs`; ไฟล์สูงสุด 20,000 แถว/10 MB; UI เลือกชนิด/โหมด + แผงความคืบหน้า. **ยังไม่ทำ/ยังเปิด:** test ทั้งหมด, export รูปแบบเดียวกับ template, ยกเลิกงานกลางทาง, import Cost แบบแนบ evidence file, การเข้าถึงเมนู import สำหรับผู้ที่มี `cost-records.create` แต่ไม่มี `items.create`, ทิศทาง blank cell ใน upsert (ตอนนี้ล้างค่า; รอ Data Steward ยืนยัน). [ADR 0026](../../adr/0026-item-import-phase2.md), [Verification](../../05-engineering/item-import-phase2-verification.md)

### G-07 — Survey Template ใน DB

**แตะ:** `backend/src/TanErp.Domain/Surveys/`, `SurveyTemplateVersionsController.cs`, registry `v1/v2` แบบ code ปัจจุบัน

- [ ] ย้าย template เป็นตาราง `survey_template_versions` (immutable เมื่อ publish) + `effectiveFrom/To`; migrate `v1/v2` เดิมเป็น seed โดย hash ของ Survey เก่าไม่เปลี่ยน
- [ ] Evidence ต่อ Area (ใช้ G-01)
- [ ] Hash version ใหม่ (`v4`) เมื่อเพิ่ม field — Survey เดิมคง version เดิม
- [ ] Tests: Ready Survey เก่า verify hash ผ่านหลัง migration, template หมดอายุใช้สร้างใหม่ไม่ได้ — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-07 (2026-10-17):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: ตาราง `crm.survey_template_versions` (trigger กันแก้/ลบ ยกเว้นตั้ง `effective_to_utc` ครั้งเดียว) พร้อม seed v1/v2 (ตรวจ hash parity ด้วยสคริปต์ครั้งเดียว: hasher เดิม = ใหม่ = จาก seed); Survey ใหม่เลือก template ที่ effective ล่าสุด ไม่มี → `SURVEY_TEMPLATE_NOT_EFFECTIVE`; hash `v4` = `v3` + evidence ต่อ Area; owner type `survey-area` (G-01); API `POST /survey-template-versions` และ `…/{code}/retire` ด้วย permission ใหม่ `survey-templates.manage`. **ยังไม่ทำ/เปิด:** FE สำหรับ Publish/Retire/ดูรายการ, seed template v4, ตารางเป็น global (รอเจ้าของตัดสิน), test ทั้งหมด, migration ยังไม่ apply. [ADR 0028](../adr/0028-survey-templates-in-db.md), [Verification](../05-engineering/survey-templates-db-verification.md)

### G-08 — Estimate Unit Conversion Snapshot

**แตะ:** `backend/src/TanErp.Domain/Estimates/`, calculation policy ที่ version แล้ว

- [ ] Cost component ที่ผูก catalog item ตรึง `conversionSnapshot { fromUnit, toUnit, factor, conversionId }` (+ `conversionScope`) ตอน **บันทึก draft** ที่เลือกหน่วยที่ไม่ใช่หน่วยฐาน (ไม่ใช่ตอนคลิก; ระดับ Work Item/Quotation ไม่เปลี่ยน)
- [ ] Calculation **Policy** Version ใหม่ที่ `unit_conversion_mode = snapshot` (ไม่ใช่ตัวนับ `CalculationVersion` ต่อ revision); Estimate เก่าไม่มี snapshot จึงคงผลและ policy string เดิม (ไม่ backfill)
- [ ] Tests: แก้ conversion ใน Item Master แล้ว Estimate ที่อนุมัติไม่เปลี่ยน, หน่วยต่างมิติถูกปฏิเสธ — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-08 (2026-10-15):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: ใช้ตาราง conversion เดิม (Item-specific ก่อน Shared, ตรงหน่วยฐานเท่านั้น) ผ่าน `IUnitConversionResolver` ที่ตรวจมิติซ้ำ; 5 คอลัมน์ snapshot บน `estimate_cost_components`; `calculation_policy_versions.unit_conversion_mode`; สูตร `round(qty × factor × base cost, 2)`; snapshot คงข้ามการบันทึกซ้ำและ `New revision`; ผู้อนุมัติเห็น conversion; endpoint `GET /api/v1/estimate-catalog/items/{itemId}/units`; error ใหม่ `ESTIMATE_UNIT_CONVERSION_NOT_FOUND` และ `…_DIMENSION_MISMATCH`. **ยังไม่ทำ/ยังเปิด:** admin API สำหรับสร้าง/publish Calculation Policy (ต้องมีแถว `snapshot` ก่อนเปิดใช้), conversion หลายขั้น, test ทั้งหมด. **G-09:** ต้องสร้างแถวต้นทุนจาก template ผ่าน `IUnitConversionResolver` และกติกาตรึง/คงค่า (D5) เดียวกัน. [Verification](../../05-engineering/estimate-unit-conversion-verification.md)

### G-09 — Quick Estimate completion

**แตะ:** `backend/src/TanErp.Application/QuickEstimates/`, `PricingTemplatesController.cs`, `frontend/src/features/quick-estimates/`

- [ ] Conversion สร้าง Official Estimate Draft พร้อมรายการตาม template mapping (ไม่ใช่ร่างเปล่า) — idempotent เหมือนเดิม
- [ ] Branch override ของ template (ต้องผ่าน maker–checker)
- [ ] แนบรูป/หลักฐานด้วย G-01
- [ ] Tests: conversion ซ้ำได้ draft เดิม, override ข้าม branch ไม่ได้ — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-09 (2026-10-15):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: conversion เติม Work Item ขอบเขตต่อเส้นวัด (ปริมาณ+หน่วยของแม่แบบ ไม่คัดลอกราคา/ต้นทุน; เติมเฉพาะร่างที่ว่าง; ไม่ backfill); Branch Rate Override (`pricing-templates.override-rate` ขอ, `pricing-templates.approve` ตัดสิน/เพิกถอน, เฉพาะ rate/ขั้นต่ำ ≤ 366 วัน, ข้ามสาขาไม่ได้); evidence owner `quick-estimate`. **ข้อเท็จจริงที่คลาดจากแผนหลัก:** แม่แบบไม่มี item/cost mapping จึงไม่ใช้ `IUnitConversionResolver` ของ G-08. **ยังไม่ทำ/ยังเปิด:** cost recipe ในแม่แบบ (decision N1), เพดาน % ของ override, branch scope ของ endpoint Quick Estimate เดิม, test ทั้งหมด. [Verification](../../05-engineering/quick-estimate-completion-verification.md)

### G-10 — Project Actual Cost / Commitment / WBS

**แตะ:** `backend/src/TanErp.Domain/Projects/` (`ProjectBudgetLine.cs`, `ProjectWbsNode.cs`, `ProjectCostLedgerEntry.cs`), `backend/src/TanErp.Infrastructure/Persistence/Projects/ProjectControlStore.cs`

- [x] WBS: `ProjectWbsNode` ลำดับชั้น (ลึกสุด 3 ระดับ TEST_ONLY); budget line ผูก node
- [x] Commitment = PO approved ที่ยังไม่รับ (คืนยอดเมื่อมี Supplier Return); Actual = Goods issue (CP-11), WO material cost (CP-12), Labor (G-20), และ Supplier Invoice Price Variance (G-11)
- [x] Cost ledger ต่อโครงการแบบ append-only อ้างเอกสารต้นทาง (ไม่คำนวณสดจากหลายตารางแบบเดา)
- [x] รายงาน Budget vs Commitment vs Actual ต่อ WBS
- [x] Tests: ยอดย้อนเอกสารต้นทางได้, void/cancel/return ต้นทาง → reversal entry, เกินงบแจ้งเตือน (`project.budget-overrun`, G-02)

### G-11 — Procurement completion

**แตะ:** `backend/src/TanErp.Domain/Procurement/`, `PurchaseOrdersController.cs`, `PurchaseRequestsController.cs`, `SupplierInvoicesController.cs`, `SuppliersController.cs`, `frontend/src/features/procurement/`

- [x] Purchase Request → อนุมัติ (G-03 วงเงิน + maker–checker) → แปลงเป็น PO (หลาย PR รวม PO ร่างใบเดียวได้ครั้งเดียว)
- [x] Supplier Return จาก Goods Receipt (ลด stock อะตอมมิกผ่าน `IProcurementStockPort` + คืนยอด Commitment โครงการ)
- [x] Supplier Invoice + 3-way match (PO/Receipt/Invoice) พร้อม tolerance (`0%` qty, `2%` unit price; เกิน `2%` พัก `on_hold` รออนุมัติ); ส่งเข้า accounting outbox (`supplier-invoice.posted` / `supplier-invoice.voided`) และบันทึกเฉพาะผลต่างราคาเข้า `ProjectCostLedgerEntries`
- [x] เลขเอกสารผ่าน Document Numbering เดิม (`PR-{YYYY}-{SEQ:4}`, `SRN-{YYYY}-{SEQ:4}`, `SINV-{YYYY}-{SEQ:4}`)
- [x] Tests: คืนเกินที่รับไม่ได้, invoice เกิน tolerance ถูก hold, PR ซ้ำแปลง PO ไม่ได้ (ผล: [Procurement Verification](../../05-engineering/procurement-verification.md))

### G-12 — Inventory completion

**แตะ:** `backend/src/TanErp.Domain/Inventory/` (`StockBalance.cs`, `StockLocationBalance.cs`, `StockSerial.cs`, `WarehouseLocation.cs`, `InventoryPeriodClose.cs`, `StockDocument.cs`), `InventoryController.cs`, `WarehousesController.cs`, `frontend/src/features/inventory/`

- [x] Location/Bin ใต้ Warehouse; balance ต่อ (item, warehouse, location, lot)
- [x] Lot/Serial tracking เปิดต่อ Item (flag `TrackLot` / `TrackSerial` ใน `ItemCapabilities`); serial unique ต่อ item
- [x] Customer/Project return เข้าสต็อก (`POST /api/v1/inventory/returns` พร้อมกลับรายการ `actual` ใน `ProjectCostLedgerEntries`)
- [x] Period close: ล็อกเอกสารย้อนหลัง (`INVENTORY_PERIOD_CLOSED`) + snapshot มูลค่าสิ้นงวด (`inventory.period_closes` & `inventory.period_close_lines`)
- [x] Migration: balance เดิม → location `DEFAULT`, lot ว่าง (`20261029164903_AddInventoryCompletionG12`)
- [x] Tests: ห้ามติดลบต่อ lot, serial ซ้ำ, ลงเอกสารในงวดที่ปิดถูกปฏิเสธ, concurrency ไม่ oversell (ใช้ pattern เดิม) (ผล: [Inventory Verification](../../05-engineering/inventory-verification.md))

### G-13 — Production completion

**แตะ:** `backend/src/TanErp.Domain/Production/` (`Bom.cs`, `BomRoutingStep.cs`, `WorkCenter.cs`, `WorkOrder.cs`, `WorkOrderOperation.cs`), `WorkCentersController.cs`, `WorkOrdersController.cs`, `BomsController.cs`, `frontend/src/features/production/`

- [x] Work Center + Routing (ลำดับ operation, เวลา setup/run) ผูก BOM revision
- [x] Capacity view ต่อ Work Center ต่อวัน (`/api/v1/work-centers/capacity`; พร้อมเชื่อมต่อ calendar ใน G-14)
- [x] Labor cost จาก Timesheet (G-20) เข้าต้นทุน WO
- [x] Subcontracting: operation ภายนอก → PO บริการ (G-11) + บันทึกต้นทุนจ้างผลิตเข้า WO
- [x] Tests: ต้นทุน WO = วัตถุดิบ + แรงงาน + subcontract ย้อนได้, routing ใน WO เป็น snapshot (ผล: [Production Verification](../../05-engineering/production-verification.md))

### G-14 — MRP completion

**แตะ:** `backend/src/TanErp.Domain/Mrp/MrpEngine.cs`, `MrpValues.cs`

- [ ] Planning parameter ต่อ Item/Warehouse: safety stock, MOQ, lot multiple, lead time (แทน lead time ระดับ supplier อย่างเดียว)
- [ ] Working calendar (วันหยุด) สำหรับ offset
- [ ] แปลงหน่วยด้วย conversion ของ Item Master
- [ ] เพิ่ม parameter เข้า input hash; run เก่าคง reproducible
- [ ] Unit tests ของ engine (pure): safety stock, MOQ ปัดขึ้น, lot multiple, ข้ามวันหยุด — เขียนก่อน implementation

### G-15 — Thai Tax

**แตะ:** `backend/src/TanErp.Domain/Finance/` (`BillingDocument.cs`), Item Tax category เดิม, `BillingsController.cs`

- [ ] VAT 7% (rate versioned, ไม่ hardcode ในโค้ดคำนวณ) แยก tax base ต่อบรรทัด
- [ ] WHT: บันทึกตอนรับชำระ (อัตราตามประเภทเงินได้) + ออกเลข 50 ทวิ reference
- [ ] Retention: % ต่อสัญญา หักตอนวางบิล ปล่อยตอนหมดประกัน (ผูก CP-14)
- [ ] Tax Invoice / Receipt เต็มรูป (PDF ผ่าน renderer G-04) + Credit Note / Debit Note อ้างฉบับเดิม
- [ ] e-Tax Invoice (XML ETDA) — แยกเป็น G-15b หลังรู้ provider
- [ ] Tests: ปัดเศษภาษีตาม policy, credit note เกินยอดเดิมไม่ได้, retention ไม่ทำให้ยอดรับเกินสัญญา

**Validation Question:** ปัดเศษ VAT ต่อบรรทัดหรือต่อเอกสาร, ออกใบกำกับเมื่อวางบิลหรือเมื่อรับเงิน (บริการ vs สินค้า), e-Tax provider

### G-16 — Credit Exposure + AR Aging

- [ ] Exposure = ยอดค้างชำระ + quotation accepted ที่ยังไม่วางบิล; aging bucket 0/30/60/90+
- [ ] Gate: ออก Quotation/Billing เกินวงเงินเครดิตใน Customer master → ต้อง override โดยผู้มีอำนาจ (G-03) พร้อมเหตุผล
- [ ] Tests: override ต้องคนละคน, exposure ลดเมื่อรับชำระ

### G-17 — Accounting Connector จริง

**แตะ:** `backend/src/TanErp.Application/Finance/IAccountingConnector.cs`, `backend/src/TanErp.Infrastructure/Persistence/Finance/UnconfiguredAccountingConnector.cs`

- [ ] **Blocked จนกว่า Finance เลือกระบบบัญชี** — ระหว่างนี้ทำ `JournalExportConnector`: สร้างไฟล์ CSV/Excel สมุดรายวันจาก outbox (สถานะ `exported` แยกจาก `posted`) ให้นักบัญชีนำเข้าเอง
- [ ] Chart-of-accounts mapping table (document type → account) แก้ได้โดย Finance
- [ ] Connector จริง: implement `IAccountingConnector` ใช้ `DedupeKey` เป็น idempotency key ของปลายทาง; secrets ผ่าน configuration ไม่อยู่ใน repo
- [ ] Tests: export ซ้ำไม่ซ้ำรายการ, mapping ขาด → dead พร้อมเหตุผล

**Validation Question:** ระบบบัญชีปลายทาง (Express, FlowAccount, PEAK, SAP B1, อื่น ๆ)

### G-18 — Service completion

**แตะ:** `backend/src/TanErp.Domain/Service/` (`ServiceRequest.cs`, `InstallationJob.cs`, `Warranty.cs`), `frontend/src/features/service/`

- [ ] แนบไฟล์ + ลายเซ็นลูกค้าตอนส่งมอบ/ปิดงาน (G-01)
- [ ] SLA policy ต่อ priority (response/resolve hours), เวลานับตาม calendar (G-14), แจ้งเตือนใกล้เกิน (G-02)
- [ ] Service charge: นอกประกัน → รายการคิดเงินส่งเข้า Billing (CP-15/G-15)
- [ ] ตารางช่าง: มอบหมายงานติดตั้ง/บริการต่อช่างต่อวัน + ตรวจชน
- [ ] Tests: งานในประกันไม่สร้างค่าบริการ, SLA หยุดนับขณะรอลูกค้า, มอบงานชนถูกเตือน

### G-19 — Dashboard & Reporting

**แตะ:** ใหม่ `backend/src/TanErp.Application/Reporting/` (Dapper read-only), `frontend/src/features/dashboard/`; reuse `useCsvExport`, `useDataExport`, `useAnalyticsData` ที่มีใน `frontend/src/hooks/`

- [ ] KPI รอบแรก: Pipeline/Win rate, Quotation value, Project budget vs actual (G-10), Stock value, AR aging (G-16), งานบริการค้าง/เกิน SLA
- [ ] ทุก query กรอง Organization/Branch scope ใน SQL; permission ต่อรายงาน
- [ ] Export CSV ผ่าน hook เดิม
- [ ] Tests: cross-branch ไม่รั่ว, ตัวเลขตรงกับ fixture ที่คำนวณมือ

### G-20 — Labor Timesheet

- [x] Timesheet ต่อผู้ใช้ต่อวัน ผูก Project WBS (G-10) หรือ Work Order (G-13) + อัตราค่าแรงต่อคน (`LaborRate`, ค่าเริ่มต้น `TEST_ONLY` 150 บาท/ชม.)
- [x] อนุมัติโดยหัวหน้า (Maker ≠ Checker) → post เข้า Project cost ledger พร้อมตรวจแจ้งเตือนเกินงบ
- [x] Tests: แก้หลังอนุมัติไม่ได้, ชั่วโมงเกินต่อวันถูกเตือน (`> 8` ชม./วัน) และห้ามเกินเพดาน 24 ชม./วัน

### G-21 — CP-02 ค้าง

- [ ] Membership ใกล้หมดอายุ → แจ้งเตือนผู้ดูแล (G-02) — Implemented 2026-10-12 (hosted worker + system-generated notification; tests written, not run; browser not checked) — [Verification](../../05-engineering/membership-expiry-verification.md)
- [ ] ตัดสินใจ User หลาย Organization (ตาม Validation Question CP-02) แล้วปรับ contract — ตัดสิน A1 เมื่อ 2026-10-17 (owner delegation) และ Implemented (guard `UserOrganizationPolicy` + สัญญา; no tests written, none run; browser not checked) — [ADR 0027](../../adr/0027-wave-a-closure-decisions.md)
- [ ] PII visibility/retention ใน Audit — ตัดสิน B1 + B3 เมื่อ 2026-10-17 (owner delegation; Security/Legal ยังต้องยืนยัน) และ Implemented (`GET /api/v1/audit-events`, หน้า `/settings/audit`, `AuditRedactor`; B2 purge ไม่ทำ มีเพียงคีย์ `Audit:RetentionDays` สงวนไว้; no tests written, none run; browser not checked) — [ADR 0027](../../adr/0027-wave-a-closure-decisions.md)

---

## 5. Self-review

- ครอบคลุมข้อจำกัดทุกแถวใน Roadmap หัวข้อ "สถานะ Implementation ปัจจุบัน": Foundation→G-03/G-21, Survey→G-07, Item→G-06/G-08, Quotation Doc→G-04, External Acceptance→G-05, Project→G-10, Procurement→G-11, Inventory→G-12, Production→G-13, MRP→G-14, Service→G-18, Finance→G-15/16/17, Quick Estimate→G-09 (mobile/offline = Future)
- Capability นอกแผนเดิม: Notification G-02, Reporting G-19, ภาษีไทย G-15, Labor G-20
- งานที่ตัดออกโดยผู้ใช้: CI, test ใหญ่/UAT/rehearsal — ระบุในหัวข้อ 1
- Business sign-off ไม่ใช่โค้ด — อยู่ใน track คู่ขนานหัวข้อ 3
