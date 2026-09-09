import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SiteList } from "./site-list";
import type { MembershipDto } from "@/lib/permissions/can";
import type { useCustomerSiteList } from "../api/site-queries";

let mockMembership: MembershipDto | null = {
  id: "membership-a",
  permissions: [
    { key: "sites.read", scope: "organization" },
    { key: "sites.manage", scope: "organization" },
  ],
};

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: mockMembership,
  }),
}));

let mockQueryResult: ReturnType<typeof useCustomerSiteList> = {
  data: {
    items: [
      {
        id: "site-1",
        organizationId: "org-1",
        customerId: "customer-1",
        label: "สำนักงานใหญ่",
        normalizedLabel: "สำนักงานใหญ่",
        addressLine1: "123 สุขุมวิท",
        addressLine2: null,
        subdistrict: "คลองเตย",
        district: "คลองเตย",
        province: "กรุงเทพมหานคร",
        postalCode: "10110",
        countryCode: "TH",
        latitude: null,
        longitude: null,
        accessNote: null,
        status: "active",
        createdAtUtc: "2026-09-09T10:00:00Z",
      },
    ],
  },
  isLoading: false,
  isError: false,
  error: null,
  refetch: vi.fn(),
} as unknown as ReturnType<typeof useCustomerSiteList>;

vi.mock("../api/site-queries", () => ({
  useCustomerSiteList: () => mockQueryResult,
}));

function renderSiteList(client: QueryClient, isCustomerActive: boolean) {
  return render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <SiteList customerId="customer-1" isCustomerActive={isCustomerActive} />
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

describe("SiteList component", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    mockMembership = {
      id: "membership-a",
      permissions: [
        { key: "sites.read", scope: "organization" },
        { key: "sites.manage", scope: "organization" },
      ],
    };
    mockQueryResult = {
      data: {
        items: [
          {
            id: "site-1",
            organizationId: "org-1",
            customerId: "customer-1",
            label: "สำนักงานใหญ่",
            normalizedLabel: "สำนักงานใหญ่",
            addressLine1: "123 สุขุมวิท",
            addressLine2: null,
            subdistrict: "คลองเตย",
            district: "คลองเตย",
            province: "กรุงเทพมหานคร",
            postalCode: "10110",
            countryCode: "TH",
            latitude: null,
            longitude: null,
            accessNote: null,
            status: "active",
            createdAtUtc: "2026-09-09T10:00:00Z",
          },
        ],
      },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useCustomerSiteList>;
  });

  it("does not render when user lacks sites.read permission", () => {
    mockMembership = {
      id: "membership-a",
      permissions: [{ key: "sites.manage", scope: "organization" }],
    };

    const { container } = renderSiteList(client, true);
    expect(container).toBeEmptyDOMElement();
  });

  it("renders site rows when sites.read is present", () => {
    renderSiteList(client, false);

    expect(screen.getByText("สถานที่ตั้ง (Sites)")).toBeInTheDocument();
    expect(screen.getByText("สำนักงานใหญ่")).toBeInTheDocument();
    expect(screen.getByText("123 สุขุมวิท")).toBeInTheDocument();
  });

  it("renders create button only when customer is active AND user has sites.manage", () => {
    // When customer is draft (not active), no create site button
    const { rerender } = renderSiteList(client, false);
    expect(screen.queryByRole("link", { name: /เพิ่มสถานที่ตั้ง/ })).not.toBeInTheDocument();

    // When customer is active and has sites.manage, show button
    rerender(
      <QueryClientProvider client={client}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SiteList customerId="customer-1" isCustomerActive={true} />
        </NextIntlClientProvider>
      </QueryClientProvider>
    );
    expect(screen.getByRole("link", { name: /เพิ่มสถานที่ตั้ง/ })).toBeInTheDocument();
  });

  it("renders empty state when items array is empty", () => {
    mockQueryResult = {
      data: { items: [] },
      isLoading: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useCustomerSiteList>;

    renderSiteList(client, true);
    expect(screen.getByText("ยังไม่มีสถานที่ตั้ง")).toBeInTheDocument();
  });

  it("renders minimal mono loading spinner when isLoading is true", () => {
    mockQueryResult = {
      data: undefined,
      isLoading: true,
      isError: false,
      error: null,
      refetch: vi.fn(),
    } as unknown as ReturnType<typeof useCustomerSiteList>;

    renderSiteList(client, true);
    expect(screen.getByText("กำลังโหลดข้อมูล...")).toBeInTheDocument();
  });
});
