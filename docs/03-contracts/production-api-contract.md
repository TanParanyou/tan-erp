# Production API Contract (ข้อตกลง API BOM และใบสั่งผลิต)

**สถานะ:** Implemented 2026-10-04 (CP-12: BOM + revision + approval, Work Order, เบิก/คืน/รับผลผลิตผ่าน Inventory). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Engineering/Production ยืนยัน** และยังไม่ผ่าน Production UAT. BOM นี้แยกจาก BOQ ของ Estimate โดยสิ้นเชิง.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| BOM | หนึ่ง Item ผลิตได้ (`canProduce` + `canStock`) มีได้หนึ่ง BOM (`BOM-nnnnn`); ประวัติเป็น **revision** (`R1, R2, …`) ครั้งละหนึ่ง draft |
| Revision | `draft → approved → obsolete`; แก้ได้เฉพาะ draft; อนุมัติแล้วแก้ไม่ได้ — ต้องสร้าง revision ใหม่ (คัดลอกจากล่าสุด); อนุมัติ revision ใหม่ทำให้ฉบับ approved เดิมเป็น `obsolete` อัตโนมัติ (มีได้ approved เดียว: partial unique index) |
| Maker–checker | ผู้จัดทำอนุมัติ revision ของตนเองไม่ได้ (`BOM_SELF_APPROVAL`; ซ้ำด้วย DB check) |
| ผลผลิต/ของเสีย | `outputQuantity` = จำนวนที่ได้ต่อหนึ่งสูตร; แต่ละวัตถุดิบมี `quantity` และ `scrapPercent` 0–50 (ไม่มี yield ต่อ output); ต้องใช้จริง = `quantity × (1 + scrap/100)` |
| วัตถุดิบ | ต้อง active + `canStock` ไม่ซ้ำใน revision; สินค้าที่ผลิตเองเป็นวัตถุดิบตัวเองไม่ได้ |
| Cycle | ตรวจทั้งตอนบันทึกและตอนอนุมัติ โดยเดินตาม approved BOM ของวัตถุดิบ (ถ้าวนกลับถึงสินค้าที่ผลิต → `BOM_CYCLE`) |
| ขอบเขตที่ไม่ทำ | ไม่มี routing/capacity/แรงงาน/ค่าโสหุ้ย, subcontracting, co-product/by-product, ทางเลือกวัตถุดิบ — ต้นทุนผลผลิต = ต้นทุนวัตถุดิบที่เบิกสุทธิเท่านั้น |
| Work Order | `draft → released → in_progress → completed` หรือ `cancelled`; เลข `WO-{YYYY}-{SEQ:4}`; สร้างได้เมื่อ Item มี BOM approved — **snapshot** ฉบับ approved ล่าสุดและจำนวนวัตถุดิบที่ต้องใช้ (`planned × gross / output`) ไว้ในใบสั่ง จึงไม่เปลี่ยนเมื่อ BOM ใหม่มีผล |
| คลัง/โครงการ | เลือกคลัง active ในสาขาเดียวกับผู้ใช้; โครงการเป็นทางเลือก (ต้องไม่ completed/cancelled) |
| เบิก | `work-orders.operate`; เฉพาะ released/in_progress; เบิกสุทธิ (เบิก − คืน) ของวัตถุดิบต้องไม่เกินที่ต้องใช้ (`PRODUCTION_OVER_ISSUED`); สต็อกไม่พอ → `INVENTORY_INSUFFICIENT_STOCK` และไม่มีอะไรเปลี่ยน; ตัดที่ต้นทุนเฉลี่ย |
| คืน | คืนได้เฉพาะส่วนที่ยังไม่ถูกใช้กับผลผลิตที่เสร็จแล้ว (`net − required × completed/planned`; เกิน → `PRODUCTION_RETURN_EXCEEDS`); คืนที่ต้นทุนเดิมที่เบิก (ไม่บิดเบือน moving average) |
| รับผลผลิต | บางส่วนได้ แต่สะสมไม่เกินแผน (`PRODUCTION_OVER_COMPLETED`); ต้องเบิกวัตถุดิบครบตามสัดส่วนที่เสร็จสะสม (`PRODUCTION_MATERIAL_SHORTAGE`); มูลค่าที่ตัดเข้าสินค้า = มูลค่าเบิกสุทธิ × สัดส่วน (ครั้งสุดท้ายรับส่วนที่เหลือทั้งหมด จึงไม่มีเศษ); สถานะเป็น `completed` เมื่อครบแผน |
| ยกเลิก | `work-orders.manage` + เหตุผล; ได้ใน draft/released/in_progress เมื่อยังไม่มีผลผลิตเสร็จและวัตถุดิบสุทธิที่เบิกเป็น 0 (`PRODUCTION_HAS_ISSUED_MATERIALS`) |
| Atomic | แต่ละการเบิก/คืน/รับผลผลิตทำใน Transaction เดียวกับการเปลี่ยนใบสั่ง ผ่าน `IProductionStockPort` (Application) ที่ Inventory implement — Production **ไม่เขียนตาราง `inventory.*` ตรง**; ล็อกแถว Work Order (`FOR UPDATE`) ก่อนล็อก balance เพื่อกันการยิงซ้อน |
| Idempotency | สร้าง BOM/Work Order และเบิก/คืน/รับผลผลิต ต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง → 409); แต่ละ stock document ผูกกับ source id ของ `work_order_transactions` |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `If-Match` (row version) สำหรับแก้ draft/อนุมัติ/เลิกใช้ BOM revision และ release/cancel Work Order.

| Action | Method/Path | Permission |
| --- | --- | --- |
| สร้าง BOM (+R1 draft) | `POST /api/v1/boms` `{itemId, outputQuantity, note?, lines:[{componentItemId, quantity, scrapPercent}]}` | `boms.manage` |
| สร้าง revision ใหม่ | `POST /api/v1/boms/{id}/revisions` | `boms.manage` |
| แก้ draft | `PUT /api/v1/boms/{id}/revisions/{revisionId}` | `boms.manage` |
| อนุมัติ / เลิกใช้ | `POST …/revisions/{revisionId}/approve` · `/obsolete` | `boms.approve` · `boms.manage` |
| อ่าน BOM | `GET /api/v1/boms[/{id}]?search=&page=&pageSize=` | `boms.read` |
| สร้าง Work Order | `POST /api/v1/work-orders` `{itemId, warehouseId, projectId?, plannedQuantity, note?}` | `work-orders.manage` |
| ปล่อยงาน / ยกเลิก | `POST /api/v1/work-orders/{id}/release` · `/cancel` `{reason}` | `work-orders.manage` |
| เบิก / คืน | `POST /api/v1/work-orders/{id}/issues` · `/returns` `{lines:[{itemId, quantity}]}` | `work-orders.operate` |
| รับผลผลิต | `POST /api/v1/work-orders/{id}/completions` `{quantity}` | `work-orders.operate` |
| อ่าน Work Order | `GET /api/v1/work-orders[/{id}]?search=&status=&projectId=&page=&pageSize=` | `work-orders.read` |

Response ของ Work Order มี `materials[]` (required/issued/returned/net/remaining/มูลค่า) และ `transactions[]` (ชนิด issue/return/completion พร้อมเลขเอกสารสต็อกและมูลค่า) เพื่อ trace ถึง Stock Movement. เอกสารสต็อกใช้ชนิดเดิม `stock-issues` (เบิก), `stock-returns` ใหม่ (`SRT`, movement `return_in`), `stock-receipts` (รับผลผลิต); แหล่งที่มา `work_order_issue|work_order_return|work_order_completion`.

## Errors

`409`: `BOM_CODE_CONFLICT`, `BOM_ALREADY_EXISTS`, `BOM_VERSION_CONFLICT`, `BOM_INVALID_STATE`, `PRODUCTION_BOM_NOT_APPROVED`, `PRODUCTION_VERSION_CONFLICT`, `PRODUCTION_INVALID_STATE`, `PRODUCTION_PROJECT_NOT_ACTIVE`, `PRODUCTION_HAS_ISSUED_MATERIALS`. `403`: `BOM_SELF_APPROVAL`. `422`: `BOM_LINE_INVALID`, `BOM_CYCLE`, `BOM_ITEM_NOT_PRODUCIBLE`, `BOM_COMPONENT_NOT_STOCKABLE`, `PRODUCTION_MATERIAL_SHORTAGE`, `PRODUCTION_OVER_COMPLETED`, `PRODUCTION_OVER_ISSUED`, `PRODUCTION_RETURN_EXCEEDS`, `PRODUCTION_MATERIAL_INVALID`, `PRODUCTION_QUANTITY_INVALID`, `PRODUCTION_REASON_REQUIRED`, `PRODUCTION_FIELD_REQUIRED/INVALID`; และจาก Inventory: `INVENTORY_INSUFFICIENT_STOCK`, `INVENTORY_WAREHOUSE_INACTIVE`, `INVENTORY_WAREHOUSE_BRANCH_MISMATCH`, `INVENTORY_ITEM_NOT_STOCKABLE`. `404` นอก Organization.

## Data (migration `AddProduction`, schema `production`)

`boms` (unique `(org, normalized_code)`, `(org, item)`), `bom_revisions` (checks status/output/approver ≠ author; partial unique index approved เดียวต่อ BOM; `row_version`), `bom_lines` (unique `(revision, component)`; checks quantity/scrap), `work_orders` (checks status/quantities/cost; `row_version`), `work_order_materials` (checks quantity/returned ≤ issued), `work_order_transactions`. Migration เดียวกันปรับ check ของ `inventory.stock_documents`/`stock_movements` ให้รับ `return` / `return_in`. Audit: `bom.*`, `work-order.*`.

## Hand-offs

รับจาก [Inventory](inventory-api-contract.md): ผ่าน `IProductionStockPort` เท่านั้น. ส่งให้ MRP (CP-13): approved BOM (วัตถุดิบ+ของเสีย), Work Order ที่ยังเปิด (ความต้องการวัตถุดิบคงเหลือ = `remainingQuantity`) และผลผลิตที่วางแผน.
