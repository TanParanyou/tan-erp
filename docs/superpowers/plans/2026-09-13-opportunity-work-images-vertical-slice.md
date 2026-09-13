# Opportunity Work Images Vertical Slice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ให้ Sales เลือกและแนบภาพงานจริง **หลายภาพต่อการบันทึกและต่อ stage** ของ Opportunity ช่วง Open, เห็นแกลเลอรีแยกตาม stage และเปิดย้อนหลังหลังปิด โดย File Service ถือไฟล์และ CRM ถือ scoped references เท่านั้น

**Architecture:** เพิ่ม `opportunity_work_images` เป็น append-first reference ของภาพงานที่ verified แล้ว; File Service กลางจัด upload-session/complete/preview **แยกต่อไฟล์** พร้อมตรวจ permission และ scope. Frontend เลือกหลายภาพ, preview และใส่ caption รายภาพในเครื่องก่อน; กด Save จึง upload/verify ทุกภาพ แล้วส่ง **คำขอ batch เดียว** ผูก verified `fileId` ทั้งชุดกับ Opportunity ด้วย If-Match/idempotency. Survey/Estimate/Commercial ยังคงเป็นเจ้าของหลักฐานของตน; gallery ของ Opportunity ต่อภาพจากโมดูลนั้นด้วย authorized references ใน slices ถัดไป ไม่คัดลอก binary.

**Tech Stack:** .NET SDK 10.0.400, ASP.NET Core/EF Core 10.0.11, PostgreSQL 17, Next.js 16.3.4, React 19.2.8, TypeScript, TanStack Query, Vitest, Playwright; reuse image optimization/`ImageInputPreview` through proposed shared `MultiImagePicker` และ generated OpenAPI types

## Global Constraints

- แผนนี้เป็น Slice 1A ของ [Opportunity master plan](2026-09-12-opportunity-module-completion-master-plan.md), **ไม่อนุญาต implementation โดยตัวเอง**; ต้องมี task อนุมัติแยกและแก้ `AGENTS.md` boundary ก่อน code.
- Authoritative source: [CRM flow](../../01-business/crm-site-survey-flow.md), [governance](../../01-business/crm-site-survey-governance.md), [field catalog](../../01-business/crm-site-survey-field-catalog.md), [API](../../03-contracts/crm-site-survey-api-contract.md), [data contract](../../04-data/crm-site-survey-data-contract.md), [module boundary](../../02-architecture/module-boundaries.md). ตอนนี้มี Survey evidence contract และ File Service upload-session concept ใน [Quick Estimate API](../../03-contracts/quick-estimate-api-contract.md) แต่ **ยังไม่มี persisted Opportunity image/file-upload API ในโค้ด**.
- ก่อน implement ต้องเสนอ Global Reuse: adapter File Service สำหรับ upload-session, complete/verify, authorized preview, retry ใช้ร่วม Survey/Quick Estimate; `ImageUpload` เดิมรองรับทีละ `File` จึงต้องเสนอ shared `MultiImagePicker` ที่ใช้ image optimization/preview เดิม รองรับ native `multiple` selection และรายการไฟล์ ไม่สร้าง upload logic เฉพาะ Opportunity ซ้ำ.
- Sales/Privacy Owner ต้องยืนยัน stage-requiredness, MIME/size, EXIF/location removal, caption, visibility/export, retention และ detach policy ใน authoritative docs. ระหว่างรอ: **แนบ optional** ใน Open stages; Closed read-only; ห้ามทำ Q gate หรือ stage gate บังคับรูปโดยเดา.
- ไฟล์อาจมีข้อมูลลูกค้า/สถานที่: File Service เก็บ binary; DB เก็บ IDs/metadata ที่จำเป็น; ห้าม Base64 ใน JSON, signed URL ใน DB/audit/log, raw caption/EXIF/location ใน audit. GET/preview ตรวจ organization/branch/own scope ทุกครั้ง, out-of-scope เป็น 404.
- UI ใช้ Thai default/English key parity, generated API DTO, central ApiClient, TanStack Query, Tailwind semantic tokens, keyboard, focus, 44px target, 320px/200% zoom, **สถานะต่อภาพ** (selected/uploading/failed/verified) และ double-submit lock. เลือกไฟล์หลายภาพ **ไม่ upload**; upload เริ่มใน `onSubmit` เท่านั้น. หากภาพใดล้มเหลว ห้ามผูกทั้ง batch หรือแสดงว่า saved; คงภาพที่ verify ผ่านใน local intent เพื่อ retry เฉพาะภาพล้มเหลวโดยไม่ upload ซ้ำ.
- ข้อมูลผู้แนบที่แสดงบนจอต้องมาจาก backend structured projection `createdBy: {id,displayName}`; ห้ามส่ง UUID แล้วให้ frontend `array.find(...)` หา label หรือใช้ fallback field ลูกโซ่. ถ้าไม่มี displayName ให้แสดงสถานะว่างตาม contract; ไม่เดาข้อมูล.
- เพิ่ม automated tests ใหม่ **5 จุด เฉพาะ success path**; รัน regressions/build/lint/check:api. Security/negative/concurrency tests เป็น hardening gate ของ master plan ก่อน Production. ไม่มี code edit, migration, test run หรือ commit ในรอบเขียนแผน.

---

## Journey and Stage Ownership

Sales เปิด Draft/Qualified/Open Opportunity → เลือก **หลายภาพในครั้งเดียวหรือเพิ่มหลายรอบก่อน Save** → เห็น local preview/กรอก caption แยกรูป/เอารูปที่เลือกผิดออกได้ → กด Save ครั้งเดียว → File Service upload/verify ต่อภาพ → API ผูกชุด `fileId` กับ Opportunity/current stage แบบ atomic → gallery แสดง **ภาพทั้งหมด**, caption/stage/เวลา และจำนวนภาพ → refresh แล้วยังเห็นครบ. เพิ่ม batch ใหม่ใน stage เดิมหรือ stage เปิดถัดไปได้; จำนวนภาพรวมต่อ Opportunity ไม่จำกัดด้วยการทำเพียง batch เดียว. ใน Surveying/Estimating/Proposed เพิ่มภาพ CRM ได้เหมือนกันตราบใดที่ Opportunity ยัง Open; image จาก Survey/Estimate/Quotation แสดงผ่าน source reference เมื่อโมดูลนั้นพร้อม. Won/Lost/Cancelled ดูย้อนหลังได้แต่ห้ามแนบ/ถอด; ต้อง Reopen ตาม policy ก่อน.

## Proposed Exact Contract to Freeze

### File Service dependency

```http
POST /api/v1/files/upload-sessions
Authorization: Bearer <Firebase ID token>
X-Membership-Id: <membership UUID>
Content-Type: application/json
```

```json
{"parentType":"opportunity","parentId":"<opportunity UUID>","mimeType":"image/webp","sizeBytes":123456}
```

File Service ตรวจ parent access/scope, MIME/size/scan policy แล้วคืน `201 {sessionId,fileId,uploadUrl,expiresAtUtc}` **ต่อภาพ**; `uploadUrl` อายุสั้น ไม่ log/store ใน CRM. Client upload bytes เข้าแต่ละ session เฉพาะหลัง Save แล้วเรียก `POST /api/v1/files/upload-sessions/{sessionId}/complete` ต่อภาพ; File Service คืน `{fileId,status:"verified"}` หลัง validate/checksum/scan. คำขอผูก Opportunity รับเฉพาะ `verified` files ทั้งหมดของ Organization/parent เดียวกัน. ถ้า upload หรือ verify ภาพใดล้มเหลว **ไม่ส่ง batch bind**; เก็บ verified file IDs ไว้ใน intent เดิมเพื่อ retry เฉพาะภาพที่ล้มเหลว. Pending/orphan files ใช้ File Service retention policy ไม่ delete รูปที่ผูกแล้วโดยตรง. Freeze API นี้ใน authoritative file contract ก่อน implement เพราะ Quick Estimate doc ระบุเพียง concept.

### Opportunity-owned image reference

```http
POST /api/v1/opportunities/{id}/work-images
Authorization: Bearer <Firebase ID token>
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
If-Match: "<current Opportunity rowVersion UUID>"
Content-Type: application/json
```

```json
{"images":[{"fileId":"<verified file UUID 1>","caption":"มุมหน้าตู้ TEST_ONLY"},{"fileId":"<verified file UUID 2>","caption":"มุมภายในตู้ TEST_ONLY"}]}
```

- Backend รับ `images` แบบ ordered array ขนาด **1–20 ภาพต่อ batch** ตามข้อเสนอเริ่มต้น (เพิ่ม batch ถัดไปได้; Sales/Technical Owner ยืนยัน limit ที่ Task 0), `fileId` ห้ามซ้ำใน batch. Backend derive `stageAtAttach`, actor, organization, branch, timestamp และ `displayOrder` จากลำดับ array; request ห้ามส่งค่าเหล่านี้. ยอมรับเฉพาะ `draft`, `qualified`, `surveying`, `estimating`, `proposed`. Reject Closed. Caption ต่อภาพ optional/trimmed สูงสุด 500 ตัวอักษรตามข้อเสนอเริ่มต้น; ทุก `fileId` ต้อง verified, parent ตรง Opportunity, scope ตรง Organization, MIME เป็นภาพที่ policy อนุญาต. ไม่มีข้อมูลไฟล์ binary ใน request. เกิน 20 ภาพใน draft เดียวต้องแจ้งให้แบ่ง Save ก่อนเริ่ม upload; ไม่มี truncation เงียบ ๆ.
- Success `201 Created`, `ETag: "<new Opportunity rowVersion>"`, body `{items:[{id,fileId,stageAtAttach,caption,displayOrder,createdAtUtc,createdBy:{id,displayName}}],opportunityRowVersion}`; transaction เดียวบันทึก **ทุก reference หรือไม่มีเลย**, rotate Opportunity rowVersion ครั้งเดียว, idempotency record หนึ่งรายการ และ `opportunity.work-images-added` audit หนึ่ง event เฉพาะ IDs/stage. Replay key + ordered body + original If-Match เดิมคืน response/ETag เดิม ไม่สร้าง record/audit ซ้ำ. Retry หลัง timeout ใช้ key/ordered body/If-Match เดิม; เปลี่ยนภาพหรือ caption ถือเป็น intent ใหม่.
- `GET /api/v1/opportunities/{id}/work-images?stage=&limit=25&cursor=` ใช้ `opportunities.read`, คืน `{items:[...],nextCursor}` เรียง `(createdAtUtc DESC,displayOrder ASC,id ASC)`; เปิดหน้าถัดไปได้เพื่อแสดงภาพทั้งหมด ไม่ตัดที่ 25 ภาพ. Response ไม่ฝัง signed URL ถาวร. UI ขอ authorized short-lived preview จาก File Service ตาม `fileId`, ไม่ reuse URL หลังสิทธิ์/สมาชิกภาพเปลี่ยน.
- `DELETE /api/v1/opportunities/{id}/work-images/{imageId}` ใช้ `opportunities.update`, `Idempotency-Key`, quoted `If-Match`; ทำ **soft detach** เฉพาะ Open, rotate rowVersion, คืน `204` + ETag ใหม่, audit เฉพาะ IDs. Binary ไม่ลบ; retention/redaction ของ File Service เป็น workflow แยก. UI ต้องมี confirmation modal สำหรับ detach.
- Error codes อิง authoritative error contract: 400 malformed/context, 401 auth, 403 permission, 404 parent/file/image out-of-scope, 409 `OPPORTUNITY_VERSION_CONFLICT`/`OPPORTUNITY_INVALID_TRANSITION`/`IDEMPOTENCY_KEY_REUSED`, 428 `IF_MATCH_REQUIRED`; เพิ่ม stable `OPPORTUNITY_IMAGE_NOT_READY` (409) เมื่อ file ยังไม่ verified และ localized `.resx` ทั้งสองภาษาก่อนใช้. Invalid MIME/size/caption เป็น 422 พร้อม stable field error ตาม file contract ที่ freeze ใน Task 0. Error ไม่สะท้อนชื่อไฟล์/caption/URL.

### Persistence

New `crm.opportunity_work_images`: `id uuid PK`, `organization_id uuid`, `opportunity_id uuid` (composite FK id+organization), `file_id uuid`, `stage_at_attach varchar(32)`, `display_order int`, `caption varchar(500) NULL` ตามข้อเสนอเริ่มต้น, `created_by_user_id uuid`, `created_at_utc timestamptz`, `detached_at_utc timestamptz NULL`; unique active `(organization_id,opportunity_id,file_id)` and index `(organization_id,opportunity_id,created_at_utc DESC,display_order ASC,id ASC)`. File Service holds bytes, content type, checksum/scan state and storage location; CRM never stores signed URL or raw EXIF. Stage at attach and display order are immutable even after transition/reopen. Retention-safe migration must be additive; rollback after business references exist requires forward-fix.

## Planned Files

| Concern | Files |
| --- | --- |
| Global File Service | New focused API/application/infrastructure files under `backend/src/TanErp.Api/Controllers/FilesController.cs`, `backend/src/TanErp.Application/Files/`, `backend/src/TanErp.Infrastructure/Files/`; shared frontend adapter `frontend/src/lib/api/file-client.ts`. Exact provider implementation selected at Task 0; do not invent provider-specific secrets in plan or code. |
| CRM domain/persistence | New `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityWorkImage.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityWorkImageConfiguration.cs`, additive EF migration; modify `Opportunity.cs`, `IOpportunityStore.cs`, `OpportunityStore.cs`, `AppDbContext.cs`. |
| API | New request/response contracts under `backend/src/TanErp.Api/Contracts/Crm/Opportunities/`; modify `OpportunitiesController.cs`, `Program.cs`, `ProblemDetailsMapper.cs` and `Errors*.resx` only for new code; generate OpenAPI types. |
| Frontend | Modify `frontend/src/lib/api/api-client.ts`, `frontend/src/features/opportunities/api/opportunity-queries.ts`, `frontend/src/features/opportunities/components/opportunity-detail.tsx`, `frontend/src/messages/th.json`, `en.json`; new reusable `frontend/src/components/forms/MultiImagePicker.tsx` ใช้ image optimization/`ImageInputPreview` เดิม; new `frontend/src/features/opportunities/components/opportunity-work-image-form.tsx` and `opportunity-work-image-gallery.tsx`. |
| Tests/docs | `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`, new File Service integration test under `backend/tests/TanErp.IntegrationTests/Api/`, `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs`, `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`, new `frontend/e2e/opportunity-work-images.spec.ts`; modify authoritative field/API/data/governance docs and UAT after approval; new `docs/05-engineering/opportunity-work-images-verification.md`. |

## Tasks and Five New Happy-Path Tests

### Task 0: Policy, reuse and provider contract (no new test)

- [ ] Confirm provider/storage location, server-side preview authorization, scanning, MIME/size/caption, stage optionality, privacy/EXIF and retention with Sales/Privacy/Security Owner. Present reusable File Service adapter proposal before code. Update authoritative contracts/field catalog/governance and `AGENTS.md` approved boundary; no silent adoption of Quick Estimate's upload-session concept as implemented API.
- [ ] Proposed commit for implementation session: `docs(opportunities): freeze work image and file contracts`.

### Task 1: Shared upload/verify path — test 1

**Files:** `FilesController.cs`, `backend/src/TanErp.Application/Files/`, `backend/src/TanErp.Infrastructure/Files/`, new File Service integration test. **Interface:** `CreateUploadSession(parentType="opportunity",parentId,mimeType,sizeBytes)` and `CompleteUploadSession(sessionId)` return verified `fileId` before CRM bind.

- [ ] Add `OpportunityImageUpload_ValidParentAndImage_ReturnsVerifiedFileId`; run filtered `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~OpportunityImageUpload_ValidParentAndImage_ReturnsVerifiedFileId` to see red.
- [ ] Implement scoped session/complete/verify adapter, provider boundary and short-lived authorized preview; rerun targeted test green. Proposed commit: `feat(files): add reusable verified image upload`.

### Task 2: CRM reference write/read — tests 2 and 3

**Files:** `OpportunityWorkImage.cs`, configuration/migration, `Opportunity.cs`, `IOpportunityStore.cs`, `OpportunityStore.cs`, `OpportunitiesController.cs`, API contracts, `OpportunityStoreTests.cs`, `OpportunitySiteEndpointsTests.cs`. **Interface:** define `VerifiedWorkImageInput(Guid FileId, string? Caption)` in Opportunity application contracts; `AddWorkImagesAsync(access, opportunityId, expectedVersion, IReadOnlyList<VerifiedWorkImageInput> images, idempotencyKey)` returns ordered image items with backend-projected `createdBy` and updated Opportunity rowVersion.

- [ ] Add `AddWorkImages_TwoVerifiedFiles_PersistsBothInOneTransactionAndOneAudit` (PostgreSQL) and `OpportunityWorkImages_PostTwoThenGet_ReturnsBothAndNewETag` (HTTP/OpenAPI); run their filtered `dotnet test` commands red.
- [ ] Implement scoped EF transaction, verify **all** file IDs before binding, stage/display order, replay, API mapping and additive migration; rerun both green. `DELETE` soft detach รายรูป follows same access/version/audit path; no new negative test in Pilot. Proposed commit: `feat(opportunities): store stage-tagged work image batches`.

### Task 3: Deferred-upload form and gallery — test 4

**Files:** `MultiImagePicker.tsx`, `file-client.ts`, `api-client.ts`, `opportunity-queries.ts`, `opportunity-work-image-form.tsx`, `opportunity-work-image-gallery.tsx`, `opportunity-detail.tsx`, `th.json`, `en.json`, `opportunity-detail.test.tsx`.

- [ ] Add `OpportunityDetail_SaveTwoWorkImages_UploadsOnlyOnSubmitThenShowsBoth`: assert native multi-select creates two previews/captions with no request, Save triggers two session/upload/verify flows and **one** ordered batch bind, gallery displays both, pending Save disabled. Run `npm --prefix frontend run test -- opportunity-detail.test.tsx` red.
- [ ] Implement shared `MultiImagePicker` using existing image optimization/preview, stable local IDs, remove-before-save and per-file status; `File[]` stays local until `onSubmit`. Keep verified file IDs in retry intent if later file fails; only bind when all verified. Use generated types, central clients and TanStack Query cache; authorize each preview. Rerun green. Proposed commit: `feat(opportunities): attach and browse multiple work images`.

### Task 4: Browser journey and evidence — test 5

**Files:** new `frontend/e2e/opportunity-work-images.spec.ts`, new verification record, `docs/README.md`.

- [ ] Add `sales attaches multiple work images and sees both after refresh`: test Draft select two images/preview/no upload, Save once, two visible thumbnails + separate captions + stage, refresh-persistent gallery, 320px viewport. Run targeted Playwright with configured File Service test fixture and record exit code; no invented result.
- [ ] Run `dotnet build backend/TanErp.slnx`, `dotnet test backend/TanErp.slnx`, `npm --prefix frontend run check:api`, `npm --prefix frontend run verify`, `npm run test:fixtures`, `git diff --check`; record exact tested SHA, commands/results and unresolved privacy decisions. Proposed commit: `test(opportunities): verify work image journey`.

## Definition of Success

- Sales ใน Open Opportunity เลือกหลายภาพในครั้งเดียว, preview/caption แยกรูป, Save ครั้งเดียวแล้วจึง upload/verify ต่อรูปและ bind ทั้งชุด; เพิ่ม batch ใหม่ภายหลังได้, gallery โหลดหน้าถัดไปจนเห็นภาพทั้งหมด, Closed ดูย้อนหลังอย่างเดียว.
- DB มี scoped references ไม่ใช่ binary/URL; batch bind แบบ all-or-nothing และ rotate rowVersion ครั้งเดียว; image ที่ยังไม่ verified ไม่ถูกผูก; audit ไม่มีภาพ/caption/location/URL; image จาก Survey/Estimate/Commercial ไม่ถูก copy เข้ามา CRM.
- 5 new happy-path tests + existing build/regressions ผ่านตามหลักฐานจริง; Thai/English parity และ responsive/keyboard checks ผ่าน. Pilot เท่านั้นจน negative/privacy/security/retention tests ใน hardening slice ผ่าน.

## Deferred Scope

- Survey evidence, Estimate/Quotation files และ acceptance documents ถูกสร้างโดยโมดูลเจ้าของใน slices ภายหลัง; Opportunity gallery แสดงผ่าน authorized cross-module references เท่านั้น.
- บังคับจำนวนรูปต่อ stage, image annotation/cropping ขั้นสูง, reorder ภาพหลังบันทึก, public sharing, export, hard delete และ retention/redaction implementation จน policy/owner ยืนยัน. **การเลือกและบันทึกหลายภาพในครั้งเดียวไม่ใช่ deferred scope.**
- Negative/security/concurrency tests เพิ่มเติมตาม module hardening; ไม่ใช้ happy-path Pilot เพื่ออ้าง Production readiness.
