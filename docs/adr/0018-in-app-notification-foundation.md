---
status: accepted
---

# Notification Foundation (in-app): สร้างใน Transaction เดียวกับ Business Change โดยไม่ใช้ Outbox

หลาย slice ต้องแจ้งผู้ใช้ (G-02 รออนุมัติ, G-05 OTP, G-18 SLA, G-21 membership หมดอายุ). รอบนี้ทำเฉพาะการแจ้งเตือนในระบบ; อีเมลถูกเลื่อนตามคำสั่งผู้ใช้.

**การตัดสินใจ 1 — แจ้งเตือนต้องไม่หายและไม่ค้างเกิน:** `INotificationPublisher` (Application) ถูกเรียกจาก store ของโมดูลที่เปลี่ยนสถานะ และ implementation *stage* `Notification` เข้า `AppDbContext` ตัวเดียวกัน (scoped) โดยไม่ `SaveChanges` เอง — transaction/`SaveChanges` ของ business change เป็นคน commit. Business change rollback → ไม่มี notification; commit → มี notification แน่นอน. ข้อเสียที่ยอมรับ: publisher ที่ผิดพลาด (payload ผิด type) ทำให้ business action ล้มเหลว จึงล็อกด้วย unit test ของ `NotificationEvents` ทุกแหล่ง และ publisher ตรวจ dedupe key ก่อน stage เพื่อไม่ให้ unique index ของ notification ทำให้ transaction ของเอกสารล้ม.

**การตัดสินใจ 2 — Notification type อยู่ในโค้ด:** `NotificationTypes` (Domain) + `NotificationTypeRegistry` (Application: permission ปลายทาง, deep-link template, payload fields). payload ใช้ allowlist ต่อ type เพื่อไม่ให้ตัวเลขต้นทุน/ราคา/margin หรือ PII หลุด; ลิงก์ในรายการคำนวณตามสิทธิ์ **ปัจจุบัน** ของผู้อ่าน (ไม่มีสิทธิ์แล้ว → ไม่มีลิงก์).

**การตัดสินใจ 3 — ไม่ใช้ outbox และไม่แตะ Finance:** แผนหลักเสนอให้ดึง retry/backoff/dead/requeue ของ Finance accounting outbox ขึ้นเป็นของกลาง (`Common/Outbox`) เพื่อใช้กับอีเมล. เมื่ออีเมลถูกเลื่อน การแจ้งเตือนในระบบเป็นแถวในฐานข้อมูลเดียวกัน ไม่มีการส่งออกภายนอก จึงไม่มีสิ่งใดต้อง retry; การ extract entity ของ Finance ที่ใช้งานจริงโดยไม่มีผู้ใช้ตัวที่สองจะเพิ่มความเสี่ยง (EF map property ของ base class, schema ของ Finance) โดยไม่ได้ประโยชน์ (YAGNI). **จึงยกเลิกงาน extract; Finance ไม่ถูกแก้.** เมื่อมี slice อีเมล (หรือช่องทางส่งออกอื่น) ให้ประเมินใหม่: ถ้ามีช่องทางที่สอง ให้ extract state machine ขึ้น `Domain/Common/Outbox` (ไม่ใช่ `Application/Common` เพราะเป็นพฤติกรรมของ entity และ Domain ห้ามพึ่ง Application) โดยป้องกันด้วย `FinanceDomainTests.Outbox_*`, `FinanceEndpointsTests.AccountingOutbox_*` และ `dotnet ef migrations has-pending-model-changes`.

ทางเลือกที่ไม่เลือก: สร้าง outbox ตารางใหม่เผื่ออนาคต (YAGNI; ผูก schema กับ provider ที่ยังไม่เลือก); เขียน notification หลัง commit ด้วย background job (event หายได้เมื่อ process ตายระหว่างสอง commit).
