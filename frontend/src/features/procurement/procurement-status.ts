export const PURCHASE_ORDER_STATUSES = [
  "draft",
  "submitted",
  "approved",
  "rejected",
  "partially_received",
  "received",
  "cancelled",
] as const;

export type PurchaseOrderStatusValue = (typeof PURCHASE_ORDER_STATUSES)[number];

export function isPurchaseOrderStatus(value: string | null | undefined): value is PurchaseOrderStatusValue {
  return PURCHASE_ORDER_STATUSES.some((status) => status === value);
}

export type ProcurementBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function purchaseOrderStatusVariant(status: string | null | undefined): ProcurementBadgeVariant {
  switch (status) {
    case "approved":
    case "received":
      return "success";
    case "partially_received":
      return "info";
    case "submitted":
      return "warning";
    case "rejected":
    case "cancelled":
      return "danger";
    default:
      return "neutral";
  }
}

export const SUPPLIER_STATUSES = ["active", "inactive"] as const;

export type SupplierStatusValue = (typeof SUPPLIER_STATUSES)[number];

export function isSupplierStatus(value: string | null | undefined): value is SupplierStatusValue {
  return SUPPLIER_STATUSES.some((status) => status === value);
}

const KNOWN_ERROR_CODES = [
  "SUPPLIER_CODE_CONFLICT",
  "SUPPLIER_VERSION_CONFLICT",
  "SUPPLIER_INVALID_STATE",
  "SUPPLIER_FIELD_INVALID",
  "PURCHASE_ORDER_VERSION_CONFLICT",
  "PURCHASE_ORDER_INVALID_STATE",
  "PURCHASE_ORDER_HAS_RECEIPTS",
  "PURCHASE_ORDER_SUPPLIER_INACTIVE",
  "PURCHASE_ORDER_PROJECT_NOT_ACTIVE",
  "PURCHASE_ORDER_OVER_BUDGET",
  "PURCHASE_ORDER_ITEM_NOT_PURCHASABLE",
  "PURCHASE_ORDER_LINE_INVALID",
  "PURCHASE_ORDER_SELF_APPROVAL",
  "PROCUREMENT_REASON_REQUIRED",
  "PROCUREMENT_FIELD_REQUIRED",
  "PROCUREMENT_FIELD_INVALID",
  "GOODS_RECEIPT_INVALID",
  "GOODS_RECEIPT_OVER_RECEIVED",
] as const;

export type ProcurementErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function procurementErrorCode(code: string | null | undefined): ProcurementErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
