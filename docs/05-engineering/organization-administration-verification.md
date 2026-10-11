# Organization Administration Verification (G-03a)

## 1. สถานะ

Implemented 2026-10-11 (G-03a: Organization profile, Branch CRUD และ Branch activation) ด้วย **focused tests, full-suite regression ของโมดูลที่แตะ, และ frontend gate** — **ยังไม่ได้ตรวจในเบราว์เซอร์จริงในรอบนี้** (ตรวจด้วย jsdom/vitest เท่านั้น; layout, สี, responsive และ keyboard ยังไม่พิสูจน์ในเบราว์เซอร์) และ **ยังไม่ผ่าน UAT หรือพร้อม Production**.

สัญญา: [Organization Administration API Contract](../03-contracts/organization-administration-api-contract.md); การตัดสินใจ: [ADR 0019](../adr/0019-organization-branch-administration.md); แผน: [แผน G-03](../superpowers/plans/2026-10-10-g03-org-role-authority.md).

G-03b (Role management) และ G-03c (Approval Authority matrix) ยังเป็น pending และไม่อยู่ในขอบเขตเอกสารนี้.

## 2. ผลที่รันจริง (2026-10-11)

รันแยกทีละคำสั่งบน branch `feat/g03-org-role-authority` ที่ commit `22199c3`.

| คำสั่ง | ผล |
| --- | --- |
| `python3` key parity `frontend/src/messages/th.json` ↔ `en.json` | `only th: []`, `only en: []`, exit 0 |
| `dotnet build backend/TanErp.slnx --nologo -v q` | Build succeeded, 0 Warning, 0 Error |
| Unit `FullyQualifiedName~Organization` | 56 passed / 0 failed |
| Architecture `backend/tests/TanErp.ArchitectureTests` | 4 passed / 0 failed |
| Integration `FullyQualifiedName~OrganizationAdministrationEndpointsTests\|FullyQualifiedName~BranchDependencyInspectorCoverageTests\|FullyQualifiedName~OpenApiContractTests` (`-m:1`) | 25 passed / 0 failed |
| Regression Integration `FullyQualifiedName~IdentityAdministrationEndpointsTests\|FullyQualifiedName~UsersEndpointsTests\|FullyQualifiedName~NotificationEndpointsTests\|FullyQualifiedName~EstimateEndpointsTests` (`-m:1`) | 94 passed / 0 failed |
| `ConnectionStrings__Database=... dotnet ef migrations has-pending-model-changes` | "No changes have been made to the model since the last migration.", exit 0 |
| `npm run check:api` (frontend) | exit 0 |
| `npm run lint` (frontend) | exit 0 |
| `npm run typecheck` (frontend) | exit 0 |
| `npx vitest run src/features/settings src/components/layout src/lib` | Test Files 35 passed (35), Tests 197 passed (197) |
| `npm run build` (frontend) | exit 0 |

ชื่อคลาส integration ที่ใช้จริงใน regression: `IdentityAdministrationEndpointsTests`, `UsersEndpointsTests`, `NotificationEndpointsTests`, `EstimateEndpointsTests` (ทั้งสี่มีอยู่ใน `backend/tests/TanErp.IntegrationTests/Api/`).

สิ่งที่ชุดทดสอบพิสูจน์ (ตาม test ที่รัน): permission ถูกตรวจก่อนการตรวจการมีอยู่ของ resource (ไม่มีสิทธิ์ → 403 เหมือนกันทุก id); id ของอีก Organization ตอบ 404; unique constraint แยกรหัสสาขา (`code`) และเลขสาขาภาษี (`tax code`); idempotency replay/key reuse; ETag/If-Match ให้ 409 เมื่อ version ไม่ตรง และ 428 เมื่อไม่ส่ง; guard การปิดสาขา 4 ชนิด (เอกสารเปิด, Warehouse active, Membership active, สาขา active สุดท้าย) และ audit ของการปิด/เปิดไม่มีข้อมูลส่วนบุคคลในค่า `changes`; `BranchDependencyInspectorCoverageTests` ไล่ entity ที่มี `BranchId` แล้วล้มเมื่อไม่อยู่ในรายการ "นับ" หรือ "ยกเว้น" อย่างชัดเจน; OpenAPI snapshot ตรงกับ endpoint จริง.

## 3. การตรวจในเบราว์เซอร์

**ไม่ได้ตรวจในเบราว์เซอร์ในรอบนี้** (ตรวจด้วย jsdom/vitest เท่านั้น) ต้องตรวจก่อน UAT:

1. `/th/settings/organization`: แก้ชื่อ/เลขภาษี → บันทึก → toast; เลขภาษีผิด → ข้อความ `ORGANIZATION_TAX_ID_INVALID` ภาษาไทย; สลับ `/en/` ข้อความเป็นอังกฤษ.
2. `/th/settings/branches`: รายการ, filter สถานะ, สร้างสาขา → เปิดหน้าแก้ไข → รหัสสาขาแก้ไม่ได้.
3. สาขาที่มีเอกสาร/ผู้ใช้ → แสดงรายการ blocker และปุ่มปิดใช้งานถูก disable; สาขาว่าง → กรอกเหตุผล → modal ยืนยัน → ปุ่มถูกล็อกระหว่างยิง API → สถานะเปลี่ยน → เปิดใช้งานกลับได้.
4. ความกว้าง 375px ไม่มี horizontal scroll; target ≥ 44px; focus ring มองเห็นได้; Tab ผ่านฟอร์มและ modal ได้ด้วยคีย์บอร์ด.
5. ผู้ใช้ที่มีเฉพาะ `organizations.read`: เห็นฟอร์มแบบ read-only และไม่เห็นเมนูสาขาที่แก้ไขได้.

## 4. Mutation check (Task 9)

ควบคุมการตรวจโดยผู้ประสานงาน (ไม่ใช่รายงานของผู้ทำ):

- ปิด guard ใน transaction ของ `SetBranchActiveAsync` ด้วย `if (false && blockers.Count > 0)` แล้วรัน `OrganizationAdministrationEndpointsTests`: 23 tests รัน, **19 passed, 4 failed** (RED ตามที่ต้องการ).
- Revert ด้วย `git checkout` แล้วตรวจซ้ำ: GREEN 23/23 ที่ commit `9cce827`.

## 5. การตัดสินใจที่ต้องให้ Business Owner ยืนยัน

1. **`service_requests`** (open/scheduled/in_progress/resolved) นับเป็น blocker แม้แผนเดิมไม่ได้ระบุ; ADR 0019 และ contract ได้เพิ่มรายการนี้แล้ว.
2. **`CostRecord`** (มี `BranchId` optional) **ไม่นับ** เป็น blocker เพราะไม่มีเส้นทางยกเลิกจาก Draft; ถ้านับจะทำให้สาขาปิดไม่ได้ตลอดไป.
3. Entity ที่มี `BranchId` และไม่นับ (เหตุผลอยู่ใน `BranchDependencyInspectorCoverageTests`): ItemBranchAvailability, CalculationPolicyVersion, TaxPolicyVersion, DocumentSequenceCounter, RolePermission, AuditEvent, GoodsReceipt, StockDocument.
4. Namespace ของ store คือ `TanErp.Infrastructure.Persistence.OrganizationAdministration` ขณะที่โฟลเดอร์คือ `Persistence/Organization` (ชื่อ `Organization` ตรงๆ ชนกับ namespace ของโดเมน).
5. OpenAPI snapshot ใน commit `8a8be98` สร้างใหม่จาก live swagger fetch (ยืนยันโดย `OpenApiContractTests` ผ่าน) ไม่ได้สร้างผ่าน test `UPDATE_OPENAPI`.

## 6. ข้อจำกัดและสิ่งที่ยังไม่ได้ทำ

- **ช่องว่างแข่งขัน**: เอกสารใหม่ที่สร้างพร้อมการปิดสาขาอาจหลุดการนับ เพราะการสร้างเอกสารไม่ล็อกแถวสาขา (ADR 0019). ถ้า Operations ต้องการรับประกันเด็ดขาด ให้เปิด slice เพิ่ม check `branch.is_active` ใน document creation.
- **Production ต้องมอบสิทธิ์ก่อนใช้**: `organizations.manage` และ `branches.manage` ถูก seed เฉพาะ Test environment. Production ต้องมอบให้ Role จริงด้วยมือหรือสคริปต์ bootstrap ก่อนใช้งาน.
- **นิยาม "เอกสารเปิด" และกฎ membership เป็น TEST_ONLY** รอ Business Owner ยืนยัน.
- **โลโก้เลื่อนไป G-04**; การมีหลาย Organization ต่อ deployment ยังเป็น Validation Question.
- **ไม่มี pagination** ของรายการสาขา.
- ยังไม่รัน full backend suite และ full frontend suite (รันเฉพาะ focused filter และ regression ของโมดูลที่แตะ).
- ยังไม่มี Playwright journey และยังไม่มี UAT ด้วย Role จริง.
