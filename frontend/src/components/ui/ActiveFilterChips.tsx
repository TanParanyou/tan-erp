"use client";

import React from "react";
import { IconClose } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface ActiveFilterChipItem {
  key: string;
  value: string;
  label: string;
}

export interface ActiveFilterChipsProps {
  filters: ActiveFilterChipItem[];
  onRemove: (key: string, value: string) => void;
  onClear: () => void;
  className?: string;
}

export function ActiveFilterChips({
  filters,
  onRemove,
  onClear,
  className = "",
}: ActiveFilterChipsProps) {
  const t = useTranslations("common.filters");

  if (filters.length === 0) return null;

  return (
    <div className={`flex flex-wrap items-center gap-2 pt-2.5 border-t border-erp-border-subtle ${className}`}>
      <span className="text-xs text-erp-text-muted font-medium mr-1">
        {t("activeFiltersLabel")}:
      </span>
      {filters.map((chip, index) => (
        <span
          key={`${chip.key}-${chip.value}-${index}`}
          className="inline-flex items-center gap-1.5 rounded-none bg-erp-surface-muted px-2.5 py-1 text-xs font-medium text-erp-navy border border-erp-border"
        >
          <span className="truncate max-w-[200px]">{chip.label}</span>
          <button
            type="button"
            onClick={() => onRemove(chip.key, chip.value)}
            aria-label={`${t("removeFilter")} ${chip.label}`}
            className="flex items-center justify-center w-4 h-4 rounded-none hover:bg-erp-border text-erp-text-muted hover:text-erp-danger transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
          >
            <IconClose size={12} />
          </button>
        </span>
      ))}
      <button
        type="button"
        onClick={onClear}
        className="text-xs font-medium text-erp-danger hover:underline px-1.5 py-1 transition-colors ml-1 focus-visible:outline-2 focus-visible:outline-erp-danger"
      >
        {t("clearAll")}
      </button>
    </div>
  );
}

export default ActiveFilterChips;
