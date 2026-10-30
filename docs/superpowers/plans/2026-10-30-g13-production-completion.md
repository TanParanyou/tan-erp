# Wave C Implementation Plan: `G-13` (Production Completion: Work Center, Routing, Capacity, Labor & Subcontracting)

**สถานะ:** Approved for Implementation (2026-10-30) — ดำเนินงานผ่าน `/tiered-orchestration`
**แผนหลัก:** [ERP Gap Closure Plan](2026-10-05-erp-gap-closure-plan.md)
**สัญญาระบบ:** [Production API Contract](../../03-contracts/production-api-contract.md), [Permission Catalog](../../03-contracts/permission-catalog.md)

---

## 1. เป้าหมายและขอบเขต (Goal & Scope)

ปิดช่องว่างของโมดูลการผลิต (**Production `G-13`**) ต่อเนื่องจาก `CP-12`, `G-20` (Labor Timesheet) และ `G-11` (Procurement):
1. **Work Center (`production.work_centers`):**
   - จัดการสถานีงาน/ศูนย์งานผลิต (`WorkCenter`) รายสาขา (`BranchId`), รหัส (`Code`, กำหนดเองหรือรันอัตโนมัติ `WC-{SEQ:5}` ผ่าน `MasterDataCodeAllocator`), ชื่อ (`Name`), ความจุชั่วโมงต่อวัน (`DailyCapacityHours`, ค่าเริ่มต้น `TEST_ONLY` = `8.00` ชม./วัน, ช่วง `0.25 .. 24.00`), อัตราต้นทุนต่อชั่วโมงมาตรฐาน (`HourlyRate >= 0`), สถานะ (`active | inactive`) และ `RowVersion`
   - ห้ามปิดใช้งาน (`inactive`) หากมี BOM Revision ที่ `approved` หรือมีใบสั่งผลิตที่ยังเปิดอยู่ (`draft`, `released`, `in_progress`) อ้างอิงอยู่ (`409 PRODUCTION_WORK_CENTER_IN_USE`)
2. **BOM Routing (`production.bom_routing_steps`):**
   - ผูกลำดับขั้นตอนการผลิต (`BomRoutingStep`) เข้ากับ `BomRevision` (optional เพื่อความเข้ากันได้ย้อนหลัง 100% กับ BOM เดิมใน `CP-12`)
   - แต่ละขั้นประกอบด้วย `Sequence` (`> 0`, ไม่ซ้ำใน Revision เดียวกัน), `OperationCode`, `OperationName`, `WorkCenterId` (จำเป็นเมื่อไม่ได้จ้างผลิตภายนอก), `SetupHours` (`>= 0`), `RunHours` (`>= 0`, ต่อ `OutputQuantity` ของสูตร), `IsSubcontracted` (`bool`), `StdSubcontractCost` (`>= 0`), `Note`
   - ต้องมีอย่างน้อย `SetupHours > 0` หรือ `RunHours > 0` หรือ `IsSubcontracted = true` (`422 BOM_ROUTING_INVALID`)
3. **Work Order Routing Snapshot (`production.work_order_operations`) & Planned Dates:**
   - `WorkOrder` รองรับ `PlannedStartDate` และ `PlannedEndDate` (`DateOnly`, ค่าเริ่มต้น = วันที่สร้างใบสั่งผลิตหากไม่ระบุ)
   - เมื่อสร้าง `WorkOrder` ระบบคัดลอก `BomRoutingStep` จาก BOM Revision ที่อนุมัติแล้วมาสร้างเป็น `WorkOrderOperation` แบบ Snapshot (`PlannedSetupHours = step.SetupHours`, `PlannedRunHours = round(step.RunHours * PlannedQuantity / revision.OutputQuantity, 4)`) ดังนั้นการแก้ไขหรือสร้าง BOM Revision ใหม่ในภายหลังจะไม่กระทบใบสั่งผลิตเดิม
4. **Daily Capacity View (`GET /api/v1/work-centers/capacity`):**
   - คำนวณภาระงานรายวันต่อ `WorkCenter` ในช่วงวันที่ `[fromDate, toDate]` (สูงสุด 31 วัน, ค่าเริ่มต้น 7 วันจากปัจจุบัน)
   - รวมชั่วโมงตามแผนที่ยังไม่เสร็จของ `WorkOrderOperation` ในใบสั่งผลิตสถานะ `released` หรือ `in_progress` กระจายตามช่วง `[PlannedStartDate, PlannedEndDate]` เทียบกับ `DailyCapacityHours` ของแต่ละ `WorkCenter` พร้อมระบุ `utilizationPercent` และ `isOverloaded`
5. **Labor Cost จาก Timesheet (`G-20`) เข้าต้นทุน Work Order:**
   - เมื่ออนุมัติ `LaborTimesheetEntry` (`G-20`) ที่ผูกกับ `WorkOrderId` ใน `ProjectControlStore.TimesheetActionAsync` ระบบจะสะสมชั่วโมง (`LaborHours`) และต้นทุนแรงงาน (`LaborCost`) เข้าสู่ `WorkOrder` อัตโนมัติ (ต้องไม่อยู่ในสถานะ `completed` หรือ `cancelled`)
   - รองรับการบันทึกหรือคำนวณต้นทุนแรงงานเข้า `WorkOrder` โดยตรง และแสดงรายการ `LaborEntries` ที่อนุมัติแล้วใน `WorkOrderProjection` เพื่อตรวจสอบย้อนกลับถึงใบลงเวลา (`TS-YYYY-NNNN`)
6. **Subcontracting Operation (`POST /api/v1/work-orders/{id}/operations/{operationId}/subcontract`):**
   - สำหรับขั้นตอนที่มี `IsSubcontracted = true` บนใบสั่งผลิตสถานะ `released` หรือ `in_progress` สามารถผูกใบสั่งซื้อบริการ (`PurchaseOrder` จาก `CP-10`/`G-11` ที่อยู่ในสถานะ `approved` หรือ `received`) พร้อมบันทึกต้นทุนจ้างผลิตภายนอก (`SubcontractCost`) เข้าสู่ขั้นตอนและสะสมเข้า `WorkOrder.SubcontractCost`
   - รองรับการบันทึกผลขั้นตอนผลิตภายใน (`POST /api/v1/work-orders/{id}/operations/{operationId}/complete` `{ actualHours, note? }`)
   - เมื่อรับผลผลิตเข้าคลัง (`POST /api/v1/work-orders/{id}/completions`) มูลค่าต้นทุนรวมของใบสั่งผลิต (`TotalManufacturingCost`) = **ต้นทุนวัตถุดิบสุทธิ (`NetIssuedValue`) + ต้นทุนแรงงาน (`LaborCost`) + ต้นทุนจ้างผลิตภายนอก (`SubcontractCost`)** จัดสรรตามสัดส่วนผลผลิตที่รับเข้าคลังอย่างแม่นยำ

---

## 2. กฎธุรกิจและสมมติฐาน `TEST_ONLY` (Business Rules & Invariants)

| หัวข้อ | กฎที่ใช้ (`TEST_ONLY` Default) | Error Code |
| :--- | :--- | :--- |
| Work Center Code & Capacity | `Code` ไม่ซ้ำภายในองค์กร (`WC-{SEQ:5}` ถ้าไม่ส่งมา); `DailyCapacityHours` อยู่ในช่วง `0.25 .. 24.00` (ค่าเริ่มต้น `8.00` ชม./วัน) | `409 PRODUCTION_WORK_CENTER_CODE_CONFLICT`, `422 PRODUCTION_WORK_CENTER_INVALID` |
| การปิดใช้งาน Work Center | ห้ามเปลี่ยนสถานะเป็น `inactive` หากถูกอ้างอิงใน BOM Revision ที่ `approved` หรือ Work Order ที่ยังเปิดอยู่ (`draft`, `released`, `in_progress`) | `409 PRODUCTION_WORK_CENTER_IN_USE` |
| BOM Routing Steps | `Sequence > 0` ไม่ซ้ำใน Revision เดียวกัน; ถ้า `!IsSubcontracted` ต้องระบุ `WorkCenterId` ที่ `active`; `SetupHours >= 0`, `RunHours >= 0` | `422 BOM_ROUTING_INVALID`, `409 PRODUCTION_WORK_CENTER_INACTIVE` |
| Work Order Routing Snapshot | คัดลอกจาก Approved BOM Revision ณ เวลาสร้าง Work Order; `PlannedRunHours = round(step.RunHours * plannedQty / bomOutputQty, 4)` | — |
| Subcontracting PO Link | ขั้นตอนต้องมี `IsSubcontracted = true`; PO ต้องอยู่ในองค์กรเดียวกันและมีสถานะ `approved` หรือ `received`; `costAmount > 0` และไม่เกินยอดรวมของ PO | `422 PRODUCTION_OPERATION_NOT_SUBCONTRACTED`, `409 PRODUCTION_SUBCONTRACT_PO_NOT_APPROVED`, `422 PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO` |
| ต้นทุนรวมใบสั่งผลิต (WO Cost) | `TotalCost = NetIssuedMaterialValue + LaborCost + SubcontractCost`; เมื่อรับผลผลิต (`Complete`) จะตัดมูลค่าตามสัดส่วนเข้าคลังสินค้า และงวดสุดท้ายรับมูลค่าคงเหลือทั้งหมด (`TotalCost - CostAllocated`) เพื่อไม่ให้มีเศษตกค้าง | — |
| การยกเลิก Work Order | นอกจากตรวจว่าไม่มีวัตถุดิบค้างเบิกแล้ว ต้องไม่มีต้นทุนแรงงาน (`LaborCost == 0`) และไม่มีต้นทุนจ้างผลิตภายนอก (`SubcontractCost == 0`) | `409 PRODUCTION_HAS_ACTUAL_COSTS` |

---

## 3. ลำดับงานสำหรับ `/tiered-orchestration`

### Phase 1: Backend Domain Architect (`Model: "inherit"`, เฉพาะ `backend/`)
1. **Domain (`TanErp.Domain/Production/` & `TanErp.Domain/DocumentNumbering/`):**
   - เพิ่ม `DocumentTypes.WorkCenters = "work-centers"` (prefix `WC`, padding 5)
   - สร้าง `WorkCenter.cs` (`Id`, `OrganizationId`, `BranchId`, `Code`, `NormalizedCode`, `Name`, `DailyCapacityHours`, `HourlyRate`, `Status`, `RowVersion`, `CreatedByUserId`, `CreatedAtUtc`, `UpdatedAtUtc`)
   - สร้าง `BomRoutingStep.cs` (`Id`, `OrganizationId`, `BomRevisionId`, `Sequence`, `OperationCode`, `OperationName`, `WorkCenterId`, `SetupHours`, `RunHours`, `IsSubcontracted`, `StdSubcontractCost`, `Note`) และขยาย `BomRevision` ให้มี `IReadOnlyCollection<BomRoutingStep> RoutingSteps` (`AddRoutingStep`, `ReplaceRoutingSteps`)
   - สร้าง `WorkOrderOperation.cs` (`Id`, `OrganizationId`, `WorkOrderId`, `Sequence`, `OperationCode`, `OperationName`, `WorkCenterId`, `PlannedSetupHours`, `PlannedRunHours`, `ActualHours`, `IsSubcontracted`, `SubcontractPurchaseOrderId`, `SubcontractCost`, `Status`, `Note`, `CompletedAtUtc`)
   - ขยาย `WorkOrder.cs` เพิ่ม `PlannedStartDate`, `PlannedEndDate`, `LaborHours`, `LaborCost`, `SubcontractCost`, `IReadOnlyCollection<WorkOrderOperation> Operations` พร้อมเมธอด `AddOperation`, `RecordLabor(decimal hours, decimal cost, DateTimeOffset now)`, `CompleteOperation(Guid operationId, decimal actualHours, string? note, DateTimeOffset now)`, `RecordSubcontract(Guid operationId, Guid purchaseOrderId, decimal costAmount, string? note, DateTimeOffset now)` และปรับ `TotalManufacturingCost = NetIssuedValue + LaborCost + SubcontractCost` ใน `Complete(...)`
2. **Persistence & Migration (`TanErp.Infrastructure/Persistence/`):**
   - อัปเดต `AppDbContext.cs`, `ProductionConfigurations.cs` (`production.work_centers`, `production.bom_routing_steps`, `production.work_order_operations`, คอลัมน์ใหม่บน `production.work_orders`)
   - สร้าง EF Core Migration `AddProductionCompletionG13`
3. **Application & Infrastructure (`TanErp.Application/Production/`, `ProductionStore.cs`, `ProjectControlStore.cs`):**
   - ขยาย `ProductionModels.cs`, `IProductionStore.cs`, `ProductionHandler.cs`, `ProductionStore.cs`
   - อัปเดต `ProjectControlStore.TimesheetActionAsync` ให้เรียก `workOrder.RecordLabor(entry.Hours, entry.CostAmount, now)` เมื่ออนุมัติใบลงเวลาที่ผูกกับ `WorkOrderId`
   - เพิ่ม Controller `WorkCentersController.cs` (`/api/v1/work-centers`, `/api/v1/work-centers/capacity`) และขยาย `BomsController.cs`, `WorkOrdersController.cs` (`/api/v1/work-orders/{id}/operations/{operationId}/complete`, `/api/v1/work-orders/{id}/operations/{operationId}/subcontract`)
   - เพิ่ม Error Codes และคำแปล 2 ภาษาใน `ProblemDetailsMapper.cs`, `Errors.resx`, `Errors.en.resx`
4. **Tests:**
   - Unit tests ใน `backend/tests/TanErp.UnitTests/Production/ProductionCompletionDomainTests.cs`
   - Integration tests ใน `backend/tests/TanErp.IntegrationTests/Api/ProductionEndpointsTests.cs` (Work Center CRUD + Capacity View, BOM Routing + WO Routing Snapshot immutability, Timesheet G-20 labor cost + Subcontract PO G-11 cost flowing into WO completion finished goods valuation)

### Phase 2: OpenAPI Contract Bridge (`Main Orchestrator`)
- รัน `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpenApiContractTests" --no-build`
- รัน `cd frontend && npm run generate:api`

### Phase 3: Frontend UI Specialist (`Model: "flash"`, เฉพาะ `frontend/`)
- อัปเดต `api-client.ts`, `production-status.ts`, `production-queries.ts`
- สร้าง `work-center-list.tsx` (จัดการ Work Center + มุมมองตาราง Capacity รายวัน) และเพิ่มแท็บ/ลิงก์ในหน้า Production
- อัปเดต `bom-detail.tsx` / `bom-list.tsx` ให้รองรับการกำหนด Routing Steps ใน BOM Revision
- อัปเดต `work-order-detail.tsx` / `work-order-list.tsx` ให้แสดงตาราง Operations Snapshot, ปุ่มบันทึกชั่วโมงจริง (`Complete Operation`), ปุ่มผูก PO จ้างผลิตภายนอก (`Subcontract PO`), และการ์ดสรุปต้นทุนรวม (Material + Labor จาก Timesheet + Subcontract)
- เพิ่มคำแปล `th.json` / `en.json` ครบ 100% และขยาย Vitest ใน `production-components.test.tsx`

### Phase 4: Verification Gates & Documentation (`Main Orchestrator`)
- อัปเดต `docs/05-engineering/production-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/01-business/open-validation-questions.md`
- รัน Verification Gates ครบทั้ง Backend และ Frontend
