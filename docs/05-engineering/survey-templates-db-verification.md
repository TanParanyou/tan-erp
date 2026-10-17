# Survey Templates in DB Verification (G-07)

## 1. สถานะ

**Implemented 2026-10-17 (G-07) — no tests written, none run (per user); browser not checked; ยังไม่ผ่าน UAT.** Migration `AddSurveyTemplateVersions` สร้างแล้วแต่ **ยังไม่ได้ apply** กับ DB ใด ๆ.

เอกสารที่เกี่ยวข้อง: [ADR 0028](../adr/0028-survey-templates-in-db.md), [Site Survey Verification](site-survey-verification.md), [Permission Catalog](../03-contracts/permission-catalog.md), [Error Contract](../03-contracts/error-contract.md)

## 2. ผลตรวจที่ทำ

| รายการ | ผล |
|---|---|
| `dotnet build` | 0 warnings / 0 errors |
| `npx tsc --noEmit`, `npm run lint`, `npm run build` | ผ่าน |
| OpenAPI regenerate + `npm run check:api` | ผ่าน (diff เป็นการเพิ่มล้วน) |
| Hash parity (สคริปต์ throwaway ครั้งเดียว ไม่ commit) | ดูหัวข้อ 3 |

## 3. Hash parity ของ seed v1/v2

สคริปต์ parse ค่า seed จาก SQL ใน migration แล้วสร้าง Revision ตัวอย่างต่อ template, คำนวณ hash 3 แบบ: (ก) `SurveySnapshotHasher` ของ `HEAD` ก่อนแก้, (ข) hasher ใหม่ด้วย code registry, (ค) hasher ใหม่ด้วย template ที่สร้างจากค่า seed ของ DB. ผล: `SURVEY-BASELINE-v1` ได้ prefix `v2` และ `SURVEY-BASELINE-v2` ได้ prefix `v3` ทั้งสามแบบ hash เท่ากันทุกตัว (`PARITY OK`) และค่า checklist/minimum evidence/hash version ของ seed ตรงกับ code ทุกฟิลด์. ไม่ได้ทดสอบกับข้อมูลจริงใน DB (ยังไม่ apply migration).

## 4. ที่ยังไม่ได้ตรวจ / เปิดอยู่

- ทดสอบทุกชุด (ทั้งใหม่และเดิม) ไม่ได้รัน; v4 hash และ area evidence ไม่เคยถูกรันจริง
- ไม่มี seed ของ template ที่ hash `v4` — ต้อง `POST /api/v1/survey-template-versions` เอง; ไม่มี FE สำหรับ Publish/Retire/ดูรายการ
- ตัดสินใจรอเจ้าของ: ตารางเป็น global หรือแยกต่อ Organization; ใครควรถือ `survey-templates.manage`
- ลิงก์ evidence ของ Area ที่ถูกลบ/Clone ไม่ถูกคัดลอกหรือล้าง
