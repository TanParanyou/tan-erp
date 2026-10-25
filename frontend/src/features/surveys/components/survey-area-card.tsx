"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray, Controller, useWatch } from "react-hook-form";
import { useTranslations, useLocale } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { Table, TableHeader, TableBody, TableRow, TableHead } from "@/components/ui/Table";
import { IconPlus, IconTrash, IconBox3D, IconChevronDown } from "@/components/common/Icons";
import { ThreeBoxVisualizer } from "@/components/ui/ThreeBoxVisualizer";
import { AttachmentList } from "@/components/forms/AttachmentList";
import { ComboboxInput, type ComboboxInputOption } from "@/components/forms/ComboboxInput";
import { cn } from "@/lib/utils/cn";
import { SurveyMeasurementRow } from "./survey-measurement-row";
import { useAreaPresets } from "../hooks/use-area-presets";
import { useBoxDimensions } from "../hooks/use-box-dimensions";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyAreaCardProps {
  areaIndex: number;
  isReady: boolean;
  /** True when the revision's template hashes area evidence (hash version v4). */
  areaEvidenceEnabled: boolean;
  onRemoveArea: () => void;
  isExpanded?: boolean;
  onToggleExpand?: () => void;
}

export function SurveyAreaCard({
  areaIndex,
  isReady,
  areaEvidenceEnabled,
  onRemoveArea,
  isExpanded: controlledExpanded,
  onToggleExpand,
}: SurveyAreaCardProps) {
  const t = useTranslations("surveys");
  const tc = useTranslations("common");
  const locale = useLocale();
  const { control, setValue } = useFormContext<SurveyWorkspaceFormData>();
  const { presets } = useAreaPresets();

  const [internalExpanded, setInternalExpanded] = useState(true);
  const isExpanded = controlledExpanded ?? internalExpanded;
  const toggleExpand = onToggleExpand ?? (() => setInternalExpanded((prev) => !prev));

  const presetOptions: ComboboxInputOption[] = presets.map((p) => ({
    value: p.code,
    label: t(p.labelKey as Parameters<typeof t>[0]),
  }));

  const handleSelectPreset = (option: ComboboxInputOption) => {
    setValue(`areas.${areaIndex}.name`, option.label, {
      shouldDirty: true,
      shouldValidate: true,
    });
  };

  const [show3D, setShow3D] = useState(false);

  const watchedMeasurements = useWatch({
    control,
    name: `areas.${areaIndex}.measurements`,
  });

  const areaId = useWatch({ control, name: `areas.${areaIndex}.id` });

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
      {/* Area Header Bar: Compact, Stable, Never Jumps */}
      <div className="mb-3 pb-3 border-b border-erp-border">
        <div className="flex flex-wrap items-center justify-between gap-2">
          {/* Left Block: Toggle, Code, Name, Preset */}
          <div className="flex items-center gap-2 flex-1 min-w-[280px]">
            {/* Toggle Expand/Collapse */}
            <button
              type="button"
              onClick={toggleExpand}
              className="h-9 w-9 shrink-0 flex items-center justify-center text-erp-text-muted hover:text-erp-navy hover:bg-erp-surface-subtle border border-erp-border transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
              title={t("toggleArea")}
              aria-label={t("toggleArea")}
              aria-expanded={isExpanded}
            >
              <IconChevronDown
                size={18}
                strokeWidth={2}
                className={cn("transition-transform duration-150", isExpanded ? "" : "-rotate-90")}
              />
            </button>

            {/* Area Code */}
            <Controller
              control={control}
              name={`areas.${areaIndex}.code`}
              render={({ field }) => (
                <Input
                  wrapperClassName="!mb-0 shrink-0"
                  className="w-24 font-mono font-bold text-xs h-9 min-h-[36px]"
                  value={field.value}
                  placeholder={t("areaCodeLabel")}
                  onChange={field.onChange}
                  disabled={isReady}
                  aria-label={t("areaCodeLabel")}
                />
              )}
            />

            {/* Area Name with Presets Combobox (Single unified input) */}
            <Controller
              control={control}
              name={`areas.${areaIndex}.name`}
              render={({ field }) => (
                <ComboboxInput
                  className="flex-1 min-w-[160px]"
                  inputClassName="font-bold text-sm h-9 min-h-[36px]"
                  value={field.value}
                  onChange={field.onChange}
                  options={presetOptions}
                  onSelectOption={handleSelectPreset}
                  disabled={isReady}
                  placeholder={t("areaNameLabel")}
                  aria-label={t("areaNameLabel")}
                />
              )}
            />
          </div>

          {/* Right Block: Stats badge + Delete */}
          <div className="flex items-center gap-1.5 shrink-0 ml-auto">
            <span className="px-2 py-1 font-mono text-[11px] font-semibold border border-erp-border bg-erp-surface-subtle text-erp-text-muted">
              {t("measurementsSummary", { count: fields.length })}
            </span>

            {!isReady && (
              <button
                type="button"
                onClick={onRemoveArea}
                className="h-9 w-9 shrink-0 flex items-center justify-center text-erp-danger hover:text-red-700 hover:bg-erp-danger-bg border border-erp-danger-border transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
                title={t("removeAreaAction")}
                aria-label={t("removeAreaAction")}
              >
                <IconTrash size={16} strokeWidth={2} />
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Measurements & Area Evidence Section - Hidden when collapsed to keep DOM inputs intact */}
      <div className={cn("space-y-4", !isExpanded && "hidden")}>
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

          <Table>
            <TableHeader className="hidden md:table-header-group">
              <TableRow className="bg-erp-surface-subtle text-erp-text-body text-xs font-semibold">
                <TableHead className="p-2 border border-erp-border w-36">
                  {t("measurementTypeLabel")}
                </TableHead>
                <TableHead className="p-2 border border-erp-border w-56">
                  {t("measurementComboLabel")}
                </TableHead>
                <TableHead className="p-2 border border-erp-border">
                  {t("measurementNotesLabel")}
                </TableHead>
                {!isReady && (
                  <TableHead className="p-2 border border-erp-border w-14 text-center">
                    {tc("actions.delete")}
                  </TableHead>
                )}
              </TableRow>
            </TableHeader>
            <TableBody>
              {fields.map((fieldItem, mIdx) => (
                <SurveyMeasurementRow
                  key={fieldItem.id}
                  areaIndex={areaIndex}
                  measurementIndex={mIdx}
                  isReady={isReady}
                  onRemove={() => remove(mIdx)}
                />
              ))}
            </TableBody>
          </Table>

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

        {areaEvidenceEnabled && (
          <div className="mt-4 pt-3 border-t border-erp-border space-y-2">
            <div className="text-xs font-bold text-erp-text-muted uppercase tracking-wider">{t("areaEvidenceTitle")}</div>
            {areaId ? (
              <AttachmentList ownerType="survey-area" ownerId={areaId} canManage={!isReady} purposes={["evidence", "general"]} />
            ) : (
              <p className="text-xs text-erp-text-muted">{t("areaEvidenceSaveFirst")}</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
