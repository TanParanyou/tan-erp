import { z } from "zod";
import { ITEM_IMAGE_MAX_BYTES } from "@/features/item-master/item-image-constants";

export const itemFormSchema = z.object({
  code: z.string().trim().max(50),
  codeMode: z.enum(["generated", "manual"]),
  itemType: z.enum(["material", "labor", "service", "subcontract", "other", "product"]),
  categoryId: z.string().uuid(),
  brandId: z.string().optional(),
  baseUnitId: z.string().uuid(),
  taxCategoryCode: z.string().trim().max(30).optional(),
  name: z.object({ th: z.string().trim().min(1), en: z.string().optional() }),
  description: z.object({ th: z.string().optional(), en: z.string().optional() }),
  availabilityMode: z.enum(["all_branches", "selected_branches"]),
  selectedBranchIds: z.array(z.string().uuid()),
  capabilities: z.object({ canSell: z.boolean(), canCost: z.boolean(), canPurchase: z.boolean(), canStock: z.boolean(), canProduce: z.boolean() }),
  attributes: z.array(
    z.object({
      key: z.string().trim(),
      value: z.string().trim(),
    })
  ).superRefine((pairs, ctx) => {
    const seen = new Set<string>();
    const reserved = ["price", "status", "permission", "currency", "unitcost", "cost"];
    pairs.forEach((pair, idx) => {
      if (!pair.key && pair.value) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: [idx, "key"],
          message: "KEY_REQUIRED",
        });
      }
      if (pair.key) {
        const lower = pair.key.toLowerCase();
        if (seen.has(lower)) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            path: [idx, "key"],
            message: "DUPLICATE_KEY",
          });
        } else {
          seen.add(lower);
        }
        if (reserved.includes(lower)) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            path: [idx, "key"],
            message: "RESERVED_KEY",
          });
        }
      }
    });
  }),
  imageFile: z.custom<File | null>((value) => value === null || (typeof File !== "undefined" && value instanceof File))
    .refine((file) => file === null || (file.size > 0 && file.size <= ITEM_IMAGE_MAX_BYTES), { message: "IMAGE_SIZE_INVALID" }),
  imageAltText: z.string().trim().max(250),
  aliases: z.array(z.object({ th: z.string().trim().min(1), en: z.string().optional() })),
}).superRefine((value, context) => {
  if (value.codeMode === "manual" && !value.code) {
    context.addIssue({ code: z.ZodIssueCode.custom, path: ["code"], message: "CODE_REQUIRED" });
  }
  if (value.availabilityMode === "selected_branches" && value.selectedBranchIds.length === 0) {
    context.addIssue({ code: z.ZodIssueCode.custom, path: ["selectedBranchIds"], message: "Select at least one branch." });
  }
  if (value.imageFile && !value.imageAltText.trim()) {
    context.addIssue({ code: z.ZodIssueCode.custom, path: ["imageAltText"], message: "IMAGE_ALT_REQUIRED" });
  }
});

export type ItemFormValues = z.infer<typeof itemFormSchema>;
