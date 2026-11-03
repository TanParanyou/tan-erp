import type { FieldSurveyItem } from "./field-survey-schema";
import { resolveNetAreaSqm } from "./field-survey-schema";

/** Codes double as i18n keys under `fieldSurvey.checks.warnings`. */
export type FieldSurveyWarningCode =
  | "noMeasurement"
  | "widthMissing"
  | "depthMissing"
  | "heightMissing"
  | "noPhoto"
  | "widthOutOfRange"
  | "depthOutOfRange"
  | "heightOutOfRange"
  | "areaOutOfRange"
  | "customOutOfRange";

// A type alias (not an interface) so it is accepted as i18n message values
export type MeasurementRange = {
  min: number;
  max: number;
};

/** Plausible ranges per measurement type, supplied by the backend reference data. A missing range skips that check. */
export interface FieldSurveyLimits {
  box?: MeasurementRange;
  area?: MeasurementRange;
  custom?: MeasurementRange;
}

export interface FieldSurveyWarning {
  itemId: string;
  itemIndex: number;
  code: FieldSurveyWarningCode;
  /** Message values for range warnings. */
  values?: MeasurementRange;
}

function isOutOfRange(value: number, range: MeasurementRange | undefined): range is MeasurementRange {
  return range !== undefined && value > 0 && (value < range.min || value > range.max);
}

/**
 * Completeness and plausibility check mirroring what the save step actually persists:
 * zero-valued dimensions are dropped, so an item can be saved with no measurement at all.
 */
export function evaluateFieldSurveyItems(
  items: readonly FieldSurveyItem[],
  limits: FieldSurveyLimits = {}
): FieldSurveyWarning[] {
  const warnings: FieldSurveyWarning[] = [];

  items.forEach((item, itemIndex) => {
    const add = (code: FieldSurveyWarningCode, values?: MeasurementRange) =>
      warnings.push({ itemId: item.id, itemIndex, code, values });

    if (item.measurementType === "box") {
      const { widthMm = 0, depthMm = 0, heightMm = 0 } = item.boxDimensions ?? {};
      if (widthMm <= 0 && depthMm <= 0 && heightMm <= 0) {
        add("noMeasurement");
      } else {
        if (widthMm <= 0) add("widthMissing");
        if (depthMm <= 0) add("depthMissing");
        if (heightMm <= 0) add("heightMissing");
      }
      if (isOutOfRange(widthMm, limits.box)) add("widthOutOfRange", limits.box);
      if (isOutOfRange(depthMm, limits.box)) add("depthOutOfRange", limits.box);
      if (isOutOfRange(heightMm, limits.box)) add("heightOutOfRange", limits.box);
    } else if (item.measurementType === "area") {
      const netArea = item.areaDimensions ? resolveNetAreaSqm(item.areaDimensions) : 0;
      if (netArea <= 0) add("noMeasurement");
      if (isOutOfRange(netArea, limits.area)) add("areaOutOfRange", limits.area);
    } else {
      const points = item.customMeasurements ?? [];
      if (!points.some((point) => point.valueMm > 0)) add("noMeasurement");
      if (points.some((point) => isOutOfRange(point.valueMm, limits.custom))) {
        add("customOutOfRange", limits.custom);
      }
    }

    if (item.photos.length === 0) add("noPhoto");
  });

  return warnings;
}
