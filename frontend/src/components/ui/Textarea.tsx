"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface TextareaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
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
      rows = 3,
      ...props
    },
    ref
  ) => {
    const generatedId =
      id || (label ? `textarea-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const errorId = generatedId ? `${generatedId}-error` : undefined;
    const helperId = generatedId ? `${generatedId}-helper` : undefined;

    return (
      <div className="erp-form-group">
        {label && (
          <label htmlFor={generatedId} className="erp-label">
            {label}
            {required && <span className="erp-label-required">*</span>}
          </label>
        )}
        <div className="erp-input-wrapper">
          <textarea
            id={generatedId}
            ref={ref}
            rows={rows}
            required={required}
            disabled={disabled}
            readOnly={readOnly}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : helperText ? helperId : undefined}
            className={cn(
              "erp-textarea",
              error && "erp-textarea-error",
              readOnly && "erp-textarea-readonly",
              className
            )}
            {...props}
          />
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

Textarea.displayName = "Textarea";
