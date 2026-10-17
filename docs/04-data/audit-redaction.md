# Audit Redaction และ Retention

อ้างอิง: ADR [0027](../adr/0027-wave-a-closure-decisions.md) D-B. สถานะ: Implemented (no tests written, none run).

## Redaction (B3)

- จุดเดียว: `AuditRedactor.Redact` ถูกเรียกใน constructor ของ `AuditEvent` (`backend/src/TanErp.Domain/Common/`). ไม่ต้องแก้ที่จุดเรียกแต่ละที่.
- key ที่ถูกปิดบัง (ค่าเริ่มต้น): `email, phone, mobile, address, taxId, nationalId, displayName, firstName, lastName, fullName` — เทียบแบบไม่สนตัวพิมพ์, ไล่ทุกระดับของ object/array; ค่าทั้งก้อนของ key นั้นกลายเป็น `"[redacted]"`.
- ปรับด้วย config `Audit:RedactedKeys` (array; ว่าง/ไม่ตั้ง = ค่าเริ่มต้น). อ่านครั้งเดียวตอน startup.
- การอ่านผ่าน `GET /api/v1/audit-events` redact ซ้ำ เพื่อครอบแถวเก่า; **ข้อมูลเดิมในตารางไม่ถูกแก้** (แถวที่เขียนก่อนมี redactor ยังอาจมีค่าเหล่านี้ใน DB).
- ข้อจำกัด: เป็น deny-list ตามชื่อ key ไม่ใช่การตรวจเนื้อหา; ค่าส่วนบุคคลที่อยู่ใน key ชื่ออื่นไม่ถูกจับ. ตารางคอลัมน์ `audit.audit_events.changes` เป็น jsonb.
- Script กู้สิทธิ์ (`scripts/ops/administrator-recover.sql`) เขียนตรงเข้าตาราง ไม่ผ่าน redactor (เก็บเฉพาะ id).

## Retention (B2 — เลื่อน)

- ไม่มีการลบ/archive อัตโนมัติ. `Audit:RetentionDays` (ค่าเริ่มต้น `null` = เก็บตลอด) **reserved** ยังไม่มีโค้ดอ่าน.
- นโยบายเริ่มต้นที่แนะนำ (รอ Legal ยืนยัน): เอกสารการเงิน/ภาษี 7 ปี, access log 1 ปี.
