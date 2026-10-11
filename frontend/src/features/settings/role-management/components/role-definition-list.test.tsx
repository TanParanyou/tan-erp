import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { RoleDefinitionList } from "./role-definition-list";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  push: vi.fn(),
  params: undefined as Record<string, unknown> | undefined,
  result: {} as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/roles",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/role-management-queries", () => ({
  useRoleDefinitions: (params: Record<string, unknown>) => {
    mocks.params = params;
    return mocks.result;
  },
}));

const items = [
  { id: "r1", name: "Test Admin", description: "ผู้ดูแล", isSystem: true, isActive: true, rowVersion: "v1", memberCount: 2, permissionCount: 40, requiresApproval: true, hasPendingChange: false },
  { id: "r2", name: "ผู้ประเมิน", description: null, isSystem: false, isActive: false, rowVersion: "v2", memberCount: 0, permissionCount: 3, requiresApproval: false, hasPendingChange: true },
];

describe("RoleDefinitionList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["roles.manage"];
    mocks.result = {
      data: { items, pagination: { page: 1, pageSize: 25, totalCount: 2, totalPages: 1 } },
      isLoading: false, isError: false, refetch: vi.fn(),
    };
  });

  it("lists roles with system, approval and pending-change badges and links to the editor", () => {
    render(<RoleDefinitionList />);

    expect(screen.getByRole("link", { name: /Test Admin/ })).toHaveAttribute("href", "/th/settings/roles/r1");
    expect(screen.getByText("ระบบ")).toBeInTheDocument();
    expect(screen.getByText("ต้องมีผู้ตรวจอนุมัติ")).toBeInTheDocument();
    expect(screen.getByText("รอการอนุมัติการแก้ไข")).toBeInTheDocument();
    expect(screen.getAllByText("ปิดใช้งาน").length).toBeGreaterThan(0); // the inactive role's status badge (the filter also lists it)
  });

  it("asks the server for a sorted, paged list and routes the add button to the create form", () => {
    render(<RoleDefinitionList />);

    expect(mocks.params).toMatchObject({ sortBy: "name", sortOrder: "asc", page: 1 });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));
    expect(mocks.push).toHaveBeenCalledWith("/th/settings/roles/create");
  });

  it("hides the add button without roles.manage", () => {
    mocks.permissions = [];
    render(<RoleDefinitionList />);

    expect(screen.queryByRole("button", { name: "เพิ่มบทบาท" })).not.toBeInTheDocument();
  });
});
