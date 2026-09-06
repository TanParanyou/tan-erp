"use client";

import React, { useEffect, useRef } from "react";
import { cn } from "@/lib/utils/cn";

export interface CheckboxProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type"> {
  label?: React.ReactNode;
  indeterminate?: boolean;
  error?: string;
}

export const Checkbox = React.forwardRef<HTMLInputElement, CheckboxProps>(
  ({ className, label, indeterminate, error, id, disabled, ...props }, forwardedRef) => {
    const internalRef = useRef<HTMLInputElement | null>(null);

    const generatedId = id || (typeof label === "string" ? `chk-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);

    useEffect(() => {
      const element = internalRef.current;
      if (element) {
        element.indeterminate = !!indeterminate;
      }
    }, [indeterminate]);

    const setRefs = (element: HTMLInputElement | null) => {
      internalRef.current = element;
      if (typeof forwardedRef === "function") {
        forwardedRef(element);
      } else if (forwardedRef) {
        forwardedRef.current = element;
      }
    };

    return (
      <div style={{ display: "inline-flex", flexDirection: "column" }}>
        <label
          htmlFor={generatedId}
          className={cn("erp-checkbox-label", disabled && "erp-checkbox-label-disabled")}
        >
          <input
            ref={setRefs}
            id={generatedId}
            type="checkbox"
            disabled={disabled}
            className={cn("erp-checkbox", className)}
            {...props}
          />
          {label && <span>{label}</span>}
        </label>
        {error && <p className="erp-error-text">{error}</p>}
      </div>
    );
  }
);

Checkbox.displayName = "Checkbox";
