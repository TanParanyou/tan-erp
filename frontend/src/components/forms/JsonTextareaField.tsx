"use client";

import React, { useState } from "react";
import type { Control, FieldValues, Path } from "react-hook-form";
import { useController } from "react-hook-form";
import { Textarea } from "@/components/ui/Textarea";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";

type Props<T extends FieldValues> = {
  label: string;
  name: Path<T>;
  control: Control<T>;
  rows?: number;
  disabled?: boolean;
  className?: string;
  placeholder?: string;
};

export function JsonTextareaField<T extends FieldValues>({
  label,
  name,
  control,
  rows = 6,
  disabled,
  className,
  placeholder,
}: Props<T>) {
  const t = useTranslations("common.form");
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
    <Textarea
      label={label}
      rows={rows}
      disabled={disabled}
      value={text}
      onChange={(event) => setText(event.target.value)}
      onBlur={handleBlur}
      placeholder={placeholder ?? t("jsonPlaceholder")}
      error={error}
      className={cn("font-mono text-xs", className)}
      wrapperClassName="text-left"
    />
  );
}

function stringifyJson(value: unknown) {
  if (!value || typeof value !== "object") return "{}";
  return JSON.stringify(value, null, 2);
}
