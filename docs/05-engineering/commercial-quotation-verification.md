# Commercial Quotation Vertical Slice Verification (Slice 5B)

**Status:** Verified (Hardened per `docs/superpowers/plans/2026-09-20-official-estimate-commercial-hardening.md`)
**Tested Commit/Date:** 2026-09-21 (Hardening verified on branch `feat/opportunity-qualification`)
**Environment:**
- macOS Apple Silicon
- PostgreSQL 17 Container (port 5432)
- Firebase Auth Emulator (port 9099)
- ASP.NET Core 10.0 WebApi (port 5005)
- Next.js 16.3.4 Frontend (port 3005)
- .NET SDK 10.0.400

---

## 1. Scope & Capabilities

This vertical slice delivers the **Commercial Quotation Vertical Slice (Slice 5B)**:
1. **Commercial Quotation Issuance:**
   - Initiated from a calculated Official Estimate in `estimating` stage.
   - Atomic document numbering via sequence counter engine (`ISequenceCounter` / `IDocumentNumberGenerator`).
   - Atomic Opportunity progression: `estimating` -> `proposed`.
   - Replay-safe idempotency handling (returns existing quotation without duplicate side effects).
2. **Customer Acceptance:**
   - Quotation transition to `accepted`.
   - Atomic Opportunity progression: `proposed` -> `won`.
   - Privacy-safe audit logging (excluding confidential negotiation note `decisionNote`).
3. **Document Numbering Configuration:**
   - Read and manage document sequences.
   - Concurrency control with `If-Match` ETag header and `RowVersion`.
   - Strict pattern and token grammar validation (strictly requires `{SEQ}` exactly once, rejects unknown reset periods).

---

## 2. Targeted Verification Gates (Hardening R2)

All 8 targeted verification gates pass with 100% success:

| # | Targeted Gate Name | Suite / File | Status | Notes |
|---|-------------------|--------------|--------|-------|
| 1 | `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot` | `EstimateEndpointsTests.cs` | **Passed** | Server derives customer, branch, and ready survey snapshot hash directly from survey evidence. |
| 2 | `CreateEstimate_ForgedOrUnreadyRelationship_IsRejected` | `EstimateEndpointsTests.cs` | **Passed** | Unready survey revision or mismatched opportunity relationships are rejected with RFC 9457 Problem Details. |
| 3 | `buildEstimateCsv_UsesServerSnapshotTotalsWithoutRecalculation` | `estimate-export.test.ts` | **Passed** | Verified server calculation snapshot totals are exported directly without client-side recalculation drift. |
| 4 | `IssueQuotation_ReplaySameIntent_ReturnsSameQuotationWithoutDuplicateEffects` | `QuotationEndpointsTests.cs` | **Passed** | Idempotency replay returns the existing quotation without allocating new numbers, stage history, or audit rows. |
| 5 | `IssueQuotation_TwoEstimates_AllocatesDistinctAtomicNumbers` | `QuotationEndpointsTests.cs` | **Passed** | Atomic document sequence engine allocates distinct monotonic numbers without race conditions or `COUNT + 1` gaps. |
| 6 | `AcceptQuotation_ReplaySameIntent_ReturnsSameWonResultWithoutDuplicateEffects` | `QuotationEndpointsTests.cs` | **Passed** | Replay of customer acceptance returns 200 without duplicate Won history entries; audit logs omit sensitive decision note. |
| 7 | `IssueQuotation_SendsBothVersionsAndReusesKeyAfterAmbiguousFailure` | `estimate-card.test.tsx` | **Passed** | Frontend sends both `expectedEstimateRevisionVersion` and `expectedOpportunityVersion`; preserves idempotency key across retries. |
| 8 | `DocumentSequence_UpdateRequiresPermissionVersionAndValidPattern` | `DocumentSequencesEndpointsTests.cs` | **Passed** | Sequence updates require `settings.numbering.manage` permission, valid `If-Match` ETag, strict reset period, and `{SEQ}` token. |

---

## 3. Automated Test Results

### 3.1 Backend Tests (.NET 10 Clean Architecture)
```bash
/Users/syaco/.dotnet/dotnet test backend/TanErp.slnx
```
**Result:** **Passed 100% (337/337 tests green, Exit Code: 0)**
- `TanErp.UnitTests`: 167/167 passed (72 ms)
- `TanErp.ArchitectureTests`: 3/3 passed (1 s)
- `TanErp.IntegrationTests`: 167/167 passed (1 m 33 s)

### 3.2 Frontend Unit & Component Tests (Vitest)
```bash
npm --prefix frontend run test
```
**Result:** **Passed 100% (112 test files, 487/487 tests green, Exit Code: 0)**
- `estimate-card.test.tsx` (Targeted Gate 7)
- `estimate-export.test.ts` (Targeted Gate 3)
- `estimate-calculations.test.ts`
- `estimate-formatters.test.ts`
- `estimate-catalog-items.test.ts`
- `estimate-templates.test.ts`
- `estimate-item-catalog-modal.test.tsx`

### 3.3 Frontend Verification Pipeline (Definition of Done)
```bash
npm --prefix frontend run verify
```
**Result:** **Passed 100% with 0 errors, 0 warnings (Exit Code: 0)**
- `check:api`: OpenAPI schema contract exact parity (`openapi-typescript` generate & git diff clean)
- `lint`: ESLint clean
- `typecheck`: TypeScript `tsc --noEmit` strict type checking clean
- `test`: Vitest 487 tests green
- `build`: Next.js 16.3.4 (Turbopack) production compilation successful

### 3.4 End-to-End Test (Playwright Full Journey)
```bash
PLAYWRIGHT_TEST_BASE_URL="http://localhost:3005" npx --prefix frontend playwright test --config frontend/playwright.config.ts frontend/e2e/official-estimate.spec.ts
```
**Result:** **Passed 100% (1 passed, 15.7s, Exit Code: 0)**
Verified complete commercial flow against live stack (PostgreSQL 17, Firebase Emulator, Backend :5005, Frontend :3005):
1. **Authentication:** Sign in with foundation credentials.
2. **Customer & Site:** Create and activate customer with primary site location.
3. **Opportunity & Qualification:** Create Opportunity, meet qualification gates, transition to `qualified`.
4. **Site Survey:** Schedule survey appointment, record measurements, and Mark Ready (with snapshot hashing).
5. **Estimating Stage:** Automatic stage transition to `estimating`.
6. **Official Estimate:** Create draft bound to server-derived customer and survey snapshot evidence.
7. **Workspace Drawer & Recalculation:** Add sections, work items, and cost components; save draft; apply discount and calculate totals.
8. **Quotation Issuance:** Issue commercial quotation via atomic document numbering; advance opportunity to `proposed`.
9. **Customer Acceptance:** Customer accepts quotation; advance opportunity to `won`.
10. **Timeline Verification:** Stage history timeline displays exact transitions (`proposed` and `won`).
