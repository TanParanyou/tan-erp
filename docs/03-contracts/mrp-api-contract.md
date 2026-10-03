# MRP API Contract (ข้อตกลง API การวางแผนวัสดุ)

**สถานะ:** Implemented 2026-10-04 (CP-13). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Production/Procurement ยืนยัน**; ยังไม่ผ่านการทดสอบกับข้อมูลจริงของผู้วางแผน.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| แหล่งความต้องการ | (1) ความต้องการที่ระบุเองต่อสินค้า/จำนวน/วันที่ต้องการ; (2) วัตถุดิบที่ยังไม่เบิกของ Work Order ที่ `released/in_progress` (เลือกรวมได้, ต้องการ ณ วันที่คำนวณ) — ไม่ดึง Sales Order/Project โดยอัตโนมัติ |
| ข้อมูลนำเข้า (snapshot) | อ่านครั้งเดียวใน Transaction `REPEATABLE READ`: demand, BOM revision ที่ approved ทุกตัว, สต็อก `available = onHand − reserved` รวมทุกคลังขององค์กร, PO ที่ค้างรับ (`approved/partially_received`, คงเหลือ = สั่ง − รับ, วันที่ = `expectedDeliveryDate` หรือ as-of + lead time ซื้อ), ผลผลิตคงเหลือของ Work Order ที่เปิด (as-of + lead time ผลิต), ความสามารถของ Item — เก็บเป็น JSONB ในรอบคำนวณพร้อม **`inputHash`** (SHA-256 ของรูปแบบมาตรฐานที่ไม่ขึ้นกับลำดับ) |
| Deterministic | แผนคำนวณจาก snapshot เท่านั้น (ไม่อ่านข้อมูลสดระหว่างคำนวณ); input เดิม → hash เดิม → ข้อเสนอแนะเดิม; สต็อกที่เปลี่ยนหลังรอบคำนวณไม่แก้รอบเดิม — ต้องสร้างรอบใหม่ |
| Multi-level | แยกชั้นตาม low-level code (ลึกสุดที่เป็น component) ประมวลผลจากสินค้าสำเร็จรูปลงวัตถุดิบ; แต่ละสินค้า net ตามวันที่ต้องการ (ก่อนหลัง) กับสต็อกก่อน แล้วของที่กำลังมาที่ถึงไม่เกินวันที่ต้องการ; ส่วนที่ขาดเป็น planned order แบบ **lot-for-lot** (ไม่มี lot size/safety stock/MOQ); planned order แบบ make ก่อ dependent demand ต่อวัตถุดิบ (`quantity × (1+scrap%) × จำนวนที่ผลิต ÷ output`) ณ วัน release ของมัน; BOM วน/ลึกเกิน 20 ชั้น → `MRP_BOM_CYCLE` |
| Lead time/ปฏิทิน | lead time ซื้อและผลิตเป็นพารามิเตอร์ต่อรอบ (วันปฏิทิน 0–365, ไม่มีปฏิทินวันหยุด/lead time ต่อ Item หรือผู้ขาย); `orderBy = needBy − lead time` (อาจย้อนก่อน as-of — แสดงตามจริงให้ผู้วางแผนเห็นว่าสายไปแล้ว) |
| หน่วย | หน่วยฐานของ Item เท่านั้น (ไม่แปลงหน่วย) |
| การจอง | สต็อกที่จองไว้ถือว่าไม่พร้อมใช้ (หักออกจาก available) |
| ข้อเสนอแนะ | `buy` (Item ซื้อได้, ไม่มี BOM), `make` (มี BOM approved), `shortage` (ไม่มีทั้งสองทาง — ผู้วางแผนตัดสินเอง อนุมัติแปลงไม่ได้); แต่ละรายการเก็บ gross/stockUsed/scheduledReceiptsUsed และ `reasons[]` (ชนิด/อ้างอิง/จำนวน/วันที่ของความต้องการที่ทำให้เกิด) |
| Workflow | `proposed → approved → converted` หรือ `rejected`; ผู้สั่งคำนวณตัดสินข้อเสนอแนะของรอบตนเองไม่ได้ (`MRP_SELF_APPROVAL`); แปลงได้เฉพาะ approved: `make` → **Work Order ฉบับร่าง** (เลือกคลัง), `buy` → **Purchase Order ฉบับร่าง** (เลือกผู้ขาย+ราคาต่อหน่วย, วันส่งมอบ = needBy) ผ่าน store ของโมดูลนั้น (ต้องมีสิทธิ์สร้างเอกสารนั้นด้วย) แล้วใช้ workflow ปกติของ PO/WO ต่อ; การแปลงใช้ key จาก id ข้อเสนอแนะ จึง retry ได้โดยไม่ซ้ำ |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST /runs` ต้องมี `Idempotency-Key`; decide/convert ใช้ `If-Match` (row version ของข้อเสนอแนะ).

| Action | Method/Path | Permission |
| --- | --- | --- |
| สร้างรอบคำนวณ | `POST /api/v1/mrp/runs` `{asOfDate, purchaseLeadTimeDays, productionLeadTimeDays, includeOpenWorkOrders, demands:[{itemId, quantity, needBy, reference?}]}` | `mrp.run` |
| อ่านรอบ/รายการ | `GET /api/v1/mrp/runs[/{id}]?search=&page=&pageSize=` | `mrp.read` |
| อนุมัติ/ปฏิเสธ | `POST /runs/{id}/recommendations/{recId}/approve|reject` | `mrp.approve` |
| แปลงเป็นเอกสาร | `POST /runs/{id}/recommendations/{recId}/convert` `{supplierId?, unitPrice?, warehouseId?}` | `mrp.convert` (+ `purchase-orders.create` หรือ `work-orders.manage`) |

เลขรอบ: `MRP-{YYYY}-{SEQ:4}` (Document Numbering `mrp-runs`).

## Errors

`409`: `MRP_INVALID_STATE`, `MRP_VERSION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`. `403`: `MRP_SELF_APPROVAL`. `422`: `MRP_BOM_CYCLE`, `MRP_NOT_CONVERTIBLE`, `MRP_FIELD_REQUIRED/INVALID`, `MRP_DEMAND_INVALID`, `MRP_CONVERT_INPUT_REQUIRED`; และจากการสร้างเอกสารปลายทาง เช่น `PRODUCTION_BOM_NOT_APPROVED`, `PURCHASE_ORDER_ITEM_NOT_PURCHASABLE`, `INVENTORY_WAREHOUSE_INACTIVE`. `404` นอก Organization.

## Data (migration `AddMrp`, schema `mrp`)

`runs` (unique `(org, number)`, `snapshot` jsonb, `input_hash`; ไม่มี endpoint แก้/ลบ), `recommendations` (checks action/status/quantity, `status='converted' ⇔ converted_id IS NOT NULL`, `row_version`). Audit: `mrp.run.created`, `mrp.recommendation.approved|rejected|converted`.
