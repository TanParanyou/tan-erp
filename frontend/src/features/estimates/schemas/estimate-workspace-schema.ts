import { z } from "zod";

export const costComponentSchema = z.object({
  id: z.string().nullable().optional(),
  type: z.string(),
  description: z.string(),
  quantity: z.number().min(0, "Quantity must be greater than or equal to 0"),
  unitCode: z.string(),
  unitCost: z.number().min(0, "Unit cost must be greater than or equal to 0"),
  currency: z.string(),
  sortOrder: z.number(),
  itemId: z.string().nullable().optional(),
  costRecordId: z.string().nullable().optional(),
  costRecordVersion: z.number().nullable().optional(),
  provisionalReasonCode: z.string().nullable().optional(),
  provisionalNote: z.string().nullable().optional(),
  isProvisional: z.boolean().optional(),
});

export type CostComponentFormData = z.infer<typeof costComponentSchema>;

export const workItemSchema = z.object({
  uiKey: z.string().optional(),
  id: z.string().nullable().optional(),
  code: z.string().min(1, "Item code is required"),
  descriptionTh: z.string(),
  descriptionEn: z.string(),
  quantity: z.number().min(0.001, "Quantity must be greater than 0"),
  unitCode: z.string(),
  sellingRuleType: z.string(),
  sellingRuleValue: z.number(),
  sellingRuleReasonCode: z.string().max(64).nullable().optional(),
  itemId: z.string().nullable().optional(),
  item: z.object({ id: z.string(), code: z.string(), nameTh: z.string(), nameEn: z.string().nullable().optional() }).nullable().optional(),
  overrideReasonCode: z.string().max(64).nullable().optional(),
  overrideReason: z.string().max(500).nullable().optional(),
  sortOrder: z.number(),
  costComponents: z.array(costComponentSchema),
}).superRefine((workItem, context) => {
  if (workItem.sellingRuleType === "fixed_price" && !workItem.sellingRuleReasonCode?.trim()) {
    context.addIssue({
      code: z.ZodIssueCode.custom,
      path: ["sellingRuleReasonCode"],
      message: "fixedPriceReasonRequired",
    });
  }
  if (!workItem.itemId && !workItem.overrideReasonCode?.trim()) context.addIssue({ code: z.ZodIssueCode.custom, path: ["overrideReasonCode"], message: "customWorkItemReasonCodeRequired" });
  if (!workItem.itemId && !workItem.overrideReason?.trim()) context.addIssue({ code: z.ZodIssueCode.custom, path: ["overrideReason"], message: "customWorkItemReasonRequired" });
});

export type WorkItemFormData = z.infer<typeof workItemSchema>;

export const sectionSchema = z.object({
  uiKey: z.string().optional(),
  id: z.string().nullable().optional(),
  code: z.string().min(1, "Section code is required"),
  nameTh: z.string(),
  nameEn: z.string(),
  sortOrder: z.number(),
  workItems: z.array(workItemSchema),
});

export type SectionFormData = z.infer<typeof sectionSchema>;

export const estimateWorkspaceSchema = z.object({
  discountType: z.enum(["", "none", "percent", "fixed-amount"]),
  discountValue: z.number().min(0, "Discount cannot be negative"),
  discountReasonCode: z.string().max(64),
  sections: z.array(sectionSchema),
});

export type EstimateWorkspaceFormData = z.infer<typeof estimateWorkspaceSchema>;
export type EstimateDiscountType = "" | "none" | "percent" | "fixed-amount";
