import { describe, it, expect, vi } from "vitest";
import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { EstimateCard } from "./estimate-card";
import type { EstimateDetailResponse } from "@/lib/api/api-client";

// Mock useToast
vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({
    toast: { success: vi.fn(), error: vi.fn() },
  }),
}));

// Mock useSelectedMembership
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "mem-1",
      role: "admin",
      organizationId: "org-1",
      permissions: [{ key: "quotations.issue", scope: "organization" }],
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
  const queryClient = new QueryClient();

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
        markupRate: 0.30,
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
        markupRate: 0.40,
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
        markupRate: 0.40,
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
        canEdit={true}
      />
    );

    expect(screen.getByText("ออกใบเสนอราคาแล้ว (Quoted)")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "ออกใบเสนอราคา" })).not.toBeInTheDocument();
  });
});
