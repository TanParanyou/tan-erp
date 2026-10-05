"use client";

import React, { useState } from "react";
import { Switch } from "@/components/ui/Switch";
import { Button } from "@/components/ui/Button";
import { useToast } from "@/hooks/useToast";
import type { SecurityPreferences } from "@/types/security";
import { cn } from "@/lib/utils/cn";

import { useTranslations } from "next-intl";

export interface SecurityPreferencesCardProps {
  preferences?: SecurityPreferences;
  className?: string;
}

export function SecurityPreferencesCard({
  preferences: initialPrefs,
  className,
}: SecurityPreferencesCardProps) {
  const t = useTranslations("security");
  const [prefs, setPrefs] = useState<SecurityPreferences>(() => {
    return (
      initialPrefs || {
        email_on_new_device: true,
        email_on_failed_login: true,
        email_on_security_change: true,
      }
    );
  });
  const { toast } = useToast();

  const handleSave = () => {
    toast.success(t("toastPreferencesSaved"));
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="border-b border-erp-border pb-3">
        <h3 className="text-sm font-bold text-erp-text-main">
          {t("notificationsTitle")}
        </h3>
        <p className="text-xs text-erp-text-muted mt-0.5">
          {t("notificationsDescription")}
        </p>
      </div>

      <div className="space-y-4 pt-4">
        <div className="flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-erp-text-main">
              {t("onNewDeviceTitle")}
            </div>
            <p className="text-[11px] text-erp-text-muted">
              {t("onNewDeviceDesc")}
            </p>
          </div>
          <Switch
            checked={prefs.email_on_new_device}
            onChange={(e) =>
              setPrefs((p) => ({ ...p, email_on_new_device: e.target.checked }))
            }
          />
        </div>

        <div className="flex items-center justify-between border-t border-erp-border pt-3">
          <div>
            <div className="text-xs font-semibold text-erp-text-main">
              {t("onFailedLoginTitle")}
            </div>
            <p className="text-[11px] text-erp-text-muted">
              {t("onFailedLoginDesc")}
            </p>
          </div>
          <Switch
            checked={prefs.email_on_failed_login}
            onChange={(e) =>
              setPrefs((p) => ({ ...p, email_on_failed_login: e.target.checked }))
            }
          />
        </div>

        <div className="flex items-center justify-between border-t border-erp-border pt-3">
          <div>
            <div className="text-xs font-semibold text-erp-text-main">
              {t("onSecurityChangeTitle")}
            </div>
            <p className="text-[11px] text-erp-text-muted">
              {t("onSecurityChangeDesc")}
            </p>
          </div>
          <Switch
            checked={prefs.email_on_security_change}
            onChange={(e) =>
              setPrefs((p) => ({ ...p, email_on_security_change: e.target.checked }))
            }
          />
        </div>

        <div className="pt-3 border-t border-erp-border flex justify-end">
          <Button variant="primary" size="sm" onClick={handleSave}>
            {t("savePreferences")}
          </Button>
        </div>
      </div>
    </div>
  );
}

SecurityPreferencesCard.displayName = "SecurityPreferencesCard";
