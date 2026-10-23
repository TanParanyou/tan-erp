import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { PricingTemplateOverrides } from "./pricing-template-overrides";
import * as overrideQueries from "../api/branch-override-queries";
import type { BranchRateOverrideListResponse, BranchRateOverrideResponse } from "@/lib/api/api-client";
import { PERMISSIONS } from "@/lib/permissions/permissions";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));

vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const mockMutations = {
  request: { mutateAsync: vi.fn(), isPending: false },
  decide: { mutateAsync: vi.fn(), isPending: false },
  revoke: { mutateAsync: vi.fn(), isPending: false },
};

function renderOverrides(
  queryResult: { data?: BranchRateOverrideListResponse; isLoading?: boolean; isError?: boolean },
  granted: string[] = [PERMISSIONS.PRICING_TEMPLATES_OVERRIDE_RATE, PERMISSIONS.PRICING_TEMPLATES_APPROVE]
) {
  permissions.granted = granted;
  vi.spyOn(overrideQueries, "useBranchOverrides").mockReturnValue({
    data: queryResult.data,
    isLoading: queryResult.isLoading ?? false,
    isError: queryResult.isError ?? false,
  } as unknown as ReturnType<typeof overrideQueries.useBranchOverrides>);

  vi.spyOn(overrideQueries, "useBranchOverrideMutations").mockReturnValue(
    mockMutations as unknown as ReturnType<typeof overrideQueries.useBranchOverrideMutations>
  );

  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <PricingTemplateOverrides templateId="template-bi" />
    </NextIntlClientProvider>
  );
}

const t = thMessages.quickEstimates.overrides;

function makeOverride(overrides: Partial<BranchRateOverrideResponse> = {}): BranchRateOverrideResponse {
  return {
    id: "override-1",
    branchId: "branch-bkk",
    branchCode: "BKK-01",
    status: "requested",
    referenceRate: 8500,
    minimumCharge: 25000,
    effectiveFrom: "2026-11-01",
    effectiveTo: "2026-11-30",
    reasonCode: "LOCAL_COST",
    reasonDetail: "ค่าแรงงานเขตกรุงเทพสูงกว่าค่าเฉลี่ย",
    sourceReference: "DOC-2026-99",
    requestedBy: { id: "user-1", displayName: "สมชาย ผู้จัดการสาขา" },
    rowVersion: "version-token-1",
    ...overrides,
  };
}

describe("PricingTemplateOverrides (G-09 Branch Rate Override)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders loading state spinner", () => {
    renderOverrides({ isLoading: true });
    expect(screen.getAllByRole("status").length).toBeGreaterThanOrEqual(1);
  });

  it("renders error alert when query fails", () => {
    renderOverrides({ isError: true });
    expect(screen.getByText(t.loadError)).toBeDefined();
  });

  it("renders empty state message when there are no branch overrides", () => {
    renderOverrides({ data: { items: [] } });
    expect(screen.getByText(t.empty)).toBeDefined();
  });

  it("renders list of overrides with badge status and financial details", () => {
    const items = [
      makeOverride({ id: "ov-1", status: "requested", branchCode: "BKK-01" }),
      makeOverride({
        id: "ov-2",
        status: "approved",
        branchCode: "CNX-02",
        referenceRate: 7200,
        minimumCharge: 18000,
        reasonCode: "PROMO_RATE",
        sourceReference: "DOC-CNX-01",
      }),
    ];

    renderOverrides({ data: { items } });

    expect(screen.getByText("BKK-01")).toBeDefined();
    expect(screen.getByText(t.statuses.requested)).toBeDefined();
    expect(screen.getByText("CNX-02")).toBeDefined();
    expect(screen.getByText(t.statuses.approved)).toBeDefined();
    expect(screen.getByText("LOCAL_COST")).toBeDefined();
    expect(screen.getByText("PROMO_RATE")).toBeDefined();
    expect(screen.getByText("DOC-2026-99")).toBeDefined();
    expect(screen.getByText("DOC-CNX-01")).toBeDefined();
  });

  it("opens request modal, submits valid override request, and shows success toast", async () => {
    mockMutations.request.mutateAsync.mockResolvedValue({});
    renderOverrides({ data: { items: [] } }, [PERMISSIONS.PRICING_TEMPLATES_OVERRIDE_RATE]);

    const requestBtn = screen.getByRole("button", { name: t.request });
    fireEvent.click(requestBtn);

    expect(screen.getByRole("heading", { name: t.requestTitle })).toBeDefined();

    fireEvent.change(screen.getByLabelText(new RegExp(t.referenceRate)), { target: { value: "9000" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.minimumCharge)), { target: { value: "20000" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.effectiveFrom)), { target: { value: "2026-11-01" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.effectiveTo)), { target: { value: "2026-11-30" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.reasonCode)), { target: { value: "PROMO" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.reasonDetail)), { target: { value: "โปรโมชันเปิดสาขาใหม่" } });
    fireEvent.change(screen.getByLabelText(new RegExp(t.sourceReference)), { target: { value: "MKT-REF-01" } });

    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));

    await waitFor(() => {
      expect(mockMutations.request.mutateAsync).toHaveBeenCalledWith(
        expect.objectContaining({
          payload: {
            referenceRate: 9000,
            minimumCharge: 20000,
            effectiveFrom: "2026-11-01",
            effectiveTo: "2026-11-30",
            reasonCode: "PROMO",
            reasonDetail: "โปรโมชันเปิดสาขาใหม่",
            sourceReference: "MKT-REF-01",
          },
          idempotencyKey: expect.any(String),
        })
      );
    });

    await waitFor(() => {
      expect(toastMocks.success).toHaveBeenCalledWith(t.done);
    });
  });

  it("handles approval flow for requested override", async () => {
    mockMutations.decide.mutateAsync.mockResolvedValue({});
    const items = [makeOverride({ id: "ov-1", status: "requested" })];

    renderOverrides({ data: { items } }, [PERMISSIONS.PRICING_TEMPLATES_APPROVE]);

    const approveBtn = screen.getByRole("button", { name: t.approve });
    fireEvent.click(approveBtn);

    expect(screen.getByText(t.approveTitle)).toBeDefined();
    // In approve mode, note is optional
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));

    await waitFor(() => {
      expect(mockMutations.decide.mutateAsync).toHaveBeenCalledWith({
        overrideId: "ov-1",
        rowVersion: "version-token-1",
        decision: "approved",
        note: null,
      });
    });

    await waitFor(() => {
      expect(toastMocks.success).toHaveBeenCalledWith(t.done);
    });
  });

  it("requires note before returning or revoking override", async () => {
    mockMutations.decide.mutateAsync.mockResolvedValue({});
    const items = [makeOverride({ id: "ov-1", status: "requested" })];

    renderOverrides({ data: { items } }, [PERMISSIONS.PRICING_TEMPLATES_APPROVE]);

    const returnBtn = screen.getByRole("button", { name: t.return });
    fireEvent.click(returnBtn);

    expect(screen.getByText(t.returnTitle)).toBeDefined();

    // Confirm without note -> shows error
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));
    expect(await screen.findByText(t.reasonRequired)).toBeDefined();
    expect(mockMutations.decide.mutateAsync).not.toHaveBeenCalled();

    // Enter note and confirm
    fireEvent.change(screen.getByLabelText(new RegExp(t.note)), { target: { value: "อัตราสูงเกินไป กรุณาปรับลด" } });
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));

    await waitFor(() => {
      expect(mockMutations.decide.mutateAsync).toHaveBeenCalledWith({
        overrideId: "ov-1",
        rowVersion: "version-token-1",
        decision: "returned",
        note: "อัตราสูงเกินไป กรุณาปรับลด",
      });
    });
  });

  it("handles revoke flow for approved override with mandatory reason", async () => {
    mockMutations.revoke.mutateAsync.mockResolvedValue({});
    const items = [makeOverride({ id: "ov-1", status: "approved" })];

    renderOverrides({ data: { items } }, [PERMISSIONS.PRICING_TEMPLATES_APPROVE]);

    const revokeBtn = screen.getByRole("button", { name: t.revoke });
    fireEvent.click(revokeBtn);

    expect(screen.getByText(t.revokeTitle)).toBeDefined();

    fireEvent.change(screen.getByLabelText(new RegExp(t.note)), { target: { value: "หมดสัญญาโปรโมชันก่อนกำหนด" } });
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));

    await waitFor(() => {
      expect(mockMutations.revoke.mutateAsync).toHaveBeenCalledWith({
        overrideId: "ov-1",
        rowVersion: "version-token-1",
        reason: "หมดสัญญาโปรโมชันก่อนกำหนด",
      });
    });
  });
});
