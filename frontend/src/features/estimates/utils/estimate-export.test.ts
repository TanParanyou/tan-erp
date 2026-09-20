import { describe, it, expect } from "vitest";
import {
  buildEstimateCsv,
  isEstimateCalculationSnapshot,
  type EstimateCalculationSnapshot,
} from "./estimate-export";

describe("estimate-export", () => {
  describe("isEstimateCalculationSnapshot", () => {
    it("returns true for a complete and valid EstimateCalculationSnapshot", () => {
      const validSnapshot: EstimateCalculationSnapshot = {
        calculationVersion: 2,
        calculatedAtUtc: "2026-09-20T10:30:00Z",
        currency: "THB",
        netCost: 50000,
        sellingBeforeDiscount: 75000,
        discountAmount: 5000,
        netBeforeTax: 70000,
        taxAmount: 4900,
        grandTotal: 74900,
        marginAmount: 20000,
        marginRate: 0.2857,
        markupRate: 0.4,
        sectionsCount: 3,
        workItemsCount: 8,
        costComponentsCount: 24,
      };

      expect(isEstimateCalculationSnapshot(validSnapshot)).toBe(true);
    });

    it("returns false for null, non-objects, or incomplete objects", () => {
      expect(isEstimateCalculationSnapshot(null)).toBe(false);
      expect(isEstimateCalculationSnapshot(undefined)).toBe(false);
      expect(isEstimateCalculationSnapshot("invalid")).toBe(false);
      expect(isEstimateCalculationSnapshot(123)).toBe(false);
      expect(isEstimateCalculationSnapshot({})).toBe(false);
      expect(
        isEstimateCalculationSnapshot({
          calculationVersion: 1,
          netCost: 100,
        })
      ).toBe(false);
      expect(
        isEstimateCalculationSnapshot({
          calculationVersion: "1", // wrong type
          calculatedAtUtc: "2026-09-20T10:30:00Z",
          currency: "THB",
          netCost: 50000,
          sellingBeforeDiscount: 75000,
          discountAmount: 5000,
          netBeforeTax: 70000,
          taxAmount: 4900,
          grandTotal: 74900,
          marginAmount: 20000,
          marginRate: 0.2857,
          markupRate: 0.4,
          sectionsCount: 3,
          workItemsCount: 8,
          costComponentsCount: 24,
        })
      ).toBe(false);
    });
  });

  describe("buildEstimateCsv_UsesServerSnapshotTotalsWithoutRecalculation", () => {
    it("generates CSV string directly from server calculation snapshot totals without recalculation", () => {
      const serverSnapshot: EstimateCalculationSnapshot = {
        calculationVersion: 3,
        calculatedAtUtc: "2026-09-20T14:15:00Z",
        currency: "THB",
        netCost: 120500.5,
        sellingBeforeDiscount: 180000,
        discountAmount: 10000,
        netBeforeTax: 170000,
        taxAmount: 11900,
        grandTotal: 181900,
        marginAmount: 49499.5,
        marginRate: 0.2912,
        markupRate: 0.4108,
        sectionsCount: 4,
        workItemsCount: 12,
        costComponentsCount: 36,
      };

      const csv = buildEstimateCsv(serverSnapshot);

      expect(typeof csv).toBe("string");
      expect(csv).toContain("Calculation Version");
      expect(csv).toContain("Net Cost");
      expect(csv).toContain("Selling Before Discount");
      expect(csv).toContain("Discount Amount");
      expect(csv).toContain("Net Before Tax");
      expect(csv).toContain("Tax Amount");
      expect(csv).toContain("Grand Total");
      expect(csv).toContain("Margin Amount");
      expect(csv).toContain("Margin Rate");
      expect(csv).toContain("Markup Rate");

      // Verify exact server values are present without re-derivation
      expect(csv).toContain("3");
      expect(csv).toContain("2026-09-20T14:15:00Z");
      expect(csv).toContain("THB");
      expect(csv).toContain("120500.5");
      expect(csv).toContain("180000");
      expect(csv).toContain("10000");
      expect(csv).toContain("170000");
      expect(csv).toContain("11900");
      expect(csv).toContain("181900");
      expect(csv).toContain("49499.5");
      expect(csv).toContain("0.2912");
      expect(csv).toContain("0.4108");
      expect(csv).toContain("4");
      expect(csv).toContain("12");
      expect(csv).toContain("36");
    });
  });
});
