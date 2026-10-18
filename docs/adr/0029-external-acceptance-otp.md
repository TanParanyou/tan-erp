---
status: accepted
---

> สถานะ implementation: Implemented, email only (no tests written, none run; browser not checked; no real email sent) — 2026-10-18. ดู [Verification](../05-engineering/external-acceptance-otp-verification.md)

# External Acceptance OTP ทางอีเมล

ลิงก์ยอมรับใบเสนอราคา ([ADR 0013/0016 เดิม](../03-contracts/external-acceptance-api-contract.md)) ใช้ token ที่เดาไม่ได้เป็นกุญแจเดียว; ถ้าลิงก์หลุดต่อ ใครก็ยอมรับแทนลูกค้าได้. ADR นี้เพิ่มการพิสูจน์ว่าผู้ยอมรับเข้าถึงอีเมลผู้ติดต่อที่ผู้ขายระบุไว้กับลิงก์.

## ตัดสินใจ

1. **ช่องทางเดียวตอนนี้ = อีเมล** ผ่านพอร์ต `IEmailSender` (Application) — G-02 มีเฉพาะ in-app notification จึงเพิ่มพอร์ตนี้ใหม่. Implementation: `SmtpEmailSender` (`System.Net.Mail`, ไม่เพิ่ม package) และ `LoggingEmailSender` เมื่อ `Email:Enabled=false` (ไม่ log รหัสและไม่ log เนื้อหา). `Email:Password` มาจาก env/secret เท่านั้น.
2. **ปิดเป็นค่าเริ่มต้น** `AcceptanceOtp:Required=false` — พฤติกรรมเดิมไม่เปลี่ยน. เปิดแล้วไม่มี `AcceptanceOtp:HmacKey` (≥ 32 ตัวอักษร) แอปไม่ start.
3. **อีเมลผู้รับผูกกับลิงก์** (`recipient_email` ตอนสร้างลิงก์) เพราะลิงก์เดิมไม่ผูกผู้ติดต่อ; ไม่ส่งกลับให้ลูกค้าหรือเขียนลง Audit. เมื่อเปิด OTP การสร้างลิงก์ต้องมีอีเมล (`ACCEPTANCE_RECIPIENT_EMAIL_REQUIRED`).
4. **นโยบาย OTP** (ปรับได้ที่ `AcceptanceOtp:*`): 6 หลักจาก CSPRNG; เก็บเฉพาะ HMAC-SHA256(key, `linkId:code`) เทียบแบบ constant-time; อายุ 10 นาที; ผิด 5 ครั้งต่อรหัส → **ลิงก์ถูกล็อก** (`otp_locked_at_utc`, ต้องออกลิงก์ใหม่); ขอใหม่ได้ทุก 60 วินาที และไม่เกิน 3 ครั้ง/ชั่วโมง/ลิงก์; ใช้ครั้งเดียว.
5. **Flow:** `POST …/otp` (ตอบเหมือนกันไม่ว่ามีอีเมลหรือไม่ — ไม่เผยอีเมล) → `POST …/otp/verify` ได้ `verificationToken` อายุ 15 นาที (เก็บเฉพาะ hash, ผูกกับ challenge ของลิงก์นั้น) → `accept` ต้องแนบ token; การยอมรับสำเร็จ consume challenge. ลิงก์ที่ใช้ไม่ได้ยังตอบ `404 ACCEPTANCE_LINK_UNAVAILABLE` เดิม.
6. **หลักฐาน:** `quotation_acceptance_evidences` เก็บ `otp_channel` (`email`), `otp_verified_at_utc`, `otp_challenge_id` — ไม่เก็บรหัส. Audit: `otp-issued`, `otp-verified`, `otp-locked` (มี challenge id และ channel เท่านั้น).
7. ใช้ rate limiter public เดิม (policy ระดับ controller) กับ endpoint ใหม่ทั้งสอง.

## วิธีเปลี่ยนในภายหลัง

- **เพิ่ม SMS:** เพิ่มค่า `sms` ใน `OtpChannels` และ CHECK constraint ของ `acceptance_otp_challenges.channel` (migration), เพิ่ม `ISmsSender` (หรือขยายพอร์ตให้เลือก channel), เก็บเบอร์ผู้รับคู่กับ `recipient_email` (เช่น `recipient_phone`) แล้วให้ `RequestOtpAsync` เลือก channel ตามที่ลิงก์กำหนด; ตรรกะ hash/ล็อก/หลักฐานใช้ร่วมกันได้ทั้งหมด.
- **ปรับเกณฑ์:** แก้ `AcceptanceOtp:CodeLength|ExpiryMinutes|MaxAttempts|ResendCooldownSeconds|MaxSendsPerHour|VerificationMinutes` ใน config ไม่ต้อง deploy โค้ด.
- **เปลี่ยน key:** หมุน `AcceptanceOtp:HmacKey` ทำให้รหัสที่ยังค้างใช้ไม่ได้ (ผู้ใช้ขอใหม่) — ไม่กระทบการยอมรับที่เสร็จแล้ว.
- **Legal / ระดับหลักฐาน (ยังเปิด):** ยังไม่ยืนยันว่า OTP อีเมลพอสำหรับการยอมรับตามกฎหมาย/นโยบายบริษัทหรือต้องเสริม (SMS, ลายเซ็น, e-signature ภายนอก) — ต้องได้คำตอบก่อนเปิดจริง.

## ข้อเสียที่ยอมรับ

ผู้ที่เข้าถึงทั้งลิงก์และอีเมลผู้รับยังยอมรับแทนได้; ล็อกแล้วต้องออกลิงก์ใหม่ (ไม่มี unlock); อีเมลผู้รับพิมพ์โดย staff จึงผิดได้; `SmtpClient` ไม่มี connection pooling/OAuth (ย้ายไป MailKit ถ้าจำเป็น); ลิงก์ที่สร้างก่อนเปิด OTP โดยไม่มีอีเมลยอมรับไม่ได้หลังเปิด.
