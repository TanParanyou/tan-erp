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
          <div>
            <label className="mb-1 block text-xs font-medium text-erp-text-main">
              {t("dimensions.width")} ({t("dimensions.unitMm")})
            </label>
            <input
              type="number"
              inputMode="numeric"
              disabled={disabled}
              value={boxDimensions.widthMm || ""}
              onChange={(e) => handleBoxFieldChange("widthMm", e.target.value)}
              placeholder="0"
              className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-sm font-semibold text-erp-navy focus:border-erp-navy focus:outline-none"
            />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-erp-text-main">
              {t("dimensions.depth")} ({t("dimensions.unitMm")})
            </label>
            <input
              type="number"
              inputMode="numeric"
              disabled={disabled}
              value={boxDimensions.depthMm || ""}
              onChange={(e) => handleBoxFieldChange("depthMm", e.target.value)}
              placeholder="0"
              className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-sm font-semibold text-erp-navy focus:border-erp-navy focus:outline-none"
            />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-erp-text-main">
              {t("dimensions.height")} ({t("dimensions.unitMm")})
            </label>
            <input
              type="number"
              inputMode="numeric"
              disabled={disabled}
              value={boxDimensions.heightMm || ""}
              onChange={(e) => handleBoxFieldChange("heightMm", e.target.value)}
              placeholder="0"
              className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-sm font-semibold text-erp-navy focus:border-erp-navy focus:outline-none"
            />
          </div>
        </div>
      )}

      {/* Mode 2: Surface Area with Deductions */}
      {measurementType === "area" && (
        <div className="space-y-4">
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <div>
              <label className="mb-1 block text-xs font-medium text-erp-text-main">
                {t("dimensions.grossWidth")} ({t("dimensions.unitMm")})
              </label>
              <input
                type="number"
                inputMode="numeric"
                disabled={disabled}
                value={areaDimensions.grossWidthMm || ""}
                onChange={(e) => handleAreaDimensionChange("grossWidthMm", e.target.value)}
                placeholder="0"
                className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-sm font-semibold text-erp-navy focus:border-erp-navy focus:outline-none"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs font-medium text-erp-text-main">
                {t("dimensions.grossLength")} ({t("dimensions.unitMm")})
              </label>
              <input
                type="number"
                inputMode="numeric"
                disabled={disabled}
                value={areaDimensions.grossLengthMm || ""}
                onChange={(e) => handleAreaDimensionChange("grossLengthMm", e.target.value)}
                placeholder="0"
                className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-sm font-semibold text-erp-navy focus:border-erp-navy focus:outline-none"
              />
            </div>
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
              <button
                type="button"
                disabled={disabled}
                onClick={handleAddDeduction}
                className="inline-flex min-h-[36px] items-center gap-1 border border-erp-border bg-white px-2.5 py-1 text-xs font-semibold text-erp-navy hover:bg-erp-surface active:bg-erp-surface-subtle"
              >
                <IconPlus size={14} />
                {t("dimensions.addDeduction")}
              </button>
            </div>

            {areaDimensions.deductions.length > 0 && (
              <div className="space-y-2">
                {areaDimensions.deductions.map((d, index) => (
                  <div
                    key={index}
                    className="flex flex-col gap-2 border border-erp-border bg-erp-surface-subtle p-2 sm:flex-row sm:items-center"
                  >
                    <input
                      type="text"
                      disabled={disabled}
                      value={d.label}
                      onChange={(e) => handleUpdateDeduction(index, { label: e.target.value })}
                      placeholder={t("dimensions.deductionLabelPlaceholder")}
                      className="min-h-[38px] flex-1 border border-erp-border bg-white px-2.5 py-1 text-xs"
                    />
                    <div className="flex items-center gap-2">
                      <input
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
                        className="min-h-[38px] w-28 border border-erp-border bg-white px-2.5 py-1 text-xs font-semibold text-erp-navy"
                      />
                      <span className="text-xs text-erp-text-muted">
                        {t("dimensions.unitSqm")}
                      </span>
                      <button
                        type="button"
                        disabled={disabled}
                        onClick={() => handleRemoveDeduction(index)}
                        className="inline-flex min-h-[38px] min-w-[38px] items-center justify-center border border-erp-border text-erp-danger hover:bg-erp-danger-bg"
                        aria-label="Remove deduction"
                      >
                        <IconTrash size={16} />
                      </button>
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
            <button
              type="button"
              disabled={disabled}
              onClick={handleAddCustomPoint}
              className="inline-flex min-h-[36px] items-center gap-1 border border-erp-border bg-white px-2.5 py-1 text-xs font-semibold text-erp-navy hover:bg-erp-surface active:bg-erp-surface-subtle"
            >
              <IconPlus size={14} />
              {t("dimensions.addCustomPoint")}
            </button>
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
                    <input
                      type="text"
                      disabled={disabled}
                      value={p.label}
                      onChange={(e) => handleUpdateCustomPoint(index, { label: e.target.value })}
                      placeholder={t("dimensions.pointLabelPlaceholder")}
                      className="min-h-[40px] flex-1 border border-erp-border bg-white px-3 py-1.5 text-xs font-medium"
                    />
                    <div className="flex items-center gap-2">
                      <input
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
                        className="min-h-[40px] w-32 border border-erp-border bg-white px-3 py-1.5 text-xs font-bold text-erp-navy"
                      />
                      <span className="text-xs text-erp-text-muted">
                        {t("dimensions.unitMm")}
                      </span>
                      <button
                        type="button"
                        disabled={disabled}
                        onClick={() => handleRemoveCustomPoint(index)}
                        className="inline-flex min-h-[40px] min-w-[40px] items-center justify-center border border-erp-border text-erp-danger hover:bg-erp-danger-bg"
                        aria-label="Remove point"
                      >
                        <IconTrash size={16} />
                      </button>
                    </div>
                  </div>
                  <input
                    type="text"
                    disabled={disabled}
                    value={p.notes ?? ""}
                    onChange={(e) => handleUpdateCustomPoint(index, { notes: e.target.value })}
                    placeholder={t("dimensions.pointNotesPlaceholder")}
                    className="min-h-[36px] w-full border border-erp-border bg-white px-3 py-1 text-xs text-erp-text-muted"
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
