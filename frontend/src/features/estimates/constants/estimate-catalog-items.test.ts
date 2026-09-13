import { describe, it, expect } from "vitest";
import {
  ESTIMATE_CATALOG_ITEMS,
  filterCatalogItems,
} from "./estimate-catalog-items";

describe("estimate-catalog-items", () => {
  it("contains valid catalog items with required business fields and localized json layer", () => {
    expect(ESTIMATE_CATALOG_ITEMS.length).toBeGreaterThan(10);
    ESTIMATE_CATALOG_ITEMS.forEach((item) => {
      expect(item.id).toBeTruthy();
      expect(item.code).toBeTruthy();
      expect(item.name.th).toBeTruthy();
      expect(item.name.en).toBeTruthy();
      expect(item.brand.name.th).toBeTruthy();
      expect(item.brand.name.en).toBeTruthy();
      expect(item.category.name.th).toBeTruthy();
      expect(item.category.name.en).toBeTruthy();
      expect(item.pricing.defaultUnitCost).toBeGreaterThan(0);
      expect(item.pricing.baseUnitCode).toBeTruthy();
    });
  });

  it("filters items by free-text search correctly", () => {
    const results = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, { search: "HMR" });
    expect(results.length).toBeGreaterThan(0);
    results.forEach((item) => {
      const match =
        item.code.includes("HMR") ||
        item.name.th.includes("HMR") ||
        item.name.en.includes("HMR");
      expect(match).toBe(true);
    });
  });

  it("finds items by colloquial alias (คำที่ผู้ใช้เรียก / colloquial terms)", () => {
    // "ไม้เขียว" is an alias for HMR board
    const results = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, { search: "ไม้เขียว" });
    expect(results.length).toBeGreaterThan(0);
    expect(results.some((item) => item.code.includes("HMR"))).toBe(true);

    // "ไฟริบบิ้น" is an alias for LED Strip
    const ledResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, { search: "ไฟริบบิ้น" });
    expect(ledResults.length).toBeGreaterThan(0);
    expect(ledResults.some((item) => item.code.includes("LED"))).toBe(true);
  });

  it("filters items by dynamic attributes (e.g. size, finish, grade, standard)", () => {
    // Tile 60x60 with Matt finish
    const tileResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      attributes: {
        size: "60x60",
        finish: "Matt",
      },
    });
    expect(tileResults.length).toBeGreaterThan(0);
    tileResults.forEach((item) => {
      expect(item.attributes?.size).toBe("60x60");
      expect(item.attributes?.finish).toBe("Matt");
    });

    // Steel DB12 with Grade SD40 and TIS standard
    const steelResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      attributes: {
        grade: "SD40",
        standard: "TIS",
      },
    });
    expect(steelResults.length).toBeGreaterThan(0);
    steelResults.forEach((item) => {
      expect(item.attributes?.grade).toBe("SD40");
      expect(item.attributes?.standard).toBe("TIS");
    });
  });

  it("filters items by subCategory", () => {
    const hingeResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      category: "fitting",
      subCategory: "hinge",
    });
    expect(hingeResults.length).toBeGreaterThan(0);
    hingeResults.forEach((item) => {
      expect(item.category.id).toBe("fitting");
      expect(item.subCategory?.id).toBe("hinge");
    });
  });

  it("filters items by supplier / vendor", () => {
    const supplierResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      suppliers: ["sup-scg-dist"],
    });
    expect(supplierResults.length).toBeGreaterThan(0);
    supplierResults.forEach((item) => {
      expect(item.supplier?.id).toBe("sup-scg-dist");
    });
  });

  it("filters items by thickness facet", () => {
    const results = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      thicknesses: ["18mm"],
    });
    expect(results.length).toBeGreaterThan(0);
    results.forEach((item) => {
      expect(item.specs?.thickness).toBe("18mm");
    });
  });

  it("filters items by multiple tiers: category, brand and itemType", () => {
    const results = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      category: "fitting",
      brands: ["blum"],
      itemTypes: ["material"],
    });

    expect(results.length).toBeGreaterThan(0);
    results.forEach((item) => {
      expect(item.category.id).toBe("fitting");
      expect(item.brand.id).toBe("blum");
      expect(item.itemType).toBe("material");
    });
  });

  it("returns empty array when filter criteria does not match any item", () => {
    const results = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      search: "NON_EXISTENT_MATERIAL_XYZ",
    });
    expect(results).toHaveLength(0);
  });

  it("supports tokenized multi-keyword search across names, specs, brand and category", () => {
    // "ไม้อัด 18mm vanachai" matches HMR board with brand Vanachai and thickness 18mm
    const multiWordResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      search: "ไม้อัด 18mm vanachai",
    });
    expect(multiWordResults.length).toBeGreaterThan(0);
    multiWordResults.forEach((item) => {
      expect(item.brand.name.en.toLowerCase()).toBe("vanachai");
      expect(item.specs?.thickness).toBe("18mm");
    });
  });

  it("finds items by searching category name or grade spec", () => {
    const catResults = filterCatalogItems(ESTIMATE_CATALOG_ITEMS, {
      search: "ฟิตติ้ง",
    });
    expect(catResults.length).toBeGreaterThan(0);
  });
});
