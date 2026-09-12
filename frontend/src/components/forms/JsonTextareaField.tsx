"use client";

import React, { useState } from "react";
import type { Control, FieldValues, Path } from "react-hook-form";
import { useController } from "react-hook-form";
import { cn } from "@/lib/utils/cn";

type Props<T extends FieldValues> = {
  label: string;
  name: Path<T>;
  control: Control<T>;
  rows?: number;
  disabled?: boolean;
  className?: string;
};

export function JsonTextareaField<T extends FieldValues>({
  label,
  name,
  control,
  rows = 6,
  disabled,
  className,
}: Props<T>) {
  const { field, fieldState } = useController({ control, name });
  const [text, setText] = useState(() => stringifyJson(field.value));
  const [parseError, setParseError] = useState<string | null>(null);

  const [lastValue, setLastValue] = useState(field.value);
  if (field.value !== lastValue) {
    setLastValue(field.value);
    setText(stringifyJson(field.value));
    setParseError(null);
  }

  const handleBlur = () => {
    field.onBlur();
    try {
      const parsed = text.trim() ? JSON.parse(text) : {};
      if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
        setParseError("JSON must be an object");
        return;
      }
      setParseError(null);
      field.onChange(parsed);
    } catch {
      setParseError("Invalid JSON");
    }
  };

  const error = parseError || fieldState.error?.message;

  return (
    <div className={cn("flex flex-col gap-1.5 text-left", className)}>
      <div className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
        {label}
      </div>
      <textarea
        rows={rows}
        disabled={disabled}
        value={text}
        onChange={(event) => setText(event.target.value)}
        onBlur={handleBlur}
        className={cn(
          "w-full border border-erp-border bg-erp-surface px-3 py-2 font-mono text-xs text-erp-text-main rounded-none outline-none transition-colors",
          "focus:border-erp-navy focus:ring-1 focus:ring-erp-navy",
          "disabled:cursor-not-allowed disabled:bg-erp-surface-subtle disabled:opacity-60",
          error ? "border-red-600 focus:border-red-600 focus:ring-red-600" : ""
        )}
      />
      {error && <p className="text-xs text-red-600 dark:text-red-400">{error}</p>}
    </div>
  );
}

function stringifyJson(value: unknown) {
  if (!value || typeof value !== "object") return "{}";
  return JSON.stringify(value, null, 2);
}
