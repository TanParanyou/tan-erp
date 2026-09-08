"use client";

import React from "react";
import {
  IconCheckCircle,
  IconAlertCircle,
  IconAlertTriangle,
  IconInfo,
  IconClose,
} from "@/components/common/Icons";
import { useToast, type ToastType } from "@/hooks/useToast";
import { cn } from "@/lib/utils/cn";

const iconMap: Record<ToastType, React.ElementType> = {
  success: IconCheckCircle,
  error: IconAlertCircle,
  warning: IconAlertTriangle,
  info: IconInfo,
};

const toneMap: Record<ToastType, string> = {
  success:
    "border-erp-border border-l-erp-success border-l-[4px] bg-erp-surface text-erp-text-main",
  error:
    "border-erp-border border-l-erp-danger border-l-[4px] bg-erp-surface text-erp-text-main",
  warning:
    "border-erp-border border-l-erp-warning border-l-[4px] bg-erp-surface text-erp-text-main",
  info:
    "border-erp-border border-l-erp-info border-l-[4px] bg-erp-surface text-erp-text-main",
};

const iconToneMap: Record<ToastType, string> = {
  success: "text-erp-success",
  error: "text-erp-danger",
  warning: "text-erp-warning",
  info: "text-erp-info",
};

export function ToastContainer() {
  const { toasts, removeToast } = useToast();

  if (toasts.length === 0) return null;

  return (
    <div
      role="region"
      aria-label="Notifications"
      className="pointer-events-none fixed right-4 top-4 z-[9999] flex w-[calc(100vw-2rem)] max-w-sm flex-col gap-2 sm:right-6 sm:top-6 sm:w-full"
    >
      {toasts.map((toast) => {
        const Icon = iconMap[toast.type];

        return (
          <div
            key={toast.id}
            role="status"
            aria-live={toast.type === "error" ? "assertive" : "polite"}
            className={cn(
              "pointer-events-auto flex items-start gap-3 rounded-none border px-4 py-3 shadow-xl backdrop-blur-md transition-all duration-200",
              toneMap[toast.type]
            )}
          >
            <Icon
              size={18}
              className={cn("mt-0.5 shrink-0", iconToneMap[toast.type])}
            />
            <div className="min-w-0 flex-1">
              <p className="text-sm font-semibold leading-5 text-erp-text-main">
                {toast.message}
              </p>
              {toast.description && (
                <p className="mt-1 text-xs leading-5 text-erp-text-muted">
                  {toast.description}
                </p>
              )}
            </div>
            <button
              type="button"
              onClick={() => removeToast(toast.id)}
              aria-label="Dismiss notification"
              className="rounded-none p-1 text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main focus-visible:outline-2 focus-visible:outline-erp-navy"
            >
              <IconClose size={16} />
            </button>
          </div>
        );
      })}
    </div>
  );
}

export default ToastContainer;
