import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { ApprovalLimitList } from "./approval-limit-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  push: vi.fn(),
  params: undefined as Record<string, unknown> | undefined,
  result: {} as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/approval-limits",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/approval-limit-queries", () => ({
  useApprovalLimits: (params: Record<string, unknown>) => {
    mocks.params = params;
    return mocks.result;
  },
}));

const items = [
  { id: "l1", rowVersion: "v1", role: { id: "r1", name: "ผู้จัดการ" }, documentType: "estimate", branch: null, currency: "THB", isUnlimited: false, maxAmount: 500000, isActive: true },
  { id: "l2", rowVersion: "v2", role: { id: "r2", name: "ผู้อำนวยการ" }, documentType: "purchase_order", branch: { id: "b1", name: "สาขา 1" }, currency: "THB", isUnlimited: true, maxAmount: null, isActive: false },
];

describe("ApprovalLimitList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["approval-limits.manage"];
    mocks.result = {
      data: { items, pagination: { page: 1, pageSize: 25, totalCount: 2, totalPages: 1 } },
      isLoading: false, isError: false, refetch: vi.fn(),
    };
  });

  it("lists limits, routes the add button to the create form and hides it without approval-limits.manage", () => {
    const { unmount } = render(<ApprovalLimitList />);

    expect(screen.getByRole("link", { name: /ผู้จัดการ/ })).toHaveAttribute("href", "/th/settings/approval-limits/l1");
    expect(screen.getAllByText("ใบสั่งซื้อ").length).toBeGreaterThan(0); // the filter also lists each type
    expect(screen.getByText("ทั้งองค์กร")).toBeInTheDocument();
    expect(screen.getByText("สาขา 1")).toBeInTheDocument();
    expect(screen.getByText("ไม่จำกัด")).toBeInTheDocument();
    expect(screen.getByText(/500,000/)).toBeInTheDocument();
    expect(mocks.params).toMatchObject({ page: 1 });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มวงเงิน" }));
    expect(mocks.push).toHaveBeenCalledWith("/th/settings/approval-limits/create");
    unmount();

    mocks.permissions = [];
    render(<ApprovalLimitList />);
    expect(screen.queryByRole("button", { name: "เพิ่มวงเงิน" })).toBeNull();
  });
});
