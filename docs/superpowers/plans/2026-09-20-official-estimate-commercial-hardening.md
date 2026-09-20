# Official Estimate and Commercial Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ทำให้ Official Estimate สร้างจาก trusted Survey evidence และทำ Quotation issuance, customer acceptance, Opportunity Proposed/Won กับ Document Numbering ให้ idempotent, concurrent-safe และ permission-correct โดยคงฟีเจอร์ทั้งหมดไว้.

**Architecture:** Backend derive Estimate relationships จาก Opportunity + Ready Survey Revision. Commercial use cases ใช้ idempotency record และ EF transaction เดียวกับ quotation, stage history และ audit; document number มาจาก atomic sequence engine เท่านั้น. Frontend ส่ง required row versions, เก็บ idempotency key ต่อ user intent และใช้ exact quotation permissions.

**Tech Stack:** ASP.NET Core, EF Core/PostgreSQL, OpenAPI, Next.js/TypeScript, TanStack Query, Vitest, Playwright.

## Global Constraints

- ก่อนแก้ implementation ต้องสร้าง authoritative Slice 5A/5B plans และแก้ `AGENTS.md` Current phase ให้ตรงคำสั่งผู้ใช้ที่ให้คงฟีเจอร์.
- Create Estimate body exact `{opportunityId,siteSurveyRevisionId,currency}`; Backend derive Customer, Branch และ snapshot hash.
- Issue Quotation ใช้ calculated current revision, Opportunity stage `estimating`, exact versions และ `quotations.issue`.
- Accept Quotation ใช้ status `issued`, Opportunity stage `proposed`, exact version และ `quotations.accept`; `opportunities.transition` ใช้แทนไม่ได้.
- Replay check มาก่อน version/state checks. Same key+same payload คืน resource เดิม; same key+different payload คืน `409 IDEMPOTENCY_KEY_REUSED`.
- ห้าม fallback `COUNT + 1`; numbering failure ต้อง rollback transaction และคืน stable Problem Details.
- Generated OpenAPI fields ต้อง required; frontend ห้ามสร้าง API DTO เอง.
- เพิ่ม targeted tests 8 จุดเท่านั้น; full regressions รันครั้งเดียวที่ exit gate.

---

## Exact Contracts

```http
POST /api/v1/estimates
Idempotency-Key: <16-128 chars>
{"opportunityId":"uuid","siteSurveyRevisionId":"uuid","currency":"THB"}
```

Backend derives `customerId`, `branchId`, `siteSurveySnapshotHash`; accepts Ready/Superseded revision belonging to same Opportunity and organization while Opportunity is `estimating`.

```http
POST /api/v1/estimates/{estimateId}/quotation
Idempotency-Key: <16-128 chars>
{"expectedEstimateVersion":"uuid","expectedOpportunityVersion":"uuid"}
```

Success `201 QuotationResponse`; atomic effects: allocate unique number, snapshot calculated revision, mark Estimate/Revision quoted, transition Opportunity `estimating → proposed`, append one history, two privacy-safe audits and one idempotency record.

```http
POST /api/v1/estimates/{estimateId}/quotation/accept
Idempotency-Key: <16-128 chars>
{"expectedOpportunityVersion":"uuid","decisionNote":null}
```

Success `200 AcceptQuotationResponse`; atomic effects: mark Quotation accepted, transition Opportunity `proposed → won`, append one history, two privacy-safe audits and one idempotency record. Audit excludes `decisionNote`.

```http
GET /api/v1/settings/document-sequences
POST /api/v1/settings/document-sequences/preview
PUT /api/v1/settings/document-sequences/{documentType}
If-Match: "<rowVersion>"
```

List uses `document-sequences.read`; preview/update use `document-sequences.manage`. Update returns new ETag/rowVersion; invalid type, reset period, token or pattern returns localized 422 and never silently defaults.

## Task 1: Freeze Slice 5A/5B authority

**Files:**
- Create: `docs/superpowers/plans/2026-09-18-official-estimate-boq-calculation-vertical-slice.md`
- Create: `docs/superpowers/plans/2026-09-20-commercial-quotation-vertical-slice.md`
- Modify: `AGENTS.md`
- Modify: `docs/03-contracts/official-estimate-api-contract.md`
- Modify: `docs/04-data/official-estimate-data-contract.md`
- Modify: `docs/05-engineering/official-estimate-uat-scenarios.md`
- Create: `docs/05-engineering/commercial-quotation-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: commits through `ddd90db`, accepted business vocabulary and exact contracts above.
- Produces: authoritative Slice 5A/5B boundaries, error matrix, file map, tests and Definition of Success.

- [ ] Reconstruct Slice 5A from accepted documents and history through `28aedbf`; keep Commercial behavior out of 5A.
- [ ] Freeze Slice 5B issue/accept/numbering contracts, permissions, idempotency precedence and atomic side effects.
- [ ] Update `AGENTS.md` boundary to authorize only specified Slice 5B behavior; quotation amendment/void/PDF/Project creation remain deferred.
- [ ] Mark current verification as remediation pending; preserve historical SHA/results.
- [ ] Validate links and `git diff --check`.
- [ ] Proposed commit: `docs(commercial): authorize quotation hardening contract`

## Task 2: Trust Estimate creation — tests 1–2

**Files:**
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/CreateEstimateDraftRequest.cs`
- Modify: `backend/src/TanErp.Api/Controllers/EstimatesController.cs`
- Modify: `backend/src/TanErp.Application/Estimates/CreateEstimateDraft/CreateEstimateDraftCommand.cs`
- Modify: `backend/src/TanErp.Application/Estimates/CreateEstimateDraft/CreateEstimateDraftHandler.cs`
- Modify: `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`

**Interfaces:**
- Consumes: organization/actor, Opportunity and Ready Survey Revision.
- Produces: trusted `CreateDraftAsync` returning `Result<EstimateDetailProjection>`.

- [ ] Add failing test `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot`.
- [ ] Add parameterized failing test `CreateEstimate_ForgedOrUnreadyRelationship_IsRejected`; cover cross-org, wrong Opportunity, Draft revision and non-estimating stage; assert no number/Estimate/audit consumed.
- [ ] Use signature:

```csharp
Task<Result<EstimateDetailProjection>> CreateDraftAsync(
    Guid organizationId,
    Guid opportunityId,
    Guid siteSurveyRevisionId,
    string currency,
    Guid actorUserId,
    string keyHash,
    string payloadHash,
    CancellationToken cancellationToken);
```

- [ ] In one execution-strategy transaction: replay first; load scoped Opportunity; require `estimating`; load matching Ready/Superseded revision; derive relationship fields/hash; allocate atomic number; persist Estimate, audit and idempotency.
- [ ] Run tests 1–2 and existing create/calculate tests.
- [ ] Proposed commit: `fix(estimates): derive draft from ready survey evidence`

## Task 3: Export server snapshot — test 3

**Files:**
- Modify: `frontend/src/features/estimates/components/estimate-workspace-drawer.tsx`
- Create: `frontend/src/features/estimates/utils/estimate-export.ts`
- Create: `frontend/src/features/estimates/utils/estimate-export.test.ts`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: latest `calculationSnapshotJson`.
- Produces: `buildEstimateCsv(snapshot: EstimateCalculationSnapshot): string`.

- [ ] Add failing test `buildEstimateCsv_UsesServerSnapshotTotalsWithoutRecalculation`.
- [ ] Parse JSON as `unknown` with strict `isEstimateCalculationSnapshot`; include calculation version, currency, cost, discount, VAT, grand total and margins.
- [ ] Remove client calculation from export path; disable export with localized message when valid snapshot absent.
- [ ] Run test 3 and existing Estimate workspace tests.
- [ ] Proposed commit: `fix(estimates): export verified calculation snapshot`

## Task 4: Idempotent quotation issue and atomic numbering — tests 4–5

**Files:**
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/IssueQuotationRequest.cs`
- Modify: `backend/src/TanErp.Application/Estimates/IssueQuotation/IssueQuotationHandler.cs`
- Modify: `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/DocumentNumbering/DocumentNumberGenerator.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Test: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`

**Interfaces:**
- Consumes: `IDocumentNumberGenerator`, expected Estimate/Opportunity versions, key/payload hashes.
- Produces: one replay-safe `QuotationDetailProjection`.

- [ ] Add test `IssueQuotation_ReplaySameIntent_ReturnsSameQuotationWithoutDuplicateEffects`; assert one Quotation/history/audit pair/idempotency record.
- [ ] Add concurrent test `IssueQuotation_TwoEstimates_AllocatesDistinctAtomicNumbers`; assert no fallback and unique numbers.
- [ ] Add `[Required]` to both version fields and compute canonical payload hash from estimate ID + both versions.
- [ ] Change store inputs from raw key to `keyHash,payloadHash`; check replay before versions/stage, reject different payload, catch PostgreSQL unique race then reload winner.
- [ ] Delete catch-all and `CountAsync()+1` fallback. Map sequence/config failure to `DOCUMENT_NUMBER_ALLOCATION_FAILED` 409 with paired resources.
- [ ] Run tests 4–5 and document-number parser/counter tests.
- [ ] Proposed commit: `fix(commercial): make quotation issue replay safe`

## Task 5: Acceptance and frontend intent correctness — tests 6–7

**Files:**
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/AcceptQuotationRequest.cs`
- Modify: `backend/src/TanErp.Application/Estimates/AcceptQuotation/AcceptQuotationHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Modify: `frontend/src/features/estimates/api/estimate-queries.ts`
- Modify: `frontend/src/features/estimates/components/estimate-card.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-card.test.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-detail.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`
- Test: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`

**Interfaces:**
- Consumes: Opportunity rowVersion from detail projection and exact quotation permissions.
- Produces: stable issue/accept mutation intent `{payload,idempotencyKey}`.

- [ ] Add backend test `AcceptQuotation_ReplaySameIntent_ReturnsSameWonResultWithoutDuplicateEffects`; different key after Won must not masquerade as replay.
- [ ] Add frontend test `IssueQuotation_SendsBothVersionsAndReusesKeyAfterAmbiguousFailure`; assert `expectedEstimateVersion`, `expectedOpportunityVersion` and stable UUID.
- [ ] Pass `opportunityRowVersion` into `EstimateCard`. `canIssue` requires only `quotations.issue`; `canAccept` requires only `quotations.accept`.
- [ ] Store idempotency key per unchanged estimate/opportunity version intent; clear on success, cancel-before-submit or version change. Apply same rule to acceptance.
- [ ] Add `[Required]` to acceptance version; backend replay check precedes Won/state/version checks and audits omit decision note.
- [ ] Run tests 6–7 plus EstimateCard/OpportunityDetail tests.
- [ ] Proposed commit: `fix(commercial): preserve quotation mutation intent`

## Task 6: Harden document-numbering settings — test 8

**Files:**
- Modify: `backend/src/TanErp.Api/Contracts/DocumentNumbering/DocumentSequenceResponse.cs`
- Modify: `backend/src/TanErp.Api/Contracts/DocumentNumbering/UpdateDocumentSequenceRequest.cs`
- Modify: `backend/src/TanErp.Api/Controllers/DocumentSequencesController.cs`
- Modify: `backend/src/TanErp.Application/DocumentNumbering/UpdateDocumentSequence/UpdateDocumentSequenceCommand.cs`
- Modify: `backend/src/TanErp.Application/DocumentNumbering/UpdateDocumentSequence/UpdateDocumentSequenceHandler.cs`
- Modify: `backend/src/TanErp.Domain/DocumentNumbering/DocumentSequenceDefinition.cs`
- Modify: `frontend/src/features/settings/document-numbering/api/document-sequence-queries.ts`
- Modify: `frontend/src/features/settings/document-numbering/components/document-sequence-drawer.tsx`
- Test: `backend/tests/TanErp.IntegrationTests/Api/DocumentSequencesEndpointsTests.cs`

**Interfaces:**
- Consumes: quoted `If-Match`, exact permissions and validated token grammar.
- Produces: `DocumentSequenceResponse.rowVersion`, response ETag and conditional update.

- [ ] Add parameterized test `DocumentSequence_UpdateRequiresPermissionVersionAndValidPattern`; success case rotates ETag; missing permission, stale version and invalid reset/token leave definition unchanged.
- [ ] Reject unknown reset period instead of defaulting Yearly; validate allowed tokens and require `{SEQ}` exactly once.
- [ ] Serialize audit with `JsonSerializer`; never interpolate user pattern into JSON.
- [ ] Update frontend mutation to send `If-Match`, lock submit and handle stable structured codes.
- [ ] Run test 8 and existing sequence tests.
- [ ] Proposed commit: `fix(numbering): validate conditional sequence settings`

## Task 7: Contract and journey verification

**Files:**
- Modify: `contracts/openapi/tan-erp.v1.json`
- Regenerate: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/e2e/official-estimate.spec.ts`
- Modify: `docs/05-engineering/official-estimate-verification.md`
- Modify: `docs/05-engineering/commercial-quotation-verification.md`

**Interfaces:**
- Consumes: Tasks 1–6.
- Produces: evidence-backed Estimate → Quotation → Won journey.

- [ ] Generate contract and assert all request/version fields required.
- [ ] Run targeted eight tests, then full backend/frontend gates once.
- [ ] Run browser journey: Ready Survey → Estimate → calculate → issue Quotation → Proposed → accept → Won; assert one history row per transition and exact permission visibility.
- [ ] Record SHA, commands, exit codes and test count. Do not call Production-ready if security/concurrency gates fail.
- [ ] Proposed commit: `test(commercial): verify quotation and numbering hardening`

## Definition of Success

- Eight named tests plus existing regressions pass.
- Estimate linkage and export use server-owned evidence/snapshot.
- Issue/accept retries return same result only for same key+payload; duplicates never create number, quotation, stage history or audit.
- Concurrent issues allocate distinct numbers through atomic counter; no `COUNT + 1` fallback remains.
- UI sends both required versions, reuses stable keys and exposes actions only for exact permissions.
- Document settings reject invalid values, require permission + current ETag and emit safe audit JSON.
- OpenAPI generated fields required; full browser journey reaches Won once.

## Deferred Scope

- Quotation amendment, void/cancel, PDF rendering/signature, external customer acceptance portal and expiry reminders.
- Maker–Checker approval, later Estimate revisions and automatic Project creation after Won.

