"use client";

import React from "react";
import { useTranslations } from "next-intl";
import {
  IconAlertTriangle,
  IconClock,
  IconLogOut,
  IconCheck,
} from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";

export interface InactivityTimeoutDialogProps {
  isOpen: boolean;
  secondsRemaining: number;
  onStayLoggedIn: () => void;
  onLogout: () => void;
}

export function InactivityTimeoutDialog({
  isOpen,
  secondsRemaining,
  onStayLoggedIn,
  onLogout,
}: InactivityTimeoutDialogProps) {
  const t = useTranslations("common.security");

  if (!isOpen) {
    return null;
  }

  return (
    <div
      role="dialog"
      aria-modal="true"
      aria-labelledby="inactivity-dialog-title"
      className="erp-modal-overlay"
    >
      <div className="w-full max-w-md bg-erp-surface border border-erp-border p-6 shadow-2xl space-y-4 rounded-none">
        <div className="flex items-start gap-3">
          <div className="h-10 w-10 shrink-0 bg-erp-warning-bg border border-erp-warning-border text-erp-warning flex items-center justify-center rounded-none">
            <IconAlertTriangle size={20} />
          </div>
          <div className="space-y-1">
            <h3
              id="inactivity-dialog-title"
              className="text-base font-semibold text-erp-text-main"
            >
              {t("sessionTimeoutWarningTitle")}
            </h3>
            <p className="text-xs text-erp-text-muted leading-relaxed">
              {t("sessionTimeoutWarningDesc")}
            </p>
          </div>
        </div>

        <div className="bg-erp-surface-muted border border-erp-border p-3 flex items-center justify-center gap-2 text-erp-text-main">
          <IconClock size={16} className="text-erp-warning" />
          <span className="text-sm font-mono font-medium">
            {t("autoLogoutIn", { seconds: secondsRemaining })}
          </span>
        </div>

        <div className="flex items-center justify-end gap-3 pt-2">
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={onLogout}
            icon={<IconLogOut size={14} />}
          >
            {t("logoutNow")}
          </Button>
          <Button
            type="button"
            variant="primary"
            size="sm"
            onClick={onStayLoggedIn}
            icon={<IconCheck size={14} />}
          >
            {t("stayLoggedIn")}
          </Button>
        </div>
      </div>
    </div>
  );
}

export default InactivityTimeoutDialog;
