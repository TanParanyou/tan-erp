import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { adminErrorKey } from "./user-admin-errors";

describe("adminErrorKey", () => {
  it("uses the backend code when it has a dedicated message", () => {
    const error = new ApiError({ status: 422, code: "LAST_ADMINISTRATOR_REQUIRED", message: "x" });
    expect(adminErrorKey(error)).toBe("LAST_ADMINISTRATOR_REQUIRED");
  });

  it("falls back to the generic message for unknown codes and non-API errors", () => {
    expect(adminErrorKey(new ApiError({ status: 500, code: "SOMETHING_NEW", message: "x" }))).toBe("GENERIC");
    expect(adminErrorKey(new Error("boom"))).toBe("GENERIC");
    expect(adminErrorKey(null)).toBe("GENERIC");
  });

  it("has a Thai and English message for every mapped code", () => {
    const codes = [
      "USER_EMAIL_ALREADY_EXISTS",
      "ADMIN_VERSION_CONFLICT",
      "LAST_ADMINISTRATOR_REQUIRED",
      "ROLE_ESCALATION_DENIED",
      "SELF_ROLE_CHANGE_FORBIDDEN",
      "ROLE_ALREADY_ASSIGNED",
      "ROLE_ASSIGNMENT_REQUEST_PENDING",
      "ROLE_ASSIGNMENT_REQUEST_NOT_PENDING",
      "ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED",
      "ROLE_NOT_ASSIGNED",
      "USER_SHARED_ACROSS_ORGANIZATIONS",
      "BRANCH_NOT_FOUND",
      "BRANCH_INACTIVE",
      "PERMISSION_DENIED",
      "RESOURCE_NOT_FOUND",
    ];
    for (const code of codes) {
      const error = new ApiError({ status: 400, code, message: "x" });
      const key = adminErrorKey(error);
      expect(key).toBe(code);
      expect((thMessages.userAdmin.errors as Record<string, string>)[key]).toBeTruthy();
      expect((enMessages.userAdmin.errors as Record<string, string>)[key]).toBeTruthy();
    }
  });
});
