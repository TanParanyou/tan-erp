# Wave C Implementation Plan: `G-14` (MRP Completion: Item/Warehouse Planning Parameters, Working Calendar, Unit Conversion & Reproducible Hash)

**สถานะ:** Approved for Implementation (2026-10-30) — ดำเนินงานผ่าน `/tiered-orchestration` (Slice สุดท้ายของ Wave C)
**แผนหลัก:** [ERP Gap Closure Plan](2026-10-05-erp-gap-closure-plan.md)
**สัญญาระบบ:** [MRP API Contract](../../03-contracts/mrp-api-contract.md), [Production API Contract](../../03-contracts/production-api-contract.md)

---

## 1. เป้าหมายและขอบเขต (Goal & Scope)

ปิดช่องว่างของโมดูลการวางแผนความต้องการวัสดุ (**MRP `G-14`**) ซึ่งเป็น Slice สุดท้ายของ Wave C:
1. **Planning Parameters ต่อ Item / Warehouse (`mrp.item_planning_parameters`):**
   - กำหนดพารามิเตอร์วางแผนต่อสินค้า (`ItemId`) และคลังสินค้า (`WarehouseId?` — `null` หมายถึงค่ามาตรฐานระดับองค์กรของสินค้านั้น และถ้าระบุ `WarehouseId` จะเป็นค่าเฉพาะคลัง):
     - `SafetyStock` (`>= 0`): สต็อกปลอดภัยขั้นต่ำที่ต้องคงไว้
     - `MinOrderQuantity` (`>= 0`): จำนวนสั่งซื้อ/สั่งผลิตขั้นต่ำ (MOQ)
     - `LotMultiple` (`>= 0`): ตัวคูณขนาดรุ่น (Lot Multiple — ปัดขึ้นเป็นทวีคูณ)
     - `LeadTimeDays` (`int?` `0..365`): ระยะเวลานำส่ง/ผลิตเฉพาะสินค้า (ถ้าเป็น `null` จะใช้ `PurchaseLeadTimeDays` หรือ `ProductionLeadTimeDays` ของรอบคำนวณ)
   - เมื่อคำนวณระดับองค์กร ระบบรวม `SafetyStock` ของทุกคลังที่มีผล (หรือใช้ค่ามาตรฐานองค์กรของสินค้านั้น) และใช้ `MinOrderQuantity`, `LotMultiple`, `LeadTimeDays` จากพารามิเตอร์ของสินค้า
2. **Working Calendar (`mrp.working_calendar_holidays`):**
   - จัดการวันหยุดทำการขององค์กร/สาขา (`HolidayDate`, `Name`, `BranchId?`)
   - เมื่อ `MrpEngine` ถอยวันสั่ง (`OrderBy` จาก `NeedBy` ตาม `LeadTimeDays`) จะนับถอยหลังเฉพาะวันทำงานโดยข้ามวันที่อยู่ใน `Holidays`
   - เชื่อมต่อกับ **Daily Capacity View ของ `G-13` (`ProductionStore.GetCapacityViewAsync`)**: วันหยุดใน `WorkingCalendarHolidays` จะมีความจุสถานีงาน (`CapacityHours`) เป็น `0` ชม. โดยอัตโนมัติ
3. **การแปลงหน่วยนับด้วย Item Master Conversion (`UnitId` ใน Demand):**
   - `MrpDemandRequest` รองรับ `unitId?: Guid` (ทางเลือก; ถ้าไม่ระบุถือเป็นหน่วยฐานของ Item)
   - ถ้าระบุ `unitId` ที่ไม่ใช่หน่วยฐาน ระบบค้นหาอัตราแปลงหน่วย (`ItemUnitConversions` เฉพาะสินค้าก่อน แล้วจึงหาใน `UnitConversions` กลางที่ `FromUnitId == unitId && ToUnitId == item.BaseUnitId && IsActive`) เพื่อแปลงเป็นปริมาณหน่วยฐาน (`round(qty × factor, 4)`) ก่อนเข้า Snapshot; หากไม่พบคู่แปลงหน่วย คืน `422 MRP_UNIT_CONVERSION_NOT_FOUND`
4. **Reproducible `InputHash` & Pure `MrpEngine` Unit Tests (TDD):**
   - ขยาย `MrpEngine.HashOf(MrpSnapshot)` ให้รวมพารามิเตอร์วางแผน (`SafetyStock`, `MinOrderQuantity`, `LotMultiple`, `LeadTimeDays`) และวันหยุด (`Holidays`) เมื่อมีการตั้งค่า โดยยังคงสร้าง `InputHash` เดิมทุกประการเมื่อพารามิเตอร์ทั้งหมดเป็นค่าเริ่มต้นว่าง เพื่อให้รอบคำนวณเก่าสามารถตรวจซ้ำ (Reproducible) ได้ 100%

---

## 2. กฎธุรกิจและสมมติฐาน `TEST_ONLY` (Business Rules & Invariants)

| หัวข้อ | กฎที่ใช้ (`TEST_ONLY` Default) | Error Code |
| :--- | :--- | :--- |
| พารามิเตอร์วางแผน (`MrpItemPlanningParameter`) | `SafetyStock >= 0`, `MinOrderQuantity >= 0`, `LotMultiple >= 0`, `LeadTimeDays` `null` หรือ `0..365` | `422 MRP_PLANNING_PARAMETER_INVALID` |
| การคิด Safety Stock | สต็อกพร้อมใช้เหนือเกณฑ์ปลอดภัย = `max(0, available - safetyStock)`; หาก `available < safetyStock` ส่วนที่ขาด (`safetyStock - available`) จะถูกเติมเป็นความต้องการ (`safety_stock`) ในวันที่มีความต้องการแรก (หรือ ณ `asOfDate` หากไม่มี Demand อื่น) | — |
| การปัดขนาดสั่ง (MOQ & Lot Multiple) | เมื่อเกิดยอดขาดสุทธิ `shortage > 0`: `qty = max(shortage, MOQ)`; ถ้า `LotMultiple > 0` ให้ปัดขึ้น `qty = ceil(qty / LotMultiple) × LotMultiple`; ส่วนที่สั่งเกินความต้องการสุทธิ (`qty - shortage`) จะคงเหลือในสต็อกคาดการณ์เพื่อหักกลบความต้องการในวันถัดไป | — |
| การถอยวันสั่งตามปฏิทินวันทำงาน | ถอยจาก `needBy` จำนวน `leadDays` วันทำการ โดยข้ามวันที่ปรากฏใน `Holidays` (หาก `leadDays == 0` แล้ว `needBy` ตรงกับวันหยุด ให้ถอยไปยังวันทำการก่อนหน้า) | `409 MRP_HOLIDAY_ALREADY_EXISTS`, `422 MRP_HOLIDAY_INVALID` |
| การแปลงหน่วยใน Demand | ใช้ `ItemUnitConversion` ก่อน `UnitConversion` กลาง (`FromUnitId → BaseUnitId`); ถ้าไม่พบปฏิเสธทันที | `422 MRP_UNIT_CONVERSION_NOT_FOUND` |

---

## 3. ลำดับงานสำหรับ `/tiered-orchestration`

### Phase 1: Backend Domain Architect (`Model: "inherit"`, เฉพาะ `backend/`)
1. **TDD Unit Tests ก่อน (`backend/tests/TanErp.UnitTests/Mrp/MrpEngineTests.cs`):**
   - เขียน Unit Tests ทดสอบ `MrpEngine.Plan` สำหรับ:
     - `SafetyStock`: สต็อกต่ำกว่า Safety Stock หรือถูกกันไว้ไม่ให้ใช้ต่ำกว่าเกณฑ์
     - `MinOrderQuantity (MOQ)` และ `LotMultiple`: ปัดขึ้นและนำส่วนเกินไปหักกลบ Demand วันถัดไป
     - `LeadTimeDays` เฉพาะ Item และ `Holidays` (Working Calendar): ถอยวันสั่งข้ามวันหยุด
     - `InputHash` Reproducibility: Snapshot เดิมได้ Hash เดิมเป๊ะ และเปลี่ยนทันทีเมื่อมี Planning Parameter หรือ Holiday เปลี่ยน
2. **Domain & Persistence (`TanErp.Domain/Mrp/`, `AppDbContext.cs`, `MrpConfigurations.cs`):**
   - อัปเดต `MrpEngine.cs` และ `MrpValues.cs` (`MrpDemandSource.SafetyStock = "safety_stock"`)
   - สร้าง `MrpItemPlanningParameter.cs` และ `WorkingCalendarHoliday.cs`
   - อัปเดต `AppDbContext.cs`, `MrpConfigurations.cs` และสร้าง Migration `AddMrpCompletionG14`
3. **Application, Store & Controllers (`TanErp.Application/Mrp/`, `MrpStore.cs`, `ProductionStore.cs`, `MrpRunsController.cs`):**
   - อัปเดต `MrpModels.cs`, `IMrpStore.cs`, `MrpHandler.cs`, `MrpStore.cs` (ดึง Planning Parameters, Holidays, และแปลงหน่วย Demand จาก `ItemUnitConversions` / `UnitConversions`)
   - อัปเดต `ProductionStore.GetCapacityViewAsync` ให้อ่าน `WorkingCalendarHolidays` และกำหนด `CapacityHours = 0` ในวันหยุด
   - ขยาย `MrpRunsController.cs` (หรือเพิ่ม Endpoints ภายใต้ `/api/v1/mrp/planning-parameters` และ `/api/v1/mrp/calendar-holidays`)
   - เพิ่ม Error Codes ใน `ProblemDetailsMapper.cs`, `Errors.resx`, `Errors.en.resx`
   - ขยาย Integration Tests ใน `backend/tests/TanErp.IntegrationTests/Api/MrpEndpointsTests.cs`

### Phase 2: OpenAPI Contract Bridge (`Main Orchestrator`)
- รัน `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpenApiContractTests" --no-build`
- รัน `cd frontend && npm run generate:api`

### Phase 3: Frontend UI Specialist (`Model: "flash"`, เฉพาะ `frontend/`)
- อัปเดต `api-client.ts`, `mrp-status.ts`, `mrp-queries.ts`
- สร้าง `mrp-settings-drawer.tsx` (2 แท็บ: พารามิเตอร์การวางแผนรายสินค้า Safety Stock / MOQ / Lot Multiple / Lead Time + ปฏิทินวันหยุดทำการ Working Calendar)
- อัปเดต `mrp-run-list.tsx`, `mrp-run-editor.tsx` (รองรับการเลือกหน่วยนับ `unitId` ในบรรทัด Demand และปุ่มเปิดตั้งค่าพารามิเตอร์/ปฏิทินวันหยุด), และ `mrp-run-detail.tsx`
- เพิ่มคำแปล `th.json` / `en.json` ครบ 100% และขยาย Vitest ใน `mrp-components.test.tsx`

### Phase 4: Verification Gates & Documentation (`Main Orchestrator`)
- อัปเดต `docs/05-engineering/mrp-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/01-business/open-validation-questions.md`
- รัน Verification Gates ครบทั้ง Backend และ Frontend
