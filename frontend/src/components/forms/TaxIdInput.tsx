"use client";

import React, { useId, useState, useEffect } from "react";
import { Input, type InputProps } from "@/components/ui/Input";
import { cleanTaxId, formatTaxId } from "@/lib/validation/tax-id";

export interface TaxIdInputProps
  extends Omit<InputProps, "value" | "defaultValue" | "onChange"> {
  /**
   * Raw or formatted tax ID value (13 digits).
   */
  value?: string;
  /**
   * Default uncontrolled value.
   */
  defaultValue?: string;
  /**
   * Callback fired with the cleaned 13-digit string (or formatted if returnFormatted is true).
   */
  onValueChange?: (value: string) => void;
  /**
   * Standard React change event callback.
   */
  onChange?: (e: React.ChangeEvent<HTMLInputElement> | { target: { name?: string; value: string } }) => void;
  /**
   * If true, onValueChange and onChange return formatted string (e.g. 0-1055-50000-00-0).
   * Default is false (returns raw cleaned digits).
   */
  returnFormatted?: boolean;
}

/**
 * Reusable Tax ID Input component adhering to Atelier Architectural Navy Sharp design.
 * Features:
 * - Automatically formats 13-digit Thai Tax ID / National ID (X-XXXX-XXXXX-XX-X) while typing
 * - Cleaned digits emission for database / API compatibility
 * - 0px border radius, sharp navy borders
 * - Accessible IDs and ARIA integration
 */
export const TaxIdInput = React.forwardRef<HTMLInputElement, TaxIdInputProps>(
  (
    {
      id,
      name,
      value: propValue,
      defaultValue,
      onValueChange,
      onChange,
      placeholder = "0-0000-00000-00-0",
      returnFormatted = false,
      maxLength = 17, // 13 digits + 4 dashes = 17 chars
      disabled,
      ...restProps
    },
    ref,
  ) => {
    const autoId = useId();
    const inputId = id || `tax-id-input-${autoId}`;

    const isControlled = propValue !== undefined;
    const initialRaw = (isControlled ? propValue : defaultValue) || "";
    const [displayValue, setDisplayValue] = useState<string>(formatTaxId(initialRaw));

    useEffect(() => {
      if (isControlled) {
        setDisplayValue(formatTaxId(propValue || ""));
      }
    }, [propValue, isControlled]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const rawInput = e.target.value;
      const cleaned = cleanTaxId(rawInput).slice(0, 13);
      const formatted = formatTaxId(cleaned);

      if (!isControlled) {
        setDisplayValue(formatted);
      }

      const emittedValue = returnFormatted ? formatted : cleaned;

      onValueChange?.(emittedValue);

      if (onChange) {
        const syntheticEvent = {
          ...e,
          target: {
            ...e.target,
            name: name ?? "",
            value: emittedValue,
          },
          currentTarget: {
            ...e.currentTarget,
            name: name ?? "",
            value: emittedValue,
          },
        };
        onChange(syntheticEvent as unknown as React.ChangeEvent<HTMLInputElement>);
      }
    };

    return (
      <Input
        {...restProps}
        ref={ref}
        id={inputId}
        name={name}
        type="text"
        inputMode="numeric"
        disabled={disabled}
        placeholder={placeholder}
        maxLength={maxLength}
        value={displayValue}
        onChange={handleChange}
      />
    );
  },
);

TaxIdInput.displayName = "TaxIdInput";

export default TaxIdInput;
