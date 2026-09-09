import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { OpportunityList } from "./opportunity-list";
import * as oppQueries from "../api/opportunity-queries";
import * as membershipContext from "@/lib/membership/selected-membership-context";
import type { CurrentUserResponse } from "@/lib/api/api-client";

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  usePathname: () => "/th/opportunities",
  useSearchParams: () => new URLSearchParams(),
}));

describe("OpportunityList Component", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    vi.clearAllMocks();
  });

  const mockMembership = {
    id: "membership-1",
    organization: { id: "org-1", name: "Org 1" },
    branch: { id: "branch-1", name: "Branch 1" },
    permissions: [
      { key: "opportunities.read", scope: "organization", scopeId: "org-1" },
      { key: "opportunities.create", scope: "organization", scopeId: "org-1" },
    ],
  };

  const sampleOpportunity = {
    id: "00000000-0000-0000-0000-000000000001",
    code: "OPP-0001",
    customerId: "10000000-0000-0000-0000-000000000001",
    primarySiteId: null,
    branchId: "branch-1",
    ownerUserId: "user-1",
    title: "โครงการปรับปรุงสำนักงาน",
    scopeSummary: "รายละเอียดงานตกแต่ง",
    workTypes: ["built-in", "interior"],
    sourceCode: "referral",
    expectedBudget: 350000,
    currencyCode: "THB",
    targetDecisionDate: "2026-10-31",
    nextActionAtUtc: "2026-09-30T10:00:00Z",
    nextActionNote: "ติดตามแบบร่าง",
    stage: "draft",
    rowVersion: "version-1",
    createdAtUtc: "2026-09-08T08:00:00Z",
  };

  const mockCurrentUser = {
    user: { id: "user-1", displayName: "Test User", email: "test@example.test" },
    memberships: [mockMembership],
  } as unknown as CurrentUserResponse;

  it("renders loading state with minimal mono spinner", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getAllByRole("status").length).toBeGreaterThanOrEqual(1);
  });

  it("renders empty state when no opportunities found", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: { pages: [{ items: [], nextCursor: null }], pageParams: [undefined] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("ยังไม่มีโอกาสทางการขาย")).toBeDefined();
  });

  it("renders opportunity items with Draft stage label in Thai", () => {
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: { pages: [{ items: [sampleOpportunity], nextCursor: null }], pageParams: [undefined] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByText("โครงการปรับปรุงสำนักงาน")).toBeDefined();
    expect(screen.getByText("OPP-0001")).toBeDefined();
    expect(screen.getAllByText("ฉบับร่าง (Draft)").length).toBeGreaterThanOrEqual(1);
  });

  it("shows create opportunity button only when opportunities.create permission is present", () => {
    // With permission
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: { pages: [{ items: [sampleOpportunity], nextCursor: null }], pageParams: [undefined] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage: vi.fn(),
      hasNextPage: false,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    const { unmount } = render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.getByRole("link", { name: /สร้างโอกาสทางการขาย/i })).toBeDefined();
    unmount();

    // Without opportunities.create permission
    const readOnlyMembership = {
      ...mockMembership,
      permissions: [{ key: "opportunities.read", scope: "organization", scopeId: "org-1" }],
    };
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: readOnlyMembership,
      memberships: [readOnlyMembership],
      setSelectedMembershipId: vi.fn(),
    });

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    expect(screen.queryByRole("link", { name: /สร้างโอกาสทางการขาย/i })).toBeNull();
  });

  it("handles keyset Load More pagination", () => {
    const fetchNextPage = vi.fn();
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      currentUser: mockCurrentUser,
      selectedMembership: mockMembership,
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    vi.spyOn(oppQueries, "useOpportunityList").mockReturnValue({
      data: { pages: [{ items: [sampleOpportunity], nextCursor: "cursor-123" }], pageParams: [undefined] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
      fetchNextPage,
      hasNextPage: true,
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof oppQueries.useOpportunityList>);

    render(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <OpportunityList />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );

    const loadMoreButton = screen.getByRole("button", { name: /โหลดเพิ่มเติม/i });
    expect(loadMoreButton).toBeDefined();
    fireEvent.click(loadMoreButton);
    expect(fetchNextPage).toHaveBeenCalledTimes(1);
  });
});
