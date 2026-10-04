export const WORK_TYPES = ["built-in", "curtain", "wallpaper"] as const;
export type WorkTypeValue = (typeof WORK_TYPES)[number];
export function isWorkType(value: string | null | undefined): value is WorkTypeValue {
  return WORK_TYPES.some((type) => type === value);
}

export const MEASUREMENT_RULES = ["area", "length", "volume", "count"] as const;
export type MeasurementRuleValue = (typeof MEASUREMENT_RULES)[number];
export function isMeasurementRule(value: string | null | undefined): value is MeasurementRuleValue {
  return MEASUREMENT_RULES.some((rule) => rule === value);
}

export const TAX_DISPLAYS = ["exclusive", "inclusive"] as const;
export type TaxDisplayValue = (typeof TAX_DISPLAYS)[number];
export function isTaxDisplay(value: string | null | undefined): value is TaxDisplayValue {
  return TAX_DISPLAYS.some((display) => display === value);
}

export const TEMPLATE_STATUSES = ["draft", "submitted", "approved", "calibration", "active", "superseded", "disabled"] as const;
export type TemplateStatusValue = (typeof TEMPLATE_STATUSES)[number];
export function isTemplateStatus(value: string | null | undefined): value is TemplateStatusValue {
  return TEMPLATE_STATUSES.some((status) => status === value);
}

export const QUICK_ESTIMATE_STATUSES = ["draft", "calculated", "pending_review", "approved", "returned", "converted"] as const;
export type QuickEstimateStatusValue = (typeof QUICK_ESTIMATE_STATUSES)[number];
export function isQuickEstimateStatus(value: string | null | undefined): value is QuickEstimateStatusValue {
  return QUICK_ESTIMATE_STATUSES.some((status) => status === value);
}

export const CONFIDENCES = ["low", "medium", "high"] as const;
export type ConfidenceValue = (typeof CONFIDENCES)[number];
export function isConfidence(value: string | null | undefined): value is ConfidenceValue {
  return CONFIDENCES.some((confidence) => confidence === value);
}

export const SHARE_DECISIONS = ["blocked", "pending_review", "shareable"] as const;
export type ShareDecisionValue = (typeof SHARE_DECISIONS)[number];
export function isShareDecision(value: string | null | undefined): value is ShareDecisionValue {
  return SHARE_DECISIONS.some((decision) => decision === value);
}

export const SHARE_REASONS = ["CUSTOM_MATERIAL", "TEMPLATE_IN_CALIBRATION", "MEASUREMENT_CONFIDENCE_LOW", "RANGE_AT_MAXIMUM", "ABOVE_DIRECT_SHARE_LIMIT"] as const;
export type ShareReasonValue = (typeof SHARE_REASONS)[number];
export function isShareReason(value: string | null | undefined): value is ShareReasonValue {
  return SHARE_REASONS.some((reason) => reason === value);
}

export type QuickEstimateBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function templateStatusVariant(status: string | null | undefined): QuickEstimateBadgeVariant {
  switch (status) {
    case "active": return "success";
    case "submitted":
    case "calibration": return "warning";
    case "approved": return "info";
    case "disabled": return "danger";
    default: return "neutral";
  }
}

export function quickEstimateStatusVariant(status: string | null | undefined): QuickEstimateBadgeVariant {
  switch (status) {
    case "approved":
    case "converted": return "success";
    case "pending_review": return "warning";
    case "returned": return "danger";
    case "calculated": return "info";
    default: return "neutral";
  }
}

export function shareDecisionVariant(decision: string | null | undefined): QuickEstimateBadgeVariant {
  switch (decision) {
    case "shareable": return "success";
    case "pending_review": return "warning";
    case "blocked": return "danger";
    default: return "neutral";
  }
}

const KNOWN_ERROR_CODES = [
  "PRICING_TEMPLATE_INVALID",
  "PRICING_TEMPLATE_CODE_EXISTS",
  "PRICING_TEMPLATE_DRAFT_EXISTS",
  "PRICING_TEMPLATE_VERSION_CONFLICT",
  "PRICING_TEMPLATE_INVALID_STATE",
  "PRICING_TEMPLATE_NOT_USABLE",
  "PRICING_TEMPLATE_SELF_APPROVAL",
  "QUICK_ESTIMATE_VERSION_CONFLICT",
  "QUICK_ESTIMATE_INVALID_STATE",
  "QUICK_ESTIMATE_STALE_VERSION",
  "QUICK_ESTIMATE_EXPIRED",
  "QUICK_ESTIMATE_ALREADY_CONVERTED",
  "QUICK_ESTIMATE_SELF_REVIEW",
  "QUICK_ESTIMATE_INPUT_INCOMPLETE",
  "QUICK_ESTIMATE_OPTION_INVALID",
  "QUICK_ESTIMATE_TEMPLATE_REQUIRED",
  "QUICK_ESTIMATE_SHARE_BLOCKED",
  "QUICK_ESTIMATE_REVIEW_REQUIRED",
  "QUICK_ESTIMATE_OPPORTUNITY_REQUIRED",
  "QUICK_ESTIMATE_REASON_REQUIRED",
  "QUICK_ESTIMATE_FIELD_REQUIRED",
  "QUICK_ESTIMATE_FIELD_INVALID",
] as const;

export type QuickEstimateErrorCode = (typeof KNOWN_ERROR_CODES)[number];

export function quickEstimateErrorCode(code: string | null | undefined): QuickEstimateErrorCode | null {
  return KNOWN_ERROR_CODES.find((known) => known === code) ?? null;
}
