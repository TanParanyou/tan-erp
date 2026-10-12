import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApprovalLimitEditor } from "./approval-limit-editor";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  create: vi.fn(),
  update: vi.fn(),
  push: vi.fn(),
  limit: { data: undefined, isPending: false, isError: false } as Record<string, unknown>,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mocks.push, replace: vi.fn() }),
  usePathname: () => "/th/settings/approval-limits/create",
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/approval-limit-queries", () => ({
  useApprovalLimit: () => mocks.limit,
  useApprovalLimitOptions: () => ({
    isPending: false,
    isError: false,
    data: {
      roles: [{ id: "r1", name: "ผู้จัดการ" }],
      branches: [{ id: "b1", name: "สาขา 1" }],
      documentTypes: ["estimate", "purchase_order", "change_order"],
      currencies: ["THB"],
    },
  }),
  useCreateApprovalLimit: () => ({ mutateAsync: mocks.create, isPending: false }),
  useUpdateApprovalLimit: () => ({ mutateAsync: mocks.update, isPending: false }),
}));

const existing = {
  id: "l1", rowVersion: "v1", role: { id: "r1", name: "ผู้จัดการ" }, documentType: "estimate", branch: null,
  currency: "THB", isUnlimited: false, maxAmount: 500000, isActive: true,
};

describe("ApprovalLimitEditor", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = ["approval-limits.manage"];
    mocks.limit = { data: existing, isPending: false, isError: false };
    mocks.create.mockResolvedValue({ id: "new-limit" });
    mocks.update.mockResolvedValue(existing);
  });

  it("requires a role, a document type and an amount before creating", async () => {
    render(<ApprovalLimitEditor limitId="create" />);

    fireEvent.click(screen.getByRole("button", { name: "เพิ่มวงเงิน" }));

    expect(await screen.findByText("เลือกบทบาท")).toBeInTheDocument();
    expect(screen.getByText("เลือกประเภทเอกสาร")).toBeInTheDocument();
    expect(screen.getByText("กรอกวงเงินสูงสุด")).toBeInTheDocument();
    expect(mocks.create).not.toHaveBeenCalled();
  });

  it("creates an unlimited organization-wide limit with an idempotency key and opens it", async () => {
    render(<ApprovalLimitEditor limitId="create" />);

    fireEvent.change(screen.getByLabelText(/^บทบาท/), { target: { value: "r1" } });
    fireEvent.change(screen.getByLabelText(/^ประเภทเอกสาร/), { target: { value: "purchase_order" } });
    fireEvent.click(screen.getByLabelText(/^ไม่จำกัดวงเงิน/));
    expect(screen.getByLabelText("วงเงินสูงสุด")).toBeDisabled();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มวงเงิน" }));

    await waitFor(() => expect(mocks.create).toHaveBeenCalledTimes(1));
    const call = mocks.create.mock.calls[0][0] as { payload: unknown; idempotencyKey: string };
    expect(call.payload).toEqual({
      roleId: "r1", documentType: "purchase_order", branchId: null, currency: "THB", isUnlimited: true, maxAmount: null,
    });
    expect(call.idempotencyKey).toMatch(/.{16,}/);
    await waitFor(() => expect(mocks.push).toHaveBeenCalledWith("/th/settings/approval-limits/new-limit"));
  });

  it("locks the identity fields on edit and saves the amount with the row version", async () => {
    render(<ApprovalLimitEditor limitId="l1" />);

    expect(screen.getByText("บทบาท ประเภทเอกสาร สาขา และสกุลเงินแก้ไขไม่ได้ หากต้องการเปลี่ยนให้เพิ่มวงเงินใหม่")).toBeInTheDocument();
    expect(screen.getByDisplayValue("ผู้จัดการ")).toBeDisabled();
    fireEvent.change(screen.getByLabelText("วงเงินสูงสุด"), { target: { value: "750000.50" } });
    fireEvent.click(screen.getByLabelText(/^ใช้งาน/));
    fireEvent.click(screen.getByRole("button", { name: "บันทึกการเปลี่ยนแปลง" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      limitId: "l1",
      payload: { isUnlimited: false, maxAmount: 750000.5, isActive: false },
      ifMatch: "v1",
    });
  });
});
