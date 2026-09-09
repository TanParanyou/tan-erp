import { describe, expect, it } from "vitest";
import { customerSiteListQueryKey } from "./site-queries";

describe("site business query keys", () => {
  it("distinguishes customer IDs", () => {
    expect(customerSiteListQueryKey("membership-a", "th", "cust-1")).not.toEqual(
      customerSiteListQueryKey("membership-a", "th", "cust-2")
    );
  });

  it("distinguishes memberships", () => {
    expect(customerSiteListQueryKey("membership-a", "th", "cust-1")).not.toEqual(
      customerSiteListQueryKey("membership-b", "th", "cust-1")
    );
  });

  it("distinguishes locales", () => {
    expect(customerSiteListQueryKey("membership-a", "th", "cust-1")).not.toEqual(
      customerSiteListQueryKey("membership-a", "en", "cust-1")
    );
  });

  it("namespaces business keys with membership and locale", () => {
    const key = customerSiteListQueryKey("membership-a", "th", "cust-1");
    expect(key[0]).toBe("business");
    expect(key).toContain("membership-a");
    expect(key).toContain("th");
    expect(key).toContain("sites");
  });
});
