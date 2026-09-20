# Commercial Quotation Vertical Slice Verification (Slice 5B)

**Status:** Remediation Pending (Hardening in progress per `docs/superpowers/plans/2026-09-20-official-estimate-commercial-hardening.md`)
**Tested Commit/Date:** Baseline commit `ddd90db` (2026-09-20)
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
   - Atomic document numbering via sequence engine (`IDocumentNumberGenerator`).
   - Atomic Opportunity progression: `estimating` -> `proposed`.
   - Replay-safe idempotency handling.
2. **Customer Acceptance:**
   - Quotation transition to `accepted`.
   - Atomic Opportunity progression: `proposed` -> `won`.
   - Privacy-safe audit (excluding `decisionNote`).
3. **Document Numbering Configuration:**
   - Read and manage document sequences.
   - Concurrency control with `If-Match`.
   - Strict pattern and token grammar validation.

---

## 2. Targeted Verification Gates

The following 8 targeted tests define the commercial hardening verification:
1. `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot`
2. `CreateEstimate_ForgedOrUnreadyRelationship_IsRejected`
3. `buildEstimateCsv_UsesServerSnapshotTotalsWithoutRecalculation`
4. `IssueQuotation_ReplaySameIntent_ReturnsSameQuotationWithoutDuplicateEffects`
5. `IssueQuotation_TwoEstimates_AllocatesDistinctAtomicNumbers`
6. `AcceptQuotation_ReplaySameIntent_ReturnsSameWonResultWithoutDuplicateEffects`
7. `IssueQuotation_SendsBothVersionsAndReusesKeyAfterAmbiguousFailure`
8. `DocumentSequence_UpdateRequiresPermissionVersionAndValidPattern`
