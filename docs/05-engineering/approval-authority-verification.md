# Approval Authority Verification (G-03c)

## 1. สถานะ

**Implemented 2026-10-12 (G-03c: Approval Authority matrix — วงเงินต่อ Role/เอกสาร/สาขา บังคับที่ Estimate, Purchase Order และ Change Order แบบ fail-closed) — tests written, not run.**

- ชุดทดสอบ backend (unit + integration) และ frontend ถูก **เขียนแล้ว แต่ยังไม่ได้รันในรอบนี้** เพราะผู้ใช้เป็นผู้รันชุดทดสอบเองทั้งหมด. ผลผ่าน/ล้มของชุดทดสอบ G-03c **ยังไม่มี** ณ วันที่เอกสารนี้.
- **ยังไม่ได้ตรวจในเบราว์เซอร์** (ดูหัวข้อ 4).
- **ยังไม่รัน full gate** ในรอบนี้, ยังไม่ผ่าน UAT, และยังไม่พร้อม Production.
- Credit Note ยังไม่ถูกบังคับ (สงวนไว้ของ G-15).

สัญญา: [Approval Limit API Contract](../03-contracts/approval-limit-api-contract.md); การตัดสินใจ: [ADR 0021](../adr/0021-approval-authority-matrix.md); แผน: [แผน G-03 Part 3](../superpowers/plans/2026-10-10-g03-org-role-authority.md); ก่อนหน้า: [Role Management Verification (G-03b)](role-management-verification.md).

## 2. ผลที่รันจริงในรอบนี้ (compile/lint เท่านั้น ไม่ใช่การรันเทสต์)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` (จาก root ของ repo) | `Build succeeded.` `0 Warning(s)` `0 Error(s)` |
| `npx tsc --noEmit` (จาก `frontend/`) | exit 0 |
| `npm run lint` (จาก `frontend/`) | exit 0 |

ผลจาก Task ก่อนหน้าในแผน G-03c (รายงานจากการทำงานของ Task นั้น ยังไม่ได้รันซ้ำในรอบนี้):

| รายการ | ผลที่รายงาน |
| --- | --- |
| `dotnet ef migrations has-pending-model-changes` | ไม่มีการเปลี่ยนแปลงค้างหลัง migration `AddApprovalLimits` |
| Migration `AddApprovalLimits` | สร้างแล้วและ apply กับฐานข้อมูล dev ในเครื่องด้วย `make db-migrate` |
| `FORMATTER_OK` (OpenAPI snapshot) | สร้างจาก live `/swagger/v1/swagger.json` และ format ด้วย writer เดียวกับ G-03a/G-03b (round-trip byte-identical ตามรายงานของ Task 9); ไม่ได้ใช้ `UPDATE_OPENAPI` |
| `npm run check:api` (generate + diff) | ผ่านตามรายงานของ Task 9 |
| key parity `frontend/src/messages/th.json` ↔ `en.json` | 3812 keys ทั้งคู่ (ตามรายงานของ Task 10–11) |
| `npm run build` (frontend production build) | ผ่านตามรายงานของ Task 11 |

**ไม่มีรายการใดในหัวข้อนี้ที่แสดงว่าเทสต์ G-03c ผ่าน.**

## 3. ชุดทดสอบที่เขียนแต่ยังไม่ได้รัน

จำนวนด้านล่างนับจากการค้นหาในซอร์สโค้ด: backend นับจำนวน attribute `[Fact]`/`[Theory]` (Theory ที่มี `InlineData` หลายแถวนับเป็น 1), frontend นับจำนวนการประกาศ `it(`/`test(` (ไม่ขยาย `.each`). จำนวนที่รันจริงจะต่างได้ และต้องดูจากผลรันเท่านั้น.

| ชุด | ไฟล์ | จำนวนที่นับได้ |
| --- | --- | ---: |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalLimitDomainTests.cs` | 4 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalAuthorityPolicyTests.cs` | 4 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/AdministrationPolicyTests.cs` (แก้ใน G-03c: ชุด approval 7 คีย์) | 12 |
| Unit | `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalPermissionCoverageTests.cs` | 1 |
| Unit | `backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs` (แก้ใน G-03c) | 1 |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/ApprovalLimitEndpointsTests.cs` | 6 |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` (เพิ่ม `ReviewEstimate_EnforcesApprovalAuthority_AndReturnNeedsNoLimit`) | 1 ใหม่ (ไฟล์รวม 42) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/ProcurementEndpointsTests.cs` (เพิ่ม `ApproveOrder_EnforcesApprovalAuthority_AndRejectNeedsNoLimit`, `ApprovalAuthority_UsesTheHighestCeilingAcrossActiveRoles_AndIgnoresOtherCurrencies`) | 2 ใหม่ (ไฟล์รวม 8) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/ProjectControlEndpointsTests.cs` (เพิ่ม `ApproveChangeOrder_EnforcesApprovalAuthority_UsingTheLargerDelta`) | 1 ใหม่ (ไฟล์รวม 6) |
| Integration | `backend/tests/TanErp.IntegrationTests/Support/ApprovalLimitTestSupport.cs` | helper ไม่มี test method (`SetLimitAsync`) |
| Frontend | `frontend/src/features/settings/approval-limits/approval-limit-errors.test.ts` | 2 |
| Frontend | `frontend/src/features/settings/approval-limits/components/approval-limit-editor.test.tsx` | 3 |
| Frontend | `frontend/src/features/settings/approval-limits/components/approval-limit-list.test.tsx` | 1 |
| Frontend | `frontend/src/components/layout/SidebarNav.test.tsx` (แก้ใน G-03c) | 11 |
| Frontend | `frontend/src/lib/api/api-client.test.ts` (แก้ใน G-03c) | 14 |

รวม backend ที่เกี่ยวกับ G-03c ที่นับได้: unit 22 (4 + 4 + 12 + 1 + 1), integration 6 (ApprovalLimitEndpointsTests) + 4 enforcement ใหม่ (Estimate 1, Procurement 2, ProjectControl 1); frontend 17 (approval-limits 6, SidebarNav 11) + api-client 14 declarations.

ชุด Estimate/PO/Change Order เดิม (`EstimateEndpointsTests`, `ProcurementEndpointsTests`, `ProjectControlEndpointsTests`) ถูกแก้ fixture ให้มีวงเงินของ Role ที่ใช้อนุมัติ แต่ **ยังไม่มีการรันยืนยันว่าผ่าน**.

**Mutation check ไม่ได้รันในรอบนี้** (ผู้ใช้กำหนดว่าไม่รันเทสต์ในเซสชันนี้). จึงยังไม่มีหลักฐานว่าชุดทดสอบจับการถอยหลังของ guard ได้จริง.

## 4. การตรวจในเบราว์เซอร์

**ไม่ได้ตรวจในเบราว์เซอร์** ในรอบนี้. ทุกข้อด้านล่างเป็นรายการที่ต้องตรวจก่อน UAT:

1. `/th/settings/approval-limits`: รายการ, filter ตาม Role/ประเภทเอกสาร/สถานะ, pagination; สลับ `/en/` ข้อความเป็นอังกฤษ.
2. สร้างวงเงินใหม่ (Role + ประเภท + สาขาหรือทั้งองค์กร + THB) → บันทึก → เปิดหน้าแก้ไข → role/ประเภท/สาขา/สกุลเปลี่ยนไม่ได้; แก้เฉพาะ `isUnlimited`, `maxAmount`, `isActive`.
3. สร้างซ้ำคู่เดิม (`APPROVAL_LIMIT_ALREADY_EXISTS`) และเลือก Role ที่ปิดใช้แล้ว → ข้อความ error แสดงถูกต้อง.
4. ทดสอบ flow อนุมัติ Estimate/PO/Change Order ด้วยผู้ใช้ที่มีวงเงินต่ำกว่ายอด → ข้อความ `APPROVAL_LIMIT_EXCEEDED` แสดงในหน้าอนุมัติเดิม; ไม่มีวงเงิน → `APPROVAL_AUTHORITY_NOT_CONFIGURED`.
5. จอ 375px: ไม่มี horizontal scroll, target ≥ 44px, focus ring มองเห็นได้, Tab ผ่านฟอร์ม/dropdown/modal ได้.

## 5. สิ่งที่ชุดทดสอบ **ออกแบบให้ตรวจ** (ยังไม่ได้รัน ยังไม่ถือเป็นหลักฐาน)

- **Policy:** ไม่มีแถวของ Role = ปฏิเสธ; ยอดเท่ากับเพดานผ่าน; ไม่จำกัด (`is_unlimited`) ผ่านทุกยอด; แถวเฉพาะสาขาชนะแถวทั้งองค์กรต่อ Role แม้ต่ำกว่า; ข้าม Role ใช้เพดานสูงสุด; Role ที่ปิดใช้หรือแถวที่ปิดใช้ไม่ให้อำนาจ; สกุลอื่นนอก THB ไม่มีวงเงิน.
- **Domain:** ค่า reserved (`credit_note`), สกุลเงิน, กฎยอด (ไม่เป็นลบ, ทศนิยมไม่เกิน 2 ตำแหน่ง, `is_unlimited` ไม่ส่ง `maxAmount`).
- **Endpoint (limit admin):** permission ตรวจก่อน store (403 เหมือนกันทุก id, other-org 404), canonicalize, validation/reference (`BRANCH_NOT_FOUND`, `APPROVAL_LIMIT_ROLE_NOT_FOUND`), unique ทั้งแถว active/inactive, Idempotency replay/reuse, ETag/If-Match (`ADMIN_VERSION_CONFLICT`), audit `approval-limits.created` / `approval-limits.updated`, filter/pagination.
- **Enforcement 3 เส้นทาง:** Estimate ตรวจเฉพาะ decision `approved` ของ step สุดท้าย (`returned` ไม่ตรวจ); PO และ Change Order ตรวจเฉพาะ Approve (Reject ไม่ตรวจวงเงิน); Change Order ใช้ `max(abs(ContractDelta), abs(BudgetDelta))` ใน THB; หลาย Role ของผู้อนุมัติ ใช้เพดานสูงสุด; เกินเพดาน = 403 `APPROVAL_LIMIT_EXCEEDED`; ไม่มีวงเงิน = 403 `APPROVAL_AUTHORITY_NOT_CONFIGURED`.
- **Permission set:** `approval-limits.manage` เป็นคีย์ที่ 7 ของ `ApprovalPermissionKeys` และ `PermissionCatalogParityTests` ล็อกชุดนี้.

## 6. Fixture ที่เดิมอนุมัติเอกสารและยังใช้ได้ (ผ่าน seed/fixture)

- `TestOnlyDataSeeder` สร้างแถววงเงิน org-wide `unlimited` THB ให้ `Test Admin` และ `Test Admin B` (estimate, purchase_order, change_order), `Test Cost Reviewer` และ `Test Estimate Reviewer` (estimate) และให้สิทธิ์ `approval-limits.manage` แก่ Test Admin ผ่านลูป seed.
- `EstimateEndpointsTests` ใน `SeedTestOnlyApprovalReviewersAsync` สร้าง role ของ TestOnly policy 4 ตัวพร้อมวงเงิน estimate unlimited; test ที่ลดวงเงินจะคืนค่าใน `finally` เพราะวงเงินใช้ร่วมกันทั้ง class ของ xUnit.
- Approver ของ `ProcurementEndpointsTests` / `ProjectControlEndpointsTests` ใช้ role `Test Admin` (มีวงเงินแล้ว).
- e2e `official-estimate.spec.ts` ใช้ `Test Estimate Reviewer` (มีวงเงินแล้ว) — **ยังไม่ได้รัน**.
- Role ใหม่ที่ test อื่นสร้างเองและ *อนุมัติ* เอกสารจริงต้องเพิ่มแถววงเงินเอง (ไม่พบในรอบนี้).

## 7. การตัดสินใจที่ต้องให้ Business Owner / Finance / Security ยืนยัน

1. **D3 — Estimate ตรวจที่ step สุดท้ายเท่านั้น:** step กลาง (Manager/Financial ใน route หลาย step) ทำหน้าที่ตรวจ ไม่ใช่ผู้ถืออำนาจวงเงิน. Production bootstrap มี step เดียวจึงตรวจทุกครั้ง.
2. **D4 / R1 — ไม่มี maker–checker ต่อการแก้วงเงิน:** ผู้ถือ `approval-limits.manage` ยกวงเงินของ Role ตนเองได้ทันที. การมอบ/เพิ่ม permission นี้ให้ Role ต้องผ่าน maker–checker เดิมของ G-03b.
3. **D2 — กฎหลาย Role และแถวสาขา:** ต่อ Role แถวเฉพาะสาขาชนะแถวทั้งองค์กร (แม้ต่ำกว่า); ข้ามหลาย Role ใช้ค่าสูงสุด (unlimited ชนะทุกตัวเลข).
4. **Change Order:** ยอดตรวจคือค่าสัมบูรณ์ที่มากกว่าระหว่าง ContractDelta และ BudgetDelta ในสกุล THB เพราะโครงการไม่มีสกุลเงินของตนเอง.
5. **วงเงินจริงทั้งหมดยังไม่ถูกกำหนด.** ไม่มี production seed; ทุกค่าวงเงินจริงต้องให้ Business Owner กำหนดผ่านหน้า `/settings/approval-limits`.

## 8. ข้อจำกัดที่ยอมรับ / ความเสี่ยง rollout

1. **Fail-closed rollout risk (ความเสี่ยงสำคัญที่สุด):** หลัง deploy การอนุมัติ Estimate/PO/Change Order ใน environment ที่ไม่ใช่ Test จะถูกปฏิเสธ (`APPROVAL_AUTHORITY_NOT_CONFIGURED`, 403) จนกว่าผู้ถือ `approval-limits.manage` จะตั้งวงเงิน. **Production ต้องมอบ `approval-limits.manage` ให้ Role จริงก่อน** (ผ่าน bootstrap/มือ เพราะไม่มี production seed และสิทธิ์นี้อยู่ใน approval set จึงต้องผ่าน maker–checker) แล้วตั้งวงเงินก่อนเปิดใช้การอนุมัติ. ไม่มี backfill และไม่มีปุ่มข้าม.
2. **ApprovalPermissionKeys เพิ่มจาก 6 เป็น 7 (`approval-limits.manage`):** การมอบ permission นี้ให้ Role ที่มีผู้ถือต้องผ่าน maker–checker ของ G-03b. **R1 คงเหลือ:** ผู้ถือสามารถยกวงเงิน Role ตนเองได้ทันที (ไม่มี maker–checker ต่อการแก้วงเงิน; ดูหัวข้อ 7.2).
3. **Credit Note:** `credit_note` อยู่ใน check constraint ของ DB แต่ API ปฏิเสธการสร้างแถว (`REQUEST_VALIDATION_FAILED`) จนกว่า G-15 จะเปิดใช้.
4. **THB เท่านั้น:** ไม่มี FX; สกุลของเอกสารไม่ตรงกับ THB = ไม่มีวงเงิน = ปฏิเสธ.
5. **ไม่เก็บหลักฐานวงเงินใน snapshot/audit ของเอกสาร:** หลักฐาน "ใครอนุมัติ ยอดเท่าไร" อยู่ใน audit เดิม; ประวัติการแก้วงเงินอยู่ใน audit ของ `ApprovalLimit`.
6. **Isolation ของ 3 flow เดิมเป็น default** (ไม่ Serializable) และใช้ advisory lock: การแก้วงเงินพร้อมกันมีผลกับการอนุมัติครั้งถัดไป. บันทึกไว้เป็นข้อสังเกตจากการอ่านโค้ด ยังไม่ได้ทดสอบ concurrency.
7. **Controller อ่าน `Idempotency-Key` / `If-Match` ก่อนตรวจ permission:** ผู้เรียกที่ไม่มีสิทธิ์และไม่ส่ง header จะได้ 400/428 แทน 403 (ไม่เปิดเผยการมีอยู่ของ resource) — สอดคล้องกับ controller admin อื่น.
8. **ไม่มี effective dates:** ปิดแถวด้วย `isActive=false` เท่านั้น; ประวัติอยู่ใน audit.
9. **ไม่มีหน้ารายงาน "ใครอนุมัติได้เท่าไร"** ต่อผู้ใช้ใน UI.
10. **ไม่ได้ตรวจในเบราว์เซอร์** (ดูหัวข้อ 4).

## 9. คำสั่งที่ผู้ใช้ควรรันเอง

รันแยกทีละคำสั่ง. ห้ามเขียนผลในเอกสารนี้ว่าผ่านจนกว่าจะเห็นผลรันจริง.

**Focused backend (จาก root ของ repo):**

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~ApprovalLimit|FullyQualifiedName~ApprovalAuthority|FullyQualifiedName~AdministrationPolicyTests|FullyQualifiedName~PermissionCatalogParityTests|FullyQualifiedName~ApprovalPermissionCoverageTests"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ApprovalLimitEndpointsTests|FullyQualifiedName~OpenApiContractTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ReviewEstimate_EnforcesApprovalAuthority|FullyQualifiedName~ApproveOrder_EnforcesApprovalAuthority|FullyQualifiedName~ApprovalAuthority_UsesTheHighestCeiling|FullyQualifiedName~ApproveChangeOrder_EnforcesApprovalAuthority" -m:1
# regression: ทุก flow ที่อนุมัติ Estimate/PO/Change Order ต้องยังผ่านด้วยวงเงินที่ seed
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests|FullyQualifiedName~ProcurementEndpointsTests|FullyQualifiedName~ProjectControlEndpointsTests|FullyQualifiedName~RoleManagementEndpointsTests|FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~ItemCatalogEstimateFlowTests" -m:1
```

**Focused frontend (จาก `frontend/`):**

```bash
npx vitest run src/features/settings/approval-limits src/components/layout/SidebarNav.test.tsx src/lib/api/api-client.test.ts
npm run check:api
```

**E2E (ต้องเริ่ม backend Test env + SeedTestData ตาม foundation-login-runbook.md):**

```bash
cd frontend && npm run test:e2e -- e2e/official-estimate.spec.ts
```

**Full gate ก่อนส่งมอบ (AGENTS.md หมวด 8, รันครั้งเดียว):**

```bash
# Backend (จาก root ของ repo)
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx

# Frontend (จาก frontend/)
npm run build
npm run lint
```

ยังไม่มีการรัน full gate สำหรับ G-03c ในเครื่องในรอบนี้.

## 10. หลักฐานที่ยังขาด

- ไม่มีผลรันชุดทดสอบ G-03c (backend, integration, frontend, e2e) ในรอบนี้.
- ไม่มี mutation check ยืนยันว่า guard จับการถอยหลังได้.
- ไม่มี Playwright/เบราว์เซอร์ journey และยังไม่มี UAT ด้วย Role จริง.
- การผ่าน compile/lint ไม่ได้แทนการผ่าน UAT หรือ Production sign-off.
