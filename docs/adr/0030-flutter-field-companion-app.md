---
status: accepted
---

> สถานะ implementation: Not started — เอกสารตัดสินใจเท่านั้น (2026-10-26). แผนเริ่มต้นอยู่ที่ [Mobile Foundation Phase 0](../superpowers/plans/2026-10-26-mobile-foundation-phase-0.md)

# แอปภาคสนามด้วย Flutter (Field Companion App)

ทีมขาย/สำรวจ/ติดตั้งทำงานหน้างานที่สัญญาณไม่แน่นอน และต้องใช้ความสามารถของเครื่องที่เว็บบน iOS ทำได้จำกัด: Bluetooth (เครื่องวัดเลเซอร์), ทำงานออฟไลน์ระยะยาว, อัปโหลดเบื้องหลัง, push ที่เชื่อถือได้, สแกนบาร์โค้ด/เอกสาร, LiDAR. เว็บ Next.js เดิมยังเป็นช่องทางหลักของงานโต๊ะ.

## ตัดสินใจ

1. **สร้างแอปเนทีฟด้วย Flutter** สำหรับ iOS/Android ทั้งมือถือและแท็บเล็ต อยู่ใน monorepo ที่ `mobile/`.
2. **ขอบเขต = Field Companion App ไม่ใช่ ERP ทั้งระบบ:** งานวันนี้, Site Survey/วัดพื้นที่, Quick Estimate หน้างาน, ถ่ายรูป/สแกน, รับของด้วยบาร์โค้ด, แจ้งเตือนและอนุมัติด่วน. Official Estimate, Master Data, Settings และงานจอใหญ่อื่นอยู่บนเว็บ.
3. **Backend เดิมเป็นเจ้าของกฎทั้งหมด:** แอปเรียก REST API เดียวกับเว็บ; สิทธิ์/RBAC/validation อยู่ที่ API (ตาม [ADR 0004](0004-firebase-identity-backend-rbac.md)). ไม่มี business logic ซ้ำในแอปเกินกว่าการแสดงผลและ validation ฝั่ง UX.
4. **Contract-first:** type ของ API generate จาก `contracts/openapi/tan-erp.v1.json` (openapi-generator `dart-dio`) — ห้ามเขียน DTO เอง เช่นเดียวกับเว็บ.
5. **Offline-first เฉพาะ flow ภาคสนาม:** ข้อมูลอ่านเก็บใน SQLite (drift), การเขียนผ่าน outbox ที่ใช้ idempotency key + `If-Match` rowVersion ที่ API มีอยู่แล้ว; conflict ให้ผู้ใช้เลือก ไม่ merge เงียบ.
6. **Auth:** Firebase Authentication (อีเมล/รหัสผ่านก่อน) ซ่อนหลัง `AuthRepository` เพื่อเพิ่ม provider ภายหลัง; token เก็บใน Keychain/Keystore, ปลดล็อกด้วย biometric.
7. **Theme มาจาก Design Tokens ชุดเดียวกับเว็บ** ตาม [ADR 0031](0031-cross-platform-design-tokens-and-theming.md).

รายละเอียด stack และโครงสร้างอยู่ที่ [Mobile Architecture](../02-architecture/mobile-architecture.md).

## ทางเลือกที่ไม่เลือก

- **Capacitor ห่อเว็บเดิม:** reuse UI ได้มากที่สุด แต่ผู้ใช้เลือก Flutter เพื่อ UX เนทีฟเต็มรูปแบบ, offline/background ที่ควบคุมได้ และ performance บนกล้อง/AR.
- **React Native/Expo:** ต้องเขียน UI ใหม่เท่ากับ Flutter แต่ไม่ได้ข้อได้เปรียบด้าน rendering/consistency ข้ามแพลตฟอร์ม.
- **PWA อย่างเดียว:** iOS ไม่มี Web Bluetooth, ลบ storage ของเว็บที่ไม่ใช้ราว 7 วัน, อัปโหลดเบื้องหลังไม่ได้.

## ข้อเสียที่ยอมรับ

- หน้าจอภาคสนามต้องเขียนใหม่ใน Dart (UI ซ้ำกับเว็บในส่วนนั้น) — ลดด้วยการจำกัดขอบเขตข้อ 2, generate contract และ token ร่วม.
- i18n มีสองชุดไฟล์ (next-intl JSON และ ARB) — ใช้คีย์รูปแบบ ICU เดียวกันและมีสคริปต์ตรวจ parity.
- ต้องมีบัญชี Apple Developer ($99/ปี) ก่อน TestFlight/App Store และก่อนใช้ push บน iOS; Google Play $25 ครั้งเดียว. ช่วงพัฒนาใช้เครื่องจริงได้ฟรี (iOS free provisioning หมดอายุทุก 7 วัน).
- ต้องมีโดเมน + HTTPS ของ API ก่อนปล่อยจริง; ช่วงพัฒนาใช้ HTTP บน LAN เฉพาะ flavor `dev`.
- Release ต้องผ่านรีวิวสโตร์ จึงต้องมี API ตรวจเวอร์ชันขั้นต่ำเพื่อบังคับอัปเดตเมื่อ contract เปลี่ยนแบบ breaking.

## วิธีเปลี่ยนในภายหลัง

- เพิ่ม flow เข้าแอปทีละ feature ตาม roadmap — ทุก feature ต้องมี endpoint บนเว็บ/API อยู่ก่อน.
- เพิ่ม Google/Apple sign-in: implement provider ใหม่ใน `AuthRepository`; ถ้าเพิ่ม Google ต้องเพิ่ม Sign in with Apple ด้วย (App Store guideline 4.8).
- LiDAR (RoomPlan) / ARCore: เขียน native module แล้วเชื่อมผ่าน Pigeon โดยไม่กระทบ feature อื่น.
