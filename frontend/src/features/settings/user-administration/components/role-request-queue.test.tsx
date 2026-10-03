import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { RoleRequestQueue } from "./role-request-queue";

const mocks = vi.hoisted(() => ({
  decide: vi.fn(),
  success: vi.fn(),
}));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }),
}));

vi.mock("../api/user-admin-queries", () => ({
  useAdminRoleRequests: () => ({
    data: {
      items: [
        {
          id: "q1",
          rowVersion: "request-v1",
          status: "pending",
          role: { id: "r1", name: "Approver" },
          membership: { id: "m1", name: "สมหญิง" },
          requestedBy: { id: "u1", name: "ผู้ขอ" },
          requestedAtUtc: "2026-10-03T00:00:00Z",
        },
      ],
    },
    isLoading: false,
    isError: false,
    refetch: vi.fn(),
  }),
  useDecideAdminRoleRequest: () => ({ mutateAsync: mocks.decide, isPending: false }),
}));

describe("RoleRequestQueue", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.decide.mockResolvedValue({});
  });

  it("lists pending requests", () => {
    render(<RoleRequestQueue />);

    expect(screen.getByText("Approver")).toBeInTheDocument();
    expect(screen.getByText("สมหญิง")).toBeInTheDocument();
    expect(screen.getAllByText("ผู้ขอ").length).toBeGreaterThan(0);
  });

  it("approves only after the confirmation dialog and sends the request row version", async () => {
    render(<RoleRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "อนุมัติ" }));
    const dialog = await screen.findByRole("dialog");
    expect(mocks.decide).not.toHaveBeenCalled();
    fireEvent.click(within(dialog).getByRole("button", { name: "ยืนยัน" }));

    await waitFor(() =>
      expect(mocks.decide).toHaveBeenCalledWith({ requestId: "q1", decision: "approve", ifMatch: "request-v1" })
    );
  });

  it("explains why the requester cannot approve their own request", async () => {
    mocks.decide.mockRejectedValue(
      new ApiError({ status: 403, code: "ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED", message: "x" })
    );
    render(<RoleRequestQueue />);

    fireEvent.click(screen.getByRole("button", { name: "อนุมัติ" }));
    fireEvent.click(within(await screen.findByRole("dialog")).getByRole("button", { name: "ยืนยัน" }));

    expect(await screen.findByText("ผู้ขอไม่สามารถอนุมัติหรือปฏิเสธคำขอของตนเองได้")).toBeInTheDocument();
  });
});
