"use client";

import React, { useState, useRef, useEffect } from "react";
import { IconInfo, IconCheckCircle, IconAlertTriangle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface NotificationItem {
  id: string;
  title: string;
  description?: string;
  timeText: string;
  type?: "info" | "success" | "warning";
  isRead?: boolean;
}

export interface NotificationCenterProps {
  notifications?: NotificationItem[];
  onMarkAllRead?: () => void;
  className?: string;
}

export function NotificationCenter({
  notifications: initialNotifications,
  onMarkAllRead,
  className,
}: NotificationCenterProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState<NotificationItem[]>(() => {
    if (initialNotifications) return initialNotifications;
    return [
      {
        id: "notif_1",
        title: "ระบบบันทึกประวัติการเปลี่ยนสถานะเรียบร้อย",
        description: "Opportunity Stage History ได้รับการอัปเดตแบบ Append-only",
        timeText: "5 นาทีที่แล้ว",
        type: "success",
        isRead: false,
      },
    ];
  });
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  const handleMarkAll = () => {
    setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
    onMarkAllRead?.();
  };

  return (
    <div ref={menuRef} className={cn("relative inline-block text-left", className)}>
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        className="relative flex h-8 w-8 items-center justify-center border border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-surface-subtle rounded-none"
        title="การแจ้งเตือน"
      >
        <IconInfo size={16} />
        {unreadCount > 0 && (
          <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center bg-erp-navy text-[9px] font-bold text-white rounded-none">
            {unreadCount}
          </span>
        )}
      </button>

      {isOpen && (
        <div className="absolute right-0 top-full z-50 mt-1 w-80 border border-erp-border bg-erp-surface shadow-lg rounded-none text-left animate-in fade-in">
          <div className="flex items-center justify-between border-b border-erp-border p-3">
            <span className="text-xs font-bold text-erp-text-main uppercase tracking-wider">
              การแจ้งเตือน ({notifications.length})
            </span>
            {unreadCount > 0 && (
              <button
                type="button"
                onClick={handleMarkAll}
                className="text-[11px] text-erp-navy hover:underline font-medium"
              >
                อ่านทั้งหมด
              </button>
            )}
          </div>

          <div className="max-h-72 overflow-y-auto divide-y divide-erp-border">
            {notifications.length === 0 ? (
              <p className="p-4 text-center text-xs text-erp-text-muted">
                ไม่มีการแจ้งเตือนใหม่
              </p>
            ) : (
              notifications.map((n) => (
                <div
                  key={n.id}
                  className={cn(
                    "p-3 transition-colors",
                    !n.isRead ? "bg-erp-surface-subtle/50" : "hover:bg-erp-surface-subtle/30"
                  )}
                >
                  <div className="flex items-start gap-2">
                    {n.type === "success" ? (
                      <IconCheckCircle size={14} className="mt-0.5 text-emerald-600 shrink-0" />
                    ) : n.type === "warning" ? (
                      <IconAlertTriangle size={14} className="mt-0.5 text-amber-600 shrink-0" />
                    ) : (
                      <IconInfo size={14} className="mt-0.5 text-erp-navy shrink-0" />
                    )}
                    <div className="flex-1">
                      <p className="text-xs font-semibold text-erp-text-main">{n.title}</p>
                      {n.description && (
                        <p className="text-[11px] text-erp-text-muted mt-0.5 leading-relaxed">
                          {n.description}
                        </p>
                      )}
                      <span className="text-[10px] text-erp-text-muted font-mono mt-1 block">
                        {n.timeText}
                      </span>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}

NotificationCenter.displayName = "NotificationCenter";
