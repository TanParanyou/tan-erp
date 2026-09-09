"use client";

import React from "react";
import { Select } from "@/components/ui/Select";
import { useTranslations } from "next-intl";

export interface ListFilterSelectOption {
  value: string;
  label: string;
}

export interface ListFilterSelectProps {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  options: ListFilterSelectOption[];
  allOptionLabel?: string;
  className?: string;
  widthClassName?: string;
}

export function ListFilterSelect({
  id,
  label,
  value,
  onChange,
  options,
  allOptionLabel,
  className = "",
  widthClassName = "w-40",
}: ListFilterSelectProps) {
  const tCommon = useTranslations("common.filters");
  const defaultAllLabel = allOptionLabel ?? `-- ${tCommon("all")} --`;

  const allOptions: ListFilterSelectOption[] = [
    { value: "", label: defaultAllLabel },
    ...options,
  ];

  return (
    <div className={`${widthClassName} ${className}`}>
      <Select
        id={id}
        label={label}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        aria-label={label}
        options={allOptions}
        wrapperClassName="!mb-0 !gap-1.5"
        labelClassName="text-sm font-semibold text-erp-navy"
        className="h-10 text-xs sm:text-sm !min-h-[40px]"
      />
    </div>
  );
}

export default ListFilterSelect;
