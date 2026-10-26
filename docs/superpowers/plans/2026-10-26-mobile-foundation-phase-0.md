# Mobile Foundation Phase 0 — Design Tokens + Flutter App Skeleton

- Status: **Proposed** (2026-10-26) — เอกสารได้รับอนุมัติให้เขียน; การ implement ต้องได้รับอนุมัติแผนนี้ก่อน
- Decisions: [ADR 0030](../../adr/0030-flutter-field-companion-app.md), [ADR 0031](../../adr/0031-cross-platform-design-tokens-and-theming.md)
- Architecture: [Mobile Architecture](../../02-architecture/mobile-architecture.md), [design.md หมวด 7](../../../design.md)
- Tests: ไม่เขียนและไม่รัน test ใหม่ (ตามข้อตกลงกับผู้ใช้); รักษา test เดิมให้ compile ได้

## Goal

วางฐานที่ feature ภาคสนามทุกตัวจะต่อยอด: (A) ระบบ Design Tokens ที่เว็บใช้จริงโดยหน้าตาไม่เปลี่ยน และ preference ธีมของผู้ใช้เก็บที่ Backend, (B) แอป Flutter ที่ล็อกอินได้ เลือกสาขาได้ มีโครง navigation, theme, i18n, network และ error handling ตามมาตรฐาน — **ยังไม่มีหน้าจอธุรกิจ**

## Out of scope (เฟสถัดไป)

Push notification (iOS ต้องมีบัญชี Apple Developer), offline/outbox, หน้าจอ Field Survey/Quick Estimate, สแกน/เลเซอร์/LiDAR, สีแบรนด์องค์กร (admin), การส่งขึ้นสโตร์

## Prerequisites (ผู้ใช้ทำ)

1. ติดตั้ง Flutter SDK (stable), Xcode + CocoaPods, Android Studio + Android SDK; `flutter doctor` ผ่าน
2. Firebase project สำหรับ dev: เพิ่มแอป iOS `com.tanerp.field.dev` และ Android `com.tanerp.field.dev`, เปิด Email/Password sign-in, ดาวน์โหลด config (ไม่ commit)
3. Backend รันบน Mac และเข้าถึงได้จากมือถือผ่าน LAN

## Track A — Design Tokens (เว็บ) + User Theme Preferences

### A1. `design-tokens/` + build
- สร้าง `design-tokens/{primitive,semantic,themes}/*.json` (DTCG) ด้วยค่าปัจจุบันจาก `frontend/src/styles/erp-theme.css` และ design.md หมวด 3 (light + dark)
- Style Dictionary config + npm script ที่ root (`npm run tokens:build`) สร้าง `frontend/src/styles/tokens.css` และ `mobile/lib/core/theme/tokens.g.dart`
- Done: build ซ้ำได้ผลเหมือนเดิม; ไฟล์ generate มี header "do not edit"

### A2. เว็บใช้ tokens.css โดยหน้าตาไม่เปลี่ยน
- `erp-theme.css` import `tokens.css` แทนการประกาศตัวแปรเอง (เก็บชื่อ `--erp-*` เดิมเพื่อไม่ต้องแก้ Tailwind config/คลาสเดิม)
- แทน `border-radius: 0 !important` (ราว 13 จุดใน `erp-theme.css`) ด้วย `var(--erp-radius-*)`; Tailwind `borderRadius` map ไปที่ตัวแปร
- Done: เปรียบเทียบหน้าหลัก (login, list, form, modal) ก่อน/หลังด้วย screenshot ใน browser — ต้องเหมือนเดิม

### A3. ล้าง hex/inline style ที่ข้าม token
- Inventory: `grep -rnE "#[0-9A-Fa-f]{3,8}\b" frontend/src --include=*.tsx` และ `style={{` ที่มีสี/radius (เช่น `erp-shell.tsx`)
- แทนด้วย Tailwind semantic classes; แก้เฉพาะสี/radius/border ไม่ refactor อื่น
- Done: grep ไม่พบ hex ใน `.tsx` นอกไฟล์ยกเว้นที่ระบุเหตุผล

### A4. Lint กันถอยหลัง
- `frontend/eslint.config.mjs` ตอนนี้ไม่มี rule — เพิ่ม `no-restricted-syntax` ห้าม string literal hex ใน JSX `className`/`style` (ขอบเขตเล็กสุด ไม่เปิด rule set อื่นในรอบนี้)
- Done: `npm run lint` ตรวจไฟล์จริง และผ่าน

### A5. User theme preferences (Backend + เว็บ)
- `User` เพิ่ม `ThemeMode` (`light|dark|system`), `Density` (`comfortable|compact`), `TextScale` (90–130) ต่อจาก `PreferredLocale`; migration; validation ใน Application
- `GET /api/v1/me` คืนค่า preferences; endpoint อัปเดต (ต่อจาก `PUT /api/v1/me/profile` หรือ `PUT /api/v1/me/preferences` — เลือกตามรูปแบบที่ profile ใช้อยู่ตอน implement)
- OpenAPI + generated TS; เว็บ: `ThemeToggle` และหน้าโปรไฟล์อ่าน/บันทึกค่าที่ server (localStorage เป็นแค่ค่าเริ่มต้นก่อนโหลด), เพิ่ม `data-density` และ text scale ที่ `<html>`
- i18n th/en ครบ
- Done: เปลี่ยนค่าบนเว็บ → reload/เครื่องอื่นได้ค่าเดียวกัน

## Track B — Flutter App Skeleton (`mobile/`)

### B1. Scaffold + มาตรฐาน
- `flutter create` (org `com.tanerp`, app `field`), flavors dev/staging/prod (`main_<flavor>.dart`, bundle ID ตาม Mobile Architecture), `config/<flavor>.json` + `.example` (commit เฉพาะ example)
- `analysis_options.yaml`: `very_good_analysis` + `strict-casts`, `strict-inference`, `strict-raw-types`
- โครงโฟลเดอร์ตาม Mobile Architecture; `.gitignore` สำหรับ build, Pods, config จริง, Firebase config
- Done: `flutter analyze` 0 issue; `flutter run --flavor dev` เปิดบน simulator/emulator

### B2. Network + Contract
- สคริปต์ generate API client จาก `contracts/openapi/tan-erp.v1.json` (openapi-generator `dart-dio`) ลง `lib/generated/api/`
- dio + interceptors: Firebase ID token, membership header (ชื่อเดียวกับเว็บ), `Accept-Language`, idempotency key สำหรับคำสั่งเขียน, `If-Match`; แปลง RFC 7807 เป็น `ApiFailure` (sealed: validation, conflict, unauthorized, forbidden, notFound, network, unknown) โดยอ่าน `code` ไม่อ่าน message
- Done: เรียก `GET /api/v1/me` ผ่าน LAN สำเร็จจาก flavor dev

### B3. Theme + UI กลาง
- `ErpTokens` ThemeExtension จาก `tokens.g.dart`; `buildTheme(mode, density, textScale)` light/dark; radius/border/spacing มาจาก token เท่านั้น
- Widget กลางใน `lib/core/ui/`: `ErpButton` (primary/outline/ghost, isLoading, ≥ 44dp), `ErpTextField` (label, placeholder จาก l10n, error), `ErpSelect`, `ErpAlert`, `ErpEmptyState`, `ErpMonoLoading`, `ErpConfirmDialog`
- ไอคอน SVG stroke ผ่าน `flutter_svg` (คัดชุดที่ใช้จาก `frontend/src/components/common/Icons`)
- `custom_lint` rule ห้าม `Color(0x…)` และ `BorderRadius.circular` นอก `lib/core/theme/`
- Done: หน้า "UI gallery" (เฉพาะ flavor dev) แสดงทุก widget ใน light/dark/compact/130%

### B4. i18n
- gen-l10n: `app_th.arb` (template, ค่าเริ่มต้น) + `app_en.arb`; สคริปต์ตรวจ parity คีย์ (fail เมื่อไม่ครบ)
- Done: สลับภาษาตาม `PreferredLocale` ของผู้ใช้

### B5. Auth + Session + Membership
- `AuthRepository` (interface) + `FirebaseEmailAuthRepository`; secure storage; biometric gate เมื่อเปิดแอปครั้งถัดไป (fallback เป็นรหัสเครื่อง)
- โหลด current user → เลือก membership/สาขา (จำค่าล่าสุด) → router guard
- Error: รหัสผ่านผิด/บัญชีถูกปิด/ไม่มี membership → ข้อความจาก l10n ตาม error code
- Done: ล็อกอิน → เลือกสาขา → เข้าหน้าหลัก; ปิด-เปิดแอปต้องผ่าน biometric; logout ล้าง secure storage

### B6. App shell + หน้า "ฉัน"
- มือถือ: bottom nav 5 เมนู (งานวันนี้ · สำรวจ · สแกน · แจ้งเตือน · ฉัน) — 4 เมนูแรกเป็น `ErpEmptyState` "กำลังพัฒนา"
- แท็บเล็ต (≥ 768dp): navigation rail + พื้นที่ master-detail
- หน้า "ฉัน": โปรไฟล์, สาขา, ภาษา, โหมด/ความแน่น/ขนาดตัวอักษร (บันทึกผ่าน API จาก A5), เวอร์ชันแอป, ออกจากระบบ
- Done: เปลี่ยนธีมในแอป → เว็บได้ค่าเดียวกันหลัง reload และกลับกัน

### B7. Version policy + Crash reporting
- Backend: `GET /api/v1/app/version-policy?platform=ios|android` คืน `minimumVersion`, `latestVersion` (ค่าจาก config, ไม่ต้องมีตาราง)
- แอป: เช็กตอนเปิด; ต่ำกว่า minimum → จอบังคับอัปเดต (ข้อความ l10n)
- Crashlytics เปิดใน flavor dev (ไม่ log token/PII)
- Done: ตั้ง minimum สูงกว่าเวอร์ชันแอป → เห็นจอบังคับอัปเดต

## ลำดับและการแยกงาน

- Track A และ B ทำขนานได้ ยกเว้น **B3 ต้องรอ A1** (tokens.g.dart) และ **B6 ส่วน preferences ต้องรอ A5**
- ลำดับแนะนำ: A1 → (A2, A3, A4, A5) ∥ (B1 → B2 → B4 → B5) → B3 → B6 → B7

## Verification gates (ก่อนส่งมอบ ครั้งเดียว)

- Backend: `dotnet build`
- เว็บ: `npm run tokens:build` (ไม่มี diff หลัง build ซ้ำ), `npx tsc --noEmit`, `npm run lint`, `npm run build`, screenshot เปรียบเทียบ A2
- แอป: `flutter analyze` (0 issue), `dart run custom_lint`, สคริปต์ l10n parity, `flutter build apk --flavor dev`, `flutter build ios --flavor dev --no-codesign`
- ไม่เขียน/ไม่รัน test ใหม่ (ตามผู้ใช้)

## ความเสี่ยง

- **ไม่มีโดเมน/HTTPS:** staging/prod ยังกำหนด `apiBaseUrl` ไม่ได้ — flavor dev เท่านั้นที่ใช้งานได้ในเฟสนี้
- **iOS free provisioning:** แอปหมดอายุทุก 7 วันบนเครื่องจริง
- **A2 อาจทำให้หน้าตาเพี้ยน** ถ้ามี selector พึ่ง `!important` — ป้องกันด้วย screenshot ก่อน/หลัง
- **AGENTS.md หมวด 6 ยังเขียนว่า `border-radius: 0px !important`** — ต้องปรับข้อความให้ตรงกับ design.md หมวด 7 (รอผู้ใช้อนุมัติแก้ AGENTS.md)
