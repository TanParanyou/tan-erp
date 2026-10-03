# Inventory Verification (CP-11)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ Stock Accuracy UAT หรือ Production sign-off. กฎ: [Inventory API Contract](../03-contracts/inventory-api-contract.md).

## ผลที่รันจริง

- Backend Integration `InventoryEndpointsTests` 9/9 บน PostgreSQL: Warehouse lifecycle/numbering/idempotency/ETag/สิทธิ์/ข้าม Organization; รับเข้าจาก Goods Receipt (ledger+balance, replay, ซ้ำ → 409, PO แสดงเลขเอกสารสต็อก); คลัง inactive รับไม่ได้ และคลังที่มีสต็อกปิดไม่ได้; moving average (2 รอบรับ → 1,300/หน่วย, เบิกที่ต้นทุนเฉลี่ย, เบิกเกิน 422 ไม่เขียนอะไร, หน่วยสุดท้ายไม่เหลือเศษมูลค่า); การจอง (ลด available, เบิกของคนอื่นถูกกัน, โครงการใช้การจองของตน, ปล่อย/stale/ปล่อยซ้ำ); โอนแบบ atomic คงมูลค่า (ผลรวมสองฝั่งเท่าเดิม, ไม่พอ → ไม่เปลี่ยนทั้งสองฝั่ง); ปรับยอด (ลด/เพิ่ม/ต้องมีเหตุผล/ต้องมีต้นทุนเมื่อยอดเป็น 0/ต่ำกว่ายอดจองไม่ได้); **ยิงเบิกพร้อมกัน 6 รายการ × 3 หน่วย กับสต็อก 10 → สำเร็จ 3 รายการพอดี ที่เหลือ 422 คงเหลือ 1 และ ledger ตรง (รันซ้ำ 3 ครั้งผ่านทุกครั้ง)**; reconciliation หลังทุกเคสไม่มีส่วนต่าง. รวมชุด Procurement/Project/DocumentSequence/OpenAPI ที่เกี่ยวข้องผ่าน.
- Frontend Vitest `features/inventory` 10/10, `tsc --noEmit`, `eslint .` ผ่าน.

## ข้อจำกัด

ค่าเริ่มต้น (moving average, ห้ามติดลบ, ไม่มี lot/serial/location/Backdate/ปิดงวด, ไม่แปลงหน่วย) ยังไม่ผ่าน Warehouse/Finance และยังไม่มี Stock Accuracy/valuation UAT; ไม่มีการคืนของเข้าคลัง/ยกเลิกเอกสาร (แก้ด้วยการปรับยอด/เอกสารกลับรายการยังไม่มี), การจองหมดอายุ, Cycle count แบบหลายรอบ/อนุมัติการปรับยอด (maker–checker), รายงานมูลค่าสต็อกตามวัน, การรับของที่ไม่ผ่าน Goods Receipt (เช่นของลูกค้าฝากหรือยกยอดต้นงวดนอกจากใช้ปรับยอด); ทดสอบ concurrency ในโปรเซสเดียว (HTTP จริงแต่ DB เดียว) ยังไม่ได้ทดสอบหลายอินสแตนซ์; ไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader; Production ต้องมอบ 8 สิทธิ์ใหม่ให้ Role จริง.
