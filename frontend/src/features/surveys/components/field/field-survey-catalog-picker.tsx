"use client";

import React, { useId } from "react";
import { useTranslations, useLocale } from "next-intl";
import type { SpecSelection } from "../../schemas/field-survey-schema";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import type { ItemResponse } from "@/lib/api/api-client";

export interface FieldSurveyCatalogPickerProps {
  value?: SpecSelection;
  onChange: (spec: SpecSelection) => void;
  disabled?: boolean;
}

interface QuickPreset {
  label: string;
  coreSkuName?: string;
  finishSkuName?: string;
  hardwareSkuName?: string;
}

const QUICK_PRESETS: readonly QuickPreset[] = [
  {
    label: "ไม้อัดยาง 15mm + ปิดผิวลามิเนต",
    coreSkuName: "ไม้อัดยาง 15 มม. เกรดเฟอร์นิเจอร์",
    finishSkuName: "แผ่นลามิเนต HPL ผิวสัมผัสด้าน",
  },
  {
    label: "ไม้สักทอง + พ่น PU ด้าน",
    coreSkuName: "ไม้สักทองคัดเกรด A อบแห้ง",
    finishSkuName: "พ่นแลคเกอร์ / PU สัมผัสด้าน 50%",
  },
  {
    label: "ปูพื้นหินอ่อน Grigio + ยาแนวตรงเบอร์",
    finishSkuName: "หินอ่อน Grigio Orobico 60x60 ซม.",
  },
  {
    label: "ผ้าม่าน 2 ชั้น + รางไฟฟ้า",
    coreSkuName: "รางม่านไฟฟ้าระบบเงียบ Zigbee",
    finishSkuName: "ผ้าโปร่ง Dim-out + ผ้าทึบกัน UV 100%",
  },
];

export function FieldSurveyCatalogPicker({
  value = {},
  onChange,
  disabled = false,
}: FieldSurveyCatalogPickerProps) {
  const t = useTranslations("fieldSurvey.spec");
  const locale = useLocale();
  const notesId = useId();

  const handleCoreChange = (id: string, item: ItemResponse | null) => {
    const name = item ? (locale === "en" ? item.name?.english : item.name?.thai) : undefined;
    onChange({
      ...value,
      coreSkuId: id || undefined,
      coreSkuName: name,
    });
  };

  const handleFinishChange = (id: string, item: ItemResponse | null) => {
    const name = item ? (locale === "en" ? item.name?.english : item.name?.thai) : undefined;
    onChange({
      ...value,
      finishSkuId: id || undefined,
      finishSkuName: name,
    });
  };

  const handleHardwareChange = (id: string, item: ItemResponse | null) => {
    const name = item ? (locale === "en" ? item.name?.english : item.name?.thai) : undefined;
    onChange({
      ...value,
      hardwareSkuId: id || undefined,
      hardwareSkuName: name,
    });
  };

  const handleNotesChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    onChange({
      ...value,
      customNotes: e.target.value,
    });
  };

  const applyPreset = (preset: QuickPreset) => {
    onChange({
      ...value,
      coreSkuName: preset.coreSkuName ?? value.coreSkuName,
      finishSkuName: preset.finishSkuName ?? value.finishSkuName,
      hardwareSkuName: preset.hardwareSkuName ?? value.hardwareSkuName,
    });
  };

  return (
    <div className="border border-erp-border bg-white p-4">
      <div className="mb-3">
        <h3 className="text-sm font-semibold text-erp-navy">{t("title")}</h3>
        <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
      </div>

      {/* Quick Presets for Mobile Surveyors */}
      <div className="mb-4">
        <span className="mb-1.5 block text-xs font-medium text-erp-text-muted">
          {t("quickSelect")}
        </span>
        <div className="flex flex-wrap gap-2">
          {QUICK_PRESETS.map((preset) => (
            <button
              key={preset.label}
              type="button"
              disabled={disabled}
              onClick={() => applyPreset(preset)}
              className="inline-flex min-h-[36px] items-center border border-erp-border bg-erp-surface-subtle px-2.5 py-1 text-xs font-medium text-erp-text-main transition-colors hover:border-erp-navy hover:bg-erp-surface active:bg-erp-navy active:text-white disabled:cursor-not-allowed disabled:opacity-50"
            >
              {preset.label}
            </button>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        {/* Core Material SKU */}
        <div>
          <label className="mb-1 block text-xs font-semibold text-erp-text-main">
            {t("coreMaterial")}
          </label>
          <ItemAutocomplete
            value={value.coreSkuId ?? null}
            onChange={handleCoreChange}
            disabled={disabled}
            placeholder={value.coreSkuName ?? t("coreMaterial")}
          />
          {value.coreSkuName && (
            <div className="mt-1 flex items-center justify-between border-l-2 border-erp-navy bg-erp-surface-subtle px-2 py-1 text-xs">
              <span className="truncate font-medium text-erp-text-main">{value.coreSkuName}</span>
            </div>
          )}
        </div>

        {/* Surface Finish SKU */}
        <div>
          <label className="mb-1 block text-xs font-semibold text-erp-text-main">
            {t("surfaceFinish")}
          </label>
          <ItemAutocomplete
            value={value.finishSkuId ?? null}
            onChange={handleFinishChange}
            disabled={disabled}
            placeholder={value.finishSkuName ?? t("surfaceFinish")}
          />
          {value.finishSkuName && (
            <div className="mt-1 flex items-center justify-between border-l-2 border-erp-navy bg-erp-surface-subtle px-2 py-1 text-xs">
              <span className="truncate font-medium text-erp-text-main">{value.finishSkuName}</span>
            </div>
          )}
        </div>

        {/* Hardware & Fittings SKU */}
        <div>
          <label className="mb-1 block text-xs font-semibold text-erp-text-main">
            {t("hardware")}
          </label>
          <ItemAutocomplete
            value={value.hardwareSkuId ?? null}
            onChange={handleHardwareChange}
            disabled={disabled}
            placeholder={value.hardwareSkuName ?? t("hardware")}
          />
          {value.hardwareSkuName && (
            <div className="mt-1 flex items-center justify-between border-l-2 border-erp-navy bg-erp-surface-subtle px-2 py-1 text-xs">
              <span className="truncate font-medium text-erp-text-main">
                {value.hardwareSkuName}
              </span>
            </div>
          )}
        </div>
      </div>

      {/* Custom Spec Notes */}
      <div className="mt-4">
        <label
          htmlFor={notesId}
          className="mb-1 block text-xs font-semibold text-erp-text-main"
        >
          {t("customNotes")}
        </label>
        <textarea
          id={notesId}
          value={value.customNotes ?? ""}
          onChange={handleNotesChange}
          disabled={disabled}
          rows={2}
          placeholder={t("customNotesPlaceholder")}
          className="w-full border border-erp-border bg-white p-2.5 text-xs text-erp-text-main focus:border-erp-navy focus:outline-none focus:ring-1 focus:ring-erp-navy disabled:cursor-not-allowed disabled:bg-erp-surface-subtle"
        />
      </div>
    </div>
  );
}
