import { describe, expect, it } from "vitest";
import { customerListQueryKey, customerDetailQueryKey } from "./customer-queries";

describe("customer business query keys", () => {
  it("distinguishes list limit filters", () => {
    expect(customerListQueryKey("membership-a", "th", { status: "draft", limit: 10 })).not.toEqual(
      customerListQueryKey("membership-a", "th", { status: "draft", limit: 25 }),
    );
  });

  it("distinguishes detail memberships", () => {
    expect(customerDetailQueryKey("membership-a", "th", "customer-1")).not.toEqual(
      customerDetailQueryKey("membership-b", "th", "customer-1"),
    );
  });

  it("distinguishes detail locales", () => {
    expect(customerDetailQueryKey("membership-a", "th", "customer-1")).not.toEqual(
      customerDetailQueryKey("membership-a", "en", "customer-1"),
    );
  });

  it("namespaces business keys with membership and locale", () => {
    const key = customerListQueryKey("membership-a", "th", { status: "draft", limit: 10 });
    expect(key[0]).toBe("business");
    expect(key).toContain("membership-a");
    expect(key).toContain("th");
  });
});
