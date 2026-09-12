"use client";

import React, { useId, useMemo } from "react";
import ReactDatePicker from "react-datepicker";
import { format, parse } from "date-fns";
import { th, enUS } from "date-fns/locale";
import { useLocale, useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import "react-datepicker/dist/react-datepicker.css";

export interface DatePickerProps {
  value?: string | null;
  onChange?: (dateString: string) => void;
  label?: string;
  error?: string;
  helperText?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
  id?: string;
}

const dateFnsLocales = { th, en: enUS };

/**
 * Atelier Architectural Navy Sharp - DatePicker
 * Professional date picker built on top of react-datepicker.
 * Eliminates native browser icon stacking and provides seamless Thai/English calendar selection.
 */
export function DatePicker({
  value,
  onChange,
  label,
  error,
  helperText,
  placeholder,
  required,
  disabled = false,
  className,
  id: customId,
}: DatePickerProps) {
  const generatedId = useId();
  const inputId = customId || generatedId;
  const errorId = `${inputId}-error`;
  const helperId = `${inputId}-helper`;
  const currentLocale = useLocale();
  const t = useTranslations("opportunities");

  const activeLocale = currentLocale === "th" ? th : enUS;

  const parsedDate = useMemo(() => {
    if (!value || typeof value !== "string") return null;
    const trimmed = value.trim().split("T")[0];
    if (!trimmed) return null;
    const d = parse(trimmed, "yyyy-MM-dd", new Date());
    return isNaN(d.getTime()) ? null : d;
  }, [value]);

  const handleChange = (date: Date | null) => {
    if (!date || isNaN(date.getTime())) {
      onChange?.("");
      return;
    }
    onChange?.(format(date, "yyyy-MM-dd"));
  };

  const defaultPlaceholder = placeholder || t("chooseDate");

  return (
    <div className={cn("erp-form-group flex flex-col gap-1.5 text-left w-full", className)}>
      {label && (
        <label htmlFor={inputId} className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      <div className="relative w-full">
        <ReactDatePicker
          id={inputId}
          selected={parsedDate}
          onChange={handleChange}
          dateFormat="dd/MM/yyyy"
          locale={activeLocale}
          placeholderText={defaultPlaceholder}
          disabled={disabled}
          isClearable={!disabled && Boolean(value)}
          showMonthDropdown
          showYearDropdown
          dropdownMode="select"
          selectsRange={false}
          selectsMultiple={false}
          className={cn(
            "erp-input w-full min-h-[44px] px-3 py-2 text-sm text-erp-text-main bg-erp-surface border border-erp-border rounded-none outline-none transition-colors",
            "focus:border-erp-navy focus:ring-1 focus:ring-erp-navy",
            "disabled:cursor-not-allowed disabled:bg-erp-surface-subtle disabled:opacity-60",
            error ? "border-red-600 focus:border-red-600 focus:ring-red-600" : ""
          )}
          wrapperClassName="w-full"
          aria-invalid={error ? "true" : "false"}
          aria-describedby={error ? errorId : helperText ? helperId : undefined}
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
}

DatePicker.displayName = "DatePicker";

export default DatePicker;
