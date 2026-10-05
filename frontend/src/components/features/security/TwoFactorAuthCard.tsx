"use client";

import React, { useState } from "react";
import { QRCodeDisplay } from "@/components/ui/QRCodeDisplay";
import { OtpInput } from "@/components/ui/OtpInput";
import { Button } from "@/components/ui/Button";
import { useToast } from "@/hooks/useToast";
import { CopyButton } from "@/components/common/CopyButton";
import { IconLock, IconCheckCircle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface TwoFactorAuthCardProps {
  isEnabled?: boolean;
  onToggle?: (enabled: boolean) => Promise<void> | void;
  className?: string;
}

export function TwoFactorAuthCard({
  isEnabled = false,
  className,
}: TwoFactorAuthCardProps) {
  const t = useTranslations("security");
  const [enabled, setEnabled] = useState(isEnabled);
  const [isSettingUp, setIsSettingUp] = useState(false);
  const [otpCode, setOtpCode] = useState("");
  const { toast } = useToast();

  const dummySecret = "JBSWY3DPEHPK3PXP";
  const dummyOtpAuth = `otpauth://totp/tan-erp:user@domain.com?secret=${dummySecret}&issuer=tan-erp`;

  const handleVerify = () => {
    if (otpCode.length < 6) return;
    setEnabled(true);
    setIsSettingUp(false);
    setOtpCode("");
    toast.success(t("toast2FaEnabled"));
  };

  const handleDisable = () => {
    setEnabled(false);
    toast.success(t("toast2FaDisabled"));
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="flex items-center justify-between border-b border-erp-border pb-3">
        <div className="flex items-center gap-2.5">
          <IconLock size={18} className="text-erp-navy" />
          <div>
            <h3 className="text-sm font-bold text-erp-text-main">
              {t("twoFactorTitle")}
            </h3>
            <p className="text-xs text-erp-text-muted mt-0.5">
              {t("twoFactorDescription")}
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {enabled ? (
            <span className="inline-flex items-center gap-1 bg-emerald-50 px-2 py-1 text-xs font-semibold text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-400 rounded-none border border-emerald-300">
              <IconCheckCircle size={12} />
              {t("statusEnabled")}
            </span>
          ) : (
            <span className="bg-amber-50 px-2 py-1 text-xs font-semibold text-amber-800 dark:bg-amber-950/40 dark:text-amber-300 rounded-none border border-amber-300">
              {t("statusDisabled")}
            </span>
          )}
        </div>
      </div>

      <div className="pt-4">
        {!enabled && !isSettingUp && (
          <div>
            <p className="text-xs text-erp-text-muted mb-4">
              {t("twoFactorInfoDisabled")}
            </p>
            <Button variant="primary" size="sm" onClick={() => setIsSettingUp(true)}>
              {t("setupButton")}
            </Button>
          </div>
        )}

        {!enabled && isSettingUp && (
          <div className="space-y-4">
            <div className="flex flex-col sm:flex-row items-center gap-5 border border-erp-border bg-erp-surface-subtle p-4 rounded-none">
              <QRCodeDisplay value={dummyOtpAuth} size={140} />
              <div className="space-y-2 text-xs text-erp-text-main">
                <p className="font-semibold">{t("stepScanQr")}</p>
                <p className="text-erp-text-muted">{t("orEnterSecretKey")}</p>
                <div className="flex items-center gap-2">
                  <code className="bg-erp-surface border border-erp-border px-2 py-1 font-mono text-xs font-bold">
                    {dummySecret}
                  </code>
                  <CopyButton text={dummySecret} label={t("copy")} />
                </div>
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-erp-text-main block">
                {t("stepEnterOtp")}
              </label>
              <OtpInput value={otpCode} onChange={setOtpCode} />
            </div>

            <div className="flex gap-2 pt-2 border-t border-erp-border">
              <Button variant="secondary" size="sm" onClick={() => setIsSettingUp(false)}>
                {t("cancel")}
              </Button>
              <Button variant="primary" size="sm" onClick={handleVerify} disabled={otpCode.length < 6}>
                {t("verifyAndEnable")}
              </Button>
            </div>
          </div>
        )}

        {enabled && (
          <div className="flex items-center justify-between">
            <p className="text-xs text-erp-text-muted">
              {t("twoFactorProtected")}
            </p>
            <Button variant="danger" size="sm" onClick={handleDisable}>
              {t("disableButton")}
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}

TwoFactorAuthCard.displayName = "TwoFactorAuthCard";
