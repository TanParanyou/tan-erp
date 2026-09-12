"use client";

import React, { useState } from "react";
import { Button } from "@/components/ui/Button";
import { useToast } from "@/hooks/useToast";
import { IconGlobe } from "@/components/common/Icons";
import type { AdminSessionItem } from "@/types/security";
import { cn } from "@/lib/utils/cn";

export interface ActiveSessionsCardProps {
  sessions?: AdminSessionItem[];
  className?: string;
}

export function ActiveSessionsCard({
  sessions: initialSessions,
  className,
}: ActiveSessionsCardProps) {
  const [sessions, setSessions] = useState<AdminSessionItem[]>(() => {
    if (initialSessions) return initialSessions;
    return [
      {
        id: "sess_1",
        ip_address: "127.0.0.1 (Localhost)",
        user_agent: "Chrome 124 on macOS",
        last_used_at: new Date().toISOString(),
        expires_at: new Date(Date.now() + 86400000).toISOString(),
        is_current: true,
        created_at: new Date().toISOString(),
      },
    ];
  });
  const { toast } = useToast();

  const handleRevoke = (id: string) => {
    setSessions((prev) => prev.filter((s) => s.id !== id));
    toast.success("ออกจากระบบอุปกรณ์ดังกล่าวเรียบร้อยแล้ว");
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="border-b border-erp-border pb-3">
        <h3 className="text-sm font-bold text-erp-text-main">
          อุปกรณ์ที่เข้าสู่ระบบค้างไว้ (Active Sessions)
        </h3>
        <p className="text-xs text-erp-text-muted mt-0.5">
          จัดการการเชื่อมต่อจากอุปกรณ์หรือเบราว์เซอร์อื่นๆ ที่เข้าสู่ระบบบัญชีนี้
        </p>
      </div>

      <div className="divide-y divide-erp-border pt-2">
        {sessions.map((sess) => (
          <div key={sess.id} className="flex items-center justify-between py-3">
            <div className="flex items-center gap-3">
              <div className="flex h-8 w-8 items-center justify-center border border-erp-border bg-erp-surface-subtle text-erp-text-muted rounded-none">
                <IconGlobe size={16} />
              </div>
              <div>
                <div className="flex items-center gap-2">
                  <span className="text-xs font-semibold text-erp-text-main">
                    {sess.user_agent}
                  </span>
                  {sess.is_current && (
                    <span className="bg-erp-navy px-1.5 py-0.2 text-[10px] font-bold text-white rounded-none">
                      อุปกรณ์นี้
                    </span>
                  )}
                </div>
                <div className="text-[11px] text-erp-text-muted font-mono mt-0.5">
                  IP: {sess.ip_address}
                </div>
              </div>
            </div>

            {!sess.is_current && (
              <Button
                variant="secondary"
                size="sm"
                onClick={() => handleRevoke(sess.id)}
                className="text-xs text-red-600 hover:text-red-700"
              >
                ออกจากระบบ
              </Button>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

ActiveSessionsCard.displayName = "ActiveSessionsCard";
