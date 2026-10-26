"use client";

import React from "react";
import { useFormContext, Controller, useController } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { TableRow, TableCell } from "@/components/ui/Table";
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

  const { field: captureMethodField } = useController({
    control,
    name: `areas.${areaIndex}.measurements.${measurementIndex}.captureMethod`,
  });
  const isPhotoEstimated = captureMethodField.value === "photo_estimated";

  return (
    <>
      {/* 1. Desktop Table Row View (>= md) */}
      <TableRow className="hidden md:table-row border-b border-erp-border hover:bg-erp-surface-subtle/50 transition-colors">
        {/* Type Select */}
        <TableCell className="py-2 px-2.5 border border-erp-border w-44 align-top">
          <Controller
            control={control}
            name={`areas.${areaIndex}.measurements.${measurementIndex}.measurementType`}
            render={({ field }) => (
              <Select
                value={field.value}
                onChange={field.onChange}
                disabled={isReady}
                wrapperClassName="!mb-0"
                className="text-xs h-9 min-h-[36px]"
                options={measurementTypes.map((type) => ({
                  value: type.code,
                  label: t(type.labelKey as Parameters<typeof t>[0]),
                }))}
              />
            )}
          />
          <div className="mt-1 min-h-[16px] invisible select-none" aria-hidden="true">-</div>
        </TableCell>

        {/* Value + Unit Compound Combo Input */}
        <TableCell className="py-2 px-2.5 border border-erp-border w-56 align-top">
          <MeasurementAmountInput
            value={valueField.value === 0 ? "" : valueField.value}
            onChange={(val) => valueField.onChange(val ?? 0)}
            unitCode={unitField.value || "m"}
            onUnitChange={unitField.onChange}
            unitOptions={units}
            disabled={isReady}
            placeholder={t("measurementValueLabel")}
            className="!mb-0"
            inputClassName="text-xs h-9 min-h-[36px]"
            selectClassName="text-xs h-9 min-h-[36px]"
            showMmConversion={true}
          />
          {isPhotoEstimated && (
            <Badge variant="info" className="mt-1">{t("capturePhotoEstimated")}</Badge>
          )}
        </TableCell>

        {/* Notes Input */}
        <TableCell className="py-2 px-2.5 border border-erp-border align-top">
          <Controller
            control={control}
            name={`areas.${areaIndex}.measurements.${measurementIndex}.notes`}
            render={({ field }) => (
              <Input
                value={field.value ?? ""}
                placeholder={t("measurementNotesLabel")}
                onChange={field.onChange}
                disabled={isReady}
                wrapperClassName="!mb-0"
                className="text-xs h-9 min-h-[36px]"
              />
            )}
          />
          <div className="mt-1 min-h-[16px] invisible select-none" aria-hidden="true">-</div>
        </TableCell>

        {/* Delete Action */}
        {!isReady && (
          <TableCell className="py-2 px-2 border border-erp-border w-12 text-center align-top">
            <div className="h-9 flex items-center justify-center">
              <button
                type="button"
                className="h-8 w-8 inline-flex items-center justify-center text-erp-danger hover:text-red-700 hover:bg-erp-danger-bg p-1.5 transition-colors cursor-pointer"
                onClick={onRemove}
                title={t("removeMeasurementAction")}
                aria-label={t("removeMeasurementAction")}
              >
                <IconTrash size={16} strokeWidth={2} />
              </button>
            </div>
            <div className="mt-1 min-h-[16px] invisible select-none" aria-hidden="true">-</div>
          </TableCell>
        )}
      </TableRow>

      {/* 2. Mobile Stacked Card View (< md) - Spacious & Non-cramped */}
      <TableRow className="md:hidden">
        <TableCell colSpan={isReady ? 3 : 4} className="p-0 border-b border-erp-border">
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
              <Controller
                control={control}
                name={`areas.${areaIndex}.measurements.${measurementIndex}.measurementType`}
                render={({ field }) => (
                  <Select
                    label={t("measurementTypeLabel")}
                    value={field.value}
                    onChange={field.onChange}
                    disabled={isReady}
                    wrapperClassName="!mb-0"
                    options={measurementTypes.map((type) => ({
                      value: type.code,
                      label: t(type.labelKey as Parameters<typeof t>[0]),
                    }))}
                  />
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
          {isPhotoEstimated && (
            <Badge variant="info" className="mt-1">{t("capturePhotoEstimated")}</Badge>
          )}
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
    </TableCell>
  </TableRow>
    </>
  );
}
