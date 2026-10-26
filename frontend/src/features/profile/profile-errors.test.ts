import { describe, expect, it } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import { passwordErrorKey, profileErrorKey } from "./profile-errors";

describe("profileErrorKey", () => {
  it("maps known API codes", () => {
    const error = new ApiError({ status: 404, code: "FILE_NOT_FOUND", message: "x" });
    expect(profileErrorKey(error)).toBe("FILE_NOT_FOUND");
  });

  it("falls back to GENERIC for unknown errors", () => {
    expect(profileErrorKey(new Error("boom"))).toBe("GENERIC");
    expect(profileErrorKey(new ApiError({ status: 500, code: "SOMETHING", message: "x" }))).toBe("GENERIC");
  });
});

describe("passwordErrorKey", () => {
  it.each([
    ["auth/wrong-password", "WRONG_PASSWORD"],
    ["auth/invalid-credential", "WRONG_PASSWORD"],
    ["auth/weak-password", "WEAK_PASSWORD"],
    ["auth/requires-recent-login", "REQUIRES_RECENT_LOGIN"],
    ["auth/too-many-requests", "TOO_MANY_REQUESTS"],
    ["auth/other", "GENERIC"],
  ])("maps %s to %s", (code, expected) => {
    expect(passwordErrorKey({ code })).toBe(expected);
  });

  it("returns GENERIC for non-object errors", () => {
    expect(passwordErrorKey("nope")).toBe("GENERIC");
  });
});
