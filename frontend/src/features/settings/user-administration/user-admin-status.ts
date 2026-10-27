import type { BadgeVariant } from "@/components/ui/StatusBadge";

export const STATUS_VARIANTS: Record<string, BadgeVariant> = {
  active: "success",
  pending: "warning",
  inactive: "neutral",
};
