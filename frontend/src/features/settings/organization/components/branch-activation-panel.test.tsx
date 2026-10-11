import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { BranchActivationPanel } from "./branch-activation-panel";

const mocks = vi.hoisted(() => ({
  setActive: vi.fn(),
  success: vi.fn(),
  isPending: false,
  check: { data: { canDeactivate: true, blockers: [] as { type: string; count: number }[] } } as Record<string, unknown>,
}));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/organization-admin-queries", () => ({
  useBranchDeactivationCheck: () => mocks.check,
  useSetAdminBranchActive: () => ({ mutateAsync: mocks.setActive, isPending: mocks.isPending }),
}));

const active: AdminBranchResponse = {
  id: "b1", code: "HQ", name: "สำนักงานใหญ่", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null,
  phone: null, isActive: true, rowVersion: "v1", createdAtUtc: "2026-10-10T00:00:00Z",
};

describe("BranchActivationPanel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.isPending = false;
    mocks.check = { data: { canDeactivate: true, blockers: [] } };
    mocks.setActive.mockResolvedValue({});
  });

  it("requires a reason before the confirmation modal can be opened, then deactivates with the row version", async () => {
    render(<BranchActivationPanel branch={active} canManage />);
    const open = screen.getByRole("button", { name: "ปิดใช้งานสาขา" });
    expect(open).toBeDisabled();

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "  ย้ายที่ตั้ง  " } });
    fireEvent.click(open);
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ branchId: "b1", active: false, reason: "ย้ายที่ตั้ง", ifMatch: "v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("ปิดใช้งานสาขาแล้ว"));
  });

  it("lists blockers from the server and keeps deactivation disabled", () => {
    mocks.check = { data: { canDeactivate: false, blockers: [{ type: "estimates", count: 3 }, { type: "last_active_branch", count: 1 }] } };
    render(<BranchActivationPanel branch={active} canManage />);

    expect(screen.getByText("ยังปิดสาขานี้ไม่ได้")).toBeInTheDocument();
    expect(screen.getByText(/ใบประเมินราคาที่ยังไม่สิ้นสุด/)).toHaveTextContent("3");
    expect(screen.getByText(/องค์กรต้องมีสาขาที่เปิดใช้อย่างน้อยหนึ่งสาขา/)).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    expect(screen.getByRole("button", { name: "ปิดใช้งานสาขา" })).toBeDisabled();
  });

  it("shows the server error when the guard rejects at confirm time", async () => {
    mocks.setActive.mockRejectedValueOnce(new ApiError({ status: 409, code: "BRANCH_HAS_OPEN_DOCUMENTS", message: "x" }));
    render(<BranchActivationPanel branch={active} canManage />);

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานสาขา" }));
    fireEvent.click(await screen.findByRole("button", { name: "ปิดใช้งาน" }));

    expect(await screen.findByText("สาขานี้ยังมีเอกสารหรือคลังที่ใช้งานอยู่")).toBeInTheDocument();
  });

  it("locks the confirm button while the request is in flight", async () => {
    mocks.isPending = true;
    render(<BranchActivationPanel branch={active} canManage />);

    fireEvent.change(screen.getByLabelText("เหตุผล"), { target: { value: "x" } });
    fireEvent.click(screen.getByRole("button", { name: "ปิดใช้งานสาขา" }));

    expect(await screen.findByRole("button", { name: "ปิดใช้งาน" })).toBeDisabled();
  });

  it("offers activation for an inactive branch and nothing without permission", async () => {
    const { rerender } = render(<BranchActivationPanel branch={{ ...active, isActive: false }} canManage />);
    fireEvent.click(screen.getByRole("button", { name: "เปิดใช้งานสาขา" }));
    fireEvent.click(await screen.findByRole("button", { name: "เปิดใช้งาน" }));
    await waitFor(() => expect(mocks.setActive).toHaveBeenCalledWith({ branchId: "b1", active: true, ifMatch: "v1" }));

    rerender(<BranchActivationPanel branch={active} canManage={false} />);
    expect(screen.queryByRole("button", { name: "ปิดใช้งานสาขา" })).not.toBeInTheDocument();
  });
});
