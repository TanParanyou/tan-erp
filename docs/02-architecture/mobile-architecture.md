# Mobile Architecture (สถาปัตยกรรมแอปภาคสนาม)

> สถานะ: Proposed — ยังไม่มีโค้ด (2026-10-26). เหตุผลการตัดสินใจ: [ADR 0030](../adr/0030-flutter-field-companion-app.md), [ADR 0031](../adr/0031-cross-platform-design-tokens-and-theming.md). แผนเริ่มต้น: [Mobile Foundation Phase 0](../superpowers/plans/2026-10-26-mobile-foundation-phase-0.md)

แอป Flutter สำหรับงานภาคสนามบน iOS/Android ทั้งมือถือและแท็บเล็ต ใช้ Backend และ API เดียวกับเว็บ ([Frontend Architecture](frontend-architecture.md), [Backend Architecture](backend-architecture.md)).

## กฎหลัก

กฎเดียวกับเว็บ แปลเป็นภาษา Dart:

| เว็บ (AGENTS.md) | แอป |
|---|---|
| ห้าม `any` / `as any` / `@ts-ignore` | ห้าม `dynamic` และ cast ไม่ตรวจ: `strict-casts`, `strict-inference`, `strict-raw-types` + `very_good_analysis` |
| Type จาก OpenAPI เท่านั้น | `lib/generated/api/` จาก openapi-generator (`dart-dio`) ห้ามเขียน DTO เอง |
| TanStack Query ถือ server state | Riverpod ถือ server state (`AsyncNotifier`/`FutureProvider`), ห้ามเก็บซ้ำใน widget state |
| ห้าม fetch ใน component | Widget เรียก provider เท่านั้น; provider เรียก repository; repository เรียก API client |
| i18n th/en ครบทุกคีย์ | ARB `app_th.arb` (ค่าเริ่มต้น) + `app_en.arb`, ตรวจ parity ใน CI |
| Central UI components | Widget กลางใน `lib/core/ui/` (`ErpButton`, `ErpTextField`, `ErpSelect`, …) ห้ามใช้ Material widget ดิบใน feature |
| ห้าม hex / radius ตายตัว | อ่านค่าจาก `Theme.of(context).extension<ErpTokens>()` เท่านั้น ([design.md หมวด 7](../../design.md)) |
| RFC 7807 → error มีโครงสร้าง | dio interceptor แปลง Problem Details เป็น `ApiFailure` (sealed class) ห้ามเทียบ string message |
| Backend คือ security authority | การซ่อนปุ่มตามสิทธิ์เป็น UX เท่านั้น |

## Tech Stack

| หน้าที่ | Library | หมายเหตุ |
|---|---|---|
| State + DI | `flutter_riverpod`, `riverpod_generator` | |
| Routing | `go_router`, `go_router_builder` | typed routes + deep link จาก push |
| HTTP | `dio` | interceptors: auth token, membership, `Accept-Language`, idempotency key, `If-Match`, Problem Details |
| API types | openapi-generator `dart-dio` | generate จาก `contracts/openapi/tan-erp.v1.json` |
| Models | `freezed`, `json_serializable` | immutable + sealed unions |
| Forms | `reactive_forms` | |
| i18n | `flutter_localizations`, `intl` (gen-l10n) | ICU format เหมือน next-intl |
| Offline DB | `drift` (SQLite) | cache อ่าน + outbox |
| Upload เบื้องหลัง | `background_downloader` | ทำงานต่อแม้ปิดแอป |
| Network state | `connectivity_plus` | |
| Auth | `firebase_core`, `firebase_auth` | อีเมล/รหัสผ่านก่อน |
| Secure storage | `flutter_secure_storage` | Keychain / Keystore |
| Biometric | `local_auth` | |
| Push | `firebase_messaging`, `flutter_local_notifications` | iOS ต้องมี APNs key (บัญชี Apple Developer) |
| Crash | `firebase_crashlytics` | |
| Log | `talker` | ห้าม log token/PII |
| กล้อง/รูป | `camera`, `image_picker`, `image` | ย่อ + ประทับข้อมูลบนรูป |
| บาร์โค้ด | `mobile_scanner` | |
| สแกนเอกสาร | `cunning_document_scanner` | |
| Bluetooth (เครื่องวัดเลเซอร์) | `flutter_blue_plus` | |
| ตำแหน่ง | `geolocator` | |
| Native module เขียนเอง | `pigeon` | RoomPlan (LiDAR), ARCore |
| ไอคอน | `flutter_svg` | SVG stroke ชุดเดียวกับเว็บ ห้าม icon pack |
| Test | `flutter_test`, `mocktail`, `integration_test` | |

## โครงสร้างโฟลเดอร์

```
mobile/
├─ lib/
│  ├─ main_dev.dart  main_staging.dart  main_prod.dart
│  ├─ app/                    ← MaterialApp.router, ProviderScope, bootstrap
│  ├─ core/
│  │  ├─ config/              ← AppConfig ต่อ flavor (apiBaseUrl, firebase options)
│  │  ├─ network/             ← dio, interceptors, ApiFailure
│  │  ├─ auth/                ← AuthRepository (interface) + Firebase impl, session, biometric gate
│  │  ├─ membership/          ← membership/branch ที่เลือก
│  │  ├─ offline/             ← drift database, outbox, sync engine, conflict model
│  │  ├─ theme/               ← tokens.g.dart (generated), ErpTokens ThemeExtension, buildTheme()
│  │  ├─ ui/                  ← widget กลาง (ErpButton, ErpTextField, ErpSelect, ErpAlert, EmptyState, MonoLoading)
│  │  ├─ l10n/                ← ARB + generated
│  │  └─ router/              ← go_router routes + guards
│  ├─ native/                 ← camera, scanner, laser_meter, location (interface + impl)
│  ├─ generated/api/          ← จาก OpenAPI (ห้ามแก้มือ)
│  └─ features/
│     └─ <feature>/
│        ├─ data/             ← repository impl, mappers, drift tables ของ feature
│        ├─ domain/           ← entities/value objects ที่ UI ใช้
│        └─ presentation/     ← providers, screens, widgets
├─ test/  integration_test/
├─ ios/  android/
└─ analysis_options.yaml
```

## ชั้นและทิศทางการพึ่งพา

```
presentation (screens/widgets) → providers (Riverpod) → repository (interface)
                                                         ↓
                                     data: API client (dio + generated) / drift / native
```

- Widget ไม่เรียก dio, drift หรือ plugin โดยตรง.
- Repository คืนค่าเป็น `Result<T, ApiFailure>` หรือ throw `ApiFailure` แบบเดียวกันทั้ง feature.
- ความสามารถของเครื่องอยู่หลัง interface ใน `lib/native/` เพื่อ mock ได้และสลับ implementation ต่อแพลตฟอร์ม.

## Environments (Flavors)

| Flavor | Bundle ID | API | Firebase | หมายเหตุ |
|---|---|---|---|---|
| dev | `com.tanerp.field.dev` | `http://<LAN IP ของ Mac>:<port>` | project dev | อนุญาต HTTP เฉพาะ flavor นี้ (iOS ATS exception, Android `usesCleartextTraffic`) |
| staging | `com.tanerp.field.staging` | `https://<staging domain>` | project staging | ยังไม่มีโดเมน — กำหนดเมื่อมี |
| prod | `com.tanerp.field` | `https://<prod domain>` | project prod | |

ค่าต่อ flavor ส่งผ่าน `--dart-define-from-file=config/<flavor>.json`; ไฟล์ที่มี secret ไม่ commit. ติดตั้งทั้งสาม flavor บนเครื่องเดียวกันได้.

## Auth และ Session

1. ล็อกอินอีเมล/รหัสผ่านด้วย Firebase → ได้ ID token.
2. เรียก `GET /api/v1/me` (current user) ตามแบบเว็บ แล้วเลือก membership/สาขา.
3. Refresh token เก็บใน secure storage; เปิดแอปครั้งต่อไปต้องผ่าน biometric (หรือ PIN ของเครื่อง) ก่อนปลด session.
4. Interceptor แนบ ID token ล่าสุดทุก request; 401 → refresh หนึ่งครั้ง แล้ว logout ถ้ายังล้มเหลว.

## Offline และ Sync

- **อ่าน:** งานที่ได้รับมอบหมาย, Site/Opportunity ที่เกี่ยวข้อง และ reference data ที่จำเป็น cache ใน drift พร้อมเวลาซิงก์ล่าสุด.
- **เขียน:** ทุกคำสั่งเขียนเข้า outbox (payload, idempotency key, expected rowVersion, ลำดับ) แล้ว sync engine ส่งตามลำดับเมื่อมีเน็ต; ไฟล์รูปอัปโหลดผ่าน `background_downloader` แล้วผูก attachment ตามขั้นตอนเดียวกับเว็บ (`useDeferredFileUpload`).
- **Conflict (412/409 rowVersion):** หยุดรายการนั้น แสดงจอเปรียบเทียบให้ผู้ใช้เลือกต่อฟิลด์ ไม่ overwrite เงียบ.
- **สถานะที่ผู้ใช้เห็นเสมอ:** แถบ ออฟไลน์ / กำลังซิงก์ / ค้าง N รายการ และหน้า "คิวซิงก์".

## Theme

ใช้ token ที่ build จาก `design-tokens/` เป็น `ErpTokens` (ThemeExtension) + `ThemeData` สำหรับ light/dark; preference ของผู้ใช้ (โหมด, ความแน่น, ขนาดตัวอักษร) โหลดจาก Backend และสีแบรนด์องค์กรมาจาก API. กติกาทั้งหมดอยู่ที่ [design.md หมวด 7](../../design.md).

## UX หลัก

มาตรฐานเต็ม (มุมคม Atelier, สีเน้นไม้โอ๊กสำหรับค่าวัด, กฎจัดหน้าจอ) อยู่ที่ [design.md หมวด 8](../../design.md).

- **มือถือ:** bottom navigation 5 เมนู — งานวันนี้ · สำรวจ · สแกน · แจ้งเตือน · ฉัน.
- **แท็บเล็ต (≥ 768dp):** navigation rail + master-detail split ตาม [design.md หมวด 2](../../design.md).
- ปุ่มกดสูง ≥ 44dp, safe area ทุกจอ, focus/semantics label ครบสำหรับ screen reader, รองรับ text scale ของระบบ.
- ฟอร์มใช้กฎเดียวกับเว็บ: double-submit protection, confirmation ก่อนลบ/ยกเลิก, deferred upload, placeholder จาก i18n.

## Flow หลัก

1. **เข้าใช้งาน:** ล็อกอิน → biometric (ครั้งถัดไป) → เลือก membership/สาขา.
2. **งานวันนี้:** รายการนัดสำรวจที่ได้รับมอบหมาย (ออฟไลน์ได้) → นำทาง/โทร → เช็กอินหน้างาน.
3. **สำรวจหน้างาน:** Opportunity → ห้อง (Area) → รายการ → วัด (เอง / เลเซอร์ / จากรูป / LiDAR) → รูป → บันทึกลงเครื่อง → sync → จัดการ conflict ถ้ามี.
4. **ประเมินราคาด่วน:** จากผลสำรวจ → Quick Estimate → แชร์ลูกค้า.
5. **สแกน:** รับของ / ติดตามชิ้นงาน ด้วยบาร์โค้ด/QR.
6. **แจ้งเตือน:** push → deep link → เปิดเอกสาร → อนุมัติ/ส่งกลับ.

## สิ่งที่ Backend ต้องเพิ่ม

| รายการ | เฟส |
|---|---|
| User preferences (โหมดธีม, ความแน่น, ขนาดตัวอักษร) ต่อจาก `PreferredLocale` | 0 |
| ตรวจเวอร์ชันแอปขั้นต่ำ (`GET /api/v1/app/version-policy`) | 0 |
| ลงทะเบียนอุปกรณ์สำหรับ push (token ต่อ user/membership/platform) + ส่ง push จาก notification module ([ADR 0018](../adr/0018-in-app-notification-foundation.md)) | 1 |
| สีแบรนด์/โลโก้องค์กร + validation contrast | 1 |
| Delta/sync endpoint ของงานที่ได้รับมอบหมาย | 2 |

## Release

- Build และ sign บน Mac ด้วย Fastlane; Android ส่ง Play Internal Testing, iOS ส่ง TestFlight (ต้องมีบัญชี Apple Developer).
- เวอร์ชัน `MAJOR.MINOR.PATCH+BUILD`; breaking change ของ API ต้องเพิ่มเวอร์ชันขั้นต่ำใน version policy.
- Crashlytics แยก project ต่อ flavor.
