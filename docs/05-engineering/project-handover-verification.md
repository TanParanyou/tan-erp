# Project Handover Verification (CP-08)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT/Production sign-off.

## ขอบเขต

Won Opportunity + Quotation `accepted` → Project `planned` หนึ่งต่อหนึ่ง พร้อม Baseline ที่ตรึง; รายการ/รายละเอียด Project; ส่วน "ส่งต่อเป็นโครงการ" ในหน้า Opportunity ที่ Won. ดู [Project API Contract](../03-contracts/project-api-contract.md).

## ผลที่รันจริง

- Backend Integration `ProjectEndpointsTests` 7/7 บน PostgreSQL: สร้างสำเร็จพร้อม Baseline/Audit/ETag, replay Key เดิม, Key ต่าง payload → 409, ซ้ำคนละ Key → `PROJECT_ALREADY_EXISTS` (เหลือ 1 แถว), stale version, Quotation ไม่ accepted/Opportunity ไม่ Won (ไม่สร้างอะไร), Owner ไม่มี Membership → 404, ไม่มีสิทธิ์ 403 + ข้าม Organization, list/search/status/422, handover-source. รวม `DocumentSequence*` 20/20 (เพิ่มประเภท `projects`) และ OpenAPI contract.
- Frontend Vitest `features/projects` 8/8 และ `features/opportunities` ผ่าน; `tsc --noEmit`, `eslint .` ผ่าน.

## ข้อจำกัด

ค่าเริ่มต้น (1 ต่อ 1, Owner ต้องระบุ, Branch ตาม Quotation, ไม่มี auto Procurement/Production) ยังไม่ผ่าน Sales/Project Owner; ไม่มีการแก้/ยกเลิก Project, การแบ่งงานหลาย Project, หรือเปลี่ยนสถานะหลัง `planned` (CP-09); "baseline ที่คงเดิมเมื่อมีฉบับใหม่" ทดสอบที่ระดับ copy ใน Project (ยังไม่มี Quotation Amendment — CP-06); ยังไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader ของ modal และหน้าใหม่; Production ต้องมอบ `projects.read`/`projects.create` ให้ Role จริง.

## Project Control (CP-09, 2026-10-04)

ขอบเขตและกฎ: [Project API Contract](../03-contracts/project-api-contract.md#project-control-cp-09-implemented-2026-10-04).

**ผลที่รันจริง (targeted):** Backend Integration `ProjectControlEndpointsTests` 5/5 + `ProjectEndpointsTests` 7/7 บน PostgreSQL (activation ต้องมีแผน+งบแล้วตรึง Baseline, stale version, lifecycle/เหตุผล/ประวัติ, Progress ถ่วงน้ำหนัก + gate ready_for_handover, Change Order maker–checker/idempotent replay/ผู้ไม่ใช่ผู้สร้างอนุมัติแล้วปรับงบและมูลค่าปัจจุบันโดยไม่แก้ Baseline/over-budget 422, สถานะ planned สร้าง CO ไม่ได้, สิทธิ์/ข้าม Organization). Frontend Vitest `features/projects` 24/24, `tsc --noEmit`, `eslint .` ผ่าน.

**ข้อจำกัด:** กฎ (lifecycle, เงื่อนไขเริ่มงาน, หมวดงบ, น้ำหนัก Milestone, maker–checker, over-budget) เป็นค่าที่ทีมพัฒนาเลือก ยังไม่ผ่าน Project Manager/Finance; ไม่มี Actual Cost/Commitment, WBS/dependency/calendar, การแก้ Milestone ใน UI (มี API), การยกเลิก Change Order หลังตัดสิน, concurrency test แบบขนานจริง (ทดสอบที่ระดับ version conflict); ยังไม่ได้รัน full Integration suite, `next build`, Playwright; ยังไม่ตรวจ 200% zoom/keyboard/screen-reader; Production ต้องมอบ 4 สิทธิ์ใหม่ให้ Role จริง.

