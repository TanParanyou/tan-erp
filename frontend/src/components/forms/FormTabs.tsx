"use client";

import React from "react";
import type { FieldErrors, FieldValues } from "react-hook-form";
import { cn } from "@/lib/utils/cn";

export interface FormTabItem<T extends string = string> {
  id: T;
  label: string;
  count?: number;
  /** Explicit error flag. If not provided, can be inferred via useFormTabErrors */
  hasError?: boolean;
  disabled?: boolean;
}

export interface FormTabsProps<T extends string = string> {
  tabs: FormTabItem<T>[];
  activeTab: T;
  onChange: (tabId: T) => void;
  ariaLabel?: string;
  className?: string;
}

/**
 * Atelier Architectural Navy Sharp FormTabs component.
 * Features:
 * - 0px sharp corners
 * - Solid Navy #0B3056 for active state
 * - Red Dot Error Indicator for tabs with validation failures
 * - Full accessibility (role="tablist", role="tab", aria-selected)
 */
export function FormTabs<T extends string = string>({
  tabs,
  activeTab,
  onChange,
  ariaLabel,
  className,
}: FormTabsProps<T>) {
  return (
    <div
      role="tablist"
      aria-label={ariaLabel}
      className={cn(
        "flex w-full border border-erp-border bg-erp-surface p-1 gap-1 select-none",
        className
      )}
    >
      {tabs.map((tab) => {
        const isActive = tab.id === activeTab;
        return (
          <button
            key={tab.id}
            type="button"
            role="tab"
            id={`tab-${tab.id}`}
            aria-controls={`tabpanel-${tab.id}`}
            aria-selected={isActive}
            disabled={tab.disabled}
            onClick={() => onChange(tab.id)}
            className={cn(
              "flex-1 min-h-[42px] sm:min-h-[44px] py-2 px-3 text-xs sm:text-sm font-bold transition-colors text-center border flex items-center justify-center gap-2 relative",
              isActive
                ? "bg-erp-navy text-white border-erp-navy shadow-xs"
                : "bg-erp-surface text-erp-text-main border-transparent hover:bg-erp-surface-subtle",
              tab.disabled && "opacity-50 cursor-not-allowed"
            )}
          >
            <span>{tab.label}</span>

            {/* Optional Item Count Badge */}
            {tab.count !== undefined && (
              <span
                className={cn(
                  "px-1.5 py-0.2 font-mono text-[10px] border",
                  isActive
                    ? "bg-white text-erp-navy border-white"
                    : "bg-erp-surface-subtle text-erp-navy border-erp-border"
                )}
              >
                {tab.count}
              </span>
            )}

            {/* Red Dot Error Indicator */}
            {tab.hasError && (
              <span
                role="status"
                aria-label="has error"
                className={cn(
                  "w-2 h-2 shrink-0 bg-erp-danger animate-pulse",
                  isActive ? "ring-2 ring-white" : ""
                )}
                title="This section contains errors"
              />
            )}
          </button>
        );
      })}
    </div>
  );
}

/**
 * Hook to manage tabs with automatic error indicator detection and auto-switch on submit error.
 *
 * @param options.tabs Tab definitions with field associations
 * @param options.errors Current react-hook-form errors object
 * @param options.activeTab Current active tab state
 * @param options.setActiveTab State setter for active tab
 */
export function useFormTabErrors<T extends string, TFieldValues extends FieldValues = FieldValues>({
  tabFieldsMap,
  errors,
  setActiveTab,
}: {
  tabFieldsMap: Record<T, (keyof TFieldValues | string)[]>;
  errors: FieldErrors<TFieldValues>;
  setActiveTab: (tab: T) => void;
}) {
  // Check which tabs have errors
  const tabErrorMap = React.useMemo(() => {
    const errorKeys = Object.keys(errors);
    const result = {} as Record<T, boolean>;

    for (const [tabId, fields] of Object.entries(tabFieldsMap) as [T, string[]][]) {
      result[tabId] = fields.some((field) => errorKeys.includes(field));
    }

    return result;
  }, [tabFieldsMap, errors]);

  // Handler to call on form submit error (onError callback of handleSubmit)
  const handleFormError = React.useCallback(
    (submitErrors: FieldErrors<TFieldValues>) => {
      const errorKeys = Object.keys(submitErrors);
      if (errorKeys.length === 0) return;

      // Find the first tab containing any of the error keys
      for (const [tabId, fields] of Object.entries(tabFieldsMap) as [T, string[]][]) {
        const hasMatchingError = fields.some((f) => errorKeys.includes(f));
        if (hasMatchingError) {
          setActiveTab(tabId);
          break;
        }
      }
    },
    [tabFieldsMap, setActiveTab]
  );

  return {
    tabErrorMap,
    handleFormError,
  };
}
