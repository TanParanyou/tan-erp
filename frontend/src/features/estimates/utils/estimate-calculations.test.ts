import { describe, it, expect } from "vitest";
import {
  calculateCostComponentSubtotal,
  calculateWorkItemUnitCost,
  calculateUnitSellingPrice,
  calculateWorkItemSummary,
  calculateLiveWorkspaceHud,
  isGrossProfitHealthy,
} from "./estimate-calculations";

describe("estimate-calculations", () => {
  it("calculates cost component subtotal correctly", () => {
    expect(calculateCostComponentSubtotal(5, 200)).toBe(1000);
    expect(calculateCostComponentSubtotal(0, 500)).toBe(0);
    expect(calculateCostComponentSubtotal(null, 500)).toBe(0);
  });

  it("calculates work item unit cost sum across components", () => {
    const comps = [
      { type: "material", description: "Wood", quantity: 2, unitCode: "lot", unitCost: 1500, currency: "THB", sortOrder: 1 },
      { type: "labor", description: "Labor", quantity: 1, unitCode: "lot", unitCost: 1000, currency: "THB", sortOrder: 2 },
    ];
    expect(calculateWorkItemUnitCost(comps)).toBe(4000);
    expect(calculateWorkItemUnitCost([])).toBe(0);
  });

  it("calculates unit selling price based on pricing rules", () => {
    // Margin 20% on 4000 cost: selling = 4000 / (1 - 0.2) = 5000
    expect(calculateUnitSellingPrice(4000, "margin", 20)).toBe(5000);

    // Markup 25% on 4000 cost: selling = 4000 * 1.25 = 5000
    expect(calculateUnitSellingPrice(4000, "markup", 25)).toBe(5000);

    // Fixed amount: selling = 6500
    expect(calculateUnitSellingPrice(4000, "fixed", 6500)).toBe(6500);

    // Margin edge case: margin >= 100% does not divide by zero
    expect(calculateUnitSellingPrice(4000, "margin", 100)).toBe(4000);
  });

  it("calculates work item summary with GP and margin rate", () => {
    const item = {
      code: "ITM-01",
      descriptionTh: "Wardrobe",
      descriptionEn: "Wardrobe",
      quantity: 2,
      unitCode: "lot",
      sellingRuleType: "margin",
      sellingRuleValue: 20,
      sortOrder: 1,
      costComponents: [
        { type: "material", description: "Wood", quantity: 1, unitCode: "lot", unitCost: 4000, currency: "THB", sortOrder: 1 },
      ],
    };

    const summary = calculateWorkItemSummary(item);
    expect(summary.unitCostSum).toBe(4000);
    expect(summary.totalCost).toBe(8000);
    expect(summary.unitSellingPrice).toBe(5000);
    expect(summary.totalSellingPrice).toBe(10000);
    expect(summary.grossProfit).toBe(2000);
    expect(summary.marginRate).toBe(20);
    expect(summary.markupRate).toBe(25);
  });

  it("calculates live workspace HUD with discount", () => {
    const sections = [
      {
        code: "SEC-01",
        nameTh: "Master Bedroom",
        nameEn: "Master Bedroom",
        sortOrder: 1,
        workItems: [
          {
            code: "ITM-01",
            descriptionTh: "Wardrobe",
            descriptionEn: "Wardrobe",
            quantity: 1,
            unitCode: "lot",
            sellingRuleType: "margin",
            sellingRuleValue: 20,
            sortOrder: 1,
            costComponents: [
              { type: "material", description: "Wood", quantity: 1, unitCode: "lot", unitCost: 4000, currency: "THB", sortOrder: 1 },
            ],
          },
        ],
      },
    ];

    const hud = calculateLiveWorkspaceHud(sections, 500);
    expect(hud.totalCost).toBe(4000);
    expect(hud.sellingBeforeDiscount).toBe(5000);
    expect(hud.discountAmount).toBe(500);
    expect(hud.netSelling).toBe(4500);
    expect(hud.grossProfit).toBe(500);
    expect(hud.totalSections).toBe(1);
    expect(hud.totalWorkItems).toBe(1);
  });

  it("correctly identifies gross profit health benchmark", () => {
    expect(isGrossProfitHealthy(35, 30)).toBe(true);
    expect(isGrossProfitHealthy(30, 30)).toBe(true);
    expect(isGrossProfitHealthy(28, 30)).toBe(false);
  });
});
