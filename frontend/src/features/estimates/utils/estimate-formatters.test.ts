import { describe, it, expect } from "vitest";
import {
  formatFinancialNumber,
  formatPercentRate,
  formatSignedFinancialAmount,
  formatIntegerCount,
} from "./estimate-formatters";

describe("estimate-formatters", () => {
  it("formats financial numbers with 2 decimals and commas", () => {
    expect(formatFinancialNumber(433150)).toBe("433,150.00");
    expect(formatFinancialNumber(0)).toBe("0.00");
    expect(formatFinancialNumber(null)).toBe("-");
    expect(formatFinancialNumber(undefined)).toBe("-");
  });

  it("formats percentage rates correctly", () => {
    expect(formatPercentRate(38.8712)).toBe("38.87%");
    expect(formatPercentRate(25)).toBe("25.00%");
    expect(formatPercentRate(null)).toBe("0.00%");
  });

  it("formats signed financial amounts with plus and minus signs", () => {
    expect(formatSignedFinancialAmount(168350, "THB")).toBe("+168,350.00 THB");
    expect(formatSignedFinancialAmount(-5000, "THB")).toBe("-5,000.00 THB");
    expect(formatSignedFinancialAmount(0)).toBe("0.00");
  });

  it("formats integer counts with commas", () => {
    expect(formatIntegerCount(1200)).toBe("1,200");
    expect(formatIntegerCount(0)).toBe("0");
    expect(formatIntegerCount(null)).toBe("-");
  });
});
