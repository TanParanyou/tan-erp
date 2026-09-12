# บันทึกผลการตรวจสอบ Opportunity Draft Q-Gate Completion Slice (Verification Record)

เอกสารนี้บันทึกผลการทดสอบและการยืนยันความถูกต้องของ **Slice 1: Draft Q-Gate Completion** ตามแผนงาน [Opportunity Module Completion Master Plan](../superpowers/plans/2026-09-12-opportunity-module-completion-master-plan.md) และ [Opportunity Draft Q-Gate Completion Plan](../superpowers/plans/2026-09-12-opportunity-draft-q-gate-completion.md)

---

## 1. ขอบเขตงานที่ดำเนินการ (Implemented Scope)

1. **Domain Model:**
   - เพิ่ม Domain method `Opportunity.EditDraftQGate(scopeSummary, workTypes, nextActionAtUtc, nextActionNote)` ที่ตรวจสอบ Invariant ในระดับ Domain:
     - ต้องเป็นสถานะ `draft` เท่านั้น
     - ห้ามลดจำนวน `workTypes` จนเหลือว่าง (ต้องระบุอย่างน้อย 1 ประเภท)
     - คู่ invariant ของการติดตาม: `nextActionAtUtc` และ `nextActionNote` ต้องระบุพร้อมกันทั้งคู่ หรือเป็น null พร้อมกันทั้งคู่
     - ทำการหมุน RowVersion ใหม่เสมอ (`Guid.NewGuid()`) เมื่อมีการแก้ไข
2. **Application Layer:**
   - สร้าง `UpdateDraftQGateCommand` และ `UpdateDraftQGateHandler`
   - ตรวจสอบสิทธิ์ `opportunities.update` ขอบเขต Organization หรือ Branch ที่เป็นเจ้าของ
   - จัดการ Idempotency Replay ผ่าน Store
   - ตรวจสอบ Concurrency conflict (ETag / RowVersion mismatch) คืนค่า Domain Conflict RFC 9457
3. **Infrastructure & Persistence:**
   - พัฒนา `UpdateDraftQGateAsync` ใน `OpportunityStore.cs` บน PostgreSQL 17
   - ใช้ Database Transaction ควบคุม Atomic Update
   - บันทึก Privacy-safe Audit Log `opportunity.updated` พร้อม Diff เฉพาะฟิลด์ที่ถูกเปลี่ยนแปลง (`changed_fields`)
   - รองรับ Idempotent Replay ผ่าน `app.idempotency_keys`
4. **API Endpoint & OpenAPI:**
   - เพิ่ม `PATCH /api/v1/opportunities/{id}` ใน `OpportunitiesController.cs` พร้อม Header `If-Match` และ `Idempotency-Key`
   - คืน HTTP 200 OK พร้อม Header `ETag: "{rowVersion}"`
   - คืน HTTP 428 Precondition Required เมื่อขาด `If-Match`
   - คืน HTTP 412 / 409 Conflict เมื่อเวอร์ชันไม่ตรง
   - อัปเดต `contracts/openapi/tan-erp.v1.json` และ regenerate TypeScript client ใน `frontend/src/generated/api/tan-erp.v1.ts`
5. **Frontend Application & UI Component:**
   - เพิ่ม `apiClient.updateDraftQGate` และ mutation hook `useUpdateDraftQGate` พร้อมการ invalidate query cache (`detail` และ `list`)
   - พัฒนาคอมโพเนนต์ `OpportunityQGateEditor` ตามมาตรฐาน Atelier Architectural Navy Sharp (0px radius, Solid Navy, Pure SVG Icons, Deferred action, Double submit protection)
   - เชื่อมต่อ `OpportunityQGateEditor` เข้ากับหน้า `OpportunityDetail` เมื่อ Opportunity อยู่ในสถานะ `draft` และผู้ใช้มีสิทธิ์ `opportunities.update`
   - ปรับปรุง `opportunity-form-schema.ts` ให้จำกัดความยาว `scopeSummary <= 2000` และ `sourceCode <= 50` สอดคล้องกับ Database schema
6. **Testing & End-to-End Journey:**
   - เพิ่ม Unit Tests และ Integration Tests ครบทั้ง Domain, Handler, PostgreSQL Store, Controller และ Frontend
   - เพิ่ม Playwright E2E spec `frontend/e2e/opportunity-draft-q-gate.spec.ts`

---

## 2. ผลการรัน Verification Gates

### 2.1 Backend Tests (`dotnet test backend/TanErp.slnx`)
- **Unit Tests:** ผ่าน 130 รายการ (รวม `EditDraftQGate_ValidDraft_UpdatesQGateAndRotatesVersion` และ `UpdateDraftQGateHandler_ValidRequest_ResolvesUpdatePermissionAndPersists`)
- **Architecture Tests:** ผ่าน 3 รายการ
- **Integration Tests:** ผ่าน 111 รายการ บน PostgreSQL 17 (รวม `UpdateDraftQGate_ValidDraft_PersistsFieldsVersionAuditAndReplay` และ `PatchOpportunity_ValidDraft_ReturnsUpdatedResponseAndETag`)
- **สถานะ:** ✅ ผ่านทั้งหมด 244 รายการ (0 failed)

### 2.2 Frontend Quality Gates (`npm --prefix frontend run verify`)
- **API Contract Verification (`check:api`):** ✅ Schema และ Generated Types สอดคล้อง 100%
- **ESLint (`lint`):** ✅ ผ่าน ไม่มี lint warning/error
- **TypeScript Typecheck (`typecheck`):** ✅ ผ่าน ไม่มี type error (strict mode, zero `any`)
- **Frontend Component/Unit Tests (`vitest`):** ✅ ผ่านทั้งหมด 88 test files, 337 tests
- **Next.js Production Build (`build`):** ✅ ผ่านทั้ง Static Generation และ Dynamic Routes

### 2.3 Fixture Contract Tests (`npm run test:fixtures`)
- **Survey Baseline Fixture:** ✅ ผ่าน 1 รายการ

---

## 3. สรุปผลลัพธ์
งานใน **Slice 1: Draft Q-Gate Completion** เสร็จสมบูรณ์ตามเกณฑ์ Definition of Done ครบถ้วนทุกมิติ พร้อมเข้าสู่ Slice ถัดไปตาม Master Plan
