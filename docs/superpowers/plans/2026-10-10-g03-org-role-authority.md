# G-03 Organization/Branch, Role Management และ Approval Authority Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Read `AGENTS.md`, `design.md`, `CONTEXT.md`, `.agents/skills/building-erp-apis/SKILL.md` and `.agents/skills/building-erp-forms/SKILL.md` before Task 1. Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: use BSD `sed -i ''` (never GNU `sed -i`).

**Goal:** ให้ผู้ดูแลระบบจัดการโครงสร้างองค์กรและอำนาจอนุมัติได้เองโดยไม่ต้องแก้ DB/seed: (a) แก้ Organization profile (ชื่อ th/en, เลขผู้เสียภาษี, ที่อยู่ออกเอกสาร) และสร้าง/แก้/ปิดใช้/เปิดใช้ Branch พร้อมเลขสาขาภาษี; (b) สร้าง/แก้ Role จาก Permission catalog เดิม โดย Role ระบบแก้ไม่ได้ และการเปลี่ยน Role ที่มี approval permission ต้อง maker–checker; (c) ตาราง Approval Authority (วงเงินต่อ Role/เอกสาร/สาขา) ที่ Backend บังคับใช้ตอนอนุมัติ Estimate, Purchase Order และ Change Order (และเป็นจุดต่อของ Credit Note ใน G-15).

**Architecture:** ใช้รูปแบบ CP-02 ซ้ำทั้งหมด — Controller บาง → Handler (ตรวจ permission ก่อน, ไม่รั่วว่าทรัพยากรมีอยู่) → Store (EF Core, transaction `Serializable`, audit ใน transaction เดียวกัน, `RowVersion` + `If-Match`, `Idempotency-Key` ตอนสร้าง). Organization/Branch เป็น admin module ใหม่ใต้ `api/v1/admin/...` แยก store (`IOrganizationAdministrationStore`) จาก `IdentityAdministrationStore` เพื่อไม่ขยายไฟล์ 700+ บรรทัด; `BranchesController` เดิม (`GET /api/v1/branches`, สาขา active ใช้ใน dropdown) **ไม่ถูกแตะ**. การปิดสาขาตรวจ "เอกสารเปิด" ผ่าน `IBranchDependencyInspector` (Infrastructure) ตัวเดียว ใช้ทั้ง endpoint `deactivation-check` และตัว guard จริงใน transaction เดียวกับการปิด. Permission catalog อยู่ใน **DB** (ตาราง `permissions`) ที่ถูก seed เฉพาะ `TestOnlyDataSeeder` (Test env) + เอกสาร `permission-catalog.md` + `PERMISSIONS` ฝั่ง FE — ไม่มี registry ในโค้ดที่ตรวจความครบ จึงเพิ่ม test parity เองใน Task ที่เพิ่ม key.

**Tech Stack:** .NET 9, EF Core 9 + Npgsql (writes/migrations), PostgreSQL, xUnit + Testcontainers (`postgres:17-alpine`, ต้องมี Docker), Next.js App Router + TypeScript strict, TanStack Query 5, react-hook-form + zod, next-intl (`frontend/src/messages/{th,en}.json`), Vitest + Testing Library, openapi-typescript.

## Scope decision (การแบ่ง slice)

G-03 ใหญ่เกินกว่าจะ merge ครั้งเดียวอย่างปลอดภัย จึงแบ่งเป็น 3 sub-slice ที่ **ส่งตามลำดับ และ merge แยกกันได้** (แต่ละอันมี task, verification doc และ gate ของตัวเอง):

| Sub-slice | ขอบเขต | พึ่งพา |
| --- | --- | --- |
| **G-03a** Organization profile + Branch CRUD | แก้ profile, Branch create/update/deactivate/reactivate, เลขสาขาภาษี, guard เอกสารเปิด/สาขา active สุดท้าย | — |
| **G-03b** Role management | สร้าง/แก้/ปิด Role จาก catalog เดิม, Role ระบบแก้ไม่ได้, anti-escalation (CP-02), maker–checker เมื่อ Role มี approval permission | G-03a ไม่จำเป็น (ใช้ pattern เดียวกัน) แต่ส่งหลัง G-03a เพื่อลดความเสี่ยง |
| **G-03c** Approval Authority matrix | `ApprovalLimit` + `IApprovalAuthority` ต่อเข้า Estimate/PO/Change Order approve | G-03b (วงเงินอ้าง `roleId`) |

**ไม่ทำในรอบนี้ (Out of scope):**
- **หลาย Organization ต่อ deployment** — เป็น Validation Question เดิมของแผนหลัก; ทุก endpoint ยังจำกัดที่ Organization ของผู้เรียก (`access.OrganizationId`) จึงไม่มี endpoint "สร้าง Organization". ไม่ตัดสินใจเรื่อง tenant provisioning ที่นี่.
- **โลโก้ Organization — ตัดสินใจ: เลื่อนไป G-04 (ไม่รวมใน G-03a).** เหตุผล: ADR 0017 owner registry ต้องลงทะเบียน owner type `organization` พร้อมกฎสิทธิ์/ขนาด/ประเภทไฟล์/การอ่านแบบ server-side สำหรับฝังใน PDF; ผู้ใช้ตัวแรกของโลโก้คือ Quotation PDF (G-04) ซึ่งจะกำหนด contract การอ่านจริง. ถ้าทำตอนนี้จะได้ฟิลด์ที่ไม่มีผู้ใช้ (YAGNI) และต้องเดา requirement. G-04 เพิ่ม `logo_attachment_id` ด้วย migration ของตัวเองได้โดยไม่ break.
- **ใบกำกับภาษี / e-Tax** — G-15. G-03a เก็บเฉพาะ `tax_identifier` (13 หลัก) และ `tax_branch_code` (5 หลัก) ที่ G-15 จะอ่าน.
- แก้ `Branch.Code` หลังสร้าง (immutable — ถูกใช้ใน preview เลขที่เอกสารและ projection ของ Estimate/Quotation); ลบ Branch จริง (hard delete ห้ามเสมอ — ปิดใช้เท่านั้น); ย้ายเอกสารข้ามสาขา; Branch hierarchy.
- ผู้ใช้ที่ผูกสาขาที่ปิดแล้ว: G-03a **ไม่ย้าย** membership อัตโนมัติ — ปิดไม่ได้ถ้ายังมี membership active (ดู guard) เพื่อไม่ทำให้ผู้ใช้ค้างสาขาที่ใช้ไม่ได้.

## Findings จากการสำรวจโค้ด (ที่แผนนี้อ้างอิง)

| หัวข้อ | สิ่งที่พบ (ตรวจแล้ว) | ผลต่อแผน |
| --- | --- | --- |
| Organization / Branch today | `Organization` มีแค่ `Name`, `IsActive`, `CreatedAtUtc` (`Domain/Organization/Organization.cs:8-10`); `Branch` มี `Code`, `Name`, `IsActive` (`Branch.cs:11-14`); ไม่มี `RowVersion`, ไม่มีเลขผู้เสียภาษี/ที่อยู่/เลขสาขาภาษี. Tables `organization.organizations`, `organization.branches` (`BranchConfiguration.cs:11`) unique `(organization_id, branch_code)` ชื่อ `ix_branches_organization_id_branch_code` (:24) | ต้องเพิ่มคอลัมน์ + `row_version`; unique ใหม่ต้องตั้ง `HasDatabaseName` และ map ตามชื่อ constraint |
| Existing endpoints | `BranchesController` มีเฉพาะ `GET /api/v1/branches` (permission `organizations.read`, `ListActiveAsync` active เท่านั้น) (`BranchesController.cs:25-41`, `OrganizationBranchReader.cs:15`). ไม่มี endpoint เขียน Organization/Branch ใด ๆ. FE ใช้ `useOrganizationBranches` เป็น dropdown | endpoint admin ใหม่ใต้ `api/v1/admin/...`; ไม่แตะ endpoint เดิม |
| Seeding | Organization/Branch/Role/Permission ถูกสร้างโดย `TestOnlyDataSeeder` (Test env เท่านั้น; `Program.cs:291` guard `IsEnvironment("Test")`; `Program.cs:21-23` ห้ามเปิดใน production). ไม่มี production seeder/bootstrap ในโค้ด | ไม่มี migration seed permission; key ใหม่เพิ่มใน `permKeys` ของ seeder (`TestOnlyDataSeeder.cs:~115-227`) + เอกสาร |
| Permission catalog | **อยู่ใน DB** (`Permission` entity, `Domain/IdentityAccess/Permission.cs`, unique `Key`), seed ผ่าน `permKeys` (`TestOnlyDataSeeder.cs`), เอกสาร `docs/03-contracts/permission-catalog.md`, mirror ฝั่ง FE `frontend/src/lib/permissions/permissions.ts`. **ไม่มี** constant ฝั่ง Backend ครอบทุก key (มีแค่ `AdministrationPermissions` 5 ตัว `AdministrationModels.cs:7-13`) | `branches.manage` และ `roles.manage` มีใน docs + `PERMISSIONS` FE แล้วแต่ **ยังไม่ถูก seed และ Backend ไม่เคยตรวจ**; `organizations.manage` ถูกอ้างใน `RequestAccessResolver.cs:183` (org-wide branch override) แต่ไม่อยู่ใน docs/seed. G-03a ใช้ `organizations.read` (เดิม), `organizations.manage` (ใหม่ใน docs+seed), `branches.manage` (มีใน docs, เพิ่มใน seed) |
| Branch FK ในเอกสาร | ตารางที่มี `BranchId`: `Estimate` (`Estimates/Estimate.cs:9`), `Quotation` (`Commercial/Quotation.cs:7`), `PurchaseOrder` (`Procurement/PurchaseOrder.cs:10`), `GoodsReceipt` (:11), `BillingDocument` (`Finance/BillingDocument.cs:16`), `WorkOrder` (`Production/WorkOrder.cs:10`), `Project` (`Projects/Project.cs:12`), `MrpRun` (`Mrp/MrpRun.cs:11`), `Opportunity` (`Crm/Opportunities/Opportunity.cs:8`), `SiteSurvey` (`Surveys/SiteSurvey.cs:8`), `InstallationJob` (`Service/InstallationJob.cs:14`), `QuickEstimate` (`QuickEstimates/QuickEstimate.cs:15`), `StockDocument` (`Inventory/StockDocument.cs:11`), `Warehouse` (`Inventory/Warehouse.cs:8`), `ItemBranchAvailability`, `DocumentSequenceCounter`, `TaxPolicyVersion`/`CalculationPolicyVersion` (branch? nullable), `Membership.BranchId`, `RolePermission.BranchId`, `AuditEvent.BranchId` | ใช้ FK เป็นตัวบอก "อ้างอิง" แต่ **นิยาม "เปิด" ต้องมาจาก status ของแต่ละตาราง** (แถวถัดไป) |
| นิยาม "เอกสารเปิด" (blocker) | นับเป็นเปิดเมื่อ: Estimate `status ∉ {quoted, cancelled}` (`EstimateValues.cs:5-10`); Quotation `status = 'issued'` (`Quotation.cs:13`); PurchaseOrder `∈ {draft, submitted, approved, partially_received}` (`ProcurementValues.cs:21-27`); BillingDocument `∈ {issued, partially_paid}` (`FinanceValues.cs:23-28`); WorkOrder `∈ {draft, released, in_progress}` (`ProductionValues.cs:20-27`); Project `∈ {planned, active, on_hold, ready_for_handover}` (`ProjectValues.cs:3-10`); InstallationJob `¬IsTerminal` = `∈ {planned, in_progress, ready_for_handover}` (`ServiceValues.cs:13-22`); SiteSurvey `∈ {scheduled, in_progress}` (`SurveyValues.cs:3-8`); Opportunity `stage ∉ {won, lost, cancelled}` (`OpportunityValues.cs:5-14`); QuickEstimate `status ≠ converted` (`QuickEstimateValues.cs:49-56`); MrpRun ที่มี `MrpRecommendation.Status = 'proposed'` (`MrpValues.cs:21`); Warehouse `status = 'active'` (`Inventory/Warehouse.cs:14`) และ Membership `is_active` ที่ผูกสาขา. **ไม่นับ** (immutable/posted, ไม่มี lifecycle เปิด): `GoodsReceipt`, `StockDocument` | `BranchDependencyInspector` นับเป็นรายประเภท → blocker list; ค่าชุดนี้เป็น TEST_ONLY default (Validation Question 2) |
| Anti-escalation + maker–checker (CP-02) | `AdministrationPolicy.Covers` (`AdministrationModels.cs:~39`) = ผู้เรียกต้องถือ permission ที่จะมอบ; `ApprovalPermissionKeys` = `{estimates.approve, cost-records.approve, cost-records.publish}` (:23-28) และ `RequiresApproval` (:31); `WouldRemoveLastAdministrator` (:49); เก็บ pending ใน `RoleAssignmentRequest`; ผู้ตัดสิน `roles.assign-approval` ห้ามเป็นผู้ขอ. `LoadPermissionsAsync` / `LoadRolesWithPermissionsAsync` / `AddAudit` / `FindReplayAsync` / `RunAsync` เป็น **private** ใน `IdentityAdministrationStore.cs` (:487, :537, :714) | G-03b ใช้ `Covers`/`RequiresApproval` ซ้ำ. **ข้อสังเกต:** `ApprovalPermissionKeys` ยังไม่ครอบ `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve` → G-03b ต้องขยาย set นี้ (เป็น decision ที่ต้อง ADR). G-03a ต้องการ `RunAsync` แบบเดียวกัน → เสนอ extract เป็น helper กลาง (Task 4, **Global Reuse proposal**) |
| Idempotency / ETag pattern | `IdempotencyRecord(OrganizationId, Operation, KeyHash, PayloadHash, ResourceId)` (`Domain/Common/IdempotencyRecord.cs`); controller ใช้ `RequestContextReader.ReadIdempotentRequest` / `ReadConditionalAuthenticatedRequest` (`RequestContextReader.cs:36,90`) และตั้ง `Response.Headers.ETag = $"\"{rowVersion}\""` (`AdminUsersController.cs:122,290`); ความผิดพลาด version → `ADMIN_VERSION_CONFLICT` 409 (`ProblemDetailsMapper.cs:17`) | reuse ทั้งหมด; ไม่สร้างกลไกใหม่ |
| Unique violation mapping | 23505 ทั่วไปถูกใช้ใน store อื่น (`EstimateStore.cs:211` ฯลฯ) แต่ `IdentityAdministrationStore.IsUniqueViolation` (:~530) ตรวจแค่ `SqlState` ไม่ดู constraint | G-03a ต้อง map ด้วย `PostgresException.ConstraintName` เฉพาะ (code vs tax-code คนละ error) |
| Approval checks ปัจจุบัน (G-03c) | **Estimate:** permission ที่ `ReviewEstimateHandler.cs:24` (`estimates.approve`), branch ที่ :30 (`HasBranchAccess`), การตัดสินจริง `EstimateStore.ReviewAsync` (`Estimates/EstimateStore.cs:859`), ตั้ง step Approved ที่ :931, ยอดคือ `EstimateRevision.GrandTotal` (`EstimateRevision.cs:33`); ผู้ตรวจอิสระคำนวณที่ `FindIndependentReviewerAsync` (:1091-1139). **PO:** permission ที่ `ProcurementHandler.cs:137` (`purchase-orders.approve` สำหรับ Approve/Reject), การตัดสินที่ `ProcurementStore.cs:358-364` (`CheckApprovalAsync` งบ → `order.Decide`), ยอดคือ `PurchaseOrder.TotalAmount` (`PurchaseOrder.cs:16`). **Change Order:** permission ที่ `ProjectControlHandlers.cs:133` (const :20 `projects.change-orders.approve`), การตัดสินที่ `ProjectControlStore.cs:438-450`, ยอดคือ `ProjectChangeOrder.ContractDelta`/`BudgetDelta` | เป็นจุดต่อ `IApprovalAuthority` ใน G-03c (ตรวจใน Store หลังโหลดเอกสาร เพราะต้องใช้ยอด+สาขา ณ เวลาตัดสิน) |
| Approval permission keys ใน repo | `estimates.approve`, `cost-records.approve`, `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve`, `roles.assign-approval` (`TestOnlyDataSeeder.cs:145,150,161,179,207,225`); `estimates.approve` มี branch-scoped grant พิเศษ (`RequestAccessResolver.cs:32`) | `approval-matrix.md` กำหนดว่า Permission ≠ Authority — G-03c เพิ่มชั้น Authority |
| Frontend admin pattern | `frontend/src/features/settings/user-administration/{api,components}` (list/detail/editor/queue, `userAdminKeys`, `adminErrorKey` + `KNOWN_ADMIN_ERROR_CODES`, `ConfirmationModal`, `useApiRequestContext`); route `app/[locale]/(erp)/settings/users/{page,[id]/page}.tsx` ใช้ `PermissionGuard` + `id==='create'\|'add'`; sidebar active flag ที่ `SidebarNav.tsx:94-95`; `PERMISSIONS` มี `BRANCHES_MANAGE`, `ROLES_MANAGE` แล้ว (`permissions.ts:8-14`) ไม่มี `ORGANIZATIONS_MANAGE` | เพิ่ม feature `settings/organization` ตามรูปแบบเดียวกัน |
| Latest ADR / migration | ADR ล่าสุด `0018-in-app-notification-foundation.md` → ใหม่คือ **0019**; migration ล่าสุด `20261009182901_AddNotifications`; `dotnet ef` ใช้ `--project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations` | |

---

## Validation Questions + TEST_ONLY defaults

ค่าด้านล่างเป็น **TEST_ONLY** — ต้องให้ Business Owner / Finance / Security ยืนยันก่อน production (ตาม `approval-matrix.md` ที่ระบุ Fail-closed จนกว่าวงเงินจริงจะ sign-off).

| # | คำถาม | TEST_ONLY default ที่ใช้ในแผน |
| --- | --- | --- |
| 1 | วงเงินอนุมัติต่อ Role × ประเภทเอกสาร (Estimate, PO, Change Order, Credit Note) เท่าไร และต่อสาขาหรือทั้งองค์กร? (G-03c) | ไม่มีแถววงเงิน = **ปฏิเสธ (fail-closed)**; seed เฉพาะ Test: `Test Admin` ไม่จำกัดสำหรับ Estimate/PO/Change Order สกุล `THB`. วงเงินจริงไม่ถูกเดา |
| 2 | ปิดสาขาได้เมื่อใด? | ปิดไม่ได้ถ้ามี "เอกสารเปิด" ตามตาราง Findings, มี Warehouse active, มี Membership active ผูกสาขา, หรือเป็นสาขา active สุดท้าย; ต้องระบุเหตุผล ≤ 500 ตัวอักษร (เก็บใน audit ไม่เก็บเป็นคอลัมน์) |
| 3 | เลขสาขาภาษี (`tax_branch_code`) บังคับหรือไม่? | เป็น optional แต่ถ้าใส่ต้อง 5 หลัก (`00000` = สำนักงานใหญ่) และไม่ซ้ำภายใน Organization; G-15 จะบังคับตอนออกใบกำกับ |
| 4 | เลขผู้เสียภาษี Organization | optional, ถ้าใส่ต้อง 13 หลักและผ่าน checksum mod-11 ของกรมสรรพากร |
| 5 | ใครแก้ Organization profile / จัดการสาขา / จัดการ Role? | Profile: `organizations.manage`; สาขา: `branches.manage`; Role: `roles.manage` (G-03b). ทั้งหมดระดับ Organization scope เท่านั้น (ไม่มี branch scope) |
| 6 | ชื่อ Role | 3–100 ตัวอักษร, ไม่ซ้ำแบบ case-insensitive ใน Organization (ใช้ `NormalizedName` เดิม), ชื่อขึ้นต้น `system.` สงวนสำหรับ Role ระบบ (G-03b) |
| 7 | เปลี่ยน approval permission ของ Role ที่มีผู้ถืออยู่ | ต้อง maker–checker (ผู้ตรวจ ≠ ผู้ขอ, ต้องถือ `roles.assign-approval`) (G-03b) |

---

## File Structure

### G-03a — Organization profile + Branch CRUD

#### Docs

| ไฟล์ | หน้าที่ |
| --- | --- |
| `docs/adr/0019-organization-branch-administration.md` (ใหม่) | ADR: store แยก, นิยามเอกสารเปิดจาก status ต่อตาราง, ไม่ hard delete, `Branch.Code` immutable, เลื่อนโลโก้ไป G-04 |
| `docs/adr/README.md` (แก้) | ลิงก์ ADR 0019 |
| `docs/03-contracts/organization-administration-api-contract.md` (ใหม่) | สัญญา API/ฟิลด์/validation/error/permission ของ G-03a |
| `docs/03-contracts/permission-catalog.md` (แก้) | เพิ่ม `organizations.manage`, ปรับคำอธิบาย `branches.manage` |
| `docs/03-contracts/error-contract.md` (แก้) | error codes ใหม่ของ G-03a |
| `docs/README.md`, `CONTEXT.md` (แก้) | แผนที่เอกสาร + ศัพท์ (Tax Branch Code, Open Document) |
| `docs/05-engineering/organization-administration-verification.md` (ใหม่) | หลักฐานการทดสอบ + ข้อจำกัด |
| `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md` (แก้) | สถานะ G-03a |

#### Backend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `backend/src/TanErp.Domain/Organization/Organization.cs` (แก้) | `NameEn`, `TaxIdentifier`, `AddressTh/En`, `Phone`, `RowVersion`, `UpdateProfile` |
| `backend/src/TanErp.Domain/Organization/Branch.cs` (แก้) | `NameEn`, `TaxBranchCode`, `AddressTh/En`, `Phone`, `RowVersion`, `UpdateDetails`, `Deactivate/Activate` bump version |
| `backend/src/TanErp.Domain/Organization/OrganizationValues.cs` (ใหม่) | `OrganizationDomainException`, `ThaiTaxIdentifier`, `TaxBranchCodeRule`, `BranchCode` value validators |
| `backend/src/TanErp.Infrastructure/Persistence/Configurations/{OrganizationConfiguration,BranchConfiguration}.cs` (แก้) | คอลัมน์ใหม่, `row_version` concurrency token, unique `ix_branches_organization_id_tax_branch_code` |
| `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddOrganizationProfileAndBranchDetails*.cs` (generate) | schema |
| `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs` (แก้) | seed `organizations.manage`, `branches.manage` |
| `backend/src/TanErp.Infrastructure/Persistence/SerializableTransactionRunner.cs` (ใหม่, Global Reuse) | helper `RunAsync` + `IsSerializationFailure` + `IsUniqueViolation(constraint)` ที่ใช้ร่วม |
| `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs` (แก้เล็กน้อย) | `RunAsync` เรียก runner กลาง (พฤติกรรมเดิม) |
| `backend/src/TanErp.Application/Organization/Administration/OrganizationAdministrationModels.cs` (ใหม่) | permission consts, records (profile, branch, blocker), `OrganizationAdminPolicy` |
| `backend/src/TanErp.Application/Organization/Administration/IOrganizationAdministrationStore.cs` (ใหม่) | port ของ store |
| `backend/src/TanErp.Application/Organization/Administration/OrganizationAdministrationHandler.cs` (ใหม่) | handler เดียวต่อ use case (permission → validate → store) |
| `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs` (ใหม่) | profile get/update, branch list/get/create/update/set-active + audit + idempotency |
| `backend/src/TanErp.Infrastructure/Persistence/Organization/BranchDependencyInspector.cs` (ใหม่) | นับ blocker ต่อประเภท (ใช้ทั้ง check และ guard) |
| `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs` (ใหม่) | request/response records |
| `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs` (ใหม่) | endpoints `api/v1/admin/organization`, `api/v1/admin/branches` |
| `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`, `Program.cs` (แก้) | error codes th/en + DI |
| `contracts/openapi/tan-erp.v1.json` (regenerate) | snapshot |
| `backend/tests/TanErp.UnitTests/Organization/*Tests.cs` (ใหม่) | domain + policy tests |
| `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs` (ใหม่) | endpoint tests (profile, branch CRUD, guard, cross-org) |

#### Frontend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `frontend/src/generated/api/tan-erp.v1.ts` (regenerate), `frontend/src/lib/api/api-client.ts` (แก้) | types + methods |
| `frontend/src/lib/permissions/permissions.ts` (แก้) | `ORGANIZATIONS_MANAGE` |
| `frontend/src/features/settings/organization/api/organization-admin-queries.ts` (ใหม่) | hooks + query keys |
| `frontend/src/features/settings/organization/organization-admin-errors.ts` (+ `.test.ts`) (ใหม่) | whitelist error → key |
| `frontend/src/features/settings/organization/components/organization-profile-form.tsx` (+ `.test.tsx`) (ใหม่) | ฟอร์ม profile (กลุ่มเดียว ไม่ใช้ FormTabs) |
| `frontend/src/features/settings/organization/components/branch-admin-list.tsx` (+ `.test.tsx`) (ใหม่) | ตารางสาขา |
| `frontend/src/features/settings/organization/components/branch-admin-editor.tsx` (+ `.test.tsx`) (ใหม่) | create/edit + deactivate modal |
| `frontend/src/app/[locale]/(erp)/settings/organization/page.tsx`, `settings/branches/page.tsx`, `settings/branches/[id]/page.tsx` (ใหม่) | routes |
| `frontend/src/components/layout/SidebarNav.tsx` (แก้) | เมนู Organization/Branches |
| `frontend/src/messages/th.json`, `en.json` (แก้) | namespace `organizationAdmin` + nav keys |

### G-03b / G-03c

ตาราง File Structure ของ G-03b และ G-03c จะถูกเพิ่มใน Part 2 และ Part 3 ด้านล่าง.

---

# Part 1: G-03a Organization profile + Branch CRUD

## Task 1: ADR, สัญญา API, permission catalog, error codes, ศัพท์ (docs)

**Files:**
- Create: `docs/adr/0019-organization-branch-administration.md`
- Create: `docs/03-contracts/organization-administration-api-contract.md`
- Modify: `docs/adr/README.md`, `docs/03-contracts/permission-catalog.md`, `docs/03-contracts/error-contract.md`, `docs/README.md`, `CONTEXT.md`

ขั้นนี้เป็นเอกสาร ไม่มี test ที่รันได้ — ตรวจด้วย link check ใน Step 6.

- [ ] **Step 1: สร้าง ADR 0019**

เขียน `docs/adr/0019-organization-branch-administration.md` (ภาษาไทย รูปแบบเดียวกับ ADR 0018):

```markdown
---
status: accepted
---

# Organization/Branch Administration: ปิดใช้แทนลบ, นิยาม "เอกสารเปิด" ตาม status รายตาราง, Branch Code แก้ไม่ได้

เอกสารจากทุกโมดูล (Estimate, Quotation, PO, Billing, Work Order, Project, Installation ฯลฯ) อ้าง `branch_id`. ก่อน G-03a ไม่มีทางแก้ Organization profile หรือสร้าง/ปิดสาขาโดยไม่แก้ฐานข้อมูล.

**การตัดสินใจ 1 — ปิดใช้ ไม่ลบ:** Branch ไม่มี hard delete (FK จาก 15+ ตารางเป็น `Restrict`). ปิดสาขาได้เมื่อไม่มี "เอกสารเปิด" (Estimate ไม่ใช่ quoted/cancelled, Quotation issued, PO draft..partially_received, Billing issued/partially_paid, Work Order draft..in_progress, Project planned..ready_for_handover, Installation ไม่ terminal, Site Survey scheduled/in_progress, Opportunity ไม่ใช่ won/lost/cancelled, Quick Estimate ไม่ใช่ converted, MRP run ที่มี recommendation proposed) ไม่มี Warehouse active ไม่มี Membership active และไม่ใช่สาขา active สุดท้าย. `GoodsReceipt`/`StockDocument` เป็นเอกสาร posted ไม่มี lifecycle เปิดจึงไม่นับ. ผู้ตรวจ (`BranchDependencyInspector`) เป็นรหัสตัวเดียวที่ใช้ทั้ง endpoint `deactivation-check` และ guard จริงใน transaction เดียวกับการปิด เพื่อให้ UI กับ Backend ไม่เห็นต่างกัน. เมื่อโมดูลใหม่เพิ่ม `branch_id` ต้องเพิ่มรายการในผู้ตรวจ — test `BranchDependencyInspectorCoverageTests` ไล่ entity ที่มี `BranchId` แล้วล้มเมื่อไม่อยู่ในรายการ "นับ" หรือ "ยกเว้น" อย่างชัดเจน.

**การตัดสินใจ 2 — `Branch.Code` แก้ไม่ได้หลังสร้าง:** ถูกใช้ใน projection ของ Estimate/Quotation และ preview เลขที่เอกสาร. แก้ได้เฉพาะชื่อ th/en, เลขสาขาภาษี, ที่อยู่, โทรศัพท์.

**การตัดสินใจ 3 — เลขผู้เสียภาษี/เลขสาขาภาษีเก็บเป็นข้อมูลธรรมดา:** ตรวจรูปแบบ (13 หลัก+checksum / 5 หลัก) แต่ไม่เรียกกรมสรรพากร. G-15 (ใบกำกับภาษี) เป็นผู้บังคับว่าต้องมี.

**การตัดสินใจ 4 — โลโก้เลื่อนไป G-04:** ต้องลงทะเบียน owner type `organization` ใน owner registry (ADR 0017) พร้อมกฎอ่านฝั่ง server สำหรับ PDF ซึ่ง G-04 เป็นผู้ใช้รายแรก; ทำก่อนจะเป็นฟิลด์ไร้ผู้ใช้.

**การตัดสินใจ 5 — store แยก:** `IOrganizationAdministrationStore` แยกจาก `IdentityAdministrationStore` (700+ บรรทัด). Helper transaction ที่ซ้ำ (`RunAsync`) ถูก extract เป็น `SerializableTransactionRunner` เพื่อให้ G-03b/G-03c ใช้ซ้ำ.

ข้อเสียที่ยอมรับ: ช่องว่างแข่งขัน — เอกสารใหม่ที่สร้างพร้อมการปิดสาขาอาจหลุดการนับ (การสร้างเอกสารไม่ล็อกแถวสาขา). ลดความเสี่ยงด้วยการปิดสาขาเป็น action ที่หายากและทำภายใต้ transaction `Serializable` + advisory lock ต่อ Organization; ไม่เพิ่มการล็อกสาขาในทุกเส้นทางสร้างเอกสาร (blast radius ใหญ่). ถ้า Operations ต้องการรับประกันเด็ดขาด ให้เปิด slice เพิ่ม `branch.is_active` check ใน document creation.
```

- [ ] **Step 2: ลิงก์ใน ADR README**

ต่อท้ายรายการใน `docs/adr/README.md`:

```markdown
- [0019 — Organization/Branch Administration](0019-organization-branch-administration.md)
```

- [ ] **Step 3: สร้างสัญญา API**

เขียน `docs/03-contracts/organization-administration-api-contract.md`:

```markdown
# Organization Administration API Contract (G-03a)

**สถานะ:** Draft → Implemented เมื่อ G-03a เสร็จ ([Verification](../05-engineering/organization-administration-verification.md)). ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0019](../adr/0019-organization-branch-administration.md). ค่าที่ระบุ TEST_ONLY รอ Business Owner ยืนยัน.

## Endpoints

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/organization` | `organizations.read` | คืน profile + `ETag` |
| PUT | `/api/v1/admin/organization` | `organizations.manage` | ต้องมี `If-Match`; คืน profile ใหม่ + `ETag` |
| GET | `/api/v1/admin/branches` | `branches.manage` | รวมสาขา inactive; `?status=active\|inactive` (ค่าว่าง = ทั้งหมด) |
| POST | `/api/v1/admin/branches` | `branches.manage` | ต้องมี `Idempotency-Key`; 201 + `ETag` |
| GET | `/api/v1/admin/branches/{id}` | `branches.manage` | other-org id → 404 |
| PUT | `/api/v1/admin/branches/{id}` | `branches.manage` | ต้องมี `If-Match`; ไม่รับ `code` |
| GET | `/api/v1/admin/branches/{id}/deactivation-check` | `branches.manage` | คืน `canDeactivate` + `blockers[]` |
| POST | `/api/v1/admin/branches/{id}/deactivate` | `branches.manage` | ต้องมี `If-Match` + body `{ "reason": string }` (1–500) |
| POST | `/api/v1/admin/branches/{id}/activate` | `branches.manage` | ต้องมี `If-Match` |

`BranchesController` เดิม `GET /api/v1/branches` (สาขา active สำหรับ dropdown, `organizations.read`) ไม่เปลี่ยน.

## Fields และ validation

| Field | Organization | Branch | กฎ |
| --- | --- | --- | --- |
| `name` | ต้องมี | ต้องมี | trim, ≤ 255 |
| `nameEn` | optional | optional | trim, ≤ 255, ว่าง = null |
| `code` | — | ต้องมีตอนสร้าง, แก้ไม่ได้ | `[A-Za-z0-9_-]`, ≤ 50, unique ต่อ Organization (case-sensitive ตามดัชนีเดิม `ix_branches_organization_id_branch_code`) |
| `taxIdentifier` | optional | — | 13 หลัก + checksum mod-11 |
| `taxBranchCode` | — | optional | 5 หลัก (`00000` = สำนักงานใหญ่), unique ต่อ Organization |
| `addressTh`, `addressEn` | optional | optional | ≤ 500 |
| `phone` | optional | optional | ≤ 30, ตัวเลข `+ - ( ) ` และช่องว่าง |
| `rowVersion` | response | response | = ค่าใน `ETag` |

## Branch response

`{ id, code, name, nameEn, taxBranchCode, addressTh, addressEn, phone, isActive, rowVersion, createdAtUtc }`

## Deactivation check response

`{ canDeactivate: boolean, blockers: [{ type: string, count: number }] }` โดย `type` เป็นหนึ่งใน
`estimates, quotations, purchase_orders, billings, work_orders, projects, installations, site_surveys, opportunities, quick_estimates, mrp_runs, warehouses, memberships, last_active_branch` (`count` ของ `last_active_branch` = 1).

## Errors

ดู [Error Contract](error-contract.md): `ORGANIZATION_TAX_ID_INVALID` 422, `BRANCH_CODE_INVALID` 422, `BRANCH_TAX_CODE_INVALID` 422, `BRANCH_CODE_ALREADY_EXISTS` 409, `BRANCH_TAX_CODE_ALREADY_EXISTS` 409, `BRANCH_HAS_OPEN_DOCUMENTS` 409, `BRANCH_HAS_ACTIVE_MEMBERSHIPS` 409, `BRANCH_LAST_ACTIVE` 409, `ADMIN_VERSION_CONFLICT` 409, `IDEMPOTENCY_KEY_REUSED` 409, `RESOURCE_NOT_FOUND` 404, `PERMISSION_DENIED` 403.

## Audit

`organization.profile-updated`, `branches.created`, `branches.updated`, `branches.deactivated` (มี `reason`), `branches.activated`. ค่าใน `changes` เป็นรายชื่อฟิลด์ที่เปลี่ยน ไม่ใส่ค่าที่อยู่/โทรศัพท์.
```

- [ ] **Step 4: permission catalog**

ใน `docs/03-contracts/permission-catalog.md` แทนแถว `branches.manage` ด้วยสองแถว:

```markdown
| Organization | `organizations.manage` | แก้ Organization profile (ชื่อ th/en, เลขผู้เสียภาษี, ที่อยู่ออกเอกสาร); ผู้ถือระดับ Organization ได้สิทธิ์ข้ามสาขาใน `RequestAccessResolver` | Organization |
| Organization | `branches.manage` | สร้าง/แก้/ปิดใช้/เปิดใช้สาขา และดูสาขาที่ปิดใช้ | Organization |
```

- [ ] **Step 5: error contract, README, CONTEXT**

ต่อท้ายตาราง error codes ใน `docs/03-contracts/error-contract.md` (ตามรูปแบบแถวเดิม `| \`CODE\` | status | คำอธิบาย |`) ครบ 8 รหัสจากสัญญา API (ภาษาไทย). ใน `docs/README.md` เพิ่มแถวแผนที่: `| API จัดการ Organization/Branch | [Organization Administration API Contract](03-contracts/organization-administration-api-contract.md) |`. ใน `CONTEXT.md` เพิ่มศัพท์ **Tax Branch Code (เลขสาขาภาษี)** = รหัส 5 หลักของสาขาตามใบกำกับภาษี และ **Open Document (เอกสารเปิด)** = เอกสารที่ยังไม่ถึงสถานะสิ้นสุด ตามตารางใน ADR 0019.

- [ ] **Step 6: ตรวจลิงก์**

Run: `grep -o '](\([^)#]*\)' docs/03-contracts/organization-administration-api-contract.md docs/adr/0019-organization-branch-administration.md | sed 's/.*](//' | sort -u | while read p; do [ -e "docs/03-contracts/$p" ] || [ -e "docs/adr/$p" ] || [ -e "docs/$p" ] || echo "MISSING $p"; done`
Expected: ไม่มีบรรทัด `MISSING` ยกเว้น `../05-engineering/organization-administration-verification.md` (สร้างใน Task 15 — ยอมรับชั่วคราว).

- [ ] **Step 7: Commit**

```bash
git add docs/adr/0019-organization-branch-administration.md docs/adr/README.md docs/03-contracts/organization-administration-api-contract.md docs/03-contracts/permission-catalog.md docs/03-contracts/error-contract.md docs/README.md CONTEXT.md
git commit -m "docs(org): add organization and branch administration ADR and contract

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: Domain — Organization profile และ Branch details (TDD)

**Files:**
- Create: `backend/src/TanErp.Domain/Organization/OrganizationValues.cs`
- Modify: `backend/src/TanErp.Domain/Organization/Organization.cs`, `backend/src/TanErp.Domain/Organization/Branch.cs`
- Create: `backend/tests/TanErp.UnitTests/Organization/OrganizationDomainTests.cs`

- [ ] **Step 1: เขียน failing test**

สร้าง `backend/tests/TanErp.UnitTests/Organization/OrganizationDomainTests.cs`:

```csharp
using TanErp.Domain.Organization;
using Xunit;

namespace TanErp.UnitTests.Organization;

public class OrganizationDomainTests
{
    private static readonly Guid OrgId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12");

    [Theory]
    [InlineData("0105536000003", true)]
    [InlineData("0105554000001", true)]
    [InlineData("0105536000004", false)] // wrong checksum
    [InlineData("010553600000", false)]  // 12 digits
    [InlineData("01055360000a3", false)]
    [InlineData("", false)]
    public void ThaiTaxIdentifier_ValidatesLengthDigitsAndChecksum(string value, bool expected) =>
        Assert.Equal(expected, ThaiTaxIdentifier.IsValid(value));

    [Theory]
    [InlineData("00000", true)]
    [InlineData("00012", true)]
    [InlineData("0001", false)]
    [InlineData("000123", false)]
    [InlineData("0001a", false)]
    public void TaxBranchCodeRule_RequiresExactlyFiveDigits(string value, bool expected) =>
        Assert.Equal(expected, TaxBranchCodeRule.IsValid(value));

    [Theory]
    [InlineData("B01", true)]
    [InlineData("hq_1-a", true)]
    [InlineData("", false)]
    [InlineData("สาขา", false)]
    [InlineData("a b", false)]
    public void BranchCode_AllowsAsciiLettersDigitsUnderscoreHyphen(string value, bool expected) =>
        Assert.Equal(expected, BranchCode.IsValid(value));

    [Fact]
    public void UpdateProfile_TrimsNormalizesBlankToNullAndBumpsRowVersion()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "เก่า");
        var before = org.RowVersion;

        org.UpdateProfile("  ใหม่  ", " ", "0105536000003", "  ที่อยู่ ", null, "  ");

        Assert.Equal("ใหม่", org.Name);
        Assert.Null(org.NameEn);
        Assert.Equal("0105536000003", org.TaxIdentifier);
        Assert.Equal("ที่อยู่", org.AddressTh);
        Assert.Null(org.Phone);
        Assert.NotEqual(before, org.RowVersion);
    }

    [Fact]
    public void UpdateProfile_InvalidTaxIdentifier_ThrowsWithCode()
    {
        var org = new TanErp.Domain.Organization.Organization(OrgId, "x");

        var ex = Assert.Throws<OrganizationDomainException>(() => org.UpdateProfile("x", null, "0105536000004", null, null, null));

        Assert.Equal("ORGANIZATION_TAX_ID_INVALID", ex.Code);
    }

    [Fact]
    public void BranchCreate_SetsDetailsAndRejectsInvalidTaxBranchCode()
    {
        var branch = Branch.Create(Guid.NewGuid(), OrgId, " B02 ", "สาขา 2", "Branch 2", "00001", "addr", null, "02-123", DateTimeOffset.UtcNow);

        Assert.Equal("B02", branch.Code);
        Assert.Equal("00001", branch.TaxBranchCode);
        Assert.True(branch.IsActive);

        var ex = Assert.Throws<OrganizationDomainException>(() =>
            Branch.Create(Guid.NewGuid(), OrgId, "B03", "x", null, "1", null, null, null, DateTimeOffset.UtcNow));
        Assert.Equal("BRANCH_TAX_CODE_INVALID", ex.Code);

        var codeEx = Assert.Throws<OrganizationDomainException>(() =>
            Branch.Create(Guid.NewGuid(), OrgId, "bad code", "x", null, null, null, null, null, DateTimeOffset.UtcNow));
        Assert.Equal("BRANCH_CODE_INVALID", codeEx.Code);
    }

    [Fact]
    public void BranchUpdateDeactivateActivate_EachBumpsRowVersionAndKeepsCode()
    {
        var branch = Branch.Create(Guid.NewGuid(), OrgId, "B04", "x", null, null, null, null, null, DateTimeOffset.UtcNow);
        var v0 = branch.RowVersion;

        branch.UpdateDetails("y", null, "00004", null, null, null);
        var v1 = branch.RowVersion;
        branch.Deactivate();
        var v2 = branch.RowVersion;
        branch.Activate();

        Assert.Equal("B04", branch.Code);
        Assert.Equal("y", branch.Name);
        Assert.True(branch.IsActive);
        Assert.Equal(4, new[] { v0, v1, v2, branch.RowVersion }.Distinct().Count());
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม (compile error)**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationDomainTests"`
Expected: build FAIL — `ThaiTaxIdentifier`, `TaxBranchCodeRule`, `BranchCode`, `OrganizationDomainException`, `Branch.Create`, `UpdateProfile` ไม่มี.

- [ ] **Step 3: สร้าง value types**

สร้าง `backend/src/TanErp.Domain/Organization/OrganizationValues.cs`:

```csharp
using System.Text.RegularExpressions;

namespace TanErp.Domain.Organization;

public sealed class OrganizationDomainException : Exception
{
    public OrganizationDomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

public static class OrganizationLimits
{
    public const int Name = 255;
    public const int Address = 500;
    public const int Phone = 30;
    public const int BranchCode = 50;
}

public static class ThaiTaxIdentifier
{
    /// <summary>13 digits where the last digit is the mod-11 check digit of the first 12 (Revenue Department rule).</summary>
    public static bool IsValid(string? value)
    {
        if (value is null || value.Length != 13 || !value.All(char.IsAsciiDigit)) return false;
        var sum = 0;
        for (var i = 0; i < 12; i++) sum += (value[i] - '0') * (13 - i);
        return (11 - sum % 11) % 10 == value[12] - '0';
    }
}

public static class TaxBranchCodeRule
{
    public static bool IsValid(string? value) => value is { Length: 5 } && value.All(char.IsAsciiDigit);
}

public static partial class BranchCode
{
    [GeneratedRegex("^[A-Za-z0-9_-]{1,50}$")]
    private static partial Regex Pattern();

    public static bool IsValid(string? value) => value is not null && Pattern().IsMatch(value);
}

internal static class ProfileText
{
    public static string? Optional(string? value, int max, string field)
    {
        var trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (trimmed is not null && trimmed.Length > max)
            throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", $"{field} cannot exceed {max} characters.");
        return trimmed;
    }
}
```

- [ ] **Step 4: แก้ `Organization.cs`**

เพิ่ม property และเมธอด (คง constructor/Activate/Deactivate เดิม; `Activate/Deactivate` เพิ่ม `RowVersion = Guid.NewGuid();`):

```csharp
public string? NameEn { get; private set; }
public string? TaxIdentifier { get; private set; }
public string? AddressTh { get; private set; }
public string? AddressEn { get; private set; }
public string? Phone { get; private set; }
public Guid RowVersion { get; private set; } = Guid.NewGuid();

public void UpdateProfile(string name, string? nameEn, string? taxIdentifier, string? addressTh, string? addressEn, string? phone)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", "Organization name cannot be empty.");
    var tax = ProfileText.Optional(taxIdentifier, 13, "Tax identifier");
    if (tax is not null && !ThaiTaxIdentifier.IsValid(tax))
        throw new OrganizationDomainException("ORGANIZATION_TAX_ID_INVALID", "Tax identifier must be 13 digits with a valid check digit.");

    Name = ProfileText.Optional(name, OrganizationLimits.Name, "Name")!;
    NameEn = ProfileText.Optional(nameEn, OrganizationLimits.Name, "English name");
    TaxIdentifier = tax;
    AddressTh = ProfileText.Optional(addressTh, OrganizationLimits.Address, "Thai address");
    AddressEn = ProfileText.Optional(addressEn, OrganizationLimits.Address, "English address");
    Phone = ProfileText.Optional(phone, OrganizationLimits.Phone, "Phone");
    RowVersion = Guid.NewGuid();
}
```

- [ ] **Step 5: แก้ `Branch.cs`**

เพิ่ม property + factory + `UpdateDetails`; เปลี่ยน `Deactivate/Activate` ให้ bump version:

```csharp
public string? NameEn { get; private set; }
public string? TaxBranchCode { get; private set; }
public string? AddressTh { get; private set; }
public string? AddressEn { get; private set; }
public string? Phone { get; private set; }
public Guid RowVersion { get; private set; } = Guid.NewGuid();

public static Branch Create(
    Guid id, Guid organizationId, string code, string name, string? nameEn, string? taxBranchCode,
    string? addressTh, string? addressEn, string? phone, DateTimeOffset createdAtUtc)
{
    var trimmedCode = code?.Trim();
    if (!BranchCode.IsValid(trimmedCode))
        throw new OrganizationDomainException("BRANCH_CODE_INVALID", "Branch code must be 1-50 characters of letters, digits, underscore or hyphen.");

    var branch = new Branch(id, organizationId, trimmedCode!, name, isActive: true, createdAtUtc);
    branch.UpdateDetails(name, nameEn, taxBranchCode, addressTh, addressEn, phone);
    return branch;
}

public void UpdateDetails(string name, string? nameEn, string? taxBranchCode, string? addressTh, string? addressEn, string? phone)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", "Branch name cannot be empty.");
    var taxCode = ProfileText.Optional(taxBranchCode, 5, "Tax branch code");
    if (taxCode is not null && !TaxBranchCodeRule.IsValid(taxCode))
        throw new OrganizationDomainException("BRANCH_TAX_CODE_INVALID", "Tax branch code must be exactly 5 digits.");

    Name = ProfileText.Optional(name, OrganizationLimits.Name, "Name")!;
    NameEn = ProfileText.Optional(nameEn, OrganizationLimits.Name, "English name");
    TaxBranchCode = taxCode;
    AddressTh = ProfileText.Optional(addressTh, OrganizationLimits.Address, "Thai address");
    AddressEn = ProfileText.Optional(addressEn, OrganizationLimits.Address, "English address");
    Phone = ProfileText.Optional(phone, OrganizationLimits.Phone, "Phone");
    RowVersion = Guid.NewGuid();
}

public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }
```

หมายเหตุ: value rule ชื่อ `TaxBranchCodeRule` (ไม่ใช่ `TaxBranchCode`) เพราะ property `Branch.TaxBranchCode` จะบังชื่อ static class ภายใน `Branch`.

- [ ] **Step 6: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationDomainTests"`
Expected: PASS ทุกเคส. จากนั้น `dotnet build backend/TanErp.slnx` ต้อง 0 error (ตัวสร้าง `Organization`/`Branch` เดิมที่ seeder ใช้ยังคอมไพล์).

- [ ] **Step 7: Commit**

```bash
git add backend/src/TanErp.Domain/Organization backend/tests/TanErp.UnitTests/Organization/OrganizationDomainTests.cs
git commit -m "feat(org): add organization profile and branch detail domain rules

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: EF configuration, migration และ seed permission keys

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/OrganizationConfiguration.cs`, `BranchConfiguration.cs`
- Generate: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddOrganizationProfileAndBranchDetails*.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs` (สร้างใน Task 5; ขั้นนี้ตรวจด้วย `has-pending-model-changes` และ unit test parity)
- Create: `backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs`

- [ ] **Step 1: เขียน failing test (parity docs ↔ seeder)**

สร้าง `backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs` — ล็อกไม่ให้ key ที่ G-03a ใช้ขาดจาก docs และ FE:

```csharp
using Xunit;

namespace TanErp.UnitTests.Organization;

public class PermissionCatalogParityTests
{
    private static string RepoFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) dir = dir.Parent;
        return Path.Combine(new[] { dir!.FullName }.Concat(parts).ToArray());
    }

    [Theory]
    [InlineData("organizations.read")]
    [InlineData("organizations.manage")]
    [InlineData("branches.manage")]
    public void GovernanceKey_IsInDocsSeederAndFrontend(string key)
    {
        Assert.Contains($"`{key}`", File.ReadAllText(RepoFile("docs", "03-contracts", "permission-catalog.md")));
        Assert.Contains($"(\"{key}\"", File.ReadAllText(RepoFile("backend", "src", "TanErp.Infrastructure", "Persistence", "TestOnlyDataSeeder.cs")));
        Assert.Contains($"\"{key}\"", File.ReadAllText(RepoFile("frontend", "src", "lib", "permissions", "permissions.ts")));
    }
}
```

- [ ] **Step 2: Run — ต้องล้มเฉพาะ `organizations.manage` (FE) และ `branches.manage`/`organizations.manage` (seeder)**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~PermissionCatalogParityTests"`
Expected: FAIL 2 เคส (`organizations.manage`, `branches.manage`); `organizations.read` PASS.

- [ ] **Step 3: seed permission keys + FE constant**

ใน `TestOnlyDataSeeder.cs` ภายใน `permKeys` (ใกล้ `("organizations.read", "Read Organization")` ที่ :117) เพิ่ม:

```csharp
            ("organizations.manage", "Manage Organization Profile"),
            ("branches.manage", "Manage Branches"),
```

ใน `frontend/src/lib/permissions/permissions.ts` เพิ่มใต้ `ORGANIZATIONS_READ`: `ORGANIZATIONS_MANAGE: "organizations.manage",`.

- [ ] **Step 4: EF configuration**

ใน `OrganizationConfiguration.cs` เพิ่มหลัง `CreatedAtUtc`:

```csharp
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(255);
        builder.Property(x => x.TaxIdentifier).HasColumnName("tax_identifier").HasMaxLength(13);
        builder.Property(x => x.AddressTh).HasColumnName("address_th").HasMaxLength(500);
        builder.Property(x => x.AddressEn).HasColumnName("address_en").HasMaxLength(500);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
```

ใน `BranchConfiguration.cs` เพิ่มหลัง `CreatedAtUtc` และก่อน `HasOne`:

```csharp
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(255);
        builder.Property(x => x.TaxBranchCode).HasColumnName("tax_branch_code").HasMaxLength(5).IsFixedLength();
        builder.Property(x => x.AddressTh).HasColumnName("address_th").HasMaxLength(500);
        builder.Property(x => x.AddressEn).HasColumnName("address_en").HasMaxLength(500);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.TaxBranchCode })
            .IsUnique()
            .HasFilter("tax_branch_code IS NOT NULL")
            .HasDatabaseName("ix_branches_organization_id_tax_branch_code");
```

- [ ] **Step 5: Generate migration (ครั้งเดียว — ถ้าต้องแก้ ให้ลบแล้ว generate ใหม่ ห้าม stack migration แก้)**

Run:
```bash
cd /Users/syaco/Documents/development/tan-erp && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddOrganizationProfileAndBranchDetails \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```
Expected: สร้างไฟล์ `*_AddOrganizationProfileAndBranchDetails.cs` + `.Designer.cs` และ `AppDbContextModelSnapshot.cs` เปลี่ยน. เปิดไฟล์ migration ตรวจว่า `row_version` ของทั้งสองตารางมี `defaultValue: new Guid("00000000-0000-0000-0000-000000000000")` แล้วเพิ่มท้าย `Up` (ก่อนปิดเมธอด) เพื่อให้แถวเดิมแต่ละแถวมีเวอร์ชันไม่ซ้ำกัน:

```csharp
            migrationBuilder.Sql("UPDATE organization.organizations SET row_version = gen_random_uuid();");
            migrationBuilder.Sql("UPDATE organization.branches SET row_version = gen_random_uuid();");
```

- [ ] **Step 6: ตรวจ model drift และ parity**

Run:
```bash
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~PermissionCatalogParityTests|FullyQualifiedName~OrganizationDomainTests"
```
Expected: `No changes have been made to the model since the last migration.` และ test PASS ทั้งหมด.

- [ ] **Step 7: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Configurations/OrganizationConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/Configurations/BranchConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/Migrations backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs frontend/src/lib/permissions/permissions.ts backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs
git commit -m "feat(org): persist organization profile and branch details

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 4: Extract `SerializableTransactionRunner` (Global Reuse)

> **Global Reuse proposal (ต้องยืนยันกับผู้ใช้ก่อนเริ่ม Task นี้ตาม AGENTS.md ข้อ 2):** `IdentityAdministrationStore.RunAsync` (`:487`) เป็น private และ G-03a/b/c ต้องการพฤติกรรมเดียวกัน (Serializable + แปลง concurrency/serialization failure เป็น 409). แทนที่จะคัดลอก 40 บรรทัดสามครั้ง ให้ extract เป็น helper กลางใน Infrastructure. ถ้าผู้ใช้ไม่เห็นด้วย ให้ข้าม Task นี้และให้ `OrganizationAdministrationStore` มี `RunAsync` private ของตัวเอง (ข้อเสีย: โค้ดซ้ำ).

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/SerializableTransactionRunner.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs`
- Test (regression): `backend/tests/TanErp.IntegrationTests/Api/IdentityAdministrationEndpointsTests.cs` (ไม่แก้)

- [ ] **Step 1: ยืนยัน baseline เขียว**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests"`
Expected: PASS ทั้งคลาส (ต้องมี Docker). ถ้าไม่ผ่านตั้งแต่ก่อนแก้ ให้หยุดและรายงาน.

- [ ] **Step 2: สร้าง helper**

สร้าง `backend/src/TanErp.Infrastructure/Persistence/SerializableTransactionRunner.cs` (ย้ายโค้ดจาก `RunAsync` เดิมแบบตรงตัว เปลี่ยนเฉพาะ `_db` เป็นพารามิเตอร์และรหัส conflict เป็นพารามิเตอร์):

```csharp
using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Results;

namespace TanErp.Infrastructure.Persistence;

/// <summary>Runs a unit of work in a Serializable transaction and maps concurrency/serialization failures to a conflict error.</summary>
public static class SerializableTransactionRunner
{
    public static async Task<Result<T>> RunAsync<T>(
        AppDbContext db, Func<Task<Result<T>>> work, string conflictCode, CancellationToken cancellationToken)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var result = await work();
                if (result.IsSuccess) await transaction.CommitAsync(cancellationToken);
                else await transaction.RollbackAsync(cancellationToken);
                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<T>.Failure(new Error(conflictCode, "The record was modified by another user."));
            }
            catch (Exception ex) when (IsSerializationFailure(ex))
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<T>.Failure(new Error(conflictCode, "The change conflicted with a concurrent update; reload and retry."));
            }
        });
    }

    public static bool IsSerializationFailure(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException!)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected })
                return true;
            if (current.InnerException is null) break;
        }

        return false;
    }

    /// <summary>True only for a unique violation of the named constraint/index, never for any 23505.</summary>
    public static bool IsUniqueViolation(DbUpdateException ex, string constraintName) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg && pg.ConstraintName == constraintName;
}
```

- [ ] **Step 3: ให้ store เดิมเรียก helper**

ใน `IdentityAdministrationStore.cs` แทนตัวเมธอด `RunAsync<T>` (:487-519) ด้วย:

```csharp
    private Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> work, CancellationToken cancellationToken) =>
        SerializableTransactionRunner.RunAsync(_db, work, "ADMIN_VERSION_CONFLICT", cancellationToken);
```

แล้วลบเมธอด `IsSerializationFailure` (:521-532) และตรวจว่าไม่มีที่อื่นเรียก: `grep -n "IsSerializationFailure" backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs` ต้องไม่พบ. ลบ `using System.Data;` ถ้าไม่มีที่อื่นใช้ (build เตือน unused ไม่ใช่ error — ปล่อยได้).

- [ ] **Step 4: Run regression — ต้องผ่านเหมือนเดิม**

Run: `dotnet build backend/TanErp.slnx && dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests"`
Expected: build 0 error; PASS ทั้งคลาส (พฤติกรรมไม่เปลี่ยน).

- [ ] **Step 5: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/SerializableTransactionRunner.cs backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs
git commit -m "refactor(identity): extract serializable transaction runner

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: Application — profile models, port และ handler (permission ก่อน existence)

Task นี้วางโครง handler/port ของ **Organization profile** เท่านั้น; ส่วน Branch เพิ่มเป็นชิ้น ๆ ใน Task 7–9 (แต่ละชิ้นเพิ่ม member เข้า port/handler/fake store ของ test เดิม).

**Files:**
- Create: `backend/src/TanErp.Application/Organization/Administration/OrganizationAdministrationModels.cs`
- Create: `backend/src/TanErp.Application/Organization/Administration/IOrganizationAdministrationStore.cs`
- Create: `backend/src/TanErp.Application/Organization/Administration/OrganizationAdministrationHandler.cs`
- Create: `backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs`

- [ ] **Step 1: เขียน failing test**

สร้าง `backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs`:

```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;
using Xunit;

namespace TanErp.UnitTests.Organization;

public class OrganizationAdministrationHandlerTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly AdminCaller Caller = new("uid", Guid.NewGuid());
    private static readonly OrganizationProfileInput ProfileInput = new("n", null, null, null, null, null);

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public string? RequestedKey { get; private set; }
        public bool Allow { get; init; } = true;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            RequestedKey = permissionKey;
            return Task.FromResult(Allow
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(Guid.NewGuid(), membershipId, OrgId, null, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class RecordingStore : IOrganizationAdministrationStore
    {
        public int Calls { get; private set; }

        public Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(Result<OrganizationProfile>.Failure(new Error("RESOURCE_NOT_FOUND", "x")));
        }

        public Task<Result<OrganizationProfile>> UpdateProfileAsync(Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
            GetProfileAsync(organizationId, ct);
    }

    [Fact]
    public async Task WithoutPermission_NeverTouchesTheStore_SoNothingLeaksAboutExistence()
    {
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(new FakeAccess { Allow = false }, store);

        var read = await handler.GetProfileAsync(Caller, default);
        var write = await handler.UpdateProfileAsync(Caller, Guid.NewGuid(), ProfileInput, "t", default);

        Assert.Equal("PERMISSION_DENIED", read.Error.Code);
        Assert.Equal("PERMISSION_DENIED", write.Error.Code);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task EachOperationAsksForItsOwnPermissionKey()
    {
        var access = new FakeAccess();
        var handler = new OrganizationAdministrationHandler(access, new RecordingStore());

        await handler.GetProfileAsync(Caller, default);
        Assert.Equal("organizations.read", access.RequestedKey);

        await handler.UpdateProfileAsync(Caller, Guid.NewGuid(), ProfileInput, "t", default);
        Assert.Equal("organizations.manage", access.RequestedKey);
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม (compile error)**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"`
Expected: build FAIL — namespace `TanErp.Application.Organization.Administration` ไม่มี.

- [ ] **Step 3: สร้าง models, port, handler**

`OrganizationAdministrationModels.cs`:

```csharp
namespace TanErp.Application.Organization.Administration;

public static class OrganizationAdminPermissions
{
    public const string OrganizationsRead = "organizations.read";
    public const string OrganizationsManage = "organizations.manage";
    public const string BranchesManage = "branches.manage";
}

public sealed record OrganizationProfile(
    Guid Id, string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone, Guid RowVersion);

public sealed record OrganizationProfileInput(
    string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone);
```

`IOrganizationAdministrationStore.cs`:

```csharp
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.Administration;

public interface IOrganizationAdministrationStore
{
    Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct);
    Task<Result<OrganizationProfile>> UpdateProfileAsync(
        Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
}
```

`OrganizationAdministrationHandler.cs`:

```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.Administration;

/// <summary>Every operation resolves its permission first; the store is never reached (and never queried) without it.</summary>
public sealed class OrganizationAdministrationHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IOrganizationAdministrationStore _store;

    public OrganizationAdministrationHandler(IRequestAccessResolver access, IOrganizationAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(AdminCaller caller, string permission, CancellationToken ct) =>
        _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static AdminActor Actor(RequestAccessContext access) => new(access.ActorUserId, access.MembershipId);

    private static Result<T> Denied<T>(Result<RequestAccessContext> access) => Result<T>.Failure(access.Error);

    public async Task<Result<OrganizationProfile>> GetProfileAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.OrganizationsRead, ct);
        return access.IsFailure ? Denied<OrganizationProfile>(access) : await _store.GetProfileAsync(access.Value!.OrganizationId, ct);
    }

    public async Task<Result<OrganizationProfile>> UpdateProfileAsync(
        AdminCaller caller, Guid ifMatch, OrganizationProfileInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.OrganizationsManage, ct);
        return access.IsFailure
            ? Denied<OrganizationProfile>(access)
            : await _store.UpdateProfileAsync(access.Value!.OrganizationId, input, ifMatch, Actor(access.Value), traceId, ct);
    }
}
```

- [ ] **Step 4: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"`
Expected: PASS 2 เคส.

- [ ] **Step 5: Commit**

```bash
git add backend/src/TanErp.Application/Organization backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs
git commit -m "feat(org): add organization profile handler and store port

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: API — Organization profile (store, controller, error codes, integration harness)

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `backend/src/TanErp.Api/Resources/Errors.resx`, `backend/src/TanErp.Api/Resources/Errors.en.resx`, `backend/src/TanErp.Api/Program.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test (พร้อม harness)**

สร้าง `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Organization;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class OrganizationAdministrationEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "admin-token";
    private const string ViewerToken = "viewer-token";
    private const string OrgBToken = "org-b-token";
    private const string ViewerUid = "org-viewer-uid";

    private static readonly Guid ViewerUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6a01");
    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6a02");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                ViewerToken => ViewerUid,
                OrgBToken => TestOnlyDataSeeder.TestFirebaseUidB,
                _ => null
            });

        public async Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = await VerifyTokenAsync(idToken, cancellationToken);
            return uid is null ? null : new FirebaseIdentity(uid, null, false);
        }
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                ["SeedTestData"] = "true"
            }));
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);
        await SeedViewerAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>A member holding only organizations.read, to prove read and write permissions are separate.</summary>
    private static async Task SeedViewerAsync(AppDbContext db)
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var read = await db.Permissions.SingleAsync(p => p.Key == "organizations.read");
        var role = new Role(Guid.NewGuid(), orgId, "Org Viewer", "read only", isActive: true);
        db.Users.Add(new User(ViewerUserId, ViewerUid, "Viewer", "org-viewer@example.test", isActive: true));
        db.Memberships.Add(new Membership(ViewerMembershipId, orgId, TestOnlyDataSeeder.TestBranchId, ViewerUserId, isActive: true));
        db.Roles.Add(role);
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, orgId, read.Id, PermissionScope.Organization, orgId));
        db.MembershipRoles.Add(new MembershipRole(ViewerMembershipId, role.Id, orgId));
        await db.SaveChangesAsync();
    }

    private AppDbContext NewDb() => _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, Guid? membershipId = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? TestOnlyDataSeeder.TestMembershipId).ToString());
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return _client.SendAsync(request);
    }

    private static async Task<string?> ReadCode(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private async Task<OrganizationProfileResponse> GetProfileAsync()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", AdminToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
    }

    [Fact]
    public async Task GetProfile_ReturnsOwnOrganizationWithEtag()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal(TestOnlyDataSeeder.TestOrgId, profile.Id);
        Assert.Equal($"\"{profile.RowVersion}\"", response.Headers.ETag?.Tag);
    }

    [Fact]
    public async Task GetProfile_OrganizationBAdmin_SeesOnlyOrganizationB()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);

        var profile = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal(TestOnlyDataSeeder.TestOrgBId, profile.Id);
    }

    [Fact]
    public async Task UpdateProfile_PersistsTrimmedValuesAndWritesAuditWithoutValues()
    {
        var before = await GetProfileAsync();

        var response = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken,
            new UpdateOrganizationProfileRequest("  บริษัท ทดสอบ  ", "Test Co", "0105536000003", "1 ถนนทดสอบ", null, "02-000-0000"),
            ifMatch: before.RowVersion);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var after = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal("บริษัท ทดสอบ", after.Name);
        Assert.Equal("0105536000003", after.TaxIdentifier);
        Assert.NotEqual(before.RowVersion, after.RowVersion);
        Assert.Equal($"\"{after.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "organization.profile-updated");
        Assert.Contains("taxIdentifier", audit.ChangesJson);
        Assert.DoesNotContain("0105536000003", audit.ChangesJson);
        Assert.DoesNotContain("ถนนทดสอบ", audit.ChangesJson);
        Assert.Equal(before.RowVersion, audit.RowVersionBefore);
        Assert.Equal(after.RowVersion, audit.RowVersionAfter);
    }

    [Fact]
    public async Task UpdateProfile_StaleIfMatch_Returns409AndWithoutIfMatch_Returns428()
    {
        var before = await GetProfileAsync();
        var body = new UpdateOrganizationProfileRequest("x", null, null, null, null, null);

        var stale = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken, body, ifMatch: Guid.NewGuid());
        var missing = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken, body);

        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
        Assert.Equal(before.RowVersion, (await GetProfileAsync()).RowVersion);
    }

    [Fact]
    public async Task UpdateProfile_InvalidTaxIdentifier_Returns422WithoutChangingTheRow()
    {
        var before = await GetProfileAsync();

        var response = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken,
            new UpdateOrganizationProfileRequest("x", null, "0105536000004", null, null, null), ifMatch: before.RowVersion);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("ORGANIZATION_TAX_ID_INVALID", await ReadCode(response));
        Assert.Equal(before.Name, (await GetProfileAsync()).Name);
    }

    [Fact]
    public async Task ReadOnlyMember_CanReadButNotUpdate()
    {
        var read = await Send(HttpMethod.Get, "/api/v1/admin/organization", ViewerToken, membershipId: ViewerMembershipId);
        var write = await Send(HttpMethod.Put, "/api/v1/admin/organization", ViewerToken,
            new UpdateOrganizationProfileRequest("hacked", null, null, null, null, null), ifMatch: Guid.NewGuid(), membershipId: ViewerMembershipId);

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, write.StatusCode);
        Assert.Equal("PERMISSION_DENIED", await ReadCode(write));
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม (compile error)**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"`
Expected: build FAIL — `TanErp.Api.Contracts.Organization` / `OrganizationProfileResponse` ไม่มี.

- [ ] **Step 3: Contracts**

สร้าง `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs`:

```csharp
using TanErp.Application.Organization.Administration;

namespace TanErp.Api.Contracts.Organization;

public sealed record UpdateOrganizationProfileRequest(
    string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone);

public sealed record OrganizationProfileResponse(
    Guid Id, string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone, Guid RowVersion)
{
    public static OrganizationProfileResponse From(OrganizationProfile p) =>
        new(p.Id, p.Name, p.NameEn, p.TaxIdentifier, p.AddressTh, p.AddressEn, p.Phone, p.RowVersion);
}
```

- [ ] **Step 4: Store (profile)**

สร้าง `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`:

```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;
using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Organization;

public sealed class OrganizationAdministrationStore : IOrganizationAdministrationStore
{
    private const string VersionConflict = "ADMIN_VERSION_CONFLICT";

    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public OrganizationAdministrationStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void AddAudit(
        Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId, string traceId,
        object changes, Guid? versionBefore, Guid? versionAfter, Guid? branchId = null)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), organizationId, actor.UserId, action, resourceType, resourceId, _clock.UtcNow, traceId,
            JsonSerializer.Serialize(changes), branchId, actorMembershipId: actor.MembershipId,
            rowVersionBefore: versionBefore, rowVersionAfter: versionAfter));
    }

    private static OrganizationProfile ToProfile(TanErp.Domain.Organization.Organization o) =>
        new(o.Id, o.Name, o.NameEn, o.TaxIdentifier, o.AddressTh, o.AddressEn, o.Phone, o.RowVersion);

    public async Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct)
    {
        var org = await _db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        return org is null ? Fail<OrganizationProfile>("RESOURCE_NOT_FOUND", "Organization was not found.") : Result<OrganizationProfile>.Success(ToProfile(org));
    }

    public Task<Result<OrganizationProfile>> UpdateProfileAsync(
        Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId, ct);
            if (org is null) return Fail<OrganizationProfile>("RESOURCE_NOT_FOUND", "Organization was not found.");
            if (org.RowVersion != ifMatch) return Fail<OrganizationProfile>(VersionConflict, "The organization was modified by another user.");

            var before = org.RowVersion;
            var changed = ChangedFields(org, input);
            try
            {
                org.UpdateProfile(input.Name, input.NameEn, input.TaxIdentifier, input.AddressTh, input.AddressEn, input.Phone);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<OrganizationProfile>(ex.Code, ex.Message);
            }

            AddAudit(organizationId, actor, "organization.profile-updated", "Organization", org.Id.ToString(), traceId,
                new { changedFields = changed }, before, org.RowVersion);
            await _db.SaveChangesAsync(ct);
            return Result<OrganizationProfile>.Success(ToProfile(org));
        }, VersionConflict, ct);

    /// <summary>Field names only: addresses and tax ids must never be copied into the audit log.</summary>
    private static string[] ChangedFields(TanErp.Domain.Organization.Organization o, OrganizationProfileInput i)
    {
        static string? N(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        var changed = new List<string>();
        if (o.Name != N(i.Name)) changed.Add("name");
        if (o.NameEn != N(i.NameEn)) changed.Add("nameEn");
        if (o.TaxIdentifier != N(i.TaxIdentifier)) changed.Add("taxIdentifier");
        if (o.AddressTh != N(i.AddressTh)) changed.Add("addressTh");
        if (o.AddressEn != N(i.AddressEn)) changed.Add("addressEn");
        if (o.Phone != N(i.Phone)) changed.Add("phone");
        return changed.ToArray();
    }
}
```

หมายเหตุ: `Fail<T>` ถูกประกาศ private ในคลาสนี้; Task 7–9 ใช้ซ้ำ. `OrganizationAdministrationStore` อยู่ใน namespace `TanErp.Infrastructure.Persistence.Organization` ซึ่งทำให้ชื่อ `Organization` ถูกตีเป็น namespace — จึงอ้าง entity ด้วย `TanErp.Domain.Organization.Organization` เต็ม.

- [ ] **Step 5: Controller**

สร้าง `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Common;
using TanErp.Api.Contracts.Organization;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;

namespace TanErp.Api.Controllers;

/// <summary>Organization profile and branch administration (G-03a). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public sealed class AdminOrganizationController : ControllerBase
{
    private readonly OrganizationAdministrationHandler _handler;

    public AdminOrganizationController(OrganizationAdministrationHandler handler) => _handler = handler;

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    [HttpGet("organization")]
    [ProducesResponseType<OrganizationProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.GetProfileAsync(new AdminCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId), cancellationToken);
        return ProfileResult(result);
    }

    [HttpPut("organization")]
    [ProducesResponseType<OrganizationProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateOrganizationProfileRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        var result = await _handler.UpdateProfileAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), conditional.Value.IfMatchRowVersion,
            new OrganizationProfileInput(request.Name, request.NameEn, request.TaxIdentifier, request.AddressTh, request.AddressEn, request.Phone),
            HttpContext.TraceIdentifier, cancellationToken);
        return ProfileResult(result);
    }

    private IActionResult ProfileResult(Result<OrganizationProfile> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(OrganizationProfileResponse.From(result.Value));
    }
}
```

- [ ] **Step 6: Error codes + DI**

ใน `ProblemDetailsMapper.cs` (`GetStatus` switch, ใกล้ `ADMIN_VERSION_CONFLICT`) เพิ่ม:

```csharp
        "ORGANIZATION_TAX_ID_INVALID" => StatusCodes.Status422UnprocessableEntity,
```

ใน `Errors.resx` (ไทย) และ `Errors.en.resx` เพิ่ม `ORGANIZATION_TAX_ID_INVALID_TITLE` / `_DETAIL` รูปแบบเดียวกับ `ADMIN_VERSION_CONFLICT_*`:

| key | th | en |
| --- | --- | --- |
| `ORGANIZATION_TAX_ID_INVALID_TITLE` | เลขประจำตัวผู้เสียภาษีไม่ถูกต้อง | Invalid tax identifier |
| `ORGANIZATION_TAX_ID_INVALID_DETAIL` | เลขประจำตัวผู้เสียภาษีต้องมี 13 หลักและผ่านการตรวจเลขตรวจสอบ | The tax identifier must be 13 digits with a valid check digit. |

ใน `Program.cs` ก่อนเพิ่ม DI ให้ตรวจว่ายังไม่มี: `grep -n "OrganizationAdministration" backend/src/TanErp.Api/Program.cs` ต้องไม่พบ แล้วเพิ่มต่อจากบรรทัด DI ของ `IIdentityAdministrationStore` (:92):

```csharp
builder.Services.AddScoped<TanErp.Application.Organization.Administration.IOrganizationAdministrationStore, TanErp.Infrastructure.Persistence.Organization.OrganizationAdministrationStore>();
builder.Services.AddScoped<TanErp.Application.Organization.Administration.OrganizationAdministrationHandler>();
```

- [ ] **Step 7: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"`
Expected: PASS 6 เคส. ถ้า `GetProfile_OrganizationBAdmin` ล้มเพราะ Org B ไม่มี key ใหม่: ตรวจว่า Task 3 เพิ่มคีย์ใน `permKeys` (Org B ได้ทุก key ใน loop :330) ไม่ใช่ list แยก.

- [ ] **Step 8: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Organization backend/src/TanErp.Api/Contracts/Organization backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs backend/src/TanErp.Api/Resources backend/src/TanErp.Api/Program.cs backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs
git commit -m "feat(org): add organization profile endpoints

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: API — Branch list, get และ create (Idempotency-Key, unique constraint ตามชื่อ)

**Files:**
- Modify: `backend/src/TanErp.Application/Organization/Administration/{OrganizationAdministrationModels,IOrganizationAdministrationStore,OrganizationAdministrationHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `backend/src/TanErp.Api/Resources/Errors.resx`, `Errors.en.resx`
- Modify: `backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs`

- [ ] **Step 1: เขียน failing unit test (permission ก่อน existence, key ถูก hash)**

ใน `OrganizationAdministrationHandlerTests.cs` เพิ่มใน `RecordingStore`:

```csharp
        public string? LastKeyHash { get; private set; }

        private static BranchDetail Branch() => new(Guid.NewGuid(), "B1", "n", null, null, null, null, null, true, Guid.NewGuid(), DateTimeOffset.UtcNow);

        public Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<BranchDetail>>(Array.Empty<BranchDetail>());
        }

        public Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(Result<BranchDetail>.Success(Branch()));
        }

        public Task<Result<BranchDetail>> CreateBranchAsync(
            Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct)
        {
            Calls++;
            LastKeyHash = keyHash;
            return Task.FromResult(Result<BranchDetail>.Success(Branch()));
        }
```

และเพิ่ม test:

```csharp
    private static readonly BranchInput BranchInputValue = new("n", null, null, null, null, null);

    [Fact]
    public async Task BranchOperations_WithoutPermission_NeverTouchTheStore()
    {
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(new FakeAccess { Allow = false }, store);

        var list = await handler.ListBranchesAsync(Caller, BranchStatusFilter.All, default);
        var get = await handler.GetBranchAsync(Caller, Guid.NewGuid(), default);
        var create = await handler.CreateBranchAsync(Caller, "key-0123456789abcdef", new CreateBranchInput("B2", BranchInputValue), "t", default);

        Assert.True(list.IsFailure && get.IsFailure && create.IsFailure);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task CreateBranch_AsksForBranchesManageAndHashesTheKey()
    {
        var access = new FakeAccess();
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(access, store);

        await handler.CreateBranchAsync(Caller, "plain-key-0123456789", new CreateBranchInput("B2", BranchInputValue), "t", default);

        Assert.Equal("branches.manage", access.RequestedKey);
        Assert.NotNull(store.LastKeyHash);
        Assert.DoesNotContain("plain-key", store.LastKeyHash);
    }
```

- [ ] **Step 2: Run — ต้องล้ม (compile error: `BranchDetail`, `BranchStatusFilter`, ... ไม่มี)**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"`
Expected: build FAIL.

- [ ] **Step 3: Application — models, port, handler**

ต่อท้าย `OrganizationAdministrationModels.cs`:

```csharp
public sealed record BranchDetail(
    Guid Id, string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn,
    string? Phone, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc);

public sealed record BranchInput(
    string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record CreateBranchInput(string Code, BranchInput Details);

public enum BranchStatusFilter { All, Active, Inactive }
```

ต่อท้ายอินเทอร์เฟซ `IOrganizationAdministrationStore`:

```csharp
    Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct);
    Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct);
    Task<Result<BranchDetail>> CreateBranchAsync(
        Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct);
```

เพิ่มใน `OrganizationAdministrationHandler` (ต้อง `using TanErp.Application.Common.Security;`):

```csharp
    public async Task<Result<IReadOnlyList<BranchDetail>>> ListBranchesAsync(AdminCaller caller, BranchStatusFilter filter, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        if (access.IsFailure) return Result<IReadOnlyList<BranchDetail>>.Failure(access.Error);
        return Result<IReadOnlyList<BranchDetail>>.Success(await _store.ListBranchesAsync(access.Value!.OrganizationId, filter, ct));
    }

    public async Task<Result<BranchDetail>> GetBranchAsync(AdminCaller caller, Guid branchId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        return access.IsFailure ? Denied<BranchDetail>(access) : await _store.GetBranchAsync(access.Value!.OrganizationId, branchId, ct);
    }

    public async Task<Result<BranchDetail>> CreateBranchAsync(
        AdminCaller caller, string idempotencyKey, CreateBranchInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        if (access.IsFailure) return Denied<BranchDetail>(access);

        var d = input.Details;
        var payloadHash = Sha256Hex.Compute($"{input.Code}|{d.Name}|{d.NameEn}|{d.TaxBranchCode}|{d.AddressTh}|{d.AddressEn}|{d.Phone}");
        return await _store.CreateBranchAsync(
            access.Value!.OrganizationId, input, Actor(access.Value), Sha256Hex.Compute(idempotencyKey), payloadHash, traceId, ct);
    }
```

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"` Expected: PASS 4 เคส.

- [ ] **Step 4: เขียน failing integration test**

ต่อท้ายคลาส `OrganizationAdministrationEndpointsTests` (ใช้ helper `Send`, `ReadCode`, `NewDb` เดิม):

```csharp
    private static CreateBranchRequest NewBranch(string code, string? taxCode = null) =>
        new(code, "สาขา " + code, "Branch " + code, taxCode, "ที่อยู่", null, "02-111-1111");

    private Task<HttpResponseMessage> CreateBranch(CreateBranchRequest body, string? key = null, string token = AdminToken, Guid? membershipId = null) =>
        Send(HttpMethod.Post, "/api/v1/admin/branches", token, body, idempotencyKey: key ?? Guid.NewGuid().ToString(), membershipId: membershipId);

    [Fact]
    public async Task CreateBranch_PersistsDetailsWritesAuditAndReturnsEtag()
    {
        var response = await CreateBranch(NewBranch("B10", "00010"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var branch = (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
        Assert.Equal("B10", branch.Code);
        Assert.Equal("00010", branch.TaxBranchCode);
        Assert.True(branch.IsActive);
        Assert.Equal($"\"{branch.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "branches.created" && a.ResourceId == branch.Id.ToString());
        Assert.Equal(branch.Id, audit.BranchId);
        Assert.DoesNotContain("ที่อยู่", audit.ChangesJson);
    }

    [Fact]
    public async Task CreateBranch_DuplicateCodeAndDuplicateTaxCode_ReturnDifferentConflictCodes()
    {
        Assert.Equal(HttpStatusCode.Created, (await CreateBranch(NewBranch("B11", "00011"))).StatusCode);

        var dupCode = await CreateBranch(NewBranch("B11", "00099"));
        var dupTax = await CreateBranch(NewBranch("B12", "00011"));

        Assert.Equal(HttpStatusCode.Conflict, dupCode.StatusCode);
        Assert.Equal("BRANCH_CODE_ALREADY_EXISTS", await ReadCode(dupCode));
        Assert.Equal(HttpStatusCode.Conflict, dupTax.StatusCode);
        Assert.Equal("BRANCH_TAX_CODE_ALREADY_EXISTS", await ReadCode(dupTax));
    }

    [Fact]
    public async Task CreateBranch_SameCodeInAnotherOrganization_IsAllowed()
    {
        // Seeded organization A already owns "B01"; organization B may create its own "B01".
        var response = await CreateBranch(NewBranch("B01"), token: OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateBranch_InvalidCodeOrTaxCode_Returns422()
    {
        var badCode = await CreateBranch(NewBranch("bad code"));
        var badTax = await CreateBranch(NewBranch("B13", "12"));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, badCode.StatusCode);
        Assert.Equal("BRANCH_CODE_INVALID", await ReadCode(badCode));
        Assert.Equal("BRANCH_TAX_CODE_INVALID", await ReadCode(badTax));
    }

    [Fact]
    public async Task CreateBranch_ReplayAndKeyReuseAndMissingKey()
    {
        var key = "branch-create-" + Guid.NewGuid();
        var first = await CreateBranch(NewBranch("B14"), key);
        var second = await CreateBranch(NewBranch("B14"), key);
        var reused = await CreateBranch(NewBranch("B15"), key);
        var noKey = await Send(HttpMethod.Post, "/api/v1/admin/branches", AdminToken, NewBranch("B16"));

        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.Equal((await first.Content.ReadFromJsonAsync<BranchResponse>())!.Id, (await second.Content.ReadFromJsonAsync<BranchResponse>())!.Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ReadCode(reused));
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", await ReadCode(noKey));
        await using var db = NewDb();
        Assert.Equal(1, await db.Branches.CountAsync(b => b.Code == "B14"));
    }

    [Fact]
    public async Task ListBranches_IncludesInactiveFiltersByStatusAndStaysInsideOrganization()
    {
        var created = (await (await CreateBranch(NewBranch("B17"))).Content.ReadFromJsonAsync<BranchResponse>())!;
        await using (var db = NewDb())
        {
            var branch = await db.Branches.SingleAsync(b => b.Id == created.Id);
            branch.Deactivate();
            await db.SaveChangesAsync();
        }

        var all = await ListAsync("", AdminToken, TestOnlyDataSeeder.TestMembershipId);
        var inactive = await ListAsync("?status=inactive", AdminToken, TestOnlyDataSeeder.TestMembershipId);
        var orgB = await ListAsync("", OrgBToken, TestOnlyDataSeeder.TestMembershipBId);
        var invalid = await Send(HttpMethod.Get, "/api/v1/admin/branches?status=bogus", AdminToken);

        Assert.Contains(all, b => b.Id == created.Id && !b.IsActive);
        Assert.Equal(new[] { created.Id }, inactive.Select(b => b.Id).ToArray());
        Assert.DoesNotContain(orgB, b => b.Id == created.Id);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    private async Task<IReadOnlyList<BranchResponse>> ListAsync(string query, string token, Guid membershipId)
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/branches" + query, token, membershipId: membershipId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<IReadOnlyList<BranchResponse>>())!;
    }

    [Fact]
    public async Task GetBranch_OtherOrganizationId_Returns404ButWithoutPermissionReturns403ForAnyId()
    {
        var orgBBranch = TestOnlyDataSeeder.TestBranchBId;
        var ownBranch = TestOnlyDataSeeder.TestBranchId;

        var otherOrg = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{orgBBranch}", AdminToken);
        var viewerExisting = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{ownBranch}", ViewerToken, membershipId: ViewerMembershipId);
        var viewerMissing = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{Guid.NewGuid()}", ViewerToken, membershipId: ViewerMembershipId);

        Assert.Equal(HttpStatusCode.NotFound, otherOrg.StatusCode);
        Assert.Equal("RESOURCE_NOT_FOUND", await ReadCode(otherOrg));
        Assert.Equal(HttpStatusCode.Forbidden, viewerExisting.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, viewerMissing.StatusCode); // identical: existence is never revealed
    }
```

- [ ] **Step 5: Run — ต้องล้ม**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"`
Expected: build FAIL — `CreateBranchRequest`, `BranchResponse` ไม่มี.

- [ ] **Step 6: Contracts**

ต่อท้าย `OrganizationAdministrationContracts.cs`:

```csharp
public sealed record CreateBranchRequest(
    string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record BranchResponse(
    Guid Id, string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn,
    string? Phone, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc)
{
    public static BranchResponse From(BranchDetail b) => new(
        b.Id, b.Code, b.Name, b.NameEn, b.TaxBranchCode, b.AddressTh, b.AddressEn, b.Phone, b.IsActive, b.RowVersion, b.CreatedAtUtc);
}
```

- [ ] **Step 7: Store — list, get, create**

เพิ่มใน `OrganizationAdministrationStore` (ต้อง `using TanErp.Domain.Common;` มีแล้ว; เพิ่มค่าคงที่และเมธอด):

```csharp
    private const string CreateOperation = "admin.branches.create";
    private const string CodeIndex = "ix_branches_organization_id_branch_code";
    private const string TaxCodeIndex = "ix_branches_organization_id_tax_branch_code";

    private static BranchDetail ToDetail(Branch b) => new(
        b.Id, b.Code, b.Name, b.NameEn, b.TaxBranchCode, b.AddressTh, b.AddressEn, b.Phone, b.IsActive, b.RowVersion, b.CreatedAtUtc);

    public async Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct)
    {
        var query = _db.Branches.AsNoTracking().Where(b => b.OrganizationId == organizationId);
        if (filter == BranchStatusFilter.Active) query = query.Where(b => b.IsActive);
        if (filter == BranchStatusFilter.Inactive) query = query.Where(b => !b.IsActive);

        var rows = await query.OrderBy(b => b.Code).ToListAsync(ct);
        return rows.Select(ToDetail).ToList();
    }

    public async Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        // Always filtered by organization: an id from another organization is indistinguishable from a missing one.
        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
        return branch is null ? Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.") : Result<BranchDetail>.Success(ToDetail(branch));
    }

    public Task<Result<BranchDetail>> CreateBranchAsync(
        Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            // The advisory lock is taken INSIDE the transaction so concurrent requests with the same key serialize and the loser replays.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({organizationId.ToString("N") + ":" + CreateOperation + ":" + keyHash}, 0))", ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == organizationId && r.Operation == CreateOperation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                    return Fail<BranchDetail>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                return await GetBranchAsync(organizationId, Guid.Parse(replay.ResourceId), ct);
            }

            Branch branch;
            try
            {
                var d = input.Details;
                branch = Branch.Create(Guid.NewGuid(), organizationId, input.Code, d.Name, d.NameEn, d.TaxBranchCode, d.AddressTh, d.AddressEn, d.Phone, _clock.UtcNow);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<BranchDetail>(ex.Code, ex.Message);
            }

            _db.Branches.Add(branch);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, CreateOperation, keyHash, payloadHash, branch.Id.ToString(), _clock.UtcNow));
            AddAudit(organizationId, actor, "branches.created", "Branch", branch.Id.ToString(), traceId,
                new { code = branch.Code, hasTaxBranchCode = branch.TaxBranchCode is not null }, null, branch.RowVersion, branch.Id);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, CodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_CODE_ALREADY_EXISTS", "A branch with this code already exists.");
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, TaxCodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_TAX_CODE_ALREADY_EXISTS", "A branch with this tax branch code already exists.");
            }

            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);
```

- [ ] **Step 8: Controller + error codes**

เพิ่มใน `AdminOrganizationController` (ต้อง `using TanErp.Application.Common.Results;` มีแล้ว):

```csharp
    private static AdminCaller Caller(AuthenticatedRequest a) => new(a.FirebaseUid, a.MembershipId);

    [HttpGet("branches")]
    [ProducesResponseType<IReadOnlyList<BranchResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListBranches([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        BranchStatusFilter filter;
        switch (status)
        {
            case null or "": filter = BranchStatusFilter.All; break;
            case "active": filter = BranchStatusFilter.Active; break;
            case "inactive": filter = BranchStatusFilter.Inactive; break;
            default: return Problem(new Error("REQUEST_VALIDATION_FAILED", "Status must be 'active' or 'inactive'."));
        }

        var result = await _handler.ListBranchesAsync(Caller(auth.Value!), filter, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(result.Value!.Select(BranchResponse.From).ToArray());
    }

    [HttpGet("branches/{branchId:guid}")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch([FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        return BranchResult(await _handler.GetBranchAsync(Caller(auth.Value!), branchId, cancellationToken));
    }

    [HttpPost("branches")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (idempotent.IsFailure) return Problem(idempotent.Error);

        var result = await _handler.CreateBranchAsync(
            new AdminCaller(idempotent.Value!.FirebaseUid, idempotent.Value.MembershipId), idempotent.Value.IdempotencyKey,
            new CreateBranchInput(request.Code, new BranchInput(request.Name, request.NameEn, request.TaxBranchCode, request.AddressTh, request.AddressEn, request.Phone)),
            HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Created($"/api/v1/admin/branches/{result.Value.Id}", BranchResponse.From(result.Value));
    }

    private IActionResult BranchResult(Result<BranchDetail> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(BranchResponse.From(result.Value));
    }
```

(`AuthenticatedRequest` คือ record ใน `TanErp.Api.RequestContext` — `RequestContextReader.cs:6`.)

ใน `ProblemDetailsMapper.GetStatus` เพิ่ม:

```csharp
        "BRANCH_CODE_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "BRANCH_TAX_CODE_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "BRANCH_CODE_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
        "BRANCH_TAX_CODE_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
```

และใน `Errors.resx`/`Errors.en.resx` เพิ่ม `_TITLE`/`_DETAIL` 4 ชุด:

| key | th | en |
| --- | --- | --- |
| `BRANCH_CODE_INVALID` | รหัสสาขาไม่ถูกต้อง / รหัสสาขาใช้ได้เฉพาะตัวอักษรอังกฤษ ตัวเลข ขีดล่างและขีดกลาง ไม่เกิน 50 ตัว | Invalid branch code / Use letters, digits, underscore or hyphen, up to 50 characters. |
| `BRANCH_TAX_CODE_INVALID` | เลขสาขาภาษีไม่ถูกต้อง / เลขสาขาภาษีต้องเป็นตัวเลข 5 หลัก | Invalid tax branch code / The tax branch code must be exactly 5 digits. |
| `BRANCH_CODE_ALREADY_EXISTS` | รหัสสาขาซ้ำ / มีสาขาที่ใช้รหัสนี้อยู่แล้วในองค์กร | Duplicate branch code / A branch with this code already exists in the organization. |
| `BRANCH_TAX_CODE_ALREADY_EXISTS` | เลขสาขาภาษีซ้ำ / มีสาขาที่ใช้เลขสาขาภาษีนี้อยู่แล้วในองค์กร | Duplicate tax branch code / A branch with this tax branch code already exists in the organization. |

(แต่ละแถว: ค่าก่อน `/` คือ TITLE, หลัง `/` คือ DETAIL.)

- [ ] **Step 9: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"`
Expected: PASS ทุกเคส (Task 6 + Task 7). ถ้า `CreateBranch_SameCodeInAnotherOrganization` ล้มด้วย `BRANCH_CODE_ALREADY_EXISTS` แปลว่า unique index ไม่รวม `organization_id` — ตรวจ `BranchConfiguration`.

- [ ] **Step 10: Commit**

```bash
git add backend/src/TanErp.Application/Organization backend/src/TanErp.Infrastructure/Persistence/Organization backend/src/TanErp.Api backend/tests/TanErp.UnitTests/Organization backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs
git commit -m "feat(org): add branch list, get and create endpoints

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 8: API — แก้ Branch (If-Match, Code แก้ไม่ได้)

**Files:**
- Modify: `backend/src/TanErp.Application/Organization/Administration/{IOrganizationAdministrationStore,OrganizationAdministrationHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs`
- Modify: `backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs`

- [ ] **Step 1: เขียน failing unit test**

ใน `RecordingStore` เพิ่ม:

```csharp
        public Task<Result<BranchDetail>> UpdateBranchAsync(
            Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
            GetBranchAsync(organizationId, branchId, ct);
```

และเพิ่ม test (ต่อจาก `BranchOperations_WithoutPermission_NeverTouchTheStore` — เพิ่มบรรทัด `var update = await handler.UpdateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), BranchInputValue, "t", default);` แล้วเพิ่ม `update.IsFailure` ใน `Assert.True(...)` และ `Assert.Equal(0, store.Calls)` ยังคงเดิม):

```csharp
    [Fact]
    public async Task UpdateBranch_AsksForBranchesManage()
    {
        var access = new FakeAccess();
        var handler = new OrganizationAdministrationHandler(access, new RecordingStore());

        await handler.UpdateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), BranchInputValue, "t", default);

        Assert.Equal("branches.manage", access.RequestedKey);
    }
```

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"` Expected: build FAIL (`UpdateBranchAsync` ไม่มีบน handler).

- [ ] **Step 2: Application — port + handler**

เพิ่มใน `IOrganizationAdministrationStore`:

```csharp
    Task<Result<BranchDetail>> UpdateBranchAsync(
        Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
```

เพิ่มใน handler:

```csharp
    public async Task<Result<BranchDetail>> UpdateBranchAsync(
        AdminCaller caller, Guid branchId, Guid ifMatch, BranchInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        return access.IsFailure
            ? Denied<BranchDetail>(access)
            : await _store.UpdateBranchAsync(access.Value!.OrganizationId, branchId, input, ifMatch, Actor(access.Value), traceId, ct);
    }
```

Run unit test อีกครั้ง — Expected: PASS.

- [ ] **Step 3: เขียน failing integration test**

ต่อท้ายคลาส `OrganizationAdministrationEndpointsTests`:

```csharp
    private async Task<BranchResponse> CreateAndReadAsync(string code, string? taxCode = null)
    {
        var response = await CreateBranch(NewBranch(code, taxCode));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
    }

    [Fact]
    public async Task UpdateBranch_ChangesDetailsKeepsCodeIgnoresCodeInBodyAndAudits()
    {
        var created = await CreateAndReadAsync("B20");

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken,
            new { code = "HACKED", name = "ชื่อใหม่", nameEn = "New", taxBranchCode = "00020", addressTh = "x", addressEn = (string?)null, phone = "1" },
            ifMatch: created.RowVersion);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
        Assert.Equal("B20", updated.Code);
        Assert.Equal("ชื่อใหม่", updated.Name);
        Assert.Equal("00020", updated.TaxBranchCode);
        Assert.NotEqual(created.RowVersion, updated.RowVersion);
        Assert.Equal($"\"{updated.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "branches.updated" && a.ResourceId == created.Id.ToString());
        Assert.Contains("taxBranchCode", audit.ChangesJson);
        Assert.DoesNotContain("00020", audit.ChangesJson);
        Assert.Equal(created.RowVersion, audit.RowVersionBefore);
    }

    [Fact]
    public async Task UpdateBranch_StaleVersion_Returns409AndMissingIfMatch_Returns428()
    {
        var created = await CreateAndReadAsync("B21");
        var body = new UpdateBranchRequest("n", null, null, null, null, null);

        var stale = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken, body, ifMatch: Guid.NewGuid());
        var missing = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken, body);

        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
    }

    [Fact]
    public async Task UpdateBranch_TaxCodeOwnedByAnotherBranch_Returns409WithTaxCodeError()
    {
        await CreateAndReadAsync("B22", "00022");
        var other = await CreateAndReadAsync("B23", "00023");

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{other.Id}", AdminToken,
            new UpdateBranchRequest("n", null, "00022", null, null, null), ifMatch: other.RowVersion);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("BRANCH_TAX_CODE_ALREADY_EXISTS", await ReadCode(response));
    }

    [Fact]
    public async Task UpdateBranch_OtherOrganizationBranch_Returns404AndLeavesItUntouched()
    {
        await using var before = NewDb();
        var orgBBranch = await before.Branches.AsNoTracking().SingleAsync(b => b.Id == TestOnlyDataSeeder.TestBranchBId);

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{orgBBranch.Id}", AdminToken,
            new UpdateBranchRequest("hijacked", null, null, null, null, null), ifMatch: orgBBranch.RowVersion);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await using var after = NewDb();
        Assert.Equal(orgBBranch.Name, (await after.Branches.AsNoTracking().SingleAsync(b => b.Id == orgBBranch.Id)).Name);
    }
```

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"` Expected: build FAIL — `UpdateBranchRequest` ไม่มี.

- [ ] **Step 4: Contracts + store + controller**

เพิ่มใน `OrganizationAdministrationContracts.cs` (ไม่มี `Code` — ไม่รับ จึง immutable ที่ระดับ contract):

```csharp
public sealed record UpdateBranchRequest(
    string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);
```

เพิ่มใน `OrganizationAdministrationStore`:

```csharp
    public Task<Result<BranchDetail>> UpdateBranchAsync(
        Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
            if (branch is null) return Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.");
            if (branch.RowVersion != ifMatch) return Fail<BranchDetail>(VersionConflict, "The branch was modified by another user.");

            var before = branch.RowVersion;
            var changed = ChangedFields(branch, input);
            try
            {
                branch.UpdateDetails(input.Name, input.NameEn, input.TaxBranchCode, input.AddressTh, input.AddressEn, input.Phone);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<BranchDetail>(ex.Code, ex.Message);
            }

            AddAudit(organizationId, actor, "branches.updated", "Branch", branch.Id.ToString(), traceId,
                new { changedFields = changed }, before, branch.RowVersion, branch.Id);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, TaxCodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_TAX_CODE_ALREADY_EXISTS", "A branch with this tax branch code already exists.");
            }

            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);

    private static string[] ChangedFields(Branch b, BranchInput i)
    {
        static string? N(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        var changed = new List<string>();
        if (b.Name != N(i.Name)) changed.Add("name");
        if (b.NameEn != N(i.NameEn)) changed.Add("nameEn");
        if (b.TaxBranchCode != N(i.TaxBranchCode)) changed.Add("taxBranchCode");
        if (b.AddressTh != N(i.AddressTh)) changed.Add("addressTh");
        if (b.AddressEn != N(i.AddressEn)) changed.Add("addressEn");
        if (b.Phone != N(i.Phone)) changed.Add("phone");
        return changed.ToArray();
    }
```

เพิ่มใน controller:

```csharp
    [HttpPut("branches/{branchId:guid}")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBranch([FromRoute] Guid branchId, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        return BranchResult(await _handler.UpdateBranchAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), branchId, conditional.Value.IfMatchRowVersion,
            new BranchInput(request.Name, request.NameEn, request.TaxBranchCode, request.AddressTh, request.AddressEn, request.Phone),
            HttpContext.TraceIdentifier, cancellationToken));
    }
```

- [ ] **Step 5: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"` Expected: PASS ทุกเคส.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Application/Organization backend/src/TanErp.Infrastructure/Persistence/Organization backend/src/TanErp.Api backend/tests/TanErp.UnitTests/Organization backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs
git commit -m "feat(org): add branch update endpoint

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 9: API — ปิด/เปิดสาขา, deactivation-check และ guard (เอกสารเปิด, membership, สาขา active สุดท้าย)

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Organization/BranchDependencyInspector.cs`
- Modify: `backend/src/TanErp.Application/Organization/Administration/{OrganizationAdministrationModels,IOrganizationAdministrationStore,OrganizationAdministrationHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Organization/OrganizationAdministrationContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminOrganizationController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`, `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/tests/TanErp.UnitTests/Organization/OrganizationAdministrationHandlerTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/BranchDependencyInspectorCoverageTests.cs`

- [ ] **Step 1: เขียน failing unit test (reason บังคับ, permission ก่อน)**

ใน `RecordingStore` เพิ่ม:

```csharp
        public Task<Result<BranchDeactivationCheck>> CheckDeactivationAsync(Guid organizationId, Guid branchId, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(Result<BranchDeactivationCheck>.Success(new BranchDeactivationCheck(true, Array.Empty<BranchBlocker>())));
        }

        public Task<Result<BranchDetail>> SetBranchActiveAsync(
            Guid organizationId, Guid branchId, bool active, string? reason, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
            GetBranchAsync(organizationId, branchId, ct);
```

เพิ่ม test:

```csharp
    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Deactivate_RequiresReason_BeforeTouchingTheStore(string? reason)
    {
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(new FakeAccess(), store);

        var result = await handler.DeactivateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), reason, "t", default);

        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task Deactivate_ReasonOver500Characters_IsRejected()
    {
        var handler = new OrganizationAdministrationHandler(new FakeAccess(), new RecordingStore());

        var result = await handler.DeactivateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), new string('x', 501), "t", default);

        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
    }

    [Fact]
    public async Task Deactivate_WithoutPermission_ReturnsPermissionDeniedEvenWithInvalidReason()
    {
        var handler = new OrganizationAdministrationHandler(new FakeAccess { Allow = false }, new RecordingStore());

        var result = await handler.DeactivateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), null, "t", default);

        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }
```

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~OrganizationAdministrationHandlerTests"` Expected: build FAIL (`BranchDeactivationCheck`, `DeactivateBranchAsync` ไม่มี).

- [ ] **Step 2: Application — models, port, handler**

ต่อท้าย `OrganizationAdministrationModels.cs`:

```csharp
public sealed record BranchBlocker(string Type, int Count);

public sealed record BranchDeactivationCheck(bool CanDeactivate, IReadOnlyList<BranchBlocker> Blockers);

/// <summary>Blocker type names are part of the API contract and of the FE i18n keys.</summary>
public static class BranchBlockerTypes
{
    public const string Estimates = "estimates";
    public const string Quotations = "quotations";
    public const string PurchaseOrders = "purchase_orders";
    public const string Billings = "billings";
    public const string WorkOrders = "work_orders";
    public const string Projects = "projects";
    public const string Installations = "installations";
    public const string SiteSurveys = "site_surveys";
    public const string Opportunities = "opportunities";
    public const string QuickEstimates = "quick_estimates";
    public const string MrpRuns = "mrp_runs";
    public const string Warehouses = "warehouses";
    public const string Memberships = "memberships";
    public const string LastActiveBranch = "last_active_branch";
}
```

เพิ่มใน port:

```csharp
    Task<Result<BranchDeactivationCheck>> CheckDeactivationAsync(Guid organizationId, Guid branchId, CancellationToken ct);
    Task<Result<BranchDetail>> SetBranchActiveAsync(
        Guid organizationId, Guid branchId, bool active, string? reason, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);
```

เพิ่มใน handler:

```csharp
    public const int MaxReasonLength = 500;

    public async Task<Result<BranchDeactivationCheck>> CheckDeactivationAsync(AdminCaller caller, Guid branchId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        return access.IsFailure ? Denied<BranchDeactivationCheck>(access) : await _store.CheckDeactivationAsync(access.Value!.OrganizationId, branchId, ct);
    }

    public async Task<Result<BranchDetail>> DeactivateBranchAsync(
        AdminCaller caller, Guid branchId, Guid ifMatch, string? reason, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        if (access.IsFailure) return Denied<BranchDetail>(access);

        var trimmed = reason?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaxReasonLength)
            return Result<BranchDetail>.Failure(new Error("REQUEST_VALIDATION_FAILED", $"Reason is required and cannot exceed {MaxReasonLength} characters."));

        return await _store.SetBranchActiveAsync(access.Value!.OrganizationId, branchId, false, trimmed, ifMatch, Actor(access.Value), traceId, ct);
    }

    public async Task<Result<BranchDetail>> ActivateBranchAsync(AdminCaller caller, Guid branchId, Guid ifMatch, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        return access.IsFailure
            ? Denied<BranchDetail>(access)
            : await _store.SetBranchActiveAsync(access.Value!.OrganizationId, branchId, true, null, ifMatch, Actor(access.Value), traceId, ct);
    }
```

Run unit test — Expected: PASS (รวม 5 Theory/Fact ใหม่).

- [ ] **Step 3: เขียน failing coverage test (บังคับให้ทุก entity ที่มี BranchId ถูกจัดประเภท)**

สร้าง `backend/tests/TanErp.IntegrationTests/Persistence/BranchDependencyInspectorCoverageTests.cs` (ไม่ต้องใช้ Docker — สร้าง model โดยไม่เชื่อมต่อ):

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Organization;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class BranchDependencyInspectorCoverageTests
{
    [Fact]
    public void EveryEntityWithBranchId_IsEitherCountedOrExcludedWithAReason()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql("Host=localhost;Database=model-only").Options;
        using var db = new AppDbContext(options);

        var withBranch = db.Model.GetEntityTypes()
            .Where(e => e.FindProperty("BranchId") is not null)
            .Select(e => e.ClrType)
            .ToHashSet();

        var classified = BranchDependencyInspector.CountedEntities.Keys.Concat(BranchDependencyInspector.ExcludedEntities.Keys).ToHashSet();

        var unclassified = withBranch.Except(classified).Select(t => t.Name).OrderBy(n => n).ToArray();
        Assert.True(unclassified.Length == 0,
            "Entities with BranchId not classified in BranchDependencyInspector (count them or exclude with a reason): " + string.Join(", ", unclassified));

        var stale = classified.Except(withBranch).Select(t => t.Name).OrderBy(n => n).ToArray();
        Assert.True(stale.Length == 0, "Classified types that no longer have BranchId: " + string.Join(", ", stale));
        Assert.All(BranchDependencyInspector.ExcludedEntities.Values, reason => Assert.False(string.IsNullOrWhiteSpace(reason)));
    }
}
```

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~BranchDependencyInspectorCoverageTests"` Expected: build FAIL (`BranchDependencyInspector` ไม่มี).

- [ ] **Step 4: สร้าง `BranchDependencyInspector`**

สร้าง `backend/src/TanErp.Infrastructure/Persistence/Organization/BranchDependencyInspector.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Organization.Administration;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;
using TanErp.Domain.Finance;
using TanErp.Domain.Inventory;
using TanErp.Domain.Mrp;
using TanErp.Domain.Procurement;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;
using TanErp.Domain.QuickEstimates;
using TanErp.Domain.Service;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Organization;

/// <summary>
/// The single definition of "open" work that keeps a branch from being deactivated (ADR 0019). Used by the
/// deactivation-check endpoint and by the guard inside the deactivation transaction, so the UI and the server cannot disagree.
/// </summary>
public sealed class BranchDependencyInspector
{
    private const string QuotationIssued = "issued"; // Quotation.Status has no constants class

    public static readonly IReadOnlyDictionary<Type, string> CountedEntities = new Dictionary<Type, string>
    {
        [typeof(Estimate)] = BranchBlockerTypes.Estimates,
        [typeof(Quotation)] = BranchBlockerTypes.Quotations,
        [typeof(PurchaseOrder)] = BranchBlockerTypes.PurchaseOrders,
        [typeof(BillingDocument)] = BranchBlockerTypes.Billings,
        [typeof(WorkOrder)] = BranchBlockerTypes.WorkOrders,
        [typeof(Project)] = BranchBlockerTypes.Projects,
        [typeof(InstallationJob)] = BranchBlockerTypes.Installations,
        [typeof(SiteSurvey)] = BranchBlockerTypes.SiteSurveys,
        [typeof(Opportunity)] = BranchBlockerTypes.Opportunities,
        [typeof(QuickEstimate)] = BranchBlockerTypes.QuickEstimates,
        [typeof(MrpRun)] = BranchBlockerTypes.MrpRuns,
        [typeof(Warehouse)] = BranchBlockerTypes.Warehouses,
        [typeof(TanErp.Domain.Organization.Membership)] = BranchBlockerTypes.Memberships,
    };

    /// <summary>Types that carry BranchId but are not open work. Every entry needs a reason; the coverage test enforces it.</summary>
    public static readonly IReadOnlyDictionary<Type, string> ExcludedEntities = new Dictionary<Type, string>
    {
        [typeof(GoodsReceipt)] = "Posted immutable receipt; no open lifecycle.",
        [typeof(StockDocument)] = "Posted immutable stock document; no open lifecycle.",
    };

    private readonly AppDbContext _db;

    public BranchDependencyInspector(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<BranchBlocker>> InspectAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        var o = organizationId;
        var b = branchId;
        var found = new List<BranchBlocker>();

        // Queries run one after another on purpose: a DbContext is not safe for concurrent use.
        async Task Add(string type, Task<int> count)
        {
            var n = await count;
            if (n > 0) found.Add(new BranchBlocker(type, n));
        }

        string[] openPo = [PurchaseOrderStatus.Draft, PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived];
        string[] openBilling = [BillingStatus.Issued, BillingStatus.PartiallyPaid];
        string[] openWorkOrder = [WorkOrderStatus.Draft, WorkOrderStatus.Released, WorkOrderStatus.InProgress];
        string[] openProject = [ProjectStatus.Planned, ProjectStatus.Active, ProjectStatus.OnHold, ProjectStatus.ReadyForHandover];
        string[] openInstallation = [InstallationStatus.Planned, InstallationStatus.InProgress, InstallationStatus.ReadyForHandover];
        string[] openSurvey = [SiteSurveyStatus.Scheduled, SiteSurveyStatus.InProgress];
        string[] closedOpportunity = [OpportunityStage.Won, OpportunityStage.Lost, OpportunityStage.Cancelled];
        string[] closedEstimate = [EstimateStatus.Quoted, EstimateStatus.Cancelled];

        await Add(BranchBlockerTypes.Estimates, _db.Estimates.CountAsync(x => x.OrganizationId == o && x.BranchId == b && !closedEstimate.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Quotations, _db.Quotations.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status == QuotationIssued, ct));
        await Add(BranchBlockerTypes.PurchaseOrders, _db.PurchaseOrders.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openPo.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Billings, _db.BillingDocuments.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openBilling.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.WorkOrders, _db.WorkOrders.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openWorkOrder.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Projects, _db.Projects.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openProject.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Installations, _db.InstallationJobs.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openInstallation.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.SiteSurveys, _db.SiteSurveys.CountAsync(x => x.OrganizationId == o && x.BranchId == b && openSurvey.Contains(x.Status), ct));
        await Add(BranchBlockerTypes.Opportunities, _db.Opportunities.CountAsync(x => x.OrganizationId == o && x.BranchId == b && !closedOpportunity.Contains(x.Stage), ct));
        await Add(BranchBlockerTypes.QuickEstimates, _db.QuickEstimates.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status != QuickEstimateStatus.Converted, ct));
        await Add(BranchBlockerTypes.MrpRuns, _db.MrpRecommendations
            .Where(r => r.OrganizationId == o && r.Status == MrpRecommendationStatus.Proposed && _db.MrpRuns.Any(run => run.Id == r.RunId && run.BranchId == b))
            .Select(r => r.RunId).Distinct().CountAsync(ct));
        await Add(BranchBlockerTypes.Warehouses, _db.Warehouses.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.Status == WarehouseStatus.Active, ct));
        await Add(BranchBlockerTypes.Memberships, _db.Memberships.CountAsync(x => x.OrganizationId == o && x.BranchId == b && x.IsActive, ct));

        return found;
    }
}
```

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~BranchDependencyInspectorCoverageTests"`
Expected: **FAIL ครั้งแรกอย่างมีเจตนา** พร้อมรายการ entity ที่มี `BranchId` แต่ยังไม่ถูกจัดประเภท (คาดว่า: `ItemBranchAvailability`, `DocumentSequenceCounter`, `TaxPolicyVersion`, `CalculationPolicyVersion`, `RolePermission`, `AuditEvent`, `CostRecord` และอื่น ๆ ที่ผลรันแสดง). จัดประเภทแต่ละตัวเป็น `ExcludedEntities` พร้อมเหตุผลสั้น ๆ (เช่น `"Configuration/grant/history keyed by branch, not open work."`) **หลังตรวจว่าไม่ใช่เอกสารที่มี lifecycle เปิดจริง**; ถ้าพบเอกสารที่เปิดได้ (เช่น type ใหม่ที่ไม่อยู่ใน Findings) ให้เพิ่มเข้า `CountedEntities` + `InspectAsync` + `BranchBlockerTypes` + สัญญา API แทน. ห้ามเพิ่ม exclusion โดยไม่มีเหตุผล. รันซ้ำจนผ่าน.

- [ ] **Step 5: เขียน failing integration test (endpoint + guard)**

ใน `InitializeAsync` ของ `OrganizationAdministrationEndpointsTests` เปลี่ยนการ seed เป็น `await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedEstimateDemoData: true);` (มี Estimate draft ที่สาขา `TestBranchId`). ต่อท้ายคลาส:

```csharp
    private async Task<HttpResponseMessage> Deactivate(Guid id, Guid ifMatch, string reason = "ปิดสาขาทดสอบ", string token = AdminToken, Guid? membershipId = null) =>
        await Send(HttpMethod.Post, $"/api/v1/admin/branches/{id}/deactivate", token, new DeactivateBranchRequest(reason), ifMatch: ifMatch, membershipId: membershipId);

    private async Task<BranchResponse> CurrentAsync(Guid id, string token = AdminToken, Guid? membershipId = null)
    {
        var response = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{id}", token, membershipId: membershipId);
        return (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
    }

    [Fact]
    public async Task Deactivate_BranchWithNothingOpen_SucceedsAuditsReasonAndCanBeReactivated()
    {
        var branch = await CreateAndReadAsync("B30");

        var off = await Deactivate(branch.Id, branch.RowVersion);

        Assert.Equal(HttpStatusCode.OK, off.StatusCode);
        var inactive = (await off.Content.ReadFromJsonAsync<BranchResponse>())!;
        Assert.False(inactive.IsActive);
        await using (var db = NewDb())
        {
            var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "branches.deactivated" && a.ResourceId == branch.Id.ToString());
            Assert.Contains("ปิดสาขาทดสอบ", audit.ChangesJson);
        }

        var on = await Send(HttpMethod.Post, $"/api/v1/admin/branches/{branch.Id}/activate", AdminToken, ifMatch: inactive.RowVersion);
        Assert.True((await on.Content.ReadFromJsonAsync<BranchResponse>())!.IsActive);
        await using var db2 = NewDb();
        Assert.True(await db2.AuditEvents.AnyAsync(a => a.Action == "branches.activated" && a.ResourceId == branch.Id.ToString()));
    }

    [Fact]
    public async Task Deactivate_BranchWithOpenEstimate_IsRejectedAndLeavesBranchActive()
    {
        var seeded = await CurrentAsync(TestOnlyDataSeeder.TestBranchId);

        var response = await Deactivate(seeded.Id, seeded.RowVersion);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("BRANCH_HAS_OPEN_DOCUMENTS", await ReadCode(response));
        Assert.True((await CurrentAsync(seeded.Id)).IsActive);

        var check = (await (await Send(HttpMethod.Get, $"/api/v1/admin/branches/{seeded.Id}/deactivation-check", AdminToken))
            .Content.ReadFromJsonAsync<BranchDeactivationCheckResponse>())!;
        Assert.False(check.CanDeactivate);
        Assert.Contains(check.Blockers, x => x.Type == "estimates" && x.Count >= 1);
        Assert.Contains(check.Blockers, x => x.Type == "memberships");
    }

    [Fact]
    public async Task Deactivate_ActiveWarehouse_BlocksAsOpenDocumentsUntilItIsInactive()
    {
        var branch = await CreateAndReadAsync("B31");
        await using (var db = NewDb())
        {
            db.Warehouses.Add(new TanErp.Domain.Inventory.Warehouse(
                Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, branch.Id, "WH-B31", "คลัง", null, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }

        var blocked = await Deactivate(branch.Id, branch.RowVersion);

        Assert.Equal("BRANCH_HAS_OPEN_DOCUMENTS", await ReadCode(blocked));
    }

    [Fact]
    public async Task Deactivate_ActiveMembershipOnly_ReturnsMembershipError()
    {
        var branch = await CreateAndReadAsync("B32");
        await using (var db = NewDb())
        {
            var userId = Guid.NewGuid();
            db.Users.Add(new User(userId, "uid-b32", "Member B32", "b32@example.test", isActive: true));
            db.Memberships.Add(new Membership(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, branch.Id, userId, isActive: true));
            await db.SaveChangesAsync();
        }

        var blocked = await Deactivate(branch.Id, branch.RowVersion);

        Assert.Equal("BRANCH_HAS_ACTIVE_MEMBERSHIPS", await ReadCode(blocked));
    }

    [Fact]
    public async Task Deactivate_LastActiveBranch_IsRejected()
    {
        await using (var db = NewDb())
        {
            // Organization B has exactly one branch; clear its only membership so the last-active guard is the sole blocker.
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE organization.memberships SET branch_id = NULL WHERE organization_id = {TestOnlyDataSeeder.TestOrgBId}");
        }

        var only = await CurrentAsync(TestOnlyDataSeeder.TestBranchBId, OrgBToken, TestOnlyDataSeeder.TestMembershipBId);
        var response = await Deactivate(only.Id, only.RowVersion, token: OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("BRANCH_LAST_ACTIVE", await ReadCode(response));
    }

    [Fact]
    public async Task Deactivate_ValidationAndConcurrency()
    {
        var branch = await CreateAndReadAsync("B33");

        var noReason = await Deactivate(branch.Id, branch.RowVersion, reason: " ");
        var stale = await Deactivate(branch.Id, Guid.NewGuid());
        var otherOrg = await Deactivate(TestOnlyDataSeeder.TestBranchBId, Guid.NewGuid());
        var viewer = await Deactivate(branch.Id, branch.RowVersion, token: ViewerToken, membershipId: ViewerMembershipId);

        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(noReason));
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal("RESOURCE_NOT_FOUND", await ReadCode(otherOrg));
        Assert.Equal(HttpStatusCode.Forbidden, viewer.StatusCode);
        Assert.True((await CurrentAsync(branch.Id)).IsActive);
    }
```

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests"` Expected: build FAIL — `DeactivateBranchRequest`, `BranchDeactivationCheckResponse` ไม่มี.

- [ ] **Step 6: Contracts**

เพิ่มใน `OrganizationAdministrationContracts.cs`:

```csharp
public sealed record DeactivateBranchRequest(string? Reason);

public sealed record BranchBlockerResponse(string Type, int Count);

public sealed record BranchDeactivationCheckResponse(bool CanDeactivate, IReadOnlyList<BranchBlockerResponse> Blockers)
{
    public static BranchDeactivationCheckResponse From(BranchDeactivationCheck c) =>
        new(c.CanDeactivate, c.Blockers.Select(b => new BranchBlockerResponse(b.Type, b.Count)).ToArray());
}
```

- [ ] **Step 7: Store — guard ใน transaction เดียวกัน**

แก้ constructor ของ `OrganizationAdministrationStore` ให้รับ `BranchDependencyInspector inspector` (field `_inspector`) แล้วเพิ่ม:

```csharp
    private async Task<List<BranchBlocker>> BlockersAsync(Branch branch, CancellationToken ct)
    {
        var blockers = (await _inspector.InspectAsync(branch.OrganizationId, branch.Id, ct)).ToList();
        var otherActive = await _db.Branches.CountAsync(b => b.OrganizationId == branch.OrganizationId && b.IsActive && b.Id != branch.Id, ct);
        if (branch.IsActive && otherActive == 0) blockers.Add(new BranchBlocker(BranchBlockerTypes.LastActiveBranch, 1));
        return blockers;
    }

    /// <summary>Priority: open work first, then memberships, then the last-active-branch rule.</summary>
    private static string BlockerErrorCode(IReadOnlyList<BranchBlocker> blockers)
    {
        if (blockers.Any(b => b.Type is not (BranchBlockerTypes.Memberships or BranchBlockerTypes.LastActiveBranch))) return "BRANCH_HAS_OPEN_DOCUMENTS";
        return blockers.Any(b => b.Type == BranchBlockerTypes.Memberships) ? "BRANCH_HAS_ACTIVE_MEMBERSHIPS" : "BRANCH_LAST_ACTIVE";
    }

    public async Task<Result<BranchDeactivationCheck>> CheckDeactivationAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
        if (branch is null) return Fail<BranchDeactivationCheck>("RESOURCE_NOT_FOUND", "Branch was not found.");

        var blockers = await BlockersAsync(branch, ct);
        return Result<BranchDeactivationCheck>.Success(new BranchDeactivationCheck(blockers.Count == 0, blockers));
    }

    public Task<Result<BranchDetail>> SetBranchActiveAsync(
        Guid organizationId, Guid branchId, bool active, string? reason, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            // One lock per organization, taken inside the transaction: two concurrent deactivations must not both see
            // "another active branch exists" and leave the organization with none.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({"branch-active:" + organizationId.ToString("N")}, 0))", ct);

            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
            if (branch is null) return Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.");
            if (branch.RowVersion != ifMatch) return Fail<BranchDetail>(VersionConflict, "The branch was modified by another user.");
            if (branch.IsActive == active) return Result<BranchDetail>.Success(ToDetail(branch));

            var before = branch.RowVersion;
            if (active)
            {
                branch.Activate();
                AddAudit(organizationId, actor, "branches.activated", "Branch", branch.Id.ToString(), traceId, new { }, before, branch.RowVersion, branch.Id);
            }
            else
            {
                var blockers = await BlockersAsync(branch, ct);
                if (blockers.Count > 0) return Fail<BranchDetail>(BlockerErrorCode(blockers), "The branch cannot be deactivated while it is in use.");

                branch.Deactivate();
                AddAudit(organizationId, actor, "branches.deactivated", "Branch", branch.Id.ToString(), traceId, new { reason }, before, branch.RowVersion, branch.Id);
            }

            await _db.SaveChangesAsync(ct);
            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);
```

- [ ] **Step 8: Controller, error codes, DI**

เพิ่มใน controller:

```csharp
    [HttpGet("branches/{branchId:guid}/deactivation-check")]
    [ProducesResponseType<BranchDeactivationCheckResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckDeactivation([FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.CheckDeactivationAsync(Caller(auth.Value!), branchId, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(BranchDeactivationCheckResponse.From(result.Value!));
    }

    [HttpPost("branches/{branchId:guid}/deactivate")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateBranch([FromRoute] Guid branchId, [FromBody] DeactivateBranchRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        return BranchResult(await _handler.DeactivateBranchAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), branchId, conditional.Value.IfMatchRowVersion,
            request.Reason, HttpContext.TraceIdentifier, cancellationToken));
    }

    [HttpPost("branches/{branchId:guid}/activate")]
    [ProducesResponseType<BranchResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateBranch([FromRoute] Guid branchId, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        return BranchResult(await _handler.ActivateBranchAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), branchId, conditional.Value.IfMatchRowVersion,
            HttpContext.TraceIdentifier, cancellationToken));
    }
```

`ProblemDetailsMapper.GetStatus`:

```csharp
        "BRANCH_HAS_OPEN_DOCUMENTS" => StatusCodes.Status409Conflict,
        "BRANCH_HAS_ACTIVE_MEMBERSHIPS" => StatusCodes.Status409Conflict,
        "BRANCH_LAST_ACTIVE" => StatusCodes.Status409Conflict,
```

Errors resx (TITLE / DETAIL):

| key | th | en |
| --- | --- | --- |
| `BRANCH_HAS_OPEN_DOCUMENTS` | ปิดสาขาไม่ได้ / สาขานี้ยังมีเอกสารหรือคลังที่ใช้งานอยู่ ต้องดำเนินการให้เสร็จก่อน | Cannot deactivate branch / This branch still has open documents or active warehouses. Complete or close them first. |
| `BRANCH_HAS_ACTIVE_MEMBERSHIPS` | ปิดสาขาไม่ได้ / ยังมีผู้ใช้ที่สังกัดสาขานี้ ให้ย้ายสาขาผู้ใช้ก่อน | Cannot deactivate branch / Users are still assigned to this branch. Move them first. |
| `BRANCH_LAST_ACTIVE` | ปิดสาขาไม่ได้ / องค์กรต้องมีสาขาที่เปิดใช้อย่างน้อยหนึ่งสาขา | Cannot deactivate branch / The organization must keep at least one active branch. |

DI: `grep -n "BranchDependencyInspector" backend/src/TanErp.Api/Program.cs` ต้องไม่พบ แล้วเพิ่มต่อจาก DI ของ store:

```csharp
builder.Services.AddScoped<TanErp.Infrastructure.Persistence.Organization.BranchDependencyInspector>();
```

- [ ] **Step 9: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests|FullyQualifiedName~BranchDependencyInspectorCoverageTests"` และ `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~Organization"`
Expected: PASS ทั้งหมด.

- [ ] **Step 10: Commit**

```bash
git add backend/src/TanErp.Application/Organization backend/src/TanErp.Infrastructure/Persistence/Organization backend/src/TanErp.Api backend/tests/TanErp.UnitTests/Organization backend/tests/TanErp.IntegrationTests/Api/OrganizationAdministrationEndpointsTests.cs backend/tests/TanErp.IntegrationTests/Persistence/BranchDependencyInspectorCoverageTests.cs
git commit -m "feat(org): add branch deactivation with open-work guards

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 11: Mutation check ของ guard (ทำหลัง commit; ไม่ commit การทดลอง)**

พิสูจน์ว่า test จับ guard ได้จริง ทีละการ mutation (แก้ → รัน → ต้องล้ม → คืนค่า):

1. ใน `BranchDependencyInspector.InspectAsync` เปลี่ยนบรรทัด estimates เป็น `x.BranchId == b && false` → รัน `--filter "Deactivate_BranchWithOpenEstimate"` Expected: **FAIL** (ปิดสำเร็จทั้งที่มี Estimate). คืนค่า: `git checkout -- backend/src/TanErp.Infrastructure/Persistence/Organization/BranchDependencyInspector.cs`.
2. ใน `OrganizationAdministrationStore.BlockersAsync` ลบบรรทัด `if (branch.IsActive && otherActive == 0) ...` → รัน `--filter "Deactivate_LastActiveBranch"` Expected: **FAIL**. คืนค่าด้วย `git checkout -- backend/src/TanErp.Infrastructure/Persistence/Organization/OrganizationAdministrationStore.cs`.
3. ใน `OrganizationAdministrationHandler.DeactivateBranchAsync` ย้าย `AccessAsync` ไปหลังการตรวจ reason → รัน `--filter "Deactivate_WithoutPermission_ReturnsPermissionDeniedEvenWithInvalidReason"` Expected: **FAIL**. คืนค่า `git checkout -- backend/src/TanErp.Application/Organization/Administration/OrganizationAdministrationHandler.cs`.
4. ใน `BranchConfiguration.cs` เปลี่ยน unique index tax code เป็นไม่ unique ไม่ได้ (ต้อง migration) — แทนที่ด้วย mutation ใน store: ให้ catch `IsUniqueViolation` ของ `CodeIndex` ใช้ `TaxCodeIndex` สลับกัน → รัน `--filter "CreateBranch_DuplicateCodeAndDuplicateTaxCode"` Expected: **FAIL** (รหัสผิดชนิด). คืนค่าด้วย `git checkout`.

หลังคืนทุกไฟล์: `git status --short` ต้องไม่มีไฟล์ของ Task นี้ค้าง และ test ชุดเดิมผ่านอีกครั้ง.

---

## Task 10: OpenAPI snapshot, generated types และ API client

**Files:**
- Regenerate: `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`, `frontend/src/lib/api/api-client.test.ts`

- [ ] **Step 1: regenerate snapshot (Backend)**

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"`
Run: `git diff --stat contracts/openapi/tan-erp.v1.json` → Expected: เพิ่มเท่านั้น (paths `/api/v1/admin/organization`, `/api/v1/admin/branches`, `/api/v1/admin/branches/{branchId}`, `/deactivation-check`, `/deactivate`, `/activate` + schemas `OrganizationProfileResponse`, `UpdateOrganizationProfileRequest`, `BranchResponse`, `CreateBranchRequest`, `UpdateBranchRequest`, `DeactivateBranchRequest`, `BranchDeactivationCheckResponse`, `BranchBlockerResponse`). ตรวจไม่มีบรรทัดถูกลบ: `git diff contracts/openapi/tan-erp.v1.json | grep '^-[^-]' | wc -l` → Expected: `0`.
Run (ไม่มี `UPDATE_OPENAPI`): `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"` → Expected: PASS.

- [ ] **Step 2: generate FE types**

Run: `cd frontend && npm run generate:api && npm run check:api`
Expected: `check:api` exit 0 (generated file ตรงกับ snapshot); `grep -c "BranchDeactivationCheckResponse" src/generated/api/tan-erp.v1.ts` ≥ 1.

- [ ] **Step 3: เขียน failing test ของ client**

ต่อท้าย `describe("ApiClient")` ใน `api-client.test.ts`:

```ts
  it("calls the organization and branch administration endpoints with If-Match and Idempotency-Key where required", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => ({}) });
    global.fetch = fetchMock;
    const client = new ApiClient("http://localhost:5000");
    const options = { token: "tok", membershipId: "m-1", locale: "th" as const };
    const branch = { name: "n", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null, phone: null };

    await client.getOrganizationProfile(options);
    await client.updateOrganizationProfile(branch, { ...options, ifMatch: "v1" });
    await client.listAdminBranches("inactive", options);
    await client.getAdminBranch("b 1", options);
    await client.createAdminBranch({ code: "B2", ...branch }, { ...options, idempotencyKey: "key-0123456789abcdef" });
    await client.updateAdminBranch("b1", branch, { ...options, ifMatch: "v2" });
    await client.getBranchDeactivationCheck("b1", options);
    await client.deactivateAdminBranch("b1", { reason: "ปิด" }, { ...options, ifMatch: "v3" });
    await client.activateAdminBranch("b1", { ...options, ifMatch: "v4" });

    expect(fetchMock.mock.calls.map(([url, init]) => [url, init.method])).toEqual([
      ["http://localhost:5000/api/v1/admin/organization", "GET"],
      ["http://localhost:5000/api/v1/admin/organization", "PUT"],
      ["http://localhost:5000/api/v1/admin/branches?status=inactive", "GET"],
      ["http://localhost:5000/api/v1/admin/branches/b%201", "GET"],
      ["http://localhost:5000/api/v1/admin/branches", "POST"],
      ["http://localhost:5000/api/v1/admin/branches/b1", "PUT"],
      ["http://localhost:5000/api/v1/admin/branches/b1/deactivation-check", "GET"],
      ["http://localhost:5000/api/v1/admin/branches/b1/deactivate", "POST"],
      ["http://localhost:5000/api/v1/admin/branches/b1/activate", "POST"],
    ]);
    expect(fetchMock.mock.calls[1][1].headers["If-Match"]).toBe('"v1"');
    expect(fetchMock.mock.calls[4][1].headers["Idempotency-Key"]).toBe("key-0123456789abcdef");
    expect(fetchMock.mock.calls[8][1].headers["If-Match"]).toBe('"v4"');
    // "all" must not add a status filter.
    await client.listAdminBranches(undefined, options);
    expect(fetchMock.mock.calls[9][0]).toBe("http://localhost:5000/api/v1/admin/branches");
  });
```

Run: `cd frontend && npx vitest run src/lib/api/api-client.test.ts` → Expected: FAIL (`client.getOrganizationProfile is not a function` / type error).

- [ ] **Step 4: เพิ่ม types + methods**

ใน `api-client.ts` ใกล้ `AdminUserResponse` (:88) เพิ่ม export types:

```ts
export type OrganizationProfileResponse = components["schemas"]["OrganizationProfileResponse"];
export type UpdateOrganizationProfileRequest = components["schemas"]["UpdateOrganizationProfileRequest"];
export type AdminBranchResponse = components["schemas"]["BranchResponse"];
export type CreateAdminBranchRequest = components["schemas"]["CreateBranchRequest"];
export type UpdateAdminBranchRequest = components["schemas"]["UpdateBranchRequest"];
export type DeactivateAdminBranchRequest = components["schemas"]["DeactivateBranchRequest"];
export type BranchDeactivationCheckResponse = components["schemas"]["BranchDeactivationCheckResponse"];
```

ในคลาส `ApiClient` ถัดจาก `listAdminRoles`:

```ts
  async getOrganizationProfile(options: RequestOptions): Promise<OrganizationProfileResponse> {
    return this.request<OrganizationProfileResponse>("/api/v1/admin/organization", "GET", options);
  }

  async updateOrganizationProfile(
    payload: UpdateOrganizationProfileRequest,
    options: RequestOptions
  ): Promise<OrganizationProfileResponse> {
    return this.request<OrganizationProfileResponse>("/api/v1/admin/organization", "PUT", options, payload);
  }

  async listAdminBranches(status: "active" | "inactive" | undefined, options: RequestOptions): Promise<AdminBranchResponse[]> {
    const query = status ? `?status=${status}` : "";
    return this.request<AdminBranchResponse[]>(`/api/v1/admin/branches${query}`, "GET", options);
  }

  async getAdminBranch(branchId: string, options: RequestOptions): Promise<AdminBranchResponse> {
    return this.request<AdminBranchResponse>(`/api/v1/admin/branches/${encodeURIComponent(branchId)}`, "GET", options);
  }

  async createAdminBranch(payload: CreateAdminBranchRequest, options: RequestOptions): Promise<AdminBranchResponse> {
    return this.request<AdminBranchResponse>("/api/v1/admin/branches", "POST", options, payload);
  }

  async updateAdminBranch(branchId: string, payload: UpdateAdminBranchRequest, options: RequestOptions): Promise<AdminBranchResponse> {
    return this.request<AdminBranchResponse>(`/api/v1/admin/branches/${encodeURIComponent(branchId)}`, "PUT", options, payload);
  }

  async getBranchDeactivationCheck(branchId: string, options: RequestOptions): Promise<BranchDeactivationCheckResponse> {
    return this.request<BranchDeactivationCheckResponse>(
      `/api/v1/admin/branches/${encodeURIComponent(branchId)}/deactivation-check`, "GET", options);
  }

  async deactivateAdminBranch(
    branchId: string,
    payload: DeactivateAdminBranchRequest,
    options: RequestOptions
  ): Promise<AdminBranchResponse> {
    return this.request<AdminBranchResponse>(
      `/api/v1/admin/branches/${encodeURIComponent(branchId)}/deactivate`, "POST", options, payload);
  }

  async activateAdminBranch(branchId: string, options: RequestOptions): Promise<AdminBranchResponse> {
    return this.request<AdminBranchResponse>(`/api/v1/admin/branches/${encodeURIComponent(branchId)}/activate`, "POST", options);
  }
```

Run: `cd frontend && npx vitest run src/lib/api/api-client.test.ts && npm run typecheck` → Expected: PASS. (ถ้า `request` ต้องการ body เป็น `undefined` สำหรับ POST ที่ไม่มี body ให้ดู `setAdminUserActive` เป็นตัวอย่าง — ใช้รูปแบบเดียวกัน.)

- [ ] **Step 5: Commit**

```bash
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api/api-client.ts frontend/src/lib/api/api-client.test.ts
git commit -m "feat(org): add organization admin api client

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 11: Frontend foundation — i18n, error whitelist และ query hooks

**Files:**
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json`
- Create: `frontend/src/features/settings/organization/organization-admin-errors.ts` (+ `.test.ts`)
- Create: `frontend/src/features/settings/organization/api/organization-admin-queries.ts`

- [ ] **Step 1: เขียน failing test (error whitelist + parity th/en)**

สร้าง `frontend/src/features/settings/organization/organization-admin-errors.test.ts`:

```ts
import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { KNOWN_ORGANIZATION_ADMIN_ERROR_CODES, organizationAdminErrorKey } from "./organization-admin-errors";

function leafKeys(value: unknown, prefix = ""): string[] {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => leafKeys(child, prefix ? `${prefix}.${key}` : key));
}

describe("organizationAdminErrorKey", () => {
  it("maps known backend codes and falls back to GENERIC", () => {
    expect(organizationAdminErrorKey(new ApiError({ status: 409, code: "BRANCH_LAST_ACTIVE", message: "x" }))).toBe("BRANCH_LAST_ACTIVE");
    expect(organizationAdminErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(organizationAdminErrorKey(new Error("BRANCH_LAST_ACTIVE"))).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code plus GENERIC", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of [...KNOWN_ORGANIZATION_ADMIN_ERROR_CODES, "GENERIC"]) {
        expect(messages.organizationAdmin.errors).toHaveProperty(code);
      }
    }
  });

  it("keeps the organizationAdmin namespace identical in Thai and English", () => {
    expect(leafKeys(thMessages.organizationAdmin).sort()).toEqual(leafKeys(enMessages.organizationAdmin).sort());
  });

  it("has a blocker label for every backend blocker type", () => {
    const types = ["estimates", "quotations", "purchase_orders", "billings", "work_orders", "projects", "installations",
      "site_surveys", "opportunities", "quick_estimates", "mrp_runs", "warehouses", "memberships", "last_active_branch"];
    for (const type of types) {
      expect(thMessages.organizationAdmin.branches.blockers).toHaveProperty(type);
      expect(enMessages.organizationAdmin.branches.blockers).toHaveProperty(type);
    }
  });
});
```

Run: `cd frontend && npx vitest run src/features/settings/organization/organization-admin-errors.test.ts` → Expected: FAIL (module/namespace ไม่มี).

- [ ] **Step 2: error whitelist**

สร้าง `organization-admin-errors.ts`:

```ts
import { ApiError } from "@/lib/api/api-error";

/** Backend codes that have a dedicated message in `organizationAdmin.errors`. Anything else shows the generic message. */
export const KNOWN_ORGANIZATION_ADMIN_ERROR_CODES = [
  "ADMIN_VERSION_CONFLICT",
  "ORGANIZATION_TAX_ID_INVALID",
  "BRANCH_CODE_INVALID",
  "BRANCH_TAX_CODE_INVALID",
  "BRANCH_CODE_ALREADY_EXISTS",
  "BRANCH_TAX_CODE_ALREADY_EXISTS",
  "BRANCH_HAS_OPEN_DOCUMENTS",
  "BRANCH_HAS_ACTIVE_MEMBERSHIPS",
  "BRANCH_LAST_ACTIVE",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

const KNOWN = new Set<string>(KNOWN_ORGANIZATION_ADMIN_ERROR_CODES);

export function organizationAdminErrorKey(error: unknown): string {
  return error instanceof ApiError && KNOWN.has(error.code) ? error.code : "GENERIC";
}
```

- [ ] **Step 3: เพิ่ม messages th/en (สคริปต์ guard round-trip เหมือน G-02)**

เพิ่มคีย์ `shell.organizationSettings` ("ข้อมูลองค์กร" / "Organization") และ `shell.branchSettings` ("สาขา" / "Branches") และ namespace `organizationAdmin` โดยรันจาก `frontend/`:

```bash
python3 - <<'PY'
import json

TH = {
  "accessDeniedTitle": "ไม่มีสิทธิ์เข้าถึง",
  "accessDeniedDetail": "คุณไม่มีสิทธิ์จัดการข้อมูลองค์กรและสาขา",
  "profile": {
    "title": "ข้อมูลองค์กร", "subtitle": "ชื่อ เลขประจำตัวผู้เสียภาษี และที่อยู่ที่ใช้ออกเอกสาร",
    "fields": {"name": "ชื่อองค์กร (ไทย)", "nameEn": "ชื่อองค์กร (อังกฤษ)", "taxIdentifier": "เลขประจำตัวผู้เสียภาษี",
               "taxIdentifierHint": "ตัวเลข 13 หลัก ไม่ต้องใส่ขีด", "addressTh": "ที่อยู่ออกเอกสาร (ไทย)",
               "addressEn": "ที่อยู่ออกเอกสาร (อังกฤษ)", "phone": "โทรศัพท์"},
    "validation": {"nameRequired": "กรุณากรอกชื่อองค์กร", "taxIdentifierFormat": "เลขประจำตัวผู้เสียภาษีต้องเป็นตัวเลข 13 หลัก"},
    "save": "บันทึกข้อมูลองค์กร", "saveSuccess": "บันทึกข้อมูลองค์กรแล้ว", "readOnlyNotice": "คุณดูข้อมูลได้เท่านั้น ไม่มีสิทธิ์แก้ไข"
  },
  "branches": {
    "title": "สาขา", "subtitle": "จัดการสาขา เลขสาขาภาษี และการเปิด/ปิดใช้งาน", "createBranch": "เพิ่มสาขา",
    "createTitle": "เพิ่มสาขา", "createSubtitle": "รหัสสาขาแก้ไขไม่ได้หลังสร้าง", "createSubmit": "เพิ่มสาขา", "createSuccess": "เพิ่มสาขาแล้ว",
    "editTitle": "แก้ไขสาขา", "saveSubmit": "บันทึกการเปลี่ยนแปลง", "saveSuccess": "บันทึกสาขาแล้ว", "backToList": "กลับไปรายการสาขา",
    "statusFilter": "สถานะ", "status": {"active": "ใช้งาน", "inactive": "ปิดใช้งาน"},
    "table": {"code": "รหัสสาขา", "name": "ชื่อสาขา", "taxBranchCode": "เลขสาขาภาษี", "status": "สถานะ"},
    "emptyTitle": "ยังไม่มีสาขา", "emptyDescription": "เพิ่มสาขาแรกเพื่อเริ่มใช้งาน",
    "fields": {"code": "รหัสสาขา", "name": "ชื่อสาขา (ไทย)", "nameEn": "ชื่อสาขา (อังกฤษ)", "taxBranchCode": "เลขสาขาภาษี",
               "taxBranchCodeHint": "ตัวเลข 5 หลัก (00000 = สำนักงานใหญ่)", "addressTh": "ที่อยู่ (ไทย)", "addressEn": "ที่อยู่ (อังกฤษ)", "phone": "โทรศัพท์"},
    "validation": {"codeRequired": "กรุณากรอกรหัสสาขา", "codeFormat": "ใช้ได้เฉพาะตัวอักษรอังกฤษ ตัวเลข ขีดล่าง และขีดกลาง",
                   "nameRequired": "กรุณากรอกชื่อสาขา", "taxBranchCodeFormat": "เลขสาขาภาษีต้องเป็นตัวเลข 5 หลัก"},
    "deactivate": {"action": "ปิดใช้งานสาขา", "title": "ปิดใช้งานสาขา", "message": "สาขานี้จะไม่แสดงในรายการเลือกสาขาอีก ยืนยันการปิดใช้งาน?",
                   "reasonLabel": "เหตุผล", "reasonRequired": "กรุณาระบุเหตุผล", "confirm": "ปิดใช้งาน", "success": "ปิดใช้งานสาขาแล้ว",
                   "blockedTitle": "ยังปิดสาขานี้ไม่ได้", "blockedIntro": "ต้องจัดการรายการต่อไปนี้ก่อน"},
    "activate": {"action": "เปิดใช้งานสาขา", "title": "เปิดใช้งานสาขา", "message": "สาขานี้จะกลับมาให้เลือกใช้ในเอกสารใหม่ ยืนยันการเปิดใช้งาน?",
                 "confirm": "เปิดใช้งาน", "success": "เปิดใช้งานสาขาแล้ว"},
    "blockers": {"estimates": "ใบประเมินราคาที่ยังไม่สิ้นสุด", "quotations": "ใบเสนอราคาที่ออกแล้วและยังไม่ปิด", "purchase_orders": "ใบสั่งซื้อที่ยังเปิดอยู่",
                 "billings": "เอกสารเรียกเก็บเงินที่ยังไม่ชำระครบ", "work_orders": "ใบสั่งผลิตที่ยังเปิดอยู่", "projects": "โครงการที่ยังไม่สิ้นสุด",
                 "installations": "งานติดตั้งที่ยังไม่สิ้นสุด", "site_surveys": "งานสำรวจหน้างานที่ยังไม่เสร็จ", "opportunities": "โอกาสทางการขายที่ยังเปิดอยู่",
                 "quick_estimates": "ประเมินราคาด่วนที่ยังไม่แปลงเป็นใบประเมิน", "mrp_runs": "รอบวางแผนที่มีรายการรออนุมัติ", "warehouses": "คลังสินค้าที่ยังใช้งาน",
                 "memberships": "ผู้ใช้ที่สังกัดสาขานี้", "last_active_branch": "องค์กรต้องมีสาขาที่เปิดใช้อย่างน้อยหนึ่งสาขา"}
  },
  "errors": {
    "GENERIC": "ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง",
    "ADMIN_VERSION_CONFLICT": "ข้อมูลถูกแก้ไขโดยผู้อื่นแล้ว กรุณาโหลดใหม่แล้วลองอีกครั้ง",
    "ORGANIZATION_TAX_ID_INVALID": "เลขประจำตัวผู้เสียภาษีไม่ถูกต้อง",
    "BRANCH_CODE_INVALID": "รหัสสาขาไม่ถูกต้อง",
    "BRANCH_TAX_CODE_INVALID": "เลขสาขาภาษีไม่ถูกต้อง",
    "BRANCH_CODE_ALREADY_EXISTS": "มีสาขาที่ใช้รหัสนี้อยู่แล้ว",
    "BRANCH_TAX_CODE_ALREADY_EXISTS": "มีสาขาที่ใช้เลขสาขาภาษีนี้อยู่แล้ว",
    "BRANCH_HAS_OPEN_DOCUMENTS": "สาขานี้ยังมีเอกสารหรือคลังที่ใช้งานอยู่",
    "BRANCH_HAS_ACTIVE_MEMBERSHIPS": "ยังมีผู้ใช้ที่สังกัดสาขานี้",
    "BRANCH_LAST_ACTIVE": "องค์กรต้องมีสาขาที่เปิดใช้อย่างน้อยหนึ่งสาขา",
    "IDEMPOTENCY_KEY_REUSED": "คำขอซ้ำไม่ตรงกับคำขอเดิม กรุณาลองใหม่",
    "PERMISSION_DENIED": "คุณไม่มีสิทธิ์ดำเนินการนี้",
    "RESOURCE_NOT_FOUND": "ไม่พบข้อมูลที่ต้องการ"
  }
}

EN = {
  "accessDeniedTitle": "Access denied",
  "accessDeniedDetail": "You do not have permission to manage organization and branch data.",
  "profile": {
    "title": "Organization", "subtitle": "Name, tax identifier and the address printed on documents",
    "fields": {"name": "Organization name (Thai)", "nameEn": "Organization name (English)", "taxIdentifier": "Tax identifier",
               "taxIdentifierHint": "13 digits, no dashes", "addressTh": "Document address (Thai)",
               "addressEn": "Document address (English)", "phone": "Phone"},
    "validation": {"nameRequired": "Enter the organization name", "taxIdentifierFormat": "The tax identifier must be 13 digits"},
    "save": "Save organization", "saveSuccess": "Organization saved", "readOnlyNotice": "You can view this data but not edit it."
  },
  "branches": {
    "title": "Branches", "subtitle": "Manage branches, tax branch codes and activation", "createBranch": "Add branch",
    "createTitle": "Add branch", "createSubtitle": "The branch code cannot be changed after creation", "createSubmit": "Add branch", "createSuccess": "Branch added",
    "editTitle": "Edit branch", "saveSubmit": "Save changes", "saveSuccess": "Branch saved", "backToList": "Back to branches",
    "statusFilter": "Status", "status": {"active": "Active", "inactive": "Inactive"},
    "table": {"code": "Branch code", "name": "Branch name", "taxBranchCode": "Tax branch code", "status": "Status"},
    "emptyTitle": "No branches yet", "emptyDescription": "Add the first branch to get started",
    "fields": {"code": "Branch code", "name": "Branch name (Thai)", "nameEn": "Branch name (English)", "taxBranchCode": "Tax branch code",
               "taxBranchCodeHint": "5 digits (00000 = head office)", "addressTh": "Address (Thai)", "addressEn": "Address (English)", "phone": "Phone"},
    "validation": {"codeRequired": "Enter the branch code", "codeFormat": "Use letters, digits, underscore or hyphen only",
                   "nameRequired": "Enter the branch name", "taxBranchCodeFormat": "The tax branch code must be 5 digits"},
    "deactivate": {"action": "Deactivate branch", "title": "Deactivate branch", "message": "This branch will no longer be offered in branch pickers. Deactivate it?",
                   "reasonLabel": "Reason", "reasonRequired": "Enter a reason", "confirm": "Deactivate", "success": "Branch deactivated",
                   "blockedTitle": "This branch cannot be deactivated yet", "blockedIntro": "Resolve the following first"},
    "activate": {"action": "Activate branch", "title": "Activate branch", "message": "This branch becomes available for new documents again. Activate it?",
                 "confirm": "Activate", "success": "Branch activated"},
    "blockers": {"estimates": "Estimates that are not finished", "quotations": "Issued quotations that are not closed", "purchase_orders": "Open purchase orders",
                 "billings": "Billing documents not fully paid", "work_orders": "Open work orders", "projects": "Projects that are not finished",
                 "installations": "Installation jobs that are not finished", "site_surveys": "Site surveys that are not completed", "opportunities": "Open opportunities",
                 "quick_estimates": "Quick estimates not yet converted", "mrp_runs": "Planning runs with recommendations awaiting approval", "warehouses": "Active warehouses",
                 "memberships": "Users assigned to this branch", "last_active_branch": "The organization must keep at least one active branch"}
  },
  "errors": {
    "GENERIC": "The action failed. Please try again.",
    "ADMIN_VERSION_CONFLICT": "This record was changed by someone else. Reload and try again.",
    "ORGANIZATION_TAX_ID_INVALID": "The tax identifier is not valid.",
    "BRANCH_CODE_INVALID": "The branch code is not valid.",
    "BRANCH_TAX_CODE_INVALID": "The tax branch code is not valid.",
    "BRANCH_CODE_ALREADY_EXISTS": "A branch with this code already exists.",
    "BRANCH_TAX_CODE_ALREADY_EXISTS": "A branch with this tax branch code already exists.",
    "BRANCH_HAS_OPEN_DOCUMENTS": "This branch still has open documents or active warehouses.",
    "BRANCH_HAS_ACTIVE_MEMBERSHIPS": "Users are still assigned to this branch.",
    "BRANCH_LAST_ACTIVE": "The organization must keep at least one active branch.",
    "IDEMPOTENCY_KEY_REUSED": "This repeated request differs from the original. Please try again.",
    "PERMISSION_DENIED": "You do not have permission to do this.",
    "RESOURCE_NOT_FOUND": "The requested record was not found."
  }
}

SHELL = {"src/messages/th.json": ("ข้อมูลองค์กร", "สาขา"), "src/messages/en.json": ("Organization", "Branches")}
for path, ns in (("src/messages/th.json", TH), ("src/messages/en.json", EN)):
    raw = open(path, encoding="utf-8").read()
    data = json.loads(raw)
    assert json.dumps(data, indent=2, ensure_ascii=False) + "\n" == raw, f"{path} does not round-trip; edit by hand"
    assert "organizationAdmin" not in data and "organizationSettings" not in data["shell"], f"{path} already updated"
    data["shell"]["organizationSettings"], data["shell"]["branchSettings"] = SHELL[path]
    data["organizationAdmin"] = ns
    open(path, "w", encoding="utf-8").write(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
PY
git diff --stat -- src/messages
```

Expected: เฉพาะบรรทัดที่เพิ่ม (insertions; deletions เฉพาะ `,` ท้ายคีย์สุดท้ายที่ต่อท้าย). ถ้า assert round-trip ล้ม ให้เพิ่มด้วย Edit ท้ายไฟล์แทน.

Run: `cd frontend && npx vitest run src/features/settings/organization/organization-admin-errors.test.ts` → Expected: PASS ทั้ง 4.

- [ ] **Step 4: query hooks (reuse pattern ของ user-admin)**

สร้าง `frontend/src/features/settings/organization/api/organization-admin-queries.ts`:

```ts
import {
  useMutation,
  useQuery,
  useQueryClient,
  type QueryClient,
  type UseMutationResult,
  type UseQueryResult,
} from "@tanstack/react-query";
import {
  apiClient,
  type AdminBranchResponse,
  type BranchDeactivationCheckResponse,
  type CreateAdminBranchRequest,
  type OrganizationProfileResponse,
  type UpdateAdminBranchRequest,
  type UpdateOrganizationProfileRequest,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const organizationAdminKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  profile: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "org-admin", "profile"] as const,
  branches: (membershipId: string | null | undefined, locale: ApiLocale, status: string) =>
    ["business", membershipId, locale, "org-admin", "branches", status] as const,
  branch: (membershipId: string | null | undefined, locale: ApiLocale, branchId: string) =>
    ["business", membershipId, locale, "org-admin", "branch", branchId] as const,
  deactivationCheck: (membershipId: string | null | undefined, locale: ApiLocale, branchId: string) =>
    ["business", membershipId, locale, "org-admin", "deactivation-check", branchId] as const,
};

/** Branch changes also change every branch picker in the app, so refresh all business queries (as user-admin does). */
function invalidateOrg(queryClient: QueryClient, membershipId: string | undefined): void {
  void queryClient.invalidateQueries({ queryKey: organizationAdminKeys.all(membershipId) });
}

export function useOrganizationProfile(): UseQueryResult<OrganizationProfileResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.profile(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.getOrganizationProfile(await context.buildOptions({ signal })),
  });
}

export function useUpdateOrganizationProfile(): UseMutationResult<
  OrganizationProfileResponse, Error, { payload: UpdateOrganizationProfileRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, ifMatch }) =>
      apiClient.updateOrganizationProfile(payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useAdminBranches(status: "active" | "inactive" | undefined): UseQueryResult<AdminBranchResponse[], Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.branches(context.membershipId, context.locale, status ?? "all"),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAdminBranches(status, await context.buildOptions({ signal })),
  });
}

export function useAdminBranch(branchId: string, enabled: boolean): UseQueryResult<AdminBranchResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.branch(context.membershipId, context.locale, branchId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getAdminBranch(branchId, await context.buildOptions({ signal })),
  });
}

export function useBranchDeactivationCheck(branchId: string, enabled: boolean): UseQueryResult<BranchDeactivationCheckResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.deactivationCheck(context.membershipId, context.locale, branchId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getBranchDeactivationCheck(branchId, await context.buildOptions({ signal })),
  });
}

export function useCreateAdminBranch(): UseMutationResult<
  AdminBranchResponse, Error, { payload: CreateAdminBranchRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createAdminBranch(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useUpdateAdminBranch(): UseMutationResult<
  AdminBranchResponse, Error, { branchId: string; payload: UpdateAdminBranchRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ branchId, payload, ifMatch }) =>
      apiClient.updateAdminBranch(branchId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useSetAdminBranchActive(): UseMutationResult<
  AdminBranchResponse, Error, { branchId: string; active: boolean; reason?: string; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ branchId, active, reason, ifMatch }) => {
      const options = await context.buildOptions({ ifMatch });
      return active
        ? apiClient.activateAdminBranch(branchId, options)
        : apiClient.deactivateAdminBranch(branchId, { reason: reason ?? "" }, options);
    },
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}
```

Run: `cd frontend && npm run typecheck && npm run lint` → Expected: 0 errors (ไม่มี `any`).

- [ ] **Step 5: Commit**

```bash
git add frontend/src/messages/th.json frontend/src/messages/en.json frontend/src/features/settings/organization
git commit -m "feat(org): add organization admin translations, error map and query hooks

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 12: Frontend — หน้า Organization profile

กลุ่มข้อมูลเดียว (ชื่อ/ภาษี/ที่อยู่) จึง **ไม่ใช้ `FormTabs`** (ตาม AGENTS.md ใช้เมื่อมีหลายกลุ่ม). ผู้ที่มี `organizations.read` แต่ไม่มี `organizations.manage` เห็นฟอร์มแบบอ่านอย่างเดียว (ตรงกับ Backend ที่ GET ใช้ `read`, PUT ใช้ `manage`).

**Files:**
- Create: `frontend/src/features/settings/organization/components/organization-profile-form.tsx` (+ `.test.tsx`)
- Create: `frontend/src/app/[locale]/(erp)/settings/organization/page.tsx`

- [ ] **Step 1: เขียน failing test**

สร้าง `organization-profile-form.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { OrganizationProfileForm } from "./organization-profile-form";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  update: vi.fn(),
  success: vi.fn(),
  isPending: false,
}));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/organization-admin-queries", () => ({
  useOrganizationProfile: () => ({
    isPending: false,
    isError: false,
    data: {
      id: "o1", name: "บริษัท เดิม", nameEn: null, taxIdentifier: "0105536000003",
      addressTh: "1 ถนนทดสอบ", addressEn: null, phone: null, rowVersion: "v1",
    },
  }),
  useUpdateOrganizationProfile: () => ({ mutateAsync: mocks.update, isPending: mocks.isPending }),
}));

describe("OrganizationProfileForm", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.isPending = false;
    mocks.permissions = ["organizations.read", "organizations.manage"];
    mocks.update.mockResolvedValue({});
  });

  it("shows the loaded values and saves with the row version, sending blank optional fields as null", async () => {
    render(<OrganizationProfileForm />);
    expect(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/)).toHaveValue("บริษัท เดิม");

    fireEvent.change(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/), { target: { value: "  บริษัท ใหม่  " } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      ifMatch: "v1",
      payload: {
        name: "บริษัท ใหม่", nameEn: null, taxIdentifier: "0105536000003",
        addressTh: "1 ถนนทดสอบ", addressEn: null, phone: null,
      },
    });
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("บันทึกข้อมูลองค์กรแล้ว"));
  });

  it("blocks a malformed tax identifier before calling the API", async () => {
    render(<OrganizationProfileForm />);

    fireEvent.change(screen.getByLabelText(/เลขประจำตัวผู้เสียภาษี/), { target: { value: "123" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    expect(await screen.findByText("เลขประจำตัวผู้เสียภาษีต้องเป็นตัวเลข 13 หลัก")).toBeInTheDocument();
    expect(mocks.update).not.toHaveBeenCalled();
  });

  it("shows the dedicated message for a server-side tax identifier error", async () => {
    mocks.update.mockRejectedValueOnce(new ApiError({ status: 422, code: "ORGANIZATION_TAX_ID_INVALID", message: "x" }));
    render(<OrganizationProfileForm />);

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    expect(await screen.findByText("เลขประจำตัวผู้เสียภาษีไม่ถูกต้อง")).toBeInTheDocument();
  });

  it("disables the save button while the request is in flight", () => {
    mocks.isPending = true;
    render(<OrganizationProfileForm />);

    expect(screen.getByRole("button", { name: /บันทึกข้อมูลองค์กร/ })).toBeDisabled();
  });

  it("is read-only without organizations.manage", () => {
    mocks.permissions = ["organizations.read"];
    render(<OrganizationProfileForm />);

    expect(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/)).toBeDisabled();
    expect(screen.queryByRole("button", { name: "บันทึกข้อมูลองค์กร" })).not.toBeInTheDocument();
    expect(screen.getByText("คุณดูข้อมูลได้เท่านั้น ไม่มีสิทธิ์แก้ไข")).toBeInTheDocument();
  });
});
```

Run: `cd frontend && npx vitest run src/features/settings/organization/components/organization-profile-form.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: เขียน component**

สร้าง `organization-profile-form.tsx`:

```tsx
"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import type { OrganizationProfileResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useOrganizationProfile, useUpdateOrganizationProfile } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";

const TAX_IDENTIFIER_PATTERN = /^\d{13}$/;

interface ProfileFormValues {
  name: string;
  nameEn: string;
  taxIdentifier: string;
  addressTh: string;
  addressEn: string;
  phone: string;
}

const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

/** Nullable API fields become empty inputs; blank inputs go back to the API as null. */
function toFormValues(profile: OrganizationProfileResponse): ProfileFormValues {
  return {
    name: profile.name,
    nameEn: profile.nameEn ?? "",
    taxIdentifier: profile.taxIdentifier ?? "",
    addressTh: profile.addressTh ?? "",
    addressEn: profile.addressEn ?? "",
    phone: profile.phone ?? "",
  };
}

export function OrganizationProfileForm() {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ORGANIZATIONS_MANAGE);
  const profileQuery = useOrganizationProfile();
  const updateMutation = useUpdateOrganizationProfile();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const schema = useMemo(
    () =>
      z.object({
        name: z.string().trim().min(1, t("profile.validation.nameRequired")).max(255),
        nameEn: z.string().trim().max(255),
        taxIdentifier: z.string().trim().refine((v) => v === "" || TAX_IDENTIFIER_PATTERN.test(v), t("profile.validation.taxIdentifierFormat")),
        addressTh: z.string().trim().max(500),
        addressEn: z.string().trim().max(500),
        phone: z.string().trim().max(30),
      }),
    [t]
  );

  const profile = profileQuery.data;
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<ProfileFormValues>({ resolver: zodResolver(schema), values: profile ? toFormValues(profile) : undefined });

  if (profileQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (profileQuery.isError || !profile) return <Alert variant="danger">{t("errors.GENERIC")}</Alert>;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    try {
      await updateMutation.mutateAsync({
        ifMatch: profile.rowVersion,
        payload: {
          name: values.name.trim(),
          nameEn: orNull(values.nameEn),
          taxIdentifier: orNull(values.taxIdentifier),
          addressTh: orNull(values.addressTh),
          addressEn: orNull(values.addressEn),
          phone: orNull(values.phone),
        },
      });
      toast.success(t("profile.saveSuccess"));
    } catch (error: unknown) {
      setSubmitError(t(`errors.${organizationAdminErrorKey(error)}`));
    }
  });

  return (
    <FormContainer
      asForm
      noValidate
      onSubmit={onSubmit}
      maxWidth="lg"
      header={<PageHeader title={t("profile.title")} subtitle={t("profile.subtitle")} />}
      errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
      actionBar={
        canManage ? (
          <FormActionBar
            isDirty={isDirty}
            isLoading={updateMutation.isPending}
            isSaveDisabled={updateMutation.isPending}
            saveText={t("profile.save")}
            saveButtonType="submit"
          />
        ) : undefined
      }
    >
      {!canManage ? <Alert variant="info">{t("profile.readOnlyNotice")}</Alert> : null}
      <FormSection title={t("profile.title")} description={t("profile.subtitle")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Input label={t("profile.fields.name")} required disabled={!canManage} error={errors.name?.message} {...register("name")} />
          <Input label={t("profile.fields.nameEn")} disabled={!canManage} error={errors.nameEn?.message} {...register("nameEn")} />
          <Input
            label={t("profile.fields.taxIdentifier")}
            helperText={t("profile.fields.taxIdentifierHint")}
            inputMode="numeric"
            maxLength={13}
            disabled={!canManage}
            error={errors.taxIdentifier?.message}
            {...register("taxIdentifier")}
          />
          <Input label={t("profile.fields.phone")} disabled={!canManage} error={errors.phone?.message} {...register("phone")} />
          <Textarea label={t("profile.fields.addressTh")} rows={3} disabled={!canManage} error={errors.addressTh?.message} {...register("addressTh")} />
          <Textarea label={t("profile.fields.addressEn")} rows={3} disabled={!canManage} error={errors.addressEn?.message} {...register("addressEn")} />
        </div>
      </FormSection>
    </FormContainer>
  );
}
```

หมายเหตุ: `Input` ใช้ `helperText` (`Input.tsx:11`) และ `Alert` รองรับ variant `info` (`Alert.tsx:13`); `saveText` ต้องตรงกับชื่อปุ่มใน test.

- [ ] **Step 3: route**

สร้าง `frontend/src/app/[locale]/(erp)/settings/organization/page.tsx`:

```tsx
"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { OrganizationProfileForm } from "@/features/settings/organization/components/organization-profile-form";

export default function OrganizationSettingsPage() {
  const t = useTranslations("organizationAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ORGANIZATIONS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <OrganizationProfileForm />
    </PermissionGuard>
  );
}
```

- [ ] **Step 4: Run — ต้องผ่าน**

Run: `cd frontend && npx vitest run src/features/settings/organization && npm run typecheck && npm run lint` → Expected: PASS, 0 errors. (jsdom ไม่ render layout จริง: test ตรวจ DOM/disabled/คำเรียก API เท่านั้น ไม่ตรวจ responsive/สี — ตรวจด้วยตาในเบราว์เซอร์ใน Task 15.)

- [ ] **Step 5: Commit**

```bash
git add frontend/src/features/settings/organization/components/organization-profile-form.tsx frontend/src/features/settings/organization/components/organization-profile-form.test.tsx "frontend/src/app/[locale]/(erp)/settings/organization"
git commit -m "feat(org): add organization profile settings page

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 13: Frontend — รายการสาขา

**Files:**
- Create: `frontend/src/features/settings/organization/components/branch-admin-list.tsx` (+ `.test.tsx`)
- Create: `frontend/src/app/[locale]/(erp)/settings/branches/page.tsx`

- [ ] **Step 1: เขียน failing test**

สร้าง `branch-admin-list.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { BranchAdminList } from "./branch-admin-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  push: vi.fn(),
  status: undefined as string | undefined,
  result: {} as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/branches",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/organization-admin-queries", () => ({
  useAdminBranches: (status: string | undefined) => {
    mocks.status = status;
    return mocks.result;
  },
}));

const rows = [
  { id: "b1", code: "HQ", name: "สำนักงานใหญ่", taxBranchCode: "00000", isActive: true, rowVersion: "v1" },
  { id: "b2", code: "CM", name: "เชียงใหม่", taxBranchCode: null, isActive: false, rowVersion: "v2" },
];

describe("BranchAdminList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["branches.manage"];
    mocks.status = undefined;
    mocks.result = { data: rows, isLoading: false, isError: false, refetch: vi.fn() };
  });

  it("lists active and inactive branches with tax branch code and status", () => {
    render(<BranchAdminList />);

    expect(screen.getByText("สำนักงานใหญ่")).toBeInTheDocument();
    expect(screen.getByText("00000")).toBeInTheDocument();
    expect(screen.getByText("ใช้งาน")).toBeInTheDocument();
    expect(screen.getByText("ปิดใช้งาน")).toBeInTheDocument();
    expect(screen.getAllByText("-").length).toBeGreaterThan(0); // missing tax branch code is shown as "-", never guessed
  });

  it("links each branch name to its detail page", () => {
    render(<BranchAdminList />);

    expect(screen.getByRole("link", { name: /เชียงใหม่/ })).toHaveAttribute("href", "/th/settings/branches/b2");
  });

  it("routes the add button to the create form", () => {
    render(<BranchAdminList />);
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));
    expect(mocks.push).toHaveBeenCalledWith("/th/settings/branches/create");
  });

  it("hides the add button without branches.manage", () => {
    mocks.permissions = [];
    render(<BranchAdminList />);

    expect(screen.queryByRole("button", { name: "เพิ่มสาขา" })).not.toBeInTheDocument();
  });

  it("shows an empty state when there are no branches", () => {
    mocks.result = { data: [], isLoading: false, isError: false, refetch: vi.fn() };
    render(<BranchAdminList />);

    expect(screen.getByText("ยังไม่มีสาขา")).toBeInTheDocument();
  });
});
```

Run: `cd frontend && npx vitest run src/features/settings/organization/components/branch-admin-list.test.tsx` → Expected: FAIL.

- [ ] **Step 2: component**

สร้าง `branch-admin-list.tsx` (pattern ของ `user-admin-list.tsx`; ข้อมูลเป็น array ขนาดจำกัดต่อองค์กร จึงไม่มี pagination ฝั่ง Backend):

```tsx
"use client";

import React, { useMemo } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/Button";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { EmptyState } from "@/components/ui/EmptyState";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord } from "@/hooks/useListState";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminBranches } from "../api/organization-admin-queries";

interface BranchFilters extends ListFilterRecord {
  status?: string;
}

const STATUS_VALUES = ["active", "inactive"] as const;
type BranchStatus = (typeof STATUS_VALUES)[number];
const isBranchStatus = (value: string | undefined): value is BranchStatus => STATUS_VALUES.some((s) => s === value);

export function BranchAdminList() {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BRANCHES_MANAGE);

  const listState = useListState<BranchFilters>({
    schema: { defaultSort: "code", defaultOrder: "asc", single: ["status"], allowedSorts: ["code", "name"] },
    debounceMs: 350,
  });
  const status = isBranchStatus(listState.params.filters.status) ? listState.params.filters.status : undefined;
  const { data, isLoading, isError, refetch } = useAdminBranches(status);
  const branches = data ?? [];

  const columns = useMemo<Column<AdminBranchResponse>[]>(
    () => [
      {
        id: "name",
        header: t("branches.table.name"),
        className: "min-w-[240px]",
        cell: (_v, row) => <TableEntityCell title={row.name} code={row.code} href={`/${locale}/settings/branches/${row.id}`} />,
      },
      { id: "taxBranchCode", header: t("branches.table.taxBranchCode"), cell: (_v, row) => row.taxBranchCode ?? "-" },
      {
        id: "status",
        header: t("branches.table.status"),
        cell: (_v, row) => (
          <StatusBadge
            label={row.isActive ? t("branches.status.active") : t("branches.status.inactive")}
            variant={row.isActive ? "success" : "neutral"}
          />
        ),
      },
    ],
    [locale, t]
  );

  const goCreate = () => router.push(`/${locale}/settings/branches/create`);

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("branches.title")}
        subtitle={t("branches.subtitle")}
        actions={
          canManage ? (
            <Button variant="primary" size="md" icon={<IconPlus size={16} />} onClick={goCreate}>
              {t("branches.createBranch")}
            </Button>
          ) : undefined
        }
      />
      <ListToolbar>
        <ListFilterSelect
          id="branch-admin-status"
          label={t("branches.statusFilter")}
          value={status ?? ""}
          onChange={(value) => listState.actions.setFilter("status", value || undefined)}
          options={STATUS_VALUES.map((value) => ({ value, label: t(`branches.status.${value}`) }))}
        />
      </ListToolbar>
      {!isLoading && !isError && branches.length === 0 ? (
        <EmptyState
          icon="empty"
          title={t("branches.emptyTitle")}
          description={t("branches.emptyDescription")}
          actionLabel={canManage ? t("branches.createBranch") : undefined}
          onAction={canManage ? goCreate : undefined}
        />
      ) : (
        <DataTable<AdminBranchResponse>
          columns={columns}
          data={branches}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
        />
      )}
    </div>
  );
}
```

หมายเหตุ: `DataTable` ไม่มี `onRowClick` และ `pagination` เป็น optional (`DataTable.tsx:43`) จึงเปิดสาขาผ่านลิงก์ใน `TableEntityCell` (เหมือน `user-admin-list.tsx:80-85`); Backend คืนรายการเรียงตาม `code` อยู่แล้ว จึงไม่ส่ง sort ไปเซิร์ฟเวอร์ และไม่มี pagination (จำนวนสาขาต่อองค์กรจำกัด; ถ้าเกิน ~200 ให้เปิด slice เพิ่ม paging).

- [ ] **Step 3: route**

สร้าง `frontend/src/app/[locale]/(erp)/settings/branches/page.tsx` (รูปแบบเดียวกับ Task 12 แต่ `permission={PERMISSIONS.BRANCHES_MANAGE}` และ render `<BranchAdminList />`).

- [ ] **Step 4: Run — ต้องผ่าน**

Run: `cd frontend && npx vitest run src/features/settings/organization/components/branch-admin-list.test.tsx && npm run typecheck && npm run lint` → Expected: PASS, 0 errors.

- [ ] **Step 5: Commit**

```bash
git add frontend/src/features/settings/organization/components/branch-admin-list.tsx frontend/src/features/settings/organization/components/branch-admin-list.test.tsx "frontend/src/app/[locale]/(erp)/settings/branches/page.tsx"
git commit -m "feat(org): add branch administration list page

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 14: Frontend — แก้/สร้างสาขา (route `[id]`), ปิด/เปิดใช้งาน และเมนู

กฎ AGENTS.md: หน้าเดียว `[id]` รองรับ create/edit (`id` = `create` หรือ `add`); ฟอร์มกลุ่มเดียวจึงไม่ใช้ `FormTabs`; ปุ่มบันทึกมี `isLoading` และ disable ทันที; ปิดสาขาต้องผ่าน `ConfirmationModal` ที่มี `isLoading` ล็อกปุ่มยืนยัน; ขณะโหลดข้อมูลใช้ `MonoSpinner` (ไม่ใช้ skeleton). `ConfirmationModal` มีแค่ `message: string` (ไม่มีช่องกรอก) จึงเก็บ "เหตุผล" ใน Textarea บนหน้า แล้วเปิด modal ยืนยัน — ไม่แก้ shared component.

**Files:**
- Create: `frontend/src/features/settings/organization/components/branch-admin-editor.tsx` (+ `.test.tsx`)
- Create: `frontend/src/features/settings/organization/components/branch-activation-panel.tsx` (+ `.test.tsx`)
- Create: `frontend/src/app/[locale]/(erp)/settings/branches/[id]/page.tsx`
- Modify: `frontend/src/components/layout/SidebarNav.tsx`, `frontend/src/components/layout/SidebarNav.test.tsx`

- [ ] **Step 1: เขียน failing test ของ panel (ปิด/เปิด)**

สร้าง `branch-activation-panel.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { BranchActivationPanel } from "./branch-activation-panel";

const mocks = vi.hoisted(() => ({
  setActive: vi.fn(),
  success: vi.fn(),
  isPending: false,
  check: { data: { canDeactivate: true, blockers: [] as { type: string; count: number }[] } } as Record<string, unknown>,
}));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/organization-admin-queries", () => ({
  useBranchDeactivationCheck: () => mocks.check,
  useSetAdminBranchActive: () => ({ mutateAsync: mocks.setActive, isPending: mocks.isPending }),
}));

const active: AdminBranchResponse = {
  id: "b1", code: "HQ", name: "สำนักงานใหญ่", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null,
  phone: null, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-10T00:00:00Z",
};

describe("BranchActivationPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.isPending = false;
    mocks.check = { data: { canDeactivate: true, blockers: [] } };
    mocks.setActive.mockResolvedValue({});
  });

  it("requires a reason before the confirmation modal can be opened, then deactivates with the row version", async () => {
    render(<BranchActivationPanel branch={active} canManage />);
    const open = screen.getByRole("button", { name: "ปิดใช้งานสาขา" });
    expect(open).toBeDisabled();

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "  ย้ายที่ตั้ง  " } });
    fireEvent.click(open);
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ branchId: "b1", active: false, reason: "ย้ายที่ตั้ง", ifMatch: "v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ปิดใช้งานสาขาแล้ว"));
  });

  it("lists blockers from the server and keeps deactivation disabled", () => {
    mocks.check = { data: { canDeactivate: false, blockers: [{ type: "estimates", count: 3 }, { type: "last_active_branch", count: 1 }] } };
    render(<BranchActivationPanel branch={active} canManage />);

    expect(screen.getByText("ยังปิดสาขานี้ไม่ได้")).toBeInTheDocument();
    expect(screen.getByText(/ใบประเมินราคาที่ยังไม่สิ้นสุด/)).toHaveTextContent("3");
    expect(screen.getByText(/องค์กรต้องมีสาขาที่เปิดใช้อย่างน้อยหนึ่งสาขา/)).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    expect(screen.getByRole("button", { name: "ปิดใช้งานสาขา" })).toBeDisabled();
  });

  it("shows the server error when the guard rejects at confirm time", async () => {
    mocks.setActive.mockRejectedValueOnce(new ApiError({ status: 409, code: "BRANCH_HAS_OPEN_DOCUMENTS", message: "x" }));
    render(<BranchActivationPanel branch={active} canManage />);

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานสาขา" }));
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    expect(await screen.findByText("สาขานี้ยังมีเอกสารหรือคลังที่ใช้งานอยู่")).toBeInTheDocument();
  });

  it("locks the confirm button while the request is in flight", async () => {
    mocks.isPending = true;
    render(<BranchActivationPanel branch={active} canManage />);

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานสาขา" }));

    expect(await screen.findByRole("button", { name: "ปิดใช้งาน" })).toBeDisabled();
  });

  it("offers activation for an inactive branch and nothing without permission", async () => {
    const { rerender } = render(<BranchActivationPanel branch={{ ...active, isActive: false }} canManage />);
    fireEvent.click(screen.getByRole("button", { name: "เปิดใช้งานสาขา" }));
    fireEvent.click(await screen.findByRole("button", { name: "เปิดใช้งาน" }));
    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ branchId: "b1", active: true, ifMatch: "v1" }));

    rerender(<BranchActivationPanel branch={active} canManage={false} />);
    expect(screen.queryByRole("button", { name: "ปิดใช้งานสาขา" })).not.toBeInTheDocument();
  });
});
```

Run: `cd frontend && npx vitest run src/features/settings/organization/components/branch-activation-panel.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: panel component**

สร้าง `branch-activation-panel.tsx`:

```tsx
"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Textarea } from "@/components/ui/Textarea";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useBranchDeactivationCheck, useSetAdminBranchActive } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";

const MAX_REASON_LENGTH = 500;

interface BranchActivationPanelProps {
  branch: AdminBranchResponse;
  canManage: boolean;
}

export function BranchActivationPanel({ branch, canManage }: BranchActivationPanelProps) {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const [reason, setReason] = useState("");
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const checkQuery = useBranchDeactivationCheck(branch.id, canManage && branch.isActive);
  const setActive = useSetAdminBranchActive();

  if (!canManage) return null;

  const blockers = checkQuery.data?.blockers ?? [];
  const trimmedReason = reason.trim();
  const canOpenDeactivate = branch.isActive && blockers.length === 0 && trimmedReason !== "";

  const confirm = async () => {
    setErrorMessage(null);
    try {
      if (branch.isActive) {
        await setActive.mutateAsync({ branchId: branch.id, active: false, reason: trimmedReason, ifMatch: branch.rowVersion });
        toast.success(t("branches.deactivate.success"));
        setReason("");
      } else {
        await setActive.mutateAsync({ branchId: branch.id, active: true, ifMatch: branch.rowVersion });
        toast.success(t("branches.activate.success"));
      }
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${organizationAdminErrorKey(error)}`));
    } finally {
      setConfirmOpen(false);
    }
  };

  const title = branch.isActive ? t("branches.deactivate.title") : t("branches.activate.title");
  const message = branch.isActive ? t("branches.deactivate.message") : t("branches.activate.message");

  return (
    <FormSection title={title}>
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      {branch.isActive && blockers.length > 0 ? (
        <Alert variant="warning">
          <p className="font-medium">{t("branches.deactivate.blockedTitle")}</p>
          <p>{t("branches.deactivate.blockedIntro")}</p>
          <ul className="list-disc pl-5">
            {blockers.map((blocker) => (
              <li key={blocker.type}>
                {t(`branches.blockers.${blocker.type}`)}
                {blocker.type === "last_active_branch" ? "" : ` (${blocker.count})`}
              </li>
            ))}
          </ul>
        </Alert>
      ) : null}
      {branch.isActive ? (
        <Textarea
          label={t("branches.deactivate.reasonLabel")}
          rows={2}
          maxLength={MAX_REASON_LENGTH}
          value={reason}
          onChange={(event) => setReason(event.target.value)}
        />
      ) : null}
      <Button
        variant={branch.isActive ? "danger" : "primary"}
        size="md"
        disabled={branch.isActive ? !canOpenDeactivate : false}
        onClick={() => setConfirmOpen(true)}
      >
        {branch.isActive ? t("branches.deactivate.action") : t("branches.activate.action")}
      </Button>
      <ConfirmationModal
        isOpen={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        onConfirm={confirm}
        title={title}
        message={message}
        confirmText={branch.isActive ? t("branches.deactivate.confirm") : t("branches.activate.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant={branch.isActive ? "danger" : "info"}
        isLoading={setActive.isPending}
      />
    </FormSection>
  );
}
```

หมายเหตุ: `t(\`branches.blockers.${blocker.type}\`)` ใช้ template key เหมือน `user-admin-list.tsx` (`status.${...}`); `blocker.type` เป็น `string` ตาม OpenAPI จึงมี test parity (Task 11) ว่าทุก blocker type มี label ทั้ง th/en.

Run: `cd frontend && npx vitest run src/features/settings/organization/components/branch-activation-panel.test.tsx` → Expected: PASS ทั้ง 5.

- [ ] **Step 3: เขียน failing test ของ editor**

สร้าง `branch-admin-editor.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { BranchAdminEditor } from "./branch-admin-editor";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  create: vi.fn(),
  update: vi.fn(),
  push: vi.fn(),
  success: vi.fn(),
  branch: { data: undefined, isPending: false, isError: false } as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/branches/create",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("./branch-activation-panel", () => ({ BranchActivationPanel: () => <div>activation-panel</div> }));
vi.mock("../api/organization-admin-queries", () => ({
  useAdminBranch: () => mocks.branch,
  useCreateAdminBranch: () => ({ mutateAsync: mocks.create, isPending: false }),
  useUpdateAdminBranch: () => ({ mutateAsync: mocks.update, isPending: false }),
}));

const existing = {
  id: "b1", code: "HQ", name: "สำนักงานใหญ่", nameEn: "Head Office", taxBranchCode: "00000", addressTh: null, addressEn: null,
  phone: null, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-10T00:00:00Z",
};

describe("BranchAdminEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["branches.manage"];
    mocks.branch = { data: existing, isPending: false, isError: false };
    mocks.create.mockResolvedValue({ id: "new-branch" });
    mocks.update.mockResolvedValue({});
  });

  it("validates code, name and tax branch code before creating", async () => {
    render(<BranchAdminEditor branchId="create" />);

    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "bad code" } });
    fireEvent.change(screen.getByLabelText(/เลขสาขาภาษี/), { target: { value: "12" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    expect(await screen.findByText("ใช้ได้เฉพาะตัวอักษรอังกฤษ ตัวเลข ขีดล่าง และขีดกลาง")).toBeInTheDocument();
    expect(screen.getByText("กรุณากรอกชื่อสาขา")).toBeInTheDocument();
    expect(screen.getByText("เลขสาขาภาษีต้องเป็นตัวเลข 5 หลัก")).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates with an idempotency key, null for blank optional fields, and opens the new branch", async () => {
    render(<BranchAdminEditor branchId="create" />);

    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "B2" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สาขา 2" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      code: "B2", name: "สาขา 2", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null, phone: null,
    });
    expect(call.idempotencyKey).toMatch(/[0-9a-f-]{36}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/branches/new-branch"));
  });

  it("reuses the idempotency key when the same create is retried after a failure", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 500, code: "INTERNAL_SERVER_ERROR", message: "x" }));
    render(<BranchAdminEditor branchId="create" />);
    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "B3" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สาขา 3" } });

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));
    await screen.findByText("ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง");
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(2));
    expect((mocks.create.mock.calls[1][0] as { idempotencyKey: string }).idempotencyKey)
      .toBe((mocks.create.mock.calls[0][0] as { idempotencyKey: string }).idempotencyKey);
  });

  it("shows the duplicate-code message from the server", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 409, code: "BRANCH_CODE_ALREADY_EXISTS", message: "x" }));
    render(<BranchAdminEditor branchId="add" />);
    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "HQ" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "x" } });

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    expect(await screen.findByText("มีสาขาที่ใช้รหัสนี้อยู่แล้ว")).toBeInTheDocument();
  });

  it("edits an existing branch: code is read-only and the save sends If-Match", async () => {
    render(<BranchAdminEditor branchId="b1" />);

    expect(screen.getByLabelText(/รหัสสาขา/)).toBeDisabled();
    expect(screen.getByLabelText(/รหัสสาขา/)).toHaveValue("HQ");
    expect(screen.getByText("activation-panel")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สำนักงานใหญ่ใหม่" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      branchId: "b1", ifMatch: "v1",
      payload: { name: "สำนักงานใหญ่ใหม่", nameEn: "Head Office", taxBranchCode: "00000", addressTh: null, addressEn: null, phone: null },
    });
  });

  it("shows not-found for an unknown branch id", () => {
    mocks.branch = { data: undefined, isPending: false, isError: true };
    render(<BranchAdminEditor branchId="missing" />);

    expect(screen.getByText("ไม่พบข้อมูลที่ต้องการ")).toBeInTheDocument();
  });
});
```

Run: `cd frontend && npx vitest run src/features/settings/organization/components/branch-admin-editor.test.tsx` → Expected: FAIL.

- [ ] **Step 4: editor component**

สร้าง `branch-admin-editor.tsx` (โครงเดียวกับ `user-admin-editor.tsx` + `organization-profile-form.tsx`):

```tsx
"use client";

import React, { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminBranch, useCreateAdminBranch, useUpdateAdminBranch } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";
import { BranchActivationPanel } from "./branch-activation-panel";

const CODE_PATTERN = /^[A-Za-z0-9_-]+$/;
const TAX_BRANCH_CODE_PATTERN = /^\d{5}$/;

interface BranchFormValues {
  code: string;
  name: string;
  nameEn: string;
  taxBranchCode: string;
  addressTh: string;
  addressEn: string;
  phone: string;
}

const EMPTY: BranchFormValues = { code: "", name: "", nameEn: "", taxBranchCode: "", addressTh: "", addressEn: "", phone: "" };
const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

function toFormValues(branch: AdminBranchResponse): BranchFormValues {
  return {
    code: branch.code,
    name: branch.name,
    nameEn: branch.nameEn ?? "",
    taxBranchCode: branch.taxBranchCode ?? "",
    addressTh: branch.addressTh ?? "",
    addressEn: branch.addressEn ?? "",
    phone: branch.phone ?? "",
  };
}

export function BranchAdminEditor({ branchId }: { branchId: string }) {
  const isCreate = branchId === "create" || branchId === "add";
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BRANCHES_MANAGE);
  const branchQuery = useAdminBranch(branchId, !isCreate);
  const createMutation = useCreateAdminBranch();
  const updateMutation = useUpdateAdminBranch();
  const [submitError, setSubmitError] = useState<string | null>(null);
  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  const schema = useMemo(
    () =>
      z.object({
        code: isCreate
          ? z.string().trim().min(1, t("branches.validation.codeRequired")).max(50).regex(CODE_PATTERN, t("branches.validation.codeFormat"))
          : z.string(),
        name: z.string().trim().min(1, t("branches.validation.nameRequired")).max(255),
        nameEn: z.string().trim().max(255),
        taxBranchCode: z.string().trim().refine((v) => v === "" || TAX_BRANCH_CODE_PATTERN.test(v), t("branches.validation.taxBranchCodeFormat")),
        addressTh: z.string().trim().max(500),
        addressEn: z.string().trim().max(500),
        phone: z.string().trim().max(30),
      }),
    [isCreate, t]
  );

  const branch = branchQuery.data;
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<BranchFormValues>({
    resolver: zodResolver(schema),
    defaultValues: EMPTY,
    values: !isCreate && branch ? toFormValues(branch) : undefined,
  });

  if (!isCreate && branchQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (!isCreate && (branchQuery.isError || !branch)) return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;

  const backHref = `/${locale}/settings/branches`;
  const isSaving = createMutation.isPending || updateMutation.isPending;

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const details = {
      name: values.name.trim(),
      nameEn: orNull(values.nameEn),
      taxBranchCode: orNull(values.taxBranchCode),
      addressTh: orNull(values.addressTh),
      addressEn: orNull(values.addressEn),
      phone: orNull(values.phone),
    };
    try {
      if (isCreate) {
        const payload = { code: values.code.trim(), ...details };
        const fingerprint = JSON.stringify(payload);
        if (intentRef.current?.fingerprint !== fingerprint) intentRef.current = { fingerprint, key: crypto.randomUUID() };
        const created = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
        intentRef.current = null;
        toast.success(t("branches.createSuccess"));
        router.push(`${backHref}/${created.id}`);
      } else if (branch) {
        await updateMutation.mutateAsync({ branchId: branch.id, payload: details, ifMatch: branch.rowVersion });
        toast.success(t("branches.saveSuccess"));
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${organizationAdminErrorKey(error)}`));
    }
  });

  return (
    <div className="space-y-6">
      <FormContainer
        asForm
        noValidate
        onSubmit={onSubmit}
        maxWidth="lg"
        header={
          <PageHeader
            title={isCreate ? t("branches.createTitle") : t("branches.editTitle")}
            subtitle={isCreate ? t("branches.createSubtitle") : branch?.name}
            backHref={backHref}
            backLabel={t("branches.backToList")}
          />
        }
        errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
        actionBar={
          canManage ? (
            <FormActionBar
              isDirty={isDirty}
              isLoading={isSaving}
              isSaveDisabled={isSaving}
              saveText={isCreate ? t("branches.createSubmit") : t("branches.saveSubmit")}
              saveButtonType="submit"
              cancelHref={backHref}
              showCancel
              cancelText={tCommon("actions.cancel")}
            />
          ) : undefined
        }
      >
        <FormSection title={t("branches.title")}>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input
              label={t("branches.fields.code")}
              required={isCreate}
              disabled={!isCreate}
              helperText={isCreate ? undefined : t("branches.createSubtitle")}
              error={errors.code?.message}
              {...register("code")}
            />
            <Input
              label={t("branches.fields.taxBranchCode")}
              helperText={t("branches.fields.taxBranchCodeHint")}
              inputMode="numeric"
              maxLength={5}
              disabled={!canManage}
              error={errors.taxBranchCode?.message}
              {...register("taxBranchCode")}
            />
            <Input label={t("branches.fields.name")} required disabled={!canManage} error={errors.name?.message} {...register("name")} />
            <Input label={t("branches.fields.nameEn")} disabled={!canManage} error={errors.nameEn?.message} {...register("nameEn")} />
            <Textarea label={t("branches.fields.addressTh")} rows={3} disabled={!canManage} error={errors.addressTh?.message} {...register("addressTh")} />
            <Textarea label={t("branches.fields.addressEn")} rows={3} disabled={!canManage} error={errors.addressEn?.message} {...register("addressEn")} />
            <Input label={t("branches.fields.phone")} disabled={!canManage} error={errors.phone?.message} {...register("phone")} />
          </div>
        </FormSection>
      </FormContainer>
      {!isCreate && branch ? <BranchActivationPanel branch={branch} canManage={canManage} /> : null}
    </div>
  );
}
```

หมายเหตุ: ช่อง `code` ในโหมดแก้ไขเป็น `disabled` จึงไม่ถูกส่งไปกับ `handleSubmit` เป็นค่าที่ผู้ใช้แก้ — และ payload ของ update ไม่มี `code` เลย (สัญญา API ก็ไม่รับ). Test "ชื่อสาขา (ไทย)" ใช้ regex เพราะ label มี `*` ของ required.

Run: `cd frontend && npx vitest run src/features/settings/organization && npm run typecheck && npm run lint` → Expected: PASS ทุกไฟล์, 0 errors. 

- [ ] **Step 5: route `[id]`**

สร้าง `frontend/src/app/[locale]/(erp)/settings/branches/[id]/page.tsx`:

```tsx
"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { BranchAdminEditor } from "@/features/settings/organization/components/branch-admin-editor";

interface BranchAdministrationDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function BranchAdministrationDynamicPage({ params }: BranchAdministrationDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("organizationAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.BRANCHES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <BranchAdminEditor branchId={id} />
    </PermissionGuard>
  );
}
```

- [ ] **Step 6: เมนู Sidebar (test ก่อน)**

ใน `SidebarNav.test.tsx` เพิ่ม:

```tsx
  it("shows organization and branch settings only to members who can manage them", () => {
    const manager: CurrentUserResponse = {
      ...mockUser,
      memberships: [
        {
          ...mockUser.memberships[0],
          permissions: [
            ...mockUser.memberships[0].permissions,
            { key: "organizations.manage", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
            { key: "branches.manage", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" },
          ],
        },
      ],
    };

    const { unmount } = render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);
    expect(screen.queryByRole("link", { name: "ข้อมูลองค์กร" })).toBeNull();
    expect(screen.queryByRole("link", { name: "สาขา" })).toBeNull();
    unmount();

    render(<SidebarNav currentUser={manager} isCollapsed={false} />);
    expect(screen.getByRole("link", { name: "ข้อมูลองค์กร" })).toHaveAttribute("href", "/th/settings/organization");
    expect(screen.getByRole("link", { name: "สาขา" })).toHaveAttribute("href", "/th/settings/branches");
  });
```

Run: `cd frontend && npx vitest run src/components/layout/SidebarNav.test.tsx` → Expected: FAIL (ไม่มีลิงก์).

แก้ `SidebarNav.tsx`: เพิ่ม `IconMapPin` ใน import จาก `@/components/common/Icons`; ใต้ `hasRoleRequestApproval` (:75) เพิ่ม

```tsx
  const hasOrganizationsManage = can(activeMembership, "organizations.manage");
  const hasBranchesManage = can(activeMembership, "branches.manage");
```

ใต้ `isRoleRequestsActive` (:95):

```tsx
  const isOrganizationSettingsActive = pathname.startsWith(`/${locale}/settings/organization`);
  const isBranchSettingsActive = pathname.startsWith(`/${locale}/settings/branches`);
```

ใต้ `isRoleRequestsVisible` (:487):

```tsx
  const isOrganizationSettingsVisible =
    hasOrganizationsManage && (!trimmedSearch || tShell("organizationSettings").toLowerCase().includes(trimmedSearch));
  const isBranchSettingsVisible =
    hasBranchesManage && (!trimmedSearch || tShell("branchSettings").toLowerCase().includes(trimmedSearch));
```

และต่อจากบล็อก `isRoleRequestsVisible` ใน JSX (ก่อน "Empty search feedback"):

```tsx
          {isOrganizationSettingsVisible && (
            <li>
              {renderLink({
                id: "organization-settings",
                href: `/${locale}/settings/organization`,
                label: tShell("organizationSettings"),
                icon: <IconBuilding size={20} />,
                isActive: isOrganizationSettingsActive,
              })}
            </li>
          )}
          {isBranchSettingsVisible && (
            <li>
              {renderLink({
                id: "branch-settings",
                href: `/${locale}/settings/branches`,
                label: tShell("branchSettings"),
                icon: <IconMapPin size={20} />,
                isActive: isBranchSettingsActive,
              })}
            </li>
          )}
```

และตรวจ `hasAnyResults` (ตัวแปรที่ใช้แสดง "ไม่พบเมนู") ว่านับเมนูเหล่านี้ด้วย: `grep -n "hasAnyResults" frontend/src/components/layout/SidebarNav.tsx` แล้วเพิ่ม `|| isOrganizationSettingsVisible || isBranchSettingsVisible` ในนิพจน์ที่ประกอบมัน (ตรงกับที่ `isUserAdminVisible` ถูกใส่ไว้).

Run: `cd frontend && npx vitest run src/components/layout/SidebarNav.test.tsx && npm run typecheck && npm run lint` → Expected: PASS, 0 errors.

- [ ] **Step 7: Commit**

```bash
git add frontend/src/features/settings/organization/components "frontend/src/app/[locale]/(erp)/settings/branches" frontend/src/components/layout/SidebarNav.tsx frontend/src/components/layout/SidebarNav.test.tsx
git commit -m "feat(org): add branch editor, activation panel and settings menu

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 15: Verification, ตรวจในเบราว์เซอร์, เอกสารสถานะ และ gate ปิด G-03a

**Files:**
- Create: `docs/05-engineering/organization-administration-verification.md`
- Modify: `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/03-contracts/organization-administration-api-contract.md` (สถานะ → Implemented)

- [ ] **Step 1: th/en key parity ทั้งไฟล์**

Run (จาก `frontend/`):

```bash
python3 - <<'PY'
import json
def keys(v, p=""):
    return [p] if not isinstance(v, dict) else [k for c, x in v.items() for k in keys(x, f"{p}.{c}" if p else c)]
th = set(keys(json.load(open("src/messages/th.json", encoding="utf-8"))))
en = set(keys(json.load(open("src/messages/en.json", encoding="utf-8"))))
print("only th:", sorted(th - en)[:20]); print("only en:", sorted(en - th)[:20])
raise SystemExit(0 if th == en else 1)
PY
```
Expected: `only th: []`, `only en: []`, exit 0. (ถ้ามีความต่างที่มีมาก่อน G-03a ให้บันทึกในเอกสาร verification — อย่าแก้ key ที่ไม่เกี่ยวข้อง.)

- [ ] **Step 2: Gate ชุดเต็มของ G-03a**

Run แยกทีละคำสั่ง (Integration ใช้ `-m:1` ตามที่ใช้ใน G-02 เพื่อลดการชน Docker):

```bash
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Organization"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OrganizationAdministrationEndpointsTests|FullyQualifiedName~BranchDependencyInspectorCoverageTests|FullyQualifiedName~OpenApiContractTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~UsersEndpointsTests|FullyQualifiedName~NotificationEndpointsTests|FullyQualifiedName~EstimateEndpointsTests" -m:1
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
cd frontend && npm run check:api && npm run lint && npm run typecheck && npx vitest run src/features/settings src/components/layout src/lib && npm run build
```
Expected: ทุกคำสั่ง exit 0; จดจำนวน passed/failed ลงเอกสาร verification (ห้ามกล่าวว่า "ผ่าน" โดยไม่มีผลจริง). ถ้าชุด regression (Identity/Users/Notification/Estimate) ล้มเพราะ permission key ใหม่หรือคอลัมน์ใหม่ ให้แก้สาเหตุใน Task ที่เกี่ยวข้อง ไม่ใช่ปรับ test เดิมให้ผ่าน.

- [ ] **Step 3: ตรวจในเบราว์เซอร์จริง (jsdom ไม่พิสูจน์ layout/สี/responsive)**

เริ่ม dev server ตามคู่มือ `docs/05-engineering/foundation-login-runbook.md` (Backend Test env + `SeedTestData=true`, Frontend dev) แล้วใช้ Browser pane ตรวจ และบันทึกผลแบบ "ตรวจแล้ว / ไม่ได้ตรวจ" อย่างตรงไปตรงมา:
1. `/th/settings/organization`: แก้ชื่อ/เลขภาษี → บันทึก → toast; เลขภาษีผิด (เช่น `0105536000004`) → ข้อความ `ORGANIZATION_TAX_ID_INVALID` ภาษาไทย; สลับ `/en/` ข้อความเป็นอังกฤษ.
2. `/th/settings/branches`: รายการ, filter สถานะ, `create` → สร้างสาขา → เปิดหน้าแก้ไข → รหัสแก้ไม่ได้.
3. สาขาที่มีเอกสาร/ผู้ใช้ (`B01` ของ Test) → หน้าแก้ไขแสดงรายการ blocker และปุ่มปิดใช้งานถูก disable; สาขาใหม่ว่าง → กรอกเหตุผล → modal ยืนยัน → ปุ่มถูกล็อกระหว่างยิง → สถานะเปลี่ยน → เปิดใช้งานกลับ.
4. หน้าจอ 375px: ไม่มี horizontal scroll, target ≥ 44px, focus ring เห็นชัด; Tab ผ่านฟอร์มและ modal ได้ด้วยคีย์บอร์ด.
5. ผู้ใช้ที่มีเฉพาะ `organizations.read` (เช่น Viewer ที่สร้างผ่านหน้าผู้ใช้): เห็นฟอร์ม read-only; ไม่เห็นเมนูสาขา.

- [ ] **Step 4: เอกสาร verification**

สร้าง `docs/05-engineering/organization-administration-verification.md` (ภาษาไทย รูปแบบเดียวกับ `notification-foundation-verification.md`): หัวข้อ 1 สถานะ (Implemented G-03a วันที่จริง; focused tests + ผลเบราว์เซอร์ตามที่ตรวจจริง; **ยังไม่ผ่าน UAT/Production**), 2 ตารางคำสั่งที่รันจริงพร้อมจำนวน passed/failed จาก Step 2, 3 สิ่งที่ชุดทดสอบพิสูจน์ (ตาม test จริง: permission-before-existence → 403 เหมือนกันทุก id, other-org id → 404, unique constraint แยกรหัส code/tax code, replay/key reuse, ETag/If-Match 409/428, guard 4 ชนิด + audit ไม่มี PII, coverage test ของ entity ที่มี `BranchId`), 4 mutation check (ผลจาก Task 9 Step 11 ตามจริง), 5 ข้อจำกัดที่ยอมรับ: (ก) ช่องว่างแข่งขันกับการสร้างเอกสารใหม่ขณะปิดสาขา (ADR 0019), (ข) `permissions` ถูก seed เฉพาะ Test env — **Production ต้องมอบ `organizations.manage`/`branches.manage` ให้ Role จริงด้วยมือ/สคริปต์ bootstrap ก่อนใช้**, (ค) นิยาม "เอกสารเปิด" และกฎ membership เป็น TEST_ONLY รอ Business Owner, (ง) โลโก้เลื่อนไป G-04, หลาย Organization ต่อ deployment ยังเป็น Validation Question, (จ) ไม่มี pagination ของรายการสาขา.

- [ ] **Step 5: อัปเดตสถานะ**

- `docs/03-contracts/organization-administration-api-contract.md`: เปลี่ยน "สถานะ" เป็น Implemented (G-03a) และลิงก์ verification ใช้ได้จริงแล้ว.
- `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` แถว G-03 (:48): ใส่ `G-03a Implemented <วันที่> (focused tests; G-03b/G-03c pending); [แผน G-03](2026-10-10-g03-org-role-authority.md); [Verification](../../05-engineering/organization-administration-verification.md)` — แก้เฉพาะแถวนี้ และติ๊ก checkbox ของ "Branch CRUD" กับ "Organization profile" ใน section `### G-03` (เว้น "โลโก้ (G-01)" ให้เขียนต่อท้ายว่า "เลื่อนไป G-04").
- `docs/00-overview/implementation-roadmap.md` แถว Foundation (:29): แก้ประโยค "ยังไม่มี Organization/Branch CRUD" เป็นสถานะใหม่ (มี Organization profile + Branch CRUD, ยังไม่มีการสร้าง/แก้ Role และ Approval Authority matrix) พร้อมลิงก์ verification.
- Run: ตรวจลิงก์ภายในด้วยคำสั่งใน Task 1 Step 6 กับไฟล์ที่แก้ → ต้องไม่มี `MISSING`.

- [ ] **Step 6: Commit**

```bash
git add docs/05-engineering/organization-administration-verification.md docs/00-overview/implementation-roadmap.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/03-contracts/organization-administration-api-contract.md
git commit -m "docs(org): record G-03a verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Part 2: G-03b Role management (tasks to be appended)

## Part 3: G-03c Approval Authority matrix (tasks to be appended)
