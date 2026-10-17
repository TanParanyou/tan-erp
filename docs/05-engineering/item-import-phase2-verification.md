# Item Import Phase 2 Verification (G-06: upsert, Cost Draft, Batch Job)

## 1. สถานะ

**Implemented 2026-10-17 (G-06: Item Import Phase 2) — no tests written, none run (per user).**

- ตามคำสั่งผู้ใช้ รอบนี้ **ไม่ได้เขียน test ใหม่** และ **ไม่ได้รัน test ใด ๆ** (`dotnet test`, `vitest`, Playwright) มีเพียง compile/lint/build
- **ไม่ได้ตรวจในเบราว์เซอร์** และไม่ได้รัน worker กับฐานข้อมูลจริง
- migration `AddItemImportJobs` สร้างด้วย `dotnet ef migrations add` แล้ว **ยังไม่ได้ apply กับฐานข้อมูลใด ๆ**
- ยังไม่ผ่าน UAT และยังไม่พร้อม Production
- ไฟล์ test เดิม `item-import-modal.test.tsx` แก้ 1 จุดเท่านั้นเพราะพฤติกรรมเปลี่ยนโดยตั้งใจ: ไฟล์ 1 MB–10 MB ถูกส่งเป็น job แทนการถูกปฏิเสธ (เพดานปฏิเสธเป็น 10 MB) — ไม่ได้รันยืนยัน

เอกสารที่เกี่ยวข้อง: [ADR 0026](../adr/0026-item-import-phase2.md), [แผน G-06](../superpowers/plans/2026-10-05-erp-gap-closure-plan.md), [Item Verification](item-master-estimate-catalog-verification.md)

## 2. ผลที่รันจริง (compile/lint/build เท่านั้น)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build` (backend) | `0 Warning(s)` `0 Error(s)` |
| `dotnet ef migrations add AddItemImportJobs` | สร้างสำเร็จ |
| `cd frontend && npx tsc --noEmit` | exit 0 |
| `cd frontend && npm run lint` | exit 0 |
| `cd frontend && npm run build` | ผ่าน |
| `cd frontend && npm run check:api` | ผ่านหลัง stage ไฟล์ (snapshot + generated types ตรงกัน) |
| OpenAPI snapshot | regenerate จาก swagger สดด้วย formatter ที่ round-trip ไบต์เดียวกับไฟล์เดิม; เปลี่ยนเฉพาะการเพิ่ม (2 path, schema ของ job, ฟิลด์ `kind`/`mode`/`outcome`/counts) |

ผลนี้ยืนยันเพียงว่าคอมไพล์ lint และ build ผ่าน ไม่ได้พิสูจน์พฤติกรรม

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
