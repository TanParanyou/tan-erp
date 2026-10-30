export const MOVEMENT_KINDS = ["receipt", "issue", "transfer_out", "transfer_in", "adjustment_in", "adjustment_out", "return_in", "return_out"] as const;

export type MovementKindValue = (typeof MOVEMENT_KINDS)[number];

export function isMovementKind(value: string | null | undefined): value is MovementKindValue {
  return MOVEMENT_KINDS.some((kind) => kind === value);
}

export const WAREHOUSE_STATUSES = ["active", "inactive"] as const;

export type WarehouseStatusValue = (typeof WAREHOUSE_STATUSES)[number];

export function isWarehouseStatus(value: string | null | undefined): value is WarehouseStatusValue {
  return WAREHOUSE_STATUSES.some((status) => status === value);
}

export const SERIAL_STATUSES = ["in_stock", "issued", "returned_out"] as const;

export type SerialStatusValue = (typeof SERIAL_STATUSES)[number];

export function isSerialStatus(value: string | null | undefined): value is SerialStatusValue {
  return SERIAL_STATUSES.some((status) => status === value);
}

const KNOWN_ERROR_CODES = [
  "WAREHOUSE_CODE_CONFLICT",
  "WAREHOUSE_VERSION_CONFLICT",
  "WAREHOUSE_INVALID_STATE",
  "WAREHOUSE_HAS_STOCK",
  "WAREHOUSE_FIELD_INVALID",
  "LOCATION_CODE_CONFLICT",
  "LOCATION_VERSION_CONFLICT",
  "LOCATION_IS_DEFAULT",
  "LOCATION_HAS_STOCK",
  "LOCATION_FIELD_INVALID",
  "INVENTORY_WAREHOUSE_INACTIVE",
  "INVENTORY_WAREHOUSE_BRANCH_MISMATCH",
  "INVENTORY_LOCATION_INACTIVE",
  "INVENTORY_RECEIPT_ALREADY_POSTED",
  "INVENTORY_ITEM_NOT_STOCKABLE",
  "INVENTORY_INSUFFICIENT_STOCK",
  "INVENTORY_LOT_REQUIRED",
  "INVENTORY_SERIAL_COUNT_MISMATCH",
  "INVENTORY_SERIAL_DUPLICATE",
  "INVENTORY_SERIAL_NOT_AVAILABLE",
  "INVENTORY_RETURN_EXCEEDS_ISSUED",
  "INVENTORY_PERIOD_INVALID",
  "INVENTORY_PERIOD_CLOSED",
  "INVENTORY_PERIOD_ALREADY_CLOSED",
  "INVENTORY_QUANTITY_INVALID",
  "INVENTORY_COST_INVALID",
  "INVENTORY_COST_REQUIRED",
  "INVENTORY_NO_CHANGE",
  "INVENTORY_LINE_INVALID",
  "INVENTORY_REASON_REQUIRED",
  "INVENTORY_TRANSFER_SAME_WAREHOUSE",
  "INVENTORY_PROJECT_NOT_ACTIVE",
  "INVENTORY_RESERVATION_INVALID",
  "INVENTORY_RESERVATION_VERSION_CONFLICT",
  "INVENTORY_VERSION_CONFLICT",
  "INVENTORY_FIELD_REQUIRED",
  "INVENTORY_FIELD_INVALID",
] as const;

export type InventoryErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function inventoryErrorCode(code: string | null | undefined): InventoryErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
