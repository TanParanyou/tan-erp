import type { FieldSurveyItem } from "./field-survey-schema";
import { resolveNetAreaSqm } from "./field-survey-schema";

/** Codes double as i18n keys under `fieldSurvey.checks.warnings`. */
export type FieldSurveyWarningCode =
  | "noMeasurement"
  | "widthMissing"
  | "depthMissing"
  | "heightMissing"
  | "noPhoto";

export interface FieldSurveyWarning {
  itemId: string;
  itemIndex: number;
  code: FieldSurveyWarningCode;
}

/**
 * Completeness check mirroring what the save step actually persists:
 * zero-valued dimensions are dropped, so an item can be saved with no measurement at all.
 */
export function evaluateFieldSurveyItems(items: readonly FieldSurveyItem[]): FieldSurveyWarning[] {
  const warnings: FieldSurveyWarning[] = [];

  items.forEach((item, itemIndex) => {
    const add = (code: FieldSurveyWarningCode) => warnings.push({ itemId: item.id, itemIndex, code });

    if (item.measurementType === "box") {
      const { widthMm = 0, depthMm = 0, heightMm = 0 } = item.boxDimensions ?? {};
      if (widthMm <= 0 && depthMm <= 0 && heightMm <= 0) {
        add("noMeasurement");
      } else {
        if (widthMm <= 0) add("widthMissing");
        if (depthMm <= 0) add("depthMissing");
        if (heightMm <= 0) add("heightMissing");
      }
    } else if (item.measurementType === "area") {
      if (!item.areaDimensions || resolveNetAreaSqm(item.areaDimensions) <= 0) add("noMeasurement");
    } else if (!item.customMeasurements?.some((point) => point.valueMm > 0)) {
      add("noMeasurement");
    }

    if (item.photos.length === 0) add("noPhoto");
  });

  return warnings;
}
