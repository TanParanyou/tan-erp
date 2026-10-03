# บันทึกผลการตรวจสอบ Opportunity Module Hardening (Slice 6)

เอกสารนี้บันทึกผลการทดสอบเชิงลึกด้านความมั่นคงปลอดภัย ความถูกต้องของข้อมูล และ Concurrency ใน **Slice 6: Module Hardening** ตามแผนงาน [Opportunity Module Completion Master Plan](../superpowers/plans/2026-09-12-opportunity-module-completion-master-plan.md). ผล technical verification เป็นหลักฐานของโค้ด ไม่ใช่การรับรอง Production หรือ business UAT.

---

## 1. ขอบเขตการทดสอบและข้อกำหนดที่ผ่านการตรวจสอบ (Hardened Invariants)

1. **Cross-Organization Scope Isolation (404 Not Found):**
   - ผู้ใช้งานจากองค์กรอื่น (Organization B) ไม่สามารถเข้าถึง แก้ไข หรือเรียกดูข้อมูลใดๆ ขององค์กร A:
     - `POST /api/v1/opportunities/{id}/stage-transitions` → คืน HTTP `404 RESOURCE_NOT_FOUND`
     - `GET /api/v1/opportunities/{id}/stage-history` → คืน HTTP `404 RESOURCE_NOT_FOUND`
     - `PATCH /api/v1/opportunities/{id}` (Draft Q-Gate) → คืน HTTP `404 RESOURCE_NOT_FOUND`
     - `POST /api/v1/opportunities/{id}/owner-changes` → คืน HTTP `404 RESOURCE_NOT_FOUND`
   - ผลการทดสอบ: ป้องกันการรั่วไหลของข้อมูลข้าม Tenant สมบูรณ์ 100%
2. **Permission Denial (403 Forbidden):**
   - ผู้ใช้งานที่ไม่มีสิทธิ์ `opportunities.transition` ใน Active Membership ไม่สามารถ Qualify, Close หรือ Reopen ได้ → คืน HTTP `403 PERMISSION_DENIED`
   - ผู้ใช้งานที่ไม่มีสิทธิ์ `opportunities.update` ไม่สามารถแก้ไข Draft หรือ Reassign Owner ได้ → คืน HTTP `403 PERMISSION_DENIED`
3. **Optimistic Concurrency Control (ETag & Versioning):**
   - ส่งคำขอ `PATCH /api/v1/opportunities/{id}` โดยไม่ระบุ Header `If-Match` → คืน HTTP `428 IF_MATCH_REQUIRED`
   - ส่งคำขอ `PATCH` หรือ `POST stage-transitions` ด้วย `expectedVersion` หรือ `If-Match` ที่ไม่ตรงกับเวอร์ชันปัจจุบัน (Stale Version) → คืน HTTP `409 OPPORTUNITY_VERSION_CONFLICT`
4. **Deterministic Idempotency Replay & Conflict Protection:**
   - การส่งคำขอด้วย `Idempotency-Key` เดิม + Payload เดิมซ้ำ → คืน HTTP `200 OK` (Replay) โดย **ไม่สร้าง Stage History หรือ Audit Event ซ้ำซ้อน**
   - การส่งคำขอด้วย `Idempotency-Key` เดิม แต่แก้ไข Payload → คืน HTTP `409 IDEMPOTENCY_KEY_REUSED` ป้องกันการขโมยหรือนำ Key กลับมาใช้ซ้ำผิดวัตถุประสงค์
5. **Privacy-Safe Audit Trail:**
   - ตาราง `crm.opportunity_stage_history` และ `app.audit_events` บันทึกเฉพาะ Metadata (`fromStage`, `toStage`, `reasonCode`) โดยปราศจาก Free-text Note หรือข้อมูลส่วนบุคคล (PII)
6. **Strict Frontend Quality & Design Standards:**
   - Strict TypeScript ปราศจาก `any`, `as any` หรือ `@ts-ignore` 100%
   - Design System ยึดมั่นตาม Atelier Architectural Navy Sharp (0px border radius, Solid Navy, Pure SVG icons)
   - i18n Key Parity สมบูรณ์ทั้งภาษาไทยและอังกฤษ

---

## 2. ผลการรัน Verification Gates

ตารางนี้เป็นผล verification baseline จากรอบก่อนหน้า; ผลตรวจล่าสุดหลังแก้ case-insensitive search อยู่ในหัวข้อ 4.

| หมวดหมู่การทดสอบ | คำสั่งที่ใช้ | ผลลัพธ์ | รายละเอียด |
|---|---|:---:|---|
| **Backend Unit Tests** | `dotnet test backend/TanErp.slnx` | ✅ ผ่าน | 141 tests (Domain + Handler + CQRS) |
| **Backend Architecture Tests** | `dotnet test backend/TanErp.slnx` | ✅ ผ่าน | 3 tests (Clean Architecture Dependency Invariants) |
| **Backend Integration Tests** | `dotnet test backend/TanErp.slnx` | ✅ ผ่าน | 128 tests บน PostgreSQL 17 จริง (รวม Hardening 5 เคสใหม่) |
| **Frontend API Contract** | `npm --prefix frontend run check:api` | ✅ ผ่าน | OpenAPI spec และ Generated types ตรงกัน 100% |
| **Frontend Lint** | `npm --prefix frontend run lint` | ✅ ผ่าน | ESLint ผ่าน 0 error/warning |
| **Frontend Typecheck** | `npm --prefix frontend run typecheck` | ✅ ผ่าน | Strict TypeScript ผ่าน 100% |
| **Frontend Tests** | `npm --prefix frontend test` | ✅ ผ่าน | 89 test files, 345 tests ผ่านทั้งหมด |
| **Frontend Build** | `npm --prefix frontend run build` | ✅ ผ่าน | Next.js Production Build สำเร็จ |
| **Fixture Contracts** | `npm run test:fixtures` | ✅ ผ่าน | Baseline fixtures ผ่านสมบูรณ์ |

---

## 3. ขอบเขตการรับรอง

โมดูล **Opportunity** มี implementation สำหรับ Create, Qualify, Draft Repair, Open Record Maintenance, Owner Reassignment, Close Outcome, Reopen, Stage History Timeline และ hardening ตามหลักฐานของ slice. เอกสารนี้ไม่รับรองความพร้อม Production; authorized-role UAT, release scope และ operations gates ยังคงอ้างอิง [Implementation Roadmap](../00-overview/implementation-roadmap.md) และ [ERP Completion Plan](../superpowers/plans/2026-09-29-erp-completion-master-plan.md).

## 4. Case-insensitive search recheck (2026-09-30)

- เพิ่ม regression test `ListOpportunities_SearchMatchesTitleWithoutCaseSensitivity` ให้ตรวจการค้นหาชื่อด้วยตัวพิมพ์ใหญ่ และรหัสด้วยตัวพิมพ์เล็ก; test ผ่าน 1/1 บน PostgreSQL Testcontainers.
- แก้ Opportunity list query ให้ normalize search term เป็น lowercase และเทียบ `Code` แบบไม่สนตัวพิมพ์เล็ก/ใหญ่.
- `dotnet build backend/TanErp.slnx --no-restore -m:1`: ผ่าน 0 warnings/errors.
- `dotnet test backend/TanErp.slnx --no-restore -m:1`: Architecture 3/3 และ Unit 288/288 ผ่าน; Integration 290/291 ผ่าน. รายการที่ล้มคือ `DocumentSequencesEndpointsTests.UpdateDocumentSequence_WithInvalidDocType_Returns400BadRequest` ระหว่าง PostgreSQL fixture startup ด้วย `Received unknown response H for SSLRequest`; regression test ของ Opportunity ผ่านใน full run.
- รัน `DocumentSequencesEndpointsTests.UpdateDocumentSequence_WithInvalidDocType_Returns400BadRequest` แยกหลัง full suite ผ่าน 1/1 ใน 11 วินาที; ผลนี้ชี้ว่า failure อาจเกิดจาก fixture startup ชั่วคราว แต่ยังไม่ยืนยัน root cause และไม่ทำให้ full suite run ที่มี 1 failure กลายเป็น green gate.
- Full suite retry ด้วย `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` ไม่จบหลัง 13 นาทีและถูกยกเลิกขณะ testhost ยังทำงาน; ไม่มีผลรวมให้ใช้เป็นหลักฐาน.
- ผลข้างต้นเป็น historical recheck. รอบถัดมาพบและแก้ review concurrency ใน Estimate; full backend gate หลังแก้ผ่าน Integration 292/292, Unit 288/288 และ Architecture 3/3 พร้อม TRX counters ไม่มี failure/timeout. ดู [Estimate Verification](official-estimate-verification.md#review-decision-concurrency-follow-up-2026-09-30). SSL startup error เดิมยังไม่ยืนยันสาเหตุ; การยกเลิก run หลัง 13 นาทีไม่ใช่หลักฐานว่า testhost ค้าง.
