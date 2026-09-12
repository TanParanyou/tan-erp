"use client";

import React from "react";
import { IconClock, IconCalendar } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import {
  formatDate,
  formatDateTime,
  formatTime,
} from "@/lib/formatters/formatters";

export interface DateTimeDisplayProps {
  date: string | Date | null | undefined;
  locale?: string;
  format?: "datetime" | "date" | "time";
  showIcon?: boolean;
  icon?: React.ReactNode;
  fallbackText?: string;
  className?: string;
}

export function DateTimeDisplay({
  date,
  locale = "th",
  format = "datetime",
  showIcon = true,
  icon,
  fallbackText = "-",
  className,
}: DateTimeDisplayProps) {
  if (!date) {
    return <span className={cn("text-inherit", className)}>{fallbackText}</span>;
  }

  let text = fallbackText;
  switch (format) {
    case "datetime":
      text = formatDateTime(date, locale);
      break;
    case "date":
      text = formatDate(date, locale);
      break;
    case "time":
      text = formatTime(date, locale);
      break;
  }

  if (text === "-") {
    return <span className={cn("text-inherit", className)}>{fallbackText}</span>;
  }

  const defaultIcon =
    format === "time" ? (
      <IconClock size={14} className="shrink-0 opacity-70" />
    ) : (
      <IconCalendar size={14} className="shrink-0 opacity-70" />
    );

  return (
    <span className={cn("inline-flex items-center gap-1.5 text-inherit", className)}>
      {showIcon && (icon ?? defaultIcon)}
      <span>{text}</span>
    </span>
  );
}

DateTimeDisplay.displayName = "DateTimeDisplay";
