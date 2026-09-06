"use client";

import React, { useState, useEffect } from "react";
import { cn } from "@/lib/utils/cn";

export interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
  isReadOnly?: boolean;
  showCount?: boolean;
}

export const Textarea = React.forwardRef<HTMLTextAreaElement, TextareaProps>(
  (
    {
      className,
      label,
      error,
      helperText,
      required,
      id,
      disabled,
      readOnly,
      isReadOnly,
      maxLength,
      showCount = false,
      value,
      defaultValue,
      onChange,
      rows = 3,
      ...props
    },
    ref
  ) => {
    const isActuallyReadOnly = readOnly || isReadOnly;
    const generatedId = id || (label ? `textarea-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);

    const initialLength = typeof value === "string" ? value.length : typeof defaultValue === "string" ? defaultValue.length : 0;
    const [charCount, setCharCount] = useState<number>(initialLength);

    useEffect(() => {
      if (typeof value === "string") {
        setCharCount(value.length);
      }
    }, [value]);

    const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
      setCharCount(e.target.value.length);
      if (onChange) {
        onChange(e);
      }
    };

    return (
      <div className="erp-form-group">
        {label && (
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <label htmlFor={generatedId} className="erp-label">
              {label}
              {required && <span className="erp-label-required">*</span>}
            </label>
            {showCount && maxLength && (
              <span style={{ fontSize: "0.75rem", color: "var(--erp-text-muted)" }}>
                {charCount} / {maxLength}
              </span>
            )}
          </div>
        )}

        <textarea
          ref={ref}
          id={generatedId}
          rows={rows}
          maxLength={maxLength}
          disabled={disabled}
          readOnly={isActuallyReadOnly}
          required={required}
          value={value}
          defaultValue={defaultValue}
          onChange={handleChange}
          className={cn(
            "erp-textarea",
            error && "erp-textarea-error",
            isActuallyReadOnly && "erp-textarea-readonly",
            className
          )}
          {...props}
        />

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

Textarea.displayName = "Textarea";
