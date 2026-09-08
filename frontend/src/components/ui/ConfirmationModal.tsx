"use client";

import React from "react";
import { Modal } from "./Modal";
import { Button } from "./Button";
import {
  IconAlertTriangle,
  IconCheckCircle,
  IconInfo,
  IconAlertCircle,
} from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export type ConfirmationVariant = "danger" | "warning" | "info" | "success";

export interface ConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void | Promise<void>;
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: ConfirmationVariant;
  isLoading?: boolean;
}

const variantConfig: Record<
  ConfirmationVariant,
  { icon: React.ElementType; iconColor: string; buttonVariant: "danger" | "primary" }
> = {
  danger: {
    icon: IconAlertTriangle,
    iconColor: "text-erp-danger",
    buttonVariant: "danger",
  },
  warning: {
    icon: IconAlertCircle,
    iconColor: "text-erp-warning",
    buttonVariant: "primary",
  },
  info: {
    icon: IconInfo,
    iconColor: "text-erp-info",
    buttonVariant: "primary",
  },
  success: {
    icon: IconCheckCircle,
    iconColor: "text-erp-success",
    buttonVariant: "primary",
  },
};

export function ConfirmationModal({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText,
  cancelText,
  variant = "danger",
  isLoading = false,
}: ConfirmationModalProps) {
  const tActions = useTranslations("common.actions");
  const tDialog = useTranslations("common.dialog");

  const resolvedTitle =
    title ||
    (variant === "danger"
      ? tDialog("confirmDeleteTitle")
      : tDialog("confirmTitle"));
  const resolvedConfirmText =
    confirmText || (variant === "danger" ? tActions("delete") : tActions("confirm"));
  const resolvedCancelText = cancelText || tActions("cancel");

  const config = variantConfig[variant];
  const Icon = config.icon;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      size="sm"
      showCloseButton={false}
      closeOnOverlayClick={!isLoading}
      closeOnEscape={!isLoading}
    >
      <div className="text-center py-2">
        <div
          className={`mx-auto mb-4 flex h-12 w-12 items-center justify-center border border-erp-border bg-erp-surface-muted rounded-none ${config.iconColor}`}
        >
          <Icon size={24} />
        </div>
        <h3 className="mb-2 text-base font-semibold text-erp-text-main">
          {resolvedTitle}
        </h3>
        <p className="mb-6 text-sm text-erp-text-muted leading-relaxed">
          {message}
        </p>
        <div className="flex gap-3 justify-center">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isLoading}
            className="flex-1"
          >
            {resolvedCancelText}
          </Button>
          <Button
            type="button"
            variant={config.buttonVariant}
            onClick={onConfirm}
            isLoading={isLoading}
            disabled={isLoading}
            className="flex-1"
          >
            {resolvedConfirmText}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

export default ConfirmationModal;
