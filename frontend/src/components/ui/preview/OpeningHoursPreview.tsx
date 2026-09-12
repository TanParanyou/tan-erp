"use client";

import React from "react";
import { IconClock } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface OpeningHourItem {
  day: string;
  hours: string;
  isOpen?: boolean;
}

export interface OpeningHoursPreviewProps {
  schedule?: OpeningHourItem[];
  title?: string;
  className?: string;
}

export function OpeningHoursPreview({
  schedule = [
    { day: "จันทร์ - ศุกร์", hours: "08:30 - 17:30", isOpen: true },
    { day: "เสาร์", hours: "09:00 - 15:00", isOpen: true },
    { day: "อาทิตย์", hours: "ปิดทำการ", isOpen: false },
  ],
  title = "เวลาทำการ (Business Hours)",
  className,
}: OpeningHoursPreviewProps) {
  return (
    <div className={cn("border border-erp-border bg-erp-surface p-4 rounded-none text-left space-y-2.5", className)}>
      <div className="flex items-center gap-2 border-b border-erp-border pb-2">
        <IconClock size={16} className="text-erp-navy" />
        <span className="text-xs font-bold uppercase tracking-wider text-erp-text-main">
          {title}
        </span>
      </div>
      <div className="divide-y divide-erp-border text-xs">
        {schedule.map((item) => (
          <div key={item.day} className="flex items-center justify-between py-1.5">
            <span className="text-erp-text-main font-medium">{item.day}</span>
            <span className={item.isOpen ? "text-erp-text-muted font-mono" : "text-red-600 font-medium"}>
              {item.hours}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}

OpeningHoursPreview.displayName = "OpeningHoursPreview";
