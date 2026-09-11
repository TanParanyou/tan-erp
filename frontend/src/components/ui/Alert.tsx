"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import {
  IconAlertCircle,
  IconAlertTriangle,
  IconCheckCircle,
  IconInfo,
  IconClose,
} from "@/components/common/Icons";

export type AlertVariant = "danger" | "warning" | "success" | "info";

export interface AlertProps
  extends Omit<React.HTMLAttributes<HTMLDivElement>, "title"> {
  /** Alert styling variant. Default: 'danger' */
  variant?: AlertVariant;
  /** Title or strong heading inside the alert */
  title?: React.ReactNode;
  /** Custom icon override or null to hide icon */
  icon?: React.ReactNode;
  /** Callback when close button is clicked. If provided, renders a dismiss button */
  onClose?: () => void;
  /** ARIA live mode. Default: 'polite' */
  "aria-live"?: "polite" | "assertive" | "off";
  /** Alert message or content */
  children: React.ReactNode;
}

const variantConfig: Record<
  AlertVariant,
  {
    cardClass: string;
    textClass: string;
    titleClass: string;
    iconClass: string;
    defaultIcon: React.ComponentType<{ size?: number; className?: string }>;
  }
> = {
  danger: {
    cardClass: "border-erp-danger-border bg-erp-danger-bg",
    textClass: "text-erp-danger",
    titleClass: "text-erp-danger",
    iconClass: "text-erp-danger",
    defaultIcon: IconAlertCircle,
  },
  warning: {
    cardClass: "border-erp-warning-border bg-erp-warning-bg",
    textClass: "text-erp-warning-text",
    titleClass: "text-erp-warning",
    iconClass: "text-erp-warning",
    defaultIcon: IconAlertTriangle,
  },
  success: {
    cardClass: "border-erp-success-border bg-erp-success-bg",
    textClass: "text-erp-success-text",
    titleClass: "text-erp-success",
    iconClass: "text-erp-success",
    defaultIcon: IconCheckCircle,
  },
  info: {
    cardClass: "border-erp-border bg-erp-surface-muted",
    textClass: "text-erp-text-main",
    titleClass: "text-erp-navy",
    iconClass: "text-erp-navy",
    defaultIcon: IconInfo,
  },
};

/**
 * Atelier Architectural Navy Sharp - Unified Alert Component
 * Accessible inline or banner alert with sharp corners, clear semantic colors,
 * and pure SVG icons.
 */
export function Alert({
  variant = "danger",
  title,
  icon,
  onClose,
  className,
  children,
  role = "alert",
  "aria-live": ariaLive = "polite",
  ...props
}: AlertProps) {
  const config = variantConfig[variant];
  const IconComponent = config.defaultIcon;

  return (
    <div
      role={role}
      aria-live={ariaLive}
      className={cn(
        "erp-card p-4 md:px-5 flex items-center gap-3",
        config.cardClass,
        className
      )}
      {...props}
    >
      {icon !== null && (
        <div className="shrink-0 flex items-center">
          {icon ?? <IconComponent size={20} className={config.iconClass} />}
        </div>
      )}

      <div className={cn("flex-1 text-sm font-medium leading-relaxed", config.textClass)}>
        {title && (
          <div className={cn("font-bold mb-0.5", config.titleClass)}>
            {title}
          </div>
        )}
        {children}
      </div>

      {onClose && (
        <button
          type="button"
          onClick={onClose}
          aria-label="Dismiss alert"
          className={cn(
            "shrink-0 p-1 -mr-1 hover:opacity-80 transition-opacity focus-visible:outline-2 focus-visible:outline-erp-navy",
            config.textClass
          )}
        >
          <IconClose size={16} />
        </button>
      )}
    </div>
  );
}

export default Alert;
