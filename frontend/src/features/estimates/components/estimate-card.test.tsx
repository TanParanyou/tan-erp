import { describe, it, expect, vi, beforeEach } from "vitest";
import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { EstimateCard } from "./estimate-card";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import { apiClient } from "@/lib/api/api-client";

// Mock useToast
const mockToast = { success: vi.fn(), error: vi.fn() };
vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({
    toast: mockToast,
  }),
}));

// Mock auth session
vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: () => Promise.resolve("mock-token"),
}));

// Mock useSelectedMembership
let mockPermissions: string[] = ["quotations.issue"];
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "mem-1",
      role: "admin",
      organizationId: "org-1",
      permissions: mockPermissions.map((k) => ({ key: k, scope: "organization" })),
    },
  }),
}));

// Mock useConfirm
vi.mock("@/hooks/useConfirm", () => ({
  useConfirm: () => ({
    confirm: vi.fn(),
    ConfirmDialog: () => null,
  }),
}));

// Mock next-intl
vi.mock("next-intl", () => ({
  useTranslations: (namespace: string) => (key: string, params?: Record<string, unknown>) => {
    if (namespace === "estimates") {
      const translations: Record<string, string> = {
        cardTitle: "การประเมินราคาทางการ (Official Estimate)",
        noEstimateYet: "ยังไม่มีการจัดทำใบประเมินราคาสำหรับโอกาสทางการขายนี้",
        createEstimate: "สร้างใบประเมินราคา",
        creating: "กำลังสร้าง...",
        openWorkspace: "เปิด Workspace ประเมินราคา",
        revision: `รุ่นที่ ${params?.number ?? 1}`,
        boqCost: "ต้นทุนประเมินภายใน (BOQ Cost)",
        totalBeforeDiscount: "ยอดรวมก่อนส่วนลด",
        discount: "ส่วนลด",
        grandTotal: "ยอดรวมสุทธิทั้งสิ้น",
        marginRate: "อัตรากำไร (Margin)",
        issueQuotation: "ออกใบเสนอราคา",
        issueQuotationModalTitle: "ยืนยันการออกใบเสนอราคา",
        issueQuotationModalDesc: "คุณต้องการออกใบเสนอราคาสำหรับงานนี้ใช่หรือไม่?",
        quotationIssuedSuccess: "ออกใบเสนอราคาสำเร็จ",
        quotationIssuedFailed: "ไม่สามารถออกใบเสนอราคาได้",
        "statuses.quoted": "ออกใบเสนอราคาแล้ว (Quoted)",
        "statuses.draft": "ฉบับร่าง (Draft)",
      };
      return translations[key] ?? key;
    }
    if (namespace === "common") {
      const translations: Record<string, string> = {
        "actions.cancel": "ยกเลิก",
        "actions.confirm": "ยืนยัน",
      };
      return translations[key] ?? key;
    }
    return key;
  },
}));

describe("EstimateCard", () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false },
        mutations: { retry: false },
      },
    });
    mockPermissions = ["quotations.issue"];
    vi.clearAllMocks();
  });

  const renderWithClient = (ui: React.ReactElement) => {
    return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>);
  };

  it("renders empty state with create button when estimate is null", () => {
    const handleCreate = vi.fn();

    renderWithClient(
      <EstimateCard
        estimate={null}
        canEdit={true}
        onCreateEstimate={handleCreate}
        isCreating={false}
      />
    );

    expect(screen.getByText("การประเมินราคาทางการ (Official Estimate)")).toBeInTheDocument();
    expect(screen.getByText("ยังไม่มีการจัดทำใบประเมินราคาสำหรับโอกาสทางการขายนี้")).toBeInTheDocument();

    const createBtn = screen.getByRole("button", { name: "สร้างใบประเมินราคา" });
    expect(createBtn).toBeInTheDocument();
    fireEvent.click(createBtn);
    expect(handleCreate).toHaveBeenCalledTimes(1);
  });

  it("renders estimate details and financial summary when estimate exists", () => {
    const mockEstimate: EstimateDetailResponse = {
      id: "est-1234",
      organizationId: "org-1",
      branchId: "branch-1",
      customerId: "cust-1",
      opportunityId: "opp-1",
      siteSurveyRevisionId: null,
      siteSurveySnapshotHash: null,
      number: "EST-2026-0099",
      status: "draft",
      currentRevisionNo: 1,
      rowVersion: "version-1",
      createdAtUtc: "2026-09-18T00:00:00Z",
      updatedAtUtc: "2026-09-18T00:00:00Z",
      currentRevision: {
        id: "rev-1",
        estimateId: "est-1234",
        revisionNo: 1,
        status: "draft",
        currency: "THB",
        calculationVersion: 1,
        calculationPolicyVersion: "v1",
        taxPolicyVersion: "v1",
        netCost: 50000,
        sellingBeforeDiscount: 70000,
        discountAmount: 5000,
        netBeforeTax: 65000,
        taxAmount: 4550,
        grandTotal: 69550,
        marginAmount: 15000,
        marginRate: 0.2308,
        markupRate: 0.3,
        calculationSnapshotJson: null,
        rowVersion: "rev-version-1",
        createdAtUtc: "2026-09-18T00:00:00Z",
        updatedAtUtc: "2026-09-18T00:00:00Z",
        sections: [],
      },
    };

    renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-ver-1"
        canEdit={true}
      />
    );

    expect(screen.getByText("EST-2026-0099")).toBeInTheDocument();
    expect(screen.getByText("รุ่นที่ 1")).toBeInTheDocument();
    expect(screen.getByText("เปิด Workspace ประเมินราคา")).toBeInTheDocument();

    // Financial numbers formatted
    expect(screen.getByText(/70,000\.00/)).toBeInTheDocument();
    expect(screen.getByText(/5,000\.00/)).toBeInTheDocument();
    expect(screen.getByText(/69,550\.00/)).toBeInTheDocument();
    expect(screen.getByText(/0\.23%/)).toBeInTheDocument();
  });

  it("renders issue quotation button and opens confirmation modal when draft has positive total", () => {
    const mockEstimate: EstimateDetailResponse = {
      id: "est-1234",
      organizationId: "org-1",
      branchId: "branch-1",
      customerId: "cust-1",
      opportunityId: "opp-1",
      siteSurveyRevisionId: null,
      siteSurveySnapshotHash: null,
      number: "EST-2026-0099",
      status: "draft",
      currentRevisionNo: 1,
      rowVersion: "version-1",
      createdAtUtc: "2026-09-18T00:00:00Z",
      updatedAtUtc: "2026-09-18T00:00:00Z",
      currentRevision: {
        id: "rev-1",
        estimateId: "est-1234",
        revisionNo: 1,
        status: "draft",
        currency: "THB",
        calculationVersion: 1,
        calculationPolicyVersion: "v1",
        taxPolicyVersion: "v1",
        netCost: 50000,
        sellingBeforeDiscount: 70000,
        discountAmount: 0,
        netBeforeTax: 70000,
        taxAmount: 4900,
        grandTotal: 74900,
        marginAmount: 20000,
        marginRate: 0.2857,
        markupRate: 0.4,
        calculationSnapshotJson: null,
        rowVersion: "rev-version-1",
        createdAtUtc: "2026-09-18T00:00:00Z",
        updatedAtUtc: "2026-09-18T00:00:00Z",
        sections: [],
      },
    };

    renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-ver-1"
        canEdit={true}
      />
    );

    const issueBtn = screen.getByRole("button", { name: "ออกใบเสนอราคา" });
    expect(issueBtn).toBeInTheDocument();

    fireEvent.click(issueBtn);
    expect(screen.getByText("ยืนยันการออกใบเสนอราคา")).toBeInTheDocument();
  });

  it("renders quoted badge and does not display issue button when already quoted", () => {
    const mockEstimate: EstimateDetailResponse = {
      id: "est-1234",
      organizationId: "org-1",
      branchId: "branch-1",
      customerId: "cust-1",
      opportunityId: "opp-1",
      siteSurveyRevisionId: null,
      siteSurveySnapshotHash: null,
      number: "EST-2026-0099",
      status: "quoted",
      currentRevisionNo: 1,
      rowVersion: "version-2",
      createdAtUtc: "2026-09-18T00:00:00Z",
      updatedAtUtc: "2026-09-18T00:00:00Z",
      currentRevision: {
        id: "rev-1",
        estimateId: "est-1234",
        revisionNo: 1,
        status: "quoted",
        currency: "THB",
        calculationVersion: 1,
        calculationPolicyVersion: "v1",
        taxPolicyVersion: "v1",
        netCost: 50000,
        sellingBeforeDiscount: 70000,
        discountAmount: 0,
        netBeforeTax: 70000,
        taxAmount: 4900,
        grandTotal: 74900,
        marginAmount: 20000,
        marginRate: 0.2857,
        markupRate: 0.4,
        calculationSnapshotJson: null,
        rowVersion: "rev-version-1",
        createdAtUtc: "2026-09-18T00:00:00Z",
        updatedAtUtc: "2026-09-18T00:00:00Z",
        sections: [],
      },
    };

    renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-ver-1"
        canEdit={true}
      />
    );

    expect(screen.getByText("ออกใบเสนอราคาแล้ว (Quoted)")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "ออกใบเสนอราคา" })).not.toBeInTheDocument();
  });

  it("canIssue requires only quotations.issue permission regardless of canEdit prop", () => {
    const mockEstimate: EstimateDetailResponse = {
      id: "est-1234",
      organizationId: "org-1",
      branchId: "branch-1",
      customerId: "cust-1",
      opportunityId: "opp-1",
      siteSurveyRevisionId: null,
      siteSurveySnapshotHash: null,
      number: "EST-2026-0099",
      status: "draft",
      currentRevisionNo: 1,
      rowVersion: "version-1",
      createdAtUtc: "2026-09-18T00:00:00Z",
      updatedAtUtc: "2026-09-18T00:00:00Z",
      currentRevision: {
        id: "rev-1",
        estimateId: "est-1234",
        revisionNo: 1,
        status: "draft",
        currency: "THB",
        calculationVersion: 1,
        calculationPolicyVersion: "v1",
        taxPolicyVersion: "v1",
        netCost: 50000,
        sellingBeforeDiscount: 70000,
        discountAmount: 0,
        netBeforeTax: 70000,
        taxAmount: 4900,
        grandTotal: 74900,
        marginAmount: 20000,
        marginRate: 0.2857,
        markupRate: 0.4,
        calculationSnapshotJson: null,
        rowVersion: "rev-version-1",
        createdAtUtc: "2026-09-18T00:00:00Z",
        updatedAtUtc: "2026-09-18T00:00:00Z",
        sections: [],
      },
    };

    // Case 1: user lacks quotations.issue even if canEdit is true
    mockPermissions = [];
    const { unmount } = renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-ver-1"
        canEdit={true}
      />
    );
    expect(screen.queryByRole("button", { name: "ออกใบเสนอราคา" })).not.toBeInTheDocument();
    unmount();

    // Case 2: user has quotations.issue even if canEdit is false
    mockPermissions = ["quotations.issue"];
    renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-ver-1"
        canEdit={false}
      />
    );
    expect(screen.getByRole("button", { name: "ออกใบเสนอราคา" })).toBeInTheDocument();
  });

  it("IssueQuotation_SendsBothVersionsAndReusesKeyAfterAmbiguousFailure", async () => {
    const mockEstimate: EstimateDetailResponse = {
      id: "est-1234",
      organizationId: "org-1",
      branchId: "branch-1",
      customerId: "cust-1",
      opportunityId: "opp-1",
      siteSurveyRevisionId: null,
      siteSurveySnapshotHash: null,
      number: "EST-2026-0002",
      status: "draft",
      currentRevisionNo: 1,
      rowVersion: "est-version-123",
      createdAtUtc: "2026-09-18T00:00:00Z",
      updatedAtUtc: "2026-09-18T00:00:00Z",
      currentRevision: {
        id: "rev-1",
        estimateId: "est-1234",
        revisionNo: 1,
        status: "draft",
        currency: "THB",
        calculationVersion: 1,
        calculationPolicyVersion: "v1",
        taxPolicyVersion: "v1",
        netCost: 50000,
        sellingBeforeDiscount: 70000,
        discountAmount: 0,
        netBeforeTax: 70000,
        taxAmount: 4900,
        grandTotal: 74900,
        marginAmount: 20000,
        marginRate: 0.2857,
        markupRate: 0.4,
        calculationSnapshotJson: null,
        rowVersion: "rev-version-1",
        createdAtUtc: "2026-09-18T00:00:00Z",
        updatedAtUtc: "2026-09-18T00:00:00Z",
        sections: [],
      },
    };

    const issueSpy = vi.spyOn(apiClient, "issueQuotation");
    // Call 1: ambiguous failure (network timeout / transient disconnect)
    issueSpy.mockRejectedValueOnce(new Error("Network timeout on issuing quotation"));
    // Call 2: retry succeeds
    issueSpy.mockResolvedValueOnce({
      quotationId: "q-001",
      estimateId: "est-1234",
      opportunityId: "opp-1",
      number: "QT-2026-0001",
      status: "issued",
      grandTotal: 74900,
      issuedAtUtc: "2026-09-20T00:00:00Z",
      estimateRevisionId: "rev-1",
      revisionNo: 1,
      opportunityStage: "proposed",
      opportunityRowVersion: "opp-version-456-won",
      estimateRowVersion: "est-version-123-quoted",
    });

    renderWithClient(
      <EstimateCard
        estimate={mockEstimate}
        opportunityId="opp-1"
        opportunityRowVersion="opp-version-456"
        canEdit={true}
      />
    );

    // Open confirmation modal
    const issueBtn = screen.getByRole("button", { name: "ออกใบเสนอราคา" });
    fireEvent.click(issueBtn);
    expect(screen.getByText("ยืนยันการออกใบเสนอราคา")).toBeInTheDocument();

    // Confirm Issue 1 (fails with network error)
    const confirmButtons = screen.getAllByRole("button", { name: "ออกใบเสนอราคา" });
    const modalConfirmBtn = confirmButtons[confirmButtons.length - 1];
    fireEvent.click(modalConfirmBtn);

    await waitFor(() => {
      expect(mockToast.error).toHaveBeenCalledWith("Network timeout on issuing quotation");
    });

    expect(issueSpy).toHaveBeenCalledTimes(1);
    const firstCallArgs = issueSpy.mock.calls[0];
    expect(firstCallArgs[0]).toBe("est-1234");
    expect(firstCallArgs[1]).toEqual({
      expectedEstimateVersion: "est-version-123",
      expectedOpportunityVersion: "opp-version-456",
    });
    const firstKey = firstCallArgs[2]?.idempotencyKey;
    expect(firstKey).toBeDefined();
    expect(typeof firstKey).toBe("string");

    // Modal is still open because of failure, user clicks confirm again to retry
    fireEvent.click(modalConfirmBtn);

    await waitFor(() => {
      expect(mockToast.success).toHaveBeenCalledWith("ออกใบเสนอราคาสำเร็จ");
    });

    expect(issueSpy).toHaveBeenCalledTimes(2);
    const secondCallArgs = issueSpy.mock.calls[1];
    expect(secondCallArgs[0]).toBe("est-1234");
    expect(secondCallArgs[1]).toEqual({
      expectedEstimateVersion: "est-version-123",
      expectedOpportunityVersion: "opp-version-456",
    });
    const secondKey = secondCallArgs[2]?.idempotencyKey;

    // Both expected versions sent, and stable UUID key reused across ambiguous failure
    expect(secondKey).toBe(firstKey);
  });
});
