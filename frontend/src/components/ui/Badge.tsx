"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export type BadgeVariant =
  | "neutral"
  | "primary"
  | "success"
  | "warning"
  | "danger"
  | "info"
  | "outline";

export interface BadgeProps extends React.HTMLAttributes<HTMLSpanElement> {
  variant?: BadgeVariant;
  size?: "sm" | "md";
  icon?: React.ReactNode;
  children: React.ReactNode;
}

const variantClasses: Record<BadgeVariant, string> = {
  neutral: "erp-badge-neutral",
  primary: "bg-erp-navy text-white border-erp-navy",
  success: "erp-badge-success",
  warning: "erp-badge-warning",
  danger: "erp-badge-danger",
  info: "erp-badge-info",
  outline: "bg-transparent text-erp-text-main border-erp-border",
};

const sizeClasses = {
  sm: "px-2 py-0.5 text-[11px] font-medium",
  md: "px-2.5 py-1 text-xs font-medium",
};

export function Badge({
  variant = "neutral",
  size = "sm",
  icon,
  children,
  className,
  ...props
}: BadgeProps) {
  return (
    <span
      className={cn(
        "erp-badge inline-flex items-center gap-1.5 rounded-none border transition-colors",
        variantClasses[variant],
        sizeClasses[size],
        className
      )}
      {...props}
    >
      {icon && <span className="inline-flex shrink-0">{icon}</span>}
      <span>{children}</span>
    </span>
  );
}

export default Badge;
