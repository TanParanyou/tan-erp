---
status: accepted
---

> สถานะ implementation: Implemented (no tests written, none run; browser not checked) — 2026-10-17. ดู [Verification](../05-engineering/survey-templates-db-verification.md)

# Survey Template ใน Database พร้อม Effective Period และ Evidence ต่อ Area

Survey Template เดิม (`SURVEY-BASELINE-v1/v2`) เป็น code registry; เพิ่มข้อกำหนดใหม่ต้องแก้โค้ดและ deploy. ADR นี้ย้าย template ลงตาราง `crm.survey_template_versions` โดย hash ของ Survey เดิมต้องไม่เปลี่ยน.

## ตัดสินใจ

1. ตาราง global (ไม่แยก Organization) `crm.survey_template_versions`: `code` (unique), `required_checklist_items text[]`, `minimum_evidence_count`, `minimum_area_evidence_count`, `snapshot_hash_version` (`v2|v3|v4`), `effective_from_utc`, `effective_to_utc`. Migration seed `v1/v2` ด้วยค่าเดียวกับ `SurveyTemplates.BaselineV1/V2`.
2. **Hash ขึ้นกับ `snapshot_hash_version` ของ template เท่านั้น** จึงไม่เปลี่ยนเมื่อย้ายแหล่งข้อมูล; payload `v2`/`v3` ไม่ถูกแตะ. `v4` = payload `v3` + `areaEvidence` (ต่อ Area: fileId, contentSha256, purpose เรียงตาม fileId).
3. Immutable: trigger ใน DB ห้ามแก้ทุกคอลัมน์ ยกเว้น `effective_to_utc` เปลี่ยนจาก NULL เป็นค่าได้ครั้งเดียว และห้ามลบแถว. API ไม่มี endpoint แก้เนื้อหา.
4. Survey ใหม่ใช้ template ที่อยู่ในช่วง effective ณ ปัจจุบันและ `effective_from_utc` ล่าสุด; ถ้าไม่มี → `SURVEY_TEMPLATE_NOT_EFFECTIVE` (409). Survey/Revision เดิมคง `survey_template_version` เดิมตลอด (Clone ก็คง template ของต้นทาง แม้หมดช่วง effective).
5. Evidence ต่อ Area ใช้ไฟล์แนบกลาง G-01: owner type `survey-area` (read `surveys.read`, manage `surveys.update`, แก้ได้เฉพาะ Revision สถานะ `draft` และ template ที่ hash `v4`). Template `v4` ที่ตั้ง `minimum_area_evidence_count > 0` บังคับทุก Area ตอน Mark Ready.
6. Admin surface = API เท่านั้น: `GET/POST /api/v1/survey-template-versions`, `POST …/{code}/retire` (retire = ตั้ง `effective_to_utc` = ตอนนี้). Publish/Retire ใช้ permission ใหม่ `survey-templates.manage` (อ่านยังใช้ `surveys.read`).

## วิธีเปลี่ยนในภายหลัง

- **เพิ่มข้อกำหนดตรวจหน้างาน/จำนวน evidence:** `POST /survey-template-versions` ด้วย code ใหม่ (เช่น `SURVEY-BASELINE-v3`), `snapshotHashVersion: "v4"`. checklist item ใหม่ต้องเพิ่มคำแปล `surveys.checklistItems.<code>` ใน `messages/th.json` + `en.json` ก่อน publish. Survey เดิมไม่เปลี่ยน; ใช้ `retire` กับ template เก่าเมื่อต้องการให้ Survey ใหม่ใช้ตัวใหม่ (หรือรอให้ตัวใหม่มี effective_from ล่าสุด).
- **เพิ่ม field ที่ต้องอยู่ใน hash:** เพิ่ม hash version ใหม่ (`v5`) ใน `SurveySnapshotHasher` + `SurveyTemplateVersionRow` + CHECK constraint ผ่าน migration; ห้ามเปลี่ยน payload ของ version ที่ใช้แล้ว.
- **แยกตามองค์กร:** เพิ่ม `organization_id` nullable (NULL = ระบบ) แล้วกรองตอนเลือก template.

## ข้อเสียที่ยอมรับ

ผู้ถือ `survey-templates.manage` กระทบทุก Organization (ตารางเป็น global); ไม่มี UI สำหรับ Publish/Retire/ดูรายการ; evidence ของ Area ไม่ถูกคัดลอกตอน Clone Revision และลิงก์ของ Area ที่ถูกลบจากร่างจะค้างโดยไม่ถูกนับใน hash; Clone ไม่ถูกบล็อกด้วย template ที่หมดช่วง effective.
