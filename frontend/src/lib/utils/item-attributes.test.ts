import { describe, it, expect } from "vitest";
import {
  recordToAttributePairs,
  attributePairsToRecord,
  validateAttributePairs,
  getAttributeKeyLabel,
  formatAttributesToText,
} from "./item-attributes";

describe("item-attributes helpers", () => {
  it("converts record to attribute pairs correctly", () => {
    const record = { thickness_mm: "18", color: "white" };
    const pairs = recordToAttributePairs(record);
    expect(pairs).toEqual([
      { key: "thickness_mm", value: "18" },
      { key: "color", value: "white" },
    ]);
  });

  it("handles empty or null record gracefully", () => {
    expect(recordToAttributePairs(null)).toEqual([]);
    expect(recordToAttributePairs(undefined)).toEqual([]);
  });

  it("converts attribute pairs to clean record and trims keys/values", () => {
    const pairs = [
      { key: " thickness_mm ", value: " 18 " },
      { key: "", value: "ignored" },
      { key: "   ", value: "ignored too" },
      { key: "color", value: "white" },
    ];
    const record = attributePairsToRecord(pairs);
    expect(record).toEqual({
      thickness_mm: "18",
      color: "white",
    });
  });

  it("validates attribute pairs: detects duplicate keys case-insensitively", () => {
    const pairs = [
      { key: "Color", value: "white" },
      { key: "color", value: "black" },
    ];
    const result = validateAttributePairs(pairs);
    expect(result.isValid).toBe(false);
    expect(result.duplicateKeys).toContain("color");
  });

  it("validates attribute pairs: detects reserved keys", () => {
    const pairs = [
      { key: "price", value: "100" },
      { key: "COST", value: "80" },
      { key: "material", value: "wood" },
    ];
    const result = validateAttributePairs(pairs);
    expect(result.isValid).toBe(false);
    expect(result.reservedKeys).toEqual(["price", "COST"]);
  });

  it("validates valid attribute pairs successfully", () => {
    const pairs = [
      { key: "thickness_mm", value: "18" },
      { key: "material", value: "plywood" },
    ];
    const result = validateAttributePairs(pairs);
    expect(result.isValid).toBe(true);
    expect(result.duplicateKeys).toHaveLength(0);
    expect(result.reservedKeys).toHaveLength(0);
  });

  it("gets localized label for preset keys and falls back to formatted key", () => {
    expect(getAttributeKeyLabel("thickness_mm", "th")).toBe("ความหนา (มม.)");
    expect(getAttributeKeyLabel("thickness_mm", "en")).toBe("Thickness (mm)");
    expect(getAttributeKeyLabel("custom_code_name", "th")).toBe("custom code name");
  });

  it("formats attributes to text for copy/export", () => {
    const record = { thickness_mm: "18", color: "white" };
    const text = formatAttributesToText(record, "th");
    expect(text).toContain("ความหนา (มม.): 18");
    expect(text).toContain("สี: white");
  });
});
