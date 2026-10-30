# Production API Contract (ข้อตกลง API BOM และใบสั่งผลิต)

**สถานะ:** Implemented 2026-10-04 (CP-12: BOM + revision + approval, Work Order, เบิก/คืน/รับผลผลิตผ่าน Inventory) และขยาย 2026-10-30 (**Wave C G-13**: Work Centers, BOM Routing Steps, Work Order Routing Snapshot, Daily Capacity View, WO Labor Cost จาก Timesheet G-20, และ Subcontracting PO Link จาก G-11). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (`TEST_ONLY`) **รอ Engineering/Production ยืนยัน** และยังไม่ผ่าน Production UAT. BOM นี้แยกจาก BOQ ของ Estimate โดยสิ้นเชิง.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| BOM | หนึ่ง Item ผลิตได้ (`canProduce` + `canStock`) มีได้หนึ่ง BOM (`BOM-nnnnn`); ประวัติเป็น **revision** (`R1, R2, …`) ครั้งละหนึ่ง draft |
| Revision | `draft → approved → obsolete`; แก้ได้เฉพาะ draft; อนุมัติแล้วแก้ไม่ได้ — ต้องสร้าง revision ใหม่ (คัดลอกจากล่าสุด); อนุมัติ revision ใหม่ทำให้ฉบับ approved เดิมเป็น `obsolete` อัตโนมัติ (มีได้ approved เดียว: partial unique index) |
| Maker–checker | ผู้จัดทำอนุมัติ revision ของตนเองไม่ได้ (`BOM_SELF_APPROVAL`; ซ้ำด้วย DB check) |
| ผลผลิต/ของเสีย | `outputQuantity` = จำนวนที่ได้ต่อหนึ่งสูตร; แต่ละวัตถุดิบมี `quantity` และ `scrapPercent` 0–50 (ไม่มี yield ต่อ output); ต้องใช้จริง = `quantity × (1 + scrap/100)` |
| วัตถุดิบ | ต้อง active + `canStock` ไม่ซ้ำใน revision; สินค้าที่ผลิตเองเป็นวัตถุดิบตัวเองไม่ได้ |
| Cycle | ตรวจทั้งตอนบันทึกและตอนอนุมัติ โดยเดินตาม approved BOM ของวัตถุดิบ (ถ้าวนกลับถึงสินค้าที่ผลิต → `BOM_CYCLE`) |
| Work Center (`G-13`) | สถานีงานรายสาขา (`WC-nnnnn` หรือรหัสที่กำหนดเอง ไม่ซ้ำในองค์กร); `dailyCapacityHours` ค่าเริ่มต้น `8.00` ชม./วัน (`0.25 .. 24.00`); `hourlyRate >= 0`; ห้ามปิดใช้งานหากมี BOM revision ที่ `approved` หรือ Work Order ที่ยังเปิดอยู่ (`draft`, `released`, `in_progress`) อ้างอิง (`PRODUCTION_WORK_CENTER_IN_USE`) |
| Routing (`G-13`) | ผูกลำดับขั้นตอน (`sequence > 0`, `operationCode`, `operationName`, `workCenterId?`, `setupHours`, `runHours`, `isSubcontracted`, `stdSubcontractCost`) กับ BOM revision (เป็นทางเลือกเพื่อรองรับ BOM เดิม); เมื่อสร้าง Work Order ระบบคัดลอกเป็น **Snapshot** (`production.work_order_operations`) พร้อมคำนวณ `plannedRunHours = round(runHours × plannedQuantity / bomOutputQuantity, 4)` |
| Capacity View (`G-13`) | คำนวณภาระงานต่อ Work Center ต่อวันจาก `plannedSetupHours + plannedRunHours` ของขั้นตอนที่ยังไม่เสร็จใน Work Order สถานะ `released` / `in_progress` กระจายตามช่วง `[plannedStartDate, plannedEndDate]` เทียบกับ `dailyCapacityHours` |
| ต้นทุนรวมใบสั่งผลิต (`G-13`) | ต้นทุนผลผลิต (`TotalManufacturingCost`) = **ต้นทุนวัตถุดิบเบิกสุทธิ (`NetIssuedValue`) + ต้นทุนแรงงานจากใบลงเวลาที่อนุมัติแล้ว (`LaborCost`, G-20) + ต้นทุนจ้างผลิตภายนอก (`SubcontractCost`, ผูก PO ที่อนุมัติแล้วจาก G-11)**; จัดสรรเข้าสินค้าสำเร็จรูปตามสัดส่วนเมื่อรับผลผลิตเข้าคลัง |
| ขอบเขตที่ไม่ทำ | ไม่มี co-product/by-product หรือทางเลือกวัตถุดิบ |
| Work Order | `draft → released → in_progress → completed` หรือ `cancelled`; เลข `WO-{YYYY}-{SEQ:4}`; สร้างได้เมื่อ Item มี BOM approved — **snapshot** ฉบับ approved ล่าสุด, จำนวนวัตถุดิบที่ต้องใช้ (`planned × gross / output`) และขั้นตอน Routing ไว้ในใบสั่ง จึงไม่เปลี่ยนเมื่อ BOM ใหม่มีผล |
| คลัง/โครงการ | เลือกคลัง active ในสาขาเดียวกับผู้ใช้; โครงการเป็นทางเลือก (ต้องไม่ completed/cancelled) |
| เบิก | `work-orders.operate`; เฉพาะ released/in_progress; เบิกสุทธิ (เบิก − คืน) ของวัตถุดิบต้องไม่เกินที่ต้องใช้ (`PRODUCTION_OVER_ISSUED`); สต็อกไม่พอ → `INVENTORY_INSUFFICIENT_STOCK` และไม่มีอะไรเปลี่ยน; ตัดที่ต้นทุนเฉลี่ย |
| คืน | คืนได้เฉพาะส่วนที่ยังไม่ถูกใช้กับผลผลิตที่เสร็จแล้ว (`net − required × completed/planned`; เกิน → `PRODUCTION_RETURN_EXCEEDS`); คืนที่ต้นทุนเดิมที่เบิก (ไม่บิดเบือน moving average) |
| รับผลผลิต | บางส่วนได้ แต่สะสมไม่เกินแผน (`PRODUCTION_OVER_COMPLETED`); ต้องเบิกวัตถุดิบครบตามสัดส่วนที่เสร็จสะสม (`PRODUCTION_MATERIAL_SHORTAGE`); มูลค่าที่ตัดเข้าสินค้า = ต้นทุนรวมสุทธิ (`NetIssuedValue + LaborCost + SubcontractCost`) × สัดส่วน (ครั้งสุดท้ายรับส่วนที่เหลือทั้งหมด จึงไม่มีเศษ); สถานะเป็น `completed` เมื่อครบแผน |
| ยกเลิก | `work-orders.manage` + เหตุผล; ได้ใน draft/released/in_progress เมื่อยังไม่มีผลผลิตเสร็จ, วัตถุดิบสุทธิที่เบิกเป็น 0 (`PRODUCTION_HAS_ISSUED_MATERIALS`) และยังไม่มีต้นทุนแรงงานหรือจ้างผลิตภายนอก (`PRODUCTION_HAS_ACTUAL_COSTS`) |
| Atomic | แต่ละการเบิก/คืน/รับผลผลิตทำใน Transaction เดียวกับการเปลี่ยนใบสั่ง ผ่าน `IProductionStockPort` (Application) ที่ Inventory implement — Production **ไม่เขียนตาราง `inventory.*` ตรง**; ล็อกแถว Work Order (`FOR UPDATE`) ก่อนล็อก balance เพื่อกันการยิงซ้อน |
| Idempotency | สร้าง BOM/Work Order/Work Center และเบิก/คืน/รับผลผลิต/บันทึกขั้นตอน ต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง → 409); แต่ละ stock document ผูกกับ source id ของ `work_order_transactions` |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `If-Match` (row version) สำหรับแก้ Work Center, แก้ draft/อนุมัติ/เลิกใช้ BOM revision และ release/cancel Work Order.

| Action | Method/Path | Permission |
| --- | --- | --- |
| สร้าง / แก้ไข Work Center (`G-13`) | `POST /api/v1/work-centers` · `PUT /api/v1/work-centers/{id}` · `POST /api/v1/work-centers/{id}/activate` · `/deactivate` | `boms.manage` |
| อ่าน Work Center / Capacity (`G-13`) | `GET /api/v1/work-centers[/{id}]?branchId=&status=&search=` · `GET /api/v1/work-centers/capacity?branchId=&fromDate=&toDate=` | `boms.read` · `work-orders.read` |
| สร้าง BOM (+R1 draft + routingSteps?) | `POST /api/v1/boms` `{itemId, outputQuantity, note?, lines:[...], routingSteps?:[...]}` | `boms.manage` |
| สร้าง revision ใหม่ | `POST /api/v1/boms/{id}/revisions` | `boms.manage` |
| แก้ draft | `PUT /api/v1/boms/{id}/revisions/{revisionId}` | `boms.manage` |
| อนุมัติ / เลิกใช้ | `POST …/revisions/{revisionId}/approve` · `/obsolete` | `boms.approve` · `boms.manage` |
| อ่าน BOM | `GET /api/v1/boms[/{id}]?search=&page=&pageSize=` | `boms.read` |
| สร้าง Work Order | `POST /api/v1/work-orders` `{itemId, warehouseId, projectId?, plannedQuantity, plannedStartDate?, plannedEndDate?, note?}` | `work-orders.manage` |
| ปล่อยงาน / ยกเลิก | `POST /api/v1/work-orders/{id}/release` · `/cancel` `{reason}` | `work-orders.manage` |
| เบิก / คืน | `POST /api/v1/work-orders/{id}/issues` · `/returns` `{lines:[{itemId, quantity}]}` | `work-orders.operate` |
| บันทึกขั้นตอนผลิตภายใน (`G-13`) | `POST /api/v1/work-orders/{id}/operations/{operationId}/complete` `{actualHours, note?}` | `work-orders.operate` |
| บันทึกจ้างผลิตภายนอก (`G-13`) | `POST /api/v1/work-orders/{id}/operations/{operationId}/subcontract` `{purchaseOrderId, costAmount, note?}` | `work-orders.operate` |
| รับผลผลิต | `POST /api/v1/work-orders/{id}/completions` `{quantity}` | `work-orders.operate` |
| อ่าน Work Order | `GET /api/v1/work-orders[/{id}]?search=&status=&projectId=&page=&pageSize=` | `work-orders.read` |

Response ของ Work Order มี `materials[]` (required/issued/returned/net/remaining/มูลค่า), `operations[]` (sequence, operationCode, operationName, workCenter, plannedSetupHours, plannedRunHours, actualHours, isSubcontracted, subcontractPurchaseOrder, subcontractCost, status), `laborEntries[]` (รายการใบลงเวลา G-20 ที่อนุมัติแล้ว), สรุปต้นทุน (`netMaterialValue`, `laborHours`, `laborCost`, `subcontractCost`, `totalManufacturingCost`, `costAllocated`), และ `transactions[]` (ชนิด issue/return/completion พร้อมเลขเอกสารสต็อกและมูลค่า) เพื่อ trace ถึง Stock Movement, Timesheet และ Purchase Order.

## Errors

`409`: `BOM_CODE_CONFLICT`, `BOM_ALREADY_EXISTS`, `BOM_VERSION_CONFLICT`, `BOM_INVALID_STATE`, `PRODUCTION_BOM_NOT_APPROVED`, `PRODUCTION_VERSION_CONFLICT`, `PRODUCTION_INVALID_STATE`, `PRODUCTION_PROJECT_NOT_ACTIVE`, `PRODUCTION_HAS_ISSUED_MATERIALS`, `PRODUCTION_HAS_ACTUAL_COSTS`, `PRODUCTION_WORK_CENTER_CODE_CONFLICT`, `PRODUCTION_WORK_CENTER_VERSION_CONFLICT`, `PRODUCTION_WORK_CENTER_IN_USE`, `PRODUCTION_WORK_CENTER_INACTIVE`, `PRODUCTION_SUBCONTRACT_PO_NOT_APPROVED`, `PRODUCTION_OPERATION_ALREADY_COMPLETED`. `403`: `BOM_SELF_APPROVAL`. `422`: `BOM_LINE_INVALID`, `BOM_ROUTING_INVALID`, `BOM_CYCLE`, `BOM_ITEM_NOT_PRODUCIBLE`, `BOM_COMPONENT_NOT_STOCKABLE`, `PRODUCTION_WORK_CENTER_INVALID`, `PRODUCTION_OPERATION_NOT_SUBCONTRACTED`, `PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO`, `PRODUCTION_MATERIAL_SHORTAGE`, `PRODUCTION_OVER_COMPLETED`, `PRODUCTION_OVER_ISSUED`, `PRODUCTION_RETURN_EXCEEDS`, `PRODUCTION_MATERIAL_INVALID`, `PRODUCTION_QUANTITY_INVALID`, `PRODUCTION_REASON_REQUIRED`, `PRODUCTION_FIELD_REQUIRED/INVALID`; และจาก Inventory: `INVENTORY_INSUFFICIENT_STOCK`, `INVENTORY_WAREHOUSE_INACTIVE`, `INVENTORY_WAREHOUSE_BRANCH_MISMATCH`, `INVENTORY_ITEM_NOT_STOCKABLE`. `404` นอก Organization.

## Data (migration `AddProduction`, schema `production`)

`boms` (unique `(org, normalized_code)`, `(org, item)`), `bom_revisions` (checks status/output/approver ≠ author; partial unique index approved เดียวต่อ BOM; `row_version`), `bom_lines` (unique `(revision, component)`; checks quantity/scrap), `work_orders` (checks status/quantities/cost; `row_version`), `work_order_materials` (checks quantity/returned ≤ issued), `work_order_transactions`. Migration เดียวกันปรับ check ของ `inventory.stock_documents`/`stock_movements` ให้รับ `return` / `return_in`. Audit: `bom.*`, `work-order.*`.

## Hand-offs

รับจาก [Inventory](inventory-api-contract.md): ผ่าน `IProductionStockPort` เท่านั้น. ส่งให้ MRP (CP-13): approved BOM (วัตถุดิบ+ของเสีย), Work Order ที่ยังเปิด (ความต้องการวัตถุดิบคงเหลือ = `remainingQuantity`) และผลผลิตที่วางแผน.
