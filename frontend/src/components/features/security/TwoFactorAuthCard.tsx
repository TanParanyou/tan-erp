"use client";

import React, { useState } from "react";
import { QRCodeDisplay } from "@/components/ui/QRCodeDisplay";
import { OtpInput } from "@/components/ui/OtpInput";
import { Button } from "@/components/ui/Button";
import { useToast } from "@/hooks/useToast";
import { CopyButton } from "@/components/common/CopyButton";
import { IconLock, IconCheckCircle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface TwoFactorAuthCardProps {
  isEnabled?: boolean;
  onToggle?: (enabled: boolean) => Promise<void> | void;
  className?: string;
}

export function TwoFactorAuthCard({
  isEnabled = false,
  className,
}: TwoFactorAuthCardProps) {
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
    toast.success("เปิดใช้งานการยืนยันตัวตนสองขั้นตอน (2FA) สำเร็จ");
  };

  const handleDisable = () => {
    setEnabled(false);
    toast.success("ปิดการใช้งาน 2FA เรียบร้อยแล้ว");
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="flex items-center justify-between border-b border-erp-border pb-3">
        <div className="flex items-center gap-2.5">
          <IconLock size={18} className="text-erp-navy" />
          <div>
            <h3 className="text-sm font-bold text-erp-text-main">
              การยืนยันตัวตนสองขั้นตอน (Two-Factor Authentication)
            </h3>
            <p className="text-xs text-erp-text-muted mt-0.5">
              เพิ่มความปลอดภัยให้กับบัญชีผู้ใช้ด้วยแอปพลิเคชัน Authenticator
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {enabled ? (
            <span className="inline-flex items-center gap-1 bg-emerald-50 px-2 py-1 text-xs font-semibold text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-400 rounded-none border border-emerald-300">
              <IconCheckCircle size={12} />
              เปิดใช้งานแล้ว
            </span>
          ) : (
            <span className="bg-amber-50 px-2 py-1 text-xs font-semibold text-amber-800 dark:bg-amber-950/40 dark:text-amber-300 rounded-none border border-amber-300">
              ยังไม่เปิดใช้งาน
            </span>
          )}
        </div>
      </div>

      <div className="pt-4">
        {!enabled && !isSettingUp && (
          <div>
            <p className="text-xs text-erp-text-muted mb-4">
              เมื่อเปิดใช้งาน คุณจะต้องป้อนรหัส 6 หลักจากแอป Authenticator (Google Authenticator, Microsoft Authenticator หรือ 1Password) ทุกครั้งที่เข้าสู่ระบบ
            </p>
            <Button variant="primary" size="sm" onClick={() => setIsSettingUp(true)}>
              เริ่มตั้งค่า 2FA
            </Button>
          </div>
        )}

        {!enabled && isSettingUp && (
          <div className="space-y-4">
            <div className="flex flex-col sm:flex-row items-center gap-5 border border-erp-border bg-erp-surface-subtle p-4 rounded-none">
              <QRCodeDisplay value={dummyOtpAuth} size={140} />
              <div className="space-y-2 text-xs text-erp-text-main">
                <p className="font-semibold">1. สแกน QR Code ด้วยแอป Authenticator ของคุณ</p>
                <p className="text-erp-text-muted">หรือป้อน Secret Key ด้วยตนเอง:</p>
                <div className="flex items-center gap-2">
                  <code className="bg-erp-surface border border-erp-border px-2 py-1 font-mono text-xs font-bold">
                    {dummySecret}
                  </code>
                  <CopyButton text={dummySecret} label="คัดลอก" />
                </div>
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase tracking-wider text-erp-text-main block">
                2. กรอกรหัส 6 หลักที่ปรากฏในแอป:
              </label>
              <OtpInput value={otpCode} onChange={setOtpCode} />
            </div>

            <div className="flex gap-2 pt-2 border-t border-erp-border">
              <Button variant="secondary" size="sm" onClick={() => setIsSettingUp(false)}>
                ยกเลิก
              </Button>
              <Button variant="primary" size="sm" onClick={handleVerify} disabled={otpCode.length < 6}>
                ยืนยันและเปิดใช้งาน
              </Button>
            </div>
          </div>
        )}

        {enabled && (
          <div className="flex items-center justify-between">
            <p className="text-xs text-erp-text-muted">
              บัญชีของคุณได้รับการปกป้องด้วย 2FA เรียบร้อยแล้ว
            </p>
            <Button variant="danger" size="sm" onClick={handleDisable}>
              ปิดใช้งาน 2FA
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}

TwoFactorAuthCard.displayName = "TwoFactorAuthCard";
