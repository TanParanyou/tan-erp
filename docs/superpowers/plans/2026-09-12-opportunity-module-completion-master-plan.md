# Opportunity Module Completion Master Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** วางงานให้ Opportunity ซึ่งเป็นส่วนของ CRM ใช้งานได้ตลอดวงจร Draft → Qualified → Surveying → Estimating → Proposed → Won/Lost/Cancelled → Reopen พร้อมภาพงานจริงที่แนบและเปิดดูย้อนหลังตามช่วงงาน โดยส่งมอบเป็น vertical slices ที่ตรวจรับแยกกันได้

**Architecture:** Opportunity aggregate เป็นเจ้าของข้อมูลการขาย, stage history และ reference ของภาพงานที่แนบใน CRM; binary อยู่ File Service ไม่อยู่ PostgreSQL. EF Core ทำ conditional mutation, idempotency, audit และ append-only history ใน transaction เดียว. Survey, Estimation และ Commercial ส่ง business outcome/หลักฐานผ่าน application integration contract ที่ระบุชัด; CRM ตรวจ scope/guard แล้วเปลี่ยน stage และแสดงลิงก์หลักฐาน ไม่คัดลอกไฟล์หรือเป็นเจ้าของ lifecycle ของโมดูลอื่น. Frontend ใช้ route `[id]`, generated OpenAPI types, central ApiClient และ TanStack Query เดิม.

**Tech Stack:** .NET SDK 10.0.400, ASP.NET Core/EF Core 10.0.11, PostgreSQL 17, Next.js 16.3.4, React 19.2.8, TypeScript, TanStack Query, Vitest, Playwright (อ้างอิง [verification ล่าสุด](../../05-engineering/opportunity-qualification-verification.md))

## Global Constraints

- นี่คือ **master plan ของ Opportunity เท่านั้น** ไม่ใช่คำสั่งเปิด implement ทั้ง CRM; Customer, Contact, Site, Site Survey, Estimation, Commercial และ Project เป็นคนละ ownership boundary ตาม [module boundaries](../../02-architecture/module-boundaries.md). ทุก slice หลัง Qualification ยังต้องมี implementation task อนุมัติแยก; `AGENTS.md` ยังจำกัด boundary ที่ Qualification.
- Authoritative rules: [flow](../../01-business/crm-site-survey-flow.md), [governance](../../01-business/crm-site-survey-governance.md), [field catalog](../../01-business/crm-site-survey-field-catalog.md), [API](../../03-contracts/crm-site-survey-api-contract.md), [errors](../../03-contracts/error-contract.md), [permissions](../../03-contracts/permission-catalog.md), [data](../../04-data/crm-site-survey-data-contract.md), [UAT](../../05-engineering/crm-site-survey-uat-scenarios.md), [DoD](../../05-engineering/definition-of-done.md), `CONTEXT.md`, `AGENTS.md`, `design.md`. Plan นี้จัดลำดับงาน; durable policy decision ไป `docs/adr/` และแก้กฎในเอกสาร authoritative ไม่สร้างสำเนา.
- Baseline: Opportunity create/list/detail และ Draft → Qualified ผ่าน Pilot ที่ commit `4a27e2931f25ac642d4aa287834791a34160cb1c`; Playwright spec มีแล้ว แต่ verification ไม่แสดงผล **รัน** E2E. ต้องรัน prerequisite E2E ก่อนเริ่มงานต่อ.
- บันทึกผลที่เพิ่มภายหลัง ได้แก่ [Draft Q-gate](../../05-engineering/opportunity-draft-q-gate-verification.md), [Outcome](../../05-engineering/opportunity-outcome-verification.md) และ [Hardening](../../05-engineering/opportunity-hardening-verification.md) เป็นหลักฐานเฉพาะ scope ที่ทดสอบเดิม; **ไม่ครอบคลุม work images Slice 1A**. ห้ามนำคำว่า Production-ready จากบันทึกเดิมมาอ้างกับความต้องการแนบภาพใหม่โดยไม่ re-verify.
- Preserve dirty worktree โดยเฉพาะ Opportunity UI/tests/messages และ field catalog; ตรวจ diff ก่อนแตะไฟล์ซ้อน ห้ามทับ user changes. ไม่มี implementation code, tests, migration หรือ commit ในรอบเขียนแผนนี้.
- Backend security authority: PostgreSQL permission + organization/branch/own scope, Firebase identity only, out-of-scope resource เป็น 404; EF Core owns writes/transactions, no generic repository, no raw SQL write. HTTP error เป็น localized RFC 9457 Problem Details พร้อม stable code/traceId.
- UI ใช้ Thai default + English key parity, generated DTO, central ApiClient, TanStack Query, strict TypeScript, Tailwind semantic tokens, 0px radius, keyboard focus, ≥44px touch target, double-submit lock, 320px/200% zoom; reuse existing primitives before new global abstraction.
- ความสามารถแนบภาพงานหลายภาพเป็น [Vertical Slice แยก](2026-09-13-opportunity-work-images-vertical-slice.md); ใช้ contract, file map, tests, privacy/reuse rules และ Definition of Success จากแผนนั้น ไม่ขยายรายละเอียดซ้ำใน master plan.
- User preference: เพิ่ม tests ใหม่เฉพาะ **happy-path** และไม่สร้าง test จำนวนมากต่อ slice. ทุก slice ยังต้องรัน existing regression/build/lint/check:api; negative/security/concurrency automated proof เป็น hardening gate ก่อน Production. ผ่าน happy-path อย่างเดียวรายงานได้เพียง Pilot ไม่ใช่ full DoD/Production.

---

## Scope and Delivery Map

| Slice | Journey ส่งมอบ | สถานะ/Dependency | New happy-path tests | Proposed commits (ตอน implement เท่านั้น) |
| --- | --- | --- | ---: | --- |
| 0 Baseline | Create/list/detail Draft → Qualify + history/audit | Implemented, Pilot verification; รัน prerequisite E2E จริงก่อนต่อ | 0 | ไม่มี |
| 1 Draft repair | เปิด Draft ที่ Q gate ไม่ครบ → เติม scope/work type/next action → save → Qualify | [detailed slice plan](2026-09-12-opportunity-draft-q-gate-completion.md); reconcile field-limit conflict ก่อน code | 7 | `docs(opportunities): freeze draft edit contract`; `feat(opportunities): save draft q-gate`; `test(opportunities): verify draft repair journey` |
| 1A Work images | เลือกหลายภาพในครั้งเดียวทุก Open stage → Save ครั้งเดียวแล้วผูกทั้งชุด → เพิ่มชุดใหม่ได้ → แกลเลอรีหลายภาพแยกตาม stage; Closed ดูย้อนหลัง | [detailed image slice plan](2026-09-13-opportunity-work-images-vertical-slice.md); File Service/retention approval | 5 | `docs(opportunities): freeze work image contract`; `feat(files): add reusable verified image flow`; `feat(opportunities): attach stage-tagged work images`; `test(opportunities): verify work images journey` |
| 2 Open record maintenance | เปิด Qualified/Open → แก้ฟิลด์การขายและ primary Site; เปลี่ยน owner ตาม scope → เห็น audit/version ใหม่ | Opportunity-owned; ต้อง freeze editable field allowlist/owner policy | 5 | `docs(opportunities): freeze open edit and ownership rules`; `feat(opportunities): update open record`; `feat(opportunities): reassign owner`; `test(opportunities): verify open maintenance` |
| 3 Outcome & reopen | เปิด record → ปิด Lost/Cancelled พร้อม reason → ผู้มีสิทธิ์ Reopen พร้อม reason → เห็น history ทั้งหมด | ต้อง freeze allowed source/target, reason catalog, reopening target | 6 | `docs(opportunities): freeze close and reopen policy`; `feat(opportunities): close with outcome`; `feat(opportunities): reopen closed record`; `test(opportunities): verify outcome journey` |
| 4 Survey/Estimate handoff | Qualified + Site → Survey appointment → Surveying; Ready revision หรือ approved bypass → Estimating | ต้องมี Survey contract/implementation และ Estimation evidence policy | 4 เมื่อ dependency พร้อม | `docs(opportunities): freeze survey estimate handoff`; `feat(opportunities): consume survey milestone`; `feat(opportunities): consume estimate readiness`; `test(opportunities): verify survey estimate handoff` |
| 5 Commercial outcome | Official Estimate/Quotation flow → Proposed → accepted Quotation → Won พร้อม reason/reference | ต้องมี Commercial contract/implementation; ห้าม CRM client PATCH Won | 4 เมื่อ dependency พร้อม | `docs(opportunities): freeze commercial handoff`; `feat(opportunities): consume quotation outcomes`; `test(opportunities): verify proposed won journey` |
| 6 Module hardening | End-to-end permissions/scope/concurrency/audit/privacy/UX across all stages | Mandatory for Production; deliberately not part of happy-path-only Pilot preference | Risk-based negative tests, count decided at gate | `test(opportunities): harden module lifecycle`; `docs(opportunities): record production verification` |

Slice 1–3 ส่งมอบได้โดยไม่รอโมดูลธุรกิจอื่น แต่ Slice 1A ต้องมี File Service ที่ตรวจสอบได้; Slice 4–5 เป็น **integration-gated** และห้ามถือว่า “ครบ Opportunity” ในเชิงใช้งานจริงจน upstream journeys ทำงาน. Slice 6 เป็น Production gate ที่ข้ามไม่ได้ถ้าจะอ้างว่า full DoD. ตัวเลข tests ใหม่ในตารางรวม 31 happy-path points **กระจายหลาย slices** ไม่รันทั้งหมดเพื่อเริ่ม Slice 1; ใช้ targeted tests ขณะพัฒนาและ full regressions ที่ exit gate.

## Decision Gates Before Freezing Each Contract

1. **Field limit conflict:** Field catalog กำหนด `scopeSummary ≤2,000`, `sourceCode ≤50`; form ปัจจุบันใช้ 1,000/100 ตามลำดับ. Sales Owner/API Owner ต้องยืนยันตัวเลขหนึ่งชุดและแก้ catalog, validation และ [detailed Slice 1 plan](2026-09-12-opportunity-draft-q-gate-completion.md) ให้ตรงก่อน implement. ห้ามเลือกค่าจากโค้ดโดยอัตโนมัติ.
2. **Open edit allowlist:** Governance ระบุ owner/branch/budget/outcome audit แต่ยังไม่ freeze ว่าแก้ `customerId`, `branchId`, `title`, `primarySiteId`, `sourceCode`, `expectedBudget`, `targetDecisionDate`, `nextAction` ได้ใน stage ใด. เสนอ: immutable `organizationId/customerId/code/createdBy`, mutable business fields เฉพาะ Open, owner reassignment เป็น command แยก; Sales Owner ต้องอนุมัติ.
3. **Stage transition matrix:** Flow ให้ลำดับหลักและเหตุผลปิดงาน แต่ diagram ยังไม่ระบุ `lost`/`cancelled` จากแต่ละ Open stage หรือ Reopen กลับ stage ไหน. Freeze allowlist เป็นตาราง source → target + guard + permission ใน flow/API/ADR ก่อน Slice 3; ไม่สร้าง generic transition ที่เดาเอง.
4. **Reason code/privacy:** กำหนด controlled `outcomeReasonCode`, free-text `outcomeNote` เฉพาะเมื่อจำเป็น, ความยาว, retention/redaction, localization และว่ากลับมา Open แล้วยังคง reason เดิมเป็น historical-only หรือเป็น current field. Audit ต้องไม่คัดลอก note/PII.
5. **Cross-module source of truth:** `Surveying` ต้องเกิดหลัง Survey appointment ที่ valid; `Estimating` หลัง Ready Survey Revision หรือ policy-approved no-survey reason; `Proposed` จาก Quotation use case; `Won` จาก accepted quotation. ระบุ event/command owner, reference ID, idempotency key, failure/retry/compensation และ ordering ใน ADR ก่อน Slice 4–5. CRM UI ห้ามกด stage เหล่านี้เอง.
6. **Work images policy:** ยืนยันความต้องการหลายภาพต่อ Opportunity/stage และต่อหนึ่ง Save แล้ว; policy ที่ยังไม่ตัดสินและวิธีส่งมอบอยู่ใน [แผนภาพงานแยก](2026-09-13-opportunity-work-images-vertical-slice.md).

## Exact Common Contract (reuse across slices)

Existing `GET /api/v1/opportunities?search=&customerId=&stage=&limit=&cursor=` and `GET /api/v1/opportunities/{id}` remain read contracts; return `OpportunityResponse` + ETag from `rowVersion`. Add `GET /api/v1/opportunities/{id}/stage-history?limit=25&cursor=` for authorized `opportunities.read`, response `{items:[{id,fromStage,toStage,reasonCode,actorUserId,occurredAtUtc,policyVersion}],nextCursor}` sorted `(occurredAtUtc DESC,id DESC)`; omit free-text note from default history response. A missing/out-of-scope opportunity returns 404. Authoritative API document must freeze this shape before implementation.

| Mutation | Request / source | Permission | Success & atomic effects |
| --- | --- | --- | --- |
| Slice 1 Draft PATCH | `PATCH /api/v1/opportunities/{id}` with four-field body from [detailed plan](2026-09-12-opportunity-draft-q-gate-completion.md), quoted UUID `If-Match`, `Idempotency-Key` | `opportunities.update` | 200 `OpportunityResponse`, new ETag, changed-field audit, no stage history |
| Slice 2 Open PATCH | Same endpoint with **stage-specific allowlisted** business fields and ETag; body schema frozen in gate 2, never accepts `stage`, ownership, reason | `opportunities.update` | 200/new ETag + metadata-only audit; no history |
| Slice 2 Reassign | `POST /api/v1/opportunities/{id}/owner-changes` body `{targetOwnerUserId,expectedVersion}`, idempotency header | `opportunities.update` + approved scope policy | 200/new ETag; target active member of branch; `opportunity.owner-changed` audit with IDs only |
| Slice 3 Close/reopen | Existing `POST /api/v1/opportunities/{id}/stage-transitions` extends body to `{targetStage,expectedVersion,reasonCode,note?}`; exact allowed targets from gate 3 | `opportunities.transition` | 200/new ETag; stage/history/audit/idempotency atomic; confirmation modal in UI |
| Slice 4–5 integrations | Application commands carrying `sourceModule`, `sourceResourceId`, `sourceVersion`, `sourceEventId`, `opportunityId`, `targetStage`, `occurredAtUtc`; source owner invokes through trusted integration, not direct browser endpoint | service authority + organization/resource scope | one monotonic transition/history/audit per source event; duplicate event replay returns prior result |

All mutating HTTP requests use Firebase bearer token, `X-Membership-Id`, `Idempotency-Key` (16–128 chars), `Accept-Language`; conditional edits also require quoted `If-Match` matching current `rowVersion`. Request body must not accept organization/actor/branch overrides; missing/malformed required context uses baseline errors. Replays are checked before version/stage checks. Version mismatch is `409 OPPORTUNITY_VERSION_CONFLICT`, absent/invalid `If-Match` is `428 IF_MATCH_REQUIRED`, unavailable stage is `409 OPPORTUNITY_INVALID_TRANSITION`, missing required stage fields are `422 OPPORTUNITY_FIELD_REQUIRED`, out-of-scope is `404 RESOURCE_NOT_FOUND`, reused key/different payload is `409 IDEMPOTENCY_KEY_REUSED`. Any new error code must be added once in authoritative error contract, mapper and both `.resx` files before use.

## File Ownership Map

| Layer | Current files to extend | New focused files per slice |
| --- | --- | --- |
| Domain | `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs`, `OpportunityValues.cs`, `OpportunityStageHistory.cs` | `OpportunityTransitionPolicy.cs` only after matrix approval; new migration for outcome fields only if policy requires current columns; image files listed in [Slice 1A](2026-09-13-opportunity-work-images-vertical-slice.md) |
| Application | `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs`, `OpportunityProjection.cs` | feature folders `UpdateDraftQGate/`, `UpdateOpenOpportunity/`, `ReassignOpportunityOwner/`, `CloseOpportunity/`, `ReopenOpportunity/`, `GetOpportunityStageHistory/`, `ApplyOpportunityMilestone/`; one command+handler each |
| Infrastructure/API | `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`, `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`, `backend/src/TanErp.Api/Program.cs` | request contracts under `backend/src/TanErp.Api/Contracts/Crm/Opportunities/`; use existing EF migration convention only when schema changes |
| Frontend | `frontend/src/features/opportunities/api/opportunity-queries.ts`, `frontend/src/features/opportunities/components/opportunity-detail.tsx`, `opportunity-list.tsx`, `frontend/src/lib/api/api-client.ts`, `frontend/src/app/[locale]/(erp)/opportunities/[id]/page.tsx` | focused edit, owner, outcome and timeline components under `frontend/src/features/opportunities/components/`; generated DTO in `frontend/src/generated/api/tan-erp.v1.ts`; copy in `frontend/src/messages/{th,en}.json` |
| Documentation/proof | [API](../../03-contracts/crm-site-survey-api-contract.md), [UAT](../../05-engineering/crm-site-survey-uat-scenarios.md), [field catalog](../../01-business/crm-site-survey-field-catalog.md), [flow](../../01-business/crm-site-survey-flow.md), `AGENTS.md`, `docs/README.md` | one `docs/05-engineering/opportunity-<slice>-verification.md` per approved slice; ADR for genuinely new lifecycle/integration policy |

## Execution Tasks and Checkpoints

### Task A: Baseline proof and Slice 1 (Draft repair)

**Files:** exactly those in [detailed Slice 1 plan](2026-09-12-opportunity-draft-q-gate-completion.md). **Produces:** `UpdateDraftQGateCommand`, Draft-only PATCH, inline Q-gate editor.

- [ ] Run existing Qualification Playwright spec with configured fixture; record command/result. Resolve field-limit gate 1 in authoritative docs and detailed plan.
- [ ] Follow Tasks 1–7 in detailed plan with red → green per named test, 7 new happy-path tests, then full regression/build/lint/check:api and Pilot verification record. Do not commit during this planning turn.

### Task A1: Stage-tagged work images (Slice 1A)

**Scope/contract/files/tests:** [Opportunity Work Images Vertical Slice Plan](2026-09-13-opportunity-work-images-vertical-slice.md).

- [ ] ดำเนินงานตามแผนแยกหลังอนุมัติ implementation task; บันทึกผลตรวจสอบเฉพาะ Slice 1A แล้วค่อยเดิน Slice 2.

### Task B: Open edit + ownership (Slice 2)

**Files:** `Opportunity.cs`, `IOpportunityStore.cs`, `OpportunityStore.cs`, `OpportunitiesController.cs`, new `UpdateOpenOpportunity/`, `ReassignOpportunityOwner/`, API request contracts, `api-client.ts`, `opportunity-queries.ts`, focused UI components and paired messages. **Produces:** conditional Open PATCH and separate owner-change command.

- [ ] Freeze gate 2; add five success tests named `EditOpenOpportunity_ValidFields_RotatesVersion`, `ReassignOwner_ActiveSameBranch_ChangesOwner`, `UpdateOpenOpportunity_ValidRequest_PersistsAudit`, `PatchOpenOpportunity_ValidRequest_ReturnsETag`, `OpportunityDetail_EditAndReassign_RefreshesDetail` in existing unit/integration/frontend suites.
- [ ] Run each targeted test red → implement minimal Domain/handler/store/API/UI path → run green. Verify audit contains before/after IDs for owner, but no title/scope/notes; list/detail query cache reflects owner and changed fields. Full regression and Pilot record.

### Task C: Close/reopen + stage history (Slice 3)

**Files:** `Opportunity.cs`, `OpportunityStageHistory.cs`, `OpportunityStore.cs`, `OpportunitiesController.cs`, new `CloseOpportunity/`, `ReopenOpportunity/`, `GetOpportunityStageHistory/`, request/response contracts, `opportunity-detail.tsx`, focused outcome/history components, messages. **Produces:** approved close/reopen transition matrix and read-only history timeline.

- [ ] Freeze gates 3–4; add six success tests named `CloseLost_ValidReason_AppendsHistory`, `CloseCancelled_ValidReason_AppendsHistory`, `Reopen_ApprovedTarget_AppendsHistory`, `CloseReopen_ValidCommands_PersistAtomicAudit`, `StageHistory_ValidRequest_ReturnsOrderedItems`, `OpportunityDetail_CloseThenReopen_ShowsTimeline`.
- [ ] Run targeted tests red → implement minimal command/store/API/UI path → green. Close/Cancel use safety confirmation and disable while pending; reopen requires reason; closed business edit hidden and rejected by backend. Verify append-only history, audit without free-text note and full regression; record Pilot result.

### Task D: Survey/Estimate integration (Slice 4; do not start until upstream ready)

**Files:** new `ApplyOpportunityMilestone/` command+handler, `Opportunity.cs`, `OpportunityStore.cs`, integration adapter at owning module boundary, stage-history/audit path; detail UI only displays source references and next action. **Produces:** Qualified → Surveying and Surveying → Estimating from trusted evidence.

- [ ] Freeze gate 5; add four success tests named `SurveyAppointmentCreated_ValidScope_EntersSurveying`, `SurveyMilestoneReplay_SameEvent_NoDuplicateHistory`, `ReadySurveyRevision_ValidReference_EntersEstimating`, `OpportunityDetail_SurveyingThenEstimating_ShowsEvidenceLink`.
- [ ] Run red → implement trusted cross-module event/command with source IDs, duplicate handling and atomic CRM mutation → green. No direct CRM UI transition button. Verify upstream+CRM suites and record Pilot evidence.

### Task E: Quotation/acceptance integration (Slice 5; do not start until Commercial ready)

**Files:** `ApplyOpportunityMilestone/`, `Opportunity.cs`, `OpportunityStore.cs`, Commercial integration adapter, stage-history/audit path and detail UI evidence links. **Produces:** Estimating → Proposed → Won only from trusted quotation/acceptance outcome.

- [ ] Add four success tests named `QuotationIssued_ValidEstimate_EntersProposed`, `QuotationAccepted_ValidAcceptance_EntersWon`, `AcceptanceReplay_SameEvent_NoDuplicateHistory`, `OpportunityDetail_ProposedThenWon_ShowsSourceReference`.
- [ ] Run red → implement source-bound idempotent commands and stage/history/audit transaction → green. No generic PATCH Won; require approved outcome reason/reference. Verify CRM+Commercial regressions, record Pilot evidence.

### Task F: Module hardening and Production decision (Slice 6)

**Files:** existing Opportunity test suites, `frontend/e2e/` scenarios, [UAT](../../05-engineering/crm-site-survey-uat-scenarios.md), [DoD](../../05-engineering/definition-of-done.md), release/runbook documents. **Produces:** evidence-backed Production readiness decision, not additional business behavior.

- [ ] Before Production, test 403 permission denial, 404 cross-org/branch/file access, stale/parallel ETag, idempotency key conflict/replay, invalid stage/missing reason, unverified/wrong-parent image, signed-URL expiry, PII/EXIF-free audit/error/log, retention/soft detach, out-of-order cross-module events, browser keyboard/320px/200%/i18n and rollback/forward-fix policy. Count determined by risk, not the happy-path cap.
- [ ] Run `dotnet build backend/TanErp.slnx`, `dotnet test backend/TanErp.slnx`, `npm --prefix frontend run check:api`, `npm --prefix frontend run verify`, `npm run test:fixtures`, targeted E2E and `git diff --check`; record exact command/exit code/tested SHA. Only then decide full DoD/Production with owners.

## Definition of Success

**Pilot per slice:** user journey in that row works end-to-end, exact contract and atomic audit/history behave as specified, new happy-path tests and all existing verification gates pass with recorded evidence, Thai/English parity holds. A slice not run through browser is not called E2E-verified.

**Complete Opportunity module:** all slices 1, 1A and 2–5 work with real upstream journeys (not stubs), stage/owner/outcome/history can be viewed correctly, cross-module source references are traceable, and Slice 6 re-verifies new image behavior against [DoD](../../05-engineering/definition-of-done.md) with explicit Product/Sales, Security and Technical owner acceptance. Until then describe status precisely as partial/Pilot, not “ครบแล้ว”.
เกณฑ์สำเร็จของภาพงานและหลักฐานที่ต้องตรวจแยกอยู่ใน [Slice 1A](2026-09-13-opportunity-work-images-vertical-slice.md).

## Deferred Outside Opportunity

Customer/Contact/Site lifecycle beyond existing features; Survey forms/revisions/evidence, Official Estimate calculations, Quotation creation/approval, Project handover, Procurement/Inventory/Production/MRP. This plan defines only **Opportunity-facing integration contracts** for those owners and never implements their aggregates. Deep negative/security/concurrency tests are deferred from incremental Pilot slices **but mandatory in Slice 6 before Production**.
