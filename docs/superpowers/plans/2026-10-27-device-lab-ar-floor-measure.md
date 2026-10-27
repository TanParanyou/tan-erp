# Device Lab: AR Floor Measure (Android, ARCore)

- Status: **Approved by user 2026-10-27** ("อยากลองเล่น feat ความสามารถเฉพาะเครื่องก่อน" → เลือก AR วัดพื้น, ใช้ emulator ไปก่อน)
- Scope: dev-only "ห้องทดลอง" (Device Lab) ในแอป `mobile/` — **Android เท่านั้น**; iOS พักไว้ตามผู้ใช้ (มีแค่ stub ให้ compile ได้)
- ไม่แตะ Backend / เว็บ; ไม่บันทึกผลลง Survey (ทดลองเท่านั้น); ไม่เขียนและไม่รัน test (ตามผู้ใช้)
- Related: [Mobile Architecture](../../02-architecture/mobile-architecture.md), [design.md หมวด 8](../../../design.md), [ADR 0030](../../adr/0030-flutter-field-companion-app.md)

## Why native instead of a plugin

ปลั๊กอิน AR ของ Flutter บน pub.dev ทุกตัว (ar_flutter_plugin*, arcore_flutter_plugin) อัปเดตล่าสุด 2022–2025, จำกัด Dart < 3 และพึ่ง Sceneform ที่ Google เลิกพัฒนา → ใช้ไม่ได้กับ Flutter 3.47. เขียน native module เองด้วย ARCore SDK (`com.google.ar:core`) ตาม ADR 0030 ("LiDAR/ARCore: native module ผ่าน Pigeon").

## Design

- **Pigeon API `ArMeasureApi`** (Dart ↔ Kotlin; Swift stub คืน unsupported):
  - `checkAvailability() -> ArAvailability` (supported / needsInstall / unsupported / unknown)
  - `measureFloor(ArMeasureRequest) -> ArMeasureResult?` (null = ผู้ใช้ยกเลิก)
  - Request: ข้อความ UI ทั้งหมด (มาจาก l10n ของ Flutter) + สีจาก `ErpTokens` (ARGB int) — native ไม่มีข้อความ/สีของตัวเอง
  - Result: `areaSqm`, `perimeterM`, `edgeLengthsM: List<double>`, `vertexCount`
- **Native `ArMeasureActivity`** (เปิดเป็น Activity แยก ไม่ใช้ PlatformView):
  - `GLSurfaceView` วาดภาพกล้องของ ARCore (shader พื้นหลังขั้นต่ำ)
  - View โปร่งใสด้านบน (Android Canvas) วาด reticle, จุด, เส้น, ความยาวต่อด้าน และพื้นที่สด โดย project anchor 3D → จอทุกเฟรม
  - Flow: หาพื้นแนวนอน → แตะเพื่อวางจุด (hit-test บน plane) → ปุ่ม ย้อนกลับ / ปิดรูป / เสร็จ → ส่งผลกลับ
  - พื้นที่: แปลงจุดเป็นพิกัดในระนาบ (x, z ของ plane pose) แล้ว shoelace; ปฏิเสธรูปที่เส้นตัดกัน
  - รูปทรงคม 0px, ปุ่ม ≥ 48dp, สีจาก token เท่านั้น
- **Manifest:** `CAMERA` permission, `com.google.ar.core` meta-data = `optional` (แอปยังติดตั้งได้บนเครื่องที่ไม่รองรับ AR), minSdk ≥ 24
- **Flutter Device Lab** (`lib/features/device_lab/`, flavor dev เท่านั้น):
  - เข้าได้จากจอ "ยังไม่ได้ตั้งค่า Firebase" และจากหน้า "ฉัน" (เฉพาะ dev)
  - การ์ด "AR วัดพื้น": แสดงสถานะ availability, ขอสิทธิ์กล้อง, ปุ่มเริ่ม, การ์ดผลลัพธ์ (พื้นที่ตัวใหญ่ตาม design.md 8.2, เส้นรอบรูป, ความยาวแต่ละด้าน)
  - ข้อความทั้งหมดใน `app_th.arb` + `app_en.arb`

## Known limits

- ARCore บน emulator arm64 ไม่อยู่ในรายชื่อที่ Google รองรับอย่างเป็นทางการ (มี build emulator เฉพาะ x86); ติดตั้ง ARCore 1.57 (arm64, ลงนามโดย Google) บน emulator แล้ว — ถ้า `checkAvailability` คืน unsupported ให้หน้าจอบอกเหตุผลชัดเจน และทดสอบจริงต้องใช้มือถือที่รองรับ ARCore
- ความแม่นยำขึ้นกับการตรวจจับพื้นของ ARCore (~1–5%)

## Verification

`flutter analyze` 0 issues, `dart run custom_lint`, l10n parity, `flutter build apk --flavor dev -t lib/main_dev.dart --debug --dart-define-from-file=config/dev.json`, ติดตั้งบน emulator แล้วเปิด Device Lab → ดู availability และ (ถ้ารองรับ) ลองวัดใน virtual scene
