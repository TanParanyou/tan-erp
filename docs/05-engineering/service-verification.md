# Installation, Handover, Warranty and Service Verification (CP-14 + G-18)

**สถานะ:** Implemented 2026-10-04 (`CP-14`) และขยายครบวงจร 2026-10-30 (`G-18`); หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT/Production sign-off. กฎ: [Service API Contract](../03-contracts/service-api-contract.md) และ [แผน G-18](../superpowers/plans/2026-10-30-g18-service-completion.md).

## ผลที่รันจริง

- Unit `ServiceDomainTests` 14/14 (รวมใน `TanErp.UnitTests` 708/708) และ Architecture 4/4:
  - `CP-14`: วันสิ้นสุดประกันวันก่อนครบรอบ/สิ้นเดือนสั้น (15 ม.ค.+1 → 14 ก.พ., 31 ม.ค.+1 → 28 ก.พ., 29 ก.พ. 2567 +12 → 28 ก.พ. 2568), ขอบเขต 1–120 เดือน, ผู้แก้ตรวจยืนยันเองไม่ได้และการเปิดซ้ำกลับมาบล็อกพร้อมส่งมอบ, ส่งมอบต้องพร้อม/ระบุประกัน/ไม่ใช่อนาคต, flow งานบริการและ event ต่อขั้น.
  - `G-18`: การหยุดพักเวลารอลูกค้า (`WaitCustomer` → `waiting_customer` และ `ResumeFromWaitCustomer` สะสม `PausedSeconds`), การคำนวณ SLA ต่อระดับความสำคัญ (`ServiceSlaCalculator` หักวันหยุด `WorkingCalendarHoliday` ของ `G-14` และหักเวลาหยุดพักรอลูกค้า พร้อมสถานะ `on_track`, `warning` >= 80%, `breached`, `paused`, `met`), การคิดค่าบริการงานนอกประกัน (`RecordServiceCharge` ปฏิเสธงานในประกันด้วย `SERVICE_CHARGE_IN_WARRANTY` และป้องกันการออกบิลซ้ำ), และการตรวจสอบความถูกต้องของ `TechnicianAssignment`.
- Backend Integration `ServiceEndpointsTests` 4/4 + `AttachmentOwnerScopeReaderParityTests` 1/1 บน PostgreSQL:
  - `CP-14` (3 tests): งานติดตั้งครบวงจร, ยกเลิก/ข้อมูลผิด, และงานบริการและการตรวจประกัน.
  - `G-18` (`ServiceCompletionG18_SlaPauseOnWaitingCustomer_ServiceChargeCreatesBillingOnlyOutOfWarranty_AndTechnicianConflictCheck`):
    1. งานบริการในประกัน (`InWarranty == true`) เรียก `POST /api/v1/service-requests/{id}/charge` → คืน `409 SERVICE_CHARGE_IN_WARRANTY` ไม่สร้างค่าบริการ
    2. งานบริการนอกประกัน (`InWarranty == false`): เปลี่ยนสถานะ `wait-customer` → `Status == "waiting_customer"` และ `Sla.Status == "paused"`, กด `resume` → กลับเป็น `in_progress`, บันทึกค่าบริการ `POST /api/v1/service-requests/{id}/charge` → สร้าง `BillingDocument` (`BIL-...`) และ `AccountingOutboxMessage` (`billing.issued`) ใน Transaction เดียวกัน พร้อมปฏิเสธการคิดเงินซ้ำ (`409 SERVICE_INVALID_STATE`)
    3. ตารางมอบหมายงานช่าง (`POST /api/v1/service-requests/technician-schedules`): มอบหมายช่างคนเดียวกันในวันซ้ำโดย `allowConflict = false` → คืน `409 TECHNICIAN_SCHEDULE_CONFLICT`; เมื่อส่ง `allowConflict = true` → บันทึกสำเร็จพร้อมคืน `hasConflict = true`
- Frontend Verification (`features/service`, `lib/attachments`, `lib/notifications`):
  - `npx tsc --noEmit` (0 errors), `npm run lint` (0 errors), `npm run build` (89/89 routes).
  - Vitest 39/39 passed (`service-components.test.tsx` 22/22, `attachment-owner-types.test.ts` 6/6, `notification-types.test.ts` 7/7, `notification-view.test.tsx` 4/4) ครอบคลุมการ์ด SLA, ปุ่ม `wait-customer` / `resume` / `charge`, ลิงก์ใบแจ้งหนี้, `<AttachmentList ownerType="service-request" />`, `<SignatureCapturePanel ownerType="service-request" />`, และ `<TechnicianSchedulePanel />`.

## ข้อจำกัด

ค่าเริ่มต้น `TEST_ONLY` รอฝ่ายบริการลูกค้าและงานติดตั้งยืนยัน ([Open Validation Questions](../01-business/open-validation-questions.md)): เกณฑ์ชั่วโมง SLA ต่อระดับความสำคัญ (`VQ-SRV-03`), นโยบายการอนุมัติค่าบริการนอกประกันก่อนออกบิล (`VQ-SRV-04`), และกฎการมอบหมายตารางช่างรายวันเทียบรายชั่วโมง (`VQ-SRV-05`); ต้องมอบสิทธิ์ 7 รายการ (`installations.*`, `warranties.read`, `service-requests.*`) ให้ Role จริงก่อนเปิดใช้ใน Production.
