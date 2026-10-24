"use client";

import { Input } from "@/components/ui/Input";
import { RadioGroup } from "@/components/ui/Radio";
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
      <RadioGroup<GeneratedCodeMode>
        name={`${id}-mode`}
        value={mode}
        onChange={onModeChange}
        orientation="horizontal"
        disabled={disabled}
        options={[
          {
            value: "generated",
            label: t("codeModeGenerated"),
          },
          {
            value: "manual",
            label: t("codeModeManual"),
          },
        ]}
      />
      {mode === "generated" ? (
        <p className="text-sm text-erp-text-muted" role="status">
          {t("codeGeneratedAtSave")}
        </p>
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
