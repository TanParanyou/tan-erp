# MRP API Contract (ข้อตกลง API การวางแผนวัสดุ)

**สถานะ:** Implemented 2026-10-04 (CP-13) และขยาย 2026-10-30 (**Wave C G-14**: พารามิเตอร์วางแผนต่อ Item/Warehouse ได้แก่ Safety Stock, MOQ, Lot Multiple, Lead Time เฉพาะสินค้า; ปฏิทินวันหยุดทำการ Working Calendar; การแปลงหน่วยนับผ่าน Item Master Conversion; และ Reproducible Input Hash). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (`TEST_ONLY`) **รอ Production/Procurement ยืนยัน**; ยังไม่ผ่านการทดสอบกับข้อมูลจริงของผู้วางแผน.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| แหล่งความต้องการ | (1) ความต้องการที่ระบุเองต่อสินค้า/จำนวน/หน่วยนับ (ทางเลือก)/วันที่ต้องการ; (2) วัตถุดิบที่ยังไม่เบิกของ Work Order ที่ `released/in_progress` (เลือกรวมได้, ต้องการ ณ วันที่คำนวณ) — ไม่ดึง Sales Order/Project โดยอัตโนมัติ |
| ข้อมูลนำเข้า (snapshot) | อ่านครั้งเดียวใน Transaction `REPEATABLE READ`: demand (แปลงเป็นหน่วยฐานแล้ว), BOM revision ที่ approved ทุกตัว, สต็อก `available = onHand − reserved` รวมทุกคลังขององค์กร, PO ที่ค้างรับ (`approved/partially_received`, คงเหลือ = สั่ง − รับ, วันที่ = `expectedDeliveryDate` หรือ as-of + lead time ซื้อ), ผลผลิตคงเหลือของ Work Order ที่เปิด (as-of + lead time ผลิต), ความสามารถและพารามิเตอร์วางแผนของ Item (`SafetyStock`, `MinOrderQuantity`, `LotMultiple`, `LeadTimeDays`), และวันหยุดใน `WorkingCalendarHolidays` — เก็บเป็น JSONB ในรอบคำนวณพร้อม **`inputHash`** (SHA-256 ของรูปแบบมาตรฐานที่ไม่ขึ้นกับลำดับ และคงค่าเดิมทุกประการสำหรับ Snapshot ที่ไม่มีพารามิเตอร์/วันหยุดเพิ่มเติม) |
| Deterministic | แผนคำนวณจาก snapshot เท่านั้น (ไม่อ่านข้อมูลสดระหว่างคำนวณ); input เดิม → hash เดิม → ข้อเสนอแนะเดิม; สต็อกที่เปลี่ยนหลังรอบคำนวณไม่แก้รอบเดิม — ต้องสร้างรอบใหม่ |
| Multi-level & Lot Sizing (`G-14`) | แยกชั้นตาม low-level code (ลึกสุดที่เป็น component) ประมวลผลจากสินค้าสำเร็จรูปลงวัตถุดิบ; หักกันสต็อกปลอดภัย (`SafetyStock`) ก่อนนำสต็อกส่วนที่เกินไปหักความต้องการ (หากสต็อกต่ำกว่า `SafetyStock` จะเติมยอดขาดเข้าความต้องการพร้อมเหตุผล `safety_stock`); ส่วนที่ขาดสุทธิถูกปัดขึ้นตาม **`MinOrderQuantity` (MOQ)** และปัดเป็นทวีคูณของ **`LotMultiple`** (ส่วนที่สั่งเกินความต้องการสุทธิจะคงเหลือไปหักความต้องการวันถัดไป); planned order แบบ make ก่อ dependent demand ต่อวัตถุดิบ (`quantity × (1+scrap%) × จำนวนที่ผลิต ÷ output`) ณ วัน release ของมัน; BOM วน/ลึกเกิน 20 ชั้น → `MRP_BOM_CYCLE` |
| Lead time & ปฏิทินวันทำงาน (`G-14`) | ใช้ `LeadTimeDays` เฉพาะของ Item ก่อน หากไม่ระบุจึงใช้ `purchaseLeadTimeDays` หรือ `productionLeadTimeDays` ของรอบคำนวณ (0–365 วัน); การคำนวณ `orderBy` นับถอยหลังเฉพาะวันทำงานโดยข้ามวันที่อยู่ใน `WorkingCalendarHolidays` (ใช้ตารางวันหยุดร่วมกับ Capacity View ของ `G-13`) |
| หน่วย (`G-14`) | รองรับการระบุ `unitId` ใน Demand แล้วแปลงเป็นหน่วยฐานของ Item อัตโนมัติผ่าน `ItemUnitConversions` (เฉพาะสินค้า) หรือ `UnitConversions` (ส่วนกลาง); หากไม่พบคู่แปลงหน่วย → `422 MRP_UNIT_CONVERSION_NOT_FOUND` |
| การจอง | สต็อกที่จองไว้ถือว่าไม่พร้อมใช้ (หักออกจาก available) |
| ข้อเสนอแนะ | `buy` (Item ซื้อได้, ไม่มี BOM), `make` (มี BOM approved), `shortage` (ไม่มีทั้งสองทาง — ผู้วางแผนตัดสินเอง อนุมัติแปลงไม่ได้); แต่ละรายการเก็บ gross/stockUsed/scheduledReceiptsUsed และ `reasons[]` (ชนิด/อ้างอิง/จำนวน/วันที่ของความต้องการที่ทำให้เกิด) |
| Workflow | `proposed → approved → converted` หรือ `rejected`; ผู้สั่งคำนวณตัดสินข้อเสนอแนะของรอบตนเองไม่ได้ (`MRP_SELF_APPROVAL`); แปลงได้เฉพาะ approved: `make` → **Work Order ฉบับร่าง** (เลือกคลัง), `buy` → **Purchase Order ฉบับร่าง** (เลือกผู้ขาย+ราคาต่อหน่วย, วันส่งมอบ = needBy) ผ่าน store ของโมดูลนั้น (ต้องมีสิทธิ์สร้างเอกสารนั้นด้วย) แล้วใช้ workflow ปกติของ PO/WO ต่อ; การแปลงใช้ key จาก id ข้อเสนอแนะ จึง retry ได้โดยไม่ซ้ำ |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST /runs` และ `POST /calendar-holidays` ต้องมี `Idempotency-Key`; decide/convert ใช้ `If-Match` (row version ของข้อเสนอแนะ).

| Action | Method/Path | Permission |
| --- | --- | --- |
| สร้างรอบคำนวณ | `POST /api/v1/mrp/runs` `{asOfDate, purchaseLeadTimeDays, productionLeadTimeDays, includeOpenWorkOrders, demands:[{itemId, quantity, unitId?, needBy, reference?}]}` | `mrp.run` |
| อ่านรอบ/รายการ | `GET /api/v1/mrp/runs[/{id}]?search=&page=&pageSize=` | `mrp.read` |
| อนุมัติ/ปฏิเสธ | `POST /runs/{id}/recommendations/{recId}/approve|reject` | `mrp.approve` |
| แปลงเป็นเอกสาร | `POST /runs/{id}/recommendations/{recId}/convert` `{supplierId?, unitPrice?, warehouseId?}` | `mrp.convert` (+ `purchase-orders.create` หรือ `work-orders.manage`) |
| อ่าน/บันทึก/ลบ พารามิเตอร์วางแผน (`G-14`) | `GET /api/v1/mrp/planning-parameters?itemId=&warehouseId=&search=&page=&pageSize=` · `PUT /api/v1/mrp/planning-parameters` `{itemId, warehouseId?, safetyStock, minOrderQuantity, lotMultiple, leadTimeDays?}` · `DELETE /api/v1/mrp/planning-parameters/{id}` | `mrp.read` · `mrp.run` |
| อ่าน/เพิ่ม/ลบ วันหยุดปฏิทินทำงาน (`G-14`) | `GET /api/v1/mrp/calendar-holidays?branchId=&fromDate=&toDate=` · `POST /api/v1/mrp/calendar-holidays` `{holidayDate, name, branchId?}` · `DELETE /api/v1/mrp/calendar-holidays/{id}` | `mrp.read` · `mrp.run` |

เลขรอบ: `MRP-{YYYY}-{SEQ:4}` (Document Numbering `mrp-runs`).

## Errors

`409`: `MRP_INVALID_STATE`, `MRP_VERSION_CONFLICT`, `MRP_HOLIDAY_ALREADY_EXISTS`, `IDEMPOTENCY_KEY_REUSED`. `403`: `MRP_SELF_APPROVAL`. `422`: `MRP_BOM_CYCLE`, `MRP_NOT_CONVERTIBLE`, `MRP_FIELD_REQUIRED/INVALID`, `MRP_DEMAND_INVALID`, `MRP_CONVERT_INPUT_REQUIRED`, `MRP_PLANNING_PARAMETER_INVALID`, `MRP_HOLIDAY_INVALID`, `MRP_UNIT_CONVERSION_NOT_FOUND`; และจากการสร้างเอกสารปลายทาง เช่น `PRODUCTION_BOM_NOT_APPROVED`, `PURCHASE_ORDER_ITEM_NOT_PURCHASABLE`, `INVENTORY_WAREHOUSE_INACTIVE`. `404` นอก Organization.

## Data (migration `AddMrp`, schema `mrp`)

`runs` (unique `(org, number)`, `snapshot` jsonb, `input_hash`; ไม่มี endpoint แก้/ลบ), `recommendations` (checks action/status/quantity, `status='converted' ⇔ converted_id IS NOT NULL`, `row_version`). Audit: `mrp.run.created`, `mrp.recommendation.approved|rejected|converted`.
