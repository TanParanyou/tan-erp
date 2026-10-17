---
status: accepted
---

# Item Import Phase 2: upsert, Cost Draft และ Batch Job

Phase 1 (CP-03) รองรับเฉพาะสร้าง Item ใหม่ทั้งไฟล์แบบ atomic สูงสุด 500 แถว. G-06 เพิ่ม 3 ความสามารถโดยไม่เปลี่ยนกติกา Phase 1.

## 1. mode `upsert` (key = Item Code)

- ทับได้เฉพาะ: `nameTh`, `nameEn`, `descriptionTh`, `descriptionEn`, `categoryCode`, `brandCode`, `taxCategoryCode`, `canSell`, `canCost`, `canPurchase`, `canStock`, `canProduce`. รายการนี้อยู่ใน `ItemImportUpsertPolicy` (ที่เดียว) และต้องตรงกับเอกสารนี้.
- `itemType` และ `baseUnitCode` เป็นค่าระบุตัวตน: ถ้าไม่ตรงกับของเดิม แถวนั้นผิด (`MISMATCH`) ไม่ถูกทับเงียบ ๆ เพราะหน่วยฐานผูกกับ conversion และต้นทุน.
- **ไม่แตะ lifecycle state เด็ดขาด:** `status`, การเปิด/ปิดใช้งาน, `availabilityMode` และสาขา, attributes, รูป, barcode. Item ที่ปิดใช้งานแล้วถูกปฏิเสธ (`LOCKED`) ไม่ถูกเปิดใหม่ผ่าน import.
- ค่าว่างในช่องที่ทับได้ = ล้างค่าที่ไม่บังคับ (ไฟล์คือแหล่งความจริงของ 12 ช่องนี้). เลือกแบบนี้เพราะทำให้ผลของไฟล์เดิมคงที่และอธิบายง่าย; ทางเลือก "ว่าง = คงของเดิม" ปลอดภัยกว่าแต่ล้างค่าไม่ได้ **รอ Data Steward ยืนยัน**.
- Idempotent: เทียบค่าที่ทับได้กับของเดิม; เหมือนทุกช่อง = `unchanged` (ไม่ UpdateDetails, ไม่เปลี่ยน RowVersion, ไม่เขียน audit). ไฟล์เดิมสองครั้งจึงได้ผลเดิม. แถวที่เปลี่ยนจริงเขียน audit `items.update` พร้อม `importBatchId`.
- สิทธิ์: `items.create` + `items.update` (ตรวจก่อนอ่านไฟล์). Preview แสดงผลต่อแถว (`created`/`updated`/`unchanged`).

## 2. Cost import เป็น Draft เท่านั้น

- สิทธิ์ `cost-records.create` เดิม **ไม่เพิ่ม permission ใหม่** เพราะ Draft คือสิ่งที่สิทธิ์นี้อนุญาตอยู่แล้ว; การ submit/approve/publish ยังใช้สิทธิ์และ maker–checker เดิม.
- สร้างด้วย `CostRecord.CreateDraft` โดย `CreatedByUserId` = ผู้ import ดังนั้นกฎ "ผู้อนุมัติต้องไม่ใช่ผู้สร้าง" ของเดิมใช้ต่อทันที. ไม่เรียก submit/approve/publish/supersede และไม่อ่าน record ที่ published เพื่อแก้.
- ทุกแถวต้องมี `costSourceCode` (manual และ active), `sourceReference`, `reason` (เท่ากับกฎ create เดิมที่ต้องมีที่มา). ไม่รองรับแนบ evidence file ใน CSV.
- แถวที่เหมือน record ที่ยังมีผล (draft/submitted/returned/approved/published) ทุกช่องราคา/ช่วง/ปริมาณ = `unchanged` ไม่สร้างซ้ำ; record ที่ superseded/disabled ไม่ขวาง. แถวซ้ำกันในไฟล์ (item, scope, branch, unit, currency, min qty, วันเริ่ม) = `DUPLICATE_COST`.
- สาขา: แถว `branch` ต้องเป็นสาขาที่ active และอยู่ในสิทธิ์สาขาของผู้ import (`BRANCH_ACCESS_DENIED`).
- ไม่มี mode `upsert` สำหรับต้นทุน (ค่าอื่น = `ITEM_IMPORT_OPTION_INVALID`).

## 3. Batch และ background job

- ไฟล์ที่เกินเพดาน preview/commit แบบทันที (500 แถว/1 MB) ส่งเป็น job: `POST /api/v1/items/imports/jobs` (ต้องมี `Idempotency-Key`) ตอบ `202`, `GET /api/v1/items/imports/jobs/{id}` ดูสถานะ. ตาราง `item_master.item_import_jobs` เก็บไฟล์ชั่วคราว (ล้างเมื่อจบ), progress, ผลนับ, row error (สูงสุด 500 แถว ตัดแล้วมี `errorsTruncated`).
- ใช้ `BackgroundService` (`ItemImportJobWorker`) แบบ poll เหมือนตัวส่งเตือน membership; ไม่ลงทะเบียนใน Test environment; ปิดได้ด้วย `ItemImportJobs:Enabled`. ทำทีละ job.
- **batch size เป็น config** (`ItemImportJobs:BatchSize`, ค่าเริ่มต้น 100, clamp 10–1000) บันทึกลง job ตอนสร้างเพื่อให้ progress คงที่ตลอดงาน. 100 แถวคือจุดกลางระหว่าง transaction สั้น (ล็อกน้อย retry ถูก) กับจำนวนรอบ DB ที่ไม่มากเกิน.
- **commit atomic ต่อ batch:** แต่ละ batch = 1 transaction (Serializable ผ่าน `SerializableTransactionRunner`) ที่เขียนแถวและ progress พร้อมกัน. แถวผิดแถวเดียว = ไม่เขียนแถวใดของ batch นั้น, บันทึก row error, batch ถัดไปยังทำต่อ. สถานะจบ: `completed` / `completed_with_errors` / `failed`. ชนกันพร้อมกัน (serialization/unique) ลองใหม่ได้ 3 ครั้งแล้วจึงถือว่า job ล้มเหลว.
- ทนต่อการล้ม: worker ถือ lease (ค่าเริ่มต้น 10 นาที, ต่ออายุทุก batch). ถ้าตายกลางทาง job อื่นรับต่อได้เมื่อ lease หมดและเริ่มที่ batch ถัดไปที่ยังไม่เสร็จ (`next_batch_index`). การตรวจ token (`row_version`) ป้องกัน worker เก่าเขียนทับหลังถูกแทนที่. สิทธิ์ผู้สั่งงานถูกตรวจซ้ำตอนรัน (ถูกเพิกถอนระหว่างทาง = job `failed`).
- แถวซ้ำรหัสข้าม batch ตรวจจากทั้งไฟล์ล่วงหน้า (ไม่ใช่เฉพาะภายใน batch).
- ผลของ job ที่มี batch ถูกปฏิเสธ: batch ที่ผ่านแล้วถูกบันทึกจริง. แก้ไฟล์แล้วนำเข้าซ้ำได้โดยแถวที่เข้าไปแล้วเป็น `unchanged` (upsert/cost) หรือ `ALREADY_EXISTS` (create) — ผู้ใช้ควรใช้ `upsert` กับ re-run.

## 4. ข้อจำกัดและเพดาน

| รายการ | ค่า |
| --- | --- |
| Preview/commit ทันที | 500 แถว, 1,000,000 ตัวอักษร, request 4 MB |
| Job | 20,000 แถว, 10,000,000 ตัวอักษร, request 21 MB |
| Row error ที่เก็บต่อ job | 500 แถว |
| Batch ต่อ transaction | 100 (config 10–1000) |

## ข้อเสียที่ยอมรับ

- Job ที่เสร็จบางส่วนไม่ได้เป็น atomic ทั้งไฟล์ (ตั้งใจ ตามแผน G-06 "atomic ต่อ batch"); ผู้ที่ต้องการ all-or-nothing ทั้งไฟล์ต้องอยู่ในเพดาน 500 แถวของ commit ทันที.
- ไม่มียกเลิก job กลางทาง, ไม่มี retry อัตโนมัติของ batch ที่ถูกปฏิเสธ, ไม่มี notification เมื่อจบ (G-02 ใช้ต่อได้ภายหลัง).
- ไฟล์ถูกเก็บใน DB จนกว่า job จบ (สูงสุด 10 MB ต่อ job); ไม่มีตัวกวาด job ค้างที่ไม่เคยถูกรับ.
- การดูสถานะ job ใช้ `items.read` หรือ `cost-records.read`; เห็น job ของทั้ง Organization (payload มีเฉพาะรหัสแถวและรหัส error ไม่มีจำนวนเงิน).
