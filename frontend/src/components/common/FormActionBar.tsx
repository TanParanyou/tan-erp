"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/Button";
import { IconSave, IconAlertTriangle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface FormActionBarProps {
  isDirty?: boolean;
  unsavedText?: string;
  saveText?: string;
  saveIcon?: React.ReactNode;
  isLoading?: boolean;
  isSaveDisabled?: boolean;
  onSave?: () => void;
  saveButtonType?: "submit" | "button";
  isEditMode?: boolean;

  showCancel?: boolean;
  cancelText?: string;
  onCancel?: () => void;
  cancelHref?: string;
  isCancelDisabled?: boolean;

  isReadOnly?: boolean;
  extraActions?: React.ReactNode;
  className?: string;
  children?: React.ReactNode;
}

export function FormActionBar({
  isDirty = false,
  unsavedText,
  saveText,
  saveIcon = <IconSave size={18} />,
  isLoading = false,
  isSaveDisabled = false,
  onSave,
  saveButtonType = "submit",
  isEditMode = false,
  showCancel = true,
  cancelText,
  onCancel,
  cancelHref,
  isCancelDisabled = false,
  isReadOnly = false,
  extraActions,
  className,
  children,
}: FormActionBarProps) {
  const router = useRouter();
  const tFeedback = useTranslations("common.feedback");
  const tActions = useTranslations("common.actions");

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    } else if (cancelHref) {
      router.push(cancelHref);
    } else {
      router.back();
    }
  };

  const resolvedUnsavedText = unsavedText || tFeedback("unsavedChanges");
  const resolvedCancelText = cancelText || tActions("cancel");
  const resolvedBackText = tActions("back");
  const resolvedSaveText = saveText || (isEditMode ? tActions("saveChanges") : tActions("save"));

  return (
    <div className={cn("erp-action-bar", className)}>
      {/* Left side: Dirty indicator or custom children */}
      <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
        {children}
        {isDirty && !isReadOnly && (
          <div className="erp-action-bar-dirty-alert">
            <IconAlertTriangle size={16} />
            <span>{resolvedUnsavedText}</span>
          </div>
        )}
      </div>

      {/* Right side: Actions */}
      <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
        {extraActions}

        {showCancel && (
          <Button
            type="button"
            variant="outline"
            size="md"
            onClick={handleCancel}
            disabled={isCancelDisabled || isLoading}
          >
            {isReadOnly ? resolvedBackText : resolvedCancelText}
          </Button>
        )}

        {!isReadOnly && (
          <Button
            type={saveButtonType}
            variant="primary"
            size="md"
            icon={saveIcon}
            isLoading={isLoading}
            disabled={isSaveDisabled || isLoading}
            onClick={saveButtonType === "button" ? onSave : undefined}
          >
            {resolvedSaveText}
          </Button>
        )}
      </div>
    </div>
  );
}
