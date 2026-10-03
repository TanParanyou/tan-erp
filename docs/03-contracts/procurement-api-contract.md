# Procurement API Contract (ข้อตกลง API จัดซื้อ)

**สถานะ:** Implemented 2026-10-04 (CP-10: Supplier → Purchase Order → Goods Receipt). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) ยังไม่ผ่าน Procurement/Finance. ไม่มี Purchase Request, Return, ใบแจ้งหนี้ผู้ขาย และไม่เขียน Stock โดยตรง (Receipt เป็นสัญญาส่งต่อให้ Inventory — CP-11).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| Supplier | Master แยกจาก Cost Source; รหัสออกอัตโนมัติ `SUP-nnnnn` (Document Numbering, master data); เงื่อนไขชำระเงิน 0–365 วัน; ปิด/เปิดใช้ได้ (ไม่ลบ) |
| สินค้าในใบสั่งซื้อ | Item ต้อง `active` และ `canPurchase`; หน่วย = หน่วยฐานของ Item (ยังไม่แปลงหน่วย); ชื่อ/รหัส/หน่วยเป็น Snapshot ในบรรทัด; Item ซ้ำในใบเดียวไม่ได้; 1–200 บรรทัด |
| ราคา | กรอกต่อบรรทัด ไม่ดึงจาก Cost Source (ไม่เดาราคา); จำนวน 4 ทศนิยม ราคา 4 ทศนิยม ยอดบรรทัด 2 ทศนิยม |
| อนุมัติ | ทุกใบต้องอนุมัติ ไม่มีเกณฑ์วงเงิน; **ผู้อนุมัติต้องไม่ใช่ผู้สร้าง** (maker–checker, มี DB check constraint); ปฏิเสธ/ยกเลิกต้องมีเหตุผล |
| รับสินค้า | รับบางส่วนได้; **ห้ามรับเกินจำนวนสั่ง** (ไม่มี tolerance; DB check `received_quantity <= quantity`); ยกเลิกใบที่มีการรับแล้วไม่ได้; ไม่มีการคืน/แก้ใบรับ (immutable) |
| งบโครงการ | ใบที่ผูก Project อนุมัติได้เฉพาะ Project `active|on_hold`; ภาระผูกพัน = ผลรวมใบ `approved|partially_received|received`; อนุมัติแล้วเกิน (งบ Baseline + Change Order ที่อนุมัติ) → `422 PURCHASE_ORDER_OVER_BUDGET`; ใบ `cancelled` ปล่อยภาระผูกพัน. แสดงใน `GET /projects/{id}/control` ที่ `budget.committedAmount`/`availableBudget` |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST` สร้าง/รับสินค้าต้องมี `Idempotency-Key`; แก้/เปลี่ยนสถานะใช้ `If-Match` (row version).

| Action | Method/Path | Permission |
| --- | --- | --- |
| Supplier สร้าง/แก้/เปิด/ปิด | `POST /api/v1/suppliers`, `PUT /{id}`, `POST /{id}/activate`, `/deactivate` | `suppliers.manage` |
| Supplier อ่าน/รายการ | `GET /api/v1/suppliers[/{id}]?search=&status=&page=&pageSize=` | `suppliers.read` |
| PO สร้าง/แก้ฉบับร่าง | `POST /api/v1/purchase-orders`, `PUT /{id}` (แทนที่บรรทัดทั้งชุด) | `purchase-orders.create` |
| PO ส่งอนุมัติ/ยกเลิก | `POST /{id}/submit`, `/cancel` `{note}` | `purchase-orders.create` |
| PO อนุมัติ/ปฏิเสธ | `POST /{id}/approve`, `/reject` `{note}` | `purchase-orders.approve` |
| PO อ่าน/รายการ | `GET /api/v1/purchase-orders[/{id}]?search=&status=&supplierId=&projectId=&page=&pageSize=` | `purchase-orders.read` |
| รับสินค้า | `POST /api/v1/purchase-orders/{id}/receipts` `{receivedAtUtc?, note, lines:[{purchaseOrderLineId, quantity}]}` | `goods-receipts.create` |

PO status: `draft → submitted → approved | rejected`; `approved → partially_received → received`; `draft|submitted|approved → cancelled` (ถ้ายังไม่มีการรับ). แก้ไขได้เฉพาะ `draft` (Supplier เปลี่ยนไม่ได้). เลขที่: `PO-{YYYY}-{SEQ:4}`, `GR-{YYYY}-{SEQ:4}` (Document Numbering `purchase-orders`, `goods-receipts`).

Receipt response คือ PO ที่อัปเดตแล้ว (บรรทัดมี `receivedQuantity`/`remainingQuantity`, `receipts[]` สรุปใบรับ). Replay Key+payload เดิมคืนผลเดิมโดยไม่รับซ้ำ; Key เดิม payload ต่างคืน `409 IDEMPOTENCY_KEY_REUSED`. รับพร้อมกันบนใบเดียว: ผู้แพ้ได้ `409 PURCHASE_ORDER_VERSION_CONFLICT` (อ่านใหม่แล้วลองอีกครั้ง) และไม่มีการเขียนบางส่วน (ทุกอย่างอยู่ใน Transaction เดียว).

## Errors

`409`: `SUPPLIER_CODE_CONFLICT`, `SUPPLIER_VERSION_CONFLICT`, `SUPPLIER_INVALID_STATE`, `PURCHASE_ORDER_VERSION_CONFLICT`, `PURCHASE_ORDER_INVALID_STATE`, `PURCHASE_ORDER_HAS_RECEIPTS`, `PURCHASE_ORDER_SUPPLIER_INACTIVE`, `PURCHASE_ORDER_PROJECT_NOT_ACTIVE`. `422`: `SUPPLIER_FIELD_INVALID`, `PURCHASE_ORDER_LINE_INVALID`, `PURCHASE_ORDER_ITEM_NOT_PURCHASABLE`, `PURCHASE_ORDER_OVER_BUDGET`, `PROCUREMENT_REASON_REQUIRED`, `PROCUREMENT_FIELD_REQUIRED/INVALID`, `GOODS_RECEIPT_INVALID`, `GOODS_RECEIPT_OVER_RECEIVED`. `403`: `PURCHASE_ORDER_SELF_APPROVAL`. `404` นอก Organization.

## Data (migration `AddProcurement`, schema `procurement`)

`suppliers` (unique `(organization_id, normalized_code)`), `purchase_orders` (unique number, status/total/decider checks, FK Restrict ไป Supplier/Project/Branch/Users), `purchase_order_lines` (item/unit snapshot, checks `quantity > 0`, `unit_price >= 0`, `0 <= received_quantity <= quantity`), `goods_receipts` (unique number), `goods_receipt_lines` (quantity > 0). Audit: `supplier.*`, `purchase-order.*`, `goods-receipt.posted`.

## Handoff to Inventory (CP-11)

`goods_receipts` + `goods_receipt_lines` (item, unit, quantity, unit price, PO/line/Project/Branch) เป็นแหล่งเดียวที่ Inventory อ่านเพื่อสร้าง Movement; Procurement ไม่เขียนตารางของ Inventory.
