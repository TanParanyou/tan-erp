"use client";

import React, { useState, useEffect, useId } from "react";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import {
  PHONE_COUNTRIES,
  DEFAULT_COUNTRY_DIAL_CODE,
  parsePhoneValue,
  formatCombinedPhone,
  findCountryByDialCode,
} from "./phone-country-codes";

export interface PhoneInputProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "onChange"> {
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
  leftIcon?: React.ReactNode;
  wrapperClassName?: string;
  defaultCountry?: string;
  hideCountrySelect?: boolean;
  onValueChange?: (value: string) => void;
  onChange?: (
    event: React.ChangeEvent<HTMLInputElement> | { target: { name?: string; value: string } },
  ) => void;
}

/**
 * Reusable composite PhoneInput component for ERP forms.
 * Follows Atelier Architectural Navy Sharp design principles:
 * - 0px border radius
 * - Solid Navy focus border
 * - 44px minimum touch target
 * - Country dial code selector on the left, defaulting to Thailand (+66)
 * - Seamless integration with React Hook Form Controller
 */
export const PhoneInput = React.forwardRef<HTMLInputElement, PhoneInputProps>(
  (
    {
      className,
      wrapperClassName,
      label,
      error,
      helperText,
      required,
      id,
      name,
      disabled,
      placeholder,
      value: propValue,
      defaultValue,
      defaultCountry = DEFAULT_COUNTRY_DIAL_CODE,
      hideCountrySelect = false,
      leftIcon,
      onValueChange,
      onChange,
      onBlur,
      ...restProps
    },
    ref,
  ) => {
    const t = useTranslations("common.phone");
    const autoId = useId();
    const generatedId = id || `phone-input-${autoId}`;
    const countrySelectId = `${generatedId}-country`;
    const errorId = `${generatedId}-error`;
    const helperId = `${generatedId}-helper`;

    // Extract initial values
    const isControlled = propValue !== undefined;
    const initialRaw = (isControlled ? propValue : defaultValue) as string | undefined;
    const initialParsed = parsePhoneValue(initialRaw, defaultCountry);

    const [selectedCountry, setSelectedCountry] = useState<string>(initialParsed.dialCode);
    const [uncontrolledNumber, setUncontrolledNumber] = useState<string>(
      initialParsed.nationalNumber,
    );

    // Sync country code when controlled propValue changes with a new country prefix
    useEffect(() => {
      if (typeof propValue === "string" && propValue.trim() !== "") {
        const parsed = parsePhoneValue(propValue, defaultCountry);
        setSelectedCountry(parsed.dialCode);
      }
    }, [propValue, defaultCountry]);

    // Compute display number in input
    const currentNationalNumber = isControlled
      ? parsePhoneValue(propValue as string | undefined, selectedCountry).nationalNumber
      : uncontrolledNumber;

    const triggerChange = (
      combinedValue: string,
      originalEvent?: React.ChangeEvent<HTMLInputElement>,
    ) => {
      onValueChange?.(combinedValue);

      if (!onChange) return;

      if (originalEvent) {
        const syntheticEvent: React.ChangeEvent<HTMLInputElement> = {
          ...originalEvent,
          target: {
            ...originalEvent.target,
            name: name ?? "",
            value: combinedValue,
          },
          currentTarget: {
            ...originalEvent.currentTarget,
            name: name ?? "",
            value: combinedValue,
          },
        };
        onChange(syntheticEvent);
      } else {
        onChange({
          target: {
            name,
            value: combinedValue,
          },
        });
      }
    };

    const countryConfig = findCountryByDialCode(selectedCountry);
    const maxInputLength = countryConfig ? countryConfig.maxInputLength : 15;

    const handleNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const rawInput = e.target.value;

      // If user pasted or typed full international number starting with +
      if (rawInput.trim().startsWith("+")) {
        const parsed = parsePhoneValue(rawInput, selectedCountry);
        setSelectedCountry(parsed.dialCode);
        if (!isControlled) {
          setUncontrolledNumber(parsed.nationalNumber);
        }
        const combined = formatCombinedPhone(parsed.dialCode, parsed.nationalNumber);
        triggerChange(combined, e);
        return;
      }

      // Restrict input to digits, spaces, and dashes for clean phone entry
      const sanitized = rawInput.replace(/[^\d\s\-]/g, "");

      if (!isControlled) {
        setUncontrolledNumber(sanitized);
      }

      const combined = formatCombinedPhone(selectedCountry, sanitized);
      triggerChange(combined, e);
    };

    const handleCountryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
      const newDialCode = e.target.value;
      setSelectedCountry(newDialCode);

      const combined = formatCombinedPhone(newDialCode, currentNationalNumber);
      triggerChange(combined);
    };

    return (
      <div className={cn("erp-form-group", wrapperClassName)}>
        {label && (
          <label htmlFor={generatedId} className="erp-label">
            {label}
            {required && <span className="erp-label-required">*</span>}
          </label>
        )}

        <div
          className={cn(
            "erp-input-wrapper flex items-stretch border bg-erp-surface h-[44px] box-border",
            error
              ? "border-erp-danger focus-within:border-erp-danger focus-within:ring-1 focus-within:ring-erp-danger"
              : "border-erp-border focus-within:border-erp-navy focus-within:ring-1 focus-within:ring-erp-navy",
            disabled && "bg-erp-surface-muted opacity-60 cursor-not-allowed",
          )}
        >
          {leftIcon && (
            <div className="flex items-center pl-3 pr-1 text-erp-slate-400 pointer-events-none shrink-0">
              {leftIcon}
            </div>
          )}

          {!hideCountrySelect && (
            <select
              id={countrySelectId}
              aria-label={t("countryLabel")}
              value={selectedCountry}
              onChange={handleCountryChange}
              disabled={disabled}
              className={cn(
                "w-[90px] h-full bg-transparent pl-2.5 pr-5 text-sm font-medium text-erp-text-main border-r border-erp-border focus:outline-none cursor-pointer shrink-0 appearance-none",
                disabled && "cursor-not-allowed",
              )}
              style={{
                backgroundImage: `url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='%234B5563' stroke-width='2' stroke-linecap='square' stroke-linejoin='miter'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E")`,
                backgroundRepeat: "no-repeat",
                backgroundPosition: "right 0.35rem center",
              }}
            >
              {PHONE_COUNTRIES.map((c) => (
                <option
                  key={c.code}
                  value={c.dialCode}
                  className="bg-erp-surface text-erp-text-main"
                >
                  {c.code} {c.dialCode}
                </option>
              ))}
            </select>
          )}

          <input
            ref={ref}
            id={generatedId}
            name={name}
            type="tel"
            inputMode="tel"
            autoComplete="tel"
            maxLength={maxInputLength}
            value={currentNationalNumber}
            onChange={handleNumberChange}
            onBlur={onBlur}
            disabled={disabled}
            required={required}
            placeholder={placeholder}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : helperText ? helperId : undefined}
            className={cn(
              "w-full flex-1 h-full bg-transparent px-3 text-sm text-erp-text-main placeholder:text-erp-slate-400 focus:outline-none",
              disabled && "cursor-not-allowed",
              className,
            )}
            {...restProps}
          />
        </div>

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
  },
);

PhoneInput.displayName = "PhoneInput";
