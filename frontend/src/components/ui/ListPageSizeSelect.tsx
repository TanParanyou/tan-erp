"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface ListPageSizeSelectProps {
  limit: number;
  onLimitChange: (limit: number) => void;
  options?: number[];
  className?: string;
  label?: string;
}

export function ListPageSizeSelect({
  limit,
  onLimitChange,
  options = [10, 20, 50, 100],
  className,
  label = "แสดงต่อหน้า",
}: ListPageSizeSelectProps) {
  return (
    <div className={cn("flex items-center gap-2 text-xs text-erp-text-muted", className)}>
      <span>{label}</span>
      <select
        value={limit}
        onChange={(e) => onLimitChange(Number(e.target.value))}
        className="border border-erp-border bg-erp-surface px-2 py-1 text-xs text-erp-text-main outline-none focus:border-erp-navy rounded-none cursor-pointer"
      >
        {options.map((opt) => (
          <option key={opt} value={opt}>
            {opt}
          </option>
        ))}
      </select>
    </div>
  );
}

ListPageSizeSelect.displayName = "ListPageSizeSelect";
