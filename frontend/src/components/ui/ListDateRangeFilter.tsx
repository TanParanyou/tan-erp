"use client";

import React from "react";
import { DateRangePicker, type DateRange } from "./DateRangePicker";
import { cn } from "@/lib/utils/cn";

export interface ListDateRangeFilterProps {
  startDate?: string | null;
  endDate?: string | null;
  onChange?: (range: DateRange) => void;
  className?: string;
  label?: string;
}

export function ListDateRangeFilter({
  startDate,
  endDate,
  onChange,
  className,
  label,
}: ListDateRangeFilterProps) {
  return (
    <div className={cn("flex flex-col gap-1", className)}>
      <DateRangePicker
        value={{ startDate, endDate }}
        onChange={onChange}
        label={label}
      />
    </div>
  );
}

ListDateRangeFilter.displayName = "ListDateRangeFilter";
