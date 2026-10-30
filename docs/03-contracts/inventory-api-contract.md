# Inventory API Contract (ข้อตกลง API คลังสินค้า)

**สถานะ:** Implemented CP-11 (2026-10-04) + Extended in Wave C `G-12` (2026-10-29: Warehouse Locations/Bins, Lot/Serial Tracking, Customer/Project Stock Return, และ Inventory Period Close). กฎด้านล่างเป็นค่าเริ่มต้น `TEST_ONLY` ที่ได้รับอนุมัติสำหรับการพัฒนา รอยืนยันจากฝ่ายคลังสินค้าและบัญชีใน `open-validation-questions.md` (`VQ-INV-01`..`VQ-INV-03`).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| โครงสร้างคลัง & Location/Bin | Warehouse ระดับสาขา (`WH-nnnnn`); ทุก Warehouse มี Location มาตรฐานรหัส `DEFAULT` อัตโนมัติ (ห้ามปิดใช้) และสร้าง Bin/Location ย่อยเพิ่มได้; ปิดใช้ Warehouse หรือ Location ได้เมื่อไม่มีสต็อกคงเหลือ (`WAREHOUSE_HAS_STOCK`, `LOCATION_HAS_STOCK`); หากไม่ระบุ `locationId` ตอนลงรายการ ระบบใช้ `DEFAULT` อัตโนมัติ |
| หน่วยและ Lot/Serial | หน่วยของสต็อก = หน่วยฐานของ Item (`canStock`); Item ตั้งค่า `trackLot` และ `trackSerial` ได้ใน `capabilities`: (1) ถ้า `trackLot = true` ต้องระบุ `lotNumber` ทุกครั้งที่เคลื่อนไหว (`422 INVENTORY_LOT_REQUIRED`) และห้ามสต็อกติดลบต่อ `(warehouse, location, item, lot)`, (2) ถ้า `trackSerial = true` จำนวนต้องเป็นจำนวนเต็มและส่ง `serialNumbers` เท่าจำนวนจริง (`422 INVENTORY_SERIAL_COUNT_MISMATCH`) โดย Serial ไม่ซ้ำต่อ Item ทั้งองค์กร (`409 INVENTORY_SERIAL_DUPLICATE`) และเบิก/โอนได้เฉพาะ Serial ที่ `in_stock` ใน Location/Lot ต้นทาง (`422 INVENTORY_SERIAL_NOT_AVAILABLE`) |
| การตีมูลค่า | **Moving weighted average** ต่อ (คลัง, Item): รับเข้าเพิ่มมูลค่าตามต้นทุนรับ; เบิก/โอนออกที่ต้นทุนเฉลี่ยปัจจุบัน; หน่วยสุดท้ายรับมูลค่าที่เหลือทั้งหมด; โอนย้ายมูลค่าเท่าเดิม |
| สต็อกติดลบ | **ห้าม** ทั้งระดับ `(warehouse, item)` และระดับ `(warehouse, location, item, lot)` (`422 INVENTORY_INSUFFICIENT_STOCK`; ซ้ำด้วย DB check `on_hand >= 0`) |
| การจอง | จองต่อ (คลัง, Item, Project) ลด `available = onHand − reserved`; เบิกให้ Project ใช้การจองของ Project นั้นก่อน; ปล่อยการจองได้ |
| รับเข้า (`Receipt`) | สร้างจาก Goods Receipt (`POST /api/v1/inventory/receipts`) เท่านั้น เลือกคลังที่อยู่สาขาเดียวกับใบรับ; หนึ่งใบรับเข้าได้ครั้งเดียว; รองรับการระบุ `locationId`, `lotNumber`, `serialNumbers` ต่อบรรทัด |
| คืนเข้าคลัง (`Project/Customer Return`) | `POST /api/v1/inventory/returns` (`SRT-{YYYY}-{SEQ:4}`, `return_in`): รับของคืนเข้าคลังพร้อมเหตุผล; หากผูก `projectId` ระบบตรวจห้ามคืนเกินยอดที่เคยเบิกให้โครงการนั้น (`422 INVENTORY_RETURN_EXCEEDS_ISSUED`) ใช้ต้นทุนจากการเบิกเดิม (หรือระบุ `unitCost`) และบันทึกกลับรายการ (`isReversal: true`, `-returnedValue`) ใน `ProjectCostLedgerEntries` (`actual`, `stock_return`) |
| ปิดงวดสต็อก (`Period Close`) | `POST /api/v1/inventory/period-closes` ปิดงวดรายเดือน `(periodYear, periodMonth)` บันทึก Snapshot มูลค่าคงเหลือสิ้นงวด (`inventory_period_close_lines`) และ **ล็อกห้ามลงรายการย้อนหลัง** ที่มี `occurredAtUtc` อยู่ในหรือก่อนวันสิ้นงวดที่ปิดแล้ว (`409 INVENTORY_PERIOD_CLOSED`) |
| Concurrency | ทุกการบันทึกล็อกแถว balance (`SELECT … FOR UPDATE`) ตามลำดับคงที่ใน Transaction เดียว + optimistic `row_version` → ไม่ oversell, ไม่ deadlock |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST` ที่บันทึกเอกสารต้องมี `Idempotency-Key`; warehouse/location update ใช้ `If-Match`.

| Action | Method/Path | Permission |
| --- | --- | --- |
| Warehouse | `POST/PUT /api/v1/warehouses[/{id}]`, `POST /{id}/activate\|deactivate` | `warehouses.manage` |
| Warehouse อ่าน/รายการ | `GET /api/v1/warehouses[/{id}]?search=&status=&page=&pageSize=` | `warehouses.read` |
| Warehouse Locations (Bins) | `GET /api/v1/warehouses/{id}/locations`, `POST /api/v1/warehouses/{id}/locations`, `PUT /api/v1/warehouses/{id}/locations/{locationId}` | `warehouses.read` / `warehouses.manage` |
| รับเข้าจาก Goods Receipt | `POST /api/v1/inventory/receipts` `{goodsReceiptId, warehouseId, locationId?, lines?:[{itemId, locationId?, lotNumber?, serialNumbers?}]}` | `inventory.receive` |
| เบิก | `POST /api/v1/inventory/issues` `{warehouseId, locationId?, projectId?, occurredAtUtc?, reason?, lines:[{itemId, quantity, locationId?, lotNumber?, serialNumbers?}]}` | `inventory.issue` |
| โอน | `POST /api/v1/inventory/transfers` `{fromWarehouseId, toWarehouseId, fromLocationId?, toLocationId?, occurredAtUtc?, reason?, lines}` | `inventory.transfer` |
| ปรับยอด | `POST /api/v1/inventory/adjustments` `{warehouseId, locationId?, occurredAtUtc?, reason, lines:[{itemId, countedQuantity, unitCost?, locationId?, lotNumber?, serialNumbers?}]}` | `inventory.adjust` |
| คืนเข้าคลัง (Project/Customer Return) | `POST /api/v1/inventory/returns` `{warehouseId, locationId?, projectId?, sourceIssueDocumentId?, occurredAtUtc?, reason, lines:[{itemId, quantity, unitCost?, locationId?, lotNumber?, serialNumbers?}]}` | `inventory.receive` |
| จอง/ปล่อย | `POST /api/v1/inventory/reservations`, `POST /reservations/{id}/release` `{expectedVersion}` | `inventory.reserve` |
| ปิดงวดสต็อก (Period Close) | `POST /api/v1/inventory/period-closes` `{periodYear, periodMonth, note?}`, `GET /api/v1/inventory/period-closes[/{id}]` | `warehouses.manage` / `inventory.read` |
| อ่านยอด/เคลื่อนไหว/Serial/Lot | `GET /api/v1/inventory/balances`, `/location-balances`, `/serials`, `/movements`, `/reservations`, `/documents/{id}`, `/reconciliation` | `inventory.read` |

## Errors

- `409`: `WAREHOUSE_CODE_CONFLICT`, `WAREHOUSE_VERSION_CONFLICT`, `WAREHOUSE_INVALID_STATE`, `WAREHOUSE_HAS_STOCK`, `LOCATION_CODE_CONFLICT`, `LOCATION_VERSION_CONFLICT`, `LOCATION_IS_DEFAULT`, `LOCATION_HAS_STOCK`, `INVENTORY_WAREHOUSE_INACTIVE`, `INVENTORY_LOCATION_INACTIVE`, `INVENTORY_RECEIPT_ALREADY_POSTED`, `INVENTORY_PROJECT_NOT_ACTIVE`, `INVENTORY_RESERVATION_INVALID`, `INVENTORY_RESERVATION_VERSION_CONFLICT`, `INVENTORY_VERSION_CONFLICT`, `INVENTORY_SERIAL_DUPLICATE`, `INVENTORY_PERIOD_CLOSED`, `INVENTORY_PERIOD_ALREADY_CLOSED`.
- `422`: `INVENTORY_INSUFFICIENT_STOCK`, `INVENTORY_ITEM_NOT_STOCKABLE`, `INVENTORY_QUANTITY_INVALID`, `INVENTORY_COST_INVALID`, `INVENTORY_COST_REQUIRED`, `INVENTORY_NO_CHANGE`, `INVENTORY_LINE_INVALID`, `INVENTORY_REASON_REQUIRED`, `INVENTORY_TRANSFER_SAME_WAREHOUSE`, `INVENTORY_WAREHOUSE_BRANCH_MISMATCH`, `WAREHOUSE_FIELD_INVALID`, `LOCATION_FIELD_INVALID`, `INVENTORY_FIELD_REQUIRED`, `INVENTORY_FIELD_INVALID`, `INVENTORY_LOT_REQUIRED`, `INVENTORY_SERIAL_COUNT_MISMATCH`, `INVENTORY_SERIAL_NOT_AVAILABLE`, `INVENTORY_RETURN_EXCEEDS_ISSUED`, `INVENTORY_PERIOD_INVALID`.
- `404`: `RESOURCE_NOT_FOUND` (ไม่พบหรืออยู่นอก Organization).
