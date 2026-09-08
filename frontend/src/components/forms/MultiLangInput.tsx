"use client";

import React, { useState } from "react";
import { IconCopy } from "@/components/common/Icons";
import { useLocale } from "next-intl";
import { cn } from "@/lib/utils/cn";

export interface MultiLangText {
  th?: string;
  en?: string;
}

export interface MultiLangInputProps {
  label: string;
  value?: MultiLangText;
  onChange: (value: MultiLangText) => void;
  type?: "input" | "textarea";
  required?: boolean;
  placeholder?: string | MultiLangText;
  error?: string;
  defaultLang?: "th" | "en";
  id?: string;
}

const langs = [
  { key: "th" as const, label: "TH" },
  { key: "en" as const, label: "EN" },
];

export function MultiLangInput({
  label,
  value,
  onChange,
  type = "input",
  required = false,
  placeholder,
  error,
  defaultLang,
  id,
}: MultiLangInputProps) {
  const systemLocale = useLocale();
  const validSystemLang = (systemLocale === "en" ? "en" : "th") as "th" | "en";
  const [selectedLang, setSelectedLang] = useState<"th" | "en" | null>(null);
  const activeLang = selectedLang ?? defaultLang ?? validSystemLang;

  const safeValue = value || { th: "", en: "" };

  const handleChange = (text: string) => {
    onChange({ ...safeValue, [activeLang]: text });
  };

  const handleCopySource = () => {
    const source = safeValue.th?.trim();
    if (!source) return;
    onChange({
      ...safeValue,
      en: safeValue.en ? safeValue.en : source,
    });
  };

  const currentPlaceholder = React.useMemo(() => {
    if (typeof placeholder === "object" && placeholder !== null) {
      return placeholder[activeLang] || placeholder.th || `${label} (${activeLang.toUpperCase()})`;
    }
    if (typeof placeholder === "string" && placeholder) {
      return placeholder;
    }
    return `${label} (${activeLang.toUpperCase()})`;
  }, [placeholder, activeLang, label]);

  const generatedId = id || `multilang-${label.replace(/\s+/g, "-").toLowerCase()}`;

  return (
    <div className="erp-form-group">
      <div className="flex items-center justify-between min-h-[24px]">
        <label htmlFor={generatedId} className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>

        <div className="flex items-center gap-1.5">
          <button
            type="button"
            onClick={handleCopySource}
            disabled={!safeValue.th?.trim()}
            title="Copy TH to EN"
            aria-label="Copy TH to EN"
            className="h-6 w-6 border border-erp-border bg-erp-surface hover:bg-erp-surface-muted text-erp-text-muted hover:text-erp-text-main disabled:opacity-40 disabled:cursor-not-allowed inline-flex items-center justify-center transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy rounded-none"
          >
            <IconCopy size={13} />
          </button>

          <div className="inline-flex border border-erp-border rounded-none overflow-hidden h-6">
            {langs.map((lang) => {
              const hasContent = Boolean(safeValue[lang.key]?.trim());
              const isActive = activeLang === lang.key;
              return (
                <button
                  key={lang.key}
                  type="button"
                  onClick={() => setSelectedLang(lang.key)}
                  className={cn(
                    "px-2 h-full text-xs font-semibold uppercase transition-colors inline-flex items-center justify-center gap-1 focus-visible:outline-2 focus-visible:outline-erp-navy",
                    isActive
                      ? "bg-erp-navy text-white font-bold"
                      : "bg-erp-surface text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-text-main"
                  )}
                >
                  <span>{lang.label}</span>
                  {hasContent && (
                    <span
                      className={cn(
                        "w-1.5 h-1.5 rounded-none",
                        isActive ? "bg-white" : "bg-erp-navy"
                      )}
                      aria-hidden="true"
                    />
                  )}
                </button>
              );
            })}
          </div>
        </div>
      </div>

      <div className="erp-input-wrapper">
        {type === "textarea" ? (
          <textarea
            id={generatedId}
            value={safeValue[activeLang] || ""}
            onChange={(e) => handleChange(e.target.value)}
            placeholder={currentPlaceholder}
            required={required && activeLang === "th"}
            rows={3}
            className={cn(
              "erp-textarea",
              error && "erp-textarea-error"
            )}
          />
        ) : (
          <input
            id={generatedId}
            type="text"
            value={safeValue[activeLang] || ""}
            onChange={(e) => handleChange(e.target.value)}
            placeholder={currentPlaceholder}
            required={required && activeLang === "th"}
            className={cn(
              "erp-input",
              error && "erp-input-error"
            )}
          />
        )}
      </div>

      {error && (
        <p className="erp-error-text" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}

export default MultiLangInput;
