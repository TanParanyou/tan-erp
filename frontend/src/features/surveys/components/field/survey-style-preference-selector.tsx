"use client";

/* eslint-disable no-restricted-syntax -- token-exempt: fallback mood-tone swatch palettes are data, not theme colours */

import React from "react";
import { useTranslations } from "next-intl";
import { IconCheckCircle } from "@/components/common/Icons";
import { Input } from "@/components/ui/Input";
import { DesignStyleThumbnail } from "../settings/design-style-thumbnail";

export interface SurveyDesignStyleOption {
  code: string;
  label: string;
  description: string;
  imageFileId?: string | null;
  imageFileIds?: (string | null | undefined)[] | null;
}

export interface SurveyMoodToneOption {
  code: string;
  label: string;
  description: string;
  colorPalette: readonly string[];
}

export interface SurveyStylePreferenceSelectorProps {
  designStyles: SurveyDesignStyleOption[];
  moodTones: SurveyMoodToneOption[];
  selectedDesignStyle: string;
  selectedMoodTone: string;
  onDesignStyleChange: (code: string) => void;
  onMoodToneChange: (code: string) => void;
  customStyleNotes?: string;
  onCustomStyleNotesChange?: (notes: string) => void;
  disabled?: boolean;
  className?: string;
}

/**
 * Reusable SurveyStylePreferenceSelector component
 * Allows selecting customer's preferred Design Style (with architectural illustrations / photos)
 * and Mood & Tone (with color swatches) and optional custom style notes.
 */
export function SurveyStylePreferenceSelector({
  designStyles,
  moodTones,
  selectedDesignStyle,
  selectedMoodTone,
  onDesignStyleChange,
  onMoodToneChange,
  customStyleNotes,
  onCustomStyleNotesChange,
  disabled = false,
  className = "space-y-4",
}: SurveyStylePreferenceSelectorProps) {
  const t = useTranslations("fieldSurvey");

  return (
    <div className={className}>
      {/* Design Style */}
      <div>
        <label className="erp-label mb-2 block font-medium text-erp-text-main">
          {t("style.designStyleLabel")}
        </label>
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
          {designStyles.map((style) => {
            const isSelected = selectedDesignStyle === style.code;
            return (
              <button
                key={style.code}
                type="button"
                onClick={() => onDesignStyleChange(isSelected ? "" : style.code)}
                disabled={disabled}
                className={`group flex flex-col items-start border p-2 text-left transition-all ${
                  isSelected
                    ? "border-erp-navy bg-erp-navy text-white shadow-sm ring-1 ring-erp-navy"
                    : "border-erp-border bg-white text-erp-text-main hover:border-erp-navy/50 hover:bg-erp-surface-subtle"
                }`}
              >
                {/* Design Style Artwork / Uploaded Photo Thumbnail */}
                <div className="relative mb-2 w-full aspect-16/10 overflow-hidden border border-erp-border bg-erp-surface-subtle">
                  <DesignStyleThumbnail
                    code={style.code}
                    imageFileId={style.imageFileId}
                    imageFileIds={style.imageFileIds}
                    name={style.label}
                    size="full"
                    allowLightbox={false}
                    className="w-full h-full border-0 shadow-none"
                  />
                  {isSelected && (
                    <div className="absolute top-1 right-1 flex items-center justify-center w-5 h-5 bg-erp-navy text-white border border-white/20 shadow-xs">
                      <IconCheckCircle size={14} className="text-white" />
                    </div>
                  )}
                </div>

                <span className="text-xs font-semibold leading-tight line-clamp-1">{style.label}</span>
                {style.description ? (
                  <span
                    className={`mt-1 line-clamp-2 text-[10px] leading-snug ${
                      isSelected ? "text-white/80" : "text-erp-text-muted"
                    }`}
                  >
                    {style.description}
                  </span>
                ) : null}
              </button>
            );
          })}
        </div>
      </div>

      {/* Mood & Tone with Color Swatches */}
      <div>
        <label className="erp-label mb-2 block font-medium text-erp-text-main">
          {t("style.moodToneLabel")}
        </label>
        <div className="grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-5">
          {moodTones.map((tone) => {
            const isSelected = selectedMoodTone === tone.code;
            return (
              <button
                key={tone.code}
                type="button"
                onClick={() => onMoodToneChange(isSelected ? "" : tone.code)}
                disabled={disabled}
                className={`flex flex-col border p-2.5 text-left transition-colors ${
                  isSelected
                    ? "border-erp-navy bg-erp-navy text-white shadow-sm"
                    : "border-erp-border bg-white text-erp-text-main hover:border-erp-navy/50 hover:bg-erp-surface-subtle"
                }`}
              >
                {tone.colorPalette && tone.colorPalette.length > 0 ? (
                  <div className="mb-2 flex h-3 w-full border border-black/10 overflow-hidden">
                    {tone.colorPalette.map((color, cIdx) => (
                      <div
                        key={cIdx}
                        className="h-full flex-1"
                        style={{ backgroundColor: color }}
                      />
                    ))}
                  </div>
                ) : null}
                <span className="text-xs font-semibold">{tone.label}</span>
                {tone.description ? (
                  <span
                    className={`mt-1 line-clamp-2 text-[10px] ${
                      isSelected ? "text-white/80" : "text-erp-text-muted"
                    }`}
                  >
                    {tone.description}
                  </span>
                ) : null}
              </button>
            );
          })}
        </div>
      </div>

      {/* Style & Color Notes */}
      {onCustomStyleNotesChange !== undefined && (
        <div>
          <Input
            label={t("style.customStyleNotes")}
            value={customStyleNotes ?? ""}
            onChange={(e) => onCustomStyleNotesChange(e.target.value)}
            disabled={disabled}
            placeholder={t("style.customStyleNotesPlaceholder")}
            wrapperClassName="!mb-0"
            className="text-xs"
          />
        </div>
      )}
    </div>
  );
}
