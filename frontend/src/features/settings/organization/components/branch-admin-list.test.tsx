import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { BranchAdminList } from "./branch-admin-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  push: vi.fn(),
  status: undefined as string | undefined,
  result: {} as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/branches",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/organization-admin-queries", () => ({
  useAdminBranches: (status: string | undefined) => {
    mocks.status = status;
    return mocks.result;
  },
}));

const rows = [
  { id: "b1", code: "HQ", name: "สำนักงานใหญ่", taxBranchCode: "00000", isActive: true, rowVersion: "v1" },
  { id: "b2", code: "CM", name: "เชียงใหม่", taxBranchCode: null, isActive: false, rowVersion: "v2" },
];

describe("BranchAdminList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["branches.manage"];
    mocks.status = undefined;
    mocks.result = { data: rows, isLoading: false, isError: false, refetch: vi.fn() };
  });

  it("lists active and inactive branches with tax branch code and status", () => {
    render(<BranchAdminList />);

    expect(screen.getByText("สำนักงานใหญ่")).toBeInTheDocument();
    expect(screen.getByText("00000")).toBeInTheDocument();
    expect(screen.getByText("ใช้งาน")).toBeInTheDocument();
    expect(screen.getByText("ปิดใช้งาน")).toBeInTheDocument();
    expect(screen.getAllByText("-").length).toBeGreaterThan(0); // missing tax branch code is shown as "-", never guessed
  });

  it("links each branch name to its detail page", () => {
    render(<BranchAdminList />);

    expect(screen.getByRole("link", { name: /เชียงใหม่/ })).toHaveAttribute("href", "/th/settings/branches/b2");
  });

  it("routes the add button to the create form", () => {
    render(<BranchAdminList />);
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));
    expect(mocks.push).toHaveBeenCalledWith("/th/settings/branches/create");
  });

  it("hides the add button without branches.manage", () => {
    mocks.permissions = [];
    render(<BranchAdminList />);

    expect(screen.queryByRole("button", { name: "เพิ่มสาขา" })).not.toBeInTheDocument();
  });

  it("shows an empty state when there are no branches", () => {
    mocks.result = { data: [], isLoading: false, isError: false, refetch: vi.fn() };
    render(<BranchAdminList />);

    expect(screen.getByText("ยังไม่มีสาขา")).toBeInTheDocument();
  });
});
