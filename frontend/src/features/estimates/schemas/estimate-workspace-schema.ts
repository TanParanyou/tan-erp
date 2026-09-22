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
});

export type CostComponentFormData = z.infer<typeof costComponentSchema>;

export const workItemSchema = z.object({
  id: z.string().nullable().optional(),
  code: z.string().min(1, "Item code is required"),
  descriptionTh: z.string(),
  descriptionEn: z.string(),
  quantity: z.number().min(0.001, "Quantity must be greater than 0"),
  unitCode: z.string(),
  sellingRuleType: z.string(),
  sellingRuleValue: z.number(),
  sortOrder: z.number(),
  costComponents: z.array(costComponentSchema),
});

export type WorkItemFormData = z.infer<typeof workItemSchema>;

export const sectionSchema = z.object({
  id: z.string().nullable().optional(),
  code: z.string().min(1, "Section code is required"),
  nameTh: z.string(),
  nameEn: z.string(),
  sortOrder: z.number(),
  workItems: z.array(workItemSchema),
});

export type SectionFormData = z.infer<typeof sectionSchema>;

export const estimateWorkspaceSchema = z.object({
  discountAmount: z.number().min(0, "Discount cannot be negative"),
  sections: z.array(sectionSchema),
});

export type EstimateWorkspaceFormData = z.infer<typeof estimateWorkspaceSchema>;
