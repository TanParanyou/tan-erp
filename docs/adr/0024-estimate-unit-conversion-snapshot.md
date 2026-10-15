---
status: accepted
---

# ตรึง Unit Conversion Snapshot บน Cost Component ของ Estimate

Estimate รับต้นทุนจาก Catalog ได้เฉพาะหน่วยฐานของ Item ทั้งที่ Item Master มี Item/Shared Unit Conversion แล้ว (FR-ITEM-005). ADR นี้กำหนดวิธีใช้ conversion ใน Estimate โดย Estimate ที่อนุมัติแล้วต้องไม่เปลี่ยนเมื่อ Item Master เพิ่มรุ่น conversion ใหม่.

## ตัดสินใจ

1. ใช้ตาราง `item_unit_conversions` (เฉพาะ Item, ชี้หน่วยฐาน) ก่อน แล้ว `unit_conversions` (ใช้ร่วม, ชี้หน่วยฐานโดยตรง) ไม่สร้างตารางใหม่ ไม่รองรับหลายขั้น; ความหมาย 1 FromUnit = Factor ToUnit.
2. ตรึง `{ conversionId, scope, fromUnit, toUnit, factor }` เป็น 5 คอลัมน์แบนบน `estimate_cost_components` ตอนบันทึก draft; ไม่มี FK ไปตาราง conversion. snapshot ที่ client echo กลับด้วย `conversionId` เดิมถูกคงไว้ ไม่ค้นใหม่.
3. ต้นทุนแถว = `round(Quantity × Factor × UnitCostSnapshot, 2)` ปัดครั้งเดียว; UnitCostSnapshot/UnitSnapshot ยังเป็นต้นทุนและหน่วยฐานที่ resolve จริง; ขั้นราคาเลือกด้วย Quantity × Factor.
4. หน่วยต่างมิติถูกปฏิเสธ (`ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`) โดยตรวจมิติ ณ วันตรึงซ้ำ เพราะมิติของหน่วยแก้ได้ภายหลัง.
5. Calculation Policy Version มี `unit_conversion_mode` (`none|snapshot`); แถวเดิมเป็น `none`. Policy ที่ไม่ใช่ `snapshot` ไม่รับหน่วยอื่นนอกจากหน่วยฐาน (พฤติกรรมเดิม) และไม่คำนวณแถวที่มี snapshot. Hash ของ policy เดิมไม่เปลี่ยน.
6. ไม่ backfill Estimate เดิม: ทุกแถวเดิมใช้หน่วยฐานอยู่แล้ว.
7. Quotation/PDF ไม่เปลี่ยนเพราะอ่านระดับ Work Item; Review Queue แสดง conversion ต่อแถวต้นทุน.

## ข้อเสียที่ยอมรับ

ไม่มี API/UI สร้าง Calculation Policy จึงต้องให้ ops สร้างแถว `snapshot` เอง (follow-up); ไม่รองรับ conversion หลายขั้น; Cost tier ใช้ปริมาณหน่วยฐานที่ไม่ปัด; Item Master ยังไม่มี API ปิด/แก้ conversion.
