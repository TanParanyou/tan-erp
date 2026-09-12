"use client";

import React from "react";
import { IconGrid, IconList } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export type ViewMode = "table" | "grid";

export interface ViewModeToggleProps {
  mode: ViewMode;
  onChange: (mode: ViewMode) => void;
  className?: string;
}

export function ViewModeToggle({
  mode,
  onChange,
  className,
}: ViewModeToggleProps) {
  return (
    <div className={cn("flex items-center border border-erp-border bg-erp-surface rounded-none p-0.5", className)}>
      <button
        type="button"
        onClick={() => onChange("table")}
        className={cn(
          "flex h-7 w-7 items-center justify-center transition-colors rounded-none",
          mode === "table"
            ? "bg-erp-navy text-white"
            : "text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-subtle"
        )}
        title="มุมมองตาราง"
      >
        <IconList size={14} />
      </button>
      <button
        type="button"
        onClick={() => onChange("grid")}
        className={cn(
          "flex h-7 w-7 items-center justify-center transition-colors rounded-none",
          mode === "grid"
            ? "bg-erp-navy text-white"
            : "text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-subtle"
        )}
        title="มุมมองการ์ด/กริด"
      >
        <IconGrid size={14} />
      </button>
    </div>
  );
}

ViewModeToggle.displayName = "ViewModeToggle";
