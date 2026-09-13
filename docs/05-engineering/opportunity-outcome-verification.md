# บันทึกผลการตรวจสอบ Opportunity Outcome (Close/Reopen) & Stage History Timeline Slice (Verification Record)

เอกสารนี้บันทึกผลการทดสอบและการยืนยันความถูกต้องของ **Slice 3: Outcome (Close/Reopen) & Stage History Timeline** ตามแผนงาน [Opportunity Module Completion Master Plan](../superpowers/plans/2026-09-12-opportunity-module-completion-master-plan.md)

---

## 1. ขอบเขตงานที่ดำเนินการ (Implemented Scope)

1. **Domain Model:**
   - ใน `OpportunityValues.cs`: กำหนด Canonical Controlled Reason Codes สำหรับ `Lost` (6 รหัส), `Cancelled` (4 รหัส), และ `Reopen` (5 รหัส)
   - ใน `Opportunity.cs`: เพิ่ม Domain Methods `Close(...)` และ `Reopen(...)`
     - `Close`: รองรับการปิดจากสถานะ Open (`draft`, `qualified`, `surveying`, `estimating`, `proposed`) ไปยัง `lost` หรือ `cancelled` พร้อมบังคับ `reasonCode`
     - `Reopen`: รองรับการเปิดใหม่จากสถานะ Closed (`lost`, `cancelled`) กลับสู่ `draft` หรือ `qualified` พร้อมบังคับ `reasonCode`
     - ทำการหมุน RowVersion ใหม่เสมอ (`Guid.NewGuid()`)
2. **Application Layer:**
   - ปรับปรุง `QualifyOpportunityCommand` / `QualifyOpportunityHandler` ให้รองรับ State Transition สู่ `qualified`, `lost`, `cancelled`, และ `reopen` พร้อมตรวจสอบ Controlled Reason Code
   - สร้าง Feature `GetOpportunityStageHistory`:
     - `GetOpportunityStageHistoryQuery` และ `GetOpportunityStageHistoryHandler`
     - ตรวจสอบสิทธิ์ `opportunities.read` ขอบเขต Organization
   - อัปเดต `IOpportunityStore`:
     - `TransitionStageAsync` รองรับ close / reopen
     - `GetStageHistoryAsync` ดึงประวัติเรียงลำดับ `occurred_at_utc DESC, id DESC`
3. **Infrastructure & Persistence:**
   - พัฒนาใน `OpportunityStore.cs` บน PostgreSQL 17:
     - `TransitionStageAsync`: บันทึก append-only `crm.opportunity_stage_history` พร้อม `reason_code` และ `note`
     - บันทึก Audit Log `opportunity.stage-changed` (Privacy-safe: บันทึกเฉพาะ metadata `fromStage`, `toStage`, `reasonCode`, ไม่มี PII หรือ free-text note)
     - `GetStageHistoryAsync`: Query ประวัติขั้นตอนตามดัชนีพร้อม mapping สู่ projection DTO
4. **API Endpoint & OpenAPI Contract:**
   - อัปเดต `POST /api/v1/opportunities/{id}/stage-transitions` ให้รับ `reasonCode` และ `note`
   - เพิ่ม `GET /api/v1/opportunities/{id}/stage-history` คืนค่า `OpportunityStageHistoryListResponse`
   - อัปเดต `contracts/openapi/tan-erp.v1.json` และ regenerate TypeScript client ใน `frontend/src/generated/api/tan-erp.v1.ts`
5. **Frontend Application & UI Components (Atelier Architectural Navy Sharp):**
   - เพิ่ม `apiClient.getOpportunityStageHistory` และ `apiClient.transitionOpportunityStage` พร้อม Query hooks ใน `opportunity-queries.ts`
   - คอมโพเนนต์ `OpportunityCloseModal`: รวม Action การปิดงานและยกเลิกไว้ใน Modal เดียว มี Dropdown ให้เลือกประเภท (Lost / Cancelled) พร้อมเหตุผลและหมายเหตุ
   - คอมโพเนนต์ `OpportunityReopenModal`: Modal เปิดงานใหม่พร้อม Dropdown เหตุผลตาม Controlled Catalog
   - คอมโพเนนต์ `OpportunityStageTimeline`: แสดงประวัติขั้นตอน (จาก → ไปยัง, วันเวลา, เหตุผล, หมายเหตุ) สไตล์ Atelier Hairline
   - ปรับปรุง `OpportunityDetail`: มีปุ่ม `[ ปิดงาน / ยกเลิก... ]` (เมื่อ Open), `[ เปิดงานใหม่ ]` (เมื่อ Closed) จัดลำดับปุ่มอย่างเป็นระเบียบ และไม่มีปุ่มลบ (No Delete) ตาม ERP Audit Invariant
   - แปลภาษาครบทั้งไทย (`th.json`) และอังกฤษ (`en.json`) โดยไม่มีดอกจันซ้ำ (`reasonCodeLabel`)
6. **Testing & Verification:**
   - Unit Tests และ Integration Tests ครอบคลุม Domain, Application, Database Persistence, Controller และ Frontend
   - Automated tests ทั้งหมดผ่าน 100%

---

## 2. ผลการรัน Verification Gates

### 2.1 Backend Tests (`dotnet test backend/TanErp.slnx`)
- **Unit Tests:** ผ่าน 141 รายการ (รวม `CloseLost_ValidReason_AppendsHistory`, `CloseCancelled_ValidReason_AppendsHistory`, `Reopen_ApprovedTarget_AppendsHistory`)
- **Architecture Tests:** ผ่าน 3 รายการ
- **Integration Tests:** ผ่าน 123 รายการ บน PostgreSQL 17 (รวม `OpportunitySiteEndpointsTests` สำหรับ Close Lost, Cancelled, และ Reopen)
- **สถานะ:** ✅ ผ่านทั้งหมด 267 รายการ (0 failed)

### 2.2 Frontend Quality Gates (`npm --prefix frontend run verify`)
- **API Contract Verification (`check:api`):** ✅ สอดคล้อง 100%
- **ESLint (`lint`):** ✅ ผ่าน ไม่มี error
- **TypeScript Typecheck (`typecheck`):** ✅ ผ่าน ไม่มี type error (strict mode, zero `any`)
- **Frontend Component/Unit Tests (`vitest`):** ✅ ผ่านทั้งหมด 89 test files, 345 tests
- **Next.js Production Build (`build`):** ✅ ผ่านสมบูรณ์

### 2.3 Fixture Contract Tests (`npm run test:fixtures`)
- **Survey Baseline Fixture:** ✅ ผ่าน 1 รายการ

---

## 3. สรุปผลลัพธ์
งานใน **Slice 3: Outcome (Close/Reopen) & Stage History Timeline** เสร็จสมบูรณ์ตามเกณฑ์ Definition of Done ครบถ้วนทุกมิติ
