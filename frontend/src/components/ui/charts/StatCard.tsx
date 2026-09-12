"use client";

import React from "react";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";

export interface StatCardProps {
  title: string;
  value?: string | number | null;
  changePercent?: number | null;
  icon?: React.ReactNode;
  subtitle?: string;
  isLoading?: boolean;
  className?: string;
}

export function StatCard({
  title,
  value,
  changePercent,
  icon,
  subtitle,
  isLoading = false,
  className,
}: StatCardProps) {
  return (
    <div
      className={cn(
        "relative flex flex-col justify-between border border-erp-border bg-erp-surface p-4 rounded-none shadow-2xs",
        className
      )}
    >
      <div className="flex items-center justify-between text-erp-text-muted">
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-muted">
          {title}
        </span>
        {icon && <div className="text-erp-text-muted opacity-80">{icon}</div>}
      </div>

      <div className="my-3">
        {isLoading ? (
          <div className="py-2">
            <MonoSpinner size="sm" />
          </div>
        ) : (
          <div className="text-2xl font-bold font-mono tracking-tight text-erp-text-main">
            {value ?? "-"}
          </div>
        )}
      </div>

      <div className="flex items-center justify-between text-xs text-erp-text-muted">
        {subtitle && <span>{subtitle}</span>}
        {changePercent !== undefined && changePercent !== null && (
          <span
            className={cn(
              "font-mono font-medium",
              changePercent >= 0 ? "text-emerald-600 dark:text-emerald-400" : "text-red-600 dark:text-red-400"
            )}
          >
            {changePercent >= 0 ? "▲ +" : "▼ "}
            {changePercent.toFixed(1)}%
          </span>
        )}
      </div>
    </div>
  );
}

StatCard.displayName = "StatCard";
