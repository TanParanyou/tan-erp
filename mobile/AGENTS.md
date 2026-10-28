# TAN Field (mobile) — Flutter Agent Guide

คู่มือและกฎระเบียบเชิงสถาปัตยกรรมสำหรับการพัฒนาแอปพลิเคชันมือถือภาคสนามด้วย **Flutter** ในโปรเจกต์ **tan-erp**  
อ้างอิงสถาปัตยกรรมหลัก: [`docs/02-architecture/mobile-architecture.md`](../docs/02-architecture/mobile-architecture.md)  
ระบบการออกแบบ: [`design.md`](../design.md) (หมวด 7: Atelier Architectural Navy Sharp)

---

## 1. Mandatory Pre-work & Development Rules (กฎเหล็กก่อนเริ่มงาน)

> ⚠️ **กฎเหล็กที่ต้องปฏิบัติตามอย่างเคร่งครัด (Non-Negotiable Rules):**
> 1. **ตอบกลับเป็นภาษาไทยเสมอ:** ตอบกลับผู้ใช้เป็นภาษาไทยทุกครั้งโดยไม่มีข้อยกเว้น ส่วน Code Comments, Identifiers, Classes และ Functions ให้ใช้ภาษาอังกฤษตามมาตรฐานสากล
> 2. **ก่อนลงมือทำให้อ่านเอกสารที่เกี่ยวข้องเสมอ:** อ่านไฟล์นี้, `design.md` และ `docs/02-architecture/mobile-architecture.md` เพื่อเข้าใจข้อจำกัดทางสถาปัตยกรรมและ Design Tokens
> 3. **Strict Scope & Minimal Blast Radius:** แก้ไขเฉพาะไฟล์และฟีเจอร์ที่ได้รับมอบหมายโดยตรง ห้าม refactor, จัดระเบียบ หรือเปลี่ยนสไตล์โค้ดในจุดที่ไม่ได้รับคำสั่ง
> 4. **ไม่ชัด ให้ถามก่อน ห้ามเดา (Clarify Before Acting):** หากข้อกำหนดมีความคลุมเครือ ให้สอบถามผู้ใช้ทันที ห้ามอนุมานเอาเอง
> 5. **ตรวจก่อน แล้วรายงานหลักฐาน (Verify & Report Evidence):** ก่อนส่งงานทุกครั้ง ต้องรัน Verification Gates (analyze, custom_lint, l10n parity, test) และรายงานผลลัพธ์พร้อมหลักฐานคำสั่งเสมอ
> 6. **Strict Dart Types & Zero `dynamic`:** ห้ามใช้ `dynamic` หรือ loose cast (`as`) โดยเด็ดขาด บังคับเปิด `strict-casts`, `strict-inference`, `strict-raw-types` ควบคู่กับ `very_good_analysis`
> 7. **OpenAPI Single Source of Truth:** ห้ามเขียน DTO หรือ Request/Response payload ขึ้นมาเองเด็ดขาด ทุกโมเดลต้องมาจาก `packages/tan_erp_api` ที่ generate ผ่าน `tool/generate_api.sh`
> 8. **ห้าม Hardcode ข้อความบน UI:** ข้อความทั้งหมดต้องดึงผ่านระบบแปลภาษา (`context.l10n`) และต้องเพิ่มคู่แปลภาษาให้ครบทั้ง `app_th.arb` (ค่าเริ่มต้น) และ `app_en.arb` เสมอ
> 9. **ห้าม Hardcode สีและ Border Radius:** ห้ามเขียน `Color(0x...)`, `Colors.xxx` หรือ `BorderRadius.circular(...)` นอกเหนือจาก `lib/core/theme/` (มี `custom_lint` ตรวจจับและบล็อกการบิลด์) ต้องอ่านผ่าน `context.tokens` เสมอ
> 10. **Central UI Components First:** ห้ามใช้ Material Widget ดิบ (`ElevatedButton`, `TextField`, `Card` ฯลฯ) ใน Feature Screens ให้ใช้คอมโพเนนต์กลางจาก `lib/core/ui/` เท่านั้น

---

## 2. Architecture & Directory Layout (Feature-First Clean Architecture)

โปรเจกต์ใช้สถาปัตยกรรม **Feature-First Clean Architecture** โดยแยกส่วนโค้ดตามขอบเขตธุรกิจและระดับชั้นความรับผิดชอบ:

```
mobile/
├─ lib/
│  ├─ main_dev.dart / main_staging.dart / main_prod.dart   ← Entrypoints แยกตาม Flavor
│  ├─ app/                     ← Application Bootstrap, MaterialApp.router, ProviderScope
│  ├─ core/                    ← โครงสร้างพื้นฐานส่วนกลางที่แชร์ทั้งแอป
│  │  ├─ auth/                 ← Authentication, Session management, Biometric gate
│  │  ├─ config/               ← AppConfig (apiBaseUrl, firebase options) ต่อ Flavor
│  │  ├─ l10n/                 ← ARB localization files (app_th.arb, app_en.arb) & Gen
│  │  ├─ membership/           ← Active Organization/Branch context
│  │  ├─ network/              ← Dio instance, Central ApiClient, Interceptors, ApiFailure
│  │  ├─ offline/              ← Drift database, Sync engine, Outbox
│  │  ├─ router/               ← GoRouter configuration, Route definitions, Guards
│  │  ├─ theme/                ← Design tokens (tokens.g.dart), ErpTokens, AppTheme
│  │  └─ ui/                   ← Central Reusable Widgets (ErpButton, ErpTextField, ...)
│  ├─ native/                  ← Hardware/Platform Abstractions (Camera, Scanner, Location)
│  └─ features/                ← Business Features
│     └─ <feature_name>/
│        ├─ data/              ← Repository implementations, Drift tables, Mappers
│        ├─ domain/            ← Pure Domain models, Value objects, Repository interfaces
│        └─ presentation/      ← Riverpod providers, Screens, Feature-specific widgets
├─ packages/
│  ├─ erp_lints/               ← Custom Analyzer Lints (ตรวจจับ hardcoded colors & radius)
│  └─ tan_erp_api/             ← Generated OpenAPI Client (Read-only, ห้ามแก้มือ)
├─ pigeons/                    ← Type-safe Native Pigeon contracts (LiDAR, ARCore)
├─ tool/                       ← Development & Automation Scripts
└─ analysis_options.yaml       ← Analyzer configuration
```

### ทิศทางการพึ่งพา (One-Way Dependency Rule)
```
Presentation (Screens / Widgets)
       ↓ (watches/reads)
Riverpod Providers / Notifiers
       ↓ (calls)
Repository (Domain Interface)
       ↓ (implemented by)
Data Layer (Data Source / ApiClient / Drift / Native Interface)
```

- **กฎเหล็กของ Layer:**
  - `presentation` ห้ามเรียก `Dio`, `ApiClient`, `Drift` หรือ Platform plugins โดยตรงเด็ดขาด ต้องผ่าน Provider และ Repository
  - `domain` ต้องเป็น Pure Dart ห้ามขึ้นต่อ Flutter UI (`package:flutter`) หรือ External infrastructure packages
  - Repository Interface ต้องประกาศใน `domain/` และ Implement ใน `data/`

---

## 3. Strict Dart & Type Safety Standards

- **Compiler Strict Modes:** บังคับใช้ค่าต่อไปนี้ใน `analysis_options.yaml`:
  ```yaml
  analyzer:
    language:
      strict-casts: true
      strict-inference: true
      strict-raw-types: true
  ```
- **Zero `dynamic` & Safe Narrowing:**
  - ห้ามใช้ Type `dynamic` เด็ดขาด หากไม่ทราบโครงสร้างข้อมูล ให้ใช้ `Object?` แล้ว narrow ด้วย `is`
  - ห้าม cast สุ่มสี่สุ่มห้าด้วย `as` หากไม่ได้ตรวจสอบ type ด้วย `is` มาก่อน
- **Exhaustive Pattern Matching:**
  - ใช้ Dart 3 Switch Expressions (`return switch (failure) { ... };`) สำหรับ Sealed Classes (`ApiFailure`) และ Enums เสมอ
  - ห้ามใส่ wildcard fallback `_ =>` ใน switch ของ sealed class โดยไม่จำเป็น เพื่อให้คอมไพเลอร์บังคับเตือนเมื่อมีการเพิ่ม subtype ใหม่
- **Immutability First:**
  - ประกาศ Domain Entities และ Models เป็น `@immutable` เสมอ
  - ใส่ `const` constructors ให้กับ Widgets และ Classes ทุกจุดที่เป็นไปได้ เพื่อเพิ่มประสิทธิภาพ Flutter Element Tree Rebuilds

---

## 4. State Management with Riverpod (3.x Best Practices)

- **Server State vs UI Ephemeral State:**
  - **Server State (Remote/Async Data):** ให้ Riverpod เป็นเจ้าของ (`AsyncNotifier` หรือ `FutureProvider`) เท่านั้น ห้ามนำไปเก็บซ้ำใน `StatefulWidget`
  - **Ephemeral State (Local UI Only):** อนุญาตให้ใช้ `StatefulWidget` หรือ `flutter_hooks` เฉพาะสถานะชั่วคราวของ UI เท่านั้น เช่น `AnimationController`, `ScrollController`
- **การใช้งาน `ref` ใน Presentation:**
  - `ref.watch`: ใช้ภายใน `build()` method ของ ConsumerWidget เพื่อ re-render เฉพาะส่วนที่จำเป็น
  - `ref.read`: ใช้ใน Event Callbacks (เช่น `onPressed: () => ref.read(provider.notifier).submit()`) **ห้ามใช้ `ref.read` ภายใน `build()` เด็ดขาด**
  - `ref.listen`: ใช้สำหรับ Side-Effects (เช่น แสดง SnackBar แจ้งเตือน, เปิด Modal หรือ Navigate ไปหน้าอื่นเมื่อ state เปลี่ยน)
  - ป้องกัน Rebuild ฟุ่มเฟือย: ใช้ `ref.watch(provider.select((s) => s.specificField))` เมื่อสนใจเฉพาะบางฟิลด์ของ State ก้อนใหญ่
- **Async Handling Standard:**
  - จัดการ State ข้อมูล Remote ผ่าน `AsyncValue` ให้ครบทั้ง 3 มิติเสมอ:
    ```dart
    final asyncData = ref.watch(myFeatureProvider);
    return asyncData.when(
      data: (data) => FeatureContent(data: data),
      loading: () => const Center(child: ErpMonoLoading()),
      error: (error, stack) => ErpAlert.error(message: FailureMessages.of(context, error)),
    );
    ```

---

## 5. Atelier Architectural Navy Sharp Design System & UI Standards

แอปพลิเคชันยึดถือมาตรฐานการออกแบบ **Atelier Architectural Navy Sharp** ตาม [`design.md`](../design.md) หมวด 7:

### 5.1 Design Tokens First (No Hardcoded Styles)
- เข้าถึง Tokens ผ่าน `context.tokens` (`Theme.of(context).extension<ErpTokens>()!`) เสมอ:
  - `t.action`: Solid Navy Primary Color (`#0B3056`)
  - `t.onAction`: Foreground text/icon บน action color
  - `t.surface` / `t.surfaceSubtle`: Background colors
  - `t.border` / `t.borderStrong`: Stroke border colors
  - `t.corner`: **0px Sharp Radius** (ทรงคมเหลี่ยม ปราศจากความโค้งมน)
- **Custom Lint Protection:** ระบบมี `packages/erp_lints` ตรวจจับและบล็อกการใช้ `Color(...)` และ `BorderRadius.circular(...)` นอกเหนือจาก `lib/core/theme/` หากพบจะคอมไพล์ไม่ผ่าน

### 5.2 Ergonomics & Touch Targets
- **Interactive Tap Target:** ช่องทางปฏิสัมพันธ์ทุกประเภทต้องมีขนาดขั้นต่ำ **48dp** (`t.tapTarget`) ตามมาตรฐาน WCAG และ Mobile Ergonomics
- **Primary Action Height:** ปุ่ม Primary ประจำหน้าจอต้องมีขนาด **52dp** (`t.primaryActionHeight`) และแสดงผลเต็มความกว้างจอ (`expand: true`)

### 5.3 Central UI Components First
ห้ามสร้างหรือใช้ Material UI Widget ดิบใน Feature ให้ใช้คอมโพเนนต์กลางจาก `lib/core/ui/`:

| Component | การใช้งาน |
|---|---|
| `ErpButton` | ปุ่มมาตรฐาน มี 3 variants (`primary`, `outline`, `ghost`) รองรับ `isLoading` ในตัว |
| `ErpTextField` | ช่องกรอกข้อความผูกกับ `reactive_forms` รองรับ Label, Placeholder และ Validation |
| `ErpSelect` | Dropdown เลือกลิสต์ข้อมูล |
| `ErpCard` | การ์ดคอนเทนเนอร์แสดงข้อมูล ขอบคม 0px สอดคล้องกับ token |
| `ErpAlert` | แถบแจ้งเตือนสถานะความสำเร็จ ข้อผิดพลาด หรือคำเตือน |
| `ErpConfirmDialog` | กล่องยืนยันก่อนทำ Action อันตราย (ลบข้อมูล/ยกเลิก) |
| `ErpMonoLoading` | ตัวโหลดกึ่งกลางแบบ Monochrome เรียบหรู (ห้ามใช้ Skeleton Loader) |
| `ErpEmptyState` | หน้าแสดงสถานะว่างเมื่อไม่มีข้อมูล พร้อมไอคอนและปุ่ม Action |
| `ErpScreen` | โครงสร้างหน้าจอหลัก จัดการ SafeArea, Background และ Keyboard Insets |
| `ErpIcon` | ตัวเรนเดอร์ Pure SVG Stroke Icons จาก `assets/icons/` (ห้ามใช้ Icon Pack ภายนอก) |

### 5.4 Double-Submit Protection & Safety
- ทุกปุ่มกดยืนยันหรือส่งข้อมูล Asynchronous ต้องส่ง `isLoading: state.isLoading` เข้าไปที่ `ErpButton` เพื่อปิดการกดซ้ำ (Disable) ทันทีระหว่างการยิง API
- ปฏิบัติการที่มีความเสี่ยงสูง (เช่น การลบเอกสาร, การยกเลิกรายการ) ต้องครอบด้วย `ErpConfirmDialog` และปุ่มยืนยันต้องล็อกสถานะ `isLoading` ระหว่างยิง Request

---

## 6. Form Handling with `reactive_forms`

- ทุกฟอร์มที่มีการบันทึกข้อมูลและตรวจสอบความถูกต้อง (Validation) ต้องใช้แพ็กเกจ `reactive_forms`
- กำหนด Schema ฟอร์มด้วย `FormGroup` และ `FormControl`:
  ```dart
  final form = fb.group({
    'title': FormControl<String>(validators: [Validators.required]),
    'quantity': FormControl<int>(validators: [Validators.required, Validators.min(1)]),
  });
  ```
- ผูกช่องกรอกผ่าน `ErpTextField` หรือ `ErpSelect`
- ข้อความ Error จาก Validation ต้องมาจาก l10n เสมอ โดยส่งผ่าน `validationMessages`:
  ```dart
  validationMessages: {
    ValidationMessage.required: (error) => context.l10n.fieldRequired,
  }
  ```

---

## 7. Network, DTOs & RFC 7807 Error Architecture

### 7.1 Central ApiClient & OpenAPI
- ยิง API ผ่าน `ApiClient` (`ref.read(apiClientProvider)`) เสมอ:
  ```dart
  final result = await ref.read(apiClientProvider).call(
    () => api.getOpportunitiesApi().getOpportunityById(id: id),
  );
  ```
- ห้ามเขียน Model หรือ DTO เลียนแบบ Backend ให้ใช้คลาสจาก package `tan_erp_api` เท่านั้น
- Interceptors จะจัดการแนบ Bearer Token (`AuthInterceptor`), Active Membership (`ContextInterceptor`) และภาษา `Accept-Language` ให้อัตโนมัติ

### 7.2 Structured `ApiFailure` (RFC 7807 Problem Details)
- ปัญหาจาก Network หรือ Backend HTTP Status จะถูกแปลงเป็น `ApiFailure` Sealed Hierarchy:
  - `ValidationFailure` (400/422): มี `fieldErrors` สำหรับผูกกับฟิลด์ในฟอร์ม
  - `UnauthorizedFailure` (401): เซสชันหมดอายุ
  - `ForbiddenFailure` (403): ไม่มีสิทธิ์เข้าถึง Resource
  - `NotFoundFailure` (404): ไม่พบข้อมูลในระบบ
  - `ConflictFailure` (409/412): ข้อมูลขัดแย้งหรือ Concurrency conflict
  - `UnknownFailure`: ข้อผิดพลาดอื่นๆ
- **กฎเหล็ก:** ห้ามเทียบ error ด้วย String message เช่น `if (e.toString().contains("404"))` เด็ดขาด! ให้ใช้ Exhaustive Pattern Matching บน `ApiFailure` และ map สตริงแสดงผลผ่าน `FailureMessages.of(context, failure)` ใน `lib/core/l10n/failure_messages.dart`

---

## 8. Localization & i18n Parity (th / en)

- แอปพลิเคชันรองรับสองภาษา โดยมี **ภาษาไทย (`th`) เป็นค่าเริ่มต้น** และมี **ภาษาอังกฤษ (`en`) เป็นคู่เทียบเสมอ**
- ไฟล์ข้อความอยู่ที่:
  - `lib/core/l10n/app_th.arb`
  - `lib/core/l10n/app_en.arb`
- เข้าถึงข้อความใน Widget ด้วย `context.l10n.<keyName>`
- **กฎ Parity:** เมื่อเพิ่มหรือลบคีย์ใน `app_th.arb` ต้องเพิ่มหรือลบคีย์ที่ตรงกันใน `app_en.arb` ด้วยเสมอ และต้องรันคำสั่งตรวจสอบ:
  ```bash
  dart run tool/check_l10n_parity.dart
  ```

---

## 9. Hardware & Platform Device Abstraction

- อุปกรณ์ฮาร์ดแวร์และการทำงานระดับ Native (กล้อง, สแกนบาร์โค้ด, เซนเซอร์ระยะเลเซอร์ Bluetooth, GPS, ARCore/LiDAR) ต้องถูกหุ้มด้วย Interface ใน `lib/native/`
- ห้ามเรียก Native Plugin จาก Presentation Layer ตรงๆ เพื่อรักษาความสามารถในการทำ Unit Testing และการสลับ Mock Implementation บน Test/Simulator
- การส่งผ่านข้อมูลประสิทธิภาพสูงระหว่าง Flutter และ Native ให้ใช้ **Pigeon** (`pigeons/*.dart`) เพื่อสร้าง Type-safe Platform Channels

---

## 10. Multi-Flavor Configuration & Mock Auth

- การรันแอปแบ่งเป็น 3 Flavors ชัดเจน:
  - `dev` (`com.tanerp.field.dev`): ใช้ทดสอบในเครื่อง อนุญาตให้ยิง HTTP Cleartext บนเครือข่าย LAN ได้
  - `staging` (`com.tanerp.field.staging`): ใช้ทดสอบ UAT
  - `prod` (`com.tanerp.field`): Production
- การส่งค่าคอนฟิกให้ใช้ `--dart-define-from-file=config/<flavor>.json` และอ่านผ่าน `appConfigProvider`
- **Mock Login:** ใน `dev` flavor สามารถตั้งค่า `"MOCK_AUTH": true` ใน `config/dev.json` เพื่อทดสอบระบบโดยไม่ต้องเชื่อมต่อ Firebase Auth จริงได้ (ระบบจะส่ง `dev-uid:<uid>` ให้กับ Backend ที่เปิด `DevelopmentTokens`)

---

## 11. Definition of Done & Verification Gates

งานพัฒนาฝั่ง Flutter จะถือว่า **เสร็จสมบูรณ์ (Done)** ได้ต่อเมื่อผ่านขั้นตอนการตรวจสอบและมีหลักฐานครบถ้วน:

1. โค้ดคอมไพล์ผ่านและไม่มีคำเตือนจาก Analyzer:
   ```bash
   cd mobile && flutter analyze
   ```
2. ผ่านการตรวจสอบ Custom Lint ด้าน Token และ Design System 100%:
   ```bash
   cd mobile && dart run custom_lint
   ```
3. คีย์แปลภาษา (i18n) มีความเท่าเทียมกันทั้งภาษาไทยและอังกฤษ:
   ```bash
   cd mobile && dart run tool/check_l10n_parity.dart
   ```
4. Automated Tests ทั้งหมดผ่าน:
   ```bash
   cd mobile && flutter test
   ```
5. หากมีการแก้ไข OpenAPI Contract หรือ Model:
   ```bash
   cd mobile && ./tool/generate_api.sh
   ```

> ⚠️ **คำเตือน:** ห้ามกล่าวอ้างว่า "งานเรียบร้อยแล้ว" หรือ "โค้ดผ่านแล้ว" ในการตอบกลับ หากยังไม่ได้รันคำสั่งตรวจสอบข้างต้นและยืนยันผลลัพธ์จริง
