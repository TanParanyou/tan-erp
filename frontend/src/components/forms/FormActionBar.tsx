"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/Button";
import { IconSave } from "@/components/common/Icons";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";

export interface FormActionBarProps {
  isDirty?: boolean;
  unsavedText?: string;
  statusContent?: React.ReactNode;
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
  extraActions?: React.ReactNode;
  actions?: React.ReactNode;
  className?: string;
  actionsClassName?: string;
  sticky?: boolean;
  children?: React.ReactNode;
}

export function FormActionBar({
  isDirty,
  unsavedText,
  statusContent,
  saveText,
  saveIcon,
  isLoading = false,
  isSaveDisabled = false,
  onSave,
  saveButtonType = "submit",
  isEditMode = false,
  showCancel,
  cancelText,
  onCancel,
  cancelHref,
  isCancelDisabled = false,
  extraActions,
  actions,
  className,
  actionsClassName,
  sticky = true,
  children,
}: FormActionBarProps) {
  const t = useTranslations("common.actions");
  const tFeedback = useTranslations("common.feedback");
  const router = useRouter();

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    } else if (cancelHref) {
      router.push(cancelHref);
    }
  };

  const shouldShowCancel = showCancel ?? (Boolean(onCancel) || Boolean(cancelHref));
  const resolvedSaveText = saveText || (isEditMode ? t("saveChanges") : t("save"));
  const hasStatus = Boolean(isDirty || statusContent);

  return (
    <div
      className={cn(
        sticky
          ? "erp-form-action-bar-fixed fixed bottom-0 left-0 right-0 z-40 w-full px-4 py-3 sm:px-6 sm:py-4 pb-[calc(0.75rem+env(safe-area-inset-bottom,0px))] sm:pb-4 border-t border-erp-border bg-erp-surface shadow-[0_-4px_16px_rgba(0,0,0,0.05)] flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:gap-4"
          : "relative mt-8 w-full p-4 border-t border-erp-border bg-erp-surface flex items-center justify-between",
        className
      )}
    >
      {/* Left Slot: Unsaved Alert or Status Content */}
      <div
        className={cn(
          "flex items-center gap-3",
          !hasStatus && "hidden sm:flex"
        )}
      >
        {isDirty && (
          <span className="erp-action-bar-dirty-alert">
            <span className="relative flex h-2 w-2">
              <span className="absolute inline-flex h-full w-full animate-ping rounded-none bg-erp-warning opacity-75" />
              <span className="relative inline-flex h-2 w-2 rounded-none bg-erp-warning" />
            </span>
            <span>{unsavedText || tFeedback("unsavedChanges")}</span>
          </span>
        )}
        {statusContent}
      </div>

      {children}

      {/* Right Slot: Action Buttons */}
      {actions ? (
        <div className={cn("flex w-full items-center gap-2.5 sm:w-auto justify-end", actionsClassName)}>
          {actions}
        </div>
      ) : (
        <div className={cn("flex w-full items-center gap-2.5 sm:w-auto justify-end", actionsClassName)}>
          {extraActions}

          {shouldShowCancel && (
            <Button
              type="button"
              variant="outline"
              onClick={handleCancel}
              disabled={isCancelDisabled || isLoading}
              className="flex-1 sm:flex-none"
            >
              {cancelText || t("cancel")}
            </Button>
          )}

          <Button
            type={saveButtonType}
            variant="primary"
            isLoading={isLoading}
            disabled={isSaveDisabled || isLoading}
            onClick={onSave}
            icon={saveIcon !== undefined ? saveIcon : <IconSave size={16} />}
            className="flex-1 sm:flex-none"
          >
            {resolvedSaveText}
          </Button>
        </div>
      )}
    </div>
  );
}

export default FormActionBar;
