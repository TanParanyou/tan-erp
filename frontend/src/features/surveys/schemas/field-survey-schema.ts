import { z } from "zod";

export const FIELD_SURVEY_MEASUREMENT_TYPES = ["box", "area", "custom"] as const;
export type FieldSurveyMeasurementType = (typeof FIELD_SURVEY_MEASUREMENT_TYPES)[number];

export const FIELD_SURVEY_PHOTO_TAGS = [
  "site_photo",
  "measurement_sketch",
  "material_swatch",
  "constraint_defect",
  "mood_tone",
  "other",
  "site_location",
  "sketch_laser",
] as const;
export type FieldSurveyPhotoTag = (typeof FIELD_SURVEY_PHOTO_TAGS)[number];

export const fieldSurveyPhotoSchema = z.object({
  id: z.string().min(1),
  url: z.string().url().or(z.string().startsWith("blob:")).or(z.string().startsWith("data:")).or(z.string().startsWith("/")),
  fileName: z.string().min(1),
  tag: z.enum(FIELD_SURVEY_PHOTO_TAGS),
  caption: z.string().nullable().optional(),
});

export type FieldSurveyPhoto = z.infer<typeof fieldSurveyPhotoSchema>;

export const customMeasurementPointSchema = z.object({
  label: z.string().min(1, "Label is required"),
  valueMm: z.number().positive("Value must be positive"),
  notes: z.string().nullable().optional(),
});

export type CustomMeasurementPoint = z.infer<typeof customMeasurementPointSchema>;

export const areaDeductionSchema = z.object({
  label: z.string().min(1, "Deduction label is required"),
  areaSqm: z.number().positive("Deduction area must be positive"),
});

export type AreaDeduction = z.infer<typeof areaDeductionSchema>;

export const boxDimensionsSchema = z.object({
  widthMm: z.number().min(0),
  depthMm: z.number().min(0),
  heightMm: z.number().min(0),
});

export type BoxDimensions = z.infer<typeof boxDimensionsSchema>;

export const areaDimensionsSchema = z.object({
  grossWidthMm: z.number().min(0),
  grossLengthMm: z.number().min(0),
  deductions: z.array(areaDeductionSchema).default([]),
});

export type AreaDimensions = z.infer<typeof areaDimensionsSchema>;

export const specSelectionSchema = z.object({
  coreSkuId: z.string().nullable().optional(),
  coreSkuName: z.string().nullable().optional(),
  finishSkuId: z.string().nullable().optional(),
  finishSkuName: z.string().nullable().optional(),
  hardwareSkuId: z.string().nullable().optional(),
  hardwareSkuName: z.string().nullable().optional(),
  customNotes: z.string().nullable().optional(),
});

export type SpecSelection = z.infer<typeof specSelectionSchema>;

export const fieldSurveyItemSchema = z.object({
  id: z.string().min(1),
  title: z.string().min(1, "Item title is required"),
  category: z.string().nullable().optional(),
  measurementType: z.enum(FIELD_SURVEY_MEASUREMENT_TYPES),
  boxDimensions: boxDimensionsSchema.optional(),
  areaDimensions: areaDimensionsSchema.optional(),
  customMeasurements: z.array(customMeasurementPointSchema).optional(),
  specSelection: specSelectionSchema.optional(),
  photos: z.array(fieldSurveyPhotoSchema).default([]),
  notes: z.string().nullable().optional(),
});

export type FieldSurveyItem = z.infer<typeof fieldSurveyItemSchema>;

export const fieldSurveyFormSchema = z.object({
  opportunityId: z.string().uuid("Invalid opportunity ID"),
  roomName: z.string().min(1, "Room name is required"),
  surveyorNotes: z.string().nullable().optional(),
  designStyle: z.string().nullable().optional(),
  moodTone: z.string().nullable().optional(),
  styleNotes: z.string().nullable().optional(),
  items: z.array(fieldSurveyItemSchema).min(1, "At least one item is required"),
});

export type FieldSurveyFormData = z.infer<typeof fieldSurveyFormSchema>;

/**
 * Calculates net area in square meters after deducting openings and pillars.
 */
export function calculateNetAreaSqm(
  grossWidthMm: number,
  grossLengthMm: number,
  deductions: readonly AreaDeduction[] = []
): number {
  const grossAreaSqm = (grossWidthMm / 1000) * (grossLengthMm / 1000);
  const totalDeductionsSqm = deductions.reduce((sum, d) => sum + d.areaSqm, 0);
  const net = Math.max(0, grossAreaSqm - totalDeductionsSqm);
  return Math.round(net * 100) / 100;
}
