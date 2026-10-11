import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { RoleChangeRequestQueue } from "./role-change-request-queue";

const mocks = vi.hoisted(() => ({ decide: vi.fn(), success: vi.fn() }));

vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("../api/role-management-queries", () => ({
  useRoleChangeRequests: () => ({
    data: {
      items: [
        {
          id: "q1", rowVersion: "request-v1", status: "pending",
          role: { id: "r1", name: "PO Approver" }, requestedBy: { id: "u1", name: "สมศรี" }, requestedAtUtc: "2026-10-11T00:00:00Z",
          proposedName: "PO Approver", proposedDescription: null,
          addedPermissionKeys: ["purchase-orders.approve"], removedPermissionKeys: ["customers.read"],
        },
      ],
    },
    isLoading: false, isError: false, refetch: vi.fn(),
  }),
  useDecideRoleChangeRequest: () => ({ mutateAsync: mocks.decide, isPending: false }),
}));

describe("RoleChangeRequestQueue", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.decide.mockResolvedValue({});
  });

  it("lists the role, the requester and what is added and removed", () => {
    render(<RoleChangeRequestQueue />);

    expect(screen.getByText("PO Approver")).toBeInTheDocument();
    expect(screen.getByText("สมศรี")).toBeInTheDocument();
    expect(screen.getByText("purchase-orders.approve")).toBeInTheDocument();
    expect(screen.getByText("customers.read")).toBeInTheDocument();
  });

  it("approves only after the confirmation dialog and sends the request row version", async () => {
    render(<RoleChangeRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "อนุมัติ" }));
    const dialog = await screen.findByRole("dialog");
    expect(mocks.decide).not.toHaveBeenCalled();
    fireEvent.click(within(dialog).getByRole("button", { name: "ยืนยัน" }));

    await waitFor(() => expect(mocks.decide).toHaveBeenCalledWith({ requestId: "q1", decision: "approve", ifMatch: "request-v1" }));
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("อนุมัติคำขอแล้ว"));
  });

  it("shows the mapped message when the checker is the requester", async () => {
    mocks.decide.mockRejectedValueOnce(new ApiError({ status: 403, code: "ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", message: "x" }));
    render(<RoleChangeRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "ปฏิเสธ" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    // The subtitle repeats this sentence, so assert on the error alert instead of plain text.
    expect(await screen.findByRole("alert")).toHaveTextContent("ผู้ตรวจต้องเป็นคนละคนกับผู้ขอ");
  });
});
