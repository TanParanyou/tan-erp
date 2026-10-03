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

export const SURVEY_CHECKLIST_RESULTS = ["pass", "fail", "not_applicable"] as const;
export type SurveyChecklistResultValue = (typeof SURVEY_CHECKLIST_RESULTS)[number];

export const SURVEY_EVIDENCE_KINDS = ["site_photo", "measurement_sketch", "other"] as const;
export type SurveyEvidenceKindValue = (typeof SURVEY_EVIDENCE_KINDS)[number];

export const surveyChecklistItemSchema = z.object({
  itemCode: z.string().min(1),
  result: z.enum(SURVEY_CHECKLIST_RESULTS),
  note: z.string().nullable().optional(),
});

export type SurveyChecklistItemFormData = z.infer<typeof surveyChecklistItemSchema>;

export const surveyEvidenceItemSchema = z.object({
  fileId: z.string().min(1),
  kind: z.enum(SURVEY_EVIDENCE_KINDS),
  caption: z.string().nullable().optional(),
  sortOrder: z.number(),
});

export type SurveyEvidenceItemFormData = z.infer<typeof surveyEvidenceItemSchema>;

export const surveyWorkspaceSchema = z.object({
  visitedAt: z.string(),
  scopeSummary: z.string(),
  assumptions: z.array(z.string()),
  constraints: z.array(z.string()),
  missingDetails: z.array(z.string()),
  areas: z.array(surveyAreaSchema),
  checklist: z.array(surveyChecklistItemSchema),
  evidence: z.array(surveyEvidenceItemSchema),
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
export interface SurveyTemplateRequirements {
  requiredChecklistItems: readonly string[];
  minimumEvidenceCount: number;
}

export function validateSurveyReadiness(
  data: SurveyWorkspaceFormData,
  template?: SurveyTemplateRequirements | null
): ReadinessValidationResult {
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

  if (template) {
    for (const itemCode of template.requiredChecklistItems) {
      const entry = data.checklist.find((c) => c.itemCode === itemCode);
      if (!entry) {
        return { isValid: false, errorKey: "readinessErrors.missingChecklistItem", errorParams: { item: itemCode } };
      }

      if (entry.result !== "pass" && !(entry.note ?? "").trim()) {
        return { isValid: false, errorKey: "readinessErrors.checklistNoteRequired", errorParams: { item: itemCode } };
      }
    }

    if (data.evidence.length < template.minimumEvidenceCount) {
      return {
        isValid: false,
        errorKey: "readinessErrors.missingEvidence",
        errorParams: { count: template.minimumEvidenceCount },
      };
    }
  }

  return { isValid: true };
}
