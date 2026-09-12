"use client";

import React, { useState } from "react";
import { Switch } from "@/components/ui/Switch";
import { Button } from "@/components/ui/Button";
import { useToast } from "@/hooks/useToast";
import type { SecurityPreferences } from "@/types/security";
import { cn } from "@/lib/utils/cn";

export interface SecurityPreferencesCardProps {
  preferences?: SecurityPreferences;
  className?: string;
}

export function SecurityPreferencesCard({
  preferences: initialPrefs,
  className,
}: SecurityPreferencesCardProps) {
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
    toast.success("บันทึกการตั้งค่าการแจ้งเตือนความปลอดภัยเรียบร้อยแล้ว");
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="border-b border-erp-border pb-3">
        <h3 className="text-sm font-bold text-erp-text-main">
          การแจ้งเตือนความปลอดภัย (Security Notifications)
        </h3>
        <p className="text-xs text-erp-text-muted mt-0.5">
          กำหนดเงื่อนไขที่ต้องการให้ระบบส่งอีเมลแจ้งเตือนด้านความปลอดภัย
        </p>
      </div>

      <div className="space-y-4 pt-4">
        <div className="flex items-center justify-between">
          <div>
            <div className="text-xs font-semibold text-erp-text-main">
              เมื่อมีการเข้าสู่ระบบจากอุปกรณ์ใหม่
            </div>
            <p className="text-[11px] text-erp-text-muted">
              แจ้งเตือนทันทีเมื่อตรวจพบ IP หรือ Browser ที่ไม่คุ้นเคย
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
              เมื่อการเข้าสู่ระบบล้มเหลวหลายครั้ง
            </div>
            <p className="text-[11px] text-erp-text-muted">
              แจ้งเตือนเมื่อมีการพยายามเข้าสู่ระบบด้วยรหัสผ่านผิด
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
              เมื่อมีการเปลี่ยนแปลงการตั้งค่าความปลอดภัย
            </div>
            <p className="text-[11px] text-erp-text-muted">
              แจ้งเตือนเมื่อรหัสผ่านหรือ 2FA มีการเปิด/ปิด
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
            บันทึกการตั้งค่า
          </Button>
        </div>
      </div>
    </div>
  );
}

SecurityPreferencesCard.displayName = "SecurityPreferencesCard";
