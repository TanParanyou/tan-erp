import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { KNOWN_ROLE_ADMIN_ERROR_CODES, roleAdminErrorKey } from "./role-management-errors";

function leafKeys(value: unknown, prefix = ""): string[] {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => leafKeys(child, prefix ? `${prefix}.${key}` : key));
}

describe("roleAdminErrorKey", () => {
  it("maps known backend codes and falls back to GENERIC", () => {
    expect(roleAdminErrorKey(new ApiError({ status: 409, code: "ROLE_IN_USE", message: "x" }))).toBe("ROLE_IN_USE");
    expect(roleAdminErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(roleAdminErrorKey(new Error("ROLE_IN_USE"))).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code plus GENERIC, and identical key sets", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of [...KNOWN_ROLE_ADMIN_ERROR_CODES, "GENERIC"]) {
        expect(messages.roleAdmin.errors).toHaveProperty(code);
      }
    }
    expect(leafKeys(thMessages.roleAdmin).sort()).toEqual(leafKeys(enMessages.roleAdmin).sort());
  });
});
