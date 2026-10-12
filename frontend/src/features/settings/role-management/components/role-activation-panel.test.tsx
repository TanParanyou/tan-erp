import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import type { RoleDefinitionResponse } from "@/lib/api/api-client";
import { RoleActivationPanel } from "./role-activation-panel";

const mocks = vi.hoisted(() => ({ setActive: vi.fn(), success: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/role-management-queries", () => ({
  useSetRoleDefinitionActive: () => ({ mutateAsync: mocks.setActive, isPending: false }),
}));

const role = (over: Partial<RoleDefinitionResponse>): RoleDefinitionResponse => ({
  id: "r1", name: "Sales", description: null, isSystem: false, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-11T00:00:00Z",
  memberCount: 0, pendingAssignmentRequestCount: 0, requiresApproval: false, permissions: [], ...over,
});

describe("RoleActivationPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.setActive.mockResolvedValue({});
  });

  it("deactivates an unused role only after confirmation, sending the row version", async () => {
    render(<RoleActivationPanel role={role({})} canManage />);

    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" }));
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ roleId: "r1", active: false, ifMatch: "v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ปิดใช้งานบทบาทแล้ว"));
  });

  it("keeps deactivation disabled and explains why when users hold the role or a change is pending", () => {
    const { rerender } = render(<RoleActivationPanel role={role({ memberCount: 3 })} canManage />);
    expect(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" })).toBeDisabled();
    expect(screen.getByText("ผู้ถือบทบาท 3 คน")).toBeInTheDocument();

    rerender(<RoleActivationPanel role={role({ pendingChange: { id: "q1", rowVersion: "q" } as RoleDefinitionResponse["pendingChange"] })} canManage />);
    expect(screen.getByRole("button", { name: "ปิดใช้งานบทบาท" })).toBeDisabled();
  });

  it("offers activation for an inactive role, shows server errors, and renders nothing for system roles", async () => {
    mocks.setActive.mockRejectedValueOnce(new ApiError({ status: 409, code: "ADMIN_VERSION_CONFLICT", message: "x" }));
    const { rerender } = render(<RoleActivationPanel role={role({ isActive: false })} canManage />);

    fireEvent.click(screen.getByRole("button", { name: "เปิดใช้งานบทบาท" }));
    fireEvent.click(await screen.findByRole("button", { name: "เปิดใช้งาน" }));
    expect(await screen.findByText("ข้อมูลถูกแก้ไขโดยผู้อื่นแล้ว กรุณาโหลดใหม่แล้วลองอีกครั้ง")).toBeInTheDocument();

    rerender(<RoleActivationPanel role={role({ isSystem: true })} canManage />);
    expect(screen.queryByRole("button", { name: "ปิดใช้งานบทบาท" })).not.toBeInTheDocument();
  });
});
