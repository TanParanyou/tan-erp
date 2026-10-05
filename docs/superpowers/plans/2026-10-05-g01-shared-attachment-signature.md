# G-01 Shared Attachment & Signature Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Read `AGENTS.md`, `design.md`, `CONTEXT.md`, `.agents/skills/building-erp-apis/SKILL.md` and `.agents/skills/building-erp-forms/SKILL.md` before Task 1. Reply to the user in Thai; code, identifiers and code comments stay in English.

**Goal:** สร้างความสามารถ "ไฟล์แนบ + ลายเซ็น" ตัวเดียวที่ทุกโมดูล (Service G-18, Quick Estimate G-09, Procurement G-11, Survey G-07) ใช้ซ้ำได้ โดยโมดูลใหม่ลงทะเบียน owner type เพิ่มได้โดยไม่แก้โค้ดกลาง. รอบนี้ลงทะเบียน owner type จริงหนึ่งตัวคือ `installation-job` (ส่งมอบงานติดตั้งของ CP-14) เพื่อพิสูจน์ทางเดินครบ (upload → link → list → unlink → capture signature → serve content).

**Architecture:** `AttachmentLink` และ `SignatureCapture` เป็น entity polymorphic ใน schema `files` ที่อ้าง owner ด้วย `(ownerType, ownerId)`; **owner type เป็น whitelist ในโค้ด** (`AttachmentOwnerTypes` ใน Domain) คู่กับ `AttachmentOwnerRegistry` ใน Application ที่แมป owner type → permission อ่าน/จัดการ/เซ็น + สถานะ owner ที่อนุญาตให้แก้ไข และ `IAttachmentOwnerScopeReader` ใน Infrastructure ที่ค้นหา owner พร้อมบังคับ Organization/Branch scope (ไม่พบ/ข้าม org/ข้ามสาขา = 404). ไฟล์ใช้โมดูล Files เดิม (upload session ผูก parent = owner, magic-number + SHA-256 แล้ว) จึงไม่มีไฟล์ใหม่ที่ยังไม่ผ่านการตรวจเข้ามา. ตรรกะหลักฐานลายเซ็นของ CP-07 (ตรวจชื่อ, PNG, hash) ถูกย้ายขึ้น `Application/Common/Security/SignatureEvidenceRules` และ `Domain/Common/SignatureEvidenceLimits` แล้วให้ทั้ง CP-07 และ G-01 เรียกใช้ที่เดียว. ไม่ต้องเพิ่ม permission key ใหม่ — ใช้ permission ของ owner ตาม registry.

**Tech Stack:** .NET 9 Clean Architecture (Domain/Application/Infrastructure/Api), EF Core + Npgsql (PostgreSQL schema `files`), xUnit (+ Testcontainers PostgreSQL สำหรับ Integration), OpenAPI snapshot (`contracts/openapi/tan-erp.v1.json`) → `openapi-typescript`, Next.js 16 App Router + TypeScript strict, TanStack Query, next-intl (`frontend/src/messages/{th,en}.json`), Vitest + Testing Library, Tailwind semantic tokens (Atelier Architectural Navy Sharp).

**ขอบเขตที่ไม่ทำ (Out of scope):** รัน full backend/frontend test suite, CI, Playwright journeys, `next build`; แต่ละ task รันเฉพาะ test ที่เกี่ยวข้อง + `dotnet build` / `npm run lint` / `npm run typecheck`. ไม่ทำไฟล์ที่ไม่ใช่รูปภาพ (PDF/เอกสาร — โมดูลไฟล์เดิมรับเฉพาะ JPEG/PNG/WebP ≤ 10 MB) ไม่ทำ digital signature ตามกฎหมาย ไม่บังคับให้ handover ต้องมีลายเซ็น (รอ Business ยืนยัน).

**TEST_ONLY defaults (รอยืนยัน):** ขนาด/ชนิดไฟล์ตามที่ Files module บังคับอยู่เดิม (`AllowedMediaTypes` ใน `backend/src/TanErp.Domain/Files/FileConstants.cs` และ 10 MB ใน `CompleteUploadSessionHandler`); ลายเซ็น = ภาพ PNG + SHA-256 + ชื่อผู้ลงนาม + เวอร์ชันถ้อยคำยินยอม ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตาม พ.ร.บ. ธุรกรรมอิเล็กทรอนิกส์; ถ้อยคำยินยอม handover เวอร์ชัน `handover-2026-10-v1`; สูงสุด 50 ไฟล์แนบ active ต่อ owner และ 20 ไฟล์ต่อคำขอ.

**Validation Question (ค้างจากแผนหลัก):** ลายเซ็นต้องเป็น e-Signature ตาม พ.ร.บ. ธุรกรรมอิเล็กทรอนิกส์ระดับใด? ถ้า Legal กำหนดระดับสูงกว่า "ภาพ + hash" ต้องเปิด slice ใหม่ (ADR 0017 บันทึกเรื่องนี้ไว้).

---

## Findings จากการสำรวจโค้ด (ที่แผนนี้อ้างอิง)

| หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- |
| ไฟล์ที่ผูกกับ parent | `FileParentTypes` (Domain/Files/FileUploadSession.cs) เป็น whitelist ของ upload session; `FileParentAccessResolver` (Infrastructure/Files) มี `switch` ต่อ parent type; handler Create/Complete/GetFileContent มีรายการ permission ซ้ำกัน 3 ที่ | แผนนี้ทำให้ owner type ที่ลงทะเบียนใน registry เป็น parent type ที่ใช้ได้อัตโนมัติ (ไม่ต้องแก้ `switch` ทุกครั้ง) และรวมรายการ permission gate ไว้ที่ `FileGatePermissions` |
| การผูกไฟล์ของโมดูลเดิม | แต่ละโมดูลทำตารางเฉพาะ (`OpportunityWorkImage`, `SiteSurveyEvidence`, `ItemImage`, `CostRecord.EvidenceFileId`) และเรียก `IFileStore.ValidateVerifiedFilesForParentAsync` | G-01 ใช้ `ValidateVerifiedFilesForParentAsync` เดิมเป็นตัวตรวจ "ไฟล์ verified + org เดียวกัน + อัปโหลดสำหรับ parent นี้" ไม่เขียนซ้ำ; โมดูลเดิมไม่ถูกย้ายในรอบนี้ (Minimal Blast Radius) |
| CP-07 | หลักฐานลายเซ็นเก็บเป็น base64 ในคอลัมน์ `signature_image` ไม่ใช่ไฟล์; hash คือ SHA-256 ของ **สตริงที่ส่งมา** ไม่ใช่ของ bytes; ตรวจ PNG แค่ 4 byte แรก | lift เฉพาะกฎร่วม (ชื่อ 2–200, ตำแหน่ง ≤100, PNG magic, ความยาว, รูปแบบ SHA-256) โดย **คง behavior เดิมของ CP-07 ทุกประการ**; G-01 เก็บภาพเป็นไฟล์และใช้ `UploadedFile.ContentSha256` (hash ของ bytes ที่เซิร์ฟเวอร์คำนวณเอง) เป็น `contentHash`. ไม่ migrate ข้อมูล CP-07 |
| สถานะ owner | `InstallationJob.Status` ใช้ `InstallationStatus.*`; มี `BranchId` | registry ระบุ state ที่แก้ไฟล์แนบได้ (`planned/in_progress/ready_for_handover`) และเซ็นได้ (`ready_for_handover`) |
| Permission | permission ถูกสร้างใน `TestOnlyDataSeeder` + `docs/03-contracts/permission-catalog.md` + `frontend/src/lib/permissions/permissions.ts` | ใช้ `installations.read` (อ่าน), `installations.operate` (จัดการไฟล์แนบ), `installations.handover` (เซ็น) ที่มีอยู่แล้ว — ไม่เพิ่ม key |
| Frontend | `SignaturePad` (data URL) และ `SignatureManager` มีอยู่แล้วใน `frontend/src/components/forms/` แต่ยังไม่มีใครใช้และไม่มี test; i18n อยู่ที่ `frontend/src/messages/{th,en}.json` | G-01 ใช้ `SignaturePad` เดิม (ไม่สร้างใหม่) แล้วเพิ่ม `SignatureCapturePanel` + `AttachmentList` |
| dotnet ef | ใช้ `--project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations` (ตามแผนเก่าใน `docs/superpowers/plans/`) | ใช้คำสั่งเดียวกัน |
| Integration test | ใช้ Testcontainers (`postgres:17-alpine`) → ต้องมี Docker รันอยู่ | ทุก step "Integration" ต้องมี Docker |

---

## File Structure

### Docs (Task 1, Task 13)

| ไฟล์ | หน้าที่ |
| --- | --- |
| `docs/adr/0017-shared-attachment-owner-registry.md` (ใหม่) | ADR: polymorphic link + owner registry ในโค้ด แทนตารางต่อโมดูล |
| `docs/adr/README.md` (แก้) | เพิ่มลิงก์ ADR 0017 |
| `docs/03-contracts/attachment-api-contract.md` (ใหม่) | สัญญา API/ข้อมูล/error/permission mapping/ขั้นตอนลงทะเบียน owner ใหม่ |
| `docs/03-contracts/permission-catalog.md` (แก้) | ตาราง mapping owner type → permission (ไม่มี key ใหม่) |
| `docs/03-contracts/error-contract.md` (แก้ถ้ามีตาราง code) | เพิ่ม error code ใหม่ |
| `docs/README.md` (แก้) | เพิ่มแถวแผนที่เอกสาร |
| `CONTEXT.md` (แก้) | ศัพท์ Attachment Link / Attachment Owner / Signature Capture |
| `docs/05-engineering/shared-attachment-signature-verification.md` (ใหม่) | หลักฐานการทดสอบ + ข้อจำกัด |
| `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` (แก้) | แถว G-01 ในตารางหัวข้อ 3 ลิงก์มาที่แผนนี้ |
| `docs/03-contracts/service-api-contract.md`, `docs/05-engineering/service-verification.md`, `docs/00-overview/implementation-roadmap.md` (แก้) | แก้ข้อความ "ไม่มีไฟล์แนบ/ลายเซ็น" ให้ชี้ G-01 |

### Backend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `backend/src/TanErp.Domain/Attachments/AttachmentValues.cs` (ใหม่) | `AttachmentDomainException`, `AttachmentOwnerTypes` (whitelist), `AttachmentPurposes` |
| `backend/src/TanErp.Domain/Attachments/AttachmentLink.cs` (ใหม่) | entity + invariant (registered owner, org เดียวกัน, ไม่ซ้ำ, จำกัดจำนวน, unlink) |
| `backend/src/TanErp.Domain/Attachments/SignatureCapture.cs` (ใหม่) | entity ลายเซ็น + invariant |
| `backend/src/TanErp.Domain/Common/SignatureEvidenceLimits.cs` (ใหม่) | ค่าจำกัดหลักฐานลายเซ็นที่ CP-07 และ G-01 ใช้ร่วมกัน |
| `backend/src/TanErp.Domain/Files/FileUploadSession.cs` (แก้) | `FileParentTypes.IsValid` ยอมรับ owner type ที่ลงทะเบียน |
| `backend/src/TanErp.Application/Common/Security/SignatureEvidenceRules.cs` (ใหม่) | กฎร่วม: ชื่อ/ตำแหน่ง, PNG, hash, SHA-256 hex |
| `backend/src/TanErp.Application/Commercial/QuotationAcceptanceHandler.cs`, `QuotationAcceptanceModels.cs` (แก้) | CP-07 เรียกกฎร่วมแทนโค้ดของตัวเอง (behavior เดิม) |
| `backend/src/TanErp.Application/Attachments/AttachmentOwnerRegistry.cs` (ใหม่) | descriptor ต่อ owner type |
| `backend/src/TanErp.Application/Attachments/AttachmentModels.cs` (ใหม่) | caller/input/projection + `SignatureConsentVersions` |
| `backend/src/TanErp.Application/Attachments/IAttachmentStore.cs` (ใหม่) | `IAttachmentStore`, `IAttachmentOwnerScopeReader`, `AttachmentOwnerScope` |
| `backend/src/TanErp.Application/Attachments/AttachmentHandler.cs` (ใหม่) | use case ทั้งหมด (permission → owner scope → validation → state → store) |
| `backend/src/TanErp.Application/Files/FileGatePermissions.cs` (ใหม่) | รายการ permission gate ของ upload/read รวม owner ที่ลงทะเบียน |
| `backend/src/TanErp.Application/Files/CreateUploadSession/CreateUploadSessionHandler.cs`, `CompleteUploadSession/CompleteUploadSessionHandler.cs`, `GetFileContent/GetFileContentHandler.cs` (แก้) | ใช้ `FileGatePermissions` |
| `backend/src/TanErp.Infrastructure/Persistence/Configurations/AttachmentLinkConfiguration.cs` (ใหม่) | ตาราง `files.attachment_links` |
| `backend/src/TanErp.Infrastructure/Persistence/Configurations/SignatureCaptureConfiguration.cs` (ใหม่) | ตาราง `files.signature_captures` |
| `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs` (แก้) | `DbSet<AttachmentLink>`, `DbSet<SignatureCapture>` |
| `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddSharedAttachmentsAndSignatures*.cs` (generate) | migration |
| `backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentStore.cs` (ใหม่) | EF store: transaction, idempotency, audit, projection |
| `backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentOwnerScopeReader.cs` (ใหม่) | ค้น owner + scope ต่อ owner type |
| `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs` (แก้) | ทางเข้า registered-owner ใน `default:` |
| `backend/src/TanErp.Api/Contracts/Attachments/AttachmentContracts.cs` (ใหม่) | request/response record |
| `backend/src/TanErp.Api/Controllers/AttachmentsController.cs` (ใหม่) | endpoint บาง ๆ |
| `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx` (แก้) | status + ข้อความ th/en |
| `backend/src/TanErp.Api/Program.cs` (แก้) | DI |
| `contracts/openapi/tan-erp.v1.json` (regenerate) | snapshot สัญญา |
| `backend/tests/TanErp.UnitTests/Attachments/AttachmentDomainTests.cs` (ใหม่) | invariant ของ domain |
| `backend/tests/TanErp.UnitTests/Common/SignatureEvidenceRulesTests.cs` (ใหม่) | กฎร่วม |
| `backend/tests/TanErp.UnitTests/Attachments/AttachmentHandlerTests.cs` (ใหม่) | handler + registry |
| `backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs` (ใหม่) | gate + `FileParentTypes` |
| `backend/tests/TanErp.IntegrationTests/Api/AttachmentEndpointsTests.cs` (ใหม่) | end-to-end บน PostgreSQL |
| `backend/tests/TanErp.IntegrationTests/Persistence/AttachmentOwnerScopeReaderParityTests.cs` (ใหม่) | registry ↔ scope reader ต้องครบคู่กัน |

### Frontend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `frontend/src/generated/api/tan-erp.v1.ts` (regenerate) | types จาก OpenAPI |
| `frontend/src/lib/api/api-client.ts` (แก้) | type alias + method attachments/signatures |
| `frontend/src/lib/attachments/attachment-owner-types.ts` (ใหม่) | whitelist ฝั่ง UI, purpose, consent version, error code guard |
| `frontend/src/lib/media/data-url-to-file.ts` (+ `.test.ts`) (ใหม่) | แปลง data URL ของ `SignaturePad` เป็น PNG `File` |
| `frontend/src/hooks/useDeferredFileUpload.ts` (แก้) | `FileParentType` รวม `AttachmentOwnerType` |
| `frontend/src/hooks/useAttachments.ts` (ใหม่) | TanStack Query hooks (attachments + signatures) |
| `frontend/src/components/forms/AttachmentList.tsx` (+ `.test.tsx`) (ใหม่) | รายการ/เพิ่ม/ลบไฟล์แนบ (อัปโหลดตอนกดบันทึกเท่านั้น) |
| `frontend/src/components/forms/SignatureCapturePanel.tsx` (+ `.test.tsx`) (ใหม่) | บันทึกลายเซ็น (ชื่อ + ยินยอม + `SignaturePad`) และรายการลายเซ็น |
| `frontend/src/components/forms/index.ts` (แก้) | export |
| `frontend/src/messages/th.json`, `en.json` (แก้) | namespace `attachments` ครบสองภาษา |
| `frontend/src/features/service/components/installation-detail.tsx` (แก้) + `service-components.test.tsx` (แก้) | ใช้เป็น consumer แรก |

---

## Task 1: ADR, สัญญา API, permission mapping, error code, ศัพท์ (docs)

**Files:**
- Create: `docs/adr/0017-shared-attachment-owner-registry.md`
- Create: `docs/03-contracts/attachment-api-contract.md`
- Modify: `docs/adr/README.md`, `docs/03-contracts/permission-catalog.md`, `docs/03-contracts/error-contract.md`, `docs/README.md`, `CONTEXT.md`

ขั้นนี้เป็นเอกสาร จึงไม่มี test ที่รันได้ — ตรวจด้วย link/JSON check ใน Step 6.

- [ ] **Step 1: สร้าง ADR 0017**

เขียน `docs/adr/0017-shared-attachment-owner-registry.md`:

```markdown
---
status: accepted
---

# ไฟล์แนบและลายเซ็นกลางด้วย Polymorphic Link + Owner Registry ในโค้ด

Service (G-18), Quick Estimate (G-09), Procurement (G-11) และ Survey (G-07) ต้องแนบรูป/หลักฐานและเก็บลายเซ็นแบบเดียวกัน. โมดูลเดิมแต่ละตัวทำตารางผูกไฟล์ของตัวเอง (`opportunity_work_images`, `site_survey_evidence`, `item_images`) ซึ่งซ้ำซ้อนและทำให้กฎ scope/permission ต่างกันไปทีละที่. เราเลือกตารางกลาง `files.attachment_links` และ `files.signature_captures` ที่อ้าง owner ด้วย `(owner_type, owner_id)` โดย **owner type เป็น whitelist ในโค้ด** (`AttachmentOwnerTypes` + `AttachmentOwnerRegistry`) ไม่ใช่ string อิสระ และ permission/สถานะที่แก้ได้/การค้นหา owner พร้อมบังคับ Organization-Branch scope ถูก resolve ต่อ owner type จึงไม่มีทางเข้าถึงไฟล์ของ owner ที่ตนไม่มีสิทธิ์ผ่านช่องทางกลางนี้.

ข้อเสียที่ยอมรับ: ฐานข้อมูลบังคับ foreign key ไปยัง owner ไม่ได้ (polymorphic) จึงต้องตรวจในแอปทุกครั้ง และ DB check constraint ตรวจได้เพียงรูปแบบของ `owner_type` ไม่ใช่รายชื่อ (เพื่อไม่ต้องแก้ migration ทุกครั้งที่ลงทะเบียน owner ใหม่); การลงทะเบียน owner ใหม่ต้องแก้สามจุดพร้อมกัน (whitelist, registry descriptor, scope reader) โดยมี test ตรวจความครบคู่. ไฟล์ยังเป็นรูปภาพ (JPEG/PNG/WebP ≤ 10 MB) ตาม Files module เดิม — เอกสาร PDF ของ Procurement ต้องขยาย Files module แยกต่างหาก.

ลายเซ็นเก็บเป็นภาพ PNG (ไฟล์) + SHA-256 ของ bytes + ชื่อผู้ลงนาม + เวอร์ชันถ้อยคำยินยอม + เวลาเซิร์ฟเวอร์ ใช้กฎร่วมกับ External Acceptance (CP-07) แต่ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย. ถ้า Legal กำหนดระดับสูงกว่านี้ ต้องเปิด slice ใหม่แล้วอ้างอิง ADR นี้.
```

- [ ] **Step 2: เพิ่มลิงก์ใน ADR README**

ต่อท้ายรายการใน `docs/adr/README.md`:

```markdown
- [0017 — ไฟล์แนบและลายเซ็นกลางด้วย Owner Registry](0017-shared-attachment-owner-registry.md)
```

- [ ] **Step 3: สร้างสัญญา API**

เขียน `docs/03-contracts/attachment-api-contract.md` (ภาษาไทย, ตัวอย่างโค้ดภาษาอังกฤษ) ให้มีหัวข้อดังนี้ครบ:

```markdown
# Shared Attachment & Signature API Contract (ข้อตกลง API ไฟล์แนบและลายเซ็นกลาง)

**สถานะ:** Draft → Implemented เมื่อ G-01 เสร็จ (ดู [Verification](../05-engineering/shared-attachment-signature-verification.md)). กฎเป็นค่าเริ่มต้น TEST_ONLY รอ Security/Legal ยืนยัน. ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0017](../adr/0017-shared-attachment-owner-registry.md).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| Owner | `(ownerType, ownerId)` โดย `ownerType` ต้องอยู่ใน registry ในโค้ด (รอบแรก: `installation-job`); ไม่ลงทะเบียน → `422 ATTACHMENT_OWNER_TYPE_INVALID` |
| ลำดับตรวจ | owner type → permission ของ owner (403) → ค้น owner ใน Organization/Branch ของผู้เรียก (ไม่พบ/ข้าม org/ข้ามสาขา = **404** เหมือนกัน) → validation (422) → สถานะ owner (409 `ATTACHMENT_OWNER_LOCKED`) |
| ไฟล์ | ต้องผ่าน upload session ของ Files module โดย `parentType = ownerType`, `parentId = ownerId`; verified, org เดียวกัน, อัปโหลดสำหรับ owner นี้; ผิดเงื่อนไขทั้งหมด → `422 ATTACHMENT_FILE_NOT_READY` (ไม่บอกว่าไฟล์มีอยู่หรือไม่) |
| Purpose | `general`, `evidence`, `handover`, `defect` (ลายเซ็นใช้ได้เฉพาะ `handover`) |
| ความซ้ำ | ไฟล์เดียวกัน + owner เดียวกัน + purpose เดียวกัน (ที่ยัง active) → `409 ATTACHMENT_DUPLICATE`; unique index บางส่วน (`removed_at_utc IS NULL`) ป้องกัน race |
| จำนวน | ≤ 20 ไฟล์ต่อคำขอ, ≤ 50 active ต่อ owner (`409 ATTACHMENT_LIMIT_EXCEEDED`) |
| Atomic | การแนบเป็น transaction เดียวต่อคำขอ: ผิดข้อเดียว = ไม่มี link ใดถูกสร้าง (ไฟล์ที่อัปโหลดแล้วยังอยู่ใน Files module แต่ไม่ถูกผูก) |
| Unlink | soft remove (`removed_at_utc/by`), ไฟล์จริงไม่ถูกลบ; unlink ซ้ำ → 404 |
| Idempotency | `POST` ต้องมี `Idempotency-Key` (replay คืนรายการปัจจุบัน, payload ต่าง → `409 IDEMPOTENCY_KEY_REUSED`); `DELETE` ไม่ต้องมี |
| ลายเซ็น | `signerName` 2–200, `signerRole` ≤100, `consentAccepted=true` และ `consentTextVersion` ตรงเวอร์ชันปัจจุบันของ purpose (`handover` → `handover-2026-10-v1`); ภาพต้องเป็นไฟล์ `image/png` ที่ผ่านการตรวจแล้วและมี `content_sha256`; `signedAtUtc` เป็นเวลาเซิร์ฟเวอร์; `contentHash` = SHA-256 ของ bytes ที่เซิร์ฟเวอร์คำนวณ; ภาพหนึ่งไฟล์เป็นลายเซ็นได้ครั้งเดียว |
| Audit | `attachment.linked`, `attachment.unlinked`, `signature.captured` บันทึก owner type/purpose/จำนวน/id ไฟล์/hash/เวอร์ชันยินยอม **ไม่บันทึกชื่อและตำแหน่งผู้ลงนาม** |
| PDPA | ชื่อ/ตำแหน่งผู้ลงนามเก็บใน `signature_captures` เท่านั้น |

## Permission mapping (ไม่มี permission key ใหม่)

| Owner type | อ่าน (list/serve) | จัดการไฟล์แนบ (attach/unlink/upload) | เซ็น (capture signature / upload ภาพลายเซ็น) | สถานะ owner ที่แก้ไฟล์แนบได้ | สถานะที่เซ็นได้ |
| --- | --- | --- | --- | --- | --- |
| `installation-job` | `installations.read` | `installations.operate` | `installations.handover` | `planned`, `in_progress`, `ready_for_handover` | `ready_for_handover` |

## Endpoints (ต้อง `Authorization` + `X-Membership-Id`)

| Action | Method/Path | Permission |
| --- | --- | --- |
| รายการไฟล์แนบ | `GET /api/v1/attachment-owners/{ownerType}/{ownerId}/attachments` → `{items}` | read |
| แนบไฟล์ | `POST .../attachments` `{purpose, fileIds[1..20]}` → `201 {items}` | manage |
| ถอดไฟล์แนบ | `DELETE .../attachments/{linkId}` → `204` | manage |
| รายการลายเซ็น | `GET .../signatures` → `{items}` | read |
| บันทึกลายเซ็น | `POST .../signatures` `{purpose, signerName, signerRole?, imageFileId, consentAccepted, consentTextVersion}` → `201` | sign |

Response เป็น structured projection: `AttachmentLinkResponse { id, ownerType, ownerId, fileId, purpose, filename, mediaType, fileSizeBytes, servingUrl, createdBy { id, displayName }, createdAtUtc }`; `SignatureCaptureResponse { id, ownerType, ownerId, purpose, signerName, signerRole, signedAtUtc, imageFileId, servingUrl, consentTextVersion, contentHash, capturedBy { id, displayName } }`.

## Errors

`403 PERMISSION_DENIED`; `404 RESOURCE_NOT_FOUND`; `409`: `ATTACHMENT_DUPLICATE`, `ATTACHMENT_LIMIT_EXCEEDED`, `ATTACHMENT_OWNER_LOCKED`, `IDEMPOTENCY_KEY_REUSED`; `422`: `ATTACHMENT_OWNER_TYPE_INVALID`, `ATTACHMENT_PURPOSE_INVALID`, `ATTACHMENT_FIELD_INVALID`, `ATTACHMENT_FILE_NOT_READY`, `ATTACHMENT_FILE_SCOPE_MISMATCH`, `SIGNATURE_SUBMISSION_INVALID`, `SIGNATURE_CONSENT_REQUIRED`, `SIGNATURE_IMAGE_INVALID`.

## Data (migration `AddSharedAttachmentsAndSignatures`, schema `files`)

`attachment_links` (org, owner_type, owner_id, file_id, purpose, created_by/at, removed_by/at; FK `(file_id, organization_id)` → `uploaded_files (id, organization_id)` เพื่อบังคับ org เดียวกันระดับ DB; unique บางส่วน `(organization_id, owner_type, owner_id, file_id, purpose) WHERE removed_at_utc IS NULL`). `signature_captures` (org, owner_type, owner_id, purpose, signer_name, signer_role, signed_at_utc, image_file_id unique, consent_text_version, content_hash, captured_by). `owner_type`/`purpose` มี check เฉพาะรูปแบบ (regex) เพราะรายชื่อจริงอยู่ในโค้ด.

## ขั้นตอนลงทะเบียน owner type ใหม่ (สำหรับ G-09/G-11/G-07/G-18)

1. เพิ่ม constant และใส่ใน `Registered` ของ `AttachmentOwnerTypes` (`backend/src/TanErp.Domain/Attachments/AttachmentValues.cs`).
2. เพิ่ม `AttachmentOwnerDescriptor` ใน `AttachmentOwnerRegistry` (permission อ่าน/จัดการ/เซ็น + สถานะที่แก้ได้/เซ็นได้).
3. เพิ่ม `case` ใน `AttachmentOwnerScopeReader.FindAsync` และใน `SupportedOwnerTypes`.
4. เพิ่มค่าใน `ATTACHMENT_OWNER_TYPES` (`frontend/src/lib/attachments/attachment-owner-types.ts`) แล้วใช้ `<AttachmentList ownerType ownerId />`.
5. ไม่ต้องแก้ controller, handler, store, component หรือ migration. test `AttachmentOwnerScopeReaderParityTests` และ `AttachmentHandlerTests.Registry_*` จะล้มถ้าลืมข้อ 1–3.

## Threat notes

owner type ที่ผู้ใช้ส่งมาไม่เคยถูกใช้เป็นชื่อตาราง/คำสั่ง SQL (ใช้เทียบกับ whitelist เท่านั้น); ไฟล์ที่ผูกแล้วอ่านผ่าน `GET /api/v1/files/{id}/content` ซึ่งตรวจ parent ของ upload session ด้วย permission ของ owner; ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย (ภาพ + hash เท่านั้น); ผู้ที่มีสิทธิ์ `installations.operate` แต่ไม่มี `installations.handover` เซ็นไม่ได้.
```

- [ ] **Step 4: เพิ่มตาราง mapping ใน permission catalog**

ต่อท้ายตาราง Service ใน `docs/03-contracts/permission-catalog.md` (ใต้แถว `service-requests.manage`) เพิ่มหมายเหตุหนึ่งย่อหน้า: `**Shared Attachment (G-01):** ไม่มี permission key ใหม่ — การอ่าน/แนบ/เซ็นไฟล์แนบของ owner ใช้ permission ของ owner ตาม [Attachment API Contract](attachment-api-contract.md) (`installation-job` → read=installations.read, manage=installations.operate, sign=installations.handover).`

- [ ] **Step 5: error code, README, ศัพท์**

1. `grep -n "ACCEPTANCE_CONSENT_REQUIRED\|INSTALLATION_VERSION_CONFLICT" docs/03-contracts/error-contract.md` — ถ้ามีตาราง code ให้เพิ่ม 11 code ใหม่ (รายชื่อในสัญญา, status ตามหัวข้อ Errors) ในรูปแบบเดียวกับแถวข้างเคียง; ถ้าไม่มี (เอกสารนี้ไม่ list ราย code) ให้ข้าม.
2. `docs/README.md`: ใต้แถว `API งานติดตั้ง ส่งมอบ ประกัน และบริการหลังการขาย` เพิ่ม `| API ไฟล์แนบและลายเซ็นกลาง (Shared Attachment & Signature) | [Attachment API Contract](03-contracts/attachment-api-contract.md) |`.
3. `CONTEXT.md`: ในส่วนที่เหมาะสมที่สุด (หลังคำศัพท์เอกสาร/หลักฐานถ้ามี มิฉะนั้นท้ายไฟล์) เพิ่ม:

```markdown
**Attachment Owner (เจ้าของไฟล์แนบ)**:
ระเบียนธุรกิจที่ลงทะเบียนในระบบว่าแนบไฟล์และลายเซ็นได้ (เช่น งานติดตั้ง) ระบุด้วยชนิดที่กำหนดในโค้ดและรหัสระเบียน
_Avoid_: Parent record, Attachment target

**Attachment Link (การผูกไฟล์แนบ)**:
ความสัมพันธ์ระหว่างไฟล์ที่ผ่านการตรวจแล้วกับ Attachment Owner พร้อมวัตถุประสงค์ของไฟล์ ถอดออกได้โดยไม่ลบไฟล์
_Avoid_: Upload, File record

**Signature Capture (หลักฐานลายเซ็น)**:
ภาพลายเซ็น PNG พร้อมชื่อผู้ลงนาม เวลา เวอร์ชันถ้อยคำยินยอม และ SHA-256 ที่ผูกกับ Attachment Owner; ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย
_Avoid_: Digital signature, e-Signature
```

- [ ] **Step 6: ตรวจลิงก์แล้ว commit**

Run: `cd /Users/syaco/Documents/development/tan-erp && grep -n "attachment-api-contract\|0017-shared" docs/README.md docs/adr/README.md docs/03-contracts/permission-catalog.md && ls docs/adr/0017-shared-attachment-owner-registry.md docs/03-contracts/attachment-api-contract.md`
Expected: บรรทัดที่เพิ่มปรากฏและไฟล์ทั้งสองมีอยู่.

```bash
git add docs/adr/0017-shared-attachment-owner-registry.md docs/adr/README.md docs/03-contracts/attachment-api-contract.md docs/03-contracts/permission-catalog.md docs/03-contracts/error-contract.md docs/README.md CONTEXT.md
git commit -F - <<'EOF'
docs(attachments): add shared attachment and signature contract and ADR 0017

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 2: Domain — `AttachmentLink`, `SignatureCapture` และ invariant

**Files:**
- Create: `backend/src/TanErp.Domain/Common/SignatureEvidenceLimits.cs`
- Create: `backend/src/TanErp.Domain/Attachments/AttachmentValues.cs`
- Create: `backend/src/TanErp.Domain/Attachments/AttachmentLink.cs`
- Create: `backend/src/TanErp.Domain/Attachments/SignatureCapture.cs`
- Modify: `backend/src/TanErp.Domain/Files/FileUploadSession.cs`
- Test: `backend/tests/TanErp.UnitTests/Attachments/AttachmentDomainTests.cs`
- Test: `backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs` (เฉพาะส่วน `FileParentTypes` ใน Task นี้; ส่วน gate เพิ่มใน Task 6)

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Attachments/AttachmentDomainTests.cs`:

```csharp
using TanErp.Domain.Attachments;
using Xunit;

namespace TanErp.UnitTests.Attachments;

public class AttachmentDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Owner = Guid.NewGuid();
    private static readonly Guid Actor = Guid.NewGuid();
    private static readonly string Hash = new('a', 64);

    private static AttachmentLink Link(
        string ownerType = AttachmentOwnerTypes.InstallationJob,
        Guid? owner = null,
        Guid? fileOrg = null,
        Guid? file = null,
        string purpose = AttachmentPurposes.Evidence) =>
        new(Guid.NewGuid(), Org, ownerType, owner ?? Owner, fileOrg ?? Org, file ?? Guid.NewGuid(), purpose, Actor, Now);

    private static string CodeOf(Action action) => Assert.Throws<AttachmentDomainException>(action).Code;

    [Fact]
    public void Link_NormalizesOwnerTypeAndPurpose()
    {
        var link = Link(ownerType: "  Installation-Job ", purpose: " EVIDENCE ");
        Assert.Equal("installation-job", link.OwnerType);
        Assert.Equal("evidence", link.Purpose);
        Assert.True(link.IsActive);
    }

    [Theory]
    [InlineData("customer")]
    [InlineData("installation_job")]
    [InlineData("")]
    [InlineData("   ")]
    public void Link_RejectsUnregisteredOwnerType(string ownerType)
    {
        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", CodeOf(() => Link(ownerType: ownerType)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("misc")]
    public void Link_RejectsUnknownPurpose(string purpose)
    {
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", CodeOf(() => Link(purpose: purpose)));
    }

    [Fact]
    public void Link_RejectsFileFromAnotherOrganization()
    {
        Assert.Equal("ATTACHMENT_FILE_SCOPE_MISMATCH", CodeOf(() => Link(fileOrg: Guid.NewGuid())));
    }

    [Fact]
    public void Link_RejectsEmptyIds()
    {
        Assert.Equal("ATTACHMENT_FIELD_INVALID", CodeOf(() => Link(owner: Guid.Empty)));
        Assert.Equal("ATTACHMENT_FIELD_INVALID", CodeOf(() => Link(file: Guid.Empty)));
    }

    [Fact]
    public void AssertCanAdd_RejectsTheSameFileOwnerAndPurpose_ButAllowsADifferentPurpose()
    {
        var fileId = Guid.NewGuid();
        var existing = Link(file: fileId, purpose: AttachmentPurposes.Evidence);
        var active = new List<AttachmentLink> { existing };

        Assert.Equal("ATTACHMENT_DUPLICATE", CodeOf(() => AttachmentLink.AssertCanAdd(active, Link(file: fileId, purpose: AttachmentPurposes.Evidence))));
        AttachmentLink.AssertCanAdd(active, Link(file: fileId, purpose: AttachmentPurposes.General));
    }

    [Fact]
    public void AssertCanAdd_IgnoresRemovedLinks()
    {
        var fileId = Guid.NewGuid();
        var removed = Link(file: fileId);
        removed.Remove(Actor, Now);

        AttachmentLink.AssertCanAdd(new List<AttachmentLink> { removed }, Link(file: fileId));
    }

    [Fact]
    public void AssertCanAdd_RejectsMoreThanTheActiveLimit()
    {
        var active = Enumerable.Range(0, AttachmentLink.MaxActiveLinksPerOwner).Select(_ => Link()).ToList();
        Assert.Equal("ATTACHMENT_LIMIT_EXCEEDED", CodeOf(() => AttachmentLink.AssertCanAdd(active, Link())));
    }

    [Fact]
    public void Remove_MarksTheLinkInactive_AndASecondRemoveIsNotFound()
    {
        var link = Link();
        link.Remove(Actor, Now);

        Assert.False(link.IsActive);
        Assert.Equal(Now, link.RemovedAtUtc);
        Assert.Equal(Actor, link.RemovedByUserId);
        Assert.Equal("RESOURCE_NOT_FOUND", CodeOf(() => link.Remove(Actor, Now)));
    }

    private static SignatureCapture Signature(
        string signerName = "คุณสมชาย ใจดี",
        string? role = "เจ้าของบ้าน",
        string purpose = AttachmentPurposes.Handover,
        Guid? imageOrg = null,
        string consent = "handover-2026-10-v1",
        string? hash = null) =>
        new(Guid.NewGuid(), Org, AttachmentOwnerTypes.InstallationJob, Owner, purpose, signerName, role, Now,
            imageOrg ?? Org, Guid.NewGuid(), consent, hash ?? Hash, Actor);

    [Fact]
    public void Signature_StoresTrimmedEvidence()
    {
        var capture = Signature(signerName: "  คุณสมชาย  ", role: "  เจ้าของบ้าน ");
        Assert.Equal("คุณสมชาย", capture.SignerName);
        Assert.Equal("เจ้าของบ้าน", capture.SignerRole);
        Assert.Equal(Now, capture.SignedAtUtc);
        Assert.Equal(Hash, capture.ContentHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ก")]
    public void Signature_RejectsShortSignerName(string name)
    {
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(signerName: name)));
    }

    [Fact]
    public void Signature_RejectsOverlongNameRoleAndConsentVersion()
    {
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(signerName: new string('x', 201))));
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(role: new string('x', 101))));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", CodeOf(() => Signature(consent: new string('x', 33))));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", CodeOf(() => Signature(consent: " ")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    public void Signature_RejectsAMalformedContentHash(string hash)
    {
        Assert.Equal("SIGNATURE_IMAGE_INVALID", CodeOf(() => Signature(hash: hash)));
        Assert.Equal("SIGNATURE_IMAGE_INVALID", CodeOf(() => Signature(hash: new string('A', 64))));
    }

    [Fact]
    public void Signature_RejectsAnImageFromAnotherOrganization_AndNonHandoverPurposes()
    {
        Assert.Equal("ATTACHMENT_FILE_SCOPE_MISMATCH", CodeOf(() => Signature(imageOrg: Guid.NewGuid())));
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", CodeOf(() => Signature(purpose: AttachmentPurposes.Evidence)));
    }
}
```

`backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs` (ส่วนแรก):

```csharp
using TanErp.Domain.Files;
using Xunit;

namespace TanErp.UnitTests.Files;

public class FileGatePermissionsTests
{
    [Theory]
    [InlineData("installation-job", true)]
    [InlineData("INSTALLATION-JOB", true)]
    [InlineData("installation_job", false)]
    [InlineData("customer", true)]
    [InlineData("unknown", false)]
    public void FileParentTypes_AcceptsRegisteredOwnerTypesAndExistingTypes(string parentType, bool expected)
    {
        Assert.Equal(expected, FileParentTypes.IsValid(parentType));
    }
}
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~AttachmentDomainTests|FullyQualifiedName~FileGatePermissionsTests"`
Expected: FAIL ที่ขั้น build (`CS0246: The type or namespace name 'Attachments' ... 'AttachmentLink' could not be found`).

- [ ] **Step 3: เขียน implementation**

`backend/src/TanErp.Domain/Common/SignatureEvidenceLimits.cs`:

```csharp
namespace TanErp.Domain.Common;

/// <summary>Limits for signature evidence shared by External Acceptance (CP-07) and shared signature capture (G-01).</summary>
public static class SignatureEvidenceLimits
{
    public const int MinSignerNameLength = 2;
    public const int MaxSignerNameLength = 200;
    public const int MaxSignerRoleLength = 100;
    public const int MaxConsentVersionLength = 32;
    public const int MaxImageChars = 150_000;
    public const int Sha256HexLength = 64;
}
```

`backend/src/TanErp.Domain/Attachments/AttachmentValues.cs`:

```csharp
namespace TanErp.Domain.Attachments;

public class AttachmentDomainException : Exception
{
    public string Code { get; }

    public AttachmentDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Code-defined whitelist of record types that can own attachments and signatures.
/// A new owner type also needs an AttachmentOwnerRegistry descriptor and an AttachmentOwnerScopeReader case.
/// </summary>
public static class AttachmentOwnerTypes
{
    public const string InstallationJob = "installation-job";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal) { InstallationJob };

    public static IReadOnlyCollection<string> All => Registered;

    public static bool IsRegistered(string? ownerType) => ownerType is not null && Registered.Contains(ownerType);

    /// <summary>Returns the canonical (trimmed, lower-case) owner type, or null when it is not registered.</summary>
    public static string? Normalize(string? ownerType)
    {
        var candidate = ownerType?.Trim().ToLowerInvariant();
        return IsRegistered(candidate) ? candidate : null;
    }
}

public static class AttachmentPurposes
{
    public const string General = "general";
    public const string Evidence = "evidence";
    public const string Handover = "handover";
    public const string Defect = "defect";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { General, Evidence, Handover, Defect };

    /// <summary>Returns the canonical purpose, or null when it is unknown.</summary>
    public static string? Normalize(string? purpose)
    {
        var candidate = purpose?.Trim().ToLowerInvariant();
        return candidate is not null && All.Contains(candidate) ? candidate : null;
    }
}
```

`backend/src/TanErp.Domain/Attachments/AttachmentLink.cs`:

```csharp
using TanErp.Domain.Common;

namespace TanErp.Domain.Attachments;

/// <summary>Links one verified file to one registered owner record for a purpose. Removing a link never deletes the file.</summary>
public class AttachmentLink : Entity
{
    public const int MaxActiveLinksPerOwner = 50;

    public Guid OrganizationId { get; private set; }
    public string OwnerType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public Guid FileId { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? RemovedAtUtc { get; private set; }
    public Guid? RemovedByUserId { get; private set; }

    public bool IsActive => RemovedAtUtc is null;

    protected AttachmentLink() { }

    public AttachmentLink(
        Guid id, Guid organizationId, string ownerType, Guid ownerId, Guid fileOrganizationId, Guid fileId,
        string purpose, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || ownerId == Guid.Empty || fileId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new AttachmentDomainException("ATTACHMENT_FIELD_INVALID", "Organization, owner, file and actor are required.");
        }

        var normalizedOwner = AttachmentOwnerTypes.Normalize(ownerType)
            ?? throw new AttachmentDomainException("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");
        var normalizedPurpose = AttachmentPurposes.Normalize(purpose)
            ?? throw new AttachmentDomainException("ATTACHMENT_PURPOSE_INVALID", $"Purpose '{purpose}' is not supported.");

        if (fileOrganizationId != organizationId)
        {
            throw new AttachmentDomainException("ATTACHMENT_FILE_SCOPE_MISMATCH", "The file does not belong to the owner's organization.");
        }

        OrganizationId = organizationId;
        OwnerType = normalizedOwner;
        OwnerId = ownerId;
        FileId = fileId;
        Purpose = normalizedPurpose;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    /// <summary>Rejects a duplicate (same owner, file and purpose) and enforces the per-owner limit. Removed links are ignored.</summary>
    public static void AssertCanAdd(IReadOnlyCollection<AttachmentLink> activeLinksOfOwner, AttachmentLink candidate)
    {
        ArgumentNullException.ThrowIfNull(activeLinksOfOwner);
        ArgumentNullException.ThrowIfNull(candidate);

        var active = activeLinksOfOwner.Where(l => l.IsActive).ToList();
        if (active.Any(l => l.OrganizationId == candidate.OrganizationId && l.OwnerType == candidate.OwnerType
                            && l.OwnerId == candidate.OwnerId && l.FileId == candidate.FileId && l.Purpose == candidate.Purpose))
        {
            throw new AttachmentDomainException("ATTACHMENT_DUPLICATE", "This file is already attached to this record for the same purpose.");
        }

        if (active.Count >= MaxActiveLinksPerOwner)
        {
            throw new AttachmentDomainException("ATTACHMENT_LIMIT_EXCEEDED", $"A record can have at most {MaxActiveLinksPerOwner} attachments.");
        }
    }

    public void Remove(Guid actorUserId, DateTimeOffset now)
    {
        if (!IsActive) throw new AttachmentDomainException("RESOURCE_NOT_FOUND", "Attachment not found.");
        RemovedByUserId = actorUserId;
        RemovedAtUtc = now.ToUniversalTime();
    }
}
```

`backend/src/TanErp.Domain/Attachments/SignatureCapture.cs`:

```csharp
using System.Text.RegularExpressions;
using TanErp.Domain.Common;

namespace TanErp.Domain.Attachments;

/// <summary>
/// Evidence that a named person signed (image of a handwritten signature) for a registered owner record.
/// This is a PNG image plus SHA-256 and consent wording version, not a legal electronic signature.
/// </summary>
public class SignatureCapture : Entity
{
    private static readonly Regex LowerSha256 = new("^[0-9a-f]{64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public Guid OrganizationId { get; private set; }
    public string OwnerType { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public string SignerName { get; private set; } = string.Empty;
    public string? SignerRole { get; private set; }
    public DateTimeOffset SignedAtUtc { get; private set; }
    public Guid ImageFileId { get; private set; }
    public string ConsentTextVersion { get; private set; } = string.Empty;
    public string ContentHash { get; private set; } = string.Empty;
    public Guid CapturedByUserId { get; private set; }

    protected SignatureCapture() { }

    public SignatureCapture(
        Guid id, Guid organizationId, string ownerType, Guid ownerId, string purpose, string signerName, string? signerRole,
        DateTimeOffset signedAtUtc, Guid imageFileOrganizationId, Guid imageFileId, string consentTextVersion, string contentHash,
        Guid capturedByUserId) : base(id)
    {
        if (organizationId == Guid.Empty || ownerId == Guid.Empty || imageFileId == Guid.Empty || capturedByUserId == Guid.Empty)
        {
            throw new AttachmentDomainException("ATTACHMENT_FIELD_INVALID", "Organization, owner, image file and actor are required.");
        }

        var normalizedOwner = AttachmentOwnerTypes.Normalize(ownerType)
            ?? throw new AttachmentDomainException("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");
        var normalizedPurpose = AttachmentPurposes.Normalize(purpose);
        if (normalizedPurpose != AttachmentPurposes.Handover)
        {
            throw new AttachmentDomainException("ATTACHMENT_PURPOSE_INVALID", "Signatures are supported only for the handover purpose.");
        }

        var name = signerName?.Trim() ?? string.Empty;
        if (name.Length < SignatureEvidenceLimits.MinSignerNameLength || name.Length > SignatureEvidenceLimits.MaxSignerNameLength)
        {
            throw new AttachmentDomainException("SIGNATURE_SUBMISSION_INVALID", "A signer name of 2-200 characters is required.");
        }

        var role = string.IsNullOrWhiteSpace(signerRole) ? null : signerRole.Trim();
        if (role is { Length: > SignatureEvidenceLimits.MaxSignerRoleLength })
        {
            throw new AttachmentDomainException("SIGNATURE_SUBMISSION_INVALID", "The signer role cannot exceed 100 characters.");
        }

        var consent = consentTextVersion?.Trim() ?? string.Empty;
        if (consent.Length == 0 || consent.Length > SignatureEvidenceLimits.MaxConsentVersionLength)
        {
            throw new AttachmentDomainException("SIGNATURE_CONSENT_REQUIRED", "The consent wording version is required.");
        }

        if (!LowerSha256.IsMatch(contentHash ?? string.Empty))
        {
            throw new AttachmentDomainException("SIGNATURE_IMAGE_INVALID", "A lower-case SHA-256 content hash of the image is required.");
        }

        if (imageFileOrganizationId != organizationId)
        {
            throw new AttachmentDomainException("ATTACHMENT_FILE_SCOPE_MISMATCH", "The image does not belong to the owner's organization.");
        }

        OrganizationId = organizationId;
        OwnerType = normalizedOwner;
        OwnerId = ownerId;
        Purpose = normalizedPurpose;
        SignerName = name;
        SignerRole = role;
        SignedAtUtc = signedAtUtc.ToUniversalTime();
        ImageFileId = imageFileId;
        ConsentTextVersion = consent;
        ContentHash = contentHash!;
        CapturedByUserId = capturedByUserId;
    }
}
```

แก้ `backend/src/TanErp.Domain/Files/FileUploadSession.cs`: เพิ่ม `using TanErp.Domain.Attachments;` และเปลี่ยน `FileParentTypes.IsValid` ให้เป็น:

```csharp
    public static bool IsValid(string? parentType) =>
        parentType is not null &&
        (string.Equals(parentType, Opportunity, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, Customer, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, Site, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, Item, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, ItemCategory, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, ItemBrand, StringComparison.OrdinalIgnoreCase)
         || string.Equals(parentType, CostRecord, StringComparison.OrdinalIgnoreCase)
         // Owner types registered for shared attachments are valid upload parents without further edits here.
         || AttachmentOwnerTypes.IsRegistered(parentType.ToLowerInvariant()));
```

- [ ] **Step 4: รันให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~AttachmentDomainTests|FullyQualifiedName~FileGatePermissionsTests|FullyQualifiedName~FileUploadSession"`
Expected: PASS ทั้งหมด (AttachmentDomainTests 19 cases, FileParentTypes 5 cases).

- [ ] **Step 5: build + commit**

Run: `dotnet build backend/TanErp.slnx --nologo -v q`
Expected: Build succeeded, 0 Error(s).

```bash
git add backend/src/TanErp.Domain/Common/SignatureEvidenceLimits.cs backend/src/TanErp.Domain/Attachments backend/src/TanErp.Domain/Files/FileUploadSession.cs backend/tests/TanErp.UnitTests/Attachments/AttachmentDomainTests.cs backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs
git commit -F - <<'EOF'
feat(attachments): add AttachmentLink and SignatureCapture domain with owner whitelist

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 3: Lift กฎหลักฐานลายเซ็นของ CP-07 ขึ้น `Application/Common/Security`

**Files:**
- Create: `backend/src/TanErp.Application/Common/Security/SignatureEvidenceRules.cs`
- Modify: `backend/src/TanErp.Application/Commercial/QuotationAcceptanceHandler.cs`
- Modify: `backend/src/TanErp.Application/Commercial/QuotationAcceptanceModels.cs`
- Test: `backend/tests/TanErp.UnitTests/Common/SignatureEvidenceRulesTests.cs`
- Regression: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` (เฉพาะ `ExternalAcceptance_*`, ไม่แก้ไฟล์)

หลักการ: **ไม่เปลี่ยน behavior ของ CP-07** — ชื่อ 2–200 ตัวอักษร, ตำแหน่ง ≤100 (วัดก่อน trim เหมือนเดิม), PNG ตรวจ 4 byte แรก (`89 50 4E 47`) และ `bytes.Length > 8`, hash = SHA-256 ของสตริงที่ส่งมา.

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Common/SignatureEvidenceRulesTests.cs`:

```csharp
using TanErp.Application.Common.Security;
using Xunit;

namespace TanErp.UnitTests.Common;

public class SignatureEvidenceRulesTests
{
    // 8-byte PNG signature + one filler byte: the shortest payload CP-07 accepts (length must be > 8).
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];

    [Theory]
    [InlineData("ab", true)]
    [InlineData("  ab  ", true)]
    [InlineData("a", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void TryNormalizeSigner_EnforcesTheNameLowerBound(string? name, bool expected)
    {
        Assert.Equal(expected, SignatureEvidenceRules.TryNormalizeSigner(name, null, out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_EnforcesTheNameUpperBound_AndTrims()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner(new string('x', 200), null, out var name, out _));
        Assert.Equal(200, name.Length);
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner(new string('x', 201), null, out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_MeasuresTheRoleBeforeTrimming_AsCp07Did()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 100), out _, out var role));
        Assert.Equal(100, role!.Length);
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 101), out _, out _));
        Assert.False(SignatureEvidenceRules.TryNormalizeSigner("ab", new string('r', 100) + " ", out _, out _));
    }

    [Fact]
    public void TryNormalizeSigner_TurnsABlankRoleIntoNull()
    {
        Assert.True(SignatureEvidenceRules.TryNormalizeSigner("ab", "   ", out _, out var role));
        Assert.Null(role);
    }

    [Fact]
    public void IsPngBase64_AcceptsRawAndDataUrlPayloads()
    {
        var base64 = Convert.ToBase64String(PngBytes);
        Assert.True(SignatureEvidenceRules.IsPngBase64(base64));
        Assert.True(SignatureEvidenceRules.IsPngBase64("data:image/png;base64," + base64));
    }

    [Fact]
    public void IsPngBase64_RejectsNonPngInvalidBase64AndTooShortPayloads()
    {
        Assert.False(SignatureEvidenceRules.IsPngBase64(Convert.ToBase64String(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0 })));
        Assert.False(SignatureEvidenceRules.IsPngBase64("not base64 !!"));
        Assert.False(SignatureEvidenceRules.IsPngBase64(Convert.ToBase64String(PngBytes[..8])));
    }

    [Fact]
    public void IsPngSignature_ChecksTheLeadingMagicBytes()
    {
        Assert.True(SignatureEvidenceRules.IsPngSignature(PngBytes));
        Assert.False(SignatureEvidenceRules.IsPngSignature(PngBytes[..8]));
        Assert.False(SignatureEvidenceRules.IsPngSignature(new byte[] { 0x00, 0x50, 0x4E, 0x47, 0, 0, 0, 0, 0 }));
    }

    [Fact]
    public void HashSubmittedImage_IsTheSha256OfTheSubmittedString()
    {
        var hash = SignatureEvidenceRules.HashSubmittedImage("abc");
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }

    [Theory]
    [InlineData("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", true)]
    [InlineData("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", false)]
    [InlineData("abc", false)]
    [InlineData(null, false)]
    public void IsSha256Hex_RequiresSixtyFourLowerCaseHexCharacters(string? value, bool expected)
    {
        Assert.Equal(expected, SignatureEvidenceRules.IsSha256Hex(value));
    }
}
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~SignatureEvidenceRulesTests"`
Expected: FAIL (build: `CS0103/CS0117 ... 'SignatureEvidenceRules'`).

- [ ] **Step 3: เขียนกฎร่วม**

`backend/src/TanErp.Application/Common/Security/SignatureEvidenceRules.cs`:

```csharp
using System.Text.RegularExpressions;
using TanErp.Domain.Common;

namespace TanErp.Application.Common.Security;

/// <summary>
/// Signature evidence rules shared by External Acceptance (CP-07) and shared signature capture (G-01).
/// Behavior is intentionally identical to what CP-07 shipped; change both consumers' tests before changing it.
/// </summary>
public static class SignatureEvidenceRules
{
    private const string PngDataUrlPrefix = "data:image/png;base64,";
    private static readonly Regex LowerSha256 = new("^[0-9a-f]{64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Validates and normalizes the signer. The role is measured before trimming, exactly as CP-07 did.</summary>
    public static bool TryNormalizeSigner(string? name, string? role, out string signerName, out string? signerRole)
    {
        signerName = name?.Trim() ?? string.Empty;
        signerRole = string.IsNullOrWhiteSpace(role) ? null : role.Trim();

        var nameOk = signerName.Length is >= SignatureEvidenceLimits.MinSignerNameLength and <= SignatureEvidenceLimits.MaxSignerNameLength;
        var roleOk = role is null || role.Length <= SignatureEvidenceLimits.MaxSignerRoleLength;
        return nameOk && roleOk;
    }

    /// <summary>True when the bytes start with the PNG signature (first four bytes) and are longer than eight bytes.</summary>
    public static bool IsPngSignature(ReadOnlySpan<byte> bytes) =>
        bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

    /// <summary>Accepts a raw base64 payload or a PNG data URL; the payload must decode to a PNG.</summary>
    public static bool IsPngBase64(string value)
    {
        var payload = value.StartsWith(PngDataUrlPrefix, StringComparison.Ordinal) ? value[PngDataUrlPrefix.Length..] : value;
        try
        {
            return IsPngSignature(Convert.FromBase64String(payload));
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>SHA-256 of the submitted string (CP-07 hashes the base64 text it received, not the decoded bytes).</summary>
    public static string HashSubmittedImage(string submittedImage) => Sha256Hex.Compute(submittedImage);

    public static bool IsSha256Hex(string? value) => value is not null && LowerSha256.IsMatch(value);
}
```

- [ ] **Step 4: รัน test กฎร่วมให้ผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~SignatureEvidenceRulesTests"`
Expected: PASS (ทุก case).

- [ ] **Step 5: ให้ CP-07 เรียกกฎร่วม**

`QuotationAcceptanceModels.cs`: เพิ่ม `using TanErp.Domain.Common;` และเปลี่ยน `AcceptanceConsent`:

```csharp
public static class AcceptanceConsent
{
    public const string CurrentVersion = "2026-10-v1";
    public const int MaxSignatureImageChars = SignatureEvidenceLimits.MaxImageChars;
}
```

`QuotationAcceptanceHandler.cs`: ตรงบล็อกตรวจ `name`/`submission`:

```csharp
        var name = submission?.SignerName?.Trim();
        if (submission is null || string.IsNullOrWhiteSpace(name) || name.Length is < 2 or > 200 || submission.SignerRole is { Length: > 100 })
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_SUBMISSION_INVALID", "A signer name of 2-200 characters is required.");
        }
```

แทนที่ด้วย:

```csharp
        if (submission is null || !SignatureEvidenceRules.TryNormalizeSigner(submission.SignerName, submission.SignerRole, out var name, out var role))
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_SUBMISSION_INVALID", "A signer name of 2-200 characters is required.");
        }
```

ในบล็อกภาพลายเซ็น แทน `!IsPngBase64(submission.SignatureImage)` ด้วย `!SignatureEvidenceRules.IsPngBase64(submission.SignatureImage)` และ `signatureHash = Sha256Hex.Compute(submission.SignatureImage);` ด้วย `signatureHash = SignatureEvidenceRules.HashSubmittedImage(submission.SignatureImage);`. ใน `RecordAcceptanceAsync(...)` เปลี่ยน `submission with { SignerName = name, SignerRole = string.IsNullOrWhiteSpace(submission.SignerRole) ? null : submission.SignerRole.Trim() }` เป็น `submission with { SignerName = name, SignerRole = role }`. ลบเมธอด private `IsPngBase64` ท้ายคลาสทิ้งทั้งก้อน (ไม่ต้องคง wrapper). ตัวแปร `name` ยังใช้ในข้อความ `($"Accepted by customer representative via acceptance link ({name}).")` ตามเดิม.

- [ ] **Step 6: build + regression ของ CP-07 (Integration, ต้องมี Docker)**

Run: `dotnet build backend/TanErp.slnx --nologo -v q`
Expected: Build succeeded.

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ExternalAcceptance|FullyQualifiedName~VoidQuotation_RecordsReasonAndAuthority" -m:1`
Expected: PASS ทั้ง 5 test (`ExternalAcceptance_CustomerAcceptsOnce_WithEvidence_AndNoCostData`, `..._UnusableLinks_AreIndistinguishableFromUnknownTokens`, `..._LinkOfAReplacedQuotation_CannotAcceptTheReplacement`, `..._PublicEndpointsAreRateLimitedPerClient`, `VoidQuotation_RecordsReasonAndAuthority_AndBlocksAcceptance`). ถ้าล้ม ให้แก้ที่การ lift (ห้ามแก้ test CP-07).

- [ ] **Step 7: commit**

```bash
git add backend/src/TanErp.Application/Common/Security/SignatureEvidenceRules.cs backend/src/TanErp.Application/Commercial/QuotationAcceptanceHandler.cs backend/src/TanErp.Application/Commercial/QuotationAcceptanceModels.cs backend/tests/TanErp.UnitTests/Common/SignatureEvidenceRulesTests.cs
git commit -F - <<'EOF'
refactor(acceptance): lift signature evidence rules into shared Application.Common.Security

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 4: Application — registry, models, interface, handler

**Files:**
- Create: `backend/src/TanErp.Application/Attachments/AttachmentOwnerRegistry.cs`
- Create: `backend/src/TanErp.Application/Attachments/AttachmentModels.cs`
- Create: `backend/src/TanErp.Application/Attachments/IAttachmentStore.cs`
- Create: `backend/src/TanErp.Application/Attachments/AttachmentHandler.cs`
- Create: `backend/src/TanErp.Application/Files/FileGatePermissions.cs` (ใช้ใน Task 6; สร้างตอนนี้พร้อม test)
- Test: `backend/tests/TanErp.UnitTests/Attachments/AttachmentHandlerTests.cs`
- Test: `backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs` (เพิ่ม case)

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Attachments/AttachmentHandlerTests.cs`:

```csharp
using TanErp.Application.Attachments;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Attachments;
using TanErp.Domain.Service;
using Xunit;

namespace TanErp.UnitTests.Attachments;

public class AttachmentHandlerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Branch = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();
    private static readonly Guid Membership = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly AttachmentCaller Caller = new("uid", Membership, "trace-1");

    private readonly FakeAccess _access = new();
    private readonly FakeScopes _scopes = new();
    private readonly FakeStore _store = new();
    private readonly AttachmentHandler _handler;

    public AttachmentHandlerTests()
    {
        _handler = new AttachmentHandler(_access, _scopes, _store);
        _access.Granted.UnionWith(["installations.read", "installations.operate", "installations.handover"]);
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.InProgress));
    }

    private static AttachFilesInput Attach(params Guid[] ids) => new("evidence", ids);

    private static SignatureCaptureInput Sign(string? name = "คุณสมชาย", bool consent = true, string? version = "handover-2026-10-v1", string? purpose = "handover") =>
        new(purpose, name, "เจ้าของบ้าน", Guid.NewGuid(), consent, version);

    [Fact]
    public void Registry_CoversExactlyTheDomainWhitelist()
    {
        Assert.True(AttachmentOwnerRegistry.OwnerTypes.ToHashSet().SetEquals(AttachmentOwnerTypes.All));
    }

    [Fact]
    public void Registry_Find_IsExactMatchOnly()
    {
        Assert.NotNull(AttachmentOwnerRegistry.Find("installation-job"));
        Assert.Null(AttachmentOwnerRegistry.Find("Installation-Job"));
        Assert.Null(AttachmentOwnerRegistry.Find(null));
        Assert.Null(AttachmentOwnerRegistry.Find("customer"));
    }

    [Fact]
    public async Task UnregisteredOwnerType_IsRejectedBeforeAnyAccessOrStoreCall()
    {
        var result = await _handler.ListAsync(Caller, "customer", OwnerId);

        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", result.Error.Code);
        Assert.Empty(_access.Requested);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task MissingPermission_ReturnsPermissionDenied_WithoutLookingUpTheOwner()
    {
        _access.Granted.Clear();

        var result = await _handler.ListAsync(Caller, "installation-job", OwnerId);

        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _scopes.Lookups);
    }

    [Fact]
    public async Task UnknownOwner_AndOwnerOfAnotherOrganization_BothReturnNotFound()
    {
        var unknown = await _handler.ListAsync(Caller, "installation-job", Guid.NewGuid());
        Assert.Equal("RESOURCE_NOT_FOUND", unknown.Error.Code);

        var foreign = Guid.NewGuid();
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, foreign, new AttachmentOwnerScope(Guid.NewGuid(), Branch, InstallationStatus.InProgress));
        var result = await _handler.ListAsync(Caller, "installation-job", foreign);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task OwnerOfAnotherBranch_IsNotFound_ForABranchScopedCaller_ButVisibleToAnOrganizationWideCaller()
    {
        _access.BranchId = Guid.NewGuid();
        Assert.Equal("RESOURCE_NOT_FOUND", (await _handler.ListAsync(Caller, "installation-job", OwnerId)).Error.Code);

        _access.BranchId = null;
        Assert.True((await _handler.ListAsync(Caller, "installation-job", OwnerId)).IsSuccess);
    }

    [Fact]
    public async Task OwnerTypeIsNormalizedBeforeLookup()
    {
        var result = await _handler.ListAsync(Caller, "  INSTALLATION-JOB ", OwnerId);
        Assert.True(result.IsSuccess);
        Assert.Equal("installation-job", _store.LastOwnerType);
    }

    [Fact]
    public async Task Attach_RejectsInvalidInput()
    {
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", null)).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach())).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.Empty))).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1",
            Attach(Enumerable.Range(0, AttachmentHandler.MaxFilesPerRequest + 1).Select(_ => Guid.NewGuid()).ToArray()))).Error.Code);
        Assert.Equal("ATTACHMENT_DUPLICATE", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1",
            Attach(Guid.Parse("11111111-1111-1111-1111-111111111111"), Guid.Parse("11111111-1111-1111-1111-111111111111")))).Error.Code);
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", new AttachFilesInput("misc", [Guid.NewGuid()]))).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Attach_RequiresTheManagePermission_NotJustRead()
    {
        _access.Granted.Remove("installations.operate");
        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()));
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Theory]
    [InlineData(InstallationStatus.HandedOver)]
    [InlineData(InstallationStatus.Cancelled)]
    public async Task Attach_AndUnlink_AreLockedForClosedOwners(string status)
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, status));

        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()))).Error.Code);
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.UnlinkAsync(Caller, "installation-job", OwnerId, Guid.NewGuid())).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Attach_PassesNormalizedInputAndHashedKeyToTheStore()
    {
        var file = Guid.NewGuid();
        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", new AttachFilesInput(" EVIDENCE ", [file]));

        Assert.True(result.IsSuccess);
        Assert.Equal("evidence", _store.LastAttach!.Purpose);
        Assert.Equal(new[] { file }, _store.LastAttach.FileIds);
        Assert.Equal(64, _store.LastKeyHash!.Length);
        Assert.NotEqual("key-1", _store.LastKeyHash);
        Assert.Equal("trace-1", _store.LastTraceId);
    }

    [Fact]
    public async Task Attach_DifferentFileListsProduceDifferentPayloadHashes()
    {
        await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", Attach(Guid.NewGuid()));
        var first = _store.LastPayloadHash;
        await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", Attach(Guid.NewGuid()));
        Assert.NotEqual(first, _store.LastPayloadHash);
    }

    [Fact]
    public async Task Signature_RequiresTheSignPermission()
    {
        _access.Granted.Remove("installations.handover");
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));
        Assert.Equal("PERMISSION_DENIED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign())).Error.Code);
    }

    [Fact]
    public async Task Signature_IsLockedUnlessTheOwnerIsReadyForHandover()
    {
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign())).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Signature_ValidatesNameConsentAndPurpose()
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));

        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(name: "ก"))).Error.Code);
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", null)).Error.Code);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(consent: false))).Error.Code);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(version: "old-v0"))).Error.Code);
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(purpose: "evidence"))).Error.Code);
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k",
            new SignatureCaptureInput("handover", "คุณสมชาย", null, Guid.Empty, true, "handover-2026-10-v1"))).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Signature_PassesNormalizedEvidenceToTheStore()
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));

        var result = await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "key-9",
            new SignatureCaptureInput(" Handover ", "  คุณสมชาย  ", "  เจ้าของบ้าน ", Guid.NewGuid(), true, "handover-2026-10-v1"));

        Assert.True(result.IsSuccess);
        Assert.Equal("handover", _store.LastSignature!.Purpose);
        Assert.Equal("คุณสมชาย", _store.LastSignature.SignerName);
        Assert.Equal("เจ้าของบ้าน", _store.LastSignature.SignerRole);
        Assert.Equal("handover-2026-10-v1", _store.LastSignature.ConsentTextVersion);
    }

    // ----- fakes -----------------------------------------------------------------------------------

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public HashSet<string> Granted { get; } = new(StringComparer.Ordinal);
        public List<string> Requested { get; } = new();
        public Guid? BranchId { get; set; } = Branch;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            Requested.Add(permissionKey);
            return Task.FromResult(Granted.Contains(permissionKey)
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, BranchId, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class FakeScopes : IAttachmentOwnerScopeReader
    {
        private readonly Dictionary<(string, Guid), AttachmentOwnerScope> _items = new();
        public int Lookups { get; private set; }

        public void Add(string ownerType, Guid ownerId, AttachmentOwnerScope scope) => _items[(ownerType, ownerId)] = scope;

        public Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default)
        {
            Lookups++;
            return Task.FromResult(_items.TryGetValue((ownerType, ownerId), out var scope) ? scope : null);
        }
    }

    private sealed class FakeStore : IAttachmentStore
    {
        public int Calls { get; private set; }
        public string? LastOwnerType { get; private set; }
        public AttachFilesInput? LastAttach { get; private set; }
        public SignatureCaptureCommand? LastSignature { get; private set; }
        public string? LastKeyHash { get; private set; }
        public string? LastPayloadHash { get; private set; }
        public string? LastTraceId { get; private set; }

        public Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
        {
            Calls++;
            LastOwnerType = ownerType;
            return Task.FromResult<IReadOnlyList<AttachmentLinkProjection>>([]);
        }

        public Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
        {
            Calls++;
            LastOwnerType = ownerType;
            LastAttach = input;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            LastTraceId = traceId;
            return Task.FromResult(Result<IReadOnlyList<AttachmentLinkProjection>>.Success([]));
        }

        public Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default)
        {
            Calls++;
            return Task.FromResult(Result<bool>.Success(true));
        }

        public Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<SignatureCaptureProjection>>([]);
        }

        public Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
        {
            Calls++;
            LastSignature = command;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(Result<SignatureCaptureProjection>.Success(new SignatureCaptureProjection(
                Guid.NewGuid(), ownerType, ownerId, command.Purpose, command.SignerName, command.SignerRole, DateTimeOffset.UtcNow,
                command.ImageFileId, "/x", command.ConsentTextVersion, new string('a', 64), new AttachmentPerson(User, "u"))));
        }
    }
}
```

เพิ่มใน `FileGatePermissionsTests.cs` (ในคลาสเดิม):

```csharp
    [Fact]
    public void UploadGate_KeepsTheExistingPermissionsAndAddsTheRegisteredOwnerManagePermissions()
    {
        Assert.Contains("opportunities.update", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("items.manage-images", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("installations.operate", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("installations.handover", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.DoesNotContain("installations.read", TanErp.Application.Files.FileGatePermissions.Upload);
    }

    [Fact]
    public void ReadGate_KeepsTheExistingPermissionsAndAddsTheRegisteredOwnerReadAndManagePermissions()
    {
        Assert.Contains("opportunities.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("items.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.operate", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.handover", TanErp.Application.Files.FileGatePermissions.Read);
    }
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~AttachmentHandlerTests|FullyQualifiedName~FileGatePermissionsTests"`
Expected: FAIL (build: `'AttachmentHandler' / 'AttachmentOwnerRegistry' / 'FileGatePermissions' could not be found`).

- [ ] **Step 3: เขียน implementation**

`backend/src/TanErp.Application/Attachments/AttachmentOwnerRegistry.cs`:

```csharp
using TanErp.Domain.Attachments;
using TanErp.Domain.Service;

namespace TanErp.Application.Attachments;

/// <summary>
/// What a registered owner type requires. Permissions are existing keys of the owner's own module; no attachment-specific keys exist.
/// </summary>
public sealed record AttachmentOwnerDescriptor(
    string OwnerType,
    string ReadPermission,
    string ManagePermission,
    string SignPermission,
    IReadOnlySet<string> ManageStates,
    IReadOnlySet<string> SignStates);

/// <summary>Code-defined registry. Must stay in step with AttachmentOwnerTypes (enforced by tests).</summary>
public static class AttachmentOwnerRegistry
{
    private static readonly IReadOnlyDictionary<string, AttachmentOwnerDescriptor> Descriptors = new[]
    {
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.InstallationJob,
            ReadPermission: "installations.read",
            ManagePermission: "installations.operate",
            SignPermission: "installations.handover",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.Planned, InstallationStatus.InProgress, InstallationStatus.ReadyForHandover },
            SignStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.ReadyForHandover })
    }.ToDictionary(d => d.OwnerType, StringComparer.Ordinal);

    public static IReadOnlyCollection<string> OwnerTypes { get; } = Descriptors.Keys.ToArray();

    /// <summary>Distinct permissions that allow reading attachments of any registered owner.</summary>
    public static IReadOnlyCollection<string> ReadPermissions { get; } = Descriptors.Values.Select(d => d.ReadPermission).Distinct(StringComparer.Ordinal).ToArray();

    /// <summary>Distinct permissions that allow uploading to (managing or signing for) any registered owner.</summary>
    public static IReadOnlyCollection<string> ManagePermissions { get; } = Descriptors.Values
        .SelectMany(d => new[] { d.ManagePermission, d.SignPermission })
        .Distinct(StringComparer.Ordinal)
        .ToArray();

    /// <summary>Exact (already normalized) owner type lookup.</summary>
    public static AttachmentOwnerDescriptor? Find(string? ownerType) =>
        ownerType is not null && Descriptors.TryGetValue(ownerType, out var descriptor) ? descriptor : null;
}
```

`backend/src/TanErp.Application/Attachments/AttachmentModels.cs`:

```csharp
using TanErp.Domain.Attachments;

namespace TanErp.Application.Attachments;

public sealed record AttachmentCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public sealed record AttachmentPerson(Guid Id, string DisplayName);

/// <summary>Where an owner record lives and what state it is in; null from the reader means "not visible to this organization".</summary>
public sealed record AttachmentOwnerScope(Guid OrganizationId, Guid? BranchId, string State);

public sealed record AttachFilesInput(string? Purpose, IReadOnlyList<Guid>? FileIds);

public sealed record SignatureCaptureInput(string? Purpose, string? SignerName, string? SignerRole, Guid ImageFileId, bool ConsentAccepted, string? ConsentTextVersion);

/// <summary>Normalized signature input that already passed handler validation.</summary>
public sealed record SignatureCaptureCommand(string Purpose, string SignerName, string? SignerRole, Guid ImageFileId, string ConsentTextVersion);

public sealed record AttachmentLinkProjection(
    Guid Id, string OwnerType, Guid OwnerId, Guid FileId, string Purpose, string Filename, string MediaType, long FileSizeBytes,
    string ServingUrl, AttachmentPerson CreatedBy, DateTimeOffset CreatedAtUtc);

public sealed record SignatureCaptureProjection(
    Guid Id, string OwnerType, Guid OwnerId, string Purpose, string SignerName, string? SignerRole, DateTimeOffset SignedAtUtc,
    Guid ImageFileId, string ServingUrl, string ConsentTextVersion, string ContentHash, AttachmentPerson CapturedBy);

/// <summary>Current consent wording version per signature purpose. The wording itself is localized in the UI.</summary>
public static class SignatureConsentVersions
{
    public const string Handover = "handover-2026-10-v1";

    public static string? Current(string purpose) => purpose == AttachmentPurposes.Handover ? Handover : null;
}
```

`backend/src/TanErp.Application/Attachments/IAttachmentStore.cs`:

```csharp
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Attachments;

/// <summary>Finds an owner record in the caller's organization. Implemented per owner type in Infrastructure.</summary>
public interface IAttachmentOwnerScopeReader
{
    Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default);
}

public interface IAttachmentStore
{
    Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default);

    Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default);

    Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
}
```

`backend/src/TanErp.Application/Attachments/AttachmentHandler.cs`:

```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Attachments;

namespace TanErp.Application.Attachments;

/// <summary>
/// Shared attachment and signature use cases. Order of checks: owner type (422) → permission (403) → owner visible in the caller's
/// organization/branch (404) → input (422) → owner state (409) → store.
/// </summary>
public class AttachmentHandler
{
    public const int MaxFilesPerRequest = 20;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IAttachmentOwnerScopeReader _scopes;
    private readonly IAttachmentStore _store;

    public AttachmentHandler(IRequestAccessResolver accessResolver, IAttachmentOwnerScopeReader scopes, IAttachmentStore store)
    {
        _accessResolver = accessResolver;
        _scopes = scopes;
        _store = store;
    }

    private enum OwnerOperation { Read, Manage, Sign }

    private sealed record ResolvedOwner(RequestAccessContext Access, AttachmentOwnerDescriptor Descriptor, AttachmentOwnerScope Scope);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private async Task<Result<ResolvedOwner>> ResolveOwnerAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, OwnerOperation operation, CancellationToken ct)
    {
        var descriptor = AttachmentOwnerRegistry.Find(AttachmentOwnerTypes.Normalize(ownerType));
        if (descriptor is null) return Fail<ResolvedOwner>("ATTACHMENT_OWNER_TYPE_INVALID", $"Owner type '{ownerType}' is not registered.");

        var permission = operation switch
        {
            OwnerOperation.Read => descriptor.ReadPermission,
            OwnerOperation.Manage => descriptor.ManagePermission,
            _ => descriptor.SignPermission
        };

        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);
        if (access.IsFailure) return Result<ResolvedOwner>.Failure(access.Error);

        var context = access.Value!;
        var scope = await _scopes.FindAsync(descriptor.OwnerType, ownerId, context.OrganizationId, ct);
        if (scope is null
            || scope.OrganizationId != context.OrganizationId
            || (scope.BranchId is { } branchId && !context.HasBranchAccess(branchId)))
        {
            return Fail<ResolvedOwner>("RESOURCE_NOT_FOUND", "Owner not found.");
        }

        return Result<ResolvedOwner>.Success(new ResolvedOwner(context, descriptor, scope));
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> ListAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Read, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<AttachmentLinkProjection>>.Failure(owner.Error);
        var items = await _store.ListLinksAsync(owner.Value!.Access.OrganizationId, owner.Value.Descriptor.OwnerType, ownerId, ct);
        return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        AttachmentCaller caller, string? ownerType, Guid ownerId, string idempotencyKey, AttachFilesInput? input, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Manage, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<AttachmentLinkProjection>>.Failure(owner.Error);

        var fileIds = input?.FileIds;
        if (input is null || fileIds is null || fileIds.Count == 0 || fileIds.Count > MaxFilesPerRequest || fileIds.Any(id => id == Guid.Empty))
        {
            return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_FIELD_INVALID", $"Between 1 and {MaxFilesPerRequest} files are required.");
        }

        var purpose = AttachmentPurposes.Normalize(input.Purpose);
        if (purpose is null) return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_PURPOSE_INVALID", $"Purpose '{input.Purpose}' is not supported.");
        if (fileIds.Distinct().Count() != fileIds.Count) return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_DUPLICATE", "The same file was listed more than once.");

        var resolved = owner.Value!;
        if (!resolved.Descriptor.ManageStates.Contains(resolved.Scope.State))
        {
            return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow attachment changes.");
        }

        var normalized = new AttachFilesInput(purpose, fileIds.ToList());
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(FormattableString.Invariant($"{resolved.Descriptor.OwnerType}|{ownerId:D}|{purpose}|{string.Join(",", fileIds.Select(id => id.ToString("D")))}"));
        return await _store.AttachAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, normalized, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<bool>> UnlinkAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, Guid linkId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Manage, ct);
        if (owner.IsFailure) return Result<bool>.Failure(owner.Error);

        var resolved = owner.Value!;
        if (!resolved.Descriptor.ManageStates.Contains(resolved.Scope.State))
        {
            return Fail<bool>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow attachment changes.");
        }

        return await _store.UnlinkAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, linkId, caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<SignatureCaptureProjection>>> ListSignaturesAsync(AttachmentCaller caller, string? ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Read, ct);
        if (owner.IsFailure) return Result<IReadOnlyList<SignatureCaptureProjection>>.Failure(owner.Error);
        var items = await _store.ListSignaturesAsync(owner.Value!.Access.OrganizationId, owner.Value.Descriptor.OwnerType, ownerId, ct);
        return Result<IReadOnlyList<SignatureCaptureProjection>>.Success(items);
    }

    public async Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        AttachmentCaller caller, string? ownerType, Guid ownerId, string idempotencyKey, SignatureCaptureInput? input, CancellationToken ct = default)
    {
        var owner = await ResolveOwnerAsync(caller, ownerType, ownerId, OwnerOperation.Sign, ct);
        if (owner.IsFailure) return Result<SignatureCaptureProjection>.Failure(owner.Error);

        if (input is null || input.ImageFileId == Guid.Empty
            || !SignatureEvidenceRules.TryNormalizeSigner(input.SignerName, input.SignerRole, out var signerName, out var signerRole))
        {
            return Fail<SignatureCaptureProjection>("SIGNATURE_SUBMISSION_INVALID", "A signer name of 2-200 characters and a signature image are required.");
        }

        var purpose = AttachmentPurposes.Normalize(input.Purpose);
        var consentVersion = purpose is null ? null : SignatureConsentVersions.Current(purpose);
        if (purpose is null || consentVersion is null)
        {
            return Fail<SignatureCaptureProjection>("ATTACHMENT_PURPOSE_INVALID", "Signatures are supported only for the handover purpose.");
        }

        if (!input.ConsentAccepted || input.ConsentTextVersion != consentVersion)
        {
            return Fail<SignatureCaptureProjection>("SIGNATURE_CONSENT_REQUIRED", "The current consent statement must be accepted.");
        }

        var resolved = owner.Value!;
        if (!resolved.Descriptor.SignStates.Contains(resolved.Scope.State))
        {
            return Fail<SignatureCaptureProjection>("ATTACHMENT_OWNER_LOCKED", "The record's status does not allow signatures.");
        }

        var command = new SignatureCaptureCommand(purpose, signerName, signerRole, input.ImageFileId, consentVersion);
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(FormattableString.Invariant(
            $"{resolved.Descriptor.OwnerType}|{ownerId:D}|{purpose}|{signerName}|{signerRole}|{input.ImageFileId:D}|{consentVersion}"));
        return await _store.CaptureSignatureAsync(resolved.Access, resolved.Descriptor.OwnerType, ownerId, command, keyHash, payloadHash, caller.TraceId, ct);
    }
}
```

`backend/src/TanErp.Application/Files/FileGatePermissions.cs`:

```csharp
using TanErp.Application.Attachments;

namespace TanErp.Application.Files;

/// <summary>
/// First-gate permission lists for the Files module (an actor needs at least one). The parent access resolver still
/// checks the exact permission for the specific parent. Owner types registered for shared attachments extend both lists.
/// </summary>
public static class FileGatePermissions
{
    private static readonly string[] UploadBase =
    [
        "opportunities.update",
        "customers.update",
        "customers.create",
        "sites.update",
        "sites.create",
        "items.manage-images",
        "items.update"
    ];

    private static readonly string[] ReadBase =
    [
        "opportunities.read",
        "opportunities.update",
        "customers.read",
        "customers.update",
        "customers.create",
        "sites.read",
        "sites.update",
        "sites.create",
        "items.read",
        "items.manage-images",
        "items.update"
    ];

    public static IReadOnlyCollection<string> Upload { get; } = [.. UploadBase, .. AttachmentOwnerRegistry.ManagePermissions];

    public static IReadOnlyCollection<string> Read { get; } =
        [.. ReadBase, .. AttachmentOwnerRegistry.ReadPermissions, .. AttachmentOwnerRegistry.ManagePermissions];
}
```

- [ ] **Step 4: รันให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~AttachmentHandlerTests|FullyQualifiedName~FileGatePermissionsTests|FullyQualifiedName~AttachmentDomainTests"`
Expected: PASS ทั้งหมด.

- [ ] **Step 5: build + commit**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded.
Run: `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj` → Expected: PASS (Application ไม่ต้องพึ่ง Infrastructure/Api).

```bash
git add backend/src/TanErp.Application/Attachments backend/src/TanErp.Application/Files/FileGatePermissions.cs backend/tests/TanErp.UnitTests/Attachments/AttachmentHandlerTests.cs backend/tests/TanErp.UnitTests/Files/FileGatePermissionsTests.cs
git commit -F - <<'EOF'
feat(attachments): add owner registry and attachment handler with scope and state checks

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 5: Infrastructure — EF configuration และ migration

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/AttachmentLinkConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/SignatureCaptureConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Generate: `backend/src/TanErp.Infrastructure/Persistence/Migrations/<timestamp>_AddSharedAttachmentsAndSignatures.cs` (+ `.Designer.cs`, + `AppDbContextModelSnapshot.cs`)

test ของขั้นนี้คือ migration ถูกสร้างและ apply ได้จริงบน PostgreSQL (Task 8 ใช้ `MigrateAsync()` ทุกครั้ง); ที่นี่ตรวจ model snapshot และ `Down()`.

- [ ] **Step 1: เขียน configuration**

`AttachmentLinkConfiguration.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Attachments;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class AttachmentLinkConfiguration : IEntityTypeConfiguration<AttachmentLink>
{
    public void Configure(EntityTypeBuilder<AttachmentLink> builder)
    {
        builder.ToTable("attachment_links", "files", t =>
        {
            // The real owner-type list lives in code (AttachmentOwnerTypes); the database only guards the shape.
            t.HasCheckConstraint("ck_attachment_links_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
            t.HasCheckConstraint("ck_attachment_links_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
            t.HasCheckConstraint("ck_attachment_links_removed_pair", "(removed_at_utc IS NULL) = (removed_by_user_id IS NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.IsActive);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.OwnerType).HasColumnName("owner_type").HasMaxLength(40).IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RemovedAtUtc).HasColumnName("removed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RemovedByUserId).HasColumnName("removed_by_user_id");

        // One active link per (owner, file, purpose); also closes the race two concurrent requests could open.
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId, x.FileId, x.Purpose })
            .IsUnique()
            .HasFilter("removed_at_utc IS NULL")
            .HasDatabaseName("ux_attachment_links_active_owner_file_purpose");
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId }).HasDatabaseName("ix_attachment_links_owner");
        builder.HasIndex(x => x.FileId).HasDatabaseName("ix_attachment_links_file");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RemovedByUserId).OnDelete(DeleteBehavior.Restrict);

        // Composite key makes "file in the same organization as the link" a database guarantee.
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.FileId, x.OrganizationId })
            .HasPrincipalKey(f => new { f.Id, f.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

`SignatureCaptureConfiguration.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Attachments;
using TanErp.Domain.Files;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SignatureCaptureConfiguration : IEntityTypeConfiguration<SignatureCapture>
{
    public void Configure(EntityTypeBuilder<SignatureCapture> builder)
    {
        builder.ToTable("signature_captures", "files", t =>
        {
            t.HasCheckConstraint("ck_signature_captures_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
            t.HasCheckConstraint("ck_signature_captures_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
            t.HasCheckConstraint("ck_signature_captures_content_hash", "content_hash ~ '^[0-9a-f]{64}$'");
            t.HasCheckConstraint("ck_signature_captures_signer_name", "char_length(btrim(signer_name)) BETWEEN 2 AND 200");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.OwnerType).HasColumnName("owner_type").HasMaxLength(40).IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SignerName).HasColumnName("signer_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.SignerRole).HasColumnName("signer_role").HasMaxLength(100);
        builder.Property(x => x.SignedAtUtc).HasColumnName("signed_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id").IsRequired();
        builder.Property(x => x.ConsentTextVersion).HasColumnName("consent_text_version").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ContentHash).HasColumnName("content_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CapturedByUserId).HasColumnName("captured_by_user_id").IsRequired();

        // One image file can be the evidence of one signature only.
        builder.HasIndex(x => x.ImageFileId).IsUnique().HasDatabaseName("ux_signature_captures_image_file");
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId, x.SignedAtUtc }).HasDatabaseName("ix_signature_captures_owner");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CapturedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.ImageFileId, x.OrganizationId })
            .HasPrincipalKey(f => new { f.Id, f.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

ใน `AppDbContext.cs` ใต้ `UploadedFiles` (บรรทัด `public DbSet<UploadedFile> UploadedFiles ...`) เพิ่ม:

```csharp
    public DbSet<TanErp.Domain.Attachments.AttachmentLink> AttachmentLinks => Set<TanErp.Domain.Attachments.AttachmentLink>();
    public DbSet<TanErp.Domain.Attachments.SignatureCapture> SignatureCaptures => Set<TanErp.Domain.Attachments.SignatureCapture>();
```

- [ ] **Step 2: build ให้ผ่านก่อน generate**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q`
Expected: Build succeeded, 0 Error(s).

- [ ] **Step 3: generate migration**

Run: `dotnet ef --version` (ถ้าไม่พบให้ `dotnet tool install --global dotnet-ef`)
Run:

```bash
cd /Users/syaco/Documents/development/tan-erp
dotnet ef migrations add AddSharedAttachmentsAndSignatures --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```

Expected: `Done.` และมีไฟล์ใหม่ `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddSharedAttachmentsAndSignatures.cs`, `.Designer.cs`, และ `AppDbContextModelSnapshot.cs` ถูกแก้.

- [ ] **Step 4: ตรวจ migration ด้วยตา**

Run: `grep -n "CreateTable\|schema: \"files\"\|name: \"attachment_links\"\|name: \"signature_captures\"\|HasFilter\|filter:" backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddSharedAttachmentsAndSignatures.cs`
Expected: `CreateTable` สองตาราง (`attachment_links`, `signature_captures`) ทั้งคู่ `schema: "files"`, มี `filter: "removed_at_utc IS NULL"` บน `ux_attachment_links_active_owner_file_purpose`, มี `foreignKey` composite ไป `uploaded_files` (`columns: new[] { "file_id", "organization_id" }`).
Run: `grep -n "Down(" -A12 backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddSharedAttachmentsAndSignatures.cs`
Expected: `Down()` มี `DropTable` ทั้งสอง (ย้อนกลับได้). ถ้ามีการเปลี่ยนตารางอื่นที่ไม่เกี่ยวกับ attachment/signature ปนมา ให้หยุดและตรวจ model drift ก่อน (อย่า commit).

- [ ] **Step 5: commit**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded.

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Configurations/AttachmentLinkConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/Configurations/SignatureCaptureConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs backend/src/TanErp.Infrastructure/Persistence/Migrations
git commit -F - <<'EOF'
feat(attachments): add attachment link and signature capture tables

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 6: Infrastructure — store, owner scope reader และเชื่อม Files module

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentOwnerScopeReader.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`
- Modify: `backend/src/TanErp.Application/Files/CreateUploadSession/CreateUploadSessionHandler.cs`, `CompleteUploadSession/CompleteUploadSessionHandler.cs`, `GetFileContent/GetFileContentHandler.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/AttachmentOwnerScopeReaderParityTests.cs`
- Regression: `backend/tests/TanErp.UnitTests/Files/CompleteUploadSessionHandlerTests.cs`

พฤติกรรมของ store/resolver ทดสอบจริงใน Task 8 (บน PostgreSQL); ที่นี่เขียน parity test (ไม่ต้องใช้ DB) ก่อน แล้วเขียน implementation.

- [ ] **Step 1: เขียน parity test ที่ล้มก่อน**

`backend/tests/TanErp.IntegrationTests/Persistence/AttachmentOwnerScopeReaderParityTests.cs`:

```csharp
using TanErp.Application.Attachments;
using TanErp.Domain.Attachments;
using TanErp.Infrastructure.Persistence.Attachments;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class AttachmentOwnerScopeReaderParityTests
{
    [Fact]
    public void EveryRegisteredOwnerType_HasAScopeReaderBranch_AndViceVersa()
    {
        var registry = AttachmentOwnerRegistry.OwnerTypes.ToHashSet(StringComparer.Ordinal);

        Assert.True(registry.SetEquals(AttachmentOwnerScopeReader.SupportedOwnerTypes),
            "AttachmentOwnerRegistry and AttachmentOwnerScopeReader.SupportedOwnerTypes must list the same owner types.");
        Assert.True(registry.SetEquals(AttachmentOwnerTypes.All),
            "AttachmentOwnerRegistry and AttachmentOwnerTypes must list the same owner types.");
    }
}
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~AttachmentOwnerScopeReaderParityTests"`
Expected: FAIL (build: `'AttachmentOwnerScopeReader' could not be found`).

- [ ] **Step 3: เขียน scope reader**

`backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentOwnerScopeReader.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Attachments;
using TanErp.Domain.Attachments;

namespace TanErp.Infrastructure.Persistence.Attachments;

/// <summary>
/// Finds an owner record inside one organization. Owner types are matched against a fixed set; the value is never used to build SQL.
/// Add one case (and one entry in SupportedOwnerTypes) per registered owner type.
/// </summary>
public class AttachmentOwnerScopeReader : IAttachmentOwnerScopeReader
{
    public static readonly IReadOnlySet<string> SupportedOwnerTypes = new HashSet<string>(StringComparer.Ordinal) { AttachmentOwnerTypes.InstallationJob };

    private readonly AppDbContext _db;

    public AttachmentOwnerScopeReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default)
    {
        switch (ownerType)
        {
            case AttachmentOwnerTypes.InstallationJob:
                return await _db.InstallationJobs.AsNoTracking()
                    .Where(j => j.Id == ownerId && j.OrganizationId == organizationId)
                    .Select(j => new AttachmentOwnerScope(j.OrganizationId, j.BranchId, j.Status))
                    .FirstOrDefaultAsync(ct);
            default:
                return null;
        }
    }
}
```

- [ ] **Step 4: เขียน store**

`backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentStore.cs`:

```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Attachments;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Domain.Attachments;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Attachments;

public class AttachmentStore : IAttachmentStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IFileStore _files;

    public AttachmentStore(AppDbContext db, IClock clock, IFileStore files)
    {
        _db = db;
        _clock = clock;
        _files = files;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static string ServingUrl(Guid fileId) => $"/api/v1/files/{fileId}/content";

    private void Audit(RequestAccessContext access, string action, Guid ownerId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "AttachmentOwner", ownerId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: null));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private static bool IsUniqueViolation(DbUpdateException ex) => ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    // ===== attachments =============================================================================

    public async Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var rows = await (
            from l in _db.AttachmentLinks.AsNoTracking()
            where l.OrganizationId == organizationId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null
            join f in _db.UploadedFiles.AsNoTracking() on new { Id = l.FileId, l.OrganizationId } equals new { f.Id, f.OrganizationId }
            join u in _db.Users.AsNoTracking() on l.CreatedByUserId equals u.Id
            orderby l.CreatedAtUtc, l.Id
            select new { Link = l, f.OriginalFilename, f.MediaType, f.FileSizeBytes, UserId = u.Id, u.DisplayName }).ToListAsync(ct);

        return rows.Select(r => new AttachmentLinkProjection(
            r.Link.Id, r.Link.OwnerType, r.Link.OwnerId, r.Link.FileId, r.Link.Purpose, r.OriginalFilename, r.MediaType, r.FileSizeBytes,
            ServingUrl(r.Link.FileId), new AttachmentPerson(r.UserId, r.DisplayName), r.Link.CreatedAtUtc)).ToList();
    }

    public async Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var operation = $"attachments.attach.{ownerType}";
        var orgId = access.OrganizationId;
        var fileIds = input.FileIds!;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<IReadOnlyList<AttachmentLinkProjection>>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(await ListLinksAsync(orgId, ownerType, ownerId, ct));
            }

            // Verified, same organization, and uploaded through a session bound to this very owner.
            var validation = await _files.ValidateVerifiedFilesForParentAsync(orgId, access.ActorUserId, ownerType, ownerId, null, fileIds, ct);
            if (validation.IsFailure)
            {
                return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_FILE_NOT_READY", "A file is not verified or was not uploaded for this record.");
            }

            var files = await _db.UploadedFiles.AsNoTracking().Where(f => fileIds.Contains(f.Id) && f.OrganizationId == orgId).ToListAsync(ct);
            var active = await _db.AttachmentLinks
                .Where(l => l.OrganizationId == orgId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null)
                .ToListAsync(ct);

            var now = _clock.UtcNow;
            try
            {
                foreach (var fileId in fileIds)
                {
                    var file = files.Single(f => f.Id == fileId);
                    var link = new AttachmentLink(Guid.NewGuid(), orgId, ownerType, ownerId, file.OrganizationId, file.Id, input.Purpose!, access.ActorUserId, now);
                    AttachmentLink.AssertCanAdd(active, link);
                    active.Add(link);
                    _db.AttachmentLinks.Add(link);
                }
            }
            catch (AttachmentDomainException ex)
            {
                // All-or-nothing: drop anything already staged in this request so no partial link can be saved.
                _db.ChangeTracker.Clear();
                return Fail<IReadOnlyList<AttachmentLinkProjection>>(ex.Code, ex.Message);
            }

            Audit(access, "attachment.linked", ownerId, traceId, new { ownerType, purpose = input.Purpose, count = fileIds.Count, fileIds }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, ownerId.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                await tx.RollbackAsync(ct);
                _db.ChangeTracker.Clear();
                return Fail<IReadOnlyList<AttachmentLinkProjection>>("ATTACHMENT_DUPLICATE", "This file is already attached to this record for the same purpose.");
            }

            return Result<IReadOnlyList<AttachmentLinkProjection>>.Success(await ListLinksAsync(orgId, ownerType, ownerId, ct));
        });
    }

    public async Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default)
    {
        var link = await _db.AttachmentLinks.FirstOrDefaultAsync(l =>
            l.Id == linkId && l.OrganizationId == access.OrganizationId && l.OwnerType == ownerType && l.OwnerId == ownerId && l.RemovedAtUtc == null, ct);
        if (link is null) return Fail<bool>("RESOURCE_NOT_FOUND", "Attachment not found.");

        var now = _clock.UtcNow;
        link.Remove(access.ActorUserId, now);
        Audit(access, "attachment.unlinked", ownerId, traceId, new { ownerType, purpose = link.Purpose, fileId = link.FileId }, now);
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    // ===== signatures ==============================================================================

    public async Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
    {
        var rows = await (
            from s in _db.SignatureCaptures.AsNoTracking()
            where s.OrganizationId == organizationId && s.OwnerType == ownerType && s.OwnerId == ownerId
            join u in _db.Users.AsNoTracking() on s.CapturedByUserId equals u.Id
            orderby s.SignedAtUtc, s.Id
            select new { Signature = s, UserId = u.Id, u.DisplayName }).ToListAsync(ct);

        return rows.Select(r => ToProjection(r.Signature, new AttachmentPerson(r.UserId, r.DisplayName))).ToList();
    }

    private static SignatureCaptureProjection ToProjection(SignatureCapture s, AttachmentPerson capturedBy) => new(
        s.Id, s.OwnerType, s.OwnerId, s.Purpose, s.SignerName, s.SignerRole, s.SignedAtUtc, s.ImageFileId, ServingUrl(s.ImageFileId),
        s.ConsentTextVersion, s.ContentHash, capturedBy);

    public async Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(
        RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var operation = $"attachments.signature.{ownerType}";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<SignatureCaptureProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id)
                    ? (await ListSignaturesAsync(orgId, ownerType, ownerId, ct)).FirstOrDefault(s => s.Id == id)
                    : null;
                if (replayed is not null) return Result<SignatureCaptureProjection>.Success(replayed);
            }

            var validation = await _files.ValidateVerifiedFilesForParentAsync(orgId, access.ActorUserId, ownerType, ownerId, null, [command.ImageFileId], ct);
            if (validation.IsFailure) return Fail<SignatureCaptureProjection>("ATTACHMENT_FILE_NOT_READY", "The image is not verified or was not uploaded for this record.");

            var file = await _db.UploadedFiles.AsNoTracking().FirstAsync(f => f.Id == command.ImageFileId && f.OrganizationId == orgId, ct);
            if (file.MediaType != "image/png" || !SignatureEvidenceRules.IsSha256Hex(file.ContentSha256))
            {
                return Fail<SignatureCaptureProjection>("SIGNATURE_IMAGE_INVALID", "The signature image must be a verified PNG.");
            }

            var now = _clock.UtcNow;
            SignatureCapture capture;
            try
            {
                capture = new SignatureCapture(
                    Guid.NewGuid(), orgId, ownerType, ownerId, command.Purpose, command.SignerName, command.SignerRole, now,
                    file.OrganizationId, file.Id, command.ConsentTextVersion, file.ContentSha256!, access.ActorUserId);
            }
            catch (AttachmentDomainException ex)
            {
                return Fail<SignatureCaptureProjection>(ex.Code, ex.Message);
            }

            _db.SignatureCaptures.Add(capture);
            // No signer name or role in the audit trail (personal data stays in the capture row only).
            Audit(access, "signature.captured", ownerId, traceId,
                new { ownerType, purpose = capture.Purpose, consentTextVersion = capture.ConsentTextVersion, contentHash = capture.ContentHash, imageFileId = capture.ImageFileId }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, capture.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                await tx.RollbackAsync(ct);
                _db.ChangeTracker.Clear();
                return Fail<SignatureCaptureProjection>("ATTACHMENT_DUPLICATE", "This image is already used as a signature.");
            }

            var actor = await _db.Users.AsNoTracking().Where(u => u.Id == access.ActorUserId).Select(u => new AttachmentPerson(u.Id, u.DisplayName)).FirstAsync(ct);
            return Result<SignatureCaptureProjection>.Success(ToProjection(capture, actor));
        });
    }
}
```

หมายเหตุสำหรับผู้ implement: ตรวจ namespace ของ `AuditEvent` และ `IdempotencyRecord` (`TanErp.Domain.Common`) และ constructor named args (`branchId`, `actorMembershipId`, `rowVersionAfter`) ให้ตรงกับที่ `ServiceStore.Audit` ใช้อยู่ (คัดลอกรูปแบบเดียวกันแล้ว).

- [ ] **Step 5: เชื่อม Files module**

`FileParentAccessResolver.cs`:

1. เพิ่ม `using TanErp.Application.Attachments;` และ field/constructor:

```csharp
    private readonly IAttachmentOwnerScopeReader _ownerScopes;

    public FileParentAccessResolver(AppDbContext db, IClock clock, IAttachmentOwnerScopeReader ownerScopes)
    {
        _db = db;
        _clock = clock;
        _ownerScopes = ownerScopes;
    }
```

2. แทนที่ `default:` ท้าย `switch` (ที่ตอนนี้ return `FILE_PARENT_TYPE_INVALID`) ด้วย:

```csharp
            default:
                return await ResolveRegisteredOwnerAsync(access, normalizedParentType, parentId, operation, cancellationToken);
```

3. เพิ่มเมธอด private (ก่อน `HasPermissionAsync`):

```csharp
    /// <summary>
    /// Parents registered for shared attachments: permission comes from the owner's descriptor and the owner must be visible
    /// in the caller's organization/branch. Read needs the read, manage or sign permission; every other operation needs manage or sign.
    /// </summary>
    private async Task<Result<FileParentAccess>> ResolveRegisteredOwnerAsync(
        RequestAccessContext access,
        string normalizedParentType,
        Guid? parentId,
        FileAccessOperation operation,
        CancellationToken cancellationToken)
    {
        var descriptor = AttachmentOwnerRegistry.Find(normalizedParentType);
        if (descriptor is null)
        {
            return Result<FileParentAccess>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", $"Parent type '{normalizedParentType}' is invalid."));
        }

        if (!parentId.HasValue)
        {
            return Result<FileParentAccess>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", $"{normalizedParentType} upload requires an existing parentId."));
        }

        var allowed = operation == FileAccessOperation.Read
            && await HasPermissionAsync(access.MembershipId, descriptor.ReadPermission, cancellationToken);
        allowed = allowed
            || await HasPermissionAsync(access.MembershipId, descriptor.ManagePermission, cancellationToken)
            || await HasPermissionAsync(access.MembershipId, descriptor.SignPermission, cancellationToken);

        if (!allowed)
        {
            return Result<FileParentAccess>.Failure(
                new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
        }

        var scope = await _ownerScopes.FindAsync(descriptor.OwnerType, parentId.Value, access.OrganizationId, cancellationToken);
        if (scope is null || (scope.BranchId is { } branchId && !access.HasBranchAccess(branchId)))
        {
            return Result<FileParentAccess>.Failure(new Error("RESOURCE_NOT_FOUND", "Owner not found."));
        }

        return Result<FileParentAccess>.Success(new FileParentAccess(normalizedParentType, parentId, null, access.OrganizationId));
    }
```

ใน Create/Complete/GetFileContent handler แทนอาร์เรย์ permission ที่เขียนตรง ๆ ด้วย `FileGatePermissions.Upload` (Create, Complete) และ `FileGatePermissions.Read` (GetFileContent), เช่น:

```csharp
        var accessResult = await _accessResolver.ResolveAnyAsync(
            command.FirebaseUid,
            command.MembershipId,
            FileGatePermissions.Upload,
            cancellationToken);
```

(`FileGatePermissions` อยู่ namespace `TanErp.Application.Files` ซึ่งทั้งสาม handler อยู่ใน sub-namespace อยู่แล้ว จึงไม่ต้อง using เพิ่ม.)

- [ ] **Step 6: รันให้ผ่าน + regression ของ Files module**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q`
Expected: Build succeeded (ถ้า DI ของ `FileParentAccessResolver` ยังไม่ลงทะเบียน `IAttachmentOwnerScopeReader` จะ fail ตอน runtime ใน Task 7 เท่านั้น).

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~AttachmentOwnerScopeReaderParityTests"`
Expected: PASS.

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~CompleteUploadSessionHandlerTests|FullyQualifiedName~FileGatePermissionsTests"`
Expected: PASS.

- [ ] **Step 7: commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Attachments backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs backend/src/TanErp.Application/Files/CreateUploadSession/CreateUploadSessionHandler.cs backend/src/TanErp.Application/Files/CompleteUploadSession/CompleteUploadSessionHandler.cs backend/src/TanErp.Application/Files/GetFileContent/GetFileContentHandler.cs backend/tests/TanErp.IntegrationTests/Persistence/AttachmentOwnerScopeReaderParityTests.cs
git commit -F - <<'EOF'
feat(attachments): add attachment store, owner scope reader and registered-owner file access

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 7: Api — error codes (th/en), contracts, controller, DI, OpenAPI snapshot

**Files:**
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`, `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Create: `backend/src/TanErp.Api/Contracts/Attachments/AttachmentContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/AttachmentsController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Regenerate: `contracts/openapi/tan-erp.v1.json`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs` (ไม่แก้ไฟล์; รันเพื่อ regenerate)

test พฤติกรรม endpoint อยู่ใน Task 8; ขั้นนี้พิสูจน์ด้วย build + OpenAPI snapshot ที่มี schema ใหม่ (Step 5 มี assertion ชั่วคราวผ่าน `grep`).

- [ ] **Step 1: error code → status**

ใน `ProblemDetailsMapper.GetStatus` เพิ่มบรรทัด (ใต้ `"ACCEPTANCE_CONFLICT" => ...`):

```csharp
        "ATTACHMENT_OWNER_TYPE_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "ATTACHMENT_PURPOSE_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "ATTACHMENT_FIELD_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "ATTACHMENT_FILE_NOT_READY" => StatusCodes.Status422UnprocessableEntity,
        "ATTACHMENT_FILE_SCOPE_MISMATCH" => StatusCodes.Status422UnprocessableEntity,
        "ATTACHMENT_DUPLICATE" => StatusCodes.Status409Conflict,
        "ATTACHMENT_LIMIT_EXCEEDED" => StatusCodes.Status409Conflict,
        "ATTACHMENT_OWNER_LOCKED" => StatusCodes.Status409Conflict,
        "SIGNATURE_SUBMISSION_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "SIGNATURE_CONSENT_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "SIGNATURE_IMAGE_INVALID" => StatusCodes.Status422UnprocessableEntity,
```

- [ ] **Step 2: ข้อความ th/en ใน resx**

สร้างสคริปต์ชั่วคราวใน scratchpad แล้วรัน (ไม่ commit สคริปต์):

```bash
cd /Users/syaco/Documents/development/tan-erp
python3 - <<'PY'
codes = {
  "ATTACHMENT_OWNER_TYPE_INVALID": ("ประเภทรายการที่แนบไฟล์ไม่ถูกต้อง", "Invalid Attachment Owner", "ประเภทรายการที่แนบไฟล์ไม่ถูกต้อง", "The attachment owner type is not supported."),
  "ATTACHMENT_PURPOSE_INVALID": ("ประเภทไฟล์แนบไม่ถูกต้อง", "Invalid Attachment Purpose", "ประเภทไฟล์แนบไม่ถูกต้อง", "The attachment purpose is not supported."),
  "ATTACHMENT_FIELD_INVALID": ("ข้อมูลไฟล์แนบไม่ถูกต้อง", "Invalid Attachment Data", "เลือกไฟล์ได้ 1-20 ไฟล์ต่อครั้งและไม่ซ้ำกัน", "Choose between 1 and 20 distinct files per request."),
  "ATTACHMENT_FILE_NOT_READY": ("ไฟล์ยังไม่พร้อมแนบ", "File Not Ready", "ไฟล์ยังไม่ผ่านการตรวจสอบหรือไม่ได้อัปโหลดสำหรับรายการนี้", "A file is not verified or was not uploaded for this record."),
  "ATTACHMENT_FILE_SCOPE_MISMATCH": ("ไฟล์อยู่นอกองค์กร", "File Scope Mismatch", "ไฟล์ไม่ได้อยู่ในองค์กรเดียวกับรายการนี้", "The file does not belong to this record's organization."),
  "ATTACHMENT_DUPLICATE": ("ไฟล์ถูกแนบแล้ว", "Attachment Already Exists", "ไฟล์นี้ถูกแนบกับรายการนี้ด้วยประเภทเดียวกันแล้ว", "This file is already attached to this record for the same purpose."),
  "ATTACHMENT_LIMIT_EXCEEDED": ("แนบไฟล์เกินจำนวนที่กำหนด", "Attachment Limit Exceeded", "แนบไฟล์ได้ไม่เกิน 50 ไฟล์ต่อรายการ", "A record can have at most 50 attachments."),
  "ATTACHMENT_OWNER_LOCKED": ("สถานะรายการไม่อนุญาต", "Record Locked", "สถานะของรายการนี้ไม่อนุญาตให้แก้ไฟล์แนบหรือลายเซ็น", "The record's status does not allow attachment or signature changes."),
  "SIGNATURE_SUBMISSION_INVALID": ("ข้อมูลลายเซ็นไม่ถูกต้อง", "Invalid Signature Data", "ชื่อผู้ลงนามต้องมี 2-200 ตัวอักษรและต้องมีภาพลายเซ็น", "A signer name of 2-200 characters and a signature image are required."),
  "SIGNATURE_CONSENT_REQUIRED": ("ต้องยอมรับข้อความยินยอม", "Consent Required", "ต้องยอมรับข้อความยินยอมฉบับปัจจุบัน", "The current consent statement must be accepted."),
  "SIGNATURE_IMAGE_INVALID": ("ภาพลายเซ็นไม่ถูกต้อง", "Invalid Signature Image", "ภาพลายเซ็นต้องเป็นไฟล์ PNG ที่ผ่านการตรวจสอบแล้ว", "The signature image must be a verified PNG file."),
}
def esc(s): return s.replace("&","&amp;").replace("<","&lt;").replace(">","&gt;")
for path, (ti, di) in {"backend/src/TanErp.Api/Resources/Errors.resx": (0, 2), "backend/src/TanErp.Api/Resources/Errors.en.resx": (1, 3)}.items():
    text = open(path, encoding="utf-8").read()
    add = ""
    for code, vals in codes.items():
        assert f'name="{code}_TITLE"' not in text, code
        add += f'  <data name="{code}_TITLE" xml:space="preserve"><value>{esc(vals[ti])}</value></data>\n'
        add += f'  <data name="{code}_DETAIL" xml:space="preserve"><value>{esc(vals[di])}</value></data>\n'
    assert text.rstrip().endswith("</root>")
    text = text.rstrip()[:-len("</root>")] + add + "</root>\n"
    open(path, "w", encoding="utf-8").write(text)
print("ok")
PY
git diff --stat backend/src/TanErp.Api/Resources
```

Expected: `ok` และ `git diff --stat` แสดงเฉพาะการเพิ่มบรรทัดในสองไฟล์ resx (22 บรรทัดต่อไฟล์).

- [ ] **Step 3: contracts**

`backend/src/TanErp.Api/Contracts/Attachments/AttachmentContracts.cs`:

```csharp
namespace TanErp.Api.Contracts.Attachments;

public sealed record AttachmentPersonResponse(Guid Id, string DisplayName);

/// <summary>Body of POST /api/v1/attachment-owners/{ownerType}/{ownerId}/attachments.</summary>
public sealed record AttachFilesRequest(string? Purpose, IReadOnlyList<Guid>? FileIds);

public sealed record AttachmentLinkResponse(
    Guid Id,
    string OwnerType,
    Guid OwnerId,
    Guid FileId,
    string Purpose,
    string Filename,
    string MediaType,
    long FileSizeBytes,
    string ServingUrl,
    AttachmentPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc);

public sealed record AttachmentListResponse(IReadOnlyList<AttachmentLinkResponse> Items);

/// <summary>Body of POST /api/v1/attachment-owners/{ownerType}/{ownerId}/signatures. The image is a PNG already uploaded through the Files module.</summary>
public sealed record CaptureSignatureRequest(
    string? Purpose,
    string? SignerName,
    string? SignerRole,
    Guid ImageFileId,
    bool ConsentAccepted,
    string? ConsentTextVersion);

public sealed record SignatureCaptureResponse(
    Guid Id,
    string OwnerType,
    Guid OwnerId,
    string Purpose,
    string SignerName,
    string? SignerRole,
    DateTimeOffset SignedAtUtc,
    Guid ImageFileId,
    string ServingUrl,
    string ConsentTextVersion,
    string ContentHash,
    AttachmentPersonResponse CapturedBy);

public sealed record SignatureCaptureListResponse(IReadOnlyList<SignatureCaptureResponse> Items);
```

- [ ] **Step 4: controller**

`backend/src/TanErp.Api/Controllers/AttachmentsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Attachments;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Attachments;

namespace TanErp.Api.Controllers;

/// <summary>Shared attachments and signatures for any registered owner type. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/attachment-owners/{ownerType}/{ownerId:guid}")]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly AttachmentHandler _handler;

    public AttachmentsController(AttachmentHandler handler)
    {
        _handler = handler;
    }

    private AttachmentCaller? ReadCaller(out string idempotencyKey, bool requireIdempotency, out IActionResult? failure)
    {
        idempotencyKey = string.Empty;
        if (requireIdempotency)
        {
            var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
            if (idempotent.IsFailure)
            {
                failure = ProblemDetailsMapper.CreateProblemResult(idempotent.Error.Code, HttpContext);
                return null;
            }

            failure = null;
            idempotencyKey = idempotent.Value!.IdempotencyKey;
            return new AttachmentCaller(idempotent.Value.FirebaseUid, idempotent.Value.MembershipId, HttpContext.TraceIdentifier);
        }

        var authenticated = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authenticated.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(authenticated.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        return new AttachmentCaller(authenticated.Value!.FirebaseUid, authenticated.Value.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    private static AttachmentPersonResponse To(AttachmentPerson person) => new(person.Id, person.DisplayName);

    private static AttachmentLinkResponse To(AttachmentLinkProjection p) => new(
        p.Id, p.OwnerType, p.OwnerId, p.FileId, p.Purpose, p.Filename, p.MediaType, p.FileSizeBytes, p.ServingUrl, To(p.CreatedBy), p.CreatedAtUtc);

    private static SignatureCaptureResponse To(SignatureCaptureProjection p) => new(
        p.Id, p.OwnerType, p.OwnerId, p.Purpose, p.SignerName, p.SignerRole, p.SignedAtUtc, p.ImageFileId, p.ServingUrl,
        p.ConsentTextVersion, p.ContentHash, To(p.CapturedBy));

    [HttpGet("attachments")]
    [ProducesResponseType<AttachmentListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ListAttachments([FromRoute] string ownerType, [FromRoute] Guid ownerId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, ownerType, ownerId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new AttachmentListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpPost("attachments")]
    [ProducesResponseType<AttachmentListResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Attach([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromBody] AttachFilesRequest request, CancellationToken ct)
    {
        var caller = ReadCaller(out var key, true, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.AttachAsync(caller, ownerType, ownerId, key, new AttachFilesInput(request.Purpose, request.FileIds), ct);
        return result.IsFailure
            ? Problem(result.Error.Code)
            : StatusCode(StatusCodes.Status201Created, new AttachmentListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpDelete("attachments/{linkId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Unlink([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromRoute] Guid linkId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.UnlinkAsync(caller, ownerType, ownerId, linkId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : NoContent();
    }

    [HttpGet("signatures")]
    [ProducesResponseType<SignatureCaptureListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ListSignatures([FromRoute] string ownerType, [FromRoute] Guid ownerId, CancellationToken ct)
    {
        var caller = ReadCaller(out _, false, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListSignaturesAsync(caller, ownerType, ownerId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new SignatureCaptureListResponse(result.Value!.Select(To).ToList()));
    }

    [HttpPost("signatures")]
    [ProducesResponseType<SignatureCaptureResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CaptureSignature([FromRoute] string ownerType, [FromRoute] Guid ownerId, [FromBody] CaptureSignatureRequest request, CancellationToken ct)
    {
        var caller = ReadCaller(out var key, true, out var failure);
        if (caller is null) return failure!;
        var input = new SignatureCaptureInput(request.Purpose, request.SignerName, request.SignerRole, request.ImageFileId, request.ConsentAccepted, request.ConsentTextVersion);
        var result = await _handler.CaptureSignatureAsync(caller, ownerType, ownerId, key, input, ct);
        return result.IsFailure ? Problem(result.Error.Code) : StatusCode(StatusCodes.Status201Created, To(result.Value!));
    }
}
```

หมายเหตุ: ตรวจชื่อ property ของ `IdempotentRequest`/`AuthenticatedRequest` ใน `RequestContextReader` (`FirebaseUid`, `MembershipId`, `IdempotencyKey` — ตาม `ServiceControllerBase`) ก่อนคอมไพล์.

- [ ] **Step 5: DI**

ใน `Program.cs` ใต้บรรทัด `builder.Services.AddScoped<TanErp.Application.QuickEstimates.QuickEstimateHandler>();` เพิ่ม:

```csharp
builder.Services.AddScoped<TanErp.Application.Attachments.IAttachmentOwnerScopeReader, TanErp.Infrastructure.Persistence.Attachments.AttachmentOwnerScopeReader>();
builder.Services.AddScoped<TanErp.Application.Attachments.IAttachmentStore, TanErp.Infrastructure.Persistence.Attachments.AttachmentStore>();
builder.Services.AddScoped<TanErp.Application.Attachments.AttachmentHandler>();
```

- [ ] **Step 6: build + regenerate OpenAPI snapshot**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q`
Expected: Build succeeded, 0 Error(s).

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests" -m:1`
Expected: PASS (1 test) และ `contracts/openapi/tan-erp.v1.json` เปลี่ยน.

Run: `grep -c "AttachmentLinkResponse\|SignatureCaptureResponse\|/api/v1/attachment-owners/{ownerType}/{ownerId}/attachments" contracts/openapi/tan-erp.v1.json`
Expected: มากกว่า 0 (schema และ path ใหม่อยู่ใน snapshot).
Run: `git diff --stat contracts/openapi/tan-erp.v1.json`
Expected: เพิ่มเฉพาะส่วน attachment/signature (ถ้ามี diff ส่วนอื่นให้หยุดตรวจว่ามาจากงานอื่นหรือไม่).

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests" -m:1`
Expected: PASS (ไม่ใช้ `UPDATE_OPENAPI`; snapshot ตรงกับ runtime).

- [ ] **Step 7: commit**

```bash
git add backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs backend/src/TanErp.Api/Resources/Errors.resx backend/src/TanErp.Api/Resources/Errors.en.resx backend/src/TanErp.Api/Contracts/Attachments backend/src/TanErp.Api/Controllers/AttachmentsController.cs backend/src/TanErp.Api/Program.cs contracts/openapi/tan-erp.v1.json
git commit -F - <<'EOF'
feat(attachments): add shared attachment and signature endpoints

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 8: Integration tests บน PostgreSQL

**Files:**
- Create: `backend/tests/TanErp.IntegrationTests/Api/AttachmentEndpointsTests.cs`

ต้องมี Docker. ใช้รูปแบบ fixture เดียวกับ `ServiceEndpointsTests` (Testcontainers `postgres:17-alpine`, `TestOnlyDataSeeder`, token verifier จำลอง). ผู้ใช้: `token-org-a` (Test Admin ที่มี `installations.*` ครบ), `token-org-b` (องค์กร B), `token-no-perm` (ไม่มี role).

- [ ] **Step 1: เขียน test ทั้งชุด** (endpoint จาก Task 7 มีอยู่แล้ว จึงคาดว่าผ่านเลย; ความเชื่อมั่นว่า test จับ regression ได้จริงมาจาก mutation check ใน Step 3)

`backend/tests/TanErp.IntegrationTests/Api/AttachmentEndpointsTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Attachments;
using TanErp.Api.Contracts.Files;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.Contracts.Service;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class AttachmentEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid UserId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidNoPerm = "uid-attachments-no-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6d01");
    private const string Owner = "installation-job";
    private const string ConsentVersion = "handover-2026-10-v1";

    private static readonly DateOnly Start = new(2026, 11, 1);
    private static readonly DateOnly End = new(2027, 1, 31);

    private static readonly byte[] JpegBytes =
    [
        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00
    ];

    private static readonly byte[] PngBytes =
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
    ];

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-perm" => UidNoPerm,
                _ => null
            });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["Storage:BasePath"] = Path.Combine(Path.GetTempPath(), $"tan-erp-attach-{Guid.NewGuid():N}"),
                    ["SeedTestData"] = "true"
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);

        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noperm-attach@example.com", "No Perm", true);
        db.Users.Add(noPermUser);
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgId, BranchId, noPermUser.Id, isActive: true));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // ----- HTTP helpers ----------------------------------------------------------------------------

    private static HttpRequestMessage Request(HttpMethod method, string url, string token = "token-org-a", string? key = null, Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = token switch
        {
            "token-org-b" => TestOnlyDataSeeder.TestMembershipBId,
            "token-no-perm" => MembershipNoPermId,
            _ => TestOnlyDataSeeder.TestMembershipId
        };
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null, string token = "token-org-a", string? key = null, Guid? ifMatch = null)
    {
        var request = Request(method, url, token, key, ifMatch);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static string Key() => Guid.NewGuid().ToString("N");

    private static async Task<string> ErrorCodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static string AttachmentsUrl(Guid ownerId, string ownerType = Owner) => $"/api/v1/attachment-owners/{ownerType}/{ownerId}/attachments";
    private static string SignaturesUrl(Guid ownerId, string ownerType = Owner) => $"/api/v1/attachment-owners/{ownerType}/{ownerId}/signatures";

    private async Task<AttachmentListResponse> ListAsync(Guid ownerId, string token = "token-org-a") =>
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Get, AttachmentsUrl(ownerId), token: token));

    /// <summary>Uploads one file through the Files module bound to a parent, as the browser does on submit.</summary>
    private async Task<Guid> UploadAsync(string parentType, Guid parentId, string filename, string mediaType, byte[] content, string token = "token-org-a")
    {
        var session = await Ok<CreateUploadSessionResponse>(await SendAsync(
            HttpMethod.Post, "/api/v1/files/upload-sessions",
            new CreateUploadSessionRequest(parentType, parentId, null, new List<FileSlotRequest> { new(filename, mediaType, content.Length) }),
            token: token, key: Key()), HttpStatusCode.Created);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        form.Add(fileContent, session.Slots[0].SlotId.ToString(), filename);

        var complete = Request(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", token);
        complete.Content = form;
        var completed = await Ok<CompleteUploadSessionResponse>(await _client.SendAsync(complete));
        return completed.Files[0].FileId;
    }

    private Task<Guid> UploadJpegAsync(Guid ownerId, string filename = "evidence.jpg", string token = "token-org-a") =>
        UploadAsync(Owner, ownerId, filename, "image/jpeg", JpegBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray(), token);

    private Task<Guid> UploadPngAsync(Guid ownerId, byte[]? content = null) =>
        UploadAsync(Owner, ownerId, "signature.png", "image/png", content ?? PngBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray());

    // ----- scenario seeding (same flow as ServiceEndpointsTests) -----------------------------------

    private sealed record Seeded(Guid QuotationId, Guid QuotationVersion, Guid OpportunityId);

    private async Task<Seeded> SeedAcceptedQuotationAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgId, UserId, CustomerType.Person, "คุณลูกค้า โครงการ TEST_ONLY", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var site = Site.CreateActive(
            Guid.NewGuid(), OrgId, customer.Id, UserId, "บ้านพักอาศัย",
            new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH"), 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, site.Id, UserId, UserId,
            "งานบิลท์อินห้องนอน", "ขอบเขตงาน", new[] { "built-in" }, null, 350000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "นัดเข้าวัดพื้นที่", now);
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, site.Id);
        opp.EnterEstimating(opp.RowVersion);
        opp.EnterProposed(opp.RowVersion);
        opp.MarkWon(opp.RowVersion);
        db.Opportunities.Add(opp);

        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, $"EST-T-{Guid.NewGuid():N}"[..14],
            siteSurveyRevisionId: null, siteSurveySnapshotHash: null);
        db.Estimates.Add(estimate);

        var quotation = new Quotation(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, estimate.Id, estimate.CurrentRevision!.Id,
            $"QT-T-{Guid.NewGuid():N}"[..14], 125000.50m, "snapshot-hash-abc", now);
        quotation.Accept(now);
        db.Quotations.Add(quotation);

        await db.SaveChangesAsync();
        return new Seeded(quotation.Id, quotation.RowVersion, opp.Id);
    }

    private async Task<Guid> CreateActiveProjectAsync()
    {
        var seeded = await SeedAcceptedQuotationAsync();
        var created = await Ok<ProjectResponse>(await SendAsync(HttpMethod.Post, "/api/v1/projects",
            new CreateProjectFromHandoverRequest(seeded.QuotationId, seeded.QuotationVersion, UserId, null, null), key: Key()), HttpStatusCode.Created);

        var control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{created.Id}/control"));
        control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{created.Id}/plan", new SetProjectPlanRequest(Start, End), ifMatch: control.RowVersion));
        control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{created.Id}/budget",
            new ReplaceProjectBudgetRequest(new List<ProjectBudgetLineRequest> { new("material", "หมวด 1", 100000m) }), ifMatch: control.RowVersion));
        await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{created.Id}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion));
        return created.Id;
    }

    private async Task<InstallationResponse> CreateInstallationAsync(Guid projectId) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, "/api/v1/installations",
            new InstallationRequest(projectId, Start, Start.AddDays(5), "ทีมติดตั้ง A", "ติดตั้งห้องนอน",
                new List<ChecklistItemRequest> { new("ตรวจวัดพื้นที่", true), new("ทำความสะอาดหน้างาน", true), new("ถ่ายรูปงาน", false) }), key: Key()), HttpStatusCode.Created);

    private async Task<InstallationResponse> StepAsync(InstallationResponse job, string path, object? body = null) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/installations/{job.Id}/{path}", body, ifMatch: job.RowVersion));

    private async Task<InstallationResponse> ReadyJobAsync()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        job = await StepAsync(job, "start");
        foreach (var item in job.Checklist.Where(c => c.Required))
        {
            job = await Ok<InstallationResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/installations/{job.Id}/checklist/{item.Id}", new ChecklistDoneRequest(true), ifMatch: job.RowVersion));
        }

        return await StepAsync(job, "ready");
    }

    // ----- tests -----------------------------------------------------------------------------------

    [Fact]
    public async Task Attach_List_ServeContent_AndUnlink_RoundTrip()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id, "ห้องนอน.jpg");

        var attached = await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id),
            new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created);

        var link = Assert.Single(attached.Items);
        Assert.Equal((Owner, job.Id, fileId, "evidence", "ห้องนอน.jpg"), (link.OwnerType, link.OwnerId, link.FileId, link.Purpose, link.Filename));
        Assert.Equal($"/api/v1/files/{fileId}/content", link.ServingUrl);
        Assert.False(string.IsNullOrWhiteSpace(link.CreatedBy.DisplayName));
        Assert.Single((await ListAsync(job.Id)).Items);

        var content = await SendAsync(HttpMethod.Get, $"/api/v1/files/{fileId}/content");
        Assert.Equal(HttpStatusCode.OK, content.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}")).StatusCode);
        Assert.Empty((await ListAsync(job.Id)).Items);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}")).StatusCode);

        // A removed link can be attached again (the unique index only covers active links).
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id),
            new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created);
    }

    [Fact]
    public async Task Attach_ReplayWithSameKeyDoesNotDuplicate_AndADifferentPayloadIsRejected()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var first = await UploadJpegAsync(job.Id);
        var second = await UploadJpegAsync(job.Id);
        var key = Key();

        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { first }), key: key), HttpStatusCode.Created);
        var replay = await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { first }), key: key), HttpStatusCode.Created);

        Assert.Single(replay.Items);
        Assert.Single((await ListAsync(job.Id)).Items);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { second }), key: key)));
        Assert.Equal(HttpStatusCode.BadRequest, (await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { second }))).StatusCode);
    }

    [Fact]
    public async Task Attach_FailedBatch_LinksNothing_SoUploadedFilesStayOrphanedAndUnlinked()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var linked = await UploadJpegAsync(job.Id);
        var orphan = await UploadJpegAsync(job.Id);
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { linked }), key: Key()), HttpStatusCode.Created);

        // The second file is already linked for the same purpose: the whole batch fails and the new file is not linked.
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan, linked }), key: Key())));
        // An unknown file id fails before any link is created.
        Assert.Equal("ATTACHMENT_FILE_NOT_READY", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan, Guid.NewGuid() }), key: Key())));

        var items = (await ListAsync(job.Id)).Items;
        Assert.Equal(new[] { linked }, items.Select(i => i.FileId).ToArray());

        // The orphan can still be linked on a later, successful submit.
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan }), key: Key()), HttpStatusCode.Created);
        Assert.Equal(2, (await ListAsync(job.Id)).Items.Count);
    }

    [Fact]
    public async Task Attach_RejectsFilesUploadedForAnotherParent_AndInvalidInput()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var seeded = await SeedAcceptedQuotationAsync();
        var foreignParentFile = await UploadAsync("opportunity", seeded.OpportunityId, "other.jpg", "image/jpeg", JpegBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray());

        Assert.Equal("ATTACHMENT_FILE_NOT_READY", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { foreignParentFile }), key: Key())));
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("misc", new[] { Guid.NewGuid() }), key: Key())));
        Assert.Equal("ATTACHMENT_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", Array.Empty<Guid>()), key: Key())));
        Assert.Empty((await ListAsync(job.Id)).Items);
    }

    [Fact]
    public async Task OtherOrganization_GetsNotFound_ForEveryOperation_AndCannotReadTheFile()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);
        var link = (await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created)).Items.Single();

        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id), token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), token: "token-org-b", key: Key())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}", token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id), token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, $"/api/v1/files/{fileId}/content", token: "token-org-b")).StatusCode);
    }

    [Fact]
    public async Task UnregisteredOwnerType_IsRejected_AndOwnerTypeMatchingIsCaseInsensitive()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());

        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id, "customer"))));
        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id, "installation_job"), new AttachFilesRequest("evidence", new[] { Guid.NewGuid() }), key: Key())));
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id, "INSTALLATION-JOB"))).StatusCode);
    }

    [Fact]
    public async Task MissingPermission_IsForbidden_ForReadAndWrite()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());

        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id), token: "token-no-perm")));
        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { Guid.NewGuid() }), token: "token-no-perm", key: Key())));
        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id), token: "token-no-perm")));
    }

    [Fact]
    public async Task CancelledInstallation_IsLockedForAttachments()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/installations/{job.Id}/cancel", new { reason = "ยกเลิกเพื่อทดสอบ" }, ifMatch: job.RowVersion));

        Assert.Equal("ATTACHMENT_OWNER_LOCKED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key())));
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id))).StatusCode);
    }

    [Fact]
    public async Task Signature_Capture_StoresHashOfTheImageBytes_AndRejectsInvalidSubmissions()
    {
        var job = await ReadyJobAsync();
        var pngBytes = PngBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray();
        var imageId = await UploadPngAsync(job.Id, pngBytes);
        var expectedHash = Convert.ToHexString(SHA256.HashData(pngBytes)).ToLowerInvariant();
        var request = new CaptureSignatureRequest("handover", "คุณสมชาย ใจดี", "เจ้าของบ้าน", imageId, true, ConsentVersion);

        var before = DateTimeOffset.UtcNow.AddMinutes(-1);
        var capture = await Ok<SignatureCaptureResponse>(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request, key: Key()), HttpStatusCode.Created);

        Assert.Equal(("handover", "คุณสมชาย ใจดี", "เจ้าของบ้าน", ConsentVersion, expectedHash, imageId), (capture.Purpose, capture.SignerName, capture.SignerRole, capture.ConsentTextVersion, capture.ContentHash, capture.ImageFileId));
        Assert.InRange(capture.SignedAtUtc, before, DateTimeOffset.UtcNow.AddMinutes(1));
        Assert.Single((await Ok<SignatureCaptureListResponse>(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id)))).Items);

        // The same image cannot be used twice, consent must match, and only verified PNG files qualify.
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request, key: Key())));
        var second = await UploadPngAsync(job.Id);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, ConsentAccepted = false }, key: Key())));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, ConsentTextVersion = "old-v0" }, key: Key())));
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, SignerName = "ก" }, key: Key())));
        var jpeg = await UploadJpegAsync(job.Id);
        Assert.Equal("SIGNATURE_IMAGE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = jpeg }, key: Key())));
        Assert.Single((await Ok<SignatureCaptureListResponse>(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id)))).Items);
    }

    [Fact]
    public async Task Signature_IsLockedUntilReadyForHandover_AndStaysOutOfTheAuditTrail()
    {
        var inProgress = await StepAsync(await CreateInstallationAsync(await CreateActiveProjectAsync()), "start");
        var early = await UploadPngAsync(inProgress.Id);
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(inProgress.Id),
            new CaptureSignatureRequest("handover", "คุณสมชาย ใจดี", null, early, true, ConsentVersion), key: Key())));

        var ready = await ReadyJobAsync();
        var imageId = await UploadPngAsync(ready.Id);
        await Ok<SignatureCaptureResponse>(await SendAsync(HttpMethod.Post, SignaturesUrl(ready.Id),
            new CaptureSignatureRequest("handover", "ผู้ลงนามเฉพาะกิจ ทดสอบ", "ผู้รับมอบ", imageId, true, ConsentVersion), key: Key()), HttpStatusCode.Created);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audits = await db.AuditEvents.AsNoTracking().Where(a => a.Action == "signature.captured" && a.ResourceId == ready.Id.ToString()).ToListAsync();
        var audit = Assert.Single(audits);
        Assert.DoesNotContain("ผู้ลงนามเฉพาะกิจ", audit.Changes);
        Assert.DoesNotContain("ผู้รับมอบ", audit.Changes);
        Assert.Contains("contentHash", audit.Changes);
    }
}
```

หมายเหตุสำหรับผู้ implement: (1) ชื่อ property ของ `AuditEvent` (`Action`, `ResourceId`, `Changes`) ต้องตรวจกับ `backend/src/TanErp.Domain/Common/AuditEvent.cs` ก่อน — ถ้าชื่อต่างให้ปรับ query ให้ตรง; (2) ไฟล์ของ Organization B ผูกกับ owner ของ Organization A ไม่ได้อยู่แล้วเพราะ owner ไม่ปรากฏต่อ Organization B (404 ใน test `OtherOrganization_...`) และ `ValidateVerifiedFilesForParentAsync` กรอง org; (3) ใช้ record `CreateUploadSessionRequest`/`FileSlotRequest`/`CompleteUploadSessionResponse` ตามที่มีใน `TanErp.Api.Contracts.Files` และ `ChecklistItemRequest`/`ChecklistDoneRequest`/`InstallationRequest` ใน `TanErp.Api.Contracts.Service`.

- [ ] **Step 2: รัน (คาดว่าผ่านหลัง Task 1–7; ถ้ามีข้อใดล้มให้แก้ implementation ไม่ใช่ test)**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~AttachmentEndpointsTests" -m:1`
Expected: PASS 9 tests (Docker ต้องรัน; ครั้งแรกอาจดึง image `postgres:17-alpine`).

- [ ] **Step 3: พิสูจน์ว่า test จับ regression ได้จริง (mutation check)**

1. ชั่วคราวใน `AttachmentStore.AttachAsync` ลบบรรทัด `_db.ChangeTracker.Clear();` ในบล็อก `catch (AttachmentDomainException ex)` แล้วรัน `--filter "FullyQualifiedName~Attach_FailedBatch_LinksNothing"` — ถ้า test ยังผ่านเพราะไม่มีการ `SaveChanges` ต่อ ให้ปล่อยไว้ (แปลว่า transaction rollback ปกป้องอยู่แล้ว) แล้วคืนบรรทัดกลับ.
2. ชั่วคราวใน `AttachmentHandler.ResolveOwnerAsync` เปลี่ยนเงื่อนไข `scope.OrganizationId != context.OrganizationId` ให้ไม่ตรวจ และให้ `AttachmentOwnerScopeReader` ไม่กรอง `OrganizationId` แล้วรัน `OtherOrganization_GetsNotFound...` — ต้อง **ล้ม** (ยืนยันว่า test ตรวจ cross-org จริง); คืนโค้ดกลับแล้วรันซ้ำให้ผ่าน. ห้าม commit การเปลี่ยนชั่วคราว (`git diff` ต้องว่างในไฟล์ src).

- [ ] **Step 4: regression ของ Files + Service เดิม**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~FileUploadSessionTests|FullyQualifiedName~ServiceEndpointsTests|FullyQualifiedName~ItemImageEndpointsTests" -m:1`
Expected: PASS (ยืนยันว่าการแก้ `FileParentAccessResolver`/`FileGatePermissions` ไม่ทำให้ parent type เดิมเสีย).

- [ ] **Step 5: commit**

```bash
git add backend/tests/TanErp.IntegrationTests/Api/AttachmentEndpointsTests.cs
git commit -F - <<'EOF'
test(attachments): cover shared attachment and signature endpoints end-to-end

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 9: Frontend — generated types, API client, owner-type lib, query hooks

**Files:**
- Regenerate: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`
- Create: `frontend/src/lib/attachments/attachment-owner-types.ts`
- Create: `frontend/src/lib/attachments/attachment-owner-types.test.ts`
- Create: `frontend/src/hooks/useAttachments.ts`
- Modify: `frontend/src/hooks/useDeferredFileUpload.ts`
- Test: `frontend/src/lib/attachments/attachment-owner-types.test.ts`, `frontend/src/hooks/useDeferredFileUpload.test.ts` (regression)

- [ ] **Step 1: generate types จาก OpenAPI**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npm run generate:api`
Expected: `src/generated/api/tan-erp.v1.ts` เปลี่ยนและมี `AttachmentLinkResponse`, `SignatureCaptureResponse`, `AttachFilesRequest`, `CaptureSignatureRequest`.
Run: `grep -c "AttachmentLinkResponse\|SignatureCaptureResponse" src/generated/api/tan-erp.v1.ts` → Expected: มากกว่า 0.
(เมื่อ commit แล้ว `npm run check:api` ต้องผ่าน — ใช้เป็นเกตใน Step 7.)

- [ ] **Step 2: เขียน test ที่ล้มก่อน**

`frontend/src/lib/attachments/attachment-owner-types.test.ts`:

```ts
import { describe, expect, it } from "vitest";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import {
  ATTACHMENT_ERROR_CODES,
  ATTACHMENT_OWNER_TYPES,
  ATTACHMENT_PURPOSES,
  SIGNATURE_CONSENT_VERSIONS,
  SIGNATURE_PURPOSES,
  attachmentErrorCode,
  isAttachmentPurpose,
} from "./attachment-owner-types";

describe("attachment owner types", () => {
  it("mirrors the backend whitelist for the registered owner types", () => {
    expect(ATTACHMENT_OWNER_TYPES).toEqual(["installation-job"]);
  });

  it("knows the backend purposes and rejects anything else", () => {
    expect(ATTACHMENT_PURPOSES).toEqual(["general", "evidence", "handover", "defect"]);
    expect(isAttachmentPurpose("evidence")).toBe(true);
    expect(isAttachmentPurpose("misc")).toBe(false);
  });

  it("narrows only known error codes", () => {
    expect(attachmentErrorCode("ATTACHMENT_DUPLICATE")).toBe("ATTACHMENT_DUPLICATE");
    expect(attachmentErrorCode("SOMETHING_ELSE")).toBeNull();
    expect(attachmentErrorCode(undefined)).toBeNull();
  });

  it("has a consent version for every signature purpose", () => {
    for (const purpose of SIGNATURE_PURPOSES) {
      expect(SIGNATURE_CONSENT_VERSIONS[purpose]).toMatch(/^[a-z]+-\d{4}-\d{2}-v\d+$/);
    }
  });

  it.each([
    ["th", thMessages],
    ["en", enMessages],
  ])("has %s labels for every purpose, error code and consent version", (_locale, messages) => {
    const ns = messages.attachments;
    for (const purpose of ATTACHMENT_PURPOSES) expect(ns.purposes[purpose]).toBeTruthy();
    for (const code of ATTACHMENT_ERROR_CODES) expect(ns.errors[code]).toBeTruthy();
    for (const purpose of SIGNATURE_PURPOSES) expect(ns.signature.consentVersions[SIGNATURE_CONSENT_VERSIONS[purpose]]).toBeTruthy();
  });
});
```

- [ ] **Step 3: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/lib/attachments/attachment-owner-types.test.ts`
Expected: FAIL (`Failed to resolve import "./attachment-owner-types"`).

- [ ] **Step 4: เขียน lib + i18n (th/en พร้อมกัน)**

`frontend/src/lib/attachments/attachment-owner-types.ts`:

```ts
/**
 * Owner types and enums of the shared attachment API. These mirror the backend whitelist
 * (AttachmentOwnerTypes / AttachmentPurposes); register a new owner type in the backend first.
 */
export const ATTACHMENT_OWNER_TYPES = ["installation-job"] as const;
export type AttachmentOwnerType = (typeof ATTACHMENT_OWNER_TYPES)[number];

export const ATTACHMENT_PURPOSES = ["general", "evidence", "handover", "defect"] as const;
export type AttachmentPurpose = (typeof ATTACHMENT_PURPOSES)[number];

export function isAttachmentPurpose(value: string): value is AttachmentPurpose {
  return ATTACHMENT_PURPOSES.some((purpose) => purpose === value);
}

export const SIGNATURE_PURPOSES = ["handover"] as const;
export type SignaturePurpose = (typeof SIGNATURE_PURPOSES)[number];

/** Consent wording version the server currently accepts per signature purpose (the wording itself is in messages). */
export const SIGNATURE_CONSENT_VERSIONS = {
  handover: "handover-2026-10-v1",
} as const satisfies Record<SignaturePurpose, string>;

/** Files module limits (TEST_ONLY): JPEG/PNG/WebP up to 10 MB each. */
export const ATTACHMENT_ACCEPTED_MEDIA_TYPES = ["image/jpeg", "image/png", "image/webp"] as const;
export const ATTACHMENT_MAX_FILE_BYTES = 10 * 1024 * 1024;

export const ATTACHMENT_ERROR_CODES = [
  "ATTACHMENT_OWNER_TYPE_INVALID",
  "ATTACHMENT_PURPOSE_INVALID",
  "ATTACHMENT_FIELD_INVALID",
  "ATTACHMENT_FILE_NOT_READY",
  "ATTACHMENT_FILE_SCOPE_MISMATCH",
  "ATTACHMENT_DUPLICATE",
  "ATTACHMENT_LIMIT_EXCEEDED",
  "ATTACHMENT_OWNER_LOCKED",
  "SIGNATURE_SUBMISSION_INVALID",
  "SIGNATURE_CONSENT_REQUIRED",
  "SIGNATURE_IMAGE_INVALID",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

export type AttachmentErrorCode = (typeof ATTACHMENT_ERROR_CODES)[number];

export function attachmentErrorCode(code: string | null | undefined): AttachmentErrorCode | null {
  return ATTACHMENT_ERROR_CODES.find((known) => known === code) ?? null;
}
```

เพิ่ม namespace `attachments` ลง `th.json` และ `en.json` ด้วยสคริปต์ (round-trip ของไฟล์ทั้งสองคงเดิมอยู่แล้ว):

```bash
cd /Users/syaco/Documents/development/tan-erp/frontend
node - <<'NODE'
const fs = require("fs");
const add = (file, ns) => {
  const path = `src/messages/${file}.json`;
  const json = JSON.parse(fs.readFileSync(path, "utf8"));
  if (json.attachments) throw new Error(`attachments already exists in ${file}`);
  json.attachments = ns;
  fs.writeFileSync(path, JSON.stringify(json, null, 2) + "\n");
};

add("th", {
  title: "ไฟล์แนบ",
  subtitle: "รูปภาพหลักฐานของรายการนี้ (JPEG, PNG, WebP ไม่เกิน 10 MB ต่อไฟล์)",
  loading: "กำลังโหลดไฟล์แนบ",
  loadError: "โหลดไฟล์แนบไม่สำเร็จ",
  empty: "ยังไม่มีไฟล์แนบ",
  emptyHint: "เลือกรูปภาพแล้วกดบันทึกไฟล์แนบ",
  selectFiles: "เลือกรูปภาพ",
  purposeLabel: "ประเภทไฟล์แนบ",
  pendingTitle: "รอบันทึก ({count} ไฟล์)",
  uploadedWaiting: "อัปโหลดแล้ว รอผูกกับรายการ ({count} ไฟล์)",
  removePending: "นำออก",
  save: "บันทึกไฟล์แนบ",
  saved: "บันทึกไฟล์แนบแล้ว",
  unlinked: "ลบไฟล์แนบแล้ว",
  fileRejected: "ไฟล์ {name} ไม่ใช่ JPEG/PNG/WebP หรือมีขนาดเกิน 10 MB",
  createdBy: "โดย {name}",
  purposes: { general: "ทั่วไป", evidence: "หลักฐาน", handover: "ส่งมอบ", defect: "ข้อบกพร่อง" },
  unlink: {
    action: "ลบไฟล์แนบ",
    title: "ลบไฟล์แนบ",
    message: "ต้องการถอดไฟล์นี้ออกจากรายการหรือไม่ ไฟล์จะไม่แสดงในรายการนี้อีก",
  },
  errors: {
    failed: "ดำเนินการไม่สำเร็จ โปรดลองอีกครั้ง",
    ATTACHMENT_OWNER_TYPE_INVALID: "ประเภทรายการที่แนบไฟล์ไม่ถูกต้อง",
    ATTACHMENT_PURPOSE_INVALID: "ประเภทไฟล์แนบไม่ถูกต้อง",
    ATTACHMENT_FIELD_INVALID: "เลือกไฟล์ได้ 1-20 ไฟล์ต่อครั้งและไม่ซ้ำกัน",
    ATTACHMENT_FILE_NOT_READY: "ไฟล์ยังไม่ผ่านการตรวจสอบหรือไม่ได้อัปโหลดสำหรับรายการนี้",
    ATTACHMENT_FILE_SCOPE_MISMATCH: "ไฟล์ไม่ได้อยู่ในองค์กรเดียวกับรายการนี้",
    ATTACHMENT_DUPLICATE: "ไฟล์นี้ถูกแนบกับรายการนี้ด้วยประเภทเดียวกันแล้ว",
    ATTACHMENT_LIMIT_EXCEEDED: "แนบไฟล์ได้ไม่เกิน 50 ไฟล์ต่อรายการ",
    ATTACHMENT_OWNER_LOCKED: "สถานะของรายการนี้ไม่อนุญาตให้แก้ไฟล์แนบหรือลายเซ็น",
    SIGNATURE_SUBMISSION_INVALID: "ชื่อผู้ลงนามต้องมี 2-200 ตัวอักษรและต้องมีภาพลายเซ็น",
    SIGNATURE_CONSENT_REQUIRED: "ต้องยอมรับข้อความยินยอมฉบับปัจจุบัน",
    SIGNATURE_IMAGE_INVALID: "ภาพลายเซ็นต้องเป็นไฟล์ PNG ที่ผ่านการตรวจสอบแล้ว",
    IDEMPOTENCY_KEY_REUSED: "คำขอนี้ถูกใช้กับข้อมูลชุดอื่นแล้ว โปรดลองบันทึกใหม่",
    PERMISSION_DENIED: "ไม่มีสิทธิ์ทำรายการนี้",
    RESOURCE_NOT_FOUND: "ไม่พบรายการ",
  },
  signature: {
    title: "ลายเซ็นผู้รับมอบ",
    subtitle: "บันทึกภาพลายเซ็นของผู้รับมอบงาน (ภาพ PNG พร้อมค่า hash ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย)",
    loadError: "โหลดลายเซ็นไม่สำเร็จ",
    empty: "ยังไม่มีลายเซ็น",
    signerName: "ชื่อผู้ลงนาม",
    signerRole: "ตำแหน่งหรือความสัมพันธ์กับลูกค้า",
    consentLabel: "ข้าพเจ้ายอมรับข้อความต่อไปนี้",
    signaturePadLabel: "ลายเซ็น",
    capture: "บันทึกลายเซ็น",
    saved: "บันทึกลายเซ็นแล้ว",
    signedBy: "{name} เมื่อ {date}",
    hashLabel: "SHA-256: {hash}",
    consentVersions: {
      "handover-2026-10-v1": "ข้าพเจ้าได้ตรวจรับงานตามรายการและยืนยันว่าลายเซ็นนี้เป็นของข้าพเจ้าหรือของผู้ที่ข้าพเจ้าได้รับมอบอำนาจ โดยยอมรับว่าระบบเก็บภาพลายเซ็นและเวลาที่ลงนามเป็นหลักฐาน",
    },
  },
});

add("en", {
  title: "Attachments",
  subtitle: "Evidence photos for this record (JPEG, PNG, WebP up to 10 MB each)",
  loading: "Loading attachments",
  loadError: "Could not load attachments",
  empty: "No attachments yet",
  emptyHint: "Choose images, then save the attachments",
  selectFiles: "Choose images",
  purposeLabel: "Attachment type",
  pendingTitle: "Waiting to save ({count} files)",
  uploadedWaiting: "Uploaded, waiting to be linked ({count} files)",
  removePending: "Remove",
  save: "Save attachments",
  saved: "Attachments saved",
  unlinked: "Attachment removed",
  fileRejected: "File {name} is not JPEG/PNG/WebP or is larger than 10 MB",
  createdBy: "By {name}",
  purposes: { general: "General", evidence: "Evidence", handover: "Handover", defect: "Defect" },
  unlink: {
    action: "Remove attachment",
    title: "Remove attachment",
    message: "Remove this file from the record? It will no longer be listed here.",
  },
  errors: {
    failed: "The action failed. Please try again.",
    ATTACHMENT_OWNER_TYPE_INVALID: "This record type cannot have attachments",
    ATTACHMENT_PURPOSE_INVALID: "The attachment type is invalid",
    ATTACHMENT_FIELD_INVALID: "Choose between 1 and 20 distinct files per request",
    ATTACHMENT_FILE_NOT_READY: "A file is not verified or was not uploaded for this record",
    ATTACHMENT_FILE_SCOPE_MISMATCH: "The file does not belong to this record's organization",
    ATTACHMENT_DUPLICATE: "This file is already attached to this record for the same type",
    ATTACHMENT_LIMIT_EXCEEDED: "A record can have at most 50 attachments",
    ATTACHMENT_OWNER_LOCKED: "The record's status does not allow attachment or signature changes",
    SIGNATURE_SUBMISSION_INVALID: "The signer name needs 2-200 characters and a signature is required",
    SIGNATURE_CONSENT_REQUIRED: "The current consent statement must be accepted",
    SIGNATURE_IMAGE_INVALID: "The signature image must be a verified PNG file",
    IDEMPOTENCY_KEY_REUSED: "This request was already used with different data. Please save again.",
    PERMISSION_DENIED: "You do not have permission to do this",
    RESOURCE_NOT_FOUND: "Record not found",
  },
  signature: {
    title: "Handover signature",
    subtitle: "Record the signature of the person accepting the work (a PNG image with a hash, not a legal electronic signature)",
    loadError: "Could not load signatures",
    empty: "No signature yet",
    signerName: "Signer name",
    signerRole: "Role or relationship to the customer",
    consentLabel: "I accept the following statement",
    signaturePadLabel: "Signature",
    capture: "Save signature",
    saved: "Signature saved",
    signedBy: "{name} on {date}",
    hashLabel: "SHA-256: {hash}",
    consentVersions: {
      "handover-2026-10-v1": "I have inspected the work against the list and confirm that this signature is mine or that of a person who authorised me, and I accept that the system keeps the signature image and signing time as evidence.",
    },
  },
});
NODE
git diff --stat src/messages
```

Expected: `git diff --stat` แสดงเฉพาะบรรทัดที่เพิ่ม (insertions เท่านั้น ไม่มี deletions) ในทั้งสองไฟล์.

- [ ] **Step 5: api-client + FileParentType + hooks**

ใน `frontend/src/lib/api/api-client.ts` ใต้ `export type WarrantyResponse = ...` เพิ่ม type alias:

```ts
export type AttachmentLinkResponse = components["schemas"]["AttachmentLinkResponse"];
export type AttachmentListResponse = components["schemas"]["AttachmentListResponse"];
export type AttachFilesRequest = components["schemas"]["AttachFilesRequest"];
export type SignatureCaptureResponse = components["schemas"]["SignatureCaptureResponse"];
export type SignatureCaptureListResponse = components["schemas"]["SignatureCaptureListResponse"];
export type CaptureSignatureRequest = components["schemas"]["CaptureSignatureRequest"];
```

และในคลาส `ApiClient` ใต้เมธอด `installationStep(...)` (ก่อนปิดเมธอดถัดไป) เพิ่ม:

```ts
  private attachmentOwnerPath(ownerType: string, ownerId: string): string {
    return `/api/v1/attachment-owners/${encodeURIComponent(ownerType)}/${encodeURIComponent(ownerId)}`;
  }

  async listAttachments(ownerType: string, ownerId: string, options: RequestOptions): Promise<AttachmentListResponse> {
    return this.request<AttachmentListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments`, "GET", options);
  }

  async attachFiles(ownerType: string, ownerId: string, payload: AttachFilesRequest, options: RequestOptions): Promise<AttachmentListResponse> {
    return this.request<AttachmentListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments`, "POST", options, payload);
  }

  async unlinkAttachment(ownerType: string, ownerId: string, linkId: string, options: RequestOptions): Promise<void> {
    return this.request<void>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments/${encodeURIComponent(linkId)}`, "DELETE", options);
  }

  async listSignatures(ownerType: string, ownerId: string, options: RequestOptions): Promise<SignatureCaptureListResponse> {
    return this.request<SignatureCaptureListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/signatures`, "GET", options);
  }

  async captureSignature(ownerType: string, ownerId: string, payload: CaptureSignatureRequest, options: RequestOptions): Promise<SignatureCaptureResponse> {
    return this.request<SignatureCaptureResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/signatures`, "POST", options, payload);
  }
```

ใน `frontend/src/hooks/useDeferredFileUpload.ts` เปลี่ยน `FileParentType` ให้รวม owner types:

```ts
import type { AttachmentOwnerType } from "@/lib/attachments/attachment-owner-types";

export type FileParentType =
  | "customer"
  | "site"
  | "opportunity"
  | "item"
  | "item-category"
  | "item-brand"
  | "costRecord"
  | AttachmentOwnerType;
```

(ย้าย `import type` ไปรวมกับ import อื่นด้านบนไฟล์.)

`frontend/src/hooks/useAttachments.ts`:

```ts
import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type AttachFilesRequest,
  type AttachmentListResponse,
  type CaptureSignatureRequest,
  type SignatureCaptureListResponse,
  type SignatureCaptureResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import type { AttachmentOwnerType } from "@/lib/attachments/attachment-owner-types";

type UiLocale = "th" | "en";

export function attachmentsKey(membershipId: string | undefined, locale: UiLocale, ownerType: AttachmentOwnerType, ownerId: string) {
  return ["business", membershipId, locale, "attachments", ownerType, ownerId] as const;
}

export function useAttachmentLinks(ownerType: AttachmentOwnerType, ownerId: string): UseQueryResult<AttachmentListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: [...attachmentsKey(membershipId, locale, ownerType, ownerId), "links"],
    enabled: Boolean(membershipId && ownerId),
    queryFn: async ({ signal }) => apiClient.listAttachments(ownerType, ownerId, await buildOptions({ signal })),
  });
}

/** Attach (idempotent) and unlink. Both refresh the owner's attachment list. */
export function useAttachmentMutations(ownerType: AttachmentOwnerType, ownerId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: attachmentsKey(membershipId, locale, ownerType, ownerId) });
  };

  const attach = useMutation({
    mutationFn: async (input: { payload: AttachFilesRequest; idempotencyKey: string }) =>
      apiClient.attachFiles(ownerType, ownerId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: refresh,
  });

  const unlink = useMutation({
    mutationFn: async (linkId: string) => apiClient.unlinkAttachment(ownerType, ownerId, linkId, await buildOptions()),
    onSuccess: refresh,
  });

  return { attach, unlink };
}

export function useSignatureCaptures(ownerType: AttachmentOwnerType, ownerId: string): UseQueryResult<SignatureCaptureListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: [...attachmentsKey(membershipId, locale, ownerType, ownerId), "signatures"],
    enabled: Boolean(membershipId && ownerId),
    queryFn: async ({ signal }) => apiClient.listSignatures(ownerType, ownerId, await buildOptions({ signal })),
  });
}

export function useCaptureSignature(ownerType: AttachmentOwnerType, ownerId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<SignatureCaptureResponse, Error, { payload: CaptureSignatureRequest; idempotencyKey: string }>({
    mutationFn: async (input) => apiClient.captureSignature(ownerType, ownerId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: attachmentsKey(membershipId, locale, ownerType, ownerId) });
    },
  });
}
```

- [ ] **Step 6: รันให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/lib/attachments/attachment-owner-types.test.ts src/hooks/useDeferredFileUpload.test.ts`
Expected: PASS ทั้งสองไฟล์.
Run: `npm run typecheck && npm run lint`
Expected: ไม่มี error.

- [ ] **Step 7: check:api แล้ว commit**

Run: `git add frontend/src/generated/api/tan-erp.v1.ts && npm run check:api`
Expected: ผ่าน (generate ใหม่แล้วไม่มี diff ต่างจากที่ stage).

```bash
cd /Users/syaco/Documents/development/tan-erp
git add frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api/api-client.ts frontend/src/lib/attachments frontend/src/hooks/useAttachments.ts frontend/src/hooks/useDeferredFileUpload.ts frontend/src/messages/th.json frontend/src/messages/en.json
git commit -F - <<'EOF'
feat(attachments): add frontend API client, owner type lib, hooks and i18n

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 10: Frontend — `dataUrlToPngFile` และ `<AttachmentList />`

**Files:**
- Create: `frontend/src/lib/media/data-url-to-file.ts`, `frontend/src/lib/media/data-url-to-file.test.ts`
- Create: `frontend/src/components/forms/AttachmentList.tsx`, `frontend/src/components/forms/AttachmentList.test.tsx`
- Modify: `frontend/src/components/forms/index.ts`

กติกา: อัปโหลดจริงเฉพาะตอนกด "บันทึกไฟล์แนบ" ผ่าน `useDeferredFileUpload` เท่านั้น (ไม่ยิง `fileClient` ตรง); ปุ่มบันทึกและปุ่มยืนยันลบมี `isLoading`; ลบต้องผ่าน `ConfirmationModal`; ไม่มี `any`/`as any`/`@ts-ignore`/chained fallback; ข้อความทั้งหมดผ่าน i18n; Tailwind + semantic tokens, ไม่มี border-radius.

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`frontend/src/lib/media/data-url-to-file.test.ts`:

```ts
import { describe, expect, it } from "vitest";
import { dataUrlToPngFile } from "./data-url-to-file";

// 8-byte PNG signature, base64 encoded.
const PNG_DATA_URL = "data:image/png;base64,iVBORw0KGgo=";

describe("dataUrlToPngFile", () => {
  it("builds a PNG File with the decoded bytes", async () => {
    const file = dataUrlToPngFile(PNG_DATA_URL, "signature.png");

    expect(file.name).toBe("signature.png");
    expect(file.type).toBe("image/png");
    expect(file.size).toBe(8);
    const bytes = new Uint8Array(await file.arrayBuffer());
    expect(Array.from(bytes)).toEqual([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]);
  });

  it.each(["", "data:image/jpeg;base64,/9j/4AAQ", "iVBORw0KGgo=", "data:image/png;base64,@@@"])("rejects %s", (value) => {
    expect(() => dataUrlToPngFile(value, "signature.png")).toThrow();
  });
});
```

`frontend/src/components/forms/AttachmentList.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ApiError } from "@/lib/api/api-error";
import type { AttachmentLinkResponse } from "@/lib/api/api-client";
import { AttachmentList } from "./AttachmentList";

const mocks = vi.hoisted(() => ({
  uploadFiles: vi.fn(),
  attach: vi.fn(),
  unlink: vi.fn(),
  links: { data: undefined as { items: AttachmentLinkResponse[] } | undefined, isLoading: false, isError: false },
  toast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("@/hooks/useDeferredFileUpload", () => ({
  useDeferredFileUpload: () => ({ uploadFiles: mocks.uploadFiles, isUploading: false }),
}));
vi.mock("@/hooks/useAttachments", () => ({
  useAttachmentLinks: () => mocks.links,
  useAttachmentMutations: () => ({
    attach: { mutateAsync: mocks.attach, isPending: false },
    unlink: { mutateAsync: mocks.unlink, isPending: false },
  }),
}));
vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: "m-1",
    locale: "th",
    buildOptions: async () => ({ token: "tok", membershipId: "m-1", locale: "th" }),
  }),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: mocks.toast }) }));
vi.mock("@/components/common/AuthenticatedFileImage", () => ({
  AuthenticatedFileImage: ({ fileId, alt }: { fileId: string; alt: string }) => <img src={`blob:${fileId}`} alt={alt} />,
}));

const t = thMessages.attachments;

function link(id: string, filename: string): AttachmentLinkResponse {
  return {
    id,
    ownerType: "installation-job",
    ownerId: "job-1",
    fileId: `file-${id}`,
    purpose: "evidence",
    filename,
    mediaType: "image/jpeg",
    fileSizeBytes: 1024,
    servingUrl: `/api/v1/files/file-${id}/content`,
    createdBy: { id: "u-1", displayName: "ช่างหนึ่ง" },
    createdAtUtc: "2026-10-05T03:00:00Z",
  };
}

function renderList(props: Partial<React.ComponentProps<typeof AttachmentList>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <AttachmentList ownerType="installation-job" ownerId="job-1" canManage purposes={["evidence", "defect"]} {...props} />
    </NextIntlClientProvider>
  );
}

function pick(files: File[]) {
  fireEvent.change(screen.getByLabelText(t.selectFiles), { target: { files } });
}

const jpeg = (name: string) => new File(["x"], name, { type: "image/jpeg" });

describe("AttachmentList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.links.data = { items: [] };
    mocks.links.isLoading = false;
    mocks.links.isError = false;
    mocks.uploadFiles.mockResolvedValue({ uploadIntentId: null, files: [{ fileId: "file-new", filename: "a.jpg", mediaType: "image/jpeg", fileSizeBytes: 1, servingUrl: "" }] });
    mocks.attach.mockResolvedValue({ items: [] });
    mocks.unlink.mockResolvedValue(undefined);
  });

  it("lists attachments with filename, purpose label and author", () => {
    mocks.links.data = { items: [link("l-1", "ห้องนอน.jpg")] };
    renderList();

    expect(screen.getByText("ห้องนอน.jpg")).toBeInTheDocument();
    expect(screen.getByText(t.purposes.evidence)).toBeInTheDocument();
    expect(screen.getByText(t.createdBy.replace("{name}", "ช่างหนึ่ง"))).toBeInTheDocument();
  });

  it("shows the empty state and hides every write control without manage permission", () => {
    renderList({ canManage: false });

    expect(screen.getByText(t.empty)).toBeInTheDocument();
    expect(screen.queryByLabelText(t.selectFiles)).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.save })).not.toBeInTheDocument();
  });

  it("does not upload when files are only selected", () => {
    renderList();
    pick([jpeg("a.jpg")]);

    expect(screen.getByText("a.jpg")).toBeInTheDocument();
    expect(mocks.uploadFiles).not.toHaveBeenCalled();
    expect(mocks.attach).not.toHaveBeenCalled();
  });

  it("uploads on submit and then attaches the uploaded file ids with the chosen purpose", async () => {
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(1));
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);
    expect(mocks.uploadFiles.mock.calls[0][0]).toHaveLength(1);
    expect(mocks.uploadFiles.mock.invocationCallOrder[0]).toBeLessThan(mocks.attach.mock.invocationCallOrder[0]);
    const call = mocks.attach.mock.calls[0][0] as { payload: { purpose: string; fileIds: string[] }; idempotencyKey: string };
    expect(call.payload).toEqual({ purpose: "evidence", fileIds: ["file-new"] });
    expect(call.idempotencyKey).not.toBe("");
    expect(mocks.toast.success).toHaveBeenCalledWith(t.saved);
  });

  it("keeps the uploaded file for a retry when attaching fails, without uploading it again", async () => {
    mocks.attach.mockRejectedValueOnce(new ApiError({ status: 409, code: "ATTACHMENT_DUPLICATE", message: "dup" }));
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    expect(await screen.findByText(t.errors.ATTACHMENT_DUPLICATE)).toBeInTheDocument();
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);

    fireEvent.click(screen.getByRole("button", { name: t.save }));
    await waitFor(() => expect(mocks.attach).toHaveBeenCalledTimes(2));
    expect(mocks.uploadFiles).toHaveBeenCalledTimes(1);
  });

  it("shows an unknown failure with the generic message and keeps the pending files when the upload itself fails", async () => {
    mocks.uploadFiles.mockRejectedValueOnce(new Error("network"));
    renderList();
    pick([jpeg("a.jpg")]);
    fireEvent.click(screen.getByRole("button", { name: t.save }));

    expect(await screen.findByText(t.errors.failed)).toBeInTheDocument();
    expect(mocks.attach).not.toHaveBeenCalled();
    expect(screen.getByText("a.jpg")).toBeInTheDocument();
  });

  it("rejects files that are not accepted images before anything is sent", () => {
    renderList();
    pick([new File(["x"], "notes.pdf", { type: "application/pdf" })]);

    expect(screen.getByText(t.fileRejected.replace("{name}", "notes.pdf"))).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.save })).not.toBeInTheDocument();
  });

  it("asks for confirmation before unlinking and only unlinks after confirming", async () => {
    mocks.links.data = { items: [link("l-1", "ห้องนอน.jpg")] };
    renderList();

    fireEvent.click(screen.getByRole("button", { name: t.unlink.action }));
    expect(screen.getByText(t.unlink.message)).toBeInTheDocument();
    expect(mocks.unlink).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.delete }));
    await waitFor(() => expect(mocks.unlink).toHaveBeenCalledWith("l-1"));
    expect(mocks.toast.success).toHaveBeenCalledWith(t.unlinked);
  });
});
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/lib/media/data-url-to-file.test.ts src/components/forms/AttachmentList.test.tsx`
Expected: FAIL (`Failed to resolve import "./data-url-to-file"` / `"./AttachmentList"`).

- [ ] **Step 3: เขียน implementation**

`frontend/src/lib/media/data-url-to-file.ts`:

```ts
const PNG_DATA_URL_PREFIX = "data:image/png;base64,";

/** Converts a PNG data URL (as produced by SignaturePad) into a File so it can go through the deferred upload flow. */
export function dataUrlToPngFile(dataUrl: string, filename: string): File {
  if (!dataUrl.startsWith(PNG_DATA_URL_PREFIX)) {
    throw new Error("Expected a PNG data URL.");
  }

  const binary = atob(dataUrl.slice(PNG_DATA_URL_PREFIX.length));
  const bytes = new Uint8Array(binary.length);
  for (let index = 0; index < binary.length; index += 1) {
    bytes[index] = binary.charCodeAt(index);
  }

  return new File([bytes], filename, { type: "image/png" });
}
```

`frontend/src/components/forms/AttachmentList.tsx`:

```tsx
"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { IconTrash, IconUpload } from "@/components/common/Icons";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { useAttachmentLinks, useAttachmentMutations } from "@/hooks/useAttachments";
import { useDeferredFileUpload, type UploadedFileResult } from "@/hooks/useDeferredFileUpload";
import { useToast } from "@/hooks/useToast";
import type { AttachmentLinkResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import {
  ATTACHMENT_ACCEPTED_MEDIA_TYPES,
  ATTACHMENT_MAX_FILE_BYTES,
  attachmentErrorCode,
  isAttachmentPurpose,
  type AttachmentOwnerType,
  type AttachmentPurpose,
} from "@/lib/attachments/attachment-owner-types";
import { formatDateTime } from "@/lib/formatters/formatters";

export interface AttachmentListProps {
  ownerType: AttachmentOwnerType;
  ownerId: string;
  /** Whether the viewer may add or remove attachments (the server still enforces permission and owner state). */
  canManage: boolean;
  /** Purposes the user can choose from; the first one is the default. */
  purposes?: readonly AttachmentPurpose[];
}

const DEFAULT_PURPOSES: readonly AttachmentPurpose[] = ["general"];

function isAcceptedImage(file: File): boolean {
  return ATTACHMENT_ACCEPTED_MEDIA_TYPES.some((type) => type === file.type) && file.size > 0 && file.size <= ATTACHMENT_MAX_FILE_BYTES;
}

/**
 * Shared attachment list for any registered owner type. Files are uploaded only when the user saves
 * (deferred upload); a failed link keeps the already-uploaded files so a retry does not upload them again.
 */
export function AttachmentList({ ownerType, ownerId, canManage, purposes = DEFAULT_PURPOSES }: AttachmentListProps) {
  const t = useTranslations("attachments");
  const tCommon = useTranslations("common.actions");
  const locale = useLocale();
  const { toast } = useToast();
  const { buildOptions } = useApiRequestContext();
  const links = useAttachmentLinks(ownerType, ownerId);
  const { attach, unlink } = useAttachmentMutations(ownerType, ownerId);
  const uploader = useDeferredFileUpload({ parentType: ownerType, parentId: ownerId });

  const inputRef = useRef<HTMLInputElement | null>(null);
  const attachKeyRef = useRef<string | null>(null);
  const [pending, setPending] = useState<File[]>([]);
  const [uploaded, setUploaded] = useState<UploadedFileResult[]>([]);
  const [purpose, setPurpose] = useState<AttachmentPurpose>(purposes[0] ?? "general");
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [toRemove, setToRemove] = useState<AttachmentLinkResponse | null>(null);

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? attachmentErrorCode(err.code) : null;
    return code ? t(`errors.${code}`) : t("errors.failed");
  }

  function handleSelect(event: React.ChangeEvent<HTMLInputElement>): void {
    const selected = Array.from(event.target.files ?? []);
    event.target.value = "";
    const rejected = selected.find((file) => !isAcceptedImage(file));
    setError(rejected ? t("fileRejected", { name: rejected.name }) : null);
    const accepted = selected.filter(isAcceptedImage);
    if (accepted.length > 0) setPending((current) => [...current, ...accepted]);
  }

  async function handleSave(): Promise<void> {
    if (isSaving || (pending.length === 0 && uploaded.length === 0)) return;
    setIsSaving(true);
    setError(null);
    try {
      let ready = uploaded;
      if (pending.length > 0) {
        const options = await buildOptions();
        const result = await uploader.uploadFiles(pending, { token: options.token, membershipId: options.membershipId, locale: options.locale });
        ready = [...uploaded, ...result.files];
        setUploaded(ready);
        setPending([]);
        attachKeyRef.current = null; // a new file list needs a new idempotency key
      }

      attachKeyRef.current ??= `attach-${crypto.randomUUID()}`;
      await attach.mutateAsync({ payload: { purpose, fileIds: ready.map((file) => file.fileId) }, idempotencyKey: attachKeyRef.current });
      setUploaded([]);
      attachKeyRef.current = null;
      toast.success(t("saved"));
    } catch (err: unknown) {
      setError(describe(err));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleUnlink(): Promise<void> {
    if (!toRemove?.id) return;
    try {
      await unlink.mutateAsync(toRemove.id);
      toast.success(t("unlinked"));
    } catch (err: unknown) {
      toast.error(describe(err));
    } finally {
      setToRemove(null);
    }
  }

  const items = links.data?.items ?? [];
  const waitingCount = pending.length + uploaded.length;

  return (
    <section className="erp-card flex flex-col gap-4 p-5" aria-labelledby={`attachments-${ownerId}`}>
      <div className="flex flex-col gap-1 border-b border-erp-border pb-3">
        <div className="flex items-center gap-2">
          <h3 id={`attachments-${ownerId}`} className="text-base font-bold text-erp-navy">{t("title")}</h3>
          <span className="border border-erp-border bg-erp-surface-muted px-1.5 py-0.5 font-mono text-[11px] font-semibold text-erp-navy">{items.length}</span>
        </div>
        <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
      </div>

      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      {links.isLoading ? (
        <div className="flex justify-center py-8" role="status" aria-label={t("loading")}><MonoSpinner size="md" /></div>
      ) : links.isError ? (
        <Alert variant="danger">{t("loadError")}</Alert>
      ) : items.length === 0 ? (
        <p className="py-4 text-sm text-erp-text-muted">{t("empty")}</p>
      ) : (
        <ul className="divide-y divide-erp-border border border-erp-border">
          {items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 p-3">
              <AuthenticatedFileImage fileId={item.fileId ?? ""} alt={item.filename ?? ""} className="h-16 w-16 border border-erp-border object-cover" />
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-semibold text-erp-navy">{item.filename}</p>
                <p className="text-xs text-erp-text-muted">
                  {item.purpose && isAttachmentPurpose(item.purpose) ? t(`purposes.${item.purpose}`) : "-"}
                </p>
                <p className="text-xs text-erp-text-muted">
                  {t("createdBy", { name: item.createdBy?.displayName ?? "-" })} · {item.createdAtUtc ? formatDateTime(item.createdAtUtc, locale) : "-"}
                </p>
              </div>
              {canManage && (
                <Button type="button" size="sm" variant="outline" className="min-h-11 min-w-11" aria-label={t("unlink.action")} onClick={() => setToRemove(item)}>
                  <IconTrash size={16} />
                </Button>
              )}
            </li>
          ))}
        </ul>
      )}

      {canManage && (
        <div className="flex flex-col gap-3 border-t border-erp-border pt-4">
          {purposes.length > 1 && (
            <Select
              label={t("purposeLabel")}
              value={purpose}
              disabled={isSaving}
              options={purposes.map((value) => ({ value, label: t(`purposes.${value}`) }))}
              onChange={(event) => { if (isAttachmentPurpose(event.target.value)) setPurpose(event.target.value); }}
            />
          )}

          <input
            ref={inputRef}
            type="file"
            multiple
            accept={ATTACHMENT_ACCEPTED_MEDIA_TYPES.join(",")}
            aria-label={t("selectFiles")}
            className="sr-only"
            onChange={handleSelect}
          />
          <div className="flex flex-wrap items-center gap-3">
            <Button type="button" variant="outline" className="min-h-11" icon={<IconUpload size={16} />} disabled={isSaving} onClick={() => inputRef.current?.click()}>
              {t("selectFiles")}
            </Button>
            {waitingCount > 0 && (
              <Button type="button" variant="primary" className="min-h-11" isLoading={isSaving} disabled={isSaving} onClick={() => void handleSave()}>
                {t("save")}
              </Button>
            )}
          </div>

          {pending.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-erp-text-muted">{t("pendingTitle", { count: pending.length })}</p>
              <ul className="mt-1 divide-y divide-erp-border border border-erp-border">
                {pending.map((file, index) => (
                  <li key={`${file.name}-${index}`} className="flex items-center justify-between gap-3 px-3 py-2 text-sm">
                    <span className="truncate">{file.name}</span>
                    <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isSaving} onClick={() => setPending((current) => current.filter((_, i) => i !== index))}>
                      {t("removePending")}
                    </Button>
                  </li>
                ))}
              </ul>
            </div>
          )}
          {uploaded.length > 0 && <p className="text-xs text-erp-text-muted">{t("uploadedWaiting", { count: uploaded.length })}</p>}
        </div>
      )}

      <ConfirmationModal
        isOpen={toRemove !== null}
        onClose={() => { if (!unlink.isPending) setToRemove(null); }}
        onConfirm={handleUnlink}
        title={t("unlink.title")}
        message={t("unlink.message")}
        confirmText={tCommon("delete")}
        cancelText={tCommon("cancel")}
        variant="danger"
        isLoading={unlink.isPending}
      />
    </section>
  );
}
```

หมายเหตุสำหรับผู้ implement: (1) ตรวจ prop ของ `Select`/`Alert`/`MonoSpinner`/`formatDateTime` ให้ตรงกับที่ `installation-detail.tsx` ใช้ (ใช้ชุดเดียวกัน); (2) ถ้า ESLint ห้าม `React.ChangeEvent` โดยไม่ import React ให้ `import type { ChangeEvent } from "react"`; (3) ฟิลด์ที่ generated type ทำเป็น optional (`item.fileId`, `item.filename`) ใช้ `?? ""`/`?? "-"` แบบค่าเดียวเท่านั้น (ไม่ chain) — ห้ามเพิ่ม `a || b || c`.

เพิ่มใน `frontend/src/components/forms/index.ts`: `export * from "./AttachmentList";`.

- [ ] **Step 4: รันให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/lib/media/data-url-to-file.test.ts src/components/forms/AttachmentList.test.tsx`
Expected: PASS (data-url 5 cases; AttachmentList 8 tests).

- [ ] **Step 5: lint + typecheck + commit**

Run: `npm run lint && npm run typecheck`
Expected: ไม่มี error/warning ใหม่ในไฟล์ที่เพิ่ม.

```bash
cd /Users/syaco/Documents/development/tan-erp
git add frontend/src/lib/media/data-url-to-file.ts frontend/src/lib/media/data-url-to-file.test.ts frontend/src/components/forms/AttachmentList.tsx frontend/src/components/forms/AttachmentList.test.tsx frontend/src/components/forms/index.ts
git commit -F - <<'EOF'
feat(attachments): add shared AttachmentList with deferred upload and confirmed unlink

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 11: Frontend — `<SignatureCapturePanel />` (ใช้ `SignaturePad` เดิม)

**Files:**
- Create: `frontend/src/components/forms/SignatureCapturePanel.tsx`, `frontend/src/components/forms/SignatureCapturePanel.test.tsx`
- Modify: `frontend/src/components/forms/index.ts`

`SignaturePad` (มีอยู่แล้ว) ให้ค่าเป็น PNG data URL ผ่าน `onChange`; panel นี้เพิ่มชื่อผู้ลงนาม ตำแหน่ง ข้อความยินยอม และส่งภาพผ่าน `useDeferredFileUpload` ตอนกดบันทึกเท่านั้น. ใน jsdom ไม่มี canvas 2D จึง mock `SignaturePad` ในเทสต์; **ต้องตรวจจริงในเบราว์เซอร์ใน Step 6**.

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`frontend/src/components/forms/SignatureCapturePanel.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { ApiError } from "@/lib/api/api-error";
import type { SignatureCaptureResponse } from "@/lib/api/api-client";
import { SignatureCapturePanel } from "./SignatureCapturePanel";

const PNG_DATA_URL = "data:image/png;base64,iVBORw0KGgo=";

const mocks = vi.hoisted(() => ({
  uploadSingleFile: vi.fn(),
  capture: vi.fn(),
  signatures: { data: undefined as { items: SignatureCaptureResponse[] } | undefined, isLoading: false, isError: false },
  toast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("@/hooks/useDeferredFileUpload", () => ({
  useDeferredFileUpload: () => ({ uploadSingleFile: mocks.uploadSingleFile, isUploading: false }),
}));
vi.mock("@/hooks/useAttachments", () => ({
  useSignatureCaptures: () => mocks.signatures,
  useCaptureSignature: () => ({ mutateAsync: mocks.capture, isPending: false }),
}));
vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: "m-1",
    locale: "th",
    buildOptions: async () => ({ token: "tok", membershipId: "m-1", locale: "th" }),
  }),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: mocks.toast }) }));
vi.mock("@/components/common/AuthenticatedFileImage", () => ({
  AuthenticatedFileImage: ({ fileId, alt }: { fileId: string; alt: string }) => <img src={`blob:${fileId}`} alt={alt} />,
}));
// jsdom has no 2D canvas, so the pad is replaced by a stub that reports a PNG data URL.
vi.mock("./SignaturePad", () => ({
  SignaturePad: ({ onChange }: { onChange: (value: string | null) => void }) => (
    <div>
      <button type="button" onClick={() => onChange(PNG_DATA_URL)}>draw</button>
      <button type="button" onClick={() => onChange(null)}>wipe</button>
    </div>
  ),
}));

const t = thMessages.attachments.signature;
const consent = t.consentVersions["handover-2026-10-v1"];

function renderPanel(props: Partial<React.ComponentProps<typeof SignatureCapturePanel>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <SignatureCapturePanel ownerType="installation-job" ownerId="job-1" purpose="handover" canCapture {...props} />
    </NextIntlClientProvider>
  );
}

function fillValid() {
  fireEvent.change(screen.getByLabelText(new RegExp(t.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
  fireEvent.change(screen.getByLabelText(new RegExp(t.signerRole)), { target: { value: "เจ้าของบ้าน" } });
  fireEvent.click(screen.getByRole("checkbox", { name: new RegExp(t.consentLabel) }));
  fireEvent.click(screen.getByRole("button", { name: "draw" }));
}

describe("SignatureCapturePanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.signatures.data = { items: [] };
    mocks.signatures.isLoading = false;
    mocks.signatures.isError = false;
    mocks.uploadSingleFile.mockResolvedValue({ uploadIntentId: null, fileId: "file-sig" });
    mocks.capture.mockResolvedValue({});
  });

  it("lists captured signatures with signer, hash and image", () => {
    mocks.signatures.data = {
      items: [{
        id: "s-1", ownerType: "installation-job", ownerId: "job-1", purpose: "handover", signerName: "คุณสมชาย ใจดี", signerRole: "เจ้าของบ้าน",
        signedAtUtc: "2026-10-05T03:00:00Z", imageFileId: "file-sig", servingUrl: "/x", consentTextVersion: "handover-2026-10-v1",
        contentHash: "a".repeat(64), capturedBy: { id: "u-1", displayName: "ผู้บันทึก" },
      }],
    };
    renderPanel({ canCapture: false });

    expect(screen.getByText(/คุณสมชาย ใจดี/)).toBeInTheDocument();
    expect(screen.getByText(t.hashLabel.replace("{hash}", "a".repeat(12)))).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: t.capture })).not.toBeInTheDocument();
  });

  it("shows the empty state when there is no signature", () => {
    renderPanel({ canCapture: false });
    expect(screen.getByText(t.empty)).toBeInTheDocument();
  });

  it("keeps the save button disabled until name, consent and a signature are all present", () => {
    renderPanel();
    const save = screen.getByRole("button", { name: t.capture });
    expect(save).toBeDisabled();

    fireEvent.change(screen.getByLabelText(new RegExp(t.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    expect(save).toBeDisabled();
    fireEvent.click(screen.getByRole("checkbox", { name: new RegExp(t.consentLabel) }));
    expect(save).toBeDisabled();
    fireEvent.click(screen.getByRole("button", { name: "draw" }));
    expect(save).toBeEnabled();
    fireEvent.click(screen.getByRole("button", { name: "wipe" }));
    expect(save).toBeDisabled();
  });

  it("shows the consent wording of the current version", () => {
    renderPanel();
    expect(screen.getByText(consent)).toBeInTheDocument();
  });

  it("uploads the PNG on submit, then captures with the consent version and the uploaded file id", async () => {
    renderPanel();
    expect(mocks.uploadSingleFile).not.toHaveBeenCalled();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    await waitFor(() => expect(mocks.capture).toHaveBeenCalledTimes(1));
    const uploaded = mocks.uploadSingleFile.mock.calls[0][0] as File;
    expect(uploaded.type).toBe("image/png");
    expect(mocks.uploadSingleFile.mock.invocationCallOrder[0]).toBeLessThan(mocks.capture.mock.invocationCallOrder[0]);
    const call = mocks.capture.mock.calls[0][0] as { payload: Record<string, unknown>; idempotencyKey: string };
    expect(call.payload).toEqual({
      purpose: "handover",
      signerName: "คุณสมชาย ใจดี",
      signerRole: "เจ้าของบ้าน",
      imageFileId: "file-sig",
      consentAccepted: true,
      consentTextVersion: "handover-2026-10-v1",
    });
    expect(mocks.toast.success).toHaveBeenCalledWith(t.saved);
  });

  it("does not capture when the upload fails", async () => {
    mocks.uploadSingleFile.mockRejectedValueOnce(new Error("network"));
    renderPanel();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    expect(await screen.findByText(thMessages.attachments.errors.failed)).toBeInTheDocument();
    expect(mocks.capture).not.toHaveBeenCalled();
  });

  it("reuses the uploaded image when the capture fails and the same signature is submitted again", async () => {
    mocks.capture.mockRejectedValueOnce(new ApiError({ status: 409, code: "ATTACHMENT_OWNER_LOCKED", message: "locked" }));
    renderPanel();
    fillValid();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));

    expect(await screen.findByText(thMessages.attachments.errors.ATTACHMENT_OWNER_LOCKED)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: t.capture }));
    await waitFor(() => expect(mocks.capture).toHaveBeenCalledTimes(2));
    expect(mocks.uploadSingleFile).toHaveBeenCalledTimes(1);
  });
});
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/components/forms/SignatureCapturePanel.test.tsx`
Expected: FAIL (`Failed to resolve import "./SignatureCapturePanel"`).

- [ ] **Step 3: เขียน implementation**

`frontend/src/components/forms/SignatureCapturePanel.tsx`:

```tsx
"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useCaptureSignature, useSignatureCaptures } from "@/hooks/useAttachments";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import {
  SIGNATURE_CONSENT_VERSIONS,
  attachmentErrorCode,
  type AttachmentOwnerType,
  type SignaturePurpose,
} from "@/lib/attachments/attachment-owner-types";
import { formatDateTime } from "@/lib/formatters/formatters";
import { dataUrlToPngFile } from "@/lib/media/data-url-to-file";
import { SignaturePad } from "./SignaturePad";

export interface SignatureCapturePanelProps {
  ownerType: AttachmentOwnerType;
  ownerId: string;
  purpose: SignaturePurpose;
  /** Whether the viewer may record a signature (the server still enforces permission and owner state). */
  canCapture: boolean;
}

const HASH_PREVIEW_LENGTH = 12;

/**
 * Captured signatures of an owner record plus a form to record a new one.
 * The signature PNG is uploaded only on submit; a failed capture keeps the uploaded image so a retry does not upload it again.
 * Evidence is an image plus SHA-256, not a legal electronic signature.
 */
export function SignatureCapturePanel({ ownerType, ownerId, purpose, canCapture }: SignatureCapturePanelProps) {
  const t = useTranslations("attachments.signature");
  const tErrors = useTranslations("attachments.errors");
  const locale = useLocale();
  const { toast } = useToast();
  const { buildOptions } = useApiRequestContext();
  const signatures = useSignatureCaptures(ownerType, ownerId);
  const capture = useCaptureSignature(ownerType, ownerId);
  const uploader = useDeferredFileUpload({ parentType: ownerType, parentId: ownerId });

  const consentVersion = SIGNATURE_CONSENT_VERSIONS[purpose];
  const keyRef = useRef<string | null>(null);
  const [signerName, setSignerName] = useState("");
  const [signerRole, setSignerRole] = useState("");
  const [consent, setConsent] = useState(false);
  const [signature, setSignature] = useState<string | null>(null);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSave = signerName.trim().length >= 2 && consent && signature !== null && !isSaving;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? attachmentErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function handleSignatureChange(value: string | null): void {
    setSignature(value);
    setUploadedFileId(null); // a different drawing is a different image
    keyRef.current = null;
  }

  async function handleSave(): Promise<void> {
    if (!canSave || signature === null) return;
    setIsSaving(true);
    setError(null);
    try {
      let imageFileId = uploadedFileId;
      if (imageFileId === null) {
        const options = await buildOptions();
        const uploaded = await uploader.uploadSingleFile(dataUrlToPngFile(signature, "signature.png"), {
          token: options.token,
          membershipId: options.membershipId,
          locale: options.locale,
        });
        if (!uploaded.fileId) {
          setError(tErrors("failed"));
          return;
        }

        imageFileId = uploaded.fileId;
        setUploadedFileId(imageFileId);
        keyRef.current = null;
      }

      keyRef.current ??= `signature-${crypto.randomUUID()}`;
      await capture.mutateAsync({
        payload: {
          purpose,
          signerName: signerName.trim(),
          signerRole: signerRole.trim() === "" ? null : signerRole.trim(),
          imageFileId,
          consentAccepted: consent,
          consentTextVersion: consentVersion,
        },
        idempotencyKey: keyRef.current,
      });
      toast.success(t("saved"));
      setSignerName("");
      setSignerRole("");
      setConsent(false);
      setSignature(null);
      setUploadedFileId(null);
      keyRef.current = null;
    } catch (err: unknown) {
      setError(describe(err));
    } finally {
      setIsSaving(false);
    }
  }

  const items = signatures.data?.items ?? [];

  return (
    <section className="erp-card flex flex-col gap-4 p-5" aria-labelledby={`signatures-${ownerId}`}>
      <div className="flex flex-col gap-1 border-b border-erp-border pb-3">
        <h3 id={`signatures-${ownerId}`} className="text-base font-bold text-erp-navy">{t("title")}</h3>
        <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
      </div>

      {signatures.isLoading ? (
        <div className="flex justify-center py-6" role="status"><MonoSpinner size="md" /></div>
      ) : signatures.isError ? (
        <Alert variant="danger">{t("loadError")}</Alert>
      ) : items.length === 0 ? (
        <p className="text-sm text-erp-text-muted">{t("empty")}</p>
      ) : (
        <ul className="divide-y divide-erp-border border border-erp-border">
          {items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 p-3">
              <AuthenticatedFileImage fileId={item.imageFileId ?? ""} alt={item.signerName ?? ""} className="h-16 w-28 border border-erp-border bg-white object-contain" />
              <div className="min-w-0 flex-1">
                <p className="text-sm font-semibold text-erp-navy">
                  {t("signedBy", { name: item.signerName ?? "-", date: item.signedAtUtc ? formatDateTime(item.signedAtUtc, locale) : "-" })}
                </p>
                {item.signerRole && <p className="text-xs text-erp-text-muted">{item.signerRole}</p>}
                <p className="font-mono text-[11px] text-erp-text-muted">{t("hashLabel", { hash: (item.contentHash ?? "").slice(0, HASH_PREVIEW_LENGTH) })}</p>
              </div>
            </li>
          ))}
        </ul>
      )}

      {canCapture && (
        <div className="flex flex-col gap-4 border-t border-erp-border pt-4">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Input label={t("signerName")} required value={signerName} maxLength={200} disabled={isSaving} onChange={(event) => setSignerName(event.target.value)} />
          <Input label={t("signerRole")} value={signerRole} maxLength={100} disabled={isSaving} onChange={(event) => setSignerRole(event.target.value)} />
          <SignaturePad value={signature} onChange={handleSignatureChange} label={t("signaturePadLabel")} />
          <label className="flex items-start gap-3 text-sm">
            <input
              type="checkbox"
              className="mt-1 h-5 w-5 border border-erp-border"
              checked={consent}
              disabled={isSaving}
              aria-label={t("consentLabel")}
              onChange={(event) => setConsent(event.target.checked)}
            />
            <span>
              <span className="block font-semibold text-erp-navy">{t("consentLabel")}</span>
              <span className="block text-erp-text-muted">{t(`consentVersions.${consentVersion}`)}</span>
            </span>
          </label>
          <div>
            <Button type="button" variant="primary" className="min-h-11" isLoading={isSaving} disabled={!canSave} onClick={() => void handleSave()}>
              {t("capture")}
            </Button>
          </div>
        </div>
      )}
    </section>
  );
}
```

หมายเหตุสำหรับผู้ implement: (1) ใน test, `getByLabelText(new RegExp(t.signerName))` ต้องพบ `Input` ที่มี label ตรงข้อความ (ตรวจว่า `Input` component ผูก label กับ input — ถ้าใช้ `required` แล้วต่อท้าย `*` regex ยังจับได้); ข้อความ `consentLabel` เป็นทั้ง `aria-label` และข้อความใน `span` — ถ้า `getByRole("checkbox", {name})` กำกวมให้เหลือเฉพาะ `aria-label`; (2) `SIGNATURE_CONSENT_VERSIONS` เป็น `as const` จึงได้ literal type `"handover-2026-10-v1"` ทำให้ `t(\`consentVersions.${consentVersion}\`)` ผ่าน type-safe messages โดยไม่ต้อง cast (ห้าม `as any`/`as never`); (3) ห้ามใช้ chained fallback — `(item.contentHash ?? "")` เป็นค่าเดียว ใช้ได้.

เพิ่มใน `frontend/src/components/forms/index.ts`: `export * from "./SignatureCapturePanel";`.

- [ ] **Step 4: รันให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/components/forms/SignatureCapturePanel.test.tsx src/components/forms/AttachmentList.test.tsx`
Expected: PASS (SignatureCapturePanel 7 tests).

- [ ] **Step 5: lint + typecheck**

Run: `npm run lint && npm run typecheck`
Expected: ไม่มี error.

- [ ] **Step 6: ตรวจ `SignaturePad` จริงในเบราว์เซอร์ (ไม่มี Playwright)**

`SignaturePad` ยังไม่เคยถูกใช้และไม่มี test (jsdom ไม่มี canvas). หลัง Task 12 เสร็จ ให้เปิดหน้าใบงานติดตั้งที่ `ready_for_handover` ด้วย `npm run dev` + backend, วาดลายเซ็น, กดบันทึก แล้วยืนยันว่า (ก) เส้นไม่หายหลังวาดเพราะ `value` เปลี่ยนแล้ว `setupCanvas` ถูกเรียกซ้ำ (ถ้าหาย ให้แก้ `SignaturePad` ให้ไม่รีเซ็ตเมื่อ `value` เปลี่ยนจากการวาดของตัวเอง — แก้ในไฟล์เดิมเท่านั้นและเพิ่มบันทึกลง verification doc), (ข) PNG ที่อัปโหลดผ่านการตรวจ (201). บันทึกผลลงใน Task 13.

- [ ] **Step 7: commit**

```bash
git add frontend/src/components/forms/SignatureCapturePanel.tsx frontend/src/components/forms/SignatureCapturePanel.test.tsx frontend/src/components/forms/index.ts
git commit -F - <<'EOF'
feat(attachments): add shared SignatureCapturePanel built on SignaturePad

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 12: ใช้งานใน InstallationDetail (consumer แรก)

**Files:**
- Modify: `frontend/src/features/service/components/installation-detail.tsx`
- Modify: `frontend/src/features/service/components/service-components.test.tsx`

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

ใน `service-components.test.tsx` ใต้ `vi.mock("@/lib/permissions/can", ...)` เพิ่ม mock ของ shared components:

```tsx
vi.mock("@/components/forms/AttachmentList", () => ({
  AttachmentList: (props: { ownerType: string; ownerId: string; canManage: boolean }) => (
    <div data-testid="attachment-list" data-owner-type={props.ownerType} data-owner-id={props.ownerId} data-can-manage={String(props.canManage)} />
  ),
}));
vi.mock("@/components/forms/SignatureCapturePanel", () => ({
  SignatureCapturePanel: (props: { ownerType: string; ownerId: string; purpose: string; canCapture: boolean }) => (
    <div data-testid="signature-panel" data-owner-type={props.ownerType} data-purpose={props.purpose} data-can-capture={String(props.canCapture)} />
  ),
}));
```

และเพิ่ม test สามตัวใน `describe("InstallationDetail", ...)` (ใช้ helper `renderJob(data, granted)` และ `job(status)` ที่มีอยู่แล้วในไฟล์; job id คือ `"job-1"`):

```tsx
  it("renders the shared attachment list and signature panel for the installation owner", () => {
    renderJob(job("ready_for_handover"), ["installations.operate", "installations.handover"]);

    const list = screen.getByTestId("attachment-list");
    expect(list.dataset.ownerType).toBe("installation-job");
    expect(list.dataset.ownerId).toBe("job-1");
    expect(list.dataset.canManage).toBe("true");

    const signature = screen.getByTestId("signature-panel");
    expect(signature.dataset.ownerType).toBe("installation-job");
    expect(signature.dataset.purpose).toBe("handover");
    expect(signature.dataset.canCapture).toBe("true");
  });

  it("limits attachment and signature controls by status and permission", () => {
    renderJob(job("handed_over"), ["installations.operate"]);
    expect(screen.getByTestId("attachment-list").dataset.canManage).toBe("false");
    expect(screen.getByTestId("signature-panel").dataset.canCapture).toBe("false");
  });

  it("does not allow signing without the handover permission", () => {
    renderJob(job("ready_for_handover"), ["installations.operate"]);
    expect(screen.getByTestId("attachment-list").dataset.canManage).toBe("true");
    expect(screen.getByTestId("signature-panel").dataset.canCapture).toBe("false");
  });
```

- [ ] **Step 2: รันให้ล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/features/service/components/service-components.test.tsx`
Expected: FAIL (`Unable to find an element by: [data-testid="attachment-list"]`).

- [ ] **Step 3: เชื่อม component**

ใน `installation-detail.tsx` เพิ่ม import:

```tsx
import { AttachmentList } from "@/components/forms/AttachmentList";
import { SignatureCapturePanel } from "@/components/forms/SignatureCapturePanel";
```

ใน `DetailView` ใต้ตัวแปร `canHandover` เพิ่ม:

```tsx
  const attachmentsEditable = canOperate && ["planned", "in_progress", "ready_for_handover"].includes(status);
  const signatureEditable = canHandover && status === "ready_for_handover";
```

และก่อนปิด `<Modal ...>` เดิม (หลังบล็อก checklist/defects ที่ปิดด้วย `</div>` ก่อน `<Modal`) เพิ่ม:

```tsx
      {job.id && (
        <>
          <AttachmentList ownerType="installation-job" ownerId={job.id} canManage={attachmentsEditable} purposes={["evidence", "defect", "general"]} />
          <SignatureCapturePanel ownerType="installation-job" ownerId={job.id} purpose="handover" canCapture={signatureEditable} />
        </>
      )}
```

(ไม่เพิ่มข้อความใน `service.*` — ข้อความทั้งหมดอยู่ใน namespace `attachments` ของ component กลาง.)

- [ ] **Step 4: รันให้ผ่าน + regression ของ feature service**

Run: `npx vitest run src/features/service src/components/forms src/lib/attachments src/lib/media src/hooks/useDeferredFileUpload.test.ts`
Expected: PASS ทั้งหมด (รวม test เดิมของ `features/service`).
Run: `npm run lint && npm run typecheck`
Expected: ไม่มี error.

- [ ] **Step 5: ตรวจในเบราว์เซอร์ (ทำ Task 11 Step 6 ที่ค้างอยู่)**

รัน backend (`dotnet run --project backend/src/TanErp.Api`) และ `npm run dev` ตาม runbook `docs/05-engineering/foundation-login-runbook.md`; เปิดใบงานติดตั้งสถานะ `in_progress` → เลือกรูป → ยืนยันว่า Network tab ไม่มีการยิง `/api/v1/files/upload-sessions` จนกดบันทึก; กดบันทึก → รูปขึ้นในรายการ; ลบ → ขึ้น modal ยืนยันก่อน; ใบงาน `ready_for_handover` → วาดลายเซ็น/ติ๊กยินยอม/บันทึก → รายการลายเซ็นมีชื่อ+hash. บันทึกผล (ผ่าน/ไม่ผ่านพร้อมข้อสังเกต) ไว้ใช้ใน Task 13. ถ้าไม่สามารถรัน stack ได้ ให้ระบุใน verification doc ว่า "ไม่ได้ตรวจในเบราว์เซอร์".

- [ ] **Step 6: commit**

```bash
cd /Users/syaco/Documents/development/tan-erp
git add frontend/src/features/service/components/installation-detail.tsx frontend/src/features/service/components/service-components.test.tsx
git commit -F - <<'EOF'
feat(service): use shared attachments and handover signature on installation detail

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 13: เอกสารตรวจรับ, อัปเดตแผนหลัก/สัญญา Service/Roadmap, gate สุดท้าย

**Files:**
- Create: `docs/05-engineering/shared-attachment-signature-verification.md`
- Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`
- Modify: `docs/03-contracts/service-api-contract.md`, `docs/05-engineering/service-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/03-contracts/attachment-api-contract.md` (สถานะ → Implemented)

- [ ] **Step 1: รัน gate เฉพาะ slice ทั้งชุดแล้วจดผลจริง**

```bash
cd /Users/syaco/Documents/development/tan-erp
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Attachment|FullyQualifiedName~SignatureEvidence|FullyQualifiedName~FileGatePermissions|FullyQualifiedName~CompleteUploadSession|FullyQualifiedName~ServiceDomain"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~AttachmentEndpointsTests|FullyQualifiedName~AttachmentOwnerScopeReaderParityTests|FullyQualifiedName~ExternalAcceptance|FullyQualifiedName~FileUploadSessionTests|FullyQualifiedName~ServiceEndpointsTests|FullyQualifiedName~OpenApiContractTests" -m:1
cd frontend && npm run check:api && npm run lint && npm run typecheck && npx vitest run src/components/forms src/features/service src/lib/attachments src/lib/media src/hooks/useDeferredFileUpload.test.ts
```

Expected: ทุกคำสั่งผ่าน. จดจำนวน test ที่ผ่านของแต่ละคำสั่ง (จากบรรทัดสรุปของ dotnet/vitest) เพื่อใส่เอกสารใน Step 2. ห้ามเขียนตัวเลขที่ไม่ได้เห็นจากผลรันจริง.

- [ ] **Step 2: เขียน verification doc**

`docs/05-engineering/shared-attachment-signature-verification.md` (ภาษาไทย) มีหัวข้อ: (1) สถานะ + ลิงก์ [Attachment API Contract](../03-contracts/attachment-api-contract.md) และ [ADR 0017](../adr/0017-shared-attachment-owner-registry.md); (2) **ผลที่รันจริง** — ตารางคำสั่งจาก Step 1 พร้อมผลและจำนวน test จริง, และสรุปสิ่งที่แต่ละชุดพิสูจน์: invariant ของ domain (owner type ต้องลงทะเบียน, ไฟล์ต่าง org ถูกปฏิเสธ, ซ้ำ/เกินจำนวนถูกปฏิเสธ), กฎร่วมลายเซ็น (CP-07 behavior เดิม: role วัดก่อน trim, PNG 4 byte แรก, hash ของสตริง) พร้อม **regression CP-07 `ExternalAcceptance_*` ผ่านหลัง lift**, handler (ลำดับ owner type → permission → scope → validation → state), Integration: cross-org = 404 ทุก operation รวมการอ่านไฟล์, owner type ไม่ลงทะเบียน = 422, permission = 403, batch ล้มเหลวไม่ผูกไฟล์ใด (ไฟล์อัปโหลดแล้วยังผูกภายหลังได้), ไฟล์ของ parent อื่น = 422, idempotency replay/คีย์ซ้ำ payload ต่าง, ลายเซ็นเก็บ hash ของ bytes + เวลาเซิร์ฟเวอร์ + ไม่อยู่ใน audit, ล็อกตามสถานะ owner, Frontend: อัปโหลดเฉพาะตอนกดบันทึก, retry ไม่อัปโหลดซ้ำ, ยืนยันก่อนลบ + isLoading, i18n th/en ครบทุก purpose/error/consent; (3) **การตรวจในเบราว์เซอร์** ตามผล Task 12 Step 5 (หรือระบุว่าไม่ได้ตรวจ) รวมข้อสังเกตเรื่อง `SignaturePad`; (4) **เกณฑ์จบ "อย่างน้อย 2 โมดูลใช้ component เดียวกันได้โดยไม่แก้ของกลาง"** — สถานะ: พิสูจน์ส่วนที่ทำได้ในรอบนี้ (owner แรก `installation-job` ใช้ `AttachmentList`/`SignatureCapturePanel`/handler/store/controller กลางโดยไม่มีโค้ดเฉพาะ installation ในของกลาง ยกเว้นจุดลงทะเบียนสามจุด) และระบุว่าผู้ใช้รายที่สอง (G-09 Quick Estimate / G-18) ต้องยืนยันตอนลงทะเบียน owner ของตนตาม checklist ในสัญญา; (5) **ข้อจำกัด**: ไฟล์เป็นรูปภาพเท่านั้น (PDF ของ Procurement ต้องขยาย Files module), ไม่บังคับลายเซ็นก่อนรับมอบ (รอ Business), ไม่ใช่ e-Signature ตามกฎหมาย (Validation Question ค้าง), โมดูลเดิม (Work Images/Survey Evidence/Item Images) ยังใช้ตารางของตัวเอง, ไม่มี DB foreign key ไปยัง owner (polymorphic) จึงพึ่ง test parity, การเปลี่ยนสถานะ owner ระหว่าง resolve กับ save ไม่ถูก lock (TEST_ONLY), ไม่ได้รัน full suite/`next build`/Playwright, ไม่มี permission key ใหม่จึงไม่ต้อง seed Role เพิ่ม.

- [ ] **Step 3: อัปเดตแผนหลัก, สัญญา Service, Roadmap, สถานะสัญญา**

1. `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` แถวตารางหัวข้อ 3: เปลี่ยนเซลล์สุดท้ายของแถว G-01 จาก `—` เป็น `[แผน G-01](2026-10-05-g01-shared-attachment-signature.md)`:

```bash
python3 - <<'PY'
p = "docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md"
s = open(p, encoding="utf-8").read()
old = "| G-01 | Shared Attachment & Signature | A | Files module เดิม | Security | — |"
new = "| G-01 | Shared Attachment & Signature | A | Files module เดิม | Security | [แผน G-01](2026-10-05-g01-shared-attachment-signature.md) |"
assert s.count(old) == 1
open(p, "w", encoding="utf-8").write(s.replace(old, new))
PY
grep -n "^| G-01" docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md
```

Expected: แถว G-01 มีลิงก์แผน.

2. `docs/03-contracts/service-api-contract.md`: ในตาราง Decisions แถว `Checklist` เปลี่ยนท้ายประโยค `ไม่มี evidence/ไฟล์แนบในรอบนี้` เป็น `ไฟล์แนบและลายเซ็นผู้รับมอบใช้ [Attachment API](attachment-api-contract.md) (owner `installation-job`, G-01); ยังไม่บังคับให้ส่งมอบต้องมีลายเซ็น`. ใน Endpoints ต่อท้ายตารางเพิ่มแถวอ้างอิง: `| ไฟล์แนบ/ลายเซ็น | ดู [Attachment API](attachment-api-contract.md) | installations.read / operate / handover |`.
3. `docs/05-engineering/service-verification.md` หัวข้อ "ข้อจำกัด": เปลี่ยน `ไม่มีไฟล์แนบ/หลักฐานภาพ (...) ... ไม่มีลายเซ็นลูกค้าตอนส่งมอบ (...)` ให้เติมท้ายว่า `(ปิดโดย G-01: ดู [Shared Attachment & Signature Verification](shared-attachment-signature-verification.md); ลายเซ็นยังไม่ถูกบังคับก่อนรับมอบ)` แทนการลบข้อจำกัดเดิม.
4. `docs/00-overview/implementation-roadmap.md`: เติมท้ายเซลล์หลักฐานของแถว `Installation/Handover/Warranty/Service`:

```bash
python3 - <<'PY'
p = "docs/00-overview/implementation-roadmap.md"
lines = open(p, encoding="utf-8").read().split("\n")
i = next(n for n, l in enumerate(lines) if l.startswith("| Installation/Handover/Warranty/Service |"))
assert lines[i].endswith(" |")
lines[i] = lines[i][:-2] + "; ไฟล์แนบและลายเซ็นผู้รับมอบ (G-01, 2026-10-05): ใช้ Attachment Link/Signature Capture กลางที่ลงทะเบียน owner `installation-job` ([Attachment API](../03-contracts/attachment-api-contract.md), [Verification](../05-engineering/shared-attachment-signature-verification.md)) |"
open(p, "w", encoding="utf-8").write("\n".join(lines))
PY
```

5. `docs/03-contracts/attachment-api-contract.md` บรรทัดสถานะ: เปลี่ยน `Draft → Implemented เมื่อ G-01 เสร็จ` เป็น `Implemented 2026-10-05 (G-01)`.

- [ ] **Step 4: ตรวจลิงก์ภายในของเอกสารที่เพิ่ม/แก้**

Run: `cd /Users/syaco/Documents/development/tan-erp && for f in docs/05-engineering/shared-attachment-signature-verification.md docs/03-contracts/attachment-api-contract.md docs/adr/0017-shared-attachment-owner-registry.md; do grep -o "](\.\./[^)#]*\|](\./[^)#]*\|]([a-z0-9-]*\.md" "$f" | sed 's/](//' | while read -r link; do target="$(dirname "$f")/$link"; [ -e "$target" ] || echo "BROKEN in $f: $link"; done; done`
Expected: ไม่มีบรรทัด `BROKEN`.
Run: `git status --short`
Expected: เฉพาะไฟล์เอกสารที่ระบุใน Task นี้ถูกแก้ (ไม่มีไฟล์ src ค้าง).

- [ ] **Step 5: commit**

```bash
git add docs/05-engineering/shared-attachment-signature-verification.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/03-contracts/service-api-contract.md docs/05-engineering/service-verification.md docs/00-overview/implementation-roadmap.md docs/03-contracts/attachment-api-contract.md
git commit -F - <<'EOF'
docs(attachments): add G-01 verification and link the plan in the gap closure table

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Self-review (ตรวจเทียบ spec G-01)

| ข้อกำหนดใน spec | ครอบคลุมโดย |
| --- | --- |
| `AttachmentLink { ownerType, ownerId, fileId, purpose, createdBy }`, owner type เป็น whitelist ในโค้ด, permission ตาม owner (registry: read/manage + owner-scope lookup บังคับ Org/Branch) | Task 2 (`AttachmentOwnerTypes`, `AttachmentLink`), Task 4 (`AttachmentOwnerRegistry`, `ResolveOwnerAsync`), Task 6 (`AttachmentOwnerScopeReader`) |
| Domain invariants: owner type ต้องลงทะเบียน, ไฟล์ org เดียวกับ owner, ไม่ซ้ำ file+owner+purpose | Task 2 (ctor + `AssertCanAdd`), Task 5 (composite FK + partial unique index) |
| `SignatureCapture { signerName, signedAtUtc, imageFileId, consentTextVersion, contentHash }` + lift ตรรกะ CP-07 ขึ้น Application/Common | Task 2 (`SignatureCapture`), Task 3 (`SignatureEvidenceRules` + regression CP-07) |
| Frontend `<AttachmentList ownerType ownerId />` และ `<SignaturePad />` ใน `components/forms/` ใช้ `useDeferredFileUpload` | Task 10 (`AttachmentList`), Task 11 (`SignatureCapturePanel` ใช้ `SignaturePad` เดิม) |
| i18n th+en ครบ, ไม่มี any/chained fallback, Tailwind tokens, ยืนยันก่อนลบ + isLoading | Task 9 (messages + parity test), Task 10–11 |
| Tests: cross-owner/cross-org 404, owner type ไม่ลงทะเบียนถูกปฏิเสธ, ไฟล์ค้างไม่ถูกผูกเมื่อ submit ล้มเหลว, unit invariants | Task 2, 4, 8, 10 |
| owner type จริงหนึ่งตัว (InstallationJob handover) พิสูจน์ end-to-end | Task 6–8, 12 |
| Contract doc + permission catalog + error code th/en | Task 1, Task 7 |
| Migration (`dotnet ef migrations add` + flags) | Task 5 |
| Frontend types (`npm run check:api`) | Task 7 (OpenAPI snapshot), Task 9 |
| Docs: verification doc + แถว G-01 ในตารางแผนหลัก | Task 13 |

**ความสอดคล้องของชื่อข้ามงาน:** `AttachmentOwnerTypes.InstallationJob = "installation-job"` (Task 2) ↔ registry key (Task 4) ↔ scope reader `case` (Task 6) ↔ `ATTACHMENT_OWNER_TYPES` (Task 9); `SignatureConsentVersions.Handover = "handover-2026-10-v1"` (Task 4) ↔ `SIGNATURE_CONSENT_VERSIONS.handover` (Task 9) ↔ i18n key (Task 9) ↔ integration test `ConsentVersion` (Task 8); `AttachFilesInput(Purpose, FileIds)` / `SignatureCaptureCommand` ใช้ตรงกันใน handler (Task 4), store (Task 6), controller (Task 7); error codes 11 ตัวเท่ากันใน mapper/resx/messages/`ATTACHMENT_ERROR_CODES`.
