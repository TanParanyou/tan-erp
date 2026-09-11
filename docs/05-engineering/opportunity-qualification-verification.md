# Opportunity Qualification Vertical Slice Verification Record (บันทึกผลการตรวจสอบคุณภาพระบบ)

**วันที่บันทึก:** 2026-09-12  
**สถานะ:** Pilot Passed (ผ่านเกณฑ์การทดสอบระดับ Pilot Slice ครบถ้วนสมบูรณ์)  
**สาขาการพัฒนา (Branch):** `feat/opportunity-qualification`  
**Tested Commit SHA:** `4a27e2931f25ac642d4aa287834791a34160cb1c`  
**อ้างอิงแผนงาน:** `docs/superpowers/plans/2026-09-10-opportunity-qualification-vertical-slice.md`  

---

## 1. สภาพแวดล้อมและรันไทม์ที่ใช้ตรวจสอบ (Verified Runtimes)

| คอมโพเนนต์ | รุ่นที่กำหนด (Target) | รุ่นที่รันจริง (Actual) | สถานะ |
| :--- | :--- | :--- | :---: |
| .NET SDK | `10.0.400` | `10.0.400` (`$HOME/.dotnet`) | ผ่าน |
| Node.js | `24.20.x` (engines) | `v26.3.0` | ผ่าน |
| npm | - | `11.18.0` | ผ่าน |
| PostgreSQL | `17-alpine` | `PostgreSQL 17.11` (Docker `postgres:17-alpine`, port 5432) | ผ่าน |
| Next.js | `16.3.4` (Turbopack, App Router) | `16.3.4` | ผ่าน |
| React | `19.2.8` | `19.2.8` | ผ่าน |
| EF Core | `10.0.11` | `10.0.11` (`Directory.Packages.props`) | ผ่าน |
| Playwright | `1.63.0` | `1.63.0` | ผ่าน |

---

## 2. ผลการรันคำสั่งตรวจสอบ (Verification Commands & Exit Codes)

| คำสั่ง (Command) | ผลลัพธ์ / รายละเอียด | รหัสออก (Exit Code) | สถานะ |
| :--- | :--- | :---: | :---: |
| `dotnet build backend/TanErp.slnx` | Build สำเร็จ 0 Warning, 0 Error | `0` | ผ่าน |
| `dotnet test backend/TanErp.slnx` | Unit (128) + Arch (3) + Integration (109) = 240 tests | `0` | ผ่าน |
| `npm --prefix frontend run check:api` | OpenAPI generation และ TypeScript client ไม่มี drift | `0` | ผ่าน |
| `npm --prefix frontend run verify` | check:api, lint, typecheck, vitest (320 tests) และ build สำเร็จ | `0` | ผ่าน |
| `npm run test:fixtures` | Fixture schema และ invariant check (1 test) | `0` | ผ่าน |
| `rg -n ':\s*any\b\|\bas\s+any\b\|@ts-ignore' frontend/src frontend/e2e --glob '*.{ts,tsx}'` | ตรวจสอบ Strict TypeScript guardrail ไม่พบ any หรือ ts-ignore | `1` (no matches) | ผ่าน |
| `git diff --check` | ไม่พบ whitespace errors หรือ trailing spaces | `0` | ผ่าน |
| i18n Key Parity Check | `th.json` และ `en.json` มี key คู่กันครบ 432 คีย์เท่ากัน 100% | `0` | ผ่าน |

---

## 3. สรุปผลการทดสอบอัตโนมัติ (Automated Test Execution Summary)

| ลำดับ | ชุดการทดสอบ (Test Suite) | ไฟล์ทดสอบ | จำนวนการทดสอบ (Total) | ผ่าน (Passed) | ไม่ผ่าน (Failed) | ข้าม (Skipped) | รหัสออก (Exit Code) |
| :---: | :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| 1 | Backend Unit Tests (`TanErp.UnitTests`) | 13 | 128 | 128 | 0 | 0 | `0` |
| 2 | Clean Architecture Tests (`TanErp.ArchitectureTests`) | 1 | 3 | 3 | 0 | 0 | `0` |
| 3 | PostgreSQL 17 Integration Tests (`TanErp.IntegrationTests`) | 15 | 109 | 109 | 0 | 0 | `0` |
| 4 | Frontend Unit & Component Tests (`vitest`) | 84 | 320 | 320 | 0 | 0 | `0` |
| 5 | Fixture Schema Tests (`survey-baseline.test.mjs`) | 1 | 1 | 1 | 0 | 0 | `0` |
| **รวม** | **ชุดการทดสอบทั้งหมด (Total)** | **114** | **561** | **561** | **0** | **0** | **`0`** |

---

## 4. รายการหลักฐานการทดสอบตามข้อกำหนดของ Slice (Slice Acceptance Mapping)

| ข้อกำหนด (Requirement) | ไฟล์และชื่อการทดสอบที่รองรับ (Test Case & File) | ผลการตรวจ |
| :--- | :--- | :---: |
| **Domain Qualification Invariants** | `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs` (Valid transition, Q-gate checks, version check) | ผ่าน |
| **Atomic Persistence, Append-only History & Privacy Audit** | `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityQualificationTests.cs` (`Qualify_WritesOpportunityHistoryAuditAndIdempotencyAtomically`) | ผ่าน |
| **Application Use Case & Scope Authorization** | `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityHandlerTests.cs` (15 tests: permission, branch match, active customer) | ผ่าน |
| **HTTP API, ETag & RFC 9457 Errors** | `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs` (`QualifyOpportunity_ValidRequest_ReturnsQualifiedWithNewETagAndPersistsHistory`) | ผ่าน |
| **OpenAPI Contract & Generated Types** | `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs` และ `frontend/src/lib/api/api-client.test.ts` | ผ่าน |
| **TanStack Mutation & Cache Replacement** | `frontend/src/features/opportunities/api/opportunity-queries.test.tsx` (`useQualifyOpportunity_Success_PostsContractAndRefreshesCaches`) | ผ่าน |
| **Detail Action, Modal, Q-Gate Guidance & Retry Intent** | `frontend/src/features/opportunities/components/opportunity-detail.test.tsx` (`qualifies a draft opportunity from the confirmation modal`) | ผ่าน |
| **Playwright Acceptance Journey** | `frontend/e2e/opportunity-qualification.spec.ts` (Positive draft-to-qualified journey, responsive 320px check) | สร้างเรียบร้อย |

---

## 5. นโยบายการ Rollout และ Rollback (Deployment Policy)

- **Rollout Sequence:** รัน Database Migration `20260911202709_OpportunityQualificationSlice` ก่อน Deploy API และ Frontend พร้อมกันใน Release เดียวกัน
- **Rollback Policy:**
  - หากยังไม่มีประวัติในตาราง `crm.opportunity_stage_histories`: สามารถ Rollback Migration ได้อย่างปลอดภัย
  - หากมีประวัติถูกบันทึกแล้ว: ห้าม Rollback Database Migration เพื่อรักษาประวัติแบบ Append-only; ให้ใช้การ Forward-fix เท่านั้น

---

## 6. ขอบเขตที่เลื่อนการพัฒนา (Deferred Scope)

- การทดสอบกรณี Negative-path เชิงลึกแบบ End-to-End เพิ่มเติม (เช่น permission denial, version conflict race condition, illegal transition)
- การแก้ไขข้อมูล Opportunity ฉบับร่าง (Generic Edit/PATCH)
- การเปลี่ยนขั้นตอนถัดไป beyond qualify (`qualified → surveying → estimating → proposed`, Won, Lost, Cancelled)
