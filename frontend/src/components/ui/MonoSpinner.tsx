import React from "react";
import { IconSpinner } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface MonoSpinnerProps {
  size?: "sm" | "md" | "lg";
  label?: string;
  className?: string;
}

const sizeMap = {
  sm: 18,
  md: 28,
  lg: 40,
};

export function MonoSpinner({ size = "md", label, className }: MonoSpinnerProps) {
  return (
    <div
      className={cn("flex flex-col items-center justify-center p-6 text-erp-navy", className)}
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        padding: "1.5rem",
        color: "var(--erp-navy)",
        gap: "0.5rem",
      }}
      role="status"
      aria-live="polite"
    >
      <IconSpinner size={sizeMap[size]} strokeWidth={2} />
      {label && (
        <span style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)" }}>
          {label}
        </span>
      )}
    </div>
  );
}
