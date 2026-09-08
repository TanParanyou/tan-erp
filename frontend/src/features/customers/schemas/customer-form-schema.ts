import { z } from "zod";

type ValidationTranslator = (
  key: "required" | "invalidEmail" | "phoneOrEmailRequired" | "invalidFormat",
) => string;

export const createCustomerFormSchema = (t: ValidationTranslator) =>
  z.object({
    customerType: z.enum(["organization", "person"]),
    displayNameTh: z.string().trim().min(1, t("required")),
    displayNameEn: z.string().trim().optional().or(z.literal("")),
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
    primaryContact: z
      .object({
        name: z.string().trim().min(1, t("required")),
        roleTitle: z.string().trim().optional().or(z.literal("")),
        phone: z.string().trim().optional().or(z.literal("")),
        email: z
          .string()
          .trim()
          .email(t("invalidEmail"))
          .optional()
          .or(z.literal("")),
        lineId: z.string().trim().max(100, t("invalidFormat")).optional().or(z.literal("")),
        preferredChannel: z.enum(["phone", "email", "line", "other"]),
      })
      .refine((value) => Boolean(value.phone || value.email), {
        message: t("phoneOrEmailRequired"),
        path: ["phone"],
      }),
  });

export type CustomerFormValues = z.infer<ReturnType<typeof createCustomerFormSchema>>;
