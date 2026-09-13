import { describe, it, expect } from "vitest";
import {
  ESTIMATE_TEMPLATES,
  QUICK_COST_PRESETS,
  createWorkItemFromTemplate,
} from "./estimate-templates";

describe("estimate-templates", () => {
  it("defines standard 5 built-in interior templates", () => {
    expect(ESTIMATE_TEMPLATES).toHaveLength(5);
    const templateIds = ESTIMATE_TEMPLATES.map((t) => t.id);
    expect(templateIds).toEqual(["wardrobe", "kitchen", "wall", "tv", "curtain"]);
  });

  it("each template has positive quantity, valid selling rule, and at least one cost component", () => {
    ESTIMATE_TEMPLATES.forEach((tpl) => {
      expect(tpl.itemCode).toBeTruthy();
      expect(tpl.itemDescTh).toBeTruthy();
      expect(tpl.itemDescEn).toBeTruthy();
      expect(tpl.quantity).toBeGreaterThan(0);
      expect(["margin", "markup", "fixed"]).toContain(tpl.sellingRule);
      expect(tpl.sellingRuleValue).toBeGreaterThan(0);
      expect(tpl.costComponents.length).toBeGreaterThan(0);

      tpl.costComponents.forEach((comp) => {
        expect(comp.description).toBeTruthy();
        expect(comp.unitCost).toBeGreaterThan(0);
        expect([
          "material",
          "labor",
          "subcontract",
          "equipment",
          "overhead",
          "other",
        ]).toContain(comp.type);
      });
    });
  });

  it("defines quick cost presets with valid cost types and descriptions", () => {
    expect(QUICK_COST_PRESETS.length).toBe(6);
    QUICK_COST_PRESETS.forEach((preset) => {
      expect(preset.id).toBeTruthy();
      expect(preset.labelKey).toBeTruthy();
      expect(preset.defaultDesc).toBeTruthy();
      expect(preset.defaultCost).toBeGreaterThan(0);
    });
  });

  it("createWorkItemFromTemplate correctly converts template to WorkItemFormData", () => {
    const tpl = ESTIMATE_TEMPLATES[0];
    const workItem = createWorkItemFromTemplate(tpl, () => "test-id-123");

    expect(workItem.id).toBe("test-id-123");
    expect(workItem.code).toBe(tpl.itemCode);
    expect(workItem.descriptionTh).toBe(tpl.itemDescTh);
    expect(workItem.descriptionEn).toBe(tpl.itemDescEn);
    expect(workItem.quantity).toBe(tpl.quantity);
    expect(workItem.unitCode).toBe(tpl.unitCode);
    expect(workItem.sellingRuleType).toBe(tpl.sellingRule);
    expect(workItem.sellingRuleValue).toBe(tpl.sellingRuleValue);
    expect(workItem.costComponents).toHaveLength(tpl.costComponents.length);
    expect(workItem.costComponents[0].id).toBe("test-id-123");
  });
});
