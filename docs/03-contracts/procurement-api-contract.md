# Procurement API Contract (ข้อตกลง API จัดซื้อ)

**สถานะ:** Implemented CP-10 (2026-10-04) + Extended in Wave C `G-11` (2026-10-29: Purchase Request `PR`, Consolidated PR → PO Conversion, Supplier Return `SRN`, Supplier Invoice `SINV` 3-Way Match & AP Outbox). กฎด้านล่างเป็นค่าเริ่มต้น `TEST_ONLY` ที่ได้รับอนุมัติสำหรับการพัฒนา รอยืนยันจากทีมจัดซื้อและบัญชีใน `open-validation-questions.md` (`VQ-PRC-01`..`VQ-PRC-03`).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| Supplier | Master แยกจาก Cost Source; รหัสออกอัตโนมัติ `SUP-nnnnn` (Document Numbering, master data); เงื่อนไขชำระเงิน 0–365 วัน; ปิด/เปิดใช้ได้ (ไม่ลบ) |
| Purchase Request (`PR`) | เลขที่ `PR-{YYYY}-{SEQ:4}` (`purchase-requests`); ผูก Branch และระบุ Project ได้; สถานะ `draft → submitted → approved \| rejected`, `draft\|submitted\|approved → cancelled`, `approved → converted`; อนุมัติแบบ Maker ≠ Checker + ตรวจวงเงินอนุมัติ (`ApprovalAuthority` ประเภท `purchase-order`); รวมหลายใบ `approved` (สาขาเดียวกัน และไม่ข้ามโครงการ) แปลงเป็น `PO` ร่างใบเดียวได้ครั้งเดียว |
| สินค้าในใบขอซื้อ/ใบสั่งซื้อ | Item ต้อง `active` และ `canPurchase`; หน่วย = หน่วยฐานของ Item; ชื่อ/รหัส/หน่วยเป็น Snapshot ในบรรทัด; Item ซ้ำในใบเดียวไม่ได้; 1–200 บรรทัด |
| ราคา | กรอกต่อบรรทัด; จำนวน 4 ทศนิยม ราคา 4 ทศนิยม ยอดบรรทัด 2 ทศนิยม |
| อนุมัติ PO | ผู้อนุมัติต้องไม่ใช่ผู้สร้าง (Maker ≠ Checker, มี DB check constraint) + ตรวจวงเงินอนุมัติ `ApprovalAuthority`; ปฏิเสธ/ยกเลิกต้องมีเหตุผล |
| รับสินค้า (`GR`) & คืนสินค้าผู้ขาย (`SRN`) | รับบางส่วนได้ ห้ามรับเกินจำนวนสั่ง (`received_quantity <= quantity`); คืนสินค้าผู้ขายอ้างอิงใบ `GR` เลขที่ `SRN-{YYYY}-{SEQ:4}` (`supplier-returns`) ต้องระบุเหตุผล ห้ามคืนเกินจำนวนที่รับสุทธิใน `GR` และห้ามคืนส่วนที่ออกใบแจ้งหนี้แล้ว (`received_quantity - returnQty >= invoiced_quantity`); หาก `GR` รับเข้าคลังแล้ว ระบบตัดสต็อก `return_out` อัตโนมัติผ่าน `IProcurementStockPort` ใน Transaction เดียวกัน และคืนยอด Commitment ของโครงการ (`+returnValue`) |
| ใบแจ้งหนี้ผู้ขาย (`SINV`) & 3-Way Match | เลขที่ระบบ `SINV-{YYYY}-{SEQ:4}` (`supplier-invoices`) คู่กับเลขใบแจ้งหนี้ผู้ขาย (`supplierInvoiceNumber` ไม่ซ้ำต่อผู้ขายเมื่อยังไม่ Void); ตรวจ **3-Way Match** ตอน Submit: (1) จำนวนบรรทัดห้ามเกินยอดรับสุทธิที่ยังไม่วางบิล (`quantity <= receivedQuantity - invoicedQuantity`, `0%` qty tolerance → เกินคืน `422 SUPPLIER_INVOICE_OVER_RECEIVED`), (2) ส่วนต่างราคาต่อหน่วยเทียบ PO ไม่เกิน `2%` (`<= 0.02`) → ผ่านเป็น `matched` ทันที; หากเกิน `2%` → พักเป็น `on_hold` แจ้งเตือนผู้อนุมัติ (`purchase-orders.approve`, Maker ≠ Checker + `ApprovalAuthority`) เพื่อ `approve` หรือ `reject`; เมื่อ `matched`/`approved` จะส่ง `AccountingOutbox` (`supplier-invoice.posted`) และบันทึกเฉพาะ **ผลต่างราคา (Price Variance)** เข้า `ProjectCostLedgerEntries` (`actual`, `supplier_invoice`); เมื่อ `void` จะส่ง `supplier-invoice.voided` คืนยอด `invoicedQuantity` และกลับรายการผลต่างราคา |
| งบโครงการ | PO ที่ผูก Project อนุมัติได้เฉพาะ Project `active\|on_hold`; ภาระผูกพันสุทธิคำนวณผ่าน `ProjectCostLedgerEntries` (`commitment` ตอนอนุมัติ PO, ลดเมื่อรับ `GR`, คืนเพิ่มเมื่อคืนของ `SRN`, กลับรายการเมื่อยกเลิก PO) |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST` สร้างเอกสาร/รับสินค้า/คืนสินค้า/แปลง PR ต้องมี `Idempotency-Key`; แก้ไข/เปลี่ยนสถานะใช้ `If-Match` (row version).

| Action | Method/Path | Permission |
| --- | --- | --- |
| Supplier สร้าง/แก้/เปิด/ปิด | `POST /api/v1/suppliers`, `PUT /{id}`, `POST /{id}/activate`, `/deactivate` | `suppliers.manage` |
| Supplier อ่าน/รายการ | `GET /api/v1/suppliers[/{id}]?search=&status=&page=&pageSize=` | `suppliers.read` |
| PR สร้าง/แก้ฉบับร่าง | `POST /api/v1/purchase-requests`, `PUT /{id}` | `purchase-orders.create` |
| PR ส่งอนุมัติ/ยกเลิก | `POST /api/v1/purchase-requests/{id}/submit`, `/cancel` `{note}` | `purchase-orders.create` |
| PR อนุมัติ/ปฏิเสธ | `POST /api/v1/purchase-requests/{id}/approve`, `/reject` `{note}` | `purchase-orders.approve` |
| PR แปลงรวมเป็น PO ร่าง | `POST /api/v1/purchase-requests/convert` `{purchaseRequestIds, supplierId, expectedDate?, note?, unitPrices?}` | `purchase-orders.create` |
| PR อ่าน/รายการ | `GET /api/v1/purchase-requests[/{id}]?search=&status=&projectId=&page=&pageSize=` | `purchase-orders.read` |
| PO สร้าง/แก้ฉบับร่าง | `POST /api/v1/purchase-orders`, `PUT /{id}` | `purchase-orders.create` |
| PO ส่งอนุมัติ/ยกเลิก | `POST /api/v1/purchase-orders/{id}/submit`, `/cancel` `{note}` | `purchase-orders.create` |
| PO อนุมัติ/ปฏิเสธ | `POST /api/v1/purchase-orders/{id}/approve`, `/reject` `{note}` | `purchase-orders.approve` |
| PO อ่าน/รายการ | `GET /api/v1/purchase-orders[/{id}]?search=&status=&supplierId=&projectId=&page=&pageSize=` | `purchase-orders.read` |
| รับสินค้า (`GR`) | `POST /api/v1/purchase-orders/{id}/receipts` `{receivedAtUtc?, note, lines:[{purchaseOrderLineId, quantity}]}` | `goods-receipts.create` |
| คืนสินค้าผู้ขาย (`SRN`) | `POST /api/v1/purchase-orders/{id}/returns` `{goodsReceiptId, returnedAtUtc?, reason, lines:[{goodsReceiptLineId, quantity}]}` | `goods-receipts.create` |
| SINV สร้าง/แก้ฉบับร่าง | `POST /api/v1/supplier-invoices`, `PUT /{id}` | `purchase-orders.create` |
| SINV ส่งตรวจ 3-Way Match / ยกเลิกร่าง / Void | `POST /api/v1/supplier-invoices/{id}/submit`, `/cancel` `{note}`, `/void` `{note}` | `purchase-orders.create` |
| SINV อนุมัติ/ปฏิเสธใบที่ติด Hold | `POST /api/v1/supplier-invoices/{id}/approve`, `/reject` `{note}` | `purchase-orders.approve` |
| SINV อ่าน/รายการ | `GET /api/v1/supplier-invoices[/{id}]?search=&status=&supplierId=&purchaseOrderId=&page=&pageSize=` | `purchase-orders.read` |

## Errors

- `409`: `SUPPLIER_CODE_CONFLICT`, `SUPPLIER_VERSION_CONFLICT`, `SUPPLIER_INVALID_STATE`, `PURCHASE_REQUEST_VERSION_CONFLICT`, `PURCHASE_REQUEST_INVALID_STATE`, `PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE`, `PURCHASE_ORDER_VERSION_CONFLICT`, `PURCHASE_ORDER_INVALID_STATE`, `PURCHASE_ORDER_HAS_RECEIPTS`, `PURCHASE_ORDER_SUPPLIER_INACTIVE`, `PURCHASE_ORDER_PROJECT_NOT_ACTIVE`, `SUPPLIER_INVOICE_VERSION_CONFLICT`, `SUPPLIER_INVOICE_INVALID_STATE`, `SUPPLIER_INVOICE_DUPLICATE_NUMBER`.
- `422`: `SUPPLIER_FIELD_INVALID`, `PURCHASE_REQUEST_LINE_INVALID`, `PURCHASE_ORDER_LINE_INVALID`, `PURCHASE_ORDER_ITEM_NOT_PURCHASABLE`, `PURCHASE_ORDER_OVER_BUDGET`, `PROCUREMENT_REASON_REQUIRED`, `PROCUREMENT_FIELD_REQUIRED`, `PROCUREMENT_FIELD_INVALID`, `GOODS_RECEIPT_INVALID`, `GOODS_RECEIPT_OVER_RECEIVED`, `SUPPLIER_RETURN_INVALID`, `SUPPLIER_RETURN_OVER_RETURNED`, `SUPPLIER_RETURN_ALREADY_INVOICED`, `INVENTORY_INSUFFICIENT_STOCK`, `SUPPLIER_INVOICE_LINE_INVALID`, `SUPPLIER_INVOICE_OVER_RECEIVED`.
- `403`: `PURCHASE_REQUEST_SELF_APPROVAL`, `PURCHASE_ORDER_SELF_APPROVAL`, `SUPPLIER_INVOICE_SELF_APPROVAL`, `APPROVAL_LIMIT_EXCEEDED`, `APPROVAL_LIMIT_NOT_CONFIGURED`.
- `404`: `RESOURCE_NOT_FOUND` (ไม่พบหรืออยู่นอก Organization).

## Data (migrations `AddProcurement`, `AddProcurementCompletionG11`, schema `procurement`)

- `suppliers`, `purchase_requests`, `purchase_request_lines`, `purchase_orders`, `purchase_order_lines` (`received_quantity`, `invoiced_quantity` พร้อม check `0 <= invoiced_quantity <= received_quantity <= quantity`), `goods_receipts`, `goods_receipt_lines` (`returned_quantity` พร้อม check `0 <= returned_quantity <= quantity`), `supplier_returns`, `supplier_return_lines`, `supplier_invoices`, `supplier_invoice_lines`.
- Audit: `supplier.*`, `purchase-request.*`, `purchase-order.*`, `goods-receipt.posted`, `supplier-return.posted`, `supplier-invoice.*`.
- Outbox (`finance.accounting_outbox`): `supplier-invoice.posted`, `supplier-invoice.voided`.
