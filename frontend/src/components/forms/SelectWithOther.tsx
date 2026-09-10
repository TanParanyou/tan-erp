"use client";

import React from "react";
import { Select, type SelectProps } from "@/components/ui/Select";
import { Input, type InputProps } from "@/components/ui/Input";
import { cn } from "@/lib/utils/cn";

export interface SelectWithOtherProps {
  selectProps: SelectProps;
  otherProps: Omit<InputProps, "type">;
  triggerValue?: string;
  containerClassName?: string;
}

/**
 * Reusable SelectWithOther component for ERP forms.
 * Pairs a dropdown selector with a conditional text input that reveals
 * when the trigger option (default: "other") is selected.
 * Adheres to Atelier Architectural Navy Sharp design standards.
 */
export function SelectWithOther({
  selectProps,
  otherProps,
  triggerValue = "other",
  containerClassName,
}: SelectWithOtherProps): React.JSX.Element {
  const isTriggerSelected = String(selectProps.value ?? "") === triggerValue;

  return (
    <div className={cn("flex flex-col gap-3", containerClassName)}>
      <Select {...selectProps} />

      {isTriggerSelected && (
        <div
          role="region"
          aria-live="polite"
          className="border-l-2 border-erp-navy pl-3 py-1 bg-erp-surface-subtle"
        >
          <Input
            type="text"
            required={true}
            {...otherProps}
          />
        </div>
      )}
    </div>
  );
}
