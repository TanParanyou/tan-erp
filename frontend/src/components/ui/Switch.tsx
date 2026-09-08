"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface SwitchProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type"> {
  label?: string;
  error?: string;
  description?: string;
}

export const Switch = React.forwardRef<HTMLInputElement, SwitchProps>(
  ({ className, label, error, description, disabled, id, ...props }, ref) => {
    const generatedId =
      id || (label ? `switch-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const errorId = generatedId ? `${generatedId}-error` : undefined;

    return (
      <div className="erp-form-group mb-2">
        <label
          htmlFor={generatedId}
          className={cn(
            "erp-switch-label",
            disabled && "erp-switch-label-disabled"
          )}
        >
          {/* Architectural Sharp Rectangular Switch */}
          <div className="relative inline-flex items-center">
            <input
              id={generatedId}
              type="checkbox"
              className="sr-only peer"
              disabled={disabled}
              aria-invalid={Boolean(error)}
              aria-describedby={error ? errorId : undefined}
              ref={ref}
              {...props}
            />
            {/* Track: Sharp 0px rectangle */}
            <div
              className={cn(
                "w-10 h-5 bg-erp-surface-muted border border-erp-border transition-colors duration-150 ease-in-out",
                "peer-checked:bg-erp-navy peer-checked:border-erp-navy",
                "peer-focus-visible:outline-2 peer-focus-visible:outline-offset-1 peer-focus-visible:outline-erp-navy",
                "peer-disabled:opacity-50 peer-disabled:cursor-not-allowed",
                className
              )}
            />
            {/* Thumb: Sharp 0px rectangle */}
            <div
              className={cn(
                "absolute left-0.5 top-0.5 w-4 h-4 bg-erp-surface border border-erp-border transition-transform duration-150 ease-in-out",
                "peer-checked:translate-x-5 peer-checked:border-white peer-checked:bg-white",
                "peer-disabled:opacity-50"
              )}
            />
          </div>
          {(label || description) && (
            <div className="flex flex-col gap-0.5 ml-2">
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

Switch.displayName = "Switch";
