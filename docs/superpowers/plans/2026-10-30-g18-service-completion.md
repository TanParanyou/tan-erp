# Service Completion (G-18) — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `.agents/skills/tiered-orchestration/SKILL.md`, `.agents/skills/building-erp-apis/SKILL.md`, and `.agents/skills/building-erp-forms/SKILL.md`.

**Goal:** เติมเต็มโมดูลงานบริการหลังการขายและงานติดตั้ง (`G-18`) ให้รองรับการแนบไฟล์และลายเซ็นลูกค้าตอนปิดงานบริการ (`G-01` owner `service-request`), นโยบาย SLA ต่อระดับความสำคัญ (`responseHours` / `resolveHours`) ที่นับเวลาตามปฏิทินวันทำงาน (`G-14`) และหยุดนับขณะรอลูกค้า (`waiting_customer`) พร้อมแจ้งเตือนเมื่อใกล้เกิน/เกิน SLA (`G-02`), การคิดค่าบริการงานนอกประกัน (`!InWarranty`) ส่งเข้าสร้าง `BillingDocument` (`CP-15`/`G-15`), และตารางมอบหมายงานช่างต่อวันพร้อมระบบตรวจชน (`TECHNICIAN_SCHEDULE_CONFLICT` / `allowConflict`).

**สถานะ:** Implemented 2026-10-30 (`TEST_ONLY` defaults — รอฝ่ายบริการลูกค้ายืนยันใน `docs/01-business/open-validation-questions.md`).

---

## 1. ขอบเขตงานและกฎธุรกิจ (`TEST_ONLY` Defaults)

1. **Shared Attachment & Customer Signature สำหรับงานบริการ (`G-01`):**
   - ลงทะเบียน `AttachmentOwnerTypes.ServiceRequest = "service-request"`:
     - `ReadPermission`: `"service-requests.read"`
     - `ManagePermission`: `"service-requests.manage"` (แก้ไขไฟล์แนบได้ในสถานะ `open`, `scheduled`, `in_progress`, `waiting_customer`, `resolved`)
     - `SignPermission`: `"service-requests.manage"` (บันทึกลายเซ็นลูกค้า `purpose = "handover"` ได้ในสถานะ `resolved` ก่อนกด `close`)
2. **SLA Policy ต่อ Priority + Working Calendar (`G-14`) + Pause เมื่อรอลูกค้า (`waiting_customer`) + Notification (`G-02`):**
   - เพิ่มสถานะ `ServiceRequestStatus.WaitingCustomer = "waiting_customer"`:
     - `POST /api/v1/service-requests/{id}/wait-customer { reason }`: เปลี่ยนจาก `open`, `scheduled`, หรือ `in_progress` ไปเป็น `waiting_customer` และบันทึก `PausedAtUtc`
     - `POST /api/v1/service-requests/{id}/resume`: เปลี่ยนจาก `waiting_customer` กลับเป็น `in_progress` พร้อมสะสม `PausedSeconds`
   - ตารางค่าเริ่มต้น `TEST_ONLY` ของ `ServiceSlaCalculator`:
     - `urgent`: `ResponseHours = 2`, `ResolveHours = 8`
     - `high`: `ResponseHours = 4`, `ResolveHours = 24`
     - `normal`: `ResponseHours = 8`, `ResolveHours = 48`
     - `low`: `ResponseHours = 24`, `ResolveHours = 72`
   - การคำนวณ `ElapsedHours`: หักวันหยุดใน `mrp.working_calendar_holidays` (`G-14`) และหัก `PausedSeconds` (รวมถึงช่วงเวลาที่กำลังอยู่ใน `waiting_customer`)
   - สถานะ SLA (`SlaStatus`): `"paused"`, `"met"`, `"on_track"`, `"warning"` (ใช้เวลา >= 80% ของ `ResolveHours`), `"breached"` (เกิน `ResolveHours`)
   - การแจ้งเตือน `NotificationTypes.ServiceRequestSlaWarning = "service-request.sla-warning"` (`G-02`) ส่งหาผู้ถือสิทธิ์ `service-requests.manage` เมื่อ SLA เข้าสู่สถานะ `warning` หรือ `breached`
3. **Service Charge สำหรับงานนอกประกัน (`!InWarranty` → `BillingDocument`):**
   - `POST /api/v1/service-requests/{id}/charge { amount, note }`:
     - หาก `InWarranty == true` → ปฏิเสธด้วย `409 SERVICE_CHARGE_IN_WARRANTY`
     - หาก `amount <= 0` → ปฏิเสธด้วย `422 SERVICE_FIELD_INVALID`
     - หากเคยตั้งหนี้ไปแล้ว (`BillingDocumentId != null`) → ปฏิเสธด้วย `409 SERVICE_INVALID_STATE`
     - เมื่อสำเร็จ: สร้าง `BillingDocument` (`Kind = "other"`, รันเลข `BIL-{YYYY}-{SEQ:4}`) พร้อมส่ง `billing.issued` เข้า `AccountingOutbox` ใน Transaction เดียวกัน และผูก `BillingDocumentId` / `BillingDocumentNumber` บน `ServiceRequest`
4. **ตารางมอบหมายงานช่าง (`TechnicianAssignment`) และการตรวจชน:**
   - ตาราง `service.technician_assignments`: ผูก `TechnicianUserId`, `WorkDate`, `AssignmentKind` (`"installation"` หรือ `"service_request"`), `InstallationJobId?`, `ServiceRequestId?`, `Note?`
   - `POST /api/v1/service-requests/technician-schedules`:
     - หากช่างคนเดียวกันมีงานอื่นใน `WorkDate` เดียวกันแล้ว และ `allowConflict == false` → ปฏิเสธด้วย `409 TECHNICIAN_SCHEDULE_CONFLICT`
     - หาก `allowConflict == true` → บันทึกสำเร็จและคืน `hasConflict = true` เพื่อให้ UI แสดงป้ายเตือนงานชน
   - `GET /api/v1/service-requests/technician-schedules?fromDate=&toDate=&technicianUserId=`
   - `DELETE /api/v1/service-requests/technician-schedules/{assignmentId}`
