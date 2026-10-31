import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { DashboardPage } from "./dashboard-page";
import * as reportingQueries from "../api/reporting-queries";
import { PERMISSIONS } from "@/lib/permissions/permissions";

const grantedPermissions = vi.hoisted(() => ({ list: [] as string[] }));
const membershipMock = vi.hoisted(() => ({
  selectedMembership: {
    id: "mem-test-1",
    branch: null,
  } as unknown,
}));
const exportAllMock = vi.hoisted(() => vi.fn());

vi.mock("next/navigation", () => ({
  usePathname: () => "/th/reports",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => membershipMock,
}));

vi.mock("@/lib/permissions/can", () => ({
  can: (_m: unknown, permission: string) => grantedPermissions.list.includes(permission),
}));

vi.mock("@/hooks/useCsvExport", () => ({
  useCsvExport: () => ({
    isExporting: false,
    exportAll: exportAllMock,
    exportSelected: vi.fn(),
  }),
}));

const ALL_REPORT_PERMISSIONS = [
  PERMISSIONS.OPPORTUNITIES_READ,
  PERMISSIONS.QUOTATIONS_READ,
  PERMISSIONS.PROJECTS_READ,
  PERMISSIONS.INVENTORY_READ,
  PERMISSIONS.BILLINGS_READ,
  PERMISSIONS.SERVICE_REQUESTS_READ,
];

const mockDashboardSummary = {
  generatedAtUtc: "2026-10-31T08:00:00Z",
  effectiveBranchId: null,
  pipeline: {
    permitted: true,
    summary: {
      totalCount: 10,
      totalExpectedBudget: 1500000,
      openCount: 7,
      openExpectedBudget: 1100000,
      wonCount: 3,
      wonBudget: 400000,
      lostCount: 0,
      winRatePercent: 30,
    },
  },
  quotations: {
    permitted: true,
    summary: {
      totalCount: 8,
      issuedCount: 5,
      issuedAmount: 850000,
      acceptedCount: 3,
      acceptedAmount: 520000,
      acceptanceRatePercent: 37.5,
    },
  },
  projectsBudget: {
    permitted: true,
    summary: {
      projectCount: 4,
      overBudgetCount: 1,
      totalCurrentBudget: 2000000,
      totalCommittedAmount: 500000,
      totalActualAmount: 1200000,
      totalVarianceAmount: 300000,
    },
  },
  stockValue: {
    permitted: true,
    summary: {
      warehouseCount: 2,
      skuCount: 45,
      totalOnHand: 1200,
      totalValue: 680000,
    },
  },
  arAging: {
    permitted: true,
    summary: {
      asOfDate: "2026-10-31",
      invoiceCount: 12,
      current: 200000,
      bucket1To30: 50000,
      bucket31To60: 30000,
      bucket61To90: 20000,
      bucketOver90: 10000,
      totalOutstanding: 310000,
    },
  },
  serviceSla: {
    permitted: true,
    summary: {
      totalCount: 15,
      openBacklogCount: 5,
      waitingCustomerCount: 1,
      slaWarningCount: 1,
      slaBreachedCount: 1,
      outOfWarrantyChargedTotal: 15000,
    },
  },
};

function setupQueries() {
  vi.spyOn(reportingQueries, "useReportingBranchesQuery").mockReturnValue({
    data: [{ id: "b-1", code: "HQ", name: "สำนักงานใหญ่" }],
    isLoading: false,
    isFetching: false,
    isError: false,
  } as unknown as ReturnType<typeof reportingQueries.useReportingBranchesQuery>);

  vi.spyOn(reportingQueries, "useDashboardSummaryQuery").mockReturnValue({
    data: mockDashboardSummary,
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useDashboardSummaryQuery>);

  vi.spyOn(reportingQueries, "usePipelineReportQuery").mockReturnValue({
    data: {
      stages: [{ stage: "won", opportunityCount: 3, expectedBudgetTotal: 400000 }],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.usePipelineReportQuery>);

  vi.spyOn(reportingQueries, "useQuotationReportQuery").mockReturnValue({
    data: {
      rows: [
        {
          quotationId: "q-1",
          number: "QT-2026-0001",
          customerCode: "CUS-01",
          customerName: "คุณสมชาย",
          branchCode: "HQ",
          status: "accepted",
          totalAmount: 520000,
          issuedAtUtc: "2026-10-15T00:00:00Z",
        },
      ],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useQuotationReportQuery>);

  vi.spyOn(reportingQueries, "useProjectBudgetReportQuery").mockReturnValue({
    data: {
      rows: [
        {
          projectId: "p-1",
          projectCode: "PRJ-01",
          projectName: "บ้านพักตากอากาศ",
          branchCode: "HQ",
          status: "in_progress",
          currentBudget: 2000000,
          actualAmount: 1200000,
          varianceAmount: 300000,
          isOverBudget: false,
        },
      ],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useProjectBudgetReportQuery>);

  vi.spyOn(reportingQueries, "useStockValueReportQuery").mockReturnValue({
    data: {
      rows: [
        {
          warehouseId: "w-1",
          warehouseCode: "WH-01",
          warehouseName: "คลังหลัก",
          branchCode: "HQ",
          itemId: "it-1",
          itemCode: "IT-001",
          itemNameTh: "บานพับสแตนเลส",
          onHand: 120,
          totalValue: 24000,
        },
      ],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useStockValueReportQuery>);

  vi.spyOn(reportingQueries, "useReportingArAgingQuery").mockReturnValue({
    data: {
      rows: [
        {
          billingDocumentId: "b-1",
          number: "BIL-2026-0001",
          projectCode: "PRJ-01",
          customerCode: "CUS-01",
          customerName: "คุณสมชาย",
          branchCode: "HQ",
          dueDate: "2026-10-20",
          daysOverdue: 11,
          agingBucket: "1_30",
          outstandingAmount: 50000,
        },
      ],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useReportingArAgingQuery>);

  vi.spyOn(reportingQueries, "useServiceSlaReportQuery").mockReturnValue({
    data: {
      rows: [
        {
          serviceRequestId: "s-1",
          number: "SRV-2026-0001",
          title: "ซ่อมบานพับ",
          projectCode: "PRJ-01",
          branchCode: "HQ",
          priority: "high",
          status: "in_progress",
          inWarranty: true,
          slaStatus: "on_track",
          elapsedHours: 4.5,
          resolveTargetHours: 24,
        },
      ],
    },
    isLoading: false,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
  } as unknown as ReturnType<typeof reportingQueries.useServiceSlaReportQuery>);
}

describe("DashboardPage Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setupQueries();
  });

  it("Test 1: Full-permission user sees all 6 KPI cards with expected formatted numbers & switches tabs across the 6 reports", () => {
    grantedPermissions.list = ALL_REPORT_PERMISSIONS;

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <DashboardPage />
      </NextIntlClientProvider>
    );

    // Verify 6 KPI cards
    expect(screen.getByText(thMessages.reporting.kpi.pipelineTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.quotationsTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.projectsBudgetTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.stockValueTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.arAgingTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.serviceSlaTitle)).toBeDefined();

    // Verify formatted values in cards
    expect(screen.getByText("30.0%")).toBeDefined();
    expect(screen.getByText("37.5%")).toBeDefined();

    // Default tab is pipeline
    expect(screen.getByText(thMessages.reporting.stages.won)).toBeDefined();

    // Switch to Quotations Tab
    fireEvent.click(screen.getByRole("button", { name: thMessages.reporting.tabs.quotations }));
    expect(screen.getByText("QT-2026-0001")).toBeDefined();
    expect(screen.getByText("คุณสมชาย")).toBeDefined();

    // Switch to Project Budget Tab
    fireEvent.click(screen.getByRole("button", { name: thMessages.reporting.tabs.projectsBudget }));
    expect(screen.getByText("PRJ-01")).toBeDefined();
    expect(screen.getByText("บ้านพักตากอากาศ")).toBeDefined();

    // Switch to Stock Value Tab
    fireEvent.click(screen.getByRole("button", { name: thMessages.reporting.tabs.stockValue }));
    expect(screen.getByText("WH-01")).toBeDefined();
    expect(screen.getByText("บานพับสแตนเลส")).toBeDefined();

    // Switch to AR Aging Tab
    fireEvent.click(screen.getByRole("button", { name: thMessages.reporting.tabs.arAging }));
    expect(screen.getByText("BIL-2026-0001")).toBeDefined();
    expect(screen.getByText("1-30 วัน")).toBeDefined();

    // Switch to Service SLA Tab
    fireEvent.click(screen.getByRole("button", { name: thMessages.reporting.tabs.serviceSla }));
    expect(screen.getByText("SRV-2026-0001")).toBeDefined();
    expect(screen.getByText("ซ่อมบานพับ")).toBeDefined();
  });

  it("Test 2: Partial-permission user sees ONLY Pipeline & Quotations KPI cards and tabs", () => {
    grantedPermissions.list = [PERMISSIONS.OPPORTUNITIES_READ, PERMISSIONS.QUOTATIONS_READ];

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <DashboardPage />
      </NextIntlClientProvider>
    );

    // Permitted cards
    expect(screen.getByText(thMessages.reporting.kpi.pipelineTitle)).toBeDefined();
    expect(screen.getByText(thMessages.reporting.kpi.quotationsTitle)).toBeDefined();

    // Restricted cards must NOT be rendered
    expect(screen.queryByText(thMessages.reporting.kpi.projectsBudgetTitle)).toBeNull();
    expect(screen.queryByText(thMessages.reporting.kpi.stockValueTitle)).toBeNull();
    expect(screen.queryByText(thMessages.reporting.kpi.arAgingTitle)).toBeNull();
    expect(screen.queryByText(thMessages.reporting.kpi.serviceSlaTitle)).toBeNull();

    // Permitted tabs
    expect(screen.getByRole("button", { name: thMessages.reporting.tabs.pipeline })).toBeDefined();
    expect(screen.getByRole("button", { name: thMessages.reporting.tabs.quotations })).toBeDefined();

    // Restricted tabs must NOT exist
    expect(screen.queryByRole("button", { name: thMessages.reporting.tabs.projectsBudget })).toBeNull();
    expect(screen.queryByRole("button", { name: thMessages.reporting.tabs.stockValue })).toBeNull();
    expect(screen.queryByRole("button", { name: thMessages.reporting.tabs.arAging })).toBeNull();
    expect(screen.queryByRole("button", { name: thMessages.reporting.tabs.serviceSla })).toBeNull();
  });

  it("Test 3: Zero-permission user sees localized noPermittedReports state", () => {
    grantedPermissions.list = [];

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <DashboardPage />
      </NextIntlClientProvider>
    );

    expect(screen.getByText(thMessages.reporting.noPermittedReports)).toBeDefined();
    expect(screen.queryByText(thMessages.reporting.kpi.pipelineTitle)).toBeNull();
  });

  it("Test 4: Clicking the CSV Export button invokes exportAll with report rows", () => {
    grantedPermissions.list = ALL_REPORT_PERMISSIONS;

    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <DashboardPage />
      </NextIntlClientProvider>
    );

    const exportBtn = screen.getByRole("button", { name: thMessages.reporting.exportCsv });
    fireEvent.click(exportBtn);

    expect(exportAllMock).toHaveBeenCalledTimes(1);
  });
});
