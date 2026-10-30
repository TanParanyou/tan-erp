# Inventory Verification (CP-11 + Wave C `G-12`)

**สถานะ:** Implemented CP-11 (2026-10-04) + Extended in Wave C `G-12` (2026-10-29); หลักฐานเป็นชุดทดสอบอัตโนมัติ ไม่ใช่ Stock Accuracy UAT หรือ Production sign-off. กฎ: [Inventory API Contract](../03-contracts/inventory-api-contract.md) และ [แผน G-12](../superpowers/plans/2026-10-29-g12-inventory-completion.md).

## ผลที่รันจริง

### 1. CP-11 Baseline (2026-10-04)
- Backend Integration `InventoryEndpointsTests` 9/9 บน PostgreSQL: Warehouse lifecycle/numbering/idempotency/ETag/สิทธิ์/ข้าม Organization; รับเข้าจาก Goods Receipt (ledger+balance, replay, ซ้ำ → 409, PO แสดงเลขเอกสารสต็อก); คลัง inactive รับไม่ได้ และคลังที่มีสต็อกปิดไม่ได้; moving average (2 รอบรับ → 1,300/หน่วย, เบิกที่ต้นทุนเฉลี่ย, เบิกเกิน 422 ไม่เขียนอะไร, หน่วยสุดท้ายไม่เหลือเศษมูลค่า); การจอง (ลด available, เบิกของคนอื่นถูกกัน, โครงการใช้การจองของตน, ปล่อย/stale/ปล่อยซ้ำ); โอนแบบ atomic คงมูลค่า (ผลรวมสองฝั่งเท่าเดิม, ไม่พอ → ไม่เปลี่ยนทั้งสองฝั่ง); ปรับยอด (ลด/เพิ่ม/ต้องมีเหตุผล/ต้องมีต้นทุนเมื่อยอดเป็น 0/ต่ำกว่ายอดจองไม่ได้); ยิงเบิกพร้อมกัน 6 รายการ × 3 หน่วย กับสต็อก 10 → สำเร็จ 3 รายการพอดี ที่เหลือ 422 คงเหลือ 1 และ ledger ตรง; reconciliation หลังทุกเคสไม่มีส่วนต่าง.

### 2. Wave C `G-12` — Inventory Completion (2026-10-29)
- **Backend Build & Static Analysis:** `dotnet build backend/TanErp.slnx` → **0 warnings, 0 errors**.
- **Backend Unit Tests (`TanErp.UnitTests`):** **676/676 passed** (รวม `InventoryCompletionDomainTests`: `WarehouseLocation` invariants & `LOCATION_IS_DEFAULT`, `StockLocationBalance` negative balance guard per `(warehouse, location, item, lot)`, `StockSerial` state transitions, และ `InventoryPeriodClose` date validation).
- **Backend Architecture Tests (`TanErp.ArchitectureTests`):** **4/4 passed**.
- **Backend Integration Tests (`TanErp.IntegrationTests` บน PostgreSQL จริง):** **39/39 passed** (`InventoryEndpointsTests` 13/13, `ProcurementEndpointsTests`, `ProductionEndpointsTests`, `MrpEndpointsTests`, `ProjectControlEndpointsTests`, `OpenApiContractTests`) ครอบคลุม:
  1. **Warehouse Locations / Bins (`DEFAULT` & Custom Bins):** สร้างคลังสินค้าแล้วสร้าง Location มาตรฐาน `DEFAULT` อัตโนมัติ, สร้าง Bin ย่อย (`BIN-A1`, `BIN-B2`), บล็อกการปิดใช้ `DEFAULT` (`409 LOCATION_IS_DEFAULT`), โอนย้ายระหว่าง Bin ภายในคลังเดียวกันสำเร็จ, และบล็อกการปิดใช้ Bin ที่ยังมีสต็อกคงเหลือ (`409 LOCATION_HAS_STOCK`).
  2. **Lot & Serial Tracking (`TrackLot` & `TrackSerial` บน `ItemCapabilities`):**
     - สินค้าที่เปิด `TrackLot`: รับเข้าหรือเบิกโดยไม่ระบุ `lotNumber` ถูกปฏิเสธ (`422 INVENTORY_LOT_REQUIRED`); เบิกเกินยอดคงเหลือของ `(warehouse, location, item, lot)` นั้นถูกปฏิเสธ (`422 INVENTORY_INSUFFICIENT_STOCK`) แม้ว่า Lot อื่นในคลังเดียวกันจะมีของเหลือ.
     - สินค้าที่เปิด `TrackSerial`: จำนวนไม่ตรงกับรายการ Serial ถูกปฏิเสธ (`422 INVENTORY_SERIAL_COUNT_MISMATCH`), รับเข้า Serial ซ้ำถูกปฏิเสธ (`409 INVENTORY_SERIAL_DUPLICATE`), เบิก Serial ที่ไม่อยู่ในสถานะ `in_stock` ที่ Location/Lot ต้นทางถูกปฏิเสธ (`422 INVENTORY_SERIAL_NOT_AVAILABLE`), และเมื่อเบิกสำเร็จสถานะ Serial เปลี่ยนเป็น `issued`.
  3. **Customer / Project Return into Stock (`POST /api/v1/inventory/returns`):** เบิกของเข้าโครงการ → บันทึกรับคืนพัสดุจากโครงการกลับเข้าคลัง (`return_in`) → ยอด `OnHand` ของคลังและ Location/Lot เพิ่มขึ้น, บันทึกกลับรายการ Actual Cost ใน `ProjectCostLedgerEntries` (`sourceType = "stock_return"`, `-returnedValue`, `isReversal = true`), และบล็อกการคืนเกินยอดที่เคยเบิกให้โครงการ (`422 INVENTORY_RETURN_EXCEEDS_ISSUED`).
  4. **Inventory Period Close (`POST /api/v1/inventory/period-closes`):** ปิดงวดสต็อกรายเดือน `(periodYear, periodMonth)` → บันทึก Snapshot มูลค่าคงเหลือ (`inventory.period_close_lines`), บล็อกการปิดงวดซ้ำ (`409 INVENTORY_PERIOD_ALREADY_CLOSED`), และบล็อกการลงรายการย้อนหลังที่มี `occurredAtUtc <= periodEndDate` (`409 INVENTORY_PERIOD_CLOSED`) ในขณะที่รายการในเดือนถัดไปยังลงได้ตามปกติ.
- **Frontend Verification (`frontend/`):**
  - `npx tsc --noEmit` & `npm run lint`: **0 errors, 0 warnings**.
  - `npx vitest run src/features/inventory/`: **14/14 passed** (ครอบคลุม Warehouse Locations Drawer, Stock Return พร้อม Lot/Serial, Period Close Modal, และ View Switcher; ทั้งโปรเจกต์ผ่าน **217/217 ไฟล์, 1,104/1,104 tests**).
  - `npm run build` (`next build`): **Compiled successfully** (89/89 routes).

## ข้อจำกัดและรายการรอยืนยันทางธุรกิจ (`TEST_ONLY`)

- กฎ `TEST_ONLY` สำหรับการตีมูลค่า Moving Average ระดับคลังควบคู่ยอดคงเหลือระดับ Location/Lot (`VQ-INV-01`), ต้นทุนรับคืนวัสดุจากโครงการ (`VQ-INV-02`), และการล็อกงวดที่ปิดแล้วแบบถาวร (`VQ-INV-03`) บันทึกไว้ใน [Open Validation Questions](../01-business/open-validation-questions.md) รอ Warehouse/Finance ยืนยันก่อนเปิดใช้งานจริง.
