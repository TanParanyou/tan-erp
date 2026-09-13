"use client";

import React, { useEffect, useCallback, useId } from "react";
import { createPortal } from "react-dom";
import { IconClose } from "@/components/common/Icons";
import { useScrollLock } from "@/hooks/useScrollLock";
import { cn } from "@/lib/utils/cn";

export type DrawerSize = "sm" | "md" | "lg" | "xl" | "full";

export interface DrawerProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  description?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: DrawerSize;
  closeOnOverlayClick?: boolean;
  closeLabel?: string;
  contentClassName?: string;
  overlayClassName?: string;
  headerActions?: React.ReactNode;
  noPadding?: boolean;
  showHeader?: boolean;
}

const sizeClasses: Record<DrawerSize, string> = {
  sm: "max-w-sm",
  md: "max-w-md",
  lg: "max-w-xl",
  xl: "max-w-3xl",
  full: "max-w-full",
};

export function Drawer({
  isOpen,
  onClose,
  title,
  description,
  children,
  footer,
  size = "md",
  closeOnOverlayClick = true,
  closeLabel = "Close drawer",
  contentClassName,
  overlayClassName,
  headerActions,
  noPadding = false,
  showHeader = true,
}: DrawerProps) {
  const titleId = useId();
  const descriptionId = useId();
  useScrollLock(isOpen);

  const handleEscape = useCallback(
    (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    },
    [onClose]
  );

  useEffect(() => {
    if (isOpen) {
      document.addEventListener("keydown", handleEscape);
    }
    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [isOpen, handleEscape]);

  if (!isOpen || typeof document === "undefined") return null;

  return createPortal(
    <div
      className={cn("erp-drawer-overlay", overlayClassName)}
      onClick={closeOnOverlayClick ? onClose : undefined}
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby={title ? titleId : undefined}
        aria-describedby={description ? descriptionId : undefined}
        className={cn(
          "erp-drawer-window w-full",
          sizeClasses[size]
        )}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        {showHeader && (
          <div className="shrink-0 flex items-start justify-between gap-4 border-b border-erp-border bg-erp-surface px-5 py-3.5">
            <div className="min-w-0 flex-1">
              {title && (
                <h2 id={titleId} className="text-base font-semibold text-erp-text-main leading-snug break-words">
                  {title}
                </h2>
              )}
              {description && (
                <p id={descriptionId} className="mt-0.5 break-words text-xs text-erp-text-muted leading-relaxed">
                  {description}
                </p>
              )}
            </div>
            <div className="shrink-0 flex items-center gap-1.5">
              {headerActions}
              <button
                type="button"
                onClick={onClose}
                className="p-1.5 text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-muted rounded-none transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
                aria-label={closeLabel}
              >
                <IconClose size={20} />
              </button>
            </div>
          </div>
        )}

        {/* Content */}
        <div
          className={cn(
            "min-h-0 flex-1 text-erp-text-body",
            noPadding ? "overflow-hidden flex flex-col" : "overflow-y-auto px-5 py-4",
            contentClassName
          )}
        >
          {children}
        </div>

        {/* Footer */}
        {footer && (
          <div className="shrink-0 border-t border-erp-border bg-erp-surface-muted px-5 py-3">
            {footer}
          </div>
        )}
      </div>
    </div>,
    document.body
  );
}

export default Drawer;
