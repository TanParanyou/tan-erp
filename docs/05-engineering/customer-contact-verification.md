# Customer + Contact Vertical Slice Verification Record (บันทึกผลการตรวจสอบคุณภาพระบบ)

**วันที่บันทึก:** 2026-09-08 (ผลรันจริงทั้งหมดในวันนี้)
**สถานะ:** ผ่านการตรวจสอบคุณภาพระบบครบถ้วนสมบูรณ์ (Passed Quality Gate)
**สาขาการพัฒนา (Branch):** `main`
**อ้างอิงแผนงาน:** `docs/superpowers/plans/2026-09-07-customer-contact-vertical-slice.md`
**อ้างอิงแผน remediation:** `docs/superpowers/plans/2026-09-08-customer-contact-remediation.md`

---

## 1. สภาพแวดล้อมและรันไทม์ที่ใช้ตรวจสอบ (Verified Runtimes — ค่าที่รันจริง)

| คอมโพเนนต์ | รุ่นที่กำหนด (Target) | รุ่นที่รันจริง (Actual) | ผลการตรวจ |
| :--- | :--- | :--- | :---: |
| .NET SDK | `10.0.400` | `10.0.400` (`$HOME/.dotnet`) | ผ่าน |
| Node.js | `24.20.x` (engines) | `v26.3.0` (ต่างจาก engines แต่ gates ผ่านทั้งหมด) | ผ่านแบบมีข้อสังเกต |
| PostgreSQL | `17-alpine` | `PostgreSQL 17.11` (image `postgres:17-alpine`) | ผ่าน |
| Next.js | `16.3.4` (App Router) | `16.3.4` | ผ่าน |
| React | `19.2.8` | `19.2.8` | ผ่าน |
| EF Core | `10.0.11` | `10.0.11` (`Directory.Packages.props`) | ผ่าน |
| Playwright | `1.63.0` | `1.63.0` | ผ่าน |

---

## 2. โครงสร้างฐานข้อมูลและการอพยพ (Database Migration — ข้อเท็จจริงจาก schema)

- **Migrations ที่ apply จริง:** `20260907161632_CustomerContactSlice` และ `20260908092940_EnforceCustomerAccessActivation` (ยืนยันด้วย `dotnet ef database update` สำเร็จบน PostgreSQL 17.11)
- **Schemas ที่ควบคุม:** `crm` (`customers`, `customer_contacts`) และ **`audit`** (`idempotency_records`) — ไม่ใช่ `common` ตามที่บันทึกเดิมระบุผิด
- **`identity_access.permissions.is_active`:** `boolean NOT NULL DEFAULT true` (migration ใหม่, ห้ามแก้ migration ที่ commit แล้ว)
- **Canonical columns:** `crm.customer_contacts.normalized_phone` / `normalized_email` — **ไม่มี** columns `phone_hash`/`email_hash` ตามที่บันทึกเดิมกล่าวอ้าง
- **Validation ระดับ Domain/Application (ไม่มี CHECK constraint บน columns เหล่านี้):** Customer Type `person|organization`, Status `draft|active|inactive`, Locale `th|en`, Channel `phone|email|line|other`; Contact ต้องมี phone หรือ email อย่างน้อยหนึ่งค่า
- **CHECK constraints ที่มีจริง:** `CK_customer_contacts_phone_or_email` (`normalized_phone IS NOT NULL OR normalized_email IS NOT NULL`), `CK_idempotency_records_hashes_not_empty`, ชุด `CK_role_permissions_*` (scope `organization|branch|own` + ความสอดคล้อง `organization_id`/`branch_id`)
- **Migration rehearsal:** rollback/reapply ทั้ง Foundation และ `EnforceCustomerAccessActivation` ผ่าน (`CustomerContactMigrationTests` 7 tests รวม assert `is_active`, `normalized_phone`/`normalized_email`, `audit.idempotency_records.key_hash` ผ่าน `information_schema`)

---

## 3. สรุปผลการทดสอบอัตโนมัติ (Automated Test Execution Summary — ผลรันจริง)

| ลำดับ | ชุดการทดสอบ (Test Suite) | จำนวนการทดสอบ | ผ่าน | ไม่ผ่าน | รหัสออก (Exit Code) |
| :---: | :--- | :---: | :---: | :---: | :---: |
| 1 | Backend Unit Tests (`TanErp.UnitTests`) | 49 | 49 | 0 | `0` |
| 2 | Clean Architecture Tests (`TanErp.ArchitectureTests`) | 3 | 3 | 0 | `0` |
| 3 | PostgreSQL 17 Integration Tests (`TanErp.IntegrationTests`) | 52 | 52 | 0 | `0` |
| 4 | Frontend Unit & Component Tests (`vitest`, 19 files) | 78 | 78 | 0 | `0` |
| 5 | Fixture Schema Tests (`node --test fixtures/survey-baseline.test.mjs`) | 1 | 1 | 0 | `0` |
| 6 | End-to-End Acceptance Journeys (`Playwright`, `auth` 1 + `customer-contact` 6) | 7 | 7 | 0 | `0` |
| **รวม** | **ชุดการทดสอบทั้งหมด (Total)** | **190** | **190** | **0** | **`0`** |

คำสั่งที่รันจริง (ทุกคำสั่ง exit `0`):

```bash
dotnet --version                                   # 10.0.400
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
npm --prefix frontend run verify                  # check:api + lint + typecheck + test + build
npm run test:fixtures
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
git diff --check                                  # ไม่มี output
rg -n "\bany\b|as any|@ts-ignore|BEGIN PRIVATE KEY|firebase-admin" frontend/src frontend/e2e
# พบเพียงคำว่า "any" ในประโยคภาษาอังกฤษ ("in any organization") ไม่ใช่ type violation
```

ข้อสังเกตซื่อตรง: การรัน `dotnet test` เต็มชุดครั้งแรกพบ `FoundationMigrationTests.CrossOrganization_And_Rollback_Tests` ล้ม 1 test ท่ามกลาง parallel testcontainers; รันซ้ำเต็มชุดผ่าน 52/52 และรันเดี่ยวผ่าน — สรุปเป็น flake จาก contention ชั่วคราว ไม่ใช่ defect ของโค้ด (รันเต็มชุดครั้งสุดท้ายผ่านครบ 104/104)

---

## 4. ผลการตรวจสอบความปลอดภัยและสถาปัตยกรรม (Security & Architecture Verification)

1. **Authorization boundary (ปิดจาก remediation Task 1):**
   - `Permission` มี lifecycle `IsActive`/`Activate()`/`Deactivate()`; resolver บังคับ active Branch (`BranchId == null || Branch.IsActive`) และ active Permission (`Permission.IsActive`, scope `organization` เท่านั้น); `CurrentUserReader` ไม่ project Permission ที่ inactive
   - Create ต้องมีทั้ง `customers.create` **และ** `customer-contacts.manage` (fail-closed, ไม่มี write เมื่อขาดสิทธิ์ใดสิทธิ์หนึ่ง)
   - Acceptance cases ที่รันจริงและผ่าน: create โดยไม่มี `customer-contacts.manage` → `403 PERMISSION_DENIED` และไม่มี customer row; inactive Permission → `403`; inactive Branch → `403`; Org B ขอ customer ของ Org A → `404 RESOURCE_NOT_FOUND` และไม่รั่ว PII; read-only user เห็น contact แบบ masked (`******`, `***@`); idempotency key เดิม + payload เดิม → customer ID เดิม; key เดิม + payload เปลี่ยน → `409 IDEMPOTENCY_KEY_REUSED`
2. **Tenant isolation:** Composite FK `(customer_id, organization_id)`, unique partial index primary contact เดียวต่อ customer, duplicate search จำกัดใน organization เดียวและไม่ auto-merge (covered by `CustomerTenantIsolation_*`, `DuplicateSearch_*`)
3. **Contract/state/UI (remediation Task 2–5):**
   - `ListCustomersParams` derive จาก OpenAPI `paths["/api/v1/customers"]["get"]["parameters"]["query"]`; ส่งเฉพาะ `organization|person`; รองรับ email-only; แสดง `draft` ถูกต้อง; ค่าที่เซิร์ฟเวอร์ส่งนอก canonical แสดง `common.feedback.operationFailed`
   - Business query keys ขึ้นต้น `business` รวม Membership/locale/filter (`search`/`status`/`limit` default 25); สลับ Membership ด้วย cancel → remove business queries ก่อน expose context ใหม่ (auth queries ไม่ถูกลบ)
   - Retry payload เดิมใช้ `Idempotency-Key` เดิม; แก้ field หลัง failure ใช้ key ใหม่; double click ยิง POST ครั้งเดียว (ปุ่ม `isLoading` + disabled)
   - Duplicate candidate ที่ mask แล้วมองเห็นได้หลัง Create พร้อม link `viewCreatedCustomer`; Detail reuse `DuplicateCandidateCard` เดียวกัน ไม่แสดง raw PII
   - ไม่มี hardcoded Customer UI copy (keys ครบ th/en parity), ไม่มี duplicated warning card, `git diff --check` สะอาด
4. **Regression ที่พบและซ่อมใน remediation นี้:**
   - `TestOnlyDataSeeder` ไม่ backfill permission keys/links ใหม่ให้ role ที่ seed ไว้ก่อน (DB เก่ามีแค่ `organizations.read`) → เพิ่ม backfill แบบ find-or-create ทั้ง Org A/B (test seed เท่านั้น)
   - Shell dashboard หายหลังย้ายเป็น `(erp)` layout (`children` เป็น React element ที่ truthy เสมอ) → แสดง dashboard ตาม `isHomeActive` (ซ่อม regression จาก slice commit `1d35a46`, ยืนยันด้วย `auth.spec` e2e)

---

## 5. ผลการตรวจ UX ด้วยมือและเบราว์เซอร์จริง (Manual UX Checks — 2026-09-08)

| เกณฑ์ | วิธีตรวจ | ผล |
| :--- | :--- | :---: |
| ภาษาไทย/อังกฤษ | e2e `auth.spec` สลับ `/en` + e2e ป้าย `นิติบุคคล`/`ฉบับร่าง`; component tests th/en | ผ่าน |
| Keyboard-only | e2e test เฉพาะ: Tab จนถึงปุ่มบันทึก → Enter → `role=alert` + `aria-invalid` ปรากฏ | ผ่าน |
| Focus visible | ตรวจ `globals.css` `:focus-visible { outline: 2px solid ... }` | ผ่าน |
| Error association | `Input` ผูก `aria-describedby` กับ error id; editor error ผูกกับ phone control | ผ่าน |
| 320px mobile | probe จริง: drawer nav + `scrollWidth - clientWidth = 0` | ผ่าน |
| 768px tablet | probe จริง: drawer nav + overflow `0` | ผ่าน |
| Desktop 1280px | e2e ทั้งชุดบน Chromium default viewport | ผ่าน |
| Zoom 200% (deviceScaleFactor 2) | probe จริงหน้า create: ปุ่มบันทึกมองเห็นได้ + overflow `0` | ผ่าน |
| 44px targets | ปุ่มหลัก `minHeight: 44px` + shell unit test assert `44px` | ผ่าน |
| Reduced motion | `globals.css` `@media (prefers-reduced-motion: reduce)` ปิด animation/transition | ผ่าน |
| Zero-radius | `globals.css` global `border-radius: 0` | ผ่าน |
| Minimal Mono Loading | list/detail ใช้ `MonoSpinner`; ไม่พบ skeleton loader ใน customers UI | ผ่าน |

---

## 6. ข้อจำกัดที่ทราบ (Known Limitations)

- Node.js ในเครื่อง (`v26.3.0`) ต่างจาก `engines` (`24.20.x`) แต่ gates ทั้งหมดผ่าน — ควรปรับ engines หรือสภาพแวดล้อมให้ตรงกันในงานถัดไป
- E2E ต้องอาศัย stack จริง (PostgreSQL + Firebase emulator + Backend `:5005` + Frontend `:3005`); ไม่ได้รันบน CI ในรอบนี้
- ไม่เพิ่ม Update/Activate/Address/Site/Opportunity/Export/Merge/Hard Delete ตามขอบเขตที่ตรึงไว้
