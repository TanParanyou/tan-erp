export const CANONICAL_OPPORTUNITY_STAGES = [
  "draft",
  "qualified",
  "surveying",
  "estimating",
  "proposed",
  "won",
  "lost",
  "cancelled",
] as const;

export type OpportunityStageKey = (typeof CANONICAL_OPPORTUNITY_STAGES)[number];

export function getOpportunityStageLabelKey(value: string | null | undefined): OpportunityStageKey | null {
  if (!value) return null;
  const normalized = value.trim().toLowerCase();
  if (CANONICAL_OPPORTUNITY_STAGES.includes(normalized as OpportunityStageKey)) {
    return normalized as OpportunityStageKey;
  }
  return null;
}

export function resolveOpportunityStageLabel(
  stage: string | null | undefined,
  t: (key: string) => string
): string {
  const key = getOpportunityStageLabelKey(stage);
  if (!key) return t("unknownStage");
  switch (key) {
    case "draft":
      return t("stageDraft");
    case "qualified":
      return t("stageQualified");
    case "surveying":
      return t("stageSurveying");
    case "estimating":
      return t("stageEstimating");
    case "proposed":
      return t("stageProposed");
    case "won":
      return t("stageWon");
    case "lost":
      return t("stageLost");
    case "cancelled":
      return t("stageCancelled");
    default:
      return t("unknownStage");
  }
}

export const CANONICAL_WORK_TYPES = [
  "built-in",
  "interior",
  "curtain",
  "wallpaper",
  "exterior",
  "other",
] as const;

export type WorkTypeKey = (typeof CANONICAL_WORK_TYPES)[number];

export function getWorkTypeLabelKey(value: string | null | undefined): WorkTypeKey | null {
  if (!value) return null;
  const normalized = value.trim().toLowerCase();
  if (CANONICAL_WORK_TYPES.includes(normalized as WorkTypeKey)) {
    return normalized as WorkTypeKey;
  }
  return null;
}

export const CANONICAL_OPPORTUNITY_LEAD_SOURCES = [
  "customer_referral",
  "architect_partner",
  "website_social",
  "expo_event",
  "direct_sales",
  "other",
] as const;

export type OpportunityLeadSourceKey = (typeof CANONICAL_OPPORTUNITY_LEAD_SOURCES)[number];

export function getOpportunityLeadSourceLabelKey(value: string | null | undefined): OpportunityLeadSourceKey | null {
  if (!value) return null;
  const normalized = value.trim().toLowerCase();
  if (CANONICAL_OPPORTUNITY_LEAD_SOURCES.includes(normalized as OpportunityLeadSourceKey)) {
    return normalized as OpportunityLeadSourceKey;
  }
  return null;
}

export function resolveWorkTypeLabel(wt: string, t: (key: string) => string): string {
  const key = getWorkTypeLabelKey(wt);
  if (!key) return wt;
  switch (key) {
    case "built-in":
      return t("workTypeBuiltIn");
    case "interior":
      return t("workTypeInterior");
    case "curtain":
      return t("workTypeCurtain");
    case "wallpaper":
      return t("workTypeWallpaper");
    case "exterior":
      return t("workTypeExterior");
    case "other":
      return t("workTypeOther");
    default:
      return wt;
  }
}

export function getOpportunityLeadSourceOptions(t: (key: string) => string): { value: string; label: string }[] {
  return [
    { value: "customer_referral", label: t("leadSourceReferral") },
    { value: "architect_partner", label: t("leadSourcePartner") },
    { value: "website_social", label: t("leadSourceWebsiteSocial") },
    { value: "expo_event", label: t("leadSourceExpo") },
    { value: "direct_sales", label: t("leadSourceDirect") },
    { value: "other", label: t("leadSourceOther") },
  ];
}

export const CANONICAL_LOST_REASONS = [
  "lost_price_too_high",
  "lost_competitor_selected",
  "lost_scope_mismatch",
  "lost_timeline_unfeasible",
  "lost_no_response",
  "lost_other",
] as const;

export type LostReasonKey = (typeof CANONICAL_LOST_REASONS)[number];

export const CANONICAL_CANCELLED_REASONS = [
  "cancelled_customer_abandoned",
  "cancelled_duplicate",
  "cancelled_invalid_lead",
  "cancelled_force_majeure",
  "cancelled_internal_decision",
  "cancelled_other",
] as const;

export type CancelledReasonKey = (typeof CANONICAL_CANCELLED_REASONS)[number];

export const CANONICAL_REOPEN_REASONS = [
  "reopen_customer_reengaged",
  "reopen_budget_adjusted",
  "reopen_scope_redefined",
  "reopen_erroneous_closure",
  "reopen_other",
] as const;

export type ReopenReasonKey = (typeof CANONICAL_REOPEN_REASONS)[number];

export function getLostReasonOptions(t: (key: string) => string): { value: string; label: string }[] {
  return CANONICAL_LOST_REASONS.map((code) => ({
    value: code,
    label: t(`reasons.${code}`),
  }));
}

export function getCancelledReasonOptions(t: (key: string) => string): { value: string; label: string }[] {
  return CANONICAL_CANCELLED_REASONS.map((code) => ({
    value: code,
    label: t(`reasons.${code}`),
  }));
}

export function getReopenReasonOptions(t: (key: string) => string): { value: string; label: string }[] {
  return CANONICAL_REOPEN_REASONS.map((code) => ({
    value: code,
    label: t(`reasons.${code}`),
  }));
}

export function resolveReasonLabel(reasonCode: string | null | undefined, t: (key: string) => string): string | null {
  if (!reasonCode) return null;
  const isKnown =
    (CANONICAL_LOST_REASONS as readonly string[]).includes(reasonCode) ||
    (CANONICAL_CANCELLED_REASONS as readonly string[]).includes(reasonCode) ||
    (CANONICAL_REOPEN_REASONS as readonly string[]).includes(reasonCode);

  if (isKnown) {
    return t(`reasons.${reasonCode}`);
  }
  return reasonCode;
}


