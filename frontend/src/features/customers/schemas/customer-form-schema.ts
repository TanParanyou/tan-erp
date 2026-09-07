import { z } from "zod";

export const customerFormSchema = z.object({
  customerType: z.enum(["corporate", "individual"]),
  displayNameTh: z.string().trim().min(1, "validation.required"),
  displayNameEn: z.string().trim().optional().or(z.literal("")),
  preferredLocale: z.enum(["th", "en"]),
  primaryContact: z.object({
    name: z.string().trim().min(1, "validation.required"),
    roleTitle: z.string().trim().optional().or(z.literal("")),
    phone: z.string().trim().min(1, "validation.required"),
    email: z
      .string()
      .trim()
      .optional()
      .or(z.literal(""))
      .refine((val) => !val || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val), {
        message: "validation.invalidEmail",
      }),
    preferredChannel: z.enum(["phone", "email", "line", "other"]).optional(),
  }),
});

export type CustomerFormValues = z.infer<typeof customerFormSchema>;
