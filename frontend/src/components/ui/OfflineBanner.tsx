"use client";

import React from "react";
import { useOnlineStatus } from "@/hooks/useOnlineStatus";
import { IconAlertTriangle } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface OfflineBannerProps {
  className?: string;
  message?: string;
}

export function OfflineBanner({
  className,
  message = "การเชื่อมต่ออินเทอร์เน็ตขาดหาย ระบบกำลังทำงานในโหมด Offline",
}: OfflineBannerProps) {
  const isOnline = useOnlineStatus();

  if (isOnline) return null;

  return (
    <div
      className={cn(
        "flex items-center justify-center gap-2 border-b border-amber-600 bg-amber-500 px-4 py-2 text-xs font-semibold text-white rounded-none shadow-xs z-50 sticky top-0",
        className
      )}
    >
      <IconAlertTriangle size={15} />
      <span>{message}</span>
    </div>
  );
}

OfflineBanner.displayName = "OfflineBanner";
