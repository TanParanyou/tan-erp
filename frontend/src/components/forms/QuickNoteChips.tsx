"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import { IconPlus } from "@/components/common/Icons";

export interface QuickTemplateItem {
  id: string;
  label: string;
  value?: string;
}

export interface QuickNoteChipsProps {
  label?: string;
  templates: QuickTemplateItem[];
  onSelect: (value: string) => void;
  disabled?: boolean;
  className?: string;
}

/**
 * QuickNoteChips: Global reusable component for clickable template chips
 * to quickly append or insert common notes into textareas and inputs.
 * Adheres strictly to Atelier Architectural Navy Sharp design (sharp corners, border-radius: 0px).
 */
export function QuickNoteChips({
  label,
  templates,
  onSelect,
  disabled = false,
  className,
}: QuickNoteChipsProps) {
  if (!templates || templates.length === 0) {
    return null;
  }

  return (
    <div className={cn("flex flex-col gap-1.5", className)}>
      {label && (
        <span className="text-xs font-semibold text-erp-text-muted tracking-wide">
          {label}
        </span>
      )}
      <div className="flex flex-wrap gap-2">
        {templates.map((tpl) => (
          <button
            key={tpl.id}
            type="button"
            disabled={disabled}
            onClick={() => onSelect(tpl.value ?? tpl.label)}
            className={cn(
              "inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium border transition-colors",
              "border-erp-border bg-erp-surface-muted text-erp-text-main",
              "hover:border-erp-navy hover:text-erp-navy hover:bg-erp-surface",
              "focus-visible:outline-2 focus-visible:outline-erp-navy",
              "disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:border-erp-border disabled:hover:text-erp-text-main"
            )}
            style={{ borderRadius: "0px" }}
          >
            <IconPlus size={12} className="text-erp-text-muted shrink-0" />
            <span>{tpl.label}</span>
          </button>
        ))}
      </div>
    </div>
  );
}
