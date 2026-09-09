import { describe, expect, it } from "vitest";
import { opportunityListQueryKey, opportunityDetailQueryKey } from "./opportunity-queries";

describe("opportunity business query keys", () => {
  it("distinguishes filters", () => {
    expect(opportunityListQueryKey("membership-a", "th", { stage: "draft", limit: 10 })).not.toEqual(
      opportunityListQueryKey("membership-a", "th", { stage: "draft", limit: 25 })
    );
    expect(opportunityListQueryKey("membership-a", "th", { customerId: "c1" })).not.toEqual(
      opportunityListQueryKey("membership-a", "th", { customerId: "c2" })
    );
  });

  it("distinguishes detail opportunities", () => {
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-a", "th", "opp-2")
    );
  });

  it("distinguishes memberships and locales", () => {
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-b", "th", "opp-1")
    );
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-a", "en", "opp-1")
    );
  });

  it("namespaces business keys with membership and locale", () => {
    const key = opportunityListQueryKey("membership-a", "th", { stage: "draft" });
    expect(key[0]).toBe("business");
    expect(key).toContain("membership-a");
    expect(key).toContain("th");
    expect(key).toContain("opportunities");
  });
});
