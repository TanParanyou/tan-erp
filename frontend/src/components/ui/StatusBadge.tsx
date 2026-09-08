"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export type BadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export interface StatusBadgeProps {
  label: string;
  variant?: BadgeVariant;
  className?: string;
  icon?: React.ReactNode;
}

const variantClasses: Record<BadgeVariant, string> = {
  success: "erp-badge-success",
  warning: "erp-badge-warning",
  danger: "erp-badge-danger",
  info: "erp-badge-info",
  neutral: "erp-badge-neutral",
};

const statusVariantMap: Record<string, BadgeVariant> = {
  active: "success",
  confirmed: "success",
  approved: "success",
  completed: "success",
  pending: "warning",
  draft: "warning",
  review: "warning",
  inactive: "neutral",
  archived: "neutral",
  disabled: "danger",
  cancelled: "danger",
  rejected: "danger",
};

export function StatusBadge({ label, variant, className, icon }: StatusBadgeProps) {
  const safeLabel = label || "";
  const resolvedVariant =
    variant || statusVariantMap[safeLabel.toLowerCase()] || "neutral";

  return (
    <span
      className={cn(
        "erp-badge",
        variantClasses[resolvedVariant],
        className
      )}
    >
      {icon && <span className="inline-flex shrink-0">{icon}</span>}
      {safeLabel}
    </span>
  );
}
