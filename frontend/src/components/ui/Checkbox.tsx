"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface CheckboxProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type"> {
  label?: React.ReactNode;
  error?: string;
  description?: string;
  labelClassName?: string;
  wrapperClassName?: string;
  onCheckedChange?: (checked: boolean) => void;
}

export const Checkbox = React.forwardRef<HTMLInputElement, CheckboxProps>(
  (
    {
      className,
      label,
      error,
      description,
      disabled,
      labelClassName,
      wrapperClassName,
      id,
      onChange,
      onCheckedChange,
      ...props
    },
    ref
  ) => {
    const generatedId =
      id || (typeof label === "string" ? `checkbox-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const errorId = generatedId ? `${generatedId}-error` : undefined;

    return (
      <div className={cn("erp-form-group mb-2", wrapperClassName)}>
        <label
          htmlFor={generatedId}
          className={cn(
            "erp-checkbox-label",
            disabled && "erp-checkbox-label-disabled",
            labelClassName
          )}
        >
          <input
            id={generatedId}
            type="checkbox"
            disabled={disabled}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : undefined}
            className={cn("erp-checkbox", className)}
            ref={ref}
            onChange={(event) => {
              onChange?.(event);
              onCheckedChange?.(event.target.checked);
            }}
            {...props}
          />
          {(label || description) && (
            <div className="flex flex-col gap-0.5">
              {label && <span className="font-medium">{label}</span>}
              {description && (
                <span className="text-xs text-erp-text-muted">{description}</span>
              )}
            </div>
          )}
        </label>
        {error && (
          <p id={errorId} className="erp-error-text" role="alert">
            {error}
          </p>
        )}
      </div>
    );
  }
);

Checkbox.displayName = "Checkbox";
