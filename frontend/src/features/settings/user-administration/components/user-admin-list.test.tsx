import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { UserAdminList } from "./user-admin-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  result: {} as Record<string, unknown>,
  push: vi.fn(),
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/users",
  useSearchParams: () => new URLSearchParams(),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-1",
      permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })),
    },
  }),
}));

vi.mock("../api/user-admin-queries", () => ({
  useAdminUsers: () => mocks.result,
}));

const sampleUsers = [
  {
    id: "u1",
    displayName: "สมชาย ผู้ดูแล",
    email: "somchai@example.test",
    status: "active",
    rowVersion: "v1",
    memberships: [
      {
        id: "m1",
        isActive: true,
        rowVersion: "mv1",
        branch: { id: "b1", name: "สาขากรุงเทพ" },
        roles: [{ id: "r1", name: "Estimator" }],
        pendingRoleRequests: [{ id: "q1" }],
      },
    ],
  },
  {
    id: "u2",
    displayName: "ผู้ใช้ใหม่",
    email: "new@example.test",
    status: "pending",
    rowVersion: "v2",
    memberships: [],
  },
];

describe("UserAdminList", () => {
  beforeEach(() => {
    mocks.permissions = ["users.read"];
    mocks.push.mockClear();
    mocks.result = {
      data: { items: sampleUsers, pagination: { page: 1, pageSize: 25, totalCount: 2, totalPages: 1 } },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    };
  });

  it("shows users with their status, roles, branch and pending request count", () => {
    render(<UserAdminList />);

    expect(screen.getByText("สมชาย ผู้ดูแล")).toBeInTheDocument();
    expect(screen.getByText("somchai@example.test")).toBeInTheDocument();
    expect(screen.getAllByText("ใช้งานอยู่").length).toBeGreaterThan(0);
    expect(screen.getAllByText("รอเข้าระบบครั้งแรก").length).toBeGreaterThan(0);
    expect(screen.getByText("Estimator")).toBeInTheDocument();
    expect(screen.getByText(/สาขากรุงเทพ/)).toBeInTheDocument();
    expect(screen.getByText(/รออนุมัติ 1 รายการ/)).toBeInTheDocument();
  });

  it("hides the add-user action unless the caller can manage users, memberships and roles", () => {
    const { rerender } = render(<UserAdminList />);
    expect(screen.queryByRole("button", { name: "เพิ่มผู้ใช้" })).not.toBeInTheDocument();

    mocks.permissions = ["users.read", "users.manage", "memberships.manage", "roles.assign"];
    rerender(<UserAdminList />);
    expect(screen.getByRole("button", { name: "เพิ่มผู้ใช้" })).toBeInTheDocument();
  });

  it("keeps the table mounted and shows the error state with retry when loading fails", () => {
    mocks.result = { data: undefined, isLoading: false, isError: true, refetch: vi.fn() };
    render(<UserAdminList />);

    expect(screen.getByRole("table")).toBeInTheDocument();
    expect(screen.getByText("ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง")).toBeInTheDocument();
  });

  it("shows the first-use empty state only when nothing exists and no filter is applied", () => {
    mocks.result = {
      data: { items: [], pagination: { page: 1, pageSize: 25, totalCount: 0, totalPages: 0 } },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    };
    render(<UserAdminList />);

    expect(screen.getByText("ยังไม่มีผู้ใช้")).toBeInTheDocument();
  });
});
