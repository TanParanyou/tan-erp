"use client";

import React from "react";
import { Select, type SelectOption, type SelectProps } from "@/components/ui/Select";

export const DEFAULT_CURRENCY_OPTIONS: SelectOption[] = [
  { value: "THB", label: "THB - บาทไทย (Thai Baht)" },
  { value: "USD", label: "USD - ดอลลาร์สหรัฐ (US Dollar)" },
  { value: "EUR", label: "EUR - ยูโร (Euro)" },
  { value: "JPY", label: "JPY - เยนญี่ปุ่น (Japanese Yen)" },
  { value: "SGD", label: "SGD - ดอลลาร์สิงคโปร์ (Singapore Dollar)" },
  { value: "CNY", label: "CNY - หยวนจีน (Chinese Yuan)" },
];

export interface CurrencySelectProps
  extends Omit<SelectProps, "options"> {
  options?: SelectOption[];
}

/**
 * CurrencySelect: Centralized ERP currency selector dropdown.
 * Uses system Select component with strict 0px border-radius Atelier styling.
 */
export const CurrencySelect = React.forwardRef<HTMLSelectElement, CurrencySelectProps>(
  ({ options = DEFAULT_CURRENCY_OPTIONS, value = "THB", label = "สกุลเงิน", ...props }, ref) => {
    return (
      <Select
        ref={ref}
        label={label}
        value={value}
        options={options}
        {...props}
      />
    );
  }
);

CurrencySelect.displayName = "CurrencySelect";

export default CurrencySelect;
