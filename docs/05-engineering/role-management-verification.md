# Role Management Verification (G-03b)

## 1. สถานะ

**Implemented 2026-10-11 (G-03b: สร้าง/แก้/ปิด-เปิดใช้ Role จาก Permission catalog และ maker–checker ของการแก้ที่เพิ่มอำนาจอนุมัติให้ Role ที่มีผู้ถือ) — tests written, not run.**

- ชุดทดสอบ backend, integration และ frontend ถูก **เขียนแล้ว แต่ยังไม่ได้รันในรอบนี้** เพราะผู้ใช้เป็นผู้รันชุดทดสอบเองทั้งหมด. ผลผ่าน/ล้มของชุดทดสอบ G-03b **ยังไม่มี** ณ วันที่เอกสารนี้.
- **ยังไม่ได้ตรวจในเบราว์เซอร์** (ดูหัวข้อ 4).
- **ยังไม่รัน full gate ในเครื่อง**, ยังไม่ผ่าน UAT, และยังไม่พร้อม Production.
- G-03c (Approval Authority matrix) ยัง pending และไม่อยู่ในขอบเขตเอกสารนี้.

สัญญา: [Role Management API Contract](../03-contracts/role-management-api-contract.md); การตัดสินใจ: [ADR 0020](../adr/0020-role-management-and-approval-permission-set.md); แผน: [แผน G-03](../superpowers/plans/2026-10-10-g03-org-role-authority.md); G-03a: [Organization Administration Verification](organization-administration-verification.md).

## 2. ผลที่รันจริงในรอบนี้ (compile/lint เท่านั้น ไม่ใช่การรันเทสต์)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build TanErp.slnx --nologo -v q` (จาก `backend/`) | `Build succeeded.` `0 Warning(s)` `0 Error(s)` |
| `npx tsc --noEmit` (จาก `frontend/`) | exit 0 |
| `npm run lint` (จาก `frontend/`) | exit 0 |

ผลจาก Task ก่อนหน้าในแผน G-03b (รายงานจากการทำงานของ Task นั้น ยังไม่ได้รันซ้ำในรอบนี้):

| รายการ | ผลที่รายงาน |
| --- | --- |
| `npm run check:api` | ผ่าน |
| `npm run build` (frontend production build) | ผ่าน |
| key parity `frontend/src/messages/th.json` ↔ `en.json` | 3748 keys ทั้งคู่, ต่างกัน 0 |
| `dotnet ef migrations has-pending-model-changes` | ไม่มีการเปลี่ยนแปลงค้างหลัง migration `AddRoleManagement` |
| Migration `AddRoleManagement` | สร้างแล้วและ apply กับฐานข้อมูล dev ในเครื่องด้วย `make db-migrate` ร่วมกับ `AddNotifications` และ `AddOrganizationProfileAndBranchDetails` |

**ไม่มีรายการใดในหัวข้อนี้ที่แสดงว่าเทสต์ G-03b ผ่าน.**

## 3. ชุดทดสอบที่เขียนแต่ยังไม่ได้รัน

จำนวนด้านล่างนับจากการค้นหาในซอร์สโค้ด: backend นับจำนวน attribute `[Fact]`/`[Theory]` (Theory ที่มี `InlineData` หลายแถวนับเป็น 1), frontend นับจำนวนการประกาศ `it(`/`test(` (ไม่ขยาย `.each`). จำนวนที่รันจริงจะต่างได้ และต้องดูจากผลรันเท่านั้น.

| ชุด | ไฟล์ | จำนวนที่นับได้ |
| --- | --- | ---: |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/AdministrationPolicyTests.cs` | 12 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalPermissionCoverageTests.cs` | 1 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/RoleChangeRequestTests.cs` | 2 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/RoleDomainTests.cs` | 5 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/RoleManagementPolicyTests.cs` | 7 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/RolePermissionTests.cs` | 12 |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs` | 34 |
| Frontend | `frontend/src/features/settings/role-management/components/permission-grant-picker.test.tsx` | 2 |
| Frontend | `frontend/src/features/settings/role-management/components/role-activation-panel.test.tsx` | 3 |
| Frontend | `frontend/src/features/settings/role-management/components/role-change-request-queue.test.tsx` | 3 |
| Frontend | `frontend/src/features/settings/role-management/components/role-definition-editor.test.tsx` | 6 |
| Frontend | `frontend/src/features/settings/role-management/components/role-definition-list.test.tsx` | 3 |
| Frontend | `frontend/src/features/settings/role-management/role-management-errors.test.ts` | 2 |
| Frontend | `frontend/src/components/layout/SidebarNav.test.tsx` | 10 |
| Frontend | `frontend/src/lib/api/api-client.test.ts` | 13 |

รวม backend ที่นับได้ 39 (unit) + 34 (integration); frontend ที่นับได้ 42 declarations (role-management 19, SidebarNav 10, api-client 13).

**Mutation check ไม่ได้รันในรอบนี้** (Task 3, 7, 8, 9, 10 ของแผนเขียนขั้นตอน mutation ไว้ แต่ผู้ใช้กำหนดว่าไม่รันเทสต์ในเซสชันนี้). จึงยังไม่มีหลักฐานว่าชุดทดสอบจับการถอยหลังของ guard ได้จริง.

## 4. การตรวจในเบราว์เซอร์

**ไม่ได้ตรวจในเบราว์เซอร์** ในรอบนี้. ทุกข้อด้านล่างเป็นรายการที่ต้องตรวจก่อน UAT:

1. `/th/settings/roles`: รายการ, ค้นหา, filter สถานะ, badge `ระบบ` ของ Test Admin; สลับ `/en/` ข้อความเป็นอังกฤษ.
2. `create` → ใส่ชื่อ + เลือกสิทธิ์ → บันทึก → เปิดหน้าแก้ไข → Role ระบบ (`Test Admin`) เป็น read-only ไม่มีปุ่มบันทึก.
3. Role ที่มีผู้ถือ: ติ๊กสิทธิ์อนุมัติ → เห็นข้อความเตือน → บันทึก → toast "ส่งคำขอแก้ไข..." → Role แสดง banner รออนุมัติ; ผู้ใช้อีกคนที่มี `roles.assign-approval` เปิด `/th/settings/role-requests` เห็นคำขอและอนุมัติได้.
4. ปิดใช้งาน Role ที่มีผู้ถือ → ปุ่ม disabled และแสดงจำนวนผู้ถือ; Role ที่ว่าง → ปิด/เปิดได้.
5. จอ 375px: ไม่มี horizontal scroll, target ≥ 44px, focus ring มองเห็นได้, Tab ผ่านฟอร์ม/picker/modal ได้.

## 5. สิ่งที่ชุดทดสอบ **ออกแบบให้ตรวจ** (ยังไม่ได้รัน ยังไม่ถือเป็นหลักฐาน)

- anti-escalation ตรวจเฉพาะ grant ที่เพิ่ม และการถอน grant ไม่ถูกปฏิเสธ (`ROLE_ESCALATION_DENIED`).
- System Role (`is_system`) แก้ ปิด เปิดไม่ได้ (`ROLE_SYSTEM_IMMUTABLE`) และ API ไม่มีทางตั้ง flag นี้.
- maker–checker เมื่อเพิ่ม grant ให้ Role ที่มีผู้ถือ, ผู้ตัดสินต้องไม่ใช่ผู้ขอ (`ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED`) และมี DB constraint กันซ้ำ.
- stale request (`ROLE_CHANGE_REQUEST_STALE`) เมื่อ Role เปลี่ยนหลังขอ.
- last administrator (`LAST_ADMINISTRATOR_REQUIRED`) เมื่อถอน `users.manage` จากแหล่งผู้ดูแลคนสุดท้าย.
- ปิด Role ที่มีผู้ถือหรือมีคำขอค้างไม่ได้ (`ROLE_IN_USE`, `ROLE_CHANGE_REQUEST_PENDING`).
- ทุก endpoint ที่ id ของอีกองค์กรตอบ 404 และไม่มีสิทธิ์ตอบ 403 เหมือนกันทุก id; idempotency replay / key reuse (`IDEMPOTENCY_KEY_REUSED`); ETag/If-Match (`ADMIN_VERSION_CONFLICT`, `IF_MATCH_REQUIRED`).
- audit ของ Role ไม่เก็บชื่อหรือคำอธิบายของ Role.
- `ApprovalPermissionCoverageTests` กวาดคีย์ `*.approve/publish/review` ใน seeder และบังคับให้อยู่ใน approval set หรือใน reviewed-not-approval อย่างชัดเจน; ล็อกชุด approval 6 คีย์.

## 6. คำสั่งที่ผู้ใช้ควรรัน

รันแยกทีละคำสั่ง. ค่า `Password=x` ในคำสั่ง EF เป็นค่าตัวอย่างสำหรับ design-time เท่านั้น.

**Backend (จาก root ของ repo):**

```bash
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~IdentityAccess"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~RoleManagementEndpointsTests|FullyQualifiedName~OpenApiContractTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~UsersEndpointsTests|FullyQualifiedName~NotificationEndpointsTests|FullyQualifiedName~OrganizationAdministrationEndpointsTests" -m:1
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```

**Frontend (จาก `frontend/`):**

```bash
npm run check:api
npm run lint
npm run typecheck
npx vitest run src/features/settings src/components/layout src/lib
npm run build
```

**Full gate ก่อนส่งมอบ (AGENTS.md หมวด 8):** `dotnet build backend/TanErp.slnx`, `dotnet test backend/TanErp.slnx` (ใช้เวลานาน ราว 12 นาทีขึ้นไป), `npm run build`, `npm run lint` ใน `frontend/`. ยังไม่มีการรัน full gate สำหรับ G-03b ในเครื่อง.

## 7. การตัดสินใจที่ต้องให้ Business Owner ตัดสิน

1. **Approval Permission Set มี 6 คีย์** (จาก 3): `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve` ถูกเพิ่ม. การมอบ Role ที่มีคีย์เหล่านี้ตอนนี้ต้องผ่าน maker–checker (ADR 0020 การตัดสินใจ 5) — เป็นการเปลี่ยนพฤติกรรมของ CP-02.
2. **กฎ "เพิ่ม grant ใดก็ได้ให้ Role ที่มีผู้ถือและ RequiresApproval"** เข้มงวดโดยตั้งใจ (ADR 0020 การตัดสินใจ 3) ต้องยืนยันว่ารับได้.
3. **ปิด Role ต้องไม่มีผู้ถือ** (ADR 0020 การตัดสินใจ 4) และไม่มีการถอน Role ให้อัตโนมัติ.
4. **`roles.manage` และ `roles.assign-approval` ยังไม่อยู่ใน approval set** (ดูข้อจำกัด 1).

## 8. ข้อจำกัดที่ยอมรับ

1. **R1 (ADR 0020):** `roles.manage` และ `roles.assign-approval` เป็น meta-privilege แต่ไม่อยู่ใน approval set จึงเพิ่มให้ Role ที่มีผู้ถือได้โดยไม่มีผู้ตรวจ. ผู้แก้ยังต้องถือสิทธิ์นั้นเองตาม anti-escalation.
2. **R2:** ไม่มี notification แจ้งผู้ตรวจ ผู้ตรวจต้องเปิดคิว `settings/role-requests` เอง.
3. **การปิด Role** ถูกบล็อกเมื่อมี `membership_roles` ใดอ้างอิง Role นั้น (รวม Membership ที่ inactive) หรือมีคำขอมอบ/แก้ไขค้าง. การบล็อกกว้างเกินเหตุในบางกรณี แต่ป้องกันการถอนสิทธิ์เงียบ ๆ.
4. **คิวคำขอแก้ไข Role** จำกัด 200 รายการและไม่มี pagination. สำหรับ status filter ที่ไม่ใช่ `pending` diff ถูกคำนวณกับ grant **ปัจจุบัน** ของ Role ดังนั้นคำขอที่ตัดสินแล้วอาจแสดง diff ที่ทำให้เข้าใจผิด (มุมมองเริ่มต้น `pending` ถูกต้อง).
5. **PUT/POST ที่ใช้ If-Match** ตอบ `428 IF_MATCH_REQUIRED` ก่อน `403` เมื่อไม่ส่ง header (สอดคล้องกับ controller อื่น และไม่เปิดเผยการมีอยู่ของ resource).
6. **Permission และ `is_system` ถูก seed เฉพาะ Test environment.** Production ต้องให้ `roles.manage` (และ `roles.assign-approval` ให้ผู้ตรวจ) แก่ Role จริง และตั้ง system role ด้วย bootstrap/สคริปต์ก่อนใช้งาน.
7. **Test seed:** `Test Admin` และ `Test Admin B` ถูกทำเครื่องหมายเป็น system role (`MarkAsSystemRole`) ดังนั้นแก้/ปิดไม่ได้จาก UI ตามที่ออกแบบ.
8. **UI ยังไม่รองรับการเพิ่ม grant ระดับ `branch` หรือ `own`** (เพิ่มได้ทาง API). เมื่อบันทึกจาก UI grant ระดับเหล่านี้ที่มีอยู่แล้วจะถูกคงไว้.
9. **ไม่มีปุ่มยกเลิกคำขอใน UI** (มี endpoint `cancel` ใน API).
10. **ไม่มีการถอน Role อัตโนมัติ** เมื่อปิด Role (ผู้ดูแลถอนที่หน้าผู้ใช้เอง โดยมี audit เดิม).

## 9. ข้อจำกัดด้านหลักฐาน

- ไม่มีการรันชุดทดสอบ G-03b ในรอบนี้ ทุกข้อในหัวข้อ 5 เป็นเพียงสิ่งที่ชุดทดสอบออกแบบให้ตรวจ.
- ไม่มี Playwright journey และยังไม่มี UAT ด้วย Role จริง.
- การผ่านการ compile/lint ไม่ได้แทนการผ่าน UAT หรือ Production sign-off.
