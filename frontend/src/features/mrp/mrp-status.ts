export const MRP_ACTIONS = ["buy", "make", "shortage"] as const;

export type MrpActionValue = (typeof MRP_ACTIONS)[number];

export function isMrpAction(value: string | null | undefined): value is MrpActionValue {
  return MRP_ACTIONS.some((action) => action === value);
}

export const MRP_RECOMMENDATION_STATUSES = ["proposed", "approved", "rejected", "converted"] as const;

export type MrpRecommendationStatusValue = (typeof MRP_RECOMMENDATION_STATUSES)[number];

export function isMrpRecommendationStatus(value: string | null | undefined): value is MrpRecommendationStatusValue {
  return MRP_RECOMMENDATION_STATUSES.some((status) => status === value);
}

export const MRP_REASON_SOURCES = ["manual", "work_order", "dependent"] as const;

export type MrpReasonSourceValue = (typeof MRP_REASON_SOURCES)[number];

export function isMrpReasonSource(value: string | null | undefined): value is MrpReasonSourceValue {
  return MRP_REASON_SOURCES.some((source) => source === value);
}

export type MrpBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function mrpStatusVariant(status: string | null | undefined): MrpBadgeVariant {
  switch (status) {
    case "converted":
      return "success";
    case "approved":
      return "info";
    case "proposed":
      return "warning";
    case "rejected":
      return "danger";
    default:
      return "neutral";
  }
}

const KNOWN_ERROR_CODES = [
  "MRP_BOM_CYCLE",
  "MRP_INVALID_STATE",
  "MRP_SELF_APPROVAL",
  "MRP_NOT_CONVERTIBLE",
  "MRP_VERSION_CONFLICT",
  "MRP_FIELD_REQUIRED",
  "MRP_FIELD_INVALID",
  "MRP_DEMAND_INVALID",
  "MRP_CONVERT_INPUT_REQUIRED",
  "PURCHASE_ORDER_ITEM_NOT_PURCHASABLE",
  "PURCHASE_ORDER_SUPPLIER_INACTIVE",
  "PRODUCTION_BOM_NOT_APPROVED",
  "INVENTORY_WAREHOUSE_INACTIVE",
  "INVENTORY_WAREHOUSE_BRANCH_MISMATCH",
] as const;

export type MrpErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function mrpErrorCode(code: string | null | undefined): MrpErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
