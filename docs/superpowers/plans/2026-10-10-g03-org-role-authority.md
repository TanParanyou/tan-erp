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

### G-03b — Role management

#### Docs

| ไฟล์ | หน้าที่ |
| --- | --- |
| `docs/adr/0020-role-management-and-approval-permission-set.md` (ใหม่), `docs/adr/README.md` (แก้) | ADR: system role ด้วย flag, maker–checker เมื่อเพิ่ม grant ให้ Role ที่มีผู้ถือ, ปิด Role ต้องไม่มีผู้ถือ, ขยาย approval set, ความเสี่ยงคงเหลือ R1/R2 |
| `docs/03-contracts/role-management-api-contract.md` (ใหม่) | สัญญา API/ฟิลด์/validation/error/audit ของ G-03b |
| `docs/03-contracts/{permission-catalog,error-contract,identity-administration-api-contract}.md`, `docs/README.md`, `CONTEXT.md` (แก้) | คำอธิบาย `roles.manage`, 9 error code, approval set 6 ตัว, แผนที่เอกสาร, ศัพท์ System Role / Role Change Request |
| `docs/05-engineering/role-management-verification.md` (ใหม่), `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` (แก้) | หลักฐานการทดสอบ + สถานะ G-03b (ไม่แตะ G-03c) |

#### Backend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `backend/src/TanErp.Domain/IdentityAccess/{Role.cs (แก้), RoleValues.cs, RoleChangeRequest.cs}` | `IsSystem`, `RowVersion`, `UpdateDetails/Deactivate/Activate`, `RoleRules`, entity คำขอแก้ Role |
| `backend/src/TanErp.Application/IdentityAccess/Administration/AdministrationModels.cs` (แก้) | ขยาย `ApprovalPermissionKeys` เป็น 6 ตัว |
| `backend/src/TanErp.Application/IdentityAccess/RoleManagement/{RoleManagementModels,RoleManagementPolicy,IRoleManagementStore,RoleManagementHandler}.cs` | models, นโยบาย (shape/diff/needs-checker), port, handler (permission ก่อน existence) |
| `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/{IdentityAdministrationSupport.cs (ใหม่, Global Reuse), IdentityAdministrationStore.cs (แก้), RoleManagementStore.cs}` | helper ร่วม, store (EF Core, Serializable, audit, Idempotency, ETag) |
| `backend/src/TanErp.Infrastructure/Persistence/{AppDbContext.cs, TestOnlyDataSeeder.cs}`, `Configurations/{RoleConfiguration,RoleChangeRequestConfiguration}.cs`, `Migrations/*_AddRoleManagement*.cs` | `is_system`, `row_version`, ตาราง `role_change_requests`, seed `roles.manage` + system role |
| `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs`, `Controllers/AdminRolesController.cs`, `ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors{,.en}.resx`, `Program.cs` | endpoints `api/v1/admin/{permissions,role-definitions,role-change-requests}`, error th/en, DI |
| `contracts/openapi/tan-erp.v1.json` (regenerate) | snapshot |
| `backend/tests/TanErp.UnitTests/IdentityAccess/{RoleDomainTests,RoleChangeRequestTests,RoleManagementPolicyTests,ApprovalPermissionCoverageTests,AdministrationPolicyTests}.cs`, `Organization/PermissionCatalogParityTests.cs` | domain + policy + test ล็อก approval set |
| `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs` | endpoint tests ทั้งหมดของ G-03b |

#### Frontend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `frontend/src/generated/api/tan-erp.v1.ts` (regenerate), `frontend/src/lib/api/api-client.ts` (+ test) | types + methods |
| `frontend/src/features/settings/role-management/{role-management-errors.ts (+test), api/role-management-queries.ts}` | whitelist error → key, hooks + query keys |
| `frontend/src/features/settings/role-management/components/{role-definition-list,permission-grant-picker,role-definition-editor,role-activation-panel,role-change-request-queue}.tsx` (+ `.test.tsx`) | รายการ, picker สิทธิ์, editor, ปิด/เปิดใช้, คิวผู้ตรวจ |
| `frontend/src/app/[locale]/(erp)/settings/roles/{page.tsx,[id]/page.tsx}`, `settings/role-requests/page.tsx` (แก้) | routes |
| `frontend/src/components/layout/SidebarNav.tsx` (+ test), `frontend/src/messages/{th,en}.json` (แก้) | เมนู "บทบาทและสิทธิ์", namespace `roleAdmin` |

### G-03c

ตาราง File Structure ของ G-03c จะถูกเพิ่มใน Part 3 ด้านล่าง.

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

# Part 2: G-03b Role management

> G-03a merge แล้ว (PR #19). Part 2 ใช้รูปแบบเดียวกับ Part 1 (handler เดียว + store แยก + `SerializableTransactionRunner`) บน branch `feat/g03b-role-management`. ทุก Task คอมไพล์และผ่าน test ของตัวเองก่อน commit; ใช้ `--filter` ตามที่ระบุเท่านั้น (ห้ามรัน IntegrationTests ทั้งโปรเจกต์ ~12+ นาที).

> **เวลารัน:** `RoleManagementEndpointsTests` เริ่ม PostgreSQL container ใหม่ต่อ `[Fact]` (รูปแบบเดียวกับ test class อื่นของ repo) ประมาณ 10–20 วินาทีต่อเคส — ระหว่างพัฒนาให้ใช้ filter ระดับเมธอด เช่น `--filter "FullyQualifiedName~RoleManagementEndpointsTests.Create_"` แล้วรันทั้งคลาสครั้งเดียวก่อน commit (ตามคำสั่งที่ระบุในแต่ละ Task).

## Part 2 — Findings จากการอ่านโค้ดจริง (ตรวจแล้ว)

| หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- |
| `Role` ยังไม่รองรับการแก้ไข | `Domain/IdentityAccess/Role.cs:6-40` มีเฉพาะ constructor; ไม่มี `IsSystem`, ไม่มี `RowVersion`, ไม่มีเมธอดแก้ชื่อ/เปิด-ปิด. ตาราง `identity_access.roles` unique `ix_roles_organization_id_normalized_role_name` (`RoleConfiguration.cs:25`) | ต้อง migration เพิ่ม `is_system` + `row_version`; "Role ระบบ" ต้องเป็น **คอลัมน์ flag** ไม่ใช่การเช็คชื่อ (`backend/AGENTS.md` ข้อ 5 ห้ามตรวจชื่อ Role) |
| endpoint Role เดิม | `GET /api/v1/admin/roles` (`AdminUsersController.cs:175-186`) คือรายการ Role สำหรับ **มอบ** (permission `roles.assign`, เฉพาะ `IsActive`, ไม่มี pagination — `IdentityAdministrationStore.cs:78-101`) | ไม่แตะ/ไม่ reuse URL นี้; endpoint จัดการใช้ `api/v1/admin/role-definitions` และ catalog `api/v1/admin/permissions` |
| Role ที่ปิดใช้ = สิทธิ์หายทันที | `RequestAccessResolver.cs:42,127,180` กรอง `mr.Role!.IsActive`; `LoadPermissionsAsync` (`IdentityAdministrationStore.cs:621-627`), `WouldRemoveLastAdministratorAsync` (:647-668) กรองเหมือนกัน | ปิด Role ที่ยังมีผู้ถือ = ถอนสิทธิ์เงียบ ๆ จากทุกคน → ตัดสินใจ fail-closed (D4) |
| anti-escalation / approval set | `AdministrationPolicy.ApprovalPermissionKeys` = `estimates.approve`, `cost-records.approve`, `cost-records.publish` (`AdministrationModels.cs:25-30`); `Covers` (:43); `WouldRemoveLastAdministrator` (:53). test เดิม `AdministrationPolicyTests.cs:56-66` | ขยาย set (D5) + test ล็อกชุดค่า |
| maker–checker ต้นแบบ | `RoleAssignmentRequest` (`Domain/IdentityAccess/RoleAssignmentRequest.cs`), config + partial unique `ux_role_assignment_requests_pending` + check ผู้ตรวจ ≠ ผู้ขอ (`RoleAssignmentRequestConfiguration.cs:12-16,30-34`), การตัดสิน `IdentityAdministrationStore.cs:407-471`, การมอบ `:303-379` | สร้าง `RoleChangeRequest` รูปแบบเดียวกัน (D3); ใช้ enum เดิม `RoleRequestDecision` และค่าคงที่ `RoleAssignmentRequestStatus` |
| helper ที่ private | `LoadPermissionsAsync` :621, `WouldRemoveLastAdministratorAsync` :647 (ดึงแถว administrator), `AddAudit` :670, `EscapeLike` :689 ใน `IdentityAdministrationStore.cs`; `RunAsync` :486 ถูก extract ไป `SerializableTransactionRunner` แล้ว (G-03a) | Task 5 (Global Reuse) extract 4 helper ที่เหลือเป็น `IdentityAdministrationSupport` |
| รูปแบบ G-03a ที่ merge แล้ว | handler เดียว `OrganizationAdministrationHandler.cs:10-108` (permission ก่อน existence), store `Persistence/Organization/OrganizationAdministrationStore.cs` แต่ **namespace จริงคือ `TanErp.Infrastructure.Persistence.OrganizationAdministration`** (ต่างจากที่ Part 1 เขียน `...Persistence.Organization`), DI `Program.cs:92-107`, controller `AdminOrganizationController.cs` (`[Route("api/v1/admin")]`) | Part 2 วาง store ที่ `Persistence/IdentityAccess/RoleManagementStore.cs` (namespace `TanErp.Infrastructure.Persistence.IdentityAccess` — ไม่มีชื่อ `Organization` ชนกัน) |
| Seeder | `roles.manage` **ไม่ถูก seed** (`TestOnlyDataSeeder.cs:117-119,151-152` มี `organizations.*`, `roles.assign`, `roles.assign-approval`); Test Admin ได้ทุก permission ที่ seed (`:251-266`); `Test Admin B` `:318-336`; มีคีย์ `*.approve` อื่นที่ไม่อยู่ใน set: `boms.approve` :175, `pricing-templates.approve` :197, และ `quick-estimates.review` :201 | seed `roles.manage` + mark Test Admin เป็น system role; test coverage กวาดคีย์ `*.approve/publish/review` ใน seeder (D5) |
| FE/Docs มี `roles.manage` แล้ว | `frontend/src/lib/permissions/permissions.ts:15`, `docs/03-contracts/permission-catalog.md:15` ("จัด Role และ Permission") | ใช้คีย์เดิม; ปรับคำอธิบายในเอกสาร |
| ADR/Migration | ADR ล่าสุด `0019-organization-branch-administration.md` → ใหม่คือ **0020**; migration ล่าสุดเป็นของ G-03a (`AddOrganizationProfileAndBranchDetails`) | |
| OpenAPI snapshot | `OpenApiContractTests.cs:254` เขียน `contracts/openapi/tan-erp.v1.json` เมื่อตั้ง `UPDATE_OPENAPI=1`; FE type ด้วย `npm run generate:api` (`frontend/package.json:12-13`). Commit `8a8be98` เปลี่ยนไฟล์ snapshot + `tan-erp.v1.ts` ตรงกับกลไกนี้ | Task 11 ใช้ `UPDATE_OPENAPI=1` + `generate:api` (ไม่ใช้ live swagger) |

### ข้อที่ Part 1 / Findings เดิมคลาดเคลื่อน (แก้ในแผนนี้ ไม่แก้ Part 1)

1. namespace ของ G-03a store คือ `...Persistence.OrganizationAdministration` (Part 1 เขียน `...Persistence.Organization`).
2. Findings เดิมกล่าวถึง "Role ระบบแก้ไม่ได้" แต่ไม่ได้ระบุว่า `Role` **ไม่มีทั้ง flag ระบบและ `RowVersion`** (ต้อง migration) และ validation question 6 เสนอ "ชื่อขึ้นต้น `system.`" ซึ่งใช้เป็นกฎ **สงวนชื่อตอนสร้าง** ได้เท่านั้น ห้ามใช้ตัดสินสิทธิ์.
3. Findings เดิมนับ approval permission 6 ตัวแต่ไม่ได้ชี้ว่า `boms.approve`, `pricing-templates.approve`, `quick-estimates.review` ก็เป็นคีย์ลักษณะอนุมัติที่ยังนอก set.
4. ADR เลขถัดไปตอนนี้คือ 0020 (Part 1 เขียน 0019 สำหรับ G-03a — merge แล้ว).

## Part 2 — การตัดสินใจ (Decisions) ที่ผู้ใช้อาจ veto

| # | ตัดสินใจ | เหตุผล |
| --- | --- | --- |
| D1 | **System Role = คอลัมน์ `is_system`** (แก้/ปิด/เปิดไม่ได้ทุกกรณี). Backfill = ไม่มี Role ไหนเป็น system จาก migration (production ยังไม่มี seed Role); seeder (Test) mark `Test Admin`/`Test Admin B` เป็น system. API ไม่มีทางสร้าง/ตั้ง flag นี้ | ไม่ใช้ชื่อเป็นตัวตัดสิน; ชื่อขึ้นต้น `system.` ถูกสงวนไม่ให้สร้างเองเพื่อกันสับสน |
| D2 | **ขอบเขตการแก้:** ชื่อ, คำอธิบาย, ชุด grant (`organization`/`branch`/`own`). Anti-escalation (`Covers`) ตรวจ **เฉพาะ grant ที่เพิ่ม** (การถอนสิทธิ์ทำได้เสมอ); แก้ Role ที่ถืออยู่เองได้ (ไม่ใช่ `SELF_ROLE_CHANGE_FORBIDDEN` เพราะเพิ่มได้เฉพาะสิ่งที่ตนถืออยู่แล้ว). FE G-03b เลือก/ถอน grant ระดับ `organization` ได้; grant ระดับ `branch`/`own` ที่มีอยู่ถูก **คงไว้และส่งกลับตามเดิม** แต่เพิ่มใหม่ผ่าน UI ไม่ได้ (API เพิ่มได้) | ลดขนาด UI โดยไม่ทำให้ข้อมูลหาย |
| D3 | **Maker–checker เมื่อไร:** การแก้ (PUT) ที่ **เพิ่ม grant** ให้ Role ที่ (ก) มีผู้ถืออยู่ ≥ 1 แถวใน `membership_roles` และ (ข) Role หลังแก้ `RequiresApproval` → ไม่มีผลทันที: สร้าง `RoleChangeRequest` (202). การสร้าง Role ใหม่ และการแก้ Role ที่ไม่มีผู้ถือ มีผลทันที เพราะการ **มอบ** Role ที่ RequiresApproval ถูกคุมด้วย `RoleAssignmentRequest` (CP-02) อยู่แล้ว. การถอน grant ไม่ต้องมีผู้ตรวจ (ลดอำนาจ = fail-safe). ผู้ตัดสิน: `roles.assign-approval`, ≠ ผู้ขอ, ต้อง `Covers` grant ที่เพิ่ม, Role ต้องไม่เปลี่ยนจากตอนขอ (`ROLE_CHANGE_REQUEST_STALE`). ระหว่างมีคำขอค้าง แก้/ปิด Role ไม่ได้ (`ROLE_CHANGE_REQUEST_PENDING`). **ไม่มี notification ใน G-03b** (ผู้ตรวจเห็นที่คิว `settings/role-requests`) | ปิดช่องโหว่ "เพิ่ม approval permission ให้ Role ที่คนถืออยู่โดยไม่มีผู้ตรวจ" โดยไม่ทำซ้ำกฎที่มีอยู่ |
| D4 | **ปิด Role (deactivate):** ปิดไม่ได้ถ้ามีแถว `membership_roles` ใด ๆ (รวม Membership ที่ inactive) หรือมี `RoleAssignmentRequest` ที่ pending → `409 ROLE_IN_USE`; ปิดไม่ได้ถ้ามี `RoleChangeRequest` pending; Role ระบบปิดไม่ได้. ไม่มีการถอน Role ให้อัตโนมัติ. เปิดใช้ได้เสมอ (ไม่มีผู้ถือจึงไม่ขยายอำนาจ; การมอบยังต้องผ่าน CP-02) | fail-closed: ไม่ถอนสิทธิ์ผู้ใช้เงียบ ๆ และไม่ทำให้ระบบหลุด last-administrator; ผู้ดูแลต้องถอน Role ที่ `/settings/users` ก่อน (มี audit เดิม) |
| D5 | **ขยาย `ApprovalPermissionKeys`** เป็น 6 ตัว: เดิม 3 + `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve`. ผลต่อ CP-02: Role ที่มีคีย์เหล่านี้จะ **ต้อง maker–checker ตอนมอบ** (พฤติกรรมเดิมเปลี่ยน). `boms.approve`, `pricing-templates.approve`, `quick-estimates.review` **ไม่ใส่** (ไม่ใช่อำนาจอนุมัติเงิน/สัญญา — ต้องให้ Business Owner ยืนยัน) และถูกบันทึกในรายการ "reviewed-not-approval" ของ test. `roles.manage`/`roles.assign-approval` (meta-privilege) **ไม่ใส่** — เป็นความเสี่ยงคงเหลือ R1 ใน ADR 0020 (เพิ่มภายหลังได้ด้วยการแก้ set + test ล็อกบรรทัดเดียว) | ตามที่ผู้ใช้ขอ; test กวาด seeder ทำให้คีย์ `*.approve` ใหม่ต้องถูกตัดสินใจ ไม่หลุดเงียบ |
| D6 | **URL:** `GET /admin/permissions` (catalog, `roles.manage`), `GET|POST /admin/role-definitions`, `GET|PUT /admin/role-definitions/{id}`, `POST .../deactivate|activate`, `GET /admin/role-change-requests` (`roles.assign-approval`), `POST /admin/role-change-requests/{id}/approve|reject|cancel` (cancel = `roles.manage`, ผู้ขอเท่านั้น) | แยกจาก `GET /admin/roles` (assign picker) ไม่ให้สองความหมาย/สองสิทธิ์อยู่บน URL เดียว |
| D7 | **Pagination/search:** รายการ Role ใช้ `search` (ILIKE ชื่อ/คำอธิบาย, escape `%_\`), `status=active\|inactive`, `sortBy=name\|createdAt`, `sortOrder`, `page`, `limit` (≤100, tie-breaker `Id`), response `{ items, pagination }` ใช้ `AdminPaginationResponse` เดิม; sort/status ผิดคืน `400 REQUEST_VALIDATION_FAILED` (ตามที่ G-03a ทำกับ status) | ตามมาตรฐาน API skill ข้อ 3-4 |
| D8 | **Audit:** เก็บ id, ชื่อฟิลด์ที่เปลี่ยน, จำนวน และ **permission key** ที่เพิ่ม/ถอน (key เป็น catalog identifier ไม่ใช่ข้อมูลส่วนบุคคล) ห้ามเก็บชื่อ/คำอธิบายของ Role | audit การเปลี่ยนอำนาจต้องบอกได้ว่าอำนาจไหนเปลี่ยน |
| D9 | `RoleChangeRequest` เป็น entity แยก และ **ทำซ้ำ logic Try* ~20 บรรทัด** แทนการสร้าง base class ร่วมกับ `RoleAssignmentRequest` (เลี่ยงแตะ entity ของ CP-02); ทบทวนเมื่อมี maker–checker ตัวที่ 3 | minimal blast radius |

## Part 2 — สรุป API contract

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/permissions` | `roles.manage` | catalog `{ items: [{ key, description, isApprovalPermission }] }` |
| GET | `/api/v1/admin/role-definitions` | `roles.manage` | `search,status,sortBy,sortOrder,page,limit` → `{ items, pagination }` |
| GET | `/api/v1/admin/role-definitions/{id}` | `roles.manage` | รายละเอียด + grants + `ETag`; other-org id → 404 |
| POST | `/api/v1/admin/role-definitions` | `roles.manage` | `Idempotency-Key`; 201 + `ETag` |
| PUT | `/api/v1/admin/role-definitions/{id}` | `roles.manage` | `If-Match`; 200 (มีผลแล้ว) หรือ 202 (สร้างคำขอ) |
| POST | `/api/v1/admin/role-definitions/{id}/deactivate` / `activate` | `roles.manage` | `If-Match` |
| GET | `/api/v1/admin/role-change-requests?status=pending` | `roles.assign-approval` | คิวผู้ตรวจ พร้อม `addedPermissionKeys/removedPermissionKeys` |
| POST | `/api/v1/admin/role-change-requests/{id}/approve` / `reject` | `roles.assign-approval` | `If-Match` (rowVersion ของคำขอ) |
| POST | `/api/v1/admin/role-change-requests/{id}/cancel` | `roles.manage` | `If-Match`; ผู้ขอเท่านั้น |

Error ใหม่ 9 รหัส: `ROLE_NAME_ALREADY_EXISTS` 409, `ROLE_NAME_RESERVED` 422, `ROLE_PERMISSION_UNKNOWN` 422, `ROLE_SYSTEM_IMMUTABLE` 409, `ROLE_IN_USE` 409, `ROLE_CHANGE_REQUEST_PENDING` 409, `ROLE_CHANGE_REQUEST_NOT_PENDING` 409, `ROLE_CHANGE_REQUEST_STALE` 409, `ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED` 403. ใช้ซ้ำ: `ROLE_ESCALATION_DENIED`, `LAST_ADMINISTRATOR_REQUIRED`, `BRANCH_NOT_FOUND`, `ADMIN_VERSION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`, `REQUEST_VALIDATION_FAILED`, `RESOURCE_NOT_FOUND`, `PERMISSION_DENIED`.

Audit action: `roles.created`, `roles.updated`, `roles.change-requested`, `roles.change-approved`, `roles.change-rejected`, `roles.change-cancelled`, `roles.deactivated`, `roles.activated`.

## Part 2 — Task Map

| Task | เรื่อง | commit |
| --- | --- | --- |
| 1 | ADR 0020, API contract, catalog/error/CONTEXT (docs) | `docs(roles): add role management ADR and contract` |
| 2 | Domain: `Role` แก้ได้ + `is_system`, `RoleChangeRequest` | `feat(roles): add role editing rules and change request entity` |
| 3 | Application: models, `RoleManagementPolicy`, ขยาย `ApprovalPermissionKeys` + test ล็อก | `feat(roles): add role management policy and extend approval permission set` |
| 4 | EF config, migration, seed `roles.manage`/system role | `feat(roles): persist system flag, row version and role change requests` |
| 5 | **Global Reuse** extract `IdentityAdministrationSupport` | `refactor(identity): share administration query helpers` |
| 6 | API อ่าน: catalog, รายการ (search/pagination), รายละเอียด + test harness | `feat(roles): add role catalog, list and detail endpoints` |
| 7 | API สร้าง Role (Idempotency, anti-escalation) | `feat(roles): add role creation endpoint` |
| 8 | API แก้ Role (system immutable, grant diff, maker–checker, last-admin) | `feat(roles): add role update with maker-checker` |
| 9 | API ตัดสินคำขอแก้ Role | `feat(roles): add role change request decisions` |
| 10 | API ปิด/เปิด Role (`ROLE_IN_USE`) | `feat(roles): add role deactivation and activation` |
| 11 | OpenAPI snapshot, generated types, API client | `feat(roles): add role management api client` |
| 12 | FE foundation: messages th/en, error whitelist, hooks | `feat(roles): add role management translations, error map and query hooks` |
| 13 | FE: รายการ Role, route, เมนู | `feat(roles): add role list page and settings menu` |
| 14 | FE: editor + permission picker + activation panel | `feat(roles): add role editor, permission picker and activation panel` |
| 15 | FE: คิวคำขอแก้ Role | `feat(roles): add role change request queue` |
| 16 | Verification, เอกสารสถานะ (G-03b เท่านั้น) | `docs(roles): record G-03b verification and update status` |

---

## Task 1: ADR 0020, สัญญา API, permission catalog, error codes, ศัพท์ (docs)

**Files:**
- Create: `docs/adr/0020-role-management-and-approval-permission-set.md`, `docs/03-contracts/role-management-api-contract.md`
- Modify: `docs/adr/README.md`, `docs/03-contracts/permission-catalog.md`, `docs/03-contracts/error-contract.md`, `docs/03-contracts/identity-administration-api-contract.md`, `docs/README.md`, `CONTEXT.md`

ขั้นนี้เป็นเอกสาร ไม่มี test ที่รันได้ — ตรวจด้วย link check ใน Step 6.

- [ ] **Step 1: สร้าง ADR 0020**

เขียน `docs/adr/0020-role-management-and-approval-permission-set.md` (ภาษาไทย รูปแบบเดียวกับ ADR 0019):

```markdown
---
status: accepted
---

# Role Management: system role ด้วย flag, maker–checker เมื่อเพิ่มอำนาจอนุมัติให้ Role ที่มีผู้ถือ, ปิด Role ต้องไม่มีผู้ถือ, ขยาย Approval Permission Set

ก่อน G-03b Role ถูกสร้างได้เฉพาะจาก seed/DB และแก้ไม่ได้จากระบบ. G-03b เพิ่มการสร้าง/แก้/ปิดใช้ Role จาก Permission catalog ที่มีอยู่ โดยรักษา invariant ของ CP-02 (anti-escalation, maker–checker, last administrator).

**การตัดสินใจ 1 — System Role คือคอลัมน์ `is_system`:** Role ระบบแก้ชื่อ/คำอธิบาย/grant และปิด-เปิดไม่ได้ (`409 ROLE_SYSTEM_IMMUTABLE`) ไม่มี API ที่ตั้ง flag นี้ (seed/bootstrap เท่านั้น). ไม่ตรวจชื่อ Role เพื่อตัดสินสิทธิ์ (`backend/AGENTS.md` ข้อ 5); ชื่อขึ้นต้น `system.` ถูกสงวนตอนสร้างเพื่อไม่ให้สับสนเท่านั้น.

**การตัดสินใจ 2 — anti-escalation ตรวจเฉพาะ grant ที่เพิ่ม:** ผู้แก้ต้องถือ permission ของ grant ที่เพิ่มด้วย `AdministrationPolicy.Covers` เดิม (Scope เท่ากันหรือกว้างกว่า). การถอน grant ทำได้เสมอ. การลบ `users.manage` ออกจาก Role ที่เป็นแหล่งผู้ดูแลคนสุดท้ายถูกปฏิเสธด้วย `LAST_ADMINISTRATOR_REQUIRED` (ใช้ query ผู้ดูแลชุดเดียวกับ CP-02).

**การตัดสินใจ 3 — maker–checker ที่จุดที่ CP-02 ยังไม่คุม:** CP-02 คุมการ *มอบ* Role ที่มี approval permission. ช่องที่เหลือคือการ *เพิ่ม* approval permission ให้ Role ที่มีคนถืออยู่ ซึ่งจะข้ามประตูการมอบ. จึงกำหนดว่า การแก้ที่เพิ่ม grant ให้ Role ที่มีผู้ถือ ≥ 1 และ Role หลังแก้ `RequiresApproval` ต้องผ่าน `RoleChangeRequest` (ผู้ขอ ≠ ผู้ตัดสิน, ผู้ตัดสินถือ `roles.assign-approval` และ `Covers` grant ที่เพิ่ม, ตรวจ `row_version` ว่า Role ไม่เปลี่ยนตั้งแต่ขอ). การสร้าง Role ใหม่และการแก้ Role ที่ไม่มีผู้ถือมีผลทันทีเพราะการมอบยังผ่าน `RoleAssignmentRequest`. กฎ "เพิ่ม grant ใด ๆ" (ไม่เฉพาะ approval key) ให้ Role ที่ RequiresApproval และมีผู้ถือเป็นแบบเข้มงวดโดยตั้งใจ.

**การตัดสินใจ 4 — ปิด Role ต้องไม่มีผู้ถือ:** Role ที่ปิดใช้ถูกกรองออกจากการคำนวณสิทธิ์ทุกจุด (`RequestAccessResolver`) ดังนั้นการปิด Role ที่มีผู้ถือ = ถอนสิทธิ์เงียบ ๆ. ปิดได้เมื่อไม่มีแถว `membership_roles` ใด (รวม Membership inactive) ไม่มี `RoleAssignmentRequest` pending และไม่มี `RoleChangeRequest` pending (`ROLE_IN_USE` / `ROLE_CHANGE_REQUEST_PENDING`). ไม่ถอน Role ให้อัตโนมัติ; ผู้ดูแลถอนที่หน้าผู้ใช้ (มี audit เดิม).

**การตัดสินใจ 5 — ขยาย `ApprovalPermissionKeys` เป็น 6 ตัว:** `estimates.approve`, `cost-records.approve`, `cost-records.publish` (เดิม) + `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve`. ผลกระทบ: การมอบ Role ที่มีคีย์ใหม่ต้อง maker–checker (พฤติกรรม CP-02 เปลี่ยน). ไม่ใส่ `boms.approve`, `pricing-templates.approve`, `quick-estimates.review` (ไม่ใช่อำนาจอนุมัติเงิน/สัญญา) — test `ApprovalPermissionCoverageTests` กวาดคีย์ `*.approve/publish/review` ใน seeder แล้วบังคับให้ทุกคีย์อยู่ใน set หรือใน "reviewed-not-approval" อย่างชัดเจน.

**ความเสี่ยงคงเหลือ (R1):** `roles.manage` และ `roles.assign-approval` เป็น meta-privilege แต่ไม่อยู่ใน set จึงเพิ่มให้ Role ที่มีผู้ถือได้โดยไม่ต้องมีผู้ตรวจ (ผู้แก้ยังต้องถือสิทธิ์นั้นเองตาม anti-escalation). เพิ่มเข้า set ได้ภายหลังด้วยการแก้ set + test ล็อก.

**ความเสี่ยงคงเหลือ (R2):** ไม่มี notification แจ้งผู้ตรวจ (ผู้ตรวจต้องเปิดคิว `settings/role-requests`).

ข้อเสียที่ยอมรับ: `RoleChangeRequest` ทำซ้ำ logic `Try*` ของ `RoleAssignmentRequest` (~20 บรรทัด) เพื่อไม่แตะ entity ของ CP-02.
```

- [ ] **Step 2: ลิงก์ใน ADR README**

ต่อท้ายรายการใน `docs/adr/README.md`:

```markdown
- [0020 — Role Management และ Approval Permission Set](0020-role-management-and-approval-permission-set.md)
```

- [ ] **Step 3: สร้างสัญญา API**

เขียน `docs/03-contracts/role-management-api-contract.md` (ภาษาไทย รูปแบบเดียวกับ `organization-administration-api-contract.md`) ประกอบด้วย: (1) สถานะ "Draft → Implemented เมื่อ G-03b เสร็จ" + ลิงก์ ADR 0020 และ `../05-engineering/role-management-verification.md`; (2) ตาราง Endpoint — คัดลอกตาราง "Part 2 — สรุป API contract" ข้างบนทั้งตาราง; (3) ตาราง Fields และ validation ด้านล่าง; (4) รูป response (รายการ/รายละเอียด/`PUT` = `200` เมื่อมีผลแล้ว หรือ `202` เมื่อสร้างคำขอ) ตามชนิดใน Task 6-9; (5) กฎธุรกิจ 6 ข้อ = ADR 0020 การตัดสินใจ 1-5 + "ทุก endpoint จำกัดที่ Organization ของผู้เรียก (id ต่างองค์กร = 404, ไม่มีสิทธิ์ = 403 เหมือนกันทุก id)"; (6) รายการ Error และ Audit ตามที่ระบุใน "Part 2 — สรุป API contract".

```markdown
| Field | กฎ |
| --- | --- |
| `name` | trim, 3–100 ตัวอักษร, ไม่ซ้ำแบบ case-insensitive ในองค์กร (`NormalizedName`), ห้ามขึ้นต้น `system.` (case-insensitive) |
| `description` | optional, trim, ≤ 500, ว่าง = null |
| `permissions[]` | `{ permissionKey, scope, branchId? }`; ≤ 200; key ต้องอยู่ใน catalog และ active; `scope` ∈ `organization`, `branch`, `own`; `branchId` บังคับเมื่อ `branch` และห้ามส่งกับสองแบบที่เหลือ; รายการซ้ำถูกรวม |
| `rowVersion` | response; ค่าเดียวกับ `ETag` / `If-Match` |
```

- [ ] **Step 4: permission catalog, identity contract, error contract**

- `docs/03-contracts/permission-catalog.md` แถว `roles.manage` (บรรทัด 15) แทนคำอธิบายด้วย: `สร้าง/แก้/ปิดใช้ Role จาก Permission catalog, ดู catalog และรายการ Role ทั้งหมด (รวมที่ปิดใช้); ไม่รวมการมอบ Role (roles.assign)`.
- `docs/03-contracts/identity-administration-api-contract.md` กฎข้อ 5 (Maker–checker): แทนรายการ `estimates.approve`, `cost-records.approve` หรือ `cost-records.publish` ด้วยรายการ 6 ตัว `estimates.approve`, `cost-records.approve`, `cost-records.publish`, `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve` และต่อท้ายประโยคว่า "ชุดนี้ล็อกด้วย test และอธิบายใน [ADR 0020](../adr/0020-role-management-and-approval-permission-set.md)".
- `docs/03-contracts/error-contract.md` ต่อท้ายตาราง (ถัดจาก `BRANCH_LAST_ACTIVE`) ครบ 9 รหัสใหม่ ในรูปแบบ `| \`CODE\` | status | คำอธิบายภาษาไทย |`.

- [ ] **Step 5: README, CONTEXT**

- `docs/README.md` ต่อจากแถว `API จัดการ Organization/Branch` (บรรทัด 102): `| API จัดการ Role และ Permission | [Role Management API Contract](03-contracts/role-management-api-contract.md) |`.
- `CONTEXT.md` หมวด "การกำกับดูแล" ใต้ **Role** เพิ่ม **System Role (บทบาทระบบ)** = Role ที่ถูกตั้งธงโดย seed/bootstrap และแก้หรือปิดไม่ได้จากระบบ (_Avoid_: Built-in Role by name) และ **Role Change Request (คำขอแก้ไขบทบาท)** = คำขอที่รอผู้ตรวจอิสระอนุมัติเมื่อการแก้ Role เพิ่มสิทธิ์ให้ Role ที่ approval permission และมีผู้ถืออยู่ (_Avoid_: Role Assignment Request).

- [ ] **Step 6: ตรวจลิงก์**

Run: `for f in docs/03-contracts/role-management-api-contract.md docs/adr/0020-role-management-and-approval-permission-set.md; do grep -o '](\([^)#]*\)' $f | sed 's/.*](//' | sort -u | while read p; do d=$(dirname $f); [ -e "$d/$p" ] || echo "MISSING $f -> $p"; done; done`
Expected: มีเฉพาะ `MISSING ... ../05-engineering/role-management-verification.md` (สร้างใน Task 16 — ยอมรับชั่วคราว).

- [ ] **Step 7: Commit**

```bash
git add docs/adr/0020-role-management-and-approval-permission-set.md docs/adr/README.md docs/03-contracts/role-management-api-contract.md docs/03-contracts/permission-catalog.md docs/03-contracts/error-contract.md docs/03-contracts/identity-administration-api-contract.md docs/README.md CONTEXT.md
git commit -m "docs(roles): add role management ADR and contract

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: Domain — `Role` แก้ได้, system flag, `RoleChangeRequest` (TDD)

**Files:**
- Create: `backend/src/TanErp.Domain/IdentityAccess/RoleValues.cs`, `backend/src/TanErp.Domain/IdentityAccess/RoleChangeRequest.cs`
- Modify: `backend/src/TanErp.Domain/IdentityAccess/Role.cs`
- Create: `backend/tests/TanErp.UnitTests/IdentityAccess/RoleDomainTests.cs`, `backend/tests/TanErp.UnitTests/IdentityAccess/RoleChangeRequestTests.cs`

- [ ] **Step 1: เขียน failing test**

สร้าง `backend/tests/TanErp.UnitTests/IdentityAccess/RoleDomainTests.cs`:

```csharp
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleDomainTests
{
    private static readonly Guid OrgId = Guid.NewGuid();

    [Fact]
    public void Normalize_TrimsNameAndTurnsBlankDescriptionIntoNull()
    {
        var (name, description) = RoleRules.Normalize("  ผู้จัดการ  ", "   ");

        Assert.Equal("ผู้จัดการ", name);
        Assert.Null(description);
    }

    [Theory]
    [InlineData("ab", "REQUEST_VALIDATION_FAILED")]
    [InlineData(null, "REQUEST_VALIDATION_FAILED")]
    [InlineData("SYSTEM.Auditor", "ROLE_NAME_RESERVED")]
    public void Normalize_RejectsInvalidNames(string? name, string expectedCode) =>
        Assert.Equal(expectedCode, Assert.Throws<RoleDomainException>(() => RoleRules.Normalize(name, null)).Code);

    [Fact]
    public void Normalize_EnforcesLengthLimits()
    {
        Assert.Equal(100, RoleRules.Normalize(new string('a', 100), null).Name.Length);
        Assert.Throws<RoleDomainException>(() => RoleRules.Normalize(new string('a', 101), null));
        Assert.Throws<RoleDomainException>(() => RoleRules.Normalize("abc", new string('d', 501)));
    }

    [Fact]
    public void CreateCustom_UpdateDetails_Deactivate_Activate_BumpTheVersion()
    {
        var role = Role.CreateCustom(Guid.NewGuid(), OrgId, " Old Name ", null, DateTimeOffset.UtcNow);
        Assert.True(role.IsActive);
        Assert.False(role.IsSystem);
        var versions = new List<Guid> { role.RowVersion };

        role.UpdateDetails("New Name", "d");
        versions.Add(role.RowVersion);
        role.Deactivate();
        versions.Add(role.RowVersion);
        role.Activate();
        versions.Add(role.RowVersion);

        Assert.Equal("NEW NAME", role.NormalizedName);
        Assert.Equal("d", role.Description);
        Assert.True(role.IsActive);
        Assert.Equal(4, versions.Distinct().Count());
    }

    [Fact]
    public void SystemRole_RejectsEveryMutation()
    {
        var role = new Role(Guid.NewGuid(), OrgId, "Built In", "d", isActive: true);
        Assert.False(role.IsSystem); // the existing constructor stays non-system
        role.MarkAsSystemRole();

        foreach (var action in new Action[] { () => role.UpdateDetails("Other Name", null), role.Deactivate, role.Activate, role.EnsureMutable })
            Assert.Equal("ROLE_SYSTEM_IMMUTABLE", Assert.Throws<RoleDomainException>(action).Code);

        Assert.Equal("Built In", role.Name);
        Assert.True(role.IsActive);
    }
}
```

สร้าง `backend/tests/TanErp.UnitTests/IdentityAccess/RoleChangeRequestTests.cs`:

```csharp
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleChangeRequestTests
{
    private static readonly Guid Maker = Guid.NewGuid();
    private static readonly Guid Checker = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 11, 0, 0, 0, TimeSpan.Zero);

    private static RoleChangeRequest NewRequest() =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Maker, Now, Guid.NewGuid(), "Name", null, "[]");

    [Fact]
    public void DecidingRequiresAnIndependentUser_AndOnlyTheRequesterMayCancel()
    {
        var request = NewRequest();
        var before = request.RowVersion;

        Assert.False(request.TryApprove(Maker, Now));
        Assert.False(request.TryCancel(Checker, Now));
        Assert.True(request.IsPending);
        Assert.True(request.TryApprove(Checker, Now));

        Assert.Equal(RoleAssignmentRequestStatus.Approved, request.Status);
        Assert.Equal(Checker, request.DecidedByUserId);
        Assert.NotEqual(before, request.RowVersion);
        Assert.False(request.TryReject(Guid.NewGuid(), Now)); // already decided
    }

    [Fact]
    public void Reject_AndCancel_RecordTheirOwnStatus()
    {
        var rejected = NewRequest();
        var cancelled = NewRequest();

        Assert.True(rejected.TryReject(Checker, Now));
        Assert.True(cancelled.TryCancel(Maker, Now));

        Assert.Equal(RoleAssignmentRequestStatus.Rejected, rejected.Status);
        Assert.Equal(RoleAssignmentRequestStatus.Cancelled, cancelled.Status);
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม (compile error)**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~RoleDomainTests|FullyQualifiedName~RoleChangeRequestTests"`
Expected: build FAIL — `RoleRules`, `RoleDomainException`, `Role.CreateCustom`, `RoleChangeRequest` ไม่มี.

- [ ] **Step 3: สร้าง value types**

สร้าง `backend/src/TanErp.Domain/IdentityAccess/RoleValues.cs`:

```csharp
namespace TanErp.Domain.IdentityAccess;

public sealed class RoleDomainException : Exception
{
    public RoleDomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

public static class RoleRules
{
    public const int NameMin = 3;
    public const int NameMax = 100;
    public const int DescriptionMax = 500;
    public const string ReservedPrefix = "system.";

    /// <summary>
    /// Trims and validates a custom role name and description. The "system." prefix is reserved for built-in roles;
    /// this is a naming rule only — authorization never inspects role names.
    /// </summary>
    public static (string Name, string? Description) Normalize(string? name, string? description)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMin or > NameMax)
            throw new RoleDomainException("REQUEST_VALIDATION_FAILED", $"Role name must be {NameMin}-{NameMax} characters.");
        if (trimmedName.StartsWith(ReservedPrefix, StringComparison.OrdinalIgnoreCase))
            throw new RoleDomainException("ROLE_NAME_RESERVED", "The 'system.' prefix is reserved for built-in roles.");

        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription is { Length: > DescriptionMax })
            throw new RoleDomainException("REQUEST_VALIDATION_FAILED", $"Description cannot exceed {DescriptionMax} characters.");

        return (trimmedName, trimmedDescription);
    }
}
```

- [ ] **Step 4: แก้ `Role.cs`**

เพิ่ม property ใต้ `CreatedAtUtc`:

```csharp
    public bool IsSystem { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();
```

เพิ่มพารามิเตอร์ท้าย constructor เดิม (ลายเซ็นเดิมยังใช้ได้) และกำหนดค่า:

```csharp
    public Role(Guid id, Guid organizationId, string name, string? description = null, bool isActive = true, DateTimeOffset? createdAtUtc = null, bool isSystem = false)
```
ในตัว constructor เพิ่มบรรทัดหลัง `IsActive = isActive;`: `IsSystem = isSystem;`.

เพิ่มเมธอดท้ายคลาส:

```csharp
    public static Role CreateCustom(Guid id, Guid organizationId, string? name, string? description, DateTimeOffset createdAtUtc)
    {
        var (normalizedName, normalizedDescription) = RoleRules.Normalize(name, description);
        return new Role(id, organizationId, normalizedName, normalizedDescription, isActive: true, createdAtUtc);
    }

    /// <summary>Seed/bootstrap only: no API marks a role as a system role.</summary>
    public void MarkAsSystemRole()
    {
        IsSystem = true;
        RowVersion = Guid.NewGuid();
    }

    public void EnsureMutable()
    {
        if (IsSystem) throw new RoleDomainException("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
    }

    public void UpdateDetails(string? name, string? description)
    {
        EnsureMutable();
        var (normalizedName, normalizedDescription) = RoleRules.Normalize(name, description);
        Name = normalizedName;
        NormalizedName = normalizedName.ToUpperInvariant();
        Description = normalizedDescription;
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate()
    {
        EnsureMutable();
        IsActive = false;
        RowVersion = Guid.NewGuid();
    }

    public void Activate()
    {
        EnsureMutable();
        IsActive = true;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Call after the role's permission grants change so concurrent editors are detected.</summary>
    public void AdvanceVersion() => RowVersion = Guid.NewGuid();
```

- [ ] **Step 5: สร้าง `RoleChangeRequest`**

สร้าง `backend/src/TanErp.Domain/IdentityAccess/RoleChangeRequest.cs` (ใช้ค่าคงที่สถานะของ `RoleAssignmentRequestStatus` ซ้ำ — ค่าเดียวกัน):

```csharp
using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

/// <summary>
/// A proposed definition of an existing Role that adds permissions to a Role people already hold and carries an
/// approval permission. An independent checker must apply it (maker-checker). Mirrors <see cref="RoleAssignmentRequest"/>.
/// </summary>
public class RoleChangeRequest : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; }
    public string Status { get; private set; } = RoleAssignmentRequestStatus.Pending;
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }

    /// <summary>The Role's row version when the request was made; approval is refused if the Role moved since.</summary>
    public Guid BaseRoleRowVersion { get; private set; }

    public string ProposedName { get; private set; } = string.Empty;
    public string? ProposedDescription { get; private set; }

    /// <summary>JSON array of { permissionKey, scope, branchId } — the complete proposed grant set.</summary>
    public string ProposedGrantsJson { get; private set; } = "[]";

    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected RoleChangeRequest() { }

    public RoleChangeRequest(
        Guid id, Guid organizationId, Guid roleId, Guid requestedByUserId, DateTimeOffset requestedAtUtc,
        Guid baseRoleRowVersion, string proposedName, string? proposedDescription, string proposedGrantsJson) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (roleId == Guid.Empty) throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (requestedByUserId == Guid.Empty) throw new ArgumentException("Requester cannot be empty.", nameof(requestedByUserId));
        if (string.IsNullOrWhiteSpace(proposedName)) throw new ArgumentException("Proposed name cannot be empty.", nameof(proposedName));

        OrganizationId = organizationId;
        RoleId = roleId;
        RequestedByUserId = requestedByUserId;
        RequestedAtUtc = requestedAtUtc;
        BaseRoleRowVersion = baseRoleRowVersion;
        ProposedName = proposedName;
        ProposedDescription = proposedDescription;
        ProposedGrantsJson = proposedGrantsJson;
    }

    public bool IsPending => Status == RoleAssignmentRequestStatus.Pending;

    public bool TryApprove(Guid deciderUserId, DateTimeOffset now) => TryDecide(RoleAssignmentRequestStatus.Approved, deciderUserId, now, requireIndependent: true);

    public bool TryReject(Guid deciderUserId, DateTimeOffset now) => TryDecide(RoleAssignmentRequestStatus.Rejected, deciderUserId, now, requireIndependent: true);

    /// <summary>Only the requester can withdraw a pending request.</summary>
    public bool TryCancel(Guid actorUserId, DateTimeOffset now)
    {
        if (!IsPending || actorUserId != RequestedByUserId) return false;
        return TryDecide(RoleAssignmentRequestStatus.Cancelled, actorUserId, now, requireIndependent: false);
    }

    private bool TryDecide(string status, Guid deciderUserId, DateTimeOffset now, bool requireIndependent)
    {
        if (!IsPending || deciderUserId == Guid.Empty) return false;
        if (requireIndependent && deciderUserId == RequestedByUserId) return false;

        Status = status;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now;
        RowVersion = Guid.NewGuid();
        return true;
    }
}
```

- [ ] **Step 6: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~RoleDomainTests|FullyQualifiedName~RoleChangeRequestTests|FullyQualifiedName~RolePermissionTests"` Expected: PASS ทุกเคส. แล้ว `dotnet build backend/TanErp.slnx --nologo -v q` ต้อง 0 error (seeder/test เดิมที่เรียก `new Role(...)` ยังคอมไพล์).

- [ ] **Step 7: Commit**

```bash
git add backend/src/TanErp.Domain/IdentityAccess backend/tests/TanErp.UnitTests/IdentityAccess/RoleDomainTests.cs backend/tests/TanErp.UnitTests/IdentityAccess/RoleChangeRequestTests.cs
git commit -m "feat(roles): add role editing rules and change request entity

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: Application — models, `RoleManagementPolicy`, ขยาย `ApprovalPermissionKeys` + test ล็อกชุดค่า (TDD)

**Files:**
- Create: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/RoleManagementModels.cs`, `backend/src/TanErp.Application/IdentityAccess/RoleManagement/RoleManagementPolicy.cs`
- Modify: `backend/src/TanErp.Application/IdentityAccess/Administration/AdministrationModels.cs`
- Modify: `backend/tests/TanErp.UnitTests/IdentityAccess/AdministrationPolicyTests.cs`
- Create: `backend/tests/TanErp.UnitTests/IdentityAccess/RoleManagementPolicyTests.cs`, `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalPermissionCoverageTests.cs`

- [ ] **Step 1: เขียน failing test (ล็อก approval set)**

ใน `AdministrationPolicyTests.cs` แก้ `[Theory] RequiresApproval_FlagsOnlyApprovalPermissions` โดยเพิ่ม InlineData ก่อน `[InlineData("estimates.read", false)]`:

```csharp
    [InlineData("purchase-orders.approve", true)]
    [InlineData("projects.change-orders.approve", true)]
    [InlineData("mrp.approve", true)]
    [InlineData("boms.approve", false)]
    [InlineData("roles.assign-approval", false)]
```

และเพิ่ม test (ท้ายคลาส):

```csharp
    [Fact]
    public void ApprovalPermissionKeys_AreExactlyTheReviewedSet()
    {
        // Changing this set changes who needs a checker (ADR 0020). Update the ADR and this list together.
        var expected = new[]
        {
            "cost-records.approve", "cost-records.publish", "estimates.approve",
            "mrp.approve", "projects.change-orders.approve", "purchase-orders.approve"
        };

        Assert.Equal(expected, AdministrationPolicy.ApprovalPermissionKeys.OrderBy(k => k, StringComparer.Ordinal).ToArray());
    }
```

สร้าง `backend/tests/TanErp.UnitTests/IdentityAccess/ApprovalPermissionCoverageTests.cs` (กวาดคีย์ใน seeder):

```csharp
using System.Text.RegularExpressions;
using TanErp.Application.IdentityAccess.Administration;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

/// <summary>Every approve/publish/review style key in the seeded catalog must be a conscious decision (ADR 0020).</summary>
public class ApprovalPermissionCoverageTests
{
    /// <summary>Reviewed and deliberately NOT treated as approval authority for maker–checker.</summary>
    private static readonly HashSet<string> ReviewedNotApproval = new(StringComparer.Ordinal)
    {
        "boms.approve",
        "pricing-templates.approve",
        "quick-estimates.review"
    };

    private static string RepoFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) dir = dir.Parent;
        return Path.Combine(new[] { dir!.FullName }.Concat(parts).ToArray());
    }

    [Fact]
    public void EverySeededApproveOrPublishKey_IsInTheApprovalSetOrExplicitlyReviewed()
    {
        var seeder = File.ReadAllText(RepoFile("backend", "src", "TanErp.Infrastructure", "Persistence", "TestOnlyDataSeeder.cs"));
        var keys = Regex.Matches(seeder, "\\(\"([a-z\\-\\.]+\\.(?:approve|publish|review))\"")
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();

        Assert.NotEmpty(keys);
        var undecided = keys
            .Where(k => !AdministrationPolicy.ApprovalPermissionKeys.Contains(k) && !ReviewedNotApproval.Contains(k))
            .ToArray();
        Assert.True(undecided.Length == 0,
            "Add these keys to AdministrationPolicy.ApprovalPermissionKeys or to ReviewedNotApproval (and ADR 0020): " + string.Join(", ", undecided));

        Assert.All(ReviewedNotApproval, k => Assert.Contains(k, keys));
    }
}
```

สร้าง `backend/tests/TanErp.UnitTests/IdentityAccess/RoleManagementPolicyTests.cs`:

```csharp
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleManagementPolicyTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();

    private static PermissionGrant Org(string key) => new(key, PermissionScope.Organization, OrgId);

    private static RoleInput Input(string name = "Sales Lead", params RoleGrantInput[] grants) => new(name, null, grants);

    private static RoleGrantInput G(string key, string scope = "organization", Guid? branchId = null) => new(key, scope, branchId);

    [Fact]
    public void ValidateShape_AcceptsAWellFormedRole()
    {
        Assert.Null(RoleManagementPolicy.ValidateShape(Input("Sales Lead", G("customers.read"), G("estimates.read", "branch", BranchId), G("quotations.read", "own"))));
    }

    [Fact]
    public void ValidateShape_RejectsBadNamesScopeMismatchesBlankKeysAndTooManyGrants()
    {
        Assert.Equal("ROLE_NAME_RESERVED", RoleManagementPolicy.ValidateShape(Input("system.root"))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("ab"))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "branch")))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "organization", BranchId)))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G("a.read", "project")))!.Code);
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", G(" ")))!.Code);
        var tooMany = Enumerable.Range(0, RoleManagementPolicy.MaxGrants + 1).Select(i => G($"k{i}.read")).ToArray();
        Assert.Equal("REQUEST_VALIDATION_FAILED", RoleManagementPolicy.ValidateShape(Input("Valid Name", tooMany))!.Code);
    }

    [Fact]
    public void Canonicalize_TrimsKeysDropsDuplicatesAndSortsForAStablePayloadHash()
    {
        var canonical = RoleManagementPolicy.Canonicalize(Input("Valid Name", G(" b.read "), G("a.read"), G("b.read")));

        Assert.Equal(new[] { "a.read", "b.read" }, canonical.Permissions.Select(p => p.PermissionKey).ToArray());
    }

    [Fact]
    public void ToGrant_UsesOrganizationIdBranchIdOrNullByScope()
    {
        Assert.Equal(new PermissionGrant("a.read", "organization", OrgId), RoleManagementPolicy.ToGrant(OrgId, "a.read", "organization", null));
        Assert.Equal(new PermissionGrant("a.read", "branch", BranchId), RoleManagementPolicy.ToGrant(OrgId, "a.read", "branch", BranchId));
        Assert.Equal(new PermissionGrant("a.read", "own", null), RoleManagementPolicy.ToGrant(OrgId, "a.read", "own", null));
    }

    [Fact]
    public void Diff_ReturnsAddedAndRemovedGrants()
    {
        var (added, removed) = RoleManagementPolicy.Diff(new[] { Org("a.read"), Org("b.read") }, new[] { Org("b.read"), Org("c.read") });

        Assert.Equal(new[] { Org("c.read") }, added);
        Assert.Equal(new[] { Org("a.read") }, removed);
    }

    [Theory]
    [InlineData(true, "estimates.approve", true)]   // holders + approval role + something added
    [InlineData(false, "estimates.approve", false)] // nobody holds it: the assignment gate (CP-02) applies later
    [InlineData(true, "customers.read", false)]     // role carries no approval permission
    public void NeedsChecker_OnlyWhenHeldRoleThatRequiresApprovalGainsGrants(bool hasHolders, string key, bool expected)
    {
        var added = new[] { Org(key) };

        Assert.Equal(expected, RoleManagementPolicy.NeedsChecker(hasHolders, new[] { key, "customers.read" }, added));
    }

    [Fact]
    public void NeedsChecker_WhenNothingIsAdded_IsFalseEvenForAHeldApprovalRole()
    {
        Assert.False(RoleManagementPolicy.NeedsChecker(true, new[] { "estimates.approve" }, Array.Empty<PermissionGrant>()));
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~AdministrationPolicyTests|FullyQualifiedName~ApprovalPermissionCoverageTests|FullyQualifiedName~RoleManagementPolicyTests"`
Expected: build FAIL (`RoleManagementPolicy` ไม่มี).

- [ ] **Step 3: models**

สร้าง `backend/src/TanErp.Application/IdentityAccess/RoleManagement/RoleManagementModels.cs`:

```csharp
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.IdentityAccess.RoleManagement;

public static class RoleManagementPermissions
{
    public const string RolesManage = "roles.manage";
    public const string RolesAssignApproval = AdministrationPermissions.RolesAssignApproval;
}

public static class RoleSortKey
{
    public const string Name = "name";
    public const string CreatedAt = "createdAt";

    public static bool IsValid(string value) => value is Name or CreatedAt;
}

public sealed record PermissionCatalogEntry(string Key, string? Description, bool IsApprovalPermission);

public sealed record RoleGrantInput(string PermissionKey, string Scope, Guid? BranchId);

public sealed record RoleGrantView(string PermissionKey, string Scope, AdminRef? Branch);

public sealed record RoleInput(string Name, string? Description, IReadOnlyList<RoleGrantInput> Permissions);

public sealed record RoleListQuery(string? Search, string? Status, string SortBy, bool Descending, int Page, int Limit);

public sealed record RoleDefinitionSummary(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion,
    int MemberCount, int PermissionCount, bool RequiresApproval, bool HasPendingChange);

public sealed record RoleDefinitionPage(IReadOnlyList<RoleDefinitionSummary> Items, int TotalCount, int Page, int PageSize);

public sealed record RoleChangeRequestView(
    Guid Id, Guid RowVersion, string Status, AdminRef Role, AdminRef RequestedBy, DateTimeOffset RequestedAtUtc,
    string ProposedName, string? ProposedDescription,
    IReadOnlyList<string> AddedPermissionKeys, IReadOnlyList<string> RemovedPermissionKeys);

public sealed record RoleDefinition(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc,
    int MemberCount, int PendingAssignmentRequestCount, bool RequiresApproval,
    IReadOnlyList<RoleGrantView> Permissions, RoleChangeRequestView? PendingChange);

public sealed record RoleUpdateOutcome(bool Pending, RoleDefinition Role, RoleChangeRequestView? Request);
```

- [ ] **Step 4: policy + ขยาย approval set**

สร้าง `backend/src/TanErp.Application/IdentityAccess/RoleManagement/RoleManagementPolicy.cs`:

```csharp
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.RoleManagement;

public static class RoleManagementPolicy
{
    public const int MaxGrants = 200;

    private static Error Invalid(string detail) => new("REQUEST_VALIDATION_FAILED", detail);

    /// <summary>Shape-only validation (name, scopes, limits). The store validates the catalog and branches.</summary>
    public static Error? ValidateShape(RoleInput input)
    {
        try
        {
            RoleRules.Normalize(input.Name, input.Description);
        }
        catch (RoleDomainException ex)
        {
            return new Error(ex.Code, ex.Message);
        }

        if (input.Permissions.Count > MaxGrants) return Invalid($"A role cannot have more than {MaxGrants} permissions.");

        foreach (var grant in input.Permissions)
        {
            if (string.IsNullOrWhiteSpace(grant.PermissionKey)) return Invalid("Permission key is required.");

            if (grant.Scope == PermissionScope.Branch)
            {
                if (grant.BranchId is null || grant.BranchId == Guid.Empty) return Invalid("Branch scope requires a branch.");
            }
            else if (grant.Scope == PermissionScope.Organization || grant.Scope == PermissionScope.Own)
            {
                if (grant.BranchId is not null) return Invalid("Only branch scope accepts a branch.");
            }
            else
            {
                return Invalid("Unsupported permission scope.");
            }
        }

        return null;
    }

    /// <summary>Trims keys, drops duplicates and sorts so equal requests hash equally.</summary>
    public static RoleInput Canonicalize(RoleInput input) => input with
    {
        Permissions = input.Permissions
            .Select(g => g with { PermissionKey = g.PermissionKey.Trim() })
            .Distinct()
            .OrderBy(g => g.PermissionKey, StringComparer.Ordinal)
            .ThenBy(g => g.Scope, StringComparer.Ordinal)
            .ThenBy(g => g.BranchId)
            .ToList()
    };

    public static PermissionGrant ToGrant(Guid organizationId, string key, string scope, Guid? branchId) =>
        new(key, scope, scope == PermissionScope.Organization ? organizationId : scope == PermissionScope.Branch ? branchId : null);

    public static (IReadOnlyList<PermissionGrant> Added, IReadOnlyList<PermissionGrant> Removed) Diff(
        IReadOnlyCollection<PermissionGrant> before, IReadOnlyCollection<PermissionGrant> after) =>
        (after.Except(before).ToList(), before.Except(after).ToList());

    /// <summary>
    /// ADR 0020 decision 3: a change that adds grants to a Role people already hold, where the resulting Role carries an
    /// approval permission, needs an independent checker. Unheld Roles are covered later by the assignment gate (CP-02).
    /// </summary>
    public static bool NeedsChecker(bool hasHolders, IEnumerable<string> resultingKeys, IReadOnlyCollection<PermissionGrant> added) =>
        hasHolders && added.Count > 0 && AdministrationPolicy.RequiresApproval(resultingKeys);
}
```

ใน `AdministrationModels.cs` แทน initializer ของ `ApprovalPermissionKeys` (บรรทัด 25-30) เป็น:

```csharp
    public static readonly IReadOnlySet<string> ApprovalPermissionKeys = new HashSet<string>(StringComparer.Ordinal)
    {
        "estimates.approve",
        "cost-records.approve",
        "cost-records.publish",
        "purchase-orders.approve",
        "projects.change-orders.approve",
        "mrp.approve"
    };
```
(คง doc-comment `<summary>` เดิมด้านบน และเพิ่มท้ายว่า `Locked by ApprovalPermissionCoverageTests; see ADR 0020.`)

- [ ] **Step 5: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~AdministrationPolicyTests|FullyQualifiedName~ApprovalPermissionCoverageTests|FullyQualifiedName~RoleManagementPolicyTests"` Expected: PASS ทั้งหมด. แล้ว `dotnet build backend/TanErp.slnx --nologo -v q` → 0 error.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Application/IdentityAccess backend/tests/TanErp.UnitTests/IdentityAccess
git commit -m "feat(roles): add role management policy and extend approval permission set

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 7: Mutation check ของ approval set (หลัง commit; ไม่ commit การทดลอง)**

ลบบรรทัด `"mrp.approve"` ออกจาก `ApprovalPermissionKeys` → รัน `--filter "FullyQualifiedName~ApprovalPermissionKeys_AreExactlyTheReviewedSet|FullyQualifiedName~EverySeededApproveOrPublishKey"` Expected: **FAIL ทั้งสอง**. คืนค่า: `git checkout -- backend/src/TanErp.Application/IdentityAccess/Administration/AdministrationModels.cs` แล้วรันซ้ำ Expected: PASS; `git status --short` ต้องว่าง.

---

## Task 4: EF configuration, migration และ seed (`roles.manage`, system role)

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/RoleConfiguration.cs`, `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`, `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/RoleChangeRequestConfiguration.cs`
- Generate: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddRoleManagement*.cs`
- Modify: `backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs`

- [ ] **Step 1: เขียน failing test**

ใน `PermissionCatalogParityTests.cs` เพิ่ม `[InlineData("roles.manage")]` ต่อจาก `[InlineData("branches.manage")]`.
Run: `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~PermissionCatalogParityTests"`
Expected: FAIL 1 เคส (`roles.manage` ไม่อยู่ใน seeder; docs และ FE มีแล้ว).

- [ ] **Step 2: seed**

ใน `TestOnlyDataSeeder.cs`:
1. ใน `permKeys` ต่อจากบรรทัด `("roles.assign-approval", "Approve Role Assignments"),` เพิ่ม `("roles.manage", "Manage Roles"),`.
2. หลังบล็อก `if (role == null) { ... }` ของ "Test Admin" (ก่อน `foreach (var perm in seededPerms)` แรก) เพิ่ม `if (!role.IsSystem) role.MarkAsSystemRole();`.
3. หลังบล็อก `if (roleB == null) { ... }` ของ "Test Admin B" เพิ่ม `if (!roleB.IsSystem) roleB.MarkAsSystemRole();`.

- [ ] **Step 3: EF configuration**

ใน `RoleConfiguration.cs` ต่อจาก `CreatedAtUtc` เพิ่ม:

```csharp
        builder.Property(x => x.IsSystem).HasColumnName("is_system").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
```

ใน `AppDbContext.cs` ใต้ `RoleAssignmentRequests` (บรรทัด 23) เพิ่ม: `public DbSet<RoleChangeRequest> RoleChangeRequests => Set<RoleChangeRequest>();`

สร้าง `RoleChangeRequestConfiguration.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class RoleChangeRequestConfiguration : IEntityTypeConfiguration<RoleChangeRequest>
{
    public void Configure(EntityTypeBuilder<RoleChangeRequest> builder)
    {
        builder.ToTable("role_change_requests", "identity_access", t =>
        {
            t.HasCheckConstraint("ck_role_change_requests_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
            t.HasCheckConstraint("ck_role_change_requests_independent_checker", "decided_by_user_id IS NULL OR status = 'cancelled' OR decided_by_user_id <> requested_by_user_id");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.BaseRoleRowVersion).HasColumnName("base_role_row_version").IsRequired();
        builder.Property(x => x.ProposedName).HasColumnName("proposed_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProposedDescription).HasColumnName("proposed_description").HasMaxLength(500);
        builder.Property(x => x.ProposedGrantsJson).HasColumnName("proposed_grants").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Status });
        builder.HasIndex(x => x.RoleId)
            .IsUnique()
            .HasFilter("status = 'pending'")
            .HasDatabaseName("ux_role_change_requests_pending");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

- [ ] **Step 4: Generate migration (ครั้งเดียว — ถ้าต้องแก้ ให้ลบแล้ว generate ใหม่)**

Run:
```bash
cd /Users/syaco/Documents/development/tan-erp && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddRoleManagement \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```
Expected: ไฟล์ `*_AddRoleManagement.cs` + `.Designer.cs`, snapshot เปลี่ยน. ตรวจว่า `AddColumn is_system` มี `defaultValue: false` และ `row_version` ของ `roles` มี `defaultValue: new Guid("00000000-0000-0000-0000-000000000000")` แล้วเพิ่มท้าย `Up`:

```csharp
            migrationBuilder.Sql("UPDATE identity_access.roles SET row_version = gen_random_uuid();");
```

- [ ] **Step 5: ตรวจ model drift และ test**

Run:
```bash
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~PermissionCatalogParityTests|FullyQualifiedName~ApprovalPermissionCoverageTests"
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `No changes have been made to the model since the last migration.`; test PASS; build 0 error.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.UnitTests/Organization/PermissionCatalogParityTests.cs
git commit -m "feat(roles): persist system flag, row version and role change requests

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: Extract `IdentityAdministrationSupport` (Global Reuse)

> **Global Reuse proposal (ต้องยืนยันกับผู้ใช้ก่อนเริ่ม Task นี้ ตาม AGENTS.md ข้อ 2):** `RoleManagementStore` ต้องใช้ 4 helper ที่ตอนนี้ private ใน `IdentityAdministrationStore` (`LoadPermissionsAsync` :621, ดึงแถวผู้ดูแลใน `WouldRemoveLastAdministratorAsync` :647, `AddAudit` :670, `EscapeLike` :689). extract เป็น `internal static` ตัวเดียวแทนการคัดลอก ~60 บรรทัด. **ถ้าผู้ใช้ไม่เห็นด้วย:** ทำ Step 2 (สร้างไฟล์) แต่ข้าม Step 3 (ไม่แก้ `IdentityAdministrationStore`) — Task 6-10 ใช้ได้เหมือนเดิม ข้อเสียคือโค้ดซ้ำ.

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationSupport.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs`
- Test (regression, ไม่แก้): `backend/tests/TanErp.IntegrationTests/Api/IdentityAdministrationEndpointsTests.cs`

- [ ] **Step 1: baseline เขียว**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests"`
Expected: PASS ทั้งคลาส (ต้องมี Docker). ถ้าไม่ผ่านก่อนแก้ ให้หยุดและรายงาน.

- [ ] **Step 2: สร้าง helper (ย้ายโค้ดตรงตัว)**

สร้าง `IdentityAdministrationSupport.cs`:

```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

/// <summary>Queries and audit helpers shared by the identity and role administration stores (G-03b).</summary>
internal static class IdentityAdministrationSupport
{
    /// <summary>Permissions a membership holds through its active roles (the anti-escalation baseline).</summary>
    public static async Task<List<PermissionGrant>> LoadHeldPermissionsAsync(AppDbContext db, Guid membershipId, CancellationToken ct) =>
        await db.MembershipRoles.AsNoTracking()
            .Where(mr => mr.MembershipId == membershipId && mr.Role!.IsActive)
            .SelectMany(mr => mr.Role!.RolePermissions)
            .Where(rp => rp.Permission!.IsActive)
            .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId))
            .ToListAsync(ct);

    /// <summary>Active administrators: users.manage at organization scope through an active role of an active membership.</summary>
    public static async Task<List<AdministratorRow>> LoadAdministratorRowsAsync(
        AppDbContext db, Guid organizationId, DateTimeOffset now, CancellationToken ct) =>
        await db.Memberships.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.IsActive)
            .Where(m => m.User!.IsActive && m.User.FirebaseUid != null && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .Where(mr => mr.Role!.RolePermissions.Any(rp =>
                    rp.Permission!.IsActive &&
                    rp.Permission.Key == AdministrationPermissions.UsersManage &&
                    rp.Scope == PermissionScope.Organization &&
                    rp.ScopeId == organizationId))
                .Select(mr => new AdministratorRow(m.UserId, m.Id, mr.RoleId)))
            .ToListAsync(ct);

    public static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    public static void AddAudit(
        AppDbContext db, Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId,
        string traceId, DateTimeOffset now, object changes, Guid? rowVersionBefore = null, Guid? rowVersionAfter = null)
    {
        db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), organizationId, actor.UserId, action, resourceType, resourceId, now, traceId,
            JsonSerializer.Serialize(changes), actorMembershipId: actor.MembershipId,
            rowVersionBefore: rowVersionBefore, rowVersionAfter: rowVersionAfter));
    }
}
```

- [ ] **Step 3: ให้ store เดิมเรียก helper (พฤติกรรมเดิม)**

ใน `IdentityAdministrationStore.cs` แทนเนื้อเมธอดทั้งสี่ด้วย delegate:

```csharp
    private Task<List<PermissionGrant>> LoadPermissionsAsync(Guid membershipId, CancellationToken cancellationToken) =>
        IdentityAdministrationSupport.LoadHeldPermissionsAsync(_db, membershipId, cancellationToken);

    private async Task<bool> WouldRemoveLastAdministratorAsync(
        Guid organizationId, Func<AdministratorRow, bool> removed, CancellationToken cancellationToken) =>
        AdministrationPolicy.WouldRemoveLastAdministrator(
            await IdentityAdministrationSupport.LoadAdministratorRowsAsync(_db, organizationId, _clock.UtcNow, cancellationToken), removed);

    private void AddAudit(
        Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId, string traceId,
        DateTimeOffset now, object changes, Guid? rowVersionBefore = null, Guid? rowVersionAfter = null) =>
        IdentityAdministrationSupport.AddAudit(_db, organizationId, actor, action, resourceType, resourceId, traceId, now, changes, rowVersionBefore, rowVersionAfter);

    private static string EscapeLike(string value) => IdentityAdministrationSupport.EscapeLike(value);
```
(ลบ `using System.Text.Json;` ถ้าไม่มีที่อื่นใช้ — เป็น warning ไม่ใช่ error.)

- [ ] **Step 4: Run regression**

Run: `dotnet build backend/TanErp.slnx --nologo -v q && dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests"`
Expected: build 0 error; PASS ทั้งคลาส (พฤติกรรมไม่เปลี่ยน).

- [ ] **Step 5: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/IdentityAccess
git commit -m "refactor(identity): share administration query helpers

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: API อ่าน — catalog, รายการ (search/pagination) และรายละเอียด (+ harness integration test)

Task นี้วางโครง handler/port/store/controller ของ G-03b; Task 7-10 เพิ่ม member ทีละชิ้นทั้งใน port, handler และ store ใน commit เดียวกันเพื่อให้ build ผ่านทุกขั้น. การพิสูจน์ "permission ก่อน existence" ทำด้วย integration test (403 เหมือนกันทุก id) และกฎ shape/นโยบายทดสอบเป็น unit test ใน Task 3 — จึงไม่มี fake store ใน unit test.

**Files:**
- Create: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/IRoleManagementStore.cs`, `RoleManagementHandler.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test (harness + อ่าน)**

สร้าง `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs` (harness นี้ถูกใช้ต่อใน Task 7-10):

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
using TanErp.Api.Contracts.IdentityAccess.RoleManagement;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class RoleManagementEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "admin-token";       // Test Admin (system role): holds every seeded permission
    private const string ViewerToken = "viewer-token";     // holds roles.assign only (NOT roles.manage)
    private const string CheckerToken = "checker-token";   // roles.manage + roles.assign-approval + a few permissions
    private const string OrgBToken = "org-b-token";
    private const string OrgCToken = "org-c-token";        // sole administrator comes from one non-system role

    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a02");
    private static readonly Guid CheckerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a12");
    private static readonly Guid OrgCId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a20");
    private static readonly Guid OrgCBranchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a21");
    private static readonly Guid OrgCMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a23");
    private static readonly Guid OrgCRoleId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a24");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                ViewerToken => "rm-viewer-uid",
                CheckerToken => "rm-checker-uid",
                OrgBToken => TestOnlyDataSeeder.TestFirebaseUidB,
                OrgCToken => "rm-org-c-uid",
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
        await SeedFixturesAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static async Task SeedFixturesAsync(AppDbContext db)
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var permissions = await db.Permissions.ToDictionaryAsync(p => p.Key);

        Role MakeRole(Guid org, string name, params string[] keys)
        {
            var role = new Role(Guid.NewGuid(), org, name, name, isActive: true);
            db.Roles.Add(role);
            foreach (var key in keys)
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, org, permissions[key].Id, PermissionScope.Organization, org));
            return role;
        }

        void MakeMember(Guid org, Guid? branch, Guid membershipId, string uid, Role role)
        {
            var userId = Guid.NewGuid();
            db.Users.Add(new User(userId, uid, uid, $"{uid}@example.test", isActive: true));
            db.Memberships.Add(new Membership(membershipId, org, branch, userId, isActive: true));
            db.MembershipRoles.Add(new MembershipRole(membershipId, role.Id, org));
        }

        MakeMember(orgId, TestOnlyDataSeeder.TestBranchId, ViewerMembershipId, "rm-viewer-uid", MakeRole(orgId, "RM Viewer", "roles.assign"));
        MakeMember(orgId, TestOnlyDataSeeder.TestBranchId, CheckerMembershipId, "rm-checker-uid", MakeRole(orgId, "RM Checker",
            "roles.manage", "roles.assign-approval", "estimates.approve", "estimates.read", "purchase-orders.approve", "customers.read"));

        // Organization C: its only administrator (users.manage) comes from one NON-system role.
        db.Organizations.Add(new TanErp.Domain.Organization.Organization(OrgCId, "RM Org C"));
        db.Branches.Add(new Branch(OrgCBranchId, OrgCId, "C01", "สาขา C"));
        var orgCRole = new Role(OrgCRoleId, OrgCId, "Org C Admin", "Org C Admin", isActive: true);
        db.Roles.Add(orgCRole);
        foreach (var key in new[] { "users.manage", "roles.manage", "customers.read" })
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), orgCRole.Id, OrgCId, permissions[key].Id, PermissionScope.Organization, OrgCId));
        MakeMember(OrgCId, OrgCBranchId, OrgCMembershipId, "rm-org-c-uid", orgCRole);

        await db.SaveChangesAsync();
    }

    private AppDbContext NewDb() => _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, Guid? membershipId = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? MembershipFor(token)).ToString());
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return _client.SendAsync(request);
    }

    private static Guid MembershipFor(string token) => token switch
    {
        ViewerToken => ViewerMembershipId,
        CheckerToken => CheckerMembershipId,
        OrgBToken => TestOnlyDataSeeder.TestMembershipBId,
        OrgCToken => OrgCMembershipId,
        _ => TestOnlyDataSeeder.TestMembershipId
    };

    private static async Task<string?> ReadCode(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private static async Task<T> ReadAs<T>(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<T>())!;

    /// <summary>Inserts a custom role straight into the database (the create endpoint is exercised separately).</summary>
    private async Task<Guid> SeedRoleAsync(string name, string[]? keys = null, bool active = true, Guid? orgId = null)
    {
        await using var db = NewDb();
        var org = orgId ?? TestOnlyDataSeeder.TestOrgId;
        var role = new Role(Guid.NewGuid(), org, name, "seeded " + name, active);
        db.Roles.Add(role);
        foreach (var key in keys ?? Array.Empty<string>())
        {
            var permission = await db.Permissions.SingleAsync(p => p.Key == key);
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, org, permission.Id, PermissionScope.Organization, org));
        }

        await db.SaveChangesAsync();
        return role.Id;
    }

    // ------------------------------------------------------------------ catalog

    [Fact]
    public async Task Catalog_ReturnsActivePermissionsSortedWithTheApprovalFlag()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/permissions", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = (await ReadAs<PermissionCatalogResponse>(response)).Items;
        Assert.Equal(items.Select(i => i.Key).OrderBy(k => k, StringComparer.Ordinal), items.Select(i => i.Key));
        Assert.True(items.Single(i => i.Key == "estimates.approve").IsApprovalPermission);
        Assert.True(items.Single(i => i.Key == "purchase-orders.approve").IsApprovalPermission);
        Assert.False(items.Single(i => i.Key == "customers.read").IsApprovalPermission);
    }

    [Fact]
    public async Task EveryEndpoint_WithoutRolesManage_Returns403WithTheSameCodeForAnyId()
    {
        var anyId = Guid.NewGuid();
        var urls = new[] { "/api/v1/admin/permissions", "/api/v1/admin/role-definitions", $"/api/v1/admin/role-definitions/{anyId}",
            $"/api/v1/admin/role-definitions/{TestOnlyDataSeeder.TestOrgId}" };

        foreach (var url in urls)
        {
            var response = await Send(HttpMethod.Get, url, ViewerToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("PERMISSION_DENIED", await ReadCode(response));
        }
    }

    // ------------------------------------------------------------------ list

    [Fact]
    public async Task List_SearchesFiltersByStatusPaginatesAndStaysInsideTheOrganization()
    {
        await SeedRoleAsync("Zeta Planner", new[] { "customers.read" });
        await SeedRoleAsync("Zeta Retired", active: false);
        await SeedRoleAsync("Zeta Org B", orgId: TestOnlyDataSeeder.TestOrgBId);

        var all = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=ZETA", AdminToken));
        var inactive = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=zeta&status=inactive", AdminToken));
        var paged = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=zeta&limit=1&page=2&sortOrder=desc", AdminToken));
        var escaped = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=%25", AdminToken));

        Assert.Equal(new[] { "Zeta Planner", "Zeta Retired" }, all.Items.Select(i => i.Name).ToArray());
        Assert.Equal(new[] { "Zeta Retired" }, inactive.Items.Select(i => i.Name).ToArray());
        Assert.Equal(2, paged.Pagination.TotalCount);
        Assert.Equal(2, paged.Pagination.TotalPages);
        Assert.Equal(new[] { "Zeta Planner" }, paged.Items.Select(i => i.Name).ToArray()); // desc: Retired, Planner
        Assert.Empty(escaped.Items); // a literal % is escaped, not a wildcard
        Assert.Contains(all.Items, i => i.Name == "Zeta Planner" && i.PermissionCount == 1 && !i.IsSystem);
    }

    [Fact]
    public async Task List_ShowsTheSeededSystemRoleAsSystemWithMembers()
    {
        var page = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=Test%20Admin", AdminToken));

        var admin = page.Items.Single(i => i.Name == "Test Admin");
        Assert.True(admin.IsSystem);
        Assert.True(admin.RequiresApproval);
        Assert.True(admin.MemberCount >= 1);
        Assert.DoesNotContain(page.Items, i => i.Name == "Test Admin B");
    }

    [Theory]
    [InlineData("status=bogus")]
    [InlineData("sortBy=bogus")]
    [InlineData("sortOrder=sideways")]
    public async Task List_InvalidQuery_Returns400(string query)
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?" + query, AdminToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(response));
    }

    // ------------------------------------------------------------------ detail

    [Fact]
    public async Task Get_ReturnsGrantsCountsAndEtag_AndOtherOrganizationIdIs404()
    {
        var roleId = await SeedRoleAsync("Detail Role", new[] { "customers.read", "estimates.approve" });
        var otherOrgRole = await SeedRoleAsync("Other Org Role", orgId: TestOnlyDataSeeder.TestOrgBId);

        var response = await Send(HttpMethod.Get, $"/api/v1/admin/role-definitions/{roleId}", AdminToken);
        var other = await Send(HttpMethod.Get, $"/api/v1/admin/role-definitions/{otherOrgRole}", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var role = await ReadAs<RoleDefinitionResponse>(response);
        Assert.Equal(new[] { "customers.read", "estimates.approve" }, role.Permissions.Select(p => p.PermissionKey).ToArray());
        Assert.All(role.Permissions, p => Assert.Equal("organization", p.Scope));
        Assert.True(role.RequiresApproval);
        Assert.Equal(0, role.MemberCount);
        Assert.Null(role.PendingChange);
        Assert.Equal($"\"{role.RowVersion}\"", response.Headers.ETag?.Tag);
        Assert.Equal(HttpStatusCode.NotFound, other.StatusCode);
        Assert.Equal("RESOURCE_NOT_FOUND", await ReadCode(other));
    }
}
```

- [ ] **Step 2: Run — ต้องล้ม (compile error)**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"` Expected: build FAIL (`TanErp.Api.Contracts.IdentityAccess.RoleManagement` ไม่มี).

- [ ] **Step 3: Application — port + handler**

`IRoleManagementStore.cs`:

```csharp
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.RoleManagement;

/// <summary>
/// Persistence for role definitions. Mutating members enforce anti-escalation, maker–checker, last-administrator and
/// concurrency in one transaction and write the audit event. Everything is scoped to <c>organizationId</c>.
/// </summary>
public interface IRoleManagementStore
{
    Task<IReadOnlyList<PermissionCatalogEntry>> ListPermissionsAsync(CancellationToken ct);

    Task<RoleDefinitionPage> ListRolesAsync(Guid organizationId, RoleListQuery query, CancellationToken ct);

    Task<Result<RoleDefinition>> GetRoleAsync(Guid organizationId, Guid roleId, CancellationToken ct);
}
```

`RoleManagementHandler.cs`:

```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.IdentityAccess.RoleManagement;

/// <summary>Every operation resolves its permission first; the store is never reached (and never queried) without it.</summary>
public sealed class RoleManagementHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IRoleManagementStore _store;

    public RoleManagementHandler(IRequestAccessResolver access, IRoleManagementStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(AdminCaller caller, string permission, CancellationToken ct) =>
        _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static AdminActor Actor(RequestAccessContext access) => new(access.ActorUserId, access.MembershipId);

    private static Result<T> Denied<T>(Result<RequestAccessContext> access) => Result<T>.Failure(access.Error);

    public async Task<Result<IReadOnlyList<PermissionCatalogEntry>>> ListPermissionsAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Result<IReadOnlyList<PermissionCatalogEntry>>.Failure(access.Error);
        return Result<IReadOnlyList<PermissionCatalogEntry>>.Success(await _store.ListPermissionsAsync(ct));
    }

    public async Task<Result<RoleDefinitionPage>> ListRolesAsync(
        AdminCaller caller, string? search, string? status, string? sortBy, string? sortOrder, int page, int limit, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleDefinitionPage>(access);

        if (status is not (null or "active" or "inactive"))
            return AdminHandlerSupport.Validation<RoleDefinitionPage>("Status must be 'active' or 'inactive'.");
        var key = string.IsNullOrWhiteSpace(sortBy) ? RoleSortKey.Name : sortBy.Trim();
        if (!RoleSortKey.IsValid(key)) return AdminHandlerSupport.Validation<RoleDefinitionPage>("Unknown role sort key.");
        var order = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder.Trim().ToLowerInvariant();
        if (order is not ("asc" or "desc")) return AdminHandlerSupport.Validation<RoleDefinitionPage>("Sort order must be 'asc' or 'desc'.");

        var query = new RoleListQuery(
            string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            status, key, order == "desc", Math.Max(page, 1),
            limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(limit, AdministrationPolicy.MaxPageSize));
        return Result<RoleDefinitionPage>.Success(await _store.ListRolesAsync(access.Value!.OrganizationId, query, ct));
    }

    public async Task<Result<RoleDefinition>> GetRoleAsync(AdminCaller caller, Guid roleId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        return access.IsFailure ? Denied<RoleDefinition>(access) : await _store.GetRoleAsync(access.Value!.OrganizationId, roleId, ct);
    }
}
```
(`Actor(...)` ยังไม่ถูกใช้ใน Task นี้ — compiler เตือนได้ ไม่ใช่ error; Task 7 ใช้.)

- [ ] **Step 4: Contracts**

สร้าง `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs` (ใช้ `AdminRefResponse`/`AdminPaginationResponse` เดิม):

```csharp
using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;

namespace TanErp.Api.Contracts.IdentityAccess.RoleManagement;

public record PermissionCatalogEntryResponse(string Key, string? Description, bool IsApprovalPermission);

public record PermissionCatalogResponse(IReadOnlyList<PermissionCatalogEntryResponse> Items)
{
    public static PermissionCatalogResponse From(IReadOnlyList<PermissionCatalogEntry> items) =>
        new(items.Select(i => new PermissionCatalogEntryResponse(i.Key, i.Description, i.IsApprovalPermission)).ToList());
}

public record RoleDefinitionSummaryResponse(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion,
    int MemberCount, int PermissionCount, bool RequiresApproval, bool HasPendingChange);

public record RoleDefinitionListResponse(IReadOnlyList<RoleDefinitionSummaryResponse> Items, AdminPaginationResponse Pagination)
{
    public static RoleDefinitionListResponse From(RoleDefinitionPage page) => new(
        page.Items.Select(i => new RoleDefinitionSummaryResponse(
            i.Id, i.Name, i.Description, i.IsSystem, i.IsActive, i.RowVersion, i.MemberCount, i.PermissionCount, i.RequiresApproval, i.HasPendingChange)).ToList(),
        new AdminPaginationResponse(page.Page, page.PageSize, page.TotalCount, (int)Math.Ceiling(page.TotalCount / (double)page.PageSize)));
}

public record RoleGrantResponse(string PermissionKey, string Scope, AdminRefResponse? Branch);

public record RoleChangeRequestResponse(
    Guid Id, Guid RowVersion, string Status, AdminRefResponse Role, AdminRefResponse RequestedBy, DateTimeOffset RequestedAtUtc,
    string ProposedName, string? ProposedDescription,
    IReadOnlyList<string> AddedPermissionKeys, IReadOnlyList<string> RemovedPermissionKeys)
{
    public static RoleChangeRequestResponse From(RoleChangeRequestView v) => new(
        v.Id, v.RowVersion, v.Status, new AdminRefResponse(v.Role.Id, v.Role.Name), new AdminRefResponse(v.RequestedBy.Id, v.RequestedBy.Name),
        v.RequestedAtUtc, v.ProposedName, v.ProposedDescription, v.AddedPermissionKeys, v.RemovedPermissionKeys);
}

public record RoleDefinitionResponse(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc,
    int MemberCount, int PendingAssignmentRequestCount, bool RequiresApproval,
    IReadOnlyList<RoleGrantResponse> Permissions, RoleChangeRequestResponse? PendingChange)
{
    public static RoleDefinitionResponse From(RoleDefinition r) => new(
        r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion, r.CreatedAtUtc, r.MemberCount, r.PendingAssignmentRequestCount, r.RequiresApproval,
        r.Permissions.Select(p => new RoleGrantResponse(p.PermissionKey, p.Scope, p.Branch is null ? null : new AdminRefResponse(p.Branch.Id, p.Branch.Name))).ToList(),
        r.PendingChange is null ? null : RoleChangeRequestResponse.From(r.PendingChange));
}
```

- [ ] **Step 5: Store (อ่าน + helper ที่ Task 7-10 ใช้ต่อ)**

สร้าง `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`:

```csharp
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class RoleManagementStore : IRoleManagementStore
{
    private const string VersionConflict = "ADMIN_VERSION_CONFLICT";

    private readonly AppDbContext _db;

    // Task 7 adds IClock (first needed by the write members).
    public RoleManagementStore(AppDbContext db) => _db = db;

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> NotFound<T>(string resource) => Fail<T>("RESOURCE_NOT_FOUND", $"{resource} was not found.");

    private Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> work, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, work, VersionConflict, ct);

    // ---------------------------------------------------------------- reads

    public async Task<IReadOnlyList<PermissionCatalogEntry>> ListPermissionsAsync(CancellationToken ct)
    {
        var rows = await _db.Permissions.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Key)
            .Select(p => new { p.Key, p.Description }).ToListAsync(ct);
        return rows.Select(p => new PermissionCatalogEntry(p.Key, p.Description, AdministrationPolicy.ApprovalPermissionKeys.Contains(p.Key))).ToList();
    }

    public async Task<RoleDefinitionPage> ListRolesAsync(Guid organizationId, RoleListQuery q, CancellationToken ct)
    {
        var query = _db.Roles.AsNoTracking().Where(r => r.OrganizationId == organizationId);
        if (q.Search is not null)
        {
            var pattern = $"%{IdentityAdministrationSupport.EscapeLike(q.Search)}%";
            query = query.Where(r => EF.Functions.ILike(r.Name, pattern, "\\") ||
                                     (r.Description != null && EF.Functions.ILike(r.Description, pattern, "\\")));
        }

        query = q.Status switch
        {
            "active" => query.Where(r => r.IsActive),
            "inactive" => query.Where(r => !r.IsActive),
            _ => query
        };

        var ordered = (q.SortBy, q.Descending) switch
        {
            (RoleSortKey.CreatedAt, false) => query.OrderBy(r => r.CreatedAtUtc),
            (RoleSortKey.CreatedAt, true) => query.OrderByDescending(r => r.CreatedAtUtc),
            (_, true) => query.OrderByDescending(r => r.NormalizedName),
            _ => query.OrderBy(r => r.NormalizedName)
        };

        var total = await query.CountAsync(ct);
        // The id is a stable tie-breaker so paging never skips or repeats rows.
        var rows = await ordered.ThenBy(r => r.Id).Skip((q.Page - 1) * q.Limit).Take(q.Limit)
            .Select(r => new
            {
                r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion,
                MemberCount = r.MembershipRoles.Count(),
                Keys = r.RolePermissions.Where(rp => rp.Permission!.IsActive).Select(rp => rp.Permission!.Key).ToList(),
                HasPending = _db.RoleChangeRequests.Any(c => c.RoleId == r.Id && c.Status == RoleAssignmentRequestStatus.Pending)
            })
            .ToListAsync(ct);

        var items = rows.Select(r => new RoleDefinitionSummary(
            r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion, r.MemberCount, r.Keys.Distinct().Count(),
            AdministrationPolicy.RequiresApproval(r.Keys), r.HasPending)).ToList();
        return new RoleDefinitionPage(items, total, q.Page, q.Limit);
    }

    public Task<Result<RoleDefinition>> GetRoleAsync(Guid organizationId, Guid roleId, CancellationToken ct) =>
        LoadDefinitionAsync(organizationId, roleId, ct);

    // ---------------------------------------------------------------- shared helpers (used by the write members in Tasks 7-10)

    private async Task<Result<RoleDefinition>> LoadDefinitionAsync(Guid organizationId, Guid roleId, CancellationToken ct)
    {
        _db.ChangeTracker.Clear();
        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
        if (role is null) return NotFound<RoleDefinition>("Role");

        var grants = await LoadGrantViewsAsync(roleId, ct);
        var members = await _db.MembershipRoles.AsNoTracking().CountAsync(mr => mr.RoleId == roleId, ct);
        var pendingAssignments = await _db.RoleAssignmentRequests.AsNoTracking()
            .CountAsync(r => r.RoleId == roleId && r.Status == RoleAssignmentRequestStatus.Pending, ct);
        var pendingRow = await ChangeRequestQuery(organizationId, c => c.RoleId == roleId && c.Status == RoleAssignmentRequestStatus.Pending)
            .FirstOrDefaultAsync(ct);

        return Result<RoleDefinition>.Success(new RoleDefinition(
            role.Id, role.Name, role.Description, role.IsSystem, role.IsActive, role.RowVersion, role.CreatedAtUtc, members, pendingAssignments,
            AdministrationPolicy.RequiresApproval(grants.Select(g => g.PermissionKey)), grants,
            pendingRow is null ? null : ToChangeView(organizationId, pendingRow, grants)));
    }

    private async Task<List<RoleGrantView>> LoadGrantViewsAsync(Guid roleId, CancellationToken ct)
    {
        var rows = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == roleId && rp.Permission!.IsActive)
            .OrderBy(rp => rp.Permission!.Key).ThenBy(rp => rp.Scope)
            .Select(rp => new
            {
                Key = rp.Permission!.Key,
                rp.Scope,
                rp.BranchId,
                BranchName = _db.Branches.Where(b => b.Id == rp.BranchId).Select(b => b.Name).FirstOrDefault()
            })
            .ToListAsync(ct);
        return rows.Select(r => new RoleGrantView(r.Key, r.Scope, r.BranchId is null ? null : new AdminRef(r.BranchId.Value, r.BranchName!))).ToList();
    }

    private sealed record ChangeRow(RoleChangeRequest Request, string RoleName, string RequestedByName);

    private IQueryable<ChangeRow> ChangeRequestQuery(Guid organizationId, Expression<Func<RoleChangeRequest, bool>> filter) =>
        from request in _db.RoleChangeRequests.AsNoTracking().Where(c => c.OrganizationId == organizationId).Where(filter)
        join role in _db.Roles.AsNoTracking() on request.RoleId equals role.Id
        join requester in _db.Users.AsNoTracking() on request.RequestedByUserId equals requester.Id
        orderby request.RequestedAtUtc, request.Id
        select new ChangeRow(request, role.Name, requester.DisplayName);

    private static List<RoleGrantInput> ParseGrants(string json) =>
        JsonSerializer.Deserialize<List<RoleGrantInput>>(json) ?? new List<RoleGrantInput>();

    /// <summary>Added/removed permission keys are computed against the role as it is now, so the checker sees the real effect.</summary>
    private static RoleChangeRequestView ToChangeView(Guid organizationId, ChangeRow row, IReadOnlyList<RoleGrantView> currentGrants)
    {
        var current = currentGrants.Select(g => RoleManagementPolicy.ToGrant(organizationId, g.PermissionKey, g.Scope, g.Branch?.Id)).ToList();
        var proposed = ParseGrants(row.Request.ProposedGrantsJson)
            .Select(g => RoleManagementPolicy.ToGrant(organizationId, g.PermissionKey, g.Scope, g.BranchId)).ToList();
        var (added, removed) = RoleManagementPolicy.Diff(current, proposed);

        return new RoleChangeRequestView(
            row.Request.Id, row.Request.RowVersion, row.Request.Status,
            new AdminRef(row.Request.RoleId, row.RoleName), new AdminRef(row.Request.RequestedByUserId, row.RequestedByName),
            row.Request.RequestedAtUtc, row.Request.ProposedName, row.Request.ProposedDescription,
            added.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList(),
            removed.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList());
    }
}
```
(`RunAsync`, `_clock`, `Fail`, `ParseGrants` ฯลฯ ยังไม่ถูกใช้ครบใน Task นี้ — เป็น warning ได้ Task 7-10 ใช้.)

- [ ] **Step 6: Controller + DI**

สร้าง `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.RoleManagement;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;

namespace TanErp.Api.Controllers;

/// <summary>Role definition administration (G-03b). Business rules live in Application/Infrastructure.</summary>
[ApiController]
[Authorize]
[Route("api/v1/admin")]
public sealed class AdminRolesController : ControllerBase
{
    private readonly RoleManagementHandler _handler;

    public AdminRolesController(RoleManagementHandler handler) => _handler = handler;

    private IActionResult Problem(Error error) => ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);

    private static AdminCaller Caller(AuthenticatedRequest a) => new(a.FirebaseUid, a.MembershipId);

    [HttpGet("permissions")]
    [ProducesResponseType<PermissionCatalogResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPermissions(CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListPermissionsAsync(Caller(auth.Value!), cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(PermissionCatalogResponse.From(result.Value!));
    }

    [HttpGet("role-definitions")]
    [ProducesResponseType<RoleDefinitionListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListRoleDefinitions(
        [FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null, [FromQuery] int page = 1, [FromQuery] int limit = 25,
        CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListRolesAsync(Caller(auth.Value!), search, status, sortBy, sortOrder, page, limit, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(RoleDefinitionListResponse.From(result.Value!));
    }

    [HttpGet("role-definitions/{roleId:guid}")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleDefinition([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        return RoleResult(await _handler.GetRoleAsync(Caller(auth.Value!), roleId, cancellationToken));
    }

    private IActionResult RoleResult(Result<RoleDefinition> result)
    {
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(RoleDefinitionResponse.From(result.Value));
    }
}
```

`Program.cs`: `grep -n "RoleManagement" backend/src/TanErp.Api/Program.cs` ต้องไม่พบ แล้วเพิ่มต่อจากบรรทัด `DecideAdminRoleRequestHandler` (:107):

```csharp
builder.Services.AddScoped<TanErp.Application.IdentityAccess.RoleManagement.IRoleManagementStore, TanErp.Infrastructure.Persistence.IdentityAccess.RoleManagementStore>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.RoleManagement.RoleManagementHandler>();
```

- [ ] **Step 7: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"`
Expected: PASS ทุกเคส. ถ้า `List_SearchesFilters...` ล้มเพราะ `Zeta Org B` ปรากฏ: store ต้องกรอง `OrganizationId`.

- [ ] **Step 8: Commit**

```bash
git add backend/src/TanErp.Application/IdentityAccess/RoleManagement backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs backend/src/TanErp.Api backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs
git commit -m "feat(roles): add role catalog, list and detail endpoints

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: API สร้าง Role (Idempotency-Key, catalog + branch validation, anti-escalation)

**Files:**
- Modify: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/{IRoleManagementStore,RoleManagementHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `backend/src/TanErp.Api/Resources/Errors.resx`, `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test**

เพิ่มในคลาส `RoleManagementEndpointsTests` (ใต้ส่วน detail):

```csharp
    // ------------------------------------------------------------------ create

    private static SaveRoleDefinitionRequest Body(string name, params string[] keys) =>
        new(name, "desc " + name, keys.Select(k => new RoleGrantRequest(k, "organization", null)).ToList());

    private Task<HttpResponseMessage> CreateRole(SaveRoleDefinitionRequest body, string? key = null, string token = AdminToken) =>
        Send(HttpMethod.Post, "/api/v1/admin/role-definitions", token, body, idempotencyKey: key ?? Guid.NewGuid().ToString());

    private async Task<RoleDefinitionResponse> GetRole(Guid roleId, string token = AdminToken) =>
        await ReadAs<RoleDefinitionResponse>(await Send(HttpMethod.Get, $"/api/v1/admin/role-definitions/{roleId}", token));

    [Fact]
    public async Task Create_PersistsRoleAndGrantsAndAuditsKeysWithoutNames()
    {
        var response = await CreateRole(Body("Quote Reviewer", "customers.read", "estimates.read"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var role = await ReadAs<RoleDefinitionResponse>(response);
        Assert.False(role.IsSystem);
        Assert.True(role.IsActive);
        Assert.Equal(new[] { "customers.read", "estimates.read" }, role.Permissions.Select(p => p.PermissionKey).ToArray());
        Assert.Equal($"\"{role.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "roles.created" && a.ResourceId == role.Id.ToString());
        Assert.Contains("customers.read", audit.ChangesJson);
        Assert.DoesNotContain("Quote Reviewer", audit.ChangesJson);
    }

    [Fact]
    public async Task Create_ApprovalRole_IsCreatedImmediatelyBecauseNobodyHoldsItYet()
    {
        var role = await ReadAs<RoleDefinitionResponse>(await CreateRole(Body("Estimate Approver", "estimates.approve")));

        Assert.True(role.RequiresApproval);
        Assert.Equal(0, role.MemberCount);
    }

    [Fact]
    public async Task Create_DuplicateNameIgnoringCase_Returns409_AndOtherOrganizationsMayReuseTheName()
    {
        Assert.Equal(HttpStatusCode.Created, (await CreateRole(Body("Shared Name"))).StatusCode);

        var duplicate = await CreateRole(Body("  SHARED name "));
        var otherOrg = await CreateRole(Body("Shared Name"), token: OrgBToken);

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("ROLE_NAME_ALREADY_EXISTS", await ReadCode(duplicate));
        Assert.Equal(HttpStatusCode.Created, otherOrg.StatusCode);
    }

    [Fact]
    public async Task Create_ReservedPrefixTooShortNameAndUnknownPermission_AreRejected()
    {
        var reserved = await CreateRole(Body("System.Root"));
        var tooShort = await CreateRole(Body("ab"));
        var unknown = await CreateRole(Body("Ghost Role", "no.such-permission"));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, reserved.StatusCode);
        Assert.Equal("ROLE_NAME_RESERVED", await ReadCode(reserved));
        Assert.Equal(HttpStatusCode.BadRequest, tooShort.StatusCode);
        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(tooShort));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unknown.StatusCode);
        Assert.Equal("ROLE_PERMISSION_UNKNOWN", await ReadCode(unknown));
    }

    [Fact]
    public async Task Create_WithAPermissionTheCallerDoesNotHold_Returns403AndCreatesNothing()
    {
        // The checker holds roles.manage but not users.manage.
        var response = await CreateRole(Body("Escalation Role", "customers.read", "users.manage"), token: CheckerToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("ROLE_ESCALATION_DENIED", await ReadCode(response));
        await using var db = NewDb();
        Assert.False(await db.Roles.AnyAsync(r => r.Name == "Escalation Role"));
    }

    [Fact]
    public async Task Create_BranchScopedGrant_NeedsABranchOfTheCallersOrganization()
    {
        SaveRoleDefinitionRequest BranchGrant(Guid branchId) =>
            new("Branch Reader", null, new[] { new RoleGrantRequest("estimates.read", "branch", branchId) });

        var foreign = await CreateRole(BranchGrant(TestOnlyDataSeeder.TestBranchBId));
        var own = await CreateRole(BranchGrant(TestOnlyDataSeeder.TestBranchId));

        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);
        Assert.Equal("BRANCH_NOT_FOUND", await ReadCode(foreign));
        var grant = Assert.Single((await ReadAs<RoleDefinitionResponse>(own)).Permissions);
        Assert.Equal("branch", grant.Scope);
        Assert.Equal(TestOnlyDataSeeder.TestBranchId, grant.Branch!.Id);
    }

    [Fact]
    public async Task Create_ReplayKeyReuseAndMissingKey()
    {
        var key = "role-create-" + Guid.NewGuid();
        var first = await CreateRole(Body("Replay Role"), key);
        var second = await CreateRole(Body("Replay Role"), key);
        var reused = await CreateRole(Body("Other Role"), key);
        var noKey = await Send(HttpMethod.Post, "/api/v1/admin/role-definitions", AdminToken, Body("No Key"));

        Assert.Equal((await ReadAs<RoleDefinitionResponse>(first)).Id, (await ReadAs<RoleDefinitionResponse>(second)).Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ReadCode(reused));
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", await ReadCode(noKey));
        await using var db = NewDb();
        Assert.Equal(1, await db.Roles.CountAsync(r => r.Name == "Replay Role"));
    }
```
Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"` Expected: build FAIL (`SaveRoleDefinitionRequest`, `RoleGrantRequest` ไม่มี).

- [ ] **Step 2: Application — port + handler**

เพิ่มใน `IRoleManagementStore`:

```csharp
    Task<Result<RoleDefinition>> CreateRoleAsync(
        Guid organizationId, RoleInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct);
```
(ต้องมี `using TanErp.Application.IdentityAccess.Administration;` ใน port สำหรับ `AdminActor`.)

เพิ่มใน `RoleManagementHandler` (ต้อง `using TanErp.Application.Common.Security;`):

```csharp
    public async Task<Result<RoleDefinition>> CreateRoleAsync(AdminCaller caller, string idempotencyKey, RoleInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleDefinition>(access);

        var invalid = RoleManagementPolicy.ValidateShape(input);
        if (invalid is not null) return Result<RoleDefinition>.Failure(invalid);

        var canonical = RoleManagementPolicy.Canonicalize(input);
        return await _store.CreateRoleAsync(
            access.Value!.OrganizationId, canonical, Actor(access.Value), Sha256Hex.Compute(idempotencyKey), PayloadHash(canonical), traceId, ct);
    }

    private static string PayloadHash(RoleInput input) => Sha256Hex.Compute(
        $"{input.Name.Trim()}|{input.Description?.Trim()}|{string.Join(",", input.Permissions.Select(g => $"{g.PermissionKey}:{g.Scope}:{g.BranchId}"))}");
```

- [ ] **Step 3: Contracts**

เพิ่มใน `RoleManagementContracts.cs`:

```csharp
public record RoleGrantRequest(string? PermissionKey, string? Scope, Guid? BranchId);

public record SaveRoleDefinitionRequest(string? Name, string? Description, IReadOnlyList<RoleGrantRequest>? Permissions)
{
    public RoleInput ToInput() => new(
        Name ?? string.Empty,
        Description,
        (Permissions ?? Array.Empty<RoleGrantRequest>())
            .Select(g => new RoleGrantInput(g.PermissionKey ?? string.Empty, g.Scope ?? string.Empty, g.BranchId)).ToList());
}
```

- [ ] **Step 4: Store**

ที่หัว `RoleManagementStore.cs` เพิ่ม `using TanErp.Domain.Common;` (สำหรับ `IdempotencyRecord`) แล้วแก้ field/constructor เป็น:

```csharp
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public RoleManagementStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }
```
จากนั้นเพิ่มค่าคงที่ + helper + เมธอดในคลาส (ต่อจากส่วน "shared helpers"):

```csharp
    private const string CreateOperation = "admin.roles.define";
    private const string NameIndex = "ix_roles_organization_id_normalized_role_name";

    private sealed record ResolvedGrant(Guid PermissionId, string Key, string Scope, Guid? BranchId);

    private static PermissionGrant ToGrant(Guid organizationId, ResolvedGrant g) =>
        RoleManagementPolicy.ToGrant(organizationId, g.Key, g.Scope, g.BranchId);

    private static RolePermission NewGrant(Guid roleId, Guid organizationId, ResolvedGrant g, DateTimeOffset now) =>
        new(Guid.NewGuid(), roleId, organizationId, g.PermissionId, g.Scope, scopeId: null, branchId: g.BranchId, assignedAtUtc: now);

    private static List<string> KeysOf(IEnumerable<PermissionGrant> grants) =>
        grants.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList();

    private void Audit(
        Guid organizationId, AdminActor actor, string action, string resourceType, Guid resourceId, string traceId,
        object changes, Guid? versionBefore = null, Guid? versionAfter = null) =>
        IdentityAdministrationSupport.AddAudit(
            _db, organizationId, actor, action, resourceType, resourceId.ToString(), traceId, _clock.UtcNow, changes, versionBefore, versionAfter);

    /// <summary>Checks every key against the active catalog and every branch against the organization.</summary>
    private async Task<Result<List<ResolvedGrant>>> ResolveGrantsAsync(Guid organizationId, IReadOnlyList<RoleGrantInput> inputs, CancellationToken ct)
    {
        var keys = inputs.Select(g => g.PermissionKey).Distinct().ToList();
        var permissions = await _db.Permissions.AsNoTracking()
            .Where(p => p.IsActive && keys.Contains(p.Key)).ToDictionaryAsync(p => p.Key, p => p.Id, ct);
        if (permissions.Count != keys.Count)
            return Fail<List<ResolvedGrant>>("ROLE_PERMISSION_UNKNOWN", "A permission key is not in the active catalog.");

        var branchIds = inputs.Where(g => g.BranchId.HasValue).Select(g => g.BranchId!.Value).Distinct().ToList();
        var branchCount = await _db.Branches.AsNoTracking()
            .CountAsync(b => b.OrganizationId == organizationId && b.IsActive && branchIds.Contains(b.Id), ct);
        if (branchCount != branchIds.Count) return Fail<List<ResolvedGrant>>("BRANCH_NOT_FOUND", "Branch not found.");

        return Result<List<ResolvedGrant>>.Success(
            inputs.Select(g => new ResolvedGrant(permissions[g.PermissionKey], g.PermissionKey, g.Scope, g.BranchId)).ToList());
    }

    public Task<Result<RoleDefinition>> CreateRoleAsync(
        Guid organizationId, RoleInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            // Taken INSIDE the transaction so concurrent requests with the same key serialize and the loser replays.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({organizationId.ToString("N") + ":" + CreateOperation + ":" + keyHash}, 0))", ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == organizationId && r.Operation == CreateOperation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                return replay.PayloadHash != payloadHash
                    ? Fail<RoleDefinition>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.")
                    : await LoadDefinitionAsync(organizationId, Guid.Parse(replay.ResourceId), ct);
            }

            var resolved = await ResolveGrantsAsync(organizationId, input.Permissions, ct);
            if (resolved.IsFailure) return Result<RoleDefinition>.Failure(resolved.Error);

            // Anti-escalation: the caller may only hand out permissions it holds (CP-02 rule, same policy).
            var held = await IdentityAdministrationSupport.LoadHeldPermissionsAsync(_db, actor.MembershipId, ct);
            var wanted = resolved.Value!.Select(g => ToGrant(organizationId, g)).ToList();
            if (!AdministrationPolicy.Covers(held, organizationId, wanted))
                return Fail<RoleDefinition>("ROLE_ESCALATION_DENIED", "The role grants permissions the caller does not hold.");

            var now = _clock.UtcNow;
            Role role;
            try
            {
                role = Role.CreateCustom(Guid.NewGuid(), organizationId, input.Name, input.Description, now);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleDefinition>(ex.Code, ex.Message);
            }

            _db.Roles.Add(role);
            foreach (var grant in resolved.Value!) _db.RolePermissions.Add(NewGrant(role.Id, organizationId, grant, now));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, CreateOperation, keyHash, payloadHash, role.Id.ToString(), now));
            Audit(organizationId, actor, "roles.created", "Role", role.Id, traceId,
                new { permissionKeys = KeysOf(wanted), requiresApproval = AdministrationPolicy.RequiresApproval(wanted.Select(g => g.Key)) }, null, role.RowVersion);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleDefinition>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            return await LoadDefinitionAsync(organizationId, role.Id, ct);
        }, ct);
```

- [ ] **Step 5: Controller, error codes**

เพิ่มใน `AdminRolesController`:

```csharp
    [HttpPost("role-definitions")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRoleDefinition([FromBody] SaveRoleDefinitionRequest request, CancellationToken cancellationToken)
    {
        var idempotent = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (idempotent.IsFailure) return Problem(idempotent.Error);

        var result = await _handler.CreateRoleAsync(
            new AdminCaller(idempotent.Value!.FirebaseUid, idempotent.Value.MembershipId), idempotent.Value.IdempotencyKey,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Created($"/api/v1/admin/role-definitions/{result.Value.Id}", RoleDefinitionResponse.From(result.Value));
    }
```

`ProblemDetailsMapper.GetStatus` (ใกล้ `ORGANIZATION_TAX_ID_INVALID`):

```csharp
        "ROLE_NAME_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
        "ROLE_NAME_RESERVED" => StatusCodes.Status422UnprocessableEntity,
        "ROLE_PERMISSION_UNKNOWN" => StatusCodes.Status422UnprocessableEntity,
```

`Errors.resx` / `Errors.en.resx` เพิ่ม `_TITLE`/`_DETAIL` (รูปแบบ `<data name="..." xml:space="preserve"><value>...</value></data>` เหมือน `ADMIN_VERSION_CONFLICT_*`):

| key | th | en |
| --- | --- | --- |
| `ROLE_NAME_ALREADY_EXISTS` | ชื่อบทบาทซ้ำ / มีบทบาทที่ใช้ชื่อนี้อยู่แล้วในองค์กร | Duplicate role name / A role with this name already exists in the organization. |
| `ROLE_NAME_RESERVED` | ชื่อบทบาทถูกสงวน / ชื่อที่ขึ้นต้นด้วย "system." สงวนไว้สำหรับบทบาทของระบบ | Reserved role name / Names starting with "system." are reserved for built-in roles. |
| `ROLE_PERMISSION_UNKNOWN` | ไม่พบสิทธิ์ที่ระบุ / มีสิทธิ์ที่ไม่อยู่ในรายการสิทธิ์ของระบบหรือถูกปิดใช้งาน | Unknown permission / A permission is not in the active catalog. |

- [ ] **Step 6: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"`
Expected: PASS ทุกเคส.

- [ ] **Step 7: Commit**

```bash
git add backend/src/TanErp.Application/IdentityAccess/RoleManagement backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs backend/src/TanErp.Api backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs
git commit -m "feat(roles): add role creation endpoint

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 8: Mutation check — anti-escalation (หลัง commit; ไม่ commit การทดลอง)**

ใน `RoleManagementStore.CreateRoleAsync` ลบทั้งบล็อก `if (!AdministrationPolicy.Covers(held, organizationId, wanted)) return Fail<RoleDefinition>("ROLE_ESCALATION_DENIED", ...);` → รัน `--filter "FullyQualifiedName~Create_WithAPermissionTheCallerDoesNotHold_Returns403AndCreatesNothing"` Expected: **FAIL** (ได้ 201). คืนค่า `git checkout -- backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs` แล้วรันซ้ำ Expected: PASS; `git status --short` ว่าง.

---

## Task 8: API แก้ Role (system immutable, grant diff, anti-escalation, last-admin, maker–checker)

**Files:**
- Modify: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/{IRoleManagementStore,RoleManagementHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test**

เพิ่มใน `RoleManagementEndpointsTests`:

```csharp
    // ------------------------------------------------------------------ update

    private async Task SeedHolderAsync(Guid roleId)
    {
        await using var db = NewDb();
        var userId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        db.Users.Add(new User(userId, "holder-" + userId, "Holder", $"{userId}@example.test", isActive: true));
        db.Memberships.Add(new Membership(membershipId, TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestBranchId, userId, isActive: true));
        db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, TestOnlyDataSeeder.TestOrgId));
        await db.SaveChangesAsync();
    }

    private async Task<HttpResponseMessage> PutRole(Guid roleId, string name, string[] keys, string token = AdminToken, Guid? ifMatch = null)
    {
        var current = await GetRole(roleId);
        return await Send(HttpMethod.Put, $"/api/v1/admin/role-definitions/{roleId}", token, Body(name, keys), ifMatch: ifMatch ?? current.RowVersion);
    }

    [Fact]
    public async Task Update_RenamesAndChangesGrantsOfAnUnheldRole_AndAuditsKeysNotNames()
    {
        var roleId = await SeedRoleAsync("Editable Role", new[] { "customers.read", "estimates.read" });

        var response = await PutRole(roleId, "Edited Role", new[] { "customers.read", "quotations.read" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadAs<UpdateRoleDefinitionResponse>(response);
        Assert.Null(body.PendingRequest);
        Assert.Equal("Edited Role", body.Role.Name);
        Assert.Equal(new[] { "customers.read", "quotations.read" }, body.Role.Permissions.Select(p => p.PermissionKey).ToArray());
        Assert.Equal($"\"{body.Role.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "roles.updated" && a.ResourceId == roleId.ToString());
        Assert.Contains("quotations.read", audit.ChangesJson);
        Assert.Contains("estimates.read", audit.ChangesJson);
        Assert.DoesNotContain("Edited Role", audit.ChangesJson);
    }

    [Fact]
    public async Task Update_SystemRole_Returns409AndChangesNothing()
    {
        Guid systemRoleId;
        await using (var db = NewDb()) systemRoleId = (await db.Roles.SingleAsync(r => r.Name == "Test Admin" && r.OrganizationId == TestOnlyDataSeeder.TestOrgId)).Id;

        var response = await PutRole(systemRoleId, "Renamed Admin", Array.Empty<string>());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ROLE_SYSTEM_IMMUTABLE", await ReadCode(response));
        var role = await GetRole(systemRoleId);
        Assert.Equal("Test Admin", role.Name);
        Assert.True(role.Permissions.Count > 10);
    }

    [Fact]
    public async Task Update_StaleOrMissingIfMatch_Returns409Or428()
    {
        var roleId = await SeedRoleAsync("Versioned Role");

        var stale = await PutRole(roleId, "Versioned Role", Array.Empty<string>(), ifMatch: Guid.NewGuid());
        var missing = await Send(HttpMethod.Put, $"/api/v1/admin/role-definitions/{roleId}", AdminToken, Body("Versioned Role"));

        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
    }

    [Fact]
    public async Task Update_AddingAPermissionTheCallerDoesNotHold_Returns403_ButRemovingIsAllowed()
    {
        var roleId = await SeedRoleAsync("Anti Escalation", new[] { "users.manage", "customers.read" });

        var add = await PutRole(roleId, "Anti Escalation", new[] { "users.manage", "customers.read", "users.read" }, CheckerToken);
        var remove = await PutRole(roleId, "Anti Escalation", new[] { "customers.read" }, CheckerToken);

        Assert.Equal("ROLE_ESCALATION_DENIED", await ReadCode(add));
        Assert.Equal(HttpStatusCode.OK, remove.StatusCode); // the checker never held users.manage, yet may take it away
    }

    [Fact]
    public async Task Update_AddingApprovalPermissionToAHeldRole_CreatesAPendingRequestAndLeavesTheRoleUnchanged()
    {
        var roleId = await SeedRoleAsync("Held Role", new[] { "customers.read" });
        await SeedHolderAsync(roleId);

        var response = await PutRole(roleId, "Held Role", new[] { "customers.read", "estimates.approve" });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var body = await ReadAs<UpdateRoleDefinitionResponse>(response);
        Assert.Equal(new[] { "estimates.approve" }, body.PendingRequest!.AddedPermissionKeys.ToArray());
        Assert.Equal(RoleAssignmentRequestStatus.Pending, body.PendingRequest.Status);
        var role = await GetRole(roleId);
        Assert.Equal(new[] { "customers.read" }, role.Permissions.Select(p => p.PermissionKey).ToArray());
        Assert.Equal(body.PendingRequest.Id, role.PendingChange!.Id);
        await using var db = NewDb();
        Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "roles.change-requested" && a.ResourceId == body.PendingRequest.Id.ToString()));
    }

    [Fact]
    public async Task Update_AddingApprovalPermissionToAnUnheldRole_AppliesDirectly()
    {
        var roleId = await SeedRoleAsync("Unheld Role", new[] { "customers.read" });

        var response = await PutRole(roleId, "Unheld Role", new[] { "customers.read", "estimates.approve" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode); // the assignment gate (CP-02) protects who may receive it
    }

    [Fact]
    public async Task Update_NonApprovalChangeOnAHeldRole_AndRemovingApprovalFromAHeldRole_ApplyDirectly()
    {
        var plain = await SeedRoleAsync("Held Plain", new[] { "customers.read" });
        await SeedHolderAsync(plain);
        var approver = await SeedRoleAsync("Held Approver", new[] { "customers.read", "estimates.approve" });
        await SeedHolderAsync(approver);

        var addPlain = await PutRole(plain, "Held Plain", new[] { "customers.read", "quotations.read" });
        var dropApproval = await PutRole(approver, "Held Approver", new[] { "customers.read" });

        Assert.Equal(HttpStatusCode.OK, addPlain.StatusCode);
        Assert.Equal(HttpStatusCode.OK, dropApproval.StatusCode); // reducing authority never needs a checker
    }

    [Fact]
    public async Task Update_WhileAChangeRequestIsPending_Returns409()
    {
        var roleId = await SeedRoleAsync("Pending Role", new[] { "customers.read" });
        await SeedHolderAsync(roleId);
        Assert.Equal(HttpStatusCode.Accepted, (await PutRole(roleId, "Pending Role", new[] { "customers.read", "estimates.approve" })).StatusCode);

        var second = await PutRole(roleId, "Pending Role", new[] { "customers.read" });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal("ROLE_CHANGE_REQUEST_PENDING", await ReadCode(second));
    }

    [Fact]
    public async Task Update_RemovingUsersManageFromTheLastAdministratorRole_Returns422_ButOtherEditsAreAllowed()
    {
        var blocked = await Send(HttpMethod.Put, $"/api/v1/admin/role-definitions/{OrgCRoleId}", OrgCToken,
            Body("Org C Admin", "roles.manage", "customers.read"), ifMatch: (await GetRole(OrgCRoleId, OrgCToken)).RowVersion);
        var harmless = await Send(HttpMethod.Put, $"/api/v1/admin/role-definitions/{OrgCRoleId}", OrgCToken,
            Body("Org C Admin", "users.manage", "roles.manage"), ifMatch: (await GetRole(OrgCRoleId, OrgCToken)).RowVersion);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, blocked.StatusCode);
        Assert.Equal("LAST_ADMINISTRATOR_REQUIRED", await ReadCode(blocked));
        Assert.Equal(HttpStatusCode.OK, harmless.StatusCode); // dropping customers.read touches no administrator
    }

    [Fact]
    public async Task Update_OtherOrganizationRoleIs404_AndDuplicateNameIs409()
    {
        var foreign = await SeedRoleAsync("Foreign Role", orgId: TestOnlyDataSeeder.TestOrgBId);
        await SeedRoleAsync("Name Owner");
        var taker = await SeedRoleAsync("Name Taker");

        var notFound = await Send(HttpMethod.Put, $"/api/v1/admin/role-definitions/{foreign}", AdminToken, Body("Hijacked"), ifMatch: Guid.NewGuid());
        var duplicate = await PutRole(taker, "name owner", Array.Empty<string>());

        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
        Assert.Equal("ROLE_NAME_ALREADY_EXISTS", await ReadCode(duplicate));
    }
```
Run → build FAIL (`UpdateRoleDefinitionResponse` ไม่มี).

- [ ] **Step 2: Application — port + handler**

port:

```csharp
    Task<Result<RoleUpdateOutcome>> UpdateRoleAsync(
        Guid organizationId, Guid roleId, Guid ifMatch, RoleInput input, AdminActor actor, string traceId, CancellationToken ct);
```
handler:

```csharp
    public async Task<Result<RoleUpdateOutcome>> UpdateRoleAsync(
        AdminCaller caller, Guid roleId, Guid ifMatch, RoleInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleUpdateOutcome>(access);

        var invalid = RoleManagementPolicy.ValidateShape(input);
        if (invalid is not null) return Result<RoleUpdateOutcome>.Failure(invalid);

        return await _store.UpdateRoleAsync(
            access.Value!.OrganizationId, roleId, ifMatch, RoleManagementPolicy.Canonicalize(input), Actor(access.Value), traceId, ct);
    }
```

- [ ] **Step 3: Contracts + controller**

เพิ่มใน `RoleManagementContracts.cs`: `public record UpdateRoleDefinitionResponse(RoleDefinitionResponse Role, RoleChangeRequestResponse? PendingRequest);`

เพิ่มใน controller:

```csharp
    [HttpPut("role-definitions/{roleId:guid}")]
    [ProducesResponseType<UpdateRoleDefinitionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UpdateRoleDefinitionResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRoleDefinition([FromRoute] Guid roleId, [FromBody] SaveRoleDefinitionRequest request, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        var result = await _handler.UpdateRoleAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), roleId, conditional.Value.IfMatchRowVersion,
            request.ToInput(), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var outcome = result.Value!;
        var body = new UpdateRoleDefinitionResponse(
            RoleDefinitionResponse.From(outcome.Role), outcome.Request is null ? null : RoleChangeRequestResponse.From(outcome.Request));
        if (outcome.Pending) return StatusCode(StatusCodes.Status202Accepted, body);

        Response.Headers.ETag = $"\"{outcome.Role.RowVersion}\"";
        return Ok(body);
    }
```

`ProblemDetailsMapper.GetStatus`: `"ROLE_SYSTEM_IMMUTABLE" => StatusCodes.Status409Conflict,` และ `"ROLE_CHANGE_REQUEST_PENDING" => StatusCodes.Status409Conflict,`. resx:

| key | th | en |
| --- | --- | --- |
| `ROLE_SYSTEM_IMMUTABLE` | แก้ไขบทบาทของระบบไม่ได้ / บทบาทของระบบแก้ไข ปิดใช้งาน หรือเปิดใช้งานไม่ได้ | System role is read-only / A system role cannot be edited, deactivated or activated. |
| `ROLE_CHANGE_REQUEST_PENDING` | มีคำขอแก้ไขรออนุมัติ / บทบาทนี้มีคำขอแก้ไขที่รอผู้ตรวจอนุมัติ ต้องตัดสินคำขอก่อน | A change request is pending / This role has a change request awaiting a checker. Decide it first. |

- [ ] **Step 4: Store**

เพิ่มใน `RoleManagementStore` (ต้อง `using TanErp.Application.IdentityAccess.Administration;` มีแล้ว):

```csharp
    private sealed record ChangePlan(List<ResolvedGrant> Wanted, IReadOnlyList<PermissionGrant> Added, IReadOnlyList<PermissionGrant> Removed);

    private Task<bool> HasPendingChangeAsync(Guid roleId, CancellationToken ct) =>
        _db.RoleChangeRequests.AsNoTracking().AnyAsync(c => c.RoleId == roleId && c.Status == RoleAssignmentRequestStatus.Pending, ct);

    private Task<bool> NameTakenAsync(Guid organizationId, Guid roleId, string name, CancellationToken ct)
    {
        var normalized = name.ToUpperInvariant();
        return _db.Roles.AsNoTracking().AnyAsync(r => r.OrganizationId == organizationId && r.NormalizedName == normalized && r.Id != roleId, ct);
    }

    /// <summary>
    /// Diffs the wanted grants against the role and runs the invariants for whoever applies the change
    /// (the editor now, or the checker at approval): anti-escalation on ADDED grants and the last-administrator guard.
    /// <paramref name="role"/> must be loaded with its RolePermissions and Permission.
    /// </summary>
    private async Task<Result<ChangePlan>> PlanChangeAsync(
        Role role, Guid organizationId, IReadOnlyList<RoleGrantInput> inputs, AdminActor actor, CancellationToken ct)
    {
        var resolved = await ResolveGrantsAsync(organizationId, inputs, ct);
        if (resolved.IsFailure) return Result<ChangePlan>.Failure(resolved.Error);

        var current = role.RolePermissions.Where(rp => rp.Permission!.IsActive)
            .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId)).ToList();
        var (added, removed) = RoleManagementPolicy.Diff(current, resolved.Value!.Select(g => ToGrant(organizationId, g)).ToList());

        var held = await IdentityAdministrationSupport.LoadHeldPermissionsAsync(_db, actor.MembershipId, ct);
        if (!AdministrationPolicy.Covers(held, organizationId, added))
            return Fail<ChangePlan>("ROLE_ESCALATION_DENIED", "The change adds permissions the caller does not hold.");

        if (removed.Any(g => g.Key == AdministrationPermissions.UsersManage && g.Scope == PermissionScope.Organization))
        {
            var rows = await IdentityAdministrationSupport.LoadAdministratorRowsAsync(_db, organizationId, _clock.UtcNow, ct);
            if (AdministrationPolicy.WouldRemoveLastAdministrator(rows, row => row.RoleId == role.Id))
                return Fail<ChangePlan>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");
        }

        return Result<ChangePlan>.Success(new ChangePlan(resolved.Value!, added, removed));
    }

    /// <summary>Applies name/description and the grant diff; throws <see cref="RoleDomainException"/> for a system role or bad name.</summary>
    private void ApplyDefinition(Role role, Guid organizationId, string name, string? description, ChangePlan plan, DateTimeOffset now)
    {
        role.UpdateDetails(name, description);
        var removable = role.RolePermissions
            .Where(rp => rp.Permission!.IsActive && plan.Removed.Contains(new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId)))
            .ToList();
        foreach (var grant in removable) _db.RolePermissions.Remove(grant);
        foreach (var grant in plan.Wanted.Where(g => plan.Added.Contains(ToGrant(organizationId, g))))
            _db.RolePermissions.Add(NewGrant(role.Id, organizationId, grant, now));
    }

    public Task<Result<RoleUpdateOutcome>> UpdateRoleAsync(
        Guid organizationId, Guid roleId, Guid ifMatch, RoleInput input, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var role = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
            if (role is null) return NotFound<RoleUpdateOutcome>("Role");
            if (role.RowVersion != ifMatch) return Fail<RoleUpdateOutcome>(VersionConflict, "The role was modified by another user.");
            if (role.IsSystem) return Fail<RoleUpdateOutcome>("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
            if (await HasPendingChangeAsync(roleId, ct))
                return Fail<RoleUpdateOutcome>("ROLE_CHANGE_REQUEST_PENDING", "A change request is already pending for this role.");

            string name;
            string? description;
            try
            {
                (name, description) = RoleRules.Normalize(input.Name, input.Description);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleUpdateOutcome>(ex.Code, ex.Message);
            }

            var plan = await PlanChangeAsync(role, organizationId, input.Permissions, actor, ct);
            if (plan.IsFailure) return Result<RoleUpdateOutcome>.Failure(plan.Error);

            var now = _clock.UtcNow;
            var addedKeys = KeysOf(plan.Value!.Added);
            var removedKeys = KeysOf(plan.Value.Removed);
            var hasHolders = await _db.MembershipRoles.AnyAsync(mr => mr.RoleId == roleId, ct);

            if (RoleManagementPolicy.NeedsChecker(hasHolders, plan.Value.Wanted.Select(g => g.Key), plan.Value.Added))
            {
                if (await NameTakenAsync(organizationId, roleId, name, ct))
                    return Fail<RoleUpdateOutcome>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");

                var request = new RoleChangeRequest(
                    Guid.NewGuid(), organizationId, roleId, actor.UserId, now, role.RowVersion, name, description,
                    JsonSerializer.Serialize(plan.Value.Wanted.Select(g => new RoleGrantInput(g.Key, g.Scope, g.BranchId)).ToList()));
                _db.RoleChangeRequests.Add(request);
                Audit(organizationId, actor, "roles.change-requested", "RoleChangeRequest", request.Id, traceId,
                    new { roleId, addedPermissionKeys = addedKeys, removedPermissionKeys = removedKeys }, null, request.RowVersion);
                await _db.SaveChangesAsync(ct);

                var pending = await LoadDefinitionAsync(organizationId, roleId, ct);
                return Result<RoleUpdateOutcome>.Success(new RoleUpdateOutcome(true, pending.Value!, pending.Value!.PendingChange));
            }

            var before = role.RowVersion;
            var changedFields = new List<string>();
            if (role.Name != name) changedFields.Add("name");
            if (role.Description != description) changedFields.Add("description");
            try
            {
                ApplyDefinition(role, organizationId, name, description, plan.Value, now);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleUpdateOutcome>(ex.Code, ex.Message);
            }

            Audit(organizationId, actor, "roles.updated", "Role", roleId, traceId,
                new { changedFields, addedPermissionKeys = addedKeys, removedPermissionKeys = removedKeys }, before, role.RowVersion);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleUpdateOutcome>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            var updated = await LoadDefinitionAsync(organizationId, roleId, ct);
            return Result<RoleUpdateOutcome>.Success(new RoleUpdateOutcome(false, updated.Value!, null));
        }, ct);
```

- [ ] **Step 5: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"`
Expected: PASS ทุกเคส. ถ้า `Update_RemovingUsersManage...` ล้มด้วย 200 ตั้งแต่ `blocked`: ตรวจว่า Org C user มี `FirebaseUid` และ Organization/Branch active (`LoadAdministratorRowsAsync` กรอง).

- [ ] **Step 6: Commit**

```bash
git add backend/src backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs
git commit -m "feat(roles): add role update with maker-checker

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 7: Mutation check — system immutability, maker–checker, last administrator (หลัง commit; ไม่ commit การทดลอง)**

ทีละการ mutation (แก้ → รัน → ต้องล้ม → `git checkout --` ไฟล์นั้น):
1. **System immutability:** ลบบรรทัด `if (role.IsSystem) return Fail<RoleUpdateOutcome>("ROLE_SYSTEM_IMMUTABLE", ...)` ใน `UpdateRoleAsync` **และ** แก้ `Role.EnsureMutable()` ให้ไม่ทำอะไร (ลบ `if (IsSystem) throw ...`) → รัน `--filter "FullyQualifiedName~Update_SystemRole_Returns409AndChangesNothing"` และ `dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~SystemRole_RejectsEveryMutation"` Expected: **FAIL ทั้งสอง**. (ลบเฉพาะ store ไม่พอ — domain ป้องกันซ้ำ ซึ่งเป็นความตั้งใจ.) คืนค่า: `git checkout -- backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs backend/src/TanErp.Domain/IdentityAccess/Role.cs`.
2. **Maker–checker:** ใน `RoleManagementPolicy.NeedsChecker` เปลี่ยนเป็น `=> false;` → รัน `--filter "FullyQualifiedName~Update_AddingApprovalPermissionToAHeldRole_CreatesAPendingRequest"` Expected: **FAIL** (ได้ 200). คืนค่า `git checkout -- backend/src/TanErp.Application/IdentityAccess/RoleManagement/RoleManagementPolicy.cs`.
3. **Last administrator:** ลบบล็อก `if (removed.Any(g => g.Key == AdministrationPermissions.UsersManage ...)) { ... }` ใน `PlanChangeAsync` → รัน `--filter "FullyQualifiedName~Update_RemovingUsersManageFromTheLastAdministratorRole"` Expected: **FAIL**. คืนค่าด้วย `git checkout -- <RoleManagementStore.cs>`.

หลังคืนทุกไฟล์: `git status --short` ว่าง และรัน filter ของ Task นี้ซ้ำ Expected: PASS.

---

## Task 9: API ตัดสินคำขอแก้ Role (approve / reject / cancel, คิวของผู้ตรวจ)

**Files:**
- Modify: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/{IRoleManagementStore,RoleManagementHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/IdentityAccess/RoleManagement/RoleManagementContracts.cs`, `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test**

```csharp
    // ------------------------------------------------------------------ change requests

    private async Task<(Guid RoleId, RoleChangeRequestResponse Request)> PendingChangeAsync(string name, params string[] addKeys)
    {
        var roleId = await SeedRoleAsync(name, new[] { "customers.read" });
        await SeedHolderAsync(roleId);
        var response = await PutRole(roleId, name, new[] { "customers.read" }.Concat(addKeys).ToArray());
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        return (roleId, (await ReadAs<UpdateRoleDefinitionResponse>(response)).PendingRequest!);
    }

    private Task<HttpResponseMessage> Decide(Guid requestId, string action, Guid ifMatch, string token) =>
        Send(HttpMethod.Post, $"/api/v1/admin/role-change-requests/{requestId}/{action}", token, ifMatch: ifMatch);

    [Fact]
    public async Task Approve_ByAnIndependentChecker_AppliesTheProposedDefinition()
    {
        var (roleId, request) = await PendingChangeAsync("Approve Me", "estimates.approve");

        var response = await Decide(request.Id, "approve", request.RowVersion, CheckerToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(RoleAssignmentRequestStatus.Approved, (await ReadAs<RoleChangeRequestResponse>(response)).Status);
        var role = await GetRole(roleId);
        Assert.Contains(role.Permissions, p => p.PermissionKey == "estimates.approve");
        Assert.Null(role.PendingChange);
        await using var db = NewDb();
        Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "roles.change-approved" && a.ResourceId == request.Id.ToString()));
    }

    [Fact]
    public async Task Approve_ByTheRequester_Returns403AndTheRoleStaysUnchanged()
    {
        var (roleId, request) = await PendingChangeAsync("Self Approve", "estimates.approve");

        var response = await Decide(request.Id, "approve", request.RowVersion, AdminToken); // the maker also holds roles.assign-approval

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", await ReadCode(response));
        Assert.DoesNotContain((await GetRole(roleId)).Permissions, p => p.PermissionKey == "estimates.approve");
    }

    [Fact]
    public async Task Decisions_RequireTheApprovalPermission_AndTheQueueNeverCrossesOrganizations()
    {
        var (_, request) = await PendingChangeAsync("Queue Role", "estimates.approve");

        var approve = await Decide(request.Id, "approve", request.RowVersion, ViewerToken);
        var list = await Send(HttpMethod.Get, "/api/v1/admin/role-change-requests", ViewerToken);
        var mine = await ReadAs<RoleChangeRequestListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-change-requests", CheckerToken));
        var foreign = await ReadAs<RoleChangeRequestListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-change-requests", OrgBToken));
        var foreignDecide = await Decide(request.Id, "approve", request.RowVersion, OrgBToken);

        Assert.Equal("PERMISSION_DENIED", await ReadCode(approve));
        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(new[] { "estimates.approve" }, mine.Items.Single(i => i.Id == request.Id).AddedPermissionKeys.ToArray());
        Assert.Empty(foreign.Items);
        Assert.Equal(HttpStatusCode.NotFound, foreignDecide.StatusCode);
    }

    [Fact]
    public async Task Approve_ByACheckerWhoLacksTheAddedPermission_Returns403AndStaysPending()
    {
        var (roleId, request) = await PendingChangeAsync("Beyond Checker", "mrp.approve"); // the checker holds no mrp.approve

        var response = await Decide(request.Id, "approve", request.RowVersion, CheckerToken);

        Assert.Equal("ROLE_ESCALATION_DENIED", await ReadCode(response));
        Assert.NotNull((await GetRole(roleId)).PendingChange);
    }

    [Fact]
    public async Task Approve_WhenTheRoleMovedSinceTheRequest_Returns409Stale()
    {
        var (roleId, request) = await PendingChangeAsync("Moved Role", "estimates.approve");
        await using (var db = NewDb())
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE identity_access.roles SET row_version = {Guid.NewGuid()} WHERE id = {roleId}");

        var response = await Decide(request.Id, "approve", request.RowVersion, CheckerToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ROLE_CHANGE_REQUEST_STALE", await ReadCode(response));
    }

    [Fact]
    public async Task Reject_KeepsTheRoleAndUnblocksNewUpdates_AndAnAlreadyDecidedRequestIs409()
    {
        var (roleId, request) = await PendingChangeAsync("Reject Me", "estimates.approve");

        var reject = await Decide(request.Id, "reject", request.RowVersion, CheckerToken);
        var decidedAgain = await Decide(request.Id, "approve", (await ReadAs<RoleChangeRequestResponse>(reject)).RowVersion, CheckerToken);
        var stale = await Decide(request.Id, "reject", Guid.NewGuid(), CheckerToken);
        var update = await PutRole(roleId, "Reject Me", new[] { "customers.read", "quotations.read" });

        Assert.Equal(HttpStatusCode.OK, reject.StatusCode);
        Assert.Equal("ROLE_CHANGE_REQUEST_NOT_PENDING", await ReadCode(decidedAgain));
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
    }

    [Fact]
    public async Task Cancel_IsOnlyForTheRequester()
    {
        var (_, request) = await PendingChangeAsync("Cancel Me", "estimates.approve");

        var byOther = await Decide(request.Id, "cancel", request.RowVersion, CheckerToken); // holds roles.manage but is not the requester
        var byRequester = await Decide(request.Id, "cancel", request.RowVersion, AdminToken);

        Assert.Equal(HttpStatusCode.Forbidden, byOther.StatusCode);
        Assert.Equal("PERMISSION_DENIED", await ReadCode(byOther));
        Assert.Equal(RoleAssignmentRequestStatus.Cancelled, (await ReadAs<RoleChangeRequestResponse>(byRequester)).Status);
    }
```
Run → build FAIL (`RoleChangeRequestListResponse` ไม่มี).

- [ ] **Step 2: Application — port + handler**

port:

```csharp
    Task<IReadOnlyList<RoleChangeRequestView>> ListChangeRequestsAsync(Guid organizationId, string status, CancellationToken ct);

    Task<Result<RoleChangeRequestView>> DecideChangeRequestAsync(
        Guid organizationId, Guid requestId, Guid ifMatch, RoleRequestDecision decision, AdminActor actor, string traceId, CancellationToken ct);
```
handler:

```csharp
    private static readonly HashSet<string> RequestStatuses = new(StringComparer.Ordinal) { "pending", "approved", "rejected", "cancelled" };

    public async Task<Result<IReadOnlyList<RoleChangeRequestView>>> ListChangeRequestsAsync(AdminCaller caller, string? status, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesAssignApproval, ct);
        if (access.IsFailure) return Result<IReadOnlyList<RoleChangeRequestView>>.Failure(access.Error);

        var wanted = string.IsNullOrWhiteSpace(status) ? "pending" : status.Trim();
        if (!RequestStatuses.Contains(wanted)) return AdminHandlerSupport.Validation<IReadOnlyList<RoleChangeRequestView>>("Unknown status filter.");
        return Result<IReadOnlyList<RoleChangeRequestView>>.Success(await _store.ListChangeRequestsAsync(access.Value!.OrganizationId, wanted, ct));
    }

    /// <summary>Cancelling is the requester's own action (roles.manage); approving or rejecting is the checker's (roles.assign-approval).</summary>
    public async Task<Result<RoleChangeRequestView>> DecideChangeRequestAsync(
        AdminCaller caller, Guid requestId, Guid ifMatch, RoleRequestDecision decision, string traceId, CancellationToken ct)
    {
        var permission = decision == RoleRequestDecision.Cancel ? RoleManagementPermissions.RolesManage : RoleManagementPermissions.RolesAssignApproval;
        var access = await AccessAsync(caller, permission, ct);
        return access.IsFailure
            ? Denied<RoleChangeRequestView>(access)
            : await _store.DecideChangeRequestAsync(access.Value!.OrganizationId, requestId, ifMatch, decision, Actor(access.Value), traceId, ct);
    }
```

- [ ] **Step 3: Contracts + controller + error codes**

เพิ่ม `public record RoleChangeRequestListResponse(IReadOnlyList<RoleChangeRequestResponse> Items);` ใน contracts. controller (ต้อง `using TanErp.Application.IdentityAccess.Administration;` มีแล้วสำหรับ `RoleRequestDecision`):

```csharp
    [HttpGet("role-change-requests")]
    [ProducesResponseType<RoleChangeRequestListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListChangeRequests([FromQuery] string? status = null, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var result = await _handler.ListChangeRequestsAsync(Caller(auth.Value!), status, cancellationToken);
        return result.IsFailure
            ? Problem(result.Error)
            : Ok(new RoleChangeRequestListResponse(result.Value!.Select(RoleChangeRequestResponse.From).ToList()));
    }

    [HttpPost("role-change-requests/{requestId:guid}/approve")]
    [ProducesResponseType<RoleChangeRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> ApproveChangeRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideChangeRequest(requestId, RoleRequestDecision.Approve, cancellationToken);

    [HttpPost("role-change-requests/{requestId:guid}/reject")]
    [ProducesResponseType<RoleChangeRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> RejectChangeRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideChangeRequest(requestId, RoleRequestDecision.Reject, cancellationToken);

    [HttpPost("role-change-requests/{requestId:guid}/cancel")]
    [ProducesResponseType<RoleChangeRequestResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> CancelChangeRequest([FromRoute] Guid requestId, CancellationToken cancellationToken) =>
        DecideChangeRequest(requestId, RoleRequestDecision.Cancel, cancellationToken);

    private async Task<IActionResult> DecideChangeRequest(Guid requestId, RoleRequestDecision decision, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        var result = await _handler.DecideChangeRequestAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), requestId, conditional.Value.IfMatchRowVersion,
            decision, HttpContext.TraceIdentifier, cancellationToken);
        return result.IsFailure ? Problem(result.Error) : Ok(RoleChangeRequestResponse.From(result.Value!));
    }
```
`GetStatus`: `ROLE_CHANGE_REQUEST_NOT_PENDING` 409, `ROLE_CHANGE_REQUEST_STALE` 409, `ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED` 403. resx:

| key | th | en |
| --- | --- | --- |
| `ROLE_CHANGE_REQUEST_NOT_PENDING` | คำขอถูกตัดสินแล้ว / คำขอแก้ไขบทบาทนี้ถูกตัดสินไปแล้ว | Request already decided / This role change request has already been decided. |
| `ROLE_CHANGE_REQUEST_STALE` | บทบาทเปลี่ยนไปตั้งแต่ขอ / บทบาทถูกเปลี่ยนหลังจากส่งคำขอ ให้ปฏิเสธคำขอนี้แล้วขอใหม่ | Role changed since the request / The role changed after the request was made. Reject it and request again. |
| `ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED` | ต้องเป็นผู้ตรวจอีกคน / การตัดสินคำขอต้องทำโดยผู้ใช้คนอื่นที่ไม่ใช่ผู้ขอ | An independent checker is required / The decision must be made by someone other than the requester. |

- [ ] **Step 4: Store**

```csharp
    public async Task<IReadOnlyList<RoleChangeRequestView>> ListChangeRequestsAsync(Guid organizationId, string status, CancellationToken ct)
    {
        var rows = await ChangeRequestQuery(organizationId, c => c.Status == status).Take(200).ToListAsync(ct);
        var views = new List<RoleChangeRequestView>();
        foreach (var row in rows)
            views.Add(ToChangeView(organizationId, row, await LoadGrantViewsAsync(row.Request.RoleId, ct)));
        return views;
    }

    public Task<Result<RoleChangeRequestView>> DecideChangeRequestAsync(
        Guid organizationId, Guid requestId, Guid ifMatch, RoleRequestDecision decision, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var request = await _db.RoleChangeRequests.FirstOrDefaultAsync(c => c.Id == requestId && c.OrganizationId == organizationId, ct);
            if (request is null) return NotFound<RoleChangeRequestView>("Role change request");
            if (request.RowVersion != ifMatch) return Fail<RoleChangeRequestView>(VersionConflict, "The request was modified by another user.");
            if (!request.IsPending) return Fail<RoleChangeRequestView>("ROLE_CHANGE_REQUEST_NOT_PENDING", "The request has already been decided.");

            var now = _clock.UtcNow;
            var before = request.RowVersion;
            string action;
            object changes = new { roleId = request.RoleId };

            if (decision == RoleRequestDecision.Cancel)
            {
                if (!request.TryCancel(actor.UserId, now)) return Fail<RoleChangeRequestView>("PERMISSION_DENIED", "Only the requester can cancel a request.");
                action = "roles.change-cancelled";
            }
            else
            {
                if (actor.UserId == request.RequestedByUserId)
                    return Fail<RoleChangeRequestView>("ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", "The decision must be made by someone other than the requester.");

                if (decision == RoleRequestDecision.Reject)
                {
                    request.TryReject(actor.UserId, now);
                    action = "roles.change-rejected";
                }
                else
                {
                    var role = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                        .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.OrganizationId == organizationId, ct);
                    if (role is null) return NotFound<RoleChangeRequestView>("Role");
                    if (role.RowVersion != request.BaseRoleRowVersion)
                        return Fail<RoleChangeRequestView>("ROLE_CHANGE_REQUEST_STALE", "The role changed after the request was made.");

                    // The checker is the one handing out the added permissions, so the checker must hold them (anti-escalation).
                    var plan = await PlanChangeAsync(role, organizationId, ParseGrants(request.ProposedGrantsJson), actor, ct);
                    if (plan.IsFailure) return Result<RoleChangeRequestView>.Failure(plan.Error);
                    try
                    {
                        ApplyDefinition(role, organizationId, request.ProposedName, request.ProposedDescription, plan.Value!, now);
                    }
                    catch (RoleDomainException ex)
                    {
                        return Fail<RoleChangeRequestView>(ex.Code, ex.Message);
                    }

                    request.TryApprove(actor.UserId, now);
                    action = "roles.change-approved";
                    changes = new { roleId = request.RoleId, addedPermissionKeys = KeysOf(plan.Value!.Added), removedPermissionKeys = KeysOf(plan.Value.Removed) };
                }
            }

            Audit(organizationId, actor, action, "RoleChangeRequest", request.Id, traceId, changes, before, request.RowVersion);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleChangeRequestView>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            _db.ChangeTracker.Clear();
            var row = await ChangeRequestQuery(organizationId, c => c.Id == requestId).FirstAsync(ct);
            return Result<RoleChangeRequestView>.Success(ToChangeView(organizationId, row, await LoadGrantViewsAsync(row.Request.RoleId, ct)));
        }, ct);
```

- [ ] **Step 5: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"` Expected: PASS ทุกเคส.

- [ ] **Step 6: Commit**

```bash
git add backend/src backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs
git commit -m "feat(roles): add role change request decisions

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 7: Mutation check — ผู้ตรวจอิสระและการตรวจ Covers ตอนอนุมัติ (หลัง commit; ไม่ commit การทดลอง)**

1. ลบบล็อก `if (actor.UserId == request.RequestedByUserId) return Fail<...>("ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", ...)` ใน `DecideChangeRequestAsync` → รัน `--filter "FullyQualifiedName~Approve_ByTheRequester_Returns403"` Expected: **FAIL** (constraint `ck_role_change_requests_independent_checker` ของ DB ทำให้ได้ 500 แทน 403 — ชั้น DB เป็นแนวป้องกันที่สอง). คืนค่า `git checkout -- backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`.
2. ใน `PlanChangeAsync` เปลี่ยน `AdministrationPolicy.Covers(held, organizationId, added)` เป็น `true` → รัน `--filter "FullyQualifiedName~Approve_ByACheckerWhoLacksTheAddedPermission|FullyQualifiedName~Update_AddingAPermissionTheCallerDoesNotHold"` Expected: **FAIL ทั้งสอง**. คืนค่าด้วย `git checkout`.

หลังคืน: `git status --short` ว่าง และ filter ของ Task นี้ผ่านอีกครั้ง.

---

## Task 10: API ปิด/เปิด Role (`ROLE_IN_USE`)

**Files:**
- Modify: `backend/src/TanErp.Application/IdentityAccess/RoleManagement/{IRoleManagementStore,RoleManagementHandler}.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`
- Modify: `backend/src/TanErp.Api/Controllers/AdminRolesController.cs`, `ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs`

- [ ] **Step 1: เขียน failing integration test**

```csharp
    // ------------------------------------------------------------------ activation

    private Task<HttpResponseMessage> SetActive(Guid roleId, string action, Guid ifMatch, string token = AdminToken) =>
        Send(HttpMethod.Post, $"/api/v1/admin/role-definitions/{roleId}/{action}", token, ifMatch: ifMatch);

    [Fact]
    public async Task Deactivate_AnUnheldRole_Succeeds_DropsOutOfTheAssignPicker_AndCanBeActivatedAgain()
    {
        var roleId = await SeedRoleAsync("Retire Me", new[] { "customers.read" });
        var role = await GetRole(roleId);

        var deactivate = await SetActive(roleId, "deactivate", role.RowVersion);
        var picker = await Send(HttpMethod.Get, "/api/v1/admin/roles", AdminToken);

        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        var deactivated = await ReadAs<RoleDefinitionResponse>(deactivate);
        Assert.False(deactivated.IsActive);
        Assert.DoesNotContain("Retire Me", await picker.Content.ReadAsStringAsync());

        var activate = await SetActive(roleId, "activate", deactivated.RowVersion);
        Assert.True((await ReadAs<RoleDefinitionResponse>(activate)).IsActive);
        await using var db = NewDb();
        Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "roles.deactivated" && a.ResourceId == roleId.ToString()));
        Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "roles.activated" && a.ResourceId == roleId.ToString()));
    }

    [Fact]
    public async Task Deactivate_AHeldRole_Returns409RoleInUse_AndKeepsItActive()
    {
        var roleId = await SeedRoleAsync("In Use", new[] { "customers.read" });
        await SeedHolderAsync(roleId);

        var response = await SetActive(roleId, "deactivate", (await GetRole(roleId)).RowVersion);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ROLE_IN_USE", await ReadCode(response));
        Assert.True((await GetRole(roleId)).IsActive);
    }

    [Fact]
    public async Task Deactivate_WithAPendingAssignmentRequest_OrPendingChange_OrSystemRole_IsRefused()
    {
        var withRequest = await SeedRoleAsync("Has Assignment Request", new[] { "estimates.approve" });
        await using (var db = NewDb())
        {
            db.RoleAssignmentRequests.Add(new RoleAssignmentRequest(
                Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, ViewerMembershipId, withRequest, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }

        var (withChange, _) = await PendingChangeAsync("Has Change Request", "estimates.approve");
        Guid systemRoleId;
        await using (var db = NewDb()) systemRoleId = (await db.Roles.SingleAsync(r => r.Name == "Test Admin" && r.OrganizationId == TestOnlyDataSeeder.TestOrgId)).Id;

        var assignment = await SetActive(withRequest, "deactivate", (await GetRole(withRequest)).RowVersion);
        var change = await SetActive(withChange, "deactivate", (await GetRole(withChange)).RowVersion);
        var system = await SetActive(systemRoleId, "deactivate", (await GetRole(systemRoleId)).RowVersion);

        Assert.Equal("ROLE_IN_USE", await ReadCode(assignment));
        Assert.Equal("ROLE_CHANGE_REQUEST_PENDING", await ReadCode(change));
        Assert.Equal("ROLE_SYSTEM_IMMUTABLE", await ReadCode(system));
    }

    [Fact]
    public async Task SetActive_RequiresRolesManageAndIfMatch_AndStaysInsideTheOrganization()
    {
        var roleId = await SeedRoleAsync("Guarded Toggle");
        var foreign = await SeedRoleAsync("Foreign Toggle", orgId: TestOnlyDataSeeder.TestOrgBId);
        var version = (await GetRole(roleId)).RowVersion;

        var denied = await SetActive(roleId, "deactivate", version, ViewerToken);
        var stale = await SetActive(roleId, "deactivate", Guid.NewGuid());
        var missing = await Send(HttpMethod.Post, $"/api/v1/admin/role-definitions/{roleId}/deactivate", AdminToken);
        var otherOrg = await SetActive(foreign, "deactivate", Guid.NewGuid());

        Assert.Equal("PERMISSION_DENIED", await ReadCode(denied));
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, otherOrg.StatusCode);
    }
```
Run → FAIL (404 route ไม่มี; ไม่ใช่ compile error เพราะใช้ HTTP).

- [ ] **Step 2: Application — port + handler**

port: `Task<Result<RoleDefinition>> SetRoleActiveAsync(Guid organizationId, Guid roleId, bool active, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct);`

handler:

```csharp
    public async Task<Result<RoleDefinition>> SetRoleActiveAsync(AdminCaller caller, Guid roleId, bool active, Guid ifMatch, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        return access.IsFailure
            ? Denied<RoleDefinition>(access)
            : await _store.SetRoleActiveAsync(access.Value!.OrganizationId, roleId, active, ifMatch, Actor(access.Value), traceId, ct);
    }
```

- [ ] **Step 3: Store, controller, error code**

store:

```csharp
    public Task<Result<RoleDefinition>> SetRoleActiveAsync(
        Guid organizationId, Guid roleId, bool active, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
            if (role is null) return NotFound<RoleDefinition>("Role");
            if (role.RowVersion != ifMatch) return Fail<RoleDefinition>(VersionConflict, "The role was modified by another user.");
            if (role.IsSystem) return Fail<RoleDefinition>("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
            if (role.IsActive == active) return await LoadDefinitionAsync(organizationId, roleId, ct);

            if (!active)
            {
                // Deactivation silently revokes the role from every holder (an inactive role grants nothing), so it is refused while in use.
                if (await HasPendingChangeAsync(roleId, ct))
                    return Fail<RoleDefinition>("ROLE_CHANGE_REQUEST_PENDING", "A change request is pending for this role.");
                var inUse = await _db.MembershipRoles.AnyAsync(mr => mr.RoleId == roleId, ct)
                    || await _db.RoleAssignmentRequests.AnyAsync(r => r.RoleId == roleId && r.Status == RoleAssignmentRequestStatus.Pending, ct);
                if (inUse) return Fail<RoleDefinition>("ROLE_IN_USE", "The role is held by users or has pending assignment requests.");
            }

            var before = role.RowVersion;
            if (active) role.Activate(); else role.Deactivate();
            Audit(organizationId, actor, active ? "roles.activated" : "roles.deactivated", "Role", roleId, traceId, new { roleId }, before, role.RowVersion);
            await _db.SaveChangesAsync(ct);
            return await LoadDefinitionAsync(organizationId, roleId, ct);
        }, ct);
```
controller:

```csharp
    [HttpPost("role-definitions/{roleId:guid}/deactivate")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> DeactivateRole([FromRoute] Guid roleId, CancellationToken cancellationToken) => SetRoleActive(roleId, false, cancellationToken);

    [HttpPost("role-definitions/{roleId:guid}/activate")]
    [ProducesResponseType<RoleDefinitionResponse>(StatusCodes.Status200OK)]
    public Task<IActionResult> ActivateRole([FromRoute] Guid roleId, CancellationToken cancellationToken) => SetRoleActive(roleId, true, cancellationToken);

    private async Task<IActionResult> SetRoleActive(Guid roleId, bool active, CancellationToken cancellationToken)
    {
        var conditional = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (conditional.IsFailure) return Problem(conditional.Error);

        return RoleResult(await _handler.SetRoleActiveAsync(
            new AdminCaller(conditional.Value!.FirebaseUid, conditional.Value.MembershipId), roleId, active,
            conditional.Value.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken));
    }
```
`GetStatus`: `"ROLE_IN_USE" => StatusCodes.Status409Conflict,`; resx: `ROLE_IN_USE` th: `ปิดใช้งานบทบาทไม่ได้ / บทบาทนี้ยังมีผู้ถืออยู่หรือมีคำขอมอบบทบาทที่รออยู่ ให้ถอนบทบาทและจัดการคำขอก่อน` en: `Cannot deactivate role / The role is still held by users or has pending assignment requests. Revoke them first.`

- [ ] **Step 4: Run — ต้องผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~RoleManagementEndpointsTests"` Expected: PASS ทั้งหมด. แล้วรัน regression ของ CP-02: `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests"` Expected: PASS (พฤติกรรมเดิมไม่เปลี่ยน แม้ขยาย approval set).

- [ ] **Step 5: Commit**

```bash
git add backend/src backend/tests/TanErp.IntegrationTests/Api/RoleManagementEndpointsTests.cs
git commit -m "feat(roles): add role deactivation and activation

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

- [ ] **Step 6: Mutation check — ROLE_IN_USE (หลัง commit; ไม่ commit การทดลอง)**

ลบบรรทัด `if (inUse) return Fail<RoleDefinition>("ROLE_IN_USE", ...)` → รัน `--filter "FullyQualifiedName~Deactivate_AHeldRole_Returns409RoleInUse"` Expected: **FAIL**. คืนค่า `git checkout -- backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/RoleManagementStore.cs`; `git status --short` ว่าง.

---

## Task 11: OpenAPI snapshot, generated types และ API client

**กลไกที่ใช้:** `OpenApiContractTests` เขียน `contracts/openapi/tan-erp.v1.json` เมื่อตั้ง `UPDATE_OPENAPI=1` (`OpenApiContractTests.cs:254`) แล้ว `npm run generate:api` สร้าง `frontend/src/generated/api/tan-erp.v1.ts` — เหมือนที่ commit `8a8be98` เปลี่ยนไฟล์ทั้งสองคู่กัน (ไม่ใช้ live swagger).

**Files:**
- Regenerate: `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`, `frontend/src/lib/api/api-client.test.ts`

- [ ] **Step 1: regenerate snapshot (Backend)**

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"`
Run: `git diff --stat contracts/openapi/tan-erp.v1.json` → Expected: เพิ่มเท่านั้น (paths `/api/v1/admin/permissions`, `/role-definitions`, `/role-definitions/{roleId}`, `/deactivate`, `/activate`, `/role-change-requests`, `/{requestId}/approve|reject|cancel` และ schemas `PermissionCatalogResponse`, `RoleDefinitionListResponse`, `RoleDefinitionSummaryResponse`, `RoleDefinitionResponse`, `RoleGrantResponse`, `RoleGrantRequest`, `SaveRoleDefinitionRequest`, `UpdateRoleDefinitionResponse`, `RoleChangeRequestResponse`, `RoleChangeRequestListResponse`). ตรวจไม่มีบรรทัดถูกลบ: `git diff contracts/openapi/tan-erp.v1.json | grep '^-[^-]' | wc -l` → Expected: `0`.
Run (ไม่มี `UPDATE_OPENAPI`): `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"` → Expected: PASS.

- [ ] **Step 2: generate FE types**

Run: `cd frontend && npm run generate:api && npm run check:api`
Expected: `check:api` exit 0; `grep -c "SaveRoleDefinitionRequest" src/generated/api/tan-erp.v1.ts` ≥ 1.

- [ ] **Step 3: เขียน failing test ของ client**

ต่อท้าย `describe("ApiClient")` ใน `api-client.test.ts`:

```ts
  it("calls the role management endpoints with If-Match and Idempotency-Key where required", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => ({}) });
    global.fetch = fetchMock;
    const client = new ApiClient("http://localhost:5000");
    const options = { token: "tok", membershipId: "m-1", locale: "th" as const };
    const role = { name: "Sales Lead", description: null, permissions: [{ permissionKey: "customers.read", scope: "organization", branchId: null }] };

    await client.listAdminPermissions(options);
    await client.listRoleDefinitions({ search: "lead", status: "active", sortBy: "name", sortOrder: "asc", page: 2, limit: 10 }, options);
    await client.getRoleDefinition("r 1", options);
    await client.createRoleDefinition(role, { ...options, idempotencyKey: "key-0123456789abcdef" });
    await client.updateRoleDefinition("r1", role, { ...options, ifMatch: "v1" });
    await client.setRoleDefinitionActive("r1", false, { ...options, ifMatch: "v2" });
    await client.setRoleDefinitionActive("r1", true, { ...options, ifMatch: "v3" });
    await client.listRoleChangeRequests("pending", options);
    await client.decideRoleChangeRequest("q1", "approve", { ...options, ifMatch: "v4" });

    expect(fetchMock.mock.calls.map(([url, init]) => [url, init.method])).toEqual([
      ["http://localhost:5000/api/v1/admin/permissions", "GET"],
      ["http://localhost:5000/api/v1/admin/role-definitions?search=lead&status=active&sortBy=name&sortOrder=asc&page=2&limit=10", "GET"],
      ["http://localhost:5000/api/v1/admin/role-definitions/r%201", "GET"],
      ["http://localhost:5000/api/v1/admin/role-definitions", "POST"],
      ["http://localhost:5000/api/v1/admin/role-definitions/r1", "PUT"],
      ["http://localhost:5000/api/v1/admin/role-definitions/r1/deactivate", "POST"],
      ["http://localhost:5000/api/v1/admin/role-definitions/r1/activate", "POST"],
      ["http://localhost:5000/api/v1/admin/role-change-requests?status=pending", "GET"],
      ["http://localhost:5000/api/v1/admin/role-change-requests/q1/approve", "POST"],
    ]);
    expect(fetchMock.mock.calls[3][1].headers["Idempotency-Key"]).toBe("key-0123456789abcdef");
    expect(fetchMock.mock.calls[4][1].headers["If-Match"]).toBe('"v1"');
    expect(fetchMock.mock.calls[8][1].headers["If-Match"]).toBe('"v4"');
  });
```
Run: `cd frontend && npx vitest run src/lib/api/api-client.test.ts` → Expected: FAIL (`client.listAdminPermissions is not a function`).

- [ ] **Step 4: types + methods**

ใน `api-client.ts` ใกล้ `BranchDeactivationCheckResponse` (:106) เพิ่ม:

```ts
export type PermissionCatalogResponse = components["schemas"]["PermissionCatalogResponse"];
export type RoleDefinitionListResponse = components["schemas"]["RoleDefinitionListResponse"];
export type RoleDefinitionSummaryResponse = components["schemas"]["RoleDefinitionSummaryResponse"];
export type RoleDefinitionResponse = components["schemas"]["RoleDefinitionResponse"];
export type RoleGrantResponse = components["schemas"]["RoleGrantResponse"];
export type RoleGrantRequest = components["schemas"]["RoleGrantRequest"];
export type SaveRoleDefinitionRequest = components["schemas"]["SaveRoleDefinitionRequest"];
export type UpdateRoleDefinitionResponse = components["schemas"]["UpdateRoleDefinitionResponse"];
export type RoleChangeRequestResponse = components["schemas"]["RoleChangeRequestResponse"];
export type RoleChangeRequestListResponse = components["schemas"]["RoleChangeRequestListResponse"];
export type RoleDefinitionListParams = NonNullable<paths["/api/v1/admin/role-definitions"]["get"]["parameters"]["query"]>;
```

ในคลาส `ApiClient` ถัดจาก `decideAdminRoleRequest`:

```ts
  async listAdminPermissions(options: RequestOptions): Promise<PermissionCatalogResponse> {
    return this.request<PermissionCatalogResponse>("/api/v1/admin/permissions", "GET", options);
  }

  async listRoleDefinitions(params: RoleDefinitionListParams, options: RequestOptions): Promise<RoleDefinitionListResponse> {
    const query = new URLSearchParams();
    if (params.search) query.set("search", params.search);
    if (params.status) query.set("status", params.status);
    if (params.sortBy) query.set("sortBy", params.sortBy);
    if (params.sortOrder) query.set("sortOrder", params.sortOrder);
    if (params.page) query.set("page", String(params.page));
    if (params.limit) query.set("limit", String(params.limit));
    const suffix = query.toString();
    return this.request<RoleDefinitionListResponse>(`/api/v1/admin/role-definitions${suffix ? `?${suffix}` : ""}`, "GET", options);
  }

  async getRoleDefinition(roleId: string, options: RequestOptions): Promise<RoleDefinitionResponse> {
    return this.request<RoleDefinitionResponse>(`/api/v1/admin/role-definitions/${encodeURIComponent(roleId)}`, "GET", options);
  }

  async createRoleDefinition(payload: SaveRoleDefinitionRequest, options: RequestOptions): Promise<RoleDefinitionResponse> {
    return this.request<RoleDefinitionResponse>("/api/v1/admin/role-definitions", "POST", options, payload);
  }

  async updateRoleDefinition(
    roleId: string,
    payload: SaveRoleDefinitionRequest,
    options: RequestOptions
  ): Promise<UpdateRoleDefinitionResponse> {
    return this.request<UpdateRoleDefinitionResponse>(
      `/api/v1/admin/role-definitions/${encodeURIComponent(roleId)}`, "PUT", options, payload);
  }

  async setRoleDefinitionActive(roleId: string, active: boolean, options: RequestOptions): Promise<RoleDefinitionResponse> {
    return this.request<RoleDefinitionResponse>(
      `/api/v1/admin/role-definitions/${encodeURIComponent(roleId)}/${active ? "activate" : "deactivate"}`, "POST", options);
  }

  async listRoleChangeRequests(status: string, options: RequestOptions): Promise<RoleChangeRequestListResponse> {
    return this.request<RoleChangeRequestListResponse>(
      `/api/v1/admin/role-change-requests?status=${encodeURIComponent(status)}`, "GET", options);
  }

  async decideRoleChangeRequest(
    requestId: string,
    decision: "approve" | "reject" | "cancel",
    options: RequestOptions
  ): Promise<RoleChangeRequestResponse> {
    return this.request<RoleChangeRequestResponse>(
      `/api/v1/admin/role-change-requests/${encodeURIComponent(requestId)}/${decision}`, "POST", options);
  }
```
Run: `cd frontend && npx vitest run src/lib/api/api-client.test.ts && npm run typecheck` → Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api/api-client.ts frontend/src/lib/api/api-client.test.ts
git commit -m "feat(roles): add role management api client

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 12: Frontend foundation — i18n, error whitelist และ query hooks

**Files:**
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json`
- Create: `frontend/src/features/settings/role-management/role-management-errors.ts` (+ `.test.ts`)
- Create: `frontend/src/features/settings/role-management/api/role-management-queries.ts`

- [ ] **Step 1: เขียน failing test (error whitelist + parity th/en)**

สร้าง `role-management-errors.test.ts`:

```ts
import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { KNOWN_ROLE_ADMIN_ERROR_CODES, roleAdminErrorKey } from "./role-management-errors";

function leafKeys(value: unknown, prefix = ""): string[] {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => leafKeys(child, prefix ? `${prefix}.${key}` : key));
}

describe("roleAdminErrorKey", () => {
  it("maps known backend codes and falls back to GENERIC", () => {
    expect(roleAdminErrorKey(new ApiError({ status: 409, code: "ROLE_IN_USE", message: "x" }))).toBe("ROLE_IN_USE");
    expect(roleAdminErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(roleAdminErrorKey(new Error("ROLE_IN_USE"))).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code plus GENERIC, and identical key sets", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of [...KNOWN_ROLE_ADMIN_ERROR_CODES, "GENERIC"]) {
        expect(messages.roleAdmin.errors).toHaveProperty(code);
      }
    }
    expect(leafKeys(thMessages.roleAdmin).sort()).toEqual(leafKeys(enMessages.roleAdmin).sort());
  });
});
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/role-management-errors.test.ts` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: error whitelist**

สร้าง `role-management-errors.ts`:

```ts
import { ApiError } from "@/lib/api/api-error";

/** Backend codes that have a dedicated message in `roleAdmin.errors`. Anything else shows the generic message. */
export const KNOWN_ROLE_ADMIN_ERROR_CODES = [
  "ADMIN_VERSION_CONFLICT",
  "ROLE_NAME_ALREADY_EXISTS",
  "ROLE_NAME_RESERVED",
  "ROLE_PERMISSION_UNKNOWN",
  "ROLE_ESCALATION_DENIED",
  "ROLE_SYSTEM_IMMUTABLE",
  "ROLE_IN_USE",
  "ROLE_CHANGE_REQUEST_PENDING",
  "ROLE_CHANGE_REQUEST_NOT_PENDING",
  "ROLE_CHANGE_REQUEST_STALE",
  "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED",
  "LAST_ADMINISTRATOR_REQUIRED",
  "BRANCH_NOT_FOUND",
  "IDEMPOTENCY_KEY_REUSED",
  "PERMISSION_DENIED",
  "RESOURCE_NOT_FOUND",
] as const;

const KNOWN = new Set<string>(KNOWN_ROLE_ADMIN_ERROR_CODES);

/** Returns the key under `roleAdmin.errors` for a failed role administration call. */
export function roleAdminErrorKey(error: unknown): string {
  return error instanceof ApiError && KNOWN.has(error.code) ? error.code : "GENERIC";
}
```

- [ ] **Step 3: เพิ่ม messages th/en (สคริปต์ guard round-trip)**

รันจาก `frontend/`:

```bash
python3 - <<'PY'
import json

TH = {
  "accessDeniedTitle": "ไม่มีสิทธิ์เข้าถึง",
  "accessDeniedDetail": "คุณไม่มีสิทธิ์จัดการบทบาทและสิทธิ์",
  "list": {
    "title": "บทบาทและสิทธิ์", "subtitle": "สร้างและแก้ไขบทบาทจากรายการสิทธิ์ที่ระบบกำหนด", "createRole": "เพิ่มบทบาท",
    "searchPlaceholder": "ค้นหาชื่อหรือคำอธิบายบทบาท", "statusFilter": "สถานะ", "status": {"active": "ใช้งาน", "inactive": "ปิดใช้งาน"},
    "table": {"name": "บทบาท", "members": "ผู้ถือบทบาท", "permissions": "สิทธิ์", "status": "สถานะ"},
    "badges": {"system": "ระบบ", "requiresApproval": "ต้องมีผู้ตรวจอนุมัติ", "pendingChange": "รอการอนุมัติการแก้ไข"},
    "emptyTitle": "ยังไม่มีบทบาท", "emptyDescription": "เพิ่มบทบาทแรกเพื่อกำหนดสิทธิ์การใช้งาน"
  },
  "editor": {
    "createTitle": "เพิ่มบทบาท", "createSubtitle": "เลือกสิทธิ์จากรายการที่ระบบกำหนด", "editTitle": "แก้ไขบทบาท",
    "createSubmit": "เพิ่มบทบาท", "saveSubmit": "บันทึกการเปลี่ยนแปลง", "createSuccess": "เพิ่มบทบาทแล้ว", "saveSuccess": "บันทึกบทบาทแล้ว",
    "pendingSubmitted": "ส่งคำขอแก้ไขให้ผู้ตรวจอนุมัติแล้ว", "backToList": "กลับไปรายการบทบาท",
    "systemReadOnly": "บทบาทของระบบแก้ไขไม่ได้",
    "pendingBanner": "มีคำขอแก้ไขรออนุมัติ จึงแก้ไขหรือปิดใช้งานบทบาทนี้ไม่ได้จนกว่าจะตัดสินคำขอ",
    "approvalNotice": "การเพิ่มสิทธิ์อนุมัติให้บทบาทที่มีผู้ถืออยู่ต้องให้ผู้ตรวจอีกคนอนุมัติก่อนมีผล",
    "fields": {"name": "ชื่อบทบาท", "description": "คำอธิบาย"},
    "validation": {"nameRequired": "กรุณากรอกชื่อบทบาท", "nameLength": "ชื่อบทบาทต้องมี 3-100 ตัวอักษร"},
    "permissions": {"title": "สิทธิ์ของบทบาท", "searchPlaceholder": "ค้นหาสิทธิ์", "approvalTag": "สิทธิ์อนุมัติ", "noMatch": "ไม่พบสิทธิ์ที่ค้นหา",
                    "selectedCount": "เลือกแล้ว {count} สิทธิ์", "scopedTitle": "สิทธิ์ที่จำกัดขอบเขต (คงไว้ตามเดิม)",
                    "scopedHint": "สิทธิ์ระดับสาขาหรือของตนเองเพิ่มผ่านหน้านี้ไม่ได้ แต่จะถูกคงไว้เมื่อบันทึก",
                    "scope": {"branch": "สาขา", "own": "ของตนเอง"}}
  },
  "activation": {
    "title": "สถานะบทบาท", "membersCount": "ผู้ถือบทบาท {count} คน", "pendingAssignments": "คำขอมอบบทบาทที่รอ {count} รายการ",
    "inUseHint": "ต้องถอนบทบาทจากผู้ใช้และจัดการคำขอที่รออยู่ให้หมดก่อนปิดใช้งาน",
    "deactivate": {"action": "ปิดใช้งานบทบาท", "message": "บทบาทนี้จะไม่แสดงในรายการมอบบทบาทอีก ยืนยันการปิดใช้งาน?", "confirm": "ปิดใช้งาน", "success": "ปิดใช้งานบทบาทแล้ว"},
    "activate": {"action": "เปิดใช้งานบทบาท", "message": "บทบาทนี้จะกลับมาให้มอบได้อีกครั้ง ยืนยันการเปิดใช้งาน?", "confirm": "เปิดใช้งาน", "success": "เปิดใช้งานบทบาทแล้ว"}
  },
  "queue": {
    "title": "คำขอแก้ไขบทบาทที่รออนุมัติ", "subtitle": "ผู้ตรวจต้องเป็นคนละคนกับผู้ขอ", "emptyTitle": "ไม่มีคำขอแก้ไขบทบาทที่รออนุมัติ", "emptyDescription": "คำขอใหม่จะแสดงที่นี่",
    "role": "บทบาท", "requestedBy": "ผู้ขอ", "requestedAt": "วันที่ขอ", "changes": "การเปลี่ยนแปลง", "added": "เพิ่ม", "removed": "ถอน",
    "approve": "อนุมัติ", "reject": "ปฏิเสธ", "approved": "อนุมัติคำขอแล้ว", "rejected": "ปฏิเสธคำขอแล้ว",
    "confirmApproveTitle": "อนุมัติการแก้ไขบทบาท", "confirmApproveMessage": "ยืนยันการอนุมัติการแก้ไขบทบาท {role}?",
    "confirmRejectTitle": "ปฏิเสธการแก้ไขบทบาท", "confirmRejectMessage": "ยืนยันการปฏิเสธการแก้ไขบทบาท {role}?"
  },
  "errors": {
    "GENERIC": "ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง",
    "ADMIN_VERSION_CONFLICT": "ข้อมูลถูกแก้ไขโดยผู้อื่นแล้ว กรุณาโหลดใหม่แล้วลองอีกครั้ง",
    "ROLE_NAME_ALREADY_EXISTS": "มีบทบาทที่ใช้ชื่อนี้อยู่แล้ว",
    "ROLE_NAME_RESERVED": "ชื่อที่ขึ้นต้นด้วย system. สงวนไว้สำหรับบทบาทของระบบ",
    "ROLE_PERMISSION_UNKNOWN": "มีสิทธิ์ที่ไม่อยู่ในรายการสิทธิ์ของระบบ",
    "ROLE_ESCALATION_DENIED": "คุณเพิ่มสิทธิ์ที่ตนเองไม่ได้ถืออยู่ไม่ได้",
    "ROLE_SYSTEM_IMMUTABLE": "บทบาทของระบบแก้ไขไม่ได้",
    "ROLE_IN_USE": "บทบาทนี้ยังมีผู้ถืออยู่หรือมีคำขอมอบบทบาทที่รออยู่",
    "ROLE_CHANGE_REQUEST_PENDING": "บทบาทนี้มีคำขอแก้ไขที่รออนุมัติอยู่",
    "ROLE_CHANGE_REQUEST_NOT_PENDING": "คำขอนี้ถูกตัดสินไปแล้ว",
    "ROLE_CHANGE_REQUEST_STALE": "บทบาทเปลี่ยนไปตั้งแต่ขอ ให้ปฏิเสธคำขอนี้แล้วขอใหม่",
    "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED": "ผู้ตรวจต้องเป็นคนละคนกับผู้ขอ",
    "LAST_ADMINISTRATOR_REQUIRED": "องค์กรต้องมีผู้ดูแลระบบที่ใช้งานอยู่อย่างน้อยหนึ่งคน",
    "BRANCH_NOT_FOUND": "ไม่พบสาขาที่ระบุ",
    "IDEMPOTENCY_KEY_REUSED": "คำขอซ้ำไม่ตรงกับคำขอเดิม กรุณาลองใหม่",
    "PERMISSION_DENIED": "คุณไม่มีสิทธิ์ดำเนินการนี้",
    "RESOURCE_NOT_FOUND": "ไม่พบข้อมูลที่ต้องการ"
  }
}

EN = {
  "accessDeniedTitle": "Access denied",
  "accessDeniedDetail": "You do not have permission to manage roles and permissions.",
  "list": {
    "title": "Roles and permissions", "subtitle": "Create and edit roles from the system permission catalog", "createRole": "Add role",
    "searchPlaceholder": "Search role name or description", "statusFilter": "Status", "status": {"active": "Active", "inactive": "Inactive"},
    "table": {"name": "Role", "members": "Holders", "permissions": "Permissions", "status": "Status"},
    "badges": {"system": "System", "requiresApproval": "Needs checker", "pendingChange": "Change pending"},
    "emptyTitle": "No roles yet", "emptyDescription": "Add the first role to define access"
  },
  "editor": {
    "createTitle": "Add role", "createSubtitle": "Pick permissions from the system catalog", "editTitle": "Edit role",
    "createSubmit": "Add role", "saveSubmit": "Save changes", "createSuccess": "Role added", "saveSuccess": "Role saved",
    "pendingSubmitted": "Change request sent to a checker", "backToList": "Back to roles",
    "systemReadOnly": "System roles cannot be edited",
    "pendingBanner": "A change request is awaiting approval, so this role cannot be edited or deactivated until it is decided.",
    "approvalNotice": "Adding approval permissions to a role that people already hold needs another checker to approve before it takes effect.",
    "fields": {"name": "Role name", "description": "Description"},
    "validation": {"nameRequired": "Enter the role name", "nameLength": "The role name must be 3-100 characters"},
    "permissions": {"title": "Role permissions", "searchPlaceholder": "Search permissions", "approvalTag": "Approval permission", "noMatch": "No permissions match",
                    "selectedCount": "{count} selected", "scopedTitle": "Scoped permissions (kept as they are)",
                    "scopedHint": "Branch or own-scope permissions cannot be added here but are preserved when you save.",
                    "scope": {"branch": "Branch", "own": "Own"}}
  },
  "activation": {
    "title": "Role status", "membersCount": "{count} holders", "pendingAssignments": "{count} pending assignment requests",
    "inUseHint": "Revoke the role from users and settle pending requests before deactivating.",
    "deactivate": {"action": "Deactivate role", "message": "This role will no longer be offered for assignment. Deactivate it?", "confirm": "Deactivate", "success": "Role deactivated"},
    "activate": {"action": "Activate role", "message": "This role can be assigned again. Activate it?", "confirm": "Activate", "success": "Role activated"}
  },
  "queue": {
    "title": "Role change requests awaiting approval", "subtitle": "The checker must be a different person from the requester", "emptyTitle": "No role change requests awaiting approval", "emptyDescription": "New requests will appear here",
    "role": "Role", "requestedBy": "Requested by", "requestedAt": "Requested on", "changes": "Changes", "added": "Added", "removed": "Removed",
    "approve": "Approve", "reject": "Reject", "approved": "Request approved", "rejected": "Request rejected",
    "confirmApproveTitle": "Approve role change", "confirmApproveMessage": "Approve the change to role {role}?",
    "confirmRejectTitle": "Reject role change", "confirmRejectMessage": "Reject the change to role {role}?"
  },
  "errors": {
    "GENERIC": "The action failed. Please try again.",
    "ADMIN_VERSION_CONFLICT": "This record was changed by someone else. Reload and try again.",
    "ROLE_NAME_ALREADY_EXISTS": "A role with this name already exists.",
    "ROLE_NAME_RESERVED": "Names starting with system. are reserved for built-in roles.",
    "ROLE_PERMISSION_UNKNOWN": "A permission is not in the system catalog.",
    "ROLE_ESCALATION_DENIED": "You cannot add permissions you do not hold yourself.",
    "ROLE_SYSTEM_IMMUTABLE": "System roles cannot be changed.",
    "ROLE_IN_USE": "This role is still held by users or has pending assignment requests.",
    "ROLE_CHANGE_REQUEST_PENDING": "This role already has a change request awaiting approval.",
    "ROLE_CHANGE_REQUEST_NOT_PENDING": "This request has already been decided.",
    "ROLE_CHANGE_REQUEST_STALE": "The role changed after the request was made. Reject it and request again.",
    "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED": "The checker must be a different person from the requester.",
    "LAST_ADMINISTRATOR_REQUIRED": "The organization must keep at least one active administrator.",
    "BRANCH_NOT_FOUND": "The branch was not found.",
    "IDEMPOTENCY_KEY_REUSED": "This repeated request differs from the original. Please try again.",
    "PERMISSION_DENIED": "You do not have permission to do this.",
    "RESOURCE_NOT_FOUND": "The requested record was not found."
  }
}

SHELL = {"src/messages/th.json": "บทบาทและสิทธิ์", "src/messages/en.json": "Roles"}
for path, ns in (("src/messages/th.json", TH), ("src/messages/en.json", EN)):
    raw = open(path, encoding="utf-8").read()
    data = json.loads(raw)
    assert json.dumps(data, indent=2, ensure_ascii=False) + "\n" == raw, f"{path} does not round-trip; edit by hand"
    assert "roleAdmin" not in data and "roleSettings" not in data["shell"], f"{path} already updated"
    data["shell"]["roleSettings"] = SHELL[path]
    data["roleAdmin"] = ns
    open(path, "w", encoding="utf-8").write(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
PY
git diff --stat -- src/messages
```
Expected: เฉพาะบรรทัดที่เพิ่ม (deletions เฉพาะ `,` ท้ายคีย์เดิมที่ต่อท้าย). ถ้า assert round-trip ล้ม ให้เพิ่มด้วย Edit ท้ายไฟล์แทน.

Run: `cd frontend && npx vitest run src/features/settings/role-management/role-management-errors.test.ts` → Expected: PASS.

- [ ] **Step 4: query hooks (รูปแบบเดียวกับ user-admin / organization)**

สร้าง `frontend/src/features/settings/role-management/api/role-management-queries.ts`:

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
  type PermissionCatalogResponse,
  type RoleChangeRequestListResponse,
  type RoleChangeRequestResponse,
  type RoleDefinitionListParams,
  type RoleDefinitionListResponse,
  type RoleDefinitionResponse,
  type SaveRoleDefinitionRequest,
  type UpdateRoleDefinitionResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const roleAdminKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  catalog: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "role-admin", "catalog"] as const,
  list: (membershipId: string | null | undefined, locale: ApiLocale, params: RoleDefinitionListParams) =>
    ["business", membershipId, locale, "role-admin", "list", params] as const,
  detail: (membershipId: string | null | undefined, locale: ApiLocale, roleId: string) =>
    ["business", membershipId, locale, "role-admin", "detail", roleId] as const,
  requests: (membershipId: string | null | undefined, locale: ApiLocale, status: string) =>
    ["business", membershipId, locale, "role-admin", "change-requests", status] as const,
};

/** A role change alters what every permission-gated screen allows, so refresh all business queries (as user-admin does). */
function invalidateRoles(queryClient: QueryClient, membershipId: string | undefined): void {
  void queryClient.invalidateQueries({ queryKey: roleAdminKeys.all(membershipId) });
}

export function useAdminPermissionCatalog(): UseQueryResult<PermissionCatalogResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.catalog(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAdminPermissions(await context.buildOptions({ signal })),
  });
}

export function useRoleDefinitions(params: RoleDefinitionListParams): UseQueryResult<RoleDefinitionListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.list(context.membershipId, context.locale, params),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listRoleDefinitions(params, await context.buildOptions({ signal })),
  });
}

export function useRoleDefinition(roleId: string, enabled: boolean): UseQueryResult<RoleDefinitionResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.detail(context.membershipId, context.locale, roleId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getRoleDefinition(roleId, await context.buildOptions({ signal })),
  });
}

export function useRoleChangeRequests(status: string, enabled = true): UseQueryResult<RoleChangeRequestListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.requests(context.membershipId, context.locale, status),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listRoleChangeRequests(status, await context.buildOptions({ signal })),
  });
}

export function useCreateRoleDefinition(): UseMutationResult<
  RoleDefinitionResponse, Error, { payload: SaveRoleDefinitionRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createRoleDefinition(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useUpdateRoleDefinition(): UseMutationResult<
  UpdateRoleDefinitionResponse, Error, { roleId: string; payload: SaveRoleDefinitionRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ roleId, payload, ifMatch }) =>
      apiClient.updateRoleDefinition(roleId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useSetRoleDefinitionActive(): UseMutationResult<
  RoleDefinitionResponse, Error, { roleId: string; active: boolean; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ roleId, active, ifMatch }) =>
      apiClient.setRoleDefinitionActive(roleId, active, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useDecideRoleChangeRequest(): UseMutationResult<
  RoleChangeRequestResponse, Error, { requestId: string; decision: "approve" | "reject" | "cancel"; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ requestId, decision, ifMatch }) =>
      apiClient.decideRoleChangeRequest(requestId, decision, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}
```
Run: `cd frontend && npm run typecheck && npm run lint` → Expected: 0 errors (ไม่มี `any`).

- [ ] **Step 5: Commit**

```bash
git add frontend/src/messages/th.json frontend/src/messages/en.json frontend/src/features/settings/role-management
git commit -m "feat(roles): add role management translations, error map and query hooks

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 13: Frontend — รายการ Role, route และเมนู

**Files:**
- Create: `frontend/src/features/settings/role-management/components/role-definition-list.tsx` (+ `.test.tsx`)
- Create: `frontend/src/app/[locale]/(erp)/settings/roles/page.tsx`
- Modify: `frontend/src/components/layout/SidebarNav.tsx`, `frontend/src/components/layout/SidebarNav.test.tsx`

- [ ] **Step 1: เขียน failing test**

สร้าง `role-definition-list.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { RoleDefinitionList } from "./role-definition-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  push: vi.fn(),
  params: undefined as Record<string, unknown> | undefined,
  result: {} as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/roles",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/role-management-queries", () => ({
  useRoleDefinitions: (params: Record<string, unknown>) => {
    mocks.params = params;
    return mocks.result;
  },
}));

const items = [
  { id: "r1", name: "Test Admin", description: "ผู้ดูแล", isSystem: true, isActive: true, rowVersion: "v1", memberCount: 2, permissionCount: 40, requiresApproval: true, hasPendingChange: false },
  { id: "r2", name: "ผู้ประเมิน", description: null, isSystem: false, isActive: false, rowVersion: "v2", memberCount: 0, permissionCount: 3, requiresApproval: false, hasPendingChange: true },
];

describe("RoleDefinitionList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["roles.manage"];
    mocks.result = {
      data: { items, pagination: { page: 1, pageSize: 25, totalCount: 2, totalPages: 1 } },
      isLoading: false, isError: false, refetch: vi.fn(),
    };
  });

  it("lists roles with system, approval and pending-change badges and links to the editor", () => {
    render(<RoleDefinitionList />);

    expect(screen.getByRole("link", { name: /Test Admin/ })).toHaveAttribute("href", "/th/settings/roles/r1");
    expect(screen.getByText("ระบบ")).toBeInTheDocument();
    expect(screen.getByText("ต้องมีผู้ตรวจอนุมัติ")).toBeInTheDocument();
    expect(screen.getByText("รอการอนุมัติการแก้ไข")).toBeInTheDocument();
    expect(screen.getAllByText("ปิดใช้งาน").length).toBeGreaterThan(0); // the inactive role's status badge (the filter also lists it)
  });

  it("asks the server for a sorted, paged list and routes the add button to the create form", () => {
    render(<RoleDefinitionList />);

    expect(mocks.params).toMatchObject({ sortBy: "name", sortOrder: "asc", page: 1 });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));
    expect(mocks.push).toHaveBeenCalledWith("/th/settings/roles/create");
  });

  it("hides the add button without roles.manage", () => {
    mocks.permissions = [];
    render(<RoleDefinitionList />);

    expect(screen.queryByRole("button", { name: "เพิ่มบทบาท" })).not.toBeInTheDocument();
  });
});
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/role-definition-list.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: component + route**

สร้าง `role-definition-list.tsx`:

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
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableEntityCell } from "@/components/ui/TableEntityCell";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import type { RoleDefinitionSummaryResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useRoleDefinitions } from "../api/role-management-queries";

interface RoleFilters extends ListFilterRecord {
  status?: string;
}

const STATUS_VALUES = ["active", "inactive"] as const;

export function RoleDefinitionList() {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ROLES_MANAGE);

  const listState = useListState<RoleFilters>({
    schema: { defaultSort: "name", defaultOrder: "asc", single: ["status"], allowedSorts: ["name", "createdAt"] },
    debounceMs: 350,
  });
  const { search, page, limit, sort, order, filters } = listState.params;
  const { data, isLoading, isError, refetch } = useRoleDefinitions({
    search: search || undefined,
    status: filters.status || undefined,
    sortBy: sort,
    sortOrder: order,
    page,
    limit,
  });

  const roles = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 1;
  const isZeroRoles = !isLoading && !isError && totalItems === 0 && !search && !filters.status;
  const goCreate = () => router.push(`/${locale}/settings/roles/create`);

  const columns = useMemo<Column<RoleDefinitionSummaryResponse>[]>(
    () => [
      {
        id: "name",
        header: t("list.table.name"),
        sortable: true,
        accessorKey: "name",
        className: "min-w-[260px]",
        cell: (_v, row) => (
          <div className="flex flex-col gap-1">
            <TableEntityCell title={row.name ?? "-"} subtitle={row.description ?? undefined} href={`/${locale}/settings/roles/${row.id}`} />
            <div className="flex flex-wrap gap-1">
              {row.isSystem ? <StatusBadge label={t("list.badges.system")} variant="info" /> : null}
              {row.requiresApproval ? <StatusBadge label={t("list.badges.requiresApproval")} variant="warning" /> : null}
              {row.hasPendingChange ? <StatusBadge label={t("list.badges.pendingChange")} variant="warning" /> : null}
            </div>
          </div>
        ),
      },
      { id: "members", header: t("list.table.members"), cell: (_v, row) => String(row.memberCount ?? "-") },
      { id: "permissions", header: t("list.table.permissions"), cell: (_v, row) => String(row.permissionCount ?? "-") },
      {
        id: "status",
        header: t("list.table.status"),
        cell: (_v, row) => (
          <StatusBadge
            label={row.isActive ? t("list.status.active") : t("list.status.inactive")}
            variant={row.isActive ? "success" : "neutral"}
          />
        ),
      },
    ],
    [locale, t]
  );

  return (
    <div className="space-y-6">
      <PageHeader
        title={t("list.title")}
        subtitle={t("list.subtitle")}
        actions={
          canManage ? (
            <Button variant="primary" size="md" icon={<IconPlus size={16} />} onClick={goCreate}>
              {t("list.createRole")}
            </Button>
          ) : undefined
        }
      />
      <ListToolbar>
        <div className="flex flex-wrap items-end gap-3">
          <ListSearchInput
            value={listState.draftSearch}
            isDebouncing={listState.isDebouncing}
            placeholder={t("list.searchPlaceholder")}
            onChange={(value) => listState.actions.setSearch(value)}
            onClear={() => listState.actions.setSearch("", true)}
            onSubmit={(value) => listState.actions.setSearch(value, true)}
          />
          <ListFilterSelect
            id="role-admin-status"
            label={t("list.statusFilter")}
            value={filters.status ?? ""}
            onChange={(value) => listState.actions.setFilter("status", value || undefined)}
            options={STATUS_VALUES.map((value) => ({ value, label: t(`list.status.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZeroRoles ? (
        <EmptyState
          icon="empty"
          title={t("list.emptyTitle")}
          description={t("list.emptyDescription")}
          actionLabel={canManage ? t("list.createRole") : undefined}
          onAction={canManage ? goCreate : undefined}
        />
      ) : (
        <DataTable<RoleDefinitionSummaryResponse>
          columns={columns}
          data={roles}
          isLoading={isLoading}
          isError={isError}
          error={isError ? t("errors.GENERIC") : null}
          onRetry={() => refetch()}
          emptyTitle={tCommon("table.noData")}
          sorting={{ key: sort ?? null, order }}
          onSort={(key) => listState.actions.setSort(key)}
          pagination={{ page, limit, totalPages, totalItems }}
          onPageChange={(next) => listState.actions.setPage(next)}
          onLimitChange={(next) => listState.actions.setLimit(next as ListPageSize)}
        />
      )}
    </div>
  );
}
```

สร้าง `frontend/src/app/[locale]/(erp)/settings/roles/page.tsx`:

```tsx
"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleDefinitionList } from "@/features/settings/role-management/components/role-definition-list";

export default function RoleSettingsPage() {
  const t = useTranslations("roleAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <RoleDefinitionList />
    </PermissionGuard>
  );
}
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/role-definition-list.test.tsx` → Expected: PASS 3.

- [ ] **Step 3: เมนู Sidebar (test ก่อน)**

ต่อท้าย `describe` ใน `SidebarNav.test.tsx`:

```tsx
  it("shows the roles menu only to members who can manage roles", () => {
    const manager: CurrentUserResponse = {
      ...mockUser,
      memberships: mockUser.memberships?.map((membership) => ({
        ...membership,
        permissions: [...(membership.permissions ?? []), { key: "roles.manage", scope: "organization", scopeId: "20000000-0000-0000-0000-000000000001" }],
      })),
    };

    const { unmount } = render(<SidebarNav currentUser={mockUser} isCollapsed={false} />);
    expect(screen.queryByRole("link", { name: "บทบาทและสิทธิ์" })).toBeNull();
    unmount();

    render(<SidebarNav currentUser={manager} isCollapsed={false} />);
    expect(screen.getByRole("link", { name: "บทบาทและสิทธิ์" })).toHaveAttribute("href", "/th/settings/roles");
  });
```
Run: `cd frontend && npx vitest run src/components/layout/SidebarNav.test.tsx` → Expected: FAIL (ไม่มีลิงก์).

แก้ `SidebarNav.tsx` (ตามรูปแบบเมนู branch settings ที่มีอยู่):
1. เพิ่ม `IconLock` ใน import จาก `@/components/common/Icons`.
2. ใต้ `const hasBranchesManage = ...`: `const hasRolesManage = can(activeMembership, "roles.manage");`
3. ใต้ `const isBranchSettingsActive = ...`: `const isRoleSettingsActive = pathname.startsWith(\`/${locale}/settings/roles\`);` (ไม่ชนกับ `/settings/role-requests` เพราะเป็น `role-` ไม่ใช่ `roles`)
4. ใต้ `const isBranchSettingsVisible = ...`: `const isRoleSettingsVisible = hasRolesManage && (!trimmedSearch || tShell("roleSettings").toLowerCase().includes(trimmedSearch));`
5. ใน `hasAnyResults` เพิ่ม `isRoleSettingsVisible ||` ต่อจาก `isBranchSettingsVisible ||`.
6. ใน JSX ต่อจากบล็อก `isBranchSettingsVisible`:

```tsx
          {isRoleSettingsVisible && (
            <li>
              {renderLink({
                id: "role-settings",
                href: `/${locale}/settings/roles`,
                label: tShell("roleSettings"),
                icon: <IconLock size={20} />,
                isActive: isRoleSettingsActive,
              })}
            </li>
          )}
```
Run: `cd frontend && npx vitest run src/components/layout/SidebarNav.test.tsx src/features/settings/role-management && npm run typecheck && npm run lint` → Expected: PASS, 0 errors.

- [ ] **Step 4: Commit**

```bash
git add frontend/src/features/settings/role-management/components "frontend/src/app/[locale]/(erp)/settings/roles" frontend/src/components/layout/SidebarNav.tsx frontend/src/components/layout/SidebarNav.test.tsx
git commit -m "feat(roles): add role list page and settings menu

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 14: Frontend — editor, permission picker และ activation panel (route `[id]`)

Single Dynamic Route `[id]` (`create`/`add` = สร้าง). ฟอร์มกลุ่มเดียว (ชื่อ/คำอธิบาย + รายการสิทธิ์) จึงไม่ใช้ `FormTabs`. Role ระบบหรือ Role ที่มีคำขอรออนุมัติแสดงแบบอ่านอย่างเดียว. รายการสิทธิ์และ flag `isApprovalPermission` มาจาก Backend (FE ไม่ hardcode ชุดอนุมัติ).

**Files:**
- Create: `frontend/src/features/settings/role-management/components/{permission-grant-picker,role-activation-panel,role-definition-editor}.tsx` (+ `.test.tsx` แต่ละไฟล์)
- Create: `frontend/src/app/[locale]/(erp)/settings/roles/[id]/page.tsx`

- [ ] **Step 1: เขียน failing test — picker และ activation panel**

`permission-grant-picker.test.tsx`:

```tsx
import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { PermissionGrantPicker } from "./permission-grant-picker";

const catalog = [
  { key: "customers.read", description: "Read customers", isApprovalPermission: false },
  { key: "estimates.approve", description: "Approve estimates", isApprovalPermission: true },
  { key: "estimates.read", description: null, isApprovalPermission: false },
];

describe("PermissionGrantPicker", () => {
  it("groups by module, toggles a key, flags approval permissions and filters by search", () => {
    const onToggle = vi.fn();
    render(<PermissionGrantPicker catalog={catalog} selected={new Set(["customers.read"])} disabled={false} onToggle={onToggle} />);

    expect(screen.getByLabelText("customers.read")).toBeChecked();
    expect(screen.getByLabelText("estimates.approve")).not.toBeChecked();
    expect(screen.getByText("สิทธิ์อนุมัติ")).toBeInTheDocument();
    expect(screen.getByText("เลือกแล้ว 1 สิทธิ์")).toBeInTheDocument();

    fireEvent.click(screen.getByLabelText("estimates.approve"));
    expect(onToggle).toHaveBeenCalledWith("estimates.approve");

    fireEvent.change(screen.getByPlaceholderText("ค้นหาสิทธิ์"), { target: { value: "zzz" } });
    expect(screen.getByText("ไม่พบสิทธิ์ที่ค้นหา")).toBeInTheDocument();
  });

  it("disables every checkbox when read-only", () => {
    render(<PermissionGrantPicker catalog={catalog} selected={new Set()} disabled onToggle={vi.fn()} />);

    expect(screen.getByLabelText("customers.read")).toBeDisabled();
  });
});
```

`role-activation-panel.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import type { RoleDefinitionResponse } from "@/lib/api/api-client";
import { RoleActivationPanel } from "./role-activation-panel";

const mocks = vi.hoisted(() => ({ setActive: vi.fn(), success: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/role-management-queries", () => ({
  useSetRoleDefinitionActive: () => ({ mutateAsync: mocks.setActive, isPending: false }),
}));

const role = (over: Partial<RoleDefinitionResponse>): RoleDefinitionResponse => ({
  id: "r1", name: "Sales", description: null, isSystem: false, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-11T00:00:00Z",
  memberCount: 0, pendingAssignmentRequestCount: 0, requiresApproval: false, permissions: [], pendingChange: null, ...over,
});

describe("RoleActivationPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.setActive.mockResolvedValue({});
  });

  it("deactivates an unused role only after confirmation, sending the row version", async () => {
    render(<RoleActivationPanel role={role({})} canManage />);

    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" }));
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ roleId: "r1", active: false, ifMatch: "v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ปิดใช้งานบทบาทแล้ว"));
  });

  it("keeps deactivation disabled and explains why when users hold the role or a change is pending", () => {
    const { rerender } = render(<RoleActivationPanel role={role({ memberCount: 3 })} canManage />);
    expect(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" })).toBeDisabled();
    expect(screen.getByText("ผู้ถือบทบาท 3 คน")).toBeInTheDocument();

    rerender(<RoleActivationPanel role={role({ pendingChange: { id: "q1", rowVersion: "q" } as RoleDefinitionResponse["pendingChange"] })} canManage />);
    expect(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" })).toBeDisabled();
  });

  it("offers activation for an inactive role, shows server errors, and renders nothing for system roles", async () => {
    mocks.setActive.mockRejectedValueOnce(new ApiError({ status: 409, code: "ADMIN_VERSION_CONFLICT", message: "x" }));
    const { rerender } = render(<RoleActivationPanel role={role({ isActive: false })} canManage />);

    fireEvent.click(screen.getByRole("button", { name: "เปิดใช้งานบทบาท" }));
    fireEvent.click(await screen.findByRole("button", { name: "เปิดใช้งาน" }));
    expect(await screen.findByText("ข้อมูลถูกแก้ไขโดยผู้อื่นแล้ว กรุณาโหลดใหม่แล้วลองอีกครั้ง")).toBeInTheDocument();

    rerender(<RoleActivationPanel role={role({ isSystem: true })} canManage />);
    expect(screen.queryByRole("button", { name: "ปิดใช้งานบทบาท" })).not.toBeInTheDocument();
  });
});
```
หมายเหตุ: ถ้า `npm run typecheck` ฟ้อง fixture ของ test เพราะ field ใน generated type เป็น required ให้เติม field ใน fixture ให้ครบ (ห้ามใช้ `any`).
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/permission-grant-picker.test.tsx src/features/settings/role-management/components/role-activation-panel.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: picker และ activation panel**

`permission-grant-picker.tsx`:

```tsx
"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { StatusBadge } from "@/components/ui/StatusBadge";

export interface CatalogItem {
  key: string;
  description: string | null;
  isApprovalPermission: boolean;
}

interface PermissionGrantPickerProps {
  catalog: ReadonlyArray<CatalogItem>;
  selected: ReadonlySet<string>;
  disabled: boolean;
  onToggle: (key: string) => void;
}

/** Organization-scope permission picker, grouped by module (the part of the key before the first dot). */
export function PermissionGrantPicker({ catalog, selected, disabled, onToggle }: PermissionGrantPickerProps) {
  const t = useTranslations("roleAdmin.editor.permissions");
  const [query, setQuery] = useState("");

  const groups = useMemo(() => {
    const needle = query.trim().toLowerCase();
    const matches = catalog.filter(
      (item) => needle === "" || item.key.toLowerCase().includes(needle) || (item.description ?? "").toLowerCase().includes(needle)
    );
    const byModule = new Map<string, CatalogItem[]>();
    for (const item of matches) {
      const moduleName = item.key.split(".")[0];
      byModule.set(moduleName, [...(byModule.get(moduleName) ?? []), item]);
    }
    return [...byModule.entries()];
  }, [catalog, query]);

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <Input
          label={t("searchPlaceholder")}
          placeholder={t("searchPlaceholder")}
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
        <span className="text-sm text-erp-text-muted">{t("selectedCount", { count: selected.size })}</span>
      </div>
      {groups.length === 0 ? <p className="text-sm text-erp-text-muted">{t("noMatch")}</p> : null}
      {groups.map(([moduleName, items]) => (
        <fieldset key={moduleName} className="space-y-1 border border-erp-border p-3">
          <legend className="px-1 text-sm font-semibold text-erp-text-main">{moduleName}</legend>
          {items.map((item) => (
            <div key={item.key} className="flex flex-wrap items-center gap-x-3">
              <Checkbox
                label={item.key}
                checked={selected.has(item.key)}
                disabled={disabled}
                onCheckedChange={() => onToggle(item.key)}
                wrapperClassName="mb-0"
              />
              {item.isApprovalPermission ? <StatusBadge label={t("approvalTag")} variant="warning" /> : null}
              {item.description ? <span className="text-xs text-erp-text-muted">{item.description}</span> : null}
            </div>
          ))}
        </fieldset>
      ))}
    </div>
  );
}
```
(`Input` ที่ใช้ `label` และ `placeholder` เหมือนกัน เพื่อให้ `getByPlaceholderText` ทำงาน; ถ้า `Input` ไม่แสดง placeholder ให้ตรวจ props ใน `components/ui/Input.tsx`.)

`role-activation-panel.tsx`:

```tsx
"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import type { RoleDefinitionResponse } from "@/lib/api/api-client";
import { useSetRoleDefinitionActive } from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";

interface RoleActivationPanelProps {
  role: RoleDefinitionResponse;
  canManage: boolean;
}

export function RoleActivationPanel({ role, canManage }: RoleActivationPanelProps) {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const setActive = useSetRoleDefinitionActive();

  // System roles are immutable; there is nothing to show.
  if (!canManage || role.isSystem === true) return null;
  const { id: roleId, rowVersion } = role;
  if (!roleId || !rowVersion) return <Alert variant="danger">{t("errors.GENERIC")}</Alert>;

  const isActive = role.isActive === true;
  const members = role.memberCount ?? 0;
  const pendingAssignments = role.pendingAssignmentRequestCount ?? 0;
  const inUse = members > 0 || pendingAssignments > 0;
  // The server decides; this only mirrors its rules so the button is not offered when it would be refused.
  const blocked = isActive && (inUse || role.pendingChange != null);
  const key = isActive ? "deactivate" : "activate";

  const confirm = async () => {
    setErrorMessage(null);
    try {
      await setActive.mutateAsync({ roleId, active: !isActive, ifMatch: rowVersion });
      toast.success(t(`activation.${key}.success`));
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${roleAdminErrorKey(error)}`));
    } finally {
      setConfirmOpen(false);
    }
  };

  return (
    <FormSection title={t("activation.title")}>
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      {isActive && inUse ? (
        <Alert variant="warning">
          {members > 0 ? <p>{t("activation.membersCount", { count: members })}</p> : null}
          {pendingAssignments > 0 ? <p>{t("activation.pendingAssignments", { count: pendingAssignments })}</p> : null}
          <p>{t("activation.inUseHint")}</p>
        </Alert>
      ) : null}
      <Button variant={isActive ? "danger" : "primary"} size="md" disabled={blocked} onClick={() => setConfirmOpen(true)}>
        {t(`activation.${key}.action`)}
      </Button>
      <ConfirmationModal
        isOpen={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        onConfirm={confirm}
        title={t(`activation.${key}.action`)}
        message={t(`activation.${key}.message`)}
        confirmText={t(`activation.${key}.confirm`)}
        cancelText={tCommon("actions.cancel")}
        variant={isActive ? "danger" : "info"}
        isLoading={setActive.isPending}
      />
    </FormSection>
  );
}
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/permission-grant-picker.test.tsx src/features/settings/role-management/components/role-activation-panel.test.tsx` → Expected: PASS ทั้ง 5.

- [ ] **Step 3: เขียน failing test — editor**

สร้าง `role-definition-editor.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { RoleDefinitionEditor } from "./role-definition-editor";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  create: vi.fn(),
  update: vi.fn(),
  push: vi.fn(),
  success: vi.fn(),
  role: { data: undefined, isPending: false, isError: false } as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/roles/create",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("./role-activation-panel", () => ({ RoleActivationPanel: () => <div>activation-panel</div> }));
vi.mock("../api/role-management-queries", () => ({
  useRoleDefinition: () => mocks.role,
  useAdminPermissionCatalog: () => ({
    isPending: false,
    isError: false,
    data: {
      items: [
        { key: "customers.read", description: "Read customers", isApprovalPermission: false },
        { key: "estimates.approve", description: "Approve estimates", isApprovalPermission: true },
      ],
    },
  }),
  useCreateRoleDefinition: () => ({ mutateAsync: mocks.create, isPending: false }),
  useUpdateRoleDefinition: () => ({ mutateAsync: mocks.update, isPending: false }),
}));

const existing = {
  id: "r1", name: "Sales Lead", description: "เดิม", isSystem: false, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-11T00:00:00Z",
  memberCount: 2, pendingAssignmentRequestCount: 0, requiresApproval: false, pendingChange: null,
  permissions: [
    { permissionKey: "customers.read", scope: "organization", branch: null },
    { permissionKey: "estimates.read", scope: "branch", branch: { id: "b1", name: "สาขา 1" } },
  ],
};

describe("RoleDefinitionEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["roles.manage"];
    mocks.role = { data: existing, isPending: false, isError: false };
    mocks.create.mockResolvedValue({ id: "new-role" });
    mocks.update.mockResolvedValue({ role: existing, pendingRequest: null });
  });

  it("validates the name before creating", async () => {
    render(<RoleDefinitionEditor roleId="create" />);

    fireEvent.change(screen.getByLabelText(/ชื่อบทบาท/), { target: { value: "ab" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));

    expect(await screen.findByText("ชื่อบทบาทต้องมี 3-100 ตัวอักษร")).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates with the selected organization-scope permissions, an idempotency key and opens the new role", async () => {
    render(<RoleDefinitionEditor roleId="create" />);

    fireEvent.change(screen.getByLabelText(/ชื่อบทบาท/), { target: { value: "  Quote Reviewer  " } });
    fireEvent.click(screen.getByLabelText("customers.read"));
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      name: "Quote Reviewer",
      description: null,
      permissions: [{ permissionKey: "customers.read", scope: "organization", branchId: null }],
    });
    expect(call.idempotencyKey).toMatch(/.{16,}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/roles/new-role"));
  });

  it("saves with the row version and preserves scoped grants it cannot edit", async () => {
    render(<RoleDefinitionEditor roleId="r1" />);

    expect(screen.getByText(/estimates\.read/)).toBeInTheDocument(); // preserved branch-scope grant is listed
    fireEvent.click(screen.getByLabelText("estimates.approve"));
    expect(screen.getByText("การเพิ่มสิทธิ์อนุมัติให้บทบาทที่มีผู้ถืออยู่ต้องให้ผู้ตรวจอีกคนอนุมัติก่อนมีผล")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      roleId: "r1",
      ifMatch: "v1",
      payload: {
        name: "Sales Lead",
        description: "เดิม",
        permissions: [
          { permissionKey: "customers.read", scope: "organization", branchId: null },
          { permissionKey: "estimates.approve", scope: "organization", branchId: null },
          { permissionKey: "estimates.read", scope: "branch", branchId: "b1" },
        ],
      },
    });
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("บันทึกบทบาทแล้ว"));
  });

  it("tells the user when the change became a request for a checker", async () => {
    mocks.update.mockResolvedValueOnce({ role: existing, pendingRequest: { id: "q1", rowVersion: "q" } });
    render(<RoleDefinitionEditor roleId="r1" />);

    fireEvent.click(screen.getByLabelText("estimates.approve"));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ส่งคำขอแก้ไขให้ผู้ตรวจอนุมัติแล้ว"));
  });

  it("shows the mapped message for a server error", async () => {
    mocks.update.mockRejectedValueOnce(new ApiError({ status: 409, code: "ROLE_NAME_ALREADY_EXISTS", message: "x" }));
    render(<RoleDefinitionEditor roleId="r1" />);

    fireEvent.click(screen.getByLabelText("customers.read"));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    expect(await screen.findByText("มีบทบาทที่ใช้ชื่อนี้อยู่แล้ว")).toBeInTheDocument();
  });

  it("is read-only for system roles and for roles with a pending change", () => {
    mocks.role = { data: { ...existing, isSystem: true }, isPending: false, isError: false };
    const { rerender } = render(<RoleDefinitionEditor roleId="r1" />);
    expect(screen.getByText("บทบาทของระบบแก้ไขไม่ได้")).toBeInTheDocument();
    expect(screen.getByLabelText("customers.read")).toBeDisabled();
    expect(screen.queryByRole("button", { name: "บันทึกการเปลี่ยนแปลง" })).not.toBeInTheDocument();

    mocks.role = { data: { ...existing, pendingChange: { id: "q1", rowVersion: "q" } }, isPending: false, isError: false };
    rerender(<RoleDefinitionEditor roleId="r1" />);
    expect(screen.getByText(/มีคำขอแก้ไขรออนุมัติ/)).toBeInTheDocument();
    expect(screen.getByLabelText(/ชื่อบทบาท/)).toBeDisabled();
  });
});
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/role-definition-editor.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 4: editor + route**

สร้าง `role-definition-editor.tsx`:

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
import type { RoleDefinitionResponse, RoleGrantRequest } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import {
  useAdminPermissionCatalog,
  useCreateRoleDefinition,
  useRoleDefinition,
  useUpdateRoleDefinition,
} from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";
import { RoleActivationPanel } from "./role-activation-panel";
import { PermissionGrantPicker, type CatalogItem } from "./permission-grant-picker";

const ORGANIZATION_SCOPE = "organization";

interface RoleFormValues {
  name: string;
  description: string;
}

interface ScopedGrant {
  permissionKey: string;
  scope: string;
  branchId: string | null;
  branchName: string | null;
}

const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

/** Branch/own-scope grants cannot be edited here; they are kept and sent back unchanged on save. */
function toScopedGrants(role: RoleDefinitionResponse | undefined): ScopedGrant[] {
  return (role?.permissions ?? []).flatMap((p) =>
    p.permissionKey && p.scope && p.scope !== ORGANIZATION_SCOPE
      ? [{ permissionKey: p.permissionKey, scope: p.scope, branchId: p.branch?.id ?? null, branchName: p.branch?.name ?? null }]
      : []
  );
}

export function RoleDefinitionEditor({ roleId }: { roleId: string }) {
  const isCreate = roleId === "create" || roleId === "add";
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ROLES_MANAGE);
  const roleQuery = useRoleDefinition(roleId, !isCreate);
  const catalogQuery = useAdminPermissionCatalog();
  const createMutation = useCreateRoleDefinition();
  const updateMutation = useUpdateRoleDefinition();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [edited, setEdited] = useState<ReadonlySet<string> | null>(null);
  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  const role = roleQuery.data;
  const existing = role?.id && role.rowVersion ? { id: role.id, rowVersion: role.rowVersion } : null;
  const catalog = useMemo<CatalogItem[]>(
    () =>
      (catalogQuery.data?.items ?? []).flatMap((item) =>
        item.key ? [{ key: item.key, description: item.description ?? null, isApprovalPermission: item.isApprovalPermission === true }] : []
      ),
    [catalogQuery.data]
  );
  const baseline = useMemo<ReadonlySet<string>>(
    () => new Set((role?.permissions ?? []).flatMap((p) => (p.permissionKey && p.scope === ORGANIZATION_SCOPE ? [p.permissionKey] : []))),
    [role]
  );
  const scoped = useMemo(() => toScopedGrants(role), [role]);
  const selected = edited ?? baseline;
  const permissionsChanged = edited !== null && (edited.size !== baseline.size || [...edited].some((key) => !baseline.has(key)));

  const schema = useMemo(
    () =>
      z.object({
        name: z.string().trim().min(1, t("editor.validation.nameRequired")).min(3, t("editor.validation.nameLength")).max(100, t("editor.validation.nameLength")),
        description: z.string().trim().max(500),
      }),
    [t]
  );
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<RoleFormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: "", description: "" },
    values: !isCreate && role ? { name: role.name ?? "", description: role.description ?? "" } : undefined,
  });

  if (!isCreate && roleQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (!isCreate && (roleQuery.isError || !existing)) return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;

  const backHref = `/${locale}/settings/roles`;
  const isSaving = createMutation.isPending || updateMutation.isPending;
  const locked = role?.isSystem === true || role?.pendingChange != null;
  const canEdit = canManage && !locked;
  const addsApproval =
    !isCreate && (role?.memberCount ?? 0) > 0 && catalog.some((c) => c.isApprovalPermission && selected.has(c.key) && !baseline.has(c.key));

  const toggle = (key: string) => {
    const next = new Set(selected);
    if (next.has(key)) next.delete(key);
    else next.add(key);
    setEdited(next);
  };

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const permissions: RoleGrantRequest[] = [
      ...[...selected].sort().map((permissionKey) => ({ permissionKey, scope: ORGANIZATION_SCOPE, branchId: null })),
      ...scoped.map((g) => ({ permissionKey: g.permissionKey, scope: g.scope, branchId: g.branchId })),
    ];
    const payload = { name: values.name.trim(), description: orNull(values.description), permissions };
    try {
      if (isCreate) {
        const fingerprint = JSON.stringify(payload);
        if (intentRef.current?.fingerprint !== fingerprint) intentRef.current = { fingerprint, key: crypto.randomUUID() };
        const created = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
        intentRef.current = null;
        toast.success(t("editor.createSuccess"));
        if (created.id) router.push(`${backHref}/${created.id}`);
      } else if (existing) {
        const result = await updateMutation.mutateAsync({ roleId: existing.id, payload, ifMatch: existing.rowVersion });
        toast.success(t(result.pendingRequest ? "editor.pendingSubmitted" : "editor.saveSuccess"));
        setEdited(null);
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${roleAdminErrorKey(error)}`));
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
            title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}
            subtitle={isCreate ? t("editor.createSubtitle") : (role?.name ?? undefined)}
            backHref={backHref}
            backLabel={t("editor.backToList")}
          />
        }
        errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
        actionBar={
          canEdit ? (
            <FormActionBar
              isDirty={isDirty || permissionsChanged}
              isLoading={isSaving}
              isSaveDisabled={isSaving}
              saveText={isCreate ? t("editor.createSubmit") : t("editor.saveSubmit")}
              saveButtonType="submit"
              cancelHref={backHref}
              showCancel
              cancelText={tCommon("actions.cancel")}
            />
          ) : undefined
        }
      >
        {role?.isSystem === true ? <Alert variant="info">{t("editor.systemReadOnly")}</Alert> : null}
        {role?.pendingChange != null ? <Alert variant="warning">{t("editor.pendingBanner")}</Alert> : null}
        <FormSection title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input label={t("editor.fields.name")} required disabled={!canEdit} error={errors.name?.message} {...register("name")} />
            <Textarea label={t("editor.fields.description")} rows={2} disabled={!canEdit} error={errors.description?.message} {...register("description")} />
          </div>
        </FormSection>
        <FormSection title={t("editor.permissions.title")}>
          {addsApproval ? <Alert variant="warning">{t("editor.approvalNotice")}</Alert> : null}
          {catalogQuery.isPending ? (
            <MonoSpinner size="md" label={tCommon("states.loading")} />
          ) : catalogQuery.isError ? (
            <Alert variant="danger">{t("errors.GENERIC")}</Alert>
          ) : (
            <PermissionGrantPicker catalog={catalog} selected={selected} disabled={!canEdit} onToggle={toggle} />
          )}
          {scoped.length > 0 ? (
            <div className="space-y-1">
              <p className="text-sm font-semibold text-erp-text-main">{t("editor.permissions.scopedTitle")}</p>
              <p className="text-xs text-erp-text-muted">{t("editor.permissions.scopedHint")}</p>
              <ul className="list-disc pl-5 text-sm">
                {scoped.map((g) => (
                  <li key={`${g.permissionKey}-${g.scope}-${g.branchId ?? "own"}`}>
                    {g.permissionKey} · {t(`editor.permissions.scope.${g.scope === "branch" ? "branch" : "own"}`)}
                    {g.branchName ? ` ${g.branchName}` : ""}
                  </li>
                ))}
              </ul>
            </div>
          ) : null}
        </FormSection>
      </FormContainer>
      {!isCreate && role ? <RoleActivationPanel role={role} canManage={canManage} /> : null}
    </div>
  );
}
```

สร้าง `frontend/src/app/[locale]/(erp)/settings/roles/[id]/page.tsx`:

```tsx
"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleDefinitionEditor } from "@/features/settings/role-management/components/role-definition-editor";

interface RoleDefinitionDynamicPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function RoleDefinitionDynamicPage({ params }: RoleDefinitionDynamicPageProps) {
  const { locale, id } = use(params);
  const t = useTranslations("roleAdmin");

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_MANAGE} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <RoleDefinitionEditor roleId={id} />
    </PermissionGuard>
  );
}
```
Run: `cd frontend && npx vitest run src/features/settings/role-management && npm run typecheck && npm run lint` → Expected: PASS ทั้งหมด, 0 errors. ถ้า test "preserved branch-scope grant is listed" ชน (`/estimates\.read/` พบหลายที่) ให้ใช้ `getAllByText(...).length > 0`.

- [ ] **Step 5: Commit**

```bash
git add frontend/src/features/settings/role-management/components "frontend/src/app/[locale]/(erp)/settings/roles"
git commit -m "feat(roles): add role editor, permission picker and activation panel

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 15: Frontend — คิวคำขอแก้ไข Role (ผู้ตรวจ)

ต่อยอดหน้า `settings/role-requests` เดิม (guard `roles.assign-approval`): เพิ่มตารางคำขอแก้ไข Role ใต้ตารางคำขอมอบ Role. การยกเลิกคำขอโดยผู้ขอทำได้ที่ API เท่านั้นใน G-03b (ไม่มีปุ่มใน UI — ดูข้อจำกัดใน Task 16).

**Files:**
- Create: `frontend/src/features/settings/role-management/components/role-change-request-queue.tsx` (+ `.test.tsx`)
- Modify: `frontend/src/app/[locale]/(erp)/settings/role-requests/page.tsx`

- [ ] **Step 1: เขียน failing test**

สร้าง `role-change-request-queue.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { RoleChangeRequestQueue } from "./role-change-request-queue";

const mocks = vi.hoisted(() => ({ decide: vi.fn(), success: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/role-management-queries", () => ({
  useRoleChangeRequests: () => ({
    data: {
      items: [
        {
          id: "q1", rowVersion: "request-v1", status: "pending",
          role: { id: "r1", name: "PO Approver" }, requestedBy: { id: "u1", name: "สมศรี" }, requestedAtUtc: "2026-10-11T00:00:00Z",
          proposedName: "PO Approver", proposedDescription: null,
          addedPermissionKeys: ["purchase-orders.approve"], removedPermissionKeys: ["customers.read"],
        },
      ],
    },
    isLoading: false, isError: false, refetch: vi.fn(),
  }),
  useDecideRoleChangeRequest: () => ({ mutateAsync: mocks.decide, isPending: false }),
}));

describe("RoleChangeRequestQueue", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.decide.mockResolvedValue({});
  });

  it("lists the role, the requester and what is added and removed", () => {
    render(<RoleChangeRequestQueue />);

    expect(screen.getByText("PO Approver")).toBeInTheDocument();
    expect(screen.getByText("สมศรี")).toBeInTheDocument();
    expect(screen.getByText("purchase-orders.approve")).toBeInTheDocument();
    expect(screen.getByText("customers.read")).toBeInTheDocument();
  });

  it("approves only after the confirmation dialog and sends the request row version", async () => {
    render(<RoleChangeRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "อนุมัติ" }));
    const dialog = await screen.findByRole("dialog");
    expect(mocks.decide).not.toHaveBeenCalled();
    fireEvent.click(within(dialog).getByRole("button", { name: "ยืนยัน" }));

    await waitFor(() => expect(mocks.decide).toHaveBeenCalledWith({ requestId: "q1", decision: "approve", ifMatch: "request-v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("อนุมัติคำขอแล้ว"));
  });

  it("shows the mapped message when the checker is the requester", async () => {
    mocks.decide.mockRejectedValueOnce(new ApiError({ status: 403, code: "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", message: "x" }));
    render(<RoleChangeRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "ปฏิเสธ" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    expect(await screen.findByText("ผู้ตรวจต้องเป็นคนละคนกับผู้ขอ")).toBeInTheDocument();
  });
});
```
Run: `cd frontend && npx vitest run src/features/settings/role-management/components/role-change-request-queue.test.tsx` → Expected: FAIL (module ไม่มี).

- [ ] **Step 2: component + page**

สร้าง `role-change-request-queue.tsx` (รูปแบบเดียวกับ `role-request-queue.tsx` ของ CP-02):

```tsx
"use client";

import React, { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { RoleChangeRequestResponse } from "@/lib/api/api-client";
import { useDecideRoleChangeRequest, useRoleChangeRequests } from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";

interface PendingDecision {
  request: RoleChangeRequestResponse;
  decision: "approve" | "reject";
}

function KeyList({ label, keys }: { label: string; keys: ReadonlyArray<string> | undefined }) {
  if (!keys || keys.length === 0) return null;
  return (
    <div className="text-xs">
      <span className="font-medium">{label}: </span>
      {keys.map((key) => (
        <code key={key} className="mr-2">{key}</code>
      ))}
    </div>
  );
}

export function RoleChangeRequestQueue() {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { data, isLoading, isError, refetch } = useRoleChangeRequests("pending");
  const decideMutation = useDecideRoleChangeRequest();
  const [pending, setPending] = useState<PendingDecision | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const confirm = async () => {
    if (!pending?.request.id || !pending.request.rowVersion) return;
    setErrorMessage(null);
    try {
      await decideMutation.mutateAsync({ requestId: pending.request.id, decision: pending.decision, ifMatch: pending.request.rowVersion });
      toast.success(t(pending.decision === "approve" ? "queue.approved" : "queue.rejected"));
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${roleAdminErrorKey(error)}`));
    } finally {
      setPending(null);
    }
  };

  const columns = useMemo<Column<RoleChangeRequestResponse>[]>(
    () => [
      { id: "role", header: t("queue.role"), className: "min-w-[180px]", cell: (_v, r) => <span className="font-medium text-erp-text-main">{r.role?.name ?? "-"}</span> },
      { id: "requestedBy", header: t("queue.requestedBy"), className: "min-w-[140px]", cell: (_v, r) => <span>{r.requestedBy?.name ?? "-"}</span> },
      { id: "requestedAt", header: t("queue.requestedAt"), className: "min-w-[160px]", cell: (_v, r) => <span>{formatDateTime(r.requestedAtUtc, locale)}</span> },
      {
        id: "changes",
        header: t("queue.changes"),
        className: "min-w-[260px]",
        cell: (_v, r) => (
          <div className="space-y-1">
            <KeyList label={t("queue.added")} keys={r.addedPermissionKeys} />
            <KeyList label={t("queue.removed")} keys={r.removedPermissionKeys} />
          </div>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.manage"),
        isAction: true,
        className: "min-w-[200px] text-right",
        cell: (_v, r) => (
          <div className="flex justify-end gap-2">
            <Button size="sm" variant="primary" disabled={decideMutation.isPending} onClick={() => setPending({ request: r, decision: "approve" })}>
              {t("queue.approve")}
            </Button>
            <Button size="sm" variant="outline" disabled={decideMutation.isPending} onClick={() => setPending({ request: r, decision: "reject" })}>
              {t("queue.reject")}
            </Button>
          </div>
        ),
      },
    ],
    [decideMutation.isPending, locale, t, tCommon]
  );

  const roleName = pending?.request.role?.name ?? "-";
  return (
    <div className="space-y-6">
      <PageHeader title={t("queue.title")} subtitle={t("queue.subtitle")} />
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      <DataTable<RoleChangeRequestResponse>
        columns={columns}
        data={data?.items ?? []}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("errors.GENERIC") : null}
        onRetry={() => refetch()}
        emptyTitle={t("queue.emptyTitle")}
        emptyDescription={t("queue.emptyDescription")}
        hidePagination={true}
        stickyActionColumn={true}
      />
      <ConfirmationModal
        isOpen={pending !== null}
        onClose={() => setPending(null)}
        onConfirm={confirm}
        title={pending?.decision === "approve" ? t("queue.confirmApproveTitle") : t("queue.confirmRejectTitle")}
        message={pending?.decision === "approve" ? t("queue.confirmApproveMessage", { role: roleName }) : t("queue.confirmRejectMessage", { role: roleName })}
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant={pending?.decision === "approve" ? "info" : "danger"}
        isLoading={decideMutation.isPending}
      />
    </div>
  );
}
```

แก้ `frontend/src/app/[locale]/(erp)/settings/role-requests/page.tsx` ให้แสดงสองคิว:

```tsx
"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { RoleRequestQueue } from "@/features/settings/user-administration/components/role-request-queue";
import { RoleChangeRequestQueue } from "@/features/settings/role-management/components/role-change-request-queue";

export default function RoleRequestsPage() {
  const t = useTranslations("userAdmin");

  return (
    <PermissionGuard permission={PERMISSIONS.ROLES_ASSIGN_APPROVAL} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <div className="space-y-10">
        <RoleRequestQueue />
        <RoleChangeRequestQueue />
      </div>
    </PermissionGuard>
  );
}
```
Run: `cd frontend && npx vitest run src/features/settings && npm run typecheck && npm run lint` → Expected: PASS, 0 errors.

- [ ] **Step 3: Commit**

```bash
git add frontend/src/features/settings/role-management/components "frontend/src/app/[locale]/(erp)/settings/role-requests/page.tsx"
git commit -m "feat(roles): add role change request queue

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 16: Verification, เอกสารสถานะ และ gate ปิด G-03b

**Files:**
- Create: `docs/05-engineering/role-management-verification.md`
- Modify: `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/03-contracts/role-management-api-contract.md` (สถานะ → Implemented)

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
Expected: `only th: []`, `only en: []`, exit 0 (ถ้ามีความต่างที่มีมาก่อน G-03b ให้บันทึกในเอกสาร verification — อย่าแก้ key ที่ไม่เกี่ยวข้อง).

- [ ] **Step 2: Gate แบบ focused ของ G-03b (ห้ามรัน IntegrationTests ทั้งโปรเจกต์)**

Run แยกทีละคำสั่ง:

```bash
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~IdentityAccess|FullyQualifiedName~Organization"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~RoleManagementEndpointsTests|FullyQualifiedName~OpenApiContractTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~OrganizationAdministrationEndpointsTests|FullyQualifiedName~UsersEndpointsTests|FullyQualifiedName~NotificationEndpointsTests" -m:1
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
cd frontend && npm run check:api && npm run lint && npm run typecheck && npx vitest run src/features/settings src/components/layout src/lib && npm run build
```
Expected: ทุกคำสั่ง exit 0; จดจำนวน passed/failed ลงเอกสาร verification (ห้ามกล่าวว่า "ผ่าน" โดยไม่มีผลจริง). ชุด regression (Identity/Organization/Users/Notification) ต้องผ่านเพราะ Task 3 ขยาย approval set และ Task 4 เพิ่มคอลัมน์ — ถ้าล้ม ให้แก้สาเหตุใน Task ที่เกี่ยวข้อง ไม่ใช่ปรับ test เดิม. **Full gate** (`dotnet test` ทั้งโซลูชัน 12+ นาที) ไม่ได้รันในขั้นนี้ — ให้ CI ของ PR รัน และบันทึกในเอกสารว่า "ยังไม่รัน full gate ในเครื่อง".

- [ ] **Step 3: ตรวจในเบราว์เซอร์ (ทำจริงหรือบันทึกว่า "ไม่ได้ตรวจ")**

ถ้าสภาพแวดล้อมมี Docker/Browser pane: เริ่ม dev server ตาม `docs/05-engineering/foundation-login-runbook.md` (Backend Test env + `SeedTestData=true`, Frontend dev) แล้วตรวจและบันทึกผลแบบ "ตรวจแล้ว / ไม่ได้ตรวจ" ตามจริง:
1. `/th/settings/roles`: รายการ, ค้นหา, filter สถานะ, badge `ระบบ` ของ Test Admin; สลับ `/en/`.
2. `create` → ใส่ชื่อ + เลือกสิทธิ์ → บันทึก → เปิดหน้าแก้ไข → Role ระบบ (`Test Admin`) เป็น read-only ไม่มีปุ่มบันทึก.
3. Role ที่มีผู้ถือ: ติ๊กสิทธิ์อนุมัติ → เห็นข้อความเตือน → บันทึก → toast "ส่งคำขอแก้ไข..." → Role มี banner รออนุมัติ; ผู้ใช้อีกคนที่มี `roles.assign-approval` เปิด `/th/settings/role-requests` เห็นคำขอและอนุมัติ.
4. ปิดใช้งาน Role ที่มีผู้ถือ → ปุ่ม disabled + แสดงจำนวนผู้ถือ; Role ว่าง → ปิด/เปิดใช้ได้.
5. จอ 375px: ไม่มี horizontal scroll, target ≥ 44px, focus ring เห็น, Tab ผ่านฟอร์ม/picker/modal ได้.
ถ้าข้อใดไม่ได้ตรวจ ให้เขียน "ไม่ได้ตรวจในเบราว์เซอร์" ในเอกสาร — ห้ามอ้างว่าตรวจแล้ว.

- [ ] **Step 4: เอกสาร verification**

สร้าง `docs/05-engineering/role-management-verification.md` (ภาษาไทย รูปแบบเดียวกับ `organization-administration-verification.md`): 1 สถานะ (Implemented G-03b วันที่จริง; focused tests + ผลเบราว์เซอร์ตามจริง; **ยังไม่ผ่าน UAT/Production; ยังไม่รัน full gate ในเครื่อง**), 2 ตารางคำสั่งที่รันจริงพร้อมจำนวน passed/failed จาก Step 2, 3 สิ่งที่ชุดทดสอบพิสูจน์ (anti-escalation เฉพาะ grant ที่เพิ่มและตอนอนุมัติ, system role แก้ไม่ได้, maker–checker เมื่อเพิ่ม grant ให้ Role ที่มีผู้ถือ, ผู้ตรวจ ≠ ผู้ขอ + DB constraint, stale request, last-administrator, `ROLE_IN_USE`, 403 เหมือนกันทุก id/other-org 404, replay/key reuse, ETag/If-Match, audit ไม่มีชื่อ/คำอธิบาย Role, test ล็อก approval set), 4 mutation check (ผลจริงจาก Task 3 Step 7, Task 7 Step 8, Task 8 Step 7, Task 9 Step 7, Task 10 Step 6), 5 การตัดสินใจที่ต้องให้ Business Owner ยืนยัน (D3 กฎ "เพิ่ม grant ใด ๆ ให้ Role ที่ RequiresApproval และมีผู้ถือ" เข้มงวดโดยตั้งใจ, D5 ชุด approval 6 ตัวและรายการ reviewed-not-approval, ปิด Role ต้องไม่มีผู้ถือ), 6 ข้อจำกัดที่ยอมรับ: (ก) R1 `roles.manage`/`roles.assign-approval` ยังไม่อยู่ใน approval set, (ข) R2 ไม่มี notification ถึงผู้ตรวจ, (ค) ไม่มีปุ่มยกเลิกคำขอใน UI (API มี), (ง) เพิ่ม grant ระดับ `branch`/`own` ผ่าน UI ไม่ได้ (API ได้, UI คงของเดิม), (จ) `permissions`/`is_system` ถูก seed เฉพาะ Test env — **Production ต้องมอบ `roles.manage` ให้ Role จริงและตั้ง `is_system` ให้ Role ระบบด้วย bootstrap/สคริปต์ก่อนใช้**, (ฉ) ไม่มีการถอน Role ให้อัตโนมัติเมื่อปิด Role, (ช) คิวคำขอจำกัด 200 รายการและ list ไม่มี pagination.

- [ ] **Step 5: อัปเดตสถานะ (G-03b เท่านั้น)**

- `docs/03-contracts/role-management-api-contract.md`: เปลี่ยน "สถานะ" เป็น Implemented (G-03b).
- `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`: แถวตาราง G-03 (:48) แก้ข้อความสถานะเป็น `G-03a Implemented 2026-10-11; G-03b Implemented <วันที่> (focused tests; G-03c pending; <ผลเบราว์เซอร์ตามจริง>); [แผน G-03](2026-10-10-g03-org-role-authority.md); [Verification G-03a](../../05-engineering/organization-administration-verification.md); [Verification G-03b](../../05-engineering/role-management-verification.md)`; ติ๊ก checkbox "Role management: สร้าง/แก้ Role ..." (:112) — **ห้ามแตะ** checkbox "Approval Authority matrix" และ "Tests" (เป็นของ G-03c).
- `docs/00-overview/implementation-roadmap.md` แถว Foundation (:29): แก้ประโยค "ยังไม่มีการสร้าง/แก้ Role และ Approval Authority matrix (G-03b/G-03c)" เป็น "มีการสร้าง/แก้/ปิดใช้ Role พร้อม maker–checker (G-03b, <วันที่>, focused tests; ยัง<ตรวจ/ไม่ได้ตรวจ>ในเบราว์เซอร์และยังไม่ผ่าน UAT) แต่ยังไม่มี Approval Authority matrix (G-03c)" และต่อท้ายว่า Production ต้องมอบ `roles.manage` และตั้ง system role ให้ Role จริง; เพิ่มลิงก์ [Role Management Verification](../05-engineering/role-management-verification.md) ในคอลัมน์หลักฐาน.
- Run: ตรวจลิงก์ภายในด้วยคำสั่งใน Task 1 Step 6 กับไฟล์ที่แก้ → ต้องไม่มี `MISSING`.

- [ ] **Step 6: Commit**

```bash
git add docs/05-engineering/role-management-verification.md docs/00-overview/implementation-roadmap.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/03-contracts/role-management-api-contract.md
git commit -m "docs(roles): record G-03b verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

## Part 3: G-03c Approval Authority matrix (tasks to be appended)
