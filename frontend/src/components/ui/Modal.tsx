"use client";

import React, { useEffect, useCallback, useId } from "react";
import { createPortal } from "react-dom";
import { IconClose } from "@/components/common/Icons";
import { useBodyScrollLock } from "@/hooks/useBodyScrollLock";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export type ModalSize = "sm" | "md" | "lg" | "xl" | "full";

export interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: React.ReactNode;
  description?: React.ReactNode;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: ModalSize;
  showCloseButton?: boolean;
  closeOnOverlayClick?: boolean;
  closeOnEscape?: boolean;
  closeAriaLabel?: string;
  className?: string;
}

const sizeWidthMap: Record<ModalSize, string> = {
  sm: "420px",
  md: "560px",
  lg: "720px",
  xl: "960px",
  full: "96vw",
};

export function Modal({
  isOpen,
  onClose,
  title,
  description,
  children,
  footer,
  size = "md",
  showCloseButton = true,
  closeOnOverlayClick = true,
  closeOnEscape = true,
  closeAriaLabel,
  className,
}: ModalProps) {
  const t = useTranslations("common.actions");
  const titleId = useId();
  const descId = useId();

  const handleKeyDown = useCallback(
    (e: KeyboardEvent) => {
      if (e.key === "Escape" && closeOnEscape) {
        onClose();
      }
    },
    [closeOnEscape, onClose]
  );

  useBodyScrollLock(isOpen);

  useEffect(() => {
    if (isOpen) {
      document.addEventListener("keydown", handleKeyDown);
      return () => {
        document.removeEventListener("keydown", handleKeyDown);
      };
    }
  }, [isOpen, handleKeyDown]);

  if (!isOpen || typeof document === "undefined") return null;

  return createPortal(
    <div
      className="erp-modal-overlay"
      onClick={closeOnOverlayClick ? onClose : undefined}
      role="presentation"
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby={title ? titleId : undefined}
        aria-describedby={description ? descId : undefined}
        className={cn("erp-modal-window", className)}
        style={{ width: sizeWidthMap[size] }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        {(title || showCloseButton) && (
          <div
            style={{
              padding: "1rem 1.5rem",
              borderBottom: "1px solid var(--erp-border)",
              display: "flex",
              alignItems: "flex-start",
              justifyContent: "space-between",
              gap: "1rem",
              backgroundColor: "var(--erp-surface-muted)",
            }}
          >
            <div>
              {title && (
                <h3
                  id={titleId}
                  style={{
                    fontSize: "1.125rem",
                    fontWeight: 600,
                    color: "var(--erp-text-main)",
                    margin: 0,
                  }}
                >
                  {title}
                </h3>
              )}
              {description && (
                <p
                  id={descId}
                  style={{
                    fontSize: "0.8125rem",
                    color: "var(--erp-text-muted)",
                    margin: "0.25rem 0 0 0",
                  }}
                >
                  {description}
                </p>
              )}
            </div>

            {showCloseButton && (
              <button
                type="button"
                onClick={onClose}
                aria-label={closeAriaLabel || t("close")}
                style={{
                  background: "transparent",
                  border: "none",
                  cursor: "pointer",
                  color: "var(--erp-text-muted)",
                  padding: "0.25rem",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <IconClose size={20} />
              </button>
            )}
          </div>
        )}

        {/* Content */}
        <div
          style={{
            padding: "1.5rem",
            overflowY: "auto",
            flex: "1 1 auto",
          }}
        >
          {children}
        </div>

        {/* Footer */}
        {footer && (
          <div
            style={{
              padding: "0.875rem 1.5rem",
              borderTop: "1px solid var(--erp-border)",
              display: "flex",
              alignItems: "center",
              justifyContent: "flex-end",
              gap: "0.75rem",
              backgroundColor: "var(--erp-surface-muted)",
            }}
          >
            {footer}
          </div>
        )}
      </div>
    </div>,
    document.body
  );
}
