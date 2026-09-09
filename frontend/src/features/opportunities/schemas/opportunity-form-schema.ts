import { z } from "zod";

export const CANONICAL_WORK_TYPES = [
  "built-in",
  "interior",
  "curtain",
  "wallpaper",
  "exterior",
  "other",
] as const;

export type WorkType = (typeof CANONICAL_WORK_TYPES)[number];

type ValidationTranslator = (
  key: "required" | "invalidFormat" | "invalidNumber" | "positiveNumber" | "actionDateNotePairRequired"
) => string;

const UUID_REGEX = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

export const createOpportunityFormSchema = (t: ValidationTranslator) =>
  z
    .object({
      customerId: z.string().trim().regex(UUID_REGEX, t("invalidFormat")),
      primarySiteId: z.string().trim().regex(UUID_REGEX, t("invalidFormat")).optional().or(z.literal("")),
      title: z.string().trim().min(1, t("required")).max(250, t("invalidFormat")),
      scopeSummary: z.string().trim().max(1000, t("invalidFormat")).optional().or(z.literal("")),
      workTypes: z
        .array(z.enum(CANONICAL_WORK_TYPES))
        .min(1, t("required")),
      sourceCode: z.string().trim().max(100, t("invalidFormat")).optional().or(z.literal("")),
      expectedBudget: z
        .number({ message: t("invalidNumber") })
        .positive(t("positiveNumber"))
        .nullable()
        .optional(),
      currencyCode: z.string().trim().optional().or(z.literal("")),
      targetDecisionDate: z
        .string()
        .trim()
        .regex(/^\d{4}-\d{2}-\d{2}$/, t("invalidFormat"))
        .optional()
        .or(z.literal("")),
      nextActionAtUtc: z.string().trim().optional().or(z.literal("")),
      nextActionNote: z.string().trim().max(500, t("invalidFormat")).optional().or(z.literal("")),
    })
    .refine(
      (data) => {
        if (data.expectedBudget !== null && data.expectedBudget !== undefined && data.expectedBudget > 0) {
          return Boolean(data.currencyCode && data.currencyCode.trim().length === 3);
        }
        return true;
      },
      {
        message: t("required"),
        path: ["currencyCode"],
      }
    )
    .refine(
      (data) => {
        const hasDate = Boolean(data.nextActionAtUtc && data.nextActionAtUtc.trim() !== "");
        const hasNote = Boolean(data.nextActionNote && data.nextActionNote.trim() !== "");
        return (hasDate && hasNote) || (!hasDate && !hasNote);
      },
      {
        message: t("actionDateNotePairRequired"),
        path: ["nextActionNote"],
      }
    );

export type OpportunityFormValues = z.infer<ReturnType<typeof createOpportunityFormSchema>>;
