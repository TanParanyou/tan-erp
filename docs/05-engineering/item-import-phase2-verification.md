# Item Import Phase 2 Verification (G-06: upsert, Cost Draft, Batch Job)

## 1. สถานะ

**Implemented 2026-10-17 (G-06: Item Import Phase 2) — Verified 2026-10-23: Unit + Integration + Vitest passed; Migration `AddItemImportJobs` applied บน local DB.**

- ชุดทดสอบ Unit (`ItemImportCsvTests`), Integration (`ItemImportEndpointsTests.Phase2_UpsertMode_CostDraftOnlyImport_AndBatchImportJobs_WorkEndToEnd`) และ Frontend Vitest (`item-import-modal.test.tsx`, `item-import-job-panel.test.tsx`) เขียนและรันผ่านครบเมื่อ 2026-10-23
- **ยังไม่ได้ตรวจด้วยสายตาในเบราว์เซอร์**
- ยังไม่ผ่าน UAT และยังไม่พร้อม Production

เอกสารที่เกี่ยวข้อง: [ADR 0026](../adr/0026-item-import-phase2.md), [แผน G-06](../superpowers/plans/2026-10-05-erp-gap-closure-plan.md), [Item Verification](item-master-estimate-catalog-verification.md)

## 2. ผลที่รันจริง (อัปเดต 2026-10-23)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build` (backend) | `0 Warning(s)` `0 Error(s)` |
| `ItemImportCsvTests` (`TanErp.UnitTests`) | **Passed** (รวม `CostImportCsv_ParseAndValidate_EnforcesColumnsScopeBranchCurrencyAndQuantities`) |
| `ItemImportEndpointsTests.Phase2_UpsertMode_CostDraftOnlyImport_AndBatchImportJobs_WorkEndToEnd` (`TanErp.IntegrationTests`) | **Passed 1/1** (10s) |
| `item-import-modal.test.tsx` + `item-import-job-panel.test.tsx` (`Vitest`) | **Passed** |
| `cd frontend && npx tsc --noEmit` | exit 0 |
| `cd frontend && npm run lint` | exit 0 |
| `cd frontend && npm run build` | ผ่าน |
| `cd frontend && npm run check:api` | ผ่าน (snapshot + generated types ตรงกัน) |

## 3. สิ่งที่เปลี่ยน

- Backend: `ItemImportCsv` (mode, limits, duplicate pre-pass), `CostImportCsv`, `ItemImportStore` (upsert/stage), `CostImportStore`, `ItemImportJob` (domain), `ItemImportJobStore`, `ItemImportJobRunner`, `ItemImportJobWorker`, handlers `CreateItemImportJob`/`GetItemImportJob`, error codes 5 ตัวใน `ProblemDetailsMapper` + `Errors.resx`/`Errors.en.resx`, config `ItemImportJobs`
- API: `POST /api/v1/items/imports/preview|commit` รับ `kind`/`mode` (ไม่ส่ง = `items`/`create` เหมือนเดิม), `POST /api/v1/items/imports/jobs` (202), `GET /api/v1/items/imports/jobs/{jobId}`
- Frontend: เลือกชนิด/โหมดในดรอว์เวอร์ import, ผล per-row `created/updated/unchanged`, แผงความคืบหน้างานเบื้องหลัง `item-import-job-panel.tsx`, template ต้นทุน, i18n th/en
- Permission: ไม่เพิ่ม — ใช้ `items.create`, `items.update`, `cost-records.create`, อ่านสถานะด้วย `items.read` หรือ `cost-records.read`

## 4. สิ่งที่ต้องตรวจเองก่อนใช้จริง

1. apply migration กับ DB ทดสอบ แล้วรัน API (ไม่ใช่ Test environment) ให้ worker ทำงาน
2. test ที่แผนระบุ (ยังไม่เขียน): upsert idempotent ไฟล์เดิมสองครั้ง, cost import ไม่กระทบราคา published และ approver ≠ ผู้ import, rollback batch เมื่อแถวใดผิด, resume หลัง lease หมด, ข้าม batch duplicate code
3. ทดสอบไฟล์ 20,000 แถวเพื่อดูเวลา/หน่วยความจำ (ไฟล์ถูก parse ทั้งก้อนในหน่วยความจำของ worker) และปรับ `BatchSize`
4. ตรวจเบราว์เซอร์: ดรอว์เวอร์ import ทั้ง 3 เส้นทาง (create / upsert / costs) และ job panel
5. ผู้ที่มี `cost-records.create` แต่ไม่มี `items.create` ยังไม่เห็นปุ่ม import (ปุ่มผูก `items.create` เดิม)

## 5. คำถามที่รอเจ้าของข้อมูล

- Data Steward: ค่าว่างใน upsert ควร "ล้างค่า" (ปัจจุบัน) หรือ "คงของเดิม"
- Cost Owner: Draft ที่ import ควรแจ้งผู้อนุมัติ (G-02) หรือไม่ และต้องแนบ evidence file ได้หรือไม่
- เพดาน 20,000 แถว/10 MB และ batch 100 เหมาะกับข้อมูลจริงหรือไม่
