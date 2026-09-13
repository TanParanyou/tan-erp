"use client";

import React, { useState } from "react";
import { cn } from "@/lib/utils/cn";
import { IconEye, IconEyeOff } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
  isReadOnly?: boolean;
  wrapperClassName?: string;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  (
    {
      className,
      wrapperClassName,
      type = "text",
      label,
      error,
      helperText,
      required,
      id,
      disabled,
      readOnly,
      isReadOnly,
      leftIcon,
      rightIcon,
      "aria-describedby": ariaDescribedByProp,
      ...props
    },
    ref
  ) => {
    const t = useTranslations("common.form");
    const [showPassword, setShowPassword] = useState(false);
    const isPasswordType = type === "password";
    const actualType = isPasswordType ? (showPassword ? "text" : "password") : type;
    const isActuallyReadOnly = readOnly || isReadOnly;

    const generatedId = id || (label ? `input-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const errorId = generatedId ? `${generatedId}-error` : undefined;
    const helperId = generatedId ? `${generatedId}-helper` : undefined;

    // Deduplicate IDs
    const tokens = new Set<string>();
    if (error && errorId) tokens.add(errorId);
    if (helperText && helperId) tokens.add(helperId);
    if (ariaDescribedByProp) {
      ariaDescribedByProp.split(/\s+/).filter(Boolean).forEach((token) => tokens.add(token));
    }
    const describedByIds = Array.from(tokens).join(" ");

    return (
      <div className={cn("erp-form-group", wrapperClassName)}>
        {label && (
          <label htmlFor={generatedId} className="erp-label">
            {label}
            {required && <span className="erp-label-required">*</span>}
          </label>
        )}

        <div className="erp-input-wrapper">
          {leftIcon && (
            <div
              style={{
                position: "absolute",
                left: "0.75rem",
                display: "flex",
                alignItems: "center",
                pointerEvents: "none",
                color: "var(--erp-text-muted)",
              }}
            >
              {leftIcon}
            </div>
          )}

          <input
            ref={ref}
            id={generatedId}
            type={actualType}
            disabled={disabled}
            readOnly={isActuallyReadOnly}
            required={required}
            aria-invalid={Boolean(error)}
            aria-describedby={describedByIds || undefined}
            className={cn(
              "erp-input",
              error && "erp-input-error",
              isActuallyReadOnly && "erp-input-readonly",
              className
            )}
            style={{
              paddingLeft: leftIcon ? "2.25rem" : undefined,
              paddingRight: rightIcon || isPasswordType ? "2.5rem" : undefined,
            }}
            {...props}
          />

          {isPasswordType ? (
            <button
              type="button"
              onClick={() => setShowPassword((prev) => !prev)}
              aria-label={showPassword ? t("hidePassword") : t("showPassword")}
              style={{
                position: "absolute",
                right: 0,
                minWidth: "44px",
                minHeight: "44px",
                display: "inline-flex",
                alignItems: "center",
                justifyContent: "center",
                background: "transparent",
                border: 0,
                cursor: "pointer",
                color: "var(--erp-text-muted)",
              }}
            >
              {showPassword ? <IconEyeOff size={16} /> : <IconEye size={16} />}
            </button>
          ) : rightIcon ? (
            <div
              style={{
                position: "absolute",
                right: "0.75rem",
                display: "flex",
                alignItems: "center",
                color: "var(--erp-text-muted)",
              }}
            >
              {rightIcon}
            </div>
          ) : null}
        </div>

        {error ? (
          <p
            id={errorId}
            className="erp-error-text"
            role="alert"
          >
            {error}
          </p>
        ) : helperText ? (
          <p
            id={helperId}
            className="erp-helper-text"
          >
            {helperText}
          </p>
        ) : null}
      </div>
    );
  }
);

Input.displayName = "Input";
