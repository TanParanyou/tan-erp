import { describe, it, expect } from "vitest";
import { cn } from "./cn";

describe("cn utility", () => {
  it("combines class strings correctly", () => {
    expect(cn("erp-btn", "erp-btn-primary")).toBe("erp-btn erp-btn-primary");
  });

  it("filters out falsy values", () => {
    expect(cn("erp-btn", false, null, undefined, "", "erp-btn-sm")).toBe("erp-btn erp-btn-sm");
  });

  it("handles conditional object notation", () => {
    expect(
      cn("erp-btn", {
        "erp-btn-primary": true,
        "erp-btn-danger": false,
      })
    ).toBe("erp-btn erp-btn-primary");
  });

  it("handles arrays and nested values", () => {
    expect(cn(["erp-btn", ["erp-btn-primary", false, "active"]])).toBe("erp-btn erp-btn-primary active");
  });
});
