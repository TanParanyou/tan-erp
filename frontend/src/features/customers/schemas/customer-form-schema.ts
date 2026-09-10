import { z } from "zod";
import { createPhoneSchema } from "@/lib/validation/phone";

export type ValidationTranslator = (
  key:
    | "required"
    | "invalidEmail"
    | "phoneOrEmailRequired"
    | "invalidFormat"
    | "invalidPhone"
    | "leadSourceNoteRequired",
) => string;

export const createCustomerFormSchema = (t: ValidationTranslator) =>
  z
    .object({
      customerType: z.enum(["organization", "person"]),
      displayNameTh: z.string().trim().min(1, t("required")).max(250, t("invalidFormat")),
      displayNameEn: z.string().trim().max(250, t("invalidFormat")).optional().or(z.literal("")),
      preferredLocale: z.enum(["th", "en"]),
      leadSource: z
        .enum([
          "walk_in",
          "facebook_ads",
          "referral",
          "project_developer",
          "website",
          "other",
        ])
        .optional()
        .or(z.literal("")),
      leadSourceNote: z.string().trim().max(200, t("invalidFormat")).optional().or(z.literal("")),
      primaryContact: z.object({
        name: z.string().trim().min(1, t("required")).max(250, t("invalidFormat")),
        roleTitle: z.string().trim().max(150, t("invalidFormat")).optional().or(z.literal("")),
        phone: createPhoneSchema(t),
        email: z
          .string()
          .trim()
          .email(t("invalidEmail"))
          .max(150, t("invalidFormat"))
          .optional()
          .or(z.literal("")),
        lineId: z.string().trim().max(100, t("invalidFormat")).optional().or(z.literal("")),
        preferredChannel: z.enum(["phone", "email", "line", "other"]),
      }),
    })
    .superRefine((data, ctx) => {
      // 1. Either phone or email must be present
      if (!data.primaryContact.phone && !data.primaryContact.email) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: t("phoneOrEmailRequired"),
          path: ["primaryContact", "phone"],
        });
      }

      // 2. If leadSource is 'other', leadSourceNote is required
      if (data.leadSource === "other" && (!data.leadSourceNote || !data.leadSourceNote.trim())) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: t("leadSourceNoteRequired"),
          path: ["leadSourceNote"],
        });
      }
    });

export type CustomerFormValues = z.infer<ReturnType<typeof createCustomerFormSchema>>;

