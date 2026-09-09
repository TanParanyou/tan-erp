import { describe, it, expect } from "vitest";
import { createOpportunityFormSchema } from "./opportunity-form-schema";

const mockT = (key: "required" | "invalidFormat" | "invalidNumber" | "positiveNumber" | "actionDateNotePairRequired") => `error.${key}`;

describe("createOpportunityFormSchema", () => {
  const schema = createOpportunityFormSchema(mockT);

  const validData = {
    customerId: "10000000-0000-0000-0000-000000000001",
    primarySiteId: "20000000-0000-0000-0000-000000000001",
    title: "โครงการปรับปรุงสำนักงานใหญ่",
    scopeSummary: "ปรับปรุงห้องประชุมและพื้นที่ส่วนกลาง",
    workTypes: ["built-in", "interior"],
    sourceCode: "referral",
    expectedBudget: 500000,
    currencyCode: "THB",
    targetDecisionDate: "2026-10-15",
    nextActionAtUtc: "2026-09-20T10:00",
    nextActionNote: "นัดประชุมนำเสนอแบบร่าง",
  };

  it("passes for valid complete opportunity data", () => {
    const result = schema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it("passes for minimal valid opportunity data with default currency THB", () => {
    const minimalData = {
      customerId: "10000000-0000-0000-0000-000000000001",
      title: "โครงการบ้านพักอาศัย",
      workTypes: ["curtain"],
    };
    const result = schema.safeParse(minimalData);
    expect(result.success).toBe(true);
  });

  it("fails when customerId is missing or invalid UUID", () => {
    const invalidCustomer = { ...validData, customerId: "" };
    const result = schema.safeParse(invalidCustomer);
    expect(result.success).toBe(false);
  });

  it("fails when title is empty or exceeds 250 characters", () => {
    const emptyTitle = { ...validData, title: "" };
    expect(schema.safeParse(emptyTitle).success).toBe(false);

    const longTitle = { ...validData, title: "a".repeat(251) };
    expect(schema.safeParse(longTitle).success).toBe(false);
  });

  it("fails when workTypes is empty or contains non-canonical work type", () => {
    const emptyWorkTypes = { ...validData, workTypes: [] };
    expect(schema.safeParse(emptyWorkTypes).success).toBe(false);

    const invalidWorkType = { ...validData, workTypes: ["invalid-type"] };
    expect(schema.safeParse(invalidWorkType).success).toBe(false);
  });

  it("validates positive budget requires THB currency", () => {
    const nonPositiveBudget = { ...validData, expectedBudget: 0 };
    expect(schema.safeParse(nonPositiveBudget).success).toBe(false);

    const negativeBudget = { ...validData, expectedBudget: -100 };
    expect(schema.safeParse(negativeBudget).success).toBe(false);

    const missingCurrencyWhenBudgetExists = { ...validData, expectedBudget: 10000, currencyCode: "" };
    expect(schema.safeParse(missingCurrencyWhenBudgetExists).success).toBe(false);
  });

  it("validates targetDecisionDate format (YYYY-MM-DD)", () => {
    const invalidDate = { ...validData, targetDecisionDate: "2026/10/15" };
    expect(schema.safeParse(invalidDate).success).toBe(false);
  });

  it("validates nextActionAtUtc and nextActionNote pairing", () => {
    const dateWithoutNote = {
      ...validData,
      nextActionAtUtc: "2026-09-20T10:00",
      nextActionNote: "",
    };
    expect(schema.safeParse(dateWithoutNote).success).toBe(false);

    const noteWithoutDate = {
      ...validData,
      nextActionAtUtc: "",
      nextActionNote: "นัดหมายล่วงหน้า",
    };
    expect(schema.safeParse(noteWithoutDate).success).toBe(false);

    const neitherProvided = {
      ...validData,
      nextActionAtUtc: "",
      nextActionNote: "",
    };
    expect(schema.safeParse(neitherProvided).success).toBe(true);
  });

  it("allows optional primarySiteId to be omitted or empty", () => {
    const noSite = { ...validData, primarySiteId: "" };
    const result = schema.safeParse(noSite);
    expect(result.success).toBe(true);
  });
});
