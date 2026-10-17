# Audit API Contract

> **สถานะ:** Implemented (no tests written, none run) 2026-10-17 — ADR [0027](../adr/0027-wave-a-closure-decisions.md) D-B/B1. ไม่ได้ตรวจในเบราว์เซอร์.

## `GET /api/v1/audit-events`

- **Permission:** `audit.read` (ตรวจก่อนเข้าถึงข้อมูลเสมอ; ไม่มีสิทธิ์ = `403 PERMISSION_DENIED`). ขอบเขตผลลัพธ์ = Organization ของ Membership ใน `X-Membership-Id` เท่านั้น.
- **Query:** `resourceType`, `resourceId`, `action` (เทียบเท่ากันตรงตัว), `actorUserId`, `from`, `to` (ISO-8601 date-time; `from` ต้องไม่เกิน `to` มิฉะนั้น `400 REQUEST_VALIDATION_FAILED`), `page` (เริ่ม 1), `limit` (ค่าเริ่มต้น 25, สูงสุด 100).
- **เรียง:** `occurredAtUtc` ใหม่ → เก่า (แล้ว `id` ใหม่ → เก่า).
- **Response 200:** `{ items: [{ id, occurredAtUtc, action, resourceType, resourceId, actor: { id, name }, branchId, traceId, requestId, reason, changesJson }], pagination: { page, pageSize, totalCount, totalPages } }`.
  - `actor` ประกอบจาก Backend (ไม่ต้อง lookup ที่ FE). `changesJson` เป็นสตริง JSON ที่ผ่าน `AuditRedactor` แล้ว ([Audit Redaction](../04-data/audit-redaction.md)).
- **Errors:** ใช้รหัสเดิม (`PERMISSION_DENIED`, `REQUEST_VALIDATION_FAILED`, auth errors); ไม่มีรหัสใหม่. RFC 7807.
- ไม่มี endpoint เขียน/ลบ Audit.
