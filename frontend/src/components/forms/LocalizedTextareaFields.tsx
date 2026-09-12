"use client";

import React, { useState } from "react";
import type { FieldErrors, FieldValues, Path, UseFormRegister, UseFormSetValue, UseFormWatch } from "react-hook-form";
import { Textarea } from "@/components/ui/Textarea";
import { cn } from "@/lib/utils/cn";

type Props<T extends FieldValues> = {
  label: string;
  name: Path<T>;
  register: UseFormRegister<T>;
  setValue?: UseFormSetValue<T>;
  watch?: UseFormWatch<T>;
  errors?: FieldErrors<T>;
  disabled?: boolean;
  activeLocale?: "th" | "en";
  onActiveLocaleChange?: (locale: "th" | "en") => void;
  rows?: number;
  className?: string;
};

export function LocalizedTextareaFields<T extends FieldValues>({
  label,
  name,
  register,
  errors,
  disabled,
  activeLocale: propActiveLocale,
  onActiveLocaleChange,
  rows = 4,
  className,
}: Props<T>) {
  const [internalLocale, setInternalLocale] = useState<"th" | "en">("th");
  const active = propActiveLocale || internalLocale;

  const setActive = (locale: "th" | "en") => {
    if (onActiveLocaleChange) {
      onActiveLocaleChange(locale);
    } else {
      setInternalLocale(locale);
    }
  };

  const fieldErrors = errors?.[name as keyof typeof errors] as Record<string, { message?: string }> | undefined;

  return (
    <div className={cn("flex flex-col gap-2", className)}>
      <div className="flex items-center justify-between">
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {label}
        </span>
        <div className="flex items-center border border-erp-border bg-erp-surface-subtle p-0.5 rounded-none">
          <button
            type="button"
            onClick={() => setActive("th")}
            className={cn(
              "px-2.5 py-0.5 text-xs font-medium transition-colors rounded-none",
              active === "th"
                ? "bg-erp-navy text-white shadow-xs"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            TH {fieldErrors?.th && <span className="text-red-400 font-bold ml-0.5">•</span>}
          </button>
          <button
            type="button"
            onClick={() => setActive("en")}
            className={cn(
              "px-2.5 py-0.5 text-xs font-medium transition-colors rounded-none",
              active === "en"
                ? "bg-erp-navy text-white shadow-xs"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            EN {fieldErrors?.en && <span className="text-red-400 font-bold ml-0.5">•</span>}
          </button>
        </div>
      </div>

      <div className={active === "th" ? "block" : "hidden"}>
        <Textarea
          {...register(`${name}.th` as Path<T>)}
          disabled={disabled}
          rows={rows}
          placeholder="ระบุรายละเอียดภาษาไทย"
          error={fieldErrors?.th?.message}
        />
      </div>

      <div className={active === "en" ? "block" : "hidden"}>
        <Textarea
          {...register(`${name}.en` as Path<T>)}
          disabled={disabled}
          rows={rows}
          placeholder="Enter details in English"
          error={fieldErrors?.en?.message}
        />
      </div>
    </div>
  );
}
