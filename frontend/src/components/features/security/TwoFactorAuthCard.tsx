"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { QRCodeDisplay } from "@/components/ui/QRCodeDisplay";
import { OtpInput } from "@/components/ui/OtpInput";
import { Button } from "@/components/ui/Button";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { CopyButton } from "@/components/common/CopyButton";
import { IconLock, IconCheckCircle, IconRefresh } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { cn } from "@/lib/utils/cn";
import {
  useMySecurityStatus,
  useSetupTwoFactor,
  useEnableTwoFactor,
  useDisableTwoFactor,
  useRegenerateBackupCodes,
} from "@/features/profile/api/security-queries";
import type { TotpSetupPayloadDto } from "@/lib/api/api-client";

export interface TwoFactorAuthCardProps {
  isEnabled?: boolean;
  onToggle?: (enabled: boolean) => Promise<void> | void;
  className?: string;
}

export function TwoFactorAuthCard({
  isEnabled: legacyEnabled,
  onToggle,
  className,
}: TwoFactorAuthCardProps) {
  const t = useTranslations("security");
  const { toast } = useToast();

  const { data: statusData, isLoading: isStatusLoading } = useMySecurityStatus();
  const setupMutation = useSetupTwoFactor();
  const enableMutation = useEnableTwoFactor();
  const disableMutation = useDisableTwoFactor();
  const regenerateMutation = useRegenerateBackupCodes();

  const isEnabled = statusData ? Boolean(statusData.isTotpEnabled) : Boolean(legacyEnabled);
  const isRequired = Boolean(statusData?.requireTwoFactor);
  const remainingCodes = statusData?.remainingBackupCodesCount ?? null;

  // Local state
  const [isSettingUp, setIsSettingUp] = useState(false);
  const [totpSetup, setTotpSetup] = useState<TotpSetupPayloadDto | null>(null);
  const [setupOtp, setSetupOtp] = useState("");
  const [setupError, setSetupError] = useState<string | null>(null);

  // Backup codes display modal (shown after enabling or regenerating)
  const [displayedBackupCodes, setDisplayedBackupCodes] = useState<string[] | null>(null);

  // Regenerate codes modal state
  const [isRegenerating, setIsRegenerating] = useState(false);
  const [regenerateOtp, setRegenerateOtp] = useState("");
  const [regenerateError, setRegenerateError] = useState<string | null>(null);

  // Disable modal state
  const [isDisabling, setIsDisabling] = useState(false);
  const [disableOtp, setDisableOtp] = useState("");
  const [disableError, setDisableError] = useState<string | null>(null);

  // Start setup flow
  const handleStartSetup = async () => {
    setSetupError(null);
    setSetupOtp("");
    try {
      const setup = await setupMutation.mutateAsync();
      setTotpSetup(setup);
      setIsSettingUp(true);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : t("toast2FaDisabled"));
    }
  };

  // Submit setup verification
  const handleVerifySetup = async () => {
    if (setupOtp.length < 6) return;
    setSetupError(null);

    try {
      const res = await enableMutation.mutateAsync({ code: setupOtp });
      setIsSettingUp(false);
      setTotpSetup(null);
      setSetupOtp("");
      if (res.backupCodes && res.backupCodes.length > 0) {
        setDisplayedBackupCodes(res.backupCodes);
      }
      toast.success(t("toast2FaEnabled"));
      if (onToggle) void onToggle(true);
    } catch (err: unknown) {
      setSetupError(err instanceof Error ? err.message : t("disable2FaModalDesc"));
    }
  };

  // Submit regenerate backup codes
  const handleConfirmRegenerate = async () => {
    if (regenerateOtp.length < 6) return;
    setRegenerateError(null);

    try {
      const res = await regenerateMutation.mutateAsync({ code: regenerateOtp });
      setIsRegenerating(false);
      setRegenerateOtp("");
      if (res.backupCodes && res.backupCodes.length > 0) {
        setDisplayedBackupCodes(res.backupCodes);
      }
      toast.success(t("toastPreferencesSaved"));
    } catch (err: unknown) {
      setRegenerateError(err instanceof Error ? err.message : t("disable2FaModalDesc"));
    }
  };

  // Submit disable
  const handleConfirmDisable = async () => {
    if (disableOtp.length < 6) return;
    setDisableError(null);

    try {
      await disableMutation.mutateAsync({ code: disableOtp });
      setIsDisabling(false);
      setDisableOtp("");
      toast.success(t("toast2FaDisabled"));
      if (onToggle) void onToggle(false);
    } catch (err: unknown) {
      setDisableError(err instanceof Error ? err.message : t("disable2FaModalDesc"));
    }
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-erp-border pb-3">
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

        <div className="flex flex-wrap items-center gap-2">
          {isRequired && (
            <StatusBadge variant="warning" label={t("requiredByAdmin")} />
          )}

          {isEnabled ? (
            <span className="inline-flex items-center gap-1 bg-emerald-50 px-2 py-1 text-xs font-semibold text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-400 border border-emerald-300">
              <IconCheckCircle size={12} />
              {t("statusEnabled")}
            </span>
          ) : (
            <StatusBadge variant="neutral" label={t("statusDisabled")} />
          )}

          {isEnabled && remainingCodes !== null && (
            <StatusBadge
              variant={remainingCodes > 2 ? "info" : "danger"}
              label={t("remainingBackupCodes", { count: remainingCodes })}
            />
          )}
        </div>
      </div>

      {/* Body */}
      <div className="pt-4 space-y-4">
        {/* Not Enabled State */}
        {!isEnabled && !isSettingUp && (
          <div className="space-y-4">
            <p className="text-xs text-erp-text-muted leading-relaxed">
              {t("twoFactorInfoDisabled")}
            </p>
            <Button
              variant="primary"
              size="sm"
              onClick={handleStartSetup}
              isLoading={setupMutation.isPending || isStatusLoading}
              disabled={setupMutation.isPending || isStatusLoading}
            >
              {t("setupButton")}
            </Button>
          </div>
        )}

        {/* Setting Up State */}
        {!isEnabled && isSettingUp && totpSetup && (
          <div className="space-y-4">
            {setupError && <Alert variant="danger">{setupError}</Alert>}

            <div className="flex flex-col sm:flex-row items-center gap-5 border border-erp-border bg-erp-surface-subtle p-4">
              <QRCodeDisplay value={totpSetup.otpAuthUri ?? ""} size={140} />
              <div className="space-y-2 text-xs text-erp-text-main">
                <p className="font-semibold">{t("stepScanQr")}</p>
                <p className="text-erp-text-muted">{t("orEnterSecretKey")}</p>
                <div className="flex items-center gap-2">
                  <code className="bg-erp-surface border border-erp-border px-2 py-1 font-mono text-xs font-bold text-erp-text-main">
                    {totpSetup.secretKey}
                  </code>
                  <CopyButton text={totpSetup.secretKey ?? ""} label={t("copy")} />
                </div>
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-erp-text-main block">
                {t("stepEnterOtp")}
              </label>
              <OtpInput value={setupOtp} onChange={setSetupOtp} />
            </div>

            <div className="flex gap-2 pt-2 border-t border-erp-border">
              <Button
                variant="secondary"
                size="sm"
                onClick={() => {
                  setIsSettingUp(false);
                  setTotpSetup(null);
                  setSetupOtp("");
                  setSetupError(null);
                }}
                disabled={enableMutation.isPending}
              >
                {t("cancel")}
              </Button>
              <Button
                variant="primary"
                size="sm"
                onClick={handleVerifySetup}
                isLoading={enableMutation.isPending}
                disabled={enableMutation.isPending || setupOtp.length < 6}
              >
                {t("verifyAndEnable")}
              </Button>
            </div>
          </div>
        )}

        {/* Enabled State Actions */}
        {isEnabled && (
          <div className="space-y-4">
            <p className="text-xs text-erp-text-muted">
              {t("twoFactorProtected")}
            </p>

            <div className="flex flex-wrap items-center justify-between gap-3 pt-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => {
                  setIsRegenerating(true);
                  setRegenerateOtp("");
                  setRegenerateError(null);
                }}
                icon={<IconRefresh size={14} />}
              >
                {t("regenerateBackupCodes")}
              </Button>

              {isRequired ? (
                <p className="text-xs text-erp-text-muted italic">
                  {t("disable2FaRestrictedByAdmin")}
                </p>
              ) : (
                <Button
                  variant="danger"
                  size="sm"
                  onClick={() => {
                    setIsDisabling(true);
                    setDisableOtp("");
                    setDisableError(null);
                  }}
                >
                  {t("disableButton")}
                </Button>
              )}
            </div>
          </div>
        )}
      </div>

      {/* Modal: Display Backup Codes */}
      <Modal
        isOpen={displayedBackupCodes !== null}
        onClose={() => setDisplayedBackupCodes(null)}
        title={t("backupCodesGeneratedTitle")}
        description={t("backupCodesWarning")}
        size="md"
      >
        {displayedBackupCodes && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-2 p-3 bg-erp-surface-subtle border border-erp-border">
              {displayedBackupCodes.map((code, idx) => (
                <code
                  key={idx}
                  className="font-mono text-sm font-semibold text-center py-1.5 bg-erp-surface border border-erp-border text-erp-text-main"
                >
                  {code}
                </code>
              ))}
            </div>

            <div className="flex justify-between items-center pt-2">
              <CopyButton text={displayedBackupCodes.join("\n")} label={t("copy")} />
              <Button
                variant="primary"
                size="sm"
                onClick={() => setDisplayedBackupCodes(null)}
              >
                {t("backupCodesSaved")}
              </Button>
            </div>
          </div>
        )}
      </Modal>

      {/* Modal: Regenerate Backup Codes */}
      <Modal
        isOpen={isRegenerating}
        onClose={() => setIsRegenerating(false)}
        title={t("regenerateBackupCodes")}
        description={t("regenerateBackupCodesDesc")}
        size="md"
      >
        <div className="space-y-4">
          {regenerateError && <Alert variant="danger">{regenerateError}</Alert>}

          <div className="space-y-2">
            <label className="text-xs font-semibold text-erp-text-main block">
              {t("enterOtpToConfirm")}
            </label>
            <OtpInput value={regenerateOtp} onChange={setRegenerateOtp} />
          </div>

          <div className="flex justify-end gap-2 pt-3 border-t border-erp-border">
            <Button
              variant="secondary"
              size="sm"
              onClick={() => setIsRegenerating(false)}
              disabled={regenerateMutation.isPending}
            >
              {t("cancel")}
            </Button>
            <Button
              variant="primary"
              size="sm"
              onClick={handleConfirmRegenerate}
              isLoading={regenerateMutation.isPending}
              disabled={regenerateMutation.isPending || regenerateOtp.length < 6}
            >
              {t("confirmRegenerate")}
            </Button>
          </div>
        </div>
      </Modal>

      {/* Modal: Disable 2FA Confirmation */}
      <Modal
        isOpen={isDisabling}
        onClose={() => setIsDisabling(false)}
        title={t("disable2FaModalTitle")}
        description={t("disable2FaModalDesc")}
        size="md"
      >
        <div className="space-y-4">
          {disableError && <Alert variant="danger">{disableError}</Alert>}

          <div className="space-y-2">
            <label className="text-xs font-semibold text-erp-text-main block">
              {t("enterOtpToConfirm")}
            </label>
            <OtpInput value={disableOtp} onChange={setDisableOtp} />
          </div>

          <div className="flex justify-end gap-2 pt-3 border-t border-erp-border">
            <Button
              variant="secondary"
              size="sm"
              onClick={() => setIsDisabling(false)}
              disabled={disableMutation.isPending}
            >
              {t("cancel")}
            </Button>
            <Button
              variant="danger"
              size="sm"
              onClick={handleConfirmDisable}
              isLoading={disableMutation.isPending}
              disabled={disableMutation.isPending || disableOtp.length < 6}
            >
              {t("disableButton")}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}

TwoFactorAuthCard.displayName = "TwoFactorAuthCard";
