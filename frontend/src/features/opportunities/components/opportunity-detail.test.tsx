import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityDetail } from "./opportunity-detail";
import * as oppQueries from "../api/opportunity-queries";
import * as customerQueries from "@/features/customers/api/customer-queries";
import * as siteQueries from "@/features/sites/api/site-queries";
import * as membershipContext from "@/lib/membership/selected-membership-context";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

describe("OpportunityDetail Component", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    vi.clearAllMocks();
  });

  const mockMembership = {
    id: "membership-1",
    organization: { id: "org-1", name: "Org 1" },
    branch: { id: "branch-1", name: "Branch 1" },
    permissions: [{ key: "opportunities.read", scope: "organization", scopeId: "org-1" }],
  };

  const sampleOpportunity = {
    id: "30000000-0000-0000-0000-000000000001",
    code: "OPP-0001",
    customerId: "10000000-0000-0000-0000-000000000001",
    primarySiteId: "20000000-0000-0000-0000-000000000001",
    branchId: "branch-1",
    ownerUserId: "user-1",
    title: "โครงการปรับปรุงอาคารสำนักงาน",
    scopeSummary: "ปรับปรุงห้องประชุมและพื้นที่ส่วนกลาง",
    workTypes: ["built-in", "interior"],
    sourceCode: "referral",
    expectedBudget: 500000,
    currencyCode: "THB",
    targetDecisionDate: "2026-10-15",
    nextActionAtUtc: "2026-09-20T10:00:00Z",
    nextActionNote: "นัดประชุมนำเสนอแบบร่าง",
    stage: "draft",
    rowVersion: "00000000-0000-0000-0000-000000000001",
    createdAtUtc: "2026-09-08T08:00:00Z",
  };

  const sampleCustomer = {
    id: "10000000-0000-0000-0000-000000000001",
    code: "CUS-0001",
    displayNameTh: "บริษัท ลูกค้าเอ จำกัด",
    status: "active",
  };

  const sampleSite = {
    id: "20000000-0000-0000-0000-000000000001",
    customerId: "10000000-0000-0000-0000-000000000001",
    label: "สำนักงานใหญ่",
    addressLine1: "123 สุขุมวิท",
    status: "active",
  };

  it("renders loading state with minimal mono spinner", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityDetail opportunityId={sampleOpportunity.id} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getAllByRole("status").length).toBeGreaterThanOrEqual(1);
  });

  it("renders opportunity details and localized Draft stage badge", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    vi.spyOn(customerQueries, "useCustomerDetail").mockReturnValue({
      data: sampleCustomer,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityDetail opportunityId={sampleOpportunity.id} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("โครงการปรับปรุงอาคารสำนักงาน")).toBeDefined();
    expect(screen.getByText("OPP-0001")).toBeDefined();
    expect(screen.getByText("ฉบับร่าง (Draft)")).toBeDefined();
    expect(screen.getByText("งานบิวท์อิน (Built-in)")).toBeDefined();
    expect(screen.getByText("งานอินทีเรีย (Interior)")).toBeDefined();
    expect(screen.getByText("500,000 THB")).toBeDefined();
  });

  it("renders customer and site links safely when loaded", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityDetail").mockReturnValue({
      data: sampleOpportunity,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    vi.spyOn(customerQueries, "useCustomerDetail").mockReturnValue({
      data: sampleCustomer,
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
      data: { items: [sampleSite] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as any);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityDetail opportunityId={sampleOpportunity.id} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    const customerLink = screen.getByRole("link", { name: /บริษัท ลูกค้าเอ จำกัด/i });
    expect(customerLink).toBeDefined();
    expect(customerLink.getAttribute("href")).toContain(`/customers/${sampleCustomer.id}`);
  });
});
