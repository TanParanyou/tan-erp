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
