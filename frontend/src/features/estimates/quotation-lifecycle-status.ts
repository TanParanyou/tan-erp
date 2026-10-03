export const QUOTATION_STATUSES = ["issued", "accepted", "superseded", "voided"] as const;

export type QuotationStatusValue = (typeof QUOTATION_STATUSES)[number];

export function isQuotationStatus(value: string | null | undefined): value is QuotationStatusValue {
  return QUOTATION_STATUSES.some((status) => status === value);
}

export type QuotationBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function quotationStatusVariant(status: string | null | undefined): QuotationBadgeVariant {
  switch (status) {
    case "accepted":
      return "success";
    case "issued":
      return "info";
    case "voided":
      return "danger";
    default:
      return "neutral";
  }
}

const KNOWN_ERROR_CODES = [
  "QUOTATION_INVALID_STATE",
  "QUOTATION_ACCEPTED_LOCKED",
  "QUOTATION_AMEND_REVISION_PENDING",
  "QUOTATION_REASON_REQUIRED",
  "QUOTATION_FIELD_INVALID",
  "QUOTATION_VERSION_CONFLICT",
  "ESTIMATE_INVALID_STATE",
  "CUSTOMER_QUOTATION_BILLING_NOT_READY",
  "IDEMPOTENCY_KEY_REUSED",
] as const;

export type QuotationLifecycleErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function quotationLifecycleErrorCode(code: string | null | undefined): QuotationLifecycleErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
