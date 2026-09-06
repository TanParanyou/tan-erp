"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface SelectOption {
  label: string;
  value: string | number;
  disabled?: boolean;
}

export interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
  options: SelectOption[];
  placeholder?: string;
  isReadOnly?: boolean;
  readOnly?: boolean;
  isClearable?: boolean;
  onClear?: () => void;
}

export const Select = React.forwardRef<HTMLSelectElement, SelectProps>(
  (
    {
      className,
      label,
      error,
      helperText,
      required,
      options = [],
      placeholder,
      id,
      disabled,
      readOnly,
      isReadOnly,
      isClearable = false,
      onClear,
      value,
      onChange,
      ...props
    },
    ref
  ) => {
    const tForm = useTranslations("common.form");
    const tActions = useTranslations("common.actions");
    const generatedId = id || (label ? `select-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const isActuallyDisabled = disabled || isReadOnly || readOnly;

    return (
      <div className="erp-form-group">
        {label && (
          <label htmlFor={generatedId} className="erp-label">
            {label}
            {required && <span className="erp-label-required">*</span>}
          </label>
        )}

        <div style={{ position: "relative", width: "100%", display: "flex", alignItems: "center" }}>
          <select
            ref={ref}
            id={generatedId}
            disabled={isActuallyDisabled}
            required={required}
            value={value}
            onChange={onChange}
            className={cn(
              "erp-select",
              error && "erp-select-error",
              (isReadOnly || readOnly) && "erp-input-readonly",
              className
            )}
            {...props}
          >
            {placeholder && (
              <option value="" disabled={!isClearable}>
                {placeholder}
              </option>
            )}
            {isClearable && !placeholder && (
              <option value="">
                {tForm("unspecified")}
              </option>
            )}
            {options.map((opt) => (
              <option key={String(opt.value)} value={opt.value} disabled={opt.disabled}>
                {opt.label}
              </option>
            ))}
          </select>

          {isClearable && value && !isActuallyDisabled && onClear && (
            <button
              type="button"
              onClick={(e) => {
                e.preventDefault();
                onClear();
              }}
              aria-label={tActions("clearSelection")}
              style={{
                position: "absolute",
                right: "2rem",
                background: "transparent",
                border: "none",
                cursor: "pointer",
                padding: "0.25rem",
                color: "var(--erp-text-muted)",
                fontSize: "0.875rem",
                lineHeight: 1,
              }}
            >
              ✕
            </button>
          )}
        </div>

        {error ? (
          <p className="erp-error-text" role="alert">
            {error}
          </p>
        ) : helperText ? (
          <p className="erp-helper-text">{helperText}</p>
        ) : null}
      </div>
    );
  }
);

Select.displayName = "Select";
