"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import { DatePicker } from "./DatePicker";

export interface DateRange {
  startDate?: string | null;
  endDate?: string | null;
}

export interface DateRangePickerProps {
  value?: DateRange;
  onChange?: (range: DateRange) => void;
  label?: string;
  error?: string;
  className?: string;
  disabled?: boolean;
}

export function DateRangePicker({
  value = {},
  onChange,
  label,
  error,
  className,
  disabled,
}: DateRangePickerProps) {
  const handleStartChange = (startDate: string) => {
    onChange?.({ ...value, startDate });
  };

  const handleEndChange = (endDate: string) => {
    onChange?.({ ...value, endDate });
  };

  return (
    <div className={cn("flex flex-col gap-1.5", className)}>
      {label && (
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {label}
        </span>
      )}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
        <DatePicker
          value={value.startDate}
          onChange={handleStartChange}
          disabled={disabled}
          placeholder="เริ่ม (YYYY-MM-DD)"
          aria-label="Start date"
        />
        <DatePicker
          value={value.endDate}
          onChange={handleEndChange}
          disabled={disabled}
          placeholder="สิ้นสุด (YYYY-MM-DD)"
          aria-label="End date"
        />
      </div>
      {error && <p className="text-xs text-red-600 dark:text-red-400">{error}</p>}
    </div>
  );
}

DateRangePicker.displayName = "DateRangePicker";
