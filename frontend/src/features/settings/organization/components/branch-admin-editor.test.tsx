import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { BranchAdminEditor } from "./branch-admin-editor";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  create: vi.fn(),
  update: vi.fn(),
  push: vi.fn(),
  success: vi.fn(),
  branch: { data: undefined, isPending: false, isError: false } as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/branches/create",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("./branch-activation-panel", () => ({ BranchActivationPanel: () => <div>activation-panel</div> }));
vi.mock("../api/organization-admin-queries", () => ({
  useAdminBranch: () => mocks.branch,
  useCreateAdminBranch: () => ({ mutateAsync: mocks.create, isPending: false }),
  useUpdateAdminBranch: () => ({ mutateAsync: mocks.update, isPending: false }),
}));

const existing = {
  id: "b1", code: "HQ", name: "สำนักงานใหญ่", nameEn: "Head Office", taxBranchCode: "00000", addressTh: null, addressEn: null,
  phone: null, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-10T00:00:00Z",
};

describe("BranchAdminEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["branches.manage"];
    mocks.branch = { data: existing, isPending: false, isError: false };
    mocks.create.mockResolvedValue({ id: "new-branch" });
    mocks.update.mockResolvedValue({});
  });

  it("validates code, name and tax branch code before creating", async () => {
    render(<BranchAdminEditor branchId="create" />);

    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "bad code" } });
    fireEvent.change(screen.getByLabelText(/เลขสาขาภาษี/), { target: { value: "12" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    expect(await screen.findByText("ใช้ได้เฉพาะตัวอักษรอังกฤษ ตัวเลข ขีดล่าง และขีดกลาง")).toBeInTheDocument();
    expect(screen.getByText("กรุณากรอกชื่อสาขา")).toBeInTheDocument();
    expect(screen.getByText("เลขสาขาภาษีต้องเป็นตัวเลข 5 หลัก")).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates with an idempotency key, null for blank optional fields, and opens the new branch", async () => {
    render(<BranchAdminEditor branchId="create" />);

    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "B2" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สาขา 2" } });
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      code: "B2", name: "สาขา 2", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null, phone: null,
    });
    expect(call.idempotencyKey).toMatch(/[0-9a-f-]{36}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/branches/new-branch"));
  });

  it("reuses the idempotency key when the same create is retried after a failure", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 500, code: "INTERNAL_SERVER_ERROR", message: "x" }));
    render(<BranchAdminEditor branchId="create" />);
    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "B3" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สาขา 3" } });

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));
    await screen.findByText("ดำเนินการไม่สำเร็จ กรุณาลองใหม่อีกครั้ง");
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(2));
    expect((mocks.create.mock.calls[1][0] as { idempotencyKey: string }).idempotencyKey)
      .toBe((mocks.create.mock.calls[0][0] as { idempotencyKey: string }).idempotencyKey);
  });

  it("shows the duplicate-code message from the server", async () => {
    mocks.create.mockRejectedValueOnce(new ApiError({ status: 409, code: "BRANCH_CODE_ALREADY_EXISTS", message: "x" }));
    render(<BranchAdminEditor branchId="add" />);
    fireEvent.change(screen.getByLabelText(/รหัสสาขา/), { target: { value: "HQ" } });
    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "x" } });

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มสาขา" }));

    expect(await screen.findByText("มีสาขาที่ใช้รหัสนี้อยู่แล้ว")).toBeInTheDocument();
  });

  it("edits an existing branch: code is read-only and the save sends If-Match", async () => {
    render(<BranchAdminEditor branchId="b1" />);

    expect(screen.getByLabelText(/รหัสสาขา/)).toBeDisabled();
    expect(screen.getByLabelText(/รหัสสาขา/)).toHaveValue("HQ");
    expect(screen.getByText("activation-panel")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText(/ชื่อสาขา \(ไทย\)/), { target: { value: "สำนักงานใหญ่ใหม่" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      branchId: "b1", ifMatch: "v1",
      payload: { name: "สำนักงานใหญ่ใหม่", nameEn: "Head Office", taxBranchCode: "00000", addressTh: null, addressEn: null, phone: null },
    });
  });

  it("shows not-found for an unknown branch id", () => {
    mocks.branch = { data: undefined, isPending: false, isError: true };
    render(<BranchAdminEditor branchId="missing" />);

    expect(screen.getByText("ไม่พบข้อมูลที่ต้องการ")).toBeInTheDocument();
  });
});
