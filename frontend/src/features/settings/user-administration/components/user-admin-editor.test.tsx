import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { UserAdminEditor } from "./user-admin-editor";

const mocks = vi.hoisted(() => ({
  create: vi.fn(),
  push: vi.fn(),
  success: vi.fn(),
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/users/create",
  useSearchParams: () => new URLSearchParams(),
}));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }),
}));

vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useOrganizationBranches: () => ({ data: [{ id: "b1", code: "HQ", name: "สาขากรุงเทพ" }] }),
}));

vi.mock("../api/user-admin-queries", () => ({
  useAdminRoles: () => ({
    isPending: false,
    isError: false,
    data: {
      items: [
        { id: "r-read", name: "Read Only", assignable: true, requiresApproval: false, permissionKeys: [] },
        { id: "r-approver", name: "Approver", assignable: true, requiresApproval: true, permissionKeys: [] },
        { id: "r-super", name: "Super", assignable: false, requiresApproval: false, permissionKeys: [] },
      ],
    },
  }),
  useCreateAdminUser: () => ({ mutateAsync: mocks.create, isPending: false }),
}));

function fillValidForm() {
  fireEvent.change(screen.getByLabelText(/ชื่อที่แสดง/), { target: { value: "สมหญิง" } });
  fireEvent.change(screen.getByLabelText(/อีเมล/), { target: { value: "somying@example.test" } });
  fireEvent.click(screen.getByLabelText("Read Only"));
}

describe("UserAdminEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.create.mockResolvedValue({ id: "new-user" });
  });

  it("validates required fields before calling the API", async () => {
    render(<UserAdminEditor />);

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มผู้ใช้" }));

    expect(await screen.findByText(/กรุณากรอกชื่อที่แสดง/)).toBeInTheDocument();
    expect(screen.getByText(/กรุณากรอกอีเมล/)).toBeInTheDocument();
    expect(screen.getByText(/กรุณาเลือกอย่างน้อยหนึ่งบทบาท/)).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates the user, sends an idempotency key and opens the new user", async () => {
    render(<UserAdminEditor />);
    fillValidForm();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มผู้ใช้" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      displayName: "สมหญิง",
      email: "somying@example.test",
      branchId: null,
      roleIds: ["r-read"],
    });
    expect(call.idempotencyKey).toMatch(/[0-9a-f-]{36}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/users/new-user"));
  });

  it("reuses the idempotency key when the same submission is retried after a failure", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 500, code: "INTERNAL_SERVER_ERROR", message: "x" }));
    render(<UserAdminEditor />);
    fillValidForm();

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มผู้ใช้" }));
    await screen.findByText("ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง");
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มผู้ใช้" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(2));
    const firstKey = (mocks.create.mock.calls[0][0] as { idempotencyKey: string }).idempotencyKey;
    const secondKey = (mocks.create.mock.calls[1][0] as { idempotencyKey: string }).idempotencyKey;
    expect(secondKey).toBe(firstKey);
  });

  it("shows the duplicate email message from the backend error code", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 409, code: "USER_EMAIL_ALREADY_EXISTS", message: "x" }));
    render(<UserAdminEditor />);
    fillValidForm();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มผู้ใช้" }));

    expect(await screen.findByText("มีผู้ใช้ที่ใช้อีเมลนี้อยู่ในระบบแล้ว")).toBeInTheDocument();
  });

  it("does not allow roles the caller cannot assign", () => {
    render(<UserAdminEditor />);

    expect(screen.getByLabelText("Super")).toBeDisabled();
    expect(screen.getByLabelText("Approver")).toBeEnabled();
  });
});
