import { describe, expect, it } from "vitest";
import { itemFormSchema } from "./item-form-schema";

const validItem = {
  code: "PRD-CHAIR-01",
  codeMode: "manual",
  itemType: "product",
  categoryId: "11111111-1111-4111-8111-111111111111",
  baseUnitId: "22222222-2222-4222-8222-222222222222",
  name: { th: "เก้าอี้", en: "Chair" },
  description: { th: "", en: "" },
  availabilityMode: "all_branches",
  selectedBranchIds: [],
  capabilities: { canSell: true, canCost: true, canPurchase: false, canStock: true, canProduce: true },
  attributes: [{ key: "color", value: "natural" }],
  imageFile: null,
  imageAltText: "",
  aliases: [],
};

describe("itemFormSchema", () => {
  it("accepts Product items and string-valued attributes", () => {
    expect(itemFormSchema.safeParse(validItem).success).toBe(true);
  });

  it("limits the internal SKU to 50 characters", () => {
    expect(itemFormSchema.safeParse({ ...validItem, code: "A".repeat(51) }).success).toBe(false);
  });

  it("allows an empty code only in generated mode", () => {
    expect(itemFormSchema.safeParse({ ...validItem, code: "", codeMode: "generated" }).success).toBe(true);
    expect(itemFormSchema.safeParse({ ...validItem, code: "", codeMode: "manual" }).success).toBe(false);
  });

  it("requires a branch when selected-branch availability is chosen", () => {
    expect(itemFormSchema.safeParse({ ...validItem, availabilityMode: "selected_branches" }).success).toBe(false);
  });

  it("rejects duplicate attribute keys and reserved keys", () => {
    expect(
      itemFormSchema.safeParse({
        ...validItem,
        attributes: [
          { key: "color", value: "natural" },
          { key: "Color", value: "black" },
        ],
      }).success
    ).toBe(false);

    expect(
      itemFormSchema.safeParse({
        ...validItem,
        attributes: [{ key: "price", value: "100" }],
      }).success
    ).toBe(false);
  });
});
