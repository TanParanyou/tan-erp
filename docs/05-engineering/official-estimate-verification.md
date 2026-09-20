# Official Estimate BOQ & Calculation Vertical Slice Verification (Slice 5A)

**Status:** Remediation Pending (Slice 5A Baseline Verified at 28aedbf; Hardening under `docs/superpowers/plans/2026-09-20-official-estimate-commercial-hardening.md`)
**Tested Commit/Date:** 2026-09-18 (commit `28aedbf`)
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
