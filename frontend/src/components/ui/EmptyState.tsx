"use client";

import React from "react";
import { IconFileText, IconSearch, IconAlertCircle } from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";

export interface EmptyStateProps {
  icon?: "empty" | "search" | "error" | React.ReactNode;
  title: string;
  description?: string;
  actionLabel?: string;
  onAction?: () => void;
  secondaryActionLabel?: string;
  onSecondaryAction?: () => void;
  children?: React.ReactNode;
  className?: string;
}

export function EmptyState({
  icon = "empty",
  title,
  description,
  actionLabel,
  onAction,
  secondaryActionLabel,
  onSecondaryAction,
  children,
  className = "",
}: EmptyStateProps) {
  const renderIcon = () => {
    if (React.isValidElement(icon)) return icon;

    switch (icon) {
      case "search":
        return <IconSearch size={28} className="text-erp-text-muted" />;
      case "error":
        return <IconAlertCircle size={28} className="text-erp-danger" />;
      case "empty":
      default:
        return <IconFileText size={28} className="text-erp-text-muted" />;
    }
  };

  return (
    <div
      className={`p-10 text-center flex flex-col items-center justify-center border border-dashed border-erp-border bg-erp-surface rounded-none ${className}`}
    >
      <div className="w-14 h-14 rounded-none border border-erp-border bg-erp-surface-muted flex items-center justify-center mb-3">
        {renderIcon()}
      </div>

      <h3 className="text-sm font-bold text-erp-text-main mb-1">{title}</h3>

      {description && (
        <p className="text-xs text-erp-text-muted max-w-sm mb-4 leading-relaxed">
          {description}
        </p>
      )}

      {children}

      {(onAction || onSecondaryAction) && (
        <div className="flex flex-wrap items-center justify-center gap-2 mt-2">
          {onAction && actionLabel && (
            <Button
              type="button"
              variant="primary"
              onClick={onAction}
              className="min-h-10 text-xs px-4"
            >
              {actionLabel}
            </Button>
          )}

          {onSecondaryAction && secondaryActionLabel && (
            <Button
              type="button"
              variant="outline"
              onClick={onSecondaryAction}
              className="min-h-10 text-xs px-3.5"
            >
              {secondaryActionLabel}
            </Button>
          )}
        </div>
      )}
    </div>
  );
}

export default EmptyState;
