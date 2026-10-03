# Production Verification (CP-12)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ Production UAT หรือ sign-off. กฎ: [Production API Contract](../03-contracts/production-api-contract.md).

## ผลที่รันจริง

- Backend Integration `ProductionEndpointsTests` 3/3 บน PostgreSQL: **BOM** (Item ที่ผลิตไม่ได้ถูกปฏิเสธ, วัตถุดิบซ้ำ, วนตัวเอง, สร้างซ้ำ/replay Key, ผู้จัดทำอนุมัติเองไม่ได้ + stale `If-Match`, แก้ draft, อนุมัติแล้วแก้ไม่ได้, revision ใหม่ทำฉบับเดิมเป็น obsolete, **cycle ทางอ้อม FG-PARENT↔FG-SUB**, ค้นหา/สิทธิ์); **Work Order เต็มวงจร** (ต้องมี BOM approved, จำนวนวัตถุดิบ = 8 / 4.4 จากสูตร + ของเสีย 10%, เบิกก่อน release ไม่ได้, เบิกเกินไม่ได้, replay เบิก ไม่เบิกซ้ำ, รับผลผลิตบางส่วน → ต้นทุน 5,100 และ stock FG เฉลี่ย 2,550, คืนเกินส่วนที่ยังไม่ถูกใช้ไม่ได้, รับผลผลิตตอนวัตถุดิบไม่พอ → shortage, รับเกินแผน → over-completed, รับครบ → completed และมูลค่า FG = เบิกสุทธิ, ยกเลิกหลังเสร็จไม่ได้); **ยกเลิก/สต็อกไม่พอ/สิทธิ์** (เบิกเกินสต็อก → 422 และใบสั่ง+คลังไม่เปลี่ยน, ยกเลิกต้องคืนวัตถุดิบหมดก่อนและต้องมีเหตุผล, กรองสถานะ, 403 ไม่มีสิทธิ์). `reconciliation` ไม่มีส่วนต่างหลังทุกเคสที่แตะสต็อก. รวมชุด Inventory, OpenAPI, DocumentSequence ผ่าน (29/29) และ Unit 323/323.
- Frontend Vitest `features/production` + inventory + procurement ผ่าน (ปุ่ม approve/แก้ draft ตามสิทธิ์, เบิกส่งเฉพาะรายการที่กรอกพร้อม Idempotency-Key, แสดงข้อความ error แปลแทนข้อความดิบ, ไม่ยิง API เมื่อไม่กรอก, ฟอร์มรับผลผลิตเฉพาะ in_progress), `tsc --noEmit`, `eslint .` ผ่าน; ข้อความ th/en ครบ.

## ข้อจำกัด

ค่าเริ่มต้นทั้งหมดรอ Engineering/Production ยืนยัน (หนึ่ง BOM ต่อ Item, ของเสียเป็น % ต่อวัตถุดิบ, ต้นทุน = วัตถุดิบล้วน ไม่มีแรงงาน/โสหุ้ย/routing/capacity, ไม่มี subcontracting/by-product/ทางเลือกวัตถุดิบ, รับผลผลิตเข้าคลังเดียวกับที่เบิก, ไม่มีการจองวัตถุดิบล่วงหน้าตอน release (เบิกแล้วจึงตัดสต็อก)); ไม่มีกลับรายการผลผลิตที่รับแล้ว (ใช้ปรับยอด); ไม่ได้ทดสอบยิงเบิกพร้อมกันบน Work Order เดียวโดยเฉพาะ (พึ่งการล็อกแถวและการทดสอบ concurrency ของ Inventory); ไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader; Production ต้องมอบ 6 สิทธิ์ใหม่ (`boms.*`, `work-orders.*`) ให้ Role จริง.
