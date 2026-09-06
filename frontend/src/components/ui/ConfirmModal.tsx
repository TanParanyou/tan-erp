"use client";

import React from "react";
import { Modal } from "@/components/ui/Modal";
import { Button, type ButtonVariant } from "@/components/ui/Button";
import { IconAlertTriangle, IconAlertCircle, IconInfo } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export type ConfirmVariant = "danger" | "warning" | "info";

export interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void | Promise<void>;
  title?: string;
  message?: string;
  description?: string;
  confirmText?: string;
  cancelText?: string;
  variant?: ConfirmVariant;
  isLoading?: boolean;
}

const variantConfig: Record<
  ConfirmVariant,
  { icon: React.ReactNode; btnVariant: ButtonVariant; color: string }
> = {
  danger: {
    icon: <IconAlertTriangle size={24} />,
    btnVariant: "danger",
    color: "var(--erp-danger)",
  },
  warning: {
    icon: <IconAlertCircle size={24} />,
    btnVariant: "primary",
    color: "var(--erp-warning)",
  },
  info: {
    icon: <IconInfo size={24} />,
    btnVariant: "primary",
    color: "var(--erp-info)",
  },
};

export function ConfirmModal({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  description,
  confirmText,
  cancelText,
  variant = "danger",
  isLoading = false,
}: ConfirmModalProps) {
  const tDialog = useTranslations("common.dialog");
  const tActions = useTranslations("common.actions");
  const currentVariant = variantConfig[variant];
  const resolvedTitle = title || tDialog("confirmTitle");
  const resolvedConfirmText = confirmText || tActions("confirm");
  const resolvedCancelText = cancelText || tActions("cancel");
  const displayMessage = description || message || tDialog("confirmGenericDesc");

  const handleConfirm = async () => {
    await onConfirm();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={isLoading ? () => {} : onClose}
      title={resolvedTitle}
      size="sm"
      closeOnEscape={!isLoading}
      closeOnOverlayClick={!isLoading}
      showCloseButton={!isLoading}
      footer={
        <>
          <Button
            variant="outline"
            size="sm"
            onClick={onClose}
            disabled={isLoading}
          >
            {resolvedCancelText}
          </Button>
          <Button
            variant={currentVariant.btnVariant}
            size="sm"
            onClick={handleConfirm}
            isLoading={isLoading}
          >
            {resolvedConfirmText}
          </Button>
        </>
      }
    >
      <div style={{ display: "flex", gap: "1rem", alignItems: "flex-start" }}>
        <div
          style={{
            color: currentVariant.color,
            flexShrink: 0,
            marginTop: "2px",
          }}
        >
          {currentVariant.icon}
        </div>
        <div style={{ fontSize: "0.875rem", color: "var(--erp-text-body)", lineHeight: 1.5 }}>
          {displayMessage}
        </div>
      </div>
    </Modal>
  );
}

// Alias export for compatibility with SKILL.md naming convention
export { ConfirmModal as ConfirmationModal };
export type { ConfirmModalProps as ConfirmationModalProps };
