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

---

## 4. Customer-safe Quotation Document (CP-04, 2026-10-03)

**Scope:** read-only projection `GET /api/v1/estimates/{id}/quotation/document`, Preview/Print page `/{locale}/estimates/{id}/quotation`, entry link on the Estimate card when the current revision is `quoted`. Allowlist and document rules are **Proposed (TEST_ONLY)**; see [API contract](../03-contracts/official-estimate-api-contract.md#quotation-document) and [ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md).

**Automated evidence (2026-10-03, branch `feat/customer-safe-quotation-document`)**
- Backend: `dotnet build backend/TanErp.slnx --no-restore -m:1` 0 warnings/errors; full `dotnet test` on the final branch state passed Architecture 3/3, Integration 292/292, Unit 292/292.
- The integration case `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` verifies: serialized property allowlist at every level, forbidden-term scan, totals vs. issued quotation, section/line totals consistency, `locale=en` without English text (null + `hasIncompleteTranslations`), unknown locale normalized to `th`, billing snapshot immutability after Customer master edit, latest-quotation selection with two quotations, unreadable billing snapshot -> 409 `ESTIMATE_INVALID_STATE`, cross-Organization 404, 403 without `quotations.read`.
- Frontend: `npm run check:api`, `npm run lint`, `tsc --noEmit` and `npm run build` passed; full Vitest 615/615 (run with `--testTimeout=60000`; `estimate-workspace-layout.test.tsx` exceeds the 5s default on a loaded machine, also on the unmodified tree).
- Playwright `e2e/official-estimate.spec.ts` passed 1/1 against the local stack (Backend :5005, Frontend :3005): opens the document from the Estimate card, checks Thai content and absence of cost/margin wording, switches to English, and verifies no horizontal overflow at 320px.

**Not verified / open**
- Business/Finance sign-off of the field allowlist, terms, branding and page size; manual UAT of the printed/PDF artifact.
- 200% zoom, keyboard and screen-reader review; browser-print pagination for long descriptions (320px overflow is covered by Playwright).
- No stored rendered file or hash (ADR 0016); `customerReference`, `validityDays`, `scopeNote` and payment/delivery terms are not in the Quotation domain.
- `quotations.read` is implemented in code and the test seeder only; production roles must be granted it by the owner of role bootstrap before release.

## 5. Quotation Amendment/Void (CP-06, 2026-10-04)

กฎ: [Quotation Lifecycle API Contract](../03-contracts/quotation-lifecycle-api-contract.md). หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT; ค่าเริ่มต้นรอ Sales/Finance ยืนยัน.

- Backend Integration (บน PostgreSQL) `EstimateEndpointsTests` 44/44 รวมสองเคสใหม่: **Amend** (ต้องมีเหตุผล, stale version, สร้างฉบับใหม่เลขใหม่ยอดเดิม, ฉบับเดิม `superseded` พร้อมลิงก์สองทิศและเหตุผล, replay Key เดิมไม่ออกซ้ำ/payload ต่าง 409, ฉบับ superseded แก้/ยกเลิกไม่ได้, การตอบรับเลือกฉบับ live, หลังยอมรับถูกล็อกทั้ง void และ amend) และ **Void** (ต้องมีเหตุผล, stale version, บันทึกผู้ยกเลิก/เวลา/เหตุผล, ยกเลิกซ้ำ/amend หลัง void ถูกปฏิเสธ, Opportunity ยังเป็น `proposed`, ตอบรับฉบับที่ยกเลิกแล้ว → `QUOTATION_INVALID_STATE`, audit ไม่มีข้อความเหตุผล). เคสเดิมที่แทรกใบเสนอราคาที่สองให้ประมาณการเดียวถูกปรับให้เป็น superseded ตามกฎใหม่ (unique index ฉบับ live). ชุด Project/OpenAPI ที่เกี่ยวข้องผ่าน.
- Frontend Vitest `features/estimates` 17 ไฟล์/93 เคส รวม `quotation-lifecycle-panel` (ปุ่มเฉพาะฉบับ `issued` ตามสิทธิ์, ต้องกรอกเหตุผล, ส่ง row version + Idempotency-Key, แสดงข้อความ error ที่แปลแล้ว) , `tsc --noEmit`, `eslint .` ผ่าน.
- ข้อจำกัด: ไม่ย้อน Opportunity/ประมาณการเมื่อ void (ตั้งใจ); ไม่มี void/amend หลังยอมรับ (ล็อก); ไม่มีลายน้ำ "ยกเลิก/ถูกแทนที่" บนใบพิมพ์ฉบับเก่า (หน้าแสดงสถานะในประวัติเท่านั้น); ไม่ได้ทดสอบ race ยอมรับ/amend พร้อมกันแบบขนาน (อาศัย row version + unique index); ไม่ได้รัน full Integration suite, `next build`, Playwright; ต้องมอบสิทธิ์ `quotations.void`/`quotations.amend` ให้ Role จริง.


## 6. External Acceptance/Signatures (CP-07, 2026-10-04)

กฎ: [External Acceptance API Contract](../03-contracts/external-acceptance-api-contract.md). หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ Security review/Legal sign-off; ค่าเริ่มต้นรอ Sales/Legal ยืนยัน.

- Backend Integration `ExternalAcceptance*` 4/4 บน PostgreSQL: ลูกค้ายอมรับผ่านลิงก์ครั้งเดียว (token เก็บเป็น hash เท่านั้นและไม่อยู่ในรายการภายใน; response สาธารณะ no-store และ **ไม่มีคำเกี่ยวกับต้นทุน/ข้อมูลภายใน**; ไม่ยินยอม/เวอร์ชันถ้อยคำผิด/ชื่อสั้น/ภาพที่ไม่ใช่ PNG ถูกปฏิเสธโดยไม่ยอมรับ; ยอมรับแล้ว Quotation=accepted, Opportunity=Won, ประวัติ Won 1 แถว, หลักฐานครบพร้อม hash ภาพ/ IP, audit ไม่มีชื่อผู้ลงนาม; ส่งซ้ำคืนผลเดิมไม่บันทึกซ้ำ), ลิงก์ที่ใช้ไม่ได้ทุกแบบ (token ไม่รู้จัก/สั้น/รูปแบบผิด, เพิกถอน, หมดอายุ, ใบเสนอราคาถูก void) **ได้ 404 `ACCEPTANCE_LINK_UNAVAILABLE` เหมือนกัน** ทั้งดูและยอมรับ, อายุเกิน 30 วันและการสร้างลิงก์ให้ใบที่ไม่ใช่ issued ถูกปฏิเสธ, ลิงก์ของใบที่ถูก amend ใช้ยอมรับฉบับใหม่ไม่ได้ (Opportunity คง `proposed`, ไม่มีหลักฐาน) ส่วนฉบับใหม่มีลิงก์ของตัวเอง, และ **rate limit** (โควตา 3 → ครั้งที่ 4–5 ได้ 429 ทั้ง GET และ POST). รวม EstimateEndpoints 44/44 + OpenAPI ผ่าน.
- Frontend Vitest `features/external-acceptance` (7 เคส: error กลางแบบเดียวสำหรับลิงก์ใช้ไม่ได้, ต้องกรอกชื่อ+ติ๊กยินยอมก่อนยิง API, ส่งชื่อ/ตำแหน่ง/เวอร์ชันยินยอม, แสดง error แปล, ยืนยันหลังยอมรับ, สถานะลิงก์/ข้อความ th-en) และ `features/estimates` ผ่าน, `tsc --noEmit`, `eslint .` ผ่าน.
- ข้อจำกัด: ไม่มีการยืนยันตัวตน/อำนาจของผู้ลงนาม (ไม่มี OTP/อีเมลตรวจสอบ), ลิงก์รั่ว = ยอมรับได้ (บรรเทาด้วยอายุ/เพิกถอน/ใช้ครั้งเดียว), ภาพลายเซ็นไม่ใช่หลักฐานที่ผ่านนโยบายกฎหมาย, ยังไม่มีนโยบาย retention/ลบหลักฐาน, ไม่ได้ส่งอีเมล/ข้อความ (เจ้าหน้าที่คัดลอกลิงก์เอง), ไม่ได้ทดสอบ race ยอมรับ/amend แบบขนานจริง (ป้องกันด้วย expectedQuotationId + unique index), rate limit เป็นต่อ IP และต้องตั้ง Forwarded Headers หลัง proxy, ไม่ได้รัน full Integration suite, `next build`, Playwright; ต้องมอบสิทธิ์ `quotations.share` ให้ Role จริง.
