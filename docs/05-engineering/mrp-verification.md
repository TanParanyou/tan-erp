# MRP Verification (CP-13)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่การยืนยันกับผู้วางแผนจริง. กฎ: [MRP API Contract](../03-contracts/mrp-api-contract.md).

## ผลที่รันจริง

- Unit `MrpEngineTests` 8/8: แตก BOM หลายชั้นพร้อมของเสียและ lead time (ตู้ 10 → แผง 20 → ไม้อัด 11 ฯลฯ ทั้งวันที่ needBy/orderBy), net สต็อกก่อนของที่กำลังมา, ไม่นับของที่มาช้ากว่าวันที่ต้องการ, ไม่มีช่องทางจัดหา → `shortage`, ผลและ hash คงที่ไม่ขึ้นกับลำดับ input และเปลี่ยนเมื่อ input เปลี่ยน, net หลายวันที่ต้องการเทียบสต็อกร่วม, BOM วนถูกปฏิเสธ, maker–checker/สถานะของข้อเสนอแนะ.
- Backend Integration `MrpEndpointsTests` 4/4 บน PostgreSQL: รอบคำนวณจาก BOM จริง (make/buy, level, ลำดับวันที่, เหตุผลย้อนกลับ), replay/ชน Key, **สต็อกเข้าหลังรอบคำนวณไม่เปลี่ยนรอบเดิม** แต่รอบใหม่เปลี่ยน และรอบที่ input เหมือนกันให้ hash/ผลเหมือนกัน; นับ PO ค้างรับ/Work Order ที่เปิด (ทั้งความต้องการวัตถุดิบและผลผลิตเป็น supply) และ validation; อนุมัติโดยผู้อื่น, stale version, แปลง make → Work Order draft และ buy → PO draft (จำนวน/วันส่งมอบ/หมายเหตุอ้างอิงรอบ), reject แล้วแปลงไม่ได้; สิทธิ์และข้าม Organization. รวมชุด OpenAPI/DocumentSequence ผ่าน.
- Frontend Vitest `features/mrp` (สถานะ/ข้อความ th-en ครบ, อนุมัติเฉพาะ buy/make ไม่ใช่ shortage, แสดงเหตุผล, ส่ง row version, แสดง error ที่แปลแล้ว, ตรวจ supplier/ราคาก่อนแปลง, ซ่อนปุ่มตามสิทธิ์), `tsc --noEmit`, `eslint .` ผ่าน.

## ข้อจำกัด

ค่าเริ่มต้นทั้งหมดรอ Production/Procurement ยืนยัน: ไม่มี safety stock, lot sizing (lot-for-lot เท่านั้น), MOQ, lead time ต่อ Item/ผู้ขาย, ปฏิทินวันหยุด, การแปลงหน่วย, ความต้องการจาก Sales Order/Project อัตโนมัติ, หลายคลัง/จัดสรรต่อคลัง (รวมทั้งองค์กร), การทดสอบเปลี่ยนสต็อกพร้อมกันระหว่างคำนวณ (อาศัย REPEATABLE READ แต่ยังไม่ได้ยิงขนาน); การแปลงเอกสารไม่ atomic ข้ามโมดูล (สร้างเอกสารแล้วจึงบันทึกลิงก์; retry ใช้ key เดิมจึงได้เอกสารเดิม); ไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader; ต้องมอบ 4 สิทธิ์ใหม่ (`mrp.*`) ให้ Role จริง และ Production ต้องยืนยันกับเคสจริงก่อนใช้ตัดสินใจซื้อ.
