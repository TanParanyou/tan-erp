import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, within } from "@testing-library/react";
import { EstimateReviewQueue } from "./estimate-review-queue";

const mocks = vi.hoisted(() => ({ queue: null as unknown, refetch: vi.fn() }));

vi.mock("../api/estimate-queries", () => ({
  useEstimateReviewQueue: () => ({ data: mocks.queue, isLoading: false, isError: false, error: null, refetch: mocks.refetch }),
}));

vi.mock("@/hooks/useListState", () => ({
  useListState: () => ({
    params: { page: 1, limit: 10, search: "", order: "desc", filters: {} },
    draftSearch: "",
    actions: { setSearch: vi.fn(), setPage: vi.fn(), setLimit: vi.fn() },
  }),
}));

vi.mock("next-intl", () => ({
  useLocale: () => "en",
  useTranslations: (namespace: string) => (key: string, values?: Record<string, string | number>) =>
    values ? `${namespace}.${key} ${Object.values(values).join(" ")}` : `${namespace}.${key}`,
}));

const queueItem = {
  approvalRequestId: "request-1",
  estimateId: "estimate-1",
  estimateNumber: "EST-001",
  estimateRowVersion: "estimate-row-1",
  revisionId: "revision-1",
  revisionNo: 1,
  revisionStatus: "submitted",
  calculationVersion: 1,
  currency: "THB",
  grandTotal: 1000,
  marginRate: 0.2,
  opportunityId: "opportunity-1",
  opportunity: { id: "opportunity-1", title: "Solar installation" },
  customer: { id: "customer-1", displayNameTh: "ลูกค้า", displayNameEn: "Customer One" },
  branch: { id: "branch-1", code: "BKK", name: "Bangkok" },
  requestedBy: { id: "user-1", displayName: "Maker" },
  requestedAtUtc: "2026-09-27T00:00:00Z",
  submissionNote: "Ready for review",
  calculationInputHash: "input-hash",
  calculationSnapshotHash: "snapshot-hash",
  frozenRoute: { policyCode: "SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER", policyVersion: 1, routeHash: "route-hash", scopeType: "branch", scopeId: "branch-1", permissionKey: "estimates.approve", reviewer: { id: "reviewer-1", displayName: "Reviewer", membershipId: "membership-1" }, triggers: [{ code: "MARGIN_BELOW_MINIMUM", actualValue: 0.2, thresholdValue: 0.3, unit: "rate" }], thresholds: null },
  readiness: "requiresAttention",
  readinessReasons: [{ code: "PROVISIONAL_COST", targetType: "costComponent", targetId: "component-1", targetField: "provisionalReason" }],
  priceOverrides: [{ workItemCode: "WI-FIX", workItemDescription: "Custom install", fixedPriceUnitAmount: 125, reasonCode: "CUSTOMER_AGREED_PRICE" }],
  costEvidence: [{ workItemCode: "WI-01", workItemDescription: "Install panels", type: "material", description: "Panels", quantity: 2, unitCode: "pcs", unitCost: 500, currency: "THB", totalCost: 1000, costOrigin: "catalog", itemCode: "PANEL-01", costRecordVersion: 3, costSourceCode: "VENDOR", sourceReference: "quote-88", evidenceFileId: "file-1", costRecordReason: null, provisionalReasonCode: null, provisionalNote: null, isProvisional: false }],
  provisionalCostCount: 0,
};

describe("EstimateReviewQueue", () => {
  beforeEach(() => {
    mocks.queue = { items: [queueItem], totalCount: 1, pageNumber: 1, pageSize: 10 };
    mocks.refetch.mockReset();
  });

  it("shows assigned estimate totals and its frozen route, readiness, and cost evidence", () => {
    render(<EstimateReviewQueue />);

    expect(screen.getByText("EST-001")).toBeInTheDocument();
    expect(screen.getByText("Solar installation")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "estimates.reviewDetails" }));

    const dialog = screen.getByRole("dialog");
    expect(within(dialog).getByText("SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER v1")).toBeInTheDocument();
    expect(within(dialog).getByText("route-hash")).toBeInTheDocument();
    expect(within(dialog).getByText("MARGIN_BELOW_MINIMUM (20%; estimates.reviewTriggerThreshold 30%)")).toBeInTheDocument();
    expect(within(dialog).getByText("PROVISIONAL_COST · costComponent · provisionalReason")).toBeInTheDocument();
    expect(within(dialog).getByText("WI-FIX · Custom install")).toBeInTheDocument();
    expect(within(dialog).getByText("estimates.sellingRuleReasonCode: CUSTOMER_AGREED_PRICE")).toBeInTheDocument();
    expect(within(dialog).getByText((_, element) => element?.textContent === "PANEL-01 · VENDOR · quote-88 · v3 · estimates.reviewEvidenceFile: file-1")).toBeInTheDocument();
  });
});
