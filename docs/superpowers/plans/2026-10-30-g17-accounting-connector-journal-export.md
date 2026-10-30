# G-17 Chart of Accounts Mapping & Journal Export Connector Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `.agents/skills/tiered-orchestration/SKILL.md` to implement this plan phase-by-phase.

**Goal:** เพิ่มตารางผังบัญชีเชื่อมโยงประเภทธุรกรรม (Chart of Accounts Mapping: `OutboxKind → Debit/Credit Account`), ระบบส่งออกสมุดรายวัน CSV (`JournalExport` สถานะ `exported` แยกจาก `sent` โดยไม่ส่งออกซ้ำ และพักเป็น `dead` พร้อมเหตุผล `ACCOUNT_MAPPING_MISSING:<kind>` เมื่อขาดการตั้งค่าผังบัญชี), และ `ConfigurableHttpAccountingConnector` ที่ส่ง `DedupeKey` เป็น `Idempotency-Key` โดยอ่าน URL/Secret จาก Configuration

**Architecture:** Clean Architecture (`Domain` → `Application` → `Infrastructure` → `Api`) + Next.js App Router (`frontend/src/features/finance/`)

---

## 1. กฎธุรกิจและค่าเริ่มต้น (`TEST_ONLY`)

1. **Chart of Accounts Mapping (`finance.account_mappings`):**
   - ผูก `(OrganizationId, Kind)` (โดย `Kind` ต้องอยู่ใน `OutboxKind.All`) เข้ากับ:
     - `DebitAccountCode` (`1..32` ตัวอักษร เช่น `"1130"`) และ `DebitAccountName` (`1..200` ตัวอักษร)
     - `CreditAccountCode` (`1..32` ตัวอักษร เช่น `"4100"`) และ `CreditAccountName` (`1..200` ตัวอักษร)
   - แก้ไขได้โดยผู้มีสิทธิ์ `finance-sync.run` ผ่าน `PUT /api/v1/finance/account-mappings` และอ่านผ่าน `GET /api/v1/finance/account-mappings` (`finance-sync.read`)
2. **Journal Export (`POST /api/v1/finance/outbox/export-journal`):**
   - ดึงรายการใน `finance.accounting_outbox` ที่มีสถานะ `pending` หรือ `failed` ด้วย `FOR UPDATE SKIP LOCKED`
   - สำหรับแต่ละรายการ:
     - หากพบ `AccountMapping` ของ `message.Kind`:
       - สร้างบรรทัดสมุดรายวันคู่บัญชี (Dr. `DebitAccountCode` / Cr. `CreditAccountCode` จำนวน `message.Amount` อ้างอิง `DedupeKey` และ `ResourceNumber`)
       - เปลี่ยนสถานะข้อความเป็น `OutboxStatus.Exported` (`"exported"`, แยกจาก `"sent"`) พร้อมบันทึก `ExternalRef = batchRef`
       - การกด Export ซ้ำในรอบถัดไปจะไม่ดึงรายการที่เป็น `exported` ซ้ำอีก (`export ซ้ำไม่ซ้ำรายการ`)
     - หากไม่พบ `AccountMapping` ของ `message.Kind`:
       - เปลี่ยนสถานะข้อความเป็น `OutboxStatus.Dead` (`"dead"`) ทันที พร้อมบันทึก `LastError = "ACCOUNT_MAPPING_MISSING:<kind>"` (`mapping ขาด → dead พร้อมเหตุผล`)
       - เมื่อตั้งค่าผังบัญชีครบแล้ว ผู้ใช้สามารถกด `Requeue` เพื่อนำกลับมา Export ใหม่ได้
   - คืนค่า `JournalExportResult` พร้อม `csvContent` ให้ดาวน์โหลดเป็นไฟล์ `.csv` นำเข้าระบบบัญชีภายนอกได้ทันที

---

## 2. ลำดับงาน (Phases)

- [x] **Pre-work:** สร้างแผน `2026-10-30-g17-accounting-connector-journal-export.md` และอัปเดต `docs/03-contracts/finance-api-contract.md`
- [ ] **Phase 1 (Backend Domain Architect — `flash`):**
  - เพิ่ม `OutboxStatus.Exported = "exported"` และ Entity `AccountMapping` ใน `TanErp.Domain/Finance/`
  - เพิ่ม `MarkExported` และ `MarkDeadMissingMapping` บน `AccountingOutboxMessage`
  - สร้าง EF Core Migration `AddJournalExportAndCoaMappingG17`
  - เพิ่ม Endpoints `GET /api/v1/finance/account-mappings`, `PUT /api/v1/finance/account-mappings`, `POST /api/v1/finance/outbox/export-journal` ใน `FinanceSyncController`
  - เขียน Unit Tests (`FinanceDomainTests`) + Integration Tests (`FinanceEndpointsTests`)
- [ ] **Phase 2 (Main Orchestrator — OpenAPI Bridge):**
  - อัปเดต `contracts/openapi/tan-erp.v1.json` และ `frontend/src/generated/api/tan-erp.v1.ts`
- [ ] **Phase 3 (Frontend UI Specialist — `flash`):**
  - อัปเดต `finance-sync-page.tsx` เพิ่มส่วนตั้งค่าผังบัญชี (Chart of Accounts Mapping) และปุ่มส่งออกสมุดรายวัน CSV (`Export Journal CSV`) พร้อมรองรับสถานะ `exported`
  - อัปเดต `th.json` / `en.json` และ Vitest (`finance-components.test.tsx`)
- [ ] **Phase 4 (Documentation & Verification):**
  - อัปเดต `docs/05-engineering/finance-verification.md`, `docs/00-overview/implementation-roadmap.md`, `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/01-business/open-validation-questions.md`
