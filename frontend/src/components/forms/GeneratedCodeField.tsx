"use client";

import { Input } from "@/components/ui/Input";
import { useTranslations } from "next-intl";

export type GeneratedCodeMode = "generated" | "manual";

interface GeneratedCodeFieldProps {
  id: string;
  label: string;
  value: string;
  mode: GeneratedCodeMode;
  maxLength: number;
  disabled?: boolean;
  error?: string;
  placeholder?: string;
  onChange: (value: string) => void;
  onModeChange: (mode: GeneratedCodeMode) => void;
}

export function GeneratedCodeField({
  id,
  label,
  value,
  mode,
  maxLength,
  disabled = false,
  error,
  placeholder,
  onChange,
  onModeChange,
}: GeneratedCodeFieldProps) {
  const t = useTranslations("itemMaster");

  return (
    <fieldset className="space-y-2">
      <legend className="erp-label">{t("codeEntryMode")}</legend>
      <div className="flex flex-wrap gap-x-5 gap-y-2">
        <label className="flex min-h-11 items-center gap-2 text-sm text-erp-text-main">
          <input
            type="radio"
            name={`${id}-mode`}
            value="generated"
            checked={mode === "generated"}
            disabled={disabled}
            onChange={() => onModeChange("generated")}
          />
          {t("codeModeGenerated")}
        </label>
        <label className="flex min-h-11 items-center gap-2 text-sm text-erp-text-main">
          <input
            type="radio"
            name={`${id}-mode`}
            value="manual"
            checked={mode === "manual"}
            disabled={disabled}
            onChange={() => onModeChange("manual")}
          />
          {t("codeModeManual")}
        </label>
      </div>
      {mode === "generated" ? (
        <p className="text-sm text-erp-text-muted" role="status">{t("codeGeneratedAtSave")}</p>
      ) : (
        <Input
          id={id}
          label={label}
          value={value}
          placeholder={placeholder ?? t("codePlaceholder")}
          onChange={(event) => onChange(event.target.value)}
          maxLength={maxLength}
          required
          disabled={disabled}
          error={error}
        />
      )}
    </fieldset>
  );
}
