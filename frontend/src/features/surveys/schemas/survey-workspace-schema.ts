import { z } from "zod";

export const surveyMeasurementSchema = z.object({
  id: z.string().nullable().optional(),
  measurementType: z.string(),
  value: z.number().min(0, "Value must be positive"),
  unitCode: z.string(),
  captureMethod: z.string(),
  notes: z.string().nullable().optional(),
  sortOrder: z.number(),
});

export type SurveyMeasurementFormData = z.infer<typeof surveyMeasurementSchema>;

export const surveyAreaSchema = z.object({
  id: z.string().nullable().optional(),
  code: z.string().min(1, "Area code is required"),
  name: z.string(),
  description: z.string().nullable().optional(),
  sortOrder: z.number(),
  measurements: z.array(surveyMeasurementSchema),
});

export type SurveyAreaFormData = z.infer<typeof surveyAreaSchema>;

export const surveyWorkspaceSchema = z.object({
  visitedAt: z.string(),
  scopeSummary: z.string(),
  assumptions: z.array(z.string()),
  constraints: z.array(z.string()),
  missingDetails: z.array(z.string()),
  areas: z.array(surveyAreaSchema),
});

export type SurveyWorkspaceFormData = z.infer<typeof surveyWorkspaceSchema>;

export interface ReadinessValidationResult {
  isValid: boolean;
  errorKey?: string;
  errorParams?: Record<string, string | number>;
}

/**
 * Validates survey workspace form data against business readiness rules for Mark Ready.
 * Returns structured result for i18n translation mapping.
 */
export function validateSurveyReadiness(data: SurveyWorkspaceFormData): ReadinessValidationResult {
  if (!data.visitedAt || !data.visitedAt.trim()) {
    return { isValid: false, errorKey: "readinessErrors.missingVisit" };
  }

  if (!data.scopeSummary || !data.scopeSummary.trim()) {
    return { isValid: false, errorKey: "readinessErrors.missingScope" };
  }

  if (!data.areas || data.areas.length === 0) {
    return { isValid: false, errorKey: "readinessErrors.missingArea" };
  }

  for (const area of data.areas) {
    if (!area.name || !area.name.trim()) {
      return {
        isValid: false,
        errorKey: "readinessErrors.missingAreaName",
        errorParams: { code: area.code },
      };
    }

    if (!area.measurements || area.measurements.length === 0) {
      return { isValid: false, errorKey: "readinessErrors.missingMeasurement" };
    }

    for (const m of area.measurements) {
      if (typeof m.value !== "number" || m.value <= 0 || isNaN(m.value)) {
        return { isValid: false, errorKey: "readinessErrors.missingMeasurement" };
      }
    }
  }

  return { isValid: true };
}
