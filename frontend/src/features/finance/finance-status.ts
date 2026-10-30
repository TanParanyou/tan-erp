export const BILLING_KINDS = ["deposit", "milestone", "final", "other"] as const;
export type BillingKindValue = (typeof BILLING_KINDS)[number];
export function isBillingKind(value: string | null | undefined): value is BillingKindValue {
  return BILLING_KINDS.some((kind) => kind === value);
}

export const BILLING_STATUSES = ["issued", "partially_paid", "paid", "voided"] as const;
export type BillingStatusValue = (typeof BILLING_STATUSES)[number];
export function isBillingStatus(value: string | null | undefined): value is BillingStatusValue {
  return BILLING_STATUSES.some((status) => status === value);
}

export const PAYMENT_METHODS = ["transfer", "cheque", "cash", "card"] as const;
export type PaymentMethodValue = (typeof PAYMENT_METHODS)[number];
export function isPaymentMethod(value: string | null | undefined): value is PaymentMethodValue {
  return PAYMENT_METHODS.some((method) => method === value);
}

export const OUTBOX_STATUSES = ["pending", "sent", "exported", "failed", "dead"] as const;
export type OutboxStatusValue = (typeof OUTBOX_STATUSES)[number];
export function isOutboxStatus(value: string | null | undefined): value is OutboxStatusValue {
  return OUTBOX_STATUSES.some((status) => status === value);
}

export const OUTBOX_KINDS = ["billing.issued", "billing.voided", "payment.recorded", "payment.reversed", "billing-note.approved"] as const;
export type OutboxKindValue = (typeof OUTBOX_KINDS)[number];
export function isOutboxKind(value: string | null | undefined): value is OutboxKindValue {
  return OUTBOX_KINDS.some((kind) => kind === value);
}

export const RECONCILIATION_ISSUES = ["unsynced", "awaiting_confirmation", "amount_mismatch", "missing_message"] as const;
export type ReconciliationIssueValue = (typeof RECONCILIATION_ISSUES)[number];
export function isReconciliationIssue(value: string | null | undefined): value is ReconciliationIssueValue {
  return RECONCILIATION_ISSUES.some((issue) => issue === value);
}

export type FinanceBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function billingStatusVariant(status: string | null | undefined): FinanceBadgeVariant {
  switch (status) {
    case "paid":
      return "success";
    case "partially_paid":
      return "info";
    case "issued":
      return "warning";
    case "voided":
      return "danger";
    default:
      return "neutral";
  }
}

export function outboxStatusVariant(status: string | null | undefined): FinanceBadgeVariant {
  switch (status) {
    case "sent":
      return "success";
    case "exported":
      return "info";
    case "pending":
      return "warning";
    case "failed":
    case "dead":
      return "danger";
    default:
      return "neutral";
  }
}

export function retentionStatusVariant(status: string | null | undefined): FinanceBadgeVariant {
  switch (status) {
    case "released":
      return "success";
    case "held":
      return "warning";
    case "none":
    default:
      return "neutral";
  }
}

export function billingNoteStatusVariant(status: string | null | undefined): FinanceBadgeVariant {
  switch (status) {
    case "approved":
      return "success";
    case "pending_approval":
      return "warning";
    default:
      return "neutral";
  }
}

export function creditOverrideStatusVariant(status: string | null | undefined): FinanceBadgeVariant {
  switch (status) {
    case "approved":
      return "success";
    case "pending_approval":
      return "warning";
    case "rejected":
    case "expired":
      return "danger";
    default:
      return "neutral";
  }
}

const KNOWN_ERROR_CODES = [
  "BILLING_VERSION_CONFLICT",
  "BILLING_INVALID_STATE",
  "BILLING_HAS_PAYMENTS",
  "BILLING_PROJECT_NOT_BILLABLE",
  "BILLING_EXCEEDS_CONTRACT",
  "BILLING_REASON_REQUIRED",
  "BILLING_FIELD_REQUIRED",
  "BILLING_FIELD_INVALID",
  "RETENTION_WARRANTY_NOT_EXPIRED",
  "BILLING_NOTE_SELF_APPROVAL",
  "BILLING_NOTE_INVALID_STATE",
  "BILLING_NOTE_EXCEEDS_ORIGINAL",
  "PAYMENT_DUPLICATE_REFERENCE",
  "PAYMENT_EXCEEDS_OUTSTANDING",
  "PAYMENT_INVALID_STATE",
  "PAYMENT_FIELD_INVALID",
  "PAYMENT_WHT_CERTIFICATE_REQUIRED",
  "CREDIT_LIMIT_EXCEEDED",
  "CREDIT_OVERRIDE_SELF_APPROVAL",
  "CREDIT_OVERRIDE_INVALID_STATE",
  "CREDIT_OVERRIDE_FIELD_INVALID",
  "ACCOUNT_MAPPING_FIELD_INVALID",
  "OUTBOX_INVALID_STATE",
  "OUTBOX_CONFIRMATION_CONFLICT",
  "OUTBOX_FIELD_INVALID",
] as const;

export type FinanceErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function financeErrorCode(code: string | null | undefined): FinanceErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
