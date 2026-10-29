# Procurement Verification (CP-10 + Wave C `G-11`)

**สถานะ:** Implemented CP-10 (2026-10-04) + Extended in Wave C `G-11` (2026-10-29); หลักฐานเป็นชุดทดสอบอัตโนมัติ ไม่ใช่ UAT/Production sign-off. กฎ: [Procurement API Contract](../03-contracts/procurement-api-contract.md) และ [แผน G-11](../superpowers/plans/2026-10-29-g11-procurement-completion.md).

## ผลที่รันจริง

### 1. CP-10 Baseline (2026-10-04)
- Backend Integration `ProcurementEndpointsTests` 6/6 บน PostgreSQL: Supplier lifecycle + idempotency/ETag/สิทธิ์/ข้าม Organization; PO validation (บรรทัดซ้ำ/จำนวน 0/Item ไม่พบ), แก้ฉบับร่าง, stale version, แก้หลังส่งไม่ได้, maker–checker (ผู้สร้างอนุมัติ → 403), ปฏิเสธต้องมี note, ยกเลิก; Supplier ปิดใช้งานแล้วสร้าง/อนุมัติไม่ได้; รับสินค้าบางส่วน/replay/Key reuse, รับเกินถูกปฏิเสธโดยไม่เขียนบางส่วน, ยกเลิกหลังรับไม่ได้, รับครบ → `received`; Project budget: Project ยัง `planned` อนุมัติไม่ได้, เกินงบ 422, ภาระผูกพัน/งบคงเหลือใน Project Control, ยกเลิกปล่อยภาระผูกพัน.

### 2. Wave C `G-11` — Procurement Completion (2026-10-29)
- **Backend Build & Static Analysis:** `dotnet build backend/TanErp.slnx` → **0 warnings, 0 errors**.
- **Backend Unit Tests (`TanErp.UnitTests`):** **671/671 passed** (รวม `ProcurementCompletionDomainTests`: PR lifecycle & maker–checker, Supplier Return over-return & already-invoiced guards, Supplier Invoice 3-way match `0%` qty / `2%` price variance boundary `<= 0.02m` vs `> 0.02m`, และ Notification registry parity สำหรับ `purchase-request.approval-requested` และ `supplier-invoice.approval-requested`).
- **Backend Architecture Tests (`TanErp.ArchitectureTests`):** **4/4 passed**.
- **Backend Integration Tests (`TanErp.IntegrationTests` บน PostgreSQL จริง):** **33/33 passed** (`ProcurementEndpointsTests`, `InventoryEndpointsTests`, `FinanceEndpointsTests`, `ProjectControlEndpointsTests`, `OpenApiContractTests`) ครอบคลุม:
  1. **Purchase Request (`PR`) & Consolidated Conversion:** สร้าง PR 2 ใบ → ส่งอนุมัติ → ผู้สร้างอนุมัติเองถูกบล็อก (`403 PURCHASE_REQUEST_SELF_APPROVAL`) → ผู้อนุมัติที่มีวงเงิน (`ApprovalLimit` `purchase-order`) อนุมัติผ่าน → รวมแปลง PR ทั้ง 2 ใบเป็น `PurchaseOrder` ร่างใบเดียวสำเร็จ (รวมจำนวนบรรทัดสินค้าเดียวกันอัตโนมัติ) → พยายามแปลงซ้ำถูกปฏิเสธ (`409 PURCHASE_REQUEST_INVALID_STATE`).
  2. **Supplier Return (`SRN`) & Atomic Inventory Return-Out:** อนุมัติ PO ผูกโครงการ → รับสินค้า (`GR`) → รับเข้าคลัง (`POST /api/v1/inventory/receipts`) → บันทึกคืนสินค้าผู้ขาย (`POST /api/v1/purchase-orders/{id}/returns`) → ยืนยันว่าตัดสต็อกคลัง (`return_out`, `StockSourceType.SupplierReturn`) อัตโนมัติใน Transaction เดียวกัน, ลด `receivedQuantity` บน PO (สถานะย้อนกลับเป็น `partially_received`), คืนยอด Commitment ใน `ProjectCostLedgerEntries` (`+returnValue`, `isReversal: true`), บล็อกการคืนเกินยอดรับสุทธิ (`422 SUPPLIER_RETURN_OVER_RETURNED`) และ Rollback ทั้งก้อนเมื่อสต็อกในคลังถูกเบิกไปใช้แล้ว (`422 INVENTORY_INSUFFICIENT_STOCK`).
  3. **Supplier Invoice (`SINV`) & 3-Way Match + AP Outbox:**
     - วางบิลเกินจำนวนรับสุทธิที่ยังไม่วางบิล → `422 SUPPLIER_INVOICE_OVER_RECEIVED` (`0%` qty tolerance).
     - ส่วนต่างราคาต่อหน่วยไม่เกิน `2%` (`<= 0.02`) → เปลี่ยนเป็น `matched` ทันที, สร้าง `AccountingOutboxMessage` (`supplier-invoice.posted`), และบันทึกเฉพาะ **ผลต่างราคา (Price Variance)** เข้า `ProjectCostLedgerEntries` (`actual`, `supplier_invoice`) โดยไม่นับซ้ำต้นทุนฐานของวัสดุ.
     - ส่วนต่างราคาต่อหน่วยเกิน `2%` (`> 0.02`) → พักเป็น `on_hold` (`matchStatus = "price_variance"`), แจ้งเตือนผู้อนุมัติ (`supplier-invoice.approval-requested`), บล็อกผู้สร้างอนุมัติเอง (`403 SUPPLIER_INVOICE_SELF_APPROVAL`), เมื่อผู้อนุมัติกด `approve` → เปลี่ยนเป็น `approved` + ส่ง `supplier-invoice.posted` เข้า Outbox + บันทึกผลต่างราคาเข้า `ProjectCostLedgerEntries`.
     - ยกเลิกใบแจ้งหนี้ที่โพสต์แล้ว (`POST /api/v1/supplier-invoices/{id}/void`) → เปลี่ยนเป็น `voided`, คืนยอด `invoicedQuantity` บนบรรทัด PO, ส่ง `supplier-invoice.voided` เข้า Outbox, และกลับรายการผลต่างราคาใน `ProjectCostLedgerEntries`.
- **Frontend Verification (`frontend/`):**
  - `npx tsc --noEmit` & `npm run lint`: **0 errors, 0 warnings**.
  - `npx vitest run src/features/procurement/ src/lib/notifications/`: **30/30 passed** (ครอบคลุม PR multi-select conversion, Supplier Return form & list, Supplier Invoice 3-way match `on_hold` approval & void, และ Notification types).
  - `npm run build` (`next build`): **Compiled successfully** (รวมหน้า `/procurement/purchase-requests`, `/procurement/purchase-requests/[id]`, `/procurement/supplier-invoices`, `/procurement/supplier-invoices/[id]`).

## ข้อจำกัดและรายการรอยืนยันทางธุรกิจ (`TEST_ONLY`)

- กฎ `TEST_ONLY` สำหรับการรวม PR ภายในโครงการเดียวกัน (`VQ-PRC-01`), การเปิดยอดค้างรับเมื่อคืนของ (`VQ-PRC-02`), และเกณฑ์ 3-Way Match `0%` qty / `2%` unit price tolerance (`VQ-PRC-03`) บันทึกไว้ใน [Open Validation Questions](../01-business/open-validation-questions.md) รอ Procurement/Finance ยืนยันก่อนเปิดใช้งานจริง.
- ยังไม่มี Supplier price list/lead time ต่อ Item (อยู่ในขอบเขต `G-14` MRP completion) และภาษีมูลค่าเพิ่ม/หัก ณ ที่จ่าย/ใบลดหนี้ตามรูปแบบกรมสรรพากร (อยู่ในขอบเขต `G-15` Thai Tax).
