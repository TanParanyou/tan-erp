import { describe, it, expect } from "vitest";
import {
  validateAndMapCatalogItem,
  validateAndMapCatalogResponse,
  EstimateCatalogContractError,
  getLocalizedText,
} from "./estimate-catalog-client";

describe("estimate-catalog-client", () => {
  const validRawItem = {
    id: "019a3cf8-0001-7000-8000-000000000001",
    code: "MAT-WOOD-001",
    name: { thai: "ไม้อัดยาง 10มม.", english: "Plywood 10mm" },
    description: { thai: "เกรด A", english: "Grade A" },
    itemType: "material",
    category: {
      id: "019a3cf8-0002-7000-8000-000000000002",
      code: "WOOD",
      name: { thai: "ไม้และแผ่นบอร์ด", english: "Wood and Boards" },
      parentCategoryId: null,
    },
    brand: {
      id: "019a3cf8-0003-7000-8000-000000000003",
      code: "SCG",
      name: { thai: "เอสซีจี", english: "SCG" },
    },
    baseUnit: {
      id: "019a3cf8-0004-7000-8000-000000000004",
      code: "SHT",
      name: { thai: "แผ่น", english: "Sheet" },
      symbol: "sht",
    },
    primaryImage: {
      fileId: "019a3cf8-0005-7000-8000-000000000005",
      altText: { thai: "รูปแผ่นไม้อัด", english: "Plywood image" },
    },
    resolvedCost: {
      costRecordId: "019a3cf8-0006-7000-8000-000000000006",
      version: 1,
      amount: 450.5,
      currency: "THB",
      unitCode: "SHT",
      scope: "organization",
      effectiveFromUtc: "2026-09-01T00:00:00Z",
      policyVersion: "v1",
    },
  };

  it("successfully validates and maps a valid catalog item", () => {
    const result = validateAndMapCatalogItem(validRawItem);
    expect(result.id).toBe(validRawItem.id);
    expect(result.code).toBe("MAT-WOOD-001");
    expect(result.resolvedCost?.amount).toBe(450.5);
    expect(result.primaryImage?.fileId).toBe("019a3cf8-0005-7000-8000-000000000005");
  });

  it("throws EstimateCatalogContractError when required fields are missing", () => {
    // Missing id
    expect(() => validateAndMapCatalogItem({ ...validRawItem, id: undefined })).toThrow(
      EstimateCatalogContractError
    );

    // Missing code
    expect(() => validateAndMapCatalogItem({ ...validRawItem, code: "" })).toThrow(
      EstimateCatalogContractError
    );

    // Missing thai name
    expect(() =>
      validateAndMapCatalogItem({ ...validRawItem, name: { thai: "", english: "Only English" } })
    ).toThrow(EstimateCatalogContractError);

    // Missing category
    expect(() => validateAndMapCatalogItem({ ...validRawItem, category: undefined })).toThrow(
      EstimateCatalogContractError
    );

    // Missing baseUnit
    expect(() => validateAndMapCatalogItem({ ...validRawItem, baseUnit: undefined })).toThrow(
      EstimateCatalogContractError
    );
  });

  it("throws when resolvedCost has invalid contract", () => {
    const badCostItem = {
      ...validRawItem,
      resolvedCost: {
        costRecordId: "",
        amount: "not a number",
      },
    };
    expect(() => validateAndMapCatalogItem(badCostItem)).toThrow(EstimateCatalogContractError);
  });

  it("maps catalog response with facets and pagination", () => {
    const rawResponse = {
      items: [validRawItem],
      facets: {
        itemTypes: [{ value: "material", count: 1 }],
        categories: [
          {
            id: validRawItem.category.id,
            name: validRawItem.category.name,
            count: 1,
          },
        ],
        brands: [],
      },
      pageInfo: {
        nextCursor: "cursor-token-123",
        hasNextPage: true,
      },
    };

    const mapped = validateAndMapCatalogResponse(rawResponse);
    expect(mapped.items).toHaveLength(1);
    expect(mapped.facets.itemTypes).toHaveLength(1);
    expect(mapped.pageInfo.nextCursor).toBe("cursor-token-123");
    expect(mapped.pageInfo.hasNextPage).toBe(true);
  });

  describe("getLocalizedText", () => {
    it("returns English when locale is en and english text is present", () => {
      const text = { thai: "ไม้", english: "Wood" };
      expect(getLocalizedText(text, "en")).toBe("Wood");
    });

    it("falls back to Thai when locale is en but english is empty", () => {
      const text = { thai: "ไม้", english: "" };
      expect(getLocalizedText(text, "en")).toBe("ไม้");
    });

    it("returns Thai when locale is th", () => {
      const text = { thai: "ไม้", english: "Wood" };
      expect(getLocalizedText(text, "th")).toBe("ไม้");
    });

    it("returns custom fallback when both are empty", () => {
      expect(getLocalizedText(null, "th", "-")).toBe("-");
    });
  });
});
