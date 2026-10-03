export const PROJECT_STATUSES = [
  "planned",
  "active",
  "on_hold",
  "ready_for_handover",
  "completed",
  "cancelled",
] as const;

export type ProjectStatusValue = (typeof PROJECT_STATUSES)[number];

export function isProjectStatus(value: string | null | undefined): value is ProjectStatusValue {
  return PROJECT_STATUSES.some((status) => status === value);
}

export type ProjectBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function projectStatusVariant(status: string | null | undefined): ProjectBadgeVariant {
  switch (status) {
    case "active":
      return "info";
    case "completed":
      return "success";
    case "on_hold":
      return "warning";
    case "cancelled":
      return "danger";
    default:
      return "neutral";
  }
}

export const PROJECT_BUDGET_CATEGORIES = ["material", "labor", "subcontract", "service", "other"] as const;

export type ProjectBudgetCategoryValue = (typeof PROJECT_BUDGET_CATEGORIES)[number];

export function isBudgetCategory(value: string | null | undefined): value is ProjectBudgetCategoryValue {
  return PROJECT_BUDGET_CATEGORIES.some((category) => category === value);
}

export const CHANGE_ORDER_STATUSES = ["draft", "submitted", "approved", "rejected", "cancelled"] as const;

export type ChangeOrderStatusValue = (typeof CHANGE_ORDER_STATUSES)[number];

export function isChangeOrderStatus(value: string | null | undefined): value is ChangeOrderStatusValue {
  return CHANGE_ORDER_STATUSES.some((status) => status === value);
}

export function changeOrderStatusVariant(status: string | null | undefined): ProjectBadgeVariant {
  switch (status) {
    case "approved":
      return "success";
    case "submitted":
      return "warning";
    case "rejected":
    case "cancelled":
      return "danger";
    default:
      return "neutral";
  }
}

/** Mirrors the backend lifecycle so the UI only offers moves the API will accept. */
export function allowedProjectTransitions(status: string | null | undefined): ProjectStatusValue[] {
  switch (status) {
    case "planned":
      return ["active", "cancelled"];
    case "active":
      return ["on_hold", "ready_for_handover", "cancelled"];
    case "on_hold":
      return ["active", "cancelled"];
    case "ready_for_handover":
      return ["active", "completed"];
    default:
      return [];
  }
}

/** Transitions that need a typed reason (backend: PROJECT_REASON_REQUIRED). */
export function transitionNeedsReason(from: string | null | undefined, target: ProjectStatusValue): boolean {
  return target === "on_hold" || target === "cancelled" || (from === "ready_for_handover" && target === "active");
}
