# Opportunity Qualification Vertical Slice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ส่งมอบ Pilot ที่ผู้ใช้มีสิทธิ์เปลี่ยน Opportunity Draft ซึ่งข้อมูลผ่าน Q gate เป็น Qualified ผ่านหน้า detail ได้ครบ UI → API → PostgreSQL พร้อม happy-path proof ที่สั้น–ส่งมอบได้เร็ว

**Architecture:** เพิ่ม business transition เดียว `draft → qualified` บน Opportunity aggregate โดย Application handler ตรวจ permission และสร้าง deterministic idempotency hashes ส่วน EF Core store ตรวจ current organization/resource state แล้วเขียน Opportunity, `opportunity_stage_history`, idempotency record และ audit event ใน transaction เดียว. Frontend ใช้ generated OpenAPI type, central `ApiClient`, TanStack Query mutation และ `ConfirmationModal` เดิมเพื่อส่ง transition จาก Opportunity detail โดยรักษา retry intent เดิมจน payload หรือ row version เปลี่ยน.

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core `10.0.11`, Npgsql EF Core `10.0.3`, PostgreSQL `17-alpine`, Next.js `16.3.4`, React `19.2.8`, TypeScript `7.0.2`, TanStack Query `5.102.8`, Vitest `5.0.0`, Playwright `1.63.0`

## Global Constraints

- เริ่ม implementation หลังผู้ใช้อนุมัติแผนนี้เท่านั้น; แผนนี้อ้าง baseline ที่ verify ผ่าน commit `b5f61a2ab289f4f1fa9ac48d33ec0df9f2c247cc` ตาม `docs/05-engineering/opportunity-site-verification.md`
- ก่อนแก้โค้ดให้รักษา user changes ที่มีอยู่ใน `frontend/next-env.d.ts` และ `frontend/src/components/ui/TableAction.tsx`; ไฟล์ทั้งสองไม่อยู่ใน scope นี้
- Slice นี้ปิดเฉพาะส่วน Qualify ของ `UAT-CRM-004`: `draft → qualified`; stage อื่น, generic Opportunity edit, owner reassignment และ close/reopen ยัง deferred
- เพิ่ม automated test ใหม่เพียง 7 จุดและทดสอบเฉพาะ successful journey: Domain, Persistence, Application handler, HTTP/OpenAPI, Frontend client/cache, Component UI และ Playwright E2E อย่างละหนึ่งจุด
- Error/security/concurrency contracts ยังต้อง implement แบบ fail-closed ตามเอกสาร แต่ negative-path tests ใหม่ถูก deferred; ผลส่งมอบจึงใช้สถานะ `Pilot Passed` และห้ามอ้างว่า Definition of Done เต็มหรือพร้อม Production
- Exact stage vocabulary ต้องตรง authoritative flow: `draft`, `qualified`, `surveying`, `estimating`, `proposed`, `won`, `lost`, `cancelled`; แก้ constants เดิมที่ใช้ `estimation`/`proposal`
- Transition ใช้ permission `opportunities.transition` และ Organization scope ที่ runtime รองรับอยู่; Backend เป็น security authority และ resource นอก Organization คืน `404 RESOURCE_NOT_FOUND`
- Qualify gate ต้องตรวจ Active Customer, Active Branch, Active Owner Membership ใน Branch เดิม, non-empty `scopeSummary`, อย่างน้อยหนึ่ง valid `workType`, และทั้ง `nextActionAtUtc` กับ `nextActionNote`
- `POST /api/v1/opportunities/{id}/stage-transitions` ต้องมี `Idempotency-Key` 16–128 characters และ payload ต้องมี `targetStage` กับ `expectedVersion`
- Key เดิม + canonical payload เดิมคืนผลเดิม; key เดิม + payload ต่างคืน `409 IDEMPOTENCY_KEY_REUSED`; request คนละ key ที่แข่งด้วย version เดียวกันต้องมีผู้ชนะหนึ่งรายและอีกคำขอคืน `409 OPPORTUNITY_VERSION_CONFLICT`
- Transition สำเร็จต้อง rotate `rowVersion` และเขียน Opportunity + append-only stage history + `opportunity.stage-changed` audit + idempotency record แบบ atomic ด้วย controlled `IClock`
- Audit/history เก็บเฉพาะ IDs, stages, actor, time, policy version และ trace ID; ห้ามคัดลอก title, scope summary, next-action note, Customer/Site data หรือ PII ลง audit/log/problem details
- HTTP errors ใช้ RFC 9457 Problem Details พร้อม stable code, localized `.resx`, `traceId` และ Thai default/English alternative
- Frontend API DTO มาจาก `frontend/src/generated/api/tan-erp.v1.ts` เท่านั้น; ห้าม handwritten request/response DTO และห้าม component `fetch`
- Frontend mutation ใช้ TanStack Query, compare error ด้วย `ApiError.code`/`instanceof`, double-submit protection และคง idempotency key เดิมเฉพาะ retry ของ intent + `expectedVersion` เดิม
- UI ใช้ `Button`, `Badge`, `ConfirmationModal`, permission helper, `MonoSpinner`, query keys และ semantic tokens เดิม; ไม่สร้าง global component ใหม่ใน Slice นี้
- ข้อความ UI ทุกข้อความต้องอยู่ใน `frontend/src/messages/th.json` และ `en.json` แบบ key parity; ไม่มี hardcoded user-facing copy
- Tailwind-first, 0px radius, semantic controls, visible focus, target ≥44px, `aria-live="polite"`, keyboard/320px/200% zoom และ reduced-motion behavior ต้องคงมาตรฐานเดิม
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, Generic Repository, role-name check, direct frontend database access หรือ raw SQL write
- ทุก Task ใช้ TDD และมี proposed commit `type(scope): description`; ตอนจัดทำแผนนี้ห้ามสร้าง commit

---

## Acceptance Journey

```text
Login → เลือก Active Membership ที่มี opportunities.transition
  → สร้าง Opportunity Draft ที่มี Scope Summary + Work Types + Next Action
  → เปิด Opportunity detail → ยืนยัน Qualify
  → POST stage-transitions ด้วย Idempotency-Key + expectedVersion
  → Opportunity เป็น Qualified และ ETag/rowVersion เปลี่ยน
  → list/detail cache แสดง Qualified
  → PostgreSQL มี stage history และ privacy-safe audit อย่างละหนึ่งรายการ
```

Pilot นี้พิสูจน์เฉพาะ sequence ด้านบนด้วยข้อมูลที่ผ่าน Q gate และสิทธิ์ถูกต้อง. Permission denial, Org B isolation, missing gate, illegal transition, stale version, idempotency conflict, concurrency และ ambiguous retry ยังคงเป็น behavior ที่ต้อง implement แต่เลื่อน automated proof ไป Hardening slice.

## Exact Slice Contract

### Request

```http
POST /api/v1/opportunities/{opportunityId}/stage-transitions
Authorization: Bearer <Firebase ID token>
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Accept-Language: th | en
Content-Type: application/json
```

```json
{
  "targetStage": "qualified",
  "expectedVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d11"
}
```

- Slice นี้รับ `targetStage` เพียง `qualified`; ไม่รับ `organizationId`, `branchId`, `ownerUserId`, `fromStage`, actor, `reasonCode` หรือ free-text `note`
- `expectedVersion` คือ UUID จาก `OpportunityResponse.rowVersion`; UUID ว่างคืน `422 OPPORTUNITY_FIELD_REQUIRED`
- Missing/invalid JSON หรือ header context คืน `400 REQUEST_VALIDATION_FAILED`, `400 MEMBERSHIP_CONTEXT_REQUIRED`, `400 IDEMPOTENCY_KEY_REQUIRED` หรือ `400 IDEMPOTENCY_KEY_INVALID` ตาม baseline

### Success

```http
HTTP/1.1 200 OK
ETag: "019a3cf8-96f0-7c9f-b207-93aa818f4d12"
Content-Type: application/json
```

Response ใช้ `OpportunityResponse` เดิมทุก field โดยเปลี่ยนเฉพาะ:

```json
{
  "stage": "qualified",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d12"
}
```

ค่าฟิลด์อื่นต้องเท่ากับ resource ก่อน transition. Retry key/payload เดิมคืน `200`, body และ ETag เดียวกับ transition แรก และไม่เพิ่ม history/audit.

### Error Matrix

| HTTP | Stable code | Condition |
| ---: | --- | --- |
| 400 | `REQUEST_VALIDATION_FAILED` | malformed body/route binding |
| 400 | `MEMBERSHIP_CONTEXT_REQUIRED` | ไม่มี/ผิดรูปแบบ `X-Membership-Id` |
| 400 | `IDEMPOTENCY_KEY_REQUIRED` / `IDEMPOTENCY_KEY_INVALID` | ไม่มี key หรือความยาวนอก 16–128 |
| 401 | `AUTHENTICATION_REQUIRED` / `AUTHENTICATION_INVALID` | ไม่มี/ใช้ token ไม่ได้ |
| 403 | `ACTIVE_MEMBERSHIP_REQUIRED` / `PERMISSION_DENIED` | membership หมดอายุ/ไม่มี `opportunities.transition` |
| 404 | `RESOURCE_NOT_FOUND` | Opportunity, Customer, Branch หรือ Owner Membership ไม่อยู่ Organization/scope ที่อนุญาต |
| 409 | `IDEMPOTENCY_KEY_REUSED` | key เดิมแต่ canonical payload ต่าง |
| 409 | `OPPORTUNITY_VERSION_CONFLICT` | `expectedVersion` ไม่ตรง current row version |
| 409 | `OPPORTUNITY_INVALID_TRANSITION` | current stage ไม่ใช่ `draft` หรือ target ไม่ใช่ `qualified` |
| 409 | `CUSTOMER_INVALID_STATE` | Customer ปัจจุบันไม่ Active |
| 422 | `OPPORTUNITY_FIELD_REQUIRED` | Q gate ขาด `scopeSummary`, work type, `nextActionAtUtc` หรือ `nextActionNote` |
| 422 | `ACTIVE_BRANCH_REQUIRED` | selected membership ไม่มี Active Branch |

Precedence หลัง authentication/context/permission: idempotency replay/conflict → resource scope → expected version → current state/target → Q gate. Precedence นี้ทำให้ retry ของคำขอที่สำเร็จแต่ client ไม่ได้รับ response ยังคง replay ได้แม้ row version ถูก rotate แล้ว.

### Persistence Contract

สร้างตาราง `crm.opportunity_stage_history`:

| Column | Type | Rule |
| --- | --- | --- |
| `id` | `uuid` | PK |
| `organization_id` | `uuid` | required, FK Organization |
| `opportunity_id` | `uuid` | required, composite FK `(id, organization_id)` |
| `from_stage` / `to_stage` | `varchar(32)` | `draft` / `qualified` สำหรับ Slice นี้ |
| `reason_code` / `note` | nullable text | null สำหรับ qualify; เตรียม schema สำหรับ closed/reopen contract ภายหลัง |
| `actor_user_id` | `uuid` | authenticated PostgreSQL User |
| `occurred_at_utc` | `timestamptz` | จาก `IClock.UtcNow` |
| `policy_version` | `varchar(64)` | `opportunity-stage-v1` |
| `trace_id` | `varchar(128)` | request trace; ไม่เก็บ payload |

Index `(organization_id, opportunity_id, occurred_at_utc, id)`. ไม่มี update/delete use case; FK ใช้ `Restrict` เพื่อรักษาประวัติ.

## Planned File Map

### Documentation

- Modify `AGENTS.md` — เปิด authorized boundary เฉพาะ Opportunity Qualification Slice 3 หลังอนุมัติ
- Modify `docs/03-contracts/crm-site-survey-api-contract.md` — freeze exact Slice 3 request/response/error/retry contract
- Modify `docs/03-contracts/error-contract.md` — เพิ่ม two stable conflict codes และ translations contract
- Modify `docs/05-engineering/crm-site-survey-uat-scenarios.md` — เพิ่ม Slice 3 exit criteria
- Create `docs/05-engineering/opportunity-qualification-verification.md` — หลักฐานหลัง implement
- Modify `docs/README.md` — add verification link beside the plan link already recorded when this plan was authored

### Backend

- Modify `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs` — `Qualify(expectedVersion)` invariant + row-version rotation
- Modify `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityValues.cs` — canonical stage values/policy version
- Create `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityExceptions.cs` — typed qualification/transition/version failures
- Create `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityStageHistory.cs` — append-only history entity
- Modify `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs` — transition-specific store method
- Create `backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityCommand.cs`
- Create `backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityHandler.cs`
- Modify `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs` — history `DbSet`
- Create `backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityStageHistoryConfiguration.cs`
- Modify `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs` — atomic transition/replay/concurrency
- Create `backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.cs`
- Create `backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.Designer.cs`
- Modify `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Create `backend/src/TanErp.Api/Contracts/Crm/Opportunities/TransitionOpportunityStageRequest.cs`
- Modify `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs` — endpoint + ETag
- Modify `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify `backend/src/TanErp.Api/Program.cs` — handler registration
- Modify `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs` — test-only permission assignment
- Modify `contracts/openapi/tan-erp.v1.json` — generated API snapshot

### Backend Tests

- Modify `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs`
- Modify `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityHandlerTests.cs`
- Create `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityQualificationTests.cs`
- Modify `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`
- Modify `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs`

### Frontend

- Modify `frontend/src/generated/api/tan-erp.v1.ts` — regenerate only
- Modify `frontend/src/lib/api/api-client.ts` — generated request alias + transition method
- Modify `frontend/src/features/opportunities/api/opportunity-queries.ts` — qualify mutation/cache policy
- Modify `frontend/src/features/opportunities/api/opportunity-queries.test.tsx`
- Modify `frontend/src/features/opportunities/components/opportunity-detail.tsx` — action, modal, states
- Modify `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`
- Modify `frontend/src/features/opportunities/opportunity-labels.ts` — canonical stage vocabulary
- Modify `frontend/src/features/opportunities/components/opportunity-list.test.tsx` — stage label regression
- Modify `frontend/src/messages/th.json`
- Modify `frontend/src/messages/en.json`
- Create `frontend/e2e/opportunity-qualification.spec.ts`

## Task 1: Freeze the Approved Boundary and Executable Contract

**Files:**
- Modify: `AGENTS.md`
- Modify: `docs/03-contracts/crm-site-survey-api-contract.md`
- Modify: `docs/03-contracts/error-contract.md`
- Modify: `docs/05-engineering/crm-site-survey-uat-scenarios.md`

**Interfaces:**
- Consumes: exact contract and deferred boundary in this plan
- Produces: one authoritative Slice 3 contract used by Backend, OpenAPI, Frontend and E2E

- [ ] **Step 1: Update the authorized implementation boundary**

Replace the prior Current phase sentence with a boundary that allows only Draft → Qualified plus stage history/audit; retain every deferred item listed below.

- [ ] **Step 2: Add the exact HTTP contract**

Copy the Request, Success, Error Matrix, precedence and canonical stage vocabulary from this plan into `crm-site-survey-api-contract.md`; do not broaden the endpoint to other target stages.

- [ ] **Step 3: Add stable errors and Slice 3 UAT gate**

Add `OPPORTUNITY_VERSION_CONFLICT` and `OPPORTUNITY_INVALID_TRANSITION` to `error-contract.md`. Add Pilot exit criteria for the successful subset of `UAT-CRM-004`, `UAT-UX-001` and `UAT-I18N-001`; list negative/security/concurrency proof as a mandatory Hardening gate before Production.

- [ ] **Step 4: Validate docs and commit**

Run: `rg -n "OPPORTUNITY_VERSION_CONFLICT|OPPORTUNITY_INVALID_TRANSITION|Opportunity Qualification" AGENTS.md docs`

Run: `git diff --check -- AGENTS.md docs/03-contracts docs/05-engineering`

Expected: codes/links appear in authoritative docs and diff check exits `0`.

```bash
git add AGENTS.md docs/03-contracts/crm-site-survey-api-contract.md docs/03-contracts/error-contract.md docs/05-engineering/crm-site-survey-uat-scenarios.md
git commit -m "docs(crm): authorize opportunity qualification slice"
```

## Task 2: Encode Qualification Invariants and Append-only History

**Files:**
- Modify: `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs`
- Modify: `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityValues.cs`
- Create: `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityExceptions.cs`
- Create: `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityStageHistory.cs`
- Modify: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs`

**Interfaces:**
- Consumes: `expectedVersion` and current aggregate fields
- Produces: `Result`-free domain method that either qualifies deterministically or throws a typed domain exception mapped by Application; history constructor with exact fields from Persistence Contract

- [ ] **Step 1: Write one failing successful aggregate test**

Add one successful test:

```csharp
[Fact] public void Qualify_FromDraftWithCompleteGate_SetsQualifiedAndRotatesVersion()
```

Assert `Stage == "qualified"`, old/new `RowVersion` differ, business fields are unchanged, and the canonical set equals the eight accepted values exactly in the same test.

- [ ] **Step 2: Run tests to verify RED**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~OpportunityTests`

Expected: FAIL because `Qualify`, typed exceptions and accepted stage constants do not exist.

- [ ] **Step 3: Implement the minimal domain behavior**

Use constants, not string literals at call sites:

```csharp
public static class OpportunityStage
{
    public const string Draft = "draft";
    public const string Qualified = "qualified";
    public const string Surveying = "surveying";
    public const string Estimating = "estimating";
    public const string Proposed = "proposed";
    public const string Won = "won";
    public const string Lost = "lost";
    public const string Cancelled = "cancelled";
}

public void Qualify(Guid expectedVersion)
{
    if (RowVersion != expectedVersion) throw new OpportunityVersionException();
    if (Stage != OpportunityStage.Draft) throw new OpportunityTransitionException(Stage, OpportunityStage.Qualified);
    if (string.IsNullOrWhiteSpace(ScopeSummary)) throw new OpportunityQualificationException(nameof(ScopeSummary));
    if (WorkTypes.Count == 0 || WorkTypes.Any(workType => !OpportunityWorkType.IsValid(workType)))
        throw new OpportunityQualificationException(nameof(WorkTypes));
    if (!NextActionAtUtc.HasValue) throw new OpportunityQualificationException(nameof(NextActionAtUtc));
    if (string.IsNullOrWhiteSpace(NextActionNote)) throw new OpportunityQualificationException(nameof(NextActionNote));
    Stage = OpportunityStage.Qualified;
    RowVersion = Guid.NewGuid();
}
```

Define `OpportunityQualificationException`, `OpportunityTransitionException` and `OpportunityVersionException` in `OpportunityExceptions.cs`; do not expose their messages as API contract.

- [ ] **Step 4: Implement history entity**

Constructor parameters must be `(id, organizationId, opportunityId, fromStage, toStage, reasonCode, note, actorUserId, occurredAtUtc, policyVersion, traceId)` and properties must have private setters. Reject blank stages/policy version; Slice 3 passes null reason/note.

- [ ] **Step 5: Run GREEN and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~OpportunityTests`

Expected: the new successful qualification test and all pre-existing Opportunity aggregate tests pass.

```bash
git add backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs backend/src/TanErp.Domain/Crm/Opportunities/OpportunityValues.cs backend/src/TanErp.Domain/Crm/Opportunities/OpportunityExceptions.cs backend/src/TanErp.Domain/Crm/Opportunities/OpportunityStageHistory.cs backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs
git commit -m "feat(crm): model opportunity qualification"
```

## Task 3: Persist Stage History and Atomic Transition

**Files:**
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityStageHistoryConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.Designer.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityQualificationTests.cs`

**Interfaces:**
- Consumes: `RequestAccessContext`, `QualifyOpportunityCommand`, SHA-256 key/payload hashes
- Produces: `Task<Result<OpportunityProjection>> QualifyAsync(...)` with atomic resource/history/idempotency/audit behavior

- [ ] **Step 1: Write one failing successful-store test**

Use one integration test that applies the latest migration before exercising the successful transaction:

```csharp
[Fact] public async Task Qualify_WritesOpportunityHistoryAuditAndIdempotencyAtomically()
```

Assert migrated table/index/FK exist, Opportunity becomes Qualified, controlled clock is used, and exactly one history, audit and idempotency row are committed without business text in audit.

- [ ] **Step 2: Run persistence tests to verify RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~OpportunityQualificationTests`

Expected: FAIL because schema and store contract do not exist.

- [ ] **Step 3: Add EF mapping and generate migration**

Run: `dotnet ef migrations add OpportunityQualificationSlice --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api`

Rename only the generated migration timestamp prefix to `20260910100000` in both files and its `[Migration("20260910100000_OpportunityQualificationSlice")]` attribute so the plan's file map stays exact. Inspect generated SQL model: schema/table/columns/index/composite FK exactly match Persistence Contract, delete behavior is Restrict, and migration does not touch Customer/Site tables or unrelated indexes.

- [ ] **Step 4: Implement replay-first atomic store**

Add interface signature:

```csharp
Task<Result<OpportunityProjection>> QualifyAsync(
    RequestAccessContext access,
    QualifyOpportunityCommand command,
    string keyHash,
    string payloadHash,
    CancellationToken cancellationToken = default);
```

Inside the execution strategy transaction: load idempotency replay first using operation `opportunities.qualify`; load Opportunity constrained by `organization_id`; validate active Customer/Branch/owner Membership; call aggregate `Qualify`; add one `OpportunityStageHistory` with policy `opportunity-stage-v1`; add one `opportunity.stage-changed` AuditEvent whose JSON is exactly `{"changedFields":["stage"],"fromStage":"draft","toStage":"qualified"}`; add idempotency record; save/commit. Translate typed domain exceptions and `DbUpdateConcurrencyException` to stable codes; on PostgreSQL `23505` rollback/clear/reload the replay winner.

- [ ] **Step 5: Run GREEN, migration-from-zero and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~OpportunityQualificationTests`

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter FullyQualifiedName~FoundationMigrationTests`

Expected: the new successful store test and existing foundation migration regression pass; migration from empty database reaches latest schema, and down migration removes only history additions.

```bash
git add backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityStageHistoryConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.cs backend/src/TanErp.Infrastructure/Persistence/Migrations/20260910100000_OpportunityQualificationSlice.Designer.cs backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs backend/tests/TanErp.IntegrationTests/Persistence/OpportunityQualificationTests.cs
git commit -m "feat(crm): persist opportunity qualification history"
```

## Task 4: Add the Permission-aware Application Use Case

**Files:**
- Create: `backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityCommand.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityHandler.cs`
- Modify: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityHandlerTests.cs`

**Interfaces:**
- Consumes: Firebase UID, Membership ID, Opportunity ID, `targetStage`, expected version, idempotency key and trace ID
- Produces: permission-checked `Result<OpportunityProjection>` and deterministic hashes for Task 3

- [ ] **Step 1: Extend the fake store and write one failing successful handler test**

Add one successful handler test:

```csharp
[Fact] public async Task Qualify_ValidCommand_UsesTransitionPermissionAndCallsStoreOnce()
```

In the same test assert resolved permission, normalized target, deterministic non-empty key/payload hashes and the exact command passed to the store.

- [ ] **Step 2: Run tests to verify RED**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~OpportunityHandlerTests`

Expected: FAIL because command/handler/store fake method are absent.

- [ ] **Step 3: Implement command and handler**

Use this record shape:

```csharp
public sealed record QualifyOpportunityCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    string TargetStage,
    Guid ExpectedVersion,
    string IdempotencyKey,
    string TraceId);
```

Resolve exactly `opportunities.transition`; require `access.BranchId`; normalize target via `Trim().ToLowerInvariant()` and reject anything except `OpportunityStage.Qualified`; hash key with `Sha256Hex`; canonical payload must be `${OpportunityId:D}|qualified|${ExpectedVersion:D}` using invariant lowercase UUID formatting.

- [ ] **Step 4: Run GREEN and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter FullyQualifiedName~OpportunityHandlerTests`

Expected: all handler tests pass and fake store records one call only for valid input.

```bash
git add backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityCommand.cs backend/src/TanErp.Application/Crm/Opportunities/QualifyOpportunity/QualifyOpportunityHandler.cs backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityHandlerTests.cs
git commit -m "feat(crm): add qualify opportunity use case"
```

## Task 5: Publish the HTTP/OpenAPI Contract and Localized Errors

**Files:**
- Create: `backend/src/TanErp.Api/Contracts/Crm/Opportunities/TransitionOpportunityStageRequest.cs`
- Modify: `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs`
- Modify: `contracts/openapi/tan-erp.v1.json`

**Interfaces:**
- Consumes: Task 4 handler
- Produces: exact HTTP endpoint, ETag, RFC 9457 mapping and machine-readable OpenAPI

- [ ] **Step 1: Write one failing successful endpoint test**

Add `QualifyOpportunity_ValidRequest_ReturnsQualifiedWithNewETagAndPersistsHistory` in `OpportunitySiteEndpointsTests.cs`. Assert `200`, Qualified response, changed ETag/rowVersion and exactly one history/audit row. Extend the existing OpenAPI snapshot assertion for the new path/schema/statuses without creating a second feature behavior test.

- [ ] **Step 2: Run API tests to verify RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpportunitySiteEndpointsTests|FullyQualifiedName~OpenApiContractTests"`

Expected: FAIL with missing route/schema/error mappings.

- [ ] **Step 3: Implement the thin endpoint**

Request record:

```csharp
public sealed record TransitionOpportunityStageRequest(string TargetStage, Guid ExpectedVersion);
```

Controller reads `RequestContextReader.ReadIdempotentRequest`, constructs `QualifyOpportunityCommand`, delegates once, maps failure through `ProblemDetailsMapper`, writes `ETag = $"\"{result.Value.RowVersion}\""`, and returns existing `OpportunityResponse`. Register handler/store only through DI; do not put business rules in controller.

- [ ] **Step 4: Add errors and test-only permission**

Map both new codes to `409`. Add `_TITLE`/`_DETAIL` values in Thai and English. Add `opportunities.transition` to TestOnlyDataSeeder roles for Org A/B; do not seed policy/business data in production.

- [ ] **Step 5: Regenerate and verify OpenAPI**

Start the Test API using the existing snapshot workflow, regenerate `contracts/openapi/tan-erp.v1.json`, and run:

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpportunitySiteEndpointsTests|FullyQualifiedName~OpenApiContractTests"`

Expected: exact route, request schema, `OpportunityResponse`, ETag and documented errors pass.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Api/Contracts/Crm/Opportunities/TransitionOpportunityStageRequest.cs backend/src/TanErp.Api/Controllers/OpportunitiesController.cs backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs backend/src/TanErp.Api/Resources/Errors.resx backend/src/TanErp.Api/Resources/Errors.en.resx backend/src/TanErp.Api/Program.cs backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs contracts/openapi/tan-erp.v1.json
git commit -m "feat(api): expose opportunity qualification"
```

## Task 6: Add Generated Client Mutation and Retry-safe Cache Updates

**Files:**
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/features/opportunities/api/opportunity-queries.ts`
- Modify: `frontend/src/features/opportunities/api/opportunity-queries.test.tsx`

**Interfaces:**
- Consumes: OpenAPI path `/api/v1/opportunities/{id}/stage-transitions`
- Produces: `TransitionOpportunityStageRequest` alias, `apiClient.transitionOpportunityStage(...)`, `useQualifyOpportunity()`

- [ ] **Step 1: Regenerate types and write one failing client/mutation test**

Run: `npm --prefix frontend run generate:api`

Add `useQualifyOpportunity_Success_PostsContractAndRefreshesCaches`; in one test assert POST path, membership/idempotency/language headers, exact body, detail cache replacement and opportunity-list invalidation.

- [ ] **Step 2: Implement typed client method**

```ts
export type TransitionOpportunityStageRequest =
  components["schemas"]["TransitionOpportunityStageRequest"];

async transitionOpportunityStage(
  id: string,
  payload: TransitionOpportunityStageRequest,
  options: RequestOptions
): Promise<OpportunityResponse> {
  return this.request<OpportunityResponse>(
    `/api/v1/opportunities/${encodeURIComponent(id)}/stage-transitions`,
    "POST",
    options,
    payload
  );
}
```

Reuse `PERMISSIONS.OPPORTUNITIES_TRANSITION` from `frontend/src/lib/permissions/permissions.ts`; it already matches the authoritative catalog, so this task must not introduce a parallel permission constant or edit the permission helper.

- [ ] **Step 3: Implement TanStack mutation**

`useQualifyOpportunity` accepts `{ opportunityId, expectedVersion, idempotencyKey }`, always sends `targetStage: "qualified"`, obtains token/membership/locale through existing contexts, sets returned resource into `opportunityDetailQueryKey`, then invalidates opportunity list prefixes. Do not store server state in component state.

- [ ] **Step 4: Run GREEN and commit**

Run: `npm --prefix frontend run test -- --run src/features/opportunities/api/opportunity-queries.test.tsx src/lib/api/api-client.test.ts`

Run: `npm --prefix frontend run check:api`

Expected: tests pass and generated API file has no drift.

```bash
git add frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api/api-client.ts frontend/src/features/opportunities/api/opportunity-queries.ts frontend/src/features/opportunities/api/opportunity-queries.test.tsx
git commit -m "feat(crm): add qualification client mutation"
```

## Task 7: Complete the Qualification Journey on Opportunity Detail

**Files:**
- Modify: `frontend/src/features/opportunities/components/opportunity-detail.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`
- Modify: `frontend/src/features/opportunities/opportunity-labels.ts`
- Modify: `frontend/src/features/opportunities/components/opportunity-list.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: Task 6 mutation, `ConfirmationModal`, `can(...)`, current Opportunity `rowVersion`
- Produces: keyboard-accessible Qualify control with double-submit and retry-intent guarantees

- [ ] **Step 1: Write one failing successful component test**

Add `qualifies a draft opportunity from the confirmation modal`; assert the action is visible for Draft + permission, opens by keyboard, sends current row version once, locks confirm while pending and renders the Qualified badge after success.

- [ ] **Step 2: Fix canonical labels first**

Replace `estimation`/`proposal` with `surveying`/`estimating`/`proposed`, add `cancelled`, and keep all eight stage labels in both locale files. Update list/detail switch exhaustively; unknown values continue to use localized `unknownStage`.

- [ ] **Step 3: Implement qualification intent and action**

Use a ref with exact shape:

```ts
interface QualificationIntent {
  idempotencyKey: string;
  opportunityId: string;
  expectedVersion: string;
}
```

Create it on first confirm with `crypto.randomUUID()`. Reuse it after network/5xx ambiguity while opportunity ID/version are unchanged. Clear it on success, cancel-before-submit, resource/version change, or `OPPORTUNITY_VERSION_CONFLICT`. Keep the modal open with localized error on failure and lock close/confirm while pending.

- [ ] **Step 4: Render eligibility guidance without inventing edit scope**

When a Draft lacks Q-gate data, keep the action available so Backend remains authoritative, but show a localized checklist for missing `scopeSummary`, work types, next-action time/note before confirmation. State explicitly that editing an existing Draft is deferred; do not add hidden patch behavior.

- [ ] **Step 5: Run component, i18n and policy checks**

Run: `npm --prefix frontend run test -- --run src/features/opportunities/components/opportunity-detail.test.tsx src/features/opportunities/components/opportunity-list.test.tsx`

Run: `npm --prefix frontend run typecheck`

Run: `node -e "const fs=require('fs');const th=JSON.parse(fs.readFileSync('frontend/src/messages/th.json'));const en=JSON.parse(fs.readFileSync('frontend/src/messages/en.json'));const walk=(o,p='')=>Object.entries(o).flatMap(([k,v])=>v&&typeof v==='object'?walk(v,p+k+'.'):[p+k]);const a=walk(th),b=walk(en);if(a.length!==b.length||a.some(k=>!b.includes(k)))process.exit(1)"`

Expected: component tests/typecheck pass and locale key parity exits `0`.

- [ ] **Step 6: Commit**

```bash
git add frontend/src/features/opportunities/components/opportunity-detail.tsx frontend/src/features/opportunities/components/opportunity-detail.test.tsx frontend/src/features/opportunities/opportunity-labels.ts frontend/src/features/opportunities/components/opportunity-list.test.tsx frontend/src/messages/th.json frontend/src/messages/en.json
git commit -m "feat(crm): add opportunity qualification action"
```

## Task 8: Prove the Slice and Publish Verification Evidence

**Files:**
- Create: `frontend/e2e/opportunity-qualification.spec.ts`
- Create: `docs/05-engineering/opportunity-qualification-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: completed Slice 3 application and accepted UAT mapping
- Produces: reproducible proof tied to one tested commit SHA and explicit remaining boundary

- [ ] **Step 1: Add one positive Playwright journey**

Add `test("qualifies opportunity from draft to qualified", ...)`: login → create gate-complete Draft → detail → keyboard-open confirmation → double-click confirm → intercept exactly one transition POST → assert request headers/body → assert Qualified badge/list filter → direct DB/API fixture confirms one history and one audit event.

- [ ] **Step 2: Add responsive assertions to the successful journey**

Set the same test to 320×800 and 200% zoom before opening detail; assert no horizontal page overflow, modal controls remain reachable, visible focus is retained and controls are at least 44px. Do not create a second E2E test.

- [ ] **Step 3: Run Backend regression gates**

Run: `dotnet build backend/TanErp.slnx`

Run: `dotnet test backend/TanErp.slnx`

Expected: exit `0`, zero failed tests.

- [ ] **Step 4: Run Frontend and fixture regression gates**

Run: `npm --prefix frontend run verify`

Run: `npm run test:fixtures`

Expected: API drift, lint, typecheck, Vitest, production build and fixture schema all pass.

- [ ] **Step 5: Run real E2E**

With PostgreSQL 17, Firebase emulator, Backend `:5005` and Frontend `:3005` running:

Run: `PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e`

Expected: auth, Customer/Contact, Opportunity/Site and Qualification journeys all pass; test discovery alone is not evidence.

- [ ] **Step 6: Run policy and migration checks**

Run: `rg -n ':\s*any\b|\bas\s+any\b|@ts-ignore' frontend/src frontend/e2e --glob '*.{ts,tsx}'`

Expected: exit `1` with no matches.

Run: `git diff --check`

Expected: exit `0`.

Apply all migrations from an empty PostgreSQL 17 database and inspect history table constraints/index; then downgrade one migration and upgrade again without data loss outside the new table.

- [ ] **Step 7: Write verification record**

Record date, branch, tested commit SHA, actual runtime versions, migration name, exact commands/exit codes/test counts, successful UAT/accessibility mapping, privacy scan result, rollout (`migration first, same release API/UI`) and rollback policy (`forward-fix after any history exists; migration rollback only before use`). Set status `Pilot Passed` only after Steps 3–6 have real evidence; list deferred negative-path tests and do not claim full Definition of Done.

- [ ] **Step 8: Commit evidence**

```bash
git add frontend/e2e/opportunity-qualification.spec.ts docs/05-engineering/opportunity-qualification-verification.md docs/README.md
git commit -m "test(crm): verify opportunity qualification slice"
```

## Definition of Success — Pilot Gate

1. ผู้ใช้ที่มี `opportunities.transition` และ scope ถูกต้อง Qualify Opportunity Draft ที่ผ่าน Q gate ได้จาก detail และเห็น Qualified ใน detail/list โดยไม่ reload ทั้งแอป
2. Backend รับเฉพาะ `draft → qualified`, ตรวจ current Customer/Branch/Owner state และไม่เชื่อ Organization/Branch/Owner/fromStage จาก client
3. Success rotate row version, ส่ง ETag ใหม่ และเขียน Opportunity, one append-only history, one privacy-safe audit และ idempotency record ใน transaction เดียว
4. Automated tests ใหม่ 7 จุดผ่าน: Domain, Persistence, Application, HTTP/OpenAPI, Frontend client/cache, Component UI และ Playwright successful journey
5. Error/security/concurrency behavior ถูก implement แบบ fail-closed ตาม contract แต่ยังไม่ถือว่าพิสูจน์จนกว่า deferred negative-path tests จะผ่าน
6. OpenAPI/generated client ไม่มี drift; UI มี Thai/English parity, double-submit lock, typed error handling, keyboard/modal/focus, 320px/200% zoom และ Atelier Navy Sharp ครบ
7. Migration from zero, regression suites เดิม, full Playwright suite, forbidden-TypeScript scan และ `git diff --check` ผ่านจริงและถูกบันทึกด้วย tested commit SHA; verification status เป็น `Pilot Passed`

## Deferred Scope

- Negative-path tests: permission denial, cross-organization isolation, missing Q gate, stale version, illegal/repeated transition, idempotency-key conflict, concurrent winner และ ambiguous-response retry; ต้องทำก่อนยกระดับจาก Pilot สู่ Production-ready
- Opportunity generic edit/PATCH, owner reassignment, Branch/Own permission scope และ stage-history read UI/API
- `qualified → surveying → estimating → proposed`, Won/Lost/Cancelled, reason catalog, Reopen และ quotation-driven transitions
- Survey appointment, Site Survey identity, Draft Revision, Measurement/Checklist/Evidence, Mark Ready/Clone/Void
- Customer/Site edit/deactivation และ duplicate merge/export/retention-redaction workflows
- Item Master, Official Estimate, Approval, Quotation, Project, Procurement, Inventory, Production และ MRP
- Runtime stage policy builder; Slice นี้ใช้ reviewed policy constant `opportunity-stage-v1`
