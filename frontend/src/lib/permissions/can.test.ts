import { describe, it, expect } from "vitest";
import { can, type MembershipDto } from "./can";

describe("can() permission helper", () => {
  const membershipWithOrgPermissions: MembershipDto = {
    id: "019a3cf8-96f0-7c9f-b207-93aa818f4a11",
    organization: {
      id: "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
      name: "Org A",
    },
    permissions: [
      {
        key: "customers.read",
        scope: "organization",
        scopeId: "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
      },
      {
        key: "customers.create",
        scope: "organization",
        scopeId: "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
      },
      {
        key: "branch.view",
        scope: "branch",
        scopeId: "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
      },
      {
        key: "own.profile",
        scope: "own",
      },
    ],
  };

  it("returns true when required permission exists with organization scope", () => {
    expect(can(membershipWithOrgPermissions, "customers.read")).toBe(true);
    expect(can(membershipWithOrgPermissions, "customers.create")).toBe(true);
  });

  it("returns false for permission not in list", () => {
    expect(can(membershipWithOrgPermissions, "customers.delete")).toBe(false);
  });

  it("returns false when permission scope is branch or own", () => {
    expect(can(membershipWithOrgPermissions, "branch.view")).toBe(false);
    expect(can(membershipWithOrgPermissions, "own.profile")).toBe(false);
  });

  it("returns false for null or undefined membership or empty permissions", () => {
    expect(can(null, "customers.read")).toBe(false);
    expect(can(undefined, "customers.read")).toBe(false);
    expect(can({ id: "id-1" }, "customers.read")).toBe(false);
    expect(can({ id: "id-1", permissions: [] }, "customers.read")).toBe(false);
  });
});
