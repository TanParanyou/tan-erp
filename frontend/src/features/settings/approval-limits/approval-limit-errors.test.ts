import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { KNOWN_APPROVAL_LIMIT_ERROR_CODES, approvalLimitErrorKey } from "./approval-limit-errors";

function leafKeys(value: unknown, prefix = ""): string[] {
  if (typeof value !== "object" || value === null) return [prefix];
  return Object.entries(value).flatMap(([key, child]) => leafKeys(child, prefix ? `${prefix}.${key}` : key));
}

describe("approvalLimitErrorKey", () => {
  it("maps known backend codes and falls back to GENERIC", () => {
    expect(approvalLimitErrorKey(new ApiError({ status: 409, code: "APPROVAL_LIMIT_ALREADY_EXISTS", message: "x" }))).toBe("APPROVAL_LIMIT_ALREADY_EXISTS");
    expect(approvalLimitErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(approvalLimitErrorKey(new Error("APPROVAL_LIMIT_ALREADY_EXISTS"))).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code plus GENERIC, and identical key sets", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of [...KNOWN_APPROVAL_LIMIT_ERROR_CODES, "GENERIC"]) {
        expect(messages.approvalLimitAdmin.errors).toHaveProperty(code);
      }
    }
    expect(leafKeys(thMessages.approvalLimitAdmin).sort()).toEqual(leafKeys(enMessages.approvalLimitAdmin).sort());
  });
});
