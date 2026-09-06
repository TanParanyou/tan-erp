"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface SwitchProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type" | "size"> {
  label?: React.ReactNode;
  size?: "sm" | "md" | "lg";
}

const sizeConfig = {
  sm: { trackW: "32px", trackH: "18px", thumbSize: "14px", travel: "14px" },
  md: { trackW: "38px", trackH: "22px", thumbSize: "16px", travel: "16px" },
  lg: { trackW: "46px", trackH: "26px", thumbSize: "20px", travel: "20px" },
};

export const Switch = React.forwardRef<HTMLInputElement, SwitchProps>(
  ({ className, label, id, checked, disabled, size = "md", onChange, ...props }, ref) => {
    const generatedId = id || (typeof label === "string" ? `sw-${label.replace(/\s+/g, "-").toLowerCase()}` : undefined);
    const cfg = sizeConfig[size] || sizeConfig.md;

    return (
      <label
        htmlFor={generatedId}
        className={cn("erp-switch-label", disabled && "erp-switch-label-disabled", className)}
      >
        <div
          style={{
            position: "relative",
            width: cfg.trackW,
            height: cfg.trackH,
            backgroundColor: checked ? "var(--erp-navy)" : "var(--erp-border)",
            transition: "background-color 150ms ease",
            display: "inline-flex",
            alignItems: "center",
            padding: "2px",
            boxSizing: "border-box",
            borderRadius: 0,
            cursor: disabled ? "not-allowed" : "pointer",
          }}
        >
          <input
            ref={ref}
            id={generatedId}
            type="checkbox"
            checked={checked}
            disabled={disabled}
            onChange={onChange}
            style={{
              position: "absolute",
              opacity: 0,
              width: 0,
              height: 0,
              margin: 0,
            }}
            {...props}
          />
          <div
            style={{
              width: cfg.thumbSize,
              height: cfg.thumbSize,
              backgroundColor: "#FFFFFF",
              transform: checked ? `translateX(${cfg.travel})` : "translateX(0)",
              transition: "transform 150ms ease",
              borderRadius: 0,
            }}
          />
        </div>
        {label && <span style={{ fontSize: "0.875rem" }}>{label}</span>}
      </label>
    );
  }
);

Switch.displayName = "Switch";
