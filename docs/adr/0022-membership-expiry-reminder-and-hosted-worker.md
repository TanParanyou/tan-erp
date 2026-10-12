---
status: accepted
---

# แจ้งเตือน Membership ใกล้หมดอายุด้วย hosted worker ขั้นต่ำ และ notification แบบ system-generated

G-21 ต้องแจ้งผู้ดูแลก่อน Membership หมดอายุ ซึ่งเป็น event ที่ไม่ได้เกิดจากคำขอของผู้ใช้ จึงต้องมี scheduler — repo ยังไม่มีงานตามเวลาใด ๆ.

**การตัดสินใจ 1 — Scheduler:** `BackgroundService` ใน `TanErp.Api` ใช้ `PeriodicTimer` เรียก `IMembershipExpiryReminderService` (scoped) ทุกรอบ (ค่าเริ่มต้น 60 นาที, เริ่มหลังแอปขึ้น 60 วินาที). ไม่ใช้ Quartz/Hangfire (ไม่มี dependency/ตารางใหม่). ไม่ลงทะเบียนเมื่อ environment = `Test` หรือ `MembershipExpiryReminder:Enabled=false`. Logic ทั้งหมดอยู่ใน service เพื่อให้ทดสอบด้วย fake clock โดยไม่ต้องรอเวลา.

**การตัดสินใจ 2 — ความถูกต้องเมื่อมีหลาย instance:** service ทำงานใน transaction เดียวและเรียก `pg_try_advisory_xact_lock`; instance ที่ไม่ได้ล็อกข้ามรอบ. unique index `ux_notifications_recipient_dedupe` ของ G-02 เป็นตาข่ายชั้นสอง (ชนแล้ว rollback รอรอบถัดไป).

**การตัดสินใจ 3 — Idempotency โดยไม่มีตารางสถานะ:** transition id ของ notification = hash แบบ deterministic ของ `(membershipId, thresholdDays, expiresAtUtc)`. ส่งได้ครั้งเดียวต่อค่าชุดนี้ต่อผู้รับ; แก้ `ExpiresAtUtc` ทำให้ hash เปลี่ยนและแจ้งใหม่ได้ (re-arm). ต่อรอบส่งเฉพาะ threshold ที่เล็กที่สุดที่เข้าเกณฑ์ (30/7/1 วัน เป็นค่า TEST_ONLY ที่ตั้งได้).

**การตัดสินใจ 4 — system-generated notification:** descriptor มีธง `SystemGenerated`; event ประเภทนี้ไม่มี actor และ payload ไม่มี `actorDisplayName`. type ปกติยังต้องมี actor ทุกกรณี (planner บังคับทั้งสองทิศทาง).

**การตัดสินใจ 5 — ผู้รับ:** ผู้ถือ `memberships.manage` ที่ Organization scope (ผู้ที่ต่ออายุได้จริง) ผ่าน resolver เดิม; ไม่ส่ง "หมดอายุแล้ว" ย้อนหลัง.

ข้อเสียที่ยอมรับ: ไม่มีหน้าจอสถานะ job; รอบสแกนละ N+1 query ขนาดเล็ก (จำกัด 200 รายการ/รอบ); ผู้ถือ `memberships.manage` ที่ไม่มี `users.read` เห็นข้อความแต่เปิดลิงก์ไม่ได้.
