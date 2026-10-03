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

export const ACCEPTANCE_LINK_STATES = ["active", "expired", "revoked", "accepted", "unavailable"] as const;

export type AcceptanceLinkState = (typeof ACCEPTANCE_LINK_STATES)[number];

interface AcceptanceLinkLike {
  status?: string | null;
  isUsable?: boolean | null;
  expiresAtUtc?: string | null;
}

/** Display state from what the API reports: a usable link is active; otherwise why it is not. */
export function acceptanceLinkStateLabel(link: AcceptanceLinkLike): AcceptanceLinkState {
  if (link.status === "accepted") return "accepted";
  if (link.status === "revoked") return "revoked";
  if (link.isUsable) return "active";
  if (link.status === "active") {
    return link.expiresAtUtc && new Date(link.expiresAtUtc).getTime() <= Date.now() ? "expired" : "unavailable";
  }

  return "unavailable";
}

export function acceptanceLinkStateVariant(state: AcceptanceLinkState): QuotationBadgeVariant {
  switch (state) {
    case "accepted":
      return "success";
    case "active":
      return "info";
    case "revoked":
      return "danger";
    default:
      return "neutral";
  }
}

const KNOWN_ACCEPTANCE_ERROR_CODES = [
  "ACCEPTANCE_LINK_UNAVAILABLE",
  "ACCEPTANCE_LINK_INVALID_STATE",
  "ACCEPTANCE_LINK_LIFETIME_INVALID",
  "ACCEPTANCE_SUBMISSION_INVALID",
  "ACCEPTANCE_CONSENT_REQUIRED",
  "ACCEPTANCE_CONFLICT",
  "QUOTATION_INVALID_STATE",
] as const;

export type AcceptanceErrorCode = (typeof KNOWN_ACCEPTANCE_ERROR_CODES)[number];

export function acceptanceLinkErrorCode(code: string | null | undefined): AcceptanceErrorCode | null {
  return KNOWN_ACCEPTANCE_ERROR_CODES.find((known) => known === code) ?? null;
}
