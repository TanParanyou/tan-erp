# G-16 Credit Exposure & AR Aging Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `.agents/skills/tiered-orchestration/SKILL.md` to implement this plan phase-by-phase.

**Goal:** เพิ่มระบบควบคุมวงเงินเครดิตลูกค้า (Customer Credit Exposure), รายงานอายุหนี้ลูกหนี้การค้า (AR Aging Buckets: Current / 1–30 / 31–60 / 61–90 / 90+ วัน), และกลไกขออนุมัติขยายเพดานเครดิตชั่วคราว (Credit Limit Override แบบ Maker–Checker + Approval Authority `G-03c`)

**Architecture:** Clean Architecture (`Domain` → `Application` → `Infrastructure` → `Api`) + Next.js App Router (`frontend/src/features/finance/`)

---

## 1. กฎธุรกิจและค่าเริ่มต้น (`TEST_ONLY`)

1. **สูตรคำนวณภาระเครดิตลูกค้า (Customer Credit Exposure Formula):**
   - `OutstandingReceivables` = ผลรวม `BillingDocument.Outstanding` ของทุกใบแจ้งหนี้ที่ยังไม่ `voided` ของโครงการภายใต้ลูกค้ารายนั้น
   - `UnbilledAcceptedAmount` =
     - สำหรับใบเสนอราคาที่ `accepted` (ฉบับล่าสุดที่ไม่ถูกแทนที่) ซึ่งเปิดโครงการ (`Project` ที่ไม่ `cancelled`) แล้ว: `max(0, Project.BaselineContractAmount - Sum(NonVoidedBilling.Amount))`
     - สำหรับใบเสนอราคาที่ `accepted` แต่ยังไม่เปิดโครงการ: `Quotation.TotalAmount`
   - `TotalExposure = OutstandingReceivables + UnbilledAcceptedAmount`
   - เมื่อรับชำระเงิน (`RecordPayment`) ยอด `OutstandingReceivables` จะลดลง ทำให้ `TotalExposure` ลดลงทันที
2. **รายงานอายุหนี้ลูกหนี้ (AR Aging Buckets `0 / 1–30 / 31–60 / 61–90 / 90+` วัน):**
   - คำนวณวันค้างชำระ (`DaysOverdue`) เทียบกับ `asOfDate` (ค่าเริ่มต้น `Today`):
     - หากใบแจ้งหนี้มี `DueDate`: `DaysOverdue = asOfDate.DayNumber - DueDate.Value.DayNumber`
     - หากใบแจ้งหนี้ไม่ระบุ `DueDate`: ใช้ `IssuedDate + Customer.CreditTermDays` เป็นวันครบกำหนดโดยอนุโลม
   - แบ่งออกเป็น 5 ช่วงอายุหนี้:
     - `Current` (`DaysOverdue <= 0`: ยังไม่ครบกำหนด)
     - `Bucket1To30` (`1 .. 30` วัน)
     - `Bucket31To60` (`31 .. 60` วัน)
     - `Bucket61To90` (`61 .. 90` วัน)
     - `BucketOver90` (`> 90` วัน)
3. **Credit Limit Gate & Credit Override Workflow (Maker–Checker + `G-03c`):**
   - หากลูกค้าไม่ได้ตั้งวงเงินเครดิต (`Customer.CreditLimit == null`) ระบบไม่บล็อกการออกเอกสาร (รักษาความเข้ากันได้ย้อนหลังกับเทสต์เดิม 100%)
   - หากลูกค้าตั้งวงเงินเครดิต (`Customer.CreditLimit.HasValue`) และเมื่อออกใบแจ้งหนี้ใหม่จะทำให้ `ProjectedExposure > EffectiveCreditCeiling` (โดย `EffectiveCreditCeiling = max(Customer.CreditLimit.Value, MaxActiveApprovedOverrideAmount)`):
     - ระบบปฏิเสธด้วย `409 CREDIT_LIMIT_EXCEEDED`
   - ผู้ใช้สามารถสร้างคำขออนุมัติขยายเพดานเครดิต (`POST /api/v1/finance/credit-overrides` พร้อม `Idempotency-Key`): `{ customerId, requestedCeilingAmount, reason, expiresOn? }`
   - ผู้อนุมัติที่เป็นคนละคนกับผู้ขอ (`Maker != Checker` มิฉะนั้นคืน `409 CREDIT_OVERRIDE_SELF_APPROVAL`) และมีวงเงินอนุมัติผ่าน `IApprovalAuthority` (`ApprovalDocumentType.ChangeOrder`) กดอนุมัติผ่าน `POST /api/v1/finance/credit-overrides/{id}/approve` (`If-Match`)

---

## 2. ลำดับงาน (Phases)

- [x] **Pre-work:** สร้างแผน `2026-10-30-g16-credit-exposure-ar-aging.md` และอัปเดต `docs/03-contracts/finance-api-contract.md`
- [ ] **Phase 1 (Backend Domain Architect — `flash`):**
  - สร้าง `CreditLimitOverrideRequest` และ pure calculator `ArAgingCalculator` ใน `TanErp.Domain/Finance/`
  - สร้าง EF Core Migration `AddCreditExposureAndArAgingG16` (`finance.credit_limit_overrides`)
  - อัปเดต `FinanceStore.CreateBillingAsync` ให้ตรวจ Credit Limit Gate เมื่อ `customer.CreditLimit.HasValue`
  - เพิ่ม Endpoints `/api/v1/finance/ar-aging`, `/api/v1/customers/{customerId}/credit-exposure`, `/api/v1/finance/credit-overrides`, `/api/v1/finance/credit-overrides/{id}/approve`
  - เขียน Unit Tests (`FinanceDomainTests`) + Integration Tests (`FinanceEndpointsTests`)
- [ ] **Phase 2 (Main Orchestrator — OpenAPI Bridge):**
  - อัปเดต `contracts/openapi/tan-erp.v1.json` และ `frontend/src/generated/api/tan-erp.v1.ts`
- [ ] **Phase 3 (Frontend UI Specialist — `flash`):**
  - เพิ่มแท็บ/ส่วนแสดงผล **AR Aging & Credit Exposure** และ **Credit Limit Overrides** ในโมดูล Finance (`billing-list.tsx` หรือคอมโพเนนต์ย่อย)
  - อัปเดต `th.json` / `en.json` และ Vitest (`finance-components.test.tsx`)
- [ ] **Phase 4 (Documentation & Verification):**
  - อัปเดต `docs/05-engineering/finance-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/01-business/open-validation-questions.md`
