import React from "react";
import { cn } from "@/lib/utils/cn";

export type StatusBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export interface StatusBadgeProps {
  label?: string;
  status?: string;
  variant?: StatusBadgeVariant;
  className?: string;
  icon?: React.ReactNode;
}

const statusVariantMap: Record<string, StatusBadgeVariant> = {
  active: "success",
  approved: "success",
  completed: "success",
  paid: "success",
  posted: "success",
  done: "success",

  pending: "warning",
  waiting: "warning",
  in_progress: "warning",
  draft: "neutral",

  cancelled: "danger",
  voided: "danger",
  rejected: "danger",
  failed: "danger",
  inactive: "neutral",

  open: "info",
  review: "info",
  processing: "info",
};

export function StatusBadge({ label, status, variant, className, icon }: StatusBadgeProps) {
  const displayLabel = label || status || "-";
  const normalizedKey = (status || label || "").toLowerCase().trim();
  const resolvedVariant = variant || statusVariantMap[normalizedKey] || "neutral";

  return (
    <span className={cn("erp-badge", `erp-badge-${resolvedVariant}`, className)}>
      {icon && <span style={{ display: "inline-flex", alignItems: "center" }}>{icon}</span>}
      {displayLabel}
    </span>
  );
}
