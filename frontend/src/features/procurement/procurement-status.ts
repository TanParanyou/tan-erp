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

export const PURCHASE_REQUEST_STATUSES = [
  "draft",
  "submitted",
  "approved",
  "rejected",
  "cancelled",
  "converted",
] as const;

export type PurchaseRequestStatusValue = (typeof PURCHASE_REQUEST_STATUSES)[number];

export function isPurchaseRequestStatus(value: string | null | undefined): value is PurchaseRequestStatusValue {
  return PURCHASE_REQUEST_STATUSES.some((status) => status === value);
}

export function purchaseRequestStatusVariant(status: string | null | undefined): ProcurementBadgeVariant {
  switch (status) {
    case "approved":
      return "success";
    case "converted":
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

export const SUPPLIER_INVOICE_STATUSES = [
  "draft",
  "on_hold",
  "matched",
  "approved",
  "rejected",
  "cancelled",
  "voided",
] as const;

export type SupplierInvoiceStatusValue = (typeof SUPPLIER_INVOICE_STATUSES)[number];

export function isSupplierInvoiceStatus(value: string | null | undefined): value is SupplierInvoiceStatusValue {
  return SUPPLIER_INVOICE_STATUSES.some((status) => status === value);
}

export function supplierInvoiceStatusVariant(status: string | null | undefined): ProcurementBadgeVariant {
  switch (status) {
    case "matched":
    case "approved":
      return "success";
    case "on_hold":
      return "warning";
    case "rejected":
    case "cancelled":
    case "voided":
      return "danger";
    default:
      return "neutral";
  }
}

export const SUPPLIER_INVOICE_MATCH_STATUSES = ["pending", "matched", "price_variance"] as const;

export type SupplierInvoiceMatchStatusValue = (typeof SUPPLIER_INVOICE_MATCH_STATUSES)[number];

export function isSupplierInvoiceMatchStatus(value: string | null | undefined): value is SupplierInvoiceMatchStatusValue {
  return SUPPLIER_INVOICE_MATCH_STATUSES.some((status) => status === value);
}

export function supplierInvoiceMatchStatusVariant(status: string | null | undefined): ProcurementBadgeVariant {
  switch (status) {
    case "matched":
      return "success";
    case "price_variance":
      return "warning";
    default:
      return "neutral";
  }
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
  "APPROVAL_LIMIT_EXCEEDED",
  "APPROVAL_AUTHORITY_NOT_CONFIGURED",
  "APPROVAL_LIMIT_NOT_CONFIGURED",
  "PROCUREMENT_REASON_REQUIRED",
  "PROCUREMENT_FIELD_REQUIRED",
  "PROCUREMENT_FIELD_INVALID",
  "GOODS_RECEIPT_INVALID",
  "GOODS_RECEIPT_OVER_RECEIVED",
  "PURCHASE_REQUEST_VERSION_CONFLICT",
  "PURCHASE_REQUEST_INVALID_STATE",
  "PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE",
  "PURCHASE_REQUEST_LINE_INVALID",
  "PURCHASE_REQUEST_SELF_APPROVAL",
  "SUPPLIER_RETURN_INVALID",
  "SUPPLIER_RETURN_OVER_RETURNED",
  "SUPPLIER_RETURN_ALREADY_INVOICED",
  "INVENTORY_INSUFFICIENT_STOCK",
  "SUPPLIER_INVOICE_VERSION_CONFLICT",
  "SUPPLIER_INVOICE_INVALID_STATE",
  "SUPPLIER_INVOICE_DUPLICATE_NUMBER",
  "SUPPLIER_INVOICE_LINE_INVALID",
  "SUPPLIER_INVOICE_OVER_RECEIVED",
  "SUPPLIER_INVOICE_SELF_APPROVAL",
] as const;

export type ProcurementErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function procurementErrorCode(code: string | null | undefined): ProcurementErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
