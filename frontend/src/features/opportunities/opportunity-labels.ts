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
