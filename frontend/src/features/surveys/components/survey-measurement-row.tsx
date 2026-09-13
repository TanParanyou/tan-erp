"use client";

import React from "react";
import { useFormContext, Controller, useController } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { IconTrash } from "@/components/common/Icons";
import { MeasurementAmountInput } from "@/components/forms/MeasurementAmountInput";
import { useMeasurementMetadata } from "../hooks/use-measurement-metadata";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyMeasurementRowProps {
  areaIndex: number;
  measurementIndex: number;
  isReady: boolean;
  onRemove: () => void;
}

export function SurveyMeasurementRow({
  areaIndex,
  measurementIndex,
  isReady,
  onRemove,
}: SurveyMeasurementRowProps) {
  const t = useTranslations("surveys");
  const { control } = useFormContext<SurveyWorkspaceFormData>();
  const { measurementTypes, units } = useMeasurementMetadata();

  const { field: valueField } = useController({
    control,
    name: `areas.${areaIndex}.measurements.${measurementIndex}.value`,
  });

  const { field: unitField } = useController({
    control,
    name: `areas.${areaIndex}.measurements.${measurementIndex}.unitCode`,
  });

  return (
    <>
      {/* 1. Desktop Table Row View (>= md) */}
      <tr className="hidden md:table-row border-b border-erp-border hover:bg-erp-surface-subtle/50 transition-colors">
        {/* Type Select */}
        <td className="p-2 border border-erp-border w-36 align-top">
          <Controller
            control={control}
            name={`areas.${areaIndex}.measurements.${measurementIndex}.measurementType`}
            render={({ field }) => (
              <select
                className="erp-select text-xs py-1.5 px-2 w-full bg-erp-surface border-erp-border"
                value={field.value}
                onChange={field.onChange}
                disabled={isReady}
              >
                {measurementTypes.map((type) => (
                  <option key={type.code} value={type.code}>
                    {t(type.labelKey as Parameters<typeof t>[0])}
                  </option>
                ))}
              </select>
            )}
          />
        </td>

        {/* Value + Unit Compound Combo Input */}
        <td className="p-2 border border-erp-border w-56 align-top">
          <MeasurementAmountInput
            value={valueField.value === 0 ? "" : valueField.value}
            onChange={(val) => valueField.onChange(val ?? 0)}
            unitCode={unitField.value || "m"}
            onUnitChange={unitField.onChange}
            unitOptions={units}
            disabled={isReady}
            placeholder={t("measurementValueLabel")}
          />
        </td>

        {/* Notes Input */}
        <td className="p-2 border border-erp-border align-top">
          <Controller
            control={control}
            name={`areas.${areaIndex}.measurements.${measurementIndex}.notes`}
            render={({ field }) => (
              <Input
                value={field.value ?? ""}
                placeholder={t("measurementNotesLabel")}
                onChange={field.onChange}
                disabled={isReady}
              />
            )}
          />
        </td>

        {/* Delete Action */}
        {!isReady && (
          <td className="p-2 border border-erp-border w-14 text-center align-middle">
            <button
              type="button"
              className="inline-flex items-center justify-center text-erp-danger hover:text-red-700 hover:bg-erp-danger-bg p-1.5 transition-colors cursor-pointer"
              onClick={onRemove}
              title={t("removeMeasurementAction")}
              aria-label={t("removeMeasurementAction")}
            >
              <IconTrash size={16} strokeWidth={2} />
            </button>
          </td>
        )}
      </tr>

      {/* 2. Mobile Stacked Card View (< md) - Spacious & Non-cramped */}
      <tr className="md:hidden">
        <td colSpan={isReady ? 3 : 4} className="p-0 border-b border-erp-border">
          <div className="p-3.5 border border-erp-border bg-erp-surface space-y-3 my-2 shadow-xs">
            {/* Header: Badge/Number + Delete */}
            <div className="flex items-center justify-between pb-2 border-b border-erp-border-subtle">
              <span className="text-xs font-bold text-erp-text-muted uppercase tracking-wider">
                {t("measurementCardTitle", { number: measurementIndex + 1 })}
              </span>
              {!isReady && (
                <button
                  type="button"
                  className="min-h-[44px] min-w-[44px] flex items-center justify-center text-erp-danger hover:bg-erp-danger-bg p-2 transition-colors cursor-pointer"
                  onClick={onRemove}
                  title={t("removeMeasurementAction")}
                  aria-label={t("removeMeasurementAction")}
                >
                  <IconTrash size={18} strokeWidth={2} />
                </button>
              )}
            </div>

            {/* Type Dropdown Full Width */}
            <div>
              <label className="block text-xs font-semibold text-erp-text-muted mb-1">
                {t("measurementTypeLabel")}
              </label>
              <Controller
                control={control}
                name={`areas.${areaIndex}.measurements.${measurementIndex}.measurementType`}
                render={({ field }) => (
                  <select
                    className="erp-select text-sm py-2 px-3 w-full bg-erp-surface border-erp-border min-h-[44px]"
                    value={field.value}
                    onChange={field.onChange}
                    disabled={isReady}
                  >
                    {measurementTypes.map((type) => (
                      <option key={type.code} value={type.code}>
                        {t(type.labelKey as Parameters<typeof t>[0])}
                      </option>
                    ))}
                  </select>
                )}
              />
            </div>

            {/* Compound Value + Unit Combo */}
            <div>
              <MeasurementAmountInput
                label={`${t("measurementValueLabel")} & ${t("measurementUnitLabel")}`}
                value={valueField.value === 0 ? "" : valueField.value}
                onChange={(val) => valueField.onChange(val ?? 0)}
                unitCode={unitField.value || "m"}
                onUnitChange={unitField.onChange}
                unitOptions={units}
                disabled={isReady}
                placeholder={t("measurementValueLabel")}
              />
            </div>

            {/* Notes */}
            <div>
              <label className="block text-xs font-semibold text-erp-text-muted mb-1">
                {t("measurementNotesLabel")}
              </label>
              <Controller
                control={control}
                name={`areas.${areaIndex}.measurements.${measurementIndex}.notes`}
                render={({ field }) => (
                  <Input
                    value={field.value ?? ""}
                    placeholder={t("measurementNotesLabel")}
                    onChange={field.onChange}
                    disabled={isReady}
                    className="min-h-[44px] text-sm"
                  />
                )}
              />
            </div>
          </div>
        </td>
      </tr>
    </>
  );
}
