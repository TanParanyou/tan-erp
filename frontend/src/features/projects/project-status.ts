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
