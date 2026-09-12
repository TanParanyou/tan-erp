"use client";

import React, { useId, useMemo } from "react";
import ReactDatePicker from "react-datepicker";
import { formatTimeToHHmm } from "@/lib/formatters/formatters";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import "react-datepicker/dist/react-datepicker.css";

export interface TimePickerProps {
  value?: string | null;
  onChange?: (timeString: string) => void;
  label?: string;
  error?: string;
  helperText?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
  id?: string;
}

const parseTimeString = (timeStr?: string | null): Date | null => {
  const formatted = formatTimeToHHmm(timeStr);
  if (!formatted) return null;
  const [hours, minutes] = formatted.split(":").map(Number);
  if (isNaN(hours) || isNaN(minutes)) return null;
  const d = new Date();
  d.setHours(hours, minutes, 0, 0);
  return d;
};

const formatAsTimeMask = (val: string, prevVal: string): string => {
  if (prevVal && prevVal.length > val.length) {
    return val;
  }
  const digits = val.replace(/[^0-9]/g, "");
  if (digits.length <= 2) {
    return digits;
  }
  const hh = digits.slice(0, 2);
  const mm = digits.slice(2, 4);

  let validHH = hh;
  if (parseInt(hh, 10) > 23) {
    validHH = "23";
  }
  let validMM = mm;
  if (mm && parseInt(mm, 10) > 59) {
    validMM = "59";
  }

  return `${validHH}:${validMM}`;
};

interface CustomTimeInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  value?: string;
}

const CustomTimeInput = React.forwardRef<HTMLInputElement, CustomTimeInputProps>(
  ({ value, onClick, onChange, onKeyDown, className, ...rest }, ref) => {
    const [displayVal, setDisplayVal] = React.useState(() => formatTimeToHHmm(value) || value || "");

    React.useEffect(() => {
      setDisplayVal(formatTimeToHHmm(value) || value || "");
    }, [value]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const rawVal = e.target.value;
      const formatted = formatAsTimeMask(rawVal, displayVal);
      setDisplayVal(formatted);

      if (onChange) {
        e.target.value = formatted;
        onChange(e);
      }
    };

    return (
      <input
        {...rest}
        ref={ref}
        value={displayVal}
        onChange={handleChange}
        onClick={onClick}
        onKeyDown={onKeyDown}
        className={className}
      />
    );
  }
);
CustomTimeInput.displayName = "CustomTimeInput";

/**
 * Atelier Architectural Navy Sharp - TimePicker
 * Popover-based time picker built on top of react-datepicker with custom time masking.
 */
export function TimePicker({
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
}: TimePickerProps) {
  const generatedId = useId();
  const inputId = customId || generatedId;
  const errorId = `${inputId}-error`;
  const helperId = `${inputId}-helper`;
  const t = useTranslations("opportunities");

  const selectedDate = useMemo(() => parseTimeString(value), [value]);

  const handleChange = (date: Date | null) => {
    if (!date || isNaN(date.getTime())) {
      onChange?.("");
      return;
    }
    const hours = date.getHours().toString().padStart(2, "0");
    const minutes = date.getMinutes().toString().padStart(2, "0");
    onChange?.(`${hours}:${minutes}`);
  };

  const defaultPlaceholder = placeholder || t("chooseTime");

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
          selected={selectedDate}
          onChange={handleChange}
          showTimeSelect
          showTimeSelectOnly
          timeIntervals={15}
          timeCaption="เวลา"
          timeFormat="HH:mm"
          dateFormat="HH:mm"
          disabled={disabled}
          placeholderText={defaultPlaceholder}
          customInput={<CustomTimeInput />}
          isClearable={!disabled && Boolean(value)}
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

TimePicker.displayName = "TimePicker";

export default TimePicker;
