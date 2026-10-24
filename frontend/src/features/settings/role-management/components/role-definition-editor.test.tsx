import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { RoleDefinitionEditor } from "./role-definition-editor";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  create: vi.fn(),
  update: vi.fn(),
  push: vi.fn(),
  success: vi.fn(),
  role: { data: undefined, isPending: false, isError: false } as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/roles/create",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("./role-activation-panel", () => ({ RoleActivationPanel: () => <div>activation-panel</div> }));
vi.mock("../api/role-management-queries", () => ({
  useRoleDefinition: () => mocks.role,
  useAdminPermissionCatalog: () => ({
    isPending: false,
    isError: false,
    data: {
      items: [
        { key: "customers.read", description: "Read customers", isApprovalPermission: false },
        { key: "estimates.approve", description: "Approve estimates", isApprovalPermission: true },
      ],
    },
  }),
  useCreateRoleDefinition: () => ({ mutateAsync: mocks.create, isPending: false }),
  useUpdateRoleDefinition: () => ({ mutateAsync: mocks.update, isPending: false }),
}));

const existing = {
  id: "r1", name: "Sales Lead", description: "เดิม", isSystem: false, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-11T00:00:00Z",
  memberCount: 2, pendingAssignmentRequestCount: 0, requiresApproval: false, pendingChange: null,
  permissions: [
    { permissionKey: "customers.read", scope: "organization", branch: null },
    { permissionKey: "estimates.read", scope: "branch", branch: { id: "b1", name: "สาขา 1" } },
  ],
};

describe("RoleDefinitionEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["roles.manage"];
    mocks.role = { data: existing, isPending: false, isError: false };
    mocks.create.mockResolvedValue({ id: "new-role" });
    mocks.update.mockResolvedValue({ role: existing, pendingRequest: null });
  });

  it("validates the name before creating", async () => {
    render(<RoleDefinitionEditor roleId="create" />);

    expect(screen.getByPlaceholderText("เช่น เจ้าหน้าที่ฝ่ายขาย, ผู้จัดการโครงการ")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("ระบุหน้าที่หรือขอบเขตความรับผิดชอบของบทบาทนี้")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText(/ชื่อบทบาท/), { target: { value: "ab" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));

    expect(await screen.findByText("ชื่อบทบาทต้องมี 3-100 ตัวอักษร")).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates with the selected organization-scope permissions, an idempotency key and opens the new role", async () => {
    render(<RoleDefinitionEditor roleId="create" />);

    fireEvent.change(screen.getByLabelText(/ชื่อบทบาท/), { target: { value: "  Quote Reviewer  " } });
    fireEvent.click(screen.getByLabelText("customers.read"));
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มบทบาท" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      name: "Quote Reviewer",
      description: null,
      permissions: [{ permissionKey: "customers.read", scope: "organization", branchId: null }],
    });
    expect(call.idempotencyKey).toMatch(/.{16,}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/roles/new-role"));
  });

  it("saves with the row version and preserves scoped grants it cannot edit", async () => {
    render(<RoleDefinitionEditor roleId="r1" />);

    expect(screen.getByText(/estimates\.read/)).toBeInTheDocument(); // preserved branch-scope grant is listed
    fireEvent.click(screen.getByLabelText("estimates.approve"));
    expect(screen.getByText("การเพิ่มสิทธิ์อนุมัติให้บทบาทที่มีผู้ถืออยู่ต้องให้ผู้ตรวจอีกคนอนุมัติก่อนมีผล")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      roleId: "r1",
      ifMatch: "v1",
      payload: {
        name: "Sales Lead",
        description: "เดิม",
        permissions: [
          { permissionKey: "customers.read", scope: "organization", branchId: null },
          { permissionKey: "estimates.approve", scope: "organization", branchId: null },
          { permissionKey: "estimates.read", scope: "branch", branchId: "b1" },
        ],
      },
    });
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("บันทึกบทบาทแล้ว"));
  });

  it("tells the user when the change became a request for a checker", async () => {
    mocks.update.mockResolvedValueOnce({ role: existing, pendingRequest: { id: "q1", rowVersion: "q" } });
    render(<RoleDefinitionEditor roleId="r1" />);

    fireEvent.click(screen.getByLabelText("estimates.approve"));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ส่งคำขอแก้ไขให้ผู้ตรวจอนุมัติแล้ว"));
  });

  it("shows the mapped message for a server error", async () => {
    mocks.update.mockRejectedValueOnce(new ApiError({ status: 409, code: "ROLE_NAME_ALREADY_EXISTS", message: "x" }));
    render(<RoleDefinitionEditor roleId="r1" />);

    fireEvent.click(screen.getByLabelText("customers.read"));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    expect(await screen.findByText("มีบทบาทที่ใช้ชื่อนี้อยู่แล้ว")).toBeInTheDocument();
  });

  it("is read-only for system roles and for roles with a pending change", () => {
    mocks.role = { data: { ...existing, isSystem: true }, isPending: false, isError: false };
    const { rerender } = render(<RoleDefinitionEditor roleId="r1" />);
    expect(screen.getByText("บทบาทของระบบแก้ไขไม่ได้")).toBeInTheDocument();
    expect(screen.getByLabelText("customers.read")).toBeDisabled();
    expect(screen.queryByRole("button", { name: "บันทึกการเปลี่ยนแปลง" })).not.toBeInTheDocument();

    mocks.role = { data: { ...existing, pendingChange: { id: "q1", rowVersion: "q" } }, isPending: false, isError: false };
    rerender(<RoleDefinitionEditor roleId="r1" />);
    expect(screen.getByText(/มีคำขอแก้ไขรออนุมัติ/)).toBeInTheDocument();
    expect(screen.getByLabelText(/ชื่อบทบาท/)).toBeDisabled();
  });
});
