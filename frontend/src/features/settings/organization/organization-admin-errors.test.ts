import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { KNOWN_ORGANIZATION_ADMIN_ERROR_CODES, organizationAdminErrorKey } from "./organization-admin-errors";

function leafKeys(value: unknown, prefix = ""): string[] {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => leafKeys(child, prefix ? `${prefix}.${key}` : key));
}

describe("organizationAdminErrorKey", () => {
  it("maps known backend codes and falls back to GENERIC", () => {
    expect(organizationAdminErrorKey(new ApiError({ status: 409, code: "BRANCH_LAST_ACTIVE", message: "x" }))).toBe("BRANCH_LAST_ACTIVE");
    expect(organizationAdminErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(organizationAdminErrorKey(new Error("BRANCH_LAST_ACTIVE"))).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code plus GENERIC", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of [...KNOWN_ORGANIZATION_ADMIN_ERROR_CODES, "GENERIC"]) {
        expect(messages.organizationAdmin.errors).toHaveProperty(code);
      }
    }
  });

  it("keeps the organizationAdmin namespace identical in Thai and English", () => {
    expect(leafKeys(thMessages.organizationAdmin).sort()).toEqual(leafKeys(enMessages.organizationAdmin).sort());
  });

  it("has a blocker label for every backend blocker type", () => {
    const types = ["estimates", "quotations", "purchase_orders", "billings", "work_orders", "projects", "installations",
      "site_surveys", "opportunities", "quick_estimates", "mrp_runs", "warehouses", "memberships", "last_active_branch"];
    for (const type of types) {
      expect(thMessages.organizationAdmin.branches.blockers).toHaveProperty(type);
      expect(enMessages.organizationAdmin.branches.blockers).toHaveProperty(type);
    }
  });
});
