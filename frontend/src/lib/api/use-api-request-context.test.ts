import { describe, expect, it, vi, beforeEach } from "vitest";
import { renderHook } from "@testing-library/react";
import { AuthenticationRequiredError, MembershipRequiredError } from "./api-error";
import { useApiRequestContext } from "./use-api-request-context";

const mocks = vi.hoisted(() => ({
  token: "token-1" as string | null,
  membershipId: "membership-1" as string | undefined,
  locale: "en",
}));

vi.mock("@/lib/auth/auth-session", () => ({ getAuthToken: () => Promise.resolve(mocks.token) }));
vi.mock("@/lib/i18n/i18n-context", () => ({ useSafeLocale: () => mocks.locale }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: mocks.membershipId ? { id: mocks.membershipId } : null }),
}));

describe("useApiRequestContext", () => {
  beforeEach(() => {
    mocks.token = "token-1";
    mocks.membershipId = "membership-1";
    mocks.locale = "en";
  });

  it("builds options from the session, membership and locale and merges extras", async () => {
    const { result } = renderHook(() => useApiRequestContext());

    expect(result.current.membershipId).toBe("membership-1");
    expect(result.current.locale).toBe("en");
    await expect(result.current.buildOptions({ ifMatch: "v1" })).resolves.toEqual({
      token: "token-1",
      membershipId: "membership-1",
      locale: "en",
      ifMatch: "v1",
    });
  });

  it("normalizes unsupported locales to Thai", () => {
    mocks.locale = "fr";
    const { result } = renderHook(() => useApiRequestContext());

    expect(result.current.locale).toBe("th");
  });

  it("rejects when there is no session or no selected membership", async () => {
    mocks.token = null;
    const noSession = renderHook(() => useApiRequestContext());
    await expect(noSession.result.current.buildOptions()).rejects.toBeInstanceOf(AuthenticationRequiredError);

    mocks.token = "token-1";
    mocks.membershipId = undefined;
    const noMembership = renderHook(() => useApiRequestContext());
    expect(noMembership.result.current.membershipId).toBeUndefined();
    await expect(noMembership.result.current.buildOptions()).rejects.toBeInstanceOf(MembershipRequiredError);
  });
});
