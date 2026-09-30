# Official Estimate BOQ & Calculation Vertical Slice Verification (Slice 5A)

**Status:** Verified (Hardened per `docs/superpowers/plans/2026-09-20-official-estimate-commercial-hardening.md`)
**Tested Commit/Date:** 2026-09-21 (Hardening verified on `feat/opportunity-qualification`)

**Current release status (2026-09-28):** Core implementation and local automated code gates pass, but the UAT trace audit identified unresolved coverage/contract gaps in UAT-EST-003/006/011/012/013/018. These are tracked in the UAT execution register and completion plan. Production evaluation is not approved until those scope gaps are resolved, authorized-role UAT and policy sign-offs are recorded, and pilot data is ready; sanitized historical-data review is also required if legacy estimates/quotations will be retained.

**Environment:**
- macOS Apple Silicon
- PostgreSQL 17 Container (port 5432)
- Firebase Auth Emulator (port 9099)
- ASP.NET Core 10.0 WebApi (port 5005)
- Next.js 16.3.4 Frontend (port 3005)
- .NET SDK 10.0.400

---

## 1. Scope & Implementation Summary

This vertical slice delivers the complete **Official Estimate BOQ & Calculation Engine (Slice 5A)** as defined in `docs/superpowers/plans/2026-09-18-official-estimate-boq-calculation-vertical-slice.md`:

1. **Official Estimate Draft Creation:**
   - Initiated from an Opportunity in the `estimating` stage.
   - Bound to Customer, Opportunity, Branch, and Ready Site Survey Revision Snapshot Hash.
   - Backend endpoint: `POST /api/v1/estimates` with Idempotency Key protection.

2. **Estimate Workspace Drawer (BOQ Management):**
   - 3-tier BOQ hierarchy: Sections -> Work Items -> Cost Components (Material, Labor, Subcontract, Service, Other).
   - Real-time pricing rules (Margin % or Markup %) per work item.
   - Live Financial HUD with real-time recalculation of total cost, selling before discount, gross profit, and margin rate.
   - Concurrency-controlled draft saving (`PUT /api/v1/estimates/{id}/revisions/{revisionId}/draft` with `If-Match` ETag header and `expectedRevisionVersion`).

3. **Official Recalculation Engine:**
   - Full server-side recalculation via `POST /api/v1/estimates/{id}/revisions/{revisionId}/calculate`.
   - Supports discount deduction, VAT 7% computation, Net Before Tax, and Grand Total.
   - Increments calculation version and stores a reproducible `CalculationSnapshotJson`.

4. **Strict Architectural Guardrails:**
   - Zero `any`, zero `@ts-ignore`, zero arbitrary string error fallback.
   - Strict `ApiError.code` handling with bilingual i18n support (`th.json` & `en.json`).
   - Atelier Architectural Navy Sharp design compliance (`0px` border-radius, Solid Navy `#0B3056`, SVG icons).

---

## 2. Automated Test Results

### 2.1 Backend Unit Tests
```bash
/Users/syaco/.dotnet/dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj
```
**Result:** Passed 100% (157/157 tests green, 43 ms)
- `CreateDraft_InitializesEstimateWithRevisionOne`
- `Calculate_WithMarginSellingRule_CalculatesCorrectTotalsAndMarginRate`
- `Calculate_WithDiscount_AdjustsTaxAndGrandTotalProperly`

### 2.2 Backend Integration Tests
```bash
/Users/syaco/.dotnet/dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests"
```
**Result:** Passed 100% (4/4 tests green, 9 s)
- `CreateEstimateDraft_ValidScope_ReturnsCreated`
- `UpdateEstimateDraft_ValidSections_UpdatesAndRotatesRowVersion`
- `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot`
- `UpdateDraft_OutdatedVersion_Returns409Conflict`

### 2.3 Frontend Unit & Component Tests (Vitest)
```bash
npm --prefix frontend run test
```
**Result:** Passed 100% (111 test files, 476 tests green, 12 s)
- `estimate-card.test.tsx`
- `estimate-calculations.test.ts`
- `estimate-formatters.test.ts`
- `estimate-catalog-items.test.ts`
- `estimate-templates.test.ts`
- `estimate-item-catalog-modal.test.tsx`

### 2.4 Frontend Verification Pipeline
```bash
npm --prefix frontend run verify
```
**Result:** All gates passed with 0 errors, 0 warnings:
- `check:api`: Passed (Generated API contract is in exact parity)
- `lint`: Passed (`eslint .` clean)
- `typecheck`: Passed (`tsc --noEmit` clean, strict types)
- `build`: Passed (`next build` compiled successfully)

### 2.5 End-to-End Test (Playwright)
```bash
npx playwright test e2e/official-estimate.spec.ts
```
**Result:** Passed 100% (1/1 test green, 10.9 s)
Verified complete journey:
1. Login with test foundation credentials
2. Create and activate customer with primary site
3. Create opportunity, meet Q-gate invariants, and qualify
4. Schedule survey appointment -> transition to `surveying`
5. Record survey measurements and mark revision ready -> transition to `estimating`
6. Create official estimate draft -> verify `EstimateCard`
7. Open Estimate Workspace Drawer -> add section -> add work item -> add cost component
8. Save draft with Concurrency ETag protection
9. Apply discount and recalculate -> verify server-calculated financial totals
10. Close Drawer -> confirm Opportunity detail reflects updated estimate summary.

---

## 3. Sign-off

- **Implementation Plan:** `docs/superpowers/plans/2026-09-18-official-estimate-boq-calculation-vertical-slice.md`
- **Verification Status:** Complete and Verified (DoD Met).

---

## 4. Current Working-Tree Readiness Follow-up (2026-09-27)

This section records follow-up work in the current working tree; the 2026-09-21 verification above describes an earlier code state and is not evidence for this follow-up.

### Synthetic catalog/cost evidence scenario

`backend/tests/TanErp.IntegrationTests/Scenarios/ItemCatalogEstimateFlowTests.cs` now defines `PilotEstimate_CatalogCostsResolveAndSnapshotSurvivesNewCostVersion` with synthetic organization/branch data, an active costable item, unit/category, published organization and branch costs, a test cost source, and a synthetic evidence PDF. Its BOQ includes material, labor, and subcontract components. The estimate resolves the branch cost, records the selected cost record/version and evidence provenance, then exercises the saved estimate after later cost versions are published. This scenario does not contain customer or production prices.

### Current implementation delta

- Draft financial edits mark the last calculation as outdated; the previous calculation snapshot remains readable until a fresh calculation is created.
- Calculation rejects a discount above the selling amount and rejects margin/markup values outside their documented ratio ranges.
- The quotation gate requires an Approved Revision with a non-outdated calculation snapshot and frozen approval snapshot/hash. The independent approval route is implemented; a focused PostgreSQL journey now exercises Calculate → Submit → Return → Recalculate → Resubmit → Independent Approve → Issue Quotation.
- No sanitized production copy or business/finance-approved policy values were available during this follow-up. Production master-data readiness and policy sign-off remain unverified.

### Versioned calculation policy follow-up (2026-09-27)

- Added additive migration `20260927110339_AddVersionedEstimatePolicies` for Calculation/Tax Policy versions and optional policy IDs/hashes on historical calculation snapshots. The migration was applied by the PostgreSQL-backed integration test; old snapshot rows remain valid with null policy references.
- Estimate calculation now resolves exactly one effective Published policy, preferring Branch over Organization scope and rejecting missing/ambiguous resolution with bilingual `ESTIMATE_POLICY_UNAVAILABLE` Problem Details. No production rate was seeded; 7% exists only in guarded `TEST_ONLY` fixtures.
- Calculation applies policy overhead to direct cost, allocates the rounded remainder deterministically, calculates selling price from loaded cost, supports exclusive/inclusive/exempt tax, and includes policy identity/hash and overhead inputs in the snapshot. History API exposes policy IDs and hashes.
- Verified on 2026-09-27: `dotnet build backend/TanErp.slnx --no-restore -m:1` passed (0 warnings/errors); `EstimateTests` passed 9/9; `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` passed 1/1; `CalculateEstimate_WithoutPublishedPolicy_FailsClosed` passed 1/1; `PilotEstimate_CatalogCostsResolveAndSnapshotSurvivesNewCostVersion` passed 1/1 with branch-scoped policy precedence.
- Remaining: full test/build gate after the latest edits; full policy test matrix; readiness reasons and Submit gate; Discount modes; policy effective-overlap constraint; cancel lifecycle and estimate UI; API generator parity; sanitized legacy quotation audit; Business/Finance sign-off.

### Follow-up verification

- Estimate domain tests: 8 passed after the calculation invalidation/range changes.
- Estimate card component tests: 6 passed; frontend TypeScript typecheck passed.
- Backend solution build: passed with 0 warnings and 0 errors; frontend lint passed.
- Focused Draft quotation gate integration test: 1 passed against PostgreSQL after the additive calculation-outdated migration.
- Focused calculation history integration test: 1 passed against PostgreSQL; it verifies the appended row, 64-character input hash, JSONB totals, and history endpoint.
- OpenAPI schema and generated TypeScript snapshot-history types were updated manually for policy IDs/hashes. Full generator parity remains unverified because the checked-in local TypeScript/openapi-typescript combination (`typescript` 7.0.2) previously threw before reading the schema. Full current-tree backend/frontend gates remain pending.

### Approval and revision follow-up (2026-09-27)

- Added additive approval workflow tables and Bootstrap independent-checker routing. Submit freezes the calculation input/snapshot hashes and reviewer route; reviewer authority is checked again at decision time. Return records a reason and invalidates the calculation; resubmission creates a new request while preserving prior decisions. Approved revision stores a frozen approval snapshot.
- Added `POST /api/v1/estimates/{id}/submit` and `POST /api/v1/estimates/{id}/review-decisions`, with If-Match, Idempotency-Key, audit, and stable Problem Details. Quotation issuance now requires the approval snapshot in addition to a current calculation.
- Added `POST /api/v1/estimates/{id}/revisions`. It requires a reason, If-Match, and Idempotency-Key, and creates a Draft revision with new IDs for the BOQ hierarchy while preserving its financial input values. The old approved revision and issued quotation remain unchanged. This endpoint has no frontend entry point yet.
- Verified 2026-09-27: focused PostgreSQL integration journey passed through Submit → Return → Recalculate → Resubmit → independent Approve → Issue Quotation → Create Revision; revision replay with the original ETag returned the same new revision. Focused domain test passed for cloning BOQ with new IDs and invalidated calculation. Backend solution build passed after adding the revision endpoint.
- Remaining in this implementation plan: cancel workflow (including authorized cancellation of a submitted approval route), readiness/reason projection, remaining policy tests, frontend submit/review/revision controls, full verification gates, sanitized data review, and Finance/Business sign-off. Bootstrap approval policy is a typed system constant, not an administratively published database policy; no real financial authority thresholds have been invented.

### Cancel lifecycle and lifecycle controls (2026-09-27)

- Added `POST /api/v1/estimates/{id}/cancel` with active `estimates.cancel` permission, `If-Match`, `Idempotency-Key`, audit, and localized failure codes. Cancelling a Submitted revision requires the assigned reviewer and closes the open approval request in the same transaction as the estimate/revision state change.
- PostgreSQL integration coverage verifies cross-organization 404, maker rejection, assigned reviewer success, closed approval route, same-key replay with stale ETag, and concurrent same-version requests (one 200, one 409, one cancellation audit event). Focused concurrency test passed 1/1 on 2026-09-27.
- Added Estimate lifecycle controls for Submit, Approve, Return, New Revision, and Cancel using shared Modal/Input/Textarea/Button components, TanStack mutations, bilingual messages, and loading lock. Return collects the contract-required reason code and note. Focused component tests passed 4/4; frontend typecheck and lint passed after these changes.
- Remaining: readiness and reason projection/Submit gate, discount modes, policy overlap and full calculation matrix, review assignment/frozen-route projection, remaining UI conflict/loading coverage, Playwright lifecycle journey, complete verification gates, sanitized legacy quotation/data review, and Business/Finance sign-off.

### Verification refresh (2026-09-27)

- `dotnet build backend/TanErp.slnx --no-restore -m:1`: passed, 0 warnings and 0 errors.
- `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-build --no-restore -m:1`: passed, 255/255.
- Estimate PostgreSQL integration tests (`FullyQualifiedName~EstimateEndpointsTests`): passed, 19/19, including submitted cancellation permissions/route closure/replay and concurrent cancellation (one success, one conflict, one audit event).
- Frontend `npm run generate:api`, `npm run typecheck`, `npm run lint`, and full `npm run test`: passed; Vitest 560/560. Estimate lifecycle/card focused tests: 10/10.
- Frontend `npm run build -- --webpack`: passed. Next.js reported the existing multiple-lockfile workspace-root warning; compilation, type check, and route generation succeeded.
- `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` and a retry with `RunConfiguration.MaxCpuCount=1` both remained in the full Integration Tests stage without output for over three minutes and were interrupted. The full backend solution test gate is therefore not confirmed; use the focused passing results above as scoped evidence only.
- `git diff --check`: passed for the working tree at verification time.

### Verification attempt after approval/revision changes

- `dotnet build backend/TanErp.slnx --no-restore -m:1`: passed, 0 warnings/errors. `TanErp.UnitTests`: 254 passed. Focused PostgreSQL approval → quotation → revision journey: 1 passed; focused OpenAPI contract regeneration/equality test: 1 passed.
- Full `TanErp.IntegrationTests` run: 259 passed, 4 failed. Failures included PostgreSQL connection timeouts from concurrent integration fixtures and an OpenAPI file mismatch caused by the newly added endpoints. Regenerated `contracts/openapi/tan-erp.v1.json`; the focused OpenAPI test passes after regeneration. Full suite has not yet been rerun after that correction.
- Frontend `typecheck` and `lint`: passed. Full Vitest: 555 passed, 1 failed in unrelated `customers/components/customer-editor.test.tsx` (duplicate confirmation modal title not found). The estimate test files passed within that run.
- Default Turbopack build could not bind a local port in this environment, including with sandbox escalation. `npm run build -- --webpack` completed successfully.
- `npm run verify` initially stopped at `check:api` because the installed TypeScript 7.0.2 API is incompatible with `openapi-typescript`; using the CLI's expected TypeScript 5.9.3 under ignored `node_modules` allowed `npm run generate:api` to complete. The OpenAPI integration test confirmed the schema against the API, and generated TypeScript includes the new approval/revision contracts. The full `verify` script has not been rerun end-to-end after updating these generated files.

### Current estimate hardening verification (2026-09-27)

This entry supersedes earlier “remaining” notes only where it lists newly completed work. It does not replace the full release gates below.

- Added Estimate domain coverage for distinct margin/markup formulas, percentage and fixed overhead allocation (including rounding remainder), all three tax modes, excessive discount rejection, zero-denominator readiness, and financial-edit invalidation with snapshot preservation. `EstimateTests` passed 24/24.
- Added PostgreSQL coverage for concurrent approval decisions, revoked reviewer authority, and rejection of a Draft write against an Approved revision. Concurrent decisions now produce one success and one stable 409 conflict; only one append-only decision is persisted. Estimate API tests passed 23/23.
- The concurrent decision test exposed database concurrency exceptions returning 500. `EstimateStore.ReviewAsync` now maps EF concurrency plus PostgreSQL unique/serialization/deadlock conflicts to `ESTIMATE_VERSION_CONFLICT`; the concurrency integration test passed after this change.
- These are focused gates only. Full backend solution tests, full current frontend verification, new/sanitized database migration rehearsal, remaining policy overlap/cost-staleness cases, reviewer queue projection, two-user Playwright lifecycle, and Business/Finance sign-off remain unverified.

### Release gate refresh after conflict handling (2026-09-27)

- `dotnet build backend/TanErp.slnx --no-restore -m:1`: passed, 0 warnings/errors.
- `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-build --no-restore -m:1`: passed 266/266. Estimate domain subset: 24/24.
- `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-build --no-restore -m:1 -- RunConfiguration.MaxCpuCount=1`: passed 269/269 after regenerating the OpenAPI snapshot. The previous full run's cross-tenant migration test timeout also passed when isolated (1/1).
- `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj --no-build --no-restore -m:1`: passed 3/3 in the preceding solution run. Estimate API subset: 23/23.
- OpenAPI runtime snapshot update and parity rerun both passed 1/1. `npm run generate:api` succeeded after the contract refresh.
- Frontend current-tree `npm run typecheck`, `npm run lint`, `npm test -- --reporter=dot`, and `npm run build -- --webpack` all passed; Vitest passed 562/562 across 130 files. Production build reports the existing multiple-lockfile workspace-root warning.
- After updating the OpenAPI snapshot, the combined `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1 -- RunConfiguration.MaxCpuCount=1` passed: Architecture 3/3, Integration 269/269, Unit 266/266. Frontend was rerun after API type generation: typecheck/lint passed, Vitest 562/562 across 130 files, and `npm run build -- --webpack` passed with the existing multiple-lockfile warning.

### Cost provenance and provisional readiness follow-up (2026-09-27)

- Added estimate cost snapshots for origin, cost source, source reference, evidence file, source reason, and provisional reason/note. The additive `20260927141500_AddEstimateCostProvenance` migration classifies historical rows from their item link only; it does not invent historical provenance or reasons. Historical manual costs and catalog records without evidence remain provisional and cannot be submitted until a reason is supplied.
- Readiness now blocks an unexplained provisional cost with `ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED`; explained provisional costs require reviewer attention. Problem Details maps the blocking condition to 422 with Thai/English messages. Source/evidence and provisional reasons are included in cost projections, calculation snapshots, and calculation input hash.
- Catalog responses now include source/evidence metadata. Cost resolution returns `ITEM_COST_STALE` (409) when matching published costs exist but none are effective on the estimate date; no price fallback is used.
- Verification on this follow-up: backend build passed with 0 warnings/errors; Unit tests 267/267; Estimate API tests 24/24; stale cost resolver test 1/1; synthetic catalog provenance pilot 1/1; OpenAPI runtime parity 1/1; frontend typecheck/lint passed and Vitest 562/562. `npm run build -- --webpack` also passed after the final UI changes. Full solution test was attempted twice, each produced Architecture 3/3 then remained silent in Integration for over three minutes and was interrupted. `npm run check:api` regenerated the TypeScript file but returned a diff against HEAD because the generated schema/types are intentional uncommitted changes in this working tree; rerunning `npm run generate:api` succeeded. Playwright E2E was not rerun.
- Remaining release work: full solution test + current frontend webpack build, cost/policy overlap and calculation replay/reproduction matrix, review queue/frozen route projection, two-user Playwright journey, sanitized legacy quotation/migration rehearsal, UAT, real pilot data and Business/Finance sign-off.

### Reviewer queue projection follow-up (2026-09-27)

- Added `GET /api/v1/estimates/review-queue`, scoped to the authenticated reviewer membership and pending assigned approval steps. Search and stable paging return structured customer/opportunity/branch/requester/reviewer fields without frontend joins.
- Queue rows include the frozen policy/version, route scope and hash, calculation input/snapshot hashes, readiness exceptions, every cost component's source/evidence/provisional metadata, and revision-to-revision total/BOQ work-item delta. The frontend uses shared `DataTable`, `ListToolbar`, `Modal`, and `useListState`; sidebar visibility requires `estimates.approve`.
- Verification: backend API build passed (0 warnings/errors); PostgreSQL estimate approval journey passed 1/1 with reviewer-only assignment, evidence, and readiness assertions; OpenAPI runtime update/parity passed 1/1. Frontend generated API, typecheck, lint, and reviewer queue component test passed. `git diff --check` passed after this change.
- Added two-user Playwright steps for Submit → Return → Edit/Recalculate → Resubmit → Independent Approve → Issue Quotation → Acceptance, followed by New Revision. `playwright test --list` recognizes the scenario; browser execution is pending.
- Latest frontend gates passed: typecheck, lint, Vitest 564/564 across 131 files, and `npm run build -- --webpack`; Next.js emitted its existing multiple-lockfile workspace-root warning. Sidebar permission and review queue component tests passed (9/9 together).
- Full current backend verification passed after adding the queue: solution build 0 warnings/errors; Architecture 3/3, PostgreSQL Integration 271/271, Unit 267/267. Full frontend verification passed: typecheck, lint, Vitest 564/564, and webpack production build. The production build reports the existing multiple-lockfile workspace-root warning.
- Browser execution of the two-user Playwright journey remains pending because no web app was listening at `localhost:3005`; sanitized database rehearsal, UAT, pilot master-data confirmation, and Business/Finance policy sign-off also remain release gates.

### Fixed Price override verification (2026-09-27)

- Fixed Price draft updates now require `estimates.override-price` resolved for the Estimate Branch and a non-empty reason code. Rejections occur before draft mutation. The reason and override trigger are included in readiness/calculation evidence; the reviewer queue shows the work item, fixed price, and reason. Customer-facing Quotation contracts do not expose this internal field.
- Added additive nullable `selling_rule_reason_code` storage so legacy rows remain readable; readiness blocks legacy Fixed Price rows without an explanation. Added bilingual validation/error messages, API/OpenAPI contracts, and workspace/reviewer presentation.
- Focused verification passed: Domain Estimate tests 29/29; `EstimateEndpointsTests` 30/30; PostgreSQL Fixed Price/review filter 4/4; end-to-end calculation→submit→review-queue scenario 1/1; OpenAPI regeneration/parity 1/1; frontend estimate tests 11/11, typecheck, lint, Vitest 567/567, and webpack production build. Full solution build passed with 0 warnings/errors.
- Full solution test rerun encountered integration fixture PostgreSQL connection timeout/resource contention; the affected SiteStore concurrency test passed alone (1/1). A controlled full Integration rerun capped xUnit at two parallel threads, but Testcontainers could not connect to Docker (`unix:///var/run/docker.sock` and `~/.docker/run/docker.sock`; socket reported “Can't assign requested address”), so 270/277 fixtures failed to initialize and 7 tests passed. This is an environment-blocked suite, not passing evidence; the focused Estimate PostgreSQL/API tests above remain the applicable code evidence.
- UAT-EST-021 is now specified and classified as a critical exit scenario. It has not been executed by authorized pilot roles. Browser E2E, sanitized migration rehearsal, pilot data readiness, remaining stale/ambiguous Estimate API and historical reproduction tests, and Business/Finance/Security sign-offs remain open release gates.

### Estimate gap closure follow-up (2026-09-27)

- Added `UpdateDraft_WithUnresolvableCatalogCost_ReturnsStructuredConflictWithoutWriting` for price mismatch, stale effective period, and ambiguous precedence. It asserts each stable Problem Details code, unchanged revision row version/BOQ, and no Calculation Snapshot append.
- Added `FinancialEdit_MarksCalculationOutdatedAndPreservesPriorSnapshot` to the Estimate API suite. It saves a calculation, edits a financial input, then checks that the revision is marked outdated while the prior snapshot/totals remain immutable and history has only the original snapshot.
- Extended `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` to add future-effective policy v2, recalculate with unchanged inputs while v1 is still effective, and compare policy IDs, input hash, totals, and the first history JSON before/after. This is a historical snapshot retention/recalculation regression; it does not claim the product has a separate auditor-triggered replay endpoint.
- These backend integration tests compile successfully: `dotnet build backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-restore -m:1` passed with 0 warnings/errors. Runtime remains unverified because `docker info` reports no daemon at `~/.docker/run/docker.sock`; do not count these new API cases as passed until rerun with PostgreSQL/Testcontainers available.
- Added `EstimateWorkspaceDrawer` component tests for duplicate-save locking, 409 input retention, and translated stale/ambiguous/missing cost messages. Added lifecycle cancellation test for required reason, confirmation modal, and pending-state lock. Focused estimate components passed 9/9. Current frontend verification passed typecheck, lint, Vitest 572/572 across 133 files, and `npm run build -- --webpack` (with the existing multiple-lockfile workspace-root warning).
- Added UAT-EST-022 for idempotent calculation replay and historical policy/cost comparison. It remains unexecuted; browser E2E, migration rehearsal, pilot data inspection, UAT, and Business/Finance/Security sign-off remain open.
- Added pure-domain historical reproduction test `HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies`: it calculates and approves a revision under v1, makes v2 effective, clones the immutable BOQ, recalculates the clone at the original calculation time with original policies, and verifies matching totals while the original approved snapshot stays unchanged. Focused test passed 1/1. This proves deterministic domain reproduction from retained revision inputs and policy objects; API/database history verification is still pending PostgreSQL availability.
- After adding the domain reproduction test, current `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings/errors; full `TanErp.UnitTests` passed 272/272 and `TanErp.ArchitectureTests` passed 3/3. Full IntegrationTests still cannot start because the Docker daemon is unavailable; no claim is made for the newly added API cases.

### Local PostgreSQL verification refresh (2026-09-28)

- Started an isolated PostgreSQL 18.4 instance under `/private/tmp` on `127.0.0.1:55439`; no repository or production database was used. Added an opt-in `TANERP_TEST_POSTGRES_CONNECTION` path to `EstimateEndpointsTests`; normal runs still use the existing Testcontainers setup.
- `EstimateEndpointsTests` passed 33/33 against PostgreSQL. This includes the new stale/ambiguous catalog-cost non-write cases, financial-edit invalidation and snapshot retention, calculation history across a future-effective policy, and the Submit → Return → Recalculate → Resubmit → Independent Approve → Quotation → New Revision journey.
- The first run exposed an outdated test fixture: it submitted calculation version 1 after a second calculation and expected the post-return calculation version not to advance. The test now submits the latest response version and expects the monotonic version 3. The corrected class run passed 33/33.
- Test database teardown now clears only that database's Npgsql pool and drops only the per-run GUID-named database with PostgreSQL `WITH (FORCE)`. Leftover databases from the failed initial run were removed from the isolated instance; the temporary PostgreSQL server was stopped after verification.
- Empty-database migration rehearsal passed on `tan_erp_rehearsal`: EF applied 39 migrations through `20260927153247_AddEstimateFixedPriceReason`. Verified `estimates.estimate_calculation_snapshots`, `estimates.estimate_approval_requests`, and `estimates.estimate_work_items.selling_rule_reason_code` exist. This does not verify migration compatibility with historical production-like rows.
- Still open: full IntegrationTests suite (other fixtures continue to require Docker), sanitized legacy database rehearsal and historical quotation audit, two-user browser execution, pilot data readiness, UAT-EST-001–022, and Business/Finance/Security sign-off. No approved production policy values or real pilot prices were introduced.

### Full local code-gate refresh (2026-09-28)

- `dotnet build backend/TanErp.slnx --no-restore -m:1`: passed with 0 warnings and 0 errors.
- `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-build --no-restore -m:1`: passed 272/272; `TanErp.ArchitectureTests` passed 3/3. The first sandboxed VSTest attempt could not bind its local socket; rerunning with the permitted local socket access produced the passing results.
- `EstimateEndpointsTests` was rerun after narrowing teardown to the individual temporary database pool and passed 33/33. The temporary PostgreSQL server was stopped after the run.
- Frontend `npm run typecheck`, `npm run lint`, `npm run test -- --reporter=dot` (572/572 across 133 files), and `npm run build -- --webpack` all passed. Next.js reported the repository's existing multiple-lockfile workspace-root warning; Vitest logged existing jsdom canvas and React `act(...)` warnings, with no failed tests.
- Generated OpenAPI TypeScript into `/private/tmp/tan-erp-openapi-generated.ts` and compared it with `frontend/src/generated/api/tan-erp.v1.ts`; `diff -u` returned no differences.
- Playwright was not executed: the current local preflight found no PostgreSQL response on 5432, no Auth Emulator on 9099, no API on 5005, and no frontend on 3005. Firebase CLI is not installed locally and `docker info` exits 1, so the documented E2E stack cannot be started in this environment. The two-user journey remains authored but unverified.
- These results close the local code gates that do not require the whole Testcontainers suite or live E2E stack. Full IntegrationTests, sanitized legacy rehearsal, pilot data, UAT, and Business/Finance/Security sign-off remain open.

### Quotation approval snapshot integrity follow-up (2026-09-28)

- A regression deliberately changed the persisted `EstimateApprovalSnapshot.CalculationSnapshotHash` after approval. Before the fix, the API still issued a quotation (the expected RED result), showing that checking only the revision's Approved status and non-empty inline snapshot fields did not verify the immutable approval record.
- `IssueQuotationAsync` now requires the current calculation snapshot and approval snapshot rows to exist and match the current revision/version. It verifies the approval input hash and calculation snapshot hash, and compares both inline revision JSON snapshots with their persisted JSONB records structurally so PostgreSQL JSONB formatting does not affect the comparison. The quotation stores the verified approval snapshot hash.
- The regression now confirms a mismatched approval hash returns `409 ESTIMATE_INVALID_STATE`, creates no quotation, leaves the opportunity in Estimating, and does not advance quotation numbering; restoring the hash allows the same request to issue the quotation.
- Verification on the same isolated PostgreSQL 18.4 instance: focused regression passed 1/1; `EstimateEndpointsTests` passed 33/33; solution build passed with 0 warnings/errors; Unit 272/272; Architecture 3/3. Full IntegrationTests and live Playwright/UAT/release sign-offs remain open as stated above.

### Two-user estimate browser journey and pricing-rule boundary fix (2026-09-28)

- The first browser run reproduced a draft-save rejection: the workspace uses percent values for Margin/Markup (`25` means 25%), while the API/domain contract stores ratios (`0.25`). The API trace confirmed the form sent `25` and received `400 ESTIMATE_INPUT_INVALID`.
- The workspace now converts Margin/Markup ratios to percentages when loading/resetting API revisions and converts percentages to ratios when saving drafts. Fixed Price remains an absolute amount. A drawer regression verifies Margin `0.25 ↔ 25`, Markup `0.3 ↔ 30`, and Fixed Price `1250 ↔ 1250`; focused drawer tests passed 5/5.
- The two-user journey required a reviewer with one active membership in the estimate's organization and read access to the Opportunity/Estimate. For the local Test API, enable `SeedTestData=true` and `SeedDedicatedEstimateReviewer=true`; the second flag creates a dedicated reviewer membership with `opportunities.read`, `estimates.read`, and `estimates.approve`. Default integration-test seeding remains unchanged. `scripts/seed-emulator-users.mjs` creates only the additional `.example.test` identity in the local Auth Emulator.
- Playwright command: `PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e -- e2e/official-estimate.spec.ts --project=chromium`. Result: passed 1/1 in 25.3 seconds against the Test API, local Auth Emulator, and a clean disposable PostgreSQL database. The database was dropped after the run. Journey: customer/site/opportunity → Ready survey → estimate BOQ save/calculate → Submit → Return → edit/recalculate/resubmit → independent Approve → Issue Quotation → customer Acceptance → New Revision.
- Post-fix local gates: solution build passed (0 warnings/errors); Unit 272/272; Architecture 3/3; focused Estimate API tests 33/33 on PostgreSQL; frontend typecheck, lint, full Vitest 573/573, and webpack production build passed. Full IntegrationTests first ran 279/280 with a PostgreSQL SSL handshake failure in `OpportunitySiteMigrationTests.Migration_ExposesExactColumns`; the isolated test passed 1/1 and the second full run passed 280/280 in 7m04s.
- Remaining release gates: sanitized legacy database/quotation audit; pilot master-data inspection; UAT-EST-001–022 with approved roles and responsive/accessibility checks; signed Business/Finance/Security policy decisions. The browser journey is automated technical evidence, not an authorized business UAT or price-policy sign-off.

### Browser usability follow-up (2026-09-28)

- Extended the two-user Chromium journey to revisit the estimate BOQ workspace in English, emulate `prefers-reduced-motion: reduce`, check keyboard focus stays within the open drawer, and verify no horizontal document overflow at a 320px viewport. This is automated UI evidence for those checks; tablet/200% zoom and full keyboard-only task completion remain for UAT.
- Browser journey passed 1/1 in 24.4 seconds against the local Test API, Firebase Auth Emulator, and a clean uniquely named PostgreSQL database. The database was dropped and the API/frontend processes started for this run were stopped. Existing PostgreSQL and Auth Emulator services were left running.
- While exercising the second-user review queue, the browser exposed a missing `estimates.calculationVersion` translation. Added “Calculation version” / “รุ่นการคำนวณ” to the English and Thai catalogs. The full frontend suite now passes: typecheck, lint, Vitest 573/573, and webpack production build. The build retains the pre-existing multiple-lockfile workspace-root warning; Vitest logs existing jsdom canvas warnings.
- The `modern-web-guidance` skill search was attempted as required before the browser/frontend change, but network access hung and the offline npm cache did not contain the package. No guidance was retrieved; local repository patterns were used.
- Remaining: authorized-role UAT scenarios and signed policy decisions, tablet/200% zoom checks, sanitized historical quotation/migration audit, and pilot data readiness. This browser run does not close those release gates.

### Final implementation verification refresh (2026-09-28)

- Fresh `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings and 0 errors. The full solution test command passed Architecture 3/3 and Unit 272/272; 273 IntegrationTests could not initialize their Testcontainers Docker dependency (7 tests completed) because neither Docker socket was available. Retrying with `DOCKER_HOST=unix:///Users/syaco/.docker/run/docker.sock` produced the same environment failure.
- The relevant PostgreSQL-backed `EstimateEndpointsTests` class passed 33/33 against a temporary PostgreSQL 18.4 cluster on port 55439. The cluster and its temporary data directory were stopped and removed.
- Fresh frontend checks passed: `npm run typecheck`, `npm run lint`, `npm test` (573/573), and `npm run build -- --webpack`. The build reports the existing multiple-lockfile workspace-root warning; Vitest reports existing jsdom canvas warnings.
- A fresh Playwright rerun was not possible after Docker Desktop's engine stopped: the Docker socket was missing and the bundled app launcher exited with an OS error. The preceding recorded browser run passed 1/1, including the English workspace, keyboard focus, 320px overflow, and reduced-motion checks.
- Added a successful quotation idempotency replay to `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot`: repeating the issue request with the original key and row versions returns the existing quotation identity, leaves one quotation row and the sequence unchanged, and records only one Estimating → Proposed stage transition. Fresh PostgreSQL-backed focused run passed 1/1. UAT-EST-011/018 no longer have this automated evidence gap, though authorized-user UAT remains unrun.
- Release remains pending resolution/explicit deferral of UAT-EST-003/006/012/013, authorized-role UAT and policy sign-off, pilot data inspection, and a sanitized historical database/quotation review when applicable. Tablet/200% zoom remains unverified.

### TEST_ONLY estimate workspace fixture (2026-09-28)

- Added an opt-in test fixture with a TEST_ONLY Customer, Site, estimating-stage Opportunity, Ready survey revision, Draft estimate, two BOQ work items, provisional sample material/labor costs, and the existing 30% Margin/7% VAT test policies. Configure `ASPNETCORE_ENVIRONMENT=Test`, `SeedTestData=true`, and `SeedEstimateDemoData=true`; the API's existing environment guard prevents the seeder from running in Production.
- The focused PostgreSQL integration test verified the fixture is absent outside the Test environment, creates the expected Draft estimate and BOQ in Test, and remains idempotent on repeat seed. Passed 1/1 on a disposable PostgreSQL 18.4 cluster; the cluster was stopped and removed.
- `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings and 0 errors. Full solution tests were not rerun for this seed change; Testcontainers remains unavailable in the current environment. No seed values are business-approved prices.
- The initial Item Catalog fixture had 3 active TEST_ONLY materials. It was expanded to 75 catalog items across all six types and three statuses so Item Catalog search, type/status filters, category/brand API filters, sorting, and pagination can be exercised; see [Item Master verification](item-master-estimate-catalog-verification.md) for the current seed shape and focused test.
- The expanded catalog and Estimate demo fixtures are seeded into the configured local `tan_erp` database with the guarded Test startup. The current database row counts are verified in the Item Master verification record; PostgreSQL remains running so the local app can read the data.

### Estimate Workspace layout and save/calculation refactor (2026-09-28)

- The production Workspace now separates grouped BOQ rows from the selected work-item inspector. The inspector reuses the existing cost editor and shared form tabs for details, costs, and pricing. Hidden tabs, filtered rows, and collapsed sections retain form values; stable client identities preserve selection through reordering and successful save responses. Mobile navigation scrolls only its workspace pane, keeping the drawer heading, view tabs, and save actions visible.
- Existing section/work-item actions, templates, quick cost presets, bilingual descriptions, provisional-cost reasons, pricing rules, discount modes, live totals, keyboard save, unsaved-close confirmation, readiness navigation, and official-snapshot CSV export remain available. New messages have 29 matching Thai/English keys under `estimates.workspace`.
- The visible Item Catalog action targets the selected work item. This refactor makes no edits to the catalog modal, catalog client, or catalog query hook; pre-existing working-tree changes to those files are retained. Multi-selection and authoritative cost-record references continue through the existing catalog contract, including cost-version conflict handling.
- Save and calculate validates the form, saves changed BOQ first, and calculates with the returned revision version. A failed save stops the calculation. A successful save followed by a failed calculation retains saved BOQ and unapplied discount settings for retry. Draft save no longer converts a displayed 10% discount into 0.1% or marks unapplied discount settings clean. Query caches receive authoritative mutation responses; an open editing session does not silently adopt a refetched revision token.
- No backend, generated API contract, or database change was introduced by this refactor. Its additional schema properties are client-only identities and are excluded from draft requests. Existing lifecycle/quotation actions remain outside the Workspace.
- Verification: frontend typecheck and lint passed; the full suite passed 587/587 across 135 files with two workers. After the final interaction changes and an additional refetch-conflict regression, focused estimates tests passed 76/76 across 14 files. `npm run build -- --webpack` passed. Default Turbopack build could not bind an internal worker port in the restricted environment, so the supported webpack build was used.
- Browser verification: Item Catalog save/reload/cost-snapshot/conflict journey and the estimate Submit → Return → Recalculate → Independent Approve → Quotation → Customer Acceptance → New Revision journey passed together (2/2). After the final mobile pane-scroll fix, the estimate journey passed again (1/1), including English, reduced motion, keyboard focus, 320px overflow checks, visible BOQ navigation, and drawer heading retention. Screenshots are build/test artifacts under `frontend/test-results/`.
- Backend `dotnet build` passed with 0 warnings/errors; UnitTests passed 272/272 and ArchitectureTests passed 3/3. The full integration run encountered `NpgsqlException: Received unknown response H for SSLRequest (expecting S or N)` during fixture database initialization before estimate cases could run; the stalled run was stopped. Do not count full Backend IntegrationTests as passed or treat these local results as production sign-off.

#### Estimate Workspace responsive layout follow-up (2026-09-28)

- BOQ section headings now use separate identity and action rows; bilingual names occupy the available width without their default bottom margin.
- The desktop inspector uses a responsive width between 360px and 460px, with reduced BOQ padding and shrink-safe pane containers. Inspector headings wrap long names without displacing the action controls.
- Discount controls wrap within the footer on narrow screens. Item Catalog internals and existing estimate capabilities remain unchanged by this follow-up.
- Verification: frontend lint, production webpack build, typecheck, 14 estimate test files (76 tests), and the official-estimate Chromium journey passed. The journey checks the 320px mobile viewport; desktop and mobile screenshots were visually inspected.

#### Estimate Workspace compact density (2026-09-28)

- Reduced workspace header, financial HUD, toolbar, section, inspector, and footer spacing; retained all financial explanations and existing actions.
- Toolbar and inline actions use the central compact Button size. Desktop inputs use the locally scoped 36px control token; mobile inputs retain the normal touch size. Save/calculation actions retain the standard Button size. UI density overrides apply only to the workspace, outside Item Catalog internals.
- Verification: frontend lint, webpack production build, typecheck, 14 estimate test files (77 tests), and the official-estimate Chromium journey passed. The updated desktop screenshot was visually inspected.

### UAT-EST-003 catalog unit validation follow-up (2026-09-29)

- Catalog Cost Components now require the selected unit to match the active Item's base unit used by `CostResolver`. An incompatible unit returns `422 ESTIMATE_UNIT_INVALID` before the draft is written; the Work Item's selling unit remains independent. Item/Shared Unit Conversion is not yet captured in an immutable Estimate conversion snapshot and remains deferred.
- Added the `unit-mismatch` case to `UpdateDraft_WithUnresolvableCatalogCost_ReturnsStructuredConflictWithoutWriting`, including stable error code and unchanged draft/version assertions. Added bilingual Problem Details resources and aligned the Field Catalog, API contract, UAT scenario, and completion plan.
- Fresh `dotnet build backend/TanErp.slnx --no-restore` passed with 0 warnings/errors; UnitTests passed 284/284. The focused API regression passed 4/4; after the UAT-EST-006 sequential-step change, the full `EstimateEndpointsTests` class passed 38/38 using disposable PostgreSQL because Docker was unavailable. The test fixture created and dropped its own databases, and both temporary clusters were stopped and removed afterward.
- Frontend `estimate-workspace-drawer.test.tsx` passed 12/12; `npm run typecheck`, `npm run lint`, and `npm run build -- --webpack` passed after adding the `ESTIMATE_UNIT_INVALID` translation mapping. This verifies localized feedback and preservation of edited input, not keyboard focus to a specific component field.

### UAT-EST-006 sequential approval lifecycle (2026-09-29)

- Review decisions now enforce the lowest pending step sequence. Approving an intermediate step leaves the request open and Estimate submitted; the review queue exposes only the next pending sequence. The final approval closes the request, approves the revision, and stores all step decisions in the immutable approval snapshot. A returned decision still closes the route immediately.
- `ReviewEstimate_SequentialApprovalRequiresEachAssignedStep` verifies two distinct reviewers, next-step queue visibility, no early approval snapshot, and final snapshot decision history. The focused journey passed 1/1 and the full `EstimateEndpointsTests` class passed 38/38 on a disposable PostgreSQL cluster.
- Fresh backend build passed with 0 warnings/errors; UnitTests passed 287/287 and ArchitectureTests 3/3. The complete integration suite could not run because Docker is unavailable; the full Estimate API class ran against disposable PostgreSQL and passed 39/39, including the opt-in profile journey. Authorized-user UAT and policy sign-off remain, so this is not UAT-EST-006 release completion.

### UAT-EST-006 test-only route profile and reviewer projection (2026-09-29)

- Added opt-in `TEST_ONLY-TH-EST-V1`, guarded by Test host environment plus explicit configuration. It combines amount, margin, discount, fixed-price override, and provisional-cost signals, selects the strongest sequential role route, and freezes policy hash, thresholds, trigger values, reviewer assignments, and calculation hashes. Production continues to use the bootstrap checker.
- Review Queue API now projects trigger codes and actual/threshold values from the frozen route; the bilingual reviewer detail panel renders them. At the time of this entry, custom-work-item detection was not connected to estimate readiness; see the later Custom Work Item verification below.
- Verification: policy unit tests are included in UnitTests (287/287 passed); the focused profile journey and Estimate API class passed on disposable PostgreSQL (40/40); Review Queue component test passed 1/1; frontend lint and typecheck passed; `npx next build --webpack` passed. The default Turbopack build failed because its CSS worker could not bind a local port in the sandbox. `npm run check:api` reports contract/type drift elsewhere in this modified working tree; generated API types were refreshed from the current OpenAPI contract. The rest of the integration suite remains unavailable while Docker is stopped.

### UAT-EST-012 organization and branch isolation matrix (2026-09-29)

- `EstimateActions_CrossOrganizationAndBranchAccessReturnNotFoundWithoutDisclosure` exercises read, draft update, calculate, and approve attempts from another organization and from a different branch in the same organization. Each denied request returns 404 without estimate number or grand-total disclosure; the test also verifies denied writes do not alter the draft revision, calculation snapshots, approval status, or decisions.
- The matrix exposed that Estimate permission resolution rejected branch-scoped roles before checking the Estimate branch, returning 403 and disclosing active organization membership. `RequestAccessResolver` now accepts branch-scoped permissions only for Estimate read/update/approve operations, and Estimate application handlers verify the caller membership branch against the resource branch before returning data or mutating state.
- Verification: focused isolation case passed 1/1 and the complete Estimate API class passed 40/40 against disposable PostgreSQL after the access-scope change. Backend build passed with 0 warnings/errors. Broader integration suite remains blocked by unavailable Docker; authorized-user UAT remains open.

### Custom Work Item catalog link and readiness verification (2026-09-29)

- Draft Work Items can link to an active, cost-capable Item Master record in the estimate's organization and branch. The API returns the linked Item's structured identity and the estimate preserves code/name snapshots. A Work Item without a catalog link must include a custom-item reason code and note; missing reason data blocks readiness, while a complete custom item adds the `CUSTOM_WORK_ITEM` approval trigger.
- The focused Estimate API suite passed 40/40 against disposable PostgreSQL after adding the catalog-link and reason validation. `PilotEstimate_CatalogCostsResolveAndSnapshotSurvivesNewCostVersion` passed 1/1 with the linked Item projection assertion. Backend solution build passed with 0 warnings/errors; UnitTests passed 287/287 and ArchitectureTests passed 3/3.
- Frontend lint, typecheck, and webpack production build passed; the three focused Estimate Work Item/catalog/mapper test files passed 19/19. OpenAPI and bilingual message JSON parsed successfully, and `git diff --check` passed. A later full `dotnet test backend/TanErp.slnx --no-restore -m:1` attempt passed ArchitectureTests 3/3 but reported an OpenAPI snapshot serialization mismatch (`E` vs `e` in a floating-point exponent) and was stopped while IntegrationTests continued without output. The full integration suite is therefore not verified by that attempt.
- This supersedes the earlier UAT-EST-006 note that custom-work-item detection was not connected to readiness. The test profile verifies implementation behavior only: production approval policy values, authorized-user UAT, and Business/Finance/Security sign-off remain open.

### OpenAPI snapshot comparison follow-up (2026-09-29)

- The full solution test attempt exposed a formatting-only failure because generated and committed OpenAPI JSON were serialized to strings before comparison (`E` versus `e` in a floating-point exponent). The assertion now uses `JsonNode.DeepEquals` to compare the parsed documents structurally.
- `TanErp.IntegrationTests` builds with 0 warnings/errors after the assertion change. UnitTests pass 287/287 and ArchitectureTests pass 3/3. Runtime verification of the OpenAPI test and full IntegrationTests remains pending because Docker is unavailable; do not treat the assertion change as a passing integration run.

### OpenAPI contract parity and generated client refresh (2026-09-29)

- The focused OpenAPI test showed the committed contract also lagged the current API. The OpenAPI test now uses a non-connecting database configuration because it exercises document generation only, and compares parsed JSON structurally. It passed 1/1 without Docker.
- Refreshed `contracts/openapi/tan-erp.v1.json` from the current API and generated `frontend/src/generated/api/tan-erp.v1.ts`. Marked the linked Item Master response `id`, `code`, and `nameTh` as required in the response DTO and added an OpenAPI assertion for those required fields, keeping FE contract types aligned with the API projection.
- Custom Work Item test fixtures now carry the required reason fields. The full frontend suite passed 597/597 across 135 files; frontend typecheck, lint, and webpack production build passed. Backend solution build passed with 0 warnings/errors; UnitTests passed 287/287, ArchitectureTests 3/3, and the focused OpenAPI contract test passed 1/1.
- Full IntegrationTests still require Docker-backed fixtures and were not run to completion. Authorized-user UAT, production policy sign-off, pilot data inspection, and migration rehearsal remain release gates.

### Full backend gate refresh (2026-09-30)

- Docker Desktop was running; the sandboxed attempt could not access its socket, so the full test command ran with authorized Docker socket access.
- `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings and 0 errors.
- `dotnet test backend/TanErp.slnx --no-restore -m:1` passed: IntegrationTests 289/289 (8m37s), UnitTests 287/287, and ArchitectureTests 3/3.
- This supersedes the immediately preceding environment-blocked full-test notes. It confirms the current backend code-test gate; authorized-user UAT, production policy sign-off, pilot data inspection, sanitized legacy migration/quotation audit, and release operations remain open.

### Browser E2E follow-up (2026-09-30)

- The browser fixture now supplies the required organization legal name, valid 13-digit tax identifier, tax branch code, and an active primary billing address before issuing a quotation. The customer save path also exposed a missing `common.feedback.saveSuccess` message; the Thai and English keys are now present.
- The first follow-up attempts were misrouted because Playwright kept its default base URL on the existing frontend at port 3005. The runner now uses `PLAYWRIGHT_TEST_BASE_URL=http://localhost:3000`; a read-only browser smoke confirmed `/api/v1/me` reached `http://localhost:55005` and returned 200. The full current Estimate journey then passed Chromium 1/1 in 57 seconds against Test API, Firebase Auth Emulator, and a fresh disposable PostgreSQL database. It covered Customer billing setup, Ready Survey, BOQ, Calculate, Submit, Return, Recalculate/Resubmit, Independent Approve, Quotation issue/acceptance, New Revision, English workspace, keyboard focus, reduced motion, and 320px overflow.
- The misrouted attempts created two additional synthetic Customers, Sites, Opportunities, Surveys, and Estimates in the existing local `tan-erp-postgres` database. They remain there because removing their estimate/audit graph directly would bypass domain behavior. The successful run created its records only in the disposable database (one Customer, Site, Opportunity, and Estimate); that container was stopped and removed after verification.

### Quotation API response allowlist (2026-09-30)

- The `Issue Quotation` response now has a focused integration assertion that compares the serialized JSON property set with the current `QuotationResponse` allowlist. This protects the internal issuance projection from accidentally including cost, margin, internal notes, approval trigger, or threshold fields. It does not verify a customer-facing artifact; Preview/Export/PDF remain deferred to CP-04.
- `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` passed 1/1 against disposable PostgreSQL. `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings/errors. The latest full-suite recheck and its intermittent PostgreSQL fixture startup failure are recorded in [Site Survey Verification](site-survey-verification.md#5-full-suite-recheck-2026-09-30).

### Review decision concurrency follow-up (2026-09-30)

- A diagnostic full run passed Architecture 3/3 and Unit 288/288, but Integration 290/291: `ReviewEstimate_ConcurrentDecisionsWithSameVersionOnlyOneSucceeds` returned 200 for Approve and 403 for the competing Return. Both requests could read the old Estimate version before one committed, then the other read the already-closed approval step under READ COMMITTED and reported permission denial.
- `EstimateStore.ReviewAsync` now takes a transaction-scoped PostgreSQL advisory lock keyed by Organization/Estimate before replay and state reads, reusing the existing transaction-lock pattern. Review commands for the same Estimate read the preceding committed decision before checking RowVersion; other Estimates remain independent. Membership permission, branch isolation, assigned-step authority and Maker–Checker checks remain in place.
- The race regression now covers both request orders and asserts one success, one 409 `ESTIMATE_VERSION_CONFLICT`, and exactly one persisted decision. Focused Review/Cancel integration tests passed 5/5.
- After the fix, the authored Chromium journey passed 1/1 in 45.6 seconds on dev frontend (`localhost:3005`), Test API (`localhost:5005`), Auth Emulator and the existing local TEST_ONLY database. It covers Submit → Return → edit/recalculate/resubmit → independent Approve → issue/accept Quotation → New Revision, English workspace, 320px overflow, keyboard focus and reduced motion. Synthetic journey records remain in the local database; no production data was used.
- Frontend lint, typecheck, OpenAPI generated-client parity and fixture checks passed; Vitest passed 608/608 across 138 files. Default Turbopack production build passed with local helper permission. Production frontend intentionally does not connect to the Auth Emulator; emulator browser verification uses the dev server as described in [Backend README](../../backend/README.md).
- Final code gate after the review lock: `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings/errors; `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` with console/TRX logging and per-test hang diagnostics exited 0. TRX counters confirm Integration 292/292, Unit 288/288 and Architecture 3/3, with no failures, timeouts or aborted tests. Both concurrent-review request orders passed in the full run.
- This clean run supersedes the earlier non-green code-gate rechecks. Earlier PostgreSQL SSL startup errors remain an unconfirmed infrastructure limitation; the previously canceled 13-minute run was inconclusive, not proof of a hang. Authorized-role UAT, real pilot prices/policies, sanitized legacy rehearsal, customer document output and release operations still require their respective evidence and sign-off.
