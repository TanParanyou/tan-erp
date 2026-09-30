import { describe, expect, it, vi, beforeEach } from "vitest";
import { act, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import { EstimateLifecycleActions } from "./estimate-lifecycle-actions";

const mocks = vi.hoisted(() => ({
  submit: vi.fn(),
  review: vi.fn(),
  revision: vi.fn(),
  cancel: vi.fn(),
  success: vi.fn(),
  error: vi.fn(),
  permissions: [] as string[],
}));

vi.mock("../api/estimate-queries", async () => {
  const { useState } = await import("react");
  return {
    useSubmitEstimate: () => ({ mutateAsync: mocks.submit, isPending: false }),
    useReviewEstimate: () => ({ mutateAsync: mocks.review, isPending: false }),
    useCreateEstimateRevision: () => ({ mutateAsync: mocks.revision, isPending: false }),
    useCancelEstimate: () => {
      const [isPending, setIsPending] = useState(false);
      return {
        isPending,
        mutateAsync: async (input: unknown) => {
          setIsPending(true);
          try {
            return await mocks.cancel(input);
          } finally {
            setIsPending(false);
          }
        },
      };
    },
  };
});

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: mocks.success, error: mocks.error } }),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-1",
      permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })),
    },
  }),
}));

vi.mock("next-intl", () => ({
  useTranslations: (namespace: string) => (key: string) => `${namespace}.${key}`,
}));

function buildEstimate(status: string, calculationOutdated = false): EstimateDetailResponse {
  return {
    id: "estimate-1",
    organizationId: "org-1",
    branchId: "branch-1",
    customerId: "customer-1",
    opportunityId: "opportunity-1",
    siteSurveyRevisionId: null,
    siteSurveySnapshotHash: null,
    number: "EST-001",
    status,
    currentRevisionNo: 1,
    rowVersion: "estimate-row-version",
    createdAtUtc: "2026-09-27T00:00:00Z",
    updatedAtUtc: "2026-09-27T00:00:00Z",
    currentRevision: {
      id: "revision-1",
      estimateId: "estimate-1",
      revisionNo: 1,
      status,
      currency: "THB",
      calculationVersion: 1,
      calculationPolicyVersion: "TEST_ONLY_CALC-v1",
      taxPolicyVersion: "TEST_ONLY_TAX-v1",
      calculationOutdated,
      readiness: calculationOutdated ? "blocked" : "ready",
      readinessReasons: calculationOutdated ? [{
        code: "ESTIMATE_CALCULATION_OUTDATED",
        targetType: "revision",
        targetId: "revision-1",
        targetField: "calculation",
      }] : [],
      netCost: 100,
      sellingBeforeDiscount: 120,
      discountAmount: 0,
      netBeforeTax: 120,
      taxAmount: 0,
      grandTotal: 120,
      marginAmount: 20,
      marginRate: 0.16,
      markupRate: 0.2,
      calculationSnapshotJson: "{}",
      rowVersion: "revision-row-version",
      createdAtUtc: "2026-09-27T00:00:00Z",
      updatedAtUtc: "2026-09-27T00:00:00Z",
      sections: [],
    },
  };
}

describe("EstimateLifecycleActions", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.permissions = [];
  });

  it("hides submit when calculation is outdated and shows it when current", () => {
    mocks.permissions = ["estimates.submit"];
    const { rerender } = render(<EstimateLifecycleActions estimate={buildEstimate("draft", true)} opportunityId="opportunity-1" />);
    expect(screen.queryByRole("button", { name: "estimates.lifecycle.submit" })).not.toBeInTheDocument();

    rerender(<EstimateLifecycleActions estimate={buildEstimate("draft")} opportunityId="opportunity-1" />);
    expect(screen.getByRole("button", { name: "estimates.lifecycle.submit" })).toBeInTheDocument();
  });

  it("submits with the estimate ETag and frozen calculation version", async () => {
    mocks.permissions = ["estimates.submit"];
    mocks.submit.mockResolvedValue(buildEstimate("submitted"));
    render(<EstimateLifecycleActions estimate={buildEstimate("draft")} opportunityId="opportunity-1" />);

    fireEvent.click(screen.getByRole("button", { name: "estimates.lifecycle.submit" }));
    fireEvent.click(within(screen.getByRole("dialog")).getByRole("button", { name: "estimates.lifecycle.submit" }));

    await waitFor(() => expect(mocks.submit).toHaveBeenCalledWith({
      payload: { revisionNo: 1, calculationVersion: 1 },
      ifMatch: "estimate-row-version",
    }));
    expect(mocks.success).toHaveBeenCalledWith("estimates.lifecycle.submitSuccess");
  });

  it("requires a reason before creating a revision", async () => {
    mocks.permissions = ["estimates.revise"];
    render(<EstimateLifecycleActions estimate={buildEstimate("approved")} opportunityId="opportunity-1" />);

    fireEvent.click(screen.getByRole("button", { name: "estimates.lifecycle.createRevision" }));
    fireEvent.click(within(screen.getByRole("dialog")).getByRole("button", { name: "estimates.lifecycle.createRevision" }));
    expect(screen.getByText("estimates.lifecycle.reasonRequired")).toBeInTheDocument();
    expect(mocks.revision).not.toHaveBeenCalled();

    fireEvent.change(screen.getByRole("textbox"), { target: { value: "TEST_ONLY change" } });
    fireEvent.click(within(screen.getByRole("dialog")).getByRole("button", { name: "estimates.lifecycle.createRevision" }));
    await waitFor(() => expect(mocks.revision).toHaveBeenCalledWith({
      payload: { reason: "TEST_ONLY change" },
      ifMatch: "estimate-row-version",
    }));
  });

  it("requires a reason code and note when returning a submitted estimate", async () => {
    mocks.permissions = ["estimates.approve"];
    mocks.review.mockResolvedValue(buildEstimate("returned"));
    render(<EstimateLifecycleActions estimate={buildEstimate("submitted")} opportunityId="opportunity-1" />);

    fireEvent.click(screen.getByRole("button", { name: "estimates.lifecycle.return" }));
    const dialog = screen.getByRole("dialog");
    fireEvent.click(within(dialog).getByRole("button", { name: "estimates.lifecycle.return" }));
    expect(screen.getByText("estimates.lifecycle.returnReasonRequired")).toBeInTheDocument();
    expect(screen.getByText("estimates.lifecycle.returnNoteRequired")).toBeInTheDocument();
    expect(mocks.review).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByRole("textbox", { name: /estimates\.lifecycle\.returnReasonCode/ }), {
      target: { value: "MISSING_LABOR_COST" },
    });
    fireEvent.change(within(dialog).getByRole("textbox", { name: /estimates\.lifecycle\.returnNote/ }), {
      target: { value: "TEST_ONLY add installation labor cost to WI-002" },
    });
    fireEvent.click(within(dialog).getByRole("button", { name: "estimates.lifecycle.return" }));

    await waitFor(() => expect(mocks.review).toHaveBeenCalledWith({
      payload: {
        revisionNo: 1,
        decision: "returned",
        reasonCode: "MISSING_LABOR_COST",
        note: "TEST_ONLY add installation labor cost to WI-002",
      },
      ifMatch: "estimate-row-version",
    }));
    expect(mocks.success).toHaveBeenCalledWith("estimates.lifecycle.returnSuccess");
  });

  it("requires cancellation confirmation and locks the modal while the request is pending", async () => {
    mocks.permissions = ["estimates.cancel"];
    let resolveCancel: ((estimate: EstimateDetailResponse) => void) | undefined;
    mocks.cancel.mockImplementation(() => new Promise<EstimateDetailResponse>((resolve) => {
      resolveCancel = resolve;
    }));
    render(<EstimateLifecycleActions estimate={buildEstimate("draft")} opportunityId="opportunity-1" />);

    fireEvent.click(screen.getByRole("button", { name: "estimates.lifecycle.cancel" }));
    const dialog = screen.getByRole("dialog");
    const confirmButton = within(dialog).getByRole("button", { name: "estimates.lifecycle.cancel" });
    fireEvent.click(confirmButton);
    expect(screen.getByText("estimates.lifecycle.reasonRequired")).toBeInTheDocument();
    expect(mocks.cancel).not.toHaveBeenCalled();

    fireEvent.change(within(dialog).getByRole("textbox"), { target: { value: "TEST_ONLY duplicate estimate" } });
    fireEvent.click(confirmButton);
    await waitFor(() => expect(mocks.cancel).toHaveBeenCalledWith({
      payload: { reason: "TEST_ONLY duplicate estimate" },
      ifMatch: "estimate-row-version",
    }));
    await waitFor(() => expect(confirmButton).toBeDisabled());

    fireEvent.click(confirmButton);
    expect(mocks.cancel).toHaveBeenCalledTimes(1);
    expect(within(dialog).getByRole("button", { name: "common.actions.cancel" })).toBeDisabled();

    await act(async () => resolveCancel?.(buildEstimate("cancelled")));
    await waitFor(() => expect(screen.queryByRole("dialog")).not.toBeInTheDocument());
    expect(mocks.success).toHaveBeenCalledWith("estimates.lifecycle.cancelSuccess");
  });
});
