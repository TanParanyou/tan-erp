# Inventory API Contract (ข้อตกลง API คลังสินค้า)

**สถานะ:** Implemented 2026-10-04 (CP-11: Warehouse, ledger, balance, reservation, receive/issue/transfer/adjust). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) ยังไม่ผ่าน Warehouse/Finance; ยังไม่ผ่าน Stock Accuracy UAT.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| โครงสร้างคลัง | Warehouse เดียวระดับสาขา (ยังไม่มี location/bin); รหัสออกอัตโนมัติ `WH-nnnnn`; ปิดใช้ได้เมื่อไม่มีสต็อก/การจองคงเหลือ (`WAREHOUSE_HAS_STOCK`) |
| หน่วยและสินค้า | หน่วยของสต็อก = หน่วยฐานของ Item (ไม่แปลงหน่วย); Item ต้อง `canStock`; ไม่มี lot/serial |
| การตีมูลค่า | **Moving weighted average** ต่อ (คลัง, Item): รับเข้าเพิ่มมูลค่าตามต้นทุนรับ; เบิก/โอนออกที่ต้นทุนเฉลี่ยปัจจุบัน; หน่วยสุดท้ายรับมูลค่าที่เหลือทั้งหมด (ไม่มีเศษ); โอนย้ายมูลค่าเท่าเดิม |
| สต็อกติดลบ | **ห้าม** (`INVENTORY_INSUFFICIENT_STOCK`; ซ้ำด้วย DB check `on_hand >= 0`, `reserved <= on_hand`) |
| การจอง | จองต่อ (คลัง, Item, Project) ลด `available = onHand − reserved`; เบิกให้ Project ใช้การจองของ Project นั้นก่อน; คนอื่นเบิกได้เฉพาะ available; ปล่อยการจองได้ |
| รับเข้า | สร้างจาก Goods Receipt (Procurement) เท่านั้น เลือกคลังที่อยู่สาขาเดียวกับใบรับ; **หนึ่งใบรับเข้าได้ครั้งเดียว** (unique `(org, source_type, source_id)`); ต้นทุนรับ = ราคาต่อหน่วยในใบสั่งซื้อ |
| ปรับยอด | เทียบจำนวนที่นับกับระบบ แล้วสร้าง `adjustment_in/out` ตามส่วนต่าง; ต้องระบุเหตุผล; เพิ่มที่ต้นทุนเฉลี่ย (ยอดเป็น 0 ต้องระบุต้นทุน); ลดต่ำกว่ายอดจองไม่ได้; ไม่มีส่วนต่าง → `INVENTORY_NO_CHANGE` |
| วันที่/ย้อนหลัง | บันทึกที่เวลาปัจจุบัน (ยกเว้นรับเข้าใช้เวลารับสินค้า); ไม่มี Backdate/ปิดงวดในรอบนี้ — ledger เรียงตามเวลาบันทึก |
| Concurrency | ทุกการบันทึกล็อกแถว balance (`SELECT … FOR UPDATE`) ตามลำดับ (คลัง, Item) ใน Transaction เดียว + optimistic `row_version` → ไม่ oversell, ไม่ deadlock; โอนล็อกสองฝั่งพร้อมกัน |
| Ledger | `stock_movements` เป็น append-only (ไม่มี endpoint แก้/ลบ); แต่ละแถวเก็บ `quantityDelta`, `unitCost`, `valueDelta`, `onHandAfter` |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST` ที่บันทึกเอกสารต้องมี `Idempotency-Key`; warehouse update/activate ใช้ `If-Match`.

| Action | Method/Path | Permission |
| --- | --- | --- |
| Warehouse | `POST/PUT /api/v1/warehouses[/{id}]`, `POST /{id}/activate|deactivate` | `warehouses.manage` |
| Warehouse อ่าน/รายการ | `GET /api/v1/warehouses[/{id}]?search=&status=&page=&pageSize=` | `warehouses.read` |
| รับเข้าจาก Goods Receipt | `POST /api/v1/inventory/receipts` `{goodsReceiptId, warehouseId}` | `inventory.receive` |
| เบิก | `POST /api/v1/inventory/issues` `{warehouseId, projectId?, reason?, lines:[{itemId, quantity}]}` | `inventory.issue` |
| โอน | `POST /api/v1/inventory/transfers` `{fromWarehouseId, toWarehouseId, reason?, lines}` | `inventory.transfer` |
| ปรับยอด | `POST /api/v1/inventory/adjustments` `{warehouseId, reason, lines:[{itemId, countedQuantity, unitCost?}]}` | `inventory.adjust` |
| จอง/ปล่อย | `POST /api/v1/inventory/reservations`, `POST /reservations/{id}/release` `{expectedVersion}` | `inventory.reserve` |
| อ่าน | `GET /api/v1/inventory/balances`, `/movements`, `/reservations`, `/documents/{id}`, `/reconciliation` | `inventory.read` |

เลขเอกสาร: `SR`/`SI`/`ST`/`SA-{YYYY}-{SEQ:4}` (Document Numbering `stock-receipts|issues|transfers|adjustments`). Response ของการบันทึกคือ `StockDocumentResponse` (header + movements). Replay Key+payload เดิมคืนเอกสารเดิมโดยไม่บันทึกซ้ำ; Key เดิม payload ต่าง `409 IDEMPOTENCY_KEY_REUSED`. `GET /reconciliation` เทียบ `on_hand`/มูลค่าใน balance กับผลรวมจาก ledger ต่อ (คลัง, Item) และบอก `isConsistent`.

## Errors

`409`: `WAREHOUSE_CODE_CONFLICT`, `WAREHOUSE_VERSION_CONFLICT`, `WAREHOUSE_INVALID_STATE`, `WAREHOUSE_HAS_STOCK`, `INVENTORY_WAREHOUSE_INACTIVE`, `INVENTORY_RECEIPT_ALREADY_POSTED`, `INVENTORY_PROJECT_NOT_ACTIVE`, `INVENTORY_RESERVATION_INVALID`, `INVENTORY_RESERVATION_VERSION_CONFLICT`, `INVENTORY_VERSION_CONFLICT`. `422`: `INVENTORY_INSUFFICIENT_STOCK`, `INVENTORY_ITEM_NOT_STOCKABLE`, `INVENTORY_QUANTITY_INVALID`, `INVENTORY_COST_INVALID/REQUIRED`, `INVENTORY_NO_CHANGE`, `INVENTORY_LINE_INVALID`, `INVENTORY_REASON_REQUIRED`, `INVENTORY_TRANSFER_SAME_WAREHOUSE`, `INVENTORY_WAREHOUSE_BRANCH_MISMATCH`, `WAREHOUSE_FIELD_INVALID`, `INVENTORY_FIELD_REQUIRED/INVALID`. `404` นอก Organization.

## Data (migration `AddInventory`, schema `inventory`)

`warehouses`, `stock_balances` (unique `(org, warehouse, item)`, check on_hand/reserved/total_value), `stock_documents` (unique number; filtered unique `(org, source_type, source_id)`), `stock_movements` (append-only, checks), `stock_reservations`. Audit: `warehouse.*`, `stock-receipt|issue|transfer|adjustment.posted`, `stock-reservation.*`.

## Hand-offs

รับจาก [Procurement](procurement-api-contract.md): Goods Receipt → Stock Receipt (หน้าใบสั่งซื้อแสดงเลขเอกสารสต็อกต่อใบรับ). ส่งให้ Production/MRP (CP-12/13): `available`, `onHand`, `reserved`, ต้นทุนเฉลี่ย และ ledger เป็น input ที่อ่านได้ ไม่มีโมดูลอื่นเขียนตาราง `inventory.*` ตรง.
