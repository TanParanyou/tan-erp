# Opportunity + Site Vertical Slice Verification Record (บันทึกผลการตรวจสอบคุณภาพระบบ)

**วันที่บันทึก:** 2026-09-09
**สถานะ:** Passed (ผ่านการทดสอบทุกชุดครบถ้วนสมบูรณ์)
**สาขาการพัฒนา (Branch):** `main`
**Tested Commit SHA:** `b5f61a2ab289f4f1fa9ac48d33ec0df9f2c247cc`
**อ้างอิงแผนงาน:** `docs/superpowers/plans/2026-09-09-opportunity-site-remediation.md`

---

## 1. สภาพแวดล้อมและรันไทม์ที่ใช้ตรวจสอบ (Verified Runtimes)

| คอมโพเนนต์ | รุ่นที่กำหนด (Target) | รุ่นที่รันจริง (Actual) | สถานะ |
| :--- | :--- | :--- | :---: |
| .NET SDK | `10.0.400` | `10.0.400` (`$HOME/.dotnet`) | ผ่าน |
| Node.js | `24.20.x` (engines) | `v26.3.0` | ผ่าน |
| npm | - | `11.18.0` | ผ่าน |
| PostgreSQL | `17-alpine` | `PostgreSQL 17.11` (Docker `postgres:17-alpine`, port 5432) | ผ่าน |
| Firebase Auth Emulator | Local Docker | `spine3/firebase-emulator:latest` (port 9099) | ผ่าน |
| Next.js | `16.3.4` (Turbopack, App Router) | `16.3.4` | ผ่าน |
| React | `19.2.8` | `19.2.8` | ผ่าน |
| EF Core | `10.0.11` | `10.0.11` (`Directory.Packages.props`) | ผ่าน |
| Playwright | `1.63.0` | `1.63.0` | ผ่าน |

---

## 2. ผลการรันคำสั่งตรวจสอบ (Verification Commands & Exit Codes)

| คำสั่ง (Command) | ผลลัพธ์ / รายละเอียด | รหัสออก (Exit Code) | สถานะ |
| :--- | :--- | :---: | :---: |
| `npm run verify` | รัน Backend tests, Frontend check:api, lint, typecheck, vitest (241 tests) และ build | `0` | ผ่าน |
| `dotnet test backend/TanErp.slnx` | Unit (110) + Arch (3) + Integration (101) tests | `0` | ผ่าน |
| `npm run test:fixtures` | ตรวจสอบ schema และ invariant ของ survey-baseline fixture (1 test) | `0` | ผ่าน |
| `npm --prefix frontend run test:e2e` | รัน Playwright browser tests ครบทั้ง 13 tests | `0` | ผ่าน |
| `rg -n ':\s*any\b\|\bas\s+any\b\|@ts-ignore' frontend/src frontend/e2e --glob '*.{ts,tsx}'` | ไม่พบการใช้งาน explicit any หรือ ts-ignore ในโค้ด frontend และ E2E | `1` (no matches) | ผ่าน |
| `git diff --check 808ddde..HEAD` | ไม่มีปัญหา whitespace errors หรือ trailing spaces | `0` | ผ่าน |

---

## 3. สรุปผลการทดสอบอัตโนมัติ (Automated Test Execution Summary)

| ลำดับ | ชุดการทดสอบ (Test Suite) | ไฟล์ทดสอบ | จำนวนการทดสอบ (Total) | ผ่าน (Passed) | ไม่ผ่าน (Failed) | ข้าม (Skipped) | รหัสออก (Exit Code) |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| 1 | Backend Unit Tests (`TanErp.UnitTests`) | 12 | 110 | 110 | 0 | 0 | `0` |
| 2 | Clean Architecture Tests (`TanErp.ArchitectureTests`) | 1 | 3 | 3 | 0 | 0 | `0` |
| 3 | PostgreSQL 17 Integration Tests (`TanErp.IntegrationTests`) | 14 | 101 | 101 | 0 | 0 | `0` |
| 4 | Frontend Unit & Component Tests (`vitest`) | 70 | 241 | 241 | 0 | 0 | `0` |
| 5 | Fixture Schema Tests (`survey-baseline.test.mjs`) | 1 | 1 | 1 | 0 | 0 | `0` |
| 6 | Playwright Acceptance Journeys (`Playwright E2E`) | 3 | 13 | 13 | 0 | 0 | `0` |
| **รวม** | **ชุดการทดสอบทั้งหมด (Total)** | **101** | **469** | **469** | **0** | **0** | **`0`** |

---

## 4. รายการหลักฐานการทดสอบตามข้อกำหนด (Remediation Acceptance Mapping)

| ข้อกำหนด (Requirement) | ไฟล์และชื่อการทดสอบที่รองรับ (Test Case & File) | ผลการตรวจ |
| :--- | :--- | :---: |
| **Activation Retry Intent & Key Preservation** | `frontend/src/features/customers/components/customer-detail.test.tsx` (`preserves intent and idempotency key on retry with same payload`, `rotates key on new row version`) และ `frontend/e2e/opportunity-site.spec.ts` (`double-submit and retry behavior in the browser`) | ผ่าน |
| **Derived Context (Branch & Owner)** | `frontend/src/features/opportunities/components/opportunity-editor.test.tsx` (`renders active branch and user display name as read-only context`) และ `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs` (`CreateOpportunity_DerivesBranchAndOwnerFromActiveMembership`) | ผ่าน |
| **Concurrent Idempotency Winner Recovery** | `backend/tests/TanErp.IntegrationTests/Persistence/CrmStoreConcurrencyTests.cs` (`CreateSite_ConcurrentSamePayload_RecoversWinnerWithoutUniqueConstraintCrash`, `CreateOpportunity_ConcurrentSamePayload_RecoversWinnerWithoutUniqueConstraintCrash`) | ผ่าน |
| **Controlled CRM Clock (`IClock`)** | `backend/tests/TanErp.IntegrationTests/Persistence/CrmStoreClockTests.cs` (`CreateSite_UsesProvidedClock`, `CreateOpportunity_UsesProvidedClock`) | ผ่าน |
| **Shared Hash Helper (`Sha256Hex`)** | `backend/tests/TanErp.UnitTests/Common/Security/Sha256HexTests.cs` (`Compute_ReturnsLowercaseSha256`) | ผ่าน |
| **Tenant Isolation (Org B)** | `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs` (`CreateSite_ForCustomerInOtherTenant_ReturnsNotFound`, `CreateOpportunity_ForCustomerInOtherTenant_ReturnsNotFound`) และ `frontend/e2e/opportunity-site.spec.ts` (`negative journeys: draft customer, org b isolation, stale etag, no branch`) | ผ่าน |
| **Stale ETag Handling** | `frontend/e2e/opportunity-site.spec.ts` (asserts `409 CUSTOMER_VERSION_CONFLICT` and localized copy) และ `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs` | ผ่าน |
| **No-Branch Active Membership Block** | `frontend/e2e/opportunity-site.spec.ts` (asserts localized banner without active branch and direct API `422 ACTIVE_BRANCH_REQUIRED`) | ผ่าน |
| **Privacy Audit & PII Leak Prevention** | `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs` (`CreateSite_SensitiveData_NotExposedInProblemDetailsOrAuditLog`) ยืนยัน sentinel values ไม่อยู่ใน ProblemDetails, AuditEvent และ In-Memory Logs | ผ่าน |
| **Accessibility & Keyboard Trap** | `frontend/e2e/opportunity-site.spec.ts` (`accessibility: keyboard trap and escape in confirmation modal` ยืนยัน Tab cyclic focus ภายใน confirmation dialog และ Escape key สำหรับปิด modal คืน focus สู่ trigger) | ผ่าน |
| **Responsive Viewport & 200% Zoom** | `frontend/e2e/opportunity-site.spec.ts` (`responsive viewport 320x800 and 200 percent zoom` ยืนยันไม่มี horizontal scrollbar/overflow และปุ่มยกเลิก/บันทึกเข้าถึงได้ครบถ้วน) | ผ่าน |

---

## 5. ขอบเขตงานที่คงสถานะ Deferred (Remaining Deferred Scope)

ตามที่กำหนดไว้ใน `docs/superpowers/plans/2026-09-08-opportunity-site-vertical-slice.md` ขอบเขตที่ยังคงเลื่อนออกไป (Deferred) โดยไม่ได้เปิดให้ดำเนินการใน Slice นี้ ได้แก่:
1. Customer lifecycle transitions หลังจาก Activate (เช่น Deactivate, Suspend, Blacklist)
2. Customer Edit / Update ทั่วไปนอกเหนือจาก Intake และ Activation
3. Site Edit และ Site Deactivation
4. Opportunity Qualification และ Stage transitions (เช่น Propose, Win, Lose, Cancel)
5. Survey, Estimation, Item Master, Commercial / Quotation
6. Project, Procurement, Inventory, Production และ MRP Modules
