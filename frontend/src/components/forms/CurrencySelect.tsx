"use client";

import React, { useMemo } from "react";
import { Select, type SelectOption, type SelectProps } from "@/components/ui/Select";
import { useTranslations } from "next-intl";

export const CURRENCY_CODES = ["THB", "USD", "EUR", "JPY", "SGD", "CNY"] as const;

export const DEFAULT_CURRENCY_OPTIONS: SelectOption[] = CURRENCY_CODES.map((code) => ({
  value: code,
  label: code,
}));

export interface CurrencySelectProps
  extends Omit<SelectProps, "options"> {
  options?: SelectOption[];
}

/**
 * CurrencySelect: Centralized ERP currency selector dropdown.
 * Uses system Select component with strict 0px border-radius Atelier styling.
 */
export const CurrencySelect = React.forwardRef<HTMLSelectElement, CurrencySelectProps>(
  ({ options, value = "THB", label, ...props }, ref) => {
    const t = useTranslations("common.currencies");

    const defaultOptions = useMemo<SelectOption[]>(() => {
      return CURRENCY_CODES.map((code) => ({
        value: code,
        label: t(code),
      }));
    }, [t]);

    return (
      <Select
        ref={ref}
        label={label ?? t("label")}
        value={value}
        options={options ?? defaultOptions}
        {...props}
      />
    );
  }
);

CurrencySelect.displayName = "CurrencySelect";

export default CurrencySelect;
