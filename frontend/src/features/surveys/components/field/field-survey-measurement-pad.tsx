"use client";

import React from "react";
import { useTranslations } from "next-intl";
import {
  type FieldSurveyMeasurementType,
  type BoxDimensions,
  type AreaDimensions,
  type CustomMeasurementPoint,
  type AreaDeduction,
  calculateNetAreaSqm,
} from "../../schemas/field-survey-schema";
import { IconPlus, IconTrash } from "@/components/common/Icons";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

export interface FieldSurveyMeasurementPadProps {
  measurementType: FieldSurveyMeasurementType;
  boxDimensions?: BoxDimensions;
  areaDimensions?: AreaDimensions;
  customMeasurements?: readonly CustomMeasurementPoint[];
  onTypeChange: (type: FieldSurveyMeasurementType) => void;
  onBoxChange?: (dimensions: BoxDimensions) => void;
  onAreaChange?: (dimensions: AreaDimensions) => void;
  onCustomChange?: (points: CustomMeasurementPoint[]) => void;
  disabled?: boolean;
}

export function FieldSurveyMeasurementPad({
  measurementType,
  boxDimensions = { widthMm: 0, depthMm: 0, heightMm: 0 },
  areaDimensions = { grossWidthMm: 0, grossLengthMm: 0, deductions: [] },
  customMeasurements = [],
  onTypeChange,
  onBoxChange,
  onAreaChange,
  onCustomChange,
  disabled = false,
}: FieldSurveyMeasurementPadProps) {
  const t = useTranslations("fieldSurvey");

  // Box Handlers
  const handleBoxFieldChange = (field: keyof BoxDimensions, valStr: string) => {
    const num = Math.max(0, Number(valStr) || 0);
    onBoxChange?.({
      ...boxDimensions,
      [field]: num,
    });
  };

  // Area Handlers
  const handleAreaDimensionChange = (field: "grossWidthMm" | "grossLengthMm", valStr: string) => {
    const num = Math.max(0, Number(valStr) || 0);
    onAreaChange?.({
      ...areaDimensions,
      [field]: num,
    });
  };

  const handleAddDeduction = () => {
    const nextDeductions: AreaDeduction[] = [
      ...areaDimensions.deductions,
      { label: "", areaSqm: 0 },
    ];
    onAreaChange?.({
      ...areaDimensions,
      deductions: nextDeductions,
    });
  };

  const handleUpdateDeduction = (index: number, updated: Partial<AreaDeduction>) => {
    const nextDeductions = areaDimensions.deductions.map((d, i) =>
      i === index ? { ...d, ...updated } : d
    );
    onAreaChange?.({
      ...areaDimensions,
      deductions: nextDeductions,
    });
  };

  const handleRemoveDeduction = (index: number) => {
    const nextDeductions = areaDimensions.deductions.filter((_, i) => i !== index);
    onAreaChange?.({
      ...areaDimensions,
      deductions: nextDeductions,
    });
  };

  // Custom Point Handlers
  const handleAddCustomPoint = () => {
    const nextPoints: CustomMeasurementPoint[] = [
      ...customMeasurements,
      { label: "", valueMm: 0 },
    ];
    onCustomChange?.(nextPoints);
  };

  const handleUpdateCustomPoint = (index: number, updated: Partial<CustomMeasurementPoint>) => {
    const nextPoints = customMeasurements.map((p, i) =>
      i === index ? { ...p, ...updated } : p
    );
    onCustomChange?.(nextPoints);
  };

  const handleRemoveCustomPoint = (index: number) => {
    const nextPoints = customMeasurements.filter((_, i) => i !== index);
    onCustomChange?.(nextPoints);
  };

  const grossAreaSqm =
    Math.round(((areaDimensions.grossWidthMm / 1000) * (areaDimensions.grossLengthMm / 1000)) * 100) / 100;
  const netAreaSqm = calculateNetAreaSqm(
    areaDimensions.grossWidthMm,
    areaDimensions.grossLengthMm,
    areaDimensions.deductions
  );

  return (
    <div className="border border-erp-border bg-white p-4">
      <div className="mb-3">
        <label className="mb-1.5 block text-xs font-semibold text-erp-navy">
          {t("measurementType")}
        </label>
        {/* Segmented Control */}
        <div className="grid grid-cols-1 gap-1 sm:grid-cols-3">
          <button
            type="button"
            disabled={disabled}
            onClick={() => onTypeChange("box")}
            className={`min-h-[44px] px-3 py-2 text-xs font-semibold transition-colors ${
              measurementType === "box"
                ? "border border-erp-navy bg-erp-navy text-white"
                : "border border-erp-border bg-erp-surface-subtle text-erp-text-main hover:bg-erp-surface"
            }`}
          >
            {t("measurementTypes.box")}
          </button>
          <button
            type="button"
            disabled={disabled}
            onClick={() => onTypeChange("area")}
            className={`min-h-[44px] px-3 py-2 text-xs font-semibold transition-colors ${
              measurementType === "area"
                ? "border border-erp-navy bg-erp-navy text-white"
                : "border border-erp-border bg-erp-surface-subtle text-erp-text-main hover:bg-erp-surface"
            }`}
          >
            {t("measurementTypes.area")}
          </button>
          <button
            type="button"
            disabled={disabled}
            onClick={() => onTypeChange("custom")}
            className={`min-h-[44px] px-3 py-2 text-xs font-semibold transition-colors ${
              measurementType === "custom"
                ? "border border-erp-navy bg-erp-navy text-white"
                : "border border-erp-border bg-erp-surface-subtle text-erp-text-main hover:bg-erp-surface"
            }`}
          >
            {t("measurementTypes.custom")}
          </button>
        </div>
      </div>

      {/* Mode 1: 3D Box Dimensions */}
      {measurementType === "box" && (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
          <Input
            label={`${t("dimensions.width")} (${t("dimensions.unitMm")})`}
            type="number"
            inputMode="numeric"
            disabled={disabled}
            value={boxDimensions.widthMm || ""}
            onChange={(e) => handleBoxFieldChange("widthMm", e.target.value)}
            placeholder="0"
            wrapperClassName="!mb-0"
            className="text-sm font-semibold text-erp-navy"
          />
          <Input
            label={`${t("dimensions.depth")} (${t("dimensions.unitMm")})`}
            type="number"
            inputMode="numeric"
            disabled={disabled}
            value={boxDimensions.depthMm || ""}
            onChange={(e) => handleBoxFieldChange("depthMm", e.target.value)}
            placeholder="0"
            wrapperClassName="!mb-0"
            className="text-sm font-semibold text-erp-navy"
          />
          <Input
            label={`${t("dimensions.height")} (${t("dimensions.unitMm")})`}
            type="number"
            inputMode="numeric"
            disabled={disabled}
            value={boxDimensions.heightMm || ""}
            onChange={(e) => handleBoxFieldChange("heightMm", e.target.value)}
            placeholder="0"
            wrapperClassName="!mb-0"
            className="text-sm font-semibold text-erp-navy"
          />
        </div>
      )}

      {/* Mode 2: Surface Area with Deductions */}
      {measurementType === "area" && (
        <div className="space-y-4">
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <Input
              label={`${t("dimensions.grossWidth")} (${t("dimensions.unitMm")})`}
              type="number"
              inputMode="numeric"
              disabled={disabled}
              value={areaDimensions.grossWidthMm || ""}
              onChange={(e) => handleAreaDimensionChange("grossWidthMm", e.target.value)}
              placeholder="0"
              wrapperClassName="!mb-0"
              className="text-sm font-semibold text-erp-navy"
            />
            <Input
              label={`${t("dimensions.grossLength")} (${t("dimensions.unitMm")})`}
              type="number"
              inputMode="numeric"
              disabled={disabled}
              value={areaDimensions.grossLengthMm || ""}
              onChange={(e) => handleAreaDimensionChange("grossLengthMm", e.target.value)}
              placeholder="0"
              wrapperClassName="!mb-0"
              className="text-sm font-semibold text-erp-navy"
            />
          </div>

          {/* Area Summary Banner */}
          <div className="flex flex-wrap items-center justify-between border-l-4 border-erp-navy bg-erp-surface-subtle p-3 text-xs">
            <div>
              <span className="text-erp-text-muted">{t("dimensions.grossArea")}: </span>
              <span className="font-semibold text-erp-text-main">
                {grossAreaSqm} {t("dimensions.unitSqm")}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted">{t("dimensions.netArea")}: </span>
              <span className="text-sm font-bold text-erp-navy">
                {netAreaSqm}
              </span>
              <span className="ml-1 font-semibold text-erp-navy">{t("dimensions.unitSqm")}</span>
            </div>
          </div>

          {/* Deductions List */}
          <div>
            <div className="mb-2 flex items-center justify-between">
              <span className="text-xs font-semibold text-erp-text-main">
                {t("dimensions.deductions")}
              </span>
              <Button
                type="button"
                variant="outline"
                size="sm"
                disabled={disabled}
                onClick={handleAddDeduction}
                icon={<IconPlus size={14} />}
              >
                {t("dimensions.addDeduction")}
              </Button>
            </div>

            {areaDimensions.deductions.length > 0 && (
              <div className="space-y-2">
                {areaDimensions.deductions.map((d, index) => (
                  <div
                    key={index}
                    className="flex flex-col gap-2 border border-erp-border bg-erp-surface-subtle p-2 sm:flex-row sm:items-center"
                  >
                    <Input
                      disabled={disabled}
                      value={d.label}
                      onChange={(e) => handleUpdateDeduction(index, { label: e.target.value })}
                      placeholder={t("dimensions.deductionLabelPlaceholder")}
                      wrapperClassName="!mb-0 flex-1"
                      className="text-xs"
                    />
                    <div className="flex items-center gap-2">
                      <Input
                        type="number"
                        inputMode="decimal"
                        step="0.01"
                        disabled={disabled}
                        value={d.areaSqm || ""}
                        onChange={(e) =>
                          handleUpdateDeduction(index, {
                            areaSqm: Math.max(0, Number(e.target.value) || 0),
                          })
                        }
                        placeholder="0.00"
                        wrapperClassName="!mb-0 w-28"
                        className="text-xs font-semibold text-erp-navy"
                      />
                      <span className="text-xs text-erp-text-muted">
                        {t("dimensions.unitSqm")}
                      </span>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        disabled={disabled}
                        onClick={() => handleRemoveDeduction(index)}
                        className="text-erp-danger hover:bg-erp-danger-bg"
                        aria-label={t("dimensions.removeDeduction")}
                      >
                        <IconTrash size={16} />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Mode 3: Custom Point Measurements */}
      {measurementType === "custom" && (
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <span className="text-xs font-semibold text-erp-text-main">
              {t("dimensions.customPoints")}
            </span>
            <Button
              type="button"
              variant="outline"
              size="sm"
              disabled={disabled}
              onClick={handleAddCustomPoint}
              icon={<IconPlus size={14} />}
            >
              {t("dimensions.addCustomPoint")}
            </Button>
          </div>

          {customMeasurements.length === 0 ? (
            <p className="border border-dashed border-erp-border p-4 text-center text-xs text-erp-text-muted">
              {t("dimensions.pointLabelPlaceholder")}
            </p>
          ) : (
            <div className="space-y-2">
              {customMeasurements.map((p, index) => (
                <div
                  key={index}
                  className="space-y-2 border border-erp-border bg-erp-surface-subtle p-3"
                >
                  <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
                    <Input
                      disabled={disabled}
                      value={p.label}
                      onChange={(e) => handleUpdateCustomPoint(index, { label: e.target.value })}
                      placeholder={t("dimensions.pointLabelPlaceholder")}
                      wrapperClassName="!mb-0 flex-1"
                      className="text-xs font-medium"
                    />
                    <div className="flex items-center gap-2">
                      <Input
                        type="number"
                        inputMode="numeric"
                        disabled={disabled}
                        value={p.valueMm || ""}
                        onChange={(e) =>
                          handleUpdateCustomPoint(index, {
                            valueMm: Math.max(0, Number(e.target.value) || 0),
                          })
                        }
                        placeholder="0"
                        wrapperClassName="!mb-0 w-32"
                        className="text-xs font-bold text-erp-navy"
                      />
                      <span className="text-xs text-erp-text-muted">
                        {t("dimensions.unitMm")}
                      </span>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        disabled={disabled}
                        onClick={() => handleRemoveCustomPoint(index)}
                        className="text-erp-danger hover:bg-erp-danger-bg"
                        aria-label={t("dimensions.removePoint")}
                      >
                        <IconTrash size={16} />
                      </Button>
                    </div>
                  </div>
                  <Input
                    disabled={disabled}
                    value={p.notes ?? ""}
                    onChange={(e) => handleUpdateCustomPoint(index, { notes: e.target.value })}
                    placeholder={t("dimensions.pointNotesPlaceholder")}
                    wrapperClassName="!mb-0 w-full"
                    className="text-xs text-erp-text-muted"
                  />
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
