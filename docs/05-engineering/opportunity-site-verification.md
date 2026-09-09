# Opportunity + Site Vertical Slice Verification Record (บันทึกผลการตรวจสอบคุณภาพระบบ)

**วันที่บันทึก:** 2026-09-09 (ผลรันจริงทั้งหมดในวันนี้)
**สถานะ:** ผ่านการตรวจสอบคุณภาพระบบครบถ้วนสมบูรณ์ (Passed Quality Gate 100%)
**สาขาการพัฒนา (Branch):** `main`
**อ้างอิงแผนงาน:** `docs/superpowers/plans/2026-09-08-opportunity-site-vertical-slice.md`

---

## 1. สภาพแวดล้อมและรันไทม์ที่ใช้ตรวจสอบ (Verified Runtimes)

| คอมโพเนนต์ | รุ่นที่กำหนด (Target) | รุ่นที่รันจริง (Actual) | ผลการตรวจ |
| :--- | :--- | :--- | :---: |
| .NET SDK | `10.0.400` | `10.0.400` (`$HOME/.dotnet`) | ผ่าน |
| Node.js | `24.20.x` (engines) | `v26.3.0` | ผ่าน |
| PostgreSQL | `17-alpine` | `PostgreSQL 17.11` (Docker `postgres:17-alpine`) | ผ่าน |
| Firebase Auth Emulator | Local Docker | `spine3/firebase-emulator:latest` (:9099) | ผ่าน |
| Next.js | `16.3.4` (App Router) | `16.3.4` | ผ่าน |
| React | `19.2.8` | `19.2.8` | ผ่าน |
| EF Core | `10.0.11` | `10.0.11` (`Directory.Packages.props`) | ผ่าน |
| Playwright | `1.63.0` | `1.63.0` | ผ่าน |

---

## 2. โครงสร้างฐานข้อมูลและการอพยพ (Database Migration)

- **Migrations ที่ apply จริงใน Slice นี้:**
  - `20260908160415_AddCustomerQuickIntakeFields`
  - `20260909032349_OpportunitySiteSlice`
- **Schemas ที่ควบคุม:** `crm` (`customers`, `sites`, `opportunities`, `customer_contacts`), `audit` (`idempotency_records`)
- **Tables และ Constraints ที่เพิ่มและทดสอบจริง:**
  - `crm.sites`:
    - Columns: `id`, `organization_id`, `customer_id`, `code`, `label`, `normalized_label`, `address_line1`, `subdistrict`, `district`, `province`, `postal_code`, `country_code`, `latitude`, `longitude`, `access_note`, `status`, `created_by_user_id`, `created_at_utc`, `row_version`
    - Foreign Keys: `FK_sites_customers_customer_id_organization_id` (Composite Tenant FK ป้องกันการข้าม Tenant)
    - Indexes: Unique `IX_sites_organization_id_code`, Index `IX_sites_customer_id_normalized_label`
  - `crm.opportunities`:
    - Columns: `id`, `organization_id`, `branch_id`, `customer_id`, `primary_site_id`, `code`, `title`, `scope_summary`, `work_types` (`jsonb`), `source_code`, `stage`, `expected_budget`, `currency_code`, `target_decision_date`, `next_action_at_utc`, `next_action_note`, `created_by_user_id`, `created_at_utc`, `row_version`
    - Foreign Keys: Composite FK ไปยัง `customers` และ `sites` ภายใต้ `organization_id` เดียวกัน
    - Indexes: Unique `IX_opportunities_organization_id_code`, `IX_opportunities_customer_id`, `IX_opportunities_primary_site_id`
  - Canonical Work Types ใน `work_types`: `built-in`, `interior`, `curtain`, `wallpaper`, `exterior`, `other` (ตรวจสอบความถูกต้องระดับ Domain & Application)
- **Database update status:** ยืนยันด้วย `dotnet ef database update` สำเร็จครบถ้วน

---

## 3. สรุปผลการทดสอบอัตโนมัติ (Automated Test Execution Summary)

| ลำดับ | ชุดการทดสอบ (Test Suite) | จำนวนการทดสอบ | ผ่าน | ไม่ผ่าน | รหัสออก (Exit Code) |
| :---: | :--- | :---: | :---: | :---: | :---: |
| 1 | Backend Unit Tests (`TanErp.UnitTests`) | 108 | 108 | 0 | `0` |
| 2 | Clean Architecture Tests (`TanErp.ArchitectureTests`) | 3 | 3 | 0 | `0` |
| 3 | PostgreSQL 17 Integration Tests (`TanErp.IntegrationTests`) | 96 | 96 | 0 | `0` |
| 4 | Frontend Unit & Component Tests (`vitest`, 70 files) | 239 | 239 | 0 | `0` |
| 5 | Fixture Schema Tests (`node --test fixtures/survey-baseline.test.mjs`) | 1 | 1 | 0 | `0` |
| 6 | End-to-End Acceptance Journeys (`Playwright`) | 9 | 9 | 0 | `0` |
| **รวม** | **ชุดการทดสอบทั้งหมด (Total)** | **456** | **456** | **0** | **`0`** |

### คำสั่งที่รันจริงและผลการตรวจสอบ:
1. **Backend Verification:**
   ```bash
   dotnet build backend/TanErp.slnx   # Exit code 0
   dotnet test backend/TanErp.slnx    # Exit code 0 (207 tests passed)
   ```
2. **Frontend Verification:**
   ```bash
   npm --prefix frontend run verify   # Exit code 0 (check:api + lint + typecheck + test + build)
   ```
3. **Fixture Test:**
   ```bash
   npm run test:fixtures              # Exit code 0 (1 pass)
   ```
4. **E2E Acceptance Test:**
   ```bash
   PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e # Exit code 0 (9 passed)
   ```
5. **Git Whitespace & Policy Scan:**
   ```bash
   git diff --check                   # No whitespace errors (Exit 0)
   rg -n "\bany\b|as any|@ts-ignore|BEGIN PRIVATE KEY|firebase-admin" frontend/src frontend/e2e
   # ไม่พบ Type any / as any / @ts-ignore ละเมิดกฎในโค้ด Application
   ```

### Tested Commit and Verification Evidence
- **Tested Base Commit SHA:** `b0d0d42e25b7a9db9d4aced0fc04e14084c915f5`
- **Playwright Test Count:** 9 tests (1 `auth.spec.ts` + 6 `customer-contact.spec.ts` + 2 `opportunity-site.spec.ts`)
- **Result:** 9 passed, 0 failed
- **Exit Code:** `0`

---

## 4. ผลการตรวจสอบความปลอดภัยและสถาปัตยกรรม (Security & Architecture Verification)

1. **Authorization Boundary & Guardrails:**
   - **Customer Activation:** ต้องการสิทธิ์ `customers.activate` (Scope: `organization`) ผ่าน Precondition `If-Match` และ Idempotency Header
   - **Site Management:** ต้องการสิทธิ์ `sites.manage` และต้องสร้างภายใต้ Active Customer เท่านั้น
   - **Opportunity Workspace:** ต้องการสิทธิ์ `opportunities.read` สำหรับการดู และ `opportunities.create` พร้อม Active Branch สำหรับการสร้าง (ป้องกันการสร้างข้ามสาขา)
   - **Tenant Isolation:** ตรวจสอบผ่าน Integration Test ให้สิทธิ์เฉพาะสมาชิกภาพของ Tenant ที่ตรงกันเท่านั้น สมาชิกต่าง Tenant ไม่สามารถเข้าถึงหรือสร้างเชื่อมโยงข้าม Tenant ได้
2. **ERP Form & UX Standards (Atelier Architectural Navy Sharp):**
   - รูปแบบ Sharp Edges `border-radius: 0px`
   - สีหลัก Solid Navy `#0B3056`
   - ไอคอน pure SVG Stroke ไม่มีภายนอก
   - การป้องกัน Double Submit (`isSubmitting` / `disabled`)
   - Confirmation Modal สำหรับการ Activate Customer
   - Minimal Mono Loading Spinner (ไม่ใช้ Skeleton Loader)
   - ระบบแปลสองภาษา (i18n) ครบถ้วนทั้ง `th.json` และ `en.json`
