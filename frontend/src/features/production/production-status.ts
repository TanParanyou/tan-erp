export const BOM_REVISION_STATUSES = ["draft", "approved", "obsolete"] as const;

export type BomRevisionStatusValue = (typeof BOM_REVISION_STATUSES)[number];

export function isBomRevisionStatus(value: string | null | undefined): value is BomRevisionStatusValue {
  return BOM_REVISION_STATUSES.some((status) => status === value);
}

export const WORK_ORDER_STATUSES = ["draft", "released", "in_progress", "completed", "cancelled"] as const;

export type WorkOrderStatusValue = (typeof WORK_ORDER_STATUSES)[number];

export function isWorkOrderStatus(value: string | null | undefined): value is WorkOrderStatusValue {
  return WORK_ORDER_STATUSES.some((status) => status === value);
}

export type ProductionBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function bomRevisionStatusVariant(status: string | null | undefined): ProductionBadgeVariant {
  switch (status) {
    case "approved":
      return "success";
    case "draft":
      return "warning";
    default:
      return "neutral";
  }
}

export function workOrderStatusVariant(status: string | null | undefined): ProductionBadgeVariant {
  switch (status) {
    case "completed":
      return "success";
    case "released":
    case "in_progress":
      return "info";
    case "cancelled":
      return "danger";
    default:
      return "neutral";
  }
}

export const WORK_ORDER_TRANSACTION_KINDS = ["issue", "return", "completion"] as const;

export type WorkOrderTransactionKindValue = (typeof WORK_ORDER_TRANSACTION_KINDS)[number];

export function isWorkOrderTransactionKind(value: string | null | undefined): value is WorkOrderTransactionKindValue {
  return WORK_ORDER_TRANSACTION_KINDS.some((kind) => kind === value);
}

export const WORK_CENTER_STATUSES = ["active", "inactive"] as const;

export type WorkCenterStatusValue = (typeof WORK_CENTER_STATUSES)[number];

export function isWorkCenterStatus(value: string | null | undefined): value is WorkCenterStatusValue {
  return WORK_CENTER_STATUSES.some((status) => status === value);
}

export const OPERATION_STATUSES = ["pending", "completed"] as const;

export type OperationStatusValue = (typeof OPERATION_STATUSES)[number];

export function isOperationStatus(value: string | null | undefined): value is OperationStatusValue {
  return OPERATION_STATUSES.some((status) => status === value);
}

const KNOWN_ERROR_CODES = [
  "BOM_CODE_CONFLICT",
  "BOM_ALREADY_EXISTS",
  "BOM_VERSION_CONFLICT",
  "BOM_INVALID_STATE",
  "BOM_SELF_APPROVAL",
  "BOM_LINE_INVALID",
  "BOM_ROUTING_INVALID",
  "BOM_CYCLE",
  "BOM_ITEM_NOT_PRODUCIBLE",
  "BOM_COMPONENT_NOT_STOCKABLE",
  "PRODUCTION_BOM_NOT_APPROVED",
  "PRODUCTION_WORK_CENTER_CODE_CONFLICT",
  "PRODUCTION_WORK_CENTER_VERSION_CONFLICT",
  "PRODUCTION_WORK_CENTER_IN_USE",
  "PRODUCTION_WORK_CENTER_INACTIVE",
  "PRODUCTION_WORK_CENTER_INVALID",
  "PRODUCTION_OPERATION_NOT_SUBCONTRACTED",
  "PRODUCTION_OPERATION_ALREADY_COMPLETED",
  "PRODUCTION_OPERATION_INVALID",
  "PRODUCTION_SUBCONTRACT_PO_NOT_APPROVED",
  "PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO",
  "PRODUCTION_HAS_ACTUAL_COSTS",
  "PRODUCTION_VERSION_CONFLICT",
  "PRODUCTION_INVALID_STATE",
  "PRODUCTION_PROJECT_NOT_ACTIVE",
  "PRODUCTION_HAS_ISSUED_MATERIALS",
  "PRODUCTION_MATERIAL_SHORTAGE",
  "PRODUCTION_OVER_COMPLETED",
  "PRODUCTION_OVER_ISSUED",
  "PRODUCTION_RETURN_EXCEEDS",
  "PRODUCTION_MATERIAL_INVALID",
  "PRODUCTION_QUANTITY_INVALID",
  "PRODUCTION_REASON_REQUIRED",
  "PRODUCTION_FIELD_REQUIRED",
  "PRODUCTION_FIELD_INVALID",
  "INVENTORY_INSUFFICIENT_STOCK",
  "INVENTORY_WAREHOUSE_INACTIVE",
  "INVENTORY_WAREHOUSE_BRANCH_MISMATCH",
  "INVENTORY_ITEM_NOT_STOCKABLE",
] as const;

export type ProductionErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function productionErrorCode(code: string | null | undefined): ProductionErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
