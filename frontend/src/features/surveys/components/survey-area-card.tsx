"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller, useWatch } from "react-hook-form";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { IconPlus, IconTrash, IconBox3D } from "@/components/common/Icons";
import { ThreeBoxVisualizer } from "@/components/ui/ThreeBoxVisualizer";
import { SurveyMeasurementRow } from "./survey-measurement-row";
import { useAreaPresets } from "../hooks/use-area-presets";
import { useBoxDimensions } from "../hooks/use-box-dimensions";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyAreaCardProps {
  areaIndex: number;
  isReady: boolean;
  onRemoveArea: () => void;
}

export function SurveyAreaCard({ areaIndex, isReady, onRemoveArea }: SurveyAreaCardProps) {
  const t = useTranslations("surveys");
  const tc = useTranslations("common");
  const locale = useLocale();
  const { control, setValue } = useFormContext<SurveyWorkspaceFormData>();
  const { presets } = useAreaPresets();

  const presetOptions = [
    { value: "", label: t("areaPresetPlaceholder") },
    ...presets.map((p) => ({
      value: p.code,
      label: t(p.labelKey as Parameters<typeof t>[0]),
    })),
  ];

  const handleSelectPreset = (presetCode: string) => {
    if (!presetCode) return;
    const found = presets.find((p) => p.code === presetCode);
    if (found) {
      const roomName = locale === "en" ? found.defaultNameEn : found.defaultNameTh;
      setValue(`areas.${areaIndex}.name`, roomName, {
        shouldDirty: true,
        shouldValidate: true,
      });
    }
  };

  const [show3D, setShow3D] = useState(false);

  const watchedMeasurements = useWatch({
    control,
    name: `areas.${areaIndex}.measurements`,
  });

  const areaName = useWatch({
    control,
    name: `areas.${areaIndex}.name`,
  });

  const dimensions = useBoxDimensions(watchedMeasurements);

  const { fields, append, remove } = useFieldArray({
    control,
    name: `areas.${areaIndex}.measurements`,
  });

  const handleAddMeasurement = () => {
    append({
      measurementType: "length",
      value: 1,
      unitCode: "m",
      captureMethod: "measured",
      notes: "",
      sortOrder: fields.length + 1,
    });
  };

  return (
    <div className="p-3 sm:p-4 border border-erp-border bg-erp-surface shadow-xs">
      {/* Area Header: Stacked on Mobile, Row on Desktop */}
      <div className="mb-4 pb-3 border-b border-erp-border space-y-2.5">
        {/* Top bar: Area Code + Delete Button */}
        <div className="flex items-center justify-between gap-3">
          <div className="flex items-center gap-2">
            <span className="text-xs font-bold text-erp-text-muted uppercase">
              {t("areaCodeLabel")}
            </span>
            <Controller
              control={control}
              name={`areas.${areaIndex}.code`}
              render={({ field }) => (
                <Input
                  className="w-28 font-mono font-bold text-sm min-h-[40px]"
                  value={field.value}
                  placeholder={t("areaCodeLabel")}
                  onChange={field.onChange}
                  disabled={isReady}
                />
              )}
            />
          </div>

          {!isReady && (
            <button
              type="button"
              onClick={onRemoveArea}
              className="min-h-[40px] min-w-[40px] flex items-center justify-center text-erp-danger hover:text-red-700 hover:bg-erp-danger-bg border border-erp-danger-border transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
              title={t("removeAreaAction")}
              aria-label={t("removeAreaAction")}
            >
              <IconTrash size={18} strokeWidth={2} />
            </button>
          )}
        </div>

        {/* Name input & Preset Select (Stacked on mobile, 2-col on sm+) */}
        <div className="grid grid-cols-1 sm:grid-cols-12 gap-2.5">
          {!isReady && (
            <div className="sm:col-span-5">
              <Select
                options={presetOptions}
                value=""
                onChange={(e) => handleSelectPreset(e.target.value)}
                placeholder={t("areaPresetPlaceholder")}
                className="w-full text-xs min-h-[44px]"
                disabled={isReady}
                aria-label={t("areaPresetLabel")}
              />
            </div>
          )}
          <div className={isReady ? "col-span-12" : "sm:col-span-7"}>
            <Controller
              control={control}
              name={`areas.${areaIndex}.name`}
              render={({ field }) => (
                <Input
                  className="w-full font-semibold text-sm min-h-[44px]"
                  value={field.value}
                  placeholder={t("areaNameLabel")}
                  onChange={field.onChange}
                  disabled={isReady}
                />
              )}
            />
          </div>
        </div>
      </div>

      {/* Measurements Section */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <div className="text-xs font-bold text-erp-text-muted uppercase tracking-wider">
            {t("measurementsTitle")}
          </div>

          {/* 3D Visualizer Toggle Button */}
          <button
            type="button"
            onClick={() => setShow3D((prev) => !prev)}
            className={`flex items-center gap-1.5 px-2.5 py-1 text-xs font-semibold border transition-colors cursor-pointer ${
              show3D
                ? "bg-erp-navy text-white border-erp-navy"
                : "bg-erp-surface hover:bg-erp-surface-subtle text-erp-navy border-erp-border"
            }`}
            title={show3D ? t("visualizer3D.toggleClose") : t("visualizer3D.toggleOpen")}
          >
            <IconBox3D size={14} strokeWidth={2} />
            <span>{show3D ? t("visualizer3D.toggleClose") : t("visualizer3D.toggleOpen")}</span>
          </button>
        </div>

        {/* 3D Box Visualizer Panel */}
        {show3D && (
          <div className="mb-3">
            <ThreeBoxVisualizer
              widthMm={dimensions.widthMm}
              lengthMm={dimensions.lengthMm}
              heightMm={dimensions.heightMm}
              title={areaName || t("areaNameLabel")}
              onClose={() => setShow3D(false)}
              warningMessage={
                !dimensions.hasCustomWidth || !dimensions.hasCustomLength || !dimensions.hasCustomHeight
                  ? t("visualizer3D.noDimensionWarning")
                  : undefined
              }
            />
          </div>
        )}

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm border-collapse">
            <thead className="hidden md:table-header-group">
              <tr className="bg-erp-surface-subtle text-erp-text-body text-xs font-semibold">
                <th className="p-2 border border-erp-border w-36">
                  {t("measurementTypeLabel")}
                </th>
                <th className="p-2 border border-erp-border w-56">
                  {t("measurementComboLabel")}
                </th>
                <th className="p-2 border border-erp-border">
                  {t("measurementNotesLabel")}
                </th>
                {!isReady && (
                  <th className="p-2 border border-erp-border w-14 text-center">
                    {tc("actions.delete")}
                  </th>
                )}
              </tr>
            </thead>
            <tbody>
              {fields.map((fieldItem, mIdx) => (
                <SurveyMeasurementRow
                  key={fieldItem.id}
                  areaIndex={areaIndex}
                  measurementIndex={mIdx}
                  isReady={isReady}
                  onRemove={() => remove(mIdx)}
                />
              ))}
            </tbody>
          </table>
        </div>

        {!isReady && (
          <div className="pt-2">
            <Button
              variant="secondary"
              size="sm"
              type="button"
              onClick={handleAddMeasurement}
              className="flex items-center gap-1.5 w-full sm:w-auto justify-center min-h-[44px]"
            >
              <IconPlus size={14} strokeWidth={2} />
              <span>{t("addMeasurementAction")}</span>
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}
