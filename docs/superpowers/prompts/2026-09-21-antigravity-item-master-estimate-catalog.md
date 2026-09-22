# Antigravity Prompt — Item Master Estimate Catalog Foundation

คัดลอกข้อความระหว่างเส้นคั่นไปให้ Antigravity โดยเริ่มจาก repository root ของ `tan-erp`

---

คุณกำลังพัฒนา Project ERP (`tan-erp`) ให้ทำ Item Master Estimate Catalog Foundation ตามแผนที่อนุมัติแล้ว ทำงานแบบ checkpoint-driven และหยุดให้ผู้ใช้ตรวจเมื่อจบแต่ละ checkpoint ห้ามขยายขอบเขตเอง

ก่อนแก้ไฟล์:

1. อ่าน `AGENTS.md`, `design.md`, `CONTEXT.md` และ `docs/README.md` ทั้งหมด
2. อ่านแผน `docs/superpowers/plans/2026-09-21-item-master-estimate-catalog-foundation.md` ทั้งหมด
3. อ่านเอกสาร authoritative ต่อไปนี้ทั้งหมด:
   - `docs/superpowers/specs/2026-09-06-item-master-cost-foundation-design.md`
   - `docs/01-business/item-master-flow.md`
   - `docs/01-business/item-master-field-catalog.md`
   - `docs/01-business/item-master-governance.md`
   - `docs/03-contracts/item-master-api-contract.md`
   - `docs/04-data/item-master-data-contract.md`
   - `docs/05-engineering/item-master-uat-scenarios.md`
   - `docs/adr/0008-typed-items-versioned-cost-records.md`
   - `docs/adr/0012-localized-jsonb-for-item-text.md`
   - `docs/adr/0013-organization-items-with-branch-availability.md`
4. อ่าน `.agents/skills/building-erp-apis/SKILL.md`; เมื่อเริ่มแก้ React ให้อ่าน skill ที่ repository กำหนดสำหรับ React/UI ก่อนลงมือ
5. รัน `git status --short` และตรวจ diff ก่อนแตะไฟล์ รักษาการแก้ไขเดิมของผู้ใช้ทุกไฟล์ หากงานต้องแตะไฟล์ที่ dirty ให้ merge เฉพาะส่วนที่เกี่ยวข้องและรายงานความเสี่ยงก่อนทำ

ลำดับงาน:

- เริ่มเฉพาะ Checkpoint A ในแผน: Tasks 1–4 (Domain/Persistence, Migration/Audit, Management API, Item Images)
- ทำ Task ตามลำดับและใช้ TDD: เขียน test ที่เจาะ invariant → รันให้เห็นว่า fail ด้วยเหตุที่คาด → implement ขั้นต่ำ → รันให้ pass → ตรวจ diff
- ใช้ `LocalizedText` กลางกับ Item, Category, Brand, Alias และ Unit; JSONB อนุญาตเฉพาะ `th`, `en`
- Category ใช้ self-reference `parent_category_id` และต้องกัน cycle; ไม่มีตาราง Subcategory แยก
- Brand เป็น Organization-scoped master; Item อ้างได้แบบ nullable
- Alias เป็น Organization/Item-scoped search term; ใช้ค้นหาได้แต่ไม่แทน Canonical Item Name
- Supplier เป็น deferred context: อย่าสร้าง Supplier Master, Item-Supplier relation หรือ Supplier filter ใน Slice นี้
- Item เป็น Organization-owned aggregate; หลายสาขาใช้ `all_branches|selected_branches` และ relation แยก
- รูปภาพ reuse File Upload Session, Verified File, `IFileStorageProvider` และ File Parent Invariant `item + itemId`
- เขียน Business State และ Audit Event ใน transaction เดียวกัน; Audit เป็น append-only และไม่มี token, signed URL, binary หรือ secret
- บังคับ same-organization relationship ด้วย composite FK/constraint และ integration test
- ใช้ strict types; UI copy ใช้ i18n ไทย/อังกฤษ; preserve Atelier Architectural Navy Sharp
- ใช้ `apply_patch` สำหรับแก้ไฟล์ รักษา minimal blast radius และไม่ format/refactor ไฟล์ที่ไม่เกี่ยวข้อง
- ไม่เปลี่ยน framework/runtime version, ไม่ติดตั้ง dependency ใหม่ และไม่แก้ verification gate เพื่อทำให้ผลผ่าน

Completion criterion ของแต่ละ Task:

- Test ที่ระบุใน Task ผ่านด้วย output ล่าสุด
- `git diff --check` ผ่าน
- ไม่มี `any`, `as any`, `@ts-ignore`, hardcoded UI copy, public file URL, `current_cost` หรือ cross-organization loose FK เพิ่มเข้ามา
- Contract/type/signature ตรงกับ Task ถัดไป
- อัปเดต checkbox ในแผนเฉพาะ step ที่มีหลักฐานผ่านจริง
- สร้าง commit ตามข้อความในแผนเมื่อ working tree แยกจากการแก้ไขของผู้ใช้ได้อย่างปลอดภัย; ห้าม push

เมื่อ Checkpoint A เสร็จ ให้หยุดและรายงาน:

1. ผลลัพธ์ที่ใช้งานได้
2. รายชื่อไฟล์ที่เปลี่ยน
3. Migration/constraint/index ที่เพิ่ม
4. Test commands พร้อมจำนวน pass/fail
5. Security invariants ที่พิสูจน์แล้ว
6. งานหรือ blocker ที่ยังเหลือ
7. Diff/commit SHA สำหรับ review

อย่าเริ่ม Checkpoint B จนกว่าผู้ใช้จะอนุมัติผล Checkpoint A อย่างชัดเจน หาก SDK/บริการภายนอกไม่พร้อม ให้เก็บหลักฐาน error ที่แน่นอน ทำ static verification ที่ปลอดภัยต่อได้ และรายงาน blocker โดยไม่เปลี่ยน runtime หรือข้าม test

---

## Follow-up หลังอนุมัติ Checkpoint A

```text
ผล Checkpoint A ได้รับอนุมัติแล้ว ให้อ่านสถานะล่าสุดและ diff/commit ที่เกิดขึ้น จากนั้นทำเฉพาะ Checkpoint B (Task 5: Versioned Cost Workflow and Deterministic Resolver) ตามแผนเดิม ใช้ TDD และรักษา Maker–Checker, published immutability, branch-over-organization precedence, overlap protection และ ambiguity failure ให้ครบ หยุดเมื่อ Checkpoint B ผ่าน verification และรายงานหลักฐานรูปแบบเดียวกับ Checkpoint A ห้ามเริ่ม Checkpoint C
```

## Follow-up หลังอนุมัติ Checkpoint B

```text
ผล Checkpoint B ได้รับอนุมัติแล้ว ให้ทำเฉพาะ Checkpoint C (Tasks 6–9) ตามแผน: Catalog Read Model, Estimate immutable snapshot, Frontend API/TanStack Query และเชื่อม estimate-item-catalog-modal.tsx ใช้ server facets สำหรับ Category/Brand/Attributes, ใช้ Alias เพื่อค้นหาโดยยังแสดง Canonical Name, และเอา static Supplier filter ออกจาก Slice นี้ รักษา private authenticated images, strict TypeScript, i18n ไทย/อังกฤษ และ server-side price authority หยุดเมื่อ Checkpoint C ผ่าน verification ห้ามเริ่ม Checkpoint D
```

## Follow-up หลังอนุมัติ Checkpoint C

```text
ผล Checkpoint C ได้รับอนุมัติแล้ว ให้ทำ Checkpoint D (Task 10) เท่านั้น เพิ่ม end-to-end/security coverage และ verification record รัน completion gates ตาม repository แล้วรายงานผลจริงทุกคำสั่ง หาก gate ใดไม่ผ่านให้ระบุ exact failure และไม่กล่าวอ้างว่างาน production-ready ห้ามแก้ test, runtime หรือ configuration เพียงเพื่อซ่อน failure
```
