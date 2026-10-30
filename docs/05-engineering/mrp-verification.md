# MRP Verification (CP-13 + G-14)

**สถานะ:** Implemented 2026-10-04 (`CP-13`) และขยายครบวงจร 2026-10-30 (`G-14`); หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่การยืนยันกับผู้วางแผนจริง. กฎ: [MRP API Contract](../03-contracts/mrp-api-contract.md) และ [แผน G-14](../superpowers/plans/2026-10-30-g14-mrp-completion.md).

## ผลที่รันจริง

- Unit `MrpEngineTests` 12/12:
  - `CP-13` (8 tests): แตก BOM หลายชั้นพร้อมของเสียและ lead time (ตู้ 10 → แผง 20 → ไม้อัด 11 ฯลฯ ทั้งวันที่ needBy/orderBy), net สต็อกก่อนของที่กำลังมา, ไม่นับของที่มาช้ากว่าวันที่ต้องการ, ไม่มีช่องทางจัดหา → `shortage`, ผลและ hash คงที่ไม่ขึ้นกับลำดับ input และเปลี่ยนเมื่อ input เปลี่ยน, net หลายวันที่ต้องการเทียบสต็อกร่วม, BOM วนถูกปฏิเสธ, maker–checker/สถานะของข้อเสนอแนะ.
  - `G-14` (4 tests):
    1. `SafetyStock`: เติมยอดให้ถึง `SafetyStock` ณ วันเริ่มต้น (`horizonStart`) พร้อมระบุ `source = "safety_stock"` และรักษาระดับ `projectedAvailable >= SafetyStock` ตลอดช่วงวางแผน.
    2. `MinOrderQuantity` + `LotMultiple`: ปัดยอดสั่งซื้อ/สั่งผลิตเป็น `max(NetShortage, MinOrderQuantity)` และปัดขึ้นเป็นทวีคูณของ `LotMultiple` พร้อมยกยอดส่วนเกิน (`excess = OrderQty - NetShortage`) ไปหักความต้องการใน Bucket ถัดไป.
    3. `LeadTimeDays` + `WorkingCalendarHolidays`: ใช้ `LeadTimeDays` ของพารามิเตอร์วางแผนก่อนค่าเริ่มต้นของ BOM/Supplier และถอยวันสั่ง (`OrderBy`) ข้ามวันหยุดในปฏิทินวันทำงาน.
    4. `HashOf`: รักษาค่า `InputHash` แบบเดิมทุกตัวอักษรเมื่อไม่มีพารามิเตอร์วางแผนและไม่มีวันหยุด (Backward Reproducibility) และเปลี่ยนค่า Hash ทันทีเมื่อตั้งค่าพารามิเตอร์หรือวันหยุด.
- Backend Integration `MrpEndpointsTests` 6/6 (+ `ProductionEndpointsTests` 4/4) บน PostgreSQL:
  - `CP-13` (4 tests): รอบคำนวณจาก BOM จริง, replay/ชน Key, สต็อกเข้าหลังรอบคำนวณไม่เปลี่ยนรอบเดิม, นับ PO ค้างรับ/Work Order ที่เปิด, อนุมัติโดยผู้อื่น, แปลง make → Work Order draft และ buy → PO draft, สิทธิ์และข้าม Organization.
  - `G-14` (2 tests):
    1. ตั้งค่า Planning Parameters (`SafetyStock`, `MinOrderQuantity`, `LotMultiple`, `LeadTimeDays`) แยกคลัง/ค่าเริ่มต้นองค์กร, เพิ่มวันหยุด `WorkingCalendarHolidays`, ส่ง Demand ด้วยหน่วยทางเลือก (`UnitId` แปลงเป็น `BaseUnitId` อัตโนมัติ และคืน `422 MRP_UNIT_CONVERSION_NOT_FOUND` เมื่อไม่มีตัวคูณ), คำนวณ MRP พร้อมตรวจปริมาณปัดขึ้น/วันสั่งข้ามวันหยุด/เหตุผล `safety_stock`, และตรวจว่า `GET /api/v1/work-centers/capacity` (`G-13`) แสดง `CapacityHours = 0` ในวันหยุด.
    2. ตรวจการแยกข้อมูลข้าม Organization (`404`) และการบังคับสิทธิ์ `mrp.run` (`403`) บน `/api/v1/mrp/planning-parameters` และ `/api/v1/mrp/calendar-holidays`.
- Frontend Verification (`features/mrp`):
  - `npx tsc --noEmit` (0 errors), `npm run lint` (0 errors), `npm run build` (89/89 routes).
  - Vitest `src/features/mrp/components/mrp-components.test.tsx` 11/11 passed (ครอบคลุม `MrpSettingsDrawer` แท็บพารามิเตอร์วางแผนและวันหยุด, การเลือกหน่วยนับต่อบรรทัด Demand ใน `MrpRunEditor`, ป้ายเหตุผล `safety_stock`, และการแปลงข้อเสนอแนะเป็น PO/WO).

## ข้อจำกัด

ค่าเริ่มต้น `TEST_ONLY` รอ Production/Procurement ยืนยัน ([Open Validation Questions หมวด 12](../01-business/open-validation-questions.md)): ลำดับความสำคัญของพารามิเตอร์เฉพาะคลังเทียบค่าเริ่มต้นองค์กร (`VQ-MRP-01`), การใช้เฉพาะรายการวันหยุดที่บันทึกไว้โดยยังไม่บังคับเสาร์-อาทิตย์อัตโนมัติ (`VQ-MRP-02`), และการปัดยอดสั่งตาม MOQ/Lot Multiple พร้อมยกยอดส่วนเกินไป Bucket ถัดไป (`VQ-MRP-03`); ยังไม่ดึงความต้องการจาก Sales Order/Project อัตโนมัติ; การแปลงเอกสารไม่ atomic ข้ามโมดูล (สร้างเอกสารแล้วจึงบันทึกลิงก์; retry ใช้ key เดิมจึงได้เอกสารเดิม); ต้องมอบ 4 สิทธิ์ (`mrp.*`) ให้ Role จริง และ Production ต้องยืนยันกับเคสจริงก่อนใช้ตัดสินใจซื้อ.
