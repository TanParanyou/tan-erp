"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";
import {
  BASELINE_MEASUREMENT_UNITS,
  convertMeasurementToMm,
  type MeasurementUnitOption,
} from "@/features/surveys/hooks/use-measurement-metadata";

export interface MeasurementAmountInputProps {
  /** Main label for the field (optional) */
  label?: string;
  /** Whether the field is required */
  required?: boolean;
  /** Whether inputs are disabled */
  disabled?: boolean;
  /** Current numeric value */
  value: number | string | undefined | null;
  /** Callback when value changes */
  onChange: (value: number | undefined) => void;
  /** Current unit code (e.g. "m", "cm", "mm", "sqm", "unit") */
  unitCode: string;
  /** Callback when unit code changes */
  onUnitChange: (unitCode: string) => void;
  /** Available units (defaults to BASELINE_MEASUREMENT_UNITS) */
  unitOptions?: MeasurementUnitOption[];
  /** Value input placeholder */
  placeholder?: string;
  /** Step precision for number input (default: 0.01) */
  step?: string | number;
  /** Minimum value (default: 0.0001) */
  min?: string | number;
  /** Error message */
  error?: string;
  /** Optional helper text */
  helperText?: string;
  /** Additional container classes */
  className?: string;
  /** HTML id prefix */
  id?: string;
  /** Whether to show live mm conversion badge under input (default: true) */
  showMmConversion?: boolean;
  /** Custom input className */
  inputClassName?: string;
  /** Custom select className */
  selectClassName?: string;
}

/**
 * Atelier Architectural Navy Sharp - Centralized Measurement Amount Compound Input.
 * Seamlessly joins a numeric dimension input with a unit selector dropdown.
 * Zero radius (0px), 44px min-touch height, live mm conversion, and unified error handling.
 */
export function MeasurementAmountInput({
  label,
  required,
  disabled,
  value,
  onChange,
  unitCode = "m",
  onUnitChange,
  unitOptions = BASELINE_MEASUREMENT_UNITS,
  placeholder = "0.00",
  step = "0.01",
  min = "0.0001",
  error,
  helperText,
  className,
  id,
  showMmConversion = true,
  inputClassName,
  selectClassName,
}: MeasurementAmountInputProps): React.JSX.Element {
  const generatedId = id || (label ? `measurement-${label.replace(/\s+/g, "-").toLowerCase()}` : "measurement-amount-input");
  const unitId = `${generatedId}-unit`;
  const errorId = `${generatedId}-error`;
  const helperId = `${generatedId}-helper`;

  const numValue = typeof value === "number" ? value : parseFloat(String(value)) || 0;
  const conversion = convertMeasurementToMm(numValue, unitCode);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>): void => {
    // Prevent typing negative sign or scientific notation
    if (e.key === "-" || e.key === "e" || e.key === "E") {
      e.preventDefault();
    }
  };

  const handleValueChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const raw = e.target.value.trim();
    if (raw === "") {
      onChange(undefined);
    } else {
      const parsed = parseFloat(raw);
      onChange(Number.isNaN(parsed) ? undefined : parsed);
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
        {/* Numeric Dimension Input */}
        <div className="relative flex-1 min-w-0">
          <input
            id={generatedId}
            type="number"
            min={min}
            step={step}
            disabled={disabled}
            placeholder={placeholder}
            value={value === undefined || value === null || value === 0 ? "" : value}
            onKeyDown={handleKeyDown}
            onChange={handleValueChange}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : helperText ? helperId : undefined}
            className={cn(
              "erp-input text-right font-mono border-r-0 focus:z-10 relative",
              error && "erp-input-error",
              inputClassName
            )}
          />
        </div>

        {/* Unit Selector Dropdown */}
        <div className="w-20 sm:w-24 shrink-0">
          <select
            id={unitId}
            disabled={disabled}
            value={unitCode || "m"}
            onChange={(e) => onUnitChange(e.target.value)}
            aria-invalid={Boolean(error)}
            aria-label={label ? `${label} (Unit)` : "Unit"}
            className={cn(
              "erp-select font-mono font-medium text-xs sm:text-sm bg-erp-surface-subtle focus:z-10 relative",
              error && "erp-select-error",
              selectClassName
            )}
          >
            {unitOptions.map((opt) => (
              <option key={opt.code} value={opt.code}>
                {opt.symbol}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Live Millimeter (mm) Conversion Result */}
      {showMmConversion && conversion.convertedValue !== null && unitCode !== "mm" && (
        <div className="mt-1 text-[11px] font-mono font-semibold text-erp-navy text-right">
          = {conversion.formattedText}
        </div>
      )}

      {/* Error or Helper Text */}
      {error ? (
        <p id={errorId} className="erp-error-text" role="alert">
          {error}
        </p>
      ) : helperText ? (
        <p id={helperId} className="erp-helper-text">
          {helperText}
        </p>
      ) : null}
    </div>
  );
}

export default MeasurementAmountInput;
