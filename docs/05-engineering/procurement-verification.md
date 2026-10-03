# Procurement Verification (CP-10)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT/Production sign-off. กฎ: [Procurement API Contract](../03-contracts/procurement-api-contract.md).

## ผลที่รันจริง

- Backend Integration `ProcurementEndpointsTests` 6/6 บน PostgreSQL: Supplier lifecycle + idempotency/ETag/สิทธิ์/ข้าม Organization; PO validation (บรรทัดซ้ำ/จำนวน 0/Item ไม่พบ), แก้ฉบับร่าง, stale version, แก้หลังส่งไม่ได้, maker–checker (ผู้สร้างอนุมัติ → 403), ปฏิเสธต้องมี note, ยกเลิก; Supplier ปิดใช้งานแล้วสร้าง/อนุมัติไม่ได้; รับสินค้าบางส่วน/replay/Key reuse, รับเกินถูกปฏิเสธโดยไม่เขียนบางส่วน, ยกเลิกหลังรับไม่ได้, รับครบ → `received`; Project budget: Project ยัง `planned` อนุมัติไม่ได้, เกินงบ 422, ภาระผูกพัน/งบคงเหลือใน Project Control, ยกเลิกปล่อยภาระผูกพัน. รวม `ProjectEndpoints/ProjectControl`, `DocumentSequence*`, OpenAPI contract ผ่านทั้งหมด.
- Frontend Vitest `features/procurement` 13/13 และทั้งชุด 703/704 (1 รายการ `customer-editor` หมดเวลาตอนรันขนานทั้งชุด แต่ผ่าน 12/12 เมื่อรันเดี่ยวและบน baseline), `tsc --noEmit`, `eslint .` ผ่าน.

## ข้อจำกัด

ค่าเริ่มต้น (อนุมัติทุกใบ, ไม่มี tolerance รับเกิน, ภาระผูกพันตามยอดรวมใบ, ราคากรอกเอง) ยังไม่ผ่าน Procurement/Finance; ไม่มี Purchase Request, การคืน/ยกเลิกใบรับ, Supplier price list/lead time, การแปลงหน่วย, วงเงินอนุมัติตามระดับ, ใบเสร็จ/Invoice ผู้ขาย (CP-15), Barcode scan; ภาระผูกพันเทียบงบรวมของโครงการ (ยังไม่แยกตามหมวดงบ); concurrency ทดสอบที่ระดับ version conflict ยังไม่ได้ยิงขนานจริง; ไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader; Production ต้องมอบ 6 สิทธิ์ใหม่ให้ Role จริง.
