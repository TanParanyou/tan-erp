# Opportunity Draft Q-Gate Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ให้ Sales เปิด Opportunity Draft ที่ยังขาดข้อมูล Q gate, เติมข้อมูลสี่ฟิลด์, บันทึก แล้ว Qualify ได้จากหน้า detail เดิมโดยไม่สร้างโมดูลใหม่

**Architecture:** เพิ่มคำสั่งแก้เฉพาะ `scopeSummary`, `workTypes`, `nextActionAtUtc`, `nextActionNote` ของ Draft ใน Opportunity aggregate; Application ตรวจ `opportunities.update`, EF Core store ทำ conditional write/audit ใน organization scope, API คืน `OpportunityResponse` และ ETag ใหม่. Frontend วางฟอร์ม Q-gate แบบ inline ใน detail เดิม ใช้ generated OpenAPI DTO, central ApiClient, TanStack Query และ UI primitives ที่มีอยู่; Qualification endpoint/history เดิมไม่เปลี่ยน.

**Tech Stack:** .NET SDK 10.0.400, ASP.NET Core/EF Core 10.0.11, PostgreSQL 17, Next.js 16.3.4, React 19.2.8, TanStack Query 5.102.8, Vitest 5.0.0, Playwright 1.63.0 (ตาม verification ล่าสุด)

## Global Constraints

- นี่เป็น **แผน** ไม่ใช่อนุญาตให้ implement; `AGENTS.md` ยังให้ Opportunity Qualification เป็น boundary ปัจจุบัน ต้องได้รับ implementation task แยกก่อนทำงานและจึงปรับ boundary ใน `AGENTS.md`.
- Baseline คือ [Qualification verification](../../05-engineering/opportunity-qualification-verification.md) ณ tested commit `4a27e2931f25ac642d4aa287834791a34160cb1c` สถานะ Pilot Passed; บันทึกนั้นมี Playwright spec แต่ไม่มีหลักฐานการ **รัน** E2E. ก่อนเริ่ม slice ให้รัน existing qualification E2E ครั้งหนึ่งและบันทึกผล; ถ้าไม่ผ่านให้หยุดแก้ prerequisite แยก ไม่อ้างว่าผ่าน.
- ปัจจุบันมี user changes ค้างใน `opportunity-detail.tsx`, `opportunity-editor.tsx`, tests และ messages; เมื่อ implement ให้ inspect diff และรักษา changes เหล่านั้น ห้าม reset/overwrite. ฟอร์มใหม่ควรอยู่ใน component เฉพาะ Q-gate ไม่ขยาย create editor.
- ใช้ authoritative [business flow](../../01-business/crm-site-survey-flow.md), [field catalog](../../01-business/crm-site-survey-field-catalog.md), [API contract](../../03-contracts/crm-site-survey-api-contract.md), [error contract](../../03-contracts/error-contract.md), [data contract](../../04-data/crm-site-survey-data-contract.md), [UAT](../../05-engineering/crm-site-survey-uat-scenarios.md), `AGENTS.md`, `CONTEXT.md`, `design.md`, และ `docs/README.md`; แก้ concern ในเอกสาร authoritative เดิม ไม่แตกกฎซ้ำ.
- เพิ่ม automated tests ใหม่ **7 จุด เฉพาะ successful path**: domain, handler, PostgreSQL store, HTTP/OpenAPI, frontend API/cache, component, Playwright อย่างละหนึ่ง. ยังต้องรัน regression/build/lint/check:api ตาม repo; negative/security/concurrency proof เลื่อนเป็น hardening และผล slice นี้อ้างได้แค่ Pilot ไม่ใช่ Production.
- Backend เป็น security authority; ใช้ permission key `opportunities.update`, active membership/branch, tenant isolation, EF Core writes, RFC 9457 Problem Details, localized `.resx`; ไม่มี raw SQL write หรือ role-name check.
- ไม่มี migration ใหม่: four fields, row version, audit และ idempotency schema มีแล้ว. ห้ามแก้ stage/history/Customer/Site IDs หรือ ownership ผ่าน PATCH นี้.
- UI เป็น Thai default, เพิ่มคู่ `th`/`en` ทุก key, Tailwind semantic tokens, 0px radius, keyboard focus, touch target ≥44px, loading/disabled submit, 320px และ 200% zoom; strict TypeScript ห้าม `any`, `as any`, `@ts-ignore`.
- ใช้ของกลางก่อน: `RequestContextReader.ReadConditionalIdempotentRequest`, `OpportunityResponse`, `ApiClient`, query keys, `Input`/`Textarea`/`DateTimePicker`/`Checkbox`/`Button`, permission helper และ `ApiError.code`. ไม่มี global reusable primitive ใหม่ใน slice นี้.
- Proposed commits ด้านล่างเป็น checkpoints สำหรับ **รอบ implement เท่านั้น**; รอบเขียนแผนนี้ห้าม commit.

---

## Acceptance Journey

Sales ที่มี `opportunities.read`, `opportunities.update`, `opportunities.transition` → เปิด Draft ที่มี work type แต่ขาด scope/next action → เปิด inline Q-gate form → กรอก scope summary, work types, next-action datetime + note → Save → detail ได้ ETag ใหม่และแสดง Q gate ครบ → ยืนยัน Qualify ผ่าน flow เดิม → เห็น Qualified ใน detail/list. PostgreSQL มี audit `opportunity.updated` แบบ changed-field names เท่านั้น และ stage history เพิ่มหนึ่ง record **เฉพาะ** หลัง Qualify.

## Exact Contract to Freeze Before Code

```http
PATCH /api/v1/opportunities/{id}
Authorization: Bearer <Firebase ID token>
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
If-Match: "<current rowVersion UUID>"
Accept-Language: th | en
Content-Type: application/json
```

```json
{
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "nextActionAtUtc": "2026-09-15T03:00:00Z",
  "nextActionNote": "โทรยืนยันนัด TEST_ONLY"
}
```

- Body เป็น **full replacement ของสี่ฟิลด์นี้เท่านั้น** (ไม่ใช่ generic/partial PATCH): ต้องส่งครบทุก key; `scopeSummary`, `nextActionAtUtc`, `nextActionNote` รับ `null` เพื่อให้ Draft ยังไม่ครบ Q gate ได้; `workTypes` ต้อง non-empty, unique canonical values. การส่ง key อื่น เช่น `stage`, `ownerUserId`, `customerId`, `primarySiteId`, `title`, budget ถูกปฏิเสธด้วย `400 REQUEST_VALIDATION_FAILED` ไม่ silently ignore.
- Normalize text เหมือน CreateDraft (`trim/collapse whitespace`); field catalog ปัจจุบันกำหนด `scopeSummary` ≤2,000 แต่ frontend schema ใช้ 1,000 จึงต้อง reconcile กับ Sales/API Owner ก่อน implement; `nextActionNote` ≤500 ตาม schema ปัจจุบัน, UTC datetime รูป RFC 3339; `nextActionAtUtc` กับ `nextActionNote` ต้องเป็นคู่ (มีทั้งสองหรือไม่มีทั้งสอง). ความไม่ครบของ scope/next-action **บันทึก Draft ได้** แต่ Qualify ยังถูก Q gate เดิมปฏิเสธ.
- `200 OK` คืน `OpportunityResponse` เดิมพร้อม `ETag: "<new rowVersion>"`; ค่า four fields เปลี่ยนและ `rowVersion` rotate, `stage=draft`, identity/ownership/other fields ไม่เปลี่ยน; replay key + canonical body + original If-Match เดิมคืน body/ETag เดิมและไม่เพิ่ม audit. ไม่มี stage history จาก PATCH.
- Error matrix: `400 REQUEST_VALIDATION_FAILED` malformed/extra/missing field/invalid pair, `400 MEMBERSHIP_CONTEXT_REQUIRED`, `400 IDEMPOTENCY_KEY_REQUIRED|INVALID`, `401 AUTHENTICATION_REQUIRED|INVALID`, `403 ACTIVE_MEMBERSHIP_REQUIRED|PERMISSION_DENIED`, `404 RESOURCE_NOT_FOUND` (including cross-org), `409 IDEMPOTENCY_KEY_REUSED`, `409 OPPORTUNITY_VERSION_CONFLICT`, `409 OPPORTUNITY_INVALID_TRANSITION` (ไม่ใช่ Draft), `422 OPPORTUNITY_FIELD_REQUIRED` (invalid canonical work type/length), `428 IF_MATCH_REQUIRED`. ให้ resolve replay ก่อน version check; version check ก่อน edit state; ทุก error เป็น localized Problem Details ไม่สะท้อน business text.
- Backend transaction เขียน Opportunity + audit `opportunity.updated` payload `{"changedFields":["scopeSummary","workTypes","nextActionAtUtc","nextActionNote"]}` เฉพาะชื่อ field ที่เปลี่ยนจริง + idempotency record; ไม่มี field values, title, note, PII ใน audit/log. ใช้ ETag optimistic concurrency แบบ compare-and-swap/EF concurrency token เพื่อไม่ให้ stale write ชนะ.
- UI ใช้ `rowVersion` จาก detail เป็น If-Match; เก็บ idempotency key เดิมสำหรับ retry intent/body/version เดิมเท่านั้น. หลังสำเร็จ replace detail cache และ invalidate list; stale version แสดง localized reload guidance โดยไม่ auto-merge. Detail แสดงแก้ไขเฉพาะ Draft และผู้มี `opportunities.update`.

## Planned File Map

| Concern | Exact files and responsibility |
| --- | --- |
| Domain/application | `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs` (EditDraftQGate), `backend/src/TanErp.Application/Crm/Opportunities/UpdateDraftQGate/UpdateDraftQGateCommand.cs`, `UpdateDraftQGateHandler.cs`, `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs` |
| Persistence/API | `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`, `backend/src/TanErp.Api/Contracts/Crm/Opportunities/UpdateDraftQGateRequest.cs`, `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`, `backend/src/TanErp.Api/Program.cs`; modify `ProblemDetailsMapper.cs` / `Errors*.resx` only if an unmapped code is required |
| Frontend | generated `frontend/src/generated/api/tan-erp.v1.ts` via OpenAPI tool, `frontend/src/lib/api/api-client.ts`, `frontend/src/features/opportunities/api/opportunity-queries.ts`, new `frontend/src/features/opportunities/components/opportunity-q-gate-editor.tsx`, existing `opportunity-detail.tsx`, `frontend/src/messages/th.json`, `en.json` |
| Proof | `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs`, `OpportunityHandlerTests.cs`; `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`; `frontend/src/features/opportunities/api/opportunity-queries.test.tsx`, `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`, `frontend/e2e/opportunity-draft-q-gate.spec.ts` |
| Docs after approval | `AGENTS.md`, `docs/03-contracts/crm-site-survey-api-contract.md`, `docs/05-engineering/crm-site-survey-uat-scenarios.md`, `docs/05-engineering/opportunity-draft-q-gate-verification.md`, `docs/README.md`; no flow JSON change because no new stage/node |

## Tasks — Exactly Seven New Success Tests

### Task 0: Prerequisite and contract freeze (no new test)

- [ ] Read current diff in all touched files; run `npm --prefix frontend exec playwright test e2e/opportunity-qualification.spec.ts` using the repo's configured test environment and record actual result. If prerequisite E2E fails, stop this slice and report evidence.
- [ ] With separate implementation authorization, update `AGENTS.md` boundary, API contract and UAT exit criteria to the exact request/response/error/audit contract above before code. Do not broaden to generic Opportunity editing.
- [ ] Proposed commit: `docs(opportunities): freeze draft q-gate completion contract`.

### Task 1: Domain edit invariant — test 1

**Files:** `Opportunity.cs`; `OpportunityTests.cs`.

**Interface:** `Opportunity.EditDraftQGate(Guid expectedVersion, string? scopeSummary, IReadOnlyCollection<string> workTypes, DateTimeOffset? nextActionAtUtc, string? nextActionNote)` mutates only four fields and rotates `RowVersion`.

- [ ] Add `EditDraftQGate_ValidDraft_UpdatesQGateAndRotatesVersion`: create Draft via existing factory, call method with current version and complete values, assert four values, unchanged stage/identity, new version.
- [ ] Run `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~EditDraftQGate_ValidDraft_UpdatesQGateAndRotatesVersion`; expect fail before implementation.
- [ ] Implement state/version/normalization/length/work-type/pair invariants in aggregate; use existing typed Opportunity exceptions and stable mapper. Run same command; expect pass.
- [ ] Proposed commit: `feat(opportunities): allow draft q-gate field updates`.

### Task 2: Application permission and command — test 2

**Files:** new `UpdateDraftQGateCommand.cs`, `UpdateDraftQGateHandler.cs`; `IOpportunityStore.cs`; `OpportunityHandlerTests.cs`.

**Interface:** `UpdateDraftQGateCommand(string FirebaseUid, Guid MembershipId, Guid OpportunityId, Guid ExpectedVersion, string? ScopeSummary, IReadOnlyList<string> WorkTypes, DateTimeOffset? NextActionAtUtc, string? NextActionNote, string IdempotencyKey, string TraceId)`; `IOpportunityStore.UpdateDraftQGateAsync(RequestAccessContext, UpdateDraftQGateCommand, string keyHash, string payloadHash, CancellationToken)`.

- [ ] Add `UpdateDraftQGateHandler_ValidRequest_ResolvesUpdatePermissionAndPersists`: fake access resolver/store, assert permission `opportunities.update`, branch present, normalized canonical payload hash and returned projection.
- [ ] Run filtered `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~UpdateDraftQGateHandler_ValidRequest_ResolvesUpdatePermissionAndPersists`; expect fail, then implement handler with SHA-256 key/payload hashes and typed result; rerun to pass.
- [ ] Proposed commit: `feat(opportunities): authorize draft q-gate command`.

### Task 3: Atomic persistence — test 3

**Files:** `OpportunityStore.cs`; `OpportunityStoreTests.cs`.

- [ ] Add `UpdateDraftQGate_ValidDraft_PersistsFieldsVersionAuditAndReplay`: PostgreSQL fixture, save Draft, update four fields, assert new version, unchanged stage/history count, one privacy-safe `opportunity.updated` audit, replay returns same projection/audit count.
- [ ] Run filtered `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~UpdateDraftQGate_ValidDraft_PersistsFieldsVersionAuditAndReplay`; expect fail, then implement EF transaction and compare-and-swap on scoped row/version using existing idempotency/audit pattern; rerun to pass.
- [ ] Proposed commit: `feat(opportunities): persist conditional draft q-gate update`.

### Task 4: HTTP/OpenAPI — test 4

**Files:** new `UpdateDraftQGateRequest.cs`; `OpportunitiesController.cs`; `Program.cs`; `OpportunitySiteEndpointsTests.cs`; generated OpenAPI output only through existing generator.

- [ ] Add `PatchOpportunity_ValidDraft_ReturnsUpdatedResponseAndETag`: PATCH with all four fields, membership/idempotency/If-Match headers, assert 200, exact response fields/new quoted ETag, then GET confirms persistence. Assert generated OpenAPI contains PATCH request schema in the same test or existing contract suite.
- [ ] Run filtered `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~PatchOpportunity_ValidDraft_ReturnsUpdatedResponseAndETag`; expect fail, then wire thin controller with `ReadConditionalIdempotentRequest`, handler registration and strict body shape; rerun to pass.
- [ ] Run `npm --prefix frontend run check:api` after regenerating types with repo script; expect no drift.
- [ ] Proposed commit: `feat(opportunities): expose conditional draft q-gate patch`.

### Task 5: Frontend client and cache — test 5

**Files:** `api-client.ts`; `opportunity-queries.ts`; `opportunity-queries.test.tsx`; generated API type.

**Interface:** `apiClient.updateDraftQGate(id, payload: UpdateDraftQGateRequest, options: RequestOptions): Promise<OpportunityResponse>`; `useUpdateDraftQGate()` mutation accepts `{opportunityId, expectedVersion, payload, idempotencyKey}`.

- [ ] Add `useUpdateDraftQGate_Success_SendsIfMatchAndRefreshesCaches`: assert PATCH, quoted If-Match, Idempotency-Key, exact four-field JSON, replacement of detail query and invalidation of list query.
- [ ] Run `npm --prefix frontend run test -- opportunity-queries.test.tsx`; expect fail, implement client/mutation using generated type and existing query keys, rerun to pass.
- [ ] Proposed commit: `feat(opportunities): connect draft q-gate mutation and cache`.

### Task 6: Inline detail form — test 6

**Files:** new `opportunity-q-gate-editor.tsx`; `opportunity-detail.tsx`; `opportunity-detail.test.tsx`; `th.json`, `en.json`.

- [ ] Add `OpportunityDetail_DraftUpdate_SaveThenEnablesQualify`: render incomplete Draft with update+transition permissions, fill scope/action pair, save, assert mutation body/version, updated guidance and Qualify enabled; test visible labels/disabled pending submit.
- [ ] Run `npm --prefix frontend run test -- opportunity-detail.test.tsx`; expect fail, implement inline editor on existing dynamic `[id]` detail (no new route), scoped visibility, schema and localized messages; rerun to pass.
- [ ] Proposed commit: `feat(opportunities): complete draft q-gate in detail`.

### Task 7: Browser journey and verification — test 7

**Files:** new `frontend/e2e/opportunity-draft-q-gate.spec.ts`; new verification record; `docs/README.md`.

- [ ] Add one Playwright happy path: login fixture → create incomplete Draft → open detail → fill/save Q-gate → Qualify → assert Qualified badge and refresh-persistent state at 320px. No new negative scenarios.
- [ ] Run targeted Playwright command and record exit code; run `dotnet build backend/TanErp.slnx`, `dotnet test backend/TanErp.slnx`, `npm --prefix frontend run verify`, `npm run test:fixtures`, `git diff --check`; record actual results in verification doc and link it from `docs/README.md`.
- [ ] If any gate or targeted E2E is not run/passing, report incomplete evidence rather than claiming Pilot Passed. Proposed commit: `test(opportunities): verify draft q-gate journey`.

## Definition of Success

1. User can repair an incomplete Draft using only four Q-gate fields and immediately Qualify through the existing action; Qualified detail/list survives refresh.
2. PATCH obeys exact headers/body/ETag, organization scope, permission, replay and atomic privacy-safe audit contract; no stage history until Qualify; no migration.
3. Exactly seven **new happy-path test points** pass, prerequisite Qualification E2E has actual run evidence, existing regression/build/lint/check:api gates pass, `th`/`en` parity holds. Verification record names tested SHA, commands, exit codes and evidence. Status can be **Pilot Passed** only; Production requires deferred hardening.

## Deferred Scope

- Generic Opportunity edit (`title`, customer/site, budget, owner/branch), edit of Qualified/closed records, autosave, bulk edit, Survey and all stages beyond Qualified.
- New negative-path automated tests: permission denial, cross-org isolation, malformed/extra fields, missing If-Match, stale/concurrent writes, idempotency conflict, audit redaction failure, Q-gate invalid pair. These behaviors remain fail-closed contract requirements and are mandatory hardening before Production; omission from the seven new tests does **not** waive them.
- Real device/assistive-technology acceptance and broader accessibility/security/concurrency QA beyond the targeted positive 320px journey.
