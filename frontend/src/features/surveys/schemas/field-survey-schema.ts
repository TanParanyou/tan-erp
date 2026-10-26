import { z } from "zod";
import type { AttachmentPurpose } from "@/lib/attachments/attachment-owner-types";

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
  /** Local file kept until save; uploaded to the survey area after the draft is saved. */
  file: z.custom<File>((value) => value instanceof File).optional(),
});

export type FieldSurveyPhoto = z.infer<typeof fieldSurveyPhotoSchema>;

/** Backend photo tags (EvidenceKind); the legacy aliases site_location / sketch_laser map to their canonical tags. */
export function resolveFieldSurveyPhotoTag(tag: FieldSurveyPhotoTag): string {
  if (tag === "site_location") return "site_photo";
  if (tag === "sketch_laser") return "measurement_sketch";
  return tag;
}

/** Attachment purpose is coarser than the tag: defects stay defects, references are general, the rest is evidence. */
export function resolveFieldSurveyPhotoPurpose(tag: FieldSurveyPhotoTag): AttachmentPurpose {
  switch (resolveFieldSurveyPhotoTag(tag)) {
    case "constraint_defect":
      return "defect";
    case "mood_tone":
    case "other":
      return "general";
    default:
      return "evidence";
  }
}

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

export const PHOTO_REFERENCE_KINDS = ["a4", "a3", "custom"] as const;
export type PhotoReferenceKind = (typeof PHOTO_REFERENCE_KINDS)[number];

export const photoMeasurementSchema = z.object({
  grossAreaSqm: z.number().min(0),
  vertexCount: z.number().int().min(3),
  referenceKind: z.enum(PHOTO_REFERENCE_KINDS),
  referenceWidthMm: z.number().positive(),
  referenceHeightMm: z.number().positive(),
});

export type PhotoMeasurement = z.infer<typeof photoMeasurementSchema>;

export const areaDimensionsSchema = z.object({
  grossWidthMm: z.number().min(0),
  grossLengthMm: z.number().min(0),
  deductions: z.array(areaDeductionSchema).default([]),
  photoMeasurement: photoMeasurementSchema.nullable().optional(),
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

/**
 * Gross area in square meters: the photo-estimated value when present, else width x length.
 */
export function resolveGrossAreaSqm(dimensions: AreaDimensions): number {
  if (dimensions.photoMeasurement) return dimensions.photoMeasurement.grossAreaSqm;
  return (dimensions.grossWidthMm / 1000) * (dimensions.grossLengthMm / 1000);
}

/**
 * Net area (gross minus deductions, floored at 0, rounded to 2 dp) used for display and saving.
 */
export function resolveNetAreaSqm(dimensions: AreaDimensions): number {
  const totalDeductionsSqm = dimensions.deductions.reduce((sum, d) => sum + d.areaSqm, 0);
  const net = Math.max(0, resolveGrossAreaSqm(dimensions) - totalDeductionsSqm);
  return Math.round(net * 100) / 100;
}
