"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface SegmentedControlOption<T extends string | number> {
  value: T;
  label: React.ReactNode;
  icon?: React.ReactNode;
  disabled?: boolean;
}

export interface SegmentedControlProps<T extends string | number> {
  value: T;
  onChange: (value: T) => void;
  options: readonly SegmentedControlOption<T>[];
  size?: "sm" | "md";
  className?: string;
  disabled?: boolean;
  "aria-label"?: string;
}

export function SegmentedControl<T extends string | number>({
  value,
  onChange,
  options,
  size = "sm",
  className,
  disabled = false,
  "aria-label": ariaLabel,
}: SegmentedControlProps<T>) {
  const sizeStyles = {
    sm: {
      container: "min-h-[36px] h-9 text-[0.8125rem]",
      button: "px-3 py-1 text-[0.8125rem]",
    },
    md: {
      container: "min-h-[44px] h-11 text-[0.875rem]",
      button: "px-4 py-2 text-[0.875rem]",
    },
  }[size];

  return (
    <div
      role="group"
      aria-label={ariaLabel}
      className={cn(
        "inline-flex border border-erp-border bg-erp-bg-neutral overflow-hidden shrink-0 rounded-none",
        sizeStyles.container,
        disabled && "opacity-55 pointer-events-none",
        className
      )}
    >
      {options.map((option) => {
        const isSelected = value === option.value;
        const isOptionDisabled = disabled || option.disabled;

        return (
          <button
            key={String(option.value)}
            type="button"
            disabled={isOptionDisabled}
            onClick={() => onChange(option.value)}
            className={cn(
              "inline-flex items-center justify-center gap-2 h-full font-medium transition-colors rounded-none focus-visible:outline-2 focus-visible:outline-erp-navy focus:z-10 select-none cursor-pointer",
              sizeStyles.button,
              isSelected
                ? "bg-erp-navy text-white"
                : "text-erp-text-main hover:bg-erp-surface-subtle",
              isOptionDisabled && "opacity-55 cursor-not-allowed pointer-events-none"
            )}
          >
            {option.icon && <span className="shrink-0">{option.icon}</span>}
            <span>{option.label}</span>
          </button>
        );
      })}
    </div>
  );
}

SegmentedControl.displayName = "SegmentedControl";

export default SegmentedControl;
