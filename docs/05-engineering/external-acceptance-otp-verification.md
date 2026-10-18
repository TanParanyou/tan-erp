# External Acceptance OTP Verification (G-05)

## 1. สถานะ

**Implemented 2026-10-18 (G-05, email only) — no tests written, none run (per user); browser not checked; no real email sent; ยังไม่ผ่าน UAT.** Migration `AddAcceptanceOtp` สร้างแล้ว **ยังไม่ apply**. ปิดเป็นค่าเริ่มต้น (`AcceptanceOtp:Required=false`).

เอกสารที่เกี่ยวข้อง: [ADR 0029](../adr/0029-external-acceptance-otp.md), [External Acceptance API Contract](../03-contracts/external-acceptance-api-contract.md), [Go-live Runbook](wave-a-go-live-runbook.md) ข้อ 10.

## 2. ผลตรวจที่ทำ

| รายการ | ผล |
|---|---|
| `dotnet build` | 0 warnings / 0 errors |
| `npx tsc --noEmit`, `npm run lint`, `npm run build` | ผ่าน |
| OpenAPI regenerate + `npm run check:api` | ผ่าน (diff เป็นการเพิ่มล้วน) |
| Tests / ส่งอีเมลจริง / เปิดหน้าในเบราว์เซอร์ | **ไม่ได้ทำ** |

## 3. รายการที่ต้องตรวจก่อนเปิดจริง (manual)

1. ตั้ง `Email:*` และ `AcceptanceOtp:HmacKey`/`Required=true` ใน staging; ขอรหัสจากลิงก์ทดสอบแล้วได้อีเมลสองภาษา.
2. รหัสถูก → ยอมรับได้; `accept` โดยไม่แนบ token → `422 ACCEPTANCE_OTP_REQUIRED`; รหัสผิด 5 ครั้ง → `423` และลิงก์ล็อก; รหัสหมดอายุ/ใช้ซ้ำ → `410`; ขอซ้ำใน 60 วินาที/เกิน 3 ครั้งใน 1 ชั่วโมง → `429`.
3. ลิงก์ที่ไม่มีอีเมล: `POST …/otp` ตอบ `202` เหมือนลิงก์ที่มีอีเมล (ไม่มีอีเมลส่งออก); ลิงก์ไม่รู้จัก → `404` เดิม.
4. Evidence มี `otp_channel=email`, `otp_verified_at_utc`, `otp_challenge_id`; Audit มี 3 เหตุการณ์ และไม่มีรหัส/อีเมลใน `changes`; log ไม่มีรหัส.
5. `AcceptanceOtp:Required=false`: หน้าลูกค้าไม่แสดงขั้น OTP และยอมรับได้เหมือนเดิม.

## 4. ข้อจำกัด / เปิดอยู่

- ไม่มี test ใหม่และไม่ได้รันชุดทดสอบเดิม; Integration test เดิมของ acceptance ไม่ได้ถูกรันกับ constructor/signature ที่เปลี่ยน (compile ผ่านเท่านั้น).
- `LoggingEmailSender` เมื่อ OTP เปิดแต่ `Email:Enabled=false` → `ACCEPTANCE_OTP_UNAVAILABLE` (503) แทนการแกล้งส่ง.
- การล็อกลิงก์ไม่มีปุ่ม unlock; ให้สร้างลิงก์ใหม่. UI เจ้าหน้าที่แสดงสถานะล็อกไม่ได้ (ยังไม่ส่งออกใน link response).
- ยังไม่มี SMS; Legal ยังไม่ยืนยันระดับหลักฐาน.
- ไม่ตรวจว่า `Email:Enabled=true` ตอน `Required=true` ตอน start (จะได้ 503 ตอนใช้งาน).
