"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface SelectOption {
  value: string | number;
  label: string;
}

export interface SelectProps
  extends Omit<React.SelectHTMLAttributes<HTMLSelectElement>, "children"> {
  label?: string;
  error?: string;
  helperText?: string;
  options: SelectOption[];
  placeholder?: string;
  required?: boolean;
  wrapperClassName?: string;
  labelClassName?: string;
}

export const Select = React.forwardRef<HTMLSelectElement, SelectProps>(
  (
    {
      className,
      wrapperClassName,
      labelClassName,
      label,
      error,
      helperText,
      options,
      placeholder,
      id,
      required,
      disabled,
      ...props
    },
    ref
  ) => {
    const generatedId =
      id || (label ? `select-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const errorId = generatedId ? `${generatedId}-error` : undefined;
    const helperId = generatedId ? `${generatedId}-helper` : undefined;

    return (
      <div className={cn("erp-form-group", wrapperClassName)}>
        {label && (
          <label htmlFor={generatedId} className={cn("erp-label", labelClassName)}>
            {label}
            {required && <span className="erp-label-required">*</span>}
          </label>
        )}
        <div className="erp-input-wrapper">
          <select
            id={generatedId}
            ref={ref}
            required={required}
            disabled={disabled}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : helperText ? helperId : undefined}
            className={cn(
              "erp-select",
              error && "erp-select-error",
              className
            )}
            {...props}
          >
            {placeholder && <option value="">{placeholder}</option>}
            {options.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </div>
        {error ? (
          <p id={errorId} className="erp-error-text" role="alert">
            {error}
          </p>
        ) : helperText ? (
          <p id={helperId} className="erp-helper-text">
            {helperText}
          </p>
        ) : null}
      </div>
    );
  }
);

Select.displayName = "Select";
