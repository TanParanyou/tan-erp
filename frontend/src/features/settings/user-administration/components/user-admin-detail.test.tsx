import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { UserAdminDetail } from "./user-admin-detail";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  user: {} as Record<string, unknown>,
  revoke: vi.fn(),
  assign: vi.fn(),
  userActive: vi.fn(),
  decide: vi.fn(),
  success: vi.fn(),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-1",
      permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })),
    },
  }),
}));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }),
}));

vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useOrganizationBranches: () => ({ data: [{ id: "b1", code: "HQ", name: "สาขากรุงเทพ" }] }),
}));

vi.mock("../api/user-admin-queries", () => {
  const idle = (mutateAsync: (...args: never[]) => unknown) => () => ({ mutateAsync, isPending: false });
  return {
    useAdminUser: () => ({ data: mocks.user, isPending: false, isError: false }),
    useAdminRoles: () => ({
      data: {
        items: [
          { id: "r-estimator", name: "Estimator", assignable: true, requiresApproval: false, permissionKeys: [] },
          { id: "r-approver", name: "Approver", assignable: true, requiresApproval: true, permissionKeys: [] },
          { id: "r-super", name: "Super", assignable: false, requiresApproval: false, permissionKeys: [] },
          { id: "r-reader", name: "Reader", assignable: true, requiresApproval: false, permissionKeys: [] },
        ],
      },
    }),
    useRenameAdminUser: idle(vi.fn()),
    useSetAdminUserActive: idle((...args: never[]) => mocks.userActive(...args)),
    useSetAdminMembershipActive: idle(vi.fn()),
    useUpdateAdminMembership: idle(vi.fn()),
    useAssignAdminRole: idle((...args: never[]) => mocks.assign(...args)),
    useRevokeAdminRole: idle((...args: never[]) => mocks.revoke(...args)),
    useDecideAdminRoleRequest: idle((...args: never[]) => mocks.decide(...args)),
  };
});

function buildUser() {
  return {
    id: "u1",
    displayName: "สมชาย",
    email: "somchai@example.test",
    status: "active",
    rowVersion: "user-v1",
    memberships: [
      {
        id: "m1",
        isActive: true,
        rowVersion: "membership-v1",
        branch: { id: "b1", name: "สาขากรุงเทพ" },
        startsAtUtc: null,
        expiresAtUtc: null,
        roles: [{ id: "r-estimator", name: "Estimator" }],
        pendingRoleRequests: [
          { id: "q1", rowVersion: "request-v1", role: { id: "r-approver", name: "Approver" }, requestedBy: { id: "u9", name: "ผู้ขอ" }, requestedAtUtc: "2026-10-03T00:00:00Z" },
        ],
      },
    ],
  };
}

describe("UserAdminDetail", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["users.read", "users.manage", "memberships.manage", "roles.assign"];
    mocks.user = buildUser();
    mocks.revoke.mockResolvedValue(undefined);
    mocks.assign.mockResolvedValue({ pendingRequest: null });
    mocks.userActive.mockResolvedValue({});
    mocks.decide.mockResolvedValue({});
  });

  it("only offers actions the caller is permitted to take", () => {
    mocks.permissions = ["users.read"];
    render(<UserAdminDetail userId="u1" />);

    expect(screen.getByText("สมชาย")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "ปิดใช้งานผู้ใช้" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "ถอนบทบาท" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "มอบบทบาท" })).not.toBeInTheDocument();
  });

  it("asks for confirmation before revoking a role and sends the request only after confirming", async () => {
    render(<UserAdminDetail userId="u1" />);

    fireEvent.click(screen.getByRole("button", { name: "ถอนบทบาท" }));
    const dialog = await screen.findByRole("dialog");
    expect(within(dialog).getByText(/Estimator/)).toBeInTheDocument();
    expect(mocks.revoke).not.toHaveBeenCalled();

    fireEvent.click(within(dialog).getByRole("button", { name: "ยืนยัน" }));
    await waitFor(() => expect(mocks.revoke).toHaveBeenCalledWith({ membershipId: "m1", roleId: "r-estimator" }));
  });

  it("shows the mapped message when the backend refuses to remove the last administrator", async () => {
    mocks.revoke.mockRejectedValue(new ApiError({ status: 422, code: "LAST_ADMINISTRATOR_REQUIRED", message: "x" }));
    render(<UserAdminDetail userId="u1" />);

    fireEvent.click(screen.getByRole("button", { name: "ถอนบทบาท" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    expect(await screen.findByText(/ต้องมีผู้ดูแลที่ใช้งานอยู่อย่างน้อยหนึ่งคน/)).toBeInTheDocument();
  });

  it("only lists roles that can be assigned and are not already held or requested", () => {
    render(<UserAdminDetail userId="u1" />);

    const select = screen.getByLabelText("มอบบทบาทเพิ่ม");
    const labels = within(select).getAllByRole("option").map((option) => option.textContent);
    expect(labels).not.toContain("Estimator");
    expect(labels).not.toContain("Super");
    expect(labels).not.toContain("Approver (ต้องมีผู้ตรวจอนุมัติ)");
  });

  it("assigns a role with an idempotency key", async () => {
    render(<UserAdminDetail userId="u1" />);

    fireEvent.change(screen.getByLabelText("มอบบทบาทเพิ่ม"), { target: { value: "r-reader" } });
    fireEvent.click(screen.getByRole("button", { name: "มอบบทบาท" }));

    await waitFor(() => expect(mocks.assign).toHaveBeenCalledTimes(1));
    const call = mocks.assign.mock.calls[0][0] as { membershipId: string; roleId: string; idempotencyKey: string };
    expect(call.membershipId).toBe("m1");
    expect(call.roleId).toBe("r-reader");
    expect(call.idempotencyKey).toMatch(/[0-9a-f-]{36}/);
  });

  it("cancels a pending request with the request row version", async () => {
    render(<UserAdminDetail userId="u1" />);

    fireEvent.click(screen.getByRole("button", { name: "ยกเลิกคำขอ" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    await waitFor(() =>
      expect(mocks.decide).toHaveBeenCalledWith({ requestId: "q1", decision: "cancel", ifMatch: "request-v1" })
    );
  });

  it("deactivates the user with the user row version after confirmation", async () => {
    render(<UserAdminDetail userId="u1" />);

    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานผู้ใช้" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    await waitFor(() => expect(mocks.userActive).toHaveBeenCalledWith({ userId: "u1", active: false, ifMatch: "user-v1" }));
  });
});
