# Wave C `G-11` — Procurement Completion Implementation Plan

> **Status:** Approved for Implementation (`TEST_ONLY` business defaults approved by user)
> **Scope:** Purchase Request (`PR`), Consolidated PR → PO Conversion, Supplier Return (`SRN`) with Atomic Inventory Return-Out & Commitment Restoration, and Supplier Invoice (`SINV`) with 3-Way Match (`0%` over-received qty tolerance, `2%` unit price variance tolerance, `on_hold` approver release, Accounting Outbox AP sync, and Project Cost Ledger price-variance posting).

---

## 1. Approved Business Rules (`TEST_ONLY`)

1. **Purchase Request (`PR`):**
   - Numbering: `DocumentTypes.PurchaseRequests = "purchase-requests"` (`PR-{YYYY}-{SEQ:4}`).
   - Status lifecycle: `draft → submitted → approved | rejected`, `draft|submitted|approved → cancelled`, `approved → converted`.
   - Approval: Requires `purchase-orders.approve`, Maker ≠ Checker (`decided_by_user_id <> created_by_user_id`), and `IApprovalAuthority.EnsureAsync` (`ApprovalDocumentType.PurchaseOrder`, total estimated amount).
   - Notification: Submitting publishes `purchase-request.approval-requested` (`TargetPermission: "purchase-orders.approve"`, deep link `/procurement/purchase-requests/{resourceId}`).
   - Consolidation / Conversion (`POST /api/v1/purchase-requests/convert`):
     - Accepts `1..50` `approved` PR IDs + `supplierId` + optional `expectedDate`, `note`, and optional `unitPriceOverrides` per item (`itemId -> unitPrice`, falling back to weighted/max estimated unit price from PR lines).
     - All PRs must belong to the same `BranchId`, and all non-null `ProjectId` values must refer to the same project (cannot mix two different projects).
     - Consolidates duplicate `ItemId` lines by summing `Quantity`, creates a single `draft` `PurchaseOrder` in the same DB transaction, links each PR (`ConvertedPurchaseOrderId = po.Id`, `Status = "converted"`), and prevents re-conversion (`409 PURCHASE_REQUEST_INVALID_STATE`).

2. **Supplier Return (`SRN`):**
   - Numbering: `DocumentTypes.SupplierReturns = "supplier-returns"` (`SRN-{YYYY}-{SEQ:4}`).
   - Endpoint: `POST /api/v1/purchase-orders/{id}/returns` (`goods-receipts.create`, `Idempotency-Key` + `If-Match` PO `RowVersion`).
   - Posted against a specific `GoodsReceipt` (`goodsReceiptId`) belonging to the PO, with mandatory `reason` (1–500 chars) and `lines: [{ goodsReceiptLineId, quantity }]`.
   - Guards:
     - `quantity > 0` and `quantity <= (grLine.Quantity - grLine.ReturnedQuantity)` (`422 SUPPLIER_RETURN_OVER_RETURNED`).
     - Cannot reduce `poLine.ReceivedQuantity` below `poLine.InvoicedQuantity` (`422 SUPPLIER_RETURN_ALREADY_INVOICED`).
   - State updates (single PostgreSQL transaction):
     - Increments `grLine.ReturnedQuantity` and decrements `poLine.ReceivedQuantity` (re-opening `RemainingQuantity` and moving PO status from `received` back to `partially_received` or `approved` if net received becomes `0`).
     - **Atomic Inventory Deduction:** Checks `StockDocuments` (`SourceType == "goods_receipt" && SourceId == receipt.Id`). If the GR was already received into warehouse stock, invokes `IProcurementStockPort.ReturnToSupplierAsync(...)` in the same transaction to issue `return_out` movements (`StockDocumentType.Return`, `SourceType == "supplier_return"`) and deduct warehouse `OnHand` (failing with `422 INVENTORY_INSUFFICIENT_STOCK` if stock was already consumed).
     - **Project Cost Ledger Commitment Restoration:** If `order.ProjectId` is set, appends a `ProjectCostLedgerEntry` (`Commitment`, `SupplierReturn`, `+returnValue`, `isReversal: true`) to restore open PO commitment reduced at GR time.

3. **Supplier Invoice (`SINV`) & 3-Way Match:**
   - Numbering: `DocumentTypes.SupplierInvoices = "supplier-invoices"` (`SINV-{YYYY}-{SEQ:4}`).
   - Unique supplier invoice number per `(organization_id, supplier_id, normalized_supplier_invoice_number)` when active (non-voided).
   - Status lifecycle:
     - `draft → matched | on_hold` (on `POST /api/v1/supplier-invoices/{id}/submit`)
     - `on_hold → approved | rejected` (`POST /{id}/approve` or `POST /{id}/reject` with `purchase-orders.approve`, Maker ≠ Checker, and `IApprovalAuthority.EnsureAsync`)
     - `draft → cancelled` (`POST /{id}/cancel`), `matched | approved → voided` (`POST /{id}/void` with mandatory reason).
   - **3-Way Match Rules (evaluated on `submit`):**
     - **Quantity (`0%` over-invoice tolerance):** Each line references `purchaseOrderLineId`; `invoiceLine.Quantity <= (poLine.ReceivedQuantity - poLine.InvoicedQuantity)`. Exceeding uninvoiced net received quantity fails immediately with `422 SUPPLIER_INVOICE_OVER_RECEIVED`.
     - **Unit Price (`2%` variance tolerance):**
       - When `poLine.UnitPrice == 0`: variance exceeds tolerance if `invoiceLine.UnitPrice > 0`.
       - When `poLine.UnitPrice > 0`: `variancePct = |invoiceLine.UnitPrice - poLine.UnitPrice| / poLine.UnitPrice`.
       - Every line within `<= 0.02m` (`2%`): reserves `poLine.InvoicedQuantity += qty`, transitions directly to `matched`, enqueues `AccountingOutboxMessage` (`kind: "supplier-invoice.posted"`), and posts non-zero price variance to `ProjectCostLedgerEntries`.
       - Any line `> 0.02m` (`> 2%`): reserves `poLine.InvoicedQuantity += qty` (preventing double-invoicing while held), transitions to `on_hold` (`MatchStatus = "price_variance"`), and publishes `supplier-invoice.approval-requested`.
       - When an approver approves `on_hold`: transitions to `approved`, enqueues `AccountingOutboxMessage` (`kind: "supplier-invoice.posted"`), and posts non-zero price variance to `ProjectCostLedgerEntries`.
       - When an approver rejects `on_hold` (or user voids `matched`/`approved`): releases `poLine.InvoicedQuantity -= qty`; for `voided`, also enqueues `AccountingOutboxMessage` (`kind: "supplier-invoice.voided"`) and posts reversal `ProjectCostLedgerEntry` (`actual`, `-varianceAmount`, `isReversal: true`) if variance had been posted.

---

## 2. Execution Phases (Tiered Subagent Orchestration)

- **Phase 1 — Backend Specialist (`Model: "inherit"`, `backend/` only):**
  - Domain models, ports (`IProcurementStockPort`), value sets, EF Core configurations & migration `AddProcurementCompletionG11` (including updated PostgreSQL `CHECK` constraints on `finance.accounting_outbox` and `inventory.stock_movements`), `ProcurementStore`, `ProcurementHandler`, API controllers (`PurchaseRequestsController`, `SupplierInvoicesController`, `PurchaseOrdersController`), localized error codes (`Errors.resx` & `Errors.en.resx`), Unit tests, and Integration tests.
- **Phase 2 — OpenAPI Contract Bridge (Main Orchestrator):**
  - `UPDATE_OPENAPI=1 dotnet test ... OpenApiContractTests` + `cd frontend && npm run generate:api`.
- **Phase 3 — Frontend UI Specialist (`Model: "flash"`, `frontend/` only):**
  - API client & React Query hooks, notification registry (`purchase-request.approval-requested`, `supplier-invoice.approval-requested`), Purchase Requests pages + consolidation modal, Supplier Returns section in PO Detail, Supplier Invoices pages with 3-way match badges & hold approval, `SidebarNav.tsx` entries, `th.json`/`en.json` parity, and Vitest tests.
- **Phase 4 — Verification Gates & Documentation (Main Orchestrator):**
  - Full backend + frontend test/lint/build verification and authoritative documentation updates.
