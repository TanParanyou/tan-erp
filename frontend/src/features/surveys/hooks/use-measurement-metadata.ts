export interface MeasurementTypeOption {
  code: string;
  labelKey: string;
  defaultUnit: string;
}

export interface MeasurementUnitOption {
  code: string;
  labelKey: string;
  symbol: string;
  toMmFactor: number | null; // null if not a length dimension (e.g. sqm, unit)
}

export const BASELINE_MEASUREMENT_TYPES: MeasurementTypeOption[] = [
  { code: "width", labelKey: "types.width", defaultUnit: "m" },
  { code: "height", labelKey: "types.height", defaultUnit: "m" },
  { code: "depth", labelKey: "types.depth", defaultUnit: "m" },
  { code: "length", labelKey: "types.length", defaultUnit: "m" },
  { code: "area", labelKey: "types.area", defaultUnit: "sqm" },
  { code: "opening", labelKey: "types.opening", defaultUnit: "m" },
  { code: "count", labelKey: "types.count", defaultUnit: "unit" },
  { code: "custom", labelKey: "types.custom", defaultUnit: "m" },
];

export const BASELINE_MEASUREMENT_UNITS: MeasurementUnitOption[] = [
  { code: "m", labelKey: "units.m", symbol: "m", toMmFactor: 1000 },
  { code: "cm", labelKey: "units.cm", symbol: "cm", toMmFactor: 10 },
  { code: "mm", labelKey: "units.mm", symbol: "mm", toMmFactor: 1 },
  { code: "sqm", labelKey: "units.sqm", symbol: "sqm", toMmFactor: null },
  { code: "unit", labelKey: "units.unit", symbol: "unit", toMmFactor: null },
];

export interface ConversionResult {
  convertedValue: number | null;
  formattedText: string;
}

/**
 * Converts length measurement to millimeters (mm) in real-time.
 * In interior architecture & joinery fabrication, millimeter (mm) is the authoritative shop standard.
 */
export function convertMeasurementToMm(value: number, unitCode: string): ConversionResult {
  if (typeof value !== "number" || isNaN(value) || value <= 0) {
    return { convertedValue: null, formattedText: "-" };
  }

  const unit = BASELINE_MEASUREMENT_UNITS.find((u) => u.code === unitCode);
  if (!unit || unit.toMmFactor === null) {
    if (unitCode === "sqm") {
      return { convertedValue: null, formattedText: `${value.toLocaleString()} m²` };
    }
    return { convertedValue: null, formattedText: `${value.toLocaleString()} ${unitCode}` };
  }

  const mmValue = Math.round(value * unit.toMmFactor * 100) / 100;
  return {
    convertedValue: mmValue,
    formattedText: `${mmValue.toLocaleString()} mm`,
  };
}

/**
 * Returns current local date and time formatted for HTML datetime-local input (YYYY-MM-DDTHH:mm).
 */
export function getDefaultVisitDateTime(): string {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, "0");
  const day = String(now.getDate()).padStart(2, "0");
  const hours = String(now.getHours()).padStart(2, "0");
  const minutes = String(now.getMinutes()).padStart(2, "0");
  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

/**
 * Master Data hook for measurements.
 * Prepared for future API call integration (e.g. useQuery from TanStack Query).
 */
export function useMeasurementMetadata() {
  // In future, this can wrap useQuery(["measurement-metadata"], fetchMeasurementMetadata)
  return {
    measurementTypes: BASELINE_MEASUREMENT_TYPES,
    units: BASELINE_MEASUREMENT_UNITS,
    getUnitByCode: (code: string) => BASELINE_MEASUREMENT_UNITS.find((u) => u.code === code),
    getTypeByCode: (code: string) => BASELINE_MEASUREMENT_TYPES.find((t) => t.code === code),
  };
}
