import { useMemo } from "react";
import { convertMeasurementToMm } from "./use-measurement-metadata";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

export interface BoxDimensionsMm {
  widthMm: number;
  lengthMm: number;
  heightMm: number;
  hasCustomWidth: boolean;
  hasCustomLength: boolean;
  hasCustomHeight: boolean;
}

/**
 * Extracts 3D box dimensions in millimeters (mm) from an area's measurements array.
 * Width (X), Length/Depth (Z), Height (Y).
 * Provides architectural sensible defaults (3.0m x 4.0m x 2.6m) if not yet measured.
 */
export function useBoxDimensions(
  measurements?: SurveyWorkspaceFormData["areas"][number]["measurements"]
): BoxDimensionsMm {
  return useMemo(() => {
    if (!measurements || measurements.length === 0) {
      return {
        widthMm: 3000,
        lengthMm: 4000,
        heightMm: 2600,
        hasCustomWidth: false,
        hasCustomLength: false,
        hasCustomHeight: false,
      };
    }

    let foundWidth: number | null = null;
    let foundLength: number | null = null;
    let foundHeight: number | null = null;

    // Pick the most recent valid measurement for each dimension type
    for (const item of measurements) {
      if (item.value <= 0) continue;
      const mmResult = convertMeasurementToMm(item.value, item.unitCode);
      if (mmResult.convertedValue === null) continue;

      if (item.measurementType === "width") {
        foundWidth = mmResult.convertedValue;
      } else if (item.measurementType === "length" || item.measurementType === "depth") {
        foundLength = mmResult.convertedValue;
      } else if (item.measurementType === "height") {
        foundHeight = mmResult.convertedValue;
      }
    }

    return {
      widthMm: foundWidth ?? 3000,
      lengthMm: foundLength ?? 4000,
      heightMm: foundHeight ?? 2600,
      hasCustomWidth: foundWidth !== null,
      hasCustomLength: foundLength !== null,
      hasCustomHeight: foundHeight !== null,
    };
  }, [measurements]);
}
