"use client";

import React, { useId } from "react";
import { createPortal } from "react-dom";
import { IconClose } from "@/components/common/Icons";
import { useDialogFocus } from "@/hooks/useDialogFocus";
import { useScrollLock } from "@/hooks/useScrollLock";
import { cn } from "@/lib/utils/cn";

export type ModalSize = "sm" | "md" | "lg" | "xl" | "2xl" | "3xl" | "full";

export interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  description?: string;
  children?: React.ReactNode;
  size?: ModalSize;
  showCloseButton?: boolean;
  closeDisabled?: boolean;
  closeOnOverlayClick?: boolean;
  closeOnEscape?: boolean;
  footer?: React.ReactNode;
  className?: string;
  contentClassName?: string;
}

const sizeClasses: Record<ModalSize, string> = {
  sm: "max-w-sm",
  md: "max-w-md",
  lg: "max-w-lg",
  xl: "max-w-2xl",
  "2xl": "max-w-5xl",
  "3xl": "max-w-6xl w-[95vw]",
  full: "w-full max-w-none h-full max-h-none sm:w-[98vw] sm:max-w-[98vw] sm:h-[95vh] sm:max-h-[95vh]",
};

export function Modal({
  isOpen,
  onClose,
  title,
  description,
  children,
  size = "md",
  showCloseButton = true,
  closeDisabled = false,
  closeOnOverlayClick = true,
  closeOnEscape = true,
  footer,
  className,
  contentClassName,
}: ModalProps) {
  const titleId = useId();
  const descriptionId = useId();
  useScrollLock(isOpen);

  const modalRef = useDialogFocus(isOpen, onClose, closeDisabled, closeOnEscape);

  if (!isOpen || typeof document === "undefined") return null;

  return createPortal(
    <div className="erp-modal-overlay" onClick={closeOnOverlayClick && !closeDisabled ? onClose : undefined}>
      <div
        ref={modalRef}
        tabIndex={-1}
        role="dialog"
        aria-modal="true"
        aria-labelledby={title ? titleId : undefined}
        aria-describedby={description ? descriptionId : undefined}
        className={cn("erp-modal-window", sizeClasses[size], className)}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        {(title || showCloseButton) && (
          <div className="shrink-0 flex items-start justify-between gap-4 border-b border-erp-border px-5 py-3.5 bg-erp-surface">
            <div className="min-w-0 flex-1">
              {title && (
                <h2
                  id={titleId}
                  className="text-base font-semibold text-erp-text-main leading-snug break-words"
                >
                  {title}
                </h2>
              )}
              {description && (
                <p
                  id={descriptionId}
                  className="mt-1 text-xs sm:text-sm text-erp-text-muted leading-relaxed"
                >
                  {description}
                </p>
              )}
            </div>
            {showCloseButton && (
              <button
                type="button"
                aria-label="Close modal"
                onClick={onClose}
                disabled={closeDisabled}
                className="shrink-0 rounded-none p-1.5 text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main disabled:cursor-not-allowed disabled:opacity-50 focus-visible:outline-2 focus-visible:outline-erp-navy"
              >
                <IconClose size={20} />
              </button>
            )}
          </div>
        )}

        {/* Body */}
        {children && (
          <div className={cn("flex-1 overflow-y-auto px-5 py-4 text-erp-text-body", contentClassName)}>
            {children}
          </div>
        )}

        {/* Footer */}
        {footer && (
          <div className="shrink-0 flex flex-wrap items-center justify-end gap-3 border-t border-erp-border bg-erp-surface-muted px-5 py-3">
            {footer}
          </div>
        )}
      </div>
    </div>,
    document.body
  );
}

export default Modal;
