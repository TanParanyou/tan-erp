"use client";

import React, { useId, useMemo, useCallback } from "react";
import { cn } from "@/lib/utils/cn";
import { DatePicker } from "./DatePicker";
import { TimePicker } from "./TimePicker";

export interface DateTimePickerProps {
  value?: string | null;
  onChange?: (val: string) => void;
  label?: string;
  error?: string;
  helperText?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
  id?: string;
}

/**
 * Atelier Architectural Navy Sharp - DateTimePicker
 * High-precision Date and Time picker using react-datepicker.
 * Eliminates browser icon collision, uses a responsive non-overflowing grid layout,
 * and outputs ISO-compatible string YYYY-MM-DDTHH:mm.
 */
export function DateTimePicker({
  value,
  onChange,
  label,
  error,
  helperText,
  required,
  disabled = false,
  className,
  id: customId,
}: DateTimePickerProps) {
  const generatedId = useId();
  const inputId = customId || generatedId;
  const errorId = `${inputId}-error`;
  const helperId = `${inputId}-helper`;

  // Parse current date and time parts
  const { datePart, timePart } = useMemo(() => {
    if (!value || typeof value !== "string") {
      return { datePart: "", timePart: "" };
    }
    const trimmed = value.trim();
    if (!trimmed) return { datePart: "", timePart: "" };

    if (trimmed.includes("T")) {
      const [d, t] = trimmed.split("T");
      return {
        datePart: d || "",
        timePart: (t || "").slice(0, 5),
      };
    }

    if (trimmed.includes(" ")) {
      const [d, t] = trimmed.split(" ");
      return {
        datePart: d || "",
        timePart: (t || "").slice(0, 5),
      };
    }

    if (/^\d{4}-\d{2}-\d{2}$/.test(trimmed)) {
      return { datePart: trimmed, timePart: "" };
    }

    return { datePart: "", timePart: "" };
  }, [value]);

  const handleDateChange = useCallback(
    (newDate: string) => {
      if (!newDate) {
        onChange?.("");
        return;
      }
      const effectiveTime = timePart || "09:00";
      onChange?.(`${newDate}T${effectiveTime}`);
    },
    [onChange, timePart]
  );

  const handleTimeChange = useCallback(
    (newTime: string) => {
      if (!newTime) {
        if (datePart) {
          onChange?.(`${datePart}T00:00`);
        } else {
          onChange?.("");
        }
        return;
      }
      const effectiveDate = datePart || new Date().toISOString().split("T")[0];
      onChange?.(`${effectiveDate}T${newTime}`);
    },
    [datePart, onChange]
  );

  return (
    <div className={cn("erp-form-group flex flex-col gap-1.5 text-left w-full", className)}>
      {label && (
        <label className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      {/* Responsive Grid Layout to guarantee zero horizontal overflow */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 w-full">
        <DatePicker
          id={`${inputId}-date`}
          value={datePart}
          onChange={handleDateChange}
          disabled={disabled}
          placeholder="เลือกวันที่ (วว/ดด/ปปปป)"
          aria-label="เลือกวันที่"
        />

        <TimePicker
          id={`${inputId}-time`}
          value={timePart}
          onChange={handleTimeChange}
          disabled={disabled}
          placeholder="เลือกเวลา (ชม.:นาที)"
          aria-label="เลือกเวลา"
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

DateTimePicker.displayName = "DateTimePicker";

export default DateTimePicker;
