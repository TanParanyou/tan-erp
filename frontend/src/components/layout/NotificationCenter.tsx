"use client";

import React, { useCallback, useEffect, useId, useRef, useState } from "react";
import { IconBell, IconInfo, IconCheckCircle, IconAlertTriangle } from "@/components/common/Icons";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface NotificationItem {
  id: string;
  title: string;
  description?: string;
  timeText: string;
  type?: "info" | "success" | "warning";
  isRead?: boolean;
  /** Locale-prefixed in-app path, or null when the reader has no access to the target. */
  href?: string | null;
}

export interface NotificationCenterProps {
  notifications?: NotificationItem[];
  /** Server-side unread total for the badge; counted from `notifications` only when omitted. */
  unreadCount?: number;
  onItemClick?: (item: NotificationItem) => void;
  onMarkAllRead?: () => void;
  onViewAll?: () => void;
  onOpenChange?: (open: boolean) => void;
  isLoading?: boolean;
  isError?: boolean;
  className?: string;
}

const BADGE_CAP = 99;

export function NotificationCenter({
  notifications = [],
  unreadCount,
  onItemClick,
  onMarkAllRead,
  onViewAll,
  onOpenChange,
  isLoading = false,
  isError = false,
  className,
}: NotificationCenterProps) {
  const t = useTranslations("common.notificationCenter");
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const panelId = useId();

  const setOpen = useCallback(
    (next: boolean) => {
      setIsOpen(next);
      onOpenChange?.(next);
    },
    [onOpenChange],
  );

  useEffect(() => {
    if (!isOpen) return;
    function handleClickOutside(event: MouseEvent) {
      if (event.target instanceof Node && menuRef.current && !menuRef.current.contains(event.target)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [isOpen, setOpen]);

  const handleKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
    if (event.key === "Escape" && isOpen) {
      setOpen(false);
      buttonRef.current?.focus();
    }
  };

  const unread = unreadCount ?? notifications.filter((n) => !n.isRead).length;
  const badgeText = unread > BADGE_CAP ? `${BADGE_CAP}+` : String(unread);

  return (
    <div ref={menuRef} onKeyDown={handleKeyDown} className={cn("relative inline-block text-left", className)}>
      <button
        ref={buttonRef}
        type="button"
        onClick={() => setOpen(!isOpen)}
        aria-expanded={isOpen}
        aria-controls={isOpen ? panelId : undefined}
        aria-label={t("title")}
        title={t("title")}
        className="relative flex h-11 w-11 items-center justify-center border border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-surface-subtle rounded-none"
      >
        <IconBell size={18} />
        {unread > 0 && (
          <span
            aria-hidden="true"
            className="absolute -top-1 -right-1 flex h-4 min-w-4 items-center justify-center bg-erp-navy px-0.5 text-[9px] font-bold text-erp-surface rounded-none"
          >
            {badgeText}
          </span>
        )}
      </button>
      {/* Polite live region: screen readers hear the count change when polling brings new items. */}
      <span role="status" aria-live="polite" className="sr-only">
        {unread > 0 ? t("unreadCount", { count: unread }) : ""}
      </span>

      {isOpen && (
        <div
          id={panelId}
          role="region"
          aria-label={t("title")}
          className="absolute right-0 top-full z-50 mt-1 w-80 max-w-[calc(100vw-2rem)] border border-erp-border bg-erp-surface shadow-lg rounded-none text-left"
        >
          <div className="flex items-center justify-between border-b border-erp-border p-3">
            <span className="text-xs font-bold text-erp-text-main uppercase tracking-wider">{t("title")}</span>
            {unread > 0 && onMarkAllRead && (
              <button
                type="button"
                onClick={onMarkAllRead}
                className="min-h-11 px-2 text-[11px] text-erp-navy hover:underline font-medium"
              >
                {t("markAllRead")}
              </button>
            )}
          </div>

          <div className="max-h-72 overflow-y-auto divide-y divide-erp-border">
            {isLoading ? (
              <MonoSpinner size="sm" label={t("loading")} />
            ) : isError ? (
              <p role="alert" className="p-4 text-center text-xs text-erp-text-muted">{t("loadFailed")}</p>
            ) : notifications.length === 0 ? (
              <p className="p-4 text-center text-xs text-erp-text-muted">{t("empty")}</p>
            ) : (
              notifications.map((n) => (
                <button
                  key={n.id}
                  type="button"
                  onClick={() => {
                    setOpen(false);
                    onItemClick?.(n);
                  }}
                  className={cn(
                    "block min-h-11 w-full p-3 text-left transition-colors rounded-none",
                    n.isRead ? "hover:bg-erp-surface-subtle/30" : "bg-erp-surface-subtle/50 hover:bg-erp-surface-subtle",
                  )}
                >
                  <span className="flex items-start gap-2">
                    {n.type === "success" ? (
                      <IconCheckCircle size={14} className="mt-0.5 text-erp-success shrink-0" />
                    ) : n.type === "warning" ? (
                      <IconAlertTriangle size={14} className="mt-0.5 text-erp-warning shrink-0" />
                    ) : (
                      <IconInfo size={14} className="mt-0.5 text-erp-navy shrink-0" />
                    )}
                    <span className="flex-1">
                      <span className={cn("block text-xs text-erp-text-main", n.isRead ? "font-normal" : "font-semibold")}>
                        {n.title}
                      </span>
                      {n.description && (
                        <span className="mt-0.5 block text-[11px] leading-relaxed text-erp-text-muted">{n.description}</span>
                      )}
                      <span className="mt-1 block font-mono text-[10px] text-erp-text-muted">{n.timeText}</span>
                    </span>
                  </span>
                </button>
              ))
            )}
          </div>

          {onViewAll && (
            <div className="border-t border-erp-border">
              <button
                type="button"
                onClick={() => {
                  setOpen(false);
                  onViewAll();
                }}
                className="min-h-11 w-full px-3 text-center text-[11px] font-medium text-erp-navy hover:underline"
              >
                {t("viewAll")}
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

NotificationCenter.displayName = "NotificationCenter";
