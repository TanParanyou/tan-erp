# Wave C Implementation Plan: `G-20` (Labor Timesheet) & `G-10` (Project WBS, Commitment & Actual Cost)

**สถานะ:** Approved for Implementation (2026-10-29) — ดำเนินงานผ่าน `/tiered-orchestration`
**แผนหลัก:** [ERP Gap Closure Plan](2026-10-05-erp-gap-closure-plan.md)
**สัญญาระบบ:** [Project API Contract](../../03-contracts/project-api-contract.md), [Permission Catalog](../../03-contracts/permission-catalog.md)

---

## 1. เป้าหมายและขอบเขต (Goal & Scope)

ปิดช่องว่างของ Wave C สองรายการแรกที่เชื่อมโยงกันโดยตรง:
1. **`G-10` — Project WBS, Commitment & Actual Cost:**
   - โครงสร้างต้นไม้ WBS (`ProjectWbsNode`, ลึกสุด 3 ระดับ `TEST_ONLY`) ต่อโครงการ
   - ผูก `ProjectBudgetLine` เข้ากับ `WbsNodeId` (optional เพื่อ backward compatibility กับข้อมูลเดิม; รายการที่ไม่ระบุจะรวมอยู่ในกลุ่ม `Unassigned`)
   - สมุดบัญชีต้นทุนโครงการแบบเพิ่มรายการเท่านั้น (`ProjectCostLedgerEntry` — Append-only) อ้างอิงเอกสารต้นทาง (`purchase_order` สำหรับภาระผูกพัน, `goods_receipt` สำหรับลดภาระผูกพันเมื่อรับของ, `stock_issue` / `work_order_issue` / `work_order_return` / `labor_timesheet` สำหรับต้นทุนจริง) พร้อมรายการกลับบัญชี (`reversal`) เมื่อยกเลิกเอกสารต้นทาง
   - รายงานเปรียบเทียบ **Budget vs Commitment vs Actual** รายหมวดและราย WBS node (คำนวณผ่าน Dapper / SQL rollup)
   - การแจ้งเตือนเมื่อต้นทุนผูกพันรวมต้นทุนจริงเกินงบปัจจุบัน (`project.budget-overrun` ผ่าน `INotificationPublisher` ของ `G-02`)
2. **`G-20` — Labor Timesheet:**
   - อัตราค่าแรงมาตรฐาน (`LaborRate` — `TEST_ONLY`) ต่อผู้ใช้หรือตำแหน่ง/บทบาท (User-specific ก่อน, fallback เป็น Role, fallback เป็นค่าเริ่มต้นองค์กร `TEST_ONLY` 150 THB/ชม.)
   - ใบบันทึกเวลาทำงานรายวัน (`LaborTimesheetEntry`) ต่อผู้ใช้ ผูกกับ Project (+ WBS Node ถ้าระบุ) หรือ Work Order (`G-13` รองรับระดับหัวใบสั่งผลิตก่อน และขยายเป็น operation ใน `G-13`)
   - การตรวจชั่วโมงทำงานต่อวัน: ห้ามเกิน 24 ชั่วโมง/วัน (`TIMESHEET_DAILY_HOURS_EXCEEDED`), และหากรวมเกิน 8 ชั่วโมง/วัน จะติดสถานะเตือน (`isOvertimeWarning = true`) เพื่อให้หัวหน้างานเห็นชัดเจน
   - การอนุมัติโดยหัวหน้างาน (**Maker ≠ Checker**: ผู้บันทึกห้ามอนุมัติรายการของตนเอง `TIMESHEET_SELF_APPROVAL`)
   - เมื่ออนุมัติ: ล็อกรายการห้ามแก้ไข (`TIMESHEET_IMMUTABLE`), บันทึก `ProjectCostLedgerEntry` (หมวด `labor`, ประเภท `actual`) เข้าโครงการ และหากผูกกับ Work Order จะสะสมต้นทุนแรงงานเข้า Work Order ด้วย

---

## 2. กฎธุรกิจและสมมติฐาน `TEST_ONLY` (Business Rules & Invariants)

| หัวข้อ | กฎที่ใช้ (`TEST_ONLY` Default) | Error Code |
| :--- | :--- | :--- |
| ความลึกของ WBS | สูงสุด 3 ระดับ (`Level` 1..3); รหัส `Code` ไม่ซ้ำภายในโครงการเดียวกัน | `422 PROJECT_WBS_DEPTH_EXCEEDED`, `409 PROJECT_WBS_CODE_CONFLICT` |
| การลบ WBS Node | ห้ามลบหากมี Node ลูก, มี Budget Line ผูกอยู่, หรือมี Cost Ledger / Timesheet อ้างอิง | `409 PROJECT_WBS_IN_USE` |
| การผูก Budget Line กับ WBS | `wbsNodeId` เป็น optional; ถ้าระบุต้องเป็น WBS ของโครงการเดียวกัน | `422 PROJECT_BUDGET_INVALID` |
| สมุดบัญชีต้นทุน (`ProjectCostLedgerEntry`) | Append-only เสมอ; `entryType` ∈ `{commitment, actual}`, `sourceType` ∈ `{purchase_order, goods_receipt, stock_issue, work_order_issue, work_order_return, labor_timesheet}`; ยกเลิกเอกสาร = เพิ่มแถว `isReversal = true` จำนวนเงินติดลบ | `409 PROJECT_COST_ENTRY_IMMUTABLE` |
| การแจ้งเตือนงบเกิน (`G-02`) | เมื่อ `Committed + Actual > CurrentBudget` (และ `CurrentBudget > 0`) หลังโพสต์รายการใหม่ ระบบสร้าง Notification `project.budget-overrun` แจ้งผู้ถือสิทธิ์ `projects.update` (dedupe ตาม `(projectId, ledgerEntryId)`) | — |
| ชั่วโมงทำงานต่อวัน (`G-20`) | ต่อครั้ง `0.25 .. 24.00` ชม.; รวมทุกใบของคนเดียวกันในวันเดียวกันต้อง `<= 24.00` ชม.; ถ้ารวม `> 8.00` ชม. จะคืน `isOvertimeWarning = true` | `422 TIMESHEET_HOURS_INVALID`, `422 TIMESHEET_DAILY_HOURS_EXCEEDED` |
| เป้าหมายการลงเวลา | ต้องระบุ `projectId` หรือ `workOrderId` อย่างน้อยหนึ่งอย่าง; ถ้าระบุ `wbsNodeId` ต้องเป็นของ `projectId` นั้น; โครงการต้องอยู่สถานะ `active` หรือ `on_hold` | `422 TIMESHEET_TARGET_REQUIRED`, `409 TIMESHEET_PROJECT_NOT_ACTIVE` |
| Maker–Checker ของ Timesheet | ผู้อนุมัติ/ปฏิเสธ (`Approve`/`Reject`) ต้องไม่ใช่ผู้บันทึก (`WorkerUserId` หรือ `CreatedByUserId`) | `403 TIMESHEET_SELF_APPROVAL` |
| ความไม่เปลี่ยนแปลงหลังอนุมัติ | รายการที่ `approved` แล้วห้ามแก้ไขหรือยกเลิก | `409 TIMESHEET_IMMUTABLE`, `409 TIMESHEET_INVALID_STATE` |

---

## 3. สิทธิ์การใช้งานเพิ่มเติม (Permissions)

ใช้สิทธิ์ของโมดูล Project เดิม (`projects.read`, `projects.update`) สำหรับ WBS, Cost Ledger และการลงเวลา/อนุมัติ เพื่อไม่ให้กระทบตาราง Role Permission ที่มีอยู่เดิม และเพิ่มความชัดเจนใน Contract:
- `projects.read`: ดู WBS, สมุดบัญชีต้นทุน (Cost Ledger), รายงานเปรียบเทียบงบ, และรายการ Timesheet ของโครงการ
- `projects.update`: จัดการ WBS nodes, บันทึก/ส่ง Timesheet, และตั้งค่าอัตราค่าแรง `TEST_ONLY`
- `projects.change-orders.approve`: อนุมัติ/ปฏิเสธ Timesheet (และรับแจ้งเตือนเมื่อมีรายการรออนุมัติหรืองบเกิน)

---

## 4. ลำดับงานสำหรับ `/tiered-orchestration`

### Phase 1: Backend Specialist (`Model: "inherit"`, เฉพาะ `backend/`)
1. **Domain (`TanErp.Domain/Projects/`):**
   - สร้าง `ProjectWbsNode.cs` (Id, OrganizationId, ProjectId, ParentId, Code, Name, Level 1..3, SortOrder, RowVersion)
   - อัปเดต `ProjectBudgetLine.cs` ให้รองรับ `Guid? WbsNodeId`
   - สร้าง `ProjectCostLedgerEntry.cs` (Id, OrganizationId, ProjectId, WbsNodeId?, Category, EntryType `commitment|actual`, SourceType, SourceId, SourceNumber, Description, Amount, IsReversal, OccurredAtUtc, ActorUserId)
   - สร้าง `LaborRate.cs` และ `LaborTimesheetEntry.cs` (Id, OrganizationId, BranchId, Number, WorkerUserId, WorkDate, Hours, HourlyRate, CostAmount, ProjectId?, WbsNodeId?, WorkOrderId?, TaskDescription, Status `draft|submitted|approved|rejected|cancelled`, IsOvertimeWarning, CreatedByUserId, DecidedByUserId?, DecisionNote?, RowVersion)
   - เพิ่ม `DocumentTypes.LaborTimesheets = "labor-timesheets"` (prefix `TS`)
   - เพิ่ม `NotificationTypes.ProjectBudgetOverrun = "project.budget-overrun"` ใน `NotificationValues.cs`, `NotificationEvents.cs`, และ `NotificationTypeRegistry.cs`
2. **Infrastructure & EF Migration (`TanErp.Infrastructure/`):**
   - สร้าง EF Configurations สำหรับ `ProjectWbsNode`, `ProjectCostLedgerEntry`, `LaborRate`, `LaborTimesheetEntry` และอัปเดต `ProjectBudgetLineConfiguration`
   - สร้าง Migration `AddProjectWbsCostLedgerAndTimesheets`
   - อัปเดต `ProjectControlStore`:
     - จัดการ WBS CRUD (`AddWbsNodeAsync`, `UpdateWbsNodeAsync`, `DeleteWbsNodeAsync`)
     - อัปเดต `ReplaceBudgetAsync` ให้รับ `WbsNodeId`
     - จัดการ `LaborRate` และ `LaborTimesheetEntry` (`CreateTimesheetAsync`, `SubmitTimesheetAsync`, `DecideTimesheetAsync`, `CancelTimesheetAsync`) พร้อมโพสต์เข้า `ProjectCostLedgerEntries` เมื่ออนุมัติ
     - คำนวณ Cost Summary & WBS Rollup (Budget vs Commitment vs Actual) และแนบใน `ProjectControlProjection`
   - เชื่อมการโพสต์ `ProjectCostLedgerEntry` อัตโนมัติใน:
     - `ProcurementStore`: ตอน Approve PO (`commitment` +), Cancel PO (`commitment` reversal -), และ Post Goods Receipt (`commitment` reduction -)
     - `InventoryStore`: ตอน Issue Stock ที่ผูก `projectId` (`actual` +) และ Return Materials จาก Work Order (`actual` -)
     - ตรวจสอบ Budget Overrun หลังเพิ่มรายการใน `ProjectCostLedgerEntries` เพื่อยิง `NotificationEvents.ProjectBudgetOverrun`
3. **API & Error Mapping (`TanErp.Api/`):**
   - เพิ่ม Endpoints ใน `ProjectControlController.cs`:
     - `POST /api/v1/projects/{projectId}/wbs`
     - `PUT /api/v1/projects/{projectId}/wbs/{wbsNodeId}`
     - `POST /api/v1/projects/{projectId}/wbs/{wbsNodeId}/delete`
     - `POST /api/v1/projects/{projectId}/timesheets`
     - `POST /api/v1/projects/{projectId}/timesheets/{timesheetId}/submit`
     - `POST /api/v1/projects/{projectId}/timesheets/{timesheetId}/approve`
     - `POST /api/v1/projects/{projectId}/timesheets/{timesheetId}/reject`
     - `POST /api/v1/projects/{projectId}/timesheets/{timesheetId}/cancel`
     - `PUT /api/v1/projects/{projectId}/labor-rates` (ตั้งค่าอัตราค่าแรง `TEST_ONLY` ต่อผู้ใช้)
   - ลงทะเบียน Error Codes ใหม่ใน `ProblemDetailsMapper.cs` และ `Errors.th.resx` / `Errors.en.resx`
4. **Tests (`TanErp.UnitTests` & `TanErp.IntegrationTests`):**
   - Unit tests สำหรับ WBS hierarchy depth (<= 3), Timesheet daily hour cap & overtime warning, Maker-Checker, และ Cost Ledger invariants
   - Integration tests ใน `ProjectControlEndpointsTests.cs` ทดสอบ WBS, Budget Line ผูก WBS, Timesheet lifecycle + post เข้า Cost Ledger, PO Commitment + Stock Issue Actual + Overrun Notification

### Phase 2: Main Orchestrator (OpenAPI Bridge)
- รัน `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpenApiContractTests" --no-build`
- รัน `cd frontend && npm run generate:api`

### Phase 3: Frontend Specialist (`Model: "flash"`, เฉพาะ `frontend/`)
- อัปเดต `frontend/src/lib/api/api-client.ts` และ `frontend/src/features/projects/api/project-queries.ts`
- สร้าง `project-wbs-card.tsx` (จัดการโครงสร้าง WBS 3 ระดับ + ตารางเปรียบเทียบ Budget vs Commitment vs Actual ราย WBS)
- อัปเดต `project-budget-card.tsx` ให้เลือกผูก WBS Node ได้ และแสดงยอด Actual Cost / Cost Variance
- สร้าง `project-cost-ledger-card.tsx` (ตารางสมุดบัญชีต้นทุนโครงการ Append-only อ้างอิงเอกสารต้นทาง)
- สร้าง `project-timesheets-card.tsx` (บันทึกเวลาทำงานรายวัน, แจ้งเตือนเมื่อเกิน 8 ชม., ปุ่มส่ง/อนุมัติ/ปฏิเสธแบบ Maker-Checker, และตั้งค่าอัตราค่าแรง `TEST_ONLY`)
- เพิ่มคำแปลครบทั้ง `messages/th.json` และ `messages/en.json` (รวมถึงชนิดการแจ้งเตือน `project.budget-overrun`)
- อัปเดตและรัน Vitest ใน `frontend/src/features/projects/`

### Phase 4: Final Verification & Docs
- รัน Backend & Frontend Verification Gates
- อัปเดต `docs/05-engineering/project-handover-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, และ `docs/01-business/open-validation-questions.md`
