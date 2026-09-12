"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import { DEFAULT_CURRENCY_OPTIONS } from "./CurrencySelect";
import type { SelectOption } from "@/components/ui/Select";

export interface CurrencyAmountInputProps {
  /** Main label for the combined currency and amount field */
  label?: string;
  /** Whether the field is required */
  required?: boolean;
  /** Whether inputs are disabled */
  disabled?: boolean;
  /** Current numeric amount value */
  amountValue: number | string | undefined | null;
  /** Callback when amount changes */
  onAmountChange: (value: number | undefined) => void;
  /** Current currency code (e.g. "THB", "USD") */
  currencyValue: string;
  /** Callback when currency changes */
  onCurrencyChange: (currency: string) => void;
  /** Available currency options (defaults to DEFAULT_CURRENCY_OPTIONS) */
  currencyOptions?: SelectOption[];
  /** Amount input placeholder */
  placeholder?: string;
  /** Error message for amount field */
  amountError?: string;
  /** Error message for currency field */
  currencyError?: string;
  /** Optional contextual helper text */
  helperText?: string;
  /** Additional container classes */
  className?: string;
  /** HTML id prefix */
  id?: string;
}

/**
 * Atelier Architectural Navy Sharp - Centralized Currency Amount Compound Input.
 * Seamlessly joins a numeric amount input with a currency code dropdown.
 * Zero radius (0px), 44px min-touch height, clear focus states, and unified error handling.
 */
export function CurrencyAmountInput({
  label,
  required,
  disabled,
  amountValue,
  onAmountChange,
  currencyValue,
  onCurrencyChange,
  currencyOptions = DEFAULT_CURRENCY_OPTIONS,
  placeholder = "0.00",
  amountError,
  currencyError,
  helperText,
  className,
  id,
}: CurrencyAmountInputProps): React.JSX.Element {
  const generatedId = id || (label ? `amount-${label.replace(/\s+/g, "-").toLowerCase()}` : "currency-amount-input");
  const currencyId = `${generatedId}-currency`;
  const errorId = `${generatedId}-error`;
  const helperId = `${generatedId}-helper`;

  const errorMessage = amountError || currencyError;
  const hasError = Boolean(errorMessage);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>): void => {
    // Prevent typing negative sign or scientific notation
    if (e.key === "-" || e.key === "e" || e.key === "E") {
      e.preventDefault();
    }
  };

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const raw = e.target.value.trim();
    if (raw === "") {
      onAmountChange(undefined);
    } else {
      const parsed = Number(raw);
      onAmountChange(Number.isNaN(parsed) ? undefined : parsed);
    }
  };

  return (
    <div className={cn("erp-form-group", className)}>
      {label && (
        <label htmlFor={generatedId} className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      <div className="flex items-stretch w-full">
        {/* Numeric Amount Input */}
        <div className="relative flex-1 min-w-0">
          <input
            id={generatedId}
            type="number"
            min="0"
            step="any"
            disabled={disabled}
            placeholder={placeholder}
            value={amountValue === undefined || amountValue === null ? "" : amountValue}
            onKeyDown={handleKeyDown}
            onChange={handleAmountChange}
            aria-invalid={hasError}
            aria-describedby={hasError ? errorId : helperText ? helperId : undefined}
            className={cn(
              "erp-input text-right font-mono",
              hasError && "erp-input-error",
              "border-r-0 focus:z-10 relative"
            )}
          />
        </div>

        {/* Currency Selector Dropdown */}
        <div className="w-28 sm:w-32 shrink-0">
          <select
            id={currencyId}
            disabled={disabled}
            value={currencyValue || "THB"}
            onChange={(e) => onCurrencyChange(e.target.value)}
            aria-invalid={hasError}
            aria-label={label ? `${label} (Currency)` : "Currency"}
            className={cn(
              "erp-select font-medium text-xs sm:text-sm bg-erp-surface-subtle",
              hasError && "erp-select-error",
              "focus:z-10 relative"
            )}
          >
            {currencyOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.value}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Unified Error or Helper Text */}
      {errorMessage ? (
        <p id={errorId} className="erp-error-text" role="alert">
          {errorMessage}
        </p>
      ) : helperText ? (
        <p id={helperId} className="erp-helper-text">
          {helperText}
        </p>
      ) : null}
    </div>
  );
}

export default CurrencyAmountInput;
