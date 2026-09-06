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
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  (
    {
      className,
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

    return (
      <div className="erp-form-group">
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
              tabIndex={-1}
              aria-label={showPassword ? t("hidePassword") : t("showPassword")}
              style={{
                position: "absolute",
                right: "0.75rem",
                background: "transparent",
                border: "none",
                cursor: "pointer",
                padding: "0.25rem",
                color: "var(--erp-text-muted)",
                display: "flex",
                alignItems: "center",
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

Input.displayName = "Input";
