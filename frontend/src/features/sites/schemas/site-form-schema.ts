import { z } from "zod";

type ValidationTranslator = (
  key: "required" | "invalidFormat" | "invalidNumber" | "coordinatePairRequired"
) => string;

export const createSiteFormSchema = (t: ValidationTranslator) =>
  z
    .object({
      label: z.string().trim().min(1, t("required")).max(100, t("invalidFormat")),
      addressLine1: z.string().trim().min(1, t("required")).max(255, t("invalidFormat")),
      subdistrict: z.string().trim().min(1, t("required")).max(100, t("invalidFormat")),
      district: z.string().trim().min(1, t("required")).max(100, t("invalidFormat")),
      province: z.string().trim().min(1, t("required")).max(100, t("invalidFormat")),
      postalCode: z.string().trim().min(1, t("required")).max(20, t("invalidFormat")),
      countryCode: z.string().trim().regex(/^[A-Z]{2}$/, t("invalidFormat")),
      latitude: z
        .number({ message: t("invalidNumber") })
        .min(-90, t("invalidFormat"))
        .max(90, t("invalidFormat"))
        .nullable()
        .optional(),
      longitude: z
        .number({ message: t("invalidNumber") })
        .min(-180, t("invalidFormat"))
        .max(180, t("invalidFormat"))
        .nullable()
        .optional(),
      accessNote: z.string().trim().max(1000, t("invalidFormat")).optional().or(z.literal("")),
    })
    .refine(
      (data) => {
        const hasLat = data.latitude !== null && data.latitude !== undefined;
        const hasLng = data.longitude !== null && data.longitude !== undefined;
        return (hasLat && hasLng) || (!hasLat && !hasLng);
      },
      {
        message: t("coordinatePairRequired"),
        path: ["latitude"],
      }
    );

export type SiteFormValues = z.infer<ReturnType<typeof createSiteFormSchema>>;
