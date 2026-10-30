# Wave C `G-12` — Inventory Completion Implementation Plan

> **Status:** Approved for Implementation (`TEST_ONLY` business defaults approved by user)
> **Scope:** Warehouse Locations/Bins (`DEFAULT` location auto-provisioning & migration backfill), Item Lot/Serial Tracking (`TrackLot`, `TrackSerial` flags on `ItemCapabilities`, granular `(warehouse, location, item, lot)` balance checks & org-wide unique serial per item), Customer/Project Stock Return (`POST /api/v1/inventory/returns` with `ProjectCostLedgerEntries` actual reversal), and Inventory Period Close (`POST /api/v1/inventory/period-closes` with backdate lock & valuation snapshot).

---

## 1. Approved Business Rules (`TEST_ONLY`)

1. **Warehouse Locations / Bins (`WarehouseLocation`):**
   - Every `Warehouse` automatically provisions a default location (`Code = "DEFAULT"`, `Name = "Default Location"`, `IsDefault = true`, `Status = "active"`).
   - Additional locations/bins can be created, updated, activated/deactivated under a warehouse (`POST /api/v1/warehouses/{warehouseId}/locations`, `PUT /api/v1/warehouses/{warehouseId}/locations/{locationId}`, `GET /api/v1/warehouses/{warehouseId}/locations`).
   - Deactivating the `DEFAULT` location or a location that holds stock (`OnHand > 0`) is rejected (`409 LOCATION_IS_DEFAULT` / `409 LOCATION_HAS_STOCK`).
   - When a stock posting omits `locationId`, the system automatically resolves the warehouse's `DEFAULT` location so all existing callers (CP-11, CP-12 Production, G-11 Procurement) remain 100% backward-compatible.

2. **Granular Location & Lot Balance (`StockLocationBalance`) + Serial Tracking (`StockSerial`):**
   - `ItemCapabilities` adds `TrackLot` (`bool`, default `false`) and `TrackSerial` (`bool`, default `false`).
   - `inventory.stock_location_balances` tracks `OnHand` per `(organization_id, warehouse_id, location_id, item_id, lot_number)` where `lot_number` is normalized uppercase (`""` when unlotted), with DB check `on_hand >= 0`.
   - Warehouse-level `inventory.stock_balances` remains the moving-average valuation & project reservation authority per `(organization_id, warehouse_id, item_id)`.
   - When `item.Capabilities.TrackLot == true`:
     - Any receipt, issue, transfer, adjustment, or return must specify a non-empty `lotNumber` (1–64 chars), otherwise fails with `422 INVENTORY_LOT_REQUIRED`.
     - Issuing, transferring out, or adjusting down more than the `OnHand` of that specific `(warehouse, location, item, lot)` fails with `422 INVENTORY_INSUFFICIENT_STOCK`.
   - When `item.Capabilities.TrackSerial == true`:
     - Movement quantity must be a positive whole number and match `serialNumbers.Count` (`422 INVENTORY_SERIAL_COUNT_MISMATCH`).
     - `inventory.stock_serials` enforces uniqueness per `(organization_id, item_id, normalized_serial_number)`.
     - Receiving a serial that is already `in_stock` (or already registered on a new receipt) fails with `409 INVENTORY_SERIAL_DUPLICATE`.
     - Issuing/transferring a serial that is not `in_stock` at the source `(warehouse, location, lot)` fails with `422 INVENTORY_SERIAL_NOT_AVAILABLE`.

3. **Customer / Project Return into Stock (`POST /api/v1/inventory/returns`):**
   - Permission: `inventory.receive`, requires `Idempotency-Key`.
   - Accepts `warehouseId`, optional `locationId`, optional `projectId`, optional `sourceIssueDocumentId`, optional `occurredAtUtc`, mandatory `reason`, and `lines: [{ itemId, quantity, unitCost?, locationId?, lotNumber?, serialNumbers? }]`.
   - When `projectId` is provided:
     - Verifies cumulative returned quantity for `(projectId, itemId)` does not exceed cumulative issued quantity (`422 INVENTORY_RETURN_EXCEEDS_ISSUED`).
     - Receives stock (`StockDocumentType.Return`, `DocumentTypes.StockReturns` `SRT-{YYYY}-{SEQ:4}`, `StockSourceType.ProjectReturn = "project_return"`, `StockMovementKind.ReturnIn`) at the original issue unit cost (or specified `unitCost` / current moving average).
     - Appends a reversal entry to `ProjectCostLedgerEntries` (`ProjectBudgetCategory.Material`, `ProjectCostEntryType.Actual`, `ProjectCostSourceType.StockReturn = "stock_return"`, `-returnedValue`, `isReversal: true`).

4. **Inventory Period Close (`InventoryPeriodClose` & `InventoryPeriodCloseLine`):**
   - Endpoints: `POST /api/v1/inventory/period-closes` (`warehouses.manage`, `Idempotency-Key`), `GET /api/v1/inventory/period-closes`, `GET /api/v1/inventory/period-closes/{id}` (`inventory.read`).
   - Closes a calendar month `(periodYear, periodMonth)` with `PeriodEndDate = last day of month`.
   - Cannot close the same `(organization_id, period_year, period_month)` twice (`409 INVENTORY_PERIOD_ALREADY_CLOSED`).
   - Freezes a valuation snapshot (`inventory_period_close_lines`) of all non-zero warehouse/item balances (`OnHand`, `Reserved`, `AverageCost`, `TotalValue`) and summary totals (`TotalOnHand`, `TotalValue`, `LineCount`).
   - **Backdate Lock:** Any stock posting (`receipt`, `issue`, `transfer`, `adjustment`, `return`, or port call) whose `OccurredAtUtc` date (`DateOnly.FromDateTime(occurredAtUtc.UtcDateTime)`) is `<= period.PeriodEndDate` of any closed period in the organization is rejected with `409 INVENTORY_PERIOD_CLOSED`.

---

## 2. Execution Phases (Tiered Subagent Orchestration)

- **Phase 1 — Backend Domain Architect (`Model: "inherit"`, `backend/` only):**
  - `ItemCapabilities` (`TrackLot`, `TrackSerial`), `WarehouseLocation`, `StockLocationBalance`, `StockSerial`, `InventoryPeriodClose` & `InventoryPeriodCloseLine`, EF Core migration `AddInventoryCompletionG12` (with SQL backfill of `DEFAULT` locations and `stock_location_balances`), `InventoryStore`, `InventoryHandler`, `WarehousesController` & `InventoryController`, localized error codes, Unit tests, and Integration tests.
- **Phase 2 — OpenAPI Contract Bridge (`Main Orchestrator`):**
  - `UPDATE_OPENAPI=1 dotnet test ... OpenApiContractTests` + `cd frontend && npm run generate:api`.
- **Phase 3 — Frontend UI Specialist (`Model: "flash"`, `frontend/` only):**
  - API client & React Query hooks, Warehouse Locations drawer/panel, Stock Document Modal support for `return` + `locationId`/`lotNumber`/`serialNumbers`, Inventory Period Close section/modal, `th.json`/`en.json` parity, and Vitest tests.
- **Phase 4 — Verification Gates & Documentation (`Main Orchestrator`):**
  - Full backend + frontend verification gates and documentation updates.
