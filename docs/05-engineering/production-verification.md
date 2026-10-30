# Production Verification (CP-12 + Wave C G-13)

**สถานะ:** Implemented 2026-10-04 (`CP-12`) และขยายเสร็จสมบูรณ์ 2026-10-30 (**Wave C `G-13`**: Work Centers, BOM Routing Steps, Work Order Routing Snapshot, Daily Capacity View, WO Labor Cost จาก Timesheet `G-20`, และ Subcontracting PO Link จาก `G-11`); หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ Production UAT หรือ sign-off. กฎ: [Production API Contract](../03-contracts/production-api-contract.md), [แผน G-13](../superpowers/plans/2026-10-30-g13-production-completion.md).

## ผลที่รันจริง

- **Backend Unit Tests (`TanErp.UnitTests`)**: `679/679 passed` — รวม `ProductionCompletionDomainTests` (`3/3`) ตรวจสอบ:
  - `WorkCenter`: ตรวจความจุต่อวัน (`0.25 .. 24.00` ชม./วัน, ค่าเริ่มต้น `8.00`), อัตราค่าแรงต่อชั่วโมง (`>= 0`), การเปลี่ยนสถานะ `active` / `inactive`
  - `BomRevision` & `BomRoutingStep`: ตรวจลำดับขั้นตอน (`Sequence`) ไม่ซ้ำภายใน Revision, ขั้นตอนผลิตภายในต้องระบุ `WorkCenterId` และมีเวลา Setup/Run > 0, ขั้นตอนจ้างผลิตภายนอก (`IsSubcontracted = true`) ห้ามผูก `WorkCenterId` ภายใน
  - `WorkOrder` Total Cost & Operations: ตรวจการจัดสรรต้นทุนรวม (`NetIssuedValue + LaborCost + SubcontractCost`) เข้าสู่ผลผลิตทั้งกรณีรับผลผลิตบางส่วนและรับครบจำนวน รวมถึงการบล็อกการยกเลิกเมื่อมีต้นทุนแรงงาน/จ้างผลิตภายนอก (`PRODUCTION_HAS_ACTUAL_COSTS`)
- **Backend Architecture Tests (`TanErp.ArchitectureTests`)**: `4/4 passed`
- **Backend Integration Tests (`ProductionEndpointsTests` + `OpenApiContractTests`)**: `6/6 passed` บน PostgreSQL 17:
  - **CP-12 เดิม (`3/3`):** BOM lifecycle + Maker–Checker + Cycle detection, Work Order เบิก/คืน/รับผลผลิตบางส่วนและเต็มจำนวน, และการยกเลิก/สต็อกไม่พอ/สิทธิ์การใช้งาน
  - **Work Center, Routing Snapshot Immutability & Daily Capacity View (`G-13`):** สร้าง Work Center (`WC-SAW`, ความจุ 8 ชม./วัน), ผูก Routing เข้ากับ BOM Revision 1 และอนุมัติ, ป้องกันการปิดใช้งาน Work Center ที่ถูกอ้างอิงใน Approved BOM (`PRODUCTION_WORK_CENTER_IN_USE`), สร้าง Work Order พร้อมคัดลอก Routing Snapshot (`PlannedSetupHours = 1`, `PlannedRunHours = 8`), สร้างและอนุมัติ BOM Revision 2 ที่เปลี่ยนเวลาเดินเครื่องแล้วพิสูจน์ว่า Routing Snapshot ใน Work Order เดิมไม่เปลี่ยนแปลง, และตรวจสอบ `/api/v1/work-centers/capacity` คำนวณชั่วโมงวางแผน 9 ชม. เทียบความจุ 8 ชม. (`UtilizationPercent = 112.5%`, `IsOverloaded = true`)
  - **Work Order Total Manufacturing Cost (`G-13` + `G-20` + `G-11`):** เบิกวัตถุดิบ (`5,100` บาท) + บันทึก/ส่ง/อนุมัติใบลงเวลา `G-20` ที่ผูกกับ `WorkOrderId` (`4` ชม. × `150` = `600` บาท) + บันทึกผลขั้นตอนภายใน + ผูกใบสั่งซื้อจ้างผลิตภายนอก `G-11` (`1,200` บาท, พร้อมตรวจบล็อกกรณีเกินยอด PO `PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO`) → รับผลผลิตเข้าคลังครบ 2 ชิ้น และพิสูจน์ว่ามูลค่าสต็อกสินค้าสำเร็จรูป (`StockBalance.TotalValue`) เท่ากับ `5,100 + 600 + 1,200 = 6,900` บาท (`AverageCost = 3,450` บาท/ชิ้น) พอดีเป๊ะ พร้อมตรวจ `reconciliation` ไม่มีผลต่าง
- **Frontend Verification (`frontend/src/features/production`)**:
  - `npm run lint` (`0 errors`), `npx tsc --noEmit` (`0 errors`), `npm run build` (`89/89 routes compiled`)
  - Vitest `src/features/production/components/production-components.test.tsx`: `17/17 passed` (ครอบคลุม BOM Routing Steps, Work Center & Daily Capacity Drawer, Work Order 4-part Cost Breakdown, Operations Snapshot & Subcontract PO modal, และตาราง Approved Labor Timesheets)

## ข้อจำกัด

ค่าเริ่มต้นทั้งหมดเป็น `TEST_ONLY` รอ Engineering/Production/Finance ยืนยัน (`VQ-PROD-01`..`VQ-PROD-03`: ความจุมาตรฐานต่อวันของ Work Center, การกระจายชั่วโมงงานตามช่วงวันที่วางแผนแบบเส้นตรง, และการรวมค่าแรง/ค่าจ้างผลิตภายนอกเข้ามูลค่าสินค้าสำเร็จรูปโดยไม่คิดโสหุ้ยโรงงานแยก); ยังไม่ผูกวันหยุดจาก Working Calendar (`G-14` จะเชื่อมเข้ามาในลำดับถัดไป); ยังไม่ผ่าน Production UAT.
