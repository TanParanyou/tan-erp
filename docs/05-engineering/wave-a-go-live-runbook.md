# Wave A Go-Live Runbook

ลำดับขั้นตอนสำหรับ Production ของงาน Wave A (G-03, G-04, G-08, G-21). ที่มา: [ADR 0027](../adr/0027-wave-a-closure-decisions.md). **สถานะ:** เอกสารและโค้ดที่เกี่ยวข้องเขียนแล้ว (no tests written, none run; ไม่ได้ตรวจในเบราว์เซอร์; ยังไม่ผ่าน UAT) — ทำตามบน staging ก่อน.

ตัวแปรสภาพแวดล้อม ASP.NET Core ใช้ `__` แทน `:` (เช่น `AccessBootstrap__Enabled=true`).

## ขั้นตอน

| # | ขั้นตอน | ค่า config / API | วิธีตรวจ |
| --- | --- | --- | --- |
| 1 | รัน migration ให้ครบ (`make db-migrate` หรือเทียบเท่า) | — | `make db-status` ไม่มีรายการค้าง |
| 2 | ตั้ง `is_system = true` ให้ Role ผู้ดูแลระบบของแต่ละ Organization (ไม่มี API; ทำด้วย SQL/bootstrap ครั้งเดียว) | `UPDATE identity_access.roles SET is_system = true WHERE id = '<admin-role-id>';` | `SELECT name, is_system FROM identity_access.roles;` |
| 3 | มอบสิทธิ์ G-03 ให้ Role ระบบ: เปิด `AccessBootstrap:Enabled=true` แล้วเริ่มแอป 1 ครั้ง (idempotent; เติมเฉพาะที่ขาด: organizations.read/manage, branches.manage, users.read/manage, memberships.manage, roles.assign, roles.assign-approval, roles.manage, approval-limits.manage, audit.read) แล้วตั้งกลับเป็น `false` | `AccessBootstrap:Enabled` | log `AccessBootstrap: N permission grant(s) added`; ผู้ดูแลล็อกอินแล้วเห็นเมนู Organization/Branches/Roles/Approval limits/Audit log |
| 4 | ตั้งวงเงินอนุมัติ (ไม่ตั้ง = Estimate/PO/Change Order อนุมัติไม่ได้, `APPROVAL_AUTHORITY_NOT_CONFIGURED`). ทำผ่านหน้า `/settings/approval-limits` หรือ `POST /api/v1/admin/approval-limits` หรือ seeder แบบ opt-in `ApprovalLimits:Seed` (ดูตัวอย่างด้านล่าง; ใช้ได้เมื่อ Role มีอยู่แล้ว; ไม่เขียนทับของเดิม) | `ApprovalLimits:Seed` = `[{ "documentType": "estimate", "roleCode": "Manager", "maxAmount": 500000, "currency": "THB" }]` | `/settings/approval-limits` เห็นแถว; log `ApprovalLimits:Seed created ...`; Audit มี `approval-limits.seeded`; ทดสอบอนุมัติเอกสารเกินวงเงินต้องได้ `APPROVAL_LIMIT_EXCEEDED` |
| 5 | G-04 PDF ใบเสนอราคา: Business ยืนยันเงื่อนไข license QuestPDF ([ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md)) แล้วตั้ง `Documents:QuestPdf:LicenseType` (`Community`/`Professional`/`Enterprise`) และ `Documents:QuestPdf:LicenseConfirmed=true`; บันทึกผู้ยืนยัน/วันที่ใน [Quotation PDF Verification](quotation-pdf-verification.md) | `Documents:QuestPdf:LicenseType`, `Documents:QuestPdf:LicenseConfirmed` | log ตอน start ไม่มี warning "QuestPDF license type ... is not confirmed" |
| 6 | G-04 ที่เก็บไฟล์: ตั้ง `Storage:BasePath` เป็นไดเรกทอรีที่เขียนได้ ถาวร และมี backup | `Storage:BasePath` | อัปโหลดไฟล์ทดสอบ/ออกใบเสนอราคา แล้วพบไฟล์ใต้ `{BasePath}/{organizationId}/...` |
| 7 | G-08 หน่วยแปลงในประมาณราคา: publish Calculation Policy v2 แบบ `unit_conversion_mode='snapshot'` (ไม่มี API; ops เพิ่มแถวเอง ตามขั้นตอนใน [Estimate Unit Conversion Verification](estimate-unit-conversion-verification.md) หัวข้อข้อจำกัดข้อ 2) | ตาราง `calculation_policy_versions` | เลือกหน่วยที่ไม่ใช่หน่วยฐานในรายการต้นทุนได้โดยไม่ได้ `ESTIMATE_UNIT_INVALID`; ไม่ทำ = ความสามารถนี้ปิดอยู่ |
| 8 | G-21 Membership ใกล้หมดอายุ: worker ทำงานเมื่อ `MembershipExpiryReminder:Enabled=true` (ค่าเริ่มต้น); ปรับ `IntervalMinutes` (60), `StartupDelaySeconds` (60), `MaxMembershipsPerRun` (200), `ThresholdDays` ([30,7,1] — ค่าเดิมเป็น TEST_ONLY ให้ธุรกิจยืนยัน). รันได้เพียง 1 instance ต่อ deployment หรือยอมรับการแจ้งซ้ำตาม [ADR 0022](../adr/0022-membership-expiry-reminder-and-hosted-worker.md) | `MembershipExpiryReminder:*` | log ของ worker รายรอบ; ผู้ถือ `memberships.manage` เห็นแจ้งเตือนเมื่อมี Membership ใกล้หมดอายุ — ดู [Membership Expiry Verification](membership-expiry-verification.md) |
| 9 | Audit: ตรวจ `Audit:RedactedKeys` (ค่าเริ่มต้นใน appsettings) และยืนยันกับ Security/Legal; `Audit:RetentionDays` สงวนไว้ (ยังไม่มีผล) | `Audit:RedactedKeys`, `Audit:RetentionDays` | เปิด `/settings/audit`; ค่าของ key ส่วนบุคคลใน `changesJson` เป็น `[redacted]` |
| 10 | G-05 OTP ยอมรับใบเสนอราคา (ปิดอยู่เป็นค่าเริ่มต้น): ตั้ง SMTP ใน `Email:*` (`Enabled=true`, `Host`, `Port`, `UseStartTls`, `Username`, `FromAddress`, `FromName`; **`Email:Password` ผ่าน env/secret store เท่านั้น**) แล้วตั้ง `AcceptanceOtp:HmacKey` (สุ่ม ≥ 32 ตัวอักษร ผ่าน secret store; ห้ามใช้ซ้ำข้าม environment) จึงเปิด `AcceptanceOtp:Required=true` (ถ้าเปิดโดยไม่มี key แอปไม่ start). เมื่อเปิด การสร้างลิงก์ต้องระบุอีเมลผู้รับ; ลิงก์ที่สร้างก่อนเปิดและไม่มีอีเมลจะยอมรับไม่ได้ ต้องสร้างใหม่. ปรับค่าอื่นได้ที่ `AcceptanceOtp:*` ตาม [ADR 0029](../adr/0029-external-acceptance-otp.md). Legal ต้องยืนยันระดับหลักฐานก่อนเปิดจริง | `Email:*`, `AcceptanceOtp:*` | ลิงก์ทดสอบ: ขอรหัสแล้วได้อีเมล, ใส่รหัสถูกจึงอนุมัติได้, ใส่ผิด 5 ครั้งลิงก์ถูกล็อก; Audit มี `otp-issued/otp-verified/otp-locked` โดยไม่มีรหัส — ดู [OTP Verification](external-acceptance-otp-verification.md) |

## ตัวอย่างตารางวงเงิน (EXAMPLE VALUES — Finance ต้องยืนยันก่อนใช้จริง)

| Role (ตัวอย่าง) | estimate | purchase_order | change_order |
| --- | ---: | ---: | ---: |
| Sales staff | 100,000 | 0 (ไม่อนุมัติ) | 0 |
| Manager | 1,000,000 | 500,000 | 500,000 |
| Director | ไม่จำกัด | ไม่จำกัด | ไม่จำกัด |

ตัวเลขข้างต้นเป็นตัวอย่างเพื่อสาธิตรูปแบบเท่านั้น ไม่ใช่นโยบาย. `ApprovalLimits:Seed` รองรับเฉพาะวงเงินที่มีจำนวนเงิน (THB, ทั้งองค์กร); "ไม่จำกัด" ให้ตั้งผ่านหน้า `/settings/approval-limits` (การให้สิทธิ์ `approval-limits.manage` เองต้องผ่าน maker–checker ตาม [ADR 0020](../adr/0020-role-management-and-approval-permission-set.md)).

## หมายเหตุ

- Seeder ทั้งสองทำงานตอน start ของแอปเฉพาะนอก Test environment และเฉพาะเมื่อเปิดใช้; ล้มเหลว = log error แอปทำงานต่อ.
- การตั้งค่าทั้งหมดยังไม่ผ่านการทดสอบอัตโนมัติ (ไม่ได้เขียนและไม่ได้รัน test ในรอบนี้ ตามคำสั่งผู้ใช้).
