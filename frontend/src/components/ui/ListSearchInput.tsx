"use client";

import React from "react";
import { IconSearch, IconClose } from "@/components/common/Icons";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";

export interface ListSearchInputProps {
  id?: string;
  label?: string;
  value: string;
  isDebouncing?: boolean;
  placeholder?: string;
  onChange: (value: string) => void;
  onClear?: () => void;
  onSubmit?: (value: string) => void;
  className?: string;
  widthClassName?: string;
}

export function ListSearchInput({
  id = "list-search-input",
  label,
  value,
  isDebouncing = false,
  placeholder,
  onChange,
  onClear,
  onSubmit,
  className,
  widthClassName = "w-full md:w-72",
}: ListSearchInputProps) {
  const tCommon = useTranslations("common.actions");
  const displayLabel = label ?? tCommon("search");

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      onSubmit?.(value);
    }
  };

  return (
    <div className={cn("flex flex-col gap-1.5", widthClassName, className)}>
      {displayLabel && (
        <label htmlFor={id} className="text-sm font-semibold text-erp-navy">
          {displayLabel}
        </label>
      )}
      <div className="relative flex items-center">
        <div className="absolute left-3 flex items-center pointer-events-none text-erp-text-muted">
          {isDebouncing ? (
            <MonoSpinner size="sm" aria-label="Searching..." />
          ) : (
            <IconSearch size={15} />
          )}
        </div>
        <input
          id={id}
          type="search"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder={placeholder}
          aria-label={displayLabel}
          className="h-10 w-full pl-9 pr-9 text-xs sm:text-sm border border-erp-border bg-erp-surface text-erp-text-main placeholder:text-erp-text-muted focus-visible:border-erp-navy focus-visible:outline-2 focus-visible:outline-offset-0 focus-visible:outline-erp-navy transition-colors rounded-none"
        />
        {value && (
          <button
            type="button"
            onClick={onClear ?? (() => onChange(""))}
            aria-label={tCommon("clear")}
            className="absolute right-1.5 p-1 text-erp-text-muted hover:text-erp-text-main transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy rounded-none"
          >
            <IconClose size={14} />
          </button>
        )}
      </div>
    </div>
  );
}

export default ListSearchInput;
